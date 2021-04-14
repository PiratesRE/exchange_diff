using System;
using System.Collections.Generic;
using System.Threading;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Server.Storage.Common;

namespace Microsoft.Exchange.Server.Storage.StoreCommonServices
{
	public sealed class ResourceMonitorDigest
	{
		public ResourceMonitorDigest()
		{
			this.activityLog = ResourceMonitorDigest.ActivityLog.GetEmpty(1L, 0);
			this.aggregatingLogBytesLog = new ResourceMonitorDigest.AggregatingActivityLog(10);
			this.timeInServerHistory = new ResourceMonitorDigest.DigestHistory();
			this.logRecodBytesHistory = new ResourceMonitorDigest.DigestHistory();
		}

		public static IDigestCollector NullCollector
		{
			get
			{
				if (ResourceMonitorDigest.nullCollector == null)
				{
					ResourceMonitorDigest.nullCollector = new ResourceMonitorDigest.NullDigestCollector();
				}
				return ResourceMonitorDigest.nullCollector;
			}
		}

		public IDigestCollector CreateDigestCollector(Guid mailboxGuid, int mailboxNumber)
		{
			if (mailboxGuid.Equals(Guid.Empty))
			{
				return ResourceMonitorDigest.NullCollector;
			}
			return new ResourceMonitorDigest.DigestCollector(this, mailboxGuid, mailboxNumber);
		}

		public ResourceMonitorDigestSnapshot GetDigestHistory()
		{
			return new ResourceMonitorDigestSnapshot
			{
				TimeInServerDigest = this.timeInServerHistory.GetHistory(),
				LogRecordBytesDigest = this.logRecodBytesHistory.GetHistory()
			};
		}

		internal static void MountEventHandler(StoreDatabase database)
		{
			Task<ResourceMonitorDigest>.TaskCallback callback = TaskExecutionWrapper<ResourceMonitorDigest>.WrapExecute(new TaskDiagnosticInformation(TaskTypeId.ResourceMonitorDigest, ClientType.System, database.MdbGuid), delegate(Context _1, ResourceMonitorDigest digest, Func<bool> _2)
			{
				digest.TakeSnapshot();
			});
			RecurringTask<ResourceMonitorDigest> task = new RecurringTask<ResourceMonitorDigest>(callback, database.ResourceDigest, TimeSpan.FromMinutes(1.0), false);
			database.TaskList.Add(task, true);
		}

		private void TakeSnapshot()
		{
			ResourceMonitorDigest.ActivityLog activityLog;
			using (LockManager.Lock(this.syncRoot))
			{
				activityLog = this.GetActivityLog();
				int num = activityLog.CurrentActivity.Array.Length;
				if (activityLog.CurrentActivity.Count < num / 4)
				{
					num /= 2;
				}
				ResourceMonitorDigest.ActivityLog empty = ResourceMonitorDigest.ActivityLog.GetEmpty(activityLog.Version + 1L, num);
				Interlocked.Exchange<ResourceMonitorDigest.ActivityLog>(ref this.activityLog, empty);
			}
			DateTime utcNow = DateTime.UtcNow;
			ResourceMonitorDigest.Digest digest = new ResourceMonitorDigest.Digest(25, (ResourceDigestStats left, ResourceDigestStats right) => left.TimeInServer.CompareTo(right.TimeInServer));
			for (int i = 0; i < activityLog.CurrentActivity.Count; i++)
			{
				ResourceDigestStats data = activityLog.CurrentActivity.Array[i].Data;
				data.SampleTime = utcNow;
				digest.Update(data);
				this.aggregatingLogBytesLog.AggregateSample(data);
			}
			ResourceMonitorDigest.Digest digest2;
			if (this.aggregatingLogBytesLog.TryComputeDigestAndReset(25, (ResourceDigestStats left, ResourceDigestStats right) => left.LogRecordBytes.CompareTo(right.LogRecordBytes), out digest2))
			{
				this.logRecodBytesHistory.AddDigest(digest2);
			}
			this.timeInServerHistory.AddDigest(digest);
		}

		private ResourceMonitorDigest.ActivityLog GetActivityLog()
		{
			return Interlocked.CompareExchange<ResourceMonitorDigest.ActivityLog>(ref this.activityLog, null, null);
		}

