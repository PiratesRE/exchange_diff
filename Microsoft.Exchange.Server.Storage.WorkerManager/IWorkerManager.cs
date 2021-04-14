using System;
using System.Collections.Generic;
using System.Threading;
using Microsoft.Exchange.Server.Storage.Common;

namespace Microsoft.Exchange.Server.Storage.WorkerManager
{
	public interface IWorkerManager
	{
		ErrorCode StartWorker(string workerPath, Guid instance, Guid dagOrServerGuid, string instanceName, Action<IWorkerProcess> workerCompleteCallback, CancellationToken cancellationToken, out IWorkerProcess worker);

		void StopWorker(Guid instance, bool terminate);

		ErrorCode GetWorker(Guid instance, out IWorkerProcess worker);

		IEnumerable<IWorkerProcess> GetActiveWorkers();
	}
}
