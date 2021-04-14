using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.ServiceModel;
using System.Text;
using System.Threading;
using Microsoft.Ceres.External.ContentApi;
using Microsoft.Ceres.External.ContentApi.DocumentFeeder;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Search;
using Microsoft.Exchange.Net;
using Microsoft.Exchange.Search.Core.Abstraction;
using Microsoft.Exchange.Search.Core.Common;
using Microsoft.Exchange.Search.Core.Diagnostics;
using Microsoft.Exchange.Search.EventLog;
using Microsoft.Exchange.Search.OperatorSchema;
using Microsoft.Exchange.Threading;

namespace Microsoft.Exchange.Search.Fast
{
	internal class FastFeeder : IDisposeTrackable, ISubmitDocument, IDisposable
	{
		public FastFeeder(string hostName, int contentSubmissionPort, TimeSpan submissionTimeout, TimeSpan processingTimeout, TimeSpan lostCallbackTimeout, bool setPerDocumentTimeout, int numSessions, string flowName)
		{
			Util.ThrowOnNullOrEmptyArgument(hostName, "hostName");
			Util.ThrowOnNullOrEmptyArgument(flowName, "flowName");
			this.diagnosticsSession = DiagnosticsSession.CreateComponentDiagnosticsSession("FastFeeder", ComponentInstance.Globals.Search.ServiceName, ExTraceGlobals.FastFeederTracer, (long)this.GetHashCode());
			this.contentSubmissionAddresses = new string[]
			{
				string.Format(CultureInfo.InvariantCulture, "{0}:{1}", new object[]
				{
					hostName,
					contentSubmissionPort
				})
			};
			this.flowName = flowName;
			this.numberOfSessions = Math.Max(1, numSessions);
			this.DocumentProcessingTimeout = processingTimeout;
			this.LostCallbackInterval = processingTimeout;
			this.LostCallbackTimeout = lostCallbackTimeout;
			this.SubmissionTimeout = submissionTimeout;
			this.disposeTracker = this.GetDisposeTracker();
			this.SetPerDocumentTimeout = setPerDocumentTimeout;
		}

		public TimeSpan SubmissionTimeout { get; set; }

		public TimeSpan ConnectionTimeout
		{
			[DebuggerStepThrough]
			get
			{
				return this.connectionTimeout;
			}
			[DebuggerStepThrough]
			set
			{
				this.connectionTimeout = value;
			}
		}

		public bool SetPerDocumentTimeout { get; private set; }

		public TimeSpan DocumentProcessingTimeout { get; set; }

		public TimeSpan LostCallbackTimeout { get; set; }

		public int DocumentFeederBatchSize
		{
			[DebuggerStepThrough]
			get
			{
				return this.documentFeederBatchSize;
			}
			[DebuggerStepThrough]
			set
			{
				this.documentFeederBatchSize = value;
			}
		}

		public int DocumentFeederMaxConnectRetries { get; set; }

		public int DocumentRetries { get; set; }

		public int CompletedCallbackCount
		{
			[DebuggerStepThrough]
			get
			{
				return this.completedCallbackCount;
			}
		}

		public int FailedCallbackCount
		{
			[DebuggerStepThrough]
			get
			{
				return this.failedCallbackCount;
			}
		}

		public IFastDocumentHelper DocumentHelper
		{
			[DebuggerStepThrough]
			get
			{
				return FastFeeder.documentHelper;
			}
		}

		public IDocumentTracker Tracker { get; set; }

		public List<string> PoisonErrorMessages { get; set; }

		public string InstanceName { get; set; }

		public string IndexSystemName { get; set; }

		internal TimeSpan LostCallbackInterval { get; set; }