		private void LogActivity(ResourceDigestStats activity, ResourceMonitorDigest.DigestCollector collector)
		{
			ResourceMonitorDigest.ActivityLog activityLog = this.GetActivityLog();
			ResourceMonitorDigest.ActivityHandle activityHandle = collector.Handle;
			if (activityLog.Version != activityHandle.Version || activityHandle.Slot >= activityLog.CurrentActivity.Array.Length)
			{
				using (LockManager.Lock(this.syncRoot))
				{
					activityLog = this.GetActivityLog();
					activityHandle = collector.Handle;
					if (activityLog.Version != activityHandle.Version)
					{
						int num;
						ResourceMonitorDigest.ActivityLog activityLog2 = activityLog.ReserveSlot(out num);
						activityLog2.CurrentActivity.Array[num] = new ResourceMonitorDigest.Activity(collector.MailboxGuid, collector.MailboxNumber);
						activityHandle = new ResourceMonitorDigest.ActivityHandle(activityLog2.Version, num);
						Interlocked.Exchange<ResourceMonitorDigest.ActivityLog>(ref this.activityLog, activityLog2);
						collector.Handle = activityHandle;
						activityLog = activityLog2;
					}
				}
			}
			activityLog.CurrentActivity.Array[activityHandle.Slot].Aggregate(activity);
		}

		public const int DigestCapacity = 25;

		public const int MaximumSnapshots = 10;

		public const int LogRecordBytesLogAggregate = 10;

		private static IDigestCollector nullCollector;

		private object syncRoot = new object();

		private ResourceMonitorDigest.ActivityLog activityLog;

		private ResourceMonitorDigest.AggregatingActivityLog aggregatingLogBytesLog;

		private ResourceMonitorDigest.DigestHistory timeInServerHistory;

		private ResourceMonitorDigest.DigestHistory logRecodBytesHistory;

		private class Activity
		{
			public Activity(Guid mailboxGuid, int mailboxNumber)
			{
				this.activity = default(ResourceDigestStats);
				this.activity.MailboxGuid = mailboxGuid;
				this.activity.MailboxNumber = mailboxNumber;
			}

			public ResourceDigestStats Data
			{
				get
				{
					ResourceDigestStats result;
					using (LockManager.Lock(this))
					{
						result = this.activity;
					}
					return result;
				}
			}

			public void Aggregate(ResourceDigestStats activity)
			{
				using (LockManager.Lock(this))
				{
					this.activity.TimeInServer = this.activity.TimeInServer + activity.TimeInServer;
					this.activity.TimeInCPU = this.activity.TimeInCPU + activity.TimeInCPU;
					this.activity.ROPCount = this.activity.ROPCount + activity.ROPCount;
					this.activity.PageRead = this.activity.PageRead + activity.PageRead;
					this.activity.PagePreread = this.activity.PagePreread + activity.PagePreread;
					this.activity.LogRecordCount = this.activity.LogRecordCount + activity.LogRecordCount;
					this.activity.LogRecordBytes = this.activity.LogRecordBytes + activity.LogRecordBytes;
					this.activity.LdapReads = this.activity.LdapReads + activity.LdapReads;
					this.activity.LdapSearches = this.activity.LdapSearches + activity.LdapSearches;
				}
			}

			private ResourceDigestStats activity;
		}

		private class ActivityLog
		{
			private ActivityLog(long version, ArraySegment<ResourceMonitorDigest.Activity> activityLog)
			{
				this.Version = version;
				this.CurrentActivity = activityLog;
			}

			public long Version { get; private set; }

			public ArraySegment<ResourceMonitorDigest.Activity> CurrentActivity { get; private set; }

			public static ResourceMonitorDigest.ActivityLog GetEmpty(long version, int initialCapacity)
			{
				if (initialCapacity < 16)
				{
					initialCapacity = 16;
				}
				return new ResourceMonitorDigest.ActivityLog(version, new ArraySegment<ResourceMonitorDigest.Activity>(new ResourceMonitorDigest.Activity[initialCapacity], 0, 0));
			}

