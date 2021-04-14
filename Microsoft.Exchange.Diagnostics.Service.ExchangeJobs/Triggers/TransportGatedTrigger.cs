using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Exchange.LogAnalyzer.Analyzers.Perflog;
using Microsoft.Exchange.LogAnalyzer.Extensions.Perflog;
using Microsoft.ExLogAnalyzer;

namespace Microsoft.Exchange.Diagnostics.Service.ExchangeJobs.Triggers
{
	public abstract class TransportGatedTrigger : PerInstanceTrigger
	{
		protected TransportGatedTrigger(IJob job, string counterNamePattern, double gatingCounterThreshold, PerfLogCounterTrigger.TriggerConfiguration configuration, HashSet<DiagnosticMeasurement> additionalCounters, HashSet<string> excludedInstances) : this(job, counterNamePattern, additionalCounters, new TransportGatedTrigger.TransportGatedConfiguration(gatingCounterThreshold, configuration, excludedInstances))
		{
		}

		protected TransportGatedTrigger(IJob job, string counterNamePattern, HashSet<DiagnosticMeasurement> additionalCounters, TransportGatedTrigger.TransportGatedConfiguration configuration) : base(job, counterNamePattern, additionalCounters, configuration)
		{
		}

		protected double GatingCounterThreshold
		{
			get
			{
				return ((TransportGatedTrigger.TransportGatedConfiguration)base.Configuration).GatingCounterThreshold;
			}
		}

		protected abstract DiagnosticMeasurement AdditionalDiagnosticMeasurement(PerfLogCounterTrigger.SurpassedThresholdContext context);

		protected override string CollectAdditionalInformation(PerfLogCounterTrigger.SurpassedThresholdContext context)
		{
			StringBuilder stringBuilder = new StringBuilder();
			DiagnosticMeasurement diagnosticMeasurement = this.AdditionalDiagnosticMeasurement(context);
			ValueStatistics valueStatistics;
			if (diagnosticMeasurement != null && context.AdditionalData.TryGetValue(diagnosticMeasurement, out valueStatistics))
			{
				string arg = DiagnosticMeasurement.FormatMeasureName(diagnosticMeasurement.MachineName, diagnosticMeasurement.ObjectName, diagnosticMeasurement.CounterName, diagnosticMeasurement.InstanceName);
				string format = "Secondary performance counter '{0}' value is '{1}'";
				stringBuilder.AppendFormat(format, arg, valueStatistics.Last);
			}
			if (stringBuilder.Length != 0)
			{
				return stringBuilder.ToString();
			}
			return base.CollectAdditionalInformation(context);
		}

		public class TransportGatedConfiguration : PerInstanceTrigger.PerInstanceConfiguration
		{
			public TransportGatedConfiguration(double gatingCounterThreshold, PerfLogCounterTrigger.TriggerConfiguration triggerConfiguration, HashSet<string> excludedInstances) : base(triggerConfiguration, excludedInstances)
			{
				string triggerPrefix = triggerConfiguration.TriggerPrefix;
				string text = triggerPrefix + "GatingCounterThreshold";
				this.defaultGatingCounterThreshold = gatingCounterThreshold;
				this.gatingCounterThreshold = Configuration.GetConfigDouble(text, double.MinValue, double.MaxValue, gatingCounterThreshold);
			}

			public double GatingCounterThreshold
			{
				get
				{
					return this.gatingCounterThreshold;
				}
			}

			public override PerfLogCounterTrigger.PerfLogCounterConfiguration Reload()
			{
				return new TransportGatedTrigger.TransportGatedConfiguration(this.defaultGatingCounterThreshold, base.DefaultTriggerConfiguration, base.DefaultExcludedInstances);
			}

			public override int GetHashCode()
			{
				return this.gatingCounterThreshold.GetHashCode() ^ base.GetHashCode();
			}

			public override bool Equals(object right)
			{
				return this.Equals(right as TransportGatedTrigger.TransportGatedConfiguration);
			}

			protected bool Equals(TransportGatedTrigger.TransportGatedConfiguration right)
			{
				return right != null && this.gatingCounterThreshold.Equals(right.gatingCounterThreshold) && base.Equals(right);
			}

			private readonly double defaultGatingCounterThreshold;

			private readonly double gatingCounterThreshold;
		}
	}
}
