using System;
using System.Collections.Generic;
using System.Threading;
using System.Xml.Linq;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Search;
using Microsoft.Exchange.Search.Core.Abstraction;
using Microsoft.Exchange.Search.Core.Common;
using Microsoft.Exchange.Search.Core.Diagnostics;
using Microsoft.Exchange.Search.OperatorSchema;
using Microsoft.Exchange.Search.Performance;

namespace Microsoft.Exchange.Search.Mdb
{
	internal class RetryFeeder : Executable, IFeeder, IExecutable, IDiagnosable, IDisposable
	{
		public RetryFeeder(MdbPerfCountersInstance perfCounterInstance, MdbInfo mdbInfo, ISubmitDocument indexFeeder, ISearchServiceConfig config, IFailedItemStorage failedItemStorage, IWatermarkStorage watermarkStorage, IIndexStatusStore indexStatusStore) : base(config)
		{
			Util.ThrowOnNullArgument(perfCounterInstance, "perfCounterInstance");
			Util.ThrowOnNullArgument(indexFeeder, "indexFeeder");
			Util.ThrowOnNullArgument(failedItemStorage, "failedItemStorage");
			Util.ThrowOnNullArgument(watermarkStorage, "watermarkStorage");
			Util.ThrowOnNullArgument(indexStatusStore, "indexStatusStore");
			base.DiagnosticsSession.ComponentName = "RetryFeeder";
			base.DiagnosticsSession.Tracer = ExTraceGlobals.MdbRetryFeederTracer;
			this.mdbInfo = mdbInfo;
			this.perfCounterInstance = perfCounterInstance;
			this.indexFeeder = indexFeeder;
			this.indexStatusStore = indexStatusStore;
			this.failedItemStorage = failedItemStorage;
			this.watermarkStorage = watermarkStorage;
			this.throttlingManager = Factory.Current.CreateFeederDelayThrottlingManager(config);
			this.documentQueueManager = new QueueManager<IDocEntry>(base.Config.QueueSize, base.Config.QueueSize, null);
			this.exceptionOccurred = Strings.ExceptionOccurred(mdbInfo.Guid);
			this.StartTime = DateTime.UtcNow;
		}

		public FeederType FeederType
		{
			get
			{
				return FeederType.Retry;
			}
		}

		internal DateTime StartTime { get; set; }

		private Semaphore QuerySemaphore
		{
			get
			{
				if (RetryFeeder.querySemaphore != null)
				{
					return RetryFeeder.querySemaphore;
				}
				Semaphore semaphore = new Semaphore(base.Config.ConcurrentRetryQueries, base.Config.ConcurrentRetryQueries);
				Semaphore semaphore2 = Interlocked.CompareExchange<Semaphore>(ref RetryFeeder.querySemaphore, semaphore, null);
				if (semaphore2 == null)
				{
					return semaphore;
				}
				semaphore.Dispose();
				return semaphore2;
			}
		}

		public override XElement GetDiagnosticInfo(DiagnosableParameters parameters)
		{
			XElement diagnosticInfo = base.GetDiagnosticInfo(parameters);
			diagnosticInfo.Add(new XElement("RetryLastPollTime", this.StartTime));
			diagnosticInfo.Add(new XElement("RetryItems", this.retryItemsCount));
			diagnosticInfo.Add(new XElement("FailedItems", this.failedItemsCount));
			return diagnosticInfo;
		}

		public override DisposeTracker GetDisposeTracker()
		{
			return DisposeTracker.Get<RetryFeeder>(this);
		}

		protected override void InternalExecutionStart()
		{
			using (IDisposable disposable = this.activeThreadCount.AcquireReference())
			{
				if (disposable == null)
				{
					return;
				}
				this.RestrictedQuery(delegate
				{
					this.failedItemsCount = this.failedItemStorage.GetPermanentFailureCount();
					base.DiagnosticsSession.SetCounterRawValue(this.perfCounterInstance.FailedItemsCount, this.failedItemsCount);
				});
				this.documentEnumerator = this.GetDocuments().GetEnumerator();
				this.MoveNextDocument();
			}
			this.TryCollectDocumentsDelayed();
		}

		protected override void InternalExecutionFinish()
		{
			this.activeThreadCount.DisableAddRef();
		}

		protected override void Dispose(bool calledFromDispose)
		{
			if (calledFromDispose)
			{
				if (!this.activeThreadCount.TryWaitForZero(base.Config.MaxOperationTimeout))
				{
					base.DiagnosticsSession.LogDiagnosticsInfo(DiagnosticsLoggingTag.Warnings, "Waiting for threads to stop: Did not shut down in a timely manner.", new object[0]);
				}
				if (this.documentEnumerator != null)
				{
					lock (this.documentEnumerator)
					{
						this.documentEnumerator.Dispose();
					}
				}
			}
			base.Dispose(calledFromDispose);
		}