			public ResourceMonitorDigest.ActivityLog ReserveSlot(out int dataSlot)
			{
				if (this.CurrentActivity.Count < this.CurrentActivity.Array.Length)
				{
					dataSlot = this.CurrentActivity.Count;
					return new ResourceMonitorDigest.ActivityLog(this.Version, new ArraySegment<ResourceMonitorDigest.Activity>(this.CurrentActivity.Array, 0, this.CurrentActivity.Count + 1));
				}
				ResourceMonitorDigest.Activity[] array = new ResourceMonitorDigest.Activity[this.CurrentActivity.Array.Length * 2];
				Array.Copy(this.CurrentActivity.Array, 0, array, 0, this.CurrentActivity.Array.Length);
				dataSlot = this.CurrentActivity.Count;
				return new ResourceMonitorDigest.ActivityLog(this.Version, new ArraySegment<ResourceMonitorDigest.Activity>(array, 0, this.CurrentActivity.Count + 1));
			}

			private const int MimimalCapacity = 16;
		}

		private class AggregatingActivityLog
		{
			public AggregatingActivityLog(int maximumLogsToAggregate)
			{
				this.maximumAggregateCount = maximumLogsToAggregate;
				this.aggregateCount = 0;
				this.aggregatedLog = new Dictionary<Guid, ResourceDigestStats>();
			}

			public void AggregateSample(ResourceDigestStats sample)
			{
				using (LockManager.Lock(this))
				{
					if (this.aggregatedLog.ContainsKey(sample.MailboxGuid))
					{
						ResourceDigestStats value = this.aggregatedLog[sample.MailboxGuid];
						value.TimeInServer += sample.TimeInServer;
						value.TimeInCPU += sample.TimeInCPU;
						value.ROPCount += sample.ROPCount;
						value.PageRead += sample.PageRead;
						value.PagePreread += sample.PagePreread;
						value.LogRecordCount += sample.LogRecordCount;
						value.LogRecordBytes += sample.LogRecordBytes;
						value.LdapReads += sample.LdapReads;
						value.LdapSearches += sample.LdapSearches;
						value.SampleTime = sample.SampleTime;
						this.aggregatedLog[sample.MailboxGuid] = value;
					}
					else
					{
						this.aggregatedLog[sample.MailboxGuid] = sample;
					}
				}
			}

			public bool TryComputeDigestAndReset(int capacity, Func<ResourceDigestStats, ResourceDigestStats, int> comparator, out ResourceMonitorDigest.Digest digest)
			{
				digest = null;
				bool result;
				using (LockManager.Lock(this))
				{
					this.aggregateCount++;
					if (this.aggregateCount >= this.maximumAggregateCount)
					{
						digest = new ResourceMonitorDigest.Digest(capacity, comparator);
						foreach (ResourceDigestStats stats in this.aggregatedLog.Values)
						{
							digest.Update(stats);
						}
						this.aggregateCount = 0;
						this.aggregatedLog.Clear();
						result = true;
					}
					else
					{
						result = false;
					}
				}
				return result;
			}

			private readonly int maximumAggregateCount;

			private Dictionary<Guid, ResourceDigestStats> aggregatedLog;

			private int aggregateCount;
		}

		private class Digest
		{
			public Digest(int capacity, Func<ResourceDigestStats, ResourceDigestStats, int> comparator)
			{
				this.capacity = capacity;
				this.count = 0;
				this.data = new ResourceDigestStats[capacity];
				this.comparator = comparator;
				this.isHeapBuilt = false;
			}

			public ResourceDigestStats[] GetSamples()
			{
				if (this.count == 0)
				{
					return Array<ResourceDigestStats>.Empty;
				}
				ResourceDigestStats[] array = new ResourceDigestStats[this.count];
				Array.Copy(this.data, 0, array, 0, this.count);
				return array;
			}

			public void Update(ResourceDigestStats stats)
			{
				if (this.count < this.capacity)
				{
					this.data[this.count] = stats;
					this.count++;
					return;
				}
				if (!this.isHeapBuilt)
				{
					for (int i = this.Parent(this.count - 1); i >= 0; i--)
					{
						this.PushToHeap(this.data[i], i);
					}
					this.isHeapBuilt = true;
				}
				int num = this.comparator(stats, this.data[0]);
				if (num > 0)
				{
					this.PushToHeap(stats, 0);
				}
			}

