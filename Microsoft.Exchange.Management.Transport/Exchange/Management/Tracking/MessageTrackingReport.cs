using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics.Components.Tracking;
using Microsoft.Exchange.InfoWorker.Common.MessageTracking;
using Microsoft.Exchange.Transport.Logging.Search;

namespace Microsoft.Exchange.Management.Tracking
{
	[Serializable]
	public class MessageTrackingReport : MessageTrackingConfigurableObject
	{
		public MessageTrackingReport()
		{
		}

		public MessageTrackingReportId MessageTrackingReportId
		{
			get
			{
				return (MessageTrackingReportId)this[MessageTrackingSharedResultSchema.MessageTrackingReportId];
			}
		}

		public DateTime SubmittedDateTime
		{
			get
			{
				return (DateTime)this[MessageTrackingSharedResultSchema.SubmittedDateTime];
			}
		}

		public string Subject
		{
			get
			{
				return (string)this[MessageTrackingSharedResultSchema.Subject];
			}
		}

		public SmtpAddress FromAddress
		{
			get
			{
				return (SmtpAddress)this[MessageTrackingSharedResultSchema.FromAddress];
			}
		}

		public string FromDisplayName
		{
			get
			{
				return (string)this[MessageTrackingSharedResultSchema.FromDisplayName];
			}
		}

		public SmtpAddress[] RecipientAddresses
		{
			get
			{
				return (SmtpAddress[])this[MessageTrackingSharedResultSchema.RecipientAddresses];
			}
			internal set
			{
				this[MessageTrackingSharedResultSchema.RecipientAddresses] = value;
			}
		}

		public string[] RecipientDisplayNames
		{
			get
			{
				return (string[])this[MessageTrackingSharedResultSchema.RecipientDisplayNames];
			}
			internal set
			{
				this[MessageTrackingSharedResultSchema.RecipientDisplayNames] = value;
			}
		}

		public int DeliveredCount
		{
			get
			{
				return (int)this[MessageTrackingReportSchema.DeliveryCount];
			}
			internal set
			{
				this[MessageTrackingReportSchema.DeliveryCount] = value;
			}
		}

		public int PendingCount
		{
			get
			{
				return (int)this[MessageTrackingReportSchema.PendingCount];
			}
			internal set
			{
				this[MessageTrackingReportSchema.PendingCount] = value;
			}
		}

		public int UnsuccessfulCount
		{
			get
			{
				return (int)this[MessageTrackingReportSchema.UnsuccessfulCount];
			}
			internal set
			{
				this[MessageTrackingReportSchema.UnsuccessfulCount] = value;
			}
		}

		public int TransferredCount
		{
			get
			{
				return (int)this[MessageTrackingReportSchema.TransferredCount];
			}
			internal set
			{
				this[MessageTrackingReportSchema.TransferredCount] = value;
			}
		}

		public RecipientTrackingEvent[] RecipientTrackingEvents
		{
			get
			{
				return (RecipientTrackingEvent[])this[MessageTrackingReportSchema.RecipientTrackingEvents];
			}
			internal set
			{
				this[MessageTrackingReportSchema.RecipientTrackingEvents] = value;
			}
		}

		internal override ObjectSchema ObjectSchema
		{
			get
			{
				return MessageTrackingReport.schema;
			}
		}

