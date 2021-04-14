using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Microsoft.Exchange.MessagingPolicies.Rules
{
	internal class TransportRulesCostMonitor
	{
		internal TransportRulesCostMonitor.CostReportingDelegate CostReporter { get; set; }

		internal TransportRulesCostMonitor(TransportRulesAgentCostComponents component)
		{
			this.Start(component);
		}

		internal void IncrementComponentProcessingCost(TimeSpan processingCost, TransportRulesAgentCostComponents component)
		{
			this.SetComponentProcessingCost(processingCost.Add(this.GetComponentProcessingCost(component)), component);
		}

		internal void SetComponentProcessingCost(TimeSpan processingCost, TransportRulesAgentCostComponents component)
		{
			this.componentCosts[component] = processingCost;
		}

		internal TimeSpan GetComponentProcessingCost(TransportRulesAgentCostComponents component)
		{
			TimeSpan result;
			if (!this.componentCosts.TryGetValue(component, out result))
			{
				return TimeSpan.Zero;
			}
			return result;
		}

		internal TimeSpan GetAggregatedProcessingCost()
		{
			return this.componentCosts.Aggregate(TimeSpan.Zero, (TimeSpan current, KeyValuePair<TransportRulesAgentCostComponents, TimeSpan> componentCost) => current + componentCost.Value);
		}

		private void Reset()
		{
			this.wallClockStopwatch.Reset();
		}

		internal void Start(TransportRulesAgentCostComponents component)
		{
			if (this.currentComponent != null)
			{
				this.Stop();
			}
			this.currentComponent = new TransportRulesAgentCostComponents?(component);
			this.wallClockStopwatch.Start();
		}

		internal void Stop()
		{
			this.wallClockStopwatch.Stop();
			if (this.currentComponent != null)
			{
				this.IncrementComponentProcessingCost(this.wallClockStopwatch.Elapsed, this.currentComponent.Value);
				if (this.CostReporter != null)
				{
					Tuple<TimeSpan, string> aggregatedCostForLogging = this.GetAggregatedCostForLogging();
					this.CostReporter((long)aggregatedCostForLogging.Item1.TotalSeconds, aggregatedCostForLogging.Item2);
				}
			}
			this.Reset();
		}

		internal void StopAndSetReporter(TransportRulesCostMonitor.CostReportingDelegate costReporter)
		{
			this.Stop();
			this.CostReporter = costReporter;
		}

		internal Tuple<TimeSpan, string> GetAggregatedCostForLogging()
		{
			StringBuilder stringBuilder = new StringBuilder();
			TimeSpan timeSpan = TimeSpan.Zero;
			foreach (object obj in Enum.GetValues(typeof(TransportRulesAgentCostComponents)))
			{
				TransportRulesAgentCostComponents transportRulesAgentCostComponents = (TransportRulesAgentCostComponents)obj;
				TimeSpan componentProcessingCost = this.GetComponentProcessingCost(transportRulesAgentCostComponents);
				if (componentProcessingCost.CompareTo(TimeSpan.Zero) != 0)
				{
					stringBuilder.Append(string.Format("{0}={1},", transportRulesAgentCostComponents.ToString(), (long)componentProcessingCost.TotalMilliseconds));
					timeSpan += componentProcessingCost;
				}
			}
			string item = string.Empty;
			if (stringBuilder.Length > 0)
			{
				item = stringBuilder.ToString(0, stringBuilder.Length - 1);
			}
			return new Tuple<TimeSpan, string>(timeSpan, item);
		}

		private Stopwatch wallClockStopwatch = new Stopwatch();

		private TransportRulesAgentCostComponents? currentComponent = null;

		private Dictionary<TransportRulesAgentCostComponents, TimeSpan> componentCosts = new Dictionary<TransportRulesAgentCostComponents, TimeSpan>();

		internal delegate void CostReportingDelegate(long cost, string costInfo);
	}
}
