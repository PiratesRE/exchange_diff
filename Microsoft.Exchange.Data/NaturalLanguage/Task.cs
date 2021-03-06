using System;
using System.ComponentModel;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Data.NaturalLanguage
{
	public class Task : IPositionedExtraction
	{
		public string TaskString { get; set; }

		public EmailUser[] Assignees { get; set; }

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

		[DefaultValue(EmailPosition.LatestReply)]
		[XmlAttribute]
		public EmailPosition Position { get; set; }

		private int startIndex = -1;
	}
}
