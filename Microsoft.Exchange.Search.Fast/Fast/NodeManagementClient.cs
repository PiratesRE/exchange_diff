using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Threading;
using System.Xml.Linq;
using Microsoft.Ceres.CoreServices.Admin;
using Microsoft.Ceres.CoreServices.Services.HealthCheck;
using Microsoft.Ceres.CoreServices.Services.SystemManager;
using Microsoft.Ceres.CoreServices.Tools.Management.Client;
using Microsoft.Ceres.HostController.WcfClient;
using Microsoft.Ceres.HostController.WcfTypes;
using Microsoft.Ceres.SearchCore.Admin;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Search;
using Microsoft.Exchange.Search.Core.Abstraction;
using Microsoft.Exchange.Search.Core.Common;
using Microsoft.Exchange.Search.Core.Diagnostics;

namespace Microsoft.Exchange.Search.Fast
{
	internal class NodeManagementClient : FastManagementClient, INodeManager
	{
		internal NodeManagementClient()
		{
			base.DiagnosticsSession.ComponentName = "NodeManagementClient";
			base.DiagnosticsSession.Tracer = ExTraceGlobals.IndexManagementTracer;
			base.ConnectManagementAgents();
		}

		public static NodeManagementClient Instance
		{
			get
			{
				if (Interlocked.CompareExchange<Hookable<NodeManagementClient>>(ref NodeManagementClient.hookableInstance, null, null) == null)
				{
					lock (NodeManagementClient.lockObject)
					{
						if (NodeManagementClient.hookableInstance == null)
						{
							Hookable<NodeManagementClient> hookable = Hookable<NodeManagementClient>.Create(true, new NodeManagementClient());
							Thread.MemoryBarrier();
							NodeManagementClient.hookableInstance = hookable;
						}
					}
				}
				return NodeManagementClient.hookableInstance.Value;
			}
		}

		protected virtual IAdminServiceManagementAgent AdminService
		{
			get
			{
				return this.adminService;
			}
		}

		protected override int ManagementPortOffset
		{
			get
			{
				return 3;
			}
		}

		protected IHostController HostController
		{
			get
			{
				if (this.hostControllerClient == null)
				{
					base.ConnectManagementAgents("localhost");
				}
				return this.hostControllerClient.HostController;
			}
		}

		public override DisposeTracker GetDisposeTracker()
		{
			return DisposeTracker.Get<NodeManagementClient>(this);
		}

		public IEnumerable<HealthCheckInfo> GetSystemInfo()
		{
			return this.PerformFastOperation<IEnumerable<HealthCheckInfo>>(() => this.AdminService.SystemInfo(), "GetSystemInfo");
		}

		public NodeInfo GetNodeInfo(string nodeName)
		{
			return this.PerformFastOperation<NodeInfo>(() => this.AdminService.NodeInfo(nodeName), "GetNodeInfo");
		}

		public XElement GetAllNodesInfoDiagnostics()
		{
			return this.PerformFastOperation<XElement>(delegate()
			{
				XElement xelement = new XElement("Nodes");
				foreach (NodeInfo nodeInfo in this.AdminService.AllNodesInfo())
				{
					XElement xelement2 = new XElement("Node");
					xelement2.Add(new XElement("Name", nodeInfo.Name));
					xelement2.Add(new XElement("Host", nodeInfo.Host));
					xelement2.Add(new XElement("Roles", nodeInfo.Roles));
					xelement2.Add(new XElement("BasePort", nodeInfo.BasePort));
					xelement2.Add(new XElement("State", nodeInfo.State));
					xelement2.Add(new XElement("ExtendedState", nodeInfo.ExtendedState));
					xelement.Add(xelement2);
				}
				return xelement;
			}, "GetAllNodesInfoDiagnostics");
		}

		public bool AreAllNodesHealthy()
		{
			return this.AreAllNodesHealthy(false);
		}

		public bool AreAllNodesHealthy(bool retry)
		{
			return this.PerformFastOperation<bool>(() => this.CheckForAllNodesHealthy(retry), "AreAllNodesHealthy");
		}

		public bool IsNodeHealthy(string nodeName)
		{
			Util.ThrowOnNullArgument(nodeName, "nodeName");
			return this.PerformFastOperation<bool>(delegate()
			{
				NodeInfo nodeInfo = this.AdminService.NodeInfo(nodeName);
				return nodeInfo.State.Equals(1);
			}, "IsNodeHealthy");
		}

		public XElement GetAllNodesHealthReport()
		{
			return this.PerformFastOperation<XElement>(delegate()
			{
				XElement xelement = new XElement("NodesHealthInfo");
				using (SystemClient systemClient = base.ConnectSystem())
				{
					foreach (NodeInfo nodeInfo in this.AdminService.AllNodesInfo())
					{
						using (WcfManagementClient wcfManagementClient = this.ConnectNode(systemClient, nodeInfo.Name))
						{
							IHealthReporterManagementAgent healthReporterAgent = this.GetHealthReporterAgent(wcfManagementClient, nodeInfo.Name);
							XElement xelement2 = new XElement("NodeHealthInfo");
							xelement2.Add(new XElement("NodeName", nodeInfo.Name));
							xelement2.Add(new XElement("HealthStatusOk", healthReporterAgent.HealthStatusOk));
							XElement xelement3 = new XElement("Records");
							IEnumerable<HealthReportItem> healthStatus = healthReporterAgent.HealthStatus;
							if (healthStatus != null)
							{
								foreach (HealthReportItem healthReportItem in healthStatus)
								{
									XElement xelement4 = new XElement("Record");
									xelement4.Add(new XElement("Id", healthReportItem.Id));
									xelement4.Add(new XElement("Level", healthReportItem.Level));
									xelement4.Add(new XElement("Message", healthReportItem.Message));
									xelement3.Add(xelement4);
								}
							}
							xelement2.Add(xelement3);
							xelement.Add(xelement2);
						}
					}
				}
				return xelement;
			}, "GetAllNodesHealthReport");
		}

