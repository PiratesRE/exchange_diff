using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Microsoft.Forefront.Monitoring.ActiveMonitoring.Smtp.Probes
{
	public class SmtpConnectionProbeAnalytics
	{
		public SmtpConnectionProbeAnalytics()
		{
			this.latencyContributions = new List<SmtpConnectionProbeLatency>();
			this.needsToBeRecalculated = true;
			this.oneStandardDeviation = new List<SmtpConnectionProbeLatency>();
			this.twoOrMoreStandardDeviation = new List<SmtpConnectionProbeLatency>();
		}

		public long Mean
		{
			get
			{
				this.RunAnalysis();
				return this.mean;
			}
			private set
			{
				this.mean = value;
			}
		}

		public long StandardDeviation
		{
			get
			{
				this.RunAnalysis();
				return this.standardDeviation;
			}
			private set
			{
				this.standardDeviation = value;
			}
		}

		internal List<SmtpConnectionProbeLatency> OneStandardDeviation
		{
			get
			{
				this.RunAnalysis();
				return this.oneStandardDeviation;
			}
			private set
			{
				this.oneStandardDeviation = value;
			}
		}

		internal List<SmtpConnectionProbeLatency> TwoOrMoreStandardDeviation
		{
			get
			{
				this.RunAnalysis();
				return this.twoOrMoreStandardDeviation;
			}
			private set
			{
				this.twoOrMoreStandardDeviation = value;
			}
		}

		internal static long GetMean(List<SmtpConnectionProbeLatency> latencies)
		{
			return (long)latencies.Average((SmtpConnectionProbeLatency l) => l.Latency);
		}

		internal static long GetStandardDeviation(long mean, List<SmtpConnectionProbeLatency> latencies)
		{
			return (long)Math.Sqrt(latencies.Average((SmtpConnectionProbeLatency l) => l.Latency - mean ^ 2L));
		}

		internal static List<SmtpConnectionProbeLatency> GetLatenciesOneStandardDeviationAboveMean(long mean, long standardDeviation, List<SmtpConnectionProbeLatency> latencies)
		{
			return new List<SmtpConnectionProbeLatency>(from latency in latencies
			where latency.Latency >= mean + standardDeviation && latency.Latency < mean + standardDeviation + standardDeviation
			orderby latency.Latency descending
			select latency);
		}

		internal static List<SmtpConnectionProbeLatency> GetLatenciesTwoOrMoreStandardDeviationAboveMean(long mean, long standardDeviation, List<SmtpConnectionProbeLatency> latencies)
		{
			return new List<SmtpConnectionProbeLatency>(from latency in latencies
			where latency.Latency >= mean + standardDeviation + standardDeviation
			orderby latency.Latency descending
			select latency);
		}

		internal void AddLatency(SmtpConnectionProbeLatency latency)
		{
			this.latencyContributions.Add(latency);
			this.needsToBeRecalculated = true;
		}

		internal SmtpConnectionProbeLatency GetHighestLatencyValue()
		{
			return (from latency in this.latencyContributions
			where latency.Latency == this.latencyContributions.Max((SmtpConnectionProbeLatency l) => l.Latency)
			select latency).FirstOrDefault<SmtpConnectionProbeLatency>();
		}

		internal virtual void RunAnalysis()
		{
			if (this.needsToBeRecalculated && this.latencyContributions.Count > 0)
			{
				this.mean = SmtpConnectionProbeAnalytics.GetMean(this.latencyContributions);
				this.standardDeviation = SmtpConnectionProbeAnalytics.GetStandardDeviation(this.mean, this.latencyContributions);
				this.oneStandardDeviation = SmtpConnectionProbeAnalytics.GetLatenciesOneStandardDeviationAboveMean(this.mean, this.standardDeviation, this.latencyContributions);
				this.twoOrMoreStandardDeviation = SmtpConnectionProbeAnalytics.GetLatenciesTwoOrMoreStandardDeviationAboveMean(this.mean, this.standardDeviation, this.latencyContributions);
			}
			this.needsToBeRecalculated = false;
		}

		internal virtual string GenerateLatencyAnalysis()
		{
			StringBuilder sb = new StringBuilder();
			if (this.OneStandardDeviation.Count == 0 && this.TwoOrMoreStandardDeviation.Count == 0)
			{
				sb.AppendFormat("No latencies were above the standard deviation. Mean: {0} Standard Deviation: {1}", this.Mean, this.StandardDeviation);
			}
			else
			{
				sb.AppendFormat("{0} latencies were above the standard deviation. Mean: {1} Standard Deviation: {2}. ", this.OneStandardDeviation.Count + this.TwoOrMoreStandardDeviation.Count, this.Mean, this.StandardDeviation);
				this.TwoOrMoreStandardDeviation.ForEach(delegate(SmtpConnectionProbeLatency latency)
				{
					sb.AppendFormat("{0} ({1}, Two) ", latency.Reason, latency.Latency);
				});
				this.OneStandardDeviation.ForEach(delegate(SmtpConnectionProbeLatency latency)
				{
					sb.AppendFormat("{0} ({1}, One) ", latency.Reason, latency.Latency);
				});
			}
			return sb.ToString();
		}

		internal string LatencySummary()
		{
			return string.Join<SmtpConnectionProbeLatency>(";", this.latencyContributions);
		}

		private List<SmtpConnectionProbeLatency> latencyContributions;

		private bool needsToBeRecalculated;

		private List<SmtpConnectionProbeLatency> oneStandardDeviation;

		private List<SmtpConnectionProbeLatency> twoOrMoreStandardDeviation;

		private long mean;

		private long standardDeviation;
	}
}
