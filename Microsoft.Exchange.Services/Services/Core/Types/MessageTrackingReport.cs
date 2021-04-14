using System;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Services.Core.Types
{
	[XmlType(TypeName = "MessageTrackingReportType", Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	public class MessageTrackingReport
	{
		public EmailAddressWrapper Sender;

		public EmailAddressWrapper PurportedSender;

		public string Subject;

		public DateTime SubmitTime;

		[XmlArrayItem("Address")]
		public EmailAddressWrapper[] OriginalRecipients;

		[XmlArrayItem("RecipientTrackingEvent", IsNullable = false)]
		public RecipientTrackingEvent[] RecipientTrackingEvents;

		[XmlArrayItem("TrackingPropertyType", IsNullable = false)]
		public TrackingPropertyType[] Properties;
	}
}