		public virtual void Initialize()
		{
			try
			{
				IndexManager instance = IndexManager.Instance;
				if (!instance.CheckForNoPendingConfigurationUpdate())
				{
					this.diagnosticsSession.LogDiagnosticsInfo(DiagnosticsLoggingTag.Failures, "Check for Pending Configuration Update failed while trying to Create the CSS connection.", new object[0]);
					throw new FastConnectionException();
				}
				if (!NodeManagementClient.Instance.AreAllNodesHealthy(true))
				{
					this.diagnosticsSession.LogDiagnosticsInfo(DiagnosticsLoggingTag.Failures, "Check for Node health failed while trying to Create the CSS connection.", new object[0]);
					throw new FastConnectionException();
				}
			}
			catch (PerformingFastOperationException innerException)
			{
				throw new FastConnectionException(innerException);
			}
			try
			{
				DocumentFeeder documentFeeder = new DocumentFeeder(new DocumentFeederOptions
				{
					CssNodeList = this.contentSubmissionAddresses,
					Flow = this.flowName,
					Causality = 1,
					BatchSubmissionTimeout = this.SubmissionTimeout,
					MaxDocsInBatch = this.DocumentFeederBatchSize,
					Name = this.flowName,
					NumberOfSessions = this.numberOfSessions,
					ConnectionTimeout = this.ConnectionTimeout,
					CancelOnDispose = true
				});
				documentFeeder.MaxRetries = this.DocumentRetries;
				if (!this.SetPerDocumentTimeout || this.DocumentProcessingTimeout > documentFeeder.DocumentTimeout)
				{
					documentFeeder.DocumentTimeout = this.DocumentProcessingTimeout;
				}
				documentFeeder.MaxConnectRetries = this.DocumentFeederMaxConnectRetries;
				documentFeeder.CompletedCallbackHandlers += this.SubmitDocumentComplete;
				documentFeeder.ReceivedCallbackHandlers += this.SubmitDocumentComplete;
				documentFeeder.FailedCallbackHandlers += this.SubmitDocumentComplete;
				if (!this.SetPerDocumentTimeout)
				{
					this.eventTimer = new GuardedTimer(new TimerCallback(this.LostCallbackTimerCallback), null, TimeSpan.Zero, this.LostCallbackInterval);
				}
				this.documentFeeder = documentFeeder;
			}
			catch (Exception ex)
			{
				if (Util.ShouldRethrowException(ex))
				{
					throw;
				}
				throw new FastConnectionException(ex);
			}
		}

		public ICancelableAsyncResult BeginSubmitDocument(IFastDocument document, AsyncCallback callback, object state)
		{
			Util.ThrowOnNullArgument(document, "document");
			FastFeeder.FastFeederAsyncResult fastFeederAsyncResult = new FastFeeder.FastFeederAsyncResult(document, state, callback);
			lock (this.pendingOperationsLock)
			{
				if (this.disposed)
				{
					throw new ObjectDisposedException("FastFeeder", "BeginSubmitDocument");
				}
				this.pendingOperations.Add(((FastDocument)document).ContextId, fastFeederAsyncResult);
			}
			if (this.SetPerDocumentTimeout)
			{
				fastFeederAsyncResult.StartTimer(this.DocumentProcessingTimeout);
			}
			this.SubmitDocumentInternal(fastFeederAsyncResult);
			return fastFeederAsyncResult;
		}

		public bool EndSubmitDocument(IAsyncResult asyncResult)
		{
			Util.ThrowOnNullArgument(asyncResult, "asyncResult");
			FastFeeder.FastFeederAsyncResult fastFeederAsyncResult = LazyAsyncResult.EndAsyncOperation<FastFeeder.FastFeederAsyncResult>(asyncResult);
			FastDocument fastDocument = (FastDocument)fastFeederAsyncResult.Document;
			lock (this.pendingOperationsLock)
			{
				this.pendingOperations.Remove(fastDocument.ContextId);
			}
			if (fastFeederAsyncResult.Result == null)
			{
				this.ClearDocumentFromTracker(fastDocument);
				return true;
			}
			Exception ex = (Exception)fastFeederAsyncResult.Result;
			this.diagnosticsSession.TraceError<Exception>("FastFeeder.EndSubmitDocument - async operation returned exception: {0}.", ex);
			if (ex is FastConnectionException)
			{
				this.diagnosticsSession.LogPeriodicEvent(MSExchangeFastSearchEventLogConstants.Tuple_FastConnectionException, this.flowName, new object[]
				{
					ex
				});
				throw new FastConnectionException(ex);
			}
			if (ex is FastPermanentDocumentException)
			{
				if (this.Tracker != null)
				{
					this.Tracker.MarkDocumentAsPoison(fastDocument.DocumentId);
				}
				throw new FastPermanentDocumentException(ex.Message, ex);
			}
			if (ex is FastTransientDocumentException)
			{
				if (this.Tracker != null)
				{
					this.Tracker.MarkDocumentAsRetriablePoison(fastDocument.DocumentId);
				}
				throw new FastTransientDocumentException(ex.Message, ex);
			}
			if (ex is TimeoutException)
			{
				throw new FastDocumentTimeoutException(ex.Message, ex);
			}
			if (ex is DocumentFeederLostCallbackException)
			{
				throw new DocumentFeederLostCallbackException(ex.Message, ex);
			}
			if (ex is OperationCanceledException)
			{
				return false;
			}
			throw new ExAssertException(string.Format("Got an Unexpected exception: {0}", ex));
		}

