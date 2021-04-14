using System;
using System.Security.Principal;
using Microsoft.Exchange.Server.Storage.StoreCommonServices;

namespace Microsoft.Exchange.Server.Storage.Diagnostics
{
	public class QueryableSessionsPerUser
	{
		public QueryableSessionsPerUser(SecurityIdentifier userSid, ClientType clientType, long sessionCount)
		{
			this.UserSid = userSid.ToString();
			this.ClientType = clientType.ToString();
			this.SessionCount = sessionCount;
		}

		public string UserSid { get; private set; }

		public string ClientType { get; private set; }

		public long SessionCount { get; private set; }
	}
}
