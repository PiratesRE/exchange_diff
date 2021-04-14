using System;

namespace Microsoft.Exchange.Server.Storage.WorkerManager
{
	public interface IWorkerProcess
	{
		int ProcessId { get; }

		Guid InstanceId { get; }

		int Generation { get; }

		string InstanceName { get; }

		DatabaseType InstanceDBType { get; set; }
	}
}
