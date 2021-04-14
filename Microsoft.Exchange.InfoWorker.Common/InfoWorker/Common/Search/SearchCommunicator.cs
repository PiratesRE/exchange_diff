using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.InfoWorker.Common.Search
{
	internal class SearchCommunicator
	{
		internal SearchCommunicator()
		{
			this.progressEvent = new AutoResetEvent(false);
			this.abortEvent = new ManualResetEvent(false);
			this.workerExceptions = new List<Pair<int, Exception>>();
			this.workerLogs = new List<StreamLogItem.LogItem>();
			this.completedWorkers = new List<SearchMailboxWorker>();
			this.processedMessages = new HashSet<UniqueItemHash>();
			this.processedMessageIds = new HashSet<string>();
		}

		internal SearchCommunicator(HashSet<UniqueItemHash> processedMessages, HashSet<string> processedMessageIds)
		{
			this.progressEvent = new AutoResetEvent(false);
			this.abortEvent = new ManualResetEvent(false);
			this.workerExceptions = new List<Pair<int, Exception>>();
			this.workerLogs = new List<StreamLogItem.LogItem>();
			this.completedWorkers = new List<SearchMailboxWorker>();
			this.processedMessages = processedMessages;
			this.processedMessageIds = processedMessageIds;
		}

		internal bool IsAborted
		{
			get
			{
				return this.abortEvent.WaitOne(0, false);
			}
		}

		internal List<Pair<int, Exception>> WorkerExceptions
		{
			get
			{
				return this.workerExceptions;
			}
			set
			{
				this.workerExceptions = value;
			}
		}

		internal List<StreamLogItem.LogItem> WorkerLogs
		{
			get
			{
				return this.workerLogs;
			}
		}

		internal List<SearchMailboxWorker> CompletedWorkers
		{
			get
			{
				return this.completedWorkers;
			}
		}

		internal AutoResetEvent ProgressEvent
		{
			get
			{
				return this.progressEvent;
			}
		}

		internal ManualResetEvent AbortEvent
		{
			get
			{
				return this.abortEvent;
			}
		}

		internal double OverallProgress { get; set; }

		internal long OverallResultItems { get; set; }

		internal ByteQuantifiedSize OverallResultSize { get; set; }

		internal int ProgressingWorkers
		{
			get
			{
				return this.progressingWorkers;
			}
			set
			{
				this.progressingWorkers = value;
			}
		}

		internal HashSet<UniqueItemHash> ProcessedMessages
		{
			get
			{
				return this.processedMessages;
			}
		}

		internal HashSet<string> ProcessedMessageIds
		{
			get
			{
				return this.processedMessageIds;
			}
		}

		internal MultiValuedProperty<string> SuccessfulMailboxes { get; set; }

		internal MultiValuedProperty<string> UnsuccessfulMailboxes { get; set; }

		internal void ReportException(int senderId, Exception e)
		{
			this.WorkerExceptions.Add(new Pair<int, Exception>(senderId, e));
			this.ProgressEvent.Set();
		}

		internal void UpdateProgress(SearchMailboxWorker worker)
		{
			int workerId = worker.WorkerId;
			if (!worker.ExcludeDuplicateMessages)
			{
				int num = worker.SearchResult.ResultItemsCount - this.workerResultItems[workerId];
				this.workerResultItems[workerId] = worker.SearchResult.ResultItemsCount;
				this.OverallResultItems += (long)num;
				ByteQuantifiedSize value = worker.SearchResult.ResultItemsSize - this.workerResultSize[workerId];
				this.workerResultSize[workerId] = worker.SearchResult.ResultItemsSize;
				this.OverallResultSize += value;
			}
			else if (worker.TargetMailbox != null && worker.TargetSubFolderId != null)
			{
				this.UpdateResults(worker.TargetMailbox, worker.TargetSubFolderId);
			}
			double num2 = (worker.CurrentProgress - this.workerProgresses[workerId]) / (double)this.workerProgresses.Length;
			if (num2 > 0.0)
			{
				this.workerProgresses[workerId] = worker.CurrentProgress;
				this.OverallProgress += num2;
				if (this.OverallProgress > 100.0)
				{
					this.OverallProgress = 100.0;
				}
				this.ProgressEvent.Set();
			}
		}

		internal void UpdateResults(MailboxSession targetMailbox, StoreId targetFolder)
		{
			int num = 0;
			ByteQuantifiedSize zero = ByteQuantifiedSize.Zero;
			SearchUtils.GetFolderItemsCountAndSize(targetMailbox, targetFolder, out num, out zero);
			this.OverallResultItems = (long)num;
			this.OverallResultSize = zero;
		}

		internal void ResetWorker(SearchMailboxWorker worker, bool done)
		{
			int workerId = worker.WorkerId;
			this.OverallProgress -= this.workerProgresses[workerId] / (double)this.workerProgresses.Length;
			this.workerProgresses[workerId] = 0.0;
			if (this.OverallProgress < 0.0)
			{
				this.OverallProgress = 0.0;
			}
			if (!worker.ExcludeDuplicateMessages)
			{
				if (worker.SearchResult.ResultItemsCount == this.workerResultItems[workerId])
				{
					this.OverallResultItems -= (long)worker.SearchResult.ResultItemsCount;
				}
				worker.SearchResult.ResultItemsCount = 0;
				this.workerResultItems[workerId] = 0;
				if (worker.SearchResult.ResultItemsSize == this.workerResultSize[workerId])
				{
					this.OverallResultSize -= worker.SearchResult.ResultItemsSize;
				}
				worker.SearchResult.ResultItemsSize = ByteQuantifiedSize.Zero;
				this.workerResultSize[workerId] = ByteQuantifiedSize.Zero;
			}
			if (!done)
			{
				this.ProgressingWorkers++;
			}
			worker.LastException = null;
			worker.TargetMailbox = null;
			this.ProgressEvent.Set();
		}

		internal void ReportCompletion(SearchMailboxWorker worker)
		{
			this.ProgressingWorkers--;
			this.CompletedWorkers.Add(worker);
			this.ProgressEvent.Set();
		}

		internal void ReportLogs(StreamLogItem.LogItem logItem)
		{
			this.WorkerLogs.Add(logItem);
			int totalLogEntries = 0;
			this.WorkerLogs.ForEach(delegate(StreamLogItem.LogItem x)
			{
				totalLogEntries += x.Logs.Count<LocalizedString>();
			});
			if (totalLogEntries > 1000)
			{
				this.ProgressEvent.Set();
			}
		}

		internal void Abort()
		{
			this.AbortEvent.Set();
		}

		internal void Reset(int workers)
		{
			this.ProgressingWorkers = workers;
			this.workerProgresses = new double[workers];
			this.workerResultItems = new int[workers];
			this.workerResultSize = new ByteQuantifiedSize[workers];
			for (int i = 0; i < this.workerProgresses.Length; i++)
			{
				this.workerProgresses[i] = 0.0;
				this.workerResultItems[i] = 0;
				this.workerResultSize[i] = ByteQuantifiedSize.Zero;
			}
			this.OverallProgress = 0.0;
			this.OverallResultItems = 0L;
			this.OverallResultSize = ByteQuantifiedSize.Zero;
			this.ProgressEvent.Reset();
			this.WorkerLogs.Clear();
			this.WorkerExceptions.Clear();
			this.CompletedWorkers.Clear();
			this.ProcessedMessages.Clear();
			this.processedMessageIds.Clear();
		}

		private const int LogBufferCapacity = 1000;

		private double[] workerProgresses;

		private int[] workerResultItems;

		private ByteQuantifiedSize[] workerResultSize;

		private int progressingWorkers;

		private AutoResetEvent progressEvent;

		private ManualResetEvent abortEvent;

		private List<Pair<int, Exception>> workerExceptions;

		private List<StreamLogItem.LogItem> workerLogs;

		private List<SearchMailboxWorker> completedWorkers;

		private HashSet<UniqueItemHash> processedMessages;

		private HashSet<string> processedMessageIds;
	}
}
