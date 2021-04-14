using System;
using Microsoft.Office.CompliancePolicy.Monitor;

namespace Microsoft.Office.CompliancePolicy.Dar
{
	public abstract class DarServiceProvider
	{
		public DarTaskQueue DarTaskQueue
		{
			get
			{
				DarTaskQueue result;
				if ((result = this.darTaskQueue) == null)
				{
					result = (this.darTaskQueue = this.CreateDarTaskQueue());
				}
				return result;
			}
		}

		public DarWorkloadHost DarWorkloadHost
		{
			get
			{
				DarWorkloadHost result;
				if ((result = this.darWorkloadHost) == null)
				{
					result = (this.darWorkloadHost = this.CreateDarWorkloadHost());
				}
				return result;
			}
		}

		public DarTaskAggregateProvider DarTaskAggregateProvider
		{
			get
			{
				DarTaskAggregateProvider result;
				if ((result = this.darTaskAggregateProvider) == null)
				{
					result = (this.darTaskAggregateProvider = this.CreateDarTaskAggregateProvider());
				}
				return result;
			}
		}

		public DarTaskFactory DarTaskFactory
		{
			get
			{
				DarTaskFactory result;
				if ((result = this.darTaskFactory) == null)
				{
					result = (this.darTaskFactory = this.CreateDarTaskFactory());
				}
				return result;
			}
		}

		public ExecutionLog ExecutionLog
		{
			get
			{
				ExecutionLog result;
				if ((result = this.executionLog) == null)
				{
					result = (this.executionLog = this.CreateExecutionLog());
				}
				return result;
			}
		}

		public PerfCounterProvider PerformanceCounter
		{
			get
			{
				PerfCounterProvider result;
				if ((result = this.performanceCounter) == null)
				{
					result = (this.performanceCounter = this.CreatePerformanceCounter());
				}
				return result;
			}
		}

		protected abstract DarTaskQueue CreateDarTaskQueue();

		protected abstract DarWorkloadHost CreateDarWorkloadHost();

		protected abstract DarTaskAggregateProvider CreateDarTaskAggregateProvider();

		protected abstract DarTaskFactory CreateDarTaskFactory();

		protected abstract ExecutionLog CreateExecutionLog();

		protected abstract PerfCounterProvider CreatePerformanceCounter();

		private DarTaskQueue darTaskQueue;

		private DarWorkloadHost darWorkloadHost;

		private DarTaskAggregateProvider darTaskAggregateProvider;

		private DarTaskFactory darTaskFactory;

		private ExecutionLog executionLog;

		private PerfCounterProvider performanceCounter;
	}
}
