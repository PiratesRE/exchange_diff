using System;
using System.Xml.Serialization;
using Microsoft.Exchange.Connections.Eas.Model.Request.AirSyncBase;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Connections.Eas.Model.Request.ItemOperations
{
	[XmlType(Namespace = "ItemOperations", TypeName = "Schema")]
	[ClassAccessLevel(AccessLevel.Implementation)]
	public class Schema
	{
		[XmlElement(ElementName = "Attachments", Namespace = "AirSyncBase")]
		public Attachment Attachments { get; set; }

		[XmlElement(ElementName = "Body", Namespace = "AirSyncBase")]
		public Body Body { get; set; }

		[XmlElement(ElementName = "CC", Namespace = "Email")]
		public string CC { get; set; }

		[XmlElement(ElementName = "DateReceived", Namespace = "Email")]
		public string DateReceived { get; set; }

		[XmlElement(ElementName = "DisplayTo", Namespace = "Email")]
		public string DisplayTo { get; set; }

		[XmlElement(ElementName = "From", Namespace = "Email")]
		public string From { get; set; }

		[XmlElement(ElementName = "Importance", Namespace = "Email")]
		public byte? Importance { get; set; }

		[XmlElement(ElementName = "InternetCPID", Namespace = "Email")]
		public int? InternetCpid { get; set; }

		[XmlElement(ElementName = "MeetingRequest", Namespace = "Email")]
		public string MeetingRequest { get; set; }

		[XmlElement(ElementName = "MessageClass", Namespace = "Email")]
		public string MessageClass { get; set; }

		[XmlElement(ElementName = "Read", Namespace = "Email")]
		public byte? Read { get; set; }

		[XmlElement(ElementName = "ReplyTo", Namespace = "Email")]
		public string ReplyTo { get; set; }

		[XmlElement(ElementName = "Subject", Namespace = "Email")]
		public string Subject { get; set; }

		[XmlElement(ElementName = "ThreadTopic", Namespace = "Email")]
		public string ThreadTopic { get; set; }

		[XmlElement(ElementName = "To", Namespace = "Email")]
		public string To { get; set; }

		[XmlIgnore]
		public bool ImportanceSpecified
		{
			get
			{
				return this.Importance != null;
			}
		}

		[XmlIgnore]
		public bool InternetCpidSpecified
		{
			get
			{
				return this.InternetCpid != null;
			}
		}

		[XmlIgnore]
		public bool ReadSpecified
		{
			get
			{
				return this.Read != null;
			}
		}
	}
}