		public bool TryCompleteSubmitDocument(IAsyncResult asyncResult)
		{
			FastFeeder.FastFeederAsyncResult fastFeederAsyncResult = (FastFeeder.FastFeederAsyncResult)asyncResult;
			return fastFeederAsyncResult.InvokeCallback();
		}

		public IFastDocument CreateFastDocument(DocumentOperation operation)
		{
			FastDocument fastDocument = new FastDocument(this.diagnosticsSession, Interlocked.Increment(ref FastFeeder.contextId).ToString("X"), operation);
			if (!string.IsNullOrEmpty(this.InstanceName))
			{
				fastDocument.InstanceName = this.InstanceName;
			}
			if (!string.IsNullOrEmpty(this.IndexSystemName))
			{
				fastDocument.IndexSystemName = this.IndexSystemName;
			}
			return fastDocument;
		}

		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		public DisposeTracker GetDisposeTracker()
		{
			return DisposeTracker.Get<FastFeeder>(this);
		}

		public void SuppressDisposeTracker()
		{
			if (this.disposeTracker != null)
			{
				this.disposeTracker.Suppress();
			}
		}

		internal virtual void WaitForCompletion(TimeSpan timeout)
		{
			if (this.documentFeeder != null)
			{
				try
				{
					this.documentFeeder.WaitForCompletion(timeout);
				}
				catch (ShutdownException)
				{
				}
				catch (TimeoutException)
				{
				}
			}
		}

		internal void ResetCounters()
		{
			this.completedCallbackCount = 0;
			this.failedCallbackCount = 0;
		}

		protected virtual void SubmitDocumentWithDocumentFeeder(FastDocument fastDocument)
		{
			this.documentFeeder.SubmitDocument(fastDocument.Document);
		}

		protected void SubmitDocumentComplete(object sender, DocumentCallback documentCallback)
		{
			string documentID = documentCallback.DocumentID;
			string text = string.Empty;
			bool flag = false;
			bool flag2 = false;
			this.diagnosticsSession.TraceDebug<string, CallbackType>("Document {0} complete, reason: {1}", documentID, documentCallback.CallbackType);
			Exception value;
			switch (documentCallback.CallbackType)
			{
			case 0:
				Interlocked.Increment(ref this.completedCallbackCount);
				value = null;
				break;
			case 1:
			{
				Interlocked.Increment(ref this.failedCallbackCount);
				StringBuilder stringBuilder = new StringBuilder();
				foreach (Message message in documentCallback.Messages)
				{
					stringBuilder.AppendLine(message.MessageText);
				}
				text = stringBuilder.ToString();
				this.diagnosticsSession.TraceError<string, string>("SubmitDocumentComplete: FAST CTS flow returned a {0} error: {1}.", documentCallback.IsTransientError ? "transient" : "permanent", text);
				if (text.Contains("timeout"))
				{
					value = new TimeoutException(text);
					flag = true;
				}
				else if (text.Contains("InvalidRecordDetected"))
				{
					value = new FastPermanentDocumentException(text);
					flag2 = true;
				}
				else if (documentCallback.IsTransientError)
				{
					value = new FastTransientDocumentException(text);
				}
				else
				{
					value = new FastPermanentDocumentException(text);
				}
				break;
			}
			case 2:
				value = null;
				break;
			default:
				text = string.Format("Unknown CallbackType: {0}", documentCallback.CallbackType);
				this.diagnosticsSession.TraceError(text, new object[0]);
				throw new ExAssertException(text);
			}
			FastFeeder.FastFeederAsyncResult fastFeederAsyncResult;
			lock (this.pendingOperationsLock)
			{
				if (!this.pendingOperations.TryGetValue(documentID, out fastFeederAsyncResult))
				{
					this.diagnosticsSession.TraceDebug<string>("Document {0} has already been completed.", documentID);
					return;
				}
			}
			if (!flag && !flag2 && !fastFeederAsyncResult.DocumentResubmission && !string.IsNullOrEmpty(text))
			{
				FastDocument fastDocument = (FastDocument)fastFeederAsyncResult.Document;
				if (fastDocument.FlowOperation == "Indexing" || fastDocument.FlowOperation == "FolderUpdate")
				{
					int errorCode = 1;
					if (text.Contains("Item truncated"))
					{
						if (this.Tracker != null)
						{
							this.Tracker.MarkDocumentAsPoison(fastDocument.DocumentId);
						}
						errorCode = EvaluationErrorsHelper.MakePermanentError(EvaluationErrors.MarsWriterTruncation);
					}
					else if (!documentCallback.IsTransientError)
					{
						if (this.Tracker != null)
						{
							this.Tracker.MarkDocumentAsPoison(fastDocument.DocumentId);
						}
						errorCode = EvaluationErrorsHelper.MakePermanentError(EvaluationErrors.PoisonDocument);
					}
					else
					{
						foreach (string value2 in this.PoisonErrorMessages)
						{
							if (text.Contains(value2))
							{
								if (this.Tracker != null)
								{
									this.Tracker.MarkDocumentAsPoison(fastDocument.DocumentId);
								}
								errorCode = EvaluationErrorsHelper.MakePermanentError(EvaluationErrors.PoisonDocument);
								break;
							}
						}
					}
					this.ResubmitFailureDocument(fastFeederAsyncResult, errorCode, text);
					return;
				}
			}
			fastFeederAsyncResult.InvokeCallback(value);
		}