		private void RestrictedQuery(Action query)
		{
			try
			{
				this.QuerySemaphore.WaitOne();
				query();
			}
			finally
			{
				this.QuerySemaphore.Release();
			}
		}

		private IEnumerable<IDocEntry> GetDocuments()
		{
			bool continueToNextRetryLoop;
			do
			{
				continueToNextRetryLoop = false;
				ICollection<IFailureEntry> failureCollection = null;
				this.RestrictedQuery(delegate
				{
					failureCollection = this.failedItemStorage.GetRetriableItems(FieldSet.RetryFeederProperties);
					this.retryItemsCount = failureCollection.Count;
					this.DiagnosticsSession.SetCounterRawValue(this.perfCounterInstance.RetriableItemsCount, (long)this.retryItemsCount);
					this.indexStatusStore.UpdateIndexStatus(this.mdbInfo.Guid, IndexStatusIndex.RetriableItemsCount, (long)this.retryItemsCount);
				});
				if (this.retryItemsCount == 0)
				{
					base.DiagnosticsSession.TraceDebug("No items to retry", new object[0]);
				}
				else
				{
					foreach (IFailureEntry document in failureCollection)
					{
						yield return document;
					}
				}
				if (!base.Config.DisableDeletedMailbox)
				{
					ICollection<MailboxState> deletedMailboxes = this.watermarkStorage.GetMailboxesForDeleting();
					base.DiagnosticsSession.SetCounterRawValue(this.perfCounterInstance.MailboxesLeftToDelete, (long)deletedMailboxes.Count);
					if (deletedMailboxes.Count == 0)
					{
						base.DiagnosticsSession.TraceDebug("No mailboxes for deletion.", new object[0]);
					}
					else
					{
						DateTime stopTime = DateTime.UtcNow + base.Config.DeletionPendingTimeout;
						foreach (MailboxState deletedMailboxState in deletedMailboxes)
						{
							int mailboxNumber = deletedMailboxState.MailboxNumber;
							ICollection<IDocEntry> collection = null;
							long count = 0L;
							this.RestrictedQuery(delegate
							{
								collection = this.failedItemStorage.GetDeletionPendingItems(mailboxNumber);
								count = (long)collection.Count;
							});
							if (count == 0L)
							{
								base.DiagnosticsSession.TraceDebug<int>("No items to delete for mailbox {0}", mailboxNumber);
								base.TryRunUnderExceptionHandler(delegate()
								{
									this.watermarkStorage.BeginSetMailboxDeletionPending(new MailboxState(mailboxNumber, int.MaxValue), new AsyncCallback(this.FinishSetMailboxDeletionPending), null);
								}, this.exceptionOccurred);
								base.DiagnosticsSession.DecrementCounter(this.perfCounterInstance.MailboxesLeftToDelete);
							}
							else
							{
								foreach (IDocEntry document2 in collection)
								{
									if (!(DateTime.UtcNow < stopTime))
									{
										continueToNextRetryLoop = true;
										break;
									}
									yield return document2;
								}
							}
						}
					}
				}
			}
			while (continueToNextRetryLoop);
			yield break;
		}

		private void FinishSetMailboxDeletionPending(IAsyncResult ar)
		{
			base.TryRunUnderExceptionHandler(delegate()
			{
				this.watermarkStorage.EndSetMailboxDeletionPending(ar);
			}, Strings.ErrorAccessingStateStorage);
		}

		private bool MoveNextDocument()
		{
			bool flag;
			for (;;)
			{
				if (!base.TryRunUnderExceptionHandler<bool>(() => !base.Stopping && this.documentEnumerator.MoveNext(), out flag, this.exceptionOccurred))
				{
					break;
				}
				IDocEntry docEntry = this.documentEnumerator.Current;
				if (docEntry != null && docEntry.RawItemId == null)
				{
					IFailureEntry failureEntry = docEntry as IFailureEntry;
					if (failureEntry == null)
					{
						base.DiagnosticsSession.LogDiagnosticsInfo(DiagnosticsLoggingTag.Warnings, "RetryFeeder.MoveNextDocument: Got DocEntry with null RawItemId for deleted mailbox. MailboxGuid={0},IndexId={1}", new object[]
						{
							docEntry.MailboxGuid,
							docEntry.IndexId
						});
					}
					else
					{
						base.DiagnosticsSession.LogDiagnosticsInfo(DiagnosticsLoggingTag.Warnings, "RetryFeeder.MoveNextDocument: Got FailureEntry with null RawItemId. MailboxGuid={0},IndexId={1},ErrorCode={2},ErrorDescription={3},AdditionalInfo={4},IsPartiallyIndexed={5},IsPermanentFailure={6},AttemptCount={7},LastAttemptTime={8}.", new object[]
						{
							failureEntry.MailboxGuid,
							failureEntry.IndexId,
							failureEntry.ErrorCode,
							failureEntry.ErrorDescription,
							failureEntry.AdditionalInfo,
							failureEntry.IsPartiallyIndexed,
							failureEntry.IsPermanentFailure,
							failureEntry.AttemptCount,
							(failureEntry.LastAttemptTime != null) ? failureEntry.LastAttemptTime.ToString() : string.Empty
						});
					}
				}
				if (!flag || this.documentEnumerator.Current.RawItemId != null || (!(this.documentEnumerator.Current is IFailureEntry) && base.Config.ProcessItemsWithNullCompositeId))
				{
					goto IL_18B;
				}
			}
			this.currentDocument = null;
			return false;
			IL_18B:
			this.currentDocument = (flag ? this.documentEnumerator.Current : null);
			return flag;
		}

