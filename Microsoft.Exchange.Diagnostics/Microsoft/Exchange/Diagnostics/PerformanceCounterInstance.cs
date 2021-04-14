using System;
using System.Xml.Linq;

namespace Microsoft.Exchange.Diagnostics
{
	public abstract class PerformanceCounterInstance
	{
		public string CategoryName
		{
			get
			{
				return this.categoryName;
			}
		}

		public string Name
		{
			get
			{
				return this.name;
			}
		}

		public void Remove()
		{
			this.counters[0].RemoveInstance();
		}

		public void Reset()
		{
			foreach (ExPerformanceCounter exPerformanceCounter in this.counters)
			{
				exPerformanceCounter.Reset();
			}
		}

		public void Close()
		{
			foreach (ExPerformanceCounter exPerformanceCounter in this.counters)
			{
				exPerformanceCounter.Close();
			}
		}

		public ExPerformanceCounter GetCounterOfName(string counterName)
		{
			foreach (ExPerformanceCounter exPerformanceCounter in this.counters)
			{
				if (exPerformanceCounter.CounterName.Equals(counterName, StringComparison.CurrentCultureIgnoreCase))
				{
					return exPerformanceCounter;
				}
			}
			return null;
		}

		public virtual void GetPerfCounterDiagnosticsInfo(XElement element)
		{
		}

		protected PerformanceCounterInstance(string name, string categoryName)
		{
			this.name = name;
			this.categoryName = categoryName;
		}

		protected ExPerformanceCounter[] counters;

		private string name;

		private string categoryName;
	}
}
