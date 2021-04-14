using System;
using System.Collections.Generic;

namespace Microsoft.Office.CompliancePolicy.PolicySync
{
	internal sealed class ObjectSyncSubWorkItem : SubWorkItemBase
	{
		public ObjectSyncSubWorkItem(SyncJob syncJob, SyncChangeInfo changeInfo) : base(syncJob, changeInfo)
		{
		}

		public override void Execute()
		{
			base.SyncJob.LogProvider.LogOneEntry("UnifiedPolicySyncAgent", base.SyncJob.CurrentWorkItem.TenantContext.TenantId.ToString(), base.SyncJob.CurrentWorkItem.ExternalIdentity, ExecutionLog.EventType.Information, "Unified Policy Sync ObjectSync SubWorkItem Begin", string.Format("Unified Policy Sync ObjectSync SubWorkItem Begin for {0} object {1}", base.ChangeInfo.ObjectType, base.ChangeInfo.ObjectId.Value), null, new KeyValuePair<string, object>[0]);
			Guid objectId = base.ChangeInfo.ObjectId.Value;
			Guid tenantId = base.SyncJob.CurrentWorkItem.TenantContext.TenantId;
			PolicyConfigurationBase deltaObject = base.SyncJob.SyncSvcClient.GetObject(SyncCallerContext.Create(base.SyncJob.SyncAgentContext.SyncAgentConfig.PartnerName), tenantId, base.ChangeInfo.ObjectType, objectId, true, base.SyncJob.MonitorEventTracker);
			base.LogDeltaObjectFromMasterStore(deltaObject);
			if (base.ShouldProcessDelta(deltaObject))
			{
				base.CommitObjectWrapper(delegate
				{
					this.ProcessDelta(deltaObject, objectId, this.ChangeInfo);
				}, SubWorkItemBase.CreateErrorStatus(base.ChangeInfo.ObjectType, objectId, SubWorkItemBase.GetParentObjectId(deltaObject), (deltaObject == null) ? null : deltaObject.Version, SubWorkItemBase.GetObjectMode(deltaObject)));
			}
			base.SyncJob.LogProvider.LogOneEntry("UnifiedPolicySyncAgent", base.SyncJob.CurrentWorkItem.TenantContext.TenantId.ToString(), base.SyncJob.CurrentWorkItem.ExternalIdentity, ExecutionLog.EventType.Information, "Unified Policy Sync ObjectSync SubWorkItem End", string.Format("Unified Policy Sync ObjectSync SubWorkItem End for {0} object {1}", base.ChangeInfo.ObjectType, base.ChangeInfo.ObjectId.Value), null, new KeyValuePair<string, object>[0]);
		}

		public override void BeginExecute(Action<SubWorkItemBase> callback)
		{
			base.SyncJob.LogProvider.LogOneEntry("UnifiedPolicySyncAgent", base.SyncJob.CurrentWorkItem.TenantContext.TenantId.ToString(), base.SyncJob.CurrentWorkItem.ExternalIdentity, ExecutionLog.EventType.Information, "Unified Policy Sync ObjectSync SubWorkItem Begin", string.Format("Unified Policy Sync ObjectSync SubWorkItem Begin for {0} object {1}", base.ChangeInfo.ObjectType, base.ChangeInfo.ObjectId.Value), null, new KeyValuePair<string, object>[0]);
			base.ExternalCallback = callback;
			Guid value = base.ChangeInfo.ObjectId.Value;
			Guid tenantId = base.SyncJob.CurrentWorkItem.TenantContext.TenantId;
			base.SyncJob.SyncSvcClient.BeginGetObject(SyncCallerContext.Create(base.SyncJob.SyncAgentContext.SyncAgentConfig.PartnerName), tenantId, base.ChangeInfo.ObjectType, value, true, new AsyncCallback(this.InternalCallback), null, base.SyncJob.MonitorEventTracker);
		}

		protected override void InternalCallback(IAsyncResult ar)
		{
			try
			{
				PolicyConfigurationBase deltaObject = base.SyncJob.SyncSvcClient.EndGetObject(ar, base.SyncJob.MonitorEventTracker, base.ChangeInfo.ObjectType);
				base.LogDeltaObjectFromMasterStore(deltaObject);
				if (base.ShouldProcessDelta(deltaObject))
				{
					base.CommitObjectWrapper(delegate
					{
						this.ProcessDelta(deltaObject, this.ChangeInfo.ObjectId.Value, this.ChangeInfo);
					}, SubWorkItemBase.CreateErrorStatus(base.ChangeInfo.ObjectType, base.ChangeInfo.ObjectId.Value, SubWorkItemBase.GetParentObjectId(deltaObject), (deltaObject == null) ? null : deltaObject.Version, SubWorkItemBase.GetObjectMode(deltaObject)));
				}
			}
			catch (Exception lastError)
			{
				base.LastError = lastError;
			}
			base.SyncJob.LogProvider.LogOneEntry("UnifiedPolicySyncAgent", base.SyncJob.CurrentWorkItem.TenantContext.TenantId.ToString(), base.SyncJob.CurrentWorkItem.ExternalIdentity, ExecutionLog.EventType.Information, "Unified Policy Sync ObjectSync SubWorkItem End", string.Format("Unified Policy Sync ObjectSync SubWorkItem End for {0} object {1}", base.ChangeInfo.ObjectType, base.ChangeInfo.ObjectId.Value), null, new KeyValuePair<string, object>[0]);
			base.ExternalCallback(this);
		}
	}
}
