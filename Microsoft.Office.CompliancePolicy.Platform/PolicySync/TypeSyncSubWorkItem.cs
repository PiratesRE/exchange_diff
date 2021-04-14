using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Office.CompliancePolicy.Monitor;
using Microsoft.Office.CompliancePolicy.PolicyConfiguration;

namespace Microsoft.Office.CompliancePolicy.PolicySync
{
	internal sealed class TypeSyncSubWorkItem : SubWorkItemBase
	{
		public TypeSyncSubWorkItem(SyncJob syncJob, SyncChangeInfo changeInfo) : base(syncJob, changeInfo)
		{
		}

		public override void Execute()
		{
			base.SyncJob.LogProvider.LogOneEntry("UnifiedPolicySyncAgent", base.SyncJob.CurrentWorkItem.TenantContext.TenantId.ToString(), base.SyncJob.CurrentWorkItem.ExternalIdentity, ExecutionLog.EventType.Information, "Unified Policy Sync TypeSync SubWorkItem Begin", string.Format("Unified Policy Sync TypeSync SubWorkItem Begin for {0}", base.ChangeInfo.ObjectType), null, new KeyValuePair<string, object>[0]);
			if (!this.Initialize())
			{
				return;
			}
			bool firstQuery = true;
			bool flag = true;
			while (!base.SyncJob.HostStateProvider.IsShuttingDown() && flag)
			{
				PolicyChange singleTenantChanges = base.SyncJob.SyncSvcClient.GetSingleTenantChanges(this.BuildTenantCookie(firstQuery), base.SyncJob.MonitorEventTracker);
				base.LogDeltaObjectCollectionFromMasterStore(singleTenantChanges);
				if (singleTenantChanges == null || singleTenantChanges.Changes == null || !singleTenantChanges.Changes.Any<PolicyConfigurationBase>())
				{
					flag = false;
				}
				else
				{
					flag = this.CommitChanges(singleTenantChanges);
					firstQuery = false;
				}
			}
			if (!base.SyncJob.HostStateProvider.IsShuttingDown())
			{
				this.OnNoMorePage();
			}
			base.SyncJob.LogProvider.LogOneEntry("UnifiedPolicySyncAgent", base.SyncJob.CurrentWorkItem.TenantContext.TenantId.ToString(), base.SyncJob.CurrentWorkItem.ExternalIdentity, ExecutionLog.EventType.Information, "Unified Policy Sync TypeSync SubWorkItem End", string.Format("Unified Policy Sync TypeSync SubWorkItem End for {0}", base.ChangeInfo.ObjectType), null, new KeyValuePair<string, object>[0]);
		}

		public override void BeginExecute(Action<SubWorkItemBase> callback)
		{
			base.SyncJob.LogProvider.LogOneEntry("UnifiedPolicySyncAgent", base.SyncJob.CurrentWorkItem.TenantContext.TenantId.ToString(), base.SyncJob.CurrentWorkItem.ExternalIdentity, ExecutionLog.EventType.Information, "Unified Policy Sync TypeSync SubWorkItem Begin", string.Format("Unified Policy Sync TypeSync SubWorkItem Begin for {0}", base.ChangeInfo.ObjectType), null, new KeyValuePair<string, object>[0]);
			base.ExternalCallback = callback;
			if (this.Initialize())
			{
				base.SyncJob.SyncSvcClient.BeginGetSingleTenantChanges(this.BuildTenantCookie(true), new AsyncCallback(this.InternalCallback), null, base.SyncJob.MonitorEventTracker);
				return;
			}
			base.ExternalCallback(this);
		}

