using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using CV.Web.Controllers;
using CV.Web.Models;
using CV.Web;

namespace CV.Web.Controllers
{
    /// <summary>
    /// User related operation center
    /// </summary>
    public class UserController : BaseController
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public UserController()
        {
            UsesDatabase = true;
        }

        #region Overrides
        protected override string SetTitle(string action)
        {
            switch (action.ToLower())
            {
                case "deleteuser": return "Delete User";
                case "logon": return "LogOn";
                case "logoff": return "LogOff";
                case "changepassword": return "Change Password";
                case "changepasswordsuccess": return "Change Password Success";
                case "register": return "Register";
                case "registerdisabled": return "Registration Disabled";
                default: return base.SetTitle(action);
            }
        }
        #endregion

        #region Actions
        /// <summary>
        /// Logon action (initial)
        /// </summary>
        /// <returns>Initial view</returns>
        public ActionResult LogOn()
        {
            return View();
        }

        /// <summary>
        /// Logon action
        /// </summary>
        /// <param name="model">Logon information</param>
        /// <param name="returnUrl">Return url, requires for some pages</param>
        /// <returns>Action result</returns>
        [HttpPost]
        public ActionResult LogOn(LogOnModel model, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                if (Membership.ValidateUser(model.UserName, model.Password))
                {
                    FormsAuthentication.SetAuthCookie(model.UserName, model.RememberMe);

                    System.Web.Profile.ProfileBase pp = System.Web.Profile.ProfileBase.Create(model.UserName, true);
                    Session["Culture"] = pp.GetPropertyValue("Culture");

                    if (Url.IsLocalUrl(returnUrl) && returnUrl.Length > 1 && returnUrl.StartsWith("/")
                        && !returnUrl.StartsWith("//") && !returnUrl.StartsWith("/\\"))
                    {
                        return Redirect(returnUrl);
                    }
                    else
                    {
                        return RedirectToIndex();
                    }
                }
                else
                {
                    ModelState.AddModelError("", "The user name or password provided is incorrect.");
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        /// <summary>
        /// Logoff action
        /// </summary>
        /// <returns>Action result</returns>
        public ActionResult LogOff()
        {
            FormsAuthentication.SignOut();
            return RedirectToIndex();
        }

        /// <summary>
        /// Register disabled action
        /// </summary>
        /// <returns>Action result</returns>
        public ActionResult RegisterDisabled()
        {
            return View();
        }

        /// <summary>
        /// Registers new user action (initial)
        /// </summary>
        /// <returns>Initial view</returns>
        public ActionResult Register()
        {
            if (!Configuration.IsRegistrationEnabled) return RedirectToAction("RegisterDisabled");

            return View();
        }

        /// <summary>
        /// Registers new user with registration information
        /// </summary>
        /// <param name="model">Registration information</param>
        /// <returns>Action result</returns>
        [HttpPost]
        public ActionResult Register(RegisterUserModel model)
        {
            if (!Configuration.IsRegistrationEnabled) return RedirectToAction("RegisterDisabled");

            if (ModelState.IsValid)
            {
                // Attempt to register the user
                MembershipCreateStatus createStatus;
                MembershipUser newUser = Membership.CreateUser(model.UserName, model.Password, model.Email, null, null, true, null, out createStatus);

                if (createStatus == MembershipCreateStatus.Success)
                {
                    FormsAuthentication.SetAuthCookie(model.UserName, false /* createPersistentCookie */);

                    System.Web.Profile.ProfileBase pp = System.Web.Profile.ProfileBase.Create(model.UserName, true);
                    pp.SetPropertyValue("Culture", Configuration.InitialCulture);
                    pp.SetPropertyValue("IsAdmin", Configuration.IsNewUserAdmin);
                    pp.Save();

                    return RedirectToIndex();
                }
                else
                {
                    ModelState.AddModelError("", ErrorCodeToString(createStatus));
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        /// <summary>
        /// Changes user's password action (initial)
        /// </summary>
        /// <returns>Initial view</returns>
        [Authorize]
        public ActionResult ChangePassword()
        {
            return View();
        }

        /// <summary>
        /// Changes user password
        /// </summary>
        /// <param name="model">User password information</param>
        /// <returns>Action result</returns>
        [Authorize, HttpPost]
        public ActionResult ChangePassword(ChangeUserPasswordModel model)
        {
            if (ModelState.IsValid)
            {

                // ChangePassword will throw an exception rather
                // than return false in certain failure scenarios.
                bool changePasswordSucceeded;
                try
                {
                    MembershipUser currentUser = Membership.GetUser(User.Identity.Name, true /* userIsOnline */);
                    changePasswordSucceeded = currentUser.ChangePassword(model.OldPassword, model.NewPassword);
                }
                catch (Exception)
                {
                    changePasswordSucceeded = false;
                }

                if (changePasswordSucceeded)
                {
                    return RedirectToAction("ChangePasswordSuccess");
                }
                else
                {
                    ModelState.AddModelError("", "The current password is incorrect or the new password is invalid.");
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        /// <summary>
        /// Change password success action
        /// </summary>
        /// <returns>View</returns>
        [Authorize]
        public ActionResult ChangePasswordSuccess()
        {
            return View();
        }

        /// <summary>
        /// Deletes an existing user (initial)
        /// </summary>
        /// <returns>Initial view</returns>
        [Authorize]
        public ActionResult DeleteUser()
        {
            if (!IsDeleteUserActive) return RedirectToIndex();

            if (!IsAdministrator) return RedirectToIndex();

            return View();
        }

        /// <summary>
        /// Deletes an existing user
        /// </summary>
        /// <param name="model">User information for delete process</param>
        /// <returns></returns>
        [Authorize, HttpPost]
        public ActionResult DeleteUser(DeleteUserModel model)
        {
            if (!IsDeleteUserActive) return RedirectToIndex();

            if (!IsAdministrator) return RedirectToIndex();

            if (ModelState.IsValid)
            {
                bool deleteSuccess = false;
                try
                {
                    deleteSuccess = Membership.DeleteUser(model.UserName);
                }
                catch (Exception)
                {
                }

                if (deleteSuccess)
                {
                    return RedirectToIndex();
                }
                else
                {
                    ModelState.AddModelError("", "The current password is incorrect or the new password is invalid.");
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        #endregion

        #region Status Codes
        private static string ErrorCodeToString(MembershipCreateStatus createStatus)
        {
            // See http://go.microsoft.com/fwlink/?LinkID=177550 for
            // a full list of status codes.
            switch (createStatus)
            {
                case MembershipCreateStatus.DuplicateUserName:
                    return "User name already exists. Please enter a different user name.";

                case MembershipCreateStatus.DuplicateEmail:
                    return "A user name for that e-mail address already exists. Please enter a different e-mail address.";

                case MembershipCreateStatus.InvalidPassword:
                    return "The password provided is invalid. Please enter a valid password value.";

                case MembershipCreateStatus.InvalidEmail:
                    return "The e-mail address provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidAnswer:
                    return "The password retrieval answer provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidQuestion:
                    return "The password retrieval question provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidUserName:
                    return "The user name provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.ProviderError:
                    return "The authentication provider returned an error. Please verify your entry and try again. If the problem persists, please contact your system administrator.";

                case MembershipCreateStatus.UserRejected:
                    return "The user creation request has been canceled. Please verify your entry and try again. If the problem persists, please contact your system administrator.";

                default:
                    return "An unknown error occurred. Please verify your entry and try again. If the problem persists, please contact your system administrator.";
            }
        }
        #endregion
    }
}
