using System;
using System.Collections.Generic;
using System.Globalization;
using System.Web;
using Microsoft.Exchange.Clients.Common;
using Microsoft.Exchange.Clients.Owa2.Server.Core;
using Microsoft.Exchange.Clients.Security;
using Microsoft.Exchange.Net;
using Microsoft.Exchange.VariantConfiguration;

namespace Microsoft.Exchange.Clients.Owa2.Server.Web
{
	internal class AppCacheManifestHandler : AppCacheManifestHandlerBase
	{
		public override string VersionString
		{
			get
			{
				if (this.buildVersion == null)
				{
					UserContext userContext = UserContextManager.GetUserContext(base.Context);
					this.buildVersion = userContext.CurrentOwaVersion;
				}
				return this.buildVersion;
			}
			set
			{
				this.buildVersion = value;
			}
		}

		protected override bool IsDatacenterNonDedicated
		{
			get
			{
				return VariantConfiguration.GetSnapshot(MachineSettingsContext.Local, null, null).Global.MultiTenancy.Enabled;
			}
		}

		protected override string ResourceDirectory
		{
			get
			{
				return "../prem/" + this.VersionString;
			}
		}

		protected override bool UseRootDirForAppCacheVdir
		{
			get
			{
				return VariantConfiguration.GetSnapshot(MachineSettingsContext.Local, null, null).OwaDeployment.UseRootDirForAppCacheVdir.Enabled;
			}
		}

		protected override bool IsHostNameSwitchFlightEnabled
		{
			get
			{
				UserContext userContext = UserContextManager.GetUserContext(base.Context);
				return userContext != null && userContext.FeaturesManager != null && userContext.FeaturesManager.ServerSettings.OwaHostNameSwitch.Enabled;
			}
		}

		public override HostNameController HostNameController
		{
			get
			{
				return RequestDispatcher.HostNameController;
			}
		}

		protected override string[] GetEnabledFeatures()
		{
			FeaturesManager featuresManager = UserContextManager.GetUserContext(base.Context).FeaturesManager;
			return featuresManager.GetClientEnabledFeatures();
		}

		internal static string GetUserContextId(HttpContext context)
		{
			UserContextCookie userContextCookie = UserContextCookie.GetUserContextCookie(context);
			if (userContextCookie == null || userContextCookie.CookieValue == null)
			{
				return string.Empty;
			}
			return userContextCookie.CookieValue;
		}

		protected override bool IsRealmRewrittenFromPathToQuery(HttpContext context)
		{
			return UrlUtilities.IsRealmRewrittenFromPathToQuery(context);
		}

		protected override string GetThemeFolder()
		{
			return ThemeManagerFactory.GetInstance(this.VersionString).GetThemeFolderName(this.UserAgent, base.Context);
		}

		protected override bool ShouldSkipThemeFolder()
		{
			return ThemeManagerFactory.GetInstance(this.VersionString).ShouldSkipThemeFolder;
		}

		protected override string GetUserUPN()
		{
			string result = string.Empty;
			UserContext userContext = UserContextManager.GetUserContext(base.Context);
			if (userContext != null && userContext.ExchangePrincipal != null)
			{
				result = userContext.UserPrincipalName;
			}
			return result;
		}

		protected override List<CultureInfo> GetSupportedCultures()
		{
			return ClientCultures.SupportedCultureInfos;
		}

		protected override bool ShouldAddDefaultMasterPageEntiresWithFlightDisabled()
		{
			if (this.IsMowaClient)
			{
				HttpCookie httpCookie = base.Context.Request.Cookies.Get("flights");
				if (httpCookie != null && !string.IsNullOrEmpty(httpCookie.Value) && httpCookie.Value.Equals("none", StringComparison.InvariantCultureIgnoreCase))
				{
					return true;
				}
			}
			return false;
		}

		private string buildVersion;
	}
}
