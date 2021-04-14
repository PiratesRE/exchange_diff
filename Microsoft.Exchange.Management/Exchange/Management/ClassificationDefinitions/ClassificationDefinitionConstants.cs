using System;
using System.Collections.Generic;
using System.Globalization;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;

namespace Microsoft.Exchange.Management.ClassificationDefinitions
{
	internal static class ClassificationDefinitionConstants
	{
		public const string ClassificationDefinitionContainerCn = "ClassificationDefinitions";

		internal const string ClassificationDefinitionNoun = "DataClassification";

		internal const string ClassificationDefinitionNamespacePrefix = "mce";

		public const string ClassificationDefinitionXmlNamespace = "http://schemas.microsoft.com/office/2011/mce";

		internal const string OutOfBoxRulePackageIdPrefix = "00000000";

		internal const string FingerprintRulePackageIdPrefix = "00000000-0000-0000-0001";

		internal const string OobRulePackageEmbeddedResourceFileName = "DefaultClassificationDefinitions.xml";

		internal const string OobEngineConfigResourceFileName = "MceConfig.xml";

		internal const string MceFunctionProcessorElementName = "Function";

		internal const string MceKeywordProcessorElementName = "Keyword";

		internal const string MceRegexProcessorElementName = "Regex";

		internal const string MceFingerprintProcessorElementName = "Fingerprint";

		internal const string MceFingerprintAttributeThreshold = "threshold";

		internal const string MceFingerprintAttributeShingleCount = "shingleCount";

		internal const string MceFingerprintAttributeDescription = "description";

		internal const string MceLangcodeAttributeName = "langcode";

		internal const string MceDefaultAttributeName = "default";

		internal const string MceNameResourceElementName = "Name";

		internal const string MceDescriptionResourceElementName = "Description";

		internal const string MceResourceElementName = "Resource";

		internal const string MceLocalizedStringsElementName = "LocalizedStrings";

		internal const string MceRulesElementName = "Rules";

		internal const string MceVersionElementName = "Version";

		internal const string MceVersionAttributeName = "minEngineVersion";

		internal const string MceIdAttributeName = "id";

		internal const string MceIdRefAttributeName = "idRef";

		internal const string MceRulePackElementName = "RulePack";

		internal const string McePublisherElementName = "Publisher";

		internal const string McePublisherNameElementName = "PublisherName";

		internal const string MceMatchElementName = "Match";

		internal const string MceIdMatchElementName = "IdMatch";

		internal const string MceAnyElementName = "Any";

		internal const string MceMinMatchesAttributeName = "minMatches";

		internal const string MceEntityElementName = "Entity";

		internal const string MceAffinityElementName = "Affinity";

		internal const string MceEvidencesProximityAttributeName = "evidencesProximity";

		internal const string MceThresholdConfidenceLevelAttributeName = "thresholdConfidenceLevel";

		internal const string MceEvidenceElementName = "Evidence";

		internal const string MceConfidenceLevelAttributeName = "confidenceLevel";

		internal const string FingerprintRulePackTemplate = "FingerprintRulePackTemplate.xml";

		internal const string RulePackageNoun = "ClassificationRuleCollection";

		internal const CompareOptions GuidIdentityStringMatchingComparison = CompareOptions.OrdinalIgnoreCase;

		internal const CompareOptions NameIdentityStringMatchingComparison = CompareOptions.IgnoreCase;

		internal static readonly string[] EmbeddedRulePackageSchemaFileNames = new string[]
		{
			"RulePackageTypes.xsd",
			"RulePackage.xsd"
		};

		internal static readonly ISet<string> MceRuleElementNames = new SortedSet<string>
		{
			"Affinity",
			"Entity"
		};

		internal static readonly ISet<string> MceProcessorElementNames = new SortedSet<string>
		{
			"Function",
			"Keyword",
			"Regex",
			"Fingerprint"
		};

		internal static readonly ISet<string> MceIdMatchElementNames = new SortedSet<string>
		{
			"IdMatch"
		};

		internal static readonly ISet<string> MceMatchElementNames = new SortedSet<string>
		{
			"IdMatch",
			"Match"
		};

		internal static readonly ISet<string> MceCustomProcessorElementNames = new SortedSet<string>
		{
			"Keyword",
			"Regex",
			"Fingerprint"
		};

		internal static readonly IEqualityComparer<string> RuleCollectionIdComparer = StringComparer.OrdinalIgnoreCase;

		internal static readonly IEqualityComparer<string> RuleIdComparer = StringComparer.OrdinalIgnoreCase;

		internal static readonly IEqualityComparer<string> TextProcessorIdComparer = StringComparer.Ordinal;

		public static readonly ADObjectId ClassificationDefinitionsRdn = new ADObjectId("CN=ClassificationDefinitions,CN=Rules,CN=Transport Settings");

		internal static readonly char HierarchicalIdentitySeparatorChar = '\\';

		internal static readonly string ExceptionSourcesListKey = "ExceptionSourcesList";

		internal static readonly string MceExecutableFileName = "Mce.dll";

		internal static readonly string OnDiskMceConfigurationDirName = "Configuration";

		internal static readonly string OnDiskMceConfigFileName = "Config.xml";

		internal static ExchangeBuild DefaultVersion = new ExchangeBuild(15, 0, 513, 0);

		internal static ExchangeBuild CompatibleEngineVersion = new ExchangeBuild(15, 0, 780, 0);

		internal static ExchangeBuild NovFunctionCompatibleEngineVersion = new ExchangeBuild(15, 0, 847, 13);

		internal static Dictionary<TextProcessorType, ExchangeBuild> TextProcessorTypeToVersions = new Dictionary<TextProcessorType, ExchangeBuild>
		{
			{
				TextProcessorType.Fingerprint,
				ClassificationDefinitionConstants.CompatibleEngineVersion
			}
		};

		internal static Dictionary<string, ExchangeBuild> FunctionNameToVersions = new Dictionary<string, ExchangeBuild>
		{
			{
				"Func_taiwanese_national_id",
				ClassificationDefinitionConstants.NovFunctionCompatibleEngineVersion
			},
			{
				"Func_pesel_identification_number",
				ClassificationDefinitionConstants.NovFunctionCompatibleEngineVersion
			},
			{
				"Func_polish_national_id",
				ClassificationDefinitionConstants.NovFunctionCompatibleEngineVersion
			},
			{
				"Func_polish_passport_number",
				ClassificationDefinitionConstants.NovFunctionCompatibleEngineVersion
			},
			{
				"Func_finnish_national_id",
				ClassificationDefinitionConstants.NovFunctionCompatibleEngineVersion
			}
		};
	}
}