		protected override void InternalCallback(IAsyncResult ar)
		{
			try
			{
				PolicyChange policyChange = base.SyncJob.SyncSvcClient.EndGetSingleTenantChanges(ar, base.SyncJob.MonitorEventTracker, base.ChangeInfo.ObjectType);
				base.LogDeltaObjectCollectionFromMasterStore(policyChange);
				bool flag = policyChange != null && policyChange.Changes != null && policyChange.Changes.Any<PolicyConfigurationBase>() && this.CommitChanges(policyChange);
				if (!base.SyncJob.HostStateProvider.IsShuttingDown() && flag)
				{
					base.SyncJob.SyncSvcClient.BeginGetSingleTenantChanges(this.BuildTenantCookie(false), new AsyncCallback(this.InternalCallback), null, base.SyncJob.MonitorEventTracker);
					return;
				}
				if (!base.SyncJob.HostStateProvider.IsShuttingDown())
				{
					this.OnNoMorePage();
				}
			}
			catch (Exception lastError)
			{
				base.LastError = lastError;
			}
			base.SyncJob.LogProvider.LogOneEntry("UnifiedPolicySyncAgent", base.SyncJob.CurrentWorkItem.TenantContext.TenantId.ToString(), base.SyncJob.CurrentWorkItem.ExternalIdentity, ExecutionLog.EventType.Information, "Unified Policy Sync TypeSync SubWorkItem End", string.Format("Unified Policy Sync TypeSync SubWorkItem End for {0}", base.ChangeInfo.ObjectType), null, new KeyValuePair<string, object>[0]);
			base.ExternalCallback(this);
		}

		private bool Initialize()
		{
			bool result = true;
			this.latestVersion = base.SyncJob.TenantInfo.SyncInfoTable[base.ChangeInfo.ObjectType].LatestVersion;
			if (base.IsFullSync)
			{
				base.LocalObjectList = this.LoadLocalObjectList();
			}
			else
			{
				PolicyVersion version = base.ChangeInfo.Version;
				if (null != this.latestVersion && null != version && this.latestVersion.CompareTo(version) >= 0)
				{
					result = false;
					base.SyncJob.LogProvider.LogOneEntry("UnifiedPolicySyncAgent", base.SyncJob.CurrentWorkItem.TenantContext.TenantId.ToString(), base.SyncJob.CurrentWorkItem.ExternalIdentity, ExecutionLog.EventType.Information, "Unified Policy Sync TypeSync SubWorkItem Skipped", string.Format("Unified Policy Sync TypeSync SubWorkItem Skipped for {0} because the workload version is equal or higher than the master version", base.ChangeInfo.ObjectType), null, new KeyValuePair<string, object>[0]);
				}
			}
			return result;
		}

		private bool CommitChanges(PolicyChange policyChanges)
		{
			IEnumerable<PolicyConfigurationBase> changes = policyChanges.Changes;
			TenantCookie newCookie = policyChanges.NewCookie;
			using (IEnumerator<PolicyConfigurationBase> enumerator = changes.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					PolicyConfigurationBase deltaObject = enumerator.Current;
					if (base.ShouldProcessDelta(deltaObject))
					{
						base.CommitObjectWrapper(delegate
						{
							this.ProcessDelta(deltaObject, deltaObject.ObjectId, this.ChangeInfo);
							if (null == this.latestVersion || (null != this.latestVersion && null != deltaObject.Version && deltaObject.Version.CompareTo(this.latestVersion) > 0))
							{
								this.latestVersion = deltaObject.Version;
							}
						}, SubWorkItemBase.CreateErrorStatus(deltaObject.ObjectType, deltaObject.ObjectId, SubWorkItemBase.GetParentObjectId(deltaObject), (deltaObject == null) ? null : deltaObject.Version, SubWorkItemBase.GetObjectMode(deltaObject)));
					}
				}
			}
			TenantInfo tenantInfo = base.SyncJob.TenantInfo;
			SyncInfo syncInfo = tenantInfo.SyncInfoTable[base.ChangeInfo.ObjectType];
			syncInfo.PreviousSyncCookie = syncInfo.CurrentSyncCookie;
			syncInfo.CurrentSyncCookie = newCookie.Cookie;
			syncInfo.LatestVersion = this.latestVersion;
			base.SyncJob.MonitorEventTracker.TrackLatencyWrapper(LatencyType.TenantInfo, delegate()
			{
				this.SyncJob.TenantInfoProvider.Save(tenantInfo);
			});
			return true;
		}

		private void OnNoMorePage()
		{
			if (base.IsFullSync)
			{
				this.DeleteLocalDangleObjects();
				return;
			}
			PolicyVersion version = base.ChangeInfo.Version;
			bool flag = null != this.latestVersion && null != version && this.latestVersion.CompareTo(version) < 0;
			if (flag)
			{
				SyncAgentTransientException ex = new SyncAgentTransientException("Due to ffo DB replica delay, the latest change can not be retrieved. To be retried in the next sync cycle", false, SyncAgentErrorCode.Generic);
				throw ex;
			}
		}

