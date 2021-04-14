using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.InfoWorker.Common.Availability.Proxy;
using Microsoft.Exchange.SoapWebClient.EWS;

namespace Microsoft.Exchange.InfoWorker.Common.MessageTracking
{
	[Serializable]
	internal sealed class RecipientTrackingEvent
	{
		public static RecipientTrackingEvent Create(string domain, Microsoft.Exchange.SoapWebClient.EWS.RecipientTrackingEventType wsRecipientTrackingEvent)
		{
			return RecipientTrackingEvent.Create(domain, (wsRecipientTrackingEvent.Recipient == null) ? null : wsRecipientTrackingEvent.Recipient.EmailAddress, (wsRecipientTrackingEvent.Recipient == null) ? null : wsRecipientTrackingEvent.Recipient.Name, wsRecipientTrackingEvent.DeliveryStatus, wsRecipientTrackingEvent.EventDescription, wsRecipientTrackingEvent.EventData, wsRecipientTrackingEvent.Server, wsRecipientTrackingEvent.Date, wsRecipientTrackingEvent.InternalId, wsRecipientTrackingEvent.UniquePathId, wsRecipientTrackingEvent.HiddenRecipient, new bool?(wsRecipientTrackingEvent.BccRecipient), wsRecipientTrackingEvent.RootAddress, wsRecipientTrackingEvent.Properties);
		}

		public static RecipientTrackingEvent Create(string domain, Microsoft.Exchange.InfoWorker.Common.Availability.Proxy.RecipientTrackingEventType rdRecipientTrackingEvent)
		{
			return RecipientTrackingEvent.Create(domain, (rdRecipientTrackingEvent.Recipient == null) ? null : rdRecipientTrackingEvent.Recipient.EmailAddress, (rdRecipientTrackingEvent.Recipient == null) ? null : rdRecipientTrackingEvent.Recipient.Name, rdRecipientTrackingEvent.DeliveryStatus, rdRecipientTrackingEvent.EventDescription, rdRecipientTrackingEvent.EventData, rdRecipientTrackingEvent.Server, rdRecipientTrackingEvent.Date, rdRecipientTrackingEvent.InternalId, rdRecipientTrackingEvent.UniquePathId, rdRecipientTrackingEvent.HiddenRecipient, new bool?(rdRecipientTrackingEvent.BccRecipient), rdRecipientTrackingEvent.RootAddress, MessageConverter.CopyTrackingProperties(rdRecipientTrackingEvent.Properties));
		}

