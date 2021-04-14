using System;
using System.Linq;
using System.Management.Automation;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	[ObjectScope(new ConfigScopes[]
	{
		ConfigScopes.TenantLocal,
		ConfigScopes.TenantSubTree
	})]
	[Serializable]
	public class DataClassificationConfig : ADConfigurationObject
	{
		internal static object QuotaSettingGetter(ADPropertyDefinition adPropertyDefinition, IPropertyBag propertyBag)
		{
			if (adPropertyDefinition == null)
			{
				throw new ArgumentNullException("adPropertyDefinition");
			}
			MultiValuedProperty<string> multiValuedProperty = (MultiValuedProperty<string>)propertyBag[DataClassificationConfigSchema.DataClassificationConfigQuotaSettings];
			string quotaSettingIdentifier = adPropertyDefinition.Name + ':';
			object result = adPropertyDefinition.DefaultValue;
			if (multiValuedProperty != null && multiValuedProperty.Count > 0)
			{
				string text = multiValuedProperty.FirstOrDefault((string item) => item.StartsWith(quotaSettingIdentifier, StringComparison.Ordinal));
				if (!string.IsNullOrEmpty(text))
				{
					try
					{
						result = ValueConvertor.ConvertValueFromString(text.Substring(quotaSettingIdentifier.Length), adPropertyDefinition.Type, null);
					}
					catch (FormatException ex)
					{
						PropertyValidationError error = new PropertyValidationError(DirectoryStrings.CannotCalculateProperty(adPropertyDefinition.Name, ex.Message), adPropertyDefinition, propertyBag[DataClassificationConfigSchema.DataClassificationConfigQuotaSettings]);
						throw new DataValidationException(error, ex);
					}
				}
			}
			return result;
		}

		internal static void QuotaSettingSetter(ADPropertyDefinition adPropertyDefinition, object quota, IPropertyBag propertyBag)
		{
			if (adPropertyDefinition == null)
			{
				throw new ArgumentNullException("adPropertyDefinition");
			}
			MultiValuedProperty<string> multiValuedProperty = (MultiValuedProperty<string>)propertyBag[DataClassificationConfigSchema.DataClassificationConfigQuotaSettings];
			string text = adPropertyDefinition.Name + ':';
			if (multiValuedProperty != null && multiValuedProperty.Count != 0)
			{
				for (int i = multiValuedProperty.Count - 1; i >= 0; i--)
				{
					if (string.IsNullOrEmpty(multiValuedProperty[i]) || multiValuedProperty[i].StartsWith(text, StringComparison.Ordinal))
					{
						multiValuedProperty.RemoveAt(i);
					}
				}
			}
			if (!object.Equals(quota, adPropertyDefinition.DefaultValue))
			{
				string arg = ValueConvertor.ConvertValueToString(quota, null);
				string item = string.Format("{0}{1}", text, arg);
				multiValuedProperty.Add(item);
			}
			propertyBag[DataClassificationConfigSchema.DataClassificationConfigQuotaSettings] = multiValuedProperty;
		}

		internal override ADObjectSchema Schema
		{
			get
			{
				return DataClassificationConfig.schema;
			}
		}

		internal override ADObjectId ParentPath
		{
			get
			{
				return DataClassificationConfig.parentPath;
			}
		}

		internal override string MostDerivedObjectClass
		{
			get
			{
				return DataClassificationConfig.ldapName;
			}
		}

		internal override ExchangeObjectVersion MaximumSupportedExchangeObjectVersion
		{
			get
			{
				return ExchangeObjectVersion.Exchange2012;
			}
		}

		[Parameter]
		public bool RegExGrammarLimit
		{
			get
			{
				return (bool)this[DataClassificationConfigSchema.RegExGrammarLimit];
			}
			set
			{
				this[DataClassificationConfigSchema.RegExGrammarLimit] = value;
			}
		}

		[Parameter]
		public int DistinctRegExes
		{
			get
			{
				return (int)this[DataClassificationConfigSchema.DistinctRegExes];
			}
			set
			{
				this[DataClassificationConfigSchema.DistinctRegExes] = value;
			}
		}

		[Parameter]
		public int KeywordLength
		{
			get
			{
				return (int)this[DataClassificationConfigSchema.KeywordLength];
			}
			set
			{
				this[DataClassificationConfigSchema.KeywordLength] = value;
			}
		}

		[Parameter]
		public int NumberOfKeywords
		{
			get
			{
				return (int)this[DataClassificationConfigSchema.NumberOfKeywords];
			}
			set
			{
				this[DataClassificationConfigSchema.NumberOfKeywords] = value;
			}
		}

		[Parameter]
		public int DistinctFunctions
		{
			get
			{
				return (int)this[DataClassificationConfigSchema.DistinctFunctions];
			}
			set
			{
				this[DataClassificationConfigSchema.DistinctFunctions] = value;
			}
		}

		[Parameter]
		public int MaxAnyBlocks
		{
			get
			{
				return (int)this[DataClassificationConfigSchema.MaxAnyBlocks];
			}
			set
			{
				this[DataClassificationConfigSchema.MaxAnyBlocks] = value;
			}
		}

		[Parameter]
		public int MaxNestedAnyBlocks
		{
			get
			{
				return (int)this[DataClassificationConfigSchema.MaxNestedAnyBlocks];
			}
			set
			{
				this[DataClassificationConfigSchema.MaxNestedAnyBlocks] = value;
			}
		}

		[Parameter]
		public int RegExLength
		{
			get
			{
				return (int)this[DataClassificationConfigSchema.RegExLength];
			}
			set
			{
				this[DataClassificationConfigSchema.RegExLength] = value;
			}
		}

		[Parameter]
		public ByteQuantifiedSize MaxRulePackageSize
		{
			get
			{
				return (ByteQuantifiedSize)this[DataClassificationConfigSchema.MaxRulePackageSize];
			}
			set
			{
				this[DataClassificationConfigSchema.MaxRulePackageSize] = value;
			}
		}

		[Parameter]
		public int MaxRulePackages
		{
			get
			{
				return (int)this[DataClassificationConfigSchema.MaxRulePackages];
			}
			set
			{
				this[DataClassificationConfigSchema.MaxRulePackages] = value;
			}
		}

		[Parameter]
		public int MaxFingerprints
		{
			get
			{
				return (int)this[DataClassificationConfigSchema.MaxFingerprints];
			}
			set
			{
				this[DataClassificationConfigSchema.MaxFingerprints] = value;
			}
		}

		[Parameter]
		public int FingerprintThreshold
		{
			get
			{
				return (int)this[DataClassificationConfigSchema.FingerprintThreshold];
			}
			set
			{
				this[DataClassificationConfigSchema.FingerprintThreshold] = value;
			}
		}

		private const char Separator = ':';

		internal const string ContainerName = "Default Data Config";

		private static readonly string ldapName = "msExchDataClassificationConfig";

		private static readonly ADObjectId parentPath = new ADObjectId("CN=Data Classification");

		private static readonly DataClassificationConfigSchema schema = ObjectSchema.GetInstance<DataClassificationConfigSchema>();
	}
}
