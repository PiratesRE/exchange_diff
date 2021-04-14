using System;
using System.Xml.Serialization;

namespace Microsoft.Exchange.AirSync
{
	public class GlobalSettingsResultItem
	{
		public GlobalSettingsResultItem()
		{
		}

		public GlobalSettingsResultItem(string name, string type, string value, string defaultValue)
		{
			this.Name = name;
			this.Type = type;
			this.Value = value;
			this.DefaultValue = defaultValue;
		}

		[XmlAttribute]
		public string Name { get; set; }

		[XmlAttribute]
		public string Type { get; set; }

		public string Value { get; set; }

		public string DefaultValue { get; set; }
	}
}
