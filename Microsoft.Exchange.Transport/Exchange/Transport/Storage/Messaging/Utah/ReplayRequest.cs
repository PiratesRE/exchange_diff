using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using Microsoft.Exchange.Data.Directory.Management;
using Microsoft.Exchange.Diagnostics.Components.Transport;

namespace Microsoft.Exchange.Transport.Storage.Messaging.Utah
{
	internal class ReplayRequest : IReplayRequest, IDisposable
	{
		public ReplayRequest(MessagingDatabase database, ReplayRequestStorage storage)
		{
			if (database == null)
			{
				throw new ArgumentNullException("database");
			}
			if (storage == null)
			{
				throw new ArgumentNullException("storage");
			}
			this.database = database;
			this.Storage = storage;
			if (this.State != ResubmitRequestState.Completed)
			{
				this.database.SuspendDataCleanup(this.StartTime, this.EndTime);
			}
		}

		public long TotalReplayedMessages
		{
			get
			{
				return this.totalReplayedMessages;
			}
		}

		public long ContinuationToken
		{
			get
			{
				return this.Storage.ContinuationToken;
			}
			private set
			{
				this.Storage.ContinuationToken = value;
			}
		}

		public DateTime DateCreated
		{
			get
			{
				return this.Storage.DateCreated;
			}
		}

		public Destination Destination
		{
			get
			{
				return this.Storage.Destination;
			}
		}

		public string DiagnosticInformation
		{
			get
			{
				return this.Storage.DiagnosticInformation;
			}
			set
			{
				this.Storage.DiagnosticInformation = value;
			}
		}

		public DateTime EndTime
		{
			get
			{
				return this.Storage.EndTime;
			}
		}

		public long RequestId
		{
			get
			{
				return this.Storage.RequestId;
			}
		}

		public long PrimaryRequestId
		{
			get
			{
				return this.Storage.PrimaryRequestId;
			}
			set
			{
				this.Storage.PrimaryRequestId = value;
			}
		}

		public DateTime StartTime
		{
			get
			{
				return this.Storage.StartTime;
			}
		}

		public ResubmitRequestState State
		{
			get
			{
				return this.Storage.State;
			}
			set
			{
				if (this.Storage.State == value)
				{
					return;
				}
				if (this.Storage.State == ResubmitRequestState.Completed)
				{
					this.database.SuspendDataCleanup(this.StartTime, this.EndTime);
				}
				if (value == ResubmitRequestState.Completed)
				{
					this.database.ResumeDataCleanup(this.StartTime, this.EndTime);
				}
				this.Storage.State = value;
			}
		}

		public int OutstandingMailItemCount
		{
			get
			{
				return this.outstandingMailItemCount;
			}
		}

		public DateTime LastResubmittedMessageOrginalDeliveryTime
		{
			get
			{
				return this.lastResubmittedMessageOrginalDeliveryTime;
			}
		}

		public Guid CorrelationId
		{
			get
			{
				return this.Storage.CorrelationId;
			}
		}

		public bool IsTestRequest
		{
			get
			{
				return this.Storage.IsTestRequest;
			}
		}

