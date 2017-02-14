using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using IdentityServer.Web.Configuration.AspNetIdentity;
using IdentityServer.Web.Configuration.Helper;
using IdentityServer3.Core.Extensions;
using IdentityServer3.Core.Models;
using IdentityServer3.Core.Services.Default;
using Microsoft.AspNet.Identity;

namespace IdentityServer.Web.Configuration.Admin.UserService
{
    public class AspNetIdentityUserService<TUser, TKey> : UserServiceBase
        where TUser : class, IUser<TKey>, new()
        where TKey : IEquatable<TKey>
    {
        /// <summary>
        ///     The convert subject to key
        /// </summary>
        protected readonly Func<string, TKey> ConvertSubjectToKey;

        /// <summary>
        ///     The user manager
        /// </summary>
        protected readonly UserManager<TUser, TKey> UserManager;

        /// <summary>
        ///     Initializes a new instance of the <see cref="AspNetIdentityUserService{TUser, TKey}" /> class.
        /// </summary>
        /// <param name="userManager">The user manager.</param>
        /// <param name="parseSubject">The parse subject.</param>
        /// <exception cref="System.ArgumentNullException">userManager</exception>
        /// <exception cref="System.InvalidOperationException">Key type not supported</exception>
        public AspNetIdentityUserService(UserManager<TUser, TKey> userManager, Func<string, TKey> parseSubject = null)
        {
            if (userManager == null) throw new ArgumentNullException(nameof(userManager));

            UserManager = userManager;

            if (parseSubject != null)
            {
                ConvertSubjectToKey = parseSubject;
            }
            else
            {
                var keyType = typeof(TKey);
                if (keyType == typeof(string)) ConvertSubjectToKey = subject => (TKey)ParseString(subject);
                else if (keyType == typeof(int)) ConvertSubjectToKey = subject => (TKey)ParseInt(subject);
                else if (keyType == typeof(uint)) ConvertSubjectToKey = subject => (TKey)ParseUInt32(subject);
                else if (keyType == typeof(long)) ConvertSubjectToKey = subject => (TKey)ParseLong(subject);
                else if (keyType == typeof(Guid)) ConvertSubjectToKey = subject => (TKey)ParseGuid(subject);
                else
                {
                    throw new InvalidOperationException("Key type not supported");
                }
            }

            EnableSecurityStamp = true;
        }

        /// <summary>
        ///     Gets or sets the display type of the name claim.
        /// </summary>
        /// <value>
        ///     The display type of the name claim.
        /// </value>
        public string DisplayNameClaimType { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether [enable security stamp].
        /// </summary>
        /// <value>
        ///     <c>true</c> if [enable security stamp]; otherwise, <c>false</c>.
        /// </value>
        public bool EnableSecurityStamp { get; set; }

        /// <summary>
        ///     Parses the string.
        /// </summary>
        /// <param name="sub">The sub.</param>
        /// <returns></returns>
        private object ParseString(string sub)
        {
            return sub;
        }

        /// <summary>
        ///     Parses the int.
        /// </summary>
        /// <param name="sub">The sub.</param>
        /// <returns></returns>
        private object ParseInt(string sub)
        {
            int key;
            if (!int.TryParse(sub, out key)) return 0;
            return key;
        }

        /// <summary>
        ///     Parses the u int32.
        /// </summary>
        /// <param name="sub">The sub.</param>
        /// <returns></returns>
        private object ParseUInt32(string sub)
        {
            uint key;
            if (!uint.TryParse(sub, out key)) return 0;
            return key;
        }

        /// <summary>
        ///     Parses the long.
        /// </summary>
        /// <param name="sub">The sub.</param>
        /// <returns></returns>
        private object ParseLong(string sub)
        {
            long key;
            if (!long.TryParse(sub, out key)) return 0;
            return key;
        }

        /// <summary>
        /// Parses the unique identifier.
        /// </summary>
        /// <param name="sub">The sub.</param>
        /// <returns></returns>
        private object ParseGuid(string sub)
        {
            Guid key;
            if (!Guid.TryParse(sub, out key)) return Guid.Empty;
            return key;
        }

        /// <summary>
        ///     Gets the profile data asynchronous.
        /// </summary>
        /// <param name="ctx">The CTX.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">subject</exception>
        /// <exception cref="System.ArgumentException">Invalid subject identifier</exception>
        public override async Task GetProfileDataAsync(ProfileDataRequestContext ctx)
        {
            var subject = ctx.Subject;
            var requestedClaimTypes = ctx.RequestedClaimTypes;

            if (subject == null) throw new ArgumentNullException(nameof(ctx.Subject));

            var key = ConvertSubjectToKey(subject.GetSubjectId());
            var acct = await UserManager.FindByIdAsync(key);
            if (acct == null)
            {
                throw new ArgumentException("Invalid subject identifier");
            }

            var claims = await GetClaimsFromAccount(acct);
            var claimTypes = requestedClaimTypes as string[] ?? requestedClaimTypes.ToArray();

            if (requestedClaimTypes != null && claimTypes.Any())
            {
                claims = claims.Where(x => claimTypes.Contains(x.Type));
            }

            ctx.IssuedClaims = claims;
        }

        protected virtual async Task<IEnumerable<Claim>> GetClaimsFromAccount(TUser user)
        {
            var myUser = user as User;

            if (myUser != null)
            {
                //ALERT: ensure you add the claim in lower case.
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypeConstants.Id, myUser.Id),
                    new Claim(ClaimTypeConstants.Subject, myUser.Id),
                    new Claim(ClaimTypeConstants.FirstName, myUser.FirstName??string.Empty),
                    new Claim(ClaimTypeConstants.MiddleName, myUser.MiddleName??string.Empty),
                    new Claim(ClaimTypeConstants.LastName, myUser.FamilyName??string.Empty),
                };

                // Get all the custom claims
                var customClaims = await UserManager.GetClaimsAsync(user.Id);

                if (customClaims.Any())
                {
                    // add all the claims to the user.
                    claims.AddRange(
                        customClaims.Select(customClaim => new Claim(customClaim.Type.ToLower(), customClaim.Value)));
                }

                if (UserManager.SupportsUserEmail)
                {
                    var email = await UserManager.GetEmailAsync(user.Id);
                    if (!string.IsNullOrWhiteSpace(email))
                    {
                        claims.Add(new Claim(global::IdentityServer3.Core.Constants.ClaimTypes.Email, email));
                        var verified = await UserManager.IsEmailConfirmedAsync(user.Id);
                        claims.Add(new Claim(global::IdentityServer3.Core.Constants.ClaimTypes.EmailVerified,
                            verified ? "true" : "false"));
                    }
                }

                if (UserManager.SupportsUserPhoneNumber)
                {
                    var phone = await UserManager.GetPhoneNumberAsync(user.Id);
                    if (!string.IsNullOrWhiteSpace(phone))
                    {
                        claims.Add(new Claim(global::IdentityServer3.Core.Constants.ClaimTypes.PhoneNumber, phone));
                        var verified = await UserManager.IsPhoneNumberConfirmedAsync(user.Id);
                        claims.Add(new Claim(global::IdentityServer3.Core.Constants.ClaimTypes.PhoneNumberVerified,
                            verified ? "true" : "false"));
                    }
                }

                if (UserManager.SupportsUserClaim)
                {
                    claims.AddRange(await UserManager.GetClaimsAsync(user.Id));
                }

                if (UserManager.SupportsUserRole)
                {
                    var roleClaims =
                        from role in await UserManager.GetRolesAsync(user.Id)
                        select new Claim(global::IdentityServer3.Core.Constants.ClaimTypes.Role, role);
                    claims.AddRange(roleClaims);
                }

                return claims;
            }

            return null;
        }

    }
}