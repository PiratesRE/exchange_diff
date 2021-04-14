using System;
using System.Threading;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Management.Search;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.Local;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.Search.Probes
{
	public class SearchCatalogAvailabilityProbe : SearchProbeBase
	{
		protected override bool SkipOnNonHealthyCatalog
		{
			get
			{
				return true;
			}
		}

		protected override bool SkipOnAutoDagExcludeFromMonitoring
		{
			get
			{
				return true;
			}
		}

		protected override void InternalDoWork(CancellationToken cancellationToken)
		{
			string targetResource = base.Definition.TargetResource;
			int @int = base.AttributeHelper.GetInt("StallThresholdMinutes", true, 0, null, null);
			bool @bool = base.AttributeHelper.GetBool("CheckNotificationLastPolledTimeOnly", false, true);
			DiagnosticInfo.FeedingControllerDiagnosticInfo feedingControllerDiagnosticInfo = null;
			string machineName = Environment.MachineName;
			try
			{
				feedingControllerDiagnosticInfo = SearchMonitoringHelper.GetCachedFeedingControllerDiagnosticInfo(targetResource);
			}
			catch (TimeoutException innerException)
			{
				LocalizedString localizedString = Strings.SearchGetDiagnosticInfoTimeout((int)SearchMonitoringHelper.GetDiagnosticInfoCallTimeout.TotalSeconds);
				if (@bool)
				{
					base.Result.StateAttribute3 = localizedString;
					return;
				}
				throw new SearchProbeFailureException(localizedString, innerException);
			}
			if (feedingControllerDiagnosticInfo == null)
			{
				string diagnosticInfoXml = (SearchMonitoringHelper.DiagnosticInfo != null) ? SearchMonitoringHelper.DiagnosticInfo.DiagnosticInfoXml : Strings.SearchInformationNotAvailable.ToString();
				LocalizedString localizedString2 = Strings.SearchCatalogNotLoaded(targetResource, machineName, diagnosticInfoXml);
				if (!@bool)
				{
					throw new SearchProbeFailureException(localizedString2);
				}
				base.Result.StateAttribute3 = localizedString2;
			}
			else
			{
				base.Result.StateAttribute1 = feedingControllerDiagnosticInfo.ToString();
				base.Result.StateAttribute2 = feedingControllerDiagnosticInfo.NotificationFeederLastPollTime.ToString();
				if (!string.IsNullOrWhiteSpace(feedingControllerDiagnosticInfo.Error))
				{
					LocalizedString localizedString3 = Strings.SearchCatalogHasError(targetResource, feedingControllerDiagnosticInfo.Error, machineName);
					if (@bool)
					{
						base.Result.StateAttribute3 = localizedString3;
						return;
					}
					throw new SearchProbeFailureException(localizedString3);
				}
				else if (feedingControllerDiagnosticInfo.NotificationFeederLastEvent == 0L)
				{
					LocalizedString value = Strings.SearchCatalogHasError(targetResource, feedingControllerDiagnosticInfo.Error, machineName);
					if (@bool)
					{
						base.Result.StateAttribute3 = value;
						return;
					}
					throw new SearchProbeFailureException(Strings.SearchCatalogNotificationFeederLastEventZero(targetResource, machineName));
				}
				else
				{
					if (feedingControllerDiagnosticInfo.NotificationFeederLastPollTime != DateTime.MinValue && DateTime.UtcNow - feedingControllerDiagnosticInfo.NotificationFeederLastPollTime > TimeSpan.FromMinutes((double)@int))
					{
						throw new SearchProbeFailureException(Strings.SearchIndexStall(targetResource, feedingControllerDiagnosticInfo.NotificationFeederLastPollTime.ToString(), machineName));
					}
					return;
				}
			}
		}
	}
}
