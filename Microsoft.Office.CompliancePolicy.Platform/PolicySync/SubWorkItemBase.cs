using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Microsoft.Office.CompliancePolicy.PolicyConfiguration;

namespace Microsoft.Office.CompliancePolicy.PolicySync
{
	internal abstract class SubWorkItemBase
	{
		public SubWorkItemBase(SyncJob syncJob, SyncChangeInfo changeInfo = null)
		{
			ArgumentValidator.ThrowIfNull("syncJob", syncJob);
			this.SyncJob = syncJob;
			this.IsFullSync = this.SyncJob.CurrentWorkItem.FullSyncForTenant;
			this.ChangeInfo = changeInfo;
		}

		public SyncChangeInfo ChangeInfo { get; private set; }

		private protected bool IsFullSync { protected get; private set; }

		protected Dictionary<Guid, PolicyConfigBase> LocalObjectList { get; set; }

		private protected SyncJob SyncJob { protected get; private set; }

		protected Exception LastError { get; set; }

		protected Action<SubWorkItemBase> ExternalCallback { get; set; }

		public abstract void Execute();

		public abstract void BeginExecute(Action<SubWorkItemBase> callback);

		public void EndExecute()
		{
			if (this.LastError != null)
			{
				throw this.LastError;
			}
		}

		internal static Guid? GetParentObjectId(PolicyConfigBase localObject)
		{
			if (localObject == null || localObject is PolicyAssociationConfig)
			{
				return null;
			}
			if (localObject is PolicyBindingSetConfig)
			{
				return new Guid?(((PolicyBindingSetConfig)localObject).PolicyDefinitionConfigId);
			}
			if (localObject is PolicyRuleConfig)
			{
				return new Guid?(((PolicyRuleConfig)localObject).PolicyDefinitionConfigId);
			}
			if (localObject is PolicyDefinitionConfig)
			{
				return new Guid?(localObject.Identity);
			}
			throw new NotSupportedException("object type " + localObject.GetType() + "is not supported");
		}

		internal static Guid? GetParentObjectId(PolicyConfigurationBase deltaObject)
		{
			if (deltaObject == null || deltaObject is AssociationConfiguration)
			{
				return null;
			}
			if (deltaObject is BindingConfiguration)
			{
				return new Guid?(((BindingConfiguration)deltaObject).PolicyId);
			}
			if (deltaObject is RuleConfiguration)
			{
				return new Guid?(((RuleConfiguration)deltaObject).ParentPolicyId);
			}
			if (deltaObject is PolicyConfiguration)
			{
				return new Guid?(deltaObject.ObjectId);
			}
			throw new NotSupportedException("object type " + deltaObject.GetType() + "is not supported");
		}

		internal static Mode GetObjectMode(PolicyConfigBase localObject)
		{
			if (localObject is PolicyRuleConfig)
			{
				return ((PolicyRuleConfig)localObject).Mode;
			}
			if (localObject is PolicyDefinitionConfig)
			{
				return ((PolicyDefinitionConfig)localObject).Mode;
			}
			return Mode.Enforce;
		}

		internal static Mode GetObjectMode(PolicyConfigurationBase deltaObject)
		{
			if (deltaObject is RuleConfiguration)
			{
				RuleConfiguration ruleConfiguration = (RuleConfiguration)deltaObject;
				if (ruleConfiguration != null && ruleConfiguration.Mode.Changed)
				{
					return ruleConfiguration.Mode.Value;
				}
			}
			else if (deltaObject is PolicyConfiguration)
			{
				PolicyConfiguration policyConfiguration = (PolicyConfiguration)deltaObject;
				if (policyConfiguration != null && policyConfiguration.Mode.Changed)
				{
					return policyConfiguration.Mode.Value;
				}
			}
			return Mode.Enforce;
		}

		protected static UnifiedPolicyStatus CreateErrorStatus(ConfigurationObjectType objectType, Guid objectId, Guid? parentObjectId, PolicyVersion objectVersion, Mode mode)
		{
			return new UnifiedPolicyStatus
			{
				ObjectType = objectType,
				ObjectId = objectId,
				ParentObjectId = parentObjectId,
				Version = objectVersion,
				Mode = mode
			};
		}

		protected virtual void InternalCallback(IAsyncResult ar)
		{
		}