		private void CollectDocuments(object state)
		{
			this.CollectDocuments(state, true);
		}

		private void CollectDocuments(object state, bool timerFired)
		{
			if (!timerFired)
			{
				return;
			}
			using (IDisposable disposable = this.activeThreadCount.AcquireReference())
			{
				if (disposable != null)
				{
					IDocEntry docEntry = null;
					try
					{
						while (!base.Stopping)
						{
							lock (this.documentEnumerator)
							{
								docEntry = this.currentDocument;
								if (docEntry == null)
								{
									break;
								}
								IFailureEntry failureEntry = docEntry as IFailureEntry;
								if (failureEntry != null)
								{
									TimeSpan t = (failureEntry.LastAttemptTime != null) ? (this.StartTime - failureEntry.LastAttemptTime.Value) : TimeSpan.MaxValue;
									if ((failureEntry.AttemptCount == 0 && t < base.Config.RetriableItemsPollInterval) || (failureEntry.AttemptCount > 0 && t < base.Config.RetriableItemsSubsequentRetryInterval))
									{
										this.MoveNextDocument();
										continue;
									}
								}
								if (!this.documentQueueManager.Enqueue(docEntry))
								{
									base.DiagnosticsSession.TraceDebug("Document queue is full", new object[0]);
									break;
								}
								if (!this.MoveNextDocument())
								{
									base.DiagnosticsSession.TraceDebug("No more documents", new object[0]);
									break;
								}
							}
						}
					}
					finally
					{
						if (docEntry == null && this.documentQueueManager.Length == 0 && this.documentQueueManager.OutstandingLength == 0)
						{
							base.DiagnosticsSession.TraceDebug("RetryFeeder has completed", new object[0]);
							base.CompleteExecute(null);
						}
						else
						{
							Interlocked.Exchange(ref this.collectingDocuments, 0);
							this.SendDocuments();
						}
					}
				}
			}
		}

		private void SendDocuments()
		{
			IEnumerable<IDocEntry> enumerable;
			lock (this.documentQueueManager)
			{
				if (this.documentQueueManager.Dequeue(out enumerable))
				{
					foreach (IDocEntry docEntry in enumerable)
					{
						if (base.Stopping)
						{
							this.documentQueueManager.Remove(docEntry);
						}
						else
						{
							IFailureEntry failureEntry = docEntry as IFailureEntry;
							IFastDocument fastDocument;
							if (failureEntry != null)
							{
								base.DiagnosticsSession.IncrementCounter(this.perfCounterInstance.NumberOfDocumentsSentForProcessingRetry);
								fastDocument = this.indexFeeder.CreateFastDocument(DocumentOperation.Insert);
								int num = failureEntry.AttemptCount + 1;
								if (num == 1)
								{
									base.DiagnosticsSession.IncrementCounter(this.perfCounterInstance.TotalDocumentsFirstRetryAttempt);
								}
								else
								{
									base.DiagnosticsSession.IncrementCounter(this.perfCounterInstance.TotalDocumentsMutlipleRetryAttempts);
								}
								if (failureEntry.ErrorCode == EvaluationErrors.AnnotationTokenError)
								{
									this.indexFeeder.DocumentHelper.PopulateFastDocumentForIndexing(fastDocument, this.mdbInfo.CatalogVersion.FeedingVersion, failureEntry.MailboxGuid, false, !this.mdbInfo.IsLagCopy, failureEntry.IndexId, failureEntry.RawItemId ?? string.Empty, 8, num);
									base.DiagnosticsSession.TraceDebug<IDocEntry>("Set error code to AnnotationTokenError for document: {0}.", docEntry);
								}
								else
								{
									this.indexFeeder.DocumentHelper.PopulateFastDocumentForIndexing(fastDocument, this.mdbInfo.CatalogVersion.FeedingVersion, failureEntry.MailboxGuid, false, !this.mdbInfo.IsLagCopy, failureEntry.IndexId, failureEntry.RawItemId ?? string.Empty, 0, num);
								}
							}
							else
							{
								base.DiagnosticsSession.IncrementCounter(this.perfCounterInstance.NumberOfDocumentsSentForDeletion);
								fastDocument = this.indexFeeder.CreateFastDocument(DocumentOperation.Delete);
								this.indexFeeder.DocumentHelper.PopulateFastDocumentForDelete(fastDocument, docEntry.MailboxGuid, docEntry.IndexId);
							}
							try
							{
								this.indexFeeder.BeginSubmitDocument(fastDocument, new AsyncCallback(this.DocumentCompleteCallback), docEntry);
							}
							catch (ObjectDisposedException result)
							{
								base.DiagnosticsSession.TraceError("FastFeeder has been disposed", new object[0]);
								base.CompleteExecute(result);
								return;
							}
						}
					}
				}
			}
			if (enumerable != null)
			{
				this.TryCollectDocumentsDelayed();
			}
		}

