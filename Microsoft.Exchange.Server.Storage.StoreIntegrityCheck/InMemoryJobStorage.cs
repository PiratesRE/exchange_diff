using System;
using System.Collections.Generic;
using Microsoft.Exchange.Server.Storage.Common;
using Microsoft.Exchange.Server.Storage.StoreCommonServices;

namespace Microsoft.Exchange.Server.Storage.StoreIntegrityCheck
{
	public class InMemoryJobStorage : IJobStorage
	{
		internal InMemoryJobStorage(int initialCapacity, StorePerDatabasePerformanceCountersInstance perfCounters)
		{
			this.capacity = initialCapacity;
			this.jobIdToJobMap = new Dictionary<Guid, IntegrityCheckJob>(initialCapacity);
			this.mailboxGuidToJobIndex = new Dictionary<Guid, HashSet<Guid>>(initialCapacity);
			this.requestGuidToJobIndex = new Dictionary<Guid, HashSet<Guid>>(initialCapacity);
			this.perfCounters = perfCounters;
		}

		public bool IsEmpty
		{
			get
			{
				return this.jobIdToJobMap.Count == 0;
			}
		}

		public bool IsFull
		{
			get
			{
				return this.jobIdToJobMap.Count > this.capacity;
			}
		}

		public static InMemoryJobStorage Instance(StoreDatabase database)
		{
			return database.ComponentData[InMemoryJobStorage.jobStorageSlot] as InMemoryJobStorage;
		}

		public static IEnumerable<IntegrityCheckJob> GetRequestQueueSnapshot(Context context)
		{
			return InMemoryJobStorage.Instance(context.Database).GetAllJobs();
		}

		public void AddJob(IntegrityCheckJob job)
		{
			using (LockManager.Lock(this))
			{
				this.jobIdToJobMap.Add(job.JobGuid, job);
				HashSet<Guid> hashSet;
				if (this.mailboxGuidToJobIndex.TryGetValue(job.MailboxGuid, out hashSet) && hashSet != null)
				{
					hashSet.Add(job.JobGuid);
				}
				else
				{
					hashSet = new HashSet<Guid>();
					hashSet.Add(job.JobGuid);
					this.mailboxGuidToJobIndex[job.MailboxGuid] = hashSet;
				}
				HashSet<Guid> hashSet2;
				if (this.requestGuidToJobIndex.TryGetValue(job.RequestGuid, out hashSet2) && hashSet2 != null)
				{
					hashSet2.Add(job.JobGuid);
				}
				else
				{
					hashSet2 = new HashSet<Guid>();
					hashSet2.Add(job.JobGuid);
					this.requestGuidToJobIndex[job.RequestGuid] = hashSet2;
				}
				this.perfCounters.ISIntegStoreTotalJobs.Increment();
			}
		}

		public void RemoveJob(Guid jobGuid)
		{
			using (LockManager.Lock(this))
			{
				IntegrityCheckJob integrityCheckJob;
				if (this.jobIdToJobMap.TryGetValue(jobGuid, out integrityCheckJob))
				{
					this.jobIdToJobMap.Remove(jobGuid);
					HashSet<Guid> hashSet;
					if (this.mailboxGuidToJobIndex.TryGetValue(integrityCheckJob.MailboxGuid, out hashSet) && hashSet != null)
					{
						hashSet.Remove(jobGuid);
					}
					HashSet<Guid> hashSet2;
					if (this.requestGuidToJobIndex.TryGetValue(integrityCheckJob.RequestGuid, out hashSet2) && hashSet2 != null)
					{
						hashSet2.Remove(jobGuid);
					}
					this.perfCounters.ISIntegStoreTotalJobs.Decrement();
				}
			}
		}

		public IntegrityCheckJob GetJob(Guid jobGuid)
		{
			IntegrityCheckJob result;
			using (LockManager.Lock(this))
			{
				IntegrityCheckJob integrityCheckJob = null;
				if (this.jobIdToJobMap.TryGetValue(jobGuid, out integrityCheckJob))
				{
					result = integrityCheckJob;
				}
				else
				{
					result = null;
				}
			}
			return result;
		}

		public IEnumerable<IntegrityCheckJob> GetJobsByRequestGuid(Guid requestGuid)
		{
			List<IntegrityCheckJob> list = null;
			using (LockManager.Lock(this))
			{
				HashSet<Guid> hashSet;
				if (this.requestGuidToJobIndex.TryGetValue(requestGuid, out hashSet) && hashSet != null)
				{
					list = new List<IntegrityCheckJob>(hashSet.Count);
					foreach (Guid key in hashSet)
					{
						list.Add(this.jobIdToJobMap[key]);
					}
				}
			}
			return list;
		}

		public IEnumerable<IntegrityCheckJob> GetJobsByMailboxGuid(Guid mailboxGuid)
		{
			List<IntegrityCheckJob> list = null;
			using (LockManager.Lock(this))
			{
				HashSet<Guid> hashSet;
				if (this.mailboxGuidToJobIndex.TryGetValue(mailboxGuid, out hashSet) && hashSet != null)
				{
					list = new List<IntegrityCheckJob>(hashSet.Count);
					foreach (Guid key in hashSet)
					{
						list.Add(this.jobIdToJobMap[key]);
					}
				}
			}
			return list;
		}

		public IEnumerable<IntegrityCheckJob> GetAllJobs()
		{
			List<IntegrityCheckJob> result = null;
			using (LockManager.Lock(this))
			{
				result = new List<IntegrityCheckJob>(this.jobIdToJobMap.Values);
			}
			return result;
		}

		internal static void Initialize()
		{
			if (InMemoryJobStorage.jobStorageSlot == -1)
			{
				InMemoryJobStorage.jobStorageSlot = StoreDatabase.AllocateComponentDataSlot();
			}
		}

		internal static void MountEventHandler(Context context, StoreDatabase database)
		{
			database.ComponentData[InMemoryJobStorage.jobStorageSlot] = new InMemoryJobStorage(ConfigurationSchema.IntegrityCheckJobStorageCapacity.Value, PerformanceCounterFactory.GetDatabaseInstance(database));
		}

		internal static void DismountEventHandler(StoreDatabase database)
		{
			database.ComponentData[InMemoryJobStorage.jobStorageSlot] = null;
		}

		private static int jobStorageSlot = -1;

		private readonly int capacity;

		private readonly StorePerDatabasePerformanceCountersInstance perfCounters;

		private Dictionary<Guid, IntegrityCheckJob> jobIdToJobMap;

		private Dictionary<Guid, HashSet<Guid>> mailboxGuidToJobIndex;

		private Dictionary<Guid, HashSet<Guid>> requestGuidToJobIndex;
	}
}
