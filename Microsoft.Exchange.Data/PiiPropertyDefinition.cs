using System;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Data
{
	public class PiiPropertyDefinition
	{
		[XmlAttribute(AttributeName = "name")]
		public string Name { get; set; }

		[XmlAttribute(AttributeName = "redactor")]
		public string Redactor { get; set; }

		[XmlAttribute(AttributeName = "enumerable")]
		public bool Enumerable { get; set; }

		[XmlAttribute(AttributeName = "addIntoMap")]
		public bool AddIntoMap { get; set; }
	}
}
