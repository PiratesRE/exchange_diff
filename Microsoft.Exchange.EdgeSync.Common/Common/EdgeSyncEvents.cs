using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.EdgeSync.Common
{
	internal static class EdgeSyncEvents
	{
		public static ExEventLog Log
		{
			get
			{
				return EdgeSyncEvents.log;
			}
		}

		private static readonly ExEventLog log = new ExEventLog(new Guid("{8169CAF8-E6F1-480b-9700-39478DEA1FD5}"), "MSExchange EdgeSync");
	}
}
