using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Directory.UnifiedPolicy;
using Microsoft.Exchange.Data.UnifiedPolicy;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Management.Tasks;
using Microsoft.Exchange.Management.Transport;
using Microsoft.Office.CompliancePolicy.PolicyConfiguration;
using Microsoft.Office.CompliancePolicy.PolicySync;

namespace Microsoft.Office.CompliancePolicy.Tasks
{
	internal sealed class PolicySettingStatusHelpers
	{
		internal static Workload GetWorkloadFromString(string container)
		{
			Workload result;
			if (Enum.TryParse<Workload>(container, out result))
			{
				return result;
			}
			return Workload.None;
		}

		internal static ConfigurationObjectType GetConfigurationObjectTypeFromString(string objectTypeString)
		{
			ConfigurationObjectType result = ConfigurationObjectType.Policy;
			if (typeof(PolicyStorage).Name.Equals(objectTypeString, StringComparison.OrdinalIgnoreCase))
			{
				result = ConfigurationObjectType.Policy;
			}
			else if (typeof(RuleStorage).Name.Equals(objectTypeString, StringComparison.OrdinalIgnoreCase))
			{
				result = ConfigurationObjectType.Rule;
			}
			else if (typeof(AssociationStorage).Name.Equals(objectTypeString, StringComparison.OrdinalIgnoreCase))
			{
				result = ConfigurationObjectType.Association;
			}
			else if (typeof(BindingStorage).Name.Equals(objectTypeString, StringComparison.OrdinalIgnoreCase))
			{
				result = ConfigurationObjectType.Binding;
			}
			else if (typeof(ScopeStorage).Name.Equals(objectTypeString, StringComparison.OrdinalIgnoreCase))
			{
				result = ConfigurationObjectType.Scope;
			}
			return result;
		}

		private void WriteVerbose(LocalizedString info, bool writeToLog = false)
		{
			if (this.task != null)
			{
				this.task.WriteVerbose(info);
			}
			if (writeToLog && this.logger != null)
			{
				this.logger.LogOneEntry(ExecutionLog.EventType.Information, "PolicySettingStatusHelpers", string.Empty, info, new object[0]);
			}
		}

		private string GetStorageObjectName(UnifiedPolicyStorageBase storage)
		{
			if (storage is PolicyStorage)
			{
				return Strings.DisplayPolicyName(storage.Name);
			}
			if (storage is RuleStorage)
			{
				return Strings.DisplayRuleName(storage.Name);
			}
			if (storage is BindingStorage)
			{
				return Strings.DisplayBindingName(storage.Workload.ToString());
			}
			if (!(storage is ScopeStorage))
			{
				return string.Empty;
			}
			BindingMetadata bindingMetadata = BindingMetadata.FromStorage(((ScopeStorage)storage).Scope);
			if (bindingMetadata != null)
			{
				return bindingMetadata.Name;
			}
			return string.Empty;
		}

		internal PolicySettingStatusHelpers(IConfigDataProvider dataSession, Task task, ExecutionLog logger)
		{
			ArgumentValidator.ThrowIfNull("dataSession", dataSession);
			this.dataSession = dataSession;
			this.task = task;
			this.logger = logger;
			if (this.task != null)
			{
				this.includeDiagnosticInfo = Utils.ExecutingUserIsForestWideAdmin(task);
			}
		}

		internal PolicyDistributionErrorDetails CreatePolicyDistributionError(Workload singleWorkload, UnifiedPolicyStorageBase storage, UnifiedPolicySettingStatus status)
		{
			bool flag = false;
			UnifiedPolicyErrorCode errorCode = UnifiedPolicyErrorCode.Unknown;
			string empty = string.Empty;
			DateTime lastErrorTime = DateTime.UtcNow;
			string additionalDiagnostics = string.Empty;
			if (status == null)
			{
				if (storage is ScopeStorage)
				{
					flag = true;
					errorCode = UnifiedPolicyErrorCode.PolicySyncTimeout;
					empty = string.Empty;
					lastErrorTime = DateTime.UtcNow;
					additionalDiagnostics = (this.includeDiagnosticInfo ? Strings.DiagnoseMissingStatusForScope(storage.WhenChangedUTC.Value) : string.Empty);
				}
				else if (storage.WhenChangedUTC != null)
				{
					DateTime dateTime = storage.WhenChangedUTC.Value.Add(PolicySettingStatusHelpers.policySyncTimeoutInterval);
					if (dateTime < DateTime.UtcNow)
					{
						flag = true;
						errorCode = UnifiedPolicyErrorCode.PolicySyncTimeout;
						empty = string.Empty;
						lastErrorTime = dateTime;
						additionalDiagnostics = (this.includeDiagnosticInfo ? Strings.DiagnosePendingStatusTimeout(storage.WhenChangedUTC.Value, PolicySettingStatusHelpers.policySyncTimeoutInterval) : string.Empty);
					}
				}
			}
			else if (status.ErrorCode != 0)
			{
				flag = true;
				errorCode = (UnifiedPolicyErrorCode)status.ErrorCode;
				empty = string.Empty;
				lastErrorTime = status.WhenProcessedUTC;
				additionalDiagnostics = (this.includeDiagnosticInfo ? status.AdditionalDiagnostics : string.Empty);
			}
			if (flag)
			{
				return new PolicyDistributionErrorDetails(this.GetStorageObjectName(storage), storage.Guid, PolicySettingStatusHelpers.GetConfigurationObjectTypeFromString(storage.GetType().Name), singleWorkload, errorCode, empty, lastErrorTime, additionalDiagnostics);
			}
			return null;
		}

