using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace Toolbelt.Web
{
    /// <summary>
    /// Offline filter which control Online/Offline mode.
    /// </summary>
    public class AppOfflineFilte
    {
        /// <summary>Gets or sets the function that return this filter is enabled or not.
        /// <para>If this filter enabled, all of any requests is handled by the action provided "Action" property.</para>
        /// </summary>
        public Func<bool> IsEnable { get; set; }

        /// <summary>Gets or sets the function that return the action to execute when this filter enabled.</summary>
        public Action<HttpApplication> Action { get; set; }

        /// <summary>Gets or sets the function that return role names to allow "pass through" access.
        /// <para>If the current authenticated user enrolled any roles which this function returned then all of any requests proceeded normaly, not filtered.</para>
        /// <para>role names are concatinated with comma separetor.</para>
        /// <para>If this property is null or empty then any roles can not back door access.</para>
        /// </summary>
        public Func<HttpApplication, bool> IsAllowPassThrough { get; set; }

        /// <summary>Initializes a new instance of the <see cref="AppOfflineFilte"/> class.
        /// <para>Set IsEnabled property with the function that returned true when the key "AppOffline.State" entry value is "offline" in appSettings, web.config.</para>
        /// <para>Set AllowBackDoorRoles property with the function that returned the value of key "AppOffline.AllowBackDoorRoles" entry in appSettings, web.config.</para>
        /// <para>Set Action property with the function that returned the action which respond HTTP404.</para>
        /// </summary>
        public AppOfflineFilte()
        {
            IsEnable = () => (ConfigurationManager.AppSettings["AppOffline.State"] ?? "").ToLower() == "offline";
            IsAllowPassThrough = IsAllowPassThroughCore;
            Action = ActionCore;
        }

        /// <summary>Determines whether is allow pass through or not.</summary>
        private static bool IsAllowPassThroughCore(HttpApplication app)
        {
            if (app.User == null || app.User.Identity.IsAuthenticated == false) return false;

            Func<string, IEnumerable<string>> get = (entryName) => (ConfigurationManager.AppSettings[entryName] ?? "")
                .Split(',')
                .Select(s => s.Trim())
                .Where(s => s != "");

            var allowUsers = get("AppOffline.AllowPassThroughUsers");
            if (allowUsers.Any(username => app.User.Identity.Name == username)) return true;

            if (Roles.Enabled)
            {
                var allowRoles = get("AppOffline.AllowPassThroughRoles");
                if (allowRoles.Any(role => Roles.IsUserInRole(role))) return true;
            }

            return false;
        }

        /// <summary>The actions respond HTTP 404 "Not Found" to client.</summary>
        private static void ActionCore(HttpApplication app)
        {
            app.Response.StatusCode = 404;
            app.Response.Write(
                "<!DOCTYPE html><html>" +
                "<head>" +
                "<title>HTTP Error 404.0 - Not Found</title>" +
                "<style>h1 {color:red; font-size:100%; font-weight:bold; border-bottom:solid 1px silver;}</style>" +
                "</head>" +
                "<body><h1>HTTP Error 404.0 - Not Found.</h1></body>" +
                "</html>");
            app.Response.End();
        }
    }
}
