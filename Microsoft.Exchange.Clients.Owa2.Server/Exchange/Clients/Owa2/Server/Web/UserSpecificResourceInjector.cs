using System;
using System.Web;
using Microsoft.Exchange.Clients.Common;
using Microsoft.Exchange.Clients.Owa2.Server.Core;

namespace Microsoft.Exchange.Clients.Owa2.Server.Web
{
	internal class UserSpecificResourceInjector : UserSpecificResourceInjectorBase
	{
		protected override IPageContext GetPageContext(HttpContext context, string owaVersion)
		{
			return new UserSpecificResourceInjector.PageContext(context, owaVersion);
		}

		public class PageContext : IPageContext
		{
			public PageContext(HttpContext context, string owaVersion)
			{
				this.UserAgent = OwaUserAgentUtilities.CreateUserAgentWithLayoutOverride(context);
				this.IsAppCacheEnabledClient = (context == null || context.Request.QueryString["appcacheclient"] == "1");
				this.ManifestType = (DefaultPageBase.IsPalEnabled(context, this.UserAgent.RawString) ? SlabManifestType.Pal : SlabManifestType.Standard);
				this.Theme = ThemeManagerFactory.GetInstance(owaVersion).GetThemeFolderName(this.UserAgent, context);
			}

			public UserAgent UserAgent { get; private set; }

			public string Theme { get; private set; }

			public bool IsAppCacheEnabledClient { get; private set; }

			public SlabManifestType ManifestType { get; private set; }

			public string FormatURIForCDN(string relativeUri, bool isBootResourceUri)
			{
				if (!this.IsAppCacheEnabledClient)
				{
					return Globals.FormatURIForCDN(relativeUri);
				}
				return relativeUri;
			}
		}
	}
}
