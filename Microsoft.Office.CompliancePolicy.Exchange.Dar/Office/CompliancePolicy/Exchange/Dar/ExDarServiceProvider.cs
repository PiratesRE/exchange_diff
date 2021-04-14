using System;
using Microsoft.Exchange.Servicelets.CommonCode;
using Microsoft.Office.CompliancePolicy.Dar;
using Microsoft.Office.CompliancePolicy.Monitor;

namespace Microsoft.Office.CompliancePolicy.Exchange.Dar
{
	internal class ExDarServiceProvider : DarServiceProvider
	{
		protected override DarTaskAggregateProvider CreateDarTaskAggregateProvider()
		{
			return new ExDarTaskAggregateProvider(this);
		}

		protected override DarTaskFactory CreateDarTaskFactory()
		{
			return new ExDarTaskFactory(this);
		}

		protected override DarTaskQueue CreateDarTaskQueue()
		{
			return new ExDarTaskQueue(this);
		}

		protected override DarWorkloadHost CreateDarWorkloadHost()
		{
			return new ExDarWorkloadHost(this);
		}

		protected override ExecutionLog CreateExecutionLog()
		{
			return new ExExecutionLog(this);
		}

		protected override PerfCounterProvider CreatePerformanceCounter()
		{
			if (this.performanceCounter == null)
			{
				lock (this.performanceCounterSingletonLock)
				{
					if (this.performanceCounter == null)
					{
						this.performanceCounter = new ExPerfCounterProvider("MSUnified Compliance Sync", UnifiedPolicySyncPerfCounters.AllCounters);
					}
				}
			}
			return this.performanceCounter;
		}

		private ExPerfCounterProvider performanceCounter;

		private object performanceCounterSingletonLock = new object();
	}
}