		private void Dispose(bool calledFromDispose)
		{
			if (calledFromDispose)
			{
				List<FastFeeder.FastFeederAsyncResult> list;
				lock (this.pendingOperationsLock)
				{
					list = new List<FastFeeder.FastFeederAsyncResult>(this.pendingOperations.Values);
					this.pendingOperations.Clear();
					this.disposed = true;
				}
				foreach (FastFeeder.FastFeederAsyncResult fastFeederAsyncResult in list)
				{
					fastFeederAsyncResult.Cancel();
				}
				if (this.documentFeeder != null)
				{
					try
					{
						this.documentFeeder.Dispose();
					}
					catch (Exception)
					{
					}
				}
				if (this.eventTimer != null)
				{
					this.eventTimer.Pause();
					this.eventTimer.Dispose(false);
				}
				if (this.disposeTracker != null)
				{
					this.disposeTracker.Dispose();
					this.disposeTracker = null;
				}
			}
		}

		private void SubmitDocumentInternal(object state)
		{
			FastFeeder.FastFeederAsyncResult fastFeederAsyncResult = (FastFeeder.FastFeederAsyncResult)state;
			FastDocument fastDocument = (FastDocument)fastFeederAsyncResult.Document;
			if (this.disposed)
			{
				fastFeederAsyncResult.InvokeCallback(new ObjectDisposedException("FastFeeder"));
				return;
			}
			Exception ex = null;
			try
			{
				if (this.Tracker != null && fastDocument.Document.Operation != Operation.Delete.Name && fastDocument.ErrorCode == 0 && !IndexId.IsWatermarkIndexId(fastDocument.DocumentId))
				{
					int num = this.Tracker.ShouldDocumentBeStampedWithError(fastDocument.DocumentId);
					if (num != 0)
					{
						fastDocument.Tracked = true;
						fastDocument.ErrorCode = num;
						this.diagnosticsSession.TraceDebug<long>("Marking document with a poison error code. DocumentId: {0}", fastDocument.DocumentId);
					}
					if (this.Tracker.ShouldDocumentBeSkipped(fastDocument.DocumentId))
					{
						this.diagnosticsSession.TraceDebug<long>("Skipping known poison document: {0}", fastDocument.DocumentId);
						fastFeederAsyncResult.InvokeCallback();
						return;
					}
				}
				fastDocument.PrepareForSubmit();
				this.DocumentHelper.ValidateDocumentConsistency(fastDocument, "Immediately before SubmitDocument.");
				this.diagnosticsSession.TraceDebug<string>("Submitting Document {0}", fastDocument.ContextId);
				this.SubmitDocumentWithDocumentFeeder(fastDocument);
			}
			catch (DocumentException ex2)
			{
				this.diagnosticsSession.TraceError<DocumentException>("Received a DocumentException from FAST. Document submission failed - {0}", ex2);
				FastTransientDocumentException value = new FastTransientDocumentException(ex2.Message, ex2);
				fastFeederAsyncResult.InvokeCallback(value);
			}
			catch (CommunicationObjectAbortedException ex3)
			{
				ex = ex3;
			}
			catch (ConnectionException ex4)
			{
				ex = ex4;
			}
			catch (KeyNotFoundException ex5)
			{
				ex = ex5;
			}
			catch (Exception ex6)
			{
				if (Util.ShouldRethrowException(ex6))
				{
					throw;
				}
				ex = ex6;
			}
			if (ex != null)
			{
				this.diagnosticsSession.TraceError<Exception>("Received a connection exception from FAST. Shutting down the document feeder: {0}", ex);
				FastConnectionException value2 = new FastConnectionException(ex);
				fastFeederAsyncResult.InvokeCallback(value2);
			}
		}

