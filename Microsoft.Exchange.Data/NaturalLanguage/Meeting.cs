using System;
using System.ComponentModel;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Data.NaturalLanguage
{
	public class Meeting : IPositionedExtraction
	{
		public string MeetingString { get; set; }

		public EmailUser[] Attendees { get; set; }

		[XmlAttribute]
		public string Location { get; set; }

		[XmlAttribute]
		public string Subject { get; set; }

		public DateTime? StartTime { get; set; }

		public DateTime? EndTime { get; set; }

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

		[XmlAttribute]
		[DefaultValue(EmailPosition.LatestReply)]
		public EmailPosition Position { get; set; }

		private int startIndex = -1;
	}
}
