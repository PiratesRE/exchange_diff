using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Search.Platform.Parallax;
using Microsoft.Search.Platform.Parallax.Util.IniFormat.FileModel;

namespace Microsoft.Exchange.VariantConfiguration.Parser
{
	internal class ConfigurationSection
	{
		internal ConfigurationSection(string sectionName, string typeName, IEnumerable<ConfigurationParameter> parameters)
		{
			this.SectionName = sectionName;
			this.TypeName = typeName;
			this.Parameters = parameters;
		}

		public string SectionName { get; private set; }

		public string TypeName { get; private set; }

		public IEnumerable<ConfigurationParameter> Parameters { get; private set; }

		public static ConfigurationSection Create(Section section)
		{
			string typeName = ConfigurationSection.GetTypeName(section);
			IEnumerable<ConfigurationParameter> parameters = ConfigurationSection.ParseParameters(section.Parameters);
			return new ConfigurationSection(section.Name, typeName, parameters);
		}

		public override bool Equals(object obj)
		{
			ConfigurationSection configurationSection = obj as ConfigurationSection;
			return configurationSection != null && string.Equals(this.SectionName, configurationSection.SectionName, StringComparison.OrdinalIgnoreCase) && string.Equals(this.TypeName, configurationSection.TypeName, StringComparison.OrdinalIgnoreCase) && this.Parameters.SequenceEqual(configurationSection.Parameters);
		}

		public override int GetHashCode()
		{
			return base.GetType().GetHashCode() ^ this.SectionName.ToLowerInvariant().GetHashCode() ^ this.TypeName.ToLowerInvariant().GetHashCode() ^ this.CalculateParametersHash();
		}

		internal IEnumerable<string> GetFlightDependencies()
		{
			List<string> list = new List<string>();
			foreach (ConfigurationParameter configurationParameter in this.Parameters)
			{
				bool parsedValue;
				IEnumerable<string> collection = from variant in configurationParameter.Variants
				where variant.Key.StartsWith("flt.", StringComparison.OrdinalIgnoreCase) && bool.TryParse(variant.Value, out parsedValue) && parsedValue
				select variant.Key.Substring("flt.".Length);
				list.AddRange(collection);
			}
			return list.Distinct<string>();
		}

		private static string GetTypeName(Section section)
		{
			foreach (ParameterAssignmentRule parameterAssignmentRule in section.Parameters)
			{
				if (string.Equals(parameterAssignmentRule.ParameterName, "_meta.type"))
				{
					if (!string.IsNullOrEmpty(parameterAssignmentRule.VariantString))
					{
						throw new TypeNotFoundException("_meta.type property should not have a variant string.");
					}
					return parameterAssignmentRule.Value;
				}
			}
			throw new TypeNotFoundException(string.Format("Section '{0}' contains no type data.", section.Name));
		}

		private static IEnumerable<ConfigurationParameter> ParseParameters(IEnumerable<ParameterAssignmentRule> parameters)
		{
			List<ConfigurationParameter> list = new List<ConfigurationParameter>();
			foreach (ParameterAssignmentRule parameterAssignmentRule in parameters)
			{
				if (!string.Equals(parameterAssignmentRule.ParameterName, "_meta.type") && !string.Equals(parameterAssignmentRule.ParameterName, "_meta.access"))
				{
					list.Add(ConfigurationParameter.Create(string.Format("{0}{1}={2}", parameterAssignmentRule.ParameterName, parameterAssignmentRule.VariantString, parameterAssignmentRule.Value)));
				}
			}
			return list;
		}

		private int CalculateParametersHash()
		{
			int num = 0;
			foreach (ConfigurationParameter configurationParameter in this.Parameters)
			{
				num ^= configurationParameter.GetHashCode();
			}
			return num;
		}
	}
}
