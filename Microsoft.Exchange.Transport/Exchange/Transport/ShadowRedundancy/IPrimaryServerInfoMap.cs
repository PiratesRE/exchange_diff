using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.Transport.ShadowRedundancy
{
	internal interface IPrimaryServerInfoMap
	{
		event Action<PrimaryServerInfo> NotifyPrimaryServerStateChanged;

		int Count { get; }

		void Add(PrimaryServerInfo primaryServerInfo);

		IEnumerable<PrimaryServerInfo> GetAll();

		PrimaryServerInfo GetActive(string serverFqdn);

		PrimaryServerInfo UpdateServerState(string serverFqdn, string state, ShadowRedundancyCompatibilityVersion version);

		bool Remove(PrimaryServerInfo primaryServerInfo);

		IEnumerable<PrimaryServerInfo> RemoveExpiredServers(DateTime now);
	}
}
