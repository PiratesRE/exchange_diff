using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using Microsoft.Exchange.Flighting;
using Microsoft.Exchange.VariantConfiguration.DataLoad;
using Microsoft.Exchange.VariantConfiguration.Parser;
using Microsoft.Search.Platform.Parallax;

namespace Microsoft.Exchange.VariantConfiguration
{
	internal class VariantConfigurationValidator
	{
		internal VariantConfigurationValidator(VariantConfigurationTypeResolver typeResolver)
		{
			this.TypeResolver = typeResolver;
		}

		internal VariantConfigurationTypeResolver TypeResolver { get; private set; }

		public static void Validate(IEnumerable<string> dataSourcePaths)
		{
			VariantConfigurationValidator.Validate(dataSourcePaths, Assembly.GetExecutingAssembly(), VariantType.Variants);
		}

		internal static void Validate(IEnumerable<string> dataSourcePaths, Assembly typesAssembly, VariantTypeCollection variantTypes)
		{
			ConfigurationParser parser = ConfigurationParser.Create(dataSourcePaths);
			VariantConfigurationTypeResolver typeResolver = VariantConfigurationTypeResolver.Create(typesAssembly);
			VariantConfigurationValidator variantConfigurationValidator = new VariantConfigurationValidator(typeResolver);
			variantConfigurationValidator.Validate(parser, variantTypes);
			VariantConfigurationValidator.ValidateLoadConfigurationFiles(dataSourcePaths);
		}

		internal static void ValidateLoadConfigurationFiles(IEnumerable<string> dataSourcePaths)
		{
			VariantObjectStoreContainer variantObjectStoreContainer = VariantObjectStoreContainerFactory.Default.Create();
			OverrideDataTransformation transformation = OverrideDataTransformation.Get(null);
			ChainedDataSourceReader chainedDataSourceReader = new ChainedDataSourceReader();
			IEnumerable<string> enumerable = (from path in dataSourcePaths
			select Path.GetDirectoryName(path)).Distinct(StringComparer.OrdinalIgnoreCase);
			foreach (string directory in enumerable)
			{
				chainedDataSourceReader.AddDataSourceReader(new FilesDataSourceReader(directory));
			}
			VariantConfigurationDataLoader variantConfigurationDataLoader = new VariantConfigurationDataLoader(chainedDataSourceReader, transformation, from path in dataSourcePaths
			select Path.GetFileName(path));
			variantObjectStoreContainer.RegisterDataLoader(variantConfigurationDataLoader);
		}

		internal static bool IsHashBucketValueParsable(string value)
		{
			return !string.IsNullOrEmpty(value) && VariantConfigurationValidator.HashBucketValueValidationRegex.IsMatch(value.Trim());
		}

		internal void Validate(ConfigurationParser parser, VariantTypeCollection variantTypes)
		{
			HashSet<string> hashSet = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
			foreach (ConfigurationComponent configurationComponent in parser.TeamComponents)
			{
				foreach (ConfigurationSection configurationSection in configurationComponent.Sections)
				{
					if (!string.Equals(configurationSection.TypeName, "Microsoft.Exchange.Flighting.ITeam"))
					{
						throw new VariantConfigurationConventionException(string.Format("'{0}' contains a section that is not of type Microsoft.Exchange.Flighting.ITeam", configurationComponent.DataSourceName));
					}
					this.Validate(configurationSection, configurationComponent.DataSourceName);
					VariantConfigurationValidator.ValidateVariants(configurationSection, configurationComponent.DataSourceName, new VariantTypeFlags[]
					{
						VariantTypeFlags.AllowedInTeams
					}, hashSet, variantTypes);
					hashSet.Add("team." + configurationSection.SectionName);
				}
			}
			foreach (ConfigurationComponent configurationComponent2 in parser.FlightComponents)
			{
				foreach (ConfigurationSection configurationSection2 in configurationComponent2.Sections)
				{
					if (!string.Equals(configurationSection2.TypeName, "Microsoft.Exchange.Flighting.IFlight"))
					{
						throw new VariantConfigurationConventionException(string.Format("Flight.ini file '{0}' contains a section that is not of type Microsoft.Exchange.Flighting.IFlight", configurationComponent2.DataSourceName));
					}
					this.Validate(configurationSection2, configurationComponent2.DataSourceName);
					VariantConfigurationValidator.ValidateVariants(configurationSection2, configurationComponent2.DataSourceName, new VariantTypeFlags[]
					{
						VariantTypeFlags.AllowedInFlights
					}, hashSet, variantTypes);
					hashSet.Add("flt." + configurationSection2.SectionName);
				}
			}
			foreach (ConfigurationComponent configurationComponent3 in parser.SettingsComponents)
			{
				foreach (ConfigurationSection configurationSection3 in configurationComponent3.Sections)
				{
					if (string.Equals(configurationSection3.TypeName, "Microsoft.Exchange.Flighting.IFlight", StringComparison.OrdinalIgnoreCase) || string.Equals(configurationSection3.TypeName, "Microsoft.Exchange.Flighting.ITeam", StringComparison.OrdinalIgnoreCase))
					{
						throw new VariantConfigurationConventionException(string.Format("Settings.ini file '{0}' should not contains a section that is of type Microsoft.Exchange.Flighting.IFlight or Microsoft.Exchange.Flighting.ITeam.", configurationComponent3.DataSourceName));
					}
					this.Validate(configurationSection3, configurationComponent3.DataSourceName);
					VariantConfigurationValidator.ValidateVariants(configurationSection3, configurationComponent3.DataSourceName, new VariantTypeFlags[]
					{
						VariantTypeFlags.AllowedInSettings
					}, hashSet, variantTypes);
				}
			}
		}

