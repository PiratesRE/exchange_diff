using System;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Services.Core.Types
{
	[XmlType(TypeName = "FindMessageTrackingSearchResultType", Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	public class FindMessageTrackingSearchResultType
	{
		internal FindMessageTrackingSearchResultType()
		{
		}

		internal FindMessageTrackingSearchResultType(string subject, EmailAddressWrapper sender, EmailAddressWrapper[] recipients, DateTime submittedTime, string messageTrackingReportId, string previousHopServer, string firstHopServer, TrackingPropertyType[] properties)
		{
			this.Subject = subject;
			this.Sender = sender;
			this.Recipients = recipients;
			this.SubmittedTime = submittedTime;
			this.MessageTrackingReportId = messageTrackingReportId;
			this.PreviousHopServer = (string.IsNullOrEmpty(previousHopServer) ? null : previousHopServer);
			this.FirstHopServer = (string.IsNullOrEmpty(firstHopServer) ? null : firstHopServer);
			this.Properties = properties;
		}

		public string Subject;

		public EmailAddressWrapper Sender;

		public EmailAddressWrapper PurportedSender;

		[XmlArrayItem("Mailbox", IsNullable = false)]
		public EmailAddressWrapper[] Recipients;

		public DateTime SubmittedTime;

		public string MessageTrackingReportId;

		public string PreviousHopServer;

		public string FirstHopServer;

		[XmlArrayItem("TrackingPropertyType", IsNullable = false)]
		public TrackingPropertyType[] Properties;
	}
}
