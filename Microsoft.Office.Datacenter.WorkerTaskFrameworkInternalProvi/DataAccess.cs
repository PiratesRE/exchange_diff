using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Office.Datacenter.WorkerTaskFramework
{
	public abstract class DataAccess : BaseDataAccess
	{
		internal abstract Task<int> AsyncGetExclusive<TEntity>(int maxWorkitemCount, int deploymentId, Action<TEntity> processResult, Func<object, Exception, bool> corruptRowHandler, CancellationToken cancellationToken, TracingContext traceContext) where TEntity : WorkDefinition, new();

		internal abstract Task<int> AsyncDeleteWorkItemResult<TWorkItemResult>(DateTime startTime, DateTime endTime, int timeOutInSeconds, CancellationToken cancellationToken, TracingContext traceContext);

		internal abstract TimeSpan? GetQuarantineTimeSpan<TEntity>(TEntity definition) where TEntity : WorkDefinition;

		internal abstract Task<int> AsyncDisableWorkDefinitions(int createdById, DateTime createdBeforeTimestamp, CancellationToken cancellationToken, TracingContext traceContext);

		internal abstract Task<List<StatusEntryCollection>> GetAllStatusEntries(CancellationToken cancellationToken, TracingContext traceContext);

		internal abstract BaseDataAccess GetTopologyDataAccessProvider();
	}
}