		protected void CommitObjectWrapper(Action commitObjectDelegate, UnifiedPolicyStatus errorStatusInfo)
		{
			try
			{
				commitObjectDelegate();
			}
			catch (SyncAgentTransientException ex)
			{
				if (!ex.IsPerObjectException || !this.SyncJob.IsLastTry)
				{
					throw;
				}
				UnifiedPolicyStatus unifiedPolicyStatus = this.FillInErrorStatus(errorStatusInfo, ex);
				this.SyncJob.LogProvider.LogOneEntry("UnifiedPolicySyncAgent", this.SyncJob.CurrentWorkItem.TenantContext.TenantId.ToString(), this.SyncJob.CurrentWorkItem.ExternalIdentity, ExecutionLog.EventType.Information, "Unified Policy Sync Agent Publish Status", unifiedPolicyStatus.ToString(), null, new KeyValuePair<string, object>[0]);
				this.SyncJob.PolicyConfigProvider.PublishStatus(new UnifiedPolicyStatus[]
				{
					unifiedPolicyStatus
				});
				this.SyncJob.Errors.Add(ex);
				this.SyncJob.MonitorEventTracker.ReportObjectLevelFailure(ex, errorStatusInfo.ObjectType, errorStatusInfo.ParentObjectId);
			}
			catch (SyncAgentPermanentException ex2)
			{
				UnifiedPolicyStatus unifiedPolicyStatus2 = this.FillInErrorStatus(errorStatusInfo, ex2);
				this.SyncJob.LogProvider.LogOneEntry("UnifiedPolicySyncAgent", this.SyncJob.CurrentWorkItem.TenantContext.TenantId.ToString(), this.SyncJob.CurrentWorkItem.ExternalIdentity, ExecutionLog.EventType.Information, "Unified Policy Sync Agent Publish Status", unifiedPolicyStatus2.ToString(), null, new KeyValuePair<string, object>[0]);
				this.SyncJob.PolicyConfigProvider.PublishStatus(new UnifiedPolicyStatus[]
				{
					unifiedPolicyStatus2
				});
				if (!ex2.IsPerObjectException)
				{
					throw;
				}
				this.SyncJob.Errors.Add(ex2);
				this.SyncJob.MonitorEventTracker.ReportObjectLevelFailure(ex2, errorStatusInfo.ObjectType, errorStatusInfo.ParentObjectId);
			}
		}

		protected bool ShouldProcessDelta(PolicyConfigurationBase deltaObject)
		{
			if (deltaObject == null)
			{
				return true;
			}
			Workload workload = this.SyncJob.CurrentWorkItem.Workload;
			if ((deltaObject.Workload & workload) == workload)
			{
				return true;
			}
			switch (deltaObject.ObjectType)
			{
			case ConfigurationObjectType.Policy:
			case ConfigurationObjectType.Rule:
				return true;
			case ConfigurationObjectType.Association:
			case ConfigurationObjectType.Binding:
				this.SyncJob.LogProvider.LogOneEntry("UnifiedPolicySyncAgent", this.SyncJob.CurrentWorkItem.TenantContext.TenantId.ToString(), this.SyncJob.CurrentWorkItem.ExternalIdentity, ExecutionLog.EventType.Information, "Unified Policy Sync Object Skipped", string.Format(CultureInfo.InvariantCulture, "Unified Policy Sync Object Skipped for type {0} object {1} because it doesn't apply to the current workload", new object[]
				{
					deltaObject.ObjectType,
					deltaObject.ObjectId
				}), null, new KeyValuePair<string, object>[0]);
				return false;
			default:
				throw new NotSupportedException();
			}
		}