		private static RecipientTrackingEvent Create(string domain, string recipientEmail, string recipientDisplayName, string deliveryStatusString, string eventDescriptionString, string[] eventData, string server, DateTime date, string internalIdString, string uniquePathId, bool hiddenRecipient, bool? bccRecipient, string rootAddress, Microsoft.Exchange.SoapWebClient.EWS.TrackingPropertyType[] properties)
		{
			if (string.IsNullOrEmpty(recipientEmail))
			{
				TraceWrapper.SearchLibraryTracer.TraceError<string>(0, "Null recipient address in WS-RecipientTrackingEvent: {0}", recipientEmail);
				TrackingFatalException.RaiseED(ErrorCode.UnexpectedErrorPermanent, "Null recipient in WS-response", new object[0]);
			}
			SmtpAddress smtpAddress = new SmtpAddress(recipientEmail);
			if (!smtpAddress.IsValidAddress)
			{
				TraceWrapper.SearchLibraryTracer.TraceError<SmtpAddress>(0, "Corrupt recipient address in RD-RecipientTrackingEvent: {0}", smtpAddress);
				TrackingFatalException.RaiseED(ErrorCode.UnexpectedErrorPermanent, "Invalid recipient address {0} in WS-response", new object[]
				{
					smtpAddress
				});
			}
			recipientDisplayName = (recipientDisplayName ?? smtpAddress.ToString());
			DeliveryStatus deliveryStatus;
			if (!EnumValidator<DeliveryStatus>.TryParse(deliveryStatusString, EnumParseOptions.Default, out deliveryStatus))
			{
				TraceWrapper.SearchLibraryTracer.TraceDebug<string>(0, "Skipping event because of unknown delivery-status value in WS-RecipientTrackingEvent: {0}", deliveryStatusString);
				return null;
			}
			EventDescription eventDescription;
			if (!EnumValidator<EventDescription>.TryParse(eventDescriptionString, EnumParseOptions.Default, out eventDescription))
			{
				TraceWrapper.SearchLibraryTracer.TraceDebug<string>(0, "Skipping event because of unknown event-description in WS-RecipientTrackingEvent: {0}", eventDescriptionString);
				return null;
			}
			if (string.IsNullOrEmpty(internalIdString))
			{
				TraceWrapper.SearchLibraryTracer.TraceError<string>(0, "Null or empty internalIdString in RD-RecipientTrackingEvent: {0}", internalIdString);
				TrackingFatalException.RaiseED(ErrorCode.UnexpectedErrorPermanent, "No InternalId {0} in WS-response", new object[0]);
			}
			long num = 0L;
			if (!long.TryParse(internalIdString, out num) || num < 0L)
			{
				TraceWrapper.SearchLibraryTracer.TraceError<string>(0, "Non-numeric or negative internalIdString in RD-RecipientTrackingEvent: {0}", internalIdString);
				TrackingFatalException.RaiseED(ErrorCode.UnexpectedErrorPermanent, "Invalid InternalId {0} in WS-response", new object[]
				{
					internalIdString
				});
			}
			TrackingExtendedProperties trackingExtendedProperties = TrackingExtendedProperties.CreateFromTrackingPropertyArray(properties);
			if (eventDescription == EventDescription.PendingModeration && !string.IsNullOrEmpty(trackingExtendedProperties.ArbitrationMailboxAddress) && !SmtpAddress.IsValidSmtpAddress(trackingExtendedProperties.ArbitrationMailboxAddress))
			{
				TraceWrapper.SearchLibraryTracer.TraceDebug<string>(0, "Arbitration address is in the extended proprties but it's invalid: {0}", trackingExtendedProperties.ArbitrationMailboxAddress);
				TrackingFatalException.RaiseED(ErrorCode.UnexpectedErrorPermanent, "Invalid ArbitrationMailboxAddress property {0} in WS-response", new object[]
				{
					trackingExtendedProperties.ArbitrationMailboxAddress
				});
			}
			return new RecipientTrackingEvent(domain, smtpAddress, recipientDisplayName, deliveryStatus, EventType.Pending, eventDescription, eventData, server, date, num, uniquePathId, hiddenRecipient, bccRecipient, rootAddress, true, trackingExtendedProperties);
		}

		public RecipientTrackingEvent(string domain, SmtpAddress recipientAddress, string recipientDisplayName, DeliveryStatus status, EventType eventType, EventDescription eventDescription, string[] eventData, string serverFqdn, DateTime date, long internalMessageId, string uniquePathId, bool hiddenRecipient, bool? bccRecipient, string rootAddress, string arbitrationMailboxAddress, string initMessageId) : this(domain, recipientAddress, recipientDisplayName, status, eventType, eventDescription, eventData, serverFqdn, date, internalMessageId, uniquePathId, hiddenRecipient, bccRecipient, rootAddress, false, new TrackingExtendedProperties(false, false, null, false, string.Empty, arbitrationMailboxAddress, initMessageId, false))
		{
		}

