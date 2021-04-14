using System;
using Microsoft.Exchange.AirSync;

namespace Microsoft.Exchange.VariantConfiguration.Reflection
{
	public sealed class VariantConfigurationActiveSyncComponent : VariantConfigurationComponent
	{
		internal VariantConfigurationActiveSyncComponent() : base("ActiveSync")
		{
			base.Add(new VariantConfigurationSection("ActiveSync.settings.ini", "SyncStateOnDirectItems", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("ActiveSync.settings.ini", "MdmSupportedPlatforms", typeof(IMdmSupportedPlatformsSettings), true));
			base.Add(new VariantConfigurationSection("ActiveSync.settings.ini", "GlobalCriminalCompliance", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("ActiveSync.settings.ini", "ConsumerOrganizationUser", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("ActiveSync.settings.ini", "HDPhotos", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("ActiveSync.settings.ini", "MailboxLoggingVerboseMode", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("ActiveSync.settings.ini", "ActiveSyncClientAccessRulesEnabled", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("ActiveSync.settings.ini", "ForceSingleNameSpaceUsage", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("ActiveSync.settings.ini", "MdmNotification", typeof(IMdmNotificationSettings), true));
			base.Add(new VariantConfigurationSection("ActiveSync.settings.ini", "ActiveSyncDiagnosticsLogABQPeriodicEvent", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("ActiveSync.settings.ini", "RedirectForOnBoarding", typeof(IFeature), true));
			base.Add(new VariantConfigurationSection("ActiveSync.settings.ini", "CloudMdmEnrolled", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("ActiveSync.settings.ini", "UseOAuthMasterSidForSecurityContext", typeof(IFeature), true));
			base.Add(new VariantConfigurationSection("ActiveSync.settings.ini", "EnableV160", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("ActiveSync.settings.ini", "EasPartialIcsSync", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("ActiveSync.settings.ini", "DisableCharsetDetectionInCopyMessageContents", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("ActiveSync.settings.ini", "GetGoidFromCalendarItemForMeetingResponse", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("ActiveSync.settings.ini", "SyncStatusOnGlobalInfo", typeof(IFeature), false));
		}

		public VariantConfigurationSection SyncStateOnDirectItems
		{
			get
			{
				return base["SyncStateOnDirectItems"];
			}
		}

		public VariantConfigurationSection MdmSupportedPlatforms
		{
			get
			{
				return base["MdmSupportedPlatforms"];
			}
		}

		public VariantConfigurationSection GlobalCriminalCompliance
		{
			get
			{
				return base["GlobalCriminalCompliance"];
			}
		}

		public VariantConfigurationSection ConsumerOrganizationUser
		{
			get
			{
				return base["ConsumerOrganizationUser"];
			}
		}

		public VariantConfigurationSection HDPhotos
		{
			get
			{
				return base["HDPhotos"];
			}
		}

		public VariantConfigurationSection MailboxLoggingVerboseMode
		{
			get
			{
				return base["MailboxLoggingVerboseMode"];
			}
		}

		public VariantConfigurationSection ActiveSyncClientAccessRulesEnabled
		{
			get
			{
				return base["ActiveSyncClientAccessRulesEnabled"];
			}
		}

		public VariantConfigurationSection ForceSingleNameSpaceUsage
		{
			get
			{
				return base["ForceSingleNameSpaceUsage"];
			}
		}

		public VariantConfigurationSection MdmNotification
		{
			get
			{
				return base["MdmNotification"];
			}
		}

		public VariantConfigurationSection ActiveSyncDiagnosticsLogABQPeriodicEvent
		{
			get
			{
				return base["ActiveSyncDiagnosticsLogABQPeriodicEvent"];
			}
		}

		public VariantConfigurationSection RedirectForOnBoarding
		{
			get
			{
				return base["RedirectForOnBoarding"];
			}
		}

		public VariantConfigurationSection CloudMdmEnrolled
		{
			get
			{
				return base["CloudMdmEnrolled"];
			}
		}

		public VariantConfigurationSection UseOAuthMasterSidForSecurityContext
		{
			get
			{
				return base["UseOAuthMasterSidForSecurityContext"];
			}
		}

		public VariantConfigurationSection EnableV160
		{
			get
			{
				return base["EnableV160"];
			}
		}

		public VariantConfigurationSection EasPartialIcsSync
		{
			get
			{
				return base["EasPartialIcsSync"];
			}
		}

		public VariantConfigurationSection DisableCharsetDetectionInCopyMessageContents
		{
			get
			{
				return base["DisableCharsetDetectionInCopyMessageContents"];
			}
		}

		public VariantConfigurationSection GetGoidFromCalendarItemForMeetingResponse
		{
			get
			{
				return base["GetGoidFromCalendarItemForMeetingResponse"];
			}
		}

		public VariantConfigurationSection SyncStatusOnGlobalInfo
		{
			get
			{
				return base["SyncStatusOnGlobalInfo"];
			}
		}
	}
}
