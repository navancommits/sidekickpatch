using Sitecore;
using Sitecore.Configuration;
using Sitecore.Diagnostics;
using Sitecore.Mvc.Pipelines.MvcEvents.ActionExecuting;
using Sitecore.Security.Accounts;
using Sitecore.Shell.Web;
using Sitecore.Sites;
using System;
//using System.Runtime.Remoting.Contexts;
using System.Web;
using System.Web.Mvc;

namespace Custom.Sitecore.Processor.CustomPipeline
{
    internal class EnsureLoggedInForPreview
    {

        public void Process(ActionExecutingArgs args)
        {            
            var path = args.Context.HttpContext?.Request?.Url?.AbsolutePath.ToLower() ?? string.Empty;
            Log.Info("navanlog- EnsureLoggedInForPreview invocation " + path, this);
            if (path.Contains("/scs/"))
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