		private RecipientTrackingEvent(string domain, SmtpAddress recipientAddress, string recipientDisplayName, DeliveryStatus status, EventType eventType, EventDescription eventDescription, string[] rawEventData, string serverFqdn, DateTime date, long internalMessageId, string uniquePathId, bool hiddenRecipient, bool? bccRecipient, string rootAddress, bool parseEventData, TrackingExtendedProperties trackingExtendedProperties)
		{
			this.domain = domain;
			this.recipientAddress = recipientAddress;
			this.recipientDisplayName = recipientDisplayName;
			this.status = status;
			this.eventType = eventType;
			this.eventDescription = eventDescription;
			this.server = serverFqdn;
			this.date = date;
			this.internalMessageId = internalMessageId;
			this.uniquePathId = uniquePathId;
			this.hiddenRecipient = hiddenRecipient;
			this.bccRecipient = bccRecipient;
			this.rootAddress = rootAddress;
			this.extendedProperties = trackingExtendedProperties;
			if (parseEventData)
			{
				VersionConverter.ConvertRawEventData(rawEventData, this);
				return;
			}
			this.eventData = rawEventData;
		}

		public string Domain
		{
			get
			{
				return this.domain;
			}
		}

		public DateTime Date
		{
			get
			{
				return this.date;
			}
		}

		public SmtpAddress RecipientAddress
		{
			get
			{
				return this.recipientAddress;
			}
			set
			{
				this.recipientAddress = value;
			}
		}

		public string RecipientDisplayName
		{
			get
			{
				return this.recipientDisplayName;
			}
			set
			{
				this.recipientDisplayName = value;
			}
		}

		public DeliveryStatus Status
		{
			get
			{
				return this.status;
			}
		}

		public EventType EventType
		{
			get
			{
				return this.eventType;
			}
		}

		public EventDescription EventDescription
		{
			get
			{
				return this.eventDescription;
			}
		}

		public string[] EventData
		{
			get
			{
				return this.eventData;
			}
			internal set
			{
				this.eventData = value;
			}
		}

		public string RootAddress
		{
			get
			{
				return this.rootAddress;
			}
			set
			{
				this.rootAddress = value;
			}
		}

		public string Server
		{
			get
			{
				return this.server;
			}
		}

		public long InternalMessageId
		{
			get
			{
				return this.internalMessageId;
			}
		}

		public string UniquePathId
		{
			get
			{
				return this.uniquePathId;
			}
			set
			{
				this.uniquePathId = value;
			}
		}

		public bool HiddenRecipient
		{
			get
			{
				return this.hiddenRecipient;
			}
			set
			{
				this.hiddenRecipient = value;
			}
		}

		public bool BccRecipient
		{
			get
			{
				return this.bccRecipient == null || this.bccRecipient.Value;
			}
			set
			{
				this.bccRecipient = new bool?(value);
			}
		}

		public string ServerHint
		{
			get
			{
				if ((this.EventDescription == EventDescription.SmtpSend || this.EventDescription == EventDescription.SmtpSendCrossForest || this.EventDescription == EventDescription.SmtpSendCrossSite) && this.eventData.Length >= 2 && !string.IsNullOrEmpty(this.eventData[1]))
				{
					return this.eventData[1];
				}
				return null;
			}
		}

		public TrackingExtendedProperties ExtendedProperties
		{
			get
			{
				return this.extendedProperties;
			}
			set
			{
				this.extendedProperties = value;
			}
		}

		internal RecipientTrackingEvent Clone()
		{
			return new RecipientTrackingEvent(this.domain, this.recipientAddress, this.recipientDisplayName, this.status, this.eventType, this.eventDescription, this.eventData, this.server, this.date, this.internalMessageId, this.uniquePathId, this.hiddenRecipient, this.bccRecipient, this.rootAddress, false, this.extendedProperties);
		}

		internal void ConvertRecipientTrackingEvent(DeliveryStatus status, EventType eventType, EventDescription eventDescription)
		{
			this.status = status;
			this.eventType = eventType;
			this.eventDescription = eventDescription;
		}

		private string domain;

		private DateTime date;

		private SmtpAddress recipientAddress = SmtpAddress.Empty;

		private string recipientDisplayName;

		private DeliveryStatus status;

		private EventType eventType;

		private EventDescription eventDescription;

		private string[] eventData;

		private string server;

		private long internalMessageId;

		private string uniquePathId;

		private bool hiddenRecipient;

		private bool? bccRecipient;

		private string rootAddress;

		private TrackingExtendedProperties extendedProperties;
	}
}
