using System;
using System.Xml.Linq;
using Microsoft.Exchange.Data.ConfigurationSettings;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration.ConfigurationSettings
{
	[Serializable]
	public class InternalExchangeSettings : ADConfigurationObject, IDiagnosableObject
	{
		public virtual XMLSerializableDictionary<string, SettingsHistory> History
		{
			get
			{
				return this.Xml.History.Value;
			}
		}

		public string XmlRaw
		{
			get
			{
				return (string)this[InternalExchangeSettingsSchema.ConfigurationXMLRaw];
			}
			set
			{
				this[InternalExchangeSettingsSchema.ConfigurationXMLRaw] = value;
			}
		}

		internal SettingsXml Xml
		{
			get
			{
				return (SettingsXml)this[InternalExchangeSettingsSchema.ConfigurationXML];
			}
			set
			{
				this[InternalExchangeSettingsSchema.ConfigurationXML] = value;
			}
		}

		internal SettingsGroupDictionary Settings
		{
			get
			{
				return this.Xml.Settings;
			}
		}

		internal override ADObjectSchema Schema
		{
			get
			{
				return InternalExchangeSettings.schema;
			}
		}

		internal override string MostDerivedObjectClass
		{
			get
			{
				return InternalExchangeSettings.mostDerivedClass;
			}
		}

		internal override ADObjectId ParentPath
		{
			get
			{
				return InternalExchangeSettings.ContainerRelativePath;
			}
		}

		string IDiagnosableObject.HashableIdentity
		{
			get
			{
				return this.Identity.ToString();
			}
		}

		public void InitializeSettings()
		{
			this.Xml = SettingsXml.Create();
		}

		public bool IsPriorityInUse(int priority, string groupNameToExclude = null)
		{
			foreach (SettingsGroup settingsGroup in this.Xml.Settings.Values)
			{
				if ((groupNameToExclude == null || !(groupNameToExclude == settingsGroup.Name)) && priority == settingsGroup.Priority)
				{
					return false;
				}
			}
			return true;
		}

		public SettingsGroup GetSettingsGroupForModification(string name)
		{
			if (!this.Settings.ContainsKey(name))
			{
				throw new ConfigurationSettingsGroupNotFoundException(name);
			}
			return this.Settings[name].Clone();
		}

		public void AddSettingsGroup(SettingsGroup settingsGroup)
		{
			this.OperateOnSettings(delegate(SettingsXml xml)
			{
				xml.AddSettingsGroup(settingsGroup);
			});
		}

		public void UpdateSettingsGroup(SettingsGroup settingsGroup)
		{
			this.OperateOnSettings(delegate(SettingsXml xml)
			{
				xml.UpdateSettingsGroup(settingsGroup);
			});
		}

		public void RemoveSettingsGroup(SettingsGroup settingsGroup, bool addHistory = true)
		{
			this.OperateOnSettings(delegate(SettingsXml xml)
			{
				xml.RemoveSettingsGroup(settingsGroup, addHistory);
			});
		}

		public void ClearHistorySettings(string name)
		{
			if (this.History == null || !this.History.ContainsKey(name))
			{
				return;
			}
			this.OperateOnSettings(delegate(SettingsXml xml)
			{
				xml.History.Value[name].ResizeHistorySettings(0);
			});
		}

		public XElement GetDiagnosticInfo(string argument)
		{
			XElement xelement = new XElement(base.Name);
			ConfigDiagnosticArgument configDiagnosticArgument = new ConfigDiagnosticArgument(argument);
			if (configDiagnosticArgument.HasArgument("configname"))
			{
				string argument2 = configDiagnosticArgument.GetArgument<string>("configname");
				XElement xelement2 = new XElement("setting", new XAttribute("name", argument2));
				foreach (SettingsGroup settingsGroup in this.Settings.Values)
				{
					string value;
					if (settingsGroup.TryGetValue(argument2, out value))
					{
						xelement2.Add(new XElement("group", new object[]
						{
							new XAttribute("name", settingsGroup.Name),
							new XAttribute("enabled", settingsGroup.Enabled),
							new XAttribute("priority", settingsGroup.Priority),
							new XAttribute("value", value)
						}));
					}
				}
				xelement.Add(xelement2);
			}
			else
			{
				foreach (SettingsGroup settingsGroup2 in this.Settings.Values)
				{
					xelement.Add(settingsGroup2.ToDiagnosticInfo(null));
				}
			}
			return xelement;
		}

		internal bool TryGetConfig(IConfigSchema configSchema, ISettingsContext context, string settingName, out string settingValue)
		{
			settingValue = null;
			int num = -1;
			foreach (SettingsGroup settingsGroup in this.Settings.Values)
			{
				string text;
				if (settingsGroup.Priority > num && settingsGroup.TryGetValue(settingName, out text) && settingsGroup.Matches(configSchema, context))
				{
					settingValue = text;
					num = settingsGroup.Priority;
				}
			}
			return num != -1;
		}

		private void OperateOnSettings(Action<SettingsXml> operation)
		{
			SettingsXml xml = this.Xml;
			operation(xml);
			this.Xml = xml;
		}

		public const string SettingElement = "setting";

		public const string GroupElement = "group";

		public const string NameAttribute = "name";

		public const string ValueAttribute = "value";

		public const string EnableAttribute = "enabled";

		public const string PriorityAttribute = "priority";

		public static ADObjectId ContainerRelativePath = new ADObjectId("CN=Configuration Settings,CN=Global Settings");

		private static InternalExchangeSettingsSchema schema = ObjectSchema.GetInstance<InternalExchangeSettingsSchema>();

		private static string mostDerivedClass = "msExchConfigSettings";
	}
}
