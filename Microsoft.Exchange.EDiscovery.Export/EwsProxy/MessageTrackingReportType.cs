using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Exchange.EDiscovery.Export.EwsProxy
{
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[DesignerCategory("code")]
	[GeneratedCode("wsdl", "4.0.30319.17627")]
	[DebuggerStepThrough]
	[Serializable]
	public class MessageTrackingReportType
	{
		public EmailAddressType Sender
		{
			get
			{
				return this.senderField;
			}
			set
			{
				this.senderField = value;
			}
		}

		public EmailAddressType PurportedSender
		{
			get
			{
				return this.purportedSenderField;
			}
			set
			{
				this.purportedSenderField = value;
			}
		}

		public string Subject
		{
			get
			{
				return this.subjectField;
			}
			set
			{
				this.subjectField = value;
			}
		}

		public DateTime SubmitTime
		{
			get
			{
				return this.submitTimeField;
			}
			set
			{
				this.submitTimeField = value;
			}
		}

		[XmlIgnore]
		public bool SubmitTimeSpecified
		{
			get
			{
				return this.submitTimeFieldSpecified;
			}
			set
			{
				this.submitTimeFieldSpecified = value;
			}
		}

		[XmlArrayItem("Address", IsNullable = false)]
		public EmailAddressType[] OriginalRecipients
		{
			get
			{
				return this.originalRecipientsField;
			}
			set
			{
				this.originalRecipientsField = value;
			}
		}

		[XmlArrayItem("RecipientTrackingEvent", IsNullable = false)]
		public RecipientTrackingEventType[] RecipientTrackingEvents
		{
			get
			{
				return this.recipientTrackingEventsField;
			}
			set
			{
				this.recipientTrackingEventsField = value;
			}
		}

		[XmlArrayItem(IsNullable = false)]
		public TrackingPropertyType[] Properties
		{
			get
			{
				return this.propertiesField;
			}
			set
			{
				this.propertiesField = value;
			}
		}

		private EmailAddressType senderField;

		private EmailAddressType purportedSenderField;

		private string subjectField;

		private DateTime submitTimeField;

		private bool submitTimeFieldSpecified;

		private EmailAddressType[] originalRecipientsField;

		private RecipientTrackingEventType[] recipientTrackingEventsField;

		private TrackingPropertyType[] propertiesField;
	}
}