		private void DocumentCompleteCallback(IAsyncResult asyncResult)
		{
			using (IDisposable disposable = this.activeThreadCount.AcquireReference())
			{
				IDocEntry docEntry = (IDocEntry)asyncResult.AsyncState;
				try
				{
					this.documentQueueManager.Remove(docEntry);
					if (!this.indexFeeder.EndSubmitDocument(asyncResult))
					{
						base.CompleteExecute(null);
						return;
					}
					base.DiagnosticsSession.IncrementCounter(this.perfCounterInstance.NumberOfDocumentsProcessed);
					if (docEntry is IFailureEntry)
					{
						base.DiagnosticsSession.DecrementCounter(this.perfCounterInstance.RetriableItemsCount);
						Interlocked.Decrement(ref this.retryItemsCount);
						this.indexStatusStore.UpdateIndexStatus(this.mdbInfo.Guid, IndexStatusIndex.RetriableItemsCount, (long)this.retryItemsCount);
						base.DiagnosticsSession.IncrementCounter(this.perfCounterInstance.NumberOfDocumentsIndexedRetry);
					}
					else
					{
						base.DiagnosticsSession.IncrementCounter(this.perfCounterInstance.NumberOfDocumentsDeleted);
					}
				}
				catch (Exception ex)
				{
					base.DiagnosticsSession.LogDiagnosticsInfo(DiagnosticsLoggingTag.Failures, "Document failure: ID: {0}, Failure: {1}", new object[]
					{
						docEntry.RawItemId,
						ex
					});
					base.CompleteExecute(ex);
					return;
				}
				if (disposable != null)
				{
					this.TryCollectDocumentsDelayed();
				}
			}
		}

		private void TryCollectDocumentsDelayed()
		{
			if (Interlocked.CompareExchange(ref this.collectingDocuments, 1, 0) == 0)
			{
				TimeSpan timeSpan = this.throttlingManager.DelayForThrottling();
				if (TimeSpan.Zero == timeSpan)
				{
					ThreadPool.QueueUserWorkItem(CallbackWrapper.WaitCallback(new WaitCallback(this.CollectDocuments)));
					return;
				}
				if (this.perfCounterInstance != null)
				{
					base.DiagnosticsSession.IncrementCounter(this.perfCounterInstance.SubmissionDelaysRetry);
					base.DiagnosticsSession.IncrementCounterBy(this.perfCounterInstance.DelayTimeRetry, (long)timeSpan.TotalMilliseconds);
				}
				try
				{
					RegisteredWaitHandleWrapper.RegisterWaitForSingleObject(base.StopEvent, CallbackWrapper.WaitOrTimerCallback(new WaitOrTimerCallback(this.CollectDocuments)), null, timeSpan, true);
				}
				catch (ObjectDisposedException result)
				{
					base.DiagnosticsSession.TraceError<MdbInfo>("RetryFeeder for Mdb: {0} has been disposed", this.mdbInfo);
					base.CompleteExecute(result);
				}
			}
		}

		private static Semaphore querySemaphore;

		private readonly ISubmitDocument indexFeeder;

		private readonly MdbInfo mdbInfo;

		private readonly LocalizedString exceptionOccurred;

		private readonly MdbPerfCountersInstance perfCounterInstance;

		private readonly IFailedItemStorage failedItemStorage;

		private readonly IIndexStatusStore indexStatusStore;

		private readonly IWatermarkStorage watermarkStorage;

		private readonly QueueManager<IDocEntry> documentQueueManager;

		private readonly RefCount activeThreadCount = new RefCount();

		private IEnumerator<IDocEntry> documentEnumerator;

		private IDocEntry currentDocument;

		private int collectingDocuments;

		private int retryItemsCount;

		private long failedItemsCount;

		private IFeederDelayThrottlingManager throttlingManager;
	}
}