		internal void Validate(ConfigurationSection section, string dataSourceName)
		{
			Type type = this.TypeResolver.ResolveType(section.TypeName);
			if (type == null)
			{
				throw new TypeNotFoundException(string.Format("Type with name '{0}' is not found in section '{1}' in data source '{2}'. Please check spelling and that necessary assembly is loaded in the AppDomain.", section.TypeName, section.SectionName, dataSourceName));
			}
			VariantConfigurationTypeInformation typeInformation = this.TypeResolver.GetTypeInformation(type);
			foreach (ConfigurationParameter parameter in section.Parameters)
			{
				this.Validate(parameter, typeInformation, section.SectionName, dataSourceName);
			}
		}

		internal void Validate(ConfigurationParameter parameter, VariantConfigurationTypeInformation configTypeInfo, string sectionName, string dataSourceName)
		{
			if (!configTypeInfo.IsValidPropertyName(parameter.Name))
			{
				throw new VariantConfigurationSyntaxException(string.Format("Section '{0}' in data source '{1}' contains property '{2}', which is not a valid property for type '{3}', valid properties: '{4}'.", new object[]
				{
					sectionName,
					dataSourceName,
					parameter.Name,
					configTypeInfo.Type.Name,
					configTypeInfo.ToString()
				}));
			}
			if (!configTypeInfo.IsValidPropertyValue(parameter.Name, parameter.Value))
			{
				throw new VariantConfigurationSyntaxException(string.Format("'{0}' is not a valid value for property '{1}' in section '{2}' of data source '{3}'.", new object[]
				{
					parameter.Value,
					parameter.Name,
					sectionName,
					dataSourceName
				}));
			}
			if (configTypeInfo.Type == typeof(IFlight) && (string.Equals(parameter.Name, "Rotate", StringComparison.OrdinalIgnoreCase) || string.Equals(parameter.Name, "Ramp", StringComparison.OrdinalIgnoreCase)) && !VariantConfigurationValidator.IsHashBucketValueParsable(parameter.Value))
			{
				throw new VariantConfigurationSyntaxException(string.Format("'{0}' is not a parsable value for property '{1}' in section '{2}' of data source '{3}'.", new object[]
				{
					parameter.Value,
					parameter.Name,
					sectionName,
					dataSourceName
				}));
			}
		}

		private static void ValidateVariants(ConfigurationSection section, string componentName, IEnumerable<VariantTypeFlags> necessaryVariantTypeFlags, ISet<string> validPrefixVariants, VariantTypeCollection variantTypeCollection)
		{
			foreach (ConfigurationParameter configurationParameter in section.Parameters)
			{
				foreach (KeyValuePair<string, string> keyValuePair in configurationParameter.Variants)
				{
					if (!variantTypeCollection.Contains(keyValuePair.Key, true))
					{
						throw new VariantConfigurationConventionException(string.Format("Section '{0}' in datasource '{1}' contains a variant '{2}', which does not exist.", section.SectionName, componentName, keyValuePair.Key));
					}
					VariantType variantByName = variantTypeCollection.GetVariantByName(keyValuePair.Key, true);
					if (!variantByName.ValidateValue(keyValuePair.Value))
					{
						throw new VariantConfigurationSyntaxException(string.Format("Variant '{0}' in section '{1}' in datasource '{2}' has an invalid value.", keyValuePair.Key, section.SectionName, componentName));
					}
					foreach (VariantTypeFlags variantTypeFlags in necessaryVariantTypeFlags)
					{
						if (!variantByName.Flags.HasFlag(variantTypeFlags))
						{
							throw new VariantConfigurationConventionException(string.Format("Section '{0}' in datasource '{1}' contains a variant '{2}', which is either not allowed in this type of file or does not exist.", section.SectionName, componentName, keyValuePair.Key));
						}
					}
					if (variantByName.Flags.HasFlag(VariantTypeFlags.Prefix) && !validPrefixVariants.Contains(keyValuePair.Key))
					{
						throw new VariantConfigurationConventionException(string.Format("Section '{0}' in datasource '{1}' contains a calculated variant '{2}', which is either not allowed in this type of file or does not exist.", section.SectionName, componentName, keyValuePair.Key));
					}
				}
			}
		}

		private const string HashPercentageRangeRegexString = "(?:100|[1-9]?\\d)%";

		private const string ZeroToNineHundredNinetyNineRegexString = "(?:0|[1-9]\\d{0,2})";

		private const string NoRotateValue = "norotate";

		private static readonly string HashBucketRangeRegexString = string.Format("(?:{0},{0})", "(?:0|[1-9]\\d{0,2})");

		private static readonly Regex HashBucketValueValidationRegex = new Regex(string.Format("^{0}$|^{1}(?::{1})*$|^(?i:{2})$", "(?:100|[1-9]?\\d)%", VariantConfigurationValidator.HashBucketRangeRegexString, "norotate"), RegexOptions.Compiled);
	}
}
