using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using Microsoft.Exchange.Data.Storage.Management;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage.Infoworker.MailboxSearch
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class SearchEventLogger
	{
		public static SearchEventLogger Instance
		{
			get
			{
				return SearchEventLogger.instance;
			}
		}

		public SearchEventLogger()
		{
			this.logger = new ExEventLog(new Guid("{8E4F12B2-E72A-42b4-816C-30462241203A}"), "MSExchange Mid-Tier Storage");
		}

		public void LogSearchObjectSavedEvent(SearchEventLogger.PropertyLogData logData)
		{
			this.logger.LogEvent(StorageEventLogConstants.Tuple_SearchObjectSaved, null, new object[]
			{
				logData
			});
		}

		public void LogSearchStatusSavedEvent(SearchEventLogger.PropertyLogData logData)
		{
			this.logger.LogEvent(StorageEventLogConstants.Tuple_SearchStatusSaved, null, new object[]
			{
				logData
			});
		}

		public void LogSearchErrorEvent(string searchId, string errorMsg)
		{
			this.logger.LogEvent(StorageEventLogConstants.Tuple_SearchStatusError, null, new object[]
			{
				searchId,
				errorMsg
			});
		}

		public void LogDiscoveryAndHoldSavedEvent(MailboxDiscoverySearch obj)
		{
			SearchEventLogger.PropertyLogData propertyLogData = new SearchEventLogger.PropertyLogData();
			propertyLogData.AddProperty(EwsStoreObjectSchema.Identity.Name, obj.Identity);
			propertyLogData.AddProperty(MailboxDiscoverySearchSchema.StatisticsOnly.Name, obj.StatisticsOnly);
			propertyLogData.AddProperty(MailboxDiscoverySearchSchema.IncludeUnsearchableItems.Name, obj.IncludeUnsearchableItems);
			propertyLogData.AddProperty(MailboxDiscoverySearchSchema.IncludeKeywordStatistics.Name, obj.IncludeKeywordStatistics);
			propertyLogData.AddProperty(MailboxDiscoverySearchSchema.LogLevel.Name, obj.LogLevel);
			propertyLogData.AddProperty(MailboxDiscoverySearchSchema.Query.Name, (obj.Query != null) ? obj.Query.Length : 0);
			propertyLogData.AddProperty(MailboxDiscoverySearchSchema.Language.Name, obj.Language);
			propertyLogData.AddProperty(MailboxDiscoverySearchSchema.CreatedTime.Name, obj.CreatedTime);
			propertyLogData.AddProperty(MailboxDiscoverySearchSchema.LastModifiedTime.Name, obj.LastModifiedTime);
			propertyLogData.AddProperty(MailboxDiscoverySearchSchema.ExcludeDuplicateMessages.Name, obj.ExcludeDuplicateMessages);
			propertyLogData.AddProperty(MailboxDiscoverySearchSchema.InPlaceHoldEnabled.Name, obj.InPlaceHoldEnabled);
			propertyLogData.AddProperty(MailboxDiscoverySearchSchema.ItemHoldPeriod.Name, obj.ItemHoldPeriod);
			propertyLogData.AddProperty(MailboxDiscoverySearchSchema.InPlaceHoldIdentity.Name, obj.InPlaceHoldIdentity);
			propertyLogData.AddProperty(MailboxDiscoverySearchSchema.KeywordHits.Name, (obj.KeywordHits != null) ? obj.KeywordHits.Count : 0);
			propertyLogData.AddOrganization(obj.ManagedByOrganization);
			this.logger.LogEvent(StorageEventLogConstants.Tuple_DiscoveryAndHoldSaved, null, new object[]
			{
				propertyLogData
			});
		}

		public void LogDiscoverySearchRpcServerRestartedEvent(Exception exception)
		{
			SearchEventLogger.PropertyLogData propertyLogData = new SearchEventLogger.PropertyLogData();
			propertyLogData.AddProperty("Exception", exception);
			this.logger.LogEvent(StorageEventLogConstants.Tuple_DiscoverySearchRpcServerRestarted, null, new object[]
			{
				propertyLogData
			});
		}

		public void LogDiscoverySearchPendingWorkItemsChangedEvent(string description, string searchName, string arbitrationMailbox, int pendingItemsCount)
		{
			SearchEventLogger.PropertyLogData propertyLogData = new SearchEventLogger.PropertyLogData();
			propertyLogData.AddProperty("Description", description);
			propertyLogData.AddProperty("ArbitrationMailbox", arbitrationMailbox);
			propertyLogData.AddProperty("PendingWorkItemsCount", pendingItemsCount);
			this.logger.LogEvent(StorageEventLogConstants.Tuple_DiscoverySearchWorkItemQueueChanged, null, new object[]
			{
				propertyLogData
			});
		}

		public void LogDiscoverySearchWorkItemQueueChangedEvent(string description, string searchName, string arbitrationMailbox, string workItemAction, bool isEstimateOnly, int queueLength, int runningWorkItemsCount, int copySearchesInProgressCount, int semaphoreCount = 0)
		{
			SearchEventLogger.PropertyLogData propertyLogData = new SearchEventLogger.PropertyLogData();
			propertyLogData.AddProperty("Description", description);
			propertyLogData.AddProperty("WorkItemAction", workItemAction);
			propertyLogData.AddProperty("IsEstimateOnly", isEstimateOnly);
			propertyLogData.AddProperty("ArbitrationMailbox", arbitrationMailbox);
			propertyLogData.AddProperty("QueueLength", queueLength);
			propertyLogData.AddProperty("RunningWorkItemsCount", runningWorkItemsCount);
			propertyLogData.AddProperty("CopySearchesInProgressCount", copySearchesInProgressCount);
			propertyLogData.AddProperty("SemaphoreCount", semaphoreCount);
			this.logger.LogEvent(StorageEventLogConstants.Tuple_DiscoverySearchWorkItemQueueChanged, null, new object[]
			{
				propertyLogData
			});
		}

		public void LogDiscoverySearchWorkItemQueueNotProcessedEvent(string searchName, string arbitrationMailbox, string workItemAction, bool isEstimateOnly, int queueLength, int runningWorkItemsCount, int copySearchesInProgressCount, int maxRunningCopySearches)
		{
			SearchEventLogger.PropertyLogData propertyLogData = new SearchEventLogger.PropertyLogData();
			propertyLogData.AddProperty("WorkItemAction", workItemAction);
			propertyLogData.AddProperty("IsEstimateOnly", isEstimateOnly);
			propertyLogData.AddProperty("ArbitrationMailbox", arbitrationMailbox);
			propertyLogData.AddProperty("QueueLength", queueLength);
			propertyLogData.AddProperty("RunningWorkItemsCount", runningWorkItemsCount);
			propertyLogData.AddProperty("CopySearchesInProgressCount", copySearchesInProgressCount);
			propertyLogData.AddProperty("MaxRunningCopySearches", maxRunningCopySearches);
			this.logger.LogEvent(StorageEventLogConstants.Tuple_DiscoverySearchWorkItemQueueNotProcessed, searchName + arbitrationMailbox, new object[]
			{
				propertyLogData
			});
		}

		public void LogDiscoverySearchStatusChangedEvent(MailboxDiscoverySearch obj, string parentCallerInfo, [CallerMemberName] string callerMember = null)
		{
			SearchEventLogger.PropertyLogData propertyLogData = new SearchEventLogger.PropertyLogData();
			propertyLogData.AddProperty("CallerInfo", parentCallerInfo + ": " + callerMember);
			propertyLogData.AddProperty(EwsStoreObjectSchema.Identity.Name, obj.Identity);
			propertyLogData.AddProperty(MailboxDiscoverySearchSchema.Status.Name, obj.Status);
			propertyLogData.AddProperty(MailboxDiscoverySearchSchema.StatisticsOnly.Name, obj.StatisticsOnly);
			propertyLogData.AddProperty(MailboxDiscoverySearchSchema.IncludeUnsearchableItems.Name, obj.IncludeUnsearchableItems);
			propertyLogData.AddProperty(MailboxDiscoverySearchSchema.IncludeKeywordStatistics.Name, obj.IncludeKeywordStatistics);
			propertyLogData.AddProperty(MailboxDiscoverySearchSchema.Query.Name, (obj.Query != null) ? obj.Query.Length : 0);
			propertyLogData.AddProperty(MailboxDiscoverySearchSchema.Language.Name, obj.Language);
			propertyLogData.AddProperty(MailboxDiscoverySearchSchema.Sources.Name, obj.Sources);
			propertyLogData.AddProperty(MailboxDiscoverySearchSchema.CreatedTime.Name, obj.CreatedTime);
			propertyLogData.AddProperty(MailboxDiscoverySearchSchema.LastModifiedTime.Name, obj.LastModifiedTime);
			propertyLogData.AddProperty(MailboxDiscoverySearchSchema.ExcludeDuplicateMessages.Name, obj.ExcludeDuplicateMessages);
			propertyLogData.AddProperty(MailboxDiscoverySearchSchema.InPlaceHoldEnabled.Name, obj.InPlaceHoldEnabled);
			propertyLogData.AddProperty(MailboxDiscoverySearchSchema.ItemHoldPeriod.Name, obj.ItemHoldPeriod);
			propertyLogData.AddProperty(MailboxDiscoverySearchSchema.InPlaceHoldIdentity.Name, obj.InPlaceHoldIdentity);
			propertyLogData.AddProperty(MailboxDiscoverySearchSchema.FailedToHoldMailboxes.Name, obj.FailedToHoldMailboxes);
			propertyLogData.AddProperty(MailboxDiscoverySearchSchema.KeywordHits.Name, (obj.KeywordHits != null) ? obj.KeywordHits.Count : 0);
			propertyLogData.AddProperty(MailboxDiscoverySearchSchema.ScenarioId.Name, obj.ScenarioId);
			propertyLogData.AddProperty(MailboxDiscoverySearchSchema.Resume.Name, obj.Resume);
			propertyLogData.AddProperty(MailboxDiscoverySearchSchema.LastStartTime.Name, obj.LastStartTime);
			propertyLogData.AddProperty(MailboxDiscoverySearchSchema.LastEndTime.Name, obj.LastEndTime);
			propertyLogData.AddProperty(MailboxDiscoverySearchSchema.PercentComplete.Name, obj.PercentComplete);
			propertyLogData.AddProperty(MailboxDiscoverySearchSchema.NumberOfMailboxes.Name, obj.NumberOfMailboxes);
			propertyLogData.AddProperty(MailboxDiscoverySearchSchema.ResultItemCountCopied.Name, obj.ResultItemCountCopied);
			propertyLogData.AddProperty(MailboxDiscoverySearchSchema.ResultItemCountEstimate.Name, obj.ResultItemCountEstimate);
			propertyLogData.AddProperty(MailboxDiscoverySearchSchema.ResultSizeCopied.Name, obj.ResultSizeCopied);
			propertyLogData.AddProperty(MailboxDiscoverySearchSchema.ResultSizeEstimate.Name, obj.ResultSizeEstimate);
			propertyLogData.AddProperty(MailboxDiscoverySearchSchema.ResultsPath.Name, obj.ResultsPath);
			propertyLogData.AddProperty(MailboxDiscoverySearchSchema.ResultsLink.Name, obj.ResultsLink);
			propertyLogData.AddProperty(MailboxDiscoverySearchSchema.CompletedMailboxes.Name, obj.CompletedMailboxes);
			propertyLogData.AddOrganization(obj.ManagedByOrganization);
			if (obj.Errors != null && obj.Errors.Count > 0)
			{
				propertyLogData.AddProperty(MailboxDiscoverySearchSchema.Errors.Name, string.Join(",", obj.Errors.ToArray()));
			}
			if (obj.IsFeatureFlighted("SearchStatsFlighted") && obj.SearchStatistics != null && obj.SearchStatistics.Count > 0 && obj.SearchStatistics[0] != null)
			{
				propertyLogData.AddProperty(MailboxDiscoverySearchSchema.SearchStatistics.Name, obj.SearchStatistics[0].ToString());
			}
			propertyLogData.AddProperty("Flighted Features", obj.FlightedFeatures);
			this.logger.LogEvent(StorageEventLogConstants.Tuple_DiscoverySearchStatusChanged, null, new object[]
			{
				propertyLogData
			});
		}

		public void LogInPlaceHoldSettingsSynchronizedEvent(MailboxDiscoverySearch obj)
		{
			SearchEventLogger.PropertyLogData propertyLogData = new SearchEventLogger.PropertyLogData();
			propertyLogData.AddProperty(MailboxDiscoverySearchSchema.InPlaceHoldEnabled.Name, obj.InPlaceHoldEnabled);
			propertyLogData.AddProperty(MailboxDiscoverySearchSchema.ItemHoldPeriod.Name, obj.ItemHoldPeriod);
			propertyLogData.AddProperty(MailboxDiscoverySearchSchema.InPlaceHoldErrors.Name, obj.InPlaceHoldErrors);
			propertyLogData.AddOrganization(obj.ManagedByOrganization);
			this.logger.LogEvent(StorageEventLogConstants.Tuple_InPlaceHoldSettingsSynchronized, null, new object[]
			{
				propertyLogData
			});
			if (obj.InPlaceHoldErrors != null && obj.InPlaceHoldErrors.Count > 0)
			{
				foreach (string stringToEscape in obj.InPlaceHoldErrors)
				{
					string value = Uri.EscapeDataString(stringToEscape);
					SearchEventLogger.PropertyLogData propertyLogData2 = new SearchEventLogger.PropertyLogData();
					propertyLogData2.AddOrganization(obj.ManagedByOrganization);
					propertyLogData2.AddProperty("Error", value);
					this.logger.LogEvent(StorageEventLogConstants.Tuple_SynchronizeInPlaceHoldError, null, new object[]
					{
						propertyLogData2
					});
				}
			}
		}

		public void LogDiscoverySearchStartRequestedEvent(MailboxDiscoverySearch obj, string rbacContext)
		{
			SearchEventLogger.PropertyLogData propertyLogData = new SearchEventLogger.PropertyLogData();
			propertyLogData.AddProperty(MailboxDiscoverySearchSchema.Resume.Name, obj.Resume);
			propertyLogData.AddProperty("RbacContext", rbacContext);
			propertyLogData.AddOrganization(obj.ManagedByOrganization);
			this.logger.LogEvent(StorageEventLogConstants.Tuple_DiscoverySearchStartRequested, null, new object[]
			{
				propertyLogData
			});
		}

		public void LogDiscoverySearchStopRequestedEvent(MailboxDiscoverySearch obj, string rbacContext)
		{
			SearchEventLogger.PropertyLogData propertyLogData = new SearchEventLogger.PropertyLogData();
			propertyLogData.AddProperty("RbacContext", rbacContext);
			propertyLogData.AddOrganization(obj.ManagedByOrganization);
			this.logger.LogEvent(StorageEventLogConstants.Tuple_DiscoverySearchStopRequested, null, new object[]
			{
				propertyLogData
			});
		}

		public void LogDiscoverySearchRemoveRequestedEvent(MailboxDiscoverySearch obj, string rbacContext)
		{
			SearchEventLogger.PropertyLogData propertyLogData = new SearchEventLogger.PropertyLogData();
			propertyLogData.AddProperty("RbacContext", rbacContext);
			propertyLogData.AddOrganization(obj.ManagedByOrganization);
			this.logger.LogEvent(StorageEventLogConstants.Tuple_DiscoverySearchRemoveRequested, null, new object[]
			{
				propertyLogData
			});
		}

		public void LogDiscoverySearchRequestPickedUpEvent(string name, string actionType, string rbacContext, string orgId)
		{
			SearchEventLogger.PropertyLogData propertyLogData = new SearchEventLogger.PropertyLogData();
			propertyLogData.AddProperty("RbacContext", rbacContext);
			propertyLogData.AddOrganization(orgId);
			this.logger.LogEvent(StorageEventLogConstants.Tuple_DiscoverySearchRequestPickedUp, null, new object[]
			{
				propertyLogData
			});
		}

		public void LogDiscoverySearchStartRequestProcessedEvent(string name, string rbacContext, string orgId)
		{
			SearchEventLogger.PropertyLogData propertyLogData = new SearchEventLogger.PropertyLogData();
			propertyLogData.AddProperty("RbacContext", rbacContext);
			propertyLogData.AddOrganization(orgId);
			this.logger.LogEvent(StorageEventLogConstants.Tuple_DiscoverySearchStartRequestProcessed, null, new object[]
			{
				propertyLogData
			});
		}

		public void LogDiscoverySearchStopRequestProcessedEvent(string name, string rbacContext, string orgId)
		{
			SearchEventLogger.PropertyLogData propertyLogData = new SearchEventLogger.PropertyLogData();
			propertyLogData.AddProperty("RbacContext", rbacContext);
			propertyLogData.AddOrganization(orgId);
			this.logger.LogEvent(StorageEventLogConstants.Tuple_DiscoverySearchStopRequestProcessed, null, new object[]
			{
				propertyLogData
			});
		}

		public void LogDiscoverySearchRemoveRequestProcessedEvent(string name, string rbacContext, string orgId)
		{
			SearchEventLogger.PropertyLogData propertyLogData = new SearchEventLogger.PropertyLogData();
			propertyLogData.AddProperty("RbacContext", rbacContext);
			propertyLogData.AddOrganization(orgId);
			this.logger.LogEvent(StorageEventLogConstants.Tuple_DiscoverySearchRemoveRequestProcessed, null, new object[]
			{
				propertyLogData
			});
		}

		public void LogDiscoverySearchServerErrorEvent(string description, string name, string arbitrationMailbox, string errorMsg)
		{
			SearchEventLogger.PropertyLogData propertyLogData = new SearchEventLogger.PropertyLogData();
			propertyLogData.AddProperty("Description", description);
			propertyLogData.AddProperty("ArbitrationMailbox", arbitrationMailbox);
			propertyLogData.AddProperty("Error", errorMsg);
			this.logger.LogEvent(StorageEventLogConstants.Tuple_DiscoverySearchServerError, null, new object[]
			{
				propertyLogData
			});
		}

		public void LogDiscoverySearchServerErrorEvent(string description, string name, string arbitrationMailbox, Exception exception)
		{
			SearchEventLogger.PropertyLogData propertyLogData = new SearchEventLogger.PropertyLogData();
			propertyLogData.AddProperty("Description", description);
			propertyLogData.AddProperty("ArbitrationMailbox", arbitrationMailbox);
			propertyLogData.AddProperty("Exception", exception);
			this.logger.LogEvent(StorageEventLogConstants.Tuple_DiscoverySearchServerError, null, new object[]
			{
				propertyLogData
			});
		}

		public void LogDiscoverySearchTaskErrorEvent(string name, string organizationHint, string errorMsg)
		{
			SearchEventLogger.PropertyLogData propertyLogData = new SearchEventLogger.PropertyLogData();
			propertyLogData.AddProperty("OrganizationHint", organizationHint);
			propertyLogData.AddProperty("Error", errorMsg);
			this.logger.LogEvent(StorageEventLogConstants.Tuple_DiscoverySearchTaskError, null, new object[]
			{
				propertyLogData
			});
		}

		public void LogDiscoverySearchTaskErrorEvent(string name, string organizationHint, Exception exception)
		{
			SearchEventLogger.PropertyLogData propertyLogData = new SearchEventLogger.PropertyLogData();
			propertyLogData.AddProperty("OrganizationHint", organizationHint);
			propertyLogData.AddProperty("Exception", exception);
			this.logger.LogEvent(StorageEventLogConstants.Tuple_DiscoverySearchTaskError, null, new object[]
			{
				propertyLogData
			});
		}

		public void LogDiscoverySearchTaskStartedEvent(string name, string organization)
		{
			SearchEventLogger.PropertyLogData propertyLogData = new SearchEventLogger.PropertyLogData();
			propertyLogData.AddOrganization(organization);
			this.logger.LogEvent(StorageEventLogConstants.Tuple_DiscoverySearchTaskStarted, null, new object[]
			{
				propertyLogData
			});
		}

		public void LogDiscoverySearchTaskCompletedEvent(string name, string organization, string status)
		{
			SearchEventLogger.PropertyLogData propertyLogData = new SearchEventLogger.PropertyLogData();
			propertyLogData.AddProperty("CompletedStatus", status);
			propertyLogData.AddOrganization(organization);
			this.logger.LogEvent(StorageEventLogConstants.Tuple_DiscoverySearchTaskCompleted, null, new object[]
			{
				propertyLogData
			});
		}

		public void LogFailedToSyncDiscoveryHoldToExchangeOnlineEvent(object e)
		{
			this.logger.LogEvent(StorageEventLogConstants.Tuple_FailedToSyncDiscoveryHoldToExchangeOnline, null, new object[]
			{
				e
			});
		}

		public void LogSyncDiscoveryHoldToExchangeOnlineStartEvent(string mailbox)
		{
			this.logger.LogEvent(StorageEventLogConstants.Tuple_SyncDiscoveryHoldToExchangeOnlineStart, null, new object[]
			{
				mailbox
			});
		}

		public void LogSyncDiscoveryHoldToExchangeOnlineDetailsEvent(int numOfHolds, string remoteAddress)
		{
			SearchEventLogger.PropertyLogData propertyLogData = new SearchEventLogger.PropertyLogData();
			propertyLogData.AddProperty("NumberOfHolds", numOfHolds);
			propertyLogData.AddProperty("RemoteAddress", remoteAddress);
			this.logger.LogEvent(StorageEventLogConstants.Tuple_SyncDiscoveryHoldToExchangeOnlineDetails, null, new object[]
			{
				propertyLogData
			});
		}

		public void LogSingleFailureSyncDiscoveryHoldToExchangeOnlineEvent(string searchId, string holdAction, string query, string inPlaceHoldIdentity, string errorCode, string errorMessage)
		{
			SearchEventLogger.PropertyLogData propertyLogData = new SearchEventLogger.PropertyLogData();
			propertyLogData.AddProperty("HoldAction", holdAction);
			propertyLogData.AddProperty("InPlaceHoldIdentity", inPlaceHoldIdentity);
			propertyLogData.AddProperty("ErrorCode", errorCode);
			propertyLogData.AddProperty("ErrorMessage", errorMessage);
			this.logger.LogEvent(StorageEventLogConstants.Tuple_SingleFailureSyncDiscoveryHoldToExchangeOnline, null, new object[]
			{
				propertyLogData
			});
		}

		public const string EventLogSourceName = "MSExchange Mid-Tier Storage";

		private static SearchEventLogger instance = new SearchEventLogger();

		private ExEventLog logger;

		public class PropertyLogData
		{
			public PropertyLogData()
			{
				this.propertyBag = new Dictionary<string, string>();
			}

			public void AddProperty(string name, object value)
			{
				if (string.IsNullOrEmpty(name))
				{
					throw new ArgumentNullException("name");
				}
				string text = string.Empty;
				if (value != null)
				{
					if (value is ICollection)
					{
						text = ((ICollection)value).Count.ToString();
					}
					else
					{
						text = value.ToString();
					}
				}
				if (text.Length > 10000)
				{
					text = text.Substring(0, 10000);
				}
				if (this.propertyBag.ContainsKey(name))
				{
					this.propertyBag[name] = text;
					return;
				}
				this.propertyBag.Add(name, text);
			}

			public override string ToString()
			{
				StringBuilder stringBuilder = new StringBuilder();
				foreach (KeyValuePair<string, string> keyValuePair in this.propertyBag)
				{
					stringBuilder.Append('&');
					stringBuilder.Append(keyValuePair.Key);
					stringBuilder.Append('=');
					stringBuilder.Append((keyValuePair.Value.IndexOfAny(new char[]
					{
						'&',
						'='
					}) >= 0) ? Uri.EscapeDataString(keyValuePair.Value) : keyValuePair.Value);
				}
				int num = (stringBuilder.Length > 0) ? 1 : 0;
				return stringBuilder.ToString(num, (stringBuilder.Length >= 32767) ? 32766 : (stringBuilder.Length - num));
			}

			public void AddOrganization(string org)
			{
				this.AddProperty("Organization", org);
			}

			public void AddSearchObject(SearchObject obj)
			{
				if (obj == null)
				{
					throw new ArgumentNullException("obj");
				}
				this.AddProperty(SearchObjectBaseSchema.Id.Name, obj.Id);
				this.AddProperty(SearchObjectSchema.Language.Name, obj.Language);
				this.AddProperty(SearchObjectSchema.StartDate.Name, obj.StartDate);
				this.AddProperty(SearchObjectSchema.EndDate.Name, obj.EndDate);
				this.AddProperty(SearchObjectSchema.MessageTypes.Name, obj.MessageTypes);
				this.AddProperty(SearchObjectSchema.SearchDumpster.Name, obj.SearchDumpster);
				this.AddProperty(SearchObjectSchema.LogLevel.Name, obj.LogLevel);
				this.AddProperty(SearchObjectSchema.IncludeUnsearchableItems.Name, obj.IncludeUnsearchableItems);
				this.AddProperty(SearchObjectSchema.IncludePersonalArchive.Name, obj.IncludePersonalArchive);
				this.AddProperty(SearchObjectSchema.IncludeRemoteAccounts.Name, obj.IncludeRemoteAccounts);
				this.AddProperty(SearchObjectSchema.StatusMailRecipients.Name, obj.StatusMailRecipients);
				this.AddProperty(SearchObjectSchema.EstimateOnly.Name, obj.EstimateOnly);
				this.AddProperty(SearchObjectSchema.ExcludeDuplicateMessages.Name, obj.ExcludeDuplicateMessages);
				this.AddProperty(SearchObjectSchema.Resume.Name, obj.Resume);
				this.AddProperty(SearchObjectSchema.SearchQuery.Name, obj.SearchQuery.Length);
				this.AddProperty(SearchObjectSchema.IncludeKeywordStatistics.Name, obj.IncludeKeywordStatistics);
			}

			public void AddSearchStatus(SearchStatus status)
			{
				if (status == null)
				{
					throw new ArgumentNullException("status");
				}
				this.AddProperty(SearchStatusSchema.Status.Name, status.Status);
				this.AddProperty(SearchStatusSchema.LastStartTime.Name, status.LastStartTime);
				this.AddProperty(SearchStatusSchema.LastEndTime.Name, status.LastEndTime);
				this.AddProperty(SearchStatusSchema.NumberMailboxesToSearch.Name, status.NumberMailboxesToSearch);
				this.AddProperty(SearchStatusSchema.PercentComplete.Name, status.PercentComplete);
				this.AddProperty(SearchStatusSchema.ResultNumber.Name, status.ResultNumber);
				this.AddProperty(SearchStatusSchema.ResultSize.Name, status.ResultSize);
				this.AddProperty(SearchStatusSchema.ResultSizeEstimate.Name, status.ResultSizeEstimate);
				this.AddProperty(SearchStatusSchema.ResultSizeCopied.Name, status.ResultSizeCopied);
				this.AddProperty(SearchStatusSchema.Errors.Name, status.Errors);
				this.AddProperty(SearchStatusSchema.KeywordHits.Name, status.KeywordHits);
				this.AddProperty(SearchStatusSchema.CompletedMailboxes.Name, status.CompletedMailboxes);
			}

			private Dictionary<string, string> propertyBag;
		}
	}
}
