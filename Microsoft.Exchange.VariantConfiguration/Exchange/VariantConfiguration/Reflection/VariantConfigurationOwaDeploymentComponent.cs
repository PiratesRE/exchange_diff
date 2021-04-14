using System;

namespace Microsoft.Exchange.VariantConfiguration.Reflection
{
	public sealed class VariantConfigurationOwaDeploymentComponent : VariantConfigurationComponent
	{
		internal VariantConfigurationOwaDeploymentComponent() : base("OwaDeployment")
		{
			base.Add(new VariantConfigurationSection("OwaDeployment.settings.ini", "PublicFolderTreePerTenanant", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("OwaDeployment.settings.ini", "ExplicitLogonAuthFilter", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("OwaDeployment.settings.ini", "Places", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("OwaDeployment.settings.ini", "IncludeAccountAccessDisclaimer", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("OwaDeployment.settings.ini", "FilterETag", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("OwaDeployment.settings.ini", "CacheUMCultures", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("OwaDeployment.settings.ini", "RedirectToServer", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("OwaDeployment.settings.ini", "UseAccessProxyForInstantMessagingServerName", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("OwaDeployment.settings.ini", "UseBackendVdirConfiguration", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("OwaDeployment.settings.ini", "OneDriveProProviderAvailable", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("OwaDeployment.settings.ini", "LogTenantInfo", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("OwaDeployment.settings.ini", "RedirectToLogoffPage", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("OwaDeployment.settings.ini", "IsBranded", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("OwaDeployment.settings.ini", "SkipPushNotificationStorageTenantId", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("OwaDeployment.settings.ini", "UseVdirConfigForInstantMessaging", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("OwaDeployment.settings.ini", "RenderPrivacyStatement", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("OwaDeployment.settings.ini", "UseRootDirForAppCacheVdir", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("OwaDeployment.settings.ini", "IsLogonFormatEmail", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("OwaDeployment.settings.ini", "WacConfigurationFromOrgConfig", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("OwaDeployment.settings.ini", "MrsConnectedAccountsSync", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("OwaDeployment.settings.ini", "UsePersistedCapabilities", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("OwaDeployment.settings.ini", "CheckFeatureRestrictions", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("OwaDeployment.settings.ini", "HideInternalUrls", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("OwaDeployment.settings.ini", "IncludeImportContactListButton", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("OwaDeployment.settings.ini", "UseThemeStorageFolder", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("OwaDeployment.settings.ini", "ConnectedAccountsSync", typeof(IFeature), false));
		}

		public VariantConfigurationSection PublicFolderTreePerTenanant
		{
			get
			{
				return base["PublicFolderTreePerTenanant"];
			}
		}

		public VariantConfigurationSection ExplicitLogonAuthFilter
		{
			get
			{
				return base["ExplicitLogonAuthFilter"];
			}
		}

		public VariantConfigurationSection Places
		{
			get
			{
				return base["Places"];
			}
		}

		public VariantConfigurationSection IncludeAccountAccessDisclaimer
		{
			get
			{
				return base["IncludeAccountAccessDisclaimer"];
			}
		}

		public VariantConfigurationSection FilterETag
		{
			get
			{
				return base["FilterETag"];
			}
		}

		public VariantConfigurationSection CacheUMCultures
		{
			get
			{
				return base["CacheUMCultures"];
			}
		}

		public VariantConfigurationSection RedirectToServer
		{
			get
			{
				return base["RedirectToServer"];
			}
		}

		public VariantConfigurationSection UseAccessProxyForInstantMessagingServerName
		{
			get
			{
				return base["UseAccessProxyForInstantMessagingServerName"];
			}
		}

		public VariantConfigurationSection UseBackendVdirConfiguration
		{
			get
			{
				return base["UseBackendVdirConfiguration"];
			}
		}

		public VariantConfigurationSection OneDriveProProviderAvailable
		{
			get
			{
				return base["OneDriveProProviderAvailable"];
			}
		}

		public VariantConfigurationSection LogTenantInfo
		{
			get
			{
				return base["LogTenantInfo"];
			}
		}

		public VariantConfigurationSection RedirectToLogoffPage
		{
			get
			{
				return base["RedirectToLogoffPage"];
			}
		}

		public VariantConfigurationSection IsBranded
		{
			get
			{
				return base["IsBranded"];
			}
		}

		public VariantConfigurationSection SkipPushNotificationStorageTenantId
		{
			get
			{
				return base["SkipPushNotificationStorageTenantId"];
			}
		}

		public VariantConfigurationSection UseVdirConfigForInstantMessaging
		{
			get
			{
				return base["UseVdirConfigForInstantMessaging"];
			}
		}

		public VariantConfigurationSection RenderPrivacyStatement
		{
			get
			{
				return base["RenderPrivacyStatement"];
			}
		}

		public VariantConfigurationSection UseRootDirForAppCacheVdir
		{
			get
			{
				return base["UseRootDirForAppCacheVdir"];
			}
		}

		public VariantConfigurationSection IsLogonFormatEmail
		{
			get
			{
				return base["IsLogonFormatEmail"];
			}
		}

		public VariantConfigurationSection WacConfigurationFromOrgConfig
		{
			get
			{
				return base["WacConfigurationFromOrgConfig"];
			}
		}

		public VariantConfigurationSection MrsConnectedAccountsSync
		{
			get
			{
				return base["MrsConnectedAccountsSync"];
			}
		}

		public VariantConfigurationSection UsePersistedCapabilities
		{
			get
			{
				return base["UsePersistedCapabilities"];
			}
		}

		public VariantConfigurationSection CheckFeatureRestrictions
		{
			get
			{
				return base["CheckFeatureRestrictions"];
			}
		}

		public VariantConfigurationSection HideInternalUrls
		{
			get
			{
				return base["HideInternalUrls"];
			}
		}

		public VariantConfigurationSection IncludeImportContactListButton
		{
			get
			{
				return base["IncludeImportContactListButton"];
			}
		}

		public VariantConfigurationSection UseThemeStorageFolder
		{
			get
			{
				return base["UseThemeStorageFolder"];
			}
		}

		public VariantConfigurationSection ConnectedAccountsSync
		{
			get
			{
				return base["ConnectedAccountsSync"];
			}
		}
	}
}
