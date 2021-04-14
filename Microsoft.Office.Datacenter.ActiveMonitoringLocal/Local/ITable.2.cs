using System;
using System.Collections.Generic;
using Microsoft.Office.Datacenter.WorkerTaskFramework;

namespace Microsoft.Office.Datacenter.ActiveMonitoring.Local
{
	internal interface ITable<TItem> : ITable
	{
		IEnumerable<TItem> GetItems<TKey>(IIndexDescriptor<TItem, TKey> indexDescriptor);

		void Insert(TItem item, TracingContext traceContext);
	}
}
