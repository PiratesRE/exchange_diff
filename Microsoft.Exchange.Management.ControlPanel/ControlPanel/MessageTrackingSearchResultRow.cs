using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Management.Tracking;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[DataContract]
	public class MessageTrackingSearchResultRow : BaseRow
	{
		public MessageTrackingSearchResultRow(MessageTrackingSearchResult messageTrackingSearchResult) : base(new Identity(messageTrackingSearchResult.MessageTrackingReportId.RawIdentity, messageTrackingSearchResult.Subject), messageTrackingSearchResult)
		{
			this.MessageTrackingSearchResult = messageTrackingSearchResult;
		}

		public MessageTrackingSearchResult MessageTrackingSearchResult { get; private set; }

		[DataMember]
		public string Subject
		{
			get
			{
				return this.MessageTrackingSearchResult.Subject;
			}
			set
			{
				throw new NotSupportedException();
			}
		}

		public DateTime SubmittedUTCDateTime
		{
			get
			{
				return this.MessageTrackingSearchResult.SubmittedDateTime;
			}
			set
			{
				throw new NotSupportedException();
			}
		}

		[DataMember]
		public string SubmittedDateTime
		{
			get
			{
				return this.MessageTrackingSearchResult.SubmittedDateTime.UtcToUserDateTimeString();
			}
			set
			{
				throw new NotSupportedException();
			}
		}

		[DataMember]
		public string RecipientDisplayNames
		{
			get
			{
				return this.MessageTrackingSearchResult.RecipientDisplayNames.StringArrayJoin(";");
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
				return this.MessageTrackingSearchResult.FromDisplayName;
			}
			protected set
			{
				throw new NotSupportedException();
			}
		}
	}
}
