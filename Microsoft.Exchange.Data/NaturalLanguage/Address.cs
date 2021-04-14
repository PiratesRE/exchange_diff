using System;
using System.ComponentModel;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Data.NaturalLanguage
{
	public class Address : IPositionedExtraction
	{
		[DefaultValue(-1)]
		[XmlAttribute]
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

		[XmlAttribute]
		[DefaultValue(EmailPosition.LatestReply)]
		public EmailPosition Position { get; set; }

		[XmlText]
		public string AddressString { get; set; }

		private int startIndex = -1;
	}
}
