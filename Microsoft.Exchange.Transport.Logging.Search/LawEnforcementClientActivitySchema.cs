using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Transport.Logging.Search
{
	public static class LawEnforcementClientActivitySchema
	{
		public static CsvTable ClientActivityEvent
		{
			get
			{
				return LawEnforcementClientActivitySchema.clientActivityEvent;
			}
		}

		private static readonly CsvTable clientActivityEvent = new CsvTable(new CsvField[]
		{
			new CsvField("time", typeof(DateTime)),
			new CsvField("protocol", typeof(string)),
			new CsvField("account", typeof(string)),
			new CsvField("client-ip", typeof(string)),
			new CsvField("server-ip", typeof(string)),
			new CsvField("access-timestamp", typeof(string)),
			new CsvField("access-duration", typeof(string)),
			new CsvField("message-download", typeof(string))
		});
	}
}
