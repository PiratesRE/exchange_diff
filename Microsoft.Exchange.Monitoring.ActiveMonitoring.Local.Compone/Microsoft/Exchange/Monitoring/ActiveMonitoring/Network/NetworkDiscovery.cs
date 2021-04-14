using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading;
using Microsoft.Exchange.Data.HA.DirectoryServices;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.ActiveMonitoring;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.ActiveMonitoring.Responders;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.Common;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.HighAvailability;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.Local;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.Local.Components.Network.Probes;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.Local.Components.Network.Responders;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.Responders;
using Microsoft.Office.Datacenter.ActiveMonitoring;
using Microsoft.Office.Datacenter.Monitoring.ActiveMonitoring.Recovery;
using Microsoft.Office.Datacenter.WorkerTaskFramework;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.Network
{
	public sealed class NetworkDiscovery : MaintenanceWorkItem
	{
		protected override void DoWork(CancellationToken cancellationToken)
		{
			this.CreateDnsHostRecordTestWorkDefinitions();
			this.CreateNetworkAdapterTestWorkDefinitions();
			WindowsServerRoleEndpoint windowsServerRoleEndpoint = LocalEndpointManager.Instance.WindowsServerRoleEndpoint;
			if (windowsServerRoleEndpoint == null)
			{
				this.TraceInfo("NetworkDiscovery::DoWork(): WindowsServerRoleEndpoint is not valid; some monitors may not be created.");
			}
			else
			{
				if (windowsServerRoleEndpoint.IsDhcpServerRoleInstalled)
				{
					this.CreateDhcpRoleWorkDefinitions();
				}
				if (windowsServerRoleEndpoint.IsDnsServerRoleInstalled)
				{
					this.CreateDnsServiceWorkDefinitions();
					this.CreateDnsServerForwardersWorkDefinitions();
				}
				if (windowsServerRoleEndpoint.IsNatServerRoleInstalled)
				{
					this.CreateNatRoleWorkDefinitions();
				}
			}
			ExchangeServerRoleEndpoint exchangeServerRoleEndpoint = LocalEndpointManager.Instance.ExchangeServerRoleEndpoint;
			if (exchangeServerRoleEndpoint == null)
			{
				this.TraceInfo("NetworkDiscovery::DoWork(): ExchangeServerRoleEndpoint is not valid; some monitors may not be created.");
			}
			else if (exchangeServerRoleEndpoint.IsMailboxRoleInstalled)
			{
				this.CreateNetworkAdapterRssTestWorkDefinitions();
				this.CreateIntraDagPingMatrixDefinitions();
			}
			if (LocalEndpointManager.IsDataCenter)
			{
				this.CreateHttpConnectivityTestWorkDefinitions();
				this.CreateFireWallTestWorkDefinitions();
				this.CreateRouteTableTestWorkDefinitions();
			}
		}

		private void CreateDhcpRoleWorkDefinitions()
		{
			string perfCounterName = "DHCP Server\\Requests/Sec";
			MonitorDefinition definition = OverallConsecutiveSampleValueAboveThresholdMonitor.CreateDefinition("DHCPServerRequestsPerSecExceeded100", PerformanceCounterNotificationItem.GenerateResultName(perfCounterName), "Network", this.componentTeam, 100.0, 3, true);
			base.Broker.AddWorkDefinition<MonitorDefinition>(definition, base.TraceContext);
			this.CreateServiceWorkDefinitions("DHCPServer");
		}

		private void CreateDnsServerForwardersWorkDefinitions()
		{
			base.Broker.AddWorkDefinition<ProbeDefinition>(new ProbeDefinition
			{
				AssemblyPath = Assembly.GetExecutingAssembly().Location,
				ServiceName = "Network",
				TypeName = typeof(DnsServerForwardersProbe).FullName,
				Name = "DnsServerForwardersProbe",
				RecurrenceIntervalSeconds = 600,
				TimeoutSeconds = 300
			}, base.TraceContext);
		}

		private void CreateDnsServiceWorkDefinitions()
		{
			int num = 300;
			int timeoutSeconds = 29;
			int num2 = 2;
			int monitoringInterval = num * num2 + 30;
			int transitionTimeoutSeconds = 0;
			int transitionTimeoutSeconds2 = num * (num2 + 1) + 60;
			int recurrenceIntervalSeconds = num + 15;
			ProbeDefinition probeDefinition = new ProbeDefinition();
			probeDefinition.AssemblyPath = Assembly.GetExecutingAssembly().Location;
			probeDefinition.ServiceName = "Network";
			probeDefinition.TypeName = typeof(DnsServiceProbe).FullName;
			probeDefinition.Name = Strings.DnsServiceProbeName;
			probeDefinition.RecurrenceIntervalSeconds = num;
			probeDefinition.TimeoutSeconds = timeoutSeconds;
			base.Broker.AddWorkDefinition<ProbeDefinition>(probeDefinition, base.TraceContext);
			MonitorDefinition monitorDefinition = OverallConsecutiveProbeFailuresMonitor.CreateDefinition(Strings.DnsServiceMonitorName, probeDefinition.Name, "Network", this.componentTeam, num2, true, monitoringInterval);
			monitorDefinition.MonitorStateTransitions = new MonitorStateTransition[]
			{
				new MonitorStateTransition(ServiceHealthStatus.Degraded, transitionTimeoutSeconds),
				new MonitorStateTransition(ServiceHealthStatus.Unrecoverable, transitionTimeoutSeconds2)
			};
			base.Broker.AddWorkDefinition<MonitorDefinition>(monitorDefinition, base.TraceContext);
			string responderName = Strings.DnsServiceRestartResponderName;
			string name = monitorDefinition.Name;
			string serviceName = "Network";
			ResponderDefinition responderDefinition = RestartServiceResponder.CreateDefinition(responderName, name, "DNS", ServiceHealthStatus.Degraded, 15, 120, 0, false, DumpMode.None, null, 15.0, 0, serviceName, null, true, true, null, false);
			responderDefinition.RecurrenceIntervalSeconds = recurrenceIntervalSeconds;
			base.Broker.AddWorkDefinition<ResponderDefinition>(responderDefinition, base.TraceContext);
		}

		private void CreateIntraDagPingMatrixDefinitions()
		{
			if (!LocalEndpointManager.IsDataCenter && !LocalEndpointManager.IsDataCenterDedicated)
			{
				this.TraceInfo("IDP Probe is not created for on-prem instances of Exchange");
				return;
			}
			int recurrenceIntervalSeconds = 60;
			int timeoutSeconds = 12;
			CachedAdReader instance = CachedAdReader.Instance;
			IADDatabaseAvailabilityGroup localDAG = instance.LocalDAG;
			if (localDAG != null && localDAG.Name != null)
			{
				string name = localDAG.Name;
			}
			this.TraceInfo("Begin creation of Intra-DAG ping matrix work item definitions.");
			IADServer localServer = instance.LocalServer;
			if (localServer == null || string.IsNullOrEmpty(localServer.Fqdn))
			{
				this.TraceInfo("Unable to identify local server FQDN.");
				return;
			}
			string localFqdn = localServer.Fqdn;
			Regex regex = new Regex("^ ([^.]{3}) [^.]* ([^.]{3}) ", RegexOptions.IgnorePatternWhitespace);
			Match match = regex.Match(localFqdn);
			if (!match.Success)
			{
				this.TraceInfo("Unable to parse the local server's FQDN because it is in an unexpected format: '{0}'.", new object[]
				{
					localFqdn
				});
				return;
			}
			string targetPartition = match.Groups[1].Value.ToUpperInvariant();
			string str = match.Groups[2].Value.ToUpperInvariant();
			IADServer[] allServersInLocalDag = instance.AllServersInLocalDag;
			if (allServersInLocalDag == null)
			{
				this.TraceInfo("Unable to discover the servers in the local DAG.");
				return;
			}
			string location = Assembly.GetExecutingAssembly().Location;
			string name2 = "IntraDagPingProbe";
			string fullName = typeof(IntraDagPingProbe).FullName;
			int num = 0;
			IEnumerable<string> enumerable = from server in allServersInLocalDag
			select server.Fqdn into fqdn
			where !string.IsNullOrEmpty(fqdn) && fqdn != localFqdn
			select fqdn;
			foreach (string text in enumerable)
			{
				match = regex.Match(text);
				if (!match.Success)
				{
					this.TraceError("Unable to parse a target server's FQDN because it is in an unexpected format: '{0}'.", new object[]
					{
						text
					});
				}
				else
				{
					string targetGroup = match.Groups[1].Value.ToUpperInvariant();
					string str2 = match.Groups[2].Value.ToUpperInvariant();
					string text2 = text.ToLowerInvariant();
					if (text2[text2.Length - 1] != '.')
					{
						text2 += '.';
					}
					base.Broker.AddWorkDefinition<ProbeDefinition>(new ProbeDefinition
					{
						AssemblyPath = location,
						ServiceName = "Network",
						TypeName = fullName,
						Name = name2,
						TargetPartition = targetPartition,
						TargetGroup = targetGroup,
						TargetResource = str + "-" + str2,
						Endpoint = text2,
						RecurrenceIntervalSeconds = recurrenceIntervalSeconds,
						TimeoutSeconds = timeoutSeconds,
						ExecutionLocation = localFqdn
					}, base.TraceContext);
					this.TraceInfo("Created probe definition #{0:00}, for target '{1}'.", new object[]
					{
						++num,
						text2
					});
				}
			}
			this.TraceInfo("Created a total of {0} Intra-DAG ping matrix work item definitions (probes).", new object[]
			{
				num
			});
		}

		private void CreateNatRoleWorkDefinitions()
		{
			this.CreateServiceWorkDefinitions("RemoteAccess");
		}

		private void CreateServiceWorkDefinitions(string windowsServiceName)
		{
			ProbeDefinition probeDefinition = new ProbeDefinition();
			probeDefinition.AssemblyPath = typeof(GenericServiceProbe).Assembly.Location;
			probeDefinition.ServiceName = "Network";
			probeDefinition.TypeName = typeof(GenericServiceProbe).FullName;
			probeDefinition.Name = windowsServiceName + "ServiceProbe";
			probeDefinition.RecurrenceIntervalSeconds = 300;
			probeDefinition.TimeoutSeconds = 10;
			probeDefinition.TargetResource = windowsServiceName;
			base.Broker.AddWorkDefinition<ProbeDefinition>(probeDefinition, base.TraceContext);
			MonitorDefinition monitorDefinition = OverallXFailuresMonitor.CreateDefinition(windowsServiceName + "ServiceMonitor", probeDefinition.Name, "Network", this.componentTeam, 630, 30, 2, true);
			monitorDefinition.MonitorStateTransitions = new MonitorStateTransition[]
			{
				new MonitorStateTransition(ServiceHealthStatus.Degraded, 0),
				new MonitorStateTransition(ServiceHealthStatus.Unrecoverable, 360)
			};
			base.Broker.AddWorkDefinition<MonitorDefinition>(monitorDefinition, base.TraceContext);
			ResponderDefinition responderDefinition = RestartServiceResponder.CreateDefinition(windowsServiceName + "ServiceRestartResponder", monitorDefinition.Name, windowsServiceName, ServiceHealthStatus.Degraded, 15, 120, 0, false, DumpMode.None, null, 15.0, 0, "Exchange", null, true, true, null, false);
			responderDefinition.RecurrenceIntervalSeconds = 30;
			base.Broker.AddWorkDefinition<ResponderDefinition>(responderDefinition, base.TraceContext);
		}

		private void CreateHttpConnectivityTestWorkDefinitions()
		{
			if (FfoLocalEndpointManager.IsForefrontForOfficeDatacenter && !FfoLocalEndpointManager.IsCentralAdminRoleInstalled)
			{
				return;
			}
			ProbeDefinition probeDefinition = new ProbeDefinition();
			probeDefinition.AssemblyPath = Assembly.GetExecutingAssembly().Location;
			probeDefinition.ServiceName = "Network";
			probeDefinition.TypeName = typeof(HttpConnectivityProbe).FullName;
			probeDefinition.Name = "HttpConnectivityProbe";
			if (ExEnvironment.IsTest)
			{
				probeDefinition.RecurrenceIntervalSeconds = 30;
				probeDefinition.TimeoutSeconds = 20;
			}
			else
			{
				probeDefinition.RecurrenceIntervalSeconds = 300;
				probeDefinition.TimeoutSeconds = 120;
			}
			base.Broker.AddWorkDefinition<ProbeDefinition>(probeDefinition, base.TraceContext);
			WTFDiagnostics.TraceInformation<int>(ExTraceGlobals.NetworkTracer, base.TraceContext, "NetworkDiscovery::CreateHttpConnectivityTestWorkDefinitions() ProbeDefinition has been generated. Recurrence Interval Seconds: {0}", probeDefinition.RecurrenceIntervalSeconds, null, "CreateHttpConnectivityTestWorkDefinitions", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Network\\Discovery\\NetworkDiscovery.cs", 481);
		}

		private void CreateDnsHostRecordTestWorkDefinitions()
		{
			ProbeDefinition probeDefinition = new ProbeDefinition();
			probeDefinition.AssemblyPath = Assembly.GetExecutingAssembly().Location;
			probeDefinition.ServiceName = "Network";
			probeDefinition.TypeName = typeof(DnsHostRecordProbe).FullName;
			probeDefinition.Name = Strings.DnsHostRecordProbeName;
			probeDefinition.RecurrenceIntervalSeconds = 1200;
			probeDefinition.TimeoutSeconds = 120;
			base.Broker.AddWorkDefinition<ProbeDefinition>(probeDefinition, base.TraceContext);
			MonitorDefinition monitorDefinition = OverallXFailuresMonitor.CreateDefinition(Strings.DnsHostRecordMonitorName, probeDefinition.Name, "Network", this.componentTeam, 1200, 1200, 1, true);
			monitorDefinition.ServicePriority = 0;
			monitorDefinition.MonitorStateTransitions = new MonitorStateTransition[]
			{
				new MonitorStateTransition(ServiceHealthStatus.Unhealthy, 0)
			};
			base.Broker.AddWorkDefinition<MonitorDefinition>(monitorDefinition, base.TraceContext);
			ResponderDefinition responderDefinition = new ResponderDefinition();
			responderDefinition.AssemblyPath = Assembly.GetExecutingAssembly().Location;
			responderDefinition.TypeName = typeof(RegisterDnsHostRecordResponder).FullName;
			responderDefinition.Name = Strings.RegisterDnsHostRecordResponderName;
			responderDefinition.ServiceName = "Network";
			responderDefinition.AlertTypeId = monitorDefinition.Name;
			responderDefinition.AlertMask = monitorDefinition.ConstructWorkItemResultName();
			responderDefinition.RecurrenceIntervalSeconds = 30;
			responderDefinition.TimeoutSeconds = 20;
			responderDefinition.MaxRetryAttempts = 1;
			responderDefinition.TargetHealthState = ServiceHealthStatus.Unhealthy;
			responderDefinition.WaitIntervalSeconds = 30;
			responderDefinition.Enabled = true;
			base.Broker.AddWorkDefinition<ResponderDefinition>(responderDefinition, base.TraceContext);
		}

		private void CreateFireWallTestWorkDefinitions()
		{
			int num = 300;
			int num2 = 1200;
			int num3 = 3;
			int timeoutSeconds = 40;
			if (ExEnvironment.IsTest)
			{
				num = 60;
				num2 = 120;
				num3 = 1;
				timeoutSeconds = 30;
			}
			ProbeDefinition probeDefinition = new ProbeDefinition();
			probeDefinition.AssemblyPath = Assembly.GetExecutingAssembly().Location;
			probeDefinition.ServiceName = "Network";
			probeDefinition.TypeName = typeof(FireWallProbe).FullName;
			probeDefinition.Name = "FireWallProbe";
			probeDefinition.RecurrenceIntervalSeconds = num;
			probeDefinition.TimeoutSeconds = timeoutSeconds;
			base.Broker.AddWorkDefinition<ProbeDefinition>(probeDefinition, base.TraceContext);
			MonitorDefinition monitorDefinition = OverallXFailuresMonitor.CreateDefinition("FireWallMonitor", probeDefinition.ConstructWorkItemResultName(), "Network", ExchangeComponent.Network, num2, num, num3, true);
			monitorDefinition.MonitorStateTransitions = new MonitorStateTransition[]
			{
				new MonitorStateTransition(ServiceHealthStatus.Degraded, 0),
				new MonitorStateTransition(ServiceHealthStatus.Unhealthy, (int)TimeSpan.FromMinutes(30.0).TotalSeconds),
				new MonitorStateTransition(ServiceHealthStatus.Unrecoverable, (int)TimeSpan.FromMinutes(60.0).TotalSeconds)
			};
			base.Broker.AddWorkDefinition<MonitorDefinition>(monitorDefinition, base.TraceContext);
			string responderName = "FireWallRestartResponder";
			string name = monitorDefinition.Name;
			string serviceName = "Network";
			ResponderDefinition responderDefinition = RestartServiceResponder.CreateDefinition(responderName, name, "NlaSvc", ServiceHealthStatus.Degraded, 15, 120, 0, false, DumpMode.None, null, 15.0, 0, serviceName, null, true, true, null, false);
			responderDefinition.RecurrenceIntervalSeconds = num;
			base.Broker.AddWorkDefinition<ResponderDefinition>(responderDefinition, base.TraceContext);
			ResponderDefinition responderDefinition2 = ForceRebootServerResponder.CreateDefinition("FireWallRebootResponder", monitorDefinition.Name, ServiceHealthStatus.Unhealthy, null, -1, "", "Firewall state was found to be incorrect.", "Datacenter, Stamp", "RecoveryData", "ArbitrationOnly", "Exchange", true, "Dag", false);
			responderDefinition2.RecurrenceIntervalSeconds = num;
			base.Broker.AddWorkDefinition<ResponderDefinition>(responderDefinition2, base.TraceContext);
			ResponderDefinition responderDefinition3 = EscalateResponder.CreateDefinition("FireWallEscalateResponder", "Network", monitorDefinition.Name, monitorDefinition.ConstructWorkItemResultName(), Environment.MachineName, ServiceHealthStatus.Unrecoverable, ExchangeComponent.Network.EscalationTeam, Strings.FireWallEscalationSubject(Environment.MachineName), Strings.FireWallEscalationMessage(num3, num2), true, NotificationServiceClass.Urgent, 14400, "Pacific Standard Time/Monday,Tuesday,Wednesday,Thursday,Friday,Saturday,Sunday/00:00/23:59", false);
			responderDefinition3.RecurrenceIntervalSeconds = num;
			responderDefinition3.NotificationServiceClass = NotificationServiceClass.Scheduled;
			base.Broker.AddWorkDefinition<ResponderDefinition>(responderDefinition3, base.TraceContext);
		}

		private void CreateNetworkAdapterTestWorkDefinitions()
		{
			WTFDiagnostics.TraceInformation(ExTraceGlobals.NetworkTracer, base.TraceContext, "NetworkDiscovery::CreateNetworkAdapterTestWorkDefinitions() ProbeDefinition will be generated.", null, "CreateNetworkAdapterTestWorkDefinitions", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Network\\Discovery\\NetworkDiscovery.cs", 651);
			int recurrenceIntervalSeconds = 180;
			int failureCount = 1;
			int monitoringInterval = 300;
			int timeoutSeconds = 40;
			if (ExEnvironment.IsTest)
			{
				recurrenceIntervalSeconds = 60;
				failureCount = 1;
				monitoringInterval = 100;
				timeoutSeconds = 30;
			}
			ProbeDefinition probeDefinition = new ProbeDefinition
			{
				AssemblyPath = Assembly.GetExecutingAssembly().Location,
				ServiceName = "Network",
				TypeName = typeof(NetworkAdapterProbe).FullName,
				Name = "NetworkAdapterProbe",
				RecurrenceIntervalSeconds = recurrenceIntervalSeconds,
				TimeoutSeconds = timeoutSeconds
			};
			base.Broker.AddWorkDefinition<ProbeDefinition>(probeDefinition, base.TraceContext);
			WTFDiagnostics.TraceInformation(ExTraceGlobals.NetworkTracer, base.TraceContext, "NetworkDiscovery::CreateNetworkAdapterTestWorkDefinitions() MonitorDefinition will be generated.", null, "CreateNetworkAdapterTestWorkDefinitions", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Network\\Discovery\\NetworkDiscovery.cs", 682);
			MonitorDefinition monitorDefinition = OverallConsecutiveProbeFailuresMonitor.CreateDefinition("NetworkAdapterMonitor", probeDefinition.Name, "Network", this.componentTeam, failureCount, true, monitoringInterval);
			monitorDefinition.ServicePriority = 0;
			monitorDefinition.MonitorStateTransitions = new MonitorStateTransition[]
			{
				new MonitorStateTransition(ServiceHealthStatus.Unhealthy, 0)
			};
			base.Broker.AddWorkDefinition<MonitorDefinition>(monitorDefinition, base.TraceContext);
			WTFDiagnostics.TraceInformation(ExTraceGlobals.NetworkTracer, base.TraceContext, "NetworkDiscovery::CreateNetworkAdapterTestWorkDefinitions() RecoveryResponderDefinition will be generated.", null, "CreateNetworkAdapterTestWorkDefinitions", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Network\\Discovery\\NetworkDiscovery.cs", 708);
			ResponderDefinition definition = NetworkAdapterRecoveryResponder.CreateDefinition(Strings.NetworkAdapterRecoveryResponderName, "Network", monitorDefinition.Name, monitorDefinition.ConstructWorkItemResultName(), ServiceHealthStatus.Unhealthy, true);
			base.Broker.AddWorkDefinition<ResponderDefinition>(definition, base.TraceContext);
		}

		private void CreateRouteTableTestWorkDefinitions()
		{
			WTFDiagnostics.TraceInformation(ExTraceGlobals.NetworkTracer, base.TraceContext, "NetworkDiscovery::CreateRouteTableTestWorkDefinitions() ProbeDefinition will be generated.", null, "CreateRouteTableTestWorkDefinitions", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Network\\Discovery\\NetworkDiscovery.cs", 731);
			int recurrenceIntervalSeconds = 300;
			int failureCount = 3;
			int monitoringInterval = 1200;
			int timeoutSeconds = 40;
			if (ExEnvironment.IsTest)
			{
				recurrenceIntervalSeconds = 60;
				failureCount = 1;
				monitoringInterval = 100;
				timeoutSeconds = 30;
			}
			ProbeDefinition probeDefinition = new ProbeDefinition
			{
				AssemblyPath = Assembly.GetExecutingAssembly().Location,
				ServiceName = "Network",
				TypeName = typeof(RouteTableProbe).FullName,
				Name = "RouteTableProbe",
				RecurrenceIntervalSeconds = recurrenceIntervalSeconds,
				TimeoutSeconds = timeoutSeconds
			};
			base.Broker.AddWorkDefinition<ProbeDefinition>(probeDefinition, base.TraceContext);
			WTFDiagnostics.TraceInformation(ExTraceGlobals.NetworkTracer, base.TraceContext, "NetworkDiscovery::CreateRouteTableTestWorkDefinitions() MonitorDefinition will be generated.", null, "CreateRouteTableTestWorkDefinitions", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Network\\Discovery\\NetworkDiscovery.cs", 762);
			MonitorDefinition monitorDefinition = OverallConsecutiveProbeFailuresMonitor.CreateDefinition("RouteTableMonitor", probeDefinition.Name, "Network", this.componentTeam, failureCount, true, monitoringInterval);
			monitorDefinition.ServicePriority = 0;
			monitorDefinition.MonitorStateTransitions = new MonitorStateTransition[]
			{
				new MonitorStateTransition(ServiceHealthStatus.Unhealthy, 0)
			};
			base.Broker.AddWorkDefinition<MonitorDefinition>(monitorDefinition, base.TraceContext);
			WTFDiagnostics.TraceInformation(ExTraceGlobals.NetworkTracer, base.TraceContext, "NetworkDiscovery::CreateRouteTableTestWorkDefinitions() RecoveryResponderDefinition will be generated.", null, "CreateRouteTableTestWorkDefinitions", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Network\\Discovery\\NetworkDiscovery.cs", 788);
			ResponderDefinition definition = RouteTableRecoveryResponder.CreateDefinition(Strings.RouteTableRecoveryResponderName, "Network", monitorDefinition.Name, monitorDefinition.ConstructWorkItemResultName(), ServiceHealthStatus.Unhealthy, true);
			base.Broker.AddWorkDefinition<ResponderDefinition>(definition, base.TraceContext);
		}

		private void CreateNetworkAdapterRssTestWorkDefinitions()
		{
			WTFDiagnostics.TraceInformation(ExTraceGlobals.NetworkTracer, base.TraceContext, "NetworkDiscovery::CreateNetworkAdapterRssTestWorkDefinitions() ProbeDefinition will be generated.", null, "CreateNetworkAdapterRssTestWorkDefinitions", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Network\\Discovery\\NetworkDiscovery.cs", 811);
			int recurrenceIntervalSeconds = 300;
			int failureCount = 3;
			int monitoringInterval = 1200;
			int timeoutSeconds = 40;
			ProbeDefinition probeDefinition = new ProbeDefinition
			{
				AssemblyPath = Assembly.GetExecutingAssembly().Location,
				ServiceName = "Network",
				TypeName = typeof(NetworkAdapterRssProbe).FullName,
				Name = "NetworkAdapterRssProbe",
				RecurrenceIntervalSeconds = recurrenceIntervalSeconds,
				TimeoutSeconds = timeoutSeconds
			};
			base.Broker.AddWorkDefinition<ProbeDefinition>(probeDefinition, base.TraceContext);
			WTFDiagnostics.TraceInformation(ExTraceGlobals.NetworkTracer, base.TraceContext, "NetworkDiscovery::CreateNetworkAdapterRssTestWorkDefinitions() MonitorDefinition will be generated.", null, "CreateNetworkAdapterRssTestWorkDefinitions", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Network\\Discovery\\NetworkDiscovery.cs", 834);
			MonitorDefinition monitorDefinition = OverallConsecutiveProbeFailuresMonitor.CreateDefinition("NetworkAdapterRssMonitor", probeDefinition.Name, "Network", this.componentTeam, failureCount, true, monitoringInterval);
			monitorDefinition.MonitorStateTransitions = new MonitorStateTransition[]
			{
				new MonitorStateTransition(ServiceHealthStatus.Unhealthy, 0)
			};
			base.Broker.AddWorkDefinition<MonitorDefinition>(monitorDefinition, base.TraceContext);
			WTFDiagnostics.TraceInformation(ExTraceGlobals.NetworkTracer, base.TraceContext, "NetworkDiscovery::CreateNetworkAdapterRssTestWorkDefinitions() EscalateResponderDefinition will be generated.", null, "CreateNetworkAdapterRssTestWorkDefinitions", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Network\\Discovery\\NetworkDiscovery.cs", 858);
			ResponderDefinition definition = EscalateResponder.CreateDefinition("NetworkAdapterRssEscalate", "Network", monitorDefinition.Name, monitorDefinition.ConstructWorkItemResultName(), Environment.MachineName, ServiceHealthStatus.Unhealthy, ExchangeComponent.Network.EscalationTeam, Strings.NetworkAdapterRssEscalationSubject(Environment.MachineName), Strings.NetworkAdapterRssEscalationMessage, true, NotificationServiceClass.UrgentInTraining, 14400, "Pacific Standard Time/Monday,Tuesday,Wednesday,Thursday,Friday,Saturday,Sunday/00:00/23:59", false);
			base.Broker.AddWorkDefinition<ResponderDefinition>(definition, base.TraceContext);
		}

		private void TraceError(string message)
		{
			WTFDiagnostics.TraceError(ExTraceGlobals.NetworkTracer, base.TraceContext, message, null, "TraceError", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Network\\Discovery\\NetworkDiscovery.cs", 885);
		}

		private void TraceError(string message, params object[] formatArgs)
		{
			WTFDiagnostics.TraceError(ExTraceGlobals.NetworkTracer, base.TraceContext, message, formatArgs, null, "TraceError", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Network\\Discovery\\NetworkDiscovery.cs", 895);
		}

		private void TraceInfo(string message)
		{
			WTFDiagnostics.TraceInformation(ExTraceGlobals.NetworkTracer, base.TraceContext, message, null, "TraceInfo", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Network\\Discovery\\NetworkDiscovery.cs", 908);
		}

		private void TraceInfo(string message, params object[] formatArgs)
		{
			WTFDiagnostics.TraceInformation(ExTraceGlobals.NetworkTracer, base.TraceContext, message, formatArgs, null, "TraceInfo", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Network\\Discovery\\NetworkDiscovery.cs", 918);
		}

		private const string ServiceName = "Network";

		public const string HttpConnectivityProbeName = "HttpConnectivityProbe";

		public const string NetworkAdapterProbeName = "NetworkAdapterProbe";

		public const string NetworkAdapterMonitorName = "NetworkAdapterMonitor";

		public const string RouteTableProbeName = "RouteTableProbe";

		public const string RouteTableMonitorName = "RouteTableMonitor";

		public const string NetworkAdapterRssProbeName = "NetworkAdapterRssProbe";

		public const string NetworkAdapterRssMonitorName = "NetworkAdapterRssMonitor";

		public const string NetworkAdapterRssResponderName = "NetworkAdapterRssEscalate";

		public const string FireWallProbeName = "FireWallProbe";

		public const string FireWallMonitorName = "FireWallMonitor";

		public const string FireWallRestartResponderName = "FireWallRestartResponder";

		public const string FireWallRebootResponderName = "FireWallRebootResponder";

		public const string FireWallEscalateResponderName = "FireWallEscalateResponder";

		private Component componentTeam = ExchangeComponent.Network;
	}
}
