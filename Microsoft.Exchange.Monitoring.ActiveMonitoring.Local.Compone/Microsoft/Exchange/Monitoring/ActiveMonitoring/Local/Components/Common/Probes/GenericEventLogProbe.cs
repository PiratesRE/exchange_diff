using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using Microsoft.Exchange.Diagnostics.Components.ActiveMonitoring;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.Common;
using Microsoft.Office.Datacenter.ActiveMonitoring;
using Microsoft.Office.Datacenter.WorkerTaskFramework;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.Local.Components.Common.Probes
{
	public class GenericEventLogProbe : ProbeWorkItem
	{
		public static ProbeDefinition CreateDefinition(string name, string serviceName, string logName, string providerName, int[] greenEventIds, int[] redEventIds, int recurrenceInterval, int timeout, int maxRetry)
		{
			if (redEventIds == null || redEventIds.Length < 1)
			{
				throw new Exception("GenericEventLogProbe: Red Event ID list must not be NULL nor contains no elements!");
			}
			ProbeDefinition probeDefinition = new ProbeDefinition();
			probeDefinition.AssemblyPath = Assembly.GetExecutingAssembly().Location;
			probeDefinition.ServiceName = serviceName;
			probeDefinition.TypeName = typeof(GenericEventLogProbe).FullName;
			probeDefinition.Name = name;
			probeDefinition.RecurrenceIntervalSeconds = recurrenceInterval;
			probeDefinition.TimeoutSeconds = timeout;
			probeDefinition.MaxRetryAttempts = maxRetry;
			probeDefinition.TargetResource = Environment.MachineName;
			probeDefinition.Attributes[GenericEventLogProbe.LogNameAttrName] = logName;
			probeDefinition.Attributes[GenericEventLogProbe.ProviderNameAttrName] = providerName;
			probeDefinition.Attributes[GenericEventLogProbe.GreenEventsAttrName] = ((greenEventIds == null) ? string.Empty : string.Join<int>(",", greenEventIds));
			probeDefinition.Attributes[GenericEventLogProbe.RedEventsAttrName] = string.Join<int>(",", redEventIds);
			return probeDefinition;
		}

		internal override IEnumerable<PropertyInformation> GetSubstitutePropertyInformation()
		{
			return new List<PropertyInformation>
			{
				new PropertyInformation(GenericEventLogProbe.RedEventsAttrName, Strings.EventLogProbeRedEvents, true),
				new PropertyInformation(GenericEventLogProbe.LogNameAttrName, Strings.EventLogProbeLogName, true),
				new PropertyInformation(GenericEventLogProbe.ProviderNameAttrName, Strings.EventLogProbeProviderName, true),
				new PropertyInformation(GenericEventLogProbe.GreenEventsAttrName, Strings.EventLogProbeGreenEvents, false)
			};
		}

		public override void PopulateDefinition<ProbeDefinition>(ProbeDefinition pDef, Dictionary<string, string> propertyBag)
		{
			if (pDef == null)
			{
				throw new ArgumentException("Please specify a value for probeDefinition");
			}
			if (!propertyBag.ContainsKey(GenericEventLogProbe.LogNameAttrName))
			{
				throw new ArgumentException("Please specify value for LogName");
			}
			pDef.Attributes[GenericEventLogProbe.LogNameAttrName] = propertyBag[GenericEventLogProbe.LogNameAttrName].ToString().Trim();
			if (!propertyBag.ContainsKey(GenericEventLogProbe.ProviderNameAttrName))
			{
				throw new ArgumentException("Please specify value for ProviderName");
			}
			pDef.Attributes[GenericEventLogProbe.ProviderNameAttrName] = propertyBag[GenericEventLogProbe.ProviderNameAttrName].ToString().Trim();
			if (!propertyBag.ContainsKey(GenericEventLogProbe.GreenEventsAttrName))
			{
				throw new ArgumentException("Please specify value for GreenEventIds");
			}
			pDef.Attributes[GenericEventLogProbe.GreenEventsAttrName] = propertyBag[GenericEventLogProbe.GreenEventsAttrName].ToString().Trim();
			if (propertyBag.ContainsKey(GenericEventLogProbe.RedEventsAttrName))
			{
				pDef.Attributes[GenericEventLogProbe.RedEventsAttrName] = propertyBag[GenericEventLogProbe.RedEventsAttrName].ToString().Trim();
				return;
			}
			throw new ArgumentException("Please specify value for RedEventIds");
		}

		protected override void DoWork(CancellationToken cancellationToken)
		{
			DateTime utcNow = DateTime.UtcNow;
			this.logName = base.Definition.Attributes[GenericEventLogProbe.LogNameAttrName];
			this.providerName = base.Definition.Attributes[GenericEventLogProbe.ProviderNameAttrName];
			string text = base.Definition.ConstructWorkItemResultName();
			if (string.IsNullOrWhiteSpace(base.Definition.Attributes[GenericEventLogProbe.RedEventsAttrName]))
			{
				throw new GenericEventLogProbe.InvalidParametersException("EventLogProbe cannot function without defining any Red Events!");
			}
			int[] array = (from id in base.Definition.Attributes[GenericEventLogProbe.RedEventsAttrName].Split(new char[]
			{
				','
			})
			select int.Parse(id)).ToArray<int>();
			int[] array2 = new int[0];
			if (!string.IsNullOrWhiteSpace(base.Definition.Attributes[GenericEventLogProbe.GreenEventsAttrName]))
			{
				array2 = (from id in base.Definition.Attributes[GenericEventLogProbe.GreenEventsAttrName].Split(new char[]
				{
					','
				})
				select int.Parse(id)).ToArray<int>();
			}
			try
			{
				CentralEventLogWatcher.EventProbeRule rule = new CentralEventLogWatcher.EventProbeRule(text, this.logName, this.providerName, array2, array);
				if (!CentralEventLogWatcher.Instance.IsEventWatchRuleExists(rule))
				{
					TimeSpan timeout = TimeSpan.FromSeconds((double)Math.Min(base.Definition.TimeoutSeconds / 2, 2));
					WTFDiagnostics.TraceInformation<string, string, string, string, string>(ExTraceGlobals.CommonComponentsTracer, base.TraceContext, "Registering LogName={0}, ProviderName={1}, GreenEventIds={2}, RedEventIds={3} as Rule={4}", base.Definition.Attributes[GenericEventLogProbe.LogNameAttrName], base.Definition.Attributes[GenericEventLogProbe.ProviderNameAttrName], base.Definition.Attributes[GenericEventLogProbe.GreenEventsAttrName], base.Definition.Attributes[GenericEventLogProbe.RedEventsAttrName], text, null, "DoWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Common\\Probes\\GenericEventLogProbe.cs", 217);
					CentralEventLogWatcher.Instance.AddEventWatchRule(rule);
					base.Result.StateAttribute13 = string.Format("Rule {0} is added into CentralEventLogWatcher (Success={1}). Waited for {2} secs before trying to get results...", text, CentralEventLogWatcher.Instance.IsEventWatchRuleExists(rule), timeout.TotalSeconds);
					Thread.Sleep(timeout);
				}
				base.Result.StateAttribute1 = Environment.MachineName;
				base.Result.StateAttribute2 = "GenericEventLogProbe";
				base.Result.StateAttribute3 = string.Format("LogName={0}, ProviderName={1}", base.Definition.Attributes[GenericEventLogProbe.LogNameAttrName], base.Definition.Attributes[GenericEventLogProbe.ProviderNameAttrName]);
				base.Result.StateAttribute4 = base.Definition.Attributes[GenericEventLogProbe.GreenEventsAttrName];
				base.Result.StateAttribute5 = base.Definition.Attributes[GenericEventLogProbe.RedEventsAttrName];
				CentralEventLogWatcher.EventProcessorStatus eventProcessorCurrentStatus = CentralEventLogWatcher.Instance.EventProcessorCurrentStatus;
				base.Result.StateAttribute14 = string.Format("EventsProcessed={0}, EventProcessorTimeSpentInMs={1}, EventProcessorLastRun={2}, EventProcessorTimer={3}, EventProcessorsCount={4}", new object[]
				{
					eventProcessorCurrentStatus.EventsProcessedSinceInstanceStart,
					eventProcessorCurrentStatus.LastEventProcessorTimeSpentInMs,
					eventProcessorCurrentStatus.LastEventProcessorRuntime.ToString(),
					eventProcessorCurrentStatus.TimerInterval,
					eventProcessorCurrentStatus.EventProcessorsRunningCount
				});
				CentralEventLogWatcher.EventRecordMini eventRecordMini = null;
				base.Result.StateAttribute6 = (double)CentralEventLogWatcher.Instance.PopLastEventRecordForRule(text, out eventRecordMini);
				if (eventRecordMini == null)
				{
					this.OnNoEvent(cancellationToken);
				}
				else if (array2.Contains(eventRecordMini.EventId))
				{
					this.OnGreenEvent(eventRecordMini);
				}
				else if (array.Contains(eventRecordMini.EventId))
				{
					this.OnRedEvent(eventRecordMini);
				}
			}
			finally
			{
				base.Result.SampleValue = (double)((int)(DateTime.UtcNow - utcNow).TotalMilliseconds);
			}
		}

		protected virtual void OnGreenEvent(CentralEventLogWatcher.EventRecordMini greenEvent)
		{
			base.Result.StateAttribute11 = string.Format("Pass - Found Green Event {0} ({1})", greenEvent.EventId, (greenEvent.TimeCreated != null) ? greenEvent.TimeCreated.Value.ToUniversalTime().ToString() : "NULL");
		}

		protected virtual void OnRedEvent(CentralEventLogWatcher.EventRecordMini redEvent)
		{
			string text = string.Join(",", redEvent.CustomizedProperties.ToArray());
			base.Result.StateAttribute11 = string.Format("Failed - Found Red Event {0} ({1}) {2}", redEvent.EventId, (redEvent.TimeCreated != null) ? redEvent.TimeCreated.Value.ToUniversalTime().ToString() : "NULL", (text.Length > 900) ? text.Substring(0, 900) : text);
			throw new GenericEventLogProbe.RedEventFoundException(base.Result.StateAttribute11);
		}

		protected virtual void OnNoEvent(CancellationToken cancellationToken)
		{
			base.Result.StateAttribute11 = string.Format("Pass - Cannot find either Green nor Red events", new object[0]);
		}

		public static readonly string LogNameAttrName = "LogName";

		public static readonly string ProviderNameAttrName = "ProviderName";

		public static readonly string GreenEventsAttrName = "GreenEventIds";

		public static readonly string RedEventsAttrName = "RedEventIds";

		protected string logName;

		protected string providerName;

		[Serializable]
		public class RedEventFoundException : Exception
		{
			public RedEventFoundException(string exceptionMessage) : base(exceptionMessage)
			{
			}
		}

		[Serializable]
		public class InvalidParametersException : Exception
		{
			public InvalidParametersException(string exceptionMessage) : base(exceptionMessage)
			{
			}
		}
	}
}