		internal static MessageTrackingReport Create(IConfigurationSession configurationSession, IRecipientSession recipientSession, MultiValuedProperty<CultureInfo> userLanguages, bool summaryEvents, bool returnHelpDeskMessages, bool trackingAsSender, MessageTrackingReport internalMessageTrackingReport, bool doNotResolve, bool isCompleteReport)
		{
			RecipientTrackingEvent[] recipientTrackingEvents = internalMessageTrackingReport.RecipientTrackingEvents;
			if (!doNotResolve && recipientTrackingEvents.Length > 256)
			{
				ExTraceGlobals.TaskTracer.TraceDebug<int, int>(0L, "Recipient events ({0}) are more than MaxDisplaynameLookupsAllowed ({1}), turning off display-names", recipientTrackingEvents.Length, 256);
				doNotResolve = true;
			}
			RecipientTrackingEvent[] array;
			if (summaryEvents)
			{
				array = MessageTrackingReport.GetRecipientEventsForSummaryReport(configurationSession, recipientSession, userLanguages, returnHelpDeskMessages, trackingAsSender, recipientTrackingEvents);
			}
			else
			{
				array = MessageTrackingReport.GetRecipientEventsForRecipientPathReport(configurationSession, recipientSession, userLanguages, returnHelpDeskMessages, trackingAsSender, recipientTrackingEvents, isCompleteReport);
			}
			if (array == null)
			{
				return null;
			}
			int capacity = summaryEvents ? array.Length : 1;
			BulkRecipientLookupCache bulkRecipientLookupCache = new BulkRecipientLookupCache(capacity);
			if (!doNotResolve)
			{
				RecipientTrackingEvent.FillDisplayNames(bulkRecipientLookupCache, array, recipientSession);
			}
			MessageTrackingReport messageTrackingReport = new MessageTrackingReport(internalMessageTrackingReport, array);
			if (summaryEvents)
			{
				messageTrackingReport.FillDisplayNames(bulkRecipientLookupCache, recipientSession);
			}
			messageTrackingReport.PrepareRecipientTrackingEvents(returnHelpDeskMessages, summaryEvents);
			return messageTrackingReport;
		}

		private static RecipientTrackingEvent[] GetRecipientEventsForRecipientPathReport(IConfigurationSession configurationSession, IRecipientSession recipientSession, MultiValuedProperty<CultureInfo> userLanguages, bool returnHelpDeskMessages, bool trackingAsSender, RecipientTrackingEvent[] internalRecipientTrackingEvents, bool isCompleteReport)
		{
			List<RecipientTrackingEvent> list = new List<RecipientTrackingEvent>(internalRecipientTrackingEvents.Length);
			RecipientTrackingEvent recipientTrackingEvent = null;
			if (isCompleteReport)
			{
				recipientTrackingEvent = RecipientTrackingEvent.GetExplanatoryMessage(internalRecipientTrackingEvents);
			}
			bool flag = false;
			for (int i = 0; i < internalRecipientTrackingEvents.Length; i++)
			{
				bool flag2 = i == internalRecipientTrackingEvents.Length - 1;
				if (!MessageTrackingReport.ShouldHideEvent(flag2, internalRecipientTrackingEvents[i], returnHelpDeskMessages, trackingAsSender) && (!flag || internalRecipientTrackingEvents[i].EventDescription != EventDescription.FailedGeneral) && (internalRecipientTrackingEvents[i].EventDescription != EventDescription.TransferredToForeignOrg || flag2) && (internalRecipientTrackingEvents[i].EventDescription != EventDescription.FailedTransportRules || returnHelpDeskMessages || (!flag && (i + 1 >= internalRecipientTrackingEvents.Length || (internalRecipientTrackingEvents[i + 1].EventDescription != EventDescription.FailedTransportRules && internalRecipientTrackingEvents[i + 1].EventDescription != EventDescription.FailedGeneral)))))
				{
					RecipientTrackingEvent recipientTrackingEvent2 = RecipientTrackingEvent.Create(flag2, configurationSession, userLanguages, returnHelpDeskMessages, internalRecipientTrackingEvents[i]);
					if (recipientTrackingEvent2 != null)
					{
						if (internalRecipientTrackingEvents[i].EventDescription == EventDescription.FailedTransportRules || internalRecipientTrackingEvents[i].EventDescription == EventDescription.FailedGeneral)
						{
							flag = true;
						}
						list.Add(recipientTrackingEvent2);
					}
					else
					{
						ExTraceGlobals.TaskTracer.TraceDebug<string, SmtpAddress>(0L, "Event: {0} for recipient {1} not translateable for end-user. It will be dropped", Names<EventDescription>.Map[(int)internalRecipientTrackingEvents[i].EventDescription], internalRecipientTrackingEvents[i].RecipientAddress);
					}
				}
			}
			if (list.Count == 0)
			{
				return null;
			}
			if (recipientTrackingEvent != null)
			{
				list.Add(recipientTrackingEvent);
			}
			return list.ToArray();
		}