		public bool IsNodeStopped(string nodeName)
		{
			Util.ThrowOnNullArgument(nodeName, "nodeName");
			return this.PerformFastOperation<bool>(delegate()
			{
				NodeInfo nodeInfo = this.AdminService.NodeInfo(nodeName);
				return nodeInfo.State.Equals(0);
			}, "IsNodeStopped");
		}

		public void StartNode(string nodeName)
		{
			base.DiagnosticsSession.LogDiagnosticsInfo(DiagnosticsLoggingTag.Informational, "Starting Node", new object[0]);
			base.PerformFastOperation(delegate()
			{
				this.HostController.StartNode("Fsis", nodeName);
			}, "StartNode");
		}

		public void KillNode(string nodeName)
		{
			base.DiagnosticsSession.LogDiagnosticsInfo(DiagnosticsLoggingTag.Informational, "Killing Node", new object[0]);
			base.PerformFastOperation(delegate()
			{
				this.HostController.KillNode("Fsis", nodeName);
			}, "KillNode");
		}

		public void KillAndRestartNode(string nodeName)
		{
			base.DiagnosticsSession.LogDiagnosticsInfo(DiagnosticsLoggingTag.Informational, "Killing and Restarting Node", new object[0]);
			base.PerformFastOperation(delegate()
			{
				this.HostController.KillAndRestartNode("Fsis", nodeName);
			}, "KillAndRestartNode");
		}

		public void StopNode(string nodeName)
		{
			base.DiagnosticsSession.LogDiagnosticsInfo(DiagnosticsLoggingTag.Informational, "Stop Node", new object[0]);
			base.PerformFastOperation(delegate()
			{
				this.AdminService.StopNode(nodeName);
			}, "StopNode");
		}

		internal static IDisposable SetInstanceTestHook(NodeManagementClient mockNodeManager)
		{
			if (NodeManagementClient.hookableInstance == null)
			{
				NodeManagementClient instance = NodeManagementClient.Instance;
			}
			return NodeManagementClient.hookableInstance.SetTestHook(mockNodeManager);
		}

		internal bool CheckForAllNodesHealthy(bool retry)
		{
			int num = retry ? 120 : 1;
			for (int i = 0; i < num; i++)
			{
				bool flag = true;
				foreach (HealthCheckInfo healthCheckInfo in this.AdminService.SystemInfo())
				{
					if ((!healthCheckInfo.Name.Equals("IndexNode1") || healthCheckInfo.State != 2) && healthCheckInfo.State != 1)
					{
						flag = false;
						break;
					}
				}
				if (flag)
				{
					return true;
				}
				if (retry)
				{
					Thread.Sleep(1000);
				}
			}
			return false;
		}

		internal List<string> GetNodeNamesBasedOnRole(string role)
		{
			List<string> list = new List<string>();
			foreach (NodeInfo nodeInfo in this.AdminService.AllNodesInfo())
			{
				if (nodeInfo.Roles.Contains(role))
				{
					list.Add(nodeInfo.Name);
				}
			}
			return list;
		}

		protected override void InternalConnectManagementAgents(WcfManagementClient client)
		{
			this.CreateWcfHostControllerClient();
			this.adminService = client.GetManagementAgent<IAdminServiceManagementAgent>("AdminService");
		}

		private void CreateWcfHostControllerClient()
		{
			if (this.hostControllerClient != null)
			{
				this.hostControllerClient.Dispose();
				this.hostControllerClient = null;
			}
			Uri uri = new Uri(string.Format("net.tcp://{0}:{1}/ceres/hostcontroller/nettcp", "localhost", FastManagementClient.FsisInstallBasePort + 1));
			DuplexChannelFactory<IHostController> duplexChannelFactory = WcfClientConfiguration.CreateDefaultChannelFactory(uri, true);
			this.hostControllerClient = new WcfHostControllerClient(duplexChannelFactory, null, new EndpointAddress(uri, EndpointIdentity.CreateUpnIdentity("*"), new AddressHeader[0]), TimeSpan.FromMinutes(1.0));
		}

		private WcfManagementClient ConnectNode(SystemClient sc, string nodeName)
		{
			ISystemManagerManagementAgent managementAgent = base.GetManagementAgent<ISystemManagerManagementAgent>("SystemManager");
			Uri managementUri = managementAgent.GetManagementUri(nodeName, false);
			return sc.GetManagementClient(managementUri);
		}

		private IHealthReporterManagementAgent GetHealthReporterAgent(WcfManagementClient connection, string nodeName)
		{
			return connection.GetManagementAgent<IHealthReporterManagementAgent>(nodeName + ".HealthReport");
		}

		private const int RetryCount = 120;

		private const int RetryInterval = 1000;

		private const int FsisHostControllerRelativePortNumber = 1;

		private const int ProxyCacheIntervalInMinutes = 1;

		private const string ServerName = "localhost";

		private static object lockObject = new object();

		private static Hookable<NodeManagementClient> hookableInstance;

		private volatile IAdminServiceManagementAgent adminService;

		private volatile WcfHostControllerClient hostControllerClient;
	}
}