		internal bool CalculatePolicyDistributionStatus(UnifiedPolicyStorageBase storage, Workload appliedWorkload, IEnumerable<UnifiedPolicySettingStatus> statuses, ref List<PolicyDistributionErrorDetails> distributionErrors, ref DateTime? lastStatusUpdateTime)
		{
			bool flag = false;
			Dictionary<Workload, UnifiedPolicySettingStatus> dictionary = new Dictionary<Workload, UnifiedPolicySettingStatus>();
			Guid policyVersion = storage.PolicyVersion;
			foreach (UnifiedPolicySettingStatus unifiedPolicySettingStatus in statuses)
			{
				this.WriteVerbose(Strings.VerboseDumpStatusObject(unifiedPolicySettingStatus.Container, unifiedPolicySettingStatus.SettingType, storage.Guid.ToString(), storage.PolicyVersion.ToString(), ((UnifiedPolicyErrorCode)unifiedPolicySettingStatus.ErrorCode).ToString(), unifiedPolicySettingStatus.ObjectVersion.ToString()), false);
				if (unifiedPolicySettingStatus.ObjectVersion == policyVersion)
				{
					Workload workloadFromString = PolicySettingStatusHelpers.GetWorkloadFromString(unifiedPolicySettingStatus.Container);
					if (!dictionary.ContainsKey(workloadFromString))
					{
						dictionary.Add(workloadFromString, unifiedPolicySettingStatus);
					}
				}
			}
			IEnumerable<Workload> enumerable = Enum.GetValues(typeof(Workload)).Cast<Workload>();
			foreach (Workload workload in enumerable)
			{
				if (workload != Workload.None && appliedWorkload.HasFlag(workload))
				{
					UnifiedPolicySettingStatus unifiedPolicySettingStatus2;
					dictionary.TryGetValue(workload, out unifiedPolicySettingStatus2);
					PolicyDistributionErrorDetails policyDistributionErrorDetails = this.CreatePolicyDistributionError(workload, storage, unifiedPolicySettingStatus2);
					flag = (flag || (unifiedPolicySettingStatus2 == null && policyDistributionErrorDetails == null));
					if (policyDistributionErrorDetails != null)
					{
						distributionErrors.Add(policyDistributionErrorDetails);
					}
					if (unifiedPolicySettingStatus2 != null && (lastStatusUpdateTime == null || lastStatusUpdateTime.Value < unifiedPolicySettingStatus2.WhenProcessedUTC))
					{
						lastStatusUpdateTime = new DateTime?(unifiedPolicySettingStatus2.WhenProcessedUTC);
					}
				}
			}
			return flag;
		}

		private bool CalculatePolicyDistributionStatus(IEnumerable<UnifiedPolicyStorageBase> storageObjects, Workload? appliedWorkload, ref List<PolicyDistributionErrorDetails> distributionErrors, ref DateTime? lastStatusUpdateTime)
		{
			bool result = false;
			if (storageObjects != null)
			{
				foreach (UnifiedPolicyStorageBase unifiedPolicyStorageBase in storageObjects)
				{
					IEnumerable<UnifiedPolicySettingStatus> statuses = PolicySettingStatusHelpers.LoadSyncStatuses(this.dataSession, Utils.GetUniversalIdentity(unifiedPolicyStorageBase), unifiedPolicyStorageBase.GetType().Name);
					if (this.CalculatePolicyDistributionStatus(unifiedPolicyStorageBase, (appliedWorkload != null) ? appliedWorkload.Value : unifiedPolicyStorageBase.Workload, statuses, ref distributionErrors, ref lastStatusUpdateTime))
					{
						result = true;
					}
				}
			}
			return result;
		}

