using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Search.Platform.Parallax;

namespace Microsoft.Exchange.VariantConfiguration.Parser
{
	public class ConfigurationParser
	{
		internal ConfigurationParser(IEnumerable<ConfigurationComponent> teamComponents, IEnumerable<ConfigurationComponent> flightComponents, IEnumerable<ConfigurationComponent> settingsComponents)
		{
			this.flightComponentMap = ConfigurationParser.CreateFlightComponentMap(flightComponents);
			this.SettingsComponents = settingsComponents;
			this.TeamComponents = teamComponents;
		}

		internal IEnumerable<ConfigurationComponent> TeamComponents { get; private set; }

		internal IEnumerable<ConfigurationComponent> FlightComponents
		{
			get
			{
				return this.flightComponentMap.Values.Distinct<ConfigurationComponent>();
			}
		}

		internal IEnumerable<ConfigurationComponent> SettingsComponents { get; private set; }

		public static ConfigurationParser Create(IEnumerable<string> dataSourcePaths)
		{
			List<ConfigurationComponent> list = new List<ConfigurationComponent>();
			List<ConfigurationComponent> list2 = new List<ConfigurationComponent>();
			List<ConfigurationComponent> list3 = new List<ConfigurationComponent>();
			foreach (string text in dataSourcePaths)
			{
				try
				{
					ConfigurationComponent configurationComponent = ConfigurationComponent.Create(text);
					switch (configurationComponent.ComponentType)
					{
					case ConfigurationComponentType.Flight:
						list2.Add(configurationComponent);
						break;
					case ConfigurationComponentType.Team:
						list.Add(configurationComponent);
						break;
					default:
						list3.Add(configurationComponent);
						break;
					}
				}
				catch (VariantConfigurationSyntaxException ex)
				{
					throw new VariantConfigurationSyntaxException(string.Format("[Component: '{0}']: {1}", text, ex.Message));
				}
				catch (TypeNotFoundException ex2)
				{
					throw new TypeNotFoundException(string.Format("[Component: '{0}']: {1}", text, ex2.Message));
				}
			}
			return new ConfigurationParser(list, list2, list3);
		}

		public bool DoesFlightUseRotate(string flightName)
		{
			return this.DoesFlightUseProperty(flightName, "Rotate");
		}

		public bool DoesFlightUseRamp(string flightName)
		{
			return this.DoesFlightUseProperty(flightName, "Ramp");
		}

		public IEnumerable<string> GetFlightDependencies(string dataSourceName, string sectionName)
		{
			if (string.IsNullOrEmpty(dataSourceName))
			{
				throw new ArgumentNullException("dataSourceName");
			}
			if (string.IsNullOrEmpty(sectionName))
			{
				throw new ArgumentNullException("sectionName");
			}
			ConfigurationComponent configurationComponent = this.SettingsComponents.FirstOrDefault((ConfigurationComponent comp) => string.Equals(comp.DataSourceName, dataSourceName, StringComparison.OrdinalIgnoreCase));
			if (configurationComponent == null)
			{
				throw new ArgumentException(string.Format("Could not find data source with name {0}", dataSourceName));
			}
			if (!configurationComponent.ContainsSection(sectionName))
			{
				throw new ArgumentException(string.Format("Could not find section {0} in data source {1}", sectionName, dataSourceName));
			}
			ConfigurationSection section = configurationComponent.GetSection(sectionName);
			return section.GetFlightDependencies();
		}

		internal bool DoesFlightUseProperty(string flightName, string propertyName)
		{
			if (string.IsNullOrEmpty(flightName))
			{
				throw new ArgumentNullException(flightName);
			}
			if (!this.flightComponentMap.ContainsKey(flightName))
			{
				throw new ArgumentException(string.Format("Flight {0} does not exist in the current context", flightName));
			}
			ConfigurationSection section = this.flightComponentMap[flightName].GetSection(flightName);
			foreach (ConfigurationParameter configurationParameter in section.Parameters)
			{
				if (string.Equals(configurationParameter.Name, propertyName, StringComparison.OrdinalIgnoreCase))
				{
					return true;
				}
			}
			return false;
		}

		private static IDictionary<string, ConfigurationComponent> CreateFlightComponentMap(IEnumerable<ConfigurationComponent> flightComponents)
		{
			Dictionary<string, ConfigurationComponent> dictionary = new Dictionary<string, ConfigurationComponent>(StringComparer.OrdinalIgnoreCase);
			foreach (ConfigurationComponent configurationComponent in flightComponents)
			{
				foreach (ConfigurationSection configurationSection in configurationComponent.Sections)
				{
					dictionary[configurationSection.SectionName] = configurationComponent;
				}
			}
			return dictionary;
		}

		internal const string FlightPrefix = "flt.";

		private IDictionary<string, ConfigurationComponent> flightComponentMap;
	}
}
