using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.MailboxTransport.StoreDriver;
using Microsoft.Exchange.Transport;
using Microsoft.Exchange.Transport.Logging.ConnectionLog;

namespace Microsoft.Exchange.MailboxTransport.StoreDriverDelivery
{
	internal sealed class DeliveryDatabaseThreadMap : SynchronizedThreadMap<string>
	{
		public DeliveryDatabaseThreadMap(Trace tracer) : base(DeliveryDatabaseThreadMap.StartingCapacity, StringComparer.OrdinalIgnoreCase, tracer, "Delivery database", 100, new Dictionary<string, int>(DeliveryDatabaseThreadMap.StartingCapacity), AckReason.MailboxDatabaseThreadLimitExceeded, true)
		{
		}

		public bool TryCheckAndIncrement(string mdbGuid, int threadLimit, ulong sessionId)
		{
			return base.TryCheckThreadLimit(mdbGuid, threadLimit, sessionId, mdbGuid, true);
		}

		public void Check(string key, int threadLimit, ulong sessionId, string mdb)
		{
			base.CheckThreadLimit(key, threadLimit, sessionId, mdb, false);
		}

		protected override void LogLimitExceeded(string key, int threadLimit, ulong sessionId, string mdb)
		{
			ConnectionLog.MapiDeliveryConnectionMdbThreadLimitReached(mdb, threadLimit);
		}

		public void UpdateMdbThreadCounters()
		{
			foreach (KeyValuePair<string, int> keyValuePair in base.ThreadMap)
			{
				StoreDriverDatabasePerfCounters.AddDeliveryThreadSample(keyValuePair.Key, (long)keyValuePair.Value);
			}
		}

		internal const string KeyDisplayName = "Delivery database";

		private const int EstimatedEntrySize = 100;

		private static readonly int StartingCapacity = 16;
	}
}
