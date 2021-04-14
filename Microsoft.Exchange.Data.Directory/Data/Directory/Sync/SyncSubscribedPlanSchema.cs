using System;

namespace Microsoft.Exchange.Data.Directory.Sync
{
	internal class SyncSubscribedPlanSchema : SyncObjectSchema
	{
		public override DirectoryObjectClass DirectoryObjectClass
		{
			get
			{
				return DirectoryObjectClass.SubscribedPlan;
			}
		}

		public static SyncPropertyDefinition AccountId = new SyncPropertyDefinition("AccountId", "AccountId", typeof(string), typeof(DirectoryPropertyGuidSingle), SyncPropertyDefinitionFlags.ForwardSync | SyncPropertyDefinitionFlags.FilteringOnly, SyncPropertyDefinition.InitialSyncPropertySetVersion, string.Empty);

		public static SyncPropertyDefinition Capability = new SyncPropertyDefinition("Capability", "Capability", typeof(string), typeof(DirectoryPropertyXmlAnySingle), SyncPropertyDefinitionFlags.ForwardSync | SyncPropertyDefinitionFlags.FilteringOnly, SyncPropertyDefinition.InitialSyncPropertySetVersion, string.Empty);

		public static SyncPropertyDefinition ServiceType = new SyncPropertyDefinition("ServiceType", "ServiceType", typeof(string), typeof(DirectoryPropertyStringSingleLength1To256), SyncPropertyDefinitionFlags.ForwardSync | SyncPropertyDefinitionFlags.FilteringOnly, SyncPropertyDefinition.InitialSyncPropertySetVersion, string.Empty);

		public static SyncPropertyDefinition MaximumOverageUnitsDetail = new SyncPropertyDefinition("MaximumOverageUnitsDetail", "MaximumOverageUnitsDetail", typeof(string), typeof(DirectoryPropertyXmlLicenseUnitsDetailSingle), SyncPropertyDefinitionFlags.ForwardSync | SyncPropertyDefinitionFlags.FilteringOnly, SyncPropertyDefinition.InitialSyncPropertySetVersion, string.Empty);

		public static SyncPropertyDefinition PrepaidUnitsDetail = new SyncPropertyDefinition("PrepaidUnitsDetail", "PrepaidUnitsDetail", typeof(string), typeof(DirectoryPropertyXmlLicenseUnitsDetailSingle), SyncPropertyDefinitionFlags.ForwardSync | SyncPropertyDefinitionFlags.FilteringOnly, SyncPropertyDefinition.InitialSyncPropertySetVersion, string.Empty);

		public static SyncPropertyDefinition TotalTrialUnitsDetail = new SyncPropertyDefinition("TotalTrialUnitsDetail", "TotalTrialUnitsDetail", typeof(string), typeof(DirectoryPropertyXmlLicenseUnitsDetailSingle), SyncPropertyDefinitionFlags.ForwardSync | SyncPropertyDefinitionFlags.FilteringOnly, SyncPropertyDefinition.InitialSyncPropertySetVersion, string.Empty);
	}
}
