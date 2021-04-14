using System;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Data
{
	public class PiiLocString
	{
		[XmlAttribute(AttributeName = "id")]
		public string Id { get; set; }

		[XmlAttribute(AttributeName = "piiParams")]
		public int[] Parameters { get; set; }
	}
}
