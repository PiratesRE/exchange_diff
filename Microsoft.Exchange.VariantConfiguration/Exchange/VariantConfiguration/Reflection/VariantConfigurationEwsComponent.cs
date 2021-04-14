using System;

namespace Microsoft.Exchange.VariantConfiguration.Reflection
{
	public sealed class VariantConfigurationEwsComponent : VariantConfigurationComponent
	{
		internal VariantConfigurationEwsComponent() : base("Ews")
		{
			base.Add(new VariantConfigurationSection("Ews.settings.ini", "AutoSubscribeNewGroupMembers", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("Ews.settings.ini", "OnlineArchive", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("Ews.settings.ini", "UserPasswordExpirationDate", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("Ews.settings.ini", "InstantSearchFoldersForPublicFolders", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("Ews.settings.ini", "LinkedAccountTokenMunging", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("Ews.settings.ini", "EwsServiceCredentials", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("Ews.settings.ini", "ExternalUser", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("Ews.settings.ini", "UseInternalEwsUrlForExtensionEwsProxyInOwa", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("Ews.settings.ini", "EwsClientAccessRulesEnabled", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("Ews.settings.ini", "LongRunningScenarioThrottling", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("Ews.settings.ini", "HttpProxyToCafe", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("Ews.settings.ini", "OData", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("Ews.settings.ini", "EwsHttpHandler", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("Ews.settings.ini", "WsPerformanceCounters", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("Ews.settings.ini", "CreateUnifiedMailbox", typeof(IFeature), false));
		}

		public VariantConfigurationSection AutoSubscribeNewGroupMembers
		{
			get
			{
				return base["AutoSubscribeNewGroupMembers"];
			}
		}

		public VariantConfigurationSection OnlineArchive
		{
			get
			{
				return base["OnlineArchive"];
			}
		}

		public VariantConfigurationSection UserPasswordExpirationDate
		{
			get
			{
				return base["UserPasswordExpirationDate"];
			}
		}

		public VariantConfigurationSection InstantSearchFoldersForPublicFolders
		{
			get
			{
				return base["InstantSearchFoldersForPublicFolders"];
			}
		}

		public VariantConfigurationSection LinkedAccountTokenMunging
		{
			get
			{
				return base["LinkedAccountTokenMunging"];
			}
		}

		public VariantConfigurationSection EwsServiceCredentials
		{
			get
			{
				return base["EwsServiceCredentials"];
			}
		}

		public VariantConfigurationSection ExternalUser
		{
			get
			{
				return base["ExternalUser"];
			}
		}

		public VariantConfigurationSection UseInternalEwsUrlForExtensionEwsProxyInOwa
		{
			get
			{
				return base["UseInternalEwsUrlForExtensionEwsProxyInOwa"];
			}
		}

		public VariantConfigurationSection EwsClientAccessRulesEnabled
		{
			get
			{
				return base["EwsClientAccessRulesEnabled"];
			}
		}

		public VariantConfigurationSection LongRunningScenarioThrottling
		{
			get
			{
				return base["LongRunningScenarioThrottling"];
			}
		}

		public VariantConfigurationSection HttpProxyToCafe
		{
			get
			{
				return base["HttpProxyToCafe"];
			}
		}

		public VariantConfigurationSection OData
		{
			get
			{
				return base["OData"];
			}
		}

		public VariantConfigurationSection EwsHttpHandler
		{
			get
			{
				return base["EwsHttpHandler"];
			}
		}

		public VariantConfigurationSection WsPerformanceCounters
		{
			get
			{
				return base["WsPerformanceCounters"];
			}
		}

		public VariantConfigurationSection CreateUnifiedMailbox
		{
			get
			{
				return base["CreateUnifiedMailbox"];
			}
		}
	}
}
