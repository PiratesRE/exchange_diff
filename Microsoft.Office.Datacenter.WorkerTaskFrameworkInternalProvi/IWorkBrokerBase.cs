using System;

namespace Microsoft.Office.Datacenter.WorkerTaskFramework
{
	public interface IWorkBrokerBase
	{
		TimeSpan DefaultResultWindow { get; }

		bool IsLocal();
	}
}