		private PolicyApplyStatus CalculatePolicyDistributionStatus(PolicyStorage policyStorage, IList<BindingStorage> bindingStorages, IConfigDataProvider dataSession, out List<PolicyDistributionErrorDetails> distributionErrors, out DateTime? lastStatusUpdateTime)
		{
			this.WriteVerbose(Strings.VerboseBeginCalculatePolicyDistributionStatus(policyStorage.Name), true);
			distributionErrors = new List<PolicyDistributionErrorDetails>();
			lastStatusUpdateTime = null;
			bool flag = false;
			int warningCount = 0;
			flag = this.CalculatePolicyDistributionStatus(new List<UnifiedPolicyStorageBase>(new UnifiedPolicyStorageBase[]
			{
				policyStorage
			}), null, ref distributionErrors, ref lastStatusUpdateTime);
			flag = (flag || this.CalculatePolicyDistributionStatus(bindingStorages, null, ref distributionErrors, ref lastStatusUpdateTime));
			if (!flag)
			{
				IList<RuleStorage> storageObjects = Utils.LoadRuleStoragesByPolicy(dataSession, policyStorage, Utils.GetRootId(dataSession));
				if (this.CalculatePolicyDistributionStatus(storageObjects, null, ref distributionErrors, ref lastStatusUpdateTime))
				{
					flag = true;
				}
			}
			distributionErrors.ForEach(delegate(PolicyDistributionErrorDetails errorDetails)
			{
				BindingStorage bindingStorage2 = bindingStorages.FirstOrDefault((BindingStorage binding) => binding.Workload == errorDetails.Workload);
				if (bindingStorage2 == null || !bindingStorage2.AppliedScopes.Any<ScopeStorage>())
				{
					errorDetails.Severity = PolicyDistributionResultSeverity.Warning;
					errorDetails.AppendAdditionalDiagnosticsInfo(errorDetails.ResultMessage);
					errorDetails.ResultMessage = Strings.DeploymentFailureWithNoImpact;
					warningCount++;
					this.WriteVerbose(Strings.VerboseTreatAsWarning(errorDetails.Endpoint, errorDetails.ObjectType.ToString(), errorDetails.Workload.ToString()), false);
				}
			});
			List<PolicyDistributionErrorDetails> collection = new List<PolicyDistributionErrorDetails>();
			if (!flag)
			{
				foreach (BindingStorage bindingStorage in bindingStorages)
				{
					this.CalculatePolicyDistributionStatus(bindingStorage.AppliedScopes, new Workload?(bindingStorage.Workload), ref collection, ref lastStatusUpdateTime);
				}
			}
			List<PolicyDistributionErrorDetails> list = distributionErrors.FindAll((PolicyDistributionErrorDetails errorDetails) => errorDetails.ResultCode != UnifiedPolicyErrorCode.PolicySyncTimeout);
			int timeoutErrorCount = distributionErrors.Count - list.Count;
			PolicyApplyStatus policyApplyStatus;
			if (flag || (list.Count != distributionErrors.Count && lastStatusUpdateTime != null && lastStatusUpdateTime.Value.Add(PolicySettingStatusHelpers.policySyncTimeoutInterval) > DateTime.UtcNow))
			{
				policyApplyStatus = PolicyApplyStatus.Pending;
				lastStatusUpdateTime = null;
				distributionErrors = list;
				timeoutErrorCount = 0;
			}
			else
			{
				if (warningCount == distributionErrors.Count)
				{
					distributionErrors.AddRange(collection);
				}
				policyApplyStatus = ((warningCount < distributionErrors.Count) ? PolicyApplyStatus.Error : PolicyApplyStatus.Success);
			}
			this.WriteVerbose(Strings.VerboseEndCalculatePolicyDistributionStatus(policyStorage.Name, policyApplyStatus.ToString(), distributionErrors.Count, timeoutErrorCount), true);
			return policyApplyStatus;
		}

		public static PolicyApplyStatus GetPolicyDistributionStatus(PolicyStorage policyStorage, IList<BindingStorage> bindingStorages, IConfigDataProvider dataSession, out List<PolicyDistributionErrorDetails> distributionErrors, out DateTime? lastStatusUpdateTime)
		{
			ArgumentValidator.ThrowIfNull("policyStorage", policyStorage);
			ArgumentValidator.ThrowIfNull("bindingStorages", bindingStorages);
			ArgumentValidator.ThrowIfNull("dataSession", dataSession);
			PolicySettingStatusHelpers policySettingStatusHelpers = new PolicySettingStatusHelpers(dataSession, null, null);
			return policySettingStatusHelpers.CalculatePolicyDistributionStatus(policyStorage, bindingStorages, dataSession, out distributionErrors, out lastStatusUpdateTime);
		}

