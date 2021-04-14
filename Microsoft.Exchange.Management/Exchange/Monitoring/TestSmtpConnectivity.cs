using System;
using System.Collections.Generic;
using System.Management;
using System.Management.Automation;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Management.SystemConfigurationTasks;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Monitoring
{
	[Cmdlet("Test", "SmtpConnectivity", SupportsShouldProcess = true)]
	public sealed class TestSmtpConnectivity : DataAccessTask<Server>
	{
		[Parameter]
		public new Fqdn DomainController
		{
			get
			{
				return base.DomainController;
			}
			set
			{
				base.DomainController = value;
			}
		}

		[Parameter(Mandatory = false, ValueFromPipeline = true, ValueFromPipelineByPropertyName = true, Position = 0)]
		[ValidateNotNullOrEmpty]
		public ServerIdParameter Identity
		{
			get
			{
				return (ServerIdParameter)base.Fields["Identity"];
			}
			set
			{
				base.Fields["Identity"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool MonitoringContext
		{
			get
			{
				return (bool)(base.Fields["MonitoringContext"] ?? false);
			}
			set
			{
				base.Fields["MonitoringContext"] = value;
			}
		}

		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				return Strings.ConfirmationMessageTestSmtpConnectivity;
			}
		}

		internal static IList<ReceiveConnector> GetReceiveConnectors(IConfigurationSession session, Server server)
		{
			List<ReceiveConnector> list = new List<ReceiveConnector>();
			ADPagedReader<ReceiveConnector> adpagedReader = session.FindPaged<ReceiveConnector>(server.Id, QueryScope.SubTree, null, null, ADGenericPagedReader<ReceiveConnector>.DefaultPageSize);
			foreach (ReceiveConnector receiveConnector in adpagedReader)
			{
				if (receiveConnector.Enabled)
				{
					list.Add(receiveConnector);
				}
			}
			return list;
		}

		internal static bool ConnectorsHaveBindings(IList<ReceiveConnector> connectors)
		{
			foreach (ReceiveConnector receiveConnector in connectors)
			{
				if (receiveConnector.Bindings.Count > 0)
				{
					return true;
				}
			}
			return false;
		}

		internal static SmtpConnectivityStatus GetStatus(Server server, ReceiveConnector connector, IPBinding binding, IPEndPoint endPoint)
		{
			SmtpConnectivityStatusCode statusCode;
			string details;
			TestSmtpConnectivity.TestEndPoint(endPoint, out statusCode, out details);
			return new SmtpConnectivityStatus(server, connector, binding, endPoint)
			{
				StatusCode = statusCode,
				Details = details
			};
		}

		internal static IList<IPEndPoint> GetEndPoints(Server server, IPBinding binding)
		{
			if (!binding.Address.Equals(IPAddress.Any) && !binding.Address.Equals(IPAddress.IPv6Any))
			{
				return new IPEndPoint[]
				{
					binding
				};
			}
			ManagementPath path = new ManagementPath(string.Format("\\\\{0}\\root\\cimv2", server.Fqdn));
			System.Management.ManagementScope scope = new System.Management.ManagementScope(path);
			IList<NetworkConnectionInfo> connectionInfo = NetworkConnectionInfo.GetConnectionInfo(scope);
			List<IPEndPoint> list = new List<IPEndPoint>();
			foreach (NetworkConnectionInfo networkConnectionInfo in connectionInfo)
			{
				foreach (IPAddress address in networkConnectionInfo.IPAddresses)
				{
					list.Add(new IPEndPoint(address, binding.Port));
				}
			}
			return list;
		}

		internal static void TestEndPoint(IPEndPoint endPoint, out SmtpConnectivityStatusCode statusCode, out string details)
		{
			statusCode = SmtpConnectivityStatusCode.Success;
			details = string.Empty;
			using (Socket socket = new Socket(endPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp))
			{
				try
				{
					socket.Connect(endPoint);
					if (!socket.Connected)
					{
						statusCode = SmtpConnectivityStatusCode.Error;
						details = Strings.UnableToConnect;
					}
					else
					{
						byte[] array = new byte[2000];
						int count = socket.Receive(array, array.Length, SocketFlags.None);
						string @string = Encoding.ASCII.GetString(array, 0, count);
						TestSmtpConnectivity.ParseBanner(@string, out statusCode, out details);
					}
				}
				catch (SocketException ex)
				{
					statusCode = SmtpConnectivityStatusCode.Error;
					details = ex.Message;
				}
			}
		}

		internal static MonitoringEvent CreateMonitoringEvent(string serverName, IList<SmtpConnectivityStatus> results)
		{
			StringBuilder stringBuilder = new StringBuilder();
			StringBuilder stringBuilder2 = new StringBuilder();
			StringBuilder stringBuilder3 = new StringBuilder();
			foreach (SmtpConnectivityStatus smtpConnectivityStatus in results)
			{
				string value = Strings.SmtpConnectivityEndPointResult(smtpConnectivityStatus.Identity.ToString(), smtpConnectivityStatus.Details);
				switch (smtpConnectivityStatus.StatusCode)
				{
				case SmtpConnectivityStatusCode.Success:
					stringBuilder2.AppendLine(value);
					break;
				case SmtpConnectivityStatusCode.Error:
					stringBuilder.AppendLine(value);
					break;
				case SmtpConnectivityStatusCode.UnableToComplete:
					stringBuilder3.AppendLine(value);
					break;
				}
			}
			string failures = (stringBuilder.Length == 0) ? string.Empty : Strings.SmtpConnectivityFailures(stringBuilder.ToString());
			string untested = (stringBuilder3.Length == 0) ? Strings.SmtpConnectivityAllTested : Strings.SmtpConnectivityNotTested(stringBuilder3.ToString());
			string successes = (stringBuilder2.Length == 0) ? Strings.SmtpConnectivityNoneSucceeded : Strings.SmtpConnectivitySuccesses(stringBuilder2.ToString());
			SmtpConnectivityStatusCode eventIdentifier;
			EventTypeEnumeration eventType;
			string eventMessage;
			if (stringBuilder.Length > 0)
			{
				eventIdentifier = SmtpConnectivityStatusCode.Error;
				eventType = EventTypeEnumeration.Error;
				eventMessage = Strings.SmtpConnectivityFailureEvent(serverName, failures, untested, successes);
			}
			else if (stringBuilder3.Length > 0)
			{
				eventIdentifier = SmtpConnectivityStatusCode.UnableToComplete;
				eventType = EventTypeEnumeration.Warning;
				eventMessage = Strings.SmtpConnectivityIncompleteEvent(serverName, untested, successes);
			}
			else if (stringBuilder2.Length > 0)
			{
				eventIdentifier = SmtpConnectivityStatusCode.Success;
				eventType = EventTypeEnumeration.Information;
				eventMessage = Strings.SmtpConnectivitySuccessEvent(serverName, successes);
			}
			else
			{
				eventIdentifier = SmtpConnectivityStatusCode.Error;
				eventType = EventTypeEnumeration.Error;
				eventMessage = Strings.SmtpConnectivityServerNotConfigured(serverName);
			}
			return new MonitoringEvent("MSExchange Monitoring SmtpConnectivity", (int)eventIdentifier, eventType, eventMessage);
		}

		internal static void ParseBanner(string banner, out SmtpConnectivityStatusCode statusCode, out string details)
		{
			banner = banner.Trim();
			if (banner.Length < 3)
			{
				statusCode = SmtpConnectivityStatusCode.Error;
				details = Strings.InvalidSmtpBanner(banner);
				return;
			}
			switch (banner[0])
			{
			case '2':
				statusCode = SmtpConnectivityStatusCode.Success;
				details = banner;
				return;
			case '4':
			case '5':
				statusCode = SmtpConnectivityStatusCode.Error;
				details = banner;
				return;
			}
			statusCode = SmtpConnectivityStatusCode.Error;
			details = Strings.InvalidSmtpBanner(banner);
		}

		protected override bool IsKnownException(Exception exception)
		{
			return base.IsKnownException(exception) || MonitoringHelper.IsKnownExceptionForMonitoring(exception);
		}

		protected override IConfigDataProvider CreateSession()
		{
			return DirectorySessionFactory.Default.CreateTopologyConfigurationSession(this.DomainController, true, ConsistencyMode.IgnoreInvalid, ADSessionSettings.FromRootOrgScopeSet(), 482, "CreateSession", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\Monitoring\\Tasks\\TestSmtpConnectivity.cs");
		}

		protected override void InternalValidate()
		{
			base.InternalValidate();
			TaskLogger.LogEnter();
			try
			{
				if (this.Identity == null)
				{
					ITopologyConfigurationSession topologyConfigurationSession = (ITopologyConfigurationSession)base.DataSession;
					Server server = topologyConfigurationSession.FindLocalServer();
					this.Identity = new ServerIdParameter(new Fqdn(server.Fqdn));
				}
				this.server = (Server)base.GetDataObject<Server>(this.Identity, base.DataSession, this.RootId, new LocalizedString?(Strings.ErrorServerNotFound(this.Identity.ToString())), new LocalizedString?(Strings.ErrorServerNotUnique(this.Identity.ToString())));
				if (!this.server.IsHubTransportServer && !this.server.IsEdgeServer)
				{
					this.WriteErrorAndAddMonitoringEvent(new NotTransportServerException(this.Identity.ToString()), ErrorCategory.InvalidArgument, SmtpConnectivityStatusCode.UnableToComplete);
				}
				if (!this.ServerHasBindings())
				{
				}
			}
			finally
			{
				if (base.HasErrors && this.MonitoringContext)
				{
					this.WriteMonitoringObject();
				}
				TaskLogger.LogExit();
			}
		}

		protected override void InternalProcessRecord()
		{
			base.InternalProcessRecord();
			TaskLogger.LogEnter();
			try
			{
				foreach (ReceiveConnector receiveConnector in this.receiveConnectors)
				{
					foreach (IPBinding ipbinding in receiveConnector.Bindings)
					{
						Exception ex = null;
						try
						{
							IList<IPEndPoint> endPoints = TestSmtpConnectivity.GetEndPoints(this.server, ipbinding);
							foreach (IPEndPoint endPoint in endPoints)
							{
								SmtpConnectivityStatus status = TestSmtpConnectivity.GetStatus(this.server, receiveConnector, ipbinding, endPoint);
								base.WriteObject(status);
								if (this.MonitoringContext)
								{
									this.AddMonitoringData(status);
								}
							}
						}
						catch (ManagementException ex2)
						{
							ex = ex2;
						}
						catch (COMException ex3)
						{
							ex = ex3;
						}
						catch (UnauthorizedAccessException ex4)
						{
							ex = ex4;
						}
						if (ex != null)
						{
							SmtpConnectivityStatus smtpConnectivityStatus = new SmtpConnectivityStatus(this.server, receiveConnector, ipbinding, ipbinding);
							smtpConnectivityStatus.StatusCode = SmtpConnectivityStatusCode.UnableToComplete;
							smtpConnectivityStatus.Details = ex.Message;
							base.WriteObject(smtpConnectivityStatus);
							if (this.MonitoringContext)
							{
								this.AddMonitoringData(smtpConnectivityStatus);
							}
						}
					}
				}
			}
			finally
			{
				if (this.MonitoringContext)
				{
					this.WriteMonitoringObject();
				}
				TaskLogger.LogExit();
			}
		}

		protected override void InternalStateReset()
		{
			base.InternalStateReset();
			this.monitoringData = new MonitoringData();
		}

		private bool ServerHasBindings()
		{
			this.receiveConnectors = TestSmtpConnectivity.GetReceiveConnectors((IConfigurationSession)base.DataSession, this.server);
			if (this.receiveConnectors.Count == 0)
			{
				this.WriteErrorAndAddMonitoringEvent(new NoReceiveConnectorsException(this.server.Fqdn), ErrorCategory.InvalidData, SmtpConnectivityStatusCode.Error);
				return false;
			}
			if (!TestSmtpConnectivity.ConnectorsHaveBindings(this.receiveConnectors))
			{
				this.WriteErrorAndAddMonitoringEvent(new NoBindingsException(this.server.Fqdn), ErrorCategory.InvalidData, SmtpConnectivityStatusCode.Error);
				return false;
			}
			return true;
		}

		private void WriteErrorAndAddMonitoringEvent(Exception exception, ErrorCategory errorCategory, SmtpConnectivityStatusCode statusCode)
		{
			this.monitoringData.Events.Add(new MonitoringEvent("MSExchange Monitoring SmtpConnectivity", (int)statusCode, EventTypeEnumeration.Error, exception.Message));
			base.WriteError(exception, errorCategory, null);
		}

		private void AddMonitoringData(SmtpConnectivityStatus status)
		{
			if (this.results == null)
			{
				this.results = new List<SmtpConnectivityStatus>();
			}
			this.results.Add(status);
		}

		private void WriteMonitoringObject()
		{
			if (this.results != null)
			{
				MonitoringEvent item = TestSmtpConnectivity.CreateMonitoringEvent(this.server.Name, this.results);
				this.monitoringData.Events.Add(item);
			}
			base.WriteObject(this.monitoringData);
		}

		private const string CmdletNoun = "SmtpConnectivity";

		private const string MonitoringEventSource = "MSExchange Monitoring SmtpConnectivity";

		private MonitoringData monitoringData;

		private Server server;

		private IList<ReceiveConnector> receiveConnectors;

		private List<SmtpConnectivityStatus> results;
	}
}
