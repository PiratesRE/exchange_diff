using System;
using Microsoft.Exchange.Flighting;

namespace Microsoft.Exchange.VariantConfiguration.Reflection
{
	public sealed class VariantConfigurationEacComponent : VariantConfigurationComponent
	{
		internal VariantConfigurationEacComponent() : base("Eac")
		{
			base.Add(new VariantConfigurationSection("Eac.settings.ini", "ManageMailboxAuditing", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("Eac.settings.ini", "UnifiedPolicy", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("Eac.settings.ini", "DiscoverySearchStats", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("Eac.settings.ini", "AllowRemoteOnboardingMovesOnly", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("Eac.settings.ini", "DlpFingerprint", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("Eac.settings.ini", "GeminiShell", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("Eac.settings.ini", "DevicePolicyMgmtUI", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("Eac.settings.ini", "UnifiedAuditPolicy", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("Eac.settings.ini", "EACClientAccessRulesEnabled", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("Eac.settings.ini", "RemoteDomain", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("Eac.settings.ini", "CmdletLogging", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("Eac.settings.ini", "UnifiedComplianceCenter", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("Eac.settings.ini", "Office365DIcon", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("Eac.settings.ini", "DiscoveryPFSearch", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("Eac.settings.ini", "ModernGroups", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("Eac.settings.ini", "OrgIdADSeverSettings", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("Eac.settings.ini", "UCCPermissions", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("Eac.settings.ini", "AllowMailboxArchiveOnlyMigration", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("Eac.settings.ini", "DiscoveryDocIdHint", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("Eac.settings.ini", "AdminHomePage", typeof(IUrl), false));
			base.Add(new VariantConfigurationSection("Eac.settings.ini", "CrossPremiseMigration", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("Eac.settings.ini", "UCCAuditReports", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("Eac.settings.ini", "UnlistedServices", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("Eac.settings.ini", "BulkPermissionAddRemove", typeof(IFeature), false));
		}

		public VariantConfigurationSection ManageMailboxAuditing
		{
			get
			{
				return base["ManageMailboxAuditing"];
			}
		}

		public VariantConfigurationSection UnifiedPolicy
		{
			get
			{
				return base["UnifiedPolicy"];
			}
		}

		public VariantConfigurationSection DiscoverySearchStats
		{
			get
			{
				return base["DiscoverySearchStats"];
			}
		}

		public VariantConfigurationSection AllowRemoteOnboardingMovesOnly
		{
			get
			{
				return base["AllowRemoteOnboardingMovesOnly"];
			}
		}

		public VariantConfigurationSection DlpFingerprint
		{
			get
			{
				return base["DlpFingerprint"];
			}
		}

		public VariantConfigurationSection GeminiShell
		{
			get
			{
				return base["GeminiShell"];
			}
		}

		public VariantConfigurationSection DevicePolicyMgmtUI
		{
			get
			{
				return base["DevicePolicyMgmtUI"];
			}
		}

		public VariantConfigurationSection UnifiedAuditPolicy
		{
			get
			{
				return base["UnifiedAuditPolicy"];
			}
		}

		public VariantConfigurationSection EACClientAccessRulesEnabled
		{
			get
			{
				return base["EACClientAccessRulesEnabled"];
			}
		}

		public VariantConfigurationSection RemoteDomain
		{
			get
			{
				return base["RemoteDomain"];
			}
		}

		public VariantConfigurationSection CmdletLogging
		{
			get
			{
				return base["CmdletLogging"];
			}
		}

		public VariantConfigurationSection UnifiedComplianceCenter
		{
			get
			{
				return base["UnifiedComplianceCenter"];
			}
		}

		public VariantConfigurationSection Office365DIcon
		{
			get
			{
				return base["Office365DIcon"];
			}
		}

		public VariantConfigurationSection DiscoveryPFSearch
		{
			get
			{
				return base["DiscoveryPFSearch"];
			}
		}

		public VariantConfigurationSection ModernGroups
		{
			get
			{
				return base["ModernGroups"];
			}
		}

		public VariantConfigurationSection OrgIdADSeverSettings
		{
			get
			{
				return base["OrgIdADSeverSettings"];
			}
		}

		public VariantConfigurationSection UCCPermissions
		{
			get
			{
				return base["UCCPermissions"];
			}
		}

		public VariantConfigurationSection AllowMailboxArchiveOnlyMigration
		{
			get
			{
				return base["AllowMailboxArchiveOnlyMigration"];
			}
		}

		public VariantConfigurationSection DiscoveryDocIdHint
		{
			get
			{
				return base["DiscoveryDocIdHint"];
			}
		}

		public VariantConfigurationSection AdminHomePage
		{
			get
			{
				return base["AdminHomePage"];
			}
		}

		public VariantConfigurationSection CrossPremiseMigration
		{
			get
			{
				return base["CrossPremiseMigration"];
			}
		}

		public VariantConfigurationSection UCCAuditReports
		{
			get
			{
				return base["UCCAuditReports"];
			}
		}

		public VariantConfigurationSection UnlistedServices
		{
			get
			{
				return base["UnlistedServices"];
			}
		}

		public VariantConfigurationSection BulkPermissionAddRemove
		{
			get
			{
				return base["BulkPermissionAddRemove"];
			}
		}
	}
}