		public IEnumerable<TransportMailItem> GetMessagesForRedelivery(int count)
		{
			int returnedCount = 0;
			string conditions = null;
			if (this.Destination.Type == Destination.DestinationType.Conditional)
			{
				try
				{
					conditions = this.Destination.ToString();
				}
				catch (Exception)
				{
					conditions = null;
				}
			}
			if (this.bookmarks == null)
			{
				if (this.Destination.Type == Destination.DestinationType.Conditional)
				{
					this.bookmarks = new ReplayRequest.ResumableCollection<Tuple<MessagingGeneration, IGrouping<int, int>>>(this.database.GetConditionalDeliveredBookmarks(this.StartTime, this.EndTime, this.Destination, this.ContinuationToken, conditions));
				}
				else
				{
					this.bookmarks = new ReplayRequest.ResumableCollection<Tuple<MessagingGeneration, IGrouping<int, int>>>(this.database.GetDeliveredBookmarks(this.Destination, this.StartTime, this.EndTime, this.ContinuationToken));
				}
				this.requestStopwatch.Reset();
			}
			this.requestStopwatch.Start();
			Stopwatch stopwatch = Stopwatch.StartNew();
			long continuationTokenFromSearch = this.ContinuationToken;
			foreach (MailItemAndRecipients mailItemAndRecipients in this.database.GetDeliveredMessages(this.bookmarks.ToIEnumerable(new int?(count)), ref continuationTokenFromSearch, conditions))
			{
				MailItemAndRecipients mailItemAndRecipients2 = mailItemAndRecipients;
				TransportMailItem transportMailItem = TransportMailItem.NewMailItem(mailItemAndRecipients2.MailItem, LatencyComponent.Replay);
				MailItemAndRecipients mailItemAndRecipients3 = mailItemAndRecipients;
				foreach (IMailRecipientStorage mailRecipientStorage in mailItemAndRecipients3.Recipients)
				{
					transportMailItem.AddRecipient(mailRecipientStorage);
					this.lastResubmittedMessageOrginalDeliveryTime = mailRecipientStorage.DeliveryTime.GetValueOrDefault();
				}
				this.ContinuationToken = transportMailItem.MsgId;
				returnedCount++;
				this.perfCounters.ReplayedItemCount.Increment();
				this.perfCounters.ReplayedItemAverageLatency.IncrementBy(stopwatch.ElapsedTicks);
				this.perfCounters.ReplayedItemAverageLatencyBase.Increment();
				this.requestStopwatch.Stop();
				stopwatch.Stop();
				yield return transportMailItem;
				this.requestStopwatch.Start();
				stopwatch.Restart();
			}
			if (returnedCount == 0)
			{
				this.ContinuationToken = continuationTokenFromSearch;
			}
			this.totalReplayedMessages += (long)returnedCount;
			this.requestStopwatch.Stop();
			if (this.bookmarks.Finished)
			{
				this.State = ResubmitRequestState.Completed;
				ExTraceGlobals.StorageTracer.TracePerformance(this.RequestId, "Request for {0}:{1} ({2}-{3}) has returned {4} messages and took {5} on db code", new object[]
				{
					this.Destination.Type,
					this.Destination,
					this.StartTime,
					this.EndTime,
					this.TotalReplayedMessages,
					this.requestStopwatch.Elapsed
				});
			}
			yield break;
		}

		public void AddMailItemReference()
		{
			Interlocked.Increment(ref this.outstandingMailItemCount);
		}

		public void ReleaseMailItemReference()
		{
			if (Interlocked.Decrement(ref this.outstandingMailItemCount) < 0)
			{
				throw new InvalidOperationException("Outstanding count cannot be negative");
			}
		}

		public virtual void Commit()
		{
			this.Storage.Commit();
		}

		public void Materialize(Transaction transaction)
		{
			this.Storage.Materialize(transaction);
		}

		public void Delete()
		{
			this.State = ResubmitRequestState.Completed;
			this.Storage.MarkToDelete();
			this.Commit();
		}

		public void Dispose()
		{
			if (this.bookmarks != null)
			{
				this.bookmarks.Dispose();
			}
		}

		public override string ToString()
		{
			return string.Format("From:{0} To:{1} Destination:{2} State:{3}", new object[]
			{
				this.StartTime,
				this.EndTime,
				this.Destination,
				this.State
			});
		}

		protected readonly ReplayRequestStorage Storage;

		private readonly Stopwatch requestStopwatch = new Stopwatch();

		private readonly MessagingDatabase database;

		private readonly DatabasePerfCountersInstance perfCounters = DatabasePerfCounters.GetInstance("other");

		private ReplayRequest.ResumableCollection<Tuple<MessagingGeneration, IGrouping<int, int>>> bookmarks;

		private long totalReplayedMessages;

		private int outstandingMailItemCount;

		private DateTime lastResubmittedMessageOrginalDeliveryTime;

		internal class ResumableCollection<T> : IDisposable
		{
			public bool Finished
			{
				get
				{
					return this.finished;
				}
			}

			public IEnumerator<T> Enumerator
			{
				get
				{
					return this.enumerator;
				}
			}

			public ResumableCollection(IEnumerable<T> collection)
			{
				this.enumerator = collection.GetEnumerator();
				this.finished = false;
			}

			public IEnumerable<T> ToIEnumerable(int? take = null)
			{
				int returnedCount = 0;
				while (take == null || returnedCount < take)
				{
					if (!this.enumerator.MoveNext())
					{
						this.finished = true;
						break;
					}
					yield return this.enumerator.Current;
					returnedCount++;
				}
				yield break;
			}

			public void Dispose()
			{
				this.enumerator.Dispose();
			}

			private readonly IEnumerator<T> enumerator;

			private bool finished;
		}
	}
}
