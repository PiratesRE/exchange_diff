using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.MailboxTransport.StoreDriver;
using Microsoft.Exchange.Transport;
using Microsoft.Exchange.Transport.Logging.ConnectionLog;

namespace Microsoft.Exchange.MailboxTransport.StoreDriverDelivery
{
	internal sealed class DeliveryServerThreadMap : SynchronizedThreadMap<string>
	{
		public DeliveryServerThreadMap(Trace tracer) : base(1, StringComparer.OrdinalIgnoreCase, tracer, "Delivery server", 100, DeliveryConfiguration.Instance.Throttling.MailboxServerThreadLimit, AckReason.MailboxServerThreadLimitExceeded, false)
		{
		}

		protected override void LogLimitExceeded(string key, int threadLimit, ulong sessionId, string mdb)
		{
			ConnectionLog.MapiDeliveryConnectionServerThreadLimitReached(mdb, key, threadLimit);
		}

		private const string KeyDisplayName = "Delivery server";

		private const int EstimatedEntrySize = 100;
	}
}