		private static RecipientTrackingEvent[] GetRecipientEventsForSummaryReport(IConfigurationSession configurationSession, IRecipientSession recipientSession, MultiValuedProperty<CultureInfo> userLanguages, bool returnHelpDeskMessages, bool trackingAsSender, RecipientTrackingEvent[] internalRecipientTrackingEvents)
		{
			Dictionary<string, RecipientTrackingEvent> dictionary = new Dictionary<string, RecipientTrackingEvent>(StringComparer.OrdinalIgnoreCase);
			for (int i = 0; i < internalRecipientTrackingEvents.Length; i++)
			{
				if (!MessageTrackingReport.ShouldHideEvent(true, internalRecipientTrackingEvents[i], returnHelpDeskMessages, trackingAsSender))
				{
					RecipientTrackingEvent recipientTrackingEvent = RecipientTrackingEvent.Create(true, configurationSession, userLanguages, returnHelpDeskMessages, internalRecipientTrackingEvents[i]);
					if (recipientTrackingEvent == null)
					{
						ExTraceGlobals.TaskTracer.TraceDebug<string, SmtpAddress>(0L, "Event: {0} not translateable for end-user for recipient: {1}, substituting with generic pending event", Names<EventDescription>.Map[(int)internalRecipientTrackingEvents[i].EventDescription], internalRecipientTrackingEvents[i].RecipientAddress);
						RecipientTrackingEvent internalRecipientTrackingEvent = new RecipientTrackingEvent(null, internalRecipientTrackingEvents[i].RecipientAddress, internalRecipientTrackingEvents[i].RecipientDisplayName, DeliveryStatus.Pending, EventType.Pending, EventDescription.Pending, null, internalRecipientTrackingEvents[i].Server, internalRecipientTrackingEvents[i].Date, internalRecipientTrackingEvents[i].InternalMessageId, null, internalRecipientTrackingEvents[i].HiddenRecipient, new bool?(internalRecipientTrackingEvents[i].BccRecipient), internalRecipientTrackingEvents[i].RootAddress, null, null);
						recipientTrackingEvent = RecipientTrackingEvent.Create(true, configurationSession, userLanguages, returnHelpDeskMessages, internalRecipientTrackingEvent);
						if (recipientTrackingEvent == null)
						{
							throw new InvalidOperationException("Generic pending event should always be creatable");
						}
					}
					RecipientTrackingEvent recipientTrackingEvent2;
					if (!dictionary.TryGetValue(recipientTrackingEvent.RecipientAddress.ToString(), out recipientTrackingEvent2) || recipientTrackingEvent2.Status != _DeliveryStatus.Delivered)
					{
						dictionary[recipientTrackingEvent.RecipientAddress.ToString()] = recipientTrackingEvent;
					}
				}
			}
			if (dictionary.Count == 0)
			{
				return null;
			}
			return dictionary.Values.ToArray<RecipientTrackingEvent>();
		}

		private static bool ShouldHideEvent(bool isLastKnownStatus, RecipientTrackingEvent trackingEvent, bool returnHelpDeskMessages, bool trackingAsSender)
		{
			if (isLastKnownStatus && (trackingEvent.EventDescription == EventDescription.TransferredToPartnerOrg || trackingEvent.EventDescription == EventDescription.SmtpSendCrossForest))
			{
				return false;
			}
			if (trackingEvent.HiddenRecipient && trackingAsSender && !returnHelpDeskMessages)
			{
				ExTraceGlobals.TaskTracer.TraceDebug<SmtpAddress>(0L, "Should hide recipient {0}, as it is a hidden recipient and may not be shown to InfoWorker role", trackingEvent.RecipientAddress);
				return true;
			}
			return false;
		}

