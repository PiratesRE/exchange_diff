using System;
using System.ComponentModel;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Data.NaturalLanguage
{
	public class Contact : IPositionedExtraction
	{
		public Person Person { get; set; }

		public Business Business { get; set; }

		public Phone[] Phones { get; set; }

		public Url[] Urls { get; set; }

		public Email[] Emails { get; set; }

		public Address[] Addresses { get; set; }

		public string ContactString { get; set; }

		[DefaultValue(-1)]
		[XmlAttribute]
		public int StartIndex
		{
			get
			{
				return this._startIndex;
			}
			set
			{
				this._startIndex = value;
			}
		}

		[XmlAttribute]
		[DefaultValue(EmailPosition.LatestReply)]
		public EmailPosition Position { get; set; }

		private int _startIndex = -1;
	}
}
