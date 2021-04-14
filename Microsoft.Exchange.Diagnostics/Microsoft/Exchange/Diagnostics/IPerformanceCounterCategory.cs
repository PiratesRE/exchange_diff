using System;

namespace Microsoft.Exchange.Diagnostics
{
	public interface IPerformanceCounterCategory
	{
		bool InstanceExists(string instanceName);

		string[] GetInstanceNames();
	}
}
