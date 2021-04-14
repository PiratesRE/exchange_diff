using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Microsoft.Exchange.Data.ApplicationLogic;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.AirSync
{
	internal sealed class AirSyncServiceHealthHandler : ExchangeDiagnosableWrapper<AirSyncServiceHealth>
	{
		protected override string UsageText
		{
			get
			{
				return "This diagnostics handler returns info about EAS process health. The handler supports \"ShowError\" argument to return error details. Below are examples for using this diagnostics handler: ";
			}
		}

		protected override string UsageSample
		{
			get
			{
				return "Example 1: Returns Service health without error details.\r\n                                    Get-ExchangeDiagnosticInfo -Process MSExchangeSyncAppPool -Component AirSyncServiceHealth\r\n\r\n                                    Example 2: Returns Service health with error details.\r\n                                    Get-ExchangeDiagnosticInfo -Process MSExchangeSyncAppPool -Component AirSyncServiceHealth -Argument ShowError";
			}
		}

		public static AirSyncServiceHealthHandler GetInstance()
		{
			if (AirSyncServiceHealthHandler.instance == null)
			{
				lock (AirSyncServiceHealthHandler.lockObject)
				{
					if (AirSyncServiceHealthHandler.instance == null)
					{
						AirSyncServiceHealthHandler.instance = new AirSyncServiceHealthHandler();
					}
				}
			}
			return AirSyncServiceHealthHandler.instance;
		}

		private AirSyncServiceHealthHandler()
		{
		}

		protected override string ComponentName
		{
			get
			{
				return "AirSyncServiceHealth";
			}
		}

		internal override AirSyncServiceHealth GetExchangeDiagnosticsInfoData(DiagnosableParameters arguments)
		{
			AirSyncServiceHealth serviceHealth = new AirSyncServiceHealth();
			serviceHealth.ServerName = Environment.MachineName;
			serviceHealth.TotalNumberOfRequests = AirSyncCounters.NumberOfRequests.RawValue;
			serviceHealth.AutoblockedDevices = AirSyncCounters.AutoBlockedDevices.RawValue;
			serviceHealth.ActiveRequests = AirSyncCounters.CurrentNumberOfRequests.RawValue;
			serviceHealth.AverageLdapLatency = AirSyncCounters.AverageLdapLatency.RawValue;
			serviceHealth.AverageRpcLatency = AirSyncCounters.AverageRpcLatency.RawValue;
			serviceHealth.CurrentlyPendingPing = AirSyncCounters.CurrentlyPendingPing.RawValue;
			serviceHealth.CurrentlyPendingSync = AirSyncCounters.CurrentlyPendingSync.RawValue;
			serviceHealth.NumberOfDroppedPing = AirSyncCounters.NumberOfDroppedPing.RawValue;
			serviceHealth.NumberOfDroppedSync = AirSyncCounters.NumberOfDroppedSync.RawValue;
			serviceHealth.NumberOfRequests = (long)ActiveSyncRequestCache.Instance.Values.Count;
			serviceHealth.SyncICSFolderCheckPercent = Command.GetFolderHierarchyICSPercentage();
			serviceHealth.PingICSFolderCheckPercent = Command.GetFolderHierarchyICSPercentage();
			serviceHealth.AverageRequestTime = ActiveSyncRequestCache.Instance.Values.Average((ActiveSyncRequestData request) => request.RequestTime);
			serviceHealth.NumberOfErroredRequests = (from request in ActiveSyncRequestCache.Instance.Values
			where request.HasErrors || request.HttpStatus != HttpStatusCode.OK || (request.CommandName != CommandType.MoveItems.ToString() && request.AirSyncStatus != StatusCode.Success.ToString()) || (request.CommandName == CommandType.MoveItems.ToString() && request.AirSyncStatus != StatusCode.Sync_InvalidSyncKey.ToString())
			select request).Count<ActiveSyncRequestData>();
			serviceHealth.Http200ResponseRatio = ((serviceHealth.NumberOfRequests > 0L) ? ((double)(serviceHealth.NumberOfRequests - (long)serviceHealth.NumberOfErroredRequests) / (double)serviceHealth.NumberOfRequests) : 0.0);
			serviceHealth.NumberOfDevices = (from s in ActiveSyncRequestCache.Instance.Values
			select s.DeviceID).Distinct<string>().Count<string>();
			serviceHealth.RateOfEASRequests = (double)ActiveSyncRequestCache.Instance.Values.Count / TimeSpan.FromMinutes((double)GlobalSettings.RequestCacheTimeInterval).TotalSeconds;
			serviceHealth.NewDevices = (from request in ActiveSyncRequestCache.Instance.Values
			where request.NewDeviceCreated
			select request).Count<ActiveSyncRequestData>();
			if (arguments.Argument.Equals("ShowError", StringComparison.InvariantCultureIgnoreCase))
			{
				serviceHealth.ErrorDetails = new List<ErrorDetail>();
				ActiveSyncRequestCache.Instance.Values.ForEach(delegate(ActiveSyncRequestData request)
				{
					if (request.ErrorDetails != null)
					{
						serviceHealth.ErrorDetails.AddRange(request.ErrorDetails);
					}
				});
			}
			return serviceHealth;
		}

		private const string ShowErrorArgument = "ShowError";

		private static AirSyncServiceHealthHandler instance;

		private static object lockObject = new object();
	}
}
