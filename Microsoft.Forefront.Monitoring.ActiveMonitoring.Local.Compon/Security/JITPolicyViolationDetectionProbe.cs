using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Office.Datacenter.ActiveMonitoring;

namespace Microsoft.Forefront.Monitoring.ActiveMonitoring.Security
{
	public class JITPolicyViolationDetectionProbe : ProbeWorkItem
	{
		protected override void DoWork(CancellationToken cancellationToken)
		{
			DateTime executionStartTime = base.Result.ExecutionStartTime;
			DateTime monitorEndLocalTime = executionStartTime.ToLocalTime();
			ProbeResult lastProbeResult = this.GetLastProbeResult(cancellationToken);
			DateTime dateTime = (lastProbeResult != null) ? lastProbeResult.ExecutionStartTime : executionStartTime.AddSeconds((double)(-(double)base.Definition.RecurrenceIntervalSeconds));
			DateTime monitorStartLocalTime = dateTime.ToLocalTime();
			using (EventLog eventLog = new EventLog("Security"))
			{
				IEnumerable<EventLogEntry> enumerable = from EventLogEntry e in eventLog.Entries
				where e.InstanceId == 5136L && (long)e.CategoryNumber == 14081L && e.TimeGenerated >= monitorStartLocalTime && e.TimeGenerated < monitorEndLocalTime
				orderby e.TimeGenerated
				select e;
				foreach (EventLogEntry eventLogEntry in enumerable)
				{
					Match match = Regex.Match(eventLogEntry.Message, "Security ID:[\\s]+(?<accountid>[^\\r]+)[\\s]+Account Name:[\\s]+(?<accountname>[^\\r]+)[\\s]+Account Domain:[\\s]+(?<domain>[^\\r]+)[\\s\\S]+DN:[\\s\\S]+CN=(?<jitaccount>[\\S\\s]+_j),CN=[\\s\\S]+LDAP Display Name:\textensionAttribute13[\\s\\S]+Value:[\\s]+(?<time>[^\\r]+)");
					if (match.Success)
					{
						string value = match.Groups["accountid"].Value;
						string value2 = match.Groups["accountname"].Value;
						string value3 = match.Groups["domain"].Value;
						string value4 = match.Groups["jitaccount"].Value;
						string value5 = match.Groups["time"].Value;
						if (!value2.Equals("SYSTEM"))
						{
							base.Result.ExecutionContext = eventLogEntry.Message;
							throw new Exception(string.Format("JIT Policy Violation(Duration time update by accounts other than SYSTEM, JIT Account :'{0}' Violation Account:({1},{2}/{3}).  Msg:{4}", new object[]
							{
								value4,
								value,
								value2,
								value3,
								eventLogEntry.Message
							}));
						}
						DateTime dateTime2 = eventLogEntry.TimeGenerated.ToUniversalTime().AddHours(48.0);
						DateTime dateTime3;
						if (!DateTime.TryParse(value5, out dateTime3))
						{
							base.Result.ExecutionContext = eventLogEntry.Message;
							throw new Exception(string.Format("JIT Policy Violation(Invalid Duration Format): JIT Account '{0}' ExpirationTime: '{1}' by  Account:({2},{3}/{4}). Msg:{5}", new object[]
							{
								value4,
								value5,
								value,
								value2,
								value3,
								eventLogEntry.Message
							}));
						}
						if (!(dateTime3 <= dateTime2))
						{
							base.Result.ExecutionContext = eventLogEntry.Message;
							throw new Exception(string.Format("JIT Policy Violation(Elevation duration greater than 48 hours): JIT Account :'{0}'  ExpirationTimeUTC: '{1}' PossibleTimeLimitUTC: '{2}' by  Account:({3},{4}/{5}). Msg:{6}", new object[]
							{
								value4,
								dateTime3,
								dateTime2,
								value,
								value2,
								value3,
								eventLogEntry.Message
							}));
						}
					}
				}
			}
			ProbeResult result = base.Result;
			result.ExecutionContext += string.Format("No violation detected in UTC time range [{0}, {1}].", dateTime, executionStartTime);
		}

		private ProbeResult GetLastProbeResult(CancellationToken cancellationToken)
		{
			ProbeResult lastProbeResult = null;
			IEnumerable<ProbeResult> query = (from result in base.Broker.GetProbeResults(base.Definition, base.Result.ExecutionStartTime.AddSeconds((double)(-2 * base.Definition.RecurrenceIntervalSeconds)))
			where !string.IsNullOrEmpty(result.ExecutionContext)
			orderby result.ExecutionStartTime descending
			select result).Take(1);
			Task<int> task = base.Broker.AsDataAccessQuery<ProbeResult>(query).ExecuteAsync(delegate(ProbeResult probeResult)
			{
				if (lastProbeResult == null)
				{
					lastProbeResult = probeResult;
				}
			}, cancellationToken, base.TraceContext);
			task.Wait(cancellationToken);
			return lastProbeResult;
		}

		private const long AttributeChangeInstanceId = 5136L;

		private const long DirectoryServiceChangesCategory = 14081L;
	}
}
