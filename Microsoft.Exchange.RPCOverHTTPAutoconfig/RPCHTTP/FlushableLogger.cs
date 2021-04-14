using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Servicelets.RPCHTTP
{
	internal class FlushableLogger
	{
		public FlushableLogger(ExEventLog log)
		{
			this.eventQueue = new List<FlushableLogger.LogEventParams>();
			this.log = log;
		}

		public void AddEvent(ExEventLog.EventTuple tuple, string periodicKey, params object[] messageArgs)
		{
			lock (this.eventQueueLock)
			{
				this.eventQueue.Add(new FlushableLogger.LogEventParams(tuple, periodicKey, messageArgs));
			}
		}

		public void Flush()
		{
			List<FlushableLogger.LogEventParams> list;
			lock (this.eventQueueLock)
			{
				list = this.eventQueue;
				this.eventQueue = new List<FlushableLogger.LogEventParams>();
			}
			foreach (FlushableLogger.LogEventParams logEventParams in list)
			{
				this.log.LogEvent(logEventParams.tuple, logEventParams.periodicKey, logEventParams.messageArgs);
			}
		}

		private readonly ExEventLog log;

		private readonly object eventQueueLock = new object();

		private List<FlushableLogger.LogEventParams> eventQueue;

		private struct LogEventParams
		{
			public LogEventParams(ExEventLog.EventTuple tuple, string periodicKey, object[] messageArgs)
			{
				this.tuple = tuple;
				this.periodicKey = periodicKey;
				this.messageArgs = messageArgs;
			}

			public readonly ExEventLog.EventTuple tuple;

			public readonly string periodicKey;

			public readonly object[] messageArgs;
		}
	}
}
