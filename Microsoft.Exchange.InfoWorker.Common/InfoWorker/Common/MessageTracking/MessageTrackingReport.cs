using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.InfoWorker.Common.MessageTracking
{
	[Serializable]
	internal sealed class MessageTrackingReport
	{
		public MessageTrackingReport()
		{
		}

		public MessageTrackingReport(MessageTrackingReportId identity, DateTime submittedDateTime, string subject, SmtpAddress? fromAddress, string fromDisplayName, SmtpAddress[] recipientAddresses, string[] recipientDisplayNames, RecipientEventData eventData)
		{
			if (recipientAddresses == null)
			{
				throw new ArgumentNullException("recipientAddresses", "Param cannot be null, pass in empty SmtpAddress[] instead");
			}
			if (recipientDisplayNames == null)
			{
				throw new ArgumentNullException("recipientDisplayNames", "Param cannot be null, pass in empty string[] instead");
			}
			this.identity = identity;
			this.submittedDateTime = submittedDateTime;
			this.subject = subject;
			this.fromAddress = fromAddress;
			this.fromDisplayName = fromDisplayName;
			this.recipientAddresses = recipientAddresses;
			this.recipientDisplayNames = recipientDisplayNames;
			this.eventData = eventData;
		}

		public MessageTrackingReportId MessageTrackingReportId
		{
			get
			{
				return this.identity;
			}
		}

		public DateTime SubmittedDateTime
		{
			get
			{
				return this.submittedDateTime;
			}
		}

		public string Subject
		{
			get
			{
				return this.subject;
			}
		}

		public SmtpAddress? FromAddress
		{
			get
			{
				return this.fromAddress;
			}
		}

		public string FromDisplayName
		{
			get
			{
				return this.fromDisplayName;
			}
		}

		public SmtpAddress[] RecipientAddresses
		{
			get
			{
				return this.recipientAddresses;
			}
			internal set
			{
				this.recipientAddresses = value;
			}
		}

		public string[] RecipientDisplayNames
		{
			get
			{
				return this.recipientDisplayNames;
			}
			internal set
			{
				this.recipientDisplayNames = value;
			}
		}

		public RecipientTrackingEvent[] RecipientTrackingEvents
		{
			get
			{
				if (this.eventData == null)
				{
					return null;
				}
				return this.eventData.Events.ToArray();
			}
		}

		public bool HasHandedOffPaths
		{
			get
			{
				return this.eventData.HandoffPaths != null;
			}
		}

		public List<RecipientTrackingEvent> RawSerializedEvents
		{
			get
			{
				return this.eventData.Serialize();
			}
		}

		public void MergeRecipientEventsFrom(MessageTrackingReport report)
		{
			if (report.eventData.Events != null)
			{
				this.eventData.Events.AddRange(report.eventData.Events);
			}
			if (report.eventData.HandoffPaths != null)
			{
				this.eventData.HandoffPaths.AddRange(report.eventData.HandoffPaths);
			}
		}

		public void AssignReportIdToRecipEvents()
		{
			string reportId = this.identity.ToString();
			if (this.eventData.Events != null)
			{
				this.AssignReportIdToRecipEvents(this.eventData.Events, reportId);
			}
			if (this.eventData.HandoffPaths != null)
			{
				foreach (List<RecipientTrackingEvent> recipEvents in this.eventData.HandoffPaths)
				{
					this.AssignReportIdToRecipEvents(recipEvents, reportId);
				}
			}
		}

		private void AssignReportIdToRecipEvents(IEnumerable<RecipientTrackingEvent> recipEvents, string reportId)
		{
			foreach (RecipientTrackingEvent recipientTrackingEvent in recipEvents)
			{
				recipientTrackingEvent.ExtendedProperties.MessageTrackingReportId = reportId;
			}
		}

		private MessageTrackingReportId identity;

		private DateTime submittedDateTime;

		private string subject;

		private SmtpAddress? fromAddress;

		private string fromDisplayName;

		private SmtpAddress[] recipientAddresses;

		private string[] recipientDisplayNames;

		private RecipientEventData eventData;
	}
}
