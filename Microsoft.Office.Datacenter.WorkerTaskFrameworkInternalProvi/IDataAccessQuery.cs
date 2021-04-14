using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Office.Datacenter.WorkerTaskFramework
{
	public interface IDataAccessQuery<T> : IEnumerable<T>, IEnumerable
	{
		IEnumerable<T> InnerQuery { get; }

		Task<int> ExecuteAsync(Action<T> processResult, CancellationToken cancellationToken, TracingContext traceContext);

		Task<T> ExecuteAsync(CancellationToken cancellationToken, TracingContext traceContext);

		IDataAccessQuery<TEntity> AsDataAccessQuery<TEntity>(IEnumerable<TEntity> query);
	}
}