		private void DeleteLocalDangleObjects()
		{
			IEnumerable<PolicyConfigBase> enumerable = from keyValPair in base.LocalObjectList
			select keyValPair.Value;
			HashSet<PolicyConfigBase> deletedObjectList = new HashSet<PolicyConfigBase>();
			using (IEnumerator<PolicyConfigBase> enumerator = enumerable.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					PolicyConfigBase dangleObj = enumerator.Current;
					PolicyConfigBase obj;
					base.CommitObjectWrapper(delegate
					{
						this.SyncJob.PolicyConfigProvider.DeleteWrapper(dangleObj, delegate(PolicyConfigBase obj)
						{
							deletedObjectList.Add(obj);
						}, this.SyncJob.MonitorEventTracker);
					}, SubWorkItemBase.CreateErrorStatus(base.ChangeInfo.ObjectType, dangleObj.Identity, SubWorkItemBase.GetParentObjectId(dangleObj), (dangleObj == null) ? null : dangleObj.Version, SubWorkItemBase.GetObjectMode(dangleObj)));
				}
			}
			foreach (PolicyConfigBase obj in deletedObjectList)
			{
				PolicyConfigBase obj;
				base.OnObjectDeleted(obj);
			}
		}

		private Dictionary<Guid, PolicyConfigBase> LoadLocalObjectList()
		{
			Dictionary<Guid, PolicyConfigBase> result = new Dictionary<Guid, PolicyConfigBase>();
			IEnumerable<PolicyConfigBase> enumerable = null;
			ConfigurationObjectType objectType = base.ChangeInfo.ObjectType;
			switch (objectType)
			{
			case ConfigurationObjectType.Policy:
				enumerable = this.LoadLocalPolicies();
				break;
			case ConfigurationObjectType.Rule:
			case ConfigurationObjectType.Binding:
				if (base.SyncJob.LocalPolicyIdList == null)
				{
					this.LoadLocalPolicies();
				}
				if (base.SyncJob.LocalPolicyIdList.Any<Guid>())
				{
					enumerable = base.GetAllWrapper(objectType, base.SyncJob.LocalPolicyIdList);
				}
				break;
			case ConfigurationObjectType.Association:
				enumerable = base.GetAllWrapper(objectType, null);
				break;
			}
			if (enumerable != null && enumerable.Any<PolicyConfigBase>())
			{
				result = enumerable.ToDictionary((PolicyConfigBase p) => p.Identity);
			}
			return result;
		}

		private IEnumerable<PolicyConfigBase> LoadLocalPolicies()
		{
			bool flag = base.SyncJob.CurrentWorkItem.WorkItemInfo.ContainsKey(ConfigurationObjectType.Binding) || base.SyncJob.CurrentWorkItem.WorkItemInfo.ContainsKey(ConfigurationObjectType.Rule);
			bool flag2 = base.SyncJob.LocalPolicyIdList == null && flag;
			if (flag2)
			{
				base.SyncJob.LocalPolicyIdList = new List<Guid>();
			}
			IEnumerable<PolicyConfigBase> allWrapper = base.GetAllWrapper(ConfigurationObjectType.Policy, null);
			if (allWrapper != null && allWrapper.Any<PolicyConfigBase>() && flag2)
			{
				base.SyncJob.LocalPolicyIdList = (from p in allWrapper
				select p.Identity).ToList<Guid>();
			}
			return allWrapper;
		}

		private TenantCookie BuildTenantCookie(bool firstQuery)
		{
			return new TenantCookie(base.SyncJob.CurrentWorkItem.TenantContext.TenantId, (base.IsFullSync && firstQuery) ? null : base.SyncJob.TenantInfo.SyncInfoTable[base.ChangeInfo.ObjectType].CurrentSyncCookie, base.SyncJob.CurrentWorkItem.Workload, base.ChangeInfo.ObjectType, base.IsFullSync ? new DateTime?(DateTime.UtcNow) : null);
		}

		private PolicyVersion latestVersion;
	}
}