		protected void ProcessDelta(PolicyConfigurationBase deltaObject, Guid deltaObjectId, SyncChangeInfo changeInfo)
		{
			if (deltaObject == null)
			{
				if (ChangeType.Delete != changeInfo.ChangeType)
				{
					SyncAgentTransientException ex = new SyncAgentTransientException("Due to ffo DB replica delay, the latest change of Object " + this.ChangeInfo.ObjectId.Value + " can not be retrieved. To be retried in the next sync cycle", true, SyncAgentErrorCode.Generic);
					throw ex;
				}
				if (ConfigurationObjectType.Binding != this.ChangeInfo.ObjectType)
				{
					PolicyVersion policyVersion = this.SaveObject(deltaObject, this.ChangeInfo.ObjectType, deltaObjectId, true);
					return;
				}
				SyncAgentTransientException ex2 = new SyncAgentTransientException("Due to ffo DB replica delay, the latest change of Object " + this.ChangeInfo.ObjectId.Value + " can not be retrieved. To be retried in the next sync cycle", true, SyncAgentErrorCode.Generic);
				throw ex2;
			}
			else
			{
				bool flag = ChangeType.Delete == deltaObject.ChangeType;
				PolicyVersion policyVersion = this.SaveObject(deltaObject, this.ChangeInfo.ObjectType, deltaObjectId, flag);
				PolicyVersion version = this.ChangeInfo.Version;
				bool flag2 = this.ChangeInfo.ObjectId != null && !flag && policyVersion != null && version != null && policyVersion.CompareTo(version) < 0;
				if (flag2)
				{
					SyncAgentTransientException ex3 = new SyncAgentTransientException("Due to ffo DB replica delay, the latest change of Object " + this.ChangeInfo.ObjectId.Value + " can not be retrieved. To be retried in the next sync cycle", true, SyncAgentErrorCode.Generic);
					throw ex3;
				}
				return;
			}
		}

		protected PolicyConfigBase GetWrapper(ConfigurationObjectType objectType, Guid objectSearchId)
		{
			switch (objectType)
			{
			case ConfigurationObjectType.Policy:
				return this.SyncJob.PolicyConfigProvider.GetWrapper(objectSearchId, this.SyncJob.MonitorEventTracker);
			case ConfigurationObjectType.Rule:
				return this.SyncJob.PolicyConfigProvider.GetWrapper(objectSearchId, this.SyncJob.MonitorEventTracker);
			case ConfigurationObjectType.Association:
				return this.SyncJob.PolicyConfigProvider.GetWrapper(objectSearchId, this.SyncJob.MonitorEventTracker);
			case ConfigurationObjectType.Binding:
				return this.SyncJob.PolicyConfigProvider.GetWrapper(objectSearchId, this.SyncJob.MonitorEventTracker);
			default:
				throw new NotSupportedException();
			}
		}

		protected IEnumerable<PolicyConfigBase> GetAllWrapper(ConfigurationObjectType objectType, IEnumerable<Guid> policyIds = null)
		{
			switch (objectType)
			{
			case ConfigurationObjectType.Policy:
				return this.SyncJob.PolicyConfigProvider.GetAllWrapper(this.SyncJob.MonitorEventTracker, policyIds);
			case ConfigurationObjectType.Rule:
				return this.SyncJob.PolicyConfigProvider.GetAllWrapper(this.SyncJob.MonitorEventTracker, policyIds);
			case ConfigurationObjectType.Association:
				return this.SyncJob.PolicyConfigProvider.GetAllWrapper(this.SyncJob.MonitorEventTracker, policyIds);
			case ConfigurationObjectType.Binding:
				return this.SyncJob.PolicyConfigProvider.GetAllWrapper(this.SyncJob.MonitorEventTracker, policyIds);
			default:
				throw new NotSupportedException();
			}
		}

		protected PolicyConfigBase NewBlankConfigInstanceWrapper(ConfigurationObjectType objectType, Guid objectId)
		{
			switch (objectType)
			{
			case ConfigurationObjectType.Policy:
				return this.SyncJob.PolicyConfigProvider.NewBlankConfigInstanceWrapper(objectId);
			case ConfigurationObjectType.Rule:
				return this.SyncJob.PolicyConfigProvider.NewBlankConfigInstanceWrapper(objectId);
			case ConfigurationObjectType.Association:
				return this.SyncJob.PolicyConfigProvider.NewBlankConfigInstanceWrapper(objectId);
			case ConfigurationObjectType.Binding:
				return this.SyncJob.PolicyConfigProvider.NewBlankConfigInstanceWrapper(objectId);
			default:
				throw new NotSupportedException();
			}
		}

		protected void OnObjectDeleted(PolicyConfigBase obj)
		{
			this.OnObjectDeleted(obj.Identity);
		}

		protected void OnObjectAddedOrUpdated(PolicyConfigBase obj)
		{
			if (this.IsFullSync)
			{
				this.LocalObjectList.Remove(obj.Identity);
			}
		}

