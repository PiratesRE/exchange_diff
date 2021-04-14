using System;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	internal sealed class ExchangeUpgradeBucketSchema : ADConfigurationObjectSchema
	{
		private const int StartUpgradeStatusBitPosition = 0;

		private const int UpgradeOrganizationMailboxesStatusBitPosition = 2;

		private const int UpgradeUserMailboxesStatusBitPosition = 4;

		private const int CompleteUpgradeStatusBitPosition = 6;

		private const int UpgradeStageStatusBitLength = 2;

		private const int DisabledUpgradeStagesBitPosition = 8;

		private const int DisabledUpgradeStagesBitLength = 4;

		private const string VersionRegex = "^\\d+\\.(\\*|\\d+\\.(\\*|\\d+\\.(\\*|\\d+)))$";

		internal static readonly ADPropertyDefinition Status = new ADPropertyDefinition("Status", ExchangeObjectVersion.Exchange2010, typeof(ExchangeUpgradeBucketStatus), "msExchOrganizationUpgradePolicyStatus", ADPropertyDefinitionFlags.PersistDefaultValue, ExchangeUpgradeBucketStatus.NotStarted, PropertyDefinitionConstraint.None, new PropertyDefinitionConstraint[]
		{
			new EnumValueDefinedConstraint(typeof(ExchangeUpgradeBucketStatus))
		}, null, null);

		internal static readonly ADPropertyDefinition Enabled = new ADPropertyDefinition("Enabled", ExchangeObjectVersion.Exchange2010, typeof(bool), "msExchOrganizationUpgradePolicyEnabled", ADPropertyDefinitionFlags.PersistDefaultValue, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		internal static readonly ADPropertyDefinition StartDate = new ADPropertyDefinition("StartDate", ExchangeObjectVersion.Exchange2010, typeof(DateTime?), "msExchOrganizationUpgradePolicyDate", ADPropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		internal static readonly ADPropertyDefinition MaxMailboxes = new ADPropertyDefinition("MaxMailboxes", ExchangeObjectVersion.Exchange2010, typeof(int?), "msExchOrganizationUpgradePolicyMaxMailboxes", ADPropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, new PropertyDefinitionConstraint[]
		{
			new RangedNullableValueConstraint<int>(1, int.MaxValue)
		}, null, null);

		internal static readonly ADPropertyDefinition Priority = new ADPropertyDefinition("Priority", ExchangeObjectVersion.Exchange2010, typeof(int), "msExchOrganizationUpgradePolicyPriority", ADPropertyDefinitionFlags.Mandatory | ADPropertyDefinitionFlags.PersistDefaultValue, 100, PropertyDefinitionConstraint.None, new PropertyDefinitionConstraint[]
		{
			new RangedValueConstraint<int>(1, 999)
		}, null, null);

		internal static readonly ADPropertyDefinition Description = new ADPropertyDefinition("Description", ExchangeObjectVersion.Exchange2010, typeof(string), "Description", ADPropertyDefinitionFlags.None, string.Empty, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		internal static readonly ADPropertyDefinition RawSourceVersion = new ADPropertyDefinition("RawSourceVersion", ExchangeObjectVersion.Exchange2010, typeof(long), "msExchOrganizationUpgradePolicySourceVersion", ADPropertyDefinitionFlags.Mandatory | ADPropertyDefinitionFlags.PersistDefaultValue, 0L, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		internal static readonly ADPropertyDefinition RawTargetVersion = new ADPropertyDefinition("RawTargetVersion", ExchangeObjectVersion.Exchange2010, typeof(long), "msExchOrganizationUpgradePolicyTargetVersion", ADPropertyDefinitionFlags.Mandatory | ADPropertyDefinitionFlags.PersistDefaultValue, 0L, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		internal static readonly ADPropertyDefinition SourceVersion = new ADPropertyDefinition("SourceVersion", ExchangeObjectVersion.Exchange2010, typeof(string), null, ADPropertyDefinitionFlags.Calculated | ADPropertyDefinitionFlags.Mandatory, string.Empty, PropertyDefinitionConstraint.None, new PropertyDefinitionConstraint[]
		{
			new RegexConstraint("^\\d+\\.(\\*|\\d+\\.(\\*|\\d+\\.(\\*|\\d+)))$", DataStrings.BucketVersionPatternDescription)
		}, new ProviderPropertyDefinition[]
		{
			ExchangeUpgradeBucketSchema.RawSourceVersion
		}, null, new GetterDelegate(ExchangeUpgradeBucket.SourceVersionGetterDelegate), new SetterDelegate(ExchangeUpgradeBucket.SourceVersionSetterDelegate), null, null);

		internal static readonly ADPropertyDefinition TargetVersion = new ADPropertyDefinition("TargetVersion", ExchangeObjectVersion.Exchange2010, typeof(string), null, ADPropertyDefinitionFlags.Calculated | ADPropertyDefinitionFlags.Mandatory, string.Empty, PropertyDefinitionConstraint.None, new PropertyDefinitionConstraint[]
		{
			new RegexConstraint("^\\d+\\.(\\*|\\d+\\.(\\*|\\d+\\.(\\*|\\d+)))$", DataStrings.BucketVersionPatternDescription)
		}, new ProviderPropertyDefinition[]
		{
			ExchangeUpgradeBucketSchema.RawTargetVersion
		}, null, new GetterDelegate(ExchangeUpgradeBucket.TargetVersionGetterDelegate), new SetterDelegate(ExchangeUpgradeBucket.TargetVersionSetterDelegate), null, null);

		public static readonly ADPropertyDefinition Organizations = new ADPropertyDefinition("Organizations", ExchangeObjectVersion.Exchange2010, typeof(ADObjectId), "msExchOrganizationUpgradePolicyBL", ADPropertyDefinitionFlags.ReadOnly | ADPropertyDefinitionFlags.MultiValued | ADPropertyDefinitionFlags.BackLink, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition MailboxCount = new ADPropertyDefinition("MailboxCount", ExchangeObjectVersion.Exchange2010, typeof(int), null, ADPropertyDefinitionFlags.PersistDefaultValue | ADPropertyDefinitionFlags.TaskPopulated, 0, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition DisabledUpgradeStages = ADObject.BitfieldProperty("EnabledUpgradeStage", 8, 4, SharedPropertyDefinitions.ProvisioningFlags);

		public static readonly ADPropertyDefinition StartUpgradeStatus = ADObject.BitfieldProperty("StartUpgradeStatus", 0, 2, SharedPropertyDefinitions.ProvisioningFlags);

		public static readonly ADPropertyDefinition UpgradeOrganizationMailboxesStatus = ADObject.BitfieldProperty("UpgradeOrganizationMailboxesStatus", 2, 2, SharedPropertyDefinitions.ProvisioningFlags);

		public static readonly ADPropertyDefinition UpgradeUserMailboxesStatus = ADObject.BitfieldProperty("UpgradeUserMailboxesStatus", 4, 2, SharedPropertyDefinitions.ProvisioningFlags);

		public static readonly ADPropertyDefinition CompleteUpgradeStatus = ADObject.BitfieldProperty("CompleteUpgradeStatus", 6, 2, SharedPropertyDefinitions.ProvisioningFlags);
	}
}
