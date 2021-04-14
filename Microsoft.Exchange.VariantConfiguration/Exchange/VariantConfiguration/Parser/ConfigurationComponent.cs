using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Search.Platform.Parallax;
using Microsoft.Search.Platform.Parallax.Util.IniFormat;
using Microsoft.Search.Platform.Parallax.Util.IniFormat.FileModel;

namespace Microsoft.Exchange.VariantConfiguration.Parser
{
	internal class ConfigurationComponent
	{
		internal ConfigurationComponent(string dataSourceName, IDictionary<string, ConfigurationSection> sections)
		{
			this.DataSourceName = dataSourceName;
			this.sections = sections;
		}

		public string DataSourceName { get; private set; }

		public ConfigurationComponentType ComponentType
		{
			get
			{
				if (string.Equals(this.DataSourceName, "Flighting.settings.ini", StringComparison.OrdinalIgnoreCase))
				{
					return ConfigurationComponentType.Team;
				}
				if (this.DataSourceName.EndsWith(".flight.ini", StringComparison.OrdinalIgnoreCase))
				{
					return ConfigurationComponentType.Flight;
				}
				return ConfigurationComponentType.Settings;
			}
		}

		public IEnumerable<ConfigurationSection> Sections
		{
			get
			{
				return this.sections.Values;
			}
		}

		public static ConfigurationComponent Create(string dataSourcePath)
		{
			if (!File.Exists(dataSourcePath))
			{
				throw new ArgumentException(string.Format("{0} does not exist", dataSourcePath));
			}
			string input = File.ReadAllText(dataSourcePath);
			string fileName = Path.GetFileName(dataSourcePath);
			return ConfigurationComponent.Create(input, fileName);
		}

		internal static ConfigurationComponent Create(string input, string dataSourceName)
		{
			ConfigurationComponent result;
			try
			{
				IniFileModel ini = IniFileModel.CreateFromString(dataSourceName, input);
				IDictionary<string, ConfigurationSection> dictionary = ConfigurationComponent.ParseSections(ini);
				result = new ConfigurationComponent(dataSourceName, dictionary);
			}
			catch (IniParseException ex)
			{
				throw new VariantConfigurationIniParseException(ex.Message, ex);
			}
			return result;
		}

		internal static bool TryCreate(string input, string dataSourceName, out ConfigurationComponent component)
		{
			try
			{
				component = ConfigurationComponent.Create(input, dataSourceName);
				return true;
			}
			catch (VariantConfigurationIniParseException)
			{
			}
			catch (VariantConfigurationConventionException)
			{
			}
			catch (VariantConfigurationSyntaxException)
			{
			}
			component = null;
			return false;
		}

		internal bool ContainsSection(string section)
		{
			return this.sections.ContainsKey(section);
		}

		internal ConfigurationSection GetSection(string section)
		{
			return this.sections[section];
		}

		private static IDictionary<string, ConfigurationSection> ParseSections(IniFileModel ini)
		{
			Dictionary<string, ConfigurationSection> dictionary = new Dictionary<string, ConfigurationSection>();
			foreach (Section section in ini.Sections.Values)
			{
				try
				{
					ConfigurationSection configurationSection = ConfigurationSection.Create(section);
					dictionary.Add(configurationSection.SectionName, configurationSection);
				}
				catch (VariantConfigurationSyntaxException ex)
				{
					throw new VariantConfigurationSyntaxException(string.Format("[Section: '{0}']: {1}", section.Name, ex.Message));
				}
				catch (TypeNotFoundException ex2)
				{
					throw new TypeNotFoundException(string.Format("[Section: '{0}']: {1}", section.Name, ex2.Message));
				}
			}
			return dictionary;
		}

		private IDictionary<string, ConfigurationSection> sections;
	}
}
