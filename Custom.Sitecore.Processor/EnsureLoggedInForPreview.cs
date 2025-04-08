using Sitecore;
using Sitecore.Configuration;
using Sitecore.Diagnostics;
using Sitecore.Mvc.Pipelines.MvcEvents.ActionExecuting;
using Sitecore.Security.Accounts;
using Sitecore.Shell.Web;
using Sitecore.Sites;
using System;
using System.Web;
using System.Web.Mvc;

namespace Custom.Sitecore.Processor.CustomPipeline
{
    internal class EnsureLoggedInForPreview
    {

        public void Process(ActionExecutingArgs args)
        {            
            var controllerName = args.Context.RouteData.Values["controller"]?.ToString().Trim().ToLowerInvariant();
            var actionName = args.Context.RouteData.Values["action"]?.ToString();

            if (controllerName.StartsWith("sidekick.")) //make it unique for sidekick call
                return;

            if (Context.PageMode.IsNormal || Context.IsLoggedIn)
                return;
            using (new SiteContextSwitcher(Factory.GetSite("shell")))
            {
                using (new UserSwitcher(Context.User))
                    ShellPage.IsLoggedIn(true, (Action<HttpContext, string>)((httpContext, loginUrl) => this.RedirectToLogin(args.Context, loginUrl)));
            }
        }

        private void RedirectToLogin(ActionExecutingContext actionContext, string loginUrl)
        {
            Assert.ArgumentNotNull((object)actionContext, nameof(actionContext));
            Assert.ArgumentNotNull((object)loginUrl, nameof(loginUrl));
            actionContext.Result = (ActionResult)new RedirectResult(loginUrl, false);
        }

    }
}
