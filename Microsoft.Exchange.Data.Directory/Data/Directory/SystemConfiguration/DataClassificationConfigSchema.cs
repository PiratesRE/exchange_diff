using System;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	internal class DataClassificationConfigSchema : ADConfigurationObjectSchema
	{
		internal const int RegexGrammarLimitDefaultValue = 1;

		internal const int DistinctRegexesDefaultValue = 20;

		internal const int KeywordLengthDefaultValue = 50;

		internal const int NumberOfKeywordDefaultValue = 512;

		internal const int DistinctFunctionsDefaultValue = 10;

		internal const int MaxAnyBlocksDefaultValue = 20;

		internal const int MaxNestedAnyBlocksDefaultValue = 5;

		internal const int RegexLengthDefaultValue = 500;

		internal const int RegExGrammarLimitShift = 0;

		internal const int DistinctRegExesShift = 1;

		internal const int DistinctRegExesLength = 8;

		internal const int KeywordLengthShift = 9;

		internal const int KeywordLengthLength = 9;

		internal const int NumberOfKeywordsShift = 18;

		internal const int NumberOfKeywordsLength = 12;

		internal const int DistinctFunctionsShift = 0;

		internal const int DistinctFunctionsLength = 6;

		internal const int MaxAnyBlocksShift = 6;

		internal const int MaxAnyBlocksLength = 7;

		internal const int MaxNestedAnyBlocksShift = 13;

		internal const int MaxNestedAnyBlocksLength = 4;

		internal const int RegExLengthShift = 17;

		internal const int RegExLengthLength = 11;

		public static readonly ADPropertyDefinition DataClassificationConfigFlags1 = new ADPropertyDefinition("DataClassificationConfigFlags1", ExchangeObjectVersion.Exchange2010, typeof(int), "msExchSpamFlags", ADPropertyDefinitionFlags.PersistDefaultValue, 134243369, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition DataClassificationConfigFlags2 = new ADPropertyDefinition("DataClassificationConfigFlags2", ExchangeObjectVersion.Exchange2010, typeof(int), "msExchMalwareFilterConfigFlags", ADPropertyDefinitionFlags.PersistDefaultValue, 65578250, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition DataClassificationConfigQuotaSettings = new ADPropertyDefinition("DataClassificationConfigQuotaSettings", ExchangeObjectVersion.Exchange2012, typeof(string), "msExchObjectCountQuota", ADPropertyDefinitionFlags.MultiValued, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		internal static readonly RangedValueConstraint<int> DistinctRegexesValidRange = new RangedValueConstraint<int>(0, 200);

		internal static readonly RangedValueConstraint<int> KeywordLengthValidRange = new RangedValueConstraint<int>(0, 100);

		internal static readonly RangedValueConstraint<int> NumberOfKeywordValidRange = new RangedValueConstraint<int>(0, 2048);

		public static readonly ADPropertyDefinition RegExGrammarLimit = ADObject.BitfieldProperty("RegExGrammarLimit", 0, DataClassificationConfigSchema.DataClassificationConfigFlags1);

		public static readonly ADPropertyDefinition DistinctRegExes = ADObject.BitfieldProperty("DistinctRegExes", 1, 8, DataClassificationConfigSchema.DataClassificationConfigFlags1, DataClassificationConfigSchema.DistinctRegexesValidRange);

		public static readonly ADPropertyDefinition KeywordLength = ADObject.BitfieldProperty("KeywordLength", 9, 9, DataClassificationConfigSchema.DataClassificationConfigFlags1, DataClassificationConfigSchema.KeywordLengthValidRange);

		public static readonly ADPropertyDefinition NumberOfKeywords = ADObject.BitfieldProperty("NumberOfKeywords", 18, 12, DataClassificationConfigSchema.DataClassificationConfigFlags1, DataClassificationConfigSchema.NumberOfKeywordValidRange);

		internal static readonly RangedValueConstraint<int> DistinctFunctionsValidRange = new RangedValueConstraint<int>(0, 50);

		internal static readonly RangedValueConstraint<int> MaxAnyBlocksValidRange = new RangedValueConstraint<int>(0, 100);

		internal static readonly RangedValueConstraint<int> MaxNestedAnyBlocksValidRange = new RangedValueConstraint<int>(0, 10);

		internal static readonly RangedValueConstraint<int> RegexLengthValidRange = new RangedValueConstraint<int>(0, 1024);

		public static readonly ADPropertyDefinition DistinctFunctions = ADObject.BitfieldProperty("DistinctFunctions", 0, 6, DataClassificationConfigSchema.DataClassificationConfigFlags2, DataClassificationConfigSchema.DistinctFunctionsValidRange);

		public static readonly ADPropertyDefinition MaxAnyBlocks = ADObject.BitfieldProperty("MaxAnyBlocks", 6, 7, DataClassificationConfigSchema.DataClassificationConfigFlags2, DataClassificationConfigSchema.MaxAnyBlocksValidRange);

		public static readonly ADPropertyDefinition MaxNestedAnyBlocks = ADObject.BitfieldProperty("MaxNestedAnyBlocks", 13, 4, DataClassificationConfigSchema.DataClassificationConfigFlags2, DataClassificationConfigSchema.MaxNestedAnyBlocksValidRange);

		public static readonly ADPropertyDefinition RegExLength = ADObject.BitfieldProperty("RegExLength", 17, 11, DataClassificationConfigSchema.DataClassificationConfigFlags2, DataClassificationConfigSchema.RegexLengthValidRange);

		public static readonly ADPropertyDefinition MaxRulePackageSize = new ADPropertyDefinition("MaxRulePackageSize", ExchangeObjectVersion.Exchange2012, typeof(ByteQuantifiedSize), null, ADPropertyDefinitionFlags.Calculated, ByteQuantifiedSize.FromKB(150UL), new PropertyDefinitionConstraint[]
		{
			new RangedValueConstraint<ByteQuantifiedSize>(ByteQuantifiedSize.FromKB(0UL), ByteQuantifiedSize.FromKB(500UL))
		}, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			DataClassificationConfigSchema.DataClassificationConfigQuotaSettings
		}, null, (IPropertyBag propertyBag) => DataClassificationConfig.QuotaSettingGetter(DataClassificationConfigSchema.MaxRulePackageSize, propertyBag), delegate(object value, IPropertyBag propertyBag)
		{
			DataClassificationConfig.QuotaSettingSetter(DataClassificationConfigSchema.MaxRulePackageSize, value, propertyBag);
		}, null, null);

		public static readonly ADPropertyDefinition MaxRulePackages = new ADPropertyDefinition("MaxRulePackages", ExchangeObjectVersion.Exchange2012, typeof(int), null, ADPropertyDefinitionFlags.Calculated, 10, new PropertyDefinitionConstraint[]
		{
			new RangedValueConstraint<int>(0, 350)
		}, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			DataClassificationConfigSchema.DataClassificationConfigQuotaSettings
		}, null, (IPropertyBag propertyBag) => DataClassificationConfig.QuotaSettingGetter(DataClassificationConfigSchema.MaxRulePackages, propertyBag), delegate(object value, IPropertyBag propertyBag)
		{
			DataClassificationConfig.QuotaSettingSetter(DataClassificationConfigSchema.MaxRulePackages, value, propertyBag);
		}, null, null);

		public static readonly ADPropertyDefinition MaxFingerprints = new ADPropertyDefinition("MaxFingerprints", ExchangeObjectVersion.Exchange2012, typeof(int), null, ADPropertyDefinitionFlags.Calculated, 100, new PropertyDefinitionConstraint[]
		{
			new RangedValueConstraint<int>(0, 500)
		}, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			DataClassificationConfigSchema.DataClassificationConfigQuotaSettings
		}, null, (IPropertyBag propertyBag) => DataClassificationConfig.QuotaSettingGetter(DataClassificationConfigSchema.MaxFingerprints, propertyBag), delegate(object value, IPropertyBag propertyBag)
		{
			DataClassificationConfig.QuotaSettingSetter(DataClassificationConfigSchema.MaxFingerprints, value, propertyBag);
		}, null, null);

		public static readonly ADPropertyDefinition FingerprintThreshold = new ADPropertyDefinition("FingerprintThreshold", ExchangeObjectVersion.Exchange2012, typeof(int), null, ADPropertyDefinitionFlags.Calculated, 50, new PropertyDefinitionConstraint[]
		{
			new RangedValueConstraint<int>(1, 100)
		}, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			DataClassificationConfigSchema.DataClassificationConfigQuotaSettings
		}, null, (IPropertyBag propertyBag) => DataClassificationConfig.QuotaSettingGetter(DataClassificationConfigSchema.FingerprintThreshold, propertyBag), delegate(object value, IPropertyBag propertyBag)
		{
			DataClassificationConfig.QuotaSettingSetter(DataClassificationConfigSchema.FingerprintThreshold, value, propertyBag);
		}, null, null);
	}
}
