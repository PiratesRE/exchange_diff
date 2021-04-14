using System;
using Microsoft.Exchange.Calendar;

namespace Microsoft.Exchange.VariantConfiguration.Reflection
{
	public sealed class VariantConfigurationDataStorageComponent : VariantConfigurationComponent
	{
		internal VariantConfigurationDataStorageComponent() : base("DataStorage")
		{
			base.Add(new VariantConfigurationSection("DataStorage.settings.ini", "CheckForRemoteConnections", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("DataStorage.settings.ini", "PeopleCentricConversation", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("DataStorage.settings.ini", "UseOfflineRms", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("DataStorage.settings.ini", "CalendarUpgrade", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("DataStorage.settings.ini", "IgnoreInessentialMetaDataLoadErrors", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("DataStorage.settings.ini", "ModernMailInfra", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("DataStorage.settings.ini", "CalendarView", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("DataStorage.settings.ini", "GroupsForOlkDesktop", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("DataStorage.settings.ini", "FindOrgMailboxInMultiTenantEnvironment", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("DataStorage.settings.ini", "DeleteGroupConversation", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("DataStorage.settings.ini", "ModernConversationPrep", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("DataStorage.settings.ini", "CheckLicense", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("DataStorage.settings.ini", "LoadHostedMailboxLimits", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("DataStorage.settings.ini", "RepresentRemoteMailbox", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("DataStorage.settings.ini", "CalendarUpgradeSettings", typeof(ICalendarUpgradeSettings), false));
			base.Add(new VariantConfigurationSection("DataStorage.settings.ini", "CrossPremiseDelegate", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("DataStorage.settings.ini", "CalendarIcalConversionSettings", typeof(ICalendarIcalConversionSettings), false));
			base.Add(new VariantConfigurationSection("DataStorage.settings.ini", "CalendarViewPropertyRule", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("DataStorage.settings.ini", "CheckR3Coexistence", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("DataStorage.settings.ini", "XOWAConsumerSharing", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("DataStorage.settings.ini", "UserConfigurationAggregation", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("DataStorage.settings.ini", "StorageAttachmentImageAnalysis", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("DataStorage.settings.ini", "LogIpEndpoints", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("DataStorage.settings.ini", "CheckExternalAccess", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("DataStorage.settings.ini", "ThreadedConversation", typeof(IFeature), false));
		}

		public VariantConfigurationSection CheckForRemoteConnections
		{
			get
			{
				return base["CheckForRemoteConnections"];
			}
		}

		public VariantConfigurationSection PeopleCentricConversation
		{
			get
			{
				return base["PeopleCentricConversation"];
			}
		}

		public VariantConfigurationSection UseOfflineRms
		{
			get
			{
				return base["UseOfflineRms"];
			}
		}

		public VariantConfigurationSection CalendarUpgrade
		{
			get
			{
				return base["CalendarUpgrade"];
			}
		}

		public VariantConfigurationSection IgnoreInessentialMetaDataLoadErrors
		{
			get
			{
				return base["IgnoreInessentialMetaDataLoadErrors"];
			}
		}

		public VariantConfigurationSection ModernMailInfra
		{
			get
			{
				return base["ModernMailInfra"];
			}
		}

		public VariantConfigurationSection CalendarView
		{
			get
			{
				return base["CalendarView"];
			}
		}

		public VariantConfigurationSection GroupsForOlkDesktop
		{
			get
			{
				return base["GroupsForOlkDesktop"];
			}
		}

		public VariantConfigurationSection FindOrgMailboxInMultiTenantEnvironment
		{
			get
			{
				return base["FindOrgMailboxInMultiTenantEnvironment"];
			}
		}

		public VariantConfigurationSection DeleteGroupConversation
		{
			get
			{
				return base["DeleteGroupConversation"];
			}
		}

		public VariantConfigurationSection ModernConversationPrep
		{
			get
			{
				return base["ModernConversationPrep"];
			}
		}

		public VariantConfigurationSection CheckLicense
		{
			get
			{
				return base["CheckLicense"];
			}
		}

		public VariantConfigurationSection LoadHostedMailboxLimits
		{
			get
			{
				return base["LoadHostedMailboxLimits"];
			}
		}

		public VariantConfigurationSection RepresentRemoteMailbox
		{
			get
			{
				return base["RepresentRemoteMailbox"];
			}
		}

		public VariantConfigurationSection CalendarUpgradeSettings
		{
			get
			{
				return base["CalendarUpgradeSettings"];
			}
		}

		public VariantConfigurationSection CrossPremiseDelegate
		{
			get
			{
				return base["CrossPremiseDelegate"];
			}
		}

		public VariantConfigurationSection CalendarIcalConversionSettings
		{
			get
			{
				return base["CalendarIcalConversionSettings"];
			}
		}

		public VariantConfigurationSection CalendarViewPropertyRule
		{
			get
			{
				return base["CalendarViewPropertyRule"];
			}
		}

		public VariantConfigurationSection CheckR3Coexistence
		{
			get
			{
				return base["CheckR3Coexistence"];
			}
		}

		public VariantConfigurationSection XOWAConsumerSharing
		{
			get
			{
				return base["XOWAConsumerSharing"];
			}
		}

		public VariantConfigurationSection UserConfigurationAggregation
		{
			get
			{
				return base["UserConfigurationAggregation"];
			}
		}

		public VariantConfigurationSection StorageAttachmentImageAnalysis
		{
			get
			{
				return base["StorageAttachmentImageAnalysis"];
			}
		}

		public VariantConfigurationSection LogIpEndpoints
		{
			get
			{
				return base["LogIpEndpoints"];
			}
		}

		public VariantConfigurationSection CheckExternalAccess
		{
			get
			{
				return base["CheckExternalAccess"];
			}
		}

		public VariantConfigurationSection ThreadedConversation
		{
			get
			{
				return base["ThreadedConversation"];
			}
		}
	}
}
