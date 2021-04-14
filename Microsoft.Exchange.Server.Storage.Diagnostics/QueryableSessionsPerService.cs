using System;
using Microsoft.Exchange.Protocols.MAPI;

namespace Microsoft.Exchange.Server.Storage.Diagnostics
{
	public class QueryableSessionsPerService
	{
		public QueryableSessionsPerService(MapiServiceType mapiServiceType, long sessionCount)
		{
			this.MapiServiceType = mapiServiceType.ToString();
			this.SessionCount = sessionCount;
		}

		public string MapiServiceType { get; private set; }

		public long SessionCount { get; private set; }
	}
}
