using System;
using System.Configuration;
using System.Threading;
using Microsoft.Exchange.Data.ApplicationLogic.UnifiedPolicy;
using Microsoft.Exchange.Data.Storage.UnifiedPolicy;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ServiceHost;
using Microsoft.Exchange.Servicelets.CommonCode;
using Microsoft.Exchange.Servicelets.UnifiedPolicySync.Messages;
using Microsoft.Office.CompliancePolicy;
using Microsoft.Office.CompliancePolicy.PolicyConfiguration;
using Microsoft.Office.CompliancePolicy.PolicySync;

namespace Microsoft.Exchange.Servicelets.UnifiedPolicySync
{
	public class Servicelet : Servicelet
	{
		public override void Work()
		{
			bool isStarted = false;
			try
			{
				GrayException.Initialize(null, delegate(Exception ex)
				{
					if (!ExWatson.IsWatsonReportAlreadySent(ex))
					{
						ExWatson.SendReport(ex, ReportOptions.None, null);
						ExWatson.SetWatsonReportAlreadySent(ex);
					}
				});
				try
				{
					Exception exception;
					GrayException.MapAndReportGrayExceptions(delegate()
					{
						this.eventLog.LogEvent(MSExchangeUnifiedPolicySyncEventLogConstants.Tuple_ServiceletStarting, "UnifiedPolicySyncServicelet", new object[0]);
						ExSyncAgentConfiguration exSyncAgentConfiguration;
						try
						{
							exSyncAgentConfiguration = new ExSyncAgentConfiguration();
						}
						catch (ConfigurationErrorsException ex3)
						{
							this.eventLog.LogEvent(MSExchangeUnifiedPolicySyncEventLogConstants.Tuple_ServiceletFailedToLoadAppConfig, null, new object[]
							{
								ex3.Message
							});
							return;
						}
						if (exSyncAgentConfiguration.DelayStartInSeconds > 0)
						{
							Thread.Sleep(TimeSpan.FromSeconds((double)exSyncAgentConfiguration.DelayStartInSeconds));
						}
						ExExecutionLog exExecutionLog = ExExecutionLog.CreateForServicelet();
						exExecutionLog.LogOneEntry("Servicelet", string.Empty, ExecutionLog.EventType.Verbose, "The Microsoft Exchange Unified Policy Sync Servicelet is starting.", null);
						SyncAgentContext syncAgentContext = new SyncAgentContext(exSyncAgentConfiguration, new ExCredentialsFactory(), new TenantInfoProviderFactory(TimeSpan.FromHours(4.0), 10, 1000), PolicyConfigProviderManager<ExPolicyConfigProviderManager>.Instance, new ExHostStateProvider(), exExecutionLog, MonitoringItemErrorPublisher.Instance, new ExPerfCounterProvider("MSUnified Compliance Sync", UnifiedPolicySyncPerfCounters.AllCounters));
						SyncManager.Initialize(syncAgentContext);
						if (RpcServerWrapper.Start(exSyncAgentConfiguration.NotifyRequestTimeout, out exception))
						{
							NotificationLoader state = new NotificationLoader(syncAgentContext.LogProvider);
							ThreadPool.QueueUserWorkItem(new WaitCallback(this.RunNotificationLoader), state);
							isStarted = true;
							this.eventLog.LogEvent(MSExchangeUnifiedPolicySyncEventLogConstants.Tuple_ServiceletStarted, "UnifiedPolicySyncServicelet", new object[0]);
							exExecutionLog.LogOneEntry("Servicelet", string.Empty, ExecutionLog.EventType.Verbose, "The Microsoft Exchange Unified Policy Sync Servicelet has started successfully.", null);
							return;
						}
						this.eventLog.LogEvent(MSExchangeUnifiedPolicySyncEventLogConstants.Tuple_ServiceletFailedToRegisterNotificationRpcEndpoint, null, new object[]
						{
							exception.Message
						});
						exExecutionLog.LogOneEntry("Servicelet", string.Empty, ExecutionLog.EventType.Error, "The Microsoft Exchange Unified Policy Sync Servicelet failed to register the RPC endpoint for notification.", exception);
					});
				}
				catch (GrayException ex)
				{
					GrayException ex2;
					this.eventLog.LogEvent(MSExchangeUnifiedPolicySyncEventLogConstants.Tuple_ServiceletFailedToStartBecauseofGrayException, null, new object[]
					{
						ex2.Message
					});
					return;
				}
				base.StopEvent.WaitOne();
			}
			finally
			{
				if (isStarted)
				{
					try
					{
						GrayException.MapAndReportGrayExceptions(delegate()
						{
							RpcServerWrapper.Stop();
						});
					}
					catch (GrayException)
					{
					}
				}
			}
		}

		private void RunNotificationLoader(object state)
		{
			try
			{
				GrayException.MapAndReportGrayExceptions(delegate()
				{
					NotificationLoader notificationLoader = state as NotificationLoader;
					if (notificationLoader == null)
					{
						throw new ArgumentException("state parameter to RunNotificationLoader delegate must be type loader and can't be null");
					}
					notificationLoader.EnqueuePendingNotifications();
				});
			}
			catch (GrayException)
			{
			}
		}

		private const string LoggingPeriodicKey = "UnifiedPolicySyncServicelet";

		private static readonly Guid ComponentGuid = new Guid("a35cde77-929b-4d78-bc91-501f0e66e4f5");

		private readonly ExEventLog eventLog = new ExEventLog(Servicelet.ComponentGuid, "MSExchange Unified Policy Sync");
	}
}
