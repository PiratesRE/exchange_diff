using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Management.Tracking;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[DataContract]
	public class RecipientStatusRow : BaseRow
	{
		public RecipientStatusRow(Identity messageTrackingReportIdentity, RecipientTrackingEvent trackingEvent) : base(RecipientStatusRow.CreateRecipientStatusRowIdentity(messageTrackingReportIdentity, trackingEvent), trackingEvent)
		{
			this.RecipientTrackingEvent = trackingEvent;
		}

		public RecipientTrackingEvent RecipientTrackingEvent { get; private set; }

		[DataMember]
		public string RecipientDisplayName
		{
			get
			{
				return this.RecipientTrackingEvent.RecipientDisplayName;
			}
			protected set
			{
				throw new NotSupportedException();
			}
		}

		[DataMember]
		public string RecipientEmail
		{
			get
			{
				return this.RecipientTrackingEvent.RecipientAddress.ToString();
			}
			protected set
			{
				throw new NotSupportedException();
			}
		}

		[DataMember]
		public string DeliveryStatus
		{
			get
			{
				return LocalizedDescriptionAttribute.FromEnumForOwaOption(typeof(_DeliveryStatus), this.RecipientTrackingEvent.Status);
			}
			protected set
			{
				throw new NotSupportedException();
			}
		}

		[DataMember]
		public string Date
		{
			get
			{
				return this.RecipientTrackingEvent.Date.UtcToUserDateTimeString();
			}
			protected set
			{
				throw new NotSupportedException();
			}
		}

		public DateTime UTCDate
		{
			get
			{
				return this.RecipientTrackingEvent.Date;
			}
			protected set
			{
				throw new NotSupportedException();
			}
		}

		public RecipientDeliveryStatus RecipientDeliveryStatus
		{
			get
			{
				return (RecipientDeliveryStatus)this.RecipientTrackingEvent.Status;
			}
			protected set
			{
				throw new NotSupportedException();
			}
		}

		private static Identity CreateRecipientStatusRowIdentity(Identity messageTrackingReportIdentity, RecipientTrackingEvent trackingEvent)
		{
			string recipient = trackingEvent.RecipientAddress.ToString();
			string displayName = trackingEvent.RecipientAddress.ToString();
			RecipientMessageTrackingReportId recipientMessageTrackingReportId = new RecipientMessageTrackingReportId(messageTrackingReportIdentity.RawIdentity, recipient);
			return new Identity(recipientMessageTrackingReportId.RawIdentity, displayName);
		}
	}
}
