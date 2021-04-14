using System;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Data.NaturalLanguage
{
	public class Keyword
	{
		[XmlText]
		public string KeywordString { get; set; }
	}
}