		private void ResubmitFailureDocument(FastFeeder.FastFeederAsyncResult asyncResult, int errorCode, string errorMessage)
		{
			FastDocument fastDocument = (FastDocument)asyncResult.Document;
			FastDocument fastDocument2 = (FastDocument)this.CreateFastDocument(DocumentOperation.Update);
			this.DocumentHelper.ValidateDocumentConsistency(fastDocument, "Original document used as source to clone for resubmit is invalid.");
			this.DocumentHelper.PopulateFastDocumentForIndexing(fastDocument2, fastDocument.FeedingVersion, fastDocument.MailboxGuid, fastDocument.IsMoveDestination, fastDocument.IsLocalMdb, fastDocument.DocumentId, fastDocument.CompositeItemId, errorCode, fastDocument.AttemptCount);
			fastDocument2.ErrorMessage = errorMessage;
			fastDocument2.Tracked = true;
			asyncResult.Document = fastDocument2;
			asyncResult.DocumentResubmission = true;
			lock (this.pendingOperationsLock)
			{
				this.pendingOperations.Add(fastDocument2.ContextId, asyncResult);
				this.pendingOperations.Remove(fastDocument.ContextId);
			}
			ThreadPool.QueueUserWorkItem(new WaitCallback(this.SubmitDocumentInternal), asyncResult);
		}

		private void ClearDocumentFromTracker(FastDocument fastDocument)
		{
			if (this.Tracker != null)
			{
				this.Tracker.RecordDocumentProcessingComplete(fastDocument.CorrelationId, fastDocument.DocumentId, fastDocument.Tracked);
			}
		}

		private void LostCallbackTimerCallback(object state)
		{
			lock (this.pendingOperationsLock)
			{
				TimeSpan t = this.DocumentProcessingTimeout + this.LostCallbackTimeout;
				List<FastFeeder.FastFeederAsyncResult> list = new List<FastFeeder.FastFeederAsyncResult>(this.pendingOperations.Values);
				foreach (FastFeeder.FastFeederAsyncResult fastFeederAsyncResult in list)
				{
					if (fastFeederAsyncResult.SubmitTime + t < DateTime.UtcNow)
					{
						fastFeederAsyncResult.InvokeCallback(new DocumentFeederLostCallbackException(fastFeederAsyncResult.Document.CompositeItemId));
					}
				}
			}
		}

		private static readonly IFastDocumentHelper documentHelper = new FastDocumentHelper();

		private static long contextId;

		private readonly IDiagnosticsSession diagnosticsSession;

		private readonly string[] contentSubmissionAddresses;

		private readonly string flowName;

		private readonly int numberOfSessions;

		private readonly Dictionary<string, FastFeeder.FastFeederAsyncResult> pendingOperations = new Dictionary<string, FastFeeder.FastFeederAsyncResult>();

		private readonly object pendingOperationsLock = new object();

		private TimeSpan connectionTimeout = TimeSpan.FromSeconds(10.0);

		private int documentFeederBatchSize = 1000;

		private DocumentFeeder documentFeeder;

		private int completedCallbackCount;

		private int failedCallbackCount;

		private GuardedTimer eventTimer;

		private bool disposed;

		private DisposeTracker disposeTracker;

		private class FastFeederAsyncResult : LazyAsyncResultWithTimeout
		{
			public FastFeederAsyncResult(IFastDocument document, object callerState, AsyncCallback callback) : base(null, callerState, callback)
			{
				this.Document = document;
				this.SubmitTime = DateTime.UtcNow;
			}

			public IFastDocument Document { get; set; }

			public bool DocumentResubmission { get; set; }

			public DateTime SubmitTime { get; set; }
		}
	}
}
