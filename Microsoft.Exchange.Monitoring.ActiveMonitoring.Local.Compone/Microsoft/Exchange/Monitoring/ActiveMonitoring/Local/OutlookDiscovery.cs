using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Microsoft.Exchange.Common;
using Microsoft.Exchange.Data.ApplicationLogic.Autodiscover;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.ActiveMonitoring;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.ActiveMonitoring.Monitors;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.ActiveMonitoring.Responders;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.Common;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.Responders;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.RpcClientAccess;
using Microsoft.Exchange.Net;
using Microsoft.Office.Datacenter.ActiveMonitoring;
using Microsoft.Office.Datacenter.Monitoring.ActiveMonitoring.Recovery;
using Microsoft.Office.Datacenter.WorkerTaskFramework;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.Local
{
	public abstract class OutlookDiscovery : DiscoveryWorkItem
	{
		internal abstract MonitorStateTransition[] GetProtocolMonitorStateTransitions();

		protected abstract bool DidDiscoveryExecuteSuccessfully();

		protected abstract void SetSuccessfulDiscoveryExecutionStatus();

		protected override Trace Trace
		{
			get
			{
				return ExTraceGlobals.RPCHTTPTracer;
			}
		}

		internal bool IsTestRun
		{
			set
			{
				this.isTestRun = value;
			}
		}

		protected override void CreateWorkTasks(CancellationToken cancellationToken)
		{
			this.ReadExtensionAttributes();
			if (this.DidDiscoveryExecuteSuccessfully())
			{
				if (!this.isTestRun)
				{
					goto IL_94;
				}
			}
			try
			{
				this.ValidateEndpoints();
				if (LocalEndpointManager.Instance.ExchangeServerRoleEndpoint.IsClientAccessRoleInstalled)
				{
					this.AddBackendActiveMonitoring();
				}
				if (LocalEndpointManager.Instance.ExchangeServerRoleEndpoint.IsCafeRoleInstalled)
				{
					this.AddCafeActiveMonitoring();
				}
				this.SetSuccessfulDiscoveryExecutionStatus();
				return;
			}
			catch (EndpointManagerEndpointUninitializedException ex)
			{
				string text = string.Format("OutlookDiscovery:: DoWork(): Endpoint initialization threw an exception. Exception:{0}", ex.ToString());
				base.Result.StateAttribute3 = text;
				WTFDiagnostics.TraceInformation(this.Trace, base.TraceContext, text, null, "CreateWorkTasks", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\MapiMT\\OutlookDiscovery.cs", 165);
				return;
			}
			IL_94:
			string text2 = "OutlookDiscovery:: DoWork(): Work definitions have already been created";
			base.Result.StateAttribute3 = text2;
			WTFDiagnostics.TraceInformation(this.Trace, base.TraceContext, text2, null, "CreateWorkTasks", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\MapiMT\\OutlookDiscovery.cs", 176);
		}

		protected virtual void AddBackendActiveMonitoring()
		{
			DiscoveryWorkItem.AddTestsForEachResource<MailboxDatabaseInfo>(delegate
			{
				return from dbInfo in LocalEndpointManager.Instance.MailboxDatabaseEndpoint.EnsureNotNull((MailboxDatabaseEndpoint x) => x.MailboxDatabaseInfoCollectionForBackend)
				where !string.IsNullOrEmpty(dbInfo.MonitoringAccount)
				select dbInfo;
			}, new Action<IEnumerable<MailboxDatabaseInfo>>[]
			{
				new Action<IEnumerable<MailboxDatabaseInfo>>(this.AddMailboxDeepTest),
				new Action<IEnumerable<MailboxDatabaseInfo>>(this.AddSelfTest)
			});
		}

		protected virtual void AddCafeActiveMonitoring()
		{
			DiscoveryWorkItem.AddTestsForEachResource<MailboxDatabaseInfo>(delegate
			{
				return from dbInfo in LocalEndpointManager.Instance.MailboxDatabaseEndpoint.EnsureNotNull((MailboxDatabaseEndpoint x) => x.MailboxDatabaseInfoCollectionForCafe)
				where !string.IsNullOrEmpty(dbInfo.MonitoringAccount)
				select dbInfo;
			}, new Action<IEnumerable<MailboxDatabaseInfo>>[]
			{
				new Action<IEnumerable<MailboxDatabaseInfo>>(this.AddMailboxCtp),
				new Action<IEnumerable<MailboxDatabaseInfo>>(this.AddArchiveCtp)
			});
		}

		private void ValidateEndpoints()
		{
			LocalEndpointManager.Instance.EnsureNotNull((LocalEndpointManager x) => x.ExchangeServerRoleEndpoint);
			LocalEndpointManager.Instance.EnsureNotNull((LocalEndpointManager x) => x.MailboxDatabaseEndpoint);
		}

		private void ReadExtensionAttributes()
		{
			this.UseServerAuthforBackEndProbes = this.ReadAttribute("UseServerAuthforBackEndProbes", false);
		}

		protected static ProbeDefinition CreateProbe<TProbe>(ProbeIdentity probeIdentity) where TProbe : ProbeWorkItem, new()
		{
			ProbeDefinition probeDefinition = new ProbeDefinition
			{
				Endpoint = ComputerInformation.DnsPhysicalFullyQualifiedDomainName,
				MaxRetryAttempts = 0,
				Enabled = true
			}.Apply(probeIdentity);
			probeDefinition.SetType(typeof(TProbe));
			return probeDefinition;
		}

		internal static ResponderDefinition CreateEscalateResponder(MonitorIdentity monitorIdentity)
		{
			string escalationTeam = monitorIdentity.Component.EscalationTeam;
			ResponderIdentity responderIdentity = monitorIdentity.CreateResponderIdentity("Escalate", null);
			return EscalateResponder.CreateDefinition(responderIdentity.Name, responderIdentity.ServiceName, monitorIdentity.Name, monitorIdentity.GetAlertMask(), monitorIdentity.TargetResource, ServiceHealthStatus.Unrecoverable, escalationTeam, Strings.RcaEscalationSubject(monitorIdentity.GetAlertMask(), Environment.MachineName), Datacenter.IsMicrosoftHostedOnly(true) ? "EscalationMessage.LocalProbe.html" : Strings.RcaEscalationBodyEnt, true, NotificationServiceClass.Urgent, 14400, "Pacific Standard Time/Monday,Tuesday,Wednesday,Thursday,Friday,Saturday,Sunday/00:00/23:59", Datacenter.IsMicrosoftHostedOnly(true)).Apply(responderIdentity).ApplyModifier(new Func<ResponderDefinition, ResponderDefinition>(OutlookEscalateResponder<InterpretedResult>.Configure));
		}

		protected static ResponderDefinition CreateServiceRestartResponder(MonitorIdentity monitorIdentity, ServiceHealthStatus targetState, string windowsServiceName)
		{
			ResponderIdentity responderIdentity = monitorIdentity.CreateResponderIdentity("Restart", windowsServiceName);
			string name = responderIdentity.Name;
			string alertMask = monitorIdentity.GetAlertMask();
			string serviceName = responderIdentity.ServiceName;
			return RestartServiceResponder.CreateDefinition(name, alertMask, windowsServiceName, targetState, 15, 120, 0, false, DumpMode.FullDump, null, 15.0, 0, serviceName, null, true, true, null, false).Apply(responderIdentity);
		}

		protected static ResponderDefinition CreateAppPoolRestartResponder(MonitorIdentity monitorIdentity, string appPoolName, ServiceHealthStatus responderTargetState)
		{
			ResponderIdentity responderIdentity = monitorIdentity.CreateResponderIdentity("Restart", appPoolName);
			string name = responderIdentity.Name;
			string alertMask = monitorIdentity.GetAlertMask();
			string serviceName = responderIdentity.ServiceName;
			return ResetIISAppPoolResponder.CreateDefinition(name, alertMask, appPoolName, responderTargetState, DumpMode.FullDump, null, 15.0, 0, serviceName, true, null).Apply(responderIdentity);
		}

		protected static ResponderDefinition CreateFailoverResponder(MonitorIdentity monitorIdentity)
		{
			ResponderIdentity responderIdentity = monitorIdentity.CreateResponderIdentity("Failover", null);
			return SystemFailoverResponder.CreateDefinition(responderIdentity.Name, monitorIdentity.GetAlertMask(), ServiceHealthStatus.Unhealthy, responderIdentity.Component.ShortName, "Exchange", true).Apply(responderIdentity);
		}

		protected MonitorDefinition CreateSnappyMonitor(ProbeIdentity probeIdentity)
		{
			MonitorIdentity monitorIdentity = probeIdentity.CreateMonitorIdentity();
			MonitorDefinition monitorDefinition = OverallConsecutiveProbeFailuresMonitor.CreateDefinition(monitorIdentity.Name, probeIdentity.GetAlertMask(), monitorIdentity.ServiceName, monitorIdentity.Component, 5, true, 300).Apply(monitorIdentity);
			monitorDefinition.RecurrenceIntervalSeconds = 0;
			monitorDefinition.MonitorStateTransitions = this.GetProtocolMonitorStateTransitions();
			monitorDefinition.ServicePriority = 0;
			monitorDefinition.ScenarioDescription = string.Format("Validate {0} health is not impacted by instantaneous recoverable issues", Extensions.MomtComponentName);
			return monitorDefinition;
		}

		protected MonitorDefinition CreatePercentSuccessMonitor(ProbeIdentity probeIdentity, double availabilityThreshold, int minimumErrorCount, TimeSpan detectionTime, TimeSpan recurrenceInterval)
		{
			MonitorIdentity monitorIdentity = probeIdentity.CreateMonitorIdentity();
			MonitorDefinition monitorDefinition = OverallPercentSuccessMonitor.CreateDefinition(monitorIdentity.Name, probeIdentity.GetAlertMask(), monitorIdentity.ServiceName, monitorIdentity.Component, availabilityThreshold, detectionTime, minimumErrorCount, true).Apply(monitorIdentity);
			monitorDefinition.RecurrenceIntervalSeconds = (int)recurrenceInterval.TotalSeconds;
			monitorDefinition.MonitorStateTransitions = this.GetProtocolMonitorStateTransitions();
			monitorDefinition.ServicePriority = 0;
			monitorDefinition.ScenarioDescription = string.Format("Validate {0} health is not impacted by alertable issues", Extensions.MomtComponentName);
			return monitorDefinition;
		}

		protected MonitorDefinition CreateChunkingMonitor(ProbeIdentity probeIdentity, int numberOfResources, TimeSpan detectionTime)
		{
			ArgumentValidator.ThrowIfZeroOrNegative("numberOfResources", numberOfResources);
			TimeSpan timeSpan = TimeSpan.FromSeconds(detectionTime.TotalSeconds / 5.0);
			MonitorIdentity monitorIdentity = probeIdentity.CreateMonitorIdentity();
			MonitorDefinition monitorDefinition = OverallPercentSuccessByStateAttribute1Monitor.CreateDefinition(monitorIdentity.Name, probeIdentity.GetAlertMask(), monitorIdentity.ServiceName, monitorIdentity.Component, 90.0, timeSpan, timeSpan, detectionTime, "", true).Apply(monitorIdentity);
			monitorDefinition.MonitorStateTransitions = this.GetProtocolMonitorStateTransitions();
			monitorDefinition.ServicePriority = 0;
			monitorDefinition.ScenarioDescription = string.Format("Validate {0} health is not impacted by alertable issues", Extensions.MomtComponentName);
			return monitorDefinition;
		}

		private void AddForEachDatabase(IEnumerable<MailboxDatabaseInfo> databases, ProbeIdentity baseProbeIdentity, Action<ProbeIdentity, MailboxDatabaseInfo> workDefinitionTaskCreator)
		{
			foreach (MailboxDatabaseInfo mailboxDatabaseInfo in databases)
			{
				base.CreateRelatedWorkItems<ProbeIdentity, MailboxDatabaseInfo>(baseProbeIdentity.ForTargetResource(mailboxDatabaseInfo.MailboxDatabaseName), mailboxDatabaseInfo, workDefinitionTaskCreator);
			}
		}

		protected void AddForEachDatabaseForScheduledAndOnDemandExecution(IEnumerable<MailboxDatabaseInfo> databases, ProbeIdentity baseProbeIdentity, Func<ProbeIdentity, MailboxDatabaseInfo, ProbeDefinition> probeDefinitionCreator)
		{
			this.AddForEachDatabase(databases, baseProbeIdentity, delegate(ProbeIdentity probeIdentity, MailboxDatabaseInfo database)
			{
				this.AddWorkDefinition<ProbeDefinition>(probeDefinitionCreator(probeIdentity, database));
			});
			base.CreateRelatedWorkItems<ProbeIdentity, MailboxDatabaseInfo>(baseProbeIdentity, databases.First<MailboxDatabaseInfo>(), delegate(ProbeIdentity probeIdentity, MailboxDatabaseInfo database)
			{
				this.AddWorkDefinition<ProbeDefinition>(probeDefinitionCreator(probeIdentity, database).MakeTemplateForOnDemandExecution());
			});
		}

		protected void AddMonitorAndResponders(MonitorDefinition monitor)
		{
			monitor.IsHaImpacting = monitor.MonitorStateTransitions.Any((MonitorStateTransition transition) => transition.ToState == ServiceHealthStatus.Unhealthy);
			base.AddWorkDefinition<MonitorDefinition>(monitor);
			ResponderDefinition[] source = this.CreateProtocolSpecificResponders(monitor);
			foreach (ResponderDefinition responderDefinition in from responder in source
			where responder != null && monitor.MonitorStateTransitions.Any((MonitorStateTransition transition) => transition.ToState == responder.TargetHealthState)
			select responder)
			{
				responderDefinition.RecurrenceIntervalSeconds = 0;
				base.AddWorkDefinition<ResponderDefinition>(responderDefinition);
			}
		}

		protected abstract ResponderDefinition[] CreateProtocolSpecificResponders(MonitorIdentity monitor);

		private void AddMailboxDeepTest(IEnumerable<MailboxDatabaseInfo> databases)
		{
			this.AddDeepTest(databases, this.GetDeepTest(), (ProbeDefinition probe, MailboxDatabaseInfo dbInfo) => probe.TargetPrimaryMailbox(dbInfo));
		}

		protected abstract ProbeIdentity GetDeepTest();

		protected abstract void AddDeepTest(IEnumerable<MailboxDatabaseInfo> databases, ProbeIdentity baseProbeIdentity, Func<ProbeDefinition, MailboxDatabaseInfo, ProbeDefinition> probeModifier);

		protected abstract void AddSelfTest(IEnumerable<MailboxDatabaseInfo> databases);

		private void AddMailboxCtp(IEnumerable<MailboxDatabaseInfo> databases)
		{
			if (!Extensions.IsDatacenter)
			{
				this.AddCtp(databases, this.GetMailboxCTPTest(), (ProbeDefinition probe, MailboxDatabaseInfo dbInfo) => probe.TargetPrimaryMailbox(dbInfo), (MonitorDefinition monitor) => monitor);
			}
		}

		private void AddArchiveCtp(IEnumerable<MailboxDatabaseInfo> databases)
		{
			if (!Extensions.IsDatacenter)
			{
				this.AddCtp(databases, this.GetArchiveCTPTest(), (ProbeDefinition probe, MailboxDatabaseInfo dbInfo) => probe.TargetArchiveMailbox(dbInfo), (MonitorDefinition monitor) => monitor.DelayStateTransitions(OutlookDiscovery.ArchiveMonitorStateTransitionDelay));
			}
		}

		protected abstract void AddCtp(IEnumerable<MailboxDatabaseInfo> databases, ProbeIdentity baseProbeIdentity, Func<ProbeDefinition, MailboxDatabaseInfo, ProbeDefinition> probeModifier, Func<MonitorDefinition, MonitorDefinition> monitorModifier);

		protected abstract ProbeIdentity GetMailboxCTPTest();

		protected abstract ProbeIdentity GetArchiveCTPTest();

		internal bool CheckRpcProxyAuthenticationSettingsAvailable(IEnumerable<AutodiscoverRpcHttpSettings> settings, WorkItemIdentity baseIdentity)
		{
			if (!settings.Any<AutodiscoverRpcHttpSettings>())
			{
				WTFDiagnostics.TraceInformation<WorkItemIdentity>(this.Trace, base.TraceContext, "Unable to get any OutlookAnywhere settings for {0}", baseIdentity, null, "CheckRpcProxyAuthenticationSettingsAvailable", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\MapiMT\\OutlookDiscovery.cs", 642);
				base.CreateRelatedWorkItems<WorkItemIdentity>(baseIdentity, delegate(WorkItemIdentity unused)
				{
					throw new OutlookAnywhereNotFoundException();
				});
				return false;
			}
			return true;
		}

		protected bool CheckMonitoringAccountsAvailable<T>(IEnumerable<T> accounts, WorkItemIdentity baseIdentity, LocalizedString message)
		{
			if (!accounts.Any<T>())
			{
				WTFDiagnostics.TraceInformation<WorkItemIdentity, LocalizedString>(this.Trace, base.TraceContext, "OutlookDiscovery.CheckMonitoringAccountsAvailable while creating work items for {0}: {1}", baseIdentity, message, null, "CheckMonitoringAccountsAvailable", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\MapiMT\\OutlookDiscovery.cs", 674);
				base.CreateRelatedWorkItems<WorkItemIdentity>(baseIdentity, delegate(WorkItemIdentity unused)
				{
					throw new NoMonitoringAccountsAvailableException(message);
				});
				return false;
			}
			return true;
		}

		public const int FailureCountToTriggerAlert = 5;

		internal const string FederatedAuthServiceName = "MSExchangeProtectedServiceHost";

		internal const string DatacenterEscalationMessageResourceName = "EscalationMessage.LocalProbe.html";

		private bool isTestRun;

		protected static readonly TimeSpan ArchiveMonitorStateTransitionDelay = TimeSpan.FromMinutes(5.0);

		protected static readonly TimeSpan FullDumpTimeout = TimeSpan.FromMinutes(3.0);

		protected bool UseServerAuthforBackEndProbes;

		protected static class RecoveryAction
		{
			public const ServiceHealthStatus RestartProtocolProxy = ServiceHealthStatus.Degraded;

			public const ServiceHealthStatus RestartMAPIHTTPCafeAppPool = ServiceHealthStatus.Degraded;

			public const ServiceHealthStatus RestartProtocolImplementation = ServiceHealthStatus.Degraded1;

			public const ServiceHealthStatus RestartMAPIHTTPBackEndAppPool = ServiceHealthStatus.Degraded1;

			public const ServiceHealthStatus RestartAuthenticationServices = ServiceHealthStatus.Degraded2;

			public const ServiceHealthStatus Failover = ServiceHealthStatus.Unhealthy;

			public const ServiceHealthStatus Reboot = ServiceHealthStatus.Unhealthy1;

			public const ServiceHealthStatus Escalate = ServiceHealthStatus.Unrecoverable;
		}
	}
}
