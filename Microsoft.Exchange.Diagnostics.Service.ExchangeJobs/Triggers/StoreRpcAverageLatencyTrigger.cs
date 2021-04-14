using System;
using System.Collections.Generic;
using Microsoft.Exchange.LogAnalyzer.Analyzers.Perflog;
using Microsoft.Exchange.LogAnalyzer.Extensions.Perflog;
using Microsoft.ExLogAnalyzer;

namespace Microsoft.Exchange.Diagnostics.Service.ExchangeJobs.Triggers
{
	public class StoreRpcAverageLatencyTrigger : PerInstanceTrigger
	{
		static StoreRpcAverageLatencyTrigger()
		{
			StoreRpcAverageLatencyTrigger.excludedInstances = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
			{
				"_Total"
			};
			StoreRpcAverageLatencyTrigger.additionalContext = new HashSet<DiagnosticMeasurement>(DiagnosticMeasurement.CounterFilterComparer.Comparer)
			{
				StoreRpcAverageLatencyTrigger.rpcOperationPerSecond
			};
		}

		public StoreRpcAverageLatencyTrigger(IJob job) : this(job, new StoreRpcAverageLatencyTrigger.MbxConfiguration(new PerfLogCounterTrigger.TriggerConfiguration("StoreRpcAverageLatencyTrigger", 70.0, 150.0, TimeSpan.FromMinutes(5.0), TimeSpan.FromMinutes(10.0), TimeSpan.FromMinutes(5.0), 0), StoreRpcAverageLatencyTrigger.excludedInstances))
		{
		}

		public StoreRpcAverageLatencyTrigger(IJob job, StoreRpcAverageLatencyTrigger.MbxConfiguration configuration) : base(job, "MSExchangeIS Store\\(.+?\\)\\\\RPC Average Latency", StoreRpcAverageLatencyTrigger.additionalContext, configuration)
		{
		}

		protected override bool ShouldTrigger(PerfLogCounterTrigger.SurpassedThresholdContext context)
		{
			bool flag = false;
			bool flag2 = base.ShouldTrigger(context);
			if (flag2)
			{
				DiagnosticMeasurement measure = DiagnosticMeasurement.GetMeasure(context.Counter.MachineName, StoreRpcAverageLatencyTrigger.rpcOperationPerSecond.ObjectName, StoreRpcAverageLatencyTrigger.rpcOperationPerSecond.CounterName, context.Counter.InstanceName);
				ValueStatistics valueStatistics;
				if (context.AdditionalData.TryGetValue(measure, out valueStatistics))
				{
					double rpcOperationPerSecondThreshold = ((StoreRpcAverageLatencyTrigger.MbxConfiguration)base.Configuration).RpcOperationPerSecondThreshold;
					float? last = valueStatistics.Last;
					double num = rpcOperationPerSecondThreshold;
					if ((double)last.GetValueOrDefault() >= num && last != null)
					{
						flag = true;
					}
				}
			}
			return flag2 && flag;
		}

		private static readonly HashSet<string> excludedInstances;

		private static readonly HashSet<DiagnosticMeasurement> additionalContext;

		private static readonly DiagnosticMeasurement rpcOperationPerSecond = DiagnosticMeasurement.GetMeasure("MSExchangeIS Store", "RPC Operations/sec");

		public class MbxConfiguration : PerInstanceTrigger.PerInstanceConfiguration
		{
			public MbxConfiguration(PerfLogCounterTrigger.TriggerConfiguration triggerConfiguration, HashSet<string> excludedInstances) : base(triggerConfiguration, excludedInstances)
			{
				string triggerPrefix = triggerConfiguration.TriggerPrefix;
				string text = triggerPrefix + "RpcOperationsPerSecondThreshold";
				this.rpcOperationPerSecondThreshold = Configuration.GetConfigDouble(text, double.MinValue, double.MaxValue, 50.0);
			}

			public double RpcOperationPerSecondThreshold
			{
				get
				{
					return this.rpcOperationPerSecondThreshold;
				}
			}

			public override PerfLogCounterTrigger.PerfLogCounterConfiguration Reload()
			{
				return new StoreRpcAverageLatencyTrigger.MbxConfiguration(base.DefaultTriggerConfiguration, base.DefaultExcludedInstances);
			}

			public override int GetHashCode()
			{
				return this.rpcOperationPerSecondThreshold.GetHashCode() ^ base.GetHashCode();
			}

			public override bool Equals(object right)
			{
				return this.Equals(right as StoreRpcAverageLatencyTrigger.MbxConfiguration);
			}

			protected bool Equals(StoreRpcAverageLatencyTrigger.MbxConfiguration right)
			{
				return right != null && this.rpcOperationPerSecondThreshold.Equals(right.rpcOperationPerSecondThreshold) && base.Equals(right);
			}

			private readonly double rpcOperationPerSecondThreshold;
		}
	}
}
