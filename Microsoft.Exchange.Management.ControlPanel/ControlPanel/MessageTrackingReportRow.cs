using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Management.Tracking;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[DataContract]
	public class MessageTrackingReportRow : BaseRow
	{
		public MessageTrackingReportRow(MessageTrackingReport messageTrackingReport) : base(new Identity(messageTrackingReport.MessageTrackingReportId.RawIdentity, OwaOptionStrings.DeliveryReport), messageTrackingReport)
		{
			this.MessageTrackingReport = messageTrackingReport;
			this.RecipientCounts = new RecipientCounts(messageTrackingReport.DeliveredCount, messageTrackingReport.PendingCount, messageTrackingReport.TransferredCount, messageTrackingReport.UnsuccessfulCount);
		}

		public MessageTrackingReport MessageTrackingReport { get; private set; }

		[DataMember]
		public RecipientCounts RecipientCounts { get; private set; }

		[DataMember]
		public string Subject
		{
			get
			{
				return this.MessageTrackingReport.Subject;
			}
			protected set
			{
				throw new NotSupportedException();
			}
		}

		[DataMember]
		public string FromDisplayName
		{
			get
			{
				return this.MessageTrackingReport.FromDisplayName;
			}
			protected set
			{
				throw new NotSupportedException();
			}
		}

		[DataMember]
		public string RecipientDisplayNames
		{
			get
			{
				return this.MessageTrackingReport.RecipientDisplayNames.StringArrayJoin(";");
			}
			protected set
			{
				throw new NotSupportedException();
			}
		}

		[DataMember]
		public string SubmittedDateTime
		{
			get
			{
				return this.MessageTrackingReport.SubmittedDateTime.UtcToUserDateTimeString();
			}
			protected set
			{
				throw new NotSupportedException();
			}
		}

		[DataMember]
		public RecipientStatusRow[] RecipientStatuses
		{
			get
			{
				return this.CreateRecipientStatusRows(this.MessageTrackingReport.RecipientTrackingEvents);
			}
			protected set
			{
				throw new NotSupportedException();
			}
		}

		private RecipientStatusRow[] CreateRecipientStatusRows(RecipientTrackingEvent[] recipientTrackingEvents)
		{
			int num = recipientTrackingEvents.Length;
			RecipientStatusRow[] array = new RecipientStatusRow[num];
			for (int i = 0; i < num; i++)
			{
				array[i] = new RecipientStatusRow(base.Identity, recipientTrackingEvents[i]);
			}
			if (array.Length > 0)
			{
				Func<RecipientStatusRow[], RecipientStatusRow[]> sortFunction = new SortOptions
				{
					PropertyName = "RecipientDeliveryStatus"
				}.GetSortFunction<RecipientStatusRow>();
				array = sortFunction(array);
			}
			return array;
		}

		private const string RecipientNamesSeparator = ";";
	}
}
