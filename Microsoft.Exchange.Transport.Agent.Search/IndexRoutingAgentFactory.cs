using System;
using System.Threading;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Transport;
using Microsoft.Exchange.Data.Transport.Routing;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Search;
using Microsoft.Exchange.ProcessManager;
using Microsoft.Exchange.Search.Core.Abstraction;
using Microsoft.Exchange.Search.Core.Common;
using Microsoft.Exchange.Search.Core.Diagnostics;
using Microsoft.Exchange.Search.Fast;
using Microsoft.Exchange.Search.OperatorSchema;

namespace Microsoft.Exchange.Transport.Agent.Search
{
	internal class IndexRoutingAgentFactory : RoutingAgentFactory
	{
		public IndexRoutingAgentFactory()
		{
			this.diagnosticsSession = DiagnosticsSession.CreateComponentDiagnosticsSession("IndexRoutingAgentFactory", ComponentInstance.Globals.Search.ServiceName, ExTraceGlobals.IndexRoutingAgentTracer, (long)this.GetHashCode());
			this.Config = new FlightingSearchConfig();
			this.enabled = this.Config.IndexAgentEnabled;
			this.diagnosticsSession.TraceDebug("Begin Factory Initialization.", new object[0]);
			int indexAgentListenPort = this.Config.IndexAgentListenPort;
			this.submissionPort = this.Config.ContentSubmissionPort;
			this.submissionHost = this.Config.HostName;
			if (this.Config.UseTransportAgentTestFlow)
			{
				this.flow = "Internal.Exchange.TransportAgentTestFlow";
			}
			else
			{
				this.flow = FlowDescriptor.GetTransportFlowDescriptor(this.Config).DisplayName;
			}
			this.documentFeeders = this.Config.IndexAgentFastFeeders;
			this.connectionTimeout = this.Config.TxDocumentFeederConnectionTimeout;
			this.transactionTimeout = this.Config.IndexAgentStreamTimeout;
			int indexAgentErrorsBeforeDelay = this.Config.IndexAgentErrorsBeforeDelay;
			TimeSpan indexAgentErrorDelayInterval = this.Config.IndexAgentErrorDelayInterval;
			if (this.enabled)
			{
				this.streamManager = StreamManager.CreateForListen(ComponentInstance.Globals.Search.ServiceName, new TcpListener.HandleFailure(this.OnTcpListenerFailure));
				this.streamManager.ConnectionTimeout = this.transactionTimeout;
				this.streamManager.ListenPort = indexAgentListenPort;
				this.streamManager.StartListening();
			}
			this.diagnosticsSession.TraceDebug("Factory Initialized.", new object[0]);
		}

		internal ITransportFlowFeeder TransportFlowFeeder { get; private set; }

		internal SearchConfig Config { get; private set; }

		public override RoutingAgent CreateAgent(SmtpServer server)
		{
			this.IsReadyToProcessMessages();
			return new IndexRoutingAgent(this);
		}

		public override void Close()
		{
			IndexRoutingAgentFactory.notActivelyInitializing.Set();
			if (this.streamManager != null)
			{
				this.streamManager.StopListening();
			}
			if (this.feeder != null)
			{
				this.feeder.Dispose();
			}
			this.diagnosticsSession.TraceDebug("Factory closed", new object[0]);
		}

		internal void ReportConnectionStatus(bool messageProcessed)
		{
			lock (IndexRoutingAgentFactory.readyToProcessMessagesLock)
			{
				if (messageProcessed)
				{
					IndexRoutingAgentFactory.failedItemCounter = 0;
					return;
				}
				if (++IndexRoutingAgentFactory.failedItemCounter <= IndexRoutingAgentFactory.errorsBeforeDelay)
				{
					return;
				}
				this.SetFactoryToFailureState();
			}
			this.diagnosticsSession.TraceError<TimeSpan>("Too many errors. Delaying for {0}", IndexRoutingAgentFactory.errorDelayInterval);
		}

