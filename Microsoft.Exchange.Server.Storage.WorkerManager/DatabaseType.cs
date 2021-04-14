using System;

namespace Microsoft.Exchange.Server.Storage.WorkerManager
{
	public enum DatabaseType : byte
	{
		None,
		Active,
		Passive,
		Recovery
	}
}