		protected void LogDeltaObjectFromMasterStore(PolicyConfigurationBase deltaObject)
		{
			if (deltaObject != null)
			{
				BindingConfiguration bindingConfiguration = deltaObject as BindingConfiguration;
				if (bindingConfiguration != null && bindingConfiguration.AppliedScopes != null && bindingConfiguration.AppliedScopes.Changed)
				{
					IEnumerable<ScopeConfiguration>[] array = new IEnumerable<ScopeConfiguration>[]
					{
						bindingConfiguration.AppliedScopes.RemovedValues,
						bindingConfiguration.AppliedScopes.ChangedValues
					};
					foreach (IEnumerable<ScopeConfiguration> enumerable in array)
					{
						if (enumerable != null)
						{
							foreach (ScopeConfiguration deltaObject2 in enumerable)
							{
								this.LogDeltaObjectFromMasterStore(deltaObject2);
							}
						}
					}
				}
				object wholeProperty = Utility.GetWholeProperty(deltaObject, "PolicyScenario");
				string text = (wholeProperty != null) ? string.Format(CultureInfo.InvariantCulture, "; scenario: {0}.", new object[]
				{
					wholeProperty.ToString()
				}) : string.Empty;
				object obj;
				string text2 = Utility.GetIncrementalProperty(deltaObject, "Mode", out obj) ? string.Format(CultureInfo.InvariantCulture, "; mode: {0}.", new object[]
				{
					obj.ToString()
				}) : string.Empty;
				this.SyncJob.LogProvider.LogOneEntry("UnifiedPolicySyncAgent", this.SyncJob.CurrentWorkItem.TenantContext.TenantId.ToString(), this.SyncJob.CurrentWorkItem.ExternalIdentity, ExecutionLog.EventType.Information, "Unified Policy Sync Get Object From EOP", string.Format(CultureInfo.InvariantCulture, "Unified Policy Sync Get Object From EOP: type: {0}; object id: {1}; object version: {2} {3} {4}", new object[]
				{
					deltaObject.ObjectType,
					deltaObject.ObjectId,
					deltaObject.Version,
					text,
					text2
				}), null, new KeyValuePair<string, object>[0]);
				return;
			}
			this.SyncJob.LogProvider.LogOneEntry("UnifiedPolicySyncAgent", this.SyncJob.CurrentWorkItem.TenantContext.TenantId.ToString(), this.SyncJob.CurrentWorkItem.ExternalIdentity, ExecutionLog.EventType.Information, "Unified Policy Sync ObjectSync Get Nothing From EOP", string.Format(CultureInfo.InvariantCulture, "Unified Policy Sync ObjectSync Get Nothing From EOP for {0} Object {1} of Version {2} of ChangeType {3}", new object[]
			{
				this.ChangeInfo.ObjectType,
				(this.ChangeInfo.ObjectId != null) ? this.ChangeInfo.ObjectId.Value.ToString() : string.Empty,
				this.ChangeInfo.Version,
				this.ChangeInfo.ChangeType
			}), null, new KeyValuePair<string, object>[0]);
		}

		protected void LogDeltaObjectCollectionFromMasterStore(PolicyChange policyChanges)
		{
			if (policyChanges == null || policyChanges.Changes == null || !policyChanges.Changes.Any<PolicyConfigurationBase>())
			{
				this.SyncJob.LogProvider.LogOneEntry("UnifiedPolicySyncAgent", this.SyncJob.CurrentWorkItem.TenantContext.TenantId.ToString(), this.SyncJob.CurrentWorkItem.ExternalIdentity, ExecutionLog.EventType.Information, "Unified Policy Sync TypeSync Get Nothing From EOP", string.Format(CultureInfo.InvariantCulture, "Unified Policy Sync TypeSync Get Nothing From EOP for Type {0}", new object[]
				{
					this.ChangeInfo.ObjectType
				}), null, new KeyValuePair<string, object>[0]);
				return;
			}
			foreach (PolicyConfigurationBase deltaObject in policyChanges.Changes)
			{
				this.LogDeltaObjectFromMasterStore(deltaObject);
			}
		}

		private void OnObjectDeleted(Guid objId)
		{
			this.SyncJob.DeletedObjectList.Add(objId);
			if (this.IsFullSync)
			{
				this.LocalObjectList.Remove(objId);
			}
		}