		internal bool IsReadyToProcessMessages()
		{
			this.diagnosticsSession.TraceDebug("Checking to see if the Feeder is ready to process messages.", new object[0]);
			if (!this.enabled)
			{
				return false;
			}
			if ((this.GetInstalledRoles() & (ServerRole.Mailbox | ServerRole.HubTransport)) == ServerRole.None)
			{
				return false;
			}
			int num = Interlocked.CompareExchange(ref IndexRoutingAgentFactory.submissionClientState, 1, 0);
			this.diagnosticsSession.TraceDebug<int>("The state of the RoutingAgent's Feeding client is currently: {0}.", num);
			switch (num)
			{
			case 0:
				break;
			case 1:
				return false;
			case 2:
				return false;
			case 3:
				return true;
			case 4:
				if (DateTime.UtcNow < IndexRoutingAgentFactory.nextFastRetryTime)
				{
					this.diagnosticsSession.TraceError("Not time to process messages.", new object[0]);
					return false;
				}
				num = Interlocked.CompareExchange(ref IndexRoutingAgentFactory.submissionClientState, 1, 4);
				if (num != 4)
				{
					return false;
				}
				break;
			default:
				throw new InvalidOperationException(string.Format("Unknown SubmissionClientState in the IndexRoutingAgentFactory. {0}", num));
			}
			IndexRoutingAgentFactory.notActivelyInitializing.Reset();
			Interlocked.Exchange(ref IndexRoutingAgentFactory.submissionClientState, 2);
			ThreadPool.QueueUserWorkItem(new WaitCallback(this.InitializeFastFeeder));
			return this.CheckForStateWhileInitializing();
		}

		private void InitializeFastFeeder(object state)
		{
			bool flag = false;
			this.diagnosticsSession.TraceDebug("Begin Initializing the Fast Feeder.", new object[0]);
			try
			{
				this.EnsureTransportCtsFlow();
				try
				{
					if (this.feeder != null)
					{
						this.feeder.Dispose();
					}
					using (DisposeGuard disposeGuard = default(DisposeGuard))
					{
						this.feeder = Factory.Current.CreateFastFeeder(this.submissionHost, this.submissionPort, this.documentFeeders, this.connectionTimeout, TimeSpan.Zero, this.transactionTimeout, this.flow);
						disposeGuard.Add<ISubmitDocument>(this.feeder);
						this.feeder.IndexSystemName = string.Empty;
						if (!this.Config.SkipMdmGeneration)
						{
							IIndexManager indexManager = Factory.Current.CreateIndexManager();
							string transportIndexSystem = indexManager.GetTransportIndexSystem();
							if (string.IsNullOrWhiteSpace(transportIndexSystem))
							{
								this.diagnosticsSession.TraceError("Need the transport flow Index system to generate an Mdm", new object[0]);
								this.SetFactoryToFailureState();
								return;
							}
							this.feeder.IndexSystemName = transportIndexSystem;
						}
						this.TransportFlowFeeder = new TransportFlowFeeder(this.streamManager, this.feeder);
						IndexRoutingAgentFactory.failedItemCounter = 0;
						flag = true;
						Interlocked.Exchange(ref IndexRoutingAgentFactory.submissionClientState, 3);
						disposeGuard.Success();
					}
				}
				catch (FastConnectionException arg)
				{
					this.diagnosticsSession.TraceError<FastConnectionException>("Exception creating FastFeeder: {0}", arg);
					this.SetFactoryToFailureState();
				}
			}
			finally
			{
				IndexRoutingAgentFactory.notActivelyInitializing.Set();
				this.diagnosticsSession.TraceDebug<string>("End Initializing the Fast Feeder. Result: {0}", flag ? "Success" : "Failure");
			}
		}

