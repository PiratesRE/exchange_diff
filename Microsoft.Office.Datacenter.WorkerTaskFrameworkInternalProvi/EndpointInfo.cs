using System;
using System.Data.Linq.Mapping;

namespace Microsoft.Office.Datacenter.WorkerTaskFramework
{
	public abstract class EndpointInfo : TableEntity
	{
		[Column]
		public abstract string Scope { get; set; }

		[Column]
		public abstract bool IsLive { get; set; }
	}
}
