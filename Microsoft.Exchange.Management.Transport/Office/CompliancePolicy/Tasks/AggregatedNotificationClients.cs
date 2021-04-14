using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.ApplicationLogic.UnifiedPolicy;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Directory.UnifiedPolicy;
using Microsoft.Exchange.Data.Storage.UnifiedPolicy;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Management.Transport;
using Microsoft.Office.CompliancePolicy.PolicyConfiguration;
using Microsoft.Office.CompliancePolicy.PolicySync;

namespace Microsoft.Office.CompliancePolicy.Tasks
{
	internal sealed class AggregatedNotificationClients
	{
		public static IList<ChangeNotificationData> NotifyChanges(Task task, IConfigurationSession configurationSession, UnifiedPolicyStorageBase policyStorageObject, ExecutionLog logger, Type client, IEnumerable<UnifiedPolicyStorageBase> bindingStorageObjects = null, IEnumerable<UnifiedPolicyStorageBase> ruleStorageObjects = null)
		{
			ArgumentValidator.ThrowIfNull("task", task);
			ArgumentValidator.ThrowIfNull("configurationSession", configurationSession);
			ArgumentValidator.ThrowIfNull("policyStorageObject", policyStorageObject);
			if (!ExPolicyConfigProvider.IsFFOOnline)
			{
				return new List<ChangeNotificationData>();
			}
			List<UnifiedPolicyStorageBase> list = new List<UnifiedPolicyStorageBase>
			{
				policyStorageObject
			};
			if (bindingStorageObjects != null)
			{
				list.AddRange(bindingStorageObjects);
			}
			if (ruleStorageObjects != null)
			{
				list.AddRange(ruleStorageObjects);
			}
			Dictionary<Workload, List<ChangeNotificationData>> dictionary = AggregatedNotificationClients.NotifyChanges(task, configurationSession, list, logger, client);
			List<ChangeNotificationData> list2 = new List<ChangeNotificationData>();
			foreach (KeyValuePair<Workload, List<ChangeNotificationData>> keyValuePair in dictionary)
			{
				list2.AddRange(keyValuePair.Value);
			}
			return list2;
		}

		internal static string NotifyChangesByWorkload(Task task, IConfigurationSession configurationSession, Workload workload, IEnumerable<SyncChangeInfo> syncChangeInfos, bool fullSync, bool syncNow, ExecutionLog logger, Type client, out string notificationIdentifier)
		{
			Exception exception = null;
			notificationIdentifier = string.Empty;
			string text = string.Empty;
			try
			{
				CompliancePolicySyncNotificationClient compliancePolicySyncNotificationClient = AggregatedNotificationClients.workloadToNotificationClientsGetter[workload](configurationSession, new WriteVerboseDelegate(task.WriteVerbose));
				if (compliancePolicySyncNotificationClient != null)
				{
					task.WriteVerbose(Strings.VerboseNotifyWorkloadWithChanges(workload.ToString(), string.Concat(from syncChangeInfo in syncChangeInfos
					select syncChangeInfo.ToString())));
					notificationIdentifier = compliancePolicySyncNotificationClient.NotifyPolicyConfigChanges(syncChangeInfos, fullSync, syncNow);
					task.WriteVerbose(Strings.VerboseNotifyWorkloadWithChangesSuccess(workload.ToString(), notificationIdentifier));
				}
				else
				{
					text = Strings.WarningNotificationClientIsMissing(workload.ToString());
				}
			}
			catch (CompliancePolicySyncNotificationClientException ex)
			{
				text = Strings.ErrorMessageForNotificationFailure(workload.ToString(), ex.Message);
				exception = ex;
			}
			if (!string.IsNullOrEmpty(text))
			{
				logger.LogOneEntry(client.Name, string.Empty, ExecutionLog.EventType.Warning, string.Format("We failed to notify workload '{0}' with error message '{1}'", workload, text), exception);
				MonitoringItemErrorPublisher.Instance.PublishEvent("UnifiedPolicySync.SendNotificationError", UnifiedPolicyConfiguration.GetInstance().GetOrganizationIdKey(configurationSession), string.Format("Workload={0};Timestamp={1}", workload, DateTime.UtcNow), exception);
			}
			else
			{
				ExecutionLog.EventType eventType = ExecutionLog.EventType.Verbose;
				string name = client.Name;
				string correlationId = notificationIdentifier;
				string format = "Notification '{0}' was sent to workload '{1}' with sync change info: '{2}'";
				object[] array = new object[3];
				array[0] = notificationIdentifier;
				array[1] = workload;
				array[2] = string.Join(",", from x in syncChangeInfos
				select x.ToString());
				logger.LogOneEntry(eventType, name, correlationId, format, array);
			}
			return text;
		}

