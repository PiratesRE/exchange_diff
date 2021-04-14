using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Security;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Threading;
using Microsoft.Exchange.Data.Directory.ExchangeTopology;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Data.Directory;
using Microsoft.Exchange.Net;

namespace Microsoft.Exchange.Data.Directory.TopologyDiscovery
{
	[GeneratedCode("System.ServiceModel", "4.0.0.0")]
	[DebuggerStepThrough]
	internal class TopologyServiceClient : ClientBase<ITopologyClient>, ITopologyClient, ITopologyService
	{
		protected TopologyServiceClient()
		{
		}

		protected TopologyServiceClient(string endpointConfigurationName) : base(endpointConfigurationName)
		{
		}

		protected TopologyServiceClient(string endpointConfigurationName, string remoteAddress) : base(endpointConfigurationName, remoteAddress)
		{
		}

		protected TopologyServiceClient(string endpointConfigurationName, EndpointAddress remoteAddress) : base(endpointConfigurationName, remoteAddress)
		{
		}

		protected TopologyServiceClient(Binding binding, EndpointAddress remoteAddress) : base(binding, remoteAddress)
		{
		}

		public byte[][] GetExchangeTopology(DateTime currentTopologyTimeStamp, ExchangeTopologyScope topologyScope, bool forceRefresh)
		{
			return base.Channel.GetExchangeTopology(currentTopologyTimeStamp, topologyScope, forceRefresh);
		}

		public ServiceVersion GetServiceVersion()
		{
			return base.Channel.GetServiceVersion();
		}

		public List<TopologyVersion> GetAllTopologyVersions()
		{
			return base.Channel.GetAllTopologyVersions();
		}

		public List<TopologyVersion> GetTopologyVersions(List<string> partitionFqdns)
		{
			return base.Channel.GetTopologyVersions(partitionFqdns);
		}

		public List<ServerInfo> GetServersForRole(string partitionFqdn, List<string> currentlyUsedServers, ADServerRole role, int serversRequested, bool forestWideAffinityRequested = false)
		{
			return base.Channel.GetServersForRole(partitionFqdn, currentlyUsedServers, role, serversRequested, forestWideAffinityRequested);
		}

		public IAsyncResult BeginGetServersForRole(string partitionFqdn, List<string> currentlyUsedServers, ADServerRole role, int serversRequested, bool forestWideAffinityRequested, AsyncCallback callback, object asyncState)
		{
			return base.Channel.BeginGetServersForRole(partitionFqdn, currentlyUsedServers, role, serversRequested, forestWideAffinityRequested, callback, asyncState);
		}

		public List<ServerInfo> EndGetServersForRole(IAsyncResult result)
		{
			return base.Channel.EndGetServersForRole(result);
		}

		public IAsyncResult BeginGetServerFromDomainDN(string domainDN, AsyncCallback callback, object asyncState)
		{
			return base.Channel.BeginGetServerFromDomainDN(domainDN, callback, asyncState);
		}

		public ServerInfo EndGetServerFromDomainDN(IAsyncResult result)
		{
			return base.Channel.EndGetServerFromDomainDN(result);
		}

		public ServerInfo GetServerFromDomainDN(string domainDN)
		{
			return base.Channel.GetServerFromDomainDN(domainDN);
		}

		public void SetConfigDC(string partitionFqdn, string serverName)
		{
			base.Channel.SetConfigDC(partitionFqdn, serverName);
		}

		public IAsyncResult BeginSetConfigDC(string partitionFqdn, string serverName, AsyncCallback callback, object asyncState)
		{
			return base.Channel.BeginSetConfigDC(partitionFqdn, serverName, callback, asyncState);
		}

		public void EndSetConfigDC(IAsyncResult result)
		{
			base.Channel.EndSetConfigDC(result);
		}

		public void ReportServerDown(string partitionFqdn, string serverName, ADServerRole role)
		{
			base.Channel.ReportServerDown(partitionFqdn, serverName, role);
		}

		public void Close(TimeSpan timeOut)
		{
			base.ChannelFactory.Close(timeOut);
		}

		internal void AddRef()
		{
			Interlocked.Increment(ref this.refCount);
		}

		internal void Release()
		{
			if (Interlocked.Decrement(ref this.refCount) == 0L)
			{
				bool flag = false;
				try
				{
					if (base.State != CommunicationState.Faulted)
					{
						base.Close();
						flag = true;
					}
				}
				catch (TimeoutException)
				{
				}
				catch (CommunicationException)
				{
				}
				finally
				{
					if (!flag)
					{
						base.Abort();
					}
					((IDisposable)this).Dispose();
				}
			}
		}

		internal static TopologyServiceClient CreateClient(string machineName)
		{
			ArgumentValidator.ThrowIfNullOrEmpty("machineName", machineName);
			NetTcpBinding binding = TopologyServiceClient.CreateAndConfigureTopologyServiceBinding(machineName);
			EndpointAddress remoteAddress = TopologyServiceClient.CreateAndConfigureTopologyServiceEndpoint(machineName);
			return new TopologyServiceClient(binding, remoteAddress);
		}

		internal static NetTcpBinding CreateAndConfigureTopologyServiceBinding(string machineName)
		{
			ArgumentValidator.ThrowIfNullOrEmpty("machineName", machineName);
			return new NetTcpBinding
			{
				Name = "Topology Client - " + machineName,
				TransactionFlow = false,
				TransferMode = TransferMode.Buffered,
				TransactionProtocol = TransactionProtocol.OleTransactions,
				HostNameComparisonMode = HostNameComparisonMode.StrongWildcard,
				ListenBacklog = 50,
				MaxBufferPoolSize = 524288L,
				MaxReceivedMessageSize = 134217728L,
				MaxConnections = 100,
				Security = 
				{
					Mode = SecurityMode.Transport,
					Transport = 
					{
						ClientCredentialType = TcpClientCredentialType.Windows,
						ProtectionLevel = ProtectionLevel.EncryptAndSign
					}
				},
				ReliableSession = 
				{
					Ordered = true,
					InactivityTimeout = ServiceProxyPool<ITopologyClient>.DefaultInactivityTimeout,
					Enabled = false
				},
				SendTimeout = TopologyServiceClient.defaultSendTimeout,
				ReaderQuotas = 
				{
					MaxStringContentLength = 262144
				}
			};
		}

		internal static EndpointAddress CreateAndConfigureTopologyServiceEndpoint(string machineName)
		{
			ArgumentValidator.ThrowIfNullOrEmpty("machineName", machineName);
			string text = string.Format("net.tcp://{0}:890/Microsoft.Exchange.Directory.TopologyService", machineName);
			EndpointAddress result;
			try
			{
				EndpointAddress endpointAddress = new EndpointAddress(text);
				result = endpointAddress;
			}
			catch (UriFormatException ex)
			{
				ExTraceGlobals.TopologyProviderTracer.TraceError<string, string>(0L, "TopologyServiceClient.CreateAndConfigureTopologyServiceEndpoint() - Invalid Server Name {0}.  Exception: {1}", machineName, ex.Message);
				throw new InvalidEndpointAddressException(ex.GetType().Name, text);
			}
			return result;
		}

		private const string WcfEndpointFormat = "net.tcp://{0}:890/Microsoft.Exchange.Directory.TopologyService";

		private static readonly TimeSpan defaultSendTimeout = TimeSpan.FromMinutes(2.0);

		private long refCount;
	}
}
