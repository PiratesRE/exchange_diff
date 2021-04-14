using System;

namespace Microsoft.Exchange.Cluster.Replay
{
	internal interface IEseDatabaseReader : IDisposable
	{
		void ForceNewLog();

		byte[] ReadOnePage(long pageNumber, out long lowGen, out long highGen);

		long PageSize { get; }

		string DatabaseName { get; }

		Guid DatabaseGuid { get; }

		long ReadPageSize();
	}
}
