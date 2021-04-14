using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.Office.CompliancePolicy.PolicySync
{
	[Serializable]
	public sealed class SyncStatusUpdateWorkitem : WorkItemBase
	{
		public SyncStatusUpdateWorkitem(string externalIdentity, bool processNow, TenantContext tenantContext, IList<UnifiedPolicyStatus> statusUpdates, string statusUpdateSvcUrl, int maxBatchSize = 0) : this(externalIdentity, default(DateTime), processNow, tenantContext, statusUpdates, statusUpdateSvcUrl, maxBatchSize)
		{
		}

		internal SyncStatusUpdateWorkitem(string externalIdentity, DateTime executeTimeUtc, bool processNow, TenantContext tenantContext, IList<UnifiedPolicyStatus> statusUpdates, string statusUpdateSvcUrl, int maxBatchSize = 0) : base(externalIdentity, executeTimeUtc, processNow, tenantContext, false)
		{
			ArgumentValidator.ThrowIfCollectionNullOrEmpty<UnifiedPolicyStatus>("statusUpdates", statusUpdates);
			ArgumentValidator.ThrowIfNullOrEmpty("statusUpdateSvcUrl", statusUpdateSvcUrl);
			Guid tenantId = statusUpdates.First<UnifiedPolicyStatus>().TenantId;
			if (statusUpdates.Any((UnifiedPolicyStatus s) => s.TenantId != tenantId))
			{
				throw new ArgumentException("The collection must contain statuses for a single tenant", "statusUpdates");
			}
			this.StatusUpdateSvcUrl = statusUpdateSvcUrl;
			base.TryCount = 0;
			this.MaxBatchSize = maxBatchSize;
			this.statusUpdates = statusUpdates.ToList<UnifiedPolicyStatus>();
			SyncStatusUpdateWorkitem.SetStatusUpdateTenantIds(this.statusUpdates, tenantContext);
		}

		public IEnumerable<UnifiedPolicyStatus> StatusUpdates
		{
			get
			{
				return this.statusUpdates;
			}
		}

		public string StatusUpdateSvcUrl { get; private set; }

		public int MaxBatchSize { get; private set; }

		public override bool Merge(WorkItemBase newWorkItem)
		{
			ArgumentValidator.ThrowIfNull("newWorkItem", newWorkItem);
			ArgumentValidator.ThrowIfWrongType("newWorkItem", newWorkItem, typeof(SyncStatusUpdateWorkitem));
			SyncStatusUpdateWorkitem syncStatusUpdateWorkitem = (SyncStatusUpdateWorkitem)newWorkItem;
			foreach (UnifiedPolicyStatus unifiedPolicyStatus in syncStatusUpdateWorkitem.StatusUpdates)
			{
				int indexOfMatchingStatus = SyncStatusUpdateWorkitem.GetIndexOfMatchingStatus(this.statusUpdates, unifiedPolicyStatus);
				if (indexOfMatchingStatus >= 0)
				{
					if (this.statusUpdates[indexOfMatchingStatus].Version.CompareTo(unifiedPolicyStatus.Version) < 0)
					{
						this.statusUpdates[indexOfMatchingStatus] = unifiedPolicyStatus;
					}
				}
				else
				{
					this.statusUpdates.Add(unifiedPolicyStatus);
				}
			}
			if (base.ExecuteTimeUTC < newWorkItem.ExecuteTimeUTC)
			{
				base.ExecuteTimeUTC = newWorkItem.ExecuteTimeUTC;
			}
			return true;
		}

		public override bool IsEqual(WorkItemBase newWorkItem)
		{
			return this == newWorkItem;
		}

		public override Guid GetPrimaryKey()
		{
			return this.StatusUpdates.First<UnifiedPolicyStatus>().TenantId;
		}

		internal static int GetIndexOfMatchingStatus(IList<UnifiedPolicyStatus> statusCollection, UnifiedPolicyStatus newStatus)
		{
			for (int i = 0; i < statusCollection.Count; i++)
			{
				if (statusCollection[i].ObjectId == newStatus.ObjectId && statusCollection[i].ObjectType == newStatus.ObjectType)
				{
					return i;
				}
			}
			return -1;
		}

		internal static void SetStatusUpdateTenantIds(IList<UnifiedPolicyStatus> statusUpdates, TenantContext tenantContext)
		{
			foreach (UnifiedPolicyStatus unifiedPolicyStatus in from status in statusUpdates
			where status.TenantId == Guid.Empty
			select status)
			{
				unifiedPolicyStatus.TenantId = tenantContext.TenantId;
			}
		}

		internal bool IsOverLimit()
		{
			return this.MaxBatchSize != 0 && this.StatusUpdates.Count<UnifiedPolicyStatus>() > this.MaxBatchSize;
		}

		internal override WorkItemBase Split()
		{
			if (!this.IsOverLimit())
			{
				return null;
			}
			SyncStatusUpdateWorkitem result = new SyncStatusUpdateWorkitem(base.ExternalIdentity, base.ExecuteTimeUTC, base.ProcessNow, base.TenantContext, this.statusUpdates.GetRange(this.MaxBatchSize, this.statusUpdates.Count - this.MaxBatchSize), this.StatusUpdateSvcUrl, this.MaxBatchSize);
			this.statusUpdates = this.statusUpdates.GetRange(0, this.MaxBatchSize);
			return result;
		}

		internal override bool RoughCompare(object other)
		{
			SyncStatusUpdateWorkitem syncStatusUpdateWorkitem = other as SyncStatusUpdateWorkitem;
			if (syncStatusUpdateWorkitem == null)
			{
				return false;
			}
			if (this.statusUpdates.Count == syncStatusUpdateWorkitem.statusUpdates.Count && this.StatusUpdateSvcUrl.Equals(syncStatusUpdateWorkitem.StatusUpdateSvcUrl, StringComparison.OrdinalIgnoreCase))
			{
				Dictionary<Guid, UnifiedPolicyStatus> dictionary = new Dictionary<Guid, UnifiedPolicyStatus>();
				foreach (UnifiedPolicyStatus unifiedPolicyStatus in this.statusUpdates)
				{
					dictionary[unifiedPolicyStatus.ObjectId] = unifiedPolicyStatus;
				}
				foreach (UnifiedPolicyStatus unifiedPolicyStatus2 in syncStatusUpdateWorkitem.statusUpdates)
				{
					if (!dictionary.ContainsKey(unifiedPolicyStatus2.ObjectId))
					{
						return false;
					}
					UnifiedPolicyStatus unifiedPolicyStatus3 = dictionary[unifiedPolicyStatus2.ObjectId];
					if (unifiedPolicyStatus3.ErrorCode != unifiedPolicyStatus2.ErrorCode || unifiedPolicyStatus3.ObjectType != unifiedPolicyStatus2.ObjectType || !unifiedPolicyStatus3.Version.Equals(unifiedPolicyStatus2.Version) || unifiedPolicyStatus3.Workload != unifiedPolicyStatus2.Workload || !(unifiedPolicyStatus3.TenantId == unifiedPolicyStatus2.TenantId))
					{
						return false;
					}
				}
				return base.RoughCompare(syncStatusUpdateWorkitem);
			}
			return false;
		}

		public const int MaxBatchSizeNoLimit = 0;

		private List<UnifiedPolicyStatus> statusUpdates;
	}
}
