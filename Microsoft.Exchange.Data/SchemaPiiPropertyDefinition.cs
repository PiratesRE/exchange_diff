using System;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Data
{
	public class SchemaPiiPropertyDefinition
	{
		[XmlAttribute(AttributeName = "name")]
		public string Name { get; set; }

		[XmlArray(ElementName = "PiiProperties")]
		[XmlArrayItem(ElementName = "Property")]
		public PiiPropertyDefinition[] PiiProperties { get; set; }
	}
}
