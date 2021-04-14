using System;
using System.Collections.Generic;
using Microsoft.Exchange.LogAnalyzer.Analyzers.Perflog;
using Microsoft.Exchange.LogAnalyzer.Extensions.Perflog;
using Microsoft.ExLogAnalyzer;

namespace Microsoft.Exchange.Diagnostics.Service.ExchangeJobs.Triggers
{
	public class OAuthAcsTimeoutTrigger : PerInstanceTrigger
	{
		static OAuthAcsTimeoutTrigger()
		{
			OAuthAcsTimeoutTrigger.excludedInstances = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
			{
				"_Total"
			};
			OAuthAcsTimeoutTrigger.additionalContext = new HashSet<DiagnosticMeasurement>(DiagnosticMeasurement.CounterFilterComparer.Comparer)
			{
				OAuthAcsTimeoutTrigger.totalOAuthAcsTimeoutRequests
			};
		}

		public OAuthAcsTimeoutTrigger(IJob job) : base(job, "MSExchange OAuth\\(.+?\\)\\\\Outbound: Total Timeout Token Requests to AuthServer", new PerfLogCounterTrigger.TriggerConfiguration("OAuthAcsTimeoutTrigger", double.NaN, 1.0, TimeSpan.FromMinutes(10.0), TimeSpan.FromMinutes(10.0), TimeSpan.FromMinutes(10.0), 0), OAuthAcsTimeoutTrigger.additionalContext, OAuthAcsTimeoutTrigger.excludedInstances)
		{
			this.lastValueOfTotalOAuthAcsTimeoutRequests = new float?(0f);
		}

		protected override bool ShouldTrigger(PerfLogCounterTrigger.SurpassedThresholdContext context)
		{
			bool result = false;
			bool flag = base.ShouldTrigger(context);
			if (flag)
			{
				DiagnosticMeasurement measure = DiagnosticMeasurement.GetMeasure(context.Counter.MachineName, OAuthAcsTimeoutTrigger.totalOAuthAcsTimeoutRequests.ObjectName, OAuthAcsTimeoutTrigger.totalOAuthAcsTimeoutRequests.CounterName, context.Counter.InstanceName);
				ValueStatistics valueStatistics;
				if (context.AdditionalData.TryGetValue(measure, out valueStatistics))
				{
					float? last = valueStatistics.Last;
					float? num = this.lastValueOfTotalOAuthAcsTimeoutRequests;
					float? num2 = (last != null & num != null) ? new float?(last.GetValueOrDefault() - num.GetValueOrDefault()) : null;
					float? num3 = num2;
					if (num3.GetValueOrDefault() > 5f && num3 != null)
					{
						float? num4 = this.lastValueOfTotalOAuthAcsTimeoutRequests;
						if (num4.GetValueOrDefault() != 0f || num4 == null)
						{
							result = true;
						}
					}
					this.lastValueOfTotalOAuthAcsTimeoutRequests = valueStatistics.Last;
				}
			}
			return result;
		}

		private static readonly HashSet<string> excludedInstances;

		private static readonly HashSet<DiagnosticMeasurement> additionalContext;

		private static readonly DiagnosticMeasurement totalOAuthAcsTimeoutRequests = DiagnosticMeasurement.GetMeasure("MSExchange OAuth", "Outbound: Total Timeout Token Requests to AuthServer");

		private float? lastValueOfTotalOAuthAcsTimeoutRequests;
	}
}