		internal void IncrementEventTypeCount(_DeliveryStatus typeToIncrement)
		{
			switch (typeToIncrement)
			{
			case _DeliveryStatus.Unsuccessful:
				this.UnsuccessfulCount++;
				return;
			case _DeliveryStatus.Pending:
				this.PendingCount++;
				return;
			case _DeliveryStatus.Delivered:
			case _DeliveryStatus.Read:
				this.DeliveredCount++;
				return;
			case _DeliveryStatus.Transferred:
				this.TransferredCount++;
				return;
			default:
				throw new InvalidOperationException();
			}
		}

		private MessageTrackingReport(MessageTrackingReport internalMessageTrackingReport, RecipientTrackingEvent[] recipientTrackingEvents)
		{
			this.internalMessageTrackingReport = internalMessageTrackingReport;
			this[MessageTrackingSharedResultSchema.MessageTrackingReportId] = new MessageTrackingReportId(internalMessageTrackingReport.MessageTrackingReportId);
			this[MessageTrackingSharedResultSchema.FromAddress] = internalMessageTrackingReport.FromAddress;
			this[MessageTrackingSharedResultSchema.FromDisplayName] = null;
			this[MessageTrackingSharedResultSchema.RecipientAddresses] = internalMessageTrackingReport.RecipientAddresses;
			this[MessageTrackingSharedResultSchema.RecipientDisplayNames] = null;
			this[MessageTrackingReportSchema.RecipientTrackingEvents] = recipientTrackingEvents;
			this[MessageTrackingSharedResultSchema.Subject] = internalMessageTrackingReport.Subject;
			this[MessageTrackingSharedResultSchema.SubmittedDateTime] = internalMessageTrackingReport.SubmittedDateTime;
		}

		private void FillDisplayNames(BulkRecipientLookupCache recipientNamesCache, IRecipientSession galSession)
		{
			string text = this.internalMessageTrackingReport.FromAddress.ToString();
			SmtpAddress[] recipientAddresses = this.internalMessageTrackingReport.RecipientAddresses;
			IEnumerable<string> addresses = (from address in recipientAddresses
			select address.ToString()).Concat(new string[]
			{
				text
			});
			IEnumerable<string> source = recipientNamesCache.Resolve(addresses, galSession);
			this[MessageTrackingSharedResultSchema.RecipientDisplayNames] = source.Take(recipientAddresses.Length).ToArray<string>();
			this[MessageTrackingSharedResultSchema.FromDisplayName] = source.Last<string>();
		}

		private void PrepareRecipientTrackingEvents(bool returnHelpDeskMessages, bool summaryMode)
		{
			if (!returnHelpDeskMessages)
			{
				foreach (RecipientTrackingEvent recipientTrackingEvent in this.RecipientTrackingEvents)
				{
					if (recipientTrackingEvent.EventDescriptionEnum != EventDescription.Expanded)
					{
						recipientTrackingEvent.EventData = null;
					}
					recipientTrackingEvent.Server = null;
				}
				return;
			}
			if (!summaryMode)
			{
				bool flag = false;
				foreach (RecipientTrackingEvent recipientTrackingEvent2 in this.RecipientTrackingEvents)
				{
					if (recipientTrackingEvent2.EventDescriptionEnum == EventDescription.Delivered)
					{
						flag = true;
					}
					else if (flag)
					{
						recipientTrackingEvent2.Server = null;
					}
				}
			}
		}

		private const int MaxDisplaynameLookupsAllowed = 256;

		private static MessageTrackingReportSchema schema = ObjectSchema.GetInstance<MessageTrackingReportSchema>();

		private MessageTrackingReport internalMessageTrackingReport;
	}
}
