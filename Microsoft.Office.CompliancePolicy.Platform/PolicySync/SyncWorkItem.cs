using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Office.CompliancePolicy.PolicyConfiguration;

namespace Microsoft.Office.CompliancePolicy.PolicySync
{
	[Serializable]
	public sealed class SyncWorkItem : WorkItemBase
	{
		public SyncWorkItem(string externalIdentity, bool processNow, TenantContext tenantContext, SyncChangeInfo[] changeInfoList, string syncSvcUrl, bool fullSyncForTenant, Workload workload, bool hasPersistentBackup = false) : this(externalIdentity, default(DateTime), processNow, tenantContext, changeInfoList, syncSvcUrl, SyncWorkItem.DefaultMaxExecuteDelayTime, fullSyncForTenant, workload, hasPersistentBackup)
		{
		}

		internal SyncWorkItem(string externalIdentity, DateTime executeTimeUTC, bool processNow, TenantContext tenantContext, SyncChangeInfo[] changeInfoList, string syncSvcUrl, TimeSpan maxExecuteDelayTime, bool fullSyncForTenant, Workload workload, bool hasPersistentBackup = false) : base(externalIdentity, executeTimeUTC, processNow, tenantContext, hasPersistentBackup)
		{
			ArgumentValidator.ThrowIfNullOrEmpty("externalIdentity", externalIdentity);
			ArgumentValidator.ThrowIfNullOrEmpty("syncSvcUrl", syncSvcUrl);
			ArgumentValidator.ThrowIfNegativeTimeSpan("maxExecuteDelayTime", maxExecuteDelayTime);
			base.ExternalIdentity = externalIdentity;
			this.syncSvcUrl = syncSvcUrl;
			this.maxExecuteDelayTime = maxExecuteDelayTime;
			this.FirstChangeArriveUTC = DateTime.UtcNow;
			this.LastChangeArriveUTC = this.FirstChangeArriveUTC;
			this.FullSyncForTenant = fullSyncForTenant;
			this.Workload = workload;
			Dictionary<ConfigurationObjectType, List<SyncChangeInfo>> dictionary = null;
			if (changeInfoList != null && changeInfoList.Any<SyncChangeInfo>())
			{
				bool flag = changeInfoList.Any((SyncChangeInfo p) => p.ObjectId != null);
				if (flag && fullSyncForTenant)
				{
					throw new ArgumentException("The changeInfoList contains object-level sync. But the sync type is tenant-level or type-level full sync.");
				}
				dictionary = new Dictionary<ConfigurationObjectType, List<SyncChangeInfo>>();
				foreach (SyncChangeInfo syncChangeInfo in changeInfoList)
				{
					if (dictionary.ContainsKey(syncChangeInfo.ObjectType))
					{
						dictionary[syncChangeInfo.ObjectType].Add(syncChangeInfo);
					}
					else
					{
						dictionary.Add(syncChangeInfo.ObjectType, new List<SyncChangeInfo>
						{
							syncChangeInfo
						});
					}
				}
			}
			if (dictionary == null || !dictionary.Any<KeyValuePair<ConfigurationObjectType, List<SyncChangeInfo>>>())
			{
				this.WorkItemInfo = new Dictionary<ConfigurationObjectType, List<SyncChangeInfo>>
				{
					{
						ConfigurationObjectType.Policy,
						new List<SyncChangeInfo>
						{
							new SyncChangeInfo(ConfigurationObjectType.Policy)
						}
					},
					{
						ConfigurationObjectType.Rule,
						new List<SyncChangeInfo>
						{
							new SyncChangeInfo(ConfigurationObjectType.Rule)
						}
					},
					{
						ConfigurationObjectType.Binding,
						new List<SyncChangeInfo>
						{
							new SyncChangeInfo(ConfigurationObjectType.Binding)
						}
					},
					{
						ConfigurationObjectType.Association,
						new List<SyncChangeInfo>
						{
							new SyncChangeInfo(ConfigurationObjectType.Association)
						}
					}
				};
				return;
			}
			this.WorkItemInfo = dictionary;
		}

		public Dictionary<ConfigurationObjectType, List<SyncChangeInfo>> WorkItemInfo { get; internal set; }

		public string SyncSvcUrl
		{
			get
			{
				return this.syncSvcUrl;
			}
			internal set
			{
				ArgumentValidator.ThrowIfNullOrEmpty("SyncSvcUrl", value);
				this.syncSvcUrl = value;
			}
		}

		public bool FullSyncForTenant { get; internal set; }

		public Workload Workload { get; internal set; }

		internal DateTime FirstChangeArriveUTC { get; set; }

		internal DateTime LastChangeArriveUTC { get; set; }