		public static void PopulatePolicyDistributionStatus(PsCompliancePolicyBase psPolicy, PolicyStorage policyStorage, IConfigDataProvider dataSession, Task task = null, ExecutionLog logger = null)
		{
			ArgumentValidator.ThrowIfNull("psPolicy", psPolicy);
			ArgumentValidator.ThrowIfNull("policyStorage", policyStorage);
			ArgumentValidator.ThrowIfNull("dataSession", dataSession);
			PolicySettingStatusHelpers policySettingStatusHelpers = new PolicySettingStatusHelpers(dataSession, task, logger);
			List<PolicyDistributionErrorDetails> value;
			DateTime? lastStatusUpdateTime;
			psPolicy.DistributionStatus = policySettingStatusHelpers.CalculatePolicyDistributionStatus(policyStorage, psPolicy.StorageBindings, dataSession, out value, out lastStatusUpdateTime);
			psPolicy.LastStatusUpdateTime = lastStatusUpdateTime;
			psPolicy.DistributionResults = new MultiValuedProperty<PolicyDistributionErrorDetails>(value);
		}

		public static IEnumerable<UnifiedPolicySettingStatus> LoadSyncStatuses(IConfigDataProvider dataSession, Guid objectId, string typeName)
		{
			return dataSession.Find<UnifiedPolicySettingStatus>(QueryFilter.AndTogether(new QueryFilter[]
			{
				new ComparisonFilter(ComparisonOperator.Equal, UnifiedPolicySettingStatusSchema.SettingType, typeName),
				new ComparisonFilter(ComparisonOperator.Equal, UnifiedPolicySettingStatusSchema.ObjectId, objectId)
			}), null, false, null).Cast<UnifiedPolicySettingStatus>();
		}

		public static void CheckNotificationResultsAndUpdateStatus(Task task, IConfigurationSession configurationSession, IEnumerable<ChangeNotificationData> notificationResults)
		{
			IEnumerable<ChangeNotificationData> source = from r in notificationResults
			where r.ErrorCode != UnifiedPolicyErrorCode.Success
			select r;
			if (!source.Any<ChangeNotificationData>())
			{
				return;
			}
			string text = configurationSession.GetOrgContainer().OrganizationId.ToExternalDirectoryOrganizationId();
			Guid tenantId;
			if (!Guid.TryParse(text, out tenantId))
			{
				task.WriteWarning(Strings.WarningInvalidTenant(text));
				return;
			}
			List<UnifiedPolicyStatus> source2 = (from failureResult in source
			select new UnifiedPolicyStatus
			{
				ErrorCode = failureResult.ErrorCode,
				ErrorMessage = Strings.PolicyNotifyErrorErrorMsg,
				AdditionalDiagnostics = failureResult.ErrorMessage,
				ObjectId = failureResult.Id,
				ObjectType = failureResult.ObjectType,
				ParentObjectId = new Guid?(failureResult.ParentId),
				TenantId = tenantId,
				Version = failureResult.Version,
				WhenProcessedUTC = DateTime.UtcNow,
				Workload = failureResult.Workload
			}).ToList<UnifiedPolicyStatus>();
			IUnifiedPolicyStatusPublisher unifiedPolicyStatusPublisher = HygieneUtils.InstantiateType<IUnifiedPolicyStatusPublisher>("Microsoft.Exchange.Hygiene.Data.Directory.UnifiedPolicy.UnifiedPolicySession");
			using (IEnumerator enumerator = Enum.GetValues(typeof(Workload)).GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					Workload workload = (Workload)enumerator.Current;
					IEnumerable<UnifiedPolicyStatus> source3 = from s in source2
					where s.Workload == workload
					select s;
					if (source3.Any<UnifiedPolicyStatus>())
					{
						try
						{
							unifiedPolicyStatusPublisher.PublishStatus(source3.Cast<object>(), false);
						}
						catch (Exception ex)
						{
							task.WriteWarning(Strings.WarningFailurePublishingStatus(ex.Message));
						}
					}
				}
			}
		}

		private static TimeSpan policySyncTimeoutInterval = UnifiedPolicyConfiguration.GetInstance().GetPolicyPendingStatusTimeout();

		private readonly bool includeDiagnosticInfo;

		private IConfigDataProvider dataSession;

		private Task task;

		private ExecutionLog logger;
	}
}
