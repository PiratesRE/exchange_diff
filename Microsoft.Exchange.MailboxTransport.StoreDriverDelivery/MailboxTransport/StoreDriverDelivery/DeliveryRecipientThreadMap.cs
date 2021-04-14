using System;
using Microsoft.Exchange.Data.Transport;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.MailboxTransport.StoreDriver;
using Microsoft.Exchange.Transport;
using Microsoft.Exchange.Transport.Logging.ConnectionLog;

namespace Microsoft.Exchange.MailboxTransport.StoreDriverDelivery
{
	internal sealed class DeliveryRecipientThreadMap : SynchronizedThreadMap<RoutingAddress>
	{
		public DeliveryRecipientThreadMap(Trace tracer) : base(600, tracer, "Delivery recipient", 100, DeliveryConfiguration.Instance.Throttling.RecipientThreadLimit, AckReason.RecipientThreadLimitExceeded, true)
		{
		}

		protected override void LogLimitExceeded(RoutingAddress key, int threadLimit, ulong sessionId, string mdb)
		{
			ConnectionLog.MapiDeliveryConnectionRecipientThreadLimitReached(sessionId, key, mdb, threadLimit);
		}

		private const string KeyDisplayName = "Delivery recipient";

		private const int EstimatedCapacity = 600;

		private const int EstimatedEntrySize = 100;
	}
}
