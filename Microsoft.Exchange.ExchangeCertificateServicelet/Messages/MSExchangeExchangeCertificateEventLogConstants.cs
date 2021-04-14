using System;
using System.Diagnostics;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Servicelets.ExchangeCertificate.Messages
{
	public static class MSExchangeExchangeCertificateEventLogConstants
	{
		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_PermanentException = new ExEventLog.EventTuple(3221227473U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		private enum Category : short
		{
			General = 1
		}

		internal enum Message : uint
		{
			PermanentException = 3221227473U
		}
	}
}
