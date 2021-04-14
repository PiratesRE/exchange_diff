using System;
using System.Globalization;
using System.Management;
using System.Threading;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Diagnostics.Components.ActiveMonitoring;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.Common;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.Local;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.Local.Components.Common.Probes;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.Search.Probes;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.Search.Responders;
using Microsoft.Office.Datacenter.ActiveMonitoring;
using Microsoft.Office.Datacenter.Monitoring.ActiveMonitoring.Recovery;
using Microsoft.Office.Datacenter.WorkerTaskFramework;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.Search
{
	public sealed class SearchDiscovery : MaintenanceWorkItem
	{
		protected override void DoWork(CancellationToken cancellationToken)
		{
			try
			{
				LocalEndpointManager instance = LocalEndpointManager.Instance;
				if (instance.ExchangeServerRoleEndpoint == null)
				{
					WTFDiagnostics.TraceInformation(ExTraceGlobals.SearchTracer, base.TraceContext, "SearchDiscovery.DoWork: Exchange Server role endpoint is not found, no need to create search related work items", null, "DoWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Search\\SearchDiscovery.cs", 72);
				}
				else
				{
					this.attributeHelper = new AttributeHelper(base.Definition);
					string @string = this.attributeHelper.GetString("EscalateDailySchedulePattern", false, SearchEscalateResponder.EscalateDailySchedulePattern);
					SearchEscalateResponder.EscalateDailySchedulePattern = @string;
					if (instance.ExchangeServerRoleEndpoint.IsMailboxRoleInstalled)
					{
						this.CreateMonitoringContexts();
					}
					else if (instance.ExchangeServerRoleEndpoint.IsFrontendTransportRoleInstalled || instance.ExchangeServerRoleEndpoint.IsBridgeheadRoleInstalled)
					{
						if (this.IsHostControllerServiceEnabled())
						{
							this.CreateMonitoringContextsForTransport();
						}
						else
						{
							WTFDiagnostics.TraceInformation(ExTraceGlobals.SearchTracer, base.TraceContext, "SearchDiscovery.DoWork: HostControllerService is not available, no need to create FAST related work items", null, "DoWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Search\\SearchDiscovery.cs", 100);
						}
					}
				}
			}
			catch (EndpointManagerEndpointUninitializedException)
			{
				SearchMonitoringHelper.LogInfo(this, "EndpointManagerEndpointUninitializedException is caught. Endpoint is not available to do monitoring.", new object[0]);
			}
		}

		private void CreateMonitoringContexts()
		{
			this.CreateCopyStatusHaImpactingContext();
			if (SearchMonitoringHelper.IsInMaintenance())
			{
				SearchMonitoringHelper.LogInfo(this, "Server is in maintenance. Skipping rest of monitoring discovery.", new object[0]);
				return;
			}
			this.CreateCatalogAvailabilityContext();
			this.CreateSearchServiceContext();
			this.CreateIndexBacklogContext();
			this.CreateIndexFailureContext();
			this.CreateLocalCopyStatusContext(SearchLocalCopyStatusProbe.TargetStatus.Mounted);
			this.CreateLocalCopyStatusContext(SearchLocalCopyStatusProbe.TargetStatus.MountedAndCrawling);
			this.CreateLocalCopyStatusContext(SearchLocalCopyStatusProbe.TargetStatus.Passive);
			this.CreateMountedCopyStatusContext();
			this.CreateCatalogSuspendedContext();
			this.CreateSingleCopyContext();
			this.CreateQueryFailureContext();
			this.CreateCatalogSizeContext();
			this.CreateProcessCrashingContext();
			this.CreateTransportAgentContext(false);
			this.CreateResourceLoadContext();
			this.CreateFeedingControllerFailureContext();
			this.CreateGracefulDegradationContext();
			this.CreateSearchMemoryUsageOverThresholdContext();
			this.CreateSearchRopNotSupportedContext();
			this.CreateGracefulDegradationManagerFailureContext();
			this.CreateGracefulDegradationStatusContext();
		}

		private void CreateMonitoringContextsForTransport()
		{
			if (SearchMonitoringHelper.IsInMaintenance())
			{
				SearchMonitoringHelper.LogInfo(this, "Server is in maintenance. Skipping monitoring discovery.", new object[0]);
				return;
			}
			this.CreateTransportAgentContext(true);
		}

		private void CreateSearchServiceContext()
		{
			bool @bool = this.attributeHelper.GetBool("SearchServiceRunningEnabled", true, true);
			int @int = this.attributeHelper.GetInt("SearchServiceRunningProbeRecurrenceIntervalSeconds", true, 0, null, null);
			ProbeDefinition probeDefinition = SearchMonitoringHelper.CreateProbeDefinition("SearchServiceRunningProbe", typeof(GenericServiceProbe), SearchDiscovery.ServiceName, @int, @bool);
			base.Broker.AddWorkDefinition<ProbeDefinition>(probeDefinition, base.TraceContext);
			int int2 = this.attributeHelper.GetInt("SearchServiceRunningMonitorMonitoringThreshold", true, 0, null, null);
			string monitorName = "SearchServiceRunningMonitor";
			string sampleMask = probeDefinition.ConstructWorkItemResultName();
			string serviceName = SearchDiscovery.ServiceName;
			int monitoringThreshold = int2;
			MonitorDefinition monitorDefinition = SearchMonitoringHelper.CreateOverallConsecutiveProbeFailuresMonitorDefinition(monitorName, sampleMask, serviceName, @int, monitoringThreshold, int2 * @int, @bool);
			monitorDefinition.ServicePriority = 1;
			monitorDefinition.ScenarioDescription = "Validate Searach health is not impacted by FastSearch issues";
			string settingPrefix = "SearchServiceRunning";
			MonitorDefinition monitorDefinition2 = monitorDefinition;
			bool restartSearchServiceNeeded = true;
			SearchMonitoringHelper.CreateResponderChainForMonitor(this, settingPrefix, monitorDefinition2, Strings.SearchServiceNotRunningEscalationMessage, @bool, false, restartSearchServiceNeeded, false, false, SearchEscalateResponder.EscalateModes.Scheduled, true);
			base.Broker.AddWorkDefinition<MonitorDefinition>(monitorDefinition, base.TraceContext);
		}

		private void CreateIndexBacklogContext()
		{
			LocalEndpointManager instance = LocalEndpointManager.Instance;
			if (instance.MailboxDatabaseEndpoint == null || instance.MailboxDatabaseEndpoint.MailboxDatabaseInfoCollectionForBackend.Count == 0)
			{
				WTFDiagnostics.TraceInformation(ExTraceGlobals.SearchTracer, base.TraceContext, "SearchDiscovery.CreateIndexBacklogContext: No mailbox database found.", null, "CreateIndexBacklogContext", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Search\\SearchDiscovery.cs", 212);
				return;
			}
			bool @bool = this.attributeHelper.GetBool("SearchIndexBacklogEnabled", true, true);
			int @int = this.attributeHelper.GetInt("SearchIndexBacklogProbeRecurrenceIntervalSeconds", true, 0, null, null);
			int recurrenceIntervalSeconds = @int;
			int int2 = this.attributeHelper.GetInt("SearchIndexBacklogProbeBacklogThresholdMinutes", true, 0, null, null);
			int int3 = this.attributeHelper.GetInt("SearchIndexBacklogProbeRetryQueueSizeThreshold", true, 0, null, null);
			int int4 = this.attributeHelper.GetInt("SearchIndexBacklogProbeRetryQueueSizeHighThreshold", true, 0, null, null);
			int int5 = this.attributeHelper.GetInt("SearchIndexBacklogProbeSystemUpTimeGracePeriodHours", true, 0, null, null);
			int int6 = this.attributeHelper.GetInt("SearchIndexBacklogMonitorMonitoringIntervalSeconds", true, 0, null, null);
			int int7 = this.attributeHelper.GetInt("SearchIndexBacklogMonitorMonitoringThreshold", true, 0, null, null);
			foreach (MailboxDatabaseInfo mailboxDatabaseInfo in instance.MailboxDatabaseEndpoint.MailboxDatabaseInfoCollectionForBackend)
			{
				string mailboxDatabaseName = mailboxDatabaseInfo.MailboxDatabaseName;
				ProbeDefinition probeDefinition = SearchMonitoringHelper.CreateProbeDefinition("SearchIndexBacklogProbe", typeof(SearchIndexBacklogProbe), mailboxDatabaseName, @int, @bool);
				probeDefinition.Attributes["BacklogThresholdMinutes"] = int2.ToString();
				probeDefinition.Attributes["RetryQueueSizeThreshold"] = int3.ToString();
				probeDefinition.Attributes["RetryQueueSizeHighThreshold"] = int4.ToString();
				probeDefinition.Attributes["SystemUpTimeGracePeriodHours"] = int5.ToString();
				probeDefinition.Attributes["SkipRetryItemsCheck"] = true.ToString();
				base.Broker.AddWorkDefinition<ProbeDefinition>(probeDefinition, base.TraceContext);
				string sampleMask = probeDefinition.ConstructWorkItemResultName();
				MonitorDefinition monitorDefinition = SearchMonitoringHelper.CreateMonitorDefinition("SearchIndexBacklogMonitor", SearchDiscovery.OverallXFailuresMonitorType, sampleMask, mailboxDatabaseName, recurrenceIntervalSeconds, int6, int7, @bool);
				monitorDefinition.ServicePriority = 1;
				monitorDefinition.ScenarioDescription = "Validate Search health is not impacted by indexing backlog issues";
				base.Broker.AddWorkDefinition<MonitorDefinition>(monitorDefinition, base.TraceContext);
				LocalizedString searchIndexBacklogAggregatedEscalationMessage = Strings.SearchIndexBacklogAggregatedEscalationMessage;
				ResponderDefinition definition = SearchMonitoringHelper.CreateEscalateResponderDefinition(monitorDefinition, searchIndexBacklogAggregatedEscalationMessage, @bool, ServiceHealthStatus.None, SearchEscalateResponder.EscalateModes.Scheduled, true);
				base.Broker.AddWorkDefinition<ResponderDefinition>(definition, base.TraceContext);
			}
		}

		private void CreateIndexFailureContext()
		{
			LocalEndpointManager instance = LocalEndpointManager.Instance;
			if (instance.MailboxDatabaseEndpoint == null || instance.MailboxDatabaseEndpoint.MailboxDatabaseInfoCollectionForBackend.Count == 0)
			{
				WTFDiagnostics.TraceInformation(ExTraceGlobals.SearchTracer, base.TraceContext, "SearchDiscovery.CreateIndexFailureContext: No mailbox database found.", null, "CreateIndexFailureContext", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Search\\SearchDiscovery.cs", 282);
				return;
			}
			bool @bool = this.attributeHelper.GetBool("SearchIndexFailureEnabled", true, true);
			int @int = this.attributeHelper.GetInt("SearchIndexFailureProbeRecurrenceIntervalSeconds", true, 0, null, null);
			double num = double.Parse(base.Definition.Attributes["SearchIndexFailureProbeFailureRateThreshold"], CultureInfo.InvariantCulture.NumberFormat);
			int int2 = this.attributeHelper.GetInt("SearchIndexFailureMonitorMonitoringThreshold", true, 0, null, null);
			ProbeDefinition probeDefinition = SearchMonitoringHelper.CreateProbeDefinition("SearchIndexFailureProbe", typeof(SearchIndexFailureProbe), string.Empty, @int, @bool);
			probeDefinition.Attributes["FailureRateThreshold"] = num.ToString();
			base.Broker.AddWorkDefinition<ProbeDefinition>(probeDefinition, base.TraceContext);
			string sampleMask = probeDefinition.ConstructWorkItemResultName();
			MonitorDefinition monitorDefinition = SearchMonitoringHelper.CreateOverallConsecutiveProbeFailuresMonitorDefinition("SearchIndexFailureMonitor", sampleMask, string.Empty, @int, int2, int2 * @int, @bool);
			monitorDefinition.ServicePriority = 1;
			monitorDefinition.ScenarioDescription = "Validate Search health is not impacted by Index failure issues";
			string settingPrefix = "SearchIndexFailure";
			MonitorDefinition monitorDefinition2 = monitorDefinition;
			bool restartSearchServiceNeeded = true;
			bool restartHostControllerServiceNeeded = true;
			SearchMonitoringHelper.CreateResponderChainForMonitor(this, settingPrefix, monitorDefinition2, Strings.SearchIndexFailureEscalationMessage, @bool, false, restartSearchServiceNeeded, false, restartHostControllerServiceNeeded, SearchEscalateResponder.EscalateModes.Scheduled, true);
			base.Broker.AddWorkDefinition<MonitorDefinition>(monitorDefinition, base.TraceContext);
		}

		private void CreateSingleCopyContext()
		{
			LocalEndpointManager instance = LocalEndpointManager.Instance;
			if (instance.MailboxDatabaseEndpoint == null || instance.MailboxDatabaseEndpoint.MailboxDatabaseInfoCollectionForBackend.Count == 0)
			{
				WTFDiagnostics.TraceInformation(ExTraceGlobals.SearchTracer, base.TraceContext, "SearchDiscovery.CreateSingleCopyContext: No mailbox database found.", null, "CreateSingleCopyContext", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Search\\SearchDiscovery.cs", 338);
				return;
			}
			bool @bool = this.attributeHelper.GetBool("SearchSingleCopyEnabled", true, true);
			int @int = this.attributeHelper.GetInt("SearchSingleCopyProbeRecurrenceIntervalSeconds", true, 0, null, null);
			int int2 = this.attributeHelper.GetInt("SearchSingleCopyProbeSeedingAllowedMinutes", false, 90, null, null);
			int recurrenceIntervalSeconds = @int;
			int int3 = this.attributeHelper.GetInt("SearchSingleCopyMonitorMonitoringIntervalSeconds", true, 0, null, null);
			int int4 = this.attributeHelper.GetInt("SearchSingleCopyMonitorMonitoringThreshold", true, 0, null, null);
			bool bool2 = this.attributeHelper.GetBool("SearchSingleCopyEscalateResponderUrgentInTraining", true, true);
			foreach (MailboxDatabaseInfo mailboxDatabaseInfo in instance.MailboxDatabaseEndpoint.MailboxDatabaseInfoCollectionForBackend)
			{
				string mailboxDatabaseName = mailboxDatabaseInfo.MailboxDatabaseName;
				ProbeDefinition probeDefinition = SearchMonitoringHelper.CreateProbeDefinition("SearchSingleCopyProbe", typeof(SearchSingleCopyProbe), mailboxDatabaseName, @int, @bool);
				probeDefinition.Attributes["SeedingAllowedMinutes"] = int2.ToString();
				base.Broker.AddWorkDefinition<ProbeDefinition>(probeDefinition, base.TraceContext);
				string sampleMask = probeDefinition.ConstructWorkItemResultName();
				MonitorDefinition monitorDefinition = SearchMonitoringHelper.CreateMonitorDefinition("SearchSingleCopyMonitor", SearchDiscovery.OverallXFailuresMonitorType, sampleMask, mailboxDatabaseName, recurrenceIntervalSeconds, int3, int4, @bool);
				monitorDefinition.ServicePriority = 1;
				monitorDefinition.ScenarioDescription = "Validate Search health is not impacted by Single Copy search issues";
				base.Broker.AddWorkDefinition<MonitorDefinition>(monitorDefinition, base.TraceContext);
				ResponderDefinition definition = SearchMonitoringHelper.CreateEscalateResponderDefinition(monitorDefinition, Strings.SearchSingleCopyEscalationMessage(mailboxDatabaseName), @bool, ServiceHealthStatus.None, SearchEscalateResponder.EscalateModes.Urgent, bool2);
				base.Broker.AddWorkDefinition<ResponderDefinition>(definition, base.TraceContext);
			}
		}

		private void CreateLocalCopyStatusContext(SearchLocalCopyStatusProbe.TargetStatus targetStatus)
		{
			LocalEndpointManager instance = LocalEndpointManager.Instance;
			string probeName;
			string monitorName;
			string text;
			SearchEscalateResponder.EscalateModes escalateMode;
			switch (targetStatus)
			{
			case SearchLocalCopyStatusProbe.TargetStatus.Mounted:
				probeName = "SearchLocalMountedCopyStatusProbe";
				monitorName = "SearchLocalMountedCopyStatusMonitor";
				text = "SearchLocalMountedCopyStatus";
				escalateMode = SearchEscalateResponder.EscalateModes.Urgent;
				break;
			case SearchLocalCopyStatusProbe.TargetStatus.MountedAndCrawling:
				probeName = "SearchCrawlingProgressProbe";
				monitorName = "SearchCrawlingProgressMonitor";
				text = "SearchCrawlingProgress";
				escalateMode = SearchEscalateResponder.EscalateModes.Scheduled;
				break;
			case SearchLocalCopyStatusProbe.TargetStatus.Passive:
				probeName = "SearchLocalPassiveCopyStatusProbe";
				monitorName = "SearchLocalPassiveCopyStatusMonitor";
				text = "SearchLocalPassiveCopyStatus";
				escalateMode = SearchEscalateResponder.EscalateModes.Scheduled;
				break;
			default:
				return;
			}
			if (instance.MailboxDatabaseEndpoint == null || instance.MailboxDatabaseEndpoint.MailboxDatabaseInfoCollectionForBackend.Count == 0)
			{
				WTFDiagnostics.TraceInformation(ExTraceGlobals.SearchTracer, base.TraceContext, "SearchDiscovery.CreateLocalCopyStatusContext: No mailbox database found.", null, "CreateLocalCopyStatusContext", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Search\\SearchDiscovery.cs", 436);
				return;
			}
			bool @bool = this.attributeHelper.GetBool(text + "Enabled", true, true);
			int @int = this.attributeHelper.GetInt(text + "ProbeRecurrenceIntervalSeconds", true, 0, null, null);
			int int2 = this.attributeHelper.GetInt(text + "ProbeFailedAndSuspendedAlertThresholdMinutes", false, 0, null, null);
			int recurrenceIntervalSeconds = @int;
			int int3 = this.attributeHelper.GetInt(text + "MonitorMonitoringIntervalSeconds", true, 0, null, null);
			int int4 = this.attributeHelper.GetInt(text + "MonitorMonitoringThreshold", true, 0, null, null);
			bool bool2 = this.attributeHelper.GetBool(text + "EscalateResponderUrgentInTraining", true, false);
			foreach (MailboxDatabaseInfo mailboxDatabaseInfo in instance.MailboxDatabaseEndpoint.MailboxDatabaseInfoCollectionForBackend)
			{
				string mailboxDatabaseName = mailboxDatabaseInfo.MailboxDatabaseName;
				ProbeDefinition probeDefinition = SearchMonitoringHelper.CreateProbeDefinition(probeName, typeof(SearchLocalCopyStatusProbe), mailboxDatabaseName, @int, @bool);
				probeDefinition.Attributes["TargetStatus"] = targetStatus.ToString();
				probeDefinition.Attributes["FailedAndSuspendedAlertThresholdMinutes"] = int2.ToString();
				base.Broker.AddWorkDefinition<ProbeDefinition>(probeDefinition, base.TraceContext);
				string sampleMask = probeDefinition.ConstructWorkItemResultName();
				MonitorDefinition monitorDefinition = SearchMonitoringHelper.CreateMonitorDefinition(monitorName, SearchDiscovery.OverallXFailuresMonitorType, sampleMask, mailboxDatabaseName, recurrenceIntervalSeconds, int3, int4, @bool);
				monitorDefinition.ServicePriority = 1;
				monitorDefinition.ScenarioDescription = "Validate Search health is not impacted by local copy search issues";
				string settingPrefix = text;
				MonitorDefinition monitorDefinition2 = monitorDefinition;
				bool databaseFailoverNeeded = targetStatus == SearchLocalCopyStatusProbe.TargetStatus.Mounted;
				bool restartSearchServiceNeeded = true;
				bool restartNodesNeeded = false;
				bool restartHostControllerServiceNeeded = true;
				SearchMonitoringHelper.CreateResponderChainForMonitor(this, settingPrefix, monitorDefinition2, Strings.SearchLocalCopyStatusEscalationMessage(mailboxDatabaseName), @bool, databaseFailoverNeeded, restartSearchServiceNeeded, restartNodesNeeded, restartHostControllerServiceNeeded, escalateMode, bool2);
				base.Broker.AddWorkDefinition<MonitorDefinition>(monitorDefinition, base.TraceContext);
			}
		}

		private void CreateMountedCopyStatusContext()
		{
			LocalEndpointManager instance = LocalEndpointManager.Instance;
			if (instance.MailboxDatabaseEndpoint == null || instance.MailboxDatabaseEndpoint.MailboxDatabaseInfoCollectionForBackend.Count == 0)
			{
				return;
			}
			bool @bool = this.attributeHelper.GetBool("SearchMountedCopyStatusEnabled", true, true);
			int @int = this.attributeHelper.GetInt("SearchMountedCopyStatusProbeRecurrenceIntervalSeconds", true, 0, null, null);
			int int2 = this.attributeHelper.GetInt("SearchMountedCopyStatusProbeMailboxesToCrawlThreshold", true, 0, null, null);
			int int3 = this.attributeHelper.GetInt("SearchMountedCopyStatusMonitorMonitoringThreshold", true, 0, null, null);
			bool bool2 = this.attributeHelper.GetBool("SearchMountedCopyStatusEscalateResponderUrgentInTraining", true, true);
			foreach (MailboxDatabaseInfo mailboxDatabaseInfo in instance.MailboxDatabaseEndpoint.MailboxDatabaseInfoCollectionForBackend)
			{
				string mailboxDatabaseName = mailboxDatabaseInfo.MailboxDatabaseName;
				ProbeDefinition probeDefinition = SearchMonitoringHelper.CreateProbeDefinition("SearchMountedCopyStatusProbe", typeof(SearchMountedCopyStatusProbe), mailboxDatabaseName, @int, @bool);
				probeDefinition.Attributes["MailboxesToCrawlThreshold"] = int2.ToString();
				base.Broker.AddWorkDefinition<ProbeDefinition>(probeDefinition, base.TraceContext);
				string sampleMask = probeDefinition.ConstructWorkItemResultName();
				MonitorDefinition monitorDefinition = SearchMonitoringHelper.CreateOverallConsecutiveProbeFailuresMonitorDefinition("SearchMountedCopyStatusMonitor", sampleMask, mailboxDatabaseName, @int, int3, int3 * @int, @bool);
				monitorDefinition.ServicePriority = 1;
				monitorDefinition.ScenarioDescription = "Validate Search health is not impacted by mailbox search crawling issues";
				string settingPrefix = "SearchMountedCopyStatus";
				MonitorDefinition monitorDefinition2 = monitorDefinition;
				bool databaseFailoverNeeded = true;
				SearchMonitoringHelper.CreateResponderChainForMonitor(this, settingPrefix, monitorDefinition2, Strings.SearchActiveCopyUnhealthyEscalationMessage(mailboxDatabaseName), @bool, databaseFailoverNeeded, false, false, false, SearchEscalateResponder.EscalateModes.Urgent, bool2);
				base.Broker.AddWorkDefinition<MonitorDefinition>(monitorDefinition, base.TraceContext);
			}
		}

		private void CreateCatalogSuspendedContext()
		{
			LocalEndpointManager instance = LocalEndpointManager.Instance;
			if (instance.MailboxDatabaseEndpoint == null || instance.MailboxDatabaseEndpoint.MailboxDatabaseInfoCollectionForBackend.Count == 0)
			{
				return;
			}
			bool @bool = this.attributeHelper.GetBool("SearchIndexSuspendedEnabled", true, true);
			int @int = this.attributeHelper.GetInt("SearchIndexSuspendedProbeRecurrenceIntervalSeconds", true, 0, null, null);
			int int2 = this.attributeHelper.GetInt("SearchIndexSuspendedMonitorMonitoringThreshold", true, 0, null, null);
			int int3 = this.attributeHelper.GetInt("SearchIndexSuspendedMonitorUnhealthy1StateSeconds", true, 0, null, null);
			int int4 = this.attributeHelper.GetInt("SearchIndexSuspendedMonitorUnrecoverableStateSeconds", true, 0, null, null);
			bool bool2 = this.attributeHelper.GetBool("SearchIndexSuspendedEscalateResponderUrgentInTraining", true, true);
			foreach (MailboxDatabaseInfo mailboxDatabaseInfo in instance.MailboxDatabaseEndpoint.MailboxDatabaseInfoCollectionForBackend)
			{
				string mailboxDatabaseName = mailboxDatabaseInfo.MailboxDatabaseName;
				ProbeDefinition probeDefinition = SearchMonitoringHelper.CreateProbeDefinition("SearchIndexSuspendedProbe", typeof(SearchIndexSuspendedProbe), mailboxDatabaseName, @int, @bool);
				base.Broker.AddWorkDefinition<ProbeDefinition>(probeDefinition, base.TraceContext);
				string sampleMask = probeDefinition.ConstructWorkItemResultName();
				MonitorDefinition monitorDefinition = SearchMonitoringHelper.CreateOverallConsecutiveProbeFailuresMonitorDefinition("SearchIndexSuspendedMonitor", sampleMask, mailboxDatabaseName, @int, int2, int2 * @int, @bool);
				monitorDefinition.MonitorStateTransitions = new MonitorStateTransition[]
				{
					new MonitorStateTransition(ServiceHealthStatus.Unhealthy, 0),
					new MonitorStateTransition(ServiceHealthStatus.Unhealthy1, int3),
					new MonitorStateTransition(ServiceHealthStatus.Unrecoverable, int4)
				};
				monitorDefinition.ServicePriority = 1;
				monitorDefinition.ScenarioDescription = "Validate Search health is not impacted by search catalog process issues";
				base.Broker.AddWorkDefinition<MonitorDefinition>(monitorDefinition, base.TraceContext);
				ResponderDefinition responderDefinition = ResumeCatalogResponder.CreateDefinition(SearchStrings.SearchResumeCatalogResponderName(monitorDefinition.Name), mailboxDatabaseName, monitorDefinition.ConstructWorkItemResultName(), ServiceHealthStatus.Unhealthy, null);
				responderDefinition.Enabled = @bool;
				base.Broker.AddWorkDefinition<ResponderDefinition>(responderDefinition, base.TraceContext);
				ResponderDefinition definition = SearchMonitoringHelper.CreateDatabaseFailoverResponderDefinition(this, monitorDefinition, ServiceHealthStatus.Unhealthy1, @bool);
				base.Broker.AddWorkDefinition<ResponderDefinition>(definition, base.TraceContext);
				ResponderDefinition definition2 = SearchMonitoringHelper.CreateEscalateResponderDefinition(monitorDefinition, Strings.SearchIndexSuspendedEscalationMessage(mailboxDatabaseName), @bool, ServiceHealthStatus.Unrecoverable, SearchEscalateResponder.EscalateModes.UrgentOnActive, bool2);
				base.Broker.AddWorkDefinition<ResponderDefinition>(definition2, base.TraceContext);
			}
		}

		private void CreateQueryFailureContext()
		{
			LocalEndpointManager instance = LocalEndpointManager.Instance;
			if (instance.MailboxDatabaseEndpoint == null || instance.MailboxDatabaseEndpoint.MailboxDatabaseInfoCollectionForBackend.Count == 0)
			{
				WTFDiagnostics.TraceInformation(ExTraceGlobals.SearchTracer, base.TraceContext, "SearchDiscovery.CreateQueryFailureContext: No mailbox database found.", null, "CreateQueryFailureContext", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Search\\SearchDiscovery.cs", 643);
				return;
			}
			bool @bool = this.attributeHelper.GetBool("SearchQueryFailureEnabled", true, true);
			int @int = this.attributeHelper.GetInt("SearchQueryFailureProbeRecurrenceIntervalSeconds", true, 0, null, null);
			double num = double.Parse(base.Definition.Attributes["SearchQueryFailureProbeFailureRateThreshold"], CultureInfo.InvariantCulture.NumberFormat);
			double num2 = double.Parse(base.Definition.Attributes["SearchQueryFailureProbeSlowRateThreshold"], CultureInfo.InvariantCulture.NumberFormat);
			int int2 = this.attributeHelper.GetInt("SearchQueryFailureMonitorMonitoringThreshold", true, 0, null, null);
			foreach (MailboxDatabaseInfo mailboxDatabaseInfo in instance.MailboxDatabaseEndpoint.MailboxDatabaseInfoCollectionForBackend)
			{
				string mailboxDatabaseName = mailboxDatabaseInfo.MailboxDatabaseName;
				ProbeDefinition probeDefinition = SearchMonitoringHelper.CreateProbeDefinition("SearchQueryFailureProbe", typeof(SearchQueryFailureProbe), mailboxDatabaseName, @int, @bool);
				probeDefinition.Attributes["FailureRateThreshold"] = num.ToString();
				probeDefinition.Attributes["SlowRateThreshold"] = num2.ToString();
				base.Broker.AddWorkDefinition<ProbeDefinition>(probeDefinition, base.TraceContext);
				string sampleMask = probeDefinition.ConstructWorkItemResultName();
				MonitorDefinition monitorDefinition = SearchMonitoringHelper.CreateOverallConsecutiveProbeFailuresMonitorDefinition("SearchQueryFailureMonitor", sampleMask, mailboxDatabaseName, @int, int2, int2 * @int, @bool);
				monitorDefinition.ServicePriority = 1;
				monitorDefinition.ScenarioDescription = "Validate Search health is not impacted by search query failure issues";
				string settingPrefix = "SearchQueryFailure";
				MonitorDefinition monitorDefinition2 = monitorDefinition;
				bool restartNodesNeeded = true;
				bool restartHostControllerServiceNeeded = true;
				SearchMonitoringHelper.CreateResponderChainForMonitor(this, settingPrefix, monitorDefinition2, Strings.SearchQueryFailureEscalationMessage(mailboxDatabaseName), @bool, false, false, restartNodesNeeded, restartHostControllerServiceNeeded, SearchEscalateResponder.EscalateModes.Scheduled, true);
				base.Broker.AddWorkDefinition<MonitorDefinition>(monitorDefinition, base.TraceContext);
			}
			WTFDiagnostics.TraceFunction(ExTraceGlobals.SearchTracer, base.TraceContext, "Exiting SearchDiscovery.CreateIndexFailureContext", null, "CreateQueryFailureContext", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Search\\SearchDiscovery.cs", 694);
		}

		private void CreateCatalogAvailabilityContext()
		{
			if (!LocalEndpointManager.IsDataCenter)
			{
				return;
			}
			LocalEndpointManager instance = LocalEndpointManager.Instance;
			if (instance.MailboxDatabaseEndpoint == null || instance.MailboxDatabaseEndpoint.MailboxDatabaseInfoCollectionForBackend.Count == 0)
			{
				WTFDiagnostics.TraceInformation(ExTraceGlobals.SearchTracer, base.TraceContext, "SearchDiscovery.CreateCatalogAvailabilityContext: No mailbox database found.", null, "CreateCatalogAvailabilityContext", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Search\\SearchDiscovery.cs", 712);
				return;
			}
			bool @bool = this.attributeHelper.GetBool("SearchCatalogAvailabilityEnabled", true, true);
			int @int = this.attributeHelper.GetInt("SearchCatalogAvailabilityProbeRecurrenceIntervalSeconds", true, 0, null, null);
			int int2 = this.attributeHelper.GetInt("SearchCatalogAvailabilityProbeStallThresholdMinutes", true, 0, null, null);
			int int3 = this.attributeHelper.GetInt("SearchCatalogAvailabilityMonitorMonitoringThreshold", true, 0, null, null);
			foreach (MailboxDatabaseInfo mailboxDatabaseInfo in instance.MailboxDatabaseEndpoint.MailboxDatabaseInfoCollectionForBackend)
			{
				string mailboxDatabaseName = mailboxDatabaseInfo.MailboxDatabaseName;
				ProbeDefinition probeDefinition = SearchMonitoringHelper.CreateProbeDefinition("SearchCatalogAvailabilityProbe", typeof(SearchCatalogAvailabilityProbe), mailboxDatabaseName, @int, @bool);
				probeDefinition.Attributes["StallThresholdMinutes"] = int2.ToString();
				base.Broker.AddWorkDefinition<ProbeDefinition>(probeDefinition, base.TraceContext);
				string sampleMask = probeDefinition.ConstructWorkItemResultName();
				MonitorDefinition monitorDefinition = SearchMonitoringHelper.CreateOverallConsecutiveProbeFailuresMonitorDefinition("SearchCatalogAvailabilityMonitor", sampleMask, mailboxDatabaseName, @int, int3, int3 * @int, @bool);
				monitorDefinition.ServicePriority = 1;
				monitorDefinition.ScenarioDescription = "Valdiate Search health is not impacted by search catalog issues";
				string settingPrefix = "SearchCatalogAvailability";
				MonitorDefinition monitorDefinition2 = monitorDefinition;
				bool restartSearchServiceNeeded = true;
				bool restartNodesNeeded = false;
				bool restartHostControllerServiceNeeded = true;
				SearchMonitoringHelper.CreateResponderChainForMonitor(this, settingPrefix, monitorDefinition2, Strings.SearchCatalogNotHealthyEscalationMessage(mailboxDatabaseName), @bool, false, restartSearchServiceNeeded, restartNodesNeeded, restartHostControllerServiceNeeded, SearchEscalateResponder.EscalateModes.Scheduled, true);
				base.Broker.AddWorkDefinition<MonitorDefinition>(monitorDefinition, base.TraceContext);
			}
		}

		private void CreateProcessCrashingContext()
		{
			string text = "MSExchangeFastSearch";
			bool @bool = this.attributeHelper.GetBool("SearchServiceCrashEnabled", true, true);
			int @int = this.attributeHelper.GetInt("SearchServiceCrashProbeRecurrenceIntervalSeconds", true, 0, null, null);
			int int2 = this.attributeHelper.GetInt("SearchServiceCrashMonitorMonitoringIntervalSeconds", true, 0, null, null);
			int int3 = this.attributeHelper.GetInt("SearchServiceCrashMonitorMonitoringThreshold", true, 0, null, null);
			ProbeDefinition probeDefinition = SearchMonitoringHelper.CreateProbeDefinition("SearchServiceCrashProbe", typeof(GenericProcessCrashDetectionProbe), text, @int, @bool);
			base.Broker.AddWorkDefinition<ProbeDefinition>(probeDefinition, base.TraceContext);
			MonitorDefinition monitorDefinition = SearchMonitoringHelper.CreateMonitorDefinition("SearchServiceCrashMonitor", SearchDiscovery.OverallXFailuresMonitorType, probeDefinition.ConstructWorkItemResultName(), text, @int, int2, int3, @bool);
			monitorDefinition.ServicePriority = 1;
			monitorDefinition.ScenarioDescription = "Validate Search health is not impacted by process crashe issues";
			string settingPrefix = "SearchServiceCrash";
			MonitorDefinition monitorDefinition2 = monitorDefinition;
			bool restartHostControllerServiceNeeded = true;
			SearchMonitoringHelper.CreateResponderChainForMonitor(this, settingPrefix, monitorDefinition2, Strings.SearchProcessCrashingTooManyTimesEscalationMessage(text, int3, int2), @bool, false, false, false, restartHostControllerServiceNeeded, SearchEscalateResponder.EscalateModes.Scheduled, true);
			base.Broker.AddWorkDefinition<MonitorDefinition>(monitorDefinition, base.TraceContext);
		}

		private void CreateCatalogSizeContext()
		{
			if (!LocalEndpointManager.IsDataCenter)
			{
				return;
			}
			LocalEndpointManager instance = LocalEndpointManager.Instance;
			if (instance.MailboxDatabaseEndpoint == null || instance.MailboxDatabaseEndpoint.MailboxDatabaseInfoCollectionForBackend.Count == 0)
			{
				WTFDiagnostics.TraceInformation(ExTraceGlobals.SearchTracer, base.TraceContext, "SearchDiscovery.CreateCatalogSizeContext: No mailbox database found.", null, "CreateCatalogSizeContext", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Search\\SearchDiscovery.cs", 823);
				return;
			}
			bool @bool = this.attributeHelper.GetBool("SearchCatalogSizeEnabled", true, true);
			int @int = this.attributeHelper.GetInt("SearchCatalogSizeProbeRecurrenceIntervalSeconds", true, 0, null, null);
			double num = double.Parse(base.Definition.Attributes["SearchCatalogSizeProbeSizePercentageThreshold"], CultureInfo.InvariantCulture.NumberFormat);
			double num2 = double.Parse(base.Definition.Attributes["SearchCatalogSizeProbeMinimumCheckSizeGb"], CultureInfo.InvariantCulture.NumberFormat);
			foreach (MailboxDatabaseInfo mailboxDatabaseInfo in instance.MailboxDatabaseEndpoint.MailboxDatabaseInfoCollectionForBackend)
			{
				string mailboxDatabaseName = mailboxDatabaseInfo.MailboxDatabaseName;
				ProbeDefinition probeDefinition = SearchMonitoringHelper.CreateProbeDefinition("SearchCatalogSizeProbe", typeof(SearchCatalogSizeProbe), mailboxDatabaseName, @int, @bool);
				probeDefinition.Attributes["SizePercentageThreshold"] = num.ToString();
				probeDefinition.Attributes["MinimumCheckSizeGb"] = num2.ToString();
				base.Broker.AddWorkDefinition<ProbeDefinition>(probeDefinition, base.TraceContext);
			}
		}

		private void CreateTransportAgentContext(bool urgentInTraining)
		{
			if (!LocalEndpointManager.IsDataCenter)
			{
				return;
			}
			bool @bool = this.attributeHelper.GetBool("SearchTransportAgentEnabled", true, true);
			int @int = this.attributeHelper.GetInt("SearchTransportAgentProbeRecurrenceIntervalSeconds", true, 0, null, null);
			double num = double.Parse(base.Definition.Attributes["SearchTransportAgentProbeFailureRateThreshold"], CultureInfo.InvariantCulture.NumberFormat);
			int int2 = this.attributeHelper.GetInt("SearchTransportAgentProbeMinimumProcessedDocuments", true, 0, null, null);
			int int3 = this.attributeHelper.GetInt("SearchTransportAgentMonitorMonitoringThreshold", true, 0, null, null);
			int int4 = this.attributeHelper.GetInt("SearchTransportAgentMonitorMonitoringIntervalSeconds", true, 0, null, null);
			ProbeDefinition probeDefinition = SearchMonitoringHelper.CreateProbeDefinition("SearchTransportAgentProbe", typeof(SearchTransportAgentProbe), string.Empty, @int, @bool);
			probeDefinition.Attributes["FailureRateThreshold"] = num.ToString();
			probeDefinition.Attributes["MinimumProcessedDocuments"] = int2.ToString();
			base.Broker.AddWorkDefinition<ProbeDefinition>(probeDefinition, base.TraceContext);
			MonitorDefinition monitorDefinition = SearchMonitoringHelper.CreateMonitorDefinition("SearchTransportAgentMonitor", SearchDiscovery.OverallXFailuresMonitorType, probeDefinition.ConstructWorkItemResultName(), string.Empty, @int, int4, int3, @bool);
			monitorDefinition.ServicePriority = 1;
			monitorDefinition.ScenarioDescription = "Validate Search health is not impacted by transport indexing agent issues";
			string settingPrefix = "SearchTransportAgent";
			MonitorDefinition monitorDefinition2 = monitorDefinition;
			bool restartSearchServiceNeeded = false;
			bool restartNodesNeeded = true;
			bool restartHostControllerServiceNeeded = true;
			string escalationMessage = Strings.SearchTransportAgentFailureEscalationMessage;
			SearchEscalateResponder.EscalateModes escalateMode = urgentInTraining ? SearchEscalateResponder.EscalateModes.Urgent : SearchEscalateResponder.EscalateModes.Scheduled;
			SearchMonitoringHelper.CreateResponderChainForMonitor(this, settingPrefix, monitorDefinition2, escalationMessage, @bool, false, restartSearchServiceNeeded, restartNodesNeeded, restartHostControllerServiceNeeded, escalateMode, urgentInTraining);
			base.Broker.AddWorkDefinition<MonitorDefinition>(monitorDefinition, base.TraceContext);
		}

		private void CreateResourceLoadContext()
		{
			if (!LocalEndpointManager.IsDataCenter)
			{
				return;
			}
			LocalEndpointManager instance = LocalEndpointManager.Instance;
			if (instance.MailboxDatabaseEndpoint == null || instance.MailboxDatabaseEndpoint.MailboxDatabaseInfoCollectionForBackend.Count == 0)
			{
				WTFDiagnostics.TraceInformation(ExTraceGlobals.SearchTracer, base.TraceContext, "SearchDiscovery.CreateResourceLoadContext: No mailbox database found.", null, "CreateResourceLoadContext", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Search\\SearchDiscovery.cs", 920);
				return;
			}
			bool @bool = this.attributeHelper.GetBool("SearchResourceLoadEnabled", true, true);
			int @int = this.attributeHelper.GetInt("SearchResourceLoadProbeRecurrenceIntervalSeconds", true, 0, null, null);
			int int2 = this.attributeHelper.GetInt("SearchResourceLoadMonitorMonitoringThreshold", true, 0, null, null);
			int int3 = this.attributeHelper.GetInt("SearchResourceLoadMonitorMonitoringIntervalSeconds", true, 0, null, null);
			bool bool2 = this.attributeHelper.GetBool("SearchResourceLoadEscalateResponderUrgentInTraining", true, true);
			foreach (MailboxDatabaseInfo mailboxDatabaseInfo in instance.MailboxDatabaseEndpoint.MailboxDatabaseInfoCollectionForBackend)
			{
				string mailboxDatabaseName = mailboxDatabaseInfo.MailboxDatabaseName;
				ProbeDefinition probeDefinition = SearchMonitoringHelper.CreateProbeDefinition("SearchResourceLoadProbe", typeof(SearchResourceLoadProbe), mailboxDatabaseName, @int, @bool);
				base.Broker.AddWorkDefinition<ProbeDefinition>(probeDefinition, base.TraceContext);
				MonitorDefinition monitorDefinition = SearchMonitoringHelper.CreateMonitorDefinition("SearchResourceLoadMonitor", SearchDiscovery.OverallXFailuresMonitorType, probeDefinition.ConstructWorkItemResultName(), mailboxDatabaseName, @int, int3, int2, @bool);
				monitorDefinition.ServicePriority = 1;
				monitorDefinition.ScenarioDescription = "Validate Search health is not impaced by search resource load issues";
				base.Broker.AddWorkDefinition<MonitorDefinition>(monitorDefinition, base.TraceContext);
				ResponderDefinition responderDefinition = SearchMonitoringHelper.CreateEscalateResponderDefinition(monitorDefinition, Strings.SearchResourceLoadEscalationMessage(mailboxDatabaseName, int3 / 60), @bool, ServiceHealthStatus.None, SearchEscalateResponder.EscalateModes.Urgent, bool2);
				int num = (int)TimeSpan.FromHours(12.0).TotalSeconds;
				responderDefinition.MinimumSecondsBetweenEscalates = num;
				responderDefinition.WaitIntervalSeconds = num;
				base.Broker.AddWorkDefinition<ResponderDefinition>(responderDefinition, base.TraceContext);
			}
		}

		private void CreateFeedingControllerFailureContext()
		{
			if (!LocalEndpointManager.IsDataCenter)
			{
				return;
			}
			LocalEndpointManager instance = LocalEndpointManager.Instance;
			if (instance.MailboxDatabaseEndpoint == null || instance.MailboxDatabaseEndpoint.MailboxDatabaseInfoCollectionForBackend.Count == 0)
			{
				WTFDiagnostics.TraceInformation(ExTraceGlobals.SearchTracer, base.TraceContext, "SearchDiscovery.CreateFeedingControllerFailureContext: No mailbox database found.", null, "CreateFeedingControllerFailureContext", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Search\\SearchDiscovery.cs", 985);
				return;
			}
			bool @bool = this.attributeHelper.GetBool("SearchFeedingControllerFailureEnabled", true, true);
			int @int = this.attributeHelper.GetInt("SearchFeedingControllerFailureMonitorRecurrenceIntervalSeconds", true, 0, new int?(1), null);
			int int2 = this.attributeHelper.GetInt("SearchFeedingControllerFailureMonitorMonitoringThreshold", true, 0, null, null);
			int int3 = this.attributeHelper.GetInt("SearchFeedingControllerFailureMonitorMonitoringIntervalSeconds", true, 0, null, null);
			foreach (MailboxDatabaseInfo mailboxDatabaseInfo in instance.MailboxDatabaseEndpoint.MailboxDatabaseInfoCollectionForBackend)
			{
				string mailboxDatabaseName = mailboxDatabaseInfo.MailboxDatabaseName;
				MonitorDefinition monitorDefinition = SearchMonitoringHelper.CreateMonitorDefinition("SearchFeedingControllerFailureMonitor", typeof(SearchFeedingControllerFailureMonitor), NotificationItem.GenerateResultName(ExchangeComponent.Search.Name, "SearchFeedingControllerFailure", mailboxDatabaseName), mailboxDatabaseName, @int, int3, int2, @bool);
				monitorDefinition.ServicePriority = 1;
				monitorDefinition.ScenarioDescription = "Validate Search health is not impacted by feeding controller failure issues";
				monitorDefinition.TimeoutSeconds = 300;
				string settingPrefix = "SearchFeedingControllerFailure";
				MonitorDefinition monitorDefinition2 = monitorDefinition;
				bool restartSearchServiceNeeded = true;
				bool restartNodesNeeded = false;
				bool restartHostControllerServiceNeeded = true;
				SearchMonitoringHelper.CreateResponderChainForMonitor(this, settingPrefix, monitorDefinition2, Strings.SearchFeedingControllerFailureEscalationMessage(mailboxDatabaseName, int2, int3 / 60), @bool, false, restartSearchServiceNeeded, restartNodesNeeded, restartHostControllerServiceNeeded, SearchEscalateResponder.EscalateModes.Scheduled, true);
				base.Broker.AddWorkDefinition<MonitorDefinition>(monitorDefinition, base.TraceContext);
			}
		}

		private bool IsHostControllerServiceEnabled()
		{
			string queryString = string.Format("SELECT Name, StartMode from Win32_Service WHERE Name LIKE \"HostControllerService\"", new object[0]);
			using (ManagementObjectSearcher managementObjectSearcher = new ManagementObjectSearcher(queryString))
			{
				using (ManagementObjectCollection managementObjectCollection = managementObjectSearcher.Get())
				{
					foreach (ManagementBaseObject managementBaseObject in managementObjectCollection)
					{
						ManagementObject managementObject = (ManagementObject)managementBaseObject;
						try
						{
							if (managementObject["StartMode"] != null && !managementObject["StartMode"].ToString().Equals("Disabled", StringComparison.OrdinalIgnoreCase))
							{
								return true;
							}
						}
						finally
						{
							managementObject.Dispose();
						}
					}
				}
			}
			return false;
		}

		private void CreateGracefulDegradationContext()
		{
			if (!LocalEndpointManager.IsDataCenter)
			{
				return;
			}
			LocalEndpointManager instance = LocalEndpointManager.Instance;
			if (instance.MailboxDatabaseEndpoint == null || instance.MailboxDatabaseEndpoint.MailboxDatabaseInfoCollectionForBackend.Count == 0)
			{
				WTFDiagnostics.TraceInformation(ExTraceGlobals.SearchTracer, base.TraceContext, "SearchDiscovery.CreateGracefulDegradationContext: No mailbox database found.", null, "CreateGracefulDegradationContext", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Search\\SearchDiscovery.cs", 1075);
				return;
			}
			bool @bool = this.attributeHelper.GetBool("SearchNumberOfParserServersDegradationEnabled", true, true);
			int @int = this.attributeHelper.GetInt("SearchNumberOfParserServersDegradationProbeRecurrenceIntervalSeconds", true, 0, null, null);
			ProbeDefinition definition = SearchMonitoringHelper.CreateProbeDefinition("SearchParserServerDegradeProbe", typeof(SearchNumberOfParserServersDegradationProbe), string.Empty, @int, @bool);
			base.Broker.AddWorkDefinition<ProbeDefinition>(definition, base.TraceContext);
		}

		private void CreateSearchMemoryUsageOverThresholdContext()
		{
			if (!LocalEndpointManager.IsDataCenter)
			{
				return;
			}
			LocalEndpointManager instance = LocalEndpointManager.Instance;
			if (instance.MailboxDatabaseEndpoint == null || instance.MailboxDatabaseEndpoint.MailboxDatabaseInfoCollectionForBackend.Count == 0)
			{
				WTFDiagnostics.TraceInformation(ExTraceGlobals.SearchTracer, base.TraceContext, "SearchDiscovery.CreateSearchMemoryUsageOverThresholdContext: No mailbox database found.", null, "CreateSearchMemoryUsageOverThresholdContext", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Search\\SearchDiscovery.cs", 1108);
				return;
			}
			bool @bool = this.attributeHelper.GetBool("SearchMemoryUsageOverThresholdEnabled", true, true);
			int @int = this.attributeHelper.GetInt("SearchMemoryUsageOverThresholdProbeRecurrenceIntervalSeconds", true, 0, null, null);
			int recurrenceIntervalSeconds = @int;
			int int2 = this.attributeHelper.GetInt("SearchMemoryUsageOverThresholdMonitorMonitoringIntervalSeconds", true, 0, null, null);
			int int3 = this.attributeHelper.GetInt("SearchMemoryUsageOverThresholdMonitorMonitoringThreshold", true, 0, null, null);
			ProbeDefinition probeDefinition = SearchMonitoringHelper.CreateProbeDefinition("SearchMemoryOverThresholdProbe", typeof(SearchMemoryUsageOverThresholdProbe), string.Empty, @int, @bool);
			base.Broker.AddWorkDefinition<ProbeDefinition>(probeDefinition, base.TraceContext);
			string sampleMask = probeDefinition.ConstructWorkItemResultName();
			MonitorDefinition monitorDefinition = SearchMonitoringHelper.CreateMonitorDefinition("SearchMemoryOverThresholdMonitor", SearchDiscovery.OverallXFailuresMonitorType, sampleMask, string.Empty, recurrenceIntervalSeconds, int2, int3, @bool);
			base.Broker.AddWorkDefinition<MonitorDefinition>(monitorDefinition, base.TraceContext);
			LocalizedString searchMemoryUsageOverThresholdEscalationMessage = Strings.SearchMemoryUsageOverThresholdEscalationMessage;
			ResponderDefinition definition = SearchMonitoringHelper.CreateEscalateResponderDefinition(monitorDefinition, searchMemoryUsageOverThresholdEscalationMessage, @bool, ServiceHealthStatus.None, SearchEscalateResponder.EscalateModes.Urgent, true);
			base.Broker.AddWorkDefinition<ResponderDefinition>(definition, base.TraceContext);
		}

		private void CreateSearchRopNotSupportedContext()
		{
			if (!LocalEndpointManager.IsDataCenter)
			{
				return;
			}
			LocalEndpointManager instance = LocalEndpointManager.Instance;
			if (instance.MailboxDatabaseEndpoint == null || instance.MailboxDatabaseEndpoint.MailboxDatabaseInfoCollectionForBackend.Count == 0)
			{
				WTFDiagnostics.TraceInformation(ExTraceGlobals.SearchTracer, base.TraceContext, "SearchDiscovery.CreateSearchRopNotSupportedContext: No mailbox database found.", null, "CreateSearchRopNotSupportedContext", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Search\\SearchDiscovery.cs", 1167);
				return;
			}
			bool @bool = this.attributeHelper.GetBool("SearchRopNotSupportedEnabled", true, true);
			int @int = this.attributeHelper.GetInt("SearchRopNotSupportedMonitorRecurrenceIntervalSeconds", true, 0, null, null);
			int int2 = this.attributeHelper.GetInt("SearchRopNotSupportedMonitorMonitoringIntervalSeconds", true, 0, null, null);
			int int3 = this.attributeHelper.GetInt("SearchRopNotSupportedMonitorMonitoringThreshold", true, 0, null, null);
			string monitorName = "SearchRopNotSupportedMonitor";
			string sampleMask = NotificationItem.GenerateResultName(ExchangeComponent.Search.Name, "SearchRetrieverProducerRopNotSupported", string.Empty);
			string empty = string.Empty;
			int recurrenceIntervalSeconds = @int;
			int monitoringInterval = int2;
			MonitorDefinition monitorDefinition = SearchMonitoringHelper.CreateOverallConsecutiveProbeFailuresMonitorDefinition(monitorName, sampleMask, empty, recurrenceIntervalSeconds, int3, monitoringInterval, @bool);
			monitorDefinition.ServicePriority = 1;
			monitorDefinition.ScenarioDescription = "Validate Search health is not impacted by ROP not supported issues";
			monitorDefinition.TimeoutSeconds = 300;
			base.Broker.AddWorkDefinition<MonitorDefinition>(monitorDefinition, base.TraceContext);
			LocalizedString searchRopNotSupportedEscalationMessage = Strings.SearchRopNotSupportedEscalationMessage;
			ResponderDefinition definition = SearchMonitoringHelper.CreateEscalateResponderDefinition(monitorDefinition, searchRopNotSupportedEscalationMessage, @bool, ServiceHealthStatus.None, SearchEscalateResponder.EscalateModes.Urgent, true);
			base.Broker.AddWorkDefinition<ResponderDefinition>(definition, base.TraceContext);
		}

		private void CreateCopyStatusHaImpactingContext()
		{
			LocalEndpointManager instance = LocalEndpointManager.Instance;
			if (instance.MailboxDatabaseEndpoint == null || instance.MailboxDatabaseEndpoint.MailboxDatabaseInfoCollectionForBackend.Count == 0)
			{
				WTFDiagnostics.TraceInformation(ExTraceGlobals.SearchTracer, base.TraceContext, "SearchDiscovery.CreateCopyStatusHaImpactingContext: No mailbox database found.", null, "CreateCopyStatusHaImpactingContext", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Search\\SearchDiscovery.cs", 1213);
				return;
			}
			string probeName = "SearchCopyStatusHaImpactingProbe";
			string monitorName = "SearchCopyStatusHaImpactingMonitor";
			string str = "SearchCopyStatusHaImpacting";
			bool @bool = this.attributeHelper.GetBool(str + "Enabled", true, true);
			int @int = this.attributeHelper.GetInt(str + "ProbeRecurrenceIntervalSeconds", true, 0, null, null);
			int recurrenceIntervalSeconds = @int;
			int int2 = this.attributeHelper.GetInt(str + "MonitorMonitoringIntervalSeconds", true, 0, null, null);
			int int3 = this.attributeHelper.GetInt(str + "MonitorMonitoringThreshold", true, 0, null, null);
			foreach (MailboxDatabaseInfo mailboxDatabaseInfo in instance.MailboxDatabaseEndpoint.MailboxDatabaseInfoCollectionForBackend)
			{
				string mailboxDatabaseName = mailboxDatabaseInfo.MailboxDatabaseName;
				ProbeDefinition probeDefinition = SearchMonitoringHelper.CreateProbeDefinition(probeName, typeof(SearchCopyStatusHaImpactingProbe), mailboxDatabaseName, @int, @bool);
				base.Broker.AddWorkDefinition<ProbeDefinition>(probeDefinition, base.TraceContext);
				string sampleMask = probeDefinition.ConstructWorkItemResultName();
				MonitorDefinition monitorDefinition = SearchMonitoringHelper.CreateMonitorDefinition(monitorName, SearchDiscovery.OverallXFailuresMonitorType, sampleMask, mailboxDatabaseName, recurrenceIntervalSeconds, int2, int3, @bool);
				monitorDefinition.IsHaImpacting = true;
				monitorDefinition.SetHaScope(HaScopeEnum.Database);
				monitorDefinition.ServicePriority = 1;
				monitorDefinition.ScenarioDescription = "Validate Search copy is healthy.";
				base.Broker.AddWorkDefinition<MonitorDefinition>(monitorDefinition, base.TraceContext);
			}
		}

		private void CreateGracefulDegradationManagerFailureContext()
		{
			if (!LocalEndpointManager.IsDataCenter)
			{
				return;
			}
			LocalEndpointManager instance = LocalEndpointManager.Instance;
			if (instance.MailboxDatabaseEndpoint == null || instance.MailboxDatabaseEndpoint.MailboxDatabaseInfoCollectionForBackend.Count == 0)
			{
				WTFDiagnostics.TraceInformation(ExTraceGlobals.SearchTracer, base.TraceContext, "SearchDiscovery.CreateGracefulDegradationManagerFailureContext: No mailbox database found.", null, "CreateGracefulDegradationManagerFailureContext", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Search\\SearchDiscovery.cs", 1277);
				return;
			}
			bool @bool = this.attributeHelper.GetBool("SearchGracefulDegradationManagerFailureEnabled", true, true);
			int @int = this.attributeHelper.GetInt("SearchGracefulDegradationManagerFailureMonitorRecurrenceIntervalSeconds", true, 0, new int?(1), null);
			int int2 = this.attributeHelper.GetInt("SearchGracefulDegradationManagerFailureMonitorMonitoringThreshold", true, 0, null, null);
			int int3 = this.attributeHelper.GetInt("SearchGracefulDegradationManagerFailureMonitorMonitoringIntervalSeconds", true, 0, null, null);
			MonitorDefinition monitorDefinition = SearchMonitoringHelper.CreateMonitorDefinition("SearchGracefulDegradationManagerFailureMonitor", SearchDiscovery.OverallXFailuresMonitorType, NotificationItem.GenerateResultName(ExchangeComponent.Search.Name, "GracefulDegradationManagerFailure", null), string.Empty, @int, int3, int2, @bool);
			LocalizedString searchGracefulDegradationManagerFailureEscalationMessage = Strings.SearchGracefulDegradationManagerFailureEscalationMessage;
			ResponderDefinition definition = SearchMonitoringHelper.CreateEscalateResponderDefinition(monitorDefinition, searchGracefulDegradationManagerFailureEscalationMessage, @bool, ServiceHealthStatus.None, SearchEscalateResponder.EscalateModes.Urgent, true);
			base.Broker.AddWorkDefinition<ResponderDefinition>(definition, base.TraceContext);
		}

		private void CreateGracefulDegradationStatusContext()
		{
			if (!LocalEndpointManager.IsDataCenter)
			{
				return;
			}
			LocalEndpointManager instance = LocalEndpointManager.Instance;
			if (instance.MailboxDatabaseEndpoint == null || instance.MailboxDatabaseEndpoint.MailboxDatabaseInfoCollectionForBackend.Count == 0)
			{
				WTFDiagnostics.TraceInformation(ExTraceGlobals.SearchTracer, base.TraceContext, "SearchDiscovery.CreateSearchGracefulDegradationStatusContext: No mailbox database found.", null, "CreateGracefulDegradationStatusContext", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Search\\SearchDiscovery.cs", 1324);
				return;
			}
			bool @bool = this.attributeHelper.GetBool("SearchGracefulDegradationStatusEnabled", true, true);
			int @int = this.attributeHelper.GetInt("SearchGracefulDegradationStatusProbeRecurrenceIntervalSeconds", true, 0, null, null);
			int recurrenceIntervalSeconds = @int;
			int int2 = this.attributeHelper.GetInt("SearchGracefulDegradationStatusMonitorMonitoringIntervalSeconds", true, 0, null, null);
			int int3 = this.attributeHelper.GetInt("SearchGracefulDegradationStatusMonitorMonitoringThreshold", true, 0, null, null);
			ProbeDefinition probeDefinition = SearchMonitoringHelper.CreateProbeDefinition("SearchGracefulDegradationStatusProbe", typeof(SearchMemoryUsageOverThresholdProbe), string.Empty, @int, @bool);
			base.Broker.AddWorkDefinition<ProbeDefinition>(probeDefinition, base.TraceContext);
			string sampleMask = probeDefinition.ConstructWorkItemResultName();
			MonitorDefinition monitorDefinition = SearchMonitoringHelper.CreateMonitorDefinition("SearchGracefulDegradationStatusMonitor", SearchDiscovery.OverallXFailuresMonitorType, sampleMask, string.Empty, recurrenceIntervalSeconds, int2, int3, @bool);
			base.Broker.AddWorkDefinition<MonitorDefinition>(monitorDefinition, base.TraceContext);
			LocalizedString searchGracefulDegradationStatusEscalationMessage = Strings.SearchGracefulDegradationStatusEscalationMessage;
			ResponderDefinition definition = SearchMonitoringHelper.CreateEscalateResponderDefinition(monitorDefinition, searchGracefulDegradationStatusEscalationMessage, @bool, ServiceHealthStatus.None, SearchEscalateResponder.EscalateModes.Urgent, true);
			base.Broker.AddWorkDefinition<ResponderDefinition>(definition, base.TraceContext);
		}

		internal static readonly string ServiceName = "MSExchangeFastSearch";

		private static readonly Type OverallXFailuresMonitorType = typeof(OverallXFailuresMonitor);

		private static readonly Type OverallConsecutiveProbeFailuresMonitorType = typeof(OverallConsecutiveProbeFailuresMonitor);

		private static readonly Type RestartServiceResponderType = typeof(RestartServiceResponder);

		private AttributeHelper attributeHelper;
	}
}
