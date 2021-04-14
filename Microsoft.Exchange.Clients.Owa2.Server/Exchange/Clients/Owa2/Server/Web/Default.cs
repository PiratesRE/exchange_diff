using System;
using System.Threading;
using System.Web;
using System.Web.Security.AntiXss;
using Microsoft.Exchange.Clients.Owa2.Server.Core;
using Microsoft.Exchange.Clients.Owa2.Server.Diagnostics;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Services;
using Microsoft.Exchange.VariantConfiguration;

namespace Microsoft.Exchange.Clients.Owa2.Server.Web
{
	public class Default : DefaultPageBase
	{
		public override string TenantId
		{
			get
			{
				UserContext userContext = UserContextManager.GetUserContext(this.Context);
				if (userContext != null && userContext.ExchangePrincipal != null && userContext.ExchangePrincipal.MailboxInfo.OrganizationId != null)
				{
					return userContext.ExchangePrincipal.MailboxInfo.OrganizationId.ToExternalDirectoryOrganizationId();
				}
				Guid guid = new Guid(new byte[16]);
				return guid.ToString();
			}
		}

		public override string MdbGuid
		{
			get
			{
				UserContext userContext = UserContextManager.GetUserContext(this.Context);
				if (userContext != null && userContext.ExchangePrincipal != null && userContext.ExchangePrincipal.MailboxInfo.MailboxDatabase != null)
				{
					return userContext.ExchangePrincipal.MailboxInfo.GetDatabaseGuid().ToString();
				}
				Guid guid = new Guid(new byte[16]);
				return guid.ToString();
			}
		}

		public override string VersionString
		{
			get
			{
				if (this.buildVersion == null)
				{
					UserContext userContext = UserContextManager.GetUserContext(this.Context);
					this.buildVersion = userContext.CurrentOwaVersion;
				}
				return this.buildVersion;
			}
			set
			{
				this.buildVersion = value;
			}
		}

		public static string MapControlUrl
		{
			get
			{
				if (Default.mapControlUrlForTesting != null)
				{
					return Default.mapControlUrlForTesting;
				}
				return PlacesConfigurationCache.GetMapControlUrl(Thread.CurrentThread.CurrentUICulture.Name);
			}
			set
			{
				Default.mapControlUrlForTesting = value;
			}
		}

		public string OwaTitle
		{
			get
			{
				return AntiXssEncoder.HtmlEncode(Strings.OwaTitle, false);
			}
		}

		public override string SlabsManifest
		{
			get
			{
				SlabManifestType slabManifestType = DefaultPageBase.IsPalEnabled(this.Context) ? SlabManifestType.Pal : SlabManifestType.Standard;
				UserContext userContext = UserContextManager.GetUserContext(HttpContext.Current, true);
				string[] features = (userContext == null) ? new string[0] : userContext.FeaturesManager.GetClientEnabledFeatures();
				return SlabManifestCollectionFactory.GetInstance(this.VersionString).GetSlabsJson(slabManifestType, features, this.UserAgent.Layout);
			}
		}

		public override string IsChangeLayoutFeatureEnabled
		{
			get
			{
				if (this.isChangeLayoutFeatureEnabled == null)
				{
					this.isChangeLayoutFeatureEnabled = new bool?(UserContextManager.GetUserContext(HttpContext.Current).FeaturesManager.ClientServerSettings.ChangeLayout.Enabled);
				}
				return this.isChangeLayoutFeatureEnabled.Value.ToString().ToLowerInvariant();
			}
		}

		public bool IsEdgeModeEnabled
		{
			get
			{
				if (this.isEdgeModeEnabled == null)
				{
					this.isEdgeModeEnabled = new bool?(UserContextManager.GetUserContext(HttpContext.Current).FeaturesManager.ServerSettings.OWAEdgeMode.Enabled);
				}
				return this.isEdgeModeEnabled.Value;
			}
		}

		public bool IsO365HeaderEnabled
		{
			get
			{
				if (this.isO365HeaderEnabled == null)
				{
					this.isO365HeaderEnabled = new bool?(UserContextManager.GetUserContext(HttpContext.Current).FeaturesManager.ClientServerSettings.O365Header.Enabled);
				}
				return this.isO365HeaderEnabled.Value;
			}
		}