		private bool CheckForStateWhileInitializing()
		{
			return IndexRoutingAgentFactory.notActivelyInitializing.WaitOne(this.transactionTimeout) && this.IsReadyToProcessMessages();
		}

		private void SetFactoryToFailureState()
		{
			IndexRoutingAgentFactory.nextFastRetryTime = DateTime.UtcNow + IndexRoutingAgentFactory.errorDelayInterval;
			Interlocked.Exchange(ref IndexRoutingAgentFactory.submissionClientState, 4);
		}

		private bool EnsureTransportCtsFlow()
		{
			bool result;
			try
			{
				this.diagnosticsSession.TraceDebug("Attempting EnsureTransportFlow", new object[0]);
				FlowManager.Instance.EnsureTransportFlow();
				this.diagnosticsSession.TraceDebug("EnsureTransportFlow success", new object[0]);
				result = true;
			}
			catch (PerformingFastOperationException arg)
			{
				this.diagnosticsSession.TraceError<PerformingFastOperationException>("Exception calling EnsureTransportFlow: {0}", arg);
				IndexRoutingAgentFactory.nextFastRetryTime = DateTime.UtcNow + IndexRoutingAgentFactory.errorDelayInterval;
				result = false;
			}
			return result;
		}

		private ServerRole GetInstalledRoles()
		{
			if (this.installedRoles != ServerRole.None)
			{
				return this.installedRoles;
			}
			Exception ex = null;
			try
			{
				ADNotificationAdapter.TryRunADOperation(delegate()
				{
					ITopologyConfigurationSession topologyConfigurationSession = DirectorySessionFactory.Default.CreateTopologyConfigurationSession(ConsistencyMode.FullyConsistent, ADSessionSettings.FromRootOrgScopeSet(), 441, "GetInstalledRoles", "f:\\15.00.1497\\sources\\dev\\mexagents\\src\\Search\\IndexRoutingAgentFactory.cs");
					Server server = topologyConfigurationSession.FindLocalServer();
					this.installedRoles = server.CurrentServerRole;
				});
			}
			catch (LocalServerNotFoundException ex2)
			{
				ex = ex2;
			}
			catch (ADTransientException ex3)
			{
				ex = ex3;
			}
			catch (ADExternalException ex4)
			{
				ex = ex4;
			}
			catch (DataValidationException ex5)
			{
				ex = ex5;
			}
			if (ex != null)
			{
				this.diagnosticsSession.TraceError<Exception>("Unable to determine installed server roles: {0}", ex);
			}
			return this.installedRoles;
		}

		private void OnTcpListenerFailure(bool addressAlreadyInUseFailure)
		{
			this.diagnosticsSession.TraceError("Stop Listening", new object[0]);
			this.enabled = false;
		}

		private const string ComponentName = "IndexRoutingAgentFactory";

		private const ServerRole RunOnRoles = ServerRole.Mailbox | ServerRole.HubTransport;

		private static readonly object readyToProcessMessagesLock = new object();

		private static DateTime nextFastRetryTime;

		private static int submissionClientState;

		private static ManualResetEvent notActivelyInitializing = new ManualResetEvent(true);

		private static int failedItemCounter;

		private static TimeSpan errorDelayInterval = TimeSpan.FromSeconds(60.0);

		private static int errorsBeforeDelay = 5;

		private readonly string submissionHost;

		private readonly int submissionPort;

		private readonly TimeSpan connectionTimeout;

		private readonly TimeSpan transactionTimeout;

		private readonly int documentFeeders;

		private readonly string flow;

		private readonly IDiagnosticsSession diagnosticsSession;

		private readonly IStreamManager streamManager;

		private bool enabled;

		private ISubmitDocument feeder;

		private ServerRole installedRoles;

		private class SubmissionClientState
		{
			public const int Uninitialized = 0;

			public const int ReadyToInitialize = 1;

			public const int Initializing = 2;

			public const int Initialized = 3;

			public const int Failed = 4;
		}
	}
}
