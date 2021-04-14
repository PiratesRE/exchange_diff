using System;
using System.Net.Security;
using System.ServiceModel;
using System.Threading.Tasks;
using Microsoft.Exchange.Cluster.Shared;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Cluster.DagManagement;

namespace Microsoft.Exchange.Cluster.Replay.Monitoring.Client
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class MonitoringServiceClient : DisposeTrackableBase
	{
		private static Trace Tracer
		{
			get
			{
				return ExTraceGlobals.MonitoringWcfClientTracer;
			}
		}

		public static MonitoringServiceClient Open(string serverName)
		{
			return MonitoringServiceClient.Open(serverName, MonitoringServiceClient.OpenTimeout, MonitoringServiceClient.CloseTimeout, MonitoringServiceClient.SendTimeout, MonitoringServiceClient.ReceiveTimeout);
		}

		public static MonitoringServiceClient Open(string serverName, TimeSpan openTimeout, TimeSpan closeTimeout, TimeSpan sendTimeout, TimeSpan receiveTimeout)
		{
			MonitoringServiceClient monitoringServiceClient = new MonitoringServiceClient();
			EndpointAddress endpointAddress = new EndpointAddress(string.Format("net.tcp://{0}:{1}/Microsoft.Exchange.HA.Monitoring", serverName, RegistryParameters.MonitoringWebServicePort));
			monitoringServiceClient.ServerName = serverName;
			MonitoringServiceClient.Tracer.TraceDebug<EndpointAddress>(0L, "Opening MonitoringServiceClient connection to endpoint: {0}", endpointAddress);
			monitoringServiceClient.m_wcfClient = new InternalMonitoringServiceClient(new NetTcpBinding
			{
				Name = "NetTcpBinding_MonitoringService",
				OpenTimeout = openTimeout,
				CloseTimeout = closeTimeout,
				SendTimeout = sendTimeout,
				ReceiveTimeout = receiveTimeout,
				MaxBufferPoolSize = 16777216L,
				MaxBufferSize = 16777216,
				MaxReceivedMessageSize = 16777216L,
				MaxConnections = 10,
				Security = 
				{
					Mode = SecurityMode.Transport,
					Message = 
					{
						ClientCredentialType = MessageCredentialType.Windows
					},
					Transport = 
					{
						ClientCredentialType = TcpClientCredentialType.Windows,
						ProtectionLevel = ProtectionLevel.EncryptAndSign
					}
				}
			}, endpointAddress);
			return monitoringServiceClient;
		}

		public string ServerName { get; private set; }

		protected override void InternalDispose(bool disposing)
		{
			if (disposing)
			{
				InternalMonitoringServiceClient internalMonitoringServiceClient = null;
				lock (this)
				{
					if (this.m_wcfClient != null)
					{
						internalMonitoringServiceClient = this.m_wcfClient;
						this.m_wcfClient = null;
					}
				}
				if (internalMonitoringServiceClient != null)
				{
					internalMonitoringServiceClient.Abort();
				}
			}
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<MonitoringServiceClient>(this);
		}

		public static Exception HandleException(Action operation)
		{
			Exception result = null;
			try
			{
				operation();
			}
			catch (FaultException<ExceptionDetail> faultException)
			{
				result = faultException;
			}
			catch (FaultException ex)
			{
				result = ex;
			}
			catch (CommunicationException ex2)
			{
				result = ex2;
			}
			catch (TimeoutException ex3)
			{
				result = ex3;
			}
			catch (AggregateException ex4)
			{
				result = ex4.Flatten().InnerExceptions[0];
			}
			return result;
		}

		public ServiceVersion GetVersion()
		{
			return this.m_wcfClient.GetVersion();
		}

		public async Task<ServiceVersion> GetVersionAsync()
		{
			MonitoringServiceClient.Tracer.TraceDebug<string>((long)this.GetHashCode(), "Calling GetVersionAsync() on server '{0}' ...", this.ServerName);
			ServiceVersion version = await this.m_wcfClient.GetVersionAsync();
			MonitoringServiceClient.Tracer.TraceDebug<long, string>((long)this.GetHashCode(), "GetVersionAsync() from server '{1}' returned version of: {0}", version.Version, this.ServerName);
			return version;
		}

		public async Task<bool> PublishDagHealthInfoAsync(HealthInfoPersisted healthInfo)
		{
			MonitoringServiceClient.Tracer.TraceDebug<string>((long)this.GetHashCode(), "Calling PublishDagHealthInfoAsync() on server '{0}' ...", this.ServerName);
			await this.m_wcfClient.PublishDagHealthInfoAsync(healthInfo);
			MonitoringServiceClient.Tracer.TraceDebug<string>((long)this.GetHashCode(), "PublishDagHealthInfoAsync() from server '{0}' returned.", this.ServerName);
			return true;
		}

		public Task<Tuple<string, bool>> WPublishDagHealthInfoAsync(HealthInfoPersisted healthInfo)
		{
			return this.RunOperationWithServerNameAsync<bool>((MonitoringServiceClient client) => client.PublishDagHealthInfoAsync(healthInfo));
		}

		public async Task<DateTime> GetDagHealthInfoUpdateTimeUtcAsync()
		{
			MonitoringServiceClient.Tracer.TraceDebug<string>((long)this.GetHashCode(), "Calling GetDagHealthInfoUpdateTimeUtcAsync() on server '{0}' ...", this.ServerName);
			DateTime lastUpdateTime = await this.m_wcfClient.GetDagHealthInfoUpdateTimeUtcAsync();
			MonitoringServiceClient.Tracer.TraceDebug<DateTime, string>((long)this.GetHashCode(), "GetDagHealthInfoUpdateTimeUtcAsync() from server '{1}' returned '{0}'.", lastUpdateTime, this.ServerName);
			return lastUpdateTime;
		}

		public Task<Tuple<string, DateTime>> WGetDagHealthInfoUpdateTimeUtcAsync()
		{
			return this.RunOperationWithServerNameAsync<DateTime>((MonitoringServiceClient client) => client.GetDagHealthInfoUpdateTimeUtcAsync());
		}

		public async Task<HealthInfoPersisted> GetDagHealthInfoAsync()
		{
			MonitoringServiceClient.Tracer.TraceDebug<string>((long)this.GetHashCode(), "Calling GetDagHealthInfoAsync() on server '{0}' ...", this.ServerName);
			HealthInfoPersisted hip = await this.m_wcfClient.GetDagHealthInfoAsync();
			MonitoringServiceClient.Tracer.TraceDebug<string, string>((long)this.GetHashCode(), "GetDagHealthInfoAsync() from server '{1}' returned a table of update time '{0}'.", hip.LastUpdateTimeUtcStr, this.ServerName);
			return hip;
		}

		public Task<Tuple<string, HealthInfoPersisted>> WGetDagHealthInfoAsync()
		{
			return this.RunOperationWithServerNameAsync<HealthInfoPersisted>((MonitoringServiceClient client) => client.GetDagHealthInfoAsync());
		}

		private async Task<Tuple<string, TReturnType>> RunOperationWithServerNameAsync<TReturnType>(Func<MonitoringServiceClient, Task<TReturnType>> remoteOperation)
		{
			TReturnType returnObj = await remoteOperation(this);
			return new Tuple<string, TReturnType>(this.ServerName, returnObj);
		}

		private const string WcfEndpointFormat = "net.tcp://{0}:{1}/Microsoft.Exchange.HA.Monitoring";

		public static readonly TimeSpan OpenTimeout = TimeSpan.FromSeconds((double)RegistryParameters.MonitoringWebServiceClientOpenTimeoutInSecs);

		public static readonly TimeSpan CloseTimeout = TimeSpan.FromSeconds((double)RegistryParameters.MonitoringWebServiceClientCloseTimeoutInSecs);

		public static readonly TimeSpan SendTimeout = TimeSpan.FromSeconds((double)RegistryParameters.MonitoringWebServiceClientSendTimeoutInSecs);

		public static readonly TimeSpan ReceiveTimeout = TimeSpan.FromSeconds((double)RegistryParameters.MonitoringWebServiceClientReceiveTimeoutInSecs);

		private InternalMonitoringServiceClient m_wcfClient;
	}
}