			private void PushToHeap(ResourceDigestStats stats, int root)
			{
				int i;
				int num4;
				for (i = root; i < this.count; i = num4)
				{
					int num = this.Left(i);
					int num2 = this.Right(i);
					int num3;
					if (num2 > 0)
					{
						num3 = this.comparator(this.data[num], this.data[num2]);
						num4 = ((num3 < 0) ? num : num2);
					}
					else
					{
						if (num <= 0)
						{
							break;
						}
						num4 = num;
					}
					num3 = this.comparator(this.data[num4], stats);
					if (num3 >= 0)
					{
						break;
					}
					this.data[i] = this.data[num4];
				}
				this.data[i] = stats;
			}

			private int Left(int parent)
			{
				int num = 2 * parent + 1;
				if (num < 0 || num >= this.count)
				{
					return -1;
				}
				return num;
			}

			private int Right(int parent)
			{
				int num = 2 * parent + 2;
				if (num < 0 || num >= this.count)
				{
					return -1;
				}
				return num;
			}

			private int Parent(int current)
			{
				if (current <= 0)
				{
					return -1;
				}
				return (current - 1) / 2;
			}

			private readonly int capacity;

			private int count;

			private ResourceDigestStats[] data;

			private Func<ResourceDigestStats, ResourceDigestStats, int> comparator;

			private bool isHeapBuilt;
		}

		private class DigestHistory
		{
			public DigestHistory()
			{
				this.history = new ResourceMonitorDigest.Digest[10];
				this.nextSnapshot = 0;
			}

			public void AddDigest(ResourceMonitorDigest.Digest digest)
			{
				using (LockManager.Lock(this))
				{
					this.history[this.nextSnapshot] = digest;
					this.nextSnapshot = (this.nextSnapshot + 1) % this.history.Length;
				}
			}

			public ResourceDigestStats[][] GetHistory()
			{
				ResourceDigestStats[][] result;
				using (LockManager.Lock(this))
				{
					int num = this.history.Length;
					for (int i = 0; i < this.history.Length; i++)
					{
						if (this.history[i] == null)
						{
							num--;
						}
					}
					if (num == 0)
					{
						result = Array<ResourceDigestStats[]>.Empty;
					}
					else
					{
						ResourceDigestStats[][] array = new ResourceDigestStats[num][];
						int num2 = 0;
						for (int j = 0; j < this.history.Length; j++)
						{
							int num3 = (2 * this.history.Length + this.nextSnapshot - 1 - j) % this.history.Length;
							if (this.history[num3] != null)
							{
								array[num2] = this.history[num3].GetSamples();
								num2++;
							}
						}
						result = array;
					}
				}
				return result;
			}

			private ResourceMonitorDigest.Digest[] history;

			private int nextSnapshot;
		}

		private class ActivityHandle
		{
			public ActivityHandle(long version, int dataSlot)
			{
				this.Version = version;
				this.Slot = dataSlot;
			}

			public long Version { get; private set; }

			public int Slot { get; private set; }
		}

		private class DigestCollector : IDigestCollector
		{
			public DigestCollector(ResourceMonitorDigest monitor, Guid mailboxGuid, int mailboxNumber)
			{
				this.monitor = monitor;
				this.mailboxGuid = mailboxGuid;
				this.mailboxNumber = mailboxNumber;
				this.activityHandle = new ResourceMonitorDigest.ActivityHandle(-1L, -1);
			}

			public Guid MailboxGuid
			{
				get
				{
					return this.mailboxGuid;
				}
			}

			public int MailboxNumber
			{
				get
				{
					return this.mailboxNumber;
				}
			}

			public ResourceMonitorDigest.ActivityHandle Handle
			{
				get
				{
					return Interlocked.CompareExchange<ResourceMonitorDigest.ActivityHandle>(ref this.activityHandle, null, null);
				}
				set
				{
					Interlocked.Exchange<ResourceMonitorDigest.ActivityHandle>(ref this.activityHandle, value);
				}
			}

			public void LogActivity(ResourceDigestStats activity)
			{
				this.monitor.LogActivity(activity, this);
			}

			private readonly Guid mailboxGuid;

			private readonly int mailboxNumber;

			private ResourceMonitorDigest monitor;

			private ResourceMonitorDigest.ActivityHandle activityHandle;
		}

		private class NullDigestCollector : IDigestCollector
		{
			public void LogActivity(ResourceDigestStats activity)
			{
			}
		}
	}
}
