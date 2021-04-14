using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Office.Datacenter.WorkerTaskFramework;

namespace Microsoft.Office.Datacenter.ActiveMonitoring
{
	public class DataAccessQuery<T> : IDataAccessQuery<T>, IEnumerable<!0>, IEnumerable
	{
		internal DataAccessQuery(IEnumerable<T> innerQuery, DataAccess dataAccess)
		{
			this.dataAccess = dataAccess;
			this.innerQuery = innerQuery;
		}

		public IEnumerable<T> InnerQuery
		{
			get
			{
				return this.innerQuery;
			}
		}

		public IEnumerator<T> GetEnumerator()
		{
			return this.innerQuery.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			throw new NotImplementedException();
		}

		public Task<int> ExecuteAsync(Action<T> processResult, CancellationToken cancellationToken, TracingContext traceContext)
		{
			return new LocalDataAccess().AsyncExecuteReader<T>(this, processResult, cancellationToken, traceContext);
		}

		public Task<T> ExecuteAsync(CancellationToken cancellationToken, TracingContext traceContext)
		{
			return new LocalDataAccess().AsyncExecuteScalar<T>(this, cancellationToken, traceContext);
		}

		public IDataAccessQuery<TEntity> AsDataAccessQuery<TEntity>(IEnumerable<TEntity> query)
		{
			return this.dataAccess.AsDataAccessQuery<TEntity>(query);
		}

		private BaseDataAccess dataAccess;

		private IEnumerable<T> innerQuery;
	}
}
