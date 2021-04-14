using System;
using System.Collections.Generic;
using Microsoft.Exchange.Configuration.Common;
using Microsoft.Exchange.Monitoring;

namespace Microsoft.Exchange.Management.Tasks
{
	internal abstract class TroubleshooterCheck
	{
		public TroubleshooterCheck(PropertyBag fields)
		{
			this.fields = fields;
		}

		public static List<TroubleshooterCheck> RunChecks(ICollection<TroubleshooterCheck> expectedCheckList, TroubleshooterCheck.ContinueToNextCheck continueToNextCheck, out MonitoringData results)
		{
			List<TroubleshooterCheck> list = new List<TroubleshooterCheck>();
			results = new MonitoringData();
			foreach (TroubleshooterCheck troubleshooterCheck in expectedCheckList)
			{
				bool errorsReturned = false;
				MonitoringData monitoringData = troubleshooterCheck.InternalRunCheck();
				foreach (MonitoringEvent monitoringEvent in monitoringData.Events)
				{
					if (monitoringEvent.EventType == EventTypeEnumeration.Error)
					{
						list.Add(troubleshooterCheck);
						errorsReturned = true;
						break;
					}
				}
				results.Events.AddRange(monitoringData.Events);
				results.PerformanceCounters.AddRange(monitoringData.PerformanceCounters);
				if (!continueToNextCheck(troubleshooterCheck, monitoringData, errorsReturned))
				{
					break;
				}
			}
			return list;
		}

		public virtual MonitoringData Resolve(MonitoringData monitoringData)
		{
			return monitoringData;
		}

		public abstract MonitoringData InternalRunCheck();

		public static bool ShouldContinue(TroubleshooterCheck troubleshooterCheck, MonitoringData monitoringData, bool errorsReturned)
		{
			return !errorsReturned;
		}

		public MonitoringEvent TSResolutionFailed(string serverName)
		{
			return new MonitoringEvent(AssistantTroubleshooterBase.EventSource, 5204, EventTypeEnumeration.Error, Strings.TSResolutionFailed(serverName));
		}

		public const string ResolveProblems = "ResolveProblems";

		public const string MonitoringContext = "MonitoringContext";

		protected PropertyBag fields;

		public delegate bool ContinueToNextCheck(TroubleshooterCheck task, MonitoringData taskResults, bool errorsReturned);
	}
}
