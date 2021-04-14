using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.MessagingPolicies;
using Microsoft.Exchange.Transport.LoggingCommon;

namespace Microsoft.Exchange.MessagingPolicies.Rules
{
	internal class RuleHealthMonitor
	{
		internal string RuleId { get; set; }

		internal string TenantId { get; set; }

		internal RuleHealthMonitor.MtlLogWriterDelegate MtlLogWriter { get; set; }

		internal RuleHealthMonitor.EventLogWriterDelegate EventLogWriter { get; set; }

		internal RuleHealthMonitor(RuleHealthMonitor.ActivityType activityType, long mtlLoggingThresholdMs, long eventLoggingThresholdMs, RuleHealthMonitor.EventLogWriterDelegate eventLogWriter)
		{
			this.activityType = activityType;
			this.mtlLoggingThresholdMs = mtlLoggingThresholdMs;
			this.eventLoggingThresholdMs = eventLoggingThresholdMs;
			this.EventLogWriter = eventLogWriter;
		}

		internal void LogMtl()
		{
			if (this.MtlLogWriter == null)
			{
				return;
			}
			List<KeyValuePair<string, string>> list = new List<KeyValuePair<string, string>>();
			list.Add(new KeyValuePair<string, string>("ruleId", this.RuleId));
			list.Add(new KeyValuePair<string, string>((this.activityType == RuleHealthMonitor.ActivityType.Load) ? "LoadW" : "ExecW", this.wallClockStopwatch.ElapsedMilliseconds.ToString()));
			list.Add(new KeyValuePair<string, string>((this.activityType == RuleHealthMonitor.ActivityType.Load) ? "Loadc" : "ExecC", this.cpuStopwatch.ElapsedMilliseconds.ToString()));
			try
			{
				this.MtlLogWriter(TrackAgentInfoAgentName.TRA.ToString("G"), TrackAgentInfoGroupName.ETRP.ToString("G"), list);
			}
			catch (InvalidOperationException)
			{
				ExTraceGlobals.TransportRulesEngineTracer.TraceWarning(0L, "InvalidOperationException thrown while attempting to log rule execution performance information. Expected when data size to Audit is high.");
			}
		}

		internal void LogEvent()
		{
			if (this.EventLogWriter == null)
			{
				return;
			}
			this.EventLogWriter(string.Format("Rule: {0}, Tenant id: {1}, Time: {2}, Threshold: {3}", new object[]
			{
				string.IsNullOrEmpty(this.RuleId) ? string.Empty : this.RuleId,
				string.IsNullOrEmpty(this.TenantId) ? string.Empty : this.TenantId,
				this.wallClockStopwatch.ElapsedMilliseconds.ToString(),
				this.eventLoggingThresholdMs
			}));
		}

		internal void LogIfThresholdExceeded()
		{
			if (this.mtlLoggingThresholdMs >= 0L && this.wallClockStopwatch.ElapsedMilliseconds >= this.mtlLoggingThresholdMs)
			{
				this.LogMtl();
			}
			if (this.eventLoggingThresholdMs >= 0L && this.wallClockStopwatch.ElapsedMilliseconds >= this.eventLoggingThresholdMs)
			{
				this.LogEvent();
			}
		}

		internal void Reset()
		{
			this.wallClockStopwatch.Reset();
			this.cpuStopwatch.Reset();
		}

		internal void Restart()
		{
			this.wallClockStopwatch.Restart();
			this.cpuStopwatch.Restart();
		}

		internal void Start()
		{
			this.wallClockStopwatch.Start();
			this.cpuStopwatch.Start();
		}

		internal void Stop(bool logIfThresholdExceeded = false)
		{
			this.wallClockStopwatch.Stop();
			this.cpuStopwatch.Stop();
			if (logIfThresholdExceeded)
			{
				this.LogIfThresholdExceeded();
			}
		}

		private readonly long mtlLoggingThresholdMs;

		private readonly long eventLoggingThresholdMs;

		private Stopwatch wallClockStopwatch = new Stopwatch();

		private CpuStopwatch cpuStopwatch = new CpuStopwatch();

		private RuleHealthMonitor.ActivityType activityType;

		internal enum ActivityType
		{
			Load,
			Execute
		}

		internal delegate void MtlLogWriterDelegate(string agentName, string eventTopic, List<KeyValuePair<string, string>> data);

		internal delegate void EventLogWriterDelegate(string eventMessageDetails);
	}
}
