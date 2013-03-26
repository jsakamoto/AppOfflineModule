using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Security;

namespace Toolbelt.Web
{
    /// <summary>
    /// The ASP.NET HTTP Module which intercept all of the requests when "Application Offline Mode" is enabled,
    /// and respond Offline Mode contents such as HTTP 400.
    /// </summary>
    public class AppOfflineModule : IHttpModule
    {
        private static AppOfflineFilte _Filter = new AppOfflineFilte();

        /// <summary>Gets or sets the filter which intercept and respond requests and provide configuration.</summary>
        public static AppOfflineFilte Filter { get { return _Filter; } set { _Filter = value; } }

        ///<summary>Initializes this instance.</summary>
        public void Init(HttpApplication context)
        {
            context.AuthenticateRequest += context_AuthenticateRequest;
        }

        /// <summary>Handles the AuthenticateRequest event of the Http Module.</summary>
        void context_AuthenticateRequest(object sender, EventArgs e)
        {
            if (Filter.IsEnable())
            {
                var app = sender as HttpApplication;
                if (IsBackDoorURL(app))
                {
                    HandleBackDoorAccess(app);
                }
                else if (Filter.IsAllowPassThrough(app))
                {
                    // NOP
                }
                else
                {
                    Filter.Action(app);
                }
            };
        }

        /// <summary>Determines whether is the URL of current request "back door URL" or not.
        /// <para>"back door URL" is specified by the value of "AppOffline.BackDoorUrl" key in appSettings, web.config.</para>
        /// <para>If the URL of request is "back door" then hanlde the request and respond with FormsAuthentication login page.</para>
        /// </summary>
        private bool IsBackDoorURL(HttpApplication app)
        {
            var appVPath = HttpRuntime.AppDomainAppVirtualPath;
            appVPath += appVPath.EndsWith("/") ? "" : "/";
            var backDoorUrl = Regex.Replace(
                ConfigurationManager.AppSettings["AppOffline.BackDoorUrl"] ?? "",
                "^~/",
                appVPath);
            return app.Request.RawUrl == backDoorUrl;
        }

        /// <summary>Handles the "back door" access.
        /// <para>Respond user/password login form HTML content at GET request, and validate user at POST request.</para>
        /// </summary>
        private static void HandleBackDoorAccess(HttpApplication app)
        {
            var response = app.Context.Response;
            switch (app.Request.HttpMethod)
            {
                case "GET":
                    RespondBackDoorPage(app);
                    break;
                case "POST":
                    var username = app.Request.Form["username"];
                    var password = app.Request.Form["password"];
                    if (Membership.ValidateUser(username, password))
                    {
                        FormsAuthentication.SetAuthCookie(username, false);
                        response.Redirect("~/");
                    }
                    else
                    {
                        RespondBackDoorPage(app, message: "Invalid login name or password.");
                    }
                    break;
                default:
                    break;
            }
        }

        /// <summary>Responds the back door page(user/password login form HTML content)</summary>
        /// <param name="message">The message text to display on the login form to report invalid username or password.</param>
        private static void RespondBackDoorPage(HttpApplication app, string message = null)
        {
            var response = app.Response;
            var username = HttpUtility.HtmlAttributeEncode(app.Request.Form["username"] ?? "");
            response.StatusCode = 200;
            response.Write(
                @"<!DOCTYPE html><html><head><title>Back Door</title></head><body><h1>Back Door.</h1>" +
                "<p>" + (message ?? "") + "</p>" +
                "<form method='POST'>" +
                "<p>user name: <input type='text' name='username' value='" + username + "' autofocus/></p>" +
                "<p>password: <input type='password' name='password'/></p>" +
                "<p><input type='submit' value='Login'/></p>" +
                "</form>" +
                "<!-- " + new string('X', 8192) + "--></body></html>");
            response.End();
        }

        /// <summary>
        /// Disposes of the resources (other than memory) used by the module that implements <see cref="T:System.Web.IHttpModule" />.
        /// </summary>
        public void Dispose()
        {
        }
    }
}