		private PolicyVersion SaveObject(PolicyConfigurationBase deltaObject, ConfigurationObjectType objectType, Guid deltaObjectId, bool isDelete)
		{
			PolicyVersion result = null;
			PolicyConfigBase policyConfigBase = this.LoadObject(objectType, deltaObjectId, deltaObject);
			if (isDelete)
			{
				if (policyConfigBase != null)
				{
					this.SyncJob.PolicyConfigProvider.DeleteWrapper(policyConfigBase, new Action<PolicyConfigBase>(this.OnObjectDeleted), this.SyncJob.MonitorEventTracker);
				}
				else
				{
					this.OnObjectDeleted(deltaObjectId);
				}
			}
			else
			{
				if (policyConfigBase == null)
				{
					policyConfigBase = this.NewBlankConfigInstanceWrapper(objectType, deltaObjectId);
				}
				PolicyVersion version = policyConfigBase.Version;
				result = version;
				if (null == version || (null != version && version.CompareTo(deltaObject.Version) <= 0))
				{
					deltaObject.MergeInto(policyConfigBase, this.ChangeInfo.ObjectId != null, this.SyncJob.PolicyConfigProvider);
					this.SyncJob.PolicyConfigProvider.SaveWrapper(policyConfigBase, new Func<Guid, bool>(this.IsObjectAlreadyDeleted), new Action<PolicyConfigBase>(this.OnObjectAddedOrUpdated), this.SyncJob.MonitorEventTracker);
					result = deltaObject.Version;
				}
				else if (this.IsFullSync)
				{
					this.LocalObjectList.Remove(policyConfigBase.Identity);
				}
			}
			return result;
		}

		private PolicyConfigBase LoadObject(ConfigurationObjectType objectType, Guid objectId, PolicyConfigurationBase deltaObject)
		{
			if (this.IsFullSync)
			{
				if (ConfigurationObjectType.Binding == objectType)
				{
					PolicyConfigBase policyConfigBase = null;
					foreach (PolicyConfigBase policyConfigBase2 in this.LocalObjectList.Values)
					{
						PolicyBindingSetConfig policyBindingSetConfig = (PolicyBindingSetConfig)policyConfigBase2;
						if (policyBindingSetConfig.PolicyDefinitionConfigId == ((BindingConfiguration)deltaObject).PolicyId)
						{
							policyConfigBase = policyBindingSetConfig;
							break;
						}
					}
					if (policyConfigBase != null)
					{
						this.LocalObjectList.Remove(policyConfigBase.Identity);
						policyConfigBase.Identity = deltaObject.ObjectId;
						this.LocalObjectList[policyConfigBase.Identity] = policyConfigBase;
					}
					return policyConfigBase;
				}
				if (!this.LocalObjectList.ContainsKey(objectId))
				{
					return null;
				}
				return this.LocalObjectList[objectId];
			}
			else
			{
				Guid objectSearchId = this.GetObjectSearchId(objectType, objectId, deltaObject);
				if (!(objectSearchId == Guid.Empty))
				{
					return this.GetWrapper(objectType, objectSearchId);
				}
				return null;
			}
		}

		private Guid GetObjectSearchId(ConfigurationObjectType objectType, Guid objectId, PolicyConfigurationBase deltaObject)
		{
			if (ConfigurationObjectType.Binding != objectType)
			{
				return objectId;
			}
			if (deltaObject != null)
			{
				return ((BindingConfiguration)deltaObject).PolicyId;
			}
			return Guid.Empty;
		}

		private bool IsObjectAlreadyDeleted(Guid id)
		{
			return this.SyncJob.DeletedObjectList.Contains(id);
		}

		private UnifiedPolicyStatus FillInErrorStatus(UnifiedPolicyStatus errorStatusInfo, Exception ex)
		{
			errorStatusInfo.TenantId = this.SyncJob.CurrentWorkItem.TenantContext.TenantId;
			errorStatusInfo.Workload = this.SyncJob.CurrentWorkItem.Workload;
			errorStatusInfo.ErrorCode = UnifiedPolicyErrorCode.InternalError;
			AdditionalDiagnostics additionalDiagnostics = new AdditionalDiagnostics(Environment.MachineName, ex);
			errorStatusInfo.AdditionalDiagnostics = additionalDiagnostics.Serialize();
			errorStatusInfo.WhenProcessedUTC = DateTime.UtcNow;
			return errorStatusInfo;
		}
	}
}
