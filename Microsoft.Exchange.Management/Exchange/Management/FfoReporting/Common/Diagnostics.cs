using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Management.Common;
using Microsoft.Exchange.Management.EventMessages;
using Microsoft.Office.Datacenter.ActiveMonitoring;

namespace Microsoft.Exchange.Management.FfoReporting.Common
{
	internal sealed class Diagnostics
	{
		static Diagnostics()
		{
			if (!Diagnostics.staticsInitialized)
			{
				lock (Diagnostics.syncobject)
				{
					if (!Diagnostics.staticsInitialized)
					{
						SystemProbe.Start();
						Diagnostics.staticsInitialized = true;
					}
				}
			}
		}

		public Diagnostics(string componentName)
		{
			this.ComponentName = componentName;
		}

		internal string ComponentName { get; private set; }

		internal string ActivityId { get; set; }

		internal void Checkpoint(string msg)
		{
			this.lastKnownCheckpoint = msg;
		}

		internal void TraceWarning(string msg)
		{
			this.warnings.Add(msg);
		}

		internal void TraceError(string msg)
		{
			this.Trace(SystemProbe.Status.Fail, msg);
		}

		internal void TraceException(string msg, Exception exception)
		{
			string text = string.Format(string.Format("{0}\n{1}", msg, Schema.Utilities.GenerateDetailedError(exception)), new object[0]);
			ExManagementApplicationLogger.LogEvent(ManagementEventLogConstants.Tuple_FfoReportingTaskFailure, new string[]
			{
				text
			});
			string msg2 = string.Format("{0}:{1}", msg, exception.Message);
			this.Trace(SystemProbe.Status.Fail, msg2);
		}

		internal void SetHealthGreen(string monitor, string msg)
		{
			FaultInjection.FaultInjectionTracer.TraceTest(39836U);
			this.Checkpoint(msg);
			this.Trace(SystemProbe.Status.Pass, string.Empty);
			EventNotificationItem.Publish(this.ComponentName, monitor, null, msg, ResultSeverityLevel.Informational, false);
		}

		internal void SetHealthRed(string monitor, string msg, Exception exception)
		{
			FaultInjection.FaultInjectionTracer.TraceTest(64160U);
			this.TraceException(msg, exception);
			EventNotificationItem.Publish(this.ComponentName, monitor, null, msg, ResultSeverityLevel.Error, false);
		}

		internal void StartTimer(string id)
		{
			this.timings[id] = new Diagnostics.Timing(id, this.timer.ElapsedMilliseconds);
		}

		internal void StopTimer(string id)
		{
			this.timings[id].Stop(this.timer.ElapsedMilliseconds);
			this.Checkpoint(id);
		}

		private void Trace(SystemProbe.Status status, string msg)
		{
			this.timer.Stop();
			if (!string.IsNullOrEmpty(this.ActivityId))
			{
				SystemProbe.ActivityId = new Guid(this.ActivityId);
			}
			StringBuilder stringBuilder = new StringBuilder(this.lastKnownCheckpoint);
			if (!string.IsNullOrEmpty(msg))
			{
				stringBuilder.AppendFormat(" Msg:{0}", msg);
			}
			if (this.warnings.Count > 0)
			{
				stringBuilder.AppendFormat(" Warning:{0}", string.Join(",", this.warnings));
			}
			stringBuilder.AppendFormat(" Total:{0}", this.timer.ElapsedMilliseconds);
			foreach (Diagnostics.Timing timing in this.timings.Values)
			{
				timing.Stop(this.timer.ElapsedMilliseconds);
				stringBuilder.AppendFormat(" {0}", timing);
			}
			SystemProbe.Trace("FFO-RWS-FfoReportingTask", status, stringBuilder.ToString(), new object[0]);
		}

		internal const string FfoDALRetrievalEvent = "FFO DAL Retrieval Status Monitor";

		internal const string FfoReportingEvent = "FFO Reporting Task Status Monitor";

		internal const string MailFilterListEvent = "FFO GetFilterValueList Status Monitor";

		internal const string FfoSmtpCheckerEvent = "FfoReporting.SmtpChecker";

		private const string ComponentTraceName = "FFO-RWS-FfoReportingTask";

		private static readonly object syncobject = new object();

		private static volatile bool staticsInitialized;

		private string lastKnownCheckpoint = string.Empty;

		private Dictionary<string, Diagnostics.Timing> timings = new Dictionary<string, Diagnostics.Timing>();

		private Stopwatch timer = Stopwatch.StartNew();

		private List<string> warnings = new List<string>();

		internal static class Checkpoints
		{
			internal const string Authentication = "Authentication";

			internal const string Conversion = "Conversion";

			internal const string DalAccess = "DalAccess";

			internal const string Validation = "Validation";

			internal const string WriteObject = "WriteObject";
		}

		private sealed class Timing
		{
			internal Timing(string id, long startTime)
			{
				this.Id = id;
				this.startTime = startTime;
			}

			public string Id { get; private set; }

			public void Stop(long endTime)
			{
				if (this.endTime == null)
				{
					this.endTime = new long?(endTime);
				}
			}

			public override string ToString()
			{
				string arg = (this.endTime != null) ? (this.endTime.Value - this.startTime).ToString() : "?";
				return string.Format("{0}:{1}", this.Id, arg);
			}

			private readonly long startTime;

			private long? endTime = null;
		}
	}
}
