using System;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using System.Threading;
using Microsoft.Exchange.Cluster.Replay;
using Microsoft.Exchange.Cluster.Shared;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Management;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.HA.DirectoryServices;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.ActiveMonitoring;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.ActiveMonitoring.Responders;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.Common;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.HighAvailability;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.Local;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.Local.Components.Common.Probes;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.Provisioning.Probes;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.Provisioning.Responders;
using Microsoft.Office.Datacenter.ActiveMonitoring;
using Microsoft.Office.Datacenter.Monitoring.ActiveMonitoring.Recovery;
using Microsoft.Office.Datacenter.WorkerTaskFramework;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.Provisioning
{
	public sealed class ProvisioningDiscovery : MaintenanceWorkItem
	{
		protected override void DoWork(CancellationToken cancellationToken)
		{
			try
			{
				LocalEndpointManager instance = LocalEndpointManager.Instance;
				if (!LocalEndpointManager.IsDataCenter || instance.ExchangeServerRoleEndpoint == null || !instance.ExchangeServerRoleEndpoint.IsClientAccessRoleInstalled)
				{
					WTFDiagnostics.TraceInformation(ExTraceGlobals.ProvisioningTracer, base.TraceContext, "ProvisioningDiscovery:: DoWork(): not running in Datacenter CAS box. Skip.", null, "DoWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Provisioning\\ProvisioningDiscovery.cs", 178);
					return;
				}
			}
			catch (EndpointManagerEndpointUninitializedException)
			{
				WTFDiagnostics.TraceInformation(ExTraceGlobals.ProvisioningTracer, base.TraceContext, "ProvisioningDiscovery:: DoWork(): EndpointManagerEndpointUninitializedException. Skip.", null, "DoWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Provisioning\\ProvisioningDiscovery.cs", 185);
				return;
			}
			this.attributeHelper = new AttributeHelper(base.Definition);
			ProvisioningDiscovery.isTestTopology = ExEnvironment.IsTest;
			this.CreateForwardSyncCookieContext();
			this.CreateForwardSyncHaltContext();
			this.CreateForwardFullSyncContext();
			this.CreateForwardSyncCompanyContext();
		}

		private void CreateProcessCrashingContext(string processName)
		{
			int num = 300;
			int num2 = 10;
			if (ProvisioningDiscovery.isTestTopology)
			{
				num = 30;
				num2 = 1;
			}
			ProbeDefinition probeDefinition = GenericProcessCrashDetectionProbe.CreateDefinition("ForwardSyncProcessRepeatedlyCrashingProbe", processName, num, null, false);
			probeDefinition.ServiceName = "Provisioning";
			base.Broker.AddWorkDefinition<ProbeDefinition>(probeDefinition, base.TraceContext);
			MonitorDefinition monitorDefinition = OverallXFailuresMonitor.CreateDefinition("ForwardSyncProcessRepeatedlyCrashingMonitor", probeDefinition.ConstructWorkItemResultName(), "Provisioning", ExchangeComponent.Provisioning, 7200, num, num2, true);
			monitorDefinition.ServicePriority = 1;
			monitorDefinition.ScenarioDescription = "Validate Provisioning health is not impacted by crashes";
			base.Broker.AddWorkDefinition<MonitorDefinition>(monitorDefinition, base.TraceContext);
			ResponderDefinition responderDefinition = EscalateResponder.CreateDefinition("ForwardSyncProcessRepeatedlyCrashingEscalate", "Provisioning", monitorDefinition.Name, monitorDefinition.ConstructWorkItemResultName(), processName, ServiceHealthStatus.None, ExchangeComponent.Provisioning.EscalationTeam, Strings.ForwardSyncProcessRepeatedlyCrashingEscalationSubject, Strings.ForwardSyncProcessRepeatedlyCrashingEscalationMessage(num2, 7200), true, NotificationServiceClass.Urgent, 14400, "Pacific Standard Time/Monday,Tuesday,Wednesday,Thursday,Friday,Saturday,Sunday/00:00/23:59", false);
			responderDefinition.RecurrenceIntervalSeconds = num;
			responderDefinition.TimeoutSeconds = num;
			if (this.IsPDTEnvironment())
			{
				responderDefinition.NotificationServiceClass = NotificationServiceClass.Scheduled;
			}
			base.Broker.AddWorkDefinition<ResponderDefinition>(responderDefinition, base.TraceContext);
		}

		private void CreateForwardSyncCookieContext()
		{
			int num = 600;
			int failureCount = 3;
			int monitoringInterval = 2400;
			int waitIntervalSeconds = 3600;
			if (ProvisioningDiscovery.isTestTopology)
			{
				num = 20;
				failureCount = 2;
				monitoringInterval = 60;
				waitIntervalSeconds = 120;
			}
			ProbeDefinition probeDefinition = ForwardSyncCookieProbe.CreateDefinition("ForwardSyncCookieNotUpToDateProbe", "Provisioning", num);
			string extensionAttributes = this.ReadAttribute("EnableSTXLogging", "true");
			probeDefinition.ExtensionAttributes = extensionAttributes;
			string targetExtension = this.ReadAttribute("CookieStaleTime", "60");
			probeDefinition.TargetExtension = targetExtension;
			probeDefinition.ServiceName = "Provisioning";
			base.Broker.AddWorkDefinition<ProbeDefinition>(probeDefinition, base.TraceContext);
			MonitorDefinition monitorDefinition = OverallConsecutiveProbeFailuresMonitor.CreateDefinition("ForwardSyncCookieNotUpToDateMonitor", probeDefinition.ConstructWorkItemResultName(), "Provisioning", ExchangeComponent.Provisioning, failureCount, true, monitoringInterval);
			monitorDefinition.RecurrenceIntervalSeconds = 0;
			monitorDefinition.MonitorStateTransitions = new MonitorStateTransition[]
			{
				new MonitorStateTransition(ServiceHealthStatus.Unhealthy, 0),
				new MonitorStateTransition(ServiceHealthStatus.Unrecoverable, (int)TimeSpan.FromMinutes(45.0).TotalSeconds)
			};
			monitorDefinition.ServicePriority = 1;
			monitorDefinition.ScenarioDescription = "Validate Provisioning health is not impacted by process crashes";
			base.Broker.AddWorkDefinition<MonitorDefinition>(monitorDefinition, base.TraceContext);
			ResponderDefinition responderDefinition = RestartServiceResponder.CreateDefinition("ForwardSyncCookieNotUpToDateRestart", monitorDefinition.ConstructWorkItemResultName(), "MSExchangeForwardSync", ServiceHealthStatus.Unhealthy, 15, 120, 0, false, DumpMode.None, null, 15.0, 0, "Exchange", null, true, true, null, false);
			ProvisioningSTXRestartServiceResponder.InitTypeNameAndAssemblyPath(responderDefinition);
			responderDefinition.Attributes["ServiceStartTimeout"] = TimeSpan.FromSeconds(300.0).ToString();
			responderDefinition.AlertTypeId = monitorDefinition.Name;
			responderDefinition.ServiceName = "Provisioning";
			responderDefinition.RecurrenceIntervalSeconds = num;
			responderDefinition.TimeoutSeconds = num;
			responderDefinition.WaitIntervalSeconds = waitIntervalSeconds;
			responderDefinition.ExtensionAttributes = extensionAttributes;
			responderDefinition.TargetResource = "Provisioning";
			base.Broker.AddWorkDefinition<ResponderDefinition>(responderDefinition, base.TraceContext);
			ResponderDefinition responderDefinition2 = EscalateResponder.CreateDefinition("ForwardSyncCookieNotUpToDateEscalate", "Provisioning", monitorDefinition.Name, monitorDefinition.ConstructWorkItemResultName(), "Provisioning", ServiceHealthStatus.Unrecoverable, ExchangeComponent.Provisioning.EscalationTeam, Strings.ForwardSyncCookieNotUpToDateEscalationSubject, Strings.ForwardSyncCookieNotUpToDateEscalationMessage, true, NotificationServiceClass.Urgent, 14400, "Pacific Standard Time/Monday,Tuesday,Wednesday,Thursday,Friday,Saturday,Sunday/00:00/23:59", false);
			ProvisioningSTXEscalateResponder.InitTypeNameAndAssemblyPath(responderDefinition2);
			responderDefinition2.RecurrenceIntervalSeconds = num;
			responderDefinition2.TimeoutSeconds = num;
			responderDefinition2.ExtensionAttributes = extensionAttributes;
			if (this.IsPDTEnvironment())
			{
				responderDefinition2.NotificationServiceClass = NotificationServiceClass.Scheduled;
			}
			base.Broker.AddWorkDefinition<ResponderDefinition>(responderDefinition2, base.TraceContext);
		}

		private void CreateForwardSyncHaltContext()
		{
			int minimumSecondsBetweenEscalates = (int)TimeSpan.FromMinutes(60.0).TotalSeconds;
			int recurrenceIntervalSeconds = (int)TimeSpan.FromMinutes(5.0).TotalSeconds;
			if (ProvisioningDiscovery.isTestTopology)
			{
				minimumSecondsBetweenEscalates = 60;
				recurrenceIntervalSeconds = 60;
			}
			MonitorDefinition monitorDefinition = OverallConsecutiveProbeFailuresMonitor.CreateDefinition("ForwardSyncHaltMonitor", NotificationItem.GenerateResultName(ExchangeComponent.Provisioning.Name, ExchangeComponent.Provisioning.Name, "ForwardSyncHalted"), "Provisioning", ExchangeComponent.Provisioning, 1, true, 300);
			monitorDefinition.MonitorStateTransitions = new MonitorStateTransition[]
			{
				new MonitorStateTransition(ServiceHealthStatus.Unrecoverable, 0)
			};
			monitorDefinition.RecurrenceIntervalSeconds = 0;
			monitorDefinition.ServicePriority = 1;
			monitorDefinition.ScenarioDescription = "Validate Provisioning health is not impacted by ForwardSync halt issues";
			base.Broker.AddWorkDefinition<MonitorDefinition>(monitorDefinition, base.TraceContext);
			ResponderDefinition responderDefinition = EscalateResponder.CreateDefinition("ForwardSyncHaltEscalate", "Provisioning", monitorDefinition.Name, monitorDefinition.ConstructWorkItemResultName(), "Provisioning", ServiceHealthStatus.Unrecoverable, ExchangeComponent.Provisioning.EscalationTeam, Strings.ForwardSyncHaltEscalationSubject, Strings.ForwardSyncHaltEscalationMessage, true, NotificationServiceClass.Urgent, minimumSecondsBetweenEscalates, "Pacific Standard Time/Monday,Tuesday,Wednesday,Thursday,Friday,Saturday,Sunday/00:00/23:59", false);
			responderDefinition.RecurrenceIntervalSeconds = recurrenceIntervalSeconds;
			if (this.IsPDTEnvironment() || !ForwardSyncEventlogUtil.IsForwardSyncActiveServer())
			{
				responderDefinition.NotificationServiceClass = NotificationServiceClass.Scheduled;
			}
			base.Broker.AddWorkDefinition<ResponderDefinition>(responderDefinition, base.TraceContext);
		}

		private bool IsServerInFirstE15DAG()
		{
			bool result = false;
			IADDatabaseAvailabilityGroup iaddatabaseAvailabilityGroup = null;
			WTFDiagnostics.TraceInformation(ExTraceGlobals.ProvisioningTracer, base.TraceContext, "ProvisioningDiscovery.IsServerInFirstE15DAG(): Get Local Server DAG.", null, "IsServerInFirstE15DAG", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Provisioning\\ProvisioningDiscovery.cs", 442);
			IADServer localServer = CachedAdReader.Instance.LocalServer;
			if (localServer == null || localServer.DatabaseAvailabilityGroup == null)
			{
				WTFDiagnostics.TraceInformation(ExTraceGlobals.ProvisioningTracer, base.TraceContext, "ProvisioningDiscovery.IsServerInFirstE15DAG() return false because Local Server DAG is null.", null, "IsServerInFirstE15DAG", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Provisioning\\ProvisioningDiscovery.cs", 446);
				return false;
			}
			IADDatabaseAvailabilityGroup localDAG = CachedAdReader.Instance.LocalDAG;
			if (localDAG != null)
			{
				WTFDiagnostics.TraceInformation<string>(ExTraceGlobals.ProvisioningTracer, base.TraceContext, "ProvisioningDiscovery.IsServerInFirstE15DAG(): Local Server DAG is {0}. Now get all DAGs sort by Name.", localDAG.Name, null, "IsServerInFirstE15DAG", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Provisioning\\ProvisioningDiscovery.cs", 453);
				ITopologyConfigurationSession topologyConfigurationSession = DirectorySessionFactory.Default.CreateTopologyConfigurationSession(ConsistencyMode.IgnoreInvalid, ADSessionSettings.FromRootOrgScopeSet(), 454, "IsServerInFirstE15DAG", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Provisioning\\ProvisioningDiscovery.cs");
				DatabaseAvailabilityGroup[] array = topologyConfigurationSession.Find<DatabaseAvailabilityGroup>(null, QueryScope.SubTree, null, new SortBy(DatabaseAvailabilityGroupSchema.Name, SortOrder.Ascending), 0);
				if (array != null)
				{
					DatabaseAvailabilityGroup[] array2 = array;
					int i = 0;
					while (i < array2.Length)
					{
						DatabaseAvailabilityGroup databaseAvailabilityGroup = array2[i];
						if (databaseAvailabilityGroup.Servers != null && databaseAvailabilityGroup.Servers.Count > 0)
						{
							try
							{
								WTFDiagnostics.TraceInformation<string>(ExTraceGlobals.ProvisioningTracer, base.TraceContext, "ProvisioningDiscovery.IsServerInFirstE15DAG(): Get PAM of DAG {0}.", databaseAvailabilityGroup.Name, null, "IsServerInFirstE15DAG", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Provisioning\\ProvisioningDiscovery.cs", 464);
								AmServerName primaryActiveManagerNode = DagTaskHelper.GetPrimaryActiveManagerNode(databaseAvailabilityGroup);
								if (primaryActiveManagerNode != null && !string.IsNullOrEmpty(primaryActiveManagerNode.Fqdn))
								{
									WTFDiagnostics.TraceInformation<string>(ExTraceGlobals.ProvisioningTracer, base.TraceContext, "ProvisioningDiscovery.IsServerInFirstE15DAG(): Check version and monitoring of {0}.", primaryActiveManagerNode.Fqdn, null, "IsServerInFirstE15DAG", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Provisioning\\ProvisioningDiscovery.cs", 468);
									Server exchangeServerByName = DirectoryAccessor.Instance.GetExchangeServerByName(primaryActiveManagerNode.Fqdn);
									if (exchangeServerByName != null && exchangeServerByName.AdminDisplayVersion.Major >= 15 && !DirectoryAccessor.Instance.IsMonitoringOffline(exchangeServerByName))
									{
										WTFDiagnostics.TraceInformation<string>(ExTraceGlobals.ProvisioningTracer, base.TraceContext, "ProvisioningDiscovery.IsServerInFirstE15DAG(): Found first E15 DAG: {0}.", databaseAvailabilityGroup.Name, null, "IsServerInFirstE15DAG", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Provisioning\\ProvisioningDiscovery.cs", 472);
										iaddatabaseAvailabilityGroup = ADObjectWrapperFactory.CreateWrapper(databaseAvailabilityGroup);
										break;
									}
								}
								goto IL_25C;
							}
							catch (Exception ex)
							{
								WTFDiagnostics.TraceInformation<string, string>(ExTraceGlobals.ProvisioningTracer, base.TraceContext, "ProvisioningDiscovery.IsServerInFirstE15DAG(): Skip DAG {0} because exception {1}.", databaseAvailabilityGroup.Name, ex.ToString(), null, "IsServerInFirstE15DAG", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Provisioning\\ProvisioningDiscovery.cs", 480);
								goto IL_25C;
							}
							goto IL_230;
						}
						goto IL_230;
						IL_25C:
						i++;
						continue;
						IL_230:
						WTFDiagnostics.TraceInformation<string>(ExTraceGlobals.ProvisioningTracer, base.TraceContext, "ProvisioningDiscovery.IsServerInFirstE15DAG(): Skip DAG {0} as it has no servers.", databaseAvailabilityGroup.Name, null, "IsServerInFirstE15DAG", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Provisioning\\ProvisioningDiscovery.cs", 485);
						goto IL_25C;
					}
				}
				if (iaddatabaseAvailabilityGroup != null && iaddatabaseAvailabilityGroup.Name.CompareTo(localDAG.Name) == 0)
				{
					WTFDiagnostics.TraceInformation(ExTraceGlobals.ProvisioningTracer, base.TraceContext, "ProvisioningDiscovery.IsServerInFirstE15DAG(): Server is in first E15 DAG.", null, "IsServerInFirstE15DAG", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Provisioning\\ProvisioningDiscovery.cs", 492);
					result = true;
				}
			}
			return result;
		}

		private void CreateForwardSyncCompanyContext()
		{
			if (!ProvisioningDiscovery.isTestTopology && !this.IsServerInFirstE15DAG())
			{
				return;
			}
			int num = ProvisioningDiscovery.isTestTopology ? 180 : 1800;
			string targetResource = string.Empty;
			string account = string.Empty;
			string text = string.Empty;
			string text2 = string.Empty;
			string text3 = string.Empty;
			string text4 = "CompanyStx*";
			WTFDiagnostics.TraceInformation(ExTraceGlobals.ProvisioningTracer, base.TraceContext, "ProvisioningDiscovery.CreateForwardSyncCompanyProbes(): Get all ForwardSync monitoring tenants.", null, "CreateForwardSyncCompanyContext", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Provisioning\\ProvisioningDiscovery.cs", 522);
			try
			{
				IEnumerable<ExchangeConfigurationUnit> enumerable = PartitionDataAggregator.FindForwardSyncMonitoringTenants();
				if (enumerable != null)
				{
					foreach (ExchangeConfigurationUnit exchangeConfigurationUnit in enumerable)
					{
						if (exchangeConfigurationUnit.AdminDisplayVersion.ExchangeBuild.Major >= 15)
						{
							WTFDiagnostics.TraceInformation<string>(ExTraceGlobals.ProvisioningTracer, base.TraceContext, "ProvisioningDiscovery.CreateForwardSyncCompanyProbes(): Get all mailcontacts with pattern {0}.", text4, null, "CreateForwardSyncCompanyContext", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Provisioning\\ProvisioningDiscovery.cs", 535);
							IRecipientSession session = DirectorySessionFactory.Default.CreateTenantRecipientSession(true, ConsistencyMode.PartiallyConsistent, ADSessionSettings.FromExternalDirectoryOrganizationId(new Guid(exchangeConfigurationUnit.ExternalDirectoryOrganizationId)), 536, "CreateForwardSyncCompanyContext", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Provisioning\\ProvisioningDiscovery.cs");
							MailContactIdParameter mailContactIdParameter = new MailContactIdParameter(text4);
							IEnumerable<ADContact> objects = mailContactIdParameter.GetObjects<ADContact>(null, session);
							if (objects != null)
							{
								text2 = exchangeConfigurationUnit.DirSyncServiceInstance;
								text3 = text2.Substring(text2.IndexOf('/') + 1);
								int companySla;
								if (text3.EndsWith("-2") || text3.EndsWith("-02"))
								{
									text = "EXCHANGELITE";
									companySla = int.Parse(this.ReadAttribute("CompanySlaSecondsBPOS_L", "300"));
								}
								else
								{
									text = "EXCHANGESTANDARD";
									companySla = int.Parse(this.ReadAttribute("CompanySlaSecondsBPOS_S", "600"));
								}
								foreach (ADContact dataObject in objects)
								{
									MailContact mailContact = new MailContact(dataObject);
									targetResource = mailContact.Alias;
									account = mailContact.ExternalEmailAddress.AddressString;
									string name = "ForwardSyncCompanyProbe" + '-' + text3;
									ProbeDefinition probeDefinition = ForwardSyncCompanyProbe.CreateDefinition(name, companySla);
									probeDefinition.RecurrenceIntervalSeconds = num;
									probeDefinition.TimeoutSeconds = num / 2;
									probeDefinition.TargetGroup = text;
									probeDefinition.TargetResource = targetResource;
									probeDefinition.Account = account;
									probeDefinition.AccountPassword = this.ReadAttribute("AdminPassword", string.Empty);
									probeDefinition.AccountDisplayName = text;
									probeDefinition.Endpoint = text2;
									string extensionAttributes = this.ReadAttribute("EnableSTXLogging", "true");
									probeDefinition.ExtensionAttributes = extensionAttributes;
									probeDefinition.Attributes["ProdCertificateSubject"] = this.attributeHelper.GetString("ProdCertificateSubject", true, "CN=auth.outlook.com, OU=Exchange, O=Microsoft Corporation, L=Redmond, S=Washington, C=US");
									probeDefinition.Attributes["SDFCertificateSubject"] = this.attributeHelper.GetString("SDFCertificateSubject", true, "CN=auth.outlook.com, OU=Exchange, O=Microsoft Corporation, L=Redmond, S=Washington, C=US");
									probeDefinition.Attributes["GallatinCertificateSubject"] = this.attributeHelper.GetString("GallatinCertificateSubject", true, "CN=auth.outlook.com, OU=Exchange, O=Microsoft Corporation, L=Redmond, S=Washington, C=US");
									probeDefinition.Attributes["TDCertificateSubject"] = this.attributeHelper.GetString("TDCertificateSubject", true, "auth.exchangelabs.live-int.com");
									probeDefinition.Attributes["ProdCertThumbprint"] = this.attributeHelper.GetString("ProdCertThumbprint", false, "");
									probeDefinition.Attributes["SDFCertThumbprint"] = this.attributeHelper.GetString("SDFCertThumbprint", false, "");
									probeDefinition.Attributes["GallatinCertThumbprint"] = this.attributeHelper.GetString("GallatinCertThumbprint", false, "");
									probeDefinition.Attributes["TDCertThumbprint"] = this.attributeHelper.GetString("TDCertThumbprint", false, "");
									probeDefinition.Attributes["ProdCompanyManagerUrl"] = this.attributeHelper.GetString("ProdCompanyManagerUrl", true, "https://companymanager.microsoftonline.com/CompanyManager.svc");
									probeDefinition.Attributes["SDFCompanyManagerUrl"] = this.attributeHelper.GetString("SDFCompanyManagerUrl", true, "https://companymanager.ccsctp.com/CompanyManager.svc");
									probeDefinition.Attributes["GallatinCompanyManagerUrl"] = this.attributeHelper.GetString("GallatinCompanyManagerUrl", true, "https://companymanager.partner.microsoftonline.cn/CompanyManager.svc");
									probeDefinition.Attributes["TDCompanyManagerUrl"] = this.attributeHelper.GetString("TDCompanyManagerUrl", true, "https://companymanager.msol-test.com/CompanyManager.svc");
									probeDefinition.Attributes["ProdProvisioningApiUrl"] = this.attributeHelper.GetString("ProdProvisioningApiUrl", true, "https://provisioningapi.microsoftonline.com/provisioningwebservice.svc");
									probeDefinition.Attributes["SDFProvisioningApiUrl"] = this.attributeHelper.GetString("SDFProvisioningApiUrl", true, "https://provisioningapi.ccsctp.com/provisioningwebservice.svc");
									probeDefinition.Attributes["GallatinProvisioningApiUrl"] = this.attributeHelper.GetString("GallatinProvisioningApiUrl", true, "https://provisioningapi.partner.microsoftonline.cn/ProvisioningWebService.svc");
									probeDefinition.Attributes["TDProvisioningApiUrl"] = this.attributeHelper.GetString("TDProvisioningApiUrl", true, "https://provisioningapi.msol-test.com/provisioningwebservice.svc");
									probeDefinition.ServiceName = "Provisioning";
									base.Broker.AddWorkDefinition<ProbeDefinition>(probeDefinition, base.TraceContext);
									this.AddForwardSyncCompanyMonitorAndResponder(probeDefinition, text3);
								}
							}
						}
					}
				}
			}
			catch (Exception ex)
			{
				WTFDiagnostics.TraceInformation<string>(ExTraceGlobals.ProvisioningTracer, base.TraceContext, "ProvisioningDiscovery:: DoWork(): Company Probe exception: {0}", ex.ToString(), null, "CreateForwardSyncCompanyContext", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Provisioning\\ProvisioningDiscovery.cs", 614);
				base.Result.StateAttribute4 = string.Format("Provisioning CompanyProbe ex {0}", ex.Message);
			}
		}

		private void AddForwardSyncCompanyMonitorAndResponder(ProbeDefinition probe, string simplifiedServiceInstanceName)
		{
			int num = 12600;
			int num2 = 3;
			if (ProvisioningDiscovery.isTestTopology)
			{
				num = 120;
				num2 = 1;
			}
			string escalationSubjectUnhealthy = Strings.ForwardSyncStandardCompanyEscalationSubject;
			string escalationMessageUnhealthy = Strings.ForwardSyncStandardCompanyEscalationMessage(num2, num);
			if (simplifiedServiceInstanceName.EndsWith("-2") || simplifiedServiceInstanceName.EndsWith("-02"))
			{
				escalationSubjectUnhealthy = Strings.ForwardSyncLiteCompanyEscalationSubject;
				escalationMessageUnhealthy = Strings.ForwardSyncLiteCompanyEscalationMessage(num2, num);
			}
			string name = "ForwardSyncCompanyMonitor" + '-' + simplifiedServiceInstanceName;
			MonitorDefinition monitorDefinition = OverallXFailuresMonitor.CreateDefinition(name, probe.ConstructWorkItemResultName(), "Provisioning", ExchangeComponent.Provisioning, num, probe.RecurrenceIntervalSeconds, num2, true);
			monitorDefinition.TargetExtension = probe.TargetResource;
			monitorDefinition.ServicePriority = 1;
			monitorDefinition.ScenarioDescription = "Validate Provisioning health is not impacted by ForwardSync issues";
			base.Broker.AddWorkDefinition<MonitorDefinition>(monitorDefinition, base.TraceContext);
			ResponderDefinition responderDefinition = EscalateResponder.CreateDefinition("ForwardSyncCompanyEscalate", "Provisioning", "ForwardSyncCompanyMonitor", monitorDefinition.ConstructWorkItemResultName(), "Provisioning", ServiceHealthStatus.None, ExchangeComponent.Provisioning.EscalationTeam, escalationSubjectUnhealthy, escalationMessageUnhealthy, true, NotificationServiceClass.Urgent, 14400, "Pacific Standard Time/Monday,Tuesday,Wednesday,Thursday,Friday,Saturday,Sunday/00:00/23:59", false);
			ProvisioningSTXEscalateResponder.InitTypeNameAndAssemblyPath(responderDefinition);
			responderDefinition.RecurrenceIntervalSeconds = probe.RecurrenceIntervalSeconds;
			responderDefinition.TimeoutSeconds = probe.RecurrenceIntervalSeconds;
			responderDefinition.TargetExtension = probe.TargetResource;
			responderDefinition.ExtensionAttributes = probe.ExtensionAttributes;
			if (this.IsPDTEnvironment())
			{
				responderDefinition.NotificationServiceClass = NotificationServiceClass.Scheduled;
			}
			base.Broker.AddWorkDefinition<ResponderDefinition>(responderDefinition, base.TraceContext);
		}

		private void CreateForwardFullSyncContext()
		{
			if (!ProvisioningDiscovery.isTestTopology && !this.IsServerInFirstE15DAG())
			{
				return;
			}
			int num = ProvisioningDiscovery.isTestTopology ? 120 : 3600;
			WTFDiagnostics.TraceInformation(ExTraceGlobals.ProvisioningTracer, base.TraceContext, "ProvisioningDiscovery.CreateForwardFullSyncProbes(): Get all ForwardSync fullsync tenants.", null, "CreateForwardFullSyncContext", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Provisioning\\ProvisioningDiscovery.cs", 711);
			ProbeDefinition probeDefinition = ForwardFullSyncProbe.CreateDefinition("ForwardFullSyncProbe");
			probeDefinition.RecurrenceIntervalSeconds = num;
			probeDefinition.ServiceName = "Provisioning";
			probeDefinition.TimeoutSeconds = num / 2;
			base.Broker.AddWorkDefinition<ProbeDefinition>(probeDefinition, base.TraceContext);
			MonitorDefinition monitorDefinition = OverallConsecutiveProbeFailuresMonitor.CreateDefinition("ForwardFullSyncMonitor", probeDefinition.ConstructWorkItemResultName(), "Provisioning", ExchangeComponent.Provisioning, 1, true, 3600);
			monitorDefinition.RecurrenceIntervalSeconds = 0;
			monitorDefinition.ServicePriority = 1;
			monitorDefinition.ScenarioDescription = "Validate Provisioning health is not impacted by tenant is stuck in full sync queue";
			base.Broker.AddWorkDefinition<MonitorDefinition>(monitorDefinition, base.TraceContext);
			string escalationSubjectUnhealthy = string.Format("ForwardFullSync probe failed {0} time(s) in {1} seconds. There may be tenant stuck in full sync queue", 1, 3600);
			string escalationMessageUnhealthy = "ForwardFullSync probe with name {{Probe.ResultName}} failed with exception: {{Probe.Exception}}";
			ResponderDefinition responderDefinition = EscalateResponder.CreateDefinition("ForwardFullSyncEscalateResponder", "Provisioning", monitorDefinition.Name, monitorDefinition.ConstructWorkItemResultName(), "Provisioning", ServiceHealthStatus.None, ExchangeComponent.Provisioning.EscalationTeam, escalationSubjectUnhealthy, escalationMessageUnhealthy, true, NotificationServiceClass.Urgent, num, "Pacific Standard Time/Monday,Tuesday,Wednesday,Thursday,Friday,Saturday,Sunday/00:00/23:59", false);
			responderDefinition.RecurrenceIntervalSeconds = num;
			responderDefinition.TimeoutSeconds = num * 2;
			responderDefinition.NotificationServiceClass = NotificationServiceClass.UrgentInTraining;
			if (this.IsPDTEnvironment())
			{
				responderDefinition.NotificationServiceClass = NotificationServiceClass.Scheduled;
			}
			base.Broker.AddWorkDefinition<ResponderDefinition>(responderDefinition, base.TraceContext);
		}

		private bool IsPDTEnvironment()
		{
			return this.GetLocalServiceEnvironmentName().Equals("Pdt");
		}

		private string GetLocalServiceEnvironmentName()
		{
			NetworkInterface[] allNetworkInterfaces = NetworkInterface.GetAllNetworkInterfaces();
			foreach (NetworkInterface networkInterface in allNetworkInterfaces)
			{
				IPInterfaceProperties ipproperties = networkInterface.GetIPProperties();
				if (!string.IsNullOrWhiteSpace(ipproperties.DnsSuffix))
				{
					string text = ipproperties.DnsSuffix.ToLowerInvariant();
					string result;
					if (text.EndsWith("prod.outlook.com") || text.EndsWith("prod.exchangelabs.com"))
					{
						result = "Prod";
					}
					else if (text.EndsWith("sdf.exchangelabs.com") && !text.EndsWith("nampdt01.sdf.exchangelabs.com"))
					{
						result = "Sdf";
					}
					else if (text.EndsWith("nampdt01.sdf.exchangelabs.com"))
					{
						result = "Pdt";
					}
					else
					{
						if (!text.EndsWith("partner.outlook.cn"))
						{
							goto IL_A9;
						}
						result = "Gallatin";
					}
					return result;
				}
				IL_A9:;
			}
			return "Test";
		}

		public const string ForwardSyncCompanyProbeName = "ForwardSyncCompanyProbe";

		public const string ForwardSyncCompanyMonitorName = "ForwardSyncCompanyMonitor";

		public const string ForwardSyncCompanyEscalateResponderName = "ForwardSyncCompanyEscalate";

		public const string ForwardSyncCookieNotUpToDateProbeName = "ForwardSyncCookieNotUpToDateProbe";

		public const string ForwardSyncCookieNotUpToDateMonitorName = "ForwardSyncCookieNotUpToDateMonitor";

		public const string ForwardSyncCookieNotUpToDateRestartResponderName = "ForwardSyncCookieNotUpToDateRestart";

		public const string ForwardSyncCookieNotUpToDateEscalateResponderName = "ForwardSyncCookieNotUpToDateEscalate";

		public const string ForwardSyncHaltTag = "ForwardSyncHalted";

		public const string ForwardSyncHaltMonitorName = "ForwardSyncHaltMonitor";

		public const string ForwardSyncHaltEscalateResponderName = "ForwardSyncHaltEscalate";

		public const string ForwardSyncProcessRepeatedlyCrashingProbeName = "ForwardSyncProcessRepeatedlyCrashingProbe";

		public const string ForwardSyncProcessRepeatedlyCrashingMonitorName = "ForwardSyncProcessRepeatedlyCrashingMonitor";

		public const string ForwardSyncProcessRepeatedlyCrashingEscalateResponderName = "ForwardSyncProcessRepeatedlyCrashingEscalate";

		public const string ForwardFullSyncMonitorName = "ForwardFullSyncMonitor";

		public const string ForwardFullSyncProbeName = "ForwardFullSyncProbe";

		public const string ForwardFullSyncEscalateResponderName = "ForwardFullSyncEscalateResponder";

		internal const string BposLTargetGroup = "EXCHANGELITE";

		internal const string BposSTargetGroup = "EXCHANGESTANDARD";

		private const int E15MajorVersion = 15;

		private const string ServiceName = "Provisioning";

		private const string ForwardSyncServiceName = "MSExchangeForwardSync";

		private const string CookieStaleTime = "CookieStaleTime";

		private static bool isTestTopology;

		private AttributeHelper attributeHelper;
	}
}
