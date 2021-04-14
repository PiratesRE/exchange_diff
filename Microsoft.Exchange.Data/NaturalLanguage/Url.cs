using System;
using System.ComponentModel;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Data.NaturalLanguage
{
	public class Url : IPositionedExtraction
	{
		[XmlElement(ElementName = "UrlString")]
		public string UrlString { get; set; }

		[XmlAttribute]
		[DefaultValue(-1)]
		public int StartIndex
		{
			get
			{
				return this.startIndex;
			}
			set
			{
				this.startIndex = value;
			}
		}

		[DefaultValue(EmailPosition.LatestReply)]
		[XmlAttribute]
		public EmailPosition Position { get; set; }

		[XmlAttribute]
		[DefaultValue(UrlType.Unspecified)]
		public UrlType Type { get; set; }

		private int startIndex = -1;
	}
}
