using System;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Data
{
	public class PiiResource
	{
		[XmlAttribute(AttributeName = "name")]
		public string Name { get; set; }

		[XmlArray(ElementName = "LocStrings")]
		[XmlArrayItem(ElementName = "LocString")]
		public PiiLocString[] LocStrings { get; set; }
	}
}
