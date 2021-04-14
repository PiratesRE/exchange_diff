using System;
using System.Collections.Generic;

namespace Microsoft.Office.Datacenter.WorkerTaskFramework
{
	public interface IIndexDescriptor<TEntity, TKey> : IIndexDescriptor
	{
		TKey Key { get; }

		IDataAccessQuery<TEntity> ApplyIndexRestriction(IDataAccessQuery<TEntity> query);

		IEnumerable<TKey> GetKeyValues(TEntity item);
	}
}
