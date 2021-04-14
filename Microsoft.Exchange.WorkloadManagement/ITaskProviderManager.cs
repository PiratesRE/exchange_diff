using System;

namespace Microsoft.Exchange.WorkloadManagement
{
	internal interface ITaskProviderManager
	{
		ITaskProvider GetNextProvider();

		int GetProviderCount();
	}
}