		private static Dictionary<Workload, List<ChangeNotificationData>> NotifyChanges(Task task, IConfigurationSession configurationSession, IEnumerable<UnifiedPolicyStorageBase> policyStorageObjects, ExecutionLog logger, Type client)
		{
			Dictionary<Workload, List<ChangeNotificationData>> dictionary = AggregatedNotificationClients.CreateSyncChangeDataGroupedByWorkload(policyStorageObjects);
			foreach (KeyValuePair<Workload, List<ChangeNotificationData>> keyValuePair in dictionary)
			{
				IEnumerable<SyncChangeInfo> enumerable = from s in keyValuePair.Value
				where s.ShouldNotify
				select s into d
				select d.CreateSyncChangeInfo(false);
				if (enumerable.Any<SyncChangeInfo>())
				{
					string text2;
					string text = AggregatedNotificationClients.NotifyChangesByWorkload(task, configurationSession, keyValuePair.Key, enumerable, false, false, logger, client, out text2);
					if (!string.IsNullOrEmpty(text))
					{
						task.WriteWarning(Strings.WarningNotifyWorkloadFailed(keyValuePair.Key.ToString()));
					}
					AggregatedNotificationClients.SetNotificationResults(keyValuePair.Value, text);
				}
			}
			return dictionary;
		}

		private static Dictionary<Workload, List<ChangeNotificationData>> CreateSyncChangeDataGroupedByWorkload(IEnumerable<UnifiedPolicyStorageBase> policyStorageObjects)
		{
			Dictionary<Workload, List<ChangeNotificationData>> dictionary = new Dictionary<Workload, List<ChangeNotificationData>>();
			foreach (Workload workload in AggregatedNotificationClients.workloadToNotificationClientsGetter.Keys)
			{
				List<ChangeNotificationData> list = new List<ChangeNotificationData>();
				foreach (UnifiedPolicyStorageBase policyStorageObject in policyStorageObjects)
				{
					ChangeNotificationData changeNotificationData = AggregatedNotificationClients.CreateChangeData(workload, policyStorageObject);
					if (changeNotificationData != null)
					{
						list.Add(changeNotificationData);
					}
				}
				if (list.Any<ChangeNotificationData>())
				{
					dictionary.Add(workload, list);
				}
			}
			return dictionary;
		}

		internal static ChangeNotificationData CreateChangeData(Workload workload, UnifiedPolicyStorageBase policyStorageObject)
		{
			if (policyStorageObject.ObjectState == ObjectState.Unchanged || (policyStorageObject.Workload != Workload.None && (policyStorageObject.Workload & workload) != workload))
			{
				return null;
			}
			Guid parentId = Guid.Empty;
			ConfigurationObjectType objectType = (policyStorageObject is ScopeStorage) ? ConfigurationObjectType.Scope : PolicyConfigConverterTable.GetConfigurationObjectType(policyStorageObject);
			if (policyStorageObject is RuleStorage)
			{
				parentId = ((RuleStorage)policyStorageObject).ParentPolicyId;
			}
			else if (policyStorageObject is BindingStorage)
			{
				parentId = ((BindingStorage)policyStorageObject).PolicyId;
			}
			return new ChangeNotificationData(policyStorageObject.Id.ObjectGuid, parentId, objectType, (policyStorageObject.ObjectState == ObjectState.Deleted) ? ChangeType.Delete : ChangeType.Update, workload, PolicyVersion.Create(policyStorageObject.PolicyVersion), UnifiedPolicyErrorCode.Unknown, "");
		}

		internal static void SetNotificationResults(IEnumerable<ChangeNotificationData> notificationResults, string errorMessage)
		{
			foreach (ChangeNotificationData changeNotificationData in notificationResults)
			{
				changeNotificationData.ErrorCode = (string.IsNullOrEmpty(errorMessage) ? UnifiedPolicyErrorCode.Success : UnifiedPolicyErrorCode.PolicyNotifyError);
				changeNotificationData.ErrorMessage = errorMessage;
			}
		}

		private static readonly Dictionary<Workload, Func<IConfigurationSession, WriteVerboseDelegate, CompliancePolicySyncNotificationClient>> workloadToNotificationClientsGetter = new Dictionary<Workload, Func<IConfigurationSession, WriteVerboseDelegate, CompliancePolicySyncNotificationClient>>
		{
			{
				Workload.Exchange,
				new Func<IConfigurationSession, WriteVerboseDelegate, CompliancePolicySyncNotificationClient>(ExCompliancePolicySyncNotificationClient.Create)
			},
			{
				Workload.SharePoint,
				new Func<IConfigurationSession, WriteVerboseDelegate, CompliancePolicySyncNotificationClient>(SpCompliancePolicySyncNotificationClient.Create)
			}
		};
	}
}
