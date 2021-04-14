using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Exchange.InfoWorker.Common.Availability.Proxy
{
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[DesignerCategory("code")]
	[GeneratedCode("wsdl", "2.0.50727.3038")]
	[DebuggerStepThrough]
	[Serializable]
	public class FindMessageTrackingSearchResultType
	{
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

		[XmlArrayItem("Mailbox", IsNullable = false)]
		public EmailAddressType[] Recipients
		{
			get
			{
				return this.recipientsField;
			}
			set
			{
				this.recipientsField = value;
			}
		}

		public DateTime SubmittedTime
		{
			get
			{
				return this.submittedTimeField;
			}
			set
			{
				this.submittedTimeField = value;
			}
		}

		public string MessageTrackingReportId
		{
			get
			{
				return this.messageTrackingReportIdField;
			}
			set
			{
				this.messageTrackingReportIdField = value;
			}
		}

		public string PreviousHopServer
		{
			get
			{
				return this.previousHopServerField;
			}
			set
			{
				this.previousHopServerField = value;
			}
		}

		public string FirstHopServer
		{
			get
			{
				return this.firstHopServerField;
			}
			set
			{
				this.firstHopServerField = value;
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

		private string subjectField;

		private EmailAddressType senderField;

		private EmailAddressType purportedSenderField;

		private EmailAddressType[] recipientsField;

		private DateTime submittedTimeField;

		private string messageTrackingReportIdField;

		private string previousHopServerField;

		private string firstHopServerField;

		private TrackingPropertyType[] propertiesField;
	}
}
