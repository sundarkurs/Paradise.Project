using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Validation;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using IdentityServer.Web.IdentityManage;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace IdentityServer.Web.AspNetIdentity
{
    /// <summary>
    ///     Extent the User Table, with custom columns
    /// </summary>
    /// <seealso cref="Microsoft.AspNet.Identity.EntityFramework.IdentityUser" />
    public class User : IdentityUser, IDisposable
    {
        /// <summary>
        ///     Gets or sets the first name.
        /// </summary>
        /// <value>
        ///     The first name.
        /// </value>
        [Required]
        [StringLength(50, ErrorMessage = "First name cannot be longer than 50 characters.")]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        /// <summary>
        ///     Gets or sets the name of the family.
        /// </summary>
        /// <value>
        ///     The name of the family.
        /// </value>
        [Required]
        [StringLength(50, ErrorMessage = "Family name cannot be longer than 50 characters.")]
        [Display(Name = "Family Name")]
        public string FamilyName { get; set; }

        /// <summary>
        ///     Gets or sets the name of the middle.
        /// </summary>
        /// <value>
        ///     The name of the middle.
        /// </value>
        [StringLength(50, ErrorMessage = "Middle name cannot be longer than 50 characters.")]
        [Display(Name = "Middle Name")]
        public string MiddleName { get; set; }

        /// <summary>
        ///     Gets or sets the identity provider.
        /// </summary>
        /// <value>
        ///     The identity provider.
        /// </value>
        [StringLength(30, ErrorMessage = "Identity Provider Name cannot be longer than 30 characters.")]
        public string LoginProvider { get; set; }

        /// <summary>
        ///     Gets or sets unique user identifier.
        /// </summary>
        /// <value>
        ///     The user identifier.
        /// </value>
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int UserId { get; set; }

        /// <summary>
        ///     Email
        /// </summary>
        [StringLength(256)]
        [Required(ErrorMessage = "The email address is required")]
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        [Display(Name = "Email Id")]
        public override string Email { get; set; }

        /// <summary>
        ///     PhoneNumber for the user
        /// </summary>
        [StringLength(50)]
        [Required(ErrorMessage = "Phone number required")]
        [Display(Name = "Phone No.")]
        public override string PhoneNumber { get; set; }

        /// <summary>
        ///     Releases unmanaged and - optionally - managed resources.
        /// </summary>
        void IDisposable.Dispose()
        {
        }
    }

    /// <summary>
    ///     User Role
    /// </summary>
    /// <seealso cref="Microsoft.AspNet.Identity.EntityFramework.IdentityRole" />
    public class Role : IdentityRole
    {
    }

    /// <summary>
    ///     Context
    /// </summary>
    /// <seealso
    ///     cref="Microsoft.AspNet.Identity.EntityFramework.IdentityDbContext{User, Role, String, IdentityUserLogin, IdentityUserRole, IdentityUserClaim}" />
    public class Context : IdentityDbContext<User, Role, string, IdentityUserLogin, IdentityUserRole, IdentityUserClaim>
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="Context" /> class.
        /// </summary>
        public Context() : this(LocalConstants.UserAdminConfig)
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="Context" /> class.
        /// </summary>
        /// <param name="connString">The connection string.</param>
        public Context(string connString)
            : base(connString)
        {
        }

        /// <summary>
        ///     Asynchronously saves all changes made in this context to the underlying database.
        /// </summary>
        /// <returns>
        ///     A task that represents the asynchronous save operation.
        ///     The task result contains the number of state entries written to the underlying database. This can include
        ///     state entries for entities and/or relationships. Relationship state entries are created for
        ///     many-to-many relationships and relationships where there is no foreign key property
        ///     included in the entity class (often referred to as independent associations).
        /// </returns>
        /// <exception cref="DbEntityValidationException">
        ///     Entity Validation Failed - errors follow:\n +
        ///     sb
        /// </exception>
        /// <remarks>
        ///     Multiple active operations on the same context instance are not supported.  Use 'await' to ensure
        ///     that any asynchronous operations have completed before calling another method on this context.
        /// </remarks>
        public override Task<int> SaveChangesAsync()
        {
            try
            {
                return base.SaveChangesAsync();
            }
            catch (DbEntityValidationException ex)
            {
                var sb = new StringBuilder();

                foreach (var failure in ex.EntityValidationErrors)
                {
                    sb.AppendFormat("{0} failed validation\n", failure.Entry.Entity.GetType());
                    foreach (var error in failure.ValidationErrors)
                    {
                        sb.AppendFormat("- {0} : {1}", error.PropertyName, error.ErrorMessage);
                        sb.AppendLine();
                    }
                }

                throw new DbEntityValidationException(
                    "Entity Validation Failed - errors follow:\n" +
                    sb, ex
                    ); // Add the original exception as the innerException
            }
        }

        ///// <summary>
        /////     Saves all changes made in this context to the underlying database.
        ///// </summary>
        ///// <returns>
        /////     The number of state entries written to the underlying database. This can include
        /////     state entries for entities and/or relationships. Relationship state entries are created for
        /////     many-to-many relationships and relationships where there is no foreign key property
        /////     included in the entity class (often referred to as independent associations).
        ///// </returns>
        ///// <exception cref="DbEntityValidationException">
        /////     Entity Validation Failed - errors follow:\n +
        /////     sb
        ///// </exception>
        //public override int SaveChanges()
        //{
        //    try
        //    {
        //        return base.SaveChanges();
        //    }
        //    catch (DbEntityValidationException ex)
        //    {
        //        var sb = new StringBuilder();

        //        foreach (var failure in ex.EntityValidationErrors)
        //        {
        //            sb.AppendFormat("{0} failed validation\n", failure.Entry.Entity.GetType());
        //            foreach (var error in failure.ValidationErrors)
        //            {
        //                sb.AppendFormat("- {0} : {1}", error.PropertyName, error.ErrorMessage);
        //                sb.AppendLine();
        //            }
        //        }

        //        throw new DbEntityValidationException(
        //            "Entity Validation Failed - errors follow:\n" +
        //            sb, ex
        //            ); // Add the original exception as the innerException
        //    }
        //    catch (Exception ex)
        //    {
        //        Logger.Error(ex, GetType().FullName + "/" + MethodBase.GetCurrentMethod().Name);
        //        return 0;
        //    }
        //}
    }

    /// <summary>
    ///     User Store
    /// </summary>
    /// <seealso
    ///     cref="Microsoft.AspNet.Identity.EntityFramework.UserStore{User, Role, String, IdentityUserLogin, IdentityUserRole, IdentityUserClaim}" />
    public class UserStore : UserStore<User, Role, string, IdentityUserLogin, IdentityUserRole, IdentityUserClaim>
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="UserStore" /> class.
        /// </summary>
        /// <param name="ctx">The CTX.</param>
        public UserStore(Context ctx)
            : base(ctx)
        {
        }
    }

    /// <summary>
    ///     Phone Number Token Provider
    /// </summary>
    /// <seealso cref="string" />
    public class CustomSmsTokenProvider : PhoneNumberTokenProvider<User, string>
    {
    }

    /// <summary>
    ///     Custom Email Token Provider
    /// </summary>
    /// <seealso cref="Microsoft.AspNet.Identity.EmailTokenProvider{User, String}" />
    public class CustomEmailTokenProvider : EmailTokenProvider<User, string>
    {
    }

    /// <summary>
    ///     User Manager
    /// </summary>
    /// <seealso cref="Microsoft.AspNet.Identity.UserManager{User, String}" />
    public sealed class UserManager : UserManager<User, string>
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="UserManager" /> class.
        /// </summary>
        /// <param name="store">The store.</param>
        public UserManager(UserStore store)
            : base(store)
        {
            //TODOSUN
            //PasswordHasher = new CustomPasswordHasher();

            RegisterTwoFactorProvider("sms", new CustomSmsTokenProvider());
            RegisterTwoFactorProvider("EmailCode", new CustomEmailTokenProvider());
        }

        /// <summary>
        ///     Create a user with no password, non GFOB user creation.
        /// </summary>
        /// <param name="user">user param</param>
        /// <returns></returns>
        public override Task<IdentityResult> CreateAsync(User user)
        {
            var result = base.CreateAsync(user);

            return result;
        }
    }

    /// <summary>
    ///     Role Store
    /// </summary>
    /// <seealso cref="Microsoft.AspNet.Identity.EntityFramework.RoleStore{Role}" />
    public class RoleStore : RoleStore<Role>
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="RoleStore" /> class.
        /// </summary>
        /// <param name="ctx">The CTX.</param>
        public RoleStore(Context ctx)
            : base(ctx)
        {
        }
    }


    /// <summary>
    ///     Role Manager
    /// </summary>
    /// <seealso cref="Microsoft.AspNet.Identity.RoleManager{Role}" />
    public class RoleManager : RoleManager<Role>
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="RoleManager" /> class.
        /// </summary>
        /// <param name="store">The store.</param>
        public RoleManager(RoleStore store)
            : base(store)
        {
        }
    }
}