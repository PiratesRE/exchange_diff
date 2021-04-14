using System;
using Microsoft.Mapi;

namespace Microsoft.Exchange.Cluster.Replay
{
	internal interface IListMDBStatus : IDisposable
	{
		MdbStatus[] ListMdbStatus(Guid[] dbGuids);

		MdbStatus[] ListMdbStatus(Guid[] dbGuids, TimeSpan? timeout);

		MdbStatus[] ListMdbStatus(bool isBasicInformation);

		MdbStatus[] ListMdbStatus(bool isBasicInformation, TimeSpan? timeout);
	}
}
