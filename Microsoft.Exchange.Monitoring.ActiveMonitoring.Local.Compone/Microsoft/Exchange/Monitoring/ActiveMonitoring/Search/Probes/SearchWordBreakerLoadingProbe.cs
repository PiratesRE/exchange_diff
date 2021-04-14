using System;
using System.Diagnostics.Eventing.Reader;
using System.Threading;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.Common;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.Local;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.Local.Components.Common.Probes;
using Microsoft.Office.Datacenter.ActiveMonitoring;
using Microsoft.Office.Datacenter.WorkerTaskFramework;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.Search.Probes
{
	public class SearchWordBreakerLoadingProbe : GenericEventLogProbe
	{
		static SearchWordBreakerLoadingProbe()
		{
			CentralEventLogWatcher.Instance.BeforeEnqueueEvent += delegate(EventRecord eventRecord, CentralEventLogWatcher.EventRecordMini eventRecordMini)
			{
				if (eventRecordMini.EventId == 151 && string.Equals(eventRecordMini.LogName, "Microsoft-Office Server-Search/Operational", StringComparison.OrdinalIgnoreCase) && string.Equals(eventRecordMini.Source, "Microsoft-Office Server-Search", StringComparison.OrdinalIgnoreCase) && eventRecord.Properties != null && eventRecord.Properties.Count >= 1 && eventRecord.Properties[0] != null)
				{
					eventRecordMini.ExtendedPropertyField1 = eventRecord.Properties[0].Value.ToString();
				}
			};
		}

		public static ProbeDefinition CreateDefinition(string name, int recurrenceIntervalSeconds, bool enabled)
		{
			ProbeDefinition probeDefinition = GenericEventLogProbe.CreateDefinition(name, ExchangeComponent.Search.Name, "Microsoft-Office Server-Search/Operational", "Microsoft-Office Server-Search", new int[]
			{
				152
			}, new int[]
			{
				151
			}, recurrenceIntervalSeconds, recurrenceIntervalSeconds, 3);
			probeDefinition.TypeName = typeof(SearchWordBreakerLoadingProbe).FullName;
			probeDefinition.Enabled = enabled;
			return probeDefinition;
		}

		protected override void OnRedEvent(CentralEventLogWatcher.EventRecordMini redEvent)
		{
			string text = null;
			if (!string.IsNullOrEmpty(redEvent.ExtendedPropertyField1))
			{
				string[] array = redEvent.ExtendedPropertyField1.Split(new char[]
				{
					'-'
				});
				if (array.Length == 2)
				{
					text = array[0];
				}
			}
			if (!string.IsNullOrEmpty(text))
			{
				base.Result.StateAttribute15 = text;
			}
			else
			{
				text = "<Unknown>";
			}
			throw new SearchProbeFailureException(Strings.SearchWordBreakerLoadingFailure(text, redEvent.TimeCreated.ToString(), 151.ToString(), "Microsoft-Office Server-Search/Operational", 152.ToString()));
		}

		protected override void OnNoEvent(CancellationToken cancellationToken)
		{
			ProbeResult lastProbeResult = SearchMonitoringHelper.GetLastProbeResult(this, base.Broker, cancellationToken);
			if (lastProbeResult != null && lastProbeResult.ResultType == ResultType.Failed && lastProbeResult.Exception.Contains("SearchProbeFailureException"))
			{
				base.Result.StateAttribute15 = lastProbeResult.StateAttribute15;
				base.Result.StateAttribute14 = lastProbeResult.ExecutionStartTime.ToString();
				throw new SearchProbeFailureException(new LocalizedString(lastProbeResult.Error));
			}
		}

		public const string Source = "Microsoft-Office Server-Search";

		public const string Channel = "Microsoft-Office Server-Search/Operational";

		public const int RedEventId = 151;

		public const int GreenEventId = 152;
	}
}