		public override bool Merge(WorkItemBase newWorkItem)
		{
			ArgumentValidator.ThrowIfNull("newWorkItem", newWorkItem);
			ArgumentValidator.ThrowIfWrongType("newWorkItem", newWorkItem, typeof(SyncWorkItem));
			SyncWorkItem syncWorkItem = (SyncWorkItem)newWorkItem;
			if (this.FullSyncForTenant)
			{
				return syncWorkItem.FullSyncForTenant;
			}
			if (syncWorkItem.FullSyncForTenant)
			{
				this.WorkItemInfo = syncWorkItem.WorkItemInfo;
				this.FullSyncForTenant = true;
				return true;
			}
			foreach (KeyValuePair<ConfigurationObjectType, List<SyncChangeInfo>> keyValuePair in syncWorkItem.WorkItemInfo)
			{
				ConfigurationObjectType key = keyValuePair.Key;
				List<SyncChangeInfo> value = keyValuePair.Value;
				if (this.WorkItemInfo.ContainsKey(key))
				{
					List<SyncChangeInfo> source = this.WorkItemInfo[key];
					if (value.First<SyncChangeInfo>().ObjectId != null != (source.First<SyncChangeInfo>().ObjectId != null))
					{
						return false;
					}
				}
			}
			foreach (KeyValuePair<ConfigurationObjectType, List<SyncChangeInfo>> keyValuePair2 in syncWorkItem.WorkItemInfo)
			{
				ConfigurationObjectType key2 = keyValuePair2.Key;
				List<SyncChangeInfo> value2 = keyValuePair2.Value;
				if (!this.WorkItemInfo.ContainsKey(key2))
				{
					this.WorkItemInfo.Add(key2, value2);
				}
				else
				{
					List<SyncChangeInfo> list = this.WorkItemInfo[key2];
					using (List<SyncChangeInfo>.Enumerator enumerator3 = value2.GetEnumerator())
					{
						while (enumerator3.MoveNext())
						{
							SyncChangeInfo newChangeInfo = enumerator3.Current;
							SyncChangeInfo syncChangeInfo = list.FirstOrDefault((SyncChangeInfo p) => p.ObjectId == newChangeInfo.ObjectId);
							if (syncChangeInfo != null)
							{
								if ((null != syncChangeInfo.Version && null != newChangeInfo.Version && syncChangeInfo.Version.CompareTo(newChangeInfo.Version) < 0) || (newChangeInfo.ObjectId != null && ChangeType.Delete == newChangeInfo.ChangeType))
								{
									syncChangeInfo.Version = newChangeInfo.Version;
									syncChangeInfo.ChangeType = newChangeInfo.ChangeType;
								}
							}
							else
							{
								list.Add(newChangeInfo);
							}
						}
					}
				}
			}
			base.TryCount = ((base.TryCount < syncWorkItem.TryCount) ? base.TryCount : syncWorkItem.TryCount);
			base.ProcessNow |= syncWorkItem.ProcessNow;
			if (this.LastChangeArriveUTC < syncWorkItem.LastChangeArriveUTC)
			{
				this.LastChangeArriveUTC = syncWorkItem.LastChangeArriveUTC;
				base.TenantContext = syncWorkItem.TenantContext;
				this.SyncSvcUrl = syncWorkItem.SyncSvcUrl;
			}
			if (this.FirstChangeArriveUTC > syncWorkItem.FirstChangeArriveUTC)
			{
				this.FirstChangeArriveUTC = syncWorkItem.FirstChangeArriveUTC;
			}
			if (this.LastChangeArriveUTC - this.FirstChangeArriveUTC < this.maxExecuteDelayTime && base.ExecuteTimeUTC < syncWorkItem.ExecuteTimeUTC)
			{
				base.ExecuteTimeUTC = syncWorkItem.ExecuteTimeUTC;
			}
			return true;
		}

		public override bool IsEqual(WorkItemBase newWorkItem)
		{
			return this == newWorkItem;
		}

		public override Guid GetPrimaryKey()
		{
			return base.TenantContext.TenantId;
		}

		internal override bool RoughCompare(object other)
		{
			SyncWorkItem syncWorkItem = other as SyncWorkItem;
			return syncWorkItem != null && (this.FullSyncForTenant == syncWorkItem.FullSyncForTenant && this.SyncSvcUrl.Equals(syncWorkItem.SyncSvcUrl, StringComparison.OrdinalIgnoreCase)) && base.RoughCompare(syncWorkItem);
		}

		private static readonly TimeSpan DefaultMaxExecuteDelayTime = TimeSpan.FromSeconds(30.0);

		private readonly TimeSpan maxExecuteDelayTime;

		private string syncSvcUrl;
	}
}
