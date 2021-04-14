using System;
using System.Diagnostics;

namespace Microsoft.Exchange.Diagnostics
{
	public interface IPerformanceCounter : IDisposable
	{
		string CategoryName { get; set; }

		string CounterName { get; set; }

		string CounterHelp { get; }

		string InstanceName { get; set; }

		bool ReadOnly { get; set; }

		PerformanceCounterInstanceLifetime InstanceLifetime { get; set; }

		PerformanceCounterType CounterType { get; }

		long RawValue { get; set; }

		void Close();

		long IncrementBy(long incrementValue);

		float NextValue();

		void RemoveInstance();
	}
}
