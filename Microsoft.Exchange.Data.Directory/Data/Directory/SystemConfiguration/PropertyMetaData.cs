using System;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	[XmlType(TypeName = "PropertyMetaData")]
	[Serializable]
	public sealed class PropertyMetaData : XMLSerializableBase
	{
		public PropertyMetaData(string name, DateTime time)
		{
			this.AttributeName = name;
			this.LastWriteTime = time;
		}

		public PropertyMetaData()
		{
		}

		[XmlElement(ElementName = "AttributeName")]
		public string AttributeName { get; set; }

		[XmlElement(ElementName = "LastWriteTime")]
		public DateTime LastWriteTime { get; set; }
	}
}
