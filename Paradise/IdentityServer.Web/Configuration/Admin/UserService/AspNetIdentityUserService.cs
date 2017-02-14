using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using IdentityModel;
using IdentityServer.Web.Configuration.AspNetIdentity;
using IdentityServer.Web.Configuration.Helper;
using IdentityServer.Web.Configuration.InMemory;
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
        protected readonly Func<string, TKey> ConvertSubjectToKey;

        protected readonly UserManager<TUser, TKey> UserManager;

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

        public string DisplayNameClaimType { get; set; }

        public bool EnableSecurityStamp { get; set; }

        private object ParseString(string sub)
        {
            return sub;
        }

        private object ParseInt(string sub)
        {
            int key;
            if (!int.TryParse(sub, out key)) return 0;
            return key;
        }

        private object ParseUInt32(string sub)
        {
            uint key;
            if (!uint.TryParse(sub, out key)) return 0;
            return key;
        }

        private object ParseLong(string sub)
        {
            long key;
            if (!long.TryParse(sub, out key)) return 0;
            return key;
        }

        private object ParseGuid(string sub)
        {
            Guid key;
            if (!Guid.TryParse(sub, out key)) return Guid.Empty;
            return key;
        }

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

            //Logger.Info("ProfileDataRequestContext : " + JsonSerializer.Serialize(ctx.Client), GetType().FullName + "/" + MethodBase.GetCurrentMethod().Name);

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
                    new Claim(ClaimTypeConstants.UserId, myUser.UserId.ToString()),
                    new Claim(ClaimTypeConstants.LoginProvider, myUser.LoginProvider??string.Empty),
                    new Claim(ClaimTypeConstants.PhoneNo, myUser.PhoneNumber??string.Empty)
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

        protected virtual async Task<string> GetDisplayNameForAccountAsync(TKey userId)
        {
            var user = await UserManager.FindByIdAsync(userId);
            var claims = await GetClaimsFromAccount(user);

            Claim nameClaim = null;
            var enumerable = claims as Claim[] ?? claims.ToArray();

            if (DisplayNameClaimType != null)
            {
                nameClaim = enumerable.FirstOrDefault(x => x.Type == DisplayNameClaimType);
            }
            if (nameClaim == null)
                nameClaim = enumerable.FirstOrDefault(x => x.Type == global::IdentityServer3.Core.Constants.ClaimTypes.Name);
            if (nameClaim == null) nameClaim = enumerable.FirstOrDefault(x => x.Type == ClaimTypes.Name);
            if (nameClaim != null) return nameClaim.Value;

            return user.UserName;
        }

        protected virtual async Task<TUser> FindUserAsync(string username)
        {
            return await UserManager.FindByNameAsync(username);
        }

        protected virtual Task<TUser> FindByEmailAsync(string emailId)
        {
            return UserManager.FindByEmailAsync(emailId);
        }

        protected virtual Task<AuthenticateResult> PostAuthenticateLocalAsync(TUser user, SignInMessage message)
        {
            return Task.FromResult<AuthenticateResult>(null);
        }

        public override async Task AuthenticateLocalAsync(LocalAuthenticationContext context)
        {
            var emailId = context.UserName;
            var password = context.Password;
            var message = context.SignInMessage;

            context.AuthenticateResult = null;

            if (UserManager.SupportsUserPassword)
            {
                var user = await FindByEmailAsync(emailId);
                if (user != null)
                {
                    if (UserManager.SupportsUserLockout &&
                        await UserManager.IsLockedOutAsync(user.Id))
                    {
                        return;
                    }

                    if (await UserManager.CheckPasswordAsync(user, password))
                    {
                        if (UserManager.SupportsUserLockout)
                        {
                            await UserManager.ResetAccessFailedCountAsync(user.Id);
                        }

                        var result = await PostAuthenticateLocalAsync(user, message);
                        if (result == null)
                        {
                            var claims = await GetClaimsForAuthenticateResult(user);
                            var enumerable = claims as IList<Claim> ?? claims.ToList();

                            result = new AuthenticateResult(user.Id.ToString(),
                                await GetDisplayNameForAccountAsync(user.Id), enumerable);
                        }
                        context.AuthenticateResult = result;
                    }
                    else if (UserManager.SupportsUserLockout)
                    {
                        await UserManager.AccessFailedAsync(user.Id);
                    }
                }
            }
        }

        protected virtual async Task<IEnumerable<Claim>> GetClaimsForAuthenticateResult(TUser user)
        {
            var claims = new List<Claim>();
            if (EnableSecurityStamp && UserManager.SupportsUserSecurityStamp)
            {
                var stamp = await UserManager.GetSecurityStampAsync(user.Id);
                if (!string.IsNullOrWhiteSpace(stamp))
                {
                    claims.Add(new Claim("security_stamp", stamp));
                }
            }
            return claims;
        }

        public override async Task AuthenticateExternalAsync(ExternalAuthenticationContext ctx)
        {
            var externalUser = ctx.ExternalIdentity;

            if (externalUser == null)
            {
                throw new ArgumentNullException(nameof(ctx.ExternalIdentity));
            }

            var user = await UserManager.FindAsync(new UserLoginInfo(externalUser.Provider, externalUser.ProviderId));
            if (user == null)
            {
                ctx.AuthenticateResult =
                    await
                        ProcessNewExternalAccountAsync(externalUser.Provider, externalUser.ProviderId,
                            externalUser.Claims);
            }
            else
            {
                ctx.AuthenticateResult =
                    await
                        ProcessExistingExternalAccountAsync(user.Id, externalUser.Provider, externalUser.ProviderId,
                            externalUser.Claims);
            }
        }

        protected virtual async Task<AuthenticateResult> ProcessNewExternalAccountAsync(string provider,
            string providerId, IEnumerable<Claim> claims)
        {
            var claimList = claims as IList<Claim> ?? claims.ToList();
            var enumerable = claims as Claim[] ?? claimList.ToArray();

            //If account is created without emailId like mobile no. then inform user to use login with emailId.
            var emailId = claimList.FirstOrDefault(x => x.Type == "email")?.Value;
            if (string.IsNullOrWhiteSpace(emailId))
            {
                string username = provider.StartsWith("google", StringComparison.InvariantCultureIgnoreCase)
                    ? claimList.Where(x => x.Type == "given_name").Select(x => x.Value).SingleOrDefault()
                    : claimList.Where(x => x.Type == "first_name").Select(x => x.Value).SingleOrDefault();
                var exceptionMessage = string.Format(MyMessages.EmailIdNotProvidedError, provider)
                    + $" User: {username}";

                //Logger.Error(new Exception(exceptionMessage), GetType().FullName + "/" + MethodBase.GetCurrentMethod().Name);

                return new AuthenticateResult(string.Format(MyMessages.EmailIdNotProvidedError, provider));
            }

            //If email id is already registered. Don't create a new account
            var userExist = UserManager.FindByEmail(claimList.FirstOrDefault(x => x.Type == "email")?.Value);
            if (userExist != null)
            {
                var userId = userExist.Id;

                var isUserEmailConfirmed = await UserManager.IsEmailConfirmedAsync(userExist.Id);

                if (!isUserEmailConfirmed)
                {
                    var token = await UserManager.GenerateEmailConfirmationTokenAsync(userId);
                    await UserManager.ConfirmEmailAsync(userId, token);

                }

                return await SignInFromExternalProviderAsync(userExist.Id, provider);
            }


            var user = await TryGetExistingUserFromExternalProviderClaimsAsync(provider, enumerable);
            if (user == null)
            {
                user = await InstantiateNewUserFromExternalProviderAsync(provider, providerId, enumerable);

                if (user == null)
                {
                    var exceptionMessage = "CreateNewAccountFromExternalProvider returned null";
                    //Logger.Error(new Exception(exceptionMessage), GetType().FullName + "/" + MethodBase.GetCurrentMethod().Name);

                    throw new InvalidOperationException(exceptionMessage);
                }

                var myUser = user as User;
                if (myUser != null)
                {
                    switch (provider.ToLower())
                    {
                        case "google":
                            // Set the User claim to User Object.
                            myUser.FirstName =
                                claimList.Where(x => x.Type == "given_name").Select(x => x.Value).SingleOrDefault();
                            myUser.FamilyName =
                                claimList.Where(x => x.Type == "family_name").Select(x => x.Value).SingleOrDefault();
                            myUser.Email =
                                claimList.Where(x => x.Type == "email").Select(x => x.Value).SingleOrDefault();
                            break;
                        case "facebook":
                            myUser.FirstName =
                                claimList.Where(x => x.Type == "first_name").Select(x => x.Value).SingleOrDefault();
                            myUser.FamilyName =
                              claimList.Where(x => x.Type == "last_name").Select(x => x.Value).SingleOrDefault();
                            myUser.Email =
                                claimList.Where(x => x.Type == "email").Select(x => x.Value).FirstOrDefault();
                            break;
                        case "microsoft":
                            myUser.FirstName =
                                claimList.Where(x => x.Type == "first_name").Select(x => x.Value).SingleOrDefault();
                            myUser.FamilyName =
                                claimList.Where(x => x.Type == "last_name").Select(x => x.Value).SingleOrDefault();
                            myUser.Email =
                                claimList.Where(x => x.Type == "email").Select(x => x.Value).SingleOrDefault();
                            break;
                    }

                    myUser.LoginProvider = provider;
                    if (myUser.FirstName != null)
                    {
                        myUser.UserName = myUser.FirstName.Substring(0, 4) + myUser.Id.Replace("-", "");
                    }

                    //myUser.PhoneNumber = Shared.Framework.Common.Constants.Constants.DefaultPhoneNumber;
                    myUser.EmailConfirmed = true;
                    myUser.PhoneNumberConfirmed = false;
                    myUser.TwoFactorEnabled = false;
                    myUser.LockoutEnabled = false;
                    myUser.AccessFailedCount = 0;
                }

                try
                {
                    var createResult = await UserManager.CreateAsync(user);
                    if (!createResult.Succeeded)
                    {
                        var exceptionMessage =
                            createResult.Errors.Aggregate(
                                "User Creation Failed - Identity Exception. Errors were: \n\r\n\r",
                                (current, error) => current + " - " + error + "\n\r");

                        //Logger.Error(new Exception(exceptionMessage), GetType().FullName + "/" + MethodBase.GetCurrentMethod().Name);

                        return new AuthenticateResult(exceptionMessage);
                    }
                    //Add claims
                    await UserManager.AddClaimAsync(user.Id, new Claim("usertype", "Customer"));
                }
                catch (Exception ex)
                {
                    //Logger.Error(ex, GetType().FullName + "/" + MethodBase.GetCurrentMethod().Name);
                }
            }

            var externalLogin = new UserLoginInfo(provider, providerId);
            var addExternalResult = await UserManager.AddLoginAsync(user.Id, externalLogin);
            if (!addExternalResult.Succeeded)
            {
                return new AuthenticateResult(addExternalResult.Errors.First());
            }

            var result = await AccountCreatedFromExternalProviderAsync(user.Id, provider, providerId, enumerable);
            if (result != null) return result;

            return await SignInFromExternalProviderAsync(user.Id, provider);
        }

        protected virtual Task<TUser> InstantiateNewUserFromExternalProviderAsync(string provider, string providerId,
            IEnumerable<Claim> claims)
        {
            var user = new TUser { UserName = Guid.NewGuid().ToString("N") };
            return Task.FromResult(user);
        }

        protected virtual Task<TUser> TryGetExistingUserFromExternalProviderClaimsAsync(string provider,
            IEnumerable<Claim> claims)
        {
            return Task.FromResult<TUser>(null);
        }

        protected virtual async Task<AuthenticateResult> AccountCreatedFromExternalProviderAsync(TKey userId,
            string provider, string providerId, IEnumerable<Claim> claims)
        {
            claims = await SetAccountEmailAsync(userId, claims);
            claims = await SetAccountPhoneAsync(userId, claims);

            return await UpdateAccountFromExternalClaimsAsync(userId, provider, providerId, claims);
        }

        protected virtual async Task<AuthenticateResult> SignInFromExternalProviderAsync(TKey userId, string provider)
        {
            var user = await UserManager.FindByIdAsync(userId);
            var claims = await GetClaimsForAuthenticateResult(user);

            return new AuthenticateResult(
                userId.ToString(),
                await GetDisplayNameForAccountAsync(userId),
                claims,
                authenticationMethod: global::IdentityServer3.Core.Constants.AuthenticationMethods.External,
                identityProvider: provider);
        }

        protected virtual async Task<AuthenticateResult> UpdateAccountFromExternalClaimsAsync(TKey userId,
            string provider, string providerId, IEnumerable<Claim> claims)
        {
            var existingClaims = await UserManager.GetClaimsAsync(userId);
            var enumerable = claims as Claim[] ?? claims.ToArray();
            var intersection = existingClaims.Intersect(enumerable, new ClaimComparer());
            var newClaims = enumerable.Except(intersection, new ClaimComparer());

            foreach (var claim in newClaims)
            {
                var result = await UserManager.AddClaimAsync(userId, claim);
                if (!result.Succeeded)
                {
                    return new AuthenticateResult(result.Errors.First());
                }
            }

            return null;
        }

        protected virtual async Task<AuthenticateResult> ProcessExistingExternalAccountAsync(TKey userId,
            string provider, string providerId, IEnumerable<Claim> claims)
        {
            return await SignInFromExternalProviderAsync(userId, provider);
        }

        protected virtual async Task<IEnumerable<Claim>> SetAccountEmailAsync(TKey userId, IEnumerable<Claim> claims)
        {
            var enumerable = claims as IList<Claim> ?? claims.ToList();
            var email = enumerable.FirstOrDefault(x => x.Type == global::IdentityServer3.Core.Constants.ClaimTypes.Email);
            if (email != null)
            {
                var userEmail = await UserManager.GetEmailAsync(userId);
                if (userEmail == null)
                {
                    // if this fails, then presumably the email is already associated with another account
                    // so ignore the error and let the claim pass thru
                    var result = await UserManager.SetEmailAsync(userId, email.Value);
                    if (result.Succeeded)
                    {
                        var emailVerified =
                            enumerable.FirstOrDefault(
                                x => x.Type == global::IdentityServer3.Core.Constants.ClaimTypes.EmailVerified);
                        if (emailVerified != null && emailVerified.Value == "true")
                        {
                            var token = await UserManager.GenerateEmailConfirmationTokenAsync(userId);
                            await UserManager.ConfirmEmailAsync(userId, token);
                        }

                        var emailClaims = new[]
                        {
                            global::IdentityServer3.Core.Constants.ClaimTypes.Email,
                            global::IdentityServer3.Core.Constants.ClaimTypes.EmailVerified
                        };
                        return enumerable.Where(x => !emailClaims.Contains(x.Type));
                    }
                }
            }

            return claims;
        }

        protected virtual async Task<IEnumerable<Claim>> SetAccountPhoneAsync(TKey userId, IEnumerable<Claim> claims)
        {
            var enumerable = claims as Claim[] ?? claims.ToArray();
            var phone = enumerable.FirstOrDefault(x => x.Type == global::IdentityServer3.Core.Constants.ClaimTypes.PhoneNumber);
            if (phone != null)
            {
                var userPhone = await UserManager.GetPhoneNumberAsync(userId);
                if (userPhone == null)
                {
                    // if this fails, then presumably the phone is already associated with another account
                    // so ignore the error and let the claim pass thru
                    var result = await UserManager.SetPhoneNumberAsync(userId, phone.Value);
                    if (result.Succeeded)
                    {
                        var phoneVerified =
                            enumerable.FirstOrDefault(
                                x => x.Type == global::IdentityServer3.Core.Constants.ClaimTypes.PhoneNumberVerified);
                        if (phoneVerified != null && phoneVerified.Value == "true")
                        {
                            var token = await UserManager.GenerateChangePhoneNumberTokenAsync(userId, phone.Value);
                            await UserManager.ChangePhoneNumberAsync(userId, phone.Value, token);
                        }

                        var phoneClaims = new[]
                        {
                            global::IdentityServer3.Core.Constants.ClaimTypes.PhoneNumber,
                            global::IdentityServer3.Core.Constants.ClaimTypes.PhoneNumberVerified
                        };
                        return enumerable.Where(x => !phoneClaims.Contains(x.Type));
                    }
                }
            }

            return claims;
        }

        public override async Task IsActiveAsync(IsActiveContext ctx)
        {
            var subject = ctx.Subject;

            if (subject == null) throw new ArgumentNullException(nameof(ctx.Subject));

            var id = subject.GetSubjectId();
            var key = ConvertSubjectToKey(id);
            var acct = await UserManager.FindByIdAsync(key);

            ctx.IsActive = false;

            if (acct != null)
            {
                if (EnableSecurityStamp && UserManager.SupportsUserSecurityStamp)
                {
                    var securityStamp =
                        subject.Claims.Where(x => x.Type == "security_stamp").Select(x => x.Value).SingleOrDefault();
                    if (securityStamp != null)
                    {
                        var dbSecurityStamp = await UserManager.GetSecurityStampAsync(key);
                        if (dbSecurityStamp != securityStamp)
                        {
                            return;
                        }
                    }
                }

                ctx.IsActive = true;
            }
        }
    }
}