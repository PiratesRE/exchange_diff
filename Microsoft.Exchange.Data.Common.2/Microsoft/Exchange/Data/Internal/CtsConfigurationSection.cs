using System;
using System.Collections.Generic;
using System.Configuration;
using System.Xml;

namespace Microsoft.Exchange.Data.Internal
{
	internal sealed class CtsConfigurationSection : ConfigurationSection
	{
		public Dictionary<string, IList<CtsConfigurationSetting>> SubSectionsDictionary
		{
			get
			{
				return this.subSections;
			}
		}

		protected override ConfigurationPropertyCollection Properties
		{
			get
			{
				if (CtsConfigurationSection.properties == null)
				{
					CtsConfigurationSection.properties = new ConfigurationPropertyCollection();
				}
				return CtsConfigurationSection.properties;
			}
		}

		protected override void DeserializeSection(XmlReader reader)
		{
			IList<CtsConfigurationSetting> list = new List<CtsConfigurationSetting>();
			this.subSections.Add(string.Empty, list);
			if (!reader.Read() || reader.NodeType != XmlNodeType.Element)
			{
				throw new ConfigurationErrorsException("error", reader);
			}
			if (!reader.IsEmptyElement)
			{
				while (reader.Read())
				{
					if (reader.NodeType == XmlNodeType.Element)
					{
						if (reader.IsEmptyElement)
						{
							CtsConfigurationSetting item = this.DeserializeSetting(reader);
							list.Add(item);
						}
						else
						{
							string name = reader.Name;
							IList<CtsConfigurationSetting> list2;
							if (!this.subSections.TryGetValue(name, out list2))
							{
								list2 = new List<CtsConfigurationSetting>();
								this.subSections.Add(name, list2);
							}
							while (reader.Read())
							{
								if (reader.NodeType == XmlNodeType.Element)
								{
									if (!reader.IsEmptyElement)
									{
										throw new ConfigurationErrorsException("error", reader);
									}
									CtsConfigurationSetting item2 = this.DeserializeSetting(reader);
									list2.Add(item2);
								}
								else
								{
									if (reader.NodeType == XmlNodeType.EndElement)
									{
										break;
									}
									if (reader.NodeType == XmlNodeType.CDATA || reader.NodeType == XmlNodeType.Text)
									{
										throw new ConfigurationErrorsException("error", reader);
									}
								}
							}
						}
					}
					else
					{
						if (reader.NodeType == XmlNodeType.EndElement)
						{
							return;
						}
						if (reader.NodeType == XmlNodeType.CDATA || reader.NodeType == XmlNodeType.Text)
						{
							throw new ConfigurationErrorsException("error", reader);
						}
					}
				}
			}
		}

		private CtsConfigurationSetting DeserializeSetting(XmlReader reader)
		{
			string name = reader.Name;
			CtsConfigurationSetting ctsConfigurationSetting = new CtsConfigurationSetting(name);
			if (reader.AttributeCount > 0)
			{
				while (reader.MoveToNextAttribute())
				{
					string name2 = reader.Name;
					string value = reader.Value;
					ctsConfigurationSetting.AddArgument(name2, value);
				}
			}
			return ctsConfigurationSetting;
		}

		private static ConfigurationPropertyCollection properties;

		private Dictionary<string, IList<CtsConfigurationSetting>> subSections = new Dictionary<string, IList<CtsConfigurationSetting>>();
	}
}
