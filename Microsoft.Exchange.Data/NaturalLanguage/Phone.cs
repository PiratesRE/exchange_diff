using System;
using System.ComponentModel;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Data.NaturalLanguage
{
	public class Phone : IPositionedExtraction
	{
		[XmlElement(ElementName = "PhoneString")]
		public string PhoneString { get; set; }

		[XmlElement(ElementName = "OriginalPhoneString")]
		public string OriginalPhoneString { get; set; }

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

		[DefaultValue(PhoneType.Unspecified)]
		[XmlAttribute]
		public PhoneType Type { get; set; }

		private int startIndex = -1;
	}
}