		public bool IsO365G2HeaderEnabled
		{
			get
			{
				if (this.isO365G2HeaderEnabled == null)
				{
					this.isO365G2HeaderEnabled = new bool?(UserContextManager.GetUserContext(HttpContext.Current).FeaturesManager.ClientServerSettings.O365G2Header.Enabled);
				}
				return this.isO365G2HeaderEnabled.Value;
			}
		}

		public bool ShouldEmitHeaderImageClass
		{
			get
			{
				if (this.shouldEmitHeaderImageClass == null)
				{
					this.shouldEmitHeaderImageClass = new bool?(ThemeManagerFactory.GetInstance(this.VersionString).ShouldSkipThemeFolder);
				}
				return this.shouldEmitHeaderImageClass.Value;
			}
		}

		protected override void OnPreInit(EventArgs e)
		{
			base.OnPreInit(e);
			string text = base.IsAppCacheEnabledClient ? "1" : "0";
			base.Response.AppendToLog("&appcache=" + text);
			RequestDetailsLogger getRequestDetailsLogger = OwaApplication.GetRequestDetailsLogger;
			RequestDetailsLoggerBase<RequestDetailsLogger>.SafeSetLogger(getRequestDetailsLogger, OwaServerLogger.LoggerData.AppCache, text);
		}

		protected bool ForcePLTOnVersionChange
		{
			get
			{
				return !this.IsDatacenterNonDedicated || !this.CdnEnabled;
			}
		}

		protected bool CheckCdnVersionAvailability
		{
			get
			{
				return base.IsAppCacheEnabledClient && this.IsDatacenterNonDedicated && this.CdnEnabled && !base.IsClientInOfflineMode;
			}
		}

		protected virtual bool IsDatacenterNonDedicated
		{
			get
			{
				return VariantConfiguration.GetSnapshot(MachineSettingsContext.Local, null, null).Global.MultiTenancy.Enabled;
			}
		}

		protected virtual bool CdnEnabled
		{
			get
			{
				if (Default.cdnEnabled == null)
				{
					Default.cdnEnabled = new bool?(!string.IsNullOrEmpty(Globals.ContentDeliveryNetworkEndpoint));
				}
				return Default.cdnEnabled.Value;
			}
		}

		public override string FormatURIForCDN(string relativeUri, bool isBootResourceUri)
		{
			if (this.IsURIAppcacheable(isBootResourceUri) || this.UserAgent.IsBrowserIE8())
			{
				return relativeUri;
			}
			return Globals.FormatURIForCDN(relativeUri);
		}

		public override string GetCdnEndpointForResources(bool bootResources)
		{
			if (this.IsURIAppcacheable(bootResources) || this.UserAgent.IsBrowserIE8())
			{
				return string.Empty;
			}
			return Globals.ContentDeliveryNetworkEndpoint ?? string.Empty;
		}

		protected bool IsURIAppcacheable(bool isBootResourceFolder)
		{
			return (base.IsAppCacheEnabledClient && isBootResourceFolder) || base.IsOfflineAppCacheEnabledClient;
		}

		protected override string GetThemeFolder()
		{
			return ThemeManagerFactory.GetInstance(this.VersionString).GetThemeFolderName(this.UserAgent, this.Context);
		}

		protected override bool ShouldSkipThemeFolder()
		{
			return ThemeManagerFactory.GetInstance(this.VersionString).ShouldSkipThemeFolder;
		}

		protected override string GetFeaturesSupportedJsonArray(FlightedFeatureScope scope)
		{
			UserContext userContext = UserContextManager.GetUserContext(HttpContext.Current);
			return UserResourcesFinder.GetEnabledFlightedFeaturesJsonArray(this.ManifestType, userContext, scope);
		}

		private const string IsAppCacheInstallationRequestLogKey = "&appcache=";

		private static bool? cdnEnabled = null;

		private static string mapControlUrlForTesting = null;

		private string buildVersion;

		private bool? isChangeLayoutFeatureEnabled;

		private bool? isEdgeModeEnabled;

		private bool? isO365HeaderEnabled;

		private bool? isO365G2HeaderEnabled;

		private bool? shouldEmitHeaderImageClass;
	}
}
