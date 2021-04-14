using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using Microsoft.Exchange.InfoWorker.Common.MessageTracking;
using Microsoft.Exchange.Management.ControlPanel.WebControls;
using Microsoft.Exchange.Management.Tracking;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[DataContract]
	public class RecipientTrackingEventRow : BaseRow
	{
		public RecipientTrackingEventRow(MessageTrackingReport messageTrackingReport) : base(messageTrackingReport)
		{
			this.MessageTrackingReport = messageTrackingReport;
			this.numberOfEvents = messageTrackingReport.RecipientTrackingEvents.Length;
		}

		public MessageTrackingReport MessageTrackingReport { get; private set; }

		public string RecipientDisplayName
		{
			get
			{
				return this.MessageTrackingReport.RecipientTrackingEvents[this.numberOfEvents - 1].RecipientDisplayName;
			}
			protected set
			{
				throw new NotSupportedException();
			}
		}

		public string RecipientEmail
		{
			get
			{
				return this.MessageTrackingReport.RecipientTrackingEvents[this.numberOfEvents - 1].RecipientAddress.ToString();
			}
			protected set
			{
				throw new NotSupportedException();
			}
		}

		[DataMember]
		public string RecipientDisplayNameAndEmail
		{
			get
			{
				return RtlUtil.ConvertToDecodedBidiString(this.RecipientDisplayName + " (" + this.RecipientEmail + ")", RtlUtil.IsRtl);
			}
			protected set
			{
				throw new NotSupportedException();
			}
		}

		[DataMember]
		public IEnumerable<TrackingEventRow> Events
		{
			get
			{
				return from trackingEvent in this.MessageTrackingReport.RecipientTrackingEvents
				select new TrackingEventRow((TrackingEventType)trackingEvent.EventType, trackingEvent.Date, this.GetLocalizedStringForEventType(trackingEvent.EventType), trackingEvent.EventDescription, trackingEvent.Server, trackingEvent.EventData);
			}
			protected set
			{
				throw new NotSupportedException();
			}
		}

		private string GetLocalizedStringForEventType(EventType eventType)
		{
			string empty = string.Empty;
			switch (eventType)
			{
			case EventType.SmtpReceive:
			case EventType.SmtpSend:
				break;
			case EventType.Fail:
				return OwaOptionStrings.MessageTrackingFailedEvent;
			case EventType.Deliver:
				return OwaOptionStrings.MessageTrackingDeliveredEvent;
			case EventType.Resolve:
			case EventType.Redirect:
				goto IL_76;
			case EventType.Expand:
				return OwaOptionStrings.MessageTrackingDLExpandedEvent;
			case EventType.Submit:
				return OwaOptionStrings.MessageTrackingSubmitEvent;
			default:
				if (eventType != EventType.Transferred)
				{
					goto IL_76;
				}
				break;
			}
			return OwaOptionStrings.MessageTrackingTransferredEvent;
			IL_76:
			return OwaOptionStrings.MessageTrackingPendingEvent;
		}

		private int numberOfEvents;
	}
}
