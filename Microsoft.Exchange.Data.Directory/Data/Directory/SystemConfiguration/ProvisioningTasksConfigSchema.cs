using System;
using System.Configuration;
using Microsoft.Exchange.Data.ConfigurationSettings;
using Microsoft.Exchange.Diagnostics.Components.Data.Directory;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	internal sealed class ProvisioningTasksConfigSchema : ConfigSchemaBase
	{
		public override string Name
		{
			get
			{
				return "ProvisioningTasks";
			}
		}

		public override string SectionName
		{
			get
			{
				return "GlobalConfiguration";
			}
		}

		[ConfigurationProperty("IsOrganizationSoftDeletionEnabled", DefaultValue = false)]
		public bool IsOrganizationSoftDeletionEnabled
		{
			get
			{
				return (bool)base["IsOrganizationSoftDeletionEnabled"];
			}
			set
			{
				base["IsOrganizationSoftDeletionEnabled"] = value;
			}
		}

		[ConfigurationProperty("IsFailedOrganizationCleanupEnabled", DefaultValue = false)]
		public bool IsFailedOrganizationCleanupEnabled
		{
			get
			{
				return (bool)base["IsFailedOrganizationCleanupEnabled"];
			}
			set
			{
				base["IsFailedOrganizationCleanupEnabled"] = value;
			}
		}

		[ConfigurationProperty("UseBecAPIsforLiveId", DefaultValue = false)]
		public bool UseBecAPIsforLiveId
		{
			get
			{
				return (bool)base["UseBecAPIsforLiveId"];
			}
			set
			{
				base["UseBecAPIsforLiveId"] = value;
			}
		}

		[ConfigurationProperty("MaxObjectFullSyncRequestsPerServiceInstance", DefaultValue = 200)]
		public int MaxObjectFullSyncRequestsPerServiceInstance
		{
			get
			{
				return (int)base["MaxObjectFullSyncRequestsPerServiceInstance"];
			}
			set
			{
				base["MaxObjectFullSyncRequestsPerServiceInstance"] = value;
			}
		}

		[ConfigurationProperty("EnableAutomatedCleaningOfCnfRbacContainer", DefaultValue = false)]
		public bool EnableAutomatedCleaningOfCnfRbacContainer
		{
			get
			{
				return (bool)base["EnableAutomatedCleaningOfCnfRbacContainer"];
			}
			set
			{
				base["EnableAutomatedCleaningOfCnfRbacContainer"] = value;
			}
		}

		[ConfigurationProperty("EnableAutomatedCleaningOfCnfSoftDeletedContainer", DefaultValue = false)]
		public bool EnableAutomatedCleaningOfCnfSoftDeletedContainer
		{
			get
			{
				return (bool)base["EnableAutomatedCleaningOfCnfSoftDeletedContainer"];
			}
			set
			{
				base["EnableAutomatedCleaningOfCnfSoftDeletedContainer"] = value;
			}
		}

		[ConfigurationProperty("EnableAutomatedCleaningOfCnfProvisioningPolicyContainer", DefaultValue = false)]
		public bool EnableAutomatedCleaningOfCnfProvisioningPolicyContainer
		{
			get
			{
				return (bool)base["EnableAutomatedCleaningOfCnfProvisioningPolicyContainer"];
			}
			set
			{
				base["EnableAutomatedCleaningOfCnfProvisioningPolicyContainer"] = value;
			}
		}

		[ConfigurationProperty("EnablePowershellBasedDivergenceProcessor", DefaultValue = false)]
		public bool EnablePowershellBasedDivergenceProcessor
		{
			get
			{
				return (bool)base["EnablePowershellBasedDivergenceProcessor"];
			}
			set
			{
				base["EnablePowershellBasedDivergenceProcessor"] = value;
			}
		}

		[ConfigurationProperty("EnableProcessingMissingLinksInGroupDivergences", DefaultValue = false)]
		public bool EnableProcessingMissingLinksInGroupDivergences
		{
			get
			{
				return (bool)base["EnableProcessingMissingLinksInGroupDivergences"];
			}
			set
			{
				base["EnableProcessingMissingLinksInGroupDivergences"] = value;
			}
		}

		[ConfigurationProperty("EnableProcessingValidationDivergences", DefaultValue = false)]
		public bool EnableProcessingValidationDivergences
		{
			get
			{
				return (bool)base["EnableProcessingValidationDivergences"];
			}
			set
			{
				base["EnableProcessingValidationDivergences"] = value;
			}
		}

		protected override bool OnDeserializeUnrecognizedAttribute(string name, string value)
		{
			ExTraceGlobals.DirectoryTasksTracer.TraceDebug<string, string>(0L, "Unrecognized configuration attribute {0}={1}", name, value);
			return base.OnDeserializeUnrecognizedAttribute(name, value);
		}

		internal static class Setting
		{
			public const string IsOrganizationSoftDeletionEnabled = "IsOrganizationSoftDeletionEnabled";

			public const string IsFailedOrganizationCleanupEnabled = "IsFailedOrganizationCleanupEnabled";

			public const string UseBecAPIsforLiveId = "UseBecAPIsforLiveId";

			public const string MaxObjectFullSyncRequestsPerServiceInstance = "MaxObjectFullSyncRequestsPerServiceInstance";

			public const string EnableAutomatedCleaningOfCnfRbacContainer = "EnableAutomatedCleaningOfCnfRbacContainer";

			public const string EnableAutomatedCleaningOfCnfSoftDeletedContainer = "EnableAutomatedCleaningOfCnfSoftDeletedContainer";

			public const string EnableAutomatedCleaningOfCnfProvisioningPolicyContainer = "EnableAutomatedCleaningOfCnfProvisioningPolicyContainer";

			public const string EnablePowershellBasedDivergenceProcessor = "EnablePowershellBasedDivergenceProcessor";

			public const string EnableProcessingMissingLinksInGroupDivergences = "EnableProcessingMissingLinksInGroupDivergences";

			public const string EnableProcessingValidationDivergences = "EnableProcessingValidationDivergences";
		}
	}
}
