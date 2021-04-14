using System;

namespace Microsoft.Exchange.VariantConfiguration.Reflection
{
	public sealed class VariantConfigurationIpaedComponent : VariantConfigurationComponent
	{
		internal VariantConfigurationIpaedComponent() : base("Ipaed")
		{
			base.Add(new VariantConfigurationSection("Ipaed.settings.ini", "ProcessedByUnjournal", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("Ipaed.settings.ini", "ProcessForestWideOrgJournal", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("Ipaed.settings.ini", "MoveDeletionsToPurges", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("Ipaed.settings.ini", "InternalJournaling", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("Ipaed.settings.ini", "IncreaseQuotaForOnHoldMailboxes", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("Ipaed.settings.ini", "AdminAuditLocalQueue", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("Ipaed.settings.ini", "AdminAuditCmdletBlockList", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("Ipaed.settings.ini", "AdminAuditEventLogThrottling", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("Ipaed.settings.ini", "AuditConfigFromUCCPolicy", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("Ipaed.settings.ini", "PartitionedMailboxAuditLogs", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("Ipaed.settings.ini", "MailboxAuditLocalQueue", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("Ipaed.settings.ini", "RemoveMailboxFromJournalRecipients", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("Ipaed.settings.ini", "MoveClearNrn", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("Ipaed.settings.ini", "FolderBindExtendedThrottling", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("Ipaed.settings.ini", "PartitionedAdminAuditLogs", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("Ipaed.settings.ini", "AdminAuditExternalAccessCheckOnDedicated", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("Ipaed.settings.ini", "LegacyJournaling", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("Ipaed.settings.ini", "EHAJournaling", typeof(IFeature), false));
		}

		public VariantConfigurationSection ProcessedByUnjournal
		{
			get
			{
				return base["ProcessedByUnjournal"];
			}
		}

		public VariantConfigurationSection ProcessForestWideOrgJournal
		{
			get
			{
				return base["ProcessForestWideOrgJournal"];
			}
		}

		public VariantConfigurationSection MoveDeletionsToPurges
		{
			get
			{
				return base["MoveDeletionsToPurges"];
			}
		}

		public VariantConfigurationSection InternalJournaling
		{
			get
			{
				return base["InternalJournaling"];
			}
		}

		public VariantConfigurationSection IncreaseQuotaForOnHoldMailboxes
		{
			get
			{
				return base["IncreaseQuotaForOnHoldMailboxes"];
			}
		}

		public VariantConfigurationSection AdminAuditLocalQueue
		{
			get
			{
				return base["AdminAuditLocalQueue"];
			}
		}

		public VariantConfigurationSection AdminAuditCmdletBlockList
		{
			get
			{
				return base["AdminAuditCmdletBlockList"];
			}
		}

		public VariantConfigurationSection AdminAuditEventLogThrottling
		{
			get
			{
				return base["AdminAuditEventLogThrottling"];
			}
		}

		public VariantConfigurationSection AuditConfigFromUCCPolicy
		{
			get
			{
				return base["AuditConfigFromUCCPolicy"];
			}
		}

		public VariantConfigurationSection PartitionedMailboxAuditLogs
		{
			get
			{
				return base["PartitionedMailboxAuditLogs"];
			}
		}

		public VariantConfigurationSection MailboxAuditLocalQueue
		{
			get
			{
				return base["MailboxAuditLocalQueue"];
			}
		}

		public VariantConfigurationSection RemoveMailboxFromJournalRecipients
		{
			get
			{
				return base["RemoveMailboxFromJournalRecipients"];
			}
		}

		public VariantConfigurationSection MoveClearNrn
		{
			get
			{
				return base["MoveClearNrn"];
			}
		}

		public VariantConfigurationSection FolderBindExtendedThrottling
		{
			get
			{
				return base["FolderBindExtendedThrottling"];
			}
		}

		public VariantConfigurationSection PartitionedAdminAuditLogs
		{
			get
			{
				return base["PartitionedAdminAuditLogs"];
			}
		}

		public VariantConfigurationSection AdminAuditExternalAccessCheckOnDedicated
		{
			get
			{
				return base["AdminAuditExternalAccessCheckOnDedicated"];
			}
		}

		public VariantConfigurationSection LegacyJournaling
		{
			get
			{
				return base["LegacyJournaling"];
			}
		}

		public VariantConfigurationSection EHAJournaling
		{
			get
			{
				return base["EHAJournaling"];
			}
		}
	}
}
