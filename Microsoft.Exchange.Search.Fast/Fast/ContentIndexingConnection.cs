using System;
using System.IO;
using System.Threading;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ProcessManager;
using Microsoft.Exchange.Search.Core.Abstraction;
using Microsoft.Exchange.Search.Core.Common;
using Microsoft.Exchange.Search.Core.Diagnostics;
using Microsoft.Exchange.Search.OperatorSchema;

namespace Microsoft.Exchange.Search.Fast
{
	internal sealed class ContentIndexingConnection : Disposable
	{
		internal ContentIndexingConnection(SearchConfig configuration)
		{
			using (DisposeGuard disposeGuard = this.Guard())
			{
				FlowManager.Instance.EnsureTransportFlow();
				this.configuration = configuration;
				string displayName = FlowDescriptor.GetTransportFlowDescriptor(configuration).DisplayName;
				this.fastFeeder = Factory.Current.CreateFastFeeder(configuration.HostName, configuration.SubmissionPort, configuration.DocumentFeeders, configuration.TxDocumentFeederConnectionTimeout, configuration.BatchSubmissionTimeout, configuration.StreamTimeout, displayName);
				this.fastFeeder.IndexSystemName = string.Empty;
				this.streamManager = StreamManager.CreateForListen(configuration.ServiceName, new TcpListener.HandleFailure(this.OnTcpListenerFailure));
				this.streamManager.ConnectionTimeout = configuration.StreamTimeout;
				this.streamManager.ListenPort = configuration.ListenPort;
				this.streamManager.StartListening();
				if (!configuration.SkipMdmGeneration)
				{
					IIndexManager indexManager = Factory.Current.CreateIndexManager();
					string transportIndexSystem = indexManager.GetTransportIndexSystem();
					if (string.IsNullOrWhiteSpace(transportIndexSystem))
					{
						throw new IndexSystemNotFoundException(displayName);
					}
					this.fastFeeder.IndexSystemName = transportIndexSystem;
				}
				this.flowFeeder = new TransportFlowFeeder(this.streamManager, this.fastFeeder);
				disposeGuard.Success();
			}
		}

		internal static IDiagnosticsSession Diagnostics
		{
			get
			{
				return ContentIndexingConnectionFactory.Diagnostics;
			}
		}

		internal int NumberOfConnectionLevelFailures
		{
			get
			{
				return this.numberOfConnectionLevelFailures;
			}
		}

		internal TimeSpan AgeOfItemToBypassNlg
		{
			get
			{
				return this.configuration.AgeOfItemToBypassNlg;
			}
		}

		internal bool SkipWordBreakingDuringMRS
		{
			get
			{
				return this.configuration.SkipWordBreakingDuringMRS;
			}
		}

		internal bool AlwaysInvalidateAnnotationToken
		{
			get
			{
				return this.configuration.AlwaysInvalidateAnnotationToken;
			}
		}

		internal static IDisposable SetProcessMessageTestHook(Action action)
		{
			return ContentIndexingConnection.processMessageTestHook.SetTestHook(action);
		}

		internal void ProcessMessage(Stream mimeStream, Stream propertyStream)
		{
			this.ProcessMessage(mimeStream, propertyStream, false);
		}

		internal void ProcessMessage(Stream mimeStream, Stream propertyStream, bool shouldBypassNlg)
		{
			if (ContentIndexingConnection.processMessageTestHook.Value != null)
			{
				ContentIndexingConnection.processMessageTestHook.Value();
			}
			TransportFlowMessageFlags transportFlowMessageFlags = shouldBypassNlg ? TransportFlowMessageFlags.ShouldBypassNlg : TransportFlowMessageFlags.None;
			if (this.configuration.SkipMdmGeneration)
			{
				transportFlowMessageFlags |= TransportFlowMessageFlags.SkipMdmGeneration;
			}
			if (this.configuration.SkipTokenInfoGeneration)
			{
				transportFlowMessageFlags |= TransportFlowMessageFlags.SkipTokenInfoGeneration;
			}
			this.flowFeeder.ProcessMessage(mimeStream, propertyStream, transportFlowMessageFlags);
		}

		internal void ProcessAnnotationFailure(bool connectionLevel, Exception exception)
		{
			ContentIndexingConnection.Diagnostics.TraceError<bool, string>("ConnectionLevel: {0}, Exception: {1}", connectionLevel, (exception != null) ? exception.ToString() : "<null>");
			if (connectionLevel)
			{
				Interlocked.Increment(ref this.numberOfConnectionLevelFailures);
				ContentIndexingConnectionFactory.OnConnectionLevelFailure(this, false);
			}
		}

		internal void MarkAlive()
		{
			Interlocked.Exchange(ref this.numberOfConnectionLevelFailures, 0);
		}

		protected sealed override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<ContentIndexingConnection>(this);
		}

		protected override void InternalDispose(bool disposing)
		{
			if (disposing)
			{
				if (this.streamManager != null)
				{
					this.streamManager.Dispose();
				}
				if (this.fastFeeder != null)
				{
					this.fastFeeder.Dispose();
				}
			}
		}

		private void OnTcpListenerFailure(bool addressAlreadyInUseFailure)
		{
			ContentIndexingConnection.Diagnostics.TraceDebug("ContentIndexingConnection::OnTcpListenerFailure() called", new object[0]);
			ContentIndexingConnectionFactory.OnConnectionLevelFailure(this, true);
		}

		private static Hookable<Action> processMessageTestHook = Hookable<Action>.Create(true, null);

		private readonly ISubmitDocument fastFeeder;

		private StreamManager streamManager;

		private TransportFlowFeeder flowFeeder;

		private int numberOfConnectionLevelFailures;

		private SearchConfig configuration;
	}
}
