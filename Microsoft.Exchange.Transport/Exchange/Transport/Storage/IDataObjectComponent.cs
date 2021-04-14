using System;

namespace Microsoft.Exchange.Transport.Storage
{
	internal interface IDataObjectComponent
	{
		bool PendingDatabaseUpdates { get; }

		int PendingDatabaseUpdateCount { get; }

		void MinimizeMemory();

		void CloneFrom(IDataObjectComponent other);
	}
}
