using System;

namespace Microsoft.Office.CompliancePolicy.Monitor
{
	public abstract class PerfCounterProvider
	{
		public PerfCounterProvider(string categoryName)
		{
			ArgumentValidator.ThrowIfNullOrEmpty("categoryName", categoryName);
			this.CategoryName = categoryName;
		}

		public PerfCounterProvider()
		{
		}

		private protected string CategoryName { protected get; private set; }

		public abstract void Increment(string counterName);

		public abstract void Increment(string counterName, string baseCounterName);

		public abstract void IncrementBy(string counterName, long incrementValue);

		public abstract void IncrementBy(string counterName, long incrementValue, string baseCounterName);
	}
}
