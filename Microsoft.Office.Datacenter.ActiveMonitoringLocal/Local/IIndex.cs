using System;
using Microsoft.Office.Datacenter.WorkerTaskFramework;

namespace Microsoft.Office.Datacenter.ActiveMonitoring.Local
{
	internal interface IIndex<TItem>
	{
		void Add(TItem item, TracingContext traceContext);
	}
}
