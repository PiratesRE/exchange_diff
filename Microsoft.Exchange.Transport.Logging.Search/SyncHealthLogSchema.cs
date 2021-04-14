using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Transport.Logging.Search
{
	public static class SyncHealthLogSchema
	{
		public static CsvTable SyncHealthLogEvent
		{
			get
			{
				return SyncHealthLogSchema.syncHealthLogEvent;
			}
		}

		private static readonly Version E14Version = new Version("14.00.0565.00");

		private static readonly CsvTable syncHealthLogEvent = new CsvTable(new CsvField[]
		{
			new CsvField("TimeStamp", typeof(DateTime), SyncHealthLogSchema.E14Version),
			new CsvField("EventId", typeof(string), SyncHealthLogSchema.E14Version),
			new CsvField("EventData", typeof(KeyValuePair<string, object>[]), SyncHealthLogSchema.E14Version)
		});
	}
}
