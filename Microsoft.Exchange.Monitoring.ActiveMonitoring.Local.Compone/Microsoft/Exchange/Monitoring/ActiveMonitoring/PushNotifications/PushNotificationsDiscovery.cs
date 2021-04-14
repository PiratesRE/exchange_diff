using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using Microsoft.Exchange.Common;
using Microsoft.Exchange.Diagnostics.Components.ActiveMonitoring;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.ActiveMonitoring.Responders;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.Common;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.Local;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.Responders;
using Microsoft.Exchange.PushNotifications;
using Microsoft.Exchange.PushNotifications.Publishers;
using Microsoft.Exchange.VariantConfiguration;
using Microsoft.Office.Datacenter.ActiveMonitoring;
using Microsoft.Office.Datacenter.Monitoring.ActiveMonitoring.Recovery;
using Microsoft.Office.Datacenter.WorkerTaskFramework;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.PushNotifications
{
	internal sealed class PushNotificationsDiscovery : MaintenanceWorkItem
	{
		private int? MonitoringIntervalOverride { get; set; }

		private int? RecurrenceIntervalOverride { get; set; }

		private bool SkipInstanceOverride { get; set; }

		private string MonitoringTenantId { get; set; }

		private bool IsRegistrationEnabledOnAzureSend { get; set; }

		private bool IsDeviceRegistrationChannelEnabled { get; set; }

		private bool IsChallengeRequestChannelEnabled { get; set; }

		internal static string EscalateResponderNameForEvent(string eventName, string targetResource = "")
		{
			return PushNotificationsDiscovery.EventNameWithSuffix(eventName, "Escalate", targetResource);
		}

		internal static string RecycleResponderNameForEvent(string eventName, string targetResource = "")
		{
			return PushNotificationsDiscovery.EventNameWithSuffix(eventName, "Recycle", targetResource);
		}

		internal static string MonitorNameForEvent(string eventName, string targetResource = "")
		{
			return PushNotificationsDiscovery.EventNameWithSuffix(eventName, "Monitor", targetResource);
		}

		private static string EventNameWithSuffix(string eventName, string suffix, string targetResource)
		{
			bool flag = string.IsNullOrEmpty(targetResource);
			return string.Format("{0}{1}{2}{3}", new object[]
			{
				eventName,
				suffix,
				flag ? string.Empty : "/",
				flag ? string.Empty : targetResource
			});
		}

		internal static PushNotificationPublisherConfiguration PublisherConfiguration
		{
			get
			{
				return PushNotificationsDiscovery.publisherConfiguration.Value;
			}
		}

		protected override void DoWork(CancellationToken cancellationToken)
		{
			LocalEndpointManager instance = LocalEndpointManager.Instance;
			try
			{
				if (instance.ExchangeServerRoleEndpoint == null)
				{
					WTFDiagnostics.TraceInformation(ExTraceGlobals.PushNotificationTracer, base.TraceContext, "PushNotificationsDiscovery:: DoWork(): Could not find ExchangeServerRoleEndpoint", null, "DoWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\PushNotifications\\PushNotificationsDiscovery.cs", 253);
					return;
				}
			}
			catch (EndpointManagerEndpointUninitializedException ex)
			{
				WTFDiagnostics.TraceInformation(ExTraceGlobals.PushNotificationTracer, base.TraceContext, string.Format("PushNotificationsDiscovery:: DoWork(): Endpoint initialisation failed. Exception:{0}", ex.ToString()), null, "DoWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\PushNotifications\\PushNotificationsDiscovery.cs", 263);
				return;
			}
			this.LoadIntervalOverrides();
			if (instance.ExchangeServerRoleEndpoint.IsMailboxRoleInstalled && VariantConfiguration.InvariantNoFlightingSnapshot.ActiveMonitoring.PushNotificationsDiscoveryMbx.Enabled)
			{
				if (instance.MailboxDatabaseEndpoint != null)
				{
					MailboxDatabaseInfo mailboxDatabaseInfo = instance.MailboxDatabaseEndpoint.MailboxDatabaseInfoCollectionForBackend.FirstOrDefault<MailboxDatabaseInfo>();
					if (mailboxDatabaseInfo != null && mailboxDatabaseInfo.MonitoringAccountOrganizationId != null)
					{
						this.MonitoringTenantId = mailboxDatabaseInfo.MonitoringAccountOrganizationId.ToExternalDirectoryOrganizationId();
					}
				}
				this.CreateAPNSPublisherChannelMonitors();
				this.CreateWnsPublisherChannelMonitors();
				this.CreateGcmPublisherChannelMonitors();
				this.CreateWebAppPublisherChannelMonitors();
				this.CreateAzurePublisherChannelMonitors();
				this.CreateAzureHubCreationPublisherChannelMonitors();
				this.CreateAzureDeviceRegistrationPublisherChannelMonitors();
				this.CreateAzureChallengeRequestPublisherChannelMonitors();
				this.CreateAzureHubCreationProbe();
				this.CreateNotificationDeliveryMonitors();
				this.CreateDeviceRegistrationProbe();
				this.CreateChallengeRequestProbe();
				this.CreateDatacenterOnPremBackendEndpointProbe();
			}
			if (instance.ExchangeServerRoleEndpoint.IsCafeRoleInstalled && VariantConfiguration.InvariantNoFlightingSnapshot.ActiveMonitoring.PushNotificationsDiscoveryCafe.Enabled)
			{
				try
				{
					if (instance.MailboxDatabaseEndpoint == null)
					{
						WTFDiagnostics.TraceInformation(ExTraceGlobals.PushNotificationTracer, base.TraceContext, "PushNotificationsDiscovery:: DoWork(): Could not find MailboxDatabaseEndpoint", null, "DoWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\PushNotifications\\PushNotificationsDiscovery.cs", 318);
						return;
					}
				}
				catch (EndpointManagerEndpointUninitializedException ex2)
				{
					WTFDiagnostics.TraceInformation(ExTraceGlobals.PushNotificationTracer, base.TraceContext, string.Format("PushNotificationsDiscovery:: DoWork(): MailboxDatabaseEndpoint initialisation failed.  Exception:{0}", ex2.ToString()), null, "DoWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\PushNotifications\\PushNotificationsDiscovery.cs", 328);
					return;
				}
				MailboxDatabaseInfo mailboxDatabaseInfo2 = instance.MailboxDatabaseEndpoint.MailboxDatabaseInfoCollectionForCafe.FirstOrDefault((MailboxDatabaseInfo db) => !string.IsNullOrWhiteSpace(db.MonitoringAccountPassword));
				if (mailboxDatabaseInfo2 != null)
				{
					this.CreateCafeOnPremProbe(mailboxDatabaseInfo2);
				}
				else
				{
					WTFDiagnostics.TraceInformation(ExTraceGlobals.PushNotificationTracer, base.TraceContext, string.Format("PushNotificationsDiscovery:: DoWork(): No valid Monitoring Mailbox found for CAFE. Skipping CafeProbe creation", new object[0]), null, "DoWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\PushNotifications\\PushNotificationsDiscovery.cs", 346);
				}
			}
			if (!LocalEndpointManager.IsDataCenter && instance.ExchangeServerRoleEndpoint.IsMailboxRoleInstalled)
			{
				this.CreateEnterpriseConnectivityProbe();
			}
		}

		private void CreateNotificationDeliveryMonitors()
		{
			ProbeDefinition probeDefinition = new ProbeDefinition();
			probeDefinition.AssemblyPath = Assembly.GetExecutingAssembly().Location;
			probeDefinition.Name = "PushNotificationsPublisherProbe";
			probeDefinition.TargetResource = "MSExchangePushNotificationsAppPool";
			probeDefinition.RecurrenceIntervalSeconds = 900;
			probeDefinition.TypeName = typeof(PublisherProbe).FullName;
			probeDefinition.TimeoutSeconds = 60;
			probeDefinition.ServiceName = ExchangeComponent.PushNotificationsProtocol.Name;
			if (!string.IsNullOrEmpty(this.MonitoringTenantId))
			{
				probeDefinition.Attributes["TenantId"] = this.MonitoringTenantId;
			}
			this.SetRecurrenceOverride(probeDefinition);
			if (PushNotificationsDiscovery.PublisherConfiguration.HasEnabledPublisherSettings)
			{
				if (PushNotificationsDiscovery.PublisherConfiguration.AzureDeviceRegistrationPublisherSettings.Count<AzureDeviceRegistrationPublisherSettings>() > 0)
				{
					AzureDeviceRegistrationPublisherSettings azureDeviceRegistrationPublisherSettings = PushNotificationsDiscovery.PublisherConfiguration.AzureDeviceRegistrationPublisherSettings.First<AzureDeviceRegistrationPublisherSettings>();
					this.IsDeviceRegistrationChannelEnabled = azureDeviceRegistrationPublisherSettings.Enabled;
				}
				if (PushNotificationsDiscovery.PublisherConfiguration.AzureDeviceRegistrationPublisherSettings.Count<AzureDeviceRegistrationPublisherSettings>() > 0)
				{
					AzureChallengeRequestPublisherSettings azureChallengeRequestPublisherSettings = PushNotificationsDiscovery.PublisherConfiguration.AzureChallengeRequestPublisherSettings.First<AzureChallengeRequestPublisherSettings>();
					this.IsChallengeRequestChannelEnabled = azureChallengeRequestPublisherSettings.Enabled;
					this.IsChallengeRequestChannelEnabled = false;
				}
				foreach (PushNotificationPublisherSettings pushNotificationPublisherSettings in PushNotificationsDiscovery.PublisherConfiguration.PublisherSettings)
				{
					if (!(pushNotificationPublisherSettings.GetType() == typeof(AzureHubCreationPublisherSettings)) && !(pushNotificationPublisherSettings.GetType() == typeof(AzureChallengeRequestPublisherSettings)) && !(pushNotificationPublisherSettings.GetType() == typeof(AzureDeviceRegistrationPublisherSettings)))
					{
						string eventName = "NotificationProcessed";
						string appId = pushNotificationPublisherSettings.AppId;
						string text = PushNotificationsDiscovery.MonitorNameForEvent(eventName, appId);
						ProbeDefinition probeDefinition2 = new ProbeDefinition();
						probeDefinition2.AssemblyPath = Assembly.GetExecutingAssembly().Location;
						probeDefinition2.Name = "PushNotificationsNotificationDeliveryProbe";
						probeDefinition2.TargetResource = appId;
						probeDefinition2.RecurrenceIntervalSeconds = 1800;
						probeDefinition2.TypeName = typeof(NotificationDeliveryProbe).FullName;
						probeDefinition2.TimeoutSeconds = 60;
						probeDefinition2.ServiceName = ExchangeComponent.PushNotificationsProtocol.Name;
						this.SetRecurrenceOverride(probeDefinition2);
						probeDefinition2.Attributes["TargetAppId"] = appId;
						probeDefinition2.Attributes["SkipInstanceTag"] = this.SkipInstanceOverride.ToString();
						if (pushNotificationPublisherSettings.GetType() == typeof(AzurePublisherSettings))
						{
							probeDefinition2.Attributes["IsAzureApp"] = true.ToString();
							probeDefinition2.Attributes["IsDeviceRegistrationChannelEnabled"] = this.IsDeviceRegistrationChannelEnabled.ToString();
							probeDefinition2.Attributes["IsChallengeRequestChannelEnabled"] = this.IsChallengeRequestChannelEnabled.ToString();
							AzurePublisherSettings azurePublisherSettings = pushNotificationPublisherSettings as AzurePublisherSettings;
							this.IsRegistrationEnabledOnAzureSend = azurePublisherSettings.ChannelSettings.IsRegistrationEnabled;
						}
						base.Broker.AddWorkDefinition<ProbeDefinition>(probeDefinition2, base.TraceContext);
						MonitorDefinition monitorDefinition = OverallConsecutiveProbeFailuresMonitor.CreateDefinition(text, PushNotificationsDiscovery.EventNameWithSuffix(probeDefinition2.Name, string.Empty, appId), ExchangeComponent.PushNotificationsProtocol.Name, ExchangeComponent.PushNotificationsProtocol, 2, true, 7200);
						monitorDefinition.MonitorStateTransitions = new MonitorStateTransition[]
						{
							new MonitorStateTransition(ServiceHealthStatus.Degraded, 0),
							new MonitorStateTransition(ServiceHealthStatus.Unrecoverable, this.RecurrenceIntervalOverride ?? probeDefinition2.RecurrenceIntervalSeconds)
						};
						this.SetMonitoringIntervalOverrides(monitorDefinition);
						monitorDefinition.ServicePriority = 2;
						monitorDefinition.ScenarioDescription = "Validate PushNotifications are delivered on timely manner";
						base.Broker.AddWorkDefinition<MonitorDefinition>(monitorDefinition, base.TraceContext);
						ResponderDefinition responderDefinition = ResetIISAppPoolResponder.CreateDefinition(PushNotificationsDiscovery.RecycleResponderNameForEvent(eventName, appId), text, "MSExchangePushNotificationsAppPool", ServiceHealthStatus.Degraded, DumpMode.None, null, 15.0, 0, "Exchange", true, "Dag");
						responderDefinition.ServiceName = ExchangeComponent.PushNotificationsProtocol.Name;
						this.SetRecurrenceOverride(responderDefinition);
						base.Broker.AddWorkDefinition<ResponderDefinition>(responderDefinition, base.TraceContext);
						string escalationMessage = string.Format("<b>{0}</b><br>{{Probe.StateAttribute12}}<br><br>", Strings.PushNotificationPublisherUnhealthy(appId, Environment.MachineName));
						this.CreateEscalateResponderForMonitor(eventName, ExchangeComponent.PushNotificationsProtocol, escalationMessage, appId);
					}
				}
				probeDefinition.Attributes["IsRegistrationEnabled"] = this.IsRegistrationEnabledOnAzureSend.ToString();
				base.Broker.AddWorkDefinition<ProbeDefinition>(probeDefinition, base.TraceContext);
			}
		}

		private void CreateDatacenterOnPremBackendEndpointProbe()
		{
			ProbeDefinition probeDefinition = new ProbeDefinition();
			probeDefinition.AssemblyPath = Assembly.GetExecutingAssembly().Location;
			probeDefinition.Name = "PushNotificationsDatacenterOnPremBackendEndpointProbe";
			probeDefinition.TargetResource = "MSExchangePushNotificationsAppPool";
			probeDefinition.RecurrenceIntervalSeconds = 900;
			probeDefinition.TypeName = typeof(DatacenterOnPremBackendEndpointProbe).FullName;
			probeDefinition.TimeoutSeconds = 60;
			probeDefinition.ServiceName = ExchangeComponent.PushNotificationsProtocol.Name;
			this.SetRecurrenceOverride(probeDefinition);
			base.Broker.AddWorkDefinition<ProbeDefinition>(probeDefinition, base.TraceContext);
			string eventName = "EnterpriseNotificationProcessed";
			string name = PushNotificationsDiscovery.MonitorNameForEvent(eventName, "");
			ProbeDefinition probeDefinition2 = new ProbeDefinition();
			probeDefinition2.AssemblyPath = Assembly.GetExecutingAssembly().Location;
			probeDefinition2.Name = "PushNotificationsOnPremNotificationDeliveryProbe";
			probeDefinition2.TargetResource = "MSExchangePushNotificationsAppPool";
			probeDefinition2.RecurrenceIntervalSeconds = 1800;
			probeDefinition2.TypeName = typeof(NotificationDeliveryProbe).FullName;
			probeDefinition2.TimeoutSeconds = 60;
			probeDefinition2.ServiceName = ExchangeComponent.PushNotificationsProtocol.Name;
			this.SetRecurrenceOverride(probeDefinition2);
			probeDefinition2.Attributes["SkipInstanceTag"] = this.SkipInstanceOverride.ToString();
			base.Broker.AddWorkDefinition<ProbeDefinition>(probeDefinition2, base.TraceContext);
			MonitorDefinition monitorDefinition = OverallConsecutiveProbeFailuresMonitor.CreateDefinition(name, probeDefinition2.Name, ExchangeComponent.PushNotificationsProtocol.Name, ExchangeComponent.PushNotificationsProtocol, 2, true, 7200);
			this.SetMonitoringIntervalOverrides(monitorDefinition);
			monitorDefinition.MonitorStateTransitions = new MonitorStateTransition[]
			{
				new MonitorStateTransition(ServiceHealthStatus.Unrecoverable, 0)
			};
			monitorDefinition.ServicePriority = 2;
			monitorDefinition.ScenarioDescription = "Validate PushNotifications health is not impacted by BE issues";
			base.Broker.AddWorkDefinition<MonitorDefinition>(monitorDefinition, base.TraceContext);
			string escalationMessage = string.Format("<b>{0}</b><br>{{Probe.StateAttribute12}}<br><br>", Strings.PushNotificationDatacenterBackendEndpointUnhealthy(Environment.MachineName));
			this.CreateEscalateResponderForMonitor(eventName, ExchangeComponent.PushNotificationsProtocol, escalationMessage, "");
		}

		private void CreateCafeOnPremProbe(MailboxDatabaseInfo dbInfo)
		{
			ProbeDefinition probeDefinition = new ProbeDefinition();
			probeDefinition.AssemblyPath = Assembly.GetExecutingAssembly().Location;
			probeDefinition.Name = "PushNotificationsCafeProbe";
			probeDefinition.TargetResource = "MSExchangePushNotificationsAppPool";
			probeDefinition.RecurrenceIntervalSeconds = 600;
			probeDefinition.TypeName = typeof(CafeOnPremProbe).FullName;
			probeDefinition.TimeoutSeconds = 60;
			probeDefinition.ServiceName = ExchangeComponent.PushNotificationsProxy.Name;
			probeDefinition.Attributes["AccountDomain"] = dbInfo.MonitoringAccountDomain;
			this.SetRecurrenceOverride(probeDefinition);
			base.Broker.AddWorkDefinition<ProbeDefinition>(probeDefinition, base.TraceContext);
			ProbeIdentity probeIdentity = probeDefinition;
			MonitorIdentity monitorIdentity = probeIdentity.CreateMonitorIdentity();
			MonitorDefinition monitorDefinition = OverallPercentSuccessMonitor.CreateDefinition(monitorIdentity.Name, probeIdentity.GetAlertMask(), ExchangeComponent.PushNotificationsProxy.Name, ExchangeComponent.PushNotificationsProxy, 60.0, TimeSpan.FromSeconds(3600.0), true);
			this.SetMonitoringIntervalOverrides(monitorDefinition);
			monitorDefinition.MonitorStateTransitions = new MonitorStateTransition[]
			{
				new MonitorStateTransition(ServiceHealthStatus.Degraded, 0),
				new MonitorStateTransition(ServiceHealthStatus.Unrecoverable, this.RecurrenceIntervalOverride ?? 3600)
			};
			monitorDefinition.ServicePriority = 2;
			monitorDefinition.ScenarioDescription = "Validate PushNotifications health is not impacted by CAFE issues";
			base.Broker.AddWorkDefinition<MonitorDefinition>(monitorDefinition, base.TraceContext);
			ResponderIdentity responderIdentity = monitorIdentity.CreateResponderIdentity("Escalate", null);
			ResponderDefinition responderDefinition = EscalateResponder.CreateDefinition(responderIdentity.Name, responderIdentity.Component.Name, monitorIdentity.Name, monitorIdentity.Name, responderIdentity.TargetResource, ServiceHealthStatus.Unrecoverable, responderIdentity.Component.EscalationTeam, Strings.EscalationSubjectUnhealthy, Strings.PushNotificationCafeEndpointUnhealthy, true, NotificationServiceClass.Scheduled, 14400, "Pacific Standard Time/Monday,Tuesday,Wednesday,Thursday,Friday,Saturday,Sunday/00:00/23:59", false);
			this.SetRecurrenceOverride(responderDefinition);
			base.Broker.AddWorkDefinition<ResponderDefinition>(responderDefinition, base.TraceContext);
		}

		private void CreateEnterpriseConnectivityProbe()
		{
			ProbeDefinition probeDefinition = new ProbeDefinition();
			probeDefinition.AssemblyPath = Assembly.GetExecutingAssembly().Location;
			probeDefinition.Name = "PushNotificationsEnterpriseConnectivityProbe";
			probeDefinition.TypeName = typeof(EnterpriseConnectivityProbe).FullName;
			probeDefinition.ServiceName = ExchangeComponent.PushNotificationsProxy.Name;
			probeDefinition.TimeoutSeconds = 360;
			probeDefinition.Enabled = false;
			base.Broker.AddWorkDefinition<ProbeDefinition>(probeDefinition, base.TraceContext);
		}

		private void CreateAPNSPublisherChannelMonitors()
		{
			this.CreatePublisherChannelMonitors(from app in PushNotificationsDiscovery.PublisherConfiguration.ApnsPublisherSettings
			select app.AppId, PublisherType.APNS);
		}

		private void CreateWnsPublisherChannelMonitors()
		{
			this.CreatePublisherChannelMonitors(from app in PushNotificationsDiscovery.PublisherConfiguration.WnsPublisherSettings
			select app.AppId, PublisherType.WNS);
		}

		private void CreateGcmPublisherChannelMonitors()
		{
			this.CreatePublisherChannelMonitors(from app in PushNotificationsDiscovery.PublisherConfiguration.GcmPublisherSettings
			select app.AppId, PublisherType.GCM);
		}

		private void CreateWebAppPublisherChannelMonitors()
		{
			this.CreatePublisherChannelMonitors(from app in PushNotificationsDiscovery.PublisherConfiguration.WebAppPublisherSettings
			select app.AppId, PublisherType.WebApp);
		}

		private void CreateAzurePublisherChannelMonitors()
		{
			this.CreatePublisherChannelMonitors(from app in PushNotificationsDiscovery.PublisherConfiguration.AzurePublisherSettings
			select app.AppId, PublisherType.Azure);
		}

		private void CreateAzureHubCreationPublisherChannelMonitors()
		{
			this.CreatePublisherChannelMonitors(from app in PushNotificationsDiscovery.PublisherConfiguration.AzureHubCreationPublisherSettings
			select app.AppId, PublisherType.AzureHubCreation);
		}

		private void CreateAzureDeviceRegistrationPublisherChannelMonitors()
		{
			this.CreatePublisherChannelMonitors(from app in PushNotificationsDiscovery.PublisherConfiguration.AzureDeviceRegistrationPublisherSettings
			select app.AppId, PublisherType.AzureDeviceRegistration);
		}

		private void CreateAzureChallengeRequestPublisherChannelMonitors()
		{
			this.CreatePublisherChannelMonitors(from app in PushNotificationsDiscovery.PublisherConfiguration.AzureChallengeRequestPublisherSettings
			select app.AppId, PublisherType.AzureChallengeRequest);
		}

		private void CreatePublisherChannelMonitors(IEnumerable<string> targetAppIds, PublisherType pubType)
		{
			foreach (string text in targetAppIds)
			{
				string name = PushNotificationsDiscovery.MonitorNameForEvent("PublisherChannelHealth", text);
				ProbeDefinition probeDefinition = new ProbeDefinition();
				probeDefinition.AssemblyPath = Assembly.GetExecutingAssembly().Location;
				probeDefinition.Name = "PushNotificationsPublisherChannelHealthProbe";
				probeDefinition.TargetResource = text;
				probeDefinition.RecurrenceIntervalSeconds = 3600;
				probeDefinition.TypeName = typeof(PublisherChannelHealthProbe).FullName;
				probeDefinition.TimeoutSeconds = 60;
				probeDefinition.ServiceName = ExchangeComponent.PushNotificationsProtocol.Name;
				probeDefinition.Attributes["TargetAppId"] = text;
				probeDefinition.Attributes["TargetAppPublisher"] = pubType.ToString();
				probeDefinition.Attributes["SkipInstanceTag"] = this.SkipInstanceOverride.ToString();
				this.SetRecurrenceOverride(probeDefinition);
				base.Broker.AddWorkDefinition<ProbeDefinition>(probeDefinition, base.TraceContext);
				MonitorDefinition monitorDefinition = OverallConsecutiveProbeFailuresMonitor.CreateDefinition(name, PushNotificationsDiscovery.EventNameWithSuffix(probeDefinition.Name, string.Empty, text), ExchangeComponent.PushNotificationsProtocol.Name, ExchangeComponent.PushNotificationsProtocol, 2, true, 14400);
				monitorDefinition.MonitorStateTransitions = new MonitorStateTransition[]
				{
					new MonitorStateTransition(ServiceHealthStatus.Unrecoverable, this.RecurrenceIntervalOverride ?? probeDefinition.RecurrenceIntervalSeconds)
				};
				this.SetMonitoringIntervalOverrides(monitorDefinition);
				monitorDefinition.ServicePriority = 2;
				monitorDefinition.ScenarioDescription = string.Format("Validate Publisher Channel for {0} is healthy", text);
				base.Broker.AddWorkDefinition<MonitorDefinition>(monitorDefinition, base.TraceContext);
				string text2 = "<b>{0}</b><br>The status of each channel events is:<br>{{Probe.StateAttribute12}}<br><br>Error Traces for the failed events: <br>{{Probe.StateAttribute13}}<br><br>";
				text2 = string.Format(text2, Strings.PushNotificationChannelError(pubType.ToString(), text, Environment.MachineName));
				this.CreateEscalateResponderForMonitor("PublisherChannelHealth", ExchangeComponent.PushNotificationsProtocol, text2, text);
			}
		}

		private void CreateAzureHubCreationProbe()
		{
			foreach (string text in from app in PushNotificationsDiscovery.PublisherConfiguration.AzurePublisherSettings
			select app.AppId)
			{
				ProbeDefinition probeDefinition = new ProbeDefinition();
				probeDefinition.AssemblyPath = Assembly.GetExecutingAssembly().Location;
				probeDefinition.Name = "PushNotificationsAzureHubCreationProbe";
				probeDefinition.TargetResource = text;
				probeDefinition.RecurrenceIntervalSeconds = 1800;
				probeDefinition.TypeName = typeof(AzureHubCreationProbe).FullName;
				probeDefinition.TimeoutSeconds = 60;
				probeDefinition.ServiceName = ExchangeComponent.PushNotificationsProtocol.Name;
				probeDefinition.Attributes["TargetAppId"] = text;
				probeDefinition.Attributes["TenantId"] = this.MonitoringTenantId;
				this.SetRecurrenceOverride(probeDefinition);
				base.Broker.AddWorkDefinition<ProbeDefinition>(probeDefinition, base.TraceContext);
			}
		}

		private void CreateDeviceRegistrationProbe()
		{
			if (!this.IsDeviceRegistrationChannelEnabled)
			{
				return;
			}
			foreach (string text in from app in PushNotificationsDiscovery.PublisherConfiguration.AzurePublisherSettings
			select app.AppId)
			{
				ProbeDefinition probeDefinition = new ProbeDefinition();
				probeDefinition.AssemblyPath = Assembly.GetExecutingAssembly().Location;
				probeDefinition.Name = "PushNotificationsDeviceRegistrationProbe";
				probeDefinition.TargetResource = text;
				probeDefinition.RecurrenceIntervalSeconds = 1800;
				probeDefinition.TypeName = typeof(AzureDeviceRegistrationProbe).FullName;
				probeDefinition.TimeoutSeconds = 60;
				probeDefinition.ServiceName = ExchangeComponent.PushNotificationsProtocol.Name;
				probeDefinition.Attributes["TargetAppId"] = text;
				probeDefinition.Attributes["TenantId"] = this.MonitoringTenantId;
				this.SetRecurrenceOverride(probeDefinition);
				base.Broker.AddWorkDefinition<ProbeDefinition>(probeDefinition, base.TraceContext);
			}
		}

		private void CreateChallengeRequestProbe()
		{
			if (!this.IsChallengeRequestChannelEnabled)
			{
				return;
			}
			foreach (string text in from app in PushNotificationsDiscovery.PublisherConfiguration.AzurePublisherSettings
			select app.AppId)
			{
				ProbeDefinition probeDefinition = new ProbeDefinition();
				probeDefinition.AssemblyPath = Assembly.GetExecutingAssembly().Location;
				probeDefinition.Name = "PushNotificationsChallengeRequestProbe";
				probeDefinition.TargetResource = text;
				probeDefinition.RecurrenceIntervalSeconds = 1800;
				probeDefinition.TypeName = typeof(AzureChallengeRequestProbe).FullName;
				probeDefinition.TimeoutSeconds = 60;
				probeDefinition.ServiceName = ExchangeComponent.PushNotificationsProtocol.Name;
				probeDefinition.Attributes["TargetAppId"] = text;
				probeDefinition.Attributes["TenantId"] = this.MonitoringTenantId;
				PushNotificationPlatform pushNotificationPlatform = PushNotificationsMonitoring.CannedAppPlatformSet[text];
				probeDefinition.Attributes["TenantId"] = pushNotificationPlatform.ToString();
				this.SetRecurrenceOverride(probeDefinition);
				probeDefinition.Enabled = false;
				base.Broker.AddWorkDefinition<ProbeDefinition>(probeDefinition, base.TraceContext);
			}
		}

		private void CreateEscalateResponderForMonitor(string eventName, Component exchangeComponent, string escalationMessage, string targetResource = "")
		{
			string text = PushNotificationsDiscovery.MonitorNameForEvent(eventName, targetResource);
			string arg = string.Format("http://aka.ms/decisiontrees?Page=RecoveryHome&service=Exchange&escalationteam=Push%20Notification%20Services&alerttypeid={0}&id=0&alertname=dummy", text);
			string format = "<br><br><a href='{0}'>Battlecards</a><br><a href='{1}'>OneNote Battlecards</a>";
			if (text.ToLower().Contains("publisherchannel"))
			{
				escalationMessage += string.Format(format, arg, "onenote:///\\\\exstore\\files\\userfiles\\servicesoncall\\OnCall%20Battlecard\\New%20Battlecards.one#PublisherChannelHealth%20Unhealthy&section-id={9E24DEE5-34D7-4463-AB20-386D859233BA}&page-id={176FC0F6-8A1C-4E9C-8F38-31F0D678279A}&end");
			}
			else if (text.ToLower().Contains("notificationprocessed"))
			{
				escalationMessage += string.Format(format, arg, "onenote:///\\\\exstore\\files\\userfiles\\servicesoncall\\OnCall%20Battlecard\\New%20Battlecards.one#NotificationProcessedMonitor%20Unhealthy&section-id={9E24DEE5-34D7-4463-AB20-386D859233BA}&page-id={1A62CAB4-5688-4009-9297-D14ADCFBE2EF}&end");
			}
			else if (text.ToLower().Contains("cafemonitor"))
			{
				escalationMessage += string.Format(format, arg, "onenote:///\\\\exstore\\files\\userfiles\\servicesoncall\\OnCall%20Battlecard\\New%20Battlecards.one#Push%20Notifications%20Cafe%20Monitor&section-id={9E24DEE5-34D7-4463-AB20-386D859233BA}&page-id={0243B6CA-4719-49D3-8A93-58D7DEDCFB39}&end");
			}
			ResponderDefinition responderDefinition = EscalateResponder.CreateDefinition(PushNotificationsDiscovery.EscalateResponderNameForEvent(eventName, targetResource), exchangeComponent.Name, text, text, string.Empty, ServiceHealthStatus.Unrecoverable, exchangeComponent.EscalationTeam, Strings.EscalationSubjectUnhealthy, escalationMessage, true, NotificationServiceClass.Scheduled, 14400, "Pacific Standard Time/Monday,Tuesday,Wednesday,Thursday,Friday,Saturday,Sunday/00:00/23:59", false);
			this.SetRecurrenceOverride(responderDefinition);
			base.Broker.AddWorkDefinition<ResponderDefinition>(responderDefinition, base.TraceContext);
		}

		private void SetMonitoringIntervalOverrides(MonitorDefinition monitor)
		{
			this.SetRecurrenceOverride(monitor);
			if (this.MonitoringIntervalOverride != null)
			{
				monitor.MonitoringIntervalSeconds = this.MonitoringIntervalOverride.Value;
			}
		}

		private void SetRecurrenceOverride(WorkDefinition definition)
		{
			definition.RecurrenceIntervalSeconds = (this.RecurrenceIntervalOverride ?? definition.RecurrenceIntervalSeconds);
		}

		private void LoadIntervalOverrides()
		{
			string text = this.ReadAttribute("MonitoringIntervalOverride", null);
			string text2 = this.ReadAttribute("RecurrenceIntervalOverride", null);
			string value = this.ReadAttribute("SkipInstanceTag", null);
			if (!string.IsNullOrEmpty(text))
			{
				this.MonitoringIntervalOverride = new int?(int.Parse(text));
			}
			if (!string.IsNullOrEmpty(text2))
			{
				this.RecurrenceIntervalOverride = new int?(int.Parse(text2));
			}
			bool skipInstanceOverride = true;
			if (!string.IsNullOrEmpty(value))
			{
				bool.TryParse(value, out skipInstanceOverride);
			}
			this.SkipInstanceOverride = skipInstanceOverride;
		}

		public const string SkipInstanceTagProperty = "SkipInstanceTag";

		public const string MonitoringIntervalOverrideProperty = "MonitoringIntervalOverride";

		public const string RecurrenceIntervalOverrideProperty = "RecurrenceIntervalOverride";

		internal const string AccountDomainAttribute = "AccountDomain";

		internal const string NotificationDeliveryProbeName = "PushNotificationsNotificationDeliveryProbe";

		internal const string PublisherChannelHealthProbeName = "PushNotificationsPublisherChannelHealthProbe";

		internal const string AzureHubCreationProbeName = "PushNotificationsAzureHubCreationProbe";

		internal const string DeviceRegistrationProbeName = "PushNotificationsDeviceRegistrationProbe";

		internal const string ChallengeRequestProbeName = "PushNotificationsChallengeRequestProbe";

		internal const string OnPremNotificationDeliveryProbeName = "PushNotificationsOnPremNotificationDeliveryProbe";

		internal const string PublisherProbeName = "PushNotificationsPublisherProbe";

		internal const string DatacenterOnPremBackendEndpointProbeName = "PushNotificationsDatacenterOnPremBackendEndpointProbe";

		internal const string CafeProbeName = "PushNotificationsCafeProbe";

		internal const string EnterpriseConnectivityProbeName = "PushNotificationsEnterpriseConnectivityProbe";

		internal const string PushNotificationsAppPool = "MSExchangePushNotificationsAppPool";

		internal const string PublisherChannelHealthEvent = "PublisherChannelHealth";

		internal const int DefaultMonitorThreshold = 2;

		private const string EscalateResponderNameSuffix = "Escalate";

		private const string RecycleResponderNameSuffix = "Recycle";

		private const string MonitorNameSuffix = "Monitor";

		private const int DefaultMonitoringIntervalInSeconds = 3600;

		private const int DefaultConsecutiveMonitoringIntervalInSeconds = 1800;

		private const double DefaultSuccessThreshold = 90.0;

		private const int DefaultProbeTimeoutInSeconds = 60;

		private const string BattleCardPageUrl = "http://aka.ms/decisiontrees?Page=RecoveryHome&service=Exchange&escalationteam=Push%20Notification%20Services&alerttypeid={0}&id=0&alertname=dummy";

		private const string OneNotePublisherChannelHealth = "onenote:///\\\\exstore\\files\\userfiles\\servicesoncall\\OnCall%20Battlecard\\New%20Battlecards.one#PublisherChannelHealth%20Unhealthy&section-id={9E24DEE5-34D7-4463-AB20-386D859233BA}&page-id={176FC0F6-8A1C-4E9C-8F38-31F0D678279A}&end";

		private const string OneNoteNotificationProcessed = "onenote:///\\\\exstore\\files\\userfiles\\servicesoncall\\OnCall%20Battlecard\\New%20Battlecards.one#NotificationProcessedMonitor%20Unhealthy&section-id={9E24DEE5-34D7-4463-AB20-386D859233BA}&page-id={1A62CAB4-5688-4009-9297-D14ADCFBE2EF}&end";

		private const string OneNoteCafeMonitor = "onenote:///\\\\exstore\\files\\userfiles\\servicesoncall\\OnCall%20Battlecard\\New%20Battlecards.one#Push%20Notifications%20Cafe%20Monitor&section-id={9E24DEE5-34D7-4463-AB20-386D859233BA}&page-id={0243B6CA-4719-49D3-8A93-58D7DEDCFB39}&end";

		private static readonly Lazy<PushNotificationPublisherConfiguration> publisherConfiguration = new Lazy<PushNotificationPublisherConfiguration>(() => new PushNotificationPublisherConfiguration(false, null));
	}
}
