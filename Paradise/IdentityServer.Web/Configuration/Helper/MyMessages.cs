using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IdentityServer.Web.Configuration.Helper
{
    public static class MyMessages
    {

        public const string UserRegistrationFailed =
            "An error occurred while registration; Please contact system administrator.";

        public const string SendingActivationEmailFailed =
            "An error occurred while sending activation email; Please contact system administrator.";

        public const string EmailIdNotProvidedError =
            "Your {0} Social Identity profile doesn’t have email id, unable to register yourself, either update your profile in {0} try signin again or use other social login.";

        public const string EmailCodeNotification = "Activation Code has been sent, check your email for Code.";

        public const string EmailPendingConfirmationMessage = "Please check your email and confirm your email address.";
    }
}