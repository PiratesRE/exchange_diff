using System;
using Microsoft.Exchange.Server.Storage.StoreCommonServices;

namespace Microsoft.Exchange.Protocols.MAPI
{
	internal static class MapiTimedEvents
	{
		internal static Guid EventSource
		{
			get
			{
				return MapiTimedEvents.eventSource;
			}
		}

		internal static void RaiseDeferredSendEvent(MapiContext context, DateTime eventTime, int mailboxNumber, ExchangeId folderId, ExchangeId messageId)
		{
			byte[] eventData = DeferredSendEvent.SerializeExtraData(folderId.ToLong(), messageId.ToLong());
			MapiTimedEvents.RaiseMapiTimedEvent(context, eventTime, new int?(mailboxNumber), MapiTimedEvents.EventType.DeferredSend, eventData);
		}

		internal static void RaiseMapiTimedEvent(MapiContext context, DateTime eventTime, int? mailboxNumber, MapiTimedEvents.EventType eventType, byte[] eventData)
		{
			TimedEventEntry timedEvent = new TimedEventEntry(eventTime, mailboxNumber, MapiTimedEvents.EventSource, (int)eventType, eventData);
			context.RaiseTimedEvent(timedEvent);
		}

		private static readonly Guid eventSource = new Guid("98F7D371-BC39-4C55-A9DD-E7DEA76EE738");

		internal enum EventType
		{
			None,
			DeferredSend
		}
	}
}
