using System;
using System.Collections.Generic;
using Microsoft.Exchange.LogAnalyzer.Analyzers.Perflog;
using Microsoft.Exchange.LogAnalyzer.Extensions.Perflog;
using Microsoft.ExLogAnalyzer;

namespace Microsoft.Exchange.Diagnostics.Service.ExchangeJobs.Triggers
{
	public abstract class CASTriggerBase : PerInstanceTrigger
	{
		protected CASTriggerBase(IJob job, string counterNamePattern, PerfLogCounterTrigger.TriggerConfiguration configuration, DiagnosticMeasurement requestsPerSecondMeasurement, double requestsPerSecondThreshold, HashSet<DiagnosticMeasurement> additionalCounters, HashSet<string> excludedInstances) : this(job, counterNamePattern, new CASTriggerBase.CasConfiguration(requestsPerSecondThreshold, configuration, excludedInstances), additionalCounters, requestsPerSecondMeasurement)
		{
		}

		private CASTriggerBase(IJob job, string counterNamePattern, CASTriggerBase.CasConfiguration configuration, HashSet<DiagnosticMeasurement> additionalCounters, DiagnosticMeasurement requestsPerSecondMeasurement) : base(job, counterNamePattern, additionalCounters, configuration)
		{
			this.requestsPerSecondMeasurement = requestsPerSecondMeasurement;
		}

		protected override bool ShouldMonitorCounter(DiagnosticMeasurement counter)
		{
			if (!base.ShouldMonitorCounter(counter))
			{
				return false;
			}
			foreach (string value in base.ExcludedInstances)
			{
				if (counter.InstanceName.Contains(value))
				{
					return false;
				}
			}
			return true;
		}

		protected override bool ShouldTrigger(PerfLogCounterTrigger.SurpassedThresholdContext context)
		{
			bool flag = false;
			bool flag2 = base.ShouldTrigger(context);
			if (flag2)
			{
				DiagnosticMeasurement measure = DiagnosticMeasurement.GetMeasure(context.Counter.MachineName, this.requestsPerSecondMeasurement.ObjectName, this.requestsPerSecondMeasurement.CounterName, context.Counter.InstanceName);
				ValueStatistics valueStatistics;
				if (context.AdditionalData.TryGetValue(measure, out valueStatistics))
				{
					double requestsPerSecondThreshold = ((CASTriggerBase.CasConfiguration)base.Configuration).RequestsPerSecondThreshold;
					float? last = valueStatistics.Last;
					double num = requestsPerSecondThreshold;
					if ((double)last.GetValueOrDefault() >= num && last != null)
					{
						flag = true;
					}
					Log.LogInformationMessage("[CASTriggerBase.ShouldTrigger] Decision is {0} trigger, {1}/s {2} {3}/s for {4}", new object[]
					{
						flag ? "will" : "won't",
						valueStatistics.Last,
						flag ? ">=" : "<",
						requestsPerSecondThreshold,
						context.Counter.ToString()
					});
				}
				else
				{
					Log.LogWarningMessage("[CASTriggerBase.ShouldTrigger] Couldn't find requests per second measurement for {0} (currentMeasurement = {1})", new object[]
					{
						context.Counter.ToString(),
						measure.ToString()
					});
				}
			}
			return flag2 && flag;
		}

		protected override string CollectAdditionalInformation(PerfLogCounterTrigger.SurpassedThresholdContext context)
		{
			string[] array = context.Counter.InstanceName.Split(new char[]
			{
				';'
			});
			return string.Format("${0}|{1}|{2}|{3}$", new object[]
			{
				context.Counter.CounterName,
				array[0],
				array[1],
				(long)context.Value
			});
		}

		private readonly DiagnosticMeasurement requestsPerSecondMeasurement;

		public class CasConfiguration : PerInstanceTrigger.PerInstanceConfiguration
		{
			public CasConfiguration(double requestsPerSecondThreshold, PerfLogCounterTrigger.TriggerConfiguration triggerConfiguration, HashSet<string> excludedInstances) : base(triggerConfiguration, excludedInstances)
			{
				string triggerPrefix = triggerConfiguration.TriggerPrefix;
				string text = triggerPrefix + "RequestsPerSecondThreshold";
				this.defaultRequestsPerSecondThreshold = requestsPerSecondThreshold;
				this.requestsPerSecondThreshold = Configuration.GetConfigDouble(text, double.MinValue, double.MaxValue, requestsPerSecondThreshold);
			}

			public double RequestsPerSecondThreshold
			{
				get
				{
					return this.requestsPerSecondThreshold;
				}
			}

			public override PerfLogCounterTrigger.PerfLogCounterConfiguration Reload()
			{
				return new CASTriggerBase.CasConfiguration(this.defaultRequestsPerSecondThreshold, base.DefaultTriggerConfiguration, base.DefaultExcludedInstances);
			}

			public override int GetHashCode()
			{
				return this.requestsPerSecondThreshold.GetHashCode() ^ base.GetHashCode();
			}

			public override bool Equals(object right)
			{
				return this.Equals(right as CASTriggerBase.CasConfiguration);
			}

			protected bool Equals(CASTriggerBase.CasConfiguration right)
			{
				return right != null && this.requestsPerSecondThreshold.Equals(right.requestsPerSecondThreshold) && base.Equals(right);
			}

			private readonly double defaultRequestsPerSecondThreshold;

			private readonly double requestsPerSecondThreshold;
		}
	}
}
