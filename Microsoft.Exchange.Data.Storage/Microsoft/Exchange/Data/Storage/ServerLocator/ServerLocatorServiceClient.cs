using System;
using System.Net.Security;
using System.ServiceModel;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Cluster.Replay;
using Microsoft.Win32;
using www.outlook.com.highavailability.ServerLocator.v1;

namespace Microsoft.Exchange.Data.Storage.ServerLocator
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class ServerLocatorServiceClient : IDisposeTrackable, IDisposable
	{
		private static int HighAvailabilityWebServicePort
		{
			get
			{
				if (ServerLocatorServiceClient.highAvailabilityWebServicePort == 0)
				{
					lock (ServerLocatorServiceClient.syncRoot)
					{
						if (ServerLocatorServiceClient.highAvailabilityWebServicePort == 0)
						{
							ServerLocatorServiceClient.Tracer.TraceDebug(0L, "Using registry to get HighAvailabilityWebServicePort");
							using (RegistryKey registryKey = Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\ExchangeServer\\v15\\Replay\\Parameters"))
							{
								if (registryKey != null)
								{
									ServerLocatorServiceClient.highAvailabilityWebServicePort = (int)registryKey.GetValue("HighAvailabilityWebServicePort", 64337);
									ServerLocatorServiceClient.Tracer.TraceDebug<string, int>(0L, "Registry key {0} found. Value: {1}.", "SOFTWARE\\Microsoft\\ExchangeServer\\v15\\Replay\\Parameters", ServerLocatorServiceClient.highAvailabilityWebServicePort);
								}
								else
								{
									ServerLocatorServiceClient.highAvailabilityWebServicePort = 64337;
									ServerLocatorServiceClient.Tracer.TraceDebug<string, int>(0L, "Registry key {0} not found. Using default value: {1}.", "SOFTWARE\\Microsoft\\ExchangeServer\\v15\\Replay\\Parameters", 64337);
								}
							}
						}
					}
				}
				return ServerLocatorServiceClient.highAvailabilityWebServicePort;
			}
		}

		private static Trace Tracer
		{
			get
			{
				return ExTraceGlobals.ServerLocatorServiceClientTracer;
			}
		}

		public bool IsUsable
		{
			get
			{
				bool result = false;
				if (this.m_client != null && this.m_client.State == CommunicationState.Opened)
				{
					result = true;
				}
				return result;
			}
		}

		public static ServerLocatorServiceClient Create(string serverName)
		{
			return new ServerLocatorServiceClient(serverName);
		}

		public static ServerLocatorServiceClient Create(string serverName, TimeSpan closeTimeout, TimeSpan openTimeout, TimeSpan receiveTimeout, TimeSpan sendTimeout)
		{
			return new ServerLocatorServiceClient(serverName, closeTimeout, openTimeout, receiveTimeout, sendTimeout);
		}

		private ServerLocatorServiceClient(string serverName, TimeSpan closeTimeout, TimeSpan openTimeout, TimeSpan receiveTimeout, TimeSpan sendTimeout)
		{
			ServerLocatorServiceClient.Tracer.TraceDebug<string>(0L, "Constructing ServerLocatorClient for Server Locator Service on {0}.", serverName);
			this.m_disposeTracker = this.GetDisposeTracker();
			NetTcpBinding netTcpBinding = new NetTcpBinding();
			netTcpBinding.Name = "NetTcpBinding_ServerLocator";
			netTcpBinding.CloseTimeout = closeTimeout;
			netTcpBinding.OpenTimeout = openTimeout;
			netTcpBinding.ReceiveTimeout = receiveTimeout;
			netTcpBinding.SendTimeout = sendTimeout;
			netTcpBinding.TransactionFlow = false;
			netTcpBinding.TransferMode = TransferMode.Buffered;
			netTcpBinding.TransactionProtocol = TransactionProtocol.OleTransactions;
			netTcpBinding.HostNameComparisonMode = HostNameComparisonMode.StrongWildcard;
			netTcpBinding.ListenBacklog = 10;
			netTcpBinding.MaxBufferPoolSize = 16777216L;
			netTcpBinding.MaxBufferSize = 16777216;
			netTcpBinding.MaxReceivedMessageSize = 16777216L;
			netTcpBinding.MaxConnections = 10;
			netTcpBinding.ReaderQuotas.MaxDepth = 32;
			netTcpBinding.ReaderQuotas.MaxStringContentLength = 8192;
			netTcpBinding.ReaderQuotas.MaxArrayLength = 16384;
			netTcpBinding.ReaderQuotas.MaxBytesPerRead = 4096;
			netTcpBinding.ReaderQuotas.MaxNameTableCharCount = 16384;
			netTcpBinding.ReliableSession.Ordered = true;
			netTcpBinding.ReliableSession.InactivityTimeout = TimeSpan.FromMinutes(10.0);
			netTcpBinding.ReliableSession.Enabled = false;
			netTcpBinding.Security.Mode = SecurityMode.Transport;
			netTcpBinding.Security.Transport.ClientCredentialType = TcpClientCredentialType.Windows;
			netTcpBinding.Security.Transport.ProtectionLevel = ProtectionLevel.EncryptAndSign;
			netTcpBinding.Security.Message.ClientCredentialType = MessageCredentialType.Windows;
			EndpointAddress endpointAddress = new EndpointAddress(string.Format("net.tcp://{0}:{1}/Exchange.HighAvailability/ServerLocator", serverName, ServerLocatorServiceClient.HighAvailabilityWebServicePort));
			try
			{
				this.m_client = new ServerLocatorClient(netTcpBinding, endpointAddress);
			}
			catch (Exception ex)
			{
				throw this.HandleAndTranslateKnownException(ex);
			}
			ServerLocatorServiceClient.Tracer.TraceDebug<string>(0L, "ServerLocatorClient for Server Locator Service on  {0} succesfully created.", endpointAddress.Uri.AbsoluteUri);
		}

		private ServerLocatorServiceClient(string serverName) : this(serverName, ServerLocatorServiceClient.defaultCloseTimeout, ServerLocatorServiceClient.defaultOpenTimeout, ServerLocatorServiceClient.defaultReceiveTimeout, ServerLocatorServiceClient.defaultSendTimeout)
		{
		}

		~ServerLocatorServiceClient()
		{
			this.Dispose(false);
		}

		public DisposeTracker GetDisposeTracker()
		{
			return DisposeTracker.Get<ServerLocatorServiceClient>(this);
		}

		public void SuppressDisposeTracker()
		{
			if (this.m_disposeTracker != null)
			{
				this.m_disposeTracker.Suppress();
			}
		}

		public void Dispose()
		{
			if (!this.m_fDisposed)
			{
				this.Dispose(true);
				GC.SuppressFinalize(this);
			}
		}

		public void Dispose(bool disposing)
		{
			lock (this)
			{
				if (!this.m_fDisposed)
				{
					if (disposing)
					{
						if (this.m_client != null)
						{
							if (this.m_client.State != CommunicationState.Opened)
							{
								this.m_client.Abort();
							}
							else
							{
								try
								{
									this.m_client.Close();
								}
								catch (CommunicationException)
								{
									this.m_client.Abort();
								}
								catch (TimeoutException)
								{
									this.m_client.Abort();
								}
							}
							this.m_client = null;
						}
						if (this.m_disposeTracker != null)
						{
							this.m_disposeTracker.Dispose();
						}
						this.m_disposeTracker = null;
					}
					this.m_fDisposed = true;
				}
			}
		}

		public IAsyncResult BeginGetServerForDatabase(Guid databaseGuid, AsyncCallback callback, object asyncState)
		{
			IAsyncResult result;
			try
			{
				result = this.m_client.BeginGetServerForDatabase(new DatabaseServerInformation
				{
					DatabaseGuid = databaseGuid,
					RequestSentUtc = DateTime.UtcNow
				}, callback, asyncState);
			}
			catch (Exception ex)
			{
				throw this.HandleAndTranslateKnownException(ex);
			}
			return result;
		}

		public DatabaseServerInformation EndGetServerForDatabase(IAsyncResult result)
		{
			DatabaseServerInformation result2;
			try
			{
				result2 = this.m_client.EndGetServerForDatabase(result);
			}
			catch (Exception ex)
			{
				throw this.HandleAndTranslateKnownException(ex);
			}
			return result2;
		}

		public DatabaseServerInformation GetServerForDatabase(Guid databaseGuid)
		{
			DatabaseServerInformation serverForDatabase;
			try
			{
				serverForDatabase = this.m_client.GetServerForDatabase(new DatabaseServerInformation
				{
					DatabaseGuid = databaseGuid,
					RequestSentUtc = DateTime.UtcNow
				});
			}
			catch (Exception ex)
			{
				throw this.HandleAndTranslateKnownException(ex);
			}
			return serverForDatabase;
		}

		public IAsyncResult BeginGetActiveCopiesForDatabaseAvailabilityGroup(AsyncCallback callback, object asyncState)
		{
			IAsyncResult result;
			try
			{
				result = this.m_client.BeginGetActiveCopiesForDatabaseAvailabilityGroup(callback, asyncState);
			}
			catch (Exception ex)
			{
				throw this.HandleAndTranslateKnownException(ex);
			}
			return result;
		}

		public DatabaseServerInformation[] EndGetActiveCopiesForDatabaseAvailabilityGroup(IAsyncResult result)
		{
			DatabaseServerInformation[] result2;
			try
			{
				result2 = this.m_client.EndGetActiveCopiesForDatabaseAvailabilityGroup(result);
			}
			catch (Exception ex)
			{
				throw this.HandleAndTranslateKnownException(ex);
			}
			return result2;
		}

		public DatabaseServerInformation[] GetActiveCopiesForDatabaseAvailabilityGroup()
		{
			DatabaseServerInformation[] activeCopiesForDatabaseAvailabilityGroup;
			try
			{
				activeCopiesForDatabaseAvailabilityGroup = this.m_client.GetActiveCopiesForDatabaseAvailabilityGroup();
			}
			catch (Exception ex)
			{
				throw this.HandleAndTranslateKnownException(ex);
			}
			return activeCopiesForDatabaseAvailabilityGroup;
		}

		public IAsyncResult BeginGetActiveCopiesForDatabaseAvailabilityGroupExtended(GetActiveCopiesForDatabaseAvailabilityGroupParameters parameters, AsyncCallback callback, object asyncState)
		{
			IAsyncResult result;
			try
			{
				result = this.m_client.BeginGetActiveCopiesForDatabaseAvailabilityGroupExtended(parameters, callback, asyncState);
			}
			catch (Exception ex)
			{
				throw this.HandleAndTranslateKnownException(ex);
			}
			return result;
		}

		public DatabaseServerInformation[] EndGetActiveCopiesForDatabaseAvailabilityGroupExtended(IAsyncResult result)
		{
			DatabaseServerInformation[] result2;
			try
			{
				result2 = this.m_client.EndGetActiveCopiesForDatabaseAvailabilityGroupExtended(result);
			}
			catch (Exception ex)
			{
				throw this.HandleAndTranslateKnownException(ex);
			}
			return result2;
		}

		public DatabaseServerInformation[] GetActiveCopiesForDatabaseAvailabilityGroupExtended(GetActiveCopiesForDatabaseAvailabilityGroupParameters parameters)
		{
			DatabaseServerInformation[] activeCopiesForDatabaseAvailabilityGroupExtended;
			try
			{
				activeCopiesForDatabaseAvailabilityGroupExtended = this.m_client.GetActiveCopiesForDatabaseAvailabilityGroupExtended(parameters);
			}
			catch (Exception ex)
			{
				throw this.HandleAndTranslateKnownException(ex);
			}
			return activeCopiesForDatabaseAvailabilityGroupExtended;
		}

		public ServiceVersion GetVersion()
		{
			ServiceVersion version;
			try
			{
				version = this.m_client.GetVersion();
			}
			catch (Exception ex)
			{
				throw this.HandleAndTranslateKnownException(ex);
			}
			return version;
		}

		private Exception HandleAndTranslateKnownException(Exception ex)
		{
			if (ex is FaultException<DatabaseServerInformationFault>)
			{
				if (((FaultException<DatabaseServerInformationFault>)ex).Detail.ErrorCode == DatabaseServerInformationType.PermanentError)
				{
					return new ServerLocatorClientException(ServerStrings.ServerLocatorServicePermanentFault, ex);
				}
				return new ServerLocatorClientTransientException(ServerStrings.ServerLocatorServiceTransientFault, ex);
			}
			else
			{
				if (ex is TimeoutException)
				{
					this.m_client.Abort();
					return new ServerLocatorClientTransientException(ServerStrings.ServerLocatorClientWCFCallTimeout, ex);
				}
				if (ex is CommunicationException)
				{
					this.m_client.Abort();
					return new ServerLocatorClientTransientException(ServerStrings.ServerLocatorClientWCFCallCommunicationError, ex);
				}
				if (ex is EndpointNotFoundException)
				{
					this.m_client.Abort();
					return new ServerLocatorClientTransientException(ServerStrings.ServerLocatorClientEndpointNotFoundException, ex);
				}
				return ex;
			}
		}

		private const string WcfEndpointFormat = "net.tcp://{0}:{1}/Exchange.HighAvailability/ServerLocator";

		private const int DefaultHighAvailabilityWebServicePort = 64337;

		private const string HighAvailabilityWebServicePortKey = "SOFTWARE\\Microsoft\\ExchangeServer\\v15\\Replay\\Parameters";

		private static TimeSpan defaultCloseTimeout = TimeSpan.FromMinutes(1.0);

		private static TimeSpan defaultOpenTimeout = TimeSpan.FromMinutes(1.0);

		private static TimeSpan defaultReceiveTimeout = TimeSpan.FromMinutes(10.0);

		private static TimeSpan defaultSendTimeout = TimeSpan.FromMinutes(1.0);

		private static object syncRoot = new object();

		private static int highAvailabilityWebServicePort = 0;

		private bool m_fDisposed;

		private DisposeTracker m_disposeTracker;

		private ServerLocatorClient m_client;
	}
}
