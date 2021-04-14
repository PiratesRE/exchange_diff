using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory.SystemConfiguration.ConfigurationSettings;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.Management;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.MailboxReplicationService;
using Microsoft.Exchange.Migration.Logging;

namespace Microsoft.Exchange.Migration
{
	internal sealed class MigrationJobItem : MigrationMessagePersistableBase
	{
		private MigrationJobItem(MigrationType migrationType)
		{
			this.currentSupportedVersion = 3L;
			this.MigrationType = migrationType;
		}

		public string Identifier { get; private set; }

		public string LocalMailboxIdentifier { get; private set; }

		public MigrationType MigrationType { get; private set; }

		public int CursorPosition { get; private set; }

		public MigrationUserStatus Status
		{
			get
			{
				return this.status;
			}
		}

		public MigrationState State
		{
			get
			{
				return this.StatusData.State;
			}
		}

		public MigrationFlags Flags { get; private set; }

		public MigrationWorkflowPosition WorkflowPosition { get; private set; }

		public MigrationStep[] SupportedSteps
		{
			get
			{
				if (this.MigrationType == MigrationType.ExchangeOutlookAnywhere)
				{
					switch (this.RecipientType)
					{
					case MigrationUserRecipientType.Mailbox:
						return new MigrationStep[]
						{
							MigrationStep.Initialization,
							MigrationStep.Provisioning,
							MigrationStep.ProvisioningUpdate,
							MigrationStep.DataMigration
						};
					case MigrationUserRecipientType.Contact:
					case MigrationUserRecipientType.Group:
					case MigrationUserRecipientType.Mailuser:
						return new MigrationStep[]
						{
							MigrationStep.Initialization,
							MigrationStep.Provisioning,
							MigrationStep.ProvisioningUpdate
						};
					}
					throw new NotSupportedException("didn't expect recipient type");
				}
				return new MigrationStep[]
				{
					MigrationStep.Initialization,
					MigrationStep.Provisioning,
					MigrationStep.ProvisioningUpdate,
					MigrationStep.DataMigration
				};
			}
		}

		public MigrationUserRecipientType RecipientType { get; private set; }

		public MigrationStatusData<MigrationUserStatus> StatusData
		{
			get
			{
				return this.statusData;
			}
			private set
			{
				this.statusData = value;
				if (this.statusData != null)
				{
					this.status = this.statusData.Status;
				}
			}
		}

		public ExDateTime? StateLastUpdated
		{
			get
			{
				return this.statusData.StateLastUpdated;
			}
		}

		public LocalizedString? LocalizedError
		{
			get
			{
				return this.statusData.LocalizedError;
			}
		}

		public Guid MigrationJobId
		{
			get
			{
				return this.migrationJobId;
			}
			private set
			{
				this.migrationJobId = value;
			}
		}

		public IMailboxData LocalMailbox
		{
			get
			{
				return this.localMailbox;
			}
			private set
			{
				this.localMailbox = value;
			}
		}

		public string RemoteIdentifier { get; private set; }

		public ProvisioningDataStorageBase ProvisioningData { get; private set; }

		public ProvisioningSnapshot ProvisioningStatistics { get; private set; }

		public ProvisioningId ProvisioningId
		{
			get
			{
				return new ProvisioningId(this.JobItemGuid, this.MigrationJobId);
			}
		}

		public ISubscriptionSettings SubscriptionSettings { get; private set; }

		public ISubscriptionStatistics SubscriptionStatistics { get; private set; }

		public ISubscriptionId SubscriptionId { get; private set; }

		public ExDateTime? SubscriptionLastChecked
		{
			get
			{
				return this.subscriptionLastChecked;
			}
			private set
			{
				this.subscriptionLastChecked = value;
			}
		}

		public ExDateTime? SubscriptionSettingsLastUpdatedTime { get; private set; }

		public long ItemsSynced
		{
			get
			{
				if (this.SubscriptionStatistics != null)
				{
					return this.SubscriptionStatistics.NumItemsSynced;
				}
				return 0L;
			}
		}

		public long ItemsSkipped
		{
			get
			{
				if (this.SubscriptionStatistics != null)
				{
					return this.SubscriptionStatistics.NumItemsSkipped;
				}
				return 0L;
			}
		}

		public MigrationJob MigrationJob { get; private set; }

		public bool SupportsAdvancedValidation
		{
			get
			{
				return this.MigrationType == MigrationType.ExchangeLocalMove || this.MigrationType == MigrationType.ExchangeRemoteMove || this.MigrationType == MigrationType.PublicFolder;
			}
		}

		public bool ShouldProvision
		{
			get
			{
				return this.MigrationType == MigrationType.ExchangeOutlookAnywhere || this.MigrationType == MigrationType.XO1;
			}
		}

		public bool ShouldMigrate
		{
			get
			{
				return (this.MigrationType != MigrationType.ExchangeOutlookAnywhere || this.RecipientType == MigrationUserRecipientType.Mailbox) && this.MigrationType != MigrationType.XO1;
			}
		}

		public override PropertyDefinition[] PropertyDefinitions
		{
			get
			{
				string key = this.MigrationType.ToString() + (this.IsPAW ? "PAW" : "non-PAW");
				PropertyDefinition[] array;
				if (!MigrationJobItem.PropertyDefinitionsHash.TryGetValue(key, out array))
				{
					array = MigrationHelper.AggregateProperties(new IList<PropertyDefinition>[]
					{
						MigrationJobItem.MigrationJobItemColumnsIndex,
						this.MailboxDataPropertyDefinitions,
						this.ProvisioningDataPropertyDefinitions,
						this.SubscriptionIdPropertyDefinitions,
						this.SubscriptionSettingsPropertyDefinitions,
						new PropertyDefinition[]
						{
							this.CursorPositionPropertyDefinition
						},
						this.MigrationJobSlotPropertyDefinitions
					});
					MigrationJobItem.PropertyDefinitionsHash[key] = array;
				}
				return array;
			}
		}

		public override PropertyDefinition[] InitializationPropertyDefinitions
		{
			get
			{
				return MigrationJobItem.MigrationJobItemColumnsTypeIndex;
			}
		}

		public override long MaximumSupportedVersion
		{
			get
			{
				return 4L;
			}
		}

		public override long MinimumSupportedVersion
		{
			get
			{
				return 3L;
			}
		}

		public override long MinimumSupportedPersistableVersion
		{
			get
			{
				return 3L;
			}
		}

		public override long CurrentSupportedVersion
		{
			get
			{
				return this.currentSupportedVersion;
			}
		}

		public bool IsPAW
		{
			get
			{
				return base.Version >= 4L;
			}
		}

		public ExDateTime? SubscriptionDisableTime { get; private set; }

		public string BatchInputId
		{
			get
			{
				return base.ExtendedProperties.Get<string>("BatchInputId", null);
			}
			private set
			{
				base.ExtendedProperties.Set<string>("BatchInputId", value);
			}
		}

		public int LastFinalizationAttempt { get; internal set; }

		public TimeSpan? InitialSyncDuration
		{
			get
			{
				return base.ExtendedProperties.Get<TimeSpan?>("InitialSyncDuration", null);
			}
			private set
			{
				base.ExtendedProperties.Set<TimeSpan?>("InitialSyncDuration", value);
			}
		}

		public TimeSpan? IncrementalSyncDuration
		{
			get
			{
				return base.ExtendedProperties.Get<TimeSpan?>("IncrementalSyncDuration", null);
			}
			private set
			{
				base.ExtendedProperties.Set<TimeSpan?>("IncrementalSyncDuration", value);
			}
		}

		public ExDateTime? ProvisionedTime { get; private set; }

		public ExDateTime? SubscriptionQueuedTime { get; private set; }

		public TimeSpan? OverallCmdletDuration { get; private set; }

		public TimeSpan? SubscriptionInjectionDuration { get; private set; }

		public TimeSpan? ProvisioningDuration { get; private set; }

		public ExDateTime? LastSuccessfulSyncTime
		{
			get
			{
				if (this.SubscriptionStatistics != null)
				{
					return this.SubscriptionStatistics.LastSyncTime;
				}
				return null;
			}
		}

		public ExDateTime? LastRestartTime { get; private set; }

		public ExDateTime? NextProcessTime { get; private set; }

		public int IncrementalSyncFailures
		{
			get
			{
				return base.ExtendedProperties.Get<int>("IncrementalSyncFailures", 0);
			}
			internal set
			{
				base.ExtendedProperties.Set<int>("IncrementalSyncFailures", value);
			}
		}

		public int PublicFolderCompletionFailures
		{
			get
			{
				return base.ExtendedProperties.Get<int>("PublicFolderCompletionFailures", 0);
			}
			internal set
			{
				base.ExtendedProperties.Set<int>("PublicFolderCompletionFailures", value);
			}
		}

		public TimeSpan? IncrementalSyncInterval
		{
			get
			{
				return base.ExtendedProperties.Get<TimeSpan?>("IncrementalSyncInterval", null);
			}
			private set
			{
				TimeSpan? value2 = value;
				if (value2 != null)
				{
					base.ExtendedProperties.Set<TimeSpan?>("IncrementalSyncInterval", value2);
					return;
				}
				base.ExtendedProperties.Remove("IncrementalSyncInterval");
			}
		}

		public string TroubleshooterNotes
		{
			get
			{
				return base.ExtendedProperties.Get<string>("TroubleshooterNotes", null);
			}
			private set
			{
				base.ExtendedProperties.Set<string>("TroubleshooterNotes", value);
			}
		}

		internal string TenantName { get; private set; }

		internal string JobName
		{
			get
			{
				return base.ExtendedProperties.Get<string>("JobName", string.Empty);
			}
			private set
			{
				base.ExtendedProperties.Set<string>("JobName", value);
			}
		}

		internal bool IsStaged
		{
			get
			{
				return base.ExtendedProperties.Get<bool>("IsStaged", false);
			}
			private set
			{
				base.ExtendedProperties.Set<bool>("IsStaged", value);
			}
		}

		internal MigrationSlotType ConsumedSlotType { get; private set; }

		internal Guid MigrationSlotProviderGuid { get; private set; }

		internal Guid JobItemGuid { get; private set; }

		internal MigrationObjectsCount CountSelf
		{
			get
			{
				switch (this.RecipientType)
				{
				case MigrationUserRecipientType.Mailbox:
					return new MigrationObjectsCount(new int?(1));
				case MigrationUserRecipientType.Contact:
					return new MigrationObjectsCount(null, null, new int?(1), false);
				case MigrationUserRecipientType.Group:
					return new MigrationObjectsCount(null, new int?(1), null, false);
				case MigrationUserRecipientType.PublicFolder:
					return new MigrationObjectsCount(null, null, null, true);
				case MigrationUserRecipientType.Mailuser:
				case MigrationUserRecipientType.MailboxOrMailuser:
					if (this.MigrationType == MigrationType.ExchangeRemoteMove || this.MigrationType == MigrationType.ExchangeLocalMove)
					{
						return new MigrationObjectsCount(new int?(1));
					}
					return new MigrationObjectsCount(null, null, new int?(1), false);
				}
				throw new InvalidOperationException("This method should not be invoked if the RecipientType is " + this.RecipientType);
			}
		}

		internal bool SupportsIncrementalSync
		{
			get
			{
				return this.SubscriptionLastChecked != null && this.SubscriptionLastChecked.Value != MigrationJobItem.MaxDateTimeValue;
			}
		}

		private PropertyDefinition CursorPositionPropertyDefinition
		{
			get
			{
				return MigrationJobItem.GetCursorPositionProperty(this.MigrationType);
			}
		}

		private PropertyDefinition[] SubscriptionSettingsPropertyDefinitions
		{
			get
			{
				return JobItemSubscriptionSettingsBase.GetPropertyDefinitions(this.MigrationType);
			}
		}

		private PropertyDefinition[] SubscriptionIdPropertyDefinitions
		{
			get
			{
				return SubscriptionIdHelper.GetPropertyDefinitions(this.MigrationType, this.IsPAW);
			}
		}

		private PropertyDefinition[] MailboxDataPropertyDefinitions
		{
			get
			{
				return MailboxDataHelper.GetPropertyDefinitions(this.MigrationType);
			}
		}

		private PropertyDefinition[] ProvisioningDataPropertyDefinitions
		{
			get
			{
				return ProvisioningDataStorageBase.GetPropertyDefinitions(this.MigrationType);
			}
		}

		private PropertyDefinition[] MigrationJobSlotPropertyDefinitions
		{
			get
			{
				return new StorePropertyDefinition[]
				{
					MigrationBatchMessageSchema.MigrationJobItemSlotType,
					MigrationBatchMessageSchema.MigrationJobItemSlotProviderId
				};
			}
		}

		public static MigrationJobItem Create(IMigrationDataProvider dataProvider, MigrationJob job, MigrationUserStatus status, IMigrationDataRow dataRow, MailboxData mailboxData)
		{
			MigrationUtil.AssertOrThrow(job.MigrationType == dataRow.MigrationType, "Job type is {0} but data row is {1}. They should both be the same.", new object[]
			{
				job.MigrationType,
				dataRow.MigrationType
			});
			MigrationJobItem migrationJobItem = new MigrationJobItem(job.MigrationType);
			migrationJobItem.Initialize(job, dataRow, mailboxData, status, null, null, null);
			migrationJobItem.MigrationJob = job;
			migrationJobItem.CreateInStore(dataProvider, null);
			job.ReportData.Append(Strings.MigrationReportJobItemCreatedInternal(migrationJobItem.Identifier));
			return migrationJobItem;
		}

		public static MigrationJobItem Create(IMigrationDataProvider dataProvider, MigrationJob job, IMigrationDataRow dataRow, MigrationUserStatus status, MigrationState? state = null, MigrationWorkflowPosition position = null)
		{
			MigrationUtil.AssertOrThrow(job.MigrationType == dataRow.MigrationType, "Job type is {0} but data row is {1}. They should both be the same.", new object[]
			{
				job.MigrationType,
				dataRow.MigrationType
			});
			MigrationJobItem migrationJobItem = new MigrationJobItem(job.MigrationType);
			migrationJobItem.Initialize(job, dataRow, null, status, null, state, position);
			migrationJobItem.MigrationJob = job;
			migrationJobItem.CreateInStore(dataProvider, null);
			job.ReportData.Append(Strings.MigrationReportJobItemCreatedInternal(migrationJobItem.Identifier));
			return migrationJobItem;
		}

		public static MigrationJobItem CreateFailed(IMigrationDataProvider dataProvider, MigrationJob job, InvalidDataRow dataRow, MigrationState? state = null, MigrationWorkflowPosition position = null)
		{
			return MigrationJobItem.CreateFailed(dataProvider, job, dataRow, new LocalizedException(dataRow.Error.LocalizedErrorMessage), state, position);
		}

		public static MigrationJobItem CreateFailed(IMigrationDataProvider dataProvider, MigrationJob job, IMigrationDataRow dataRow, LocalizedException localizedError, MigrationState? state = null, MigrationWorkflowPosition position = null)
		{
			MigrationUtil.AssertOrThrow(job.MigrationType == dataRow.MigrationType, "Job type is {0} but data row is {1}. They should both be the same.", new object[]
			{
				job.MigrationType,
				dataRow.MigrationType
			});
			MigrationJobItem migrationJobItem = new MigrationJobItem(job.MigrationType);
			migrationJobItem.Initialize(job, dataRow, null, MigrationUserStatus.Failed, localizedError, state, position);
			migrationJobItem.CreateInStore(dataProvider, null);
			job.ReportData.Append(Strings.MigrationReportJobItemWithError(migrationJobItem.Identifier, localizedError.LocalizedString), localizedError, ReportEntryFlags.Failure | ReportEntryFlags.Fatal | ReportEntryFlags.Target);
			MigrationFailureLog.LogFailureEvent(migrationJobItem, localizedError, MigrationFailureFlags.Fatal, null);
			return migrationJobItem;
		}

		public static IEnumerable<MigrationJobItem> GetMigratableByStateLastUpdated(IMigrationDataProvider provider, MigrationJob job, ExDateTime? stateLastUpdatedCutoff, MigrationUserStatus status, int maxCount)
		{
			MigrationUtil.ThrowOnNullArgument(provider, "provider");
			MigrationUtil.ThrowOnNullArgument(job, "job");
			MigrationJobObjectCache migrationJobObjectCache = new MigrationJobObjectCache(provider);
			migrationJobObjectCache.PreSeed(job);
			MigrationEqualityFilter primaryFilter = new MigrationEqualityFilter(MigrationBatchMessageSchema.MigrationUserStatus, status);
			SortBy[] additionalSorts = new SortBy[]
			{
				new SortBy(MigrationBatchMessageSchema.MigrationJobId, SortOrder.Ascending),
				new SortBy(MigrationBatchMessageSchema.MigrationJobItemStateLastUpdated, SortOrder.Ascending)
			};
			ExDateTime timeNow = ExDateTime.UtcNow;
			IEnumerable<StoreObjectId> messageIdList = provider.FindMessageIds(primaryFilter, MigrationJobItem.MigrationJobItemSubscriptionStateLastUpdated, additionalSorts, delegate(IDictionary<PropertyDefinition, object> rowData)
			{
				object obj;
				if (rowData.TryGetValue(MigrationBatchMessageSchema.MigrationJobItemSubscriptionLastChecked, out obj) && obj is ExDateTime && (ExDateTime)obj > timeNow)
				{
					return MigrationRowSelectorResult.RejectRowContinueProcessing;
				}
				return MigrationJobItem.FilterJobItemsByColumnLastUpdated(rowData, MigrationBatchMessageSchema.MigrationJobItemStateLastUpdated, new Guid?(job.JobId), stateLastUpdatedCutoff, new MigrationUserStatus?(status));
			}, new int?(maxCount));
			return MigrationJobItem.LoadJobItemsWithStatus(provider, messageIdList, status, migrationJobObjectCache);
		}

		public static ExDateTime? GetOldestLastSyncSubscriptionTime(IMigrationDataProvider provider, MigrationType migrationType, Guid jobId)
		{
			MigrationUtil.ThrowOnNullArgument(provider, "provider");
			SortBy[] sortBy = new SortBy[]
			{
				new SortBy(StoreObjectSchema.ItemClass, SortOrder.Ascending),
				new SortBy(MigrationBatchMessageSchema.MigrationJobId, SortOrder.Ascending),
				new SortBy(MigrationBatchMessageSchema.MigrationLastSuccessfulSyncTime, SortOrder.Ascending)
			};
			QueryFilter filter = new AndFilter(new QueryFilter[]
			{
				MigrationJobItem.MigrationJobItemMessageClassFilter,
				new ComparisonFilter(ComparisonOperator.Equal, MigrationBatchMessageSchema.MigrationJobId, jobId)
			});
			if (migrationType == MigrationType.ExchangeOutlookAnywhere)
			{
				sortBy = new SortBy[]
				{
					new SortBy(StoreObjectSchema.ItemClass, SortOrder.Ascending),
					new SortBy(MigrationBatchMessageSchema.MigrationJobId, SortOrder.Ascending),
					new SortBy(MigrationBatchMessageSchema.MigrationJobItemRecipientType, SortOrder.Ascending),
					new SortBy(MigrationBatchMessageSchema.MigrationLastSuccessfulSyncTime, SortOrder.Ascending)
				};
				filter = new AndFilter(new QueryFilter[]
				{
					MigrationJobItem.MigrationJobItemMessageClassFilter,
					new ComparisonFilter(ComparisonOperator.Equal, MigrationBatchMessageSchema.MigrationJobId, jobId),
					new ComparisonFilter(ComparisonOperator.Equal, MigrationBatchMessageSchema.MigrationJobItemRecipientType, MigrationUserRecipientType.Mailbox)
				});
			}
			object[] array = provider.QueryRow(filter, sortBy, new StorePropertyDefinition[]
			{
				MigrationBatchMessageSchema.MigrationLastSuccessfulSyncTime
			});
			if (array != null && !(array[0] is PropertyError))
			{
				return (ExDateTime?)array[0];
			}
			return null;
		}

		public static IEnumerable<MigrationJobItem> GetBySubscriptionLastChecked(IMigrationDataProvider provider, MigrationJob job, ExDateTime? lastCheckedTime, MigrationUserStatus status, int maxCount)
		{
			return MigrationJobItem.GetByColumnLastUpdated(provider, MigrationBatchMessageSchema.MigrationJobItemSubscriptionLastChecked, job, lastCheckedTime, status, maxCount);
		}

		public static IEnumerable<MigrationJobItem> GetItemsNotInStatus(IMigrationDataProvider provider, MigrationJob job, MigrationUserStatus status, int maxCount)
		{
			MigrationUtil.ThrowOnNullArgument(provider, "provider");
			MigrationUtil.ThrowOnNullArgument(job, "job");
			MigrationEqualityFilter primaryFilter = new MigrationEqualityFilter(MigrationBatchMessageSchema.MigrationJobId, job.JobId);
			SortBy[] sort = new SortBy[]
			{
				new SortBy(StoreObjectSchema.ItemClass, SortOrder.Ascending),
				new SortBy(MigrationBatchMessageSchema.MigrationUserStatus, SortOrder.Ascending)
			};
			PropertyDefinition[] filterColumns = new StorePropertyDefinition[]
			{
				StoreObjectSchema.ItemClass,
				MigrationBatchMessageSchema.MigrationUserStatus
			};
			IEnumerable<StoreObjectId> messageIdList = provider.FindMessageIds(primaryFilter, filterColumns, sort, delegate(IDictionary<PropertyDefinition, object> rowData)
			{
				if (!MigrationHelper.IsEqualXsoValues(job.JobId, rowData[MigrationBatchMessageSchema.MigrationJobId]))
				{
					return MigrationRowSelectorResult.RejectRowStopProcessing;
				}
				if (!StringComparer.InvariantCultureIgnoreCase.Equals(rowData[StoreObjectSchema.ItemClass], MigrationBatchMessageSchema.MigrationJobItemClass))
				{
					return MigrationRowSelectorResult.RejectRowContinueProcessing;
				}
				if (status == (MigrationUserStatus)rowData[MigrationBatchMessageSchema.MigrationUserStatus])
				{
					return MigrationRowSelectorResult.RejectRowContinueProcessing;
				}
				return MigrationRowSelectorResult.AcceptRow;
			}, new int?(maxCount));
			messageIdList = new List<StoreObjectId>(messageIdList);
			MigrationJobObjectCache jobCache = new MigrationJobObjectCache(provider);
			jobCache.PreSeed(job);
			foreach (StoreObjectId messageId in messageIdList)
			{
				MigrationJobItem jobItem = MigrationJobItem.Load(provider, messageId, jobCache, true);
				if (jobItem.Status != status)
				{
					yield return jobItem;
				}
			}
			yield break;
		}

		public static IEnumerable<MigrationJobItem> GetAll(IMigrationDataProvider provider, MigrationJob job, int? maxSize)
		{
			MigrationUtil.ThrowOnNullArgument(provider, "provider");
			MigrationJobObjectCache migrationJobObjectCache = new MigrationJobObjectCache(provider);
			MigrationEqualityFilter[] secondaryFilters = null;
			if (job != null)
			{
				secondaryFilters = new MigrationEqualityFilter[]
				{
					new MigrationEqualityFilter(MigrationBatchMessageSchema.MigrationJobId, job.JobId)
				};
				migrationJobObjectCache.PreSeed(job);
			}
			return MigrationJobItem.GetByFilter(provider, MigrationJobItem.MessageClassEqualityFilter, secondaryFilters, null, migrationJobObjectCache, maxSize);
		}

		public static IEnumerable<MigrationJobItem> GetAllSortedByIdentifier(IMigrationDataProvider provider, MigrationJob job, int maxCount)
		{
			MigrationUtil.ThrowOnNullArgument(provider, "provider");
			MigrationUtil.ThrowOnNullArgument(job, "job");
			MigrationEqualityFilter primaryFilter = new MigrationEqualityFilter(MigrationBatchMessageSchema.MigrationJobId, job.JobId);
			SortBy[] additionalSorts = new SortBy[]
			{
				new SortBy(MigrationBatchMessageSchema.MigrationJobItemIdentifier, SortOrder.Ascending)
			};
			MigrationJobObjectCache migrationJobObjectCache = new MigrationJobObjectCache(provider);
			migrationJobObjectCache.PreSeed(job);
			return MigrationJobItem.GetByFilter(provider, primaryFilter, MigrationJobItem.MigrationJobItemMessageClassFilterCollection, additionalSorts, migrationJobObjectCache, new int?(maxCount));
		}

		public static IEnumerable<MigrationJobItem> GetNextJobItems(IMigrationDataProvider provider, MigrationJob job, string identifier, int maxCount)
		{
			MigrationUtil.ThrowOnNullArgument(provider, "provider");
			MigrationUtil.ThrowOnNullArgument(job, "job");
			MigrationUtil.ThrowOnNullOrEmptyArgument(identifier, "identifier");
			maxCount++;
			MigrationEqualityFilter primarySortIndex = new MigrationEqualityFilter(MigrationBatchMessageSchema.MigrationJobItemIdentifier, identifier);
			PropertyDefinition[] additionalDataColumns = new PropertyDefinition[]
			{
				StoreObjectSchema.ItemClass,
				MigrationBatchMessageSchema.MigrationJobId
			};
			IEnumerable<StoreObjectId> messageIds = provider.FindMessageIds(primarySortIndex, additionalDataColumns, null, delegate(IDictionary<PropertyDefinition, object> rowData)
			{
				if (!MigrationBatchMessageSchema.MigrationJobItemClass.Equals(rowData[StoreObjectSchema.ItemClass]))
				{
					return MigrationRowSelectorResult.RejectRowContinueProcessing;
				}
				if (!MigrationHelper.IsEqualXsoValues(job.JobId, rowData[MigrationBatchMessageSchema.MigrationJobId]))
				{
					return MigrationRowSelectorResult.RejectRowContinueProcessing;
				}
				return MigrationRowSelectorResult.AcceptRow;
			}, new int?(maxCount));
			IEnumerator<StoreObjectId> messageIdEnumerator = messageIds.GetEnumerator();
			messageIdEnumerator.MoveNext();
			MigrationJobObjectCache jobCache = new MigrationJobObjectCache(provider);
			jobCache.PreSeed(job);
			while (messageIdEnumerator.MoveNext())
			{
				StoreObjectId messageId = messageIdEnumerator.Current;
				MigrationJobItem item = MigrationJobItem.Load(provider, messageId, jobCache, true);
				yield return item;
			}
			yield break;
		}

		public static IEnumerable<MigrationJobItem> GetByUserId(IMigrationDataProvider provider, MigrationUserId id)
		{
			if (id.JobItemGuid != Guid.Empty)
			{
				MigrationJobItem byGuid = MigrationJobItem.GetByGuid(provider, id.JobItemGuid);
				return Enumerable.Repeat<MigrationJobItem>(byGuid, (byGuid != null) ? 1 : 0);
			}
			return MigrationJobItem.GetByIdentifier(provider, null, id.Id, null);
		}

		public static MigrationJobItem GetByGuid(IMigrationDataProvider provider, Guid identity)
		{
			MigrationUtil.ThrowOnNullArgument(provider, "provider");
			MigrationUtil.ThrowOnNullArgument(identity, "identity");
			MigrationEqualityFilter primaryFilter = new MigrationEqualityFilter(MigrationBatchMessageSchema.MigrationJobItemId, identity);
			return MigrationJobItem.GetByFilter(provider, primaryFilter, null, null, new MigrationJobObjectCache(provider), null).FirstOrDefault<MigrationJobItem>();
		}

		public static IEnumerable<MigrationJobItem> GetByIdentifier(IMigrationDataProvider provider, MigrationJob job, string identifier, MigrationJobObjectCache jobCache = null)
		{
			MigrationUtil.ThrowOnNullArgument(provider, "provider");
			MigrationUtil.ThrowOnNullArgument(identifier, "identifier");
			MigrationEqualityFilter primaryFilter = new MigrationEqualityFilter(MigrationBatchMessageSchema.MigrationJobItemIdentifier, identifier);
			List<MigrationEqualityFilter> list = new List<MigrationEqualityFilter>(2);
			if (jobCache == null)
			{
				jobCache = new MigrationJobObjectCache(provider);
			}
			if (job != null)
			{
				list.Add(new MigrationEqualityFilter(MigrationBatchMessageSchema.MigrationJobId, job.JobId));
				jobCache.PreSeed(job);
			}
			list.Add(MigrationJobItem.MessageClassEqualityFilter);
			return MigrationJobItem.GetByFilter(provider, primaryFilter, list.ToArray(), null, jobCache, null);
		}

		public static int GetCount(IMigrationDataProvider dataProvider, Guid endpointGuid, MigrationSlotType consumedSlotType)
		{
			MigrationUtil.ThrowOnNullArgument(dataProvider, "dataProvider");
			MigrationUtil.ThrowOnGuidEmptyArgument(endpointGuid, "endpointGuid");
			SortBy[] sortBy = new SortBy[]
			{
				new SortBy(StoreObjectSchema.ItemClass, SortOrder.Ascending),
				new SortBy(MigrationBatchMessageSchema.MigrationJobItemSlotProviderId, SortOrder.Ascending),
				new SortBy(MigrationBatchMessageSchema.MigrationJobItemSlotType, SortOrder.Ascending)
			};
			QueryFilter filter = QueryFilter.AndTogether(new QueryFilter[]
			{
				MigrationJobItem.MigrationJobItemMessageClassFilter,
				new ComparisonFilter(ComparisonOperator.Equal, MigrationBatchMessageSchema.MigrationJobItemSlotProviderId, endpointGuid),
				new ComparisonFilter(ComparisonOperator.Equal, MigrationBatchMessageSchema.MigrationJobItemSlotType, consumedSlotType)
			});
			return dataProvider.CountMessages(filter, sortBy);
		}

		public static IEnumerable<MigrationJobItem> GetBySlotId(IMigrationDataProvider provider, Guid slotId, int? maxCount)
		{
			MigrationUtil.ThrowOnNullArgument(provider, "provider");
			MigrationEqualityFilter primaryFilter = new MigrationEqualityFilter(MigrationBatchMessageSchema.MigrationJobItemSlotProviderId, slotId);
			return MigrationJobItem.GetByFilter(provider, primaryFilter, MigrationJobItem.MigrationJobItemMessageClassFilterCollection, null, new MigrationJobObjectCache(provider), maxCount);
		}

		public static IEnumerable<MigrationJobItem> GetByLegacyDN(IMigrationDataProvider provider, string identifier)
		{
			MigrationUtil.ThrowOnNullArgument(provider, "provider");
			MigrationUtil.ThrowOnNullArgument(identifier, "identifier");
			MigrationEqualityFilter primaryFilter = new MigrationEqualityFilter(MigrationBatchMessageSchema.MigrationJobItemMailboxLegacyDN, identifier);
			return MigrationJobItem.GetByFilter(provider, primaryFilter, MigrationJobItem.MigrationJobItemMessageClassFilterCollection, null, new MigrationJobObjectCache(provider), null);
		}

		public static IEnumerable<MigrationJobItem> GetJobItemsByTypeAndGroupMemberProvisionedState(IMigrationDataProvider provider, MigrationJob job, MigrationUserRecipientType recipientType, MigrationUserStatus status, GroupMembershipProvisioningState provisioningState, int maxCount)
		{
			MigrationUtil.ThrowOnNullArgument(provider, "provider");
			MigrationUtil.ThrowOnNullArgument(job, "job");
			MigrationEqualityFilter primaryFilter = new MigrationEqualityFilter(MigrationBatchMessageSchema.MigrationJobItemRecipientType, recipientType);
			PropertyDefinition[] filterColumns = new PropertyDefinition[]
			{
				MigrationBatchMessageSchema.MigrationJobId,
				MigrationBatchMessageSchema.MigrationUserStatus,
				MigrationBatchMessageSchema.MigrationJobItemGroupMemberProvisioningState,
				StoreObjectSchema.ItemClass
			};
			SortBy[] additionalSorts = new SortBy[]
			{
				new SortBy(MigrationBatchMessageSchema.MigrationJobId, SortOrder.Ascending),
				new SortBy(MigrationBatchMessageSchema.MigrationUserStatus, SortOrder.Ascending),
				new SortBy(MigrationBatchMessageSchema.MigrationJobItemGroupMemberProvisioningState, SortOrder.Ascending)
			};
			IEnumerable<StoreObjectId> messageIdList = provider.FindMessageIds(primaryFilter, filterColumns, additionalSorts, delegate(IDictionary<PropertyDefinition, object> rowData)
			{
				if (!object.Equals(rowData[StoreObjectSchema.ItemClass], MigrationBatchMessageSchema.MigrationJobItemClass))
				{
					return MigrationRowSelectorResult.RejectRowContinueProcessing;
				}
				if ((MigrationUserStatus)rowData[MigrationBatchMessageSchema.MigrationUserStatus] != status)
				{
					return MigrationRowSelectorResult.RejectRowContinueProcessing;
				}
				if (!MigrationHelper.IsEqualXsoValues(job.JobId, rowData[MigrationBatchMessageSchema.MigrationJobId]))
				{
					return MigrationRowSelectorResult.RejectRowContinueProcessing;
				}
				if (recipientType != (MigrationUserRecipientType)rowData[MigrationBatchMessageSchema.MigrationJobItemRecipientType])
				{
					return MigrationRowSelectorResult.RejectRowStopProcessing;
				}
				object obj;
				if (!rowData.TryGetValue(MigrationBatchMessageSchema.MigrationJobItemGroupMemberProvisioningState, out obj))
				{
					MigrationLogger.Log(MigrationEventType.Error, "We should not hit this case. This will not cause incorrect behavior but will hide perf issues", new object[0]);
					return MigrationRowSelectorResult.RejectRowContinueProcessing;
				}
				int num = (obj == null) ? 0 : ((int)obj);
				if (!Enum.IsDefined(typeof(GroupMembershipProvisioningState), num))
				{
					throw new MigrationDataCorruptionException("Invalid MigrationJobItemGroupMemberProvisioningState.");
				}
				GroupMembershipProvisioningState groupMembershipProvisioningState = (GroupMembershipProvisioningState)num;
				if (groupMembershipProvisioningState == provisioningState)
				{
					return MigrationRowSelectorResult.AcceptRow;
				}
				return MigrationRowSelectorResult.RejectRowStopProcessing;
			}, new int?(maxCount));
			messageIdList = new List<StoreObjectId>(messageIdList);
			MigrationJobObjectCache jobCache = new MigrationJobObjectCache(provider);
			jobCache.PreSeed(job);
			foreach (StoreObjectId messageId in messageIdList)
			{
				yield return MigrationJobItem.Load(provider, messageId, jobCache, true);
			}
			yield break;
		}

		public static int GetProvisionedCount(IMigrationDataProvider provider, Guid jobId)
		{
			MigrationUtil.ThrowOnNullArgument(provider, "provider");
			QueryFilter filter = new AndFilter(new QueryFilter[]
			{
				MigrationJobItem.MigrationJobItemMessageClassFilter,
				new ComparisonFilter(ComparisonOperator.Equal, MigrationBatchMessageSchema.MigrationJobId, jobId),
				new ComparisonFilter(ComparisonOperator.GreaterThanOrEqual, MigrationBatchMessageSchema.MigrationProvisionedTime, ExDateTime.MinValue)
			});
			SortBy[] sortBy = new SortBy[]
			{
				new SortBy(StoreObjectSchema.ItemClass, SortOrder.Ascending),
				new SortBy(MigrationBatchMessageSchema.MigrationJobId, SortOrder.Ascending),
				new SortBy(MigrationBatchMessageSchema.MigrationProvisionedTime, SortOrder.Ascending)
			};
			return provider.CountMessages(filter, sortBy);
		}

		public static int GetCount(IMigrationDataProvider provider, Guid jobId, params MigrationUserStatus[] statuses)
		{
			return MigrationJobItem.GetCount(provider, jobId, null, statuses);
		}

		public static int GetCount(IMigrationDataProvider provider, Guid jobId, ExDateTime? lastRestartTime, params MigrationUserStatus[] statuses)
		{
			MigrationUtil.ThrowOnNullArgument(provider, "provider");
			SortBy[] sortBy = new SortBy[]
			{
				new SortBy(StoreObjectSchema.ItemClass, SortOrder.Ascending),
				new SortBy(MigrationBatchMessageSchema.MigrationJobId, SortOrder.Ascending),
				new SortBy(MigrationBatchMessageSchema.MigrationUserStatus, SortOrder.Ascending),
				new SortBy(MigrationBatchMessageSchema.MigrationJobLastRestartTime, SortOrder.Ascending)
			};
			if (statuses == null || statuses.Length == 0)
			{
				QueryFilter filter = QueryFilter.AndTogether(new QueryFilter[]
				{
					MigrationJobItem.MigrationJobItemMessageClassFilter,
					new ComparisonFilter(ComparisonOperator.Equal, MigrationBatchMessageSchema.MigrationJobId, jobId)
				});
				return provider.CountMessages(filter, sortBy);
			}
			int num = 0;
			foreach (MigrationUserStatus migrationUserStatus in statuses)
			{
				QueryFilter filter2;
				if (lastRestartTime != null)
				{
					filter2 = QueryFilter.AndTogether(new QueryFilter[]
					{
						MigrationJobItem.MigrationJobItemMessageClassFilter,
						new ComparisonFilter(ComparisonOperator.Equal, MigrationBatchMessageSchema.MigrationJobId, jobId),
						new ComparisonFilter(ComparisonOperator.Equal, MigrationBatchMessageSchema.MigrationUserStatus, migrationUserStatus),
						new ComparisonFilter(ComparisonOperator.LessThan, MigrationBatchMessageSchema.MigrationJobLastRestartTime, lastRestartTime.Value)
					});
				}
				else
				{
					filter2 = QueryFilter.AndTogether(new QueryFilter[]
					{
						MigrationJobItem.MigrationJobItemMessageClassFilter,
						new ComparisonFilter(ComparisonOperator.Equal, MigrationBatchMessageSchema.MigrationJobId, jobId),
						new ComparisonFilter(ComparisonOperator.Equal, MigrationBatchMessageSchema.MigrationUserStatus, migrationUserStatus)
					});
				}
				num += provider.CountMessages(filter2, sortBy);
			}
			return num;
		}

		public static IEnumerable<MigrationJobItem> GetByStatus(IMigrationDataProvider provider, MigrationJob job, MigrationUserStatus status, int? maxCount)
		{
			return MigrationJobItem.GetByStatusAndRestartTime(provider, job, status, null, maxCount);
		}

		public static IEnumerable<MigrationJobItem> GetByStatusAndRestartTime(IMigrationDataProvider provider, MigrationJob job, MigrationUserStatus status, ExDateTime? lastRestartTime, int? maxCount)
		{
			MigrationUtil.ThrowOnNullArgument(provider, "provider");
			MigrationEqualityFilter primaryFilter = MigrationJobItem.MessageClassEqualityFilter;
			List<MigrationEqualityFilter> secondaryFilters = new List<MigrationEqualityFilter>(2);
			MigrationJobObjectCache jobCache = new MigrationJobObjectCache(provider);
			if (job != null)
			{
				secondaryFilters.Add(new MigrationEqualityFilter(MigrationBatchMessageSchema.MigrationJobId, job.JobId));
				jobCache.PreSeed(job);
			}
			secondaryFilters.Add(new MigrationEqualityFilter(MigrationBatchMessageSchema.MigrationUserStatus, status));
			if (lastRestartTime != null)
			{
				secondaryFilters.Add(new MigrationEqualityFilter(MigrationBatchMessageSchema.MigrationJobLastRestartTime, lastRestartTime, ComparisonOperator.LessThan));
			}
			IEnumerable<MigrationJobItem> jobItems = MigrationJobItem.GetByFilter(provider, primaryFilter, secondaryFilters.ToArray(), null, jobCache, maxCount);
			foreach (MigrationJobItem jobItem in jobItems)
			{
				if (jobItem.Status == status && (lastRestartTime == null || jobItem.LastRestartTime == null || jobItem.LastRestartTime.Value < lastRestartTime.Value))
				{
					yield return jobItem;
				}
				else
				{
					MigrationLogger.Log(MigrationEventType.Information, "MigrationJobItem.GetByStatus: jobitem {0}, status {1} changed since load to {2}", new object[]
					{
						jobItem,
						status,
						jobItem.Status
					});
				}
			}
			yield break;
		}

		public override bool TryLoad(IMigrationDataProvider dataProvider, StoreObjectId id)
		{
			this.TenantName = dataProvider.TenantName;
			return base.TryLoad(dataProvider, id);
		}

		public bool HasFinalized()
		{
			return this.Status == MigrationUserStatus.Completing || this.Status == MigrationUserStatus.Completed || this.Status == MigrationUserStatus.CompletionSynced || this.Status == MigrationUserStatus.CompletedWithWarnings || this.Status == MigrationUserStatus.CompletionFailed;
		}

		public void SetUserMailboxProperties(IMigrationDataProvider provider, MigrationUserStatus? status, MailboxData mailboxData, Exception error, ExDateTime? provisionedTime)
		{
			MigrationUtil.ThrowOnNullArgument(provider, "provider");
			if (error != null && status == null)
			{
				throw new ArgumentException("if error has value, then status needs to be passed in");
			}
			MigrationStatusData<MigrationUserStatus> migrationStatusData = null;
			if (status != null)
			{
				migrationStatusData = new MigrationStatusData<MigrationUserStatus>(this.StatusData);
				if (MigrationJobItem.IsFailedStatus(status.Value))
				{
					migrationStatusData.UpdateStatus(status.Value, error, MigrationLogger.CombineInternalError("SetUserMailboxProperties: failed status", error), true, null);
				}
				else if (error != null)
				{
					migrationStatusData.UpdateStatus(status.Value, error, MigrationLogger.CombineInternalError("SetUserMailboxProroperties: setting warning", error), null);
				}
				else
				{
					migrationStatusData.UpdateStatus(status.Value, null);
				}
			}
			this.SetUserMailboxProperties(provider, migrationStatusData, mailboxData, provisionedTime);
		}

		public void UpdateAndEnableJobItem(IMigrationDataProvider dataProvider, MigrationJob job, MigrationUserStatus newStatus)
		{
			MigrationUtil.ThrowOnNullArgument(dataProvider, "dataProvider");
			MigrationUtil.ThrowOnNullArgument(job, "job");
			MigrationUtil.AssertOrThrow(base.StoreObjectId != null, "Should only work on an item that's been persisted", new object[0]);
			MigrationLogger.Log(MigrationEventType.Verbose, "MigrationJobItem.UpdateJobData on migration item", new object[0]);
			MigrationStatusData<MigrationUserStatus> statusData = new MigrationStatusData<MigrationUserStatus>(this.StatusData);
			statusData.ClearError();
			statusData.UpdateStatus(newStatus, null);
			PropertyDefinition[] propertiesToUpdate = MigrationHelper.AggregateProperties(new IList<PropertyDefinition>[]
			{
				MigrationJobItem.updatePropertyDefinitionsBase,
				MigrationJobItem.DisableMigrationProperties,
				this.SubscriptionSettingsPropertyDefinitions,
				this.SubscriptionIdPropertyDefinitions,
				this.MigrationJobSlotPropertyDefinitions,
				new PropertyDefinition[]
				{
					this.CursorPositionPropertyDefinition,
					MigrationBatchMessageSchema.MigrationJobLastRestartTime
				}
			});
			ExDateTime timeNowUtc = ExDateTime.UtcNow;
			this.UpdatePersistedMessage(dataProvider, propertiesToUpdate, delegate(IMigrationMessageItem message)
			{
				statusData.WriteToMessageItem(message, true);
				MigrationHelperBase.SetExDateTimeProperty(message, MigrationBatchMessageSchema.MigrationJobLastRestartTime, new ExDateTime?(timeNowUtc));
				this.CheckAndReleaseSlotAssignmentIfNeeded(statusData.Status, message);
				this.WriteExtendedPropertiesToMessageItem(message);
				if (this.ShouldMigrate)
				{
					message.Delete(MigrationBatchMessageSchema.MigrationDisableTime);
					message.Delete(MigrationBatchMessageSchema.MigrationJobItemSubscriptionLastChecked);
				}
			});
			this.LastRestartTime = new ExDateTime?(timeNowUtc);
			this.StatusData = statusData;
			if (this.ShouldMigrate)
			{
				this.SubscriptionLastChecked = null;
				this.SubscriptionDisableTime = null;
			}
		}

		public bool UpdateDataRow(IMigrationDataProvider dataProvider, MigrationJob job, IMigrationDataRow request)
		{
			MigrationUtil.ThrowOnNullArgument(dataProvider, "dataProvider");
			MigrationUtil.ThrowOnNullArgument(job, "job");
			MigrationUtil.ThrowOnNullArgument(request, "request");
			MigrationUtil.AssertOrThrow(base.StoreObjectId != null, "Should only work on an item that's been persisted", new object[0]);
			MigrationLogger.Log(MigrationEventType.Verbose, "MigrationJobItem.UpdateDataRow on migration item", new object[0]);
			bool result = this.MigrationJobId == job.JobId;
			this.BatchInputId = job.BatchInputId;
			this.MigrationJob = job;
			this.IsStaged = job.IsStaged;
			this.JobName = job.JobName;
			this.IncrementalSyncInterval = null;
			PropertyDefinition[] propertiesToUpdate = MigrationHelper.AggregateProperties(new IList<PropertyDefinition>[]
			{
				MigrationJobItem.updatePropertyDefinitionsBase,
				MigrationJobItem.DisableMigrationProperties,
				this.ProvisioningDataPropertyDefinitions,
				this.SubscriptionSettingsPropertyDefinitions,
				this.SubscriptionIdPropertyDefinitions,
				new PropertyDefinition[]
				{
					this.CursorPositionPropertyDefinition,
					MigrationBatchMessageSchema.MigrationJobItemIncomingUsername
				}
			});
			ProvisioningDataStorageBase provisioningData = ProvisioningDataStorageBase.CreateFromDataRow(request, false);
			JobItemSubscriptionSettingsBase jobItemSubscriptionSettingsBase;
			if (this.SubscriptionSettings != null)
			{
				jobItemSubscriptionSettingsBase = ((JobItemSubscriptionSettingsBase)this.SubscriptionSettings).Clone();
				jobItemSubscriptionSettingsBase.UpdateFromDataRow(request);
			}
			else
			{
				jobItemSubscriptionSettingsBase = JobItemSubscriptionSettingsBase.CreateFromDataRow(request);
			}
			JobItemSubscriptionSettingsBase subscriptionSettingsToSave = jobItemSubscriptionSettingsBase ?? JobItemSubscriptionSettingsBase.Create(this.MigrationType);
			this.UpdatePersistedMessage(dataProvider, propertiesToUpdate, delegate(IMigrationMessageItem message)
			{
				message[MigrationBatchMessageSchema.MigrationJobId] = job.JobId;
				message[this.CursorPositionPropertyDefinition] = request.CursorPosition;
				if (provisioningData != null)
				{
					provisioningData.WriteToMessageItem(message, true);
				}
				if (subscriptionSettingsToSave != null)
				{
					subscriptionSettingsToSave.WriteToMessageItem(message, true);
				}
				if (request.SupportsRemoteIdentifier)
				{
					message[MigrationBatchMessageSchema.MigrationJobItemIncomingUsername] = request.RemoteIdentifier;
				}
				this.WriteExtendedPropertiesToMessageItem(message);
				if (!this.ShouldMigrate)
				{
					MigrationLogger.Log(MigrationEventType.Verbose, "setting subscription last check to max date time for recipient type: {0}", new object[]
					{
						this.RecipientType
					});
					message[MigrationBatchMessageSchema.MigrationJobItemSubscriptionLastChecked] = MigrationJobItem.MaxDateTimeValue;
				}
			});
			if (request.SupportsRemoteIdentifier)
			{
				this.RemoteIdentifier = request.RemoteIdentifier;
			}
			this.MigrationJobId = job.JobId;
			this.ProvisioningData = provisioningData;
			this.CursorPosition = request.CursorPosition;
			this.SubscriptionSettings = jobItemSubscriptionSettingsBase;
			if (!this.ShouldMigrate)
			{
				MigrationLogger.Log(MigrationEventType.Verbose, "setting subscription last check to max date time for recipient type: {0}", new object[]
				{
					this.RecipientType
				});
				this.SubscriptionLastChecked = new ExDateTime?(MigrationJobItem.MaxDateTimeValue);
			}
			return result;
		}

		public void SetTransientError(IMigrationDataProvider provider, Exception exception)
		{
			MigrationUtil.ThrowOnNullArgument(provider, "provider");
			MigrationUtil.ThrowOnNullArgument(exception, "exception");
			string diagnosticInfo = MigrationLogger.GetDiagnosticInfo(exception, null);
			MigrationLogger.Log(MigrationEventType.Error, "MigrationJobItem.SetTransientError: job {0}, {1}", new object[]
			{
				this,
				diagnosticInfo
			});
			MigrationStatusData<MigrationUserStatus> migrationStatusData = new MigrationStatusData<MigrationUserStatus>(this.StatusData);
			migrationStatusData.SetTransientError(exception, null, null);
			this.SetStatusData(provider, migrationStatusData);
			MigrationFailureLog.LogFailureEvent(this, exception, MigrationFailureFlags.None, null);
		}

		public void SetCorruptStatus(IMigrationDataProvider provider, Exception exception)
		{
			MigrationUtil.ThrowOnNullArgument(provider, "provider");
			MigrationUtil.ThrowOnNullArgument(exception, "exception");
			string diagnosticInfo = MigrationLogger.GetDiagnosticInfo(exception, null);
			MigrationLogger.Log(MigrationEventType.Error, "MigrationJobItem.SetFailedStatus: jobitem {0}, error {1}", new object[]
			{
				this,
				diagnosticInfo
			});
			MigrationStatusData<MigrationUserStatus> migrationStatusData = new MigrationStatusData<MigrationUserStatus>(this.StatusData);
			migrationStatusData.SetFailedStatus(MigrationUserStatus.Corrupted, exception, diagnosticInfo, null);
			this.SetStatusData(provider, migrationStatusData);
			if (this.MigrationJob != null && this.MigrationJob.ReportData != null)
			{
				this.MigrationJob.ReportData.Append(Strings.MigrationReportJobItemCorrupted(this.Identifier), exception, ReportEntryFlags.Failure | ReportEntryFlags.Fatal | ReportEntryFlags.Target);
				provider.FlushReport(this.MigrationJob.ReportData);
			}
			MigrationFailureLog.LogFailureEvent(this, exception, MigrationFailureFlags.Corruption, null);
		}

		public void SetFailedStatus(IMigrationDataProvider provider, MigrationUserStatus status, LocalizedException localizedError, string internalError)
		{
			this.SetFailedStatus(provider, status, localizedError, internalError, false);
		}

		public void SetFailedStatus(IMigrationDataProvider provider, MigrationUserStatus status, LocalizedException localizedError, string internalError, bool setLastRestartTime)
		{
			MigrationUtil.ThrowOnNullArgument(provider, "provider");
			if (!MigrationJobItem.IsFailedStatus(status))
			{
				throw new ArgumentException("Expect a failed status");
			}
			MigrationUtil.ThrowOnNullOrEmptyArgument(internalError, "internalError");
			MigrationStatusData<MigrationUserStatus> migrationStatusData = new MigrationStatusData<MigrationUserStatus>(this.StatusData);
			migrationStatusData.UpdateStatus(status, localizedError, internalError, true, null);
			MigrationLogger.Log(MigrationEventType.Error, "MigrationJobItem.SetStatus: jobitem {0}, statusData {1}", new object[]
			{
				this,
				migrationStatusData
			});
			this.SetStatusData(provider, migrationStatusData, setLastRestartTime);
			if (this.MigrationJob != null && this.MigrationJob.ReportData != null)
			{
				this.MigrationJob.ReportData.Append(Strings.MigrationReportJobItemFailed(this.Identifier, localizedError.LocalizedString), localizedError, ReportEntryFlags.Failure | ReportEntryFlags.Fatal | ReportEntryFlags.Target);
				provider.FlushReport(this.MigrationJob.ReportData);
			}
			MigrationFailureLog.LogFailureEvent(this, localizedError, MigrationFailureFlags.Fatal, null);
		}

		public void SetStatus(IMigrationDataProvider provider, MigrationUserStatus status, MigrationState state, MigrationFlags? flags = null, MigrationWorkflowPosition position = null, TimeSpan? delayTime = null, IMailboxData mailboxData = null, IStepSettings stepSettings = null, IStepSnapshot stepSnapshot = null, bool updated = false, LocalizedException exception = null)
		{
			MigrationUtil.ThrowOnNullArgument(provider, "provider");
			MigrationUtil.AssertOrThrow(base.StoreObjectId != null, "Should only work on an item that's been persisted", new object[0]);
			string internalError = null;
			if (exception != null)
			{
				internalError = MigrationLogger.GetDiagnosticInfo(exception, null);
			}
			MigrationStatusData<MigrationUserStatus> statusData = new MigrationStatusData<MigrationUserStatus>(this.StatusData);
			if (state == MigrationState.Failed || state == MigrationState.Corrupted)
			{
				statusData.SetFailedStatus(status, exception, internalError, new MigrationState?(state));
			}
			else if (exception != null)
			{
				statusData.SetTransientError(exception, new MigrationUserStatus?(status), new MigrationState?(state));
			}
			else
			{
				statusData.UpdateStatus(status, new MigrationState?(state));
			}
			MigrationLogger.Log(MigrationEventType.Verbose, "MigrationJobItem.SetStatusAndSubscription: jobitem {0}, status {1}", new object[]
			{
				this,
				statusData
			});
			PropertyDefinition[] propertiesToUpdate = MigrationHelper.AggregateProperties(new IList<PropertyDefinition>[]
			{
				MigrationJobItem.MigrationJobItemColumnsStatusIndex,
				this.MailboxDataPropertyDefinitions,
				MigrationWorkflowPosition.MigrationWorkflowPositionProperties,
				this.SubscriptionIdPropertyDefinitions,
				this.SubscriptionSettingsPropertyDefinitions,
				this.MigrationJobSlotPropertyDefinitions,
				new StorePropertyDefinition[]
				{
					MigrationBatchMessageSchema.MigrationFlags,
					MigrationBatchMessageSchema.MigrationNextProcessTime,
					MigrationBatchMessageSchema.MigrationJobItemSubscriptionSettingsLastUpdatedTime,
					MigrationBatchMessageSchema.MigrationJobItemSubscriptionLastChecked,
					MigrationBatchMessageSchema.MigrationJobItemSubscriptionQueuedTime,
					MigrationBatchMessageSchema.MigrationProvisionedTime
				}
			});
			ExDateTime? subscriptionLastChecked = null;
			ExDateTime? provisionedTime = null;
			ExDateTime? subscriptionQueuedTime = null;
			if (stepSnapshot != null)
			{
				if (stepSnapshot is ISubscriptionStatistics)
				{
					subscriptionLastChecked = new ExDateTime?(ExDateTime.UtcNow);
					subscriptionQueuedTime = stepSnapshot.InjectionCompletedTime;
				}
				else if (stepSnapshot is ProvisioningSnapshot)
				{
					if (stepSnapshot.Status == SnapshotStatus.Finalized && this.WorkflowPosition.Step == MigrationStep.Provisioning)
					{
						provisionedTime = stepSnapshot.InjectionCompletedTime;
					}
					mailboxData = ((ProvisioningSnapshot)stepSnapshot).MailboxData;
				}
			}
			ExDateTime? nextProcessTime = null;
			if (delayTime != null)
			{
				nextProcessTime = new ExDateTime?(ExDateTime.UtcNow + delayTime.Value);
			}
			else
			{
				nextProcessTime = new ExDateTime?(ExDateTime.UtcNow);
			}
			ExDateTime? subscriptionSettingsLastUpdatedTime = null;
			if (updated)
			{
				subscriptionSettingsLastUpdatedTime = new ExDateTime?(ExDateTime.UtcNow);
			}
			this.UpdatePersistedMessage(provider, propertiesToUpdate, delegate(IMigrationMessageItem message)
			{
				if (statusData != null)
				{
					statusData.WriteToMessageItem(message, true);
					this.WriteExtendedPropertiesToMessageItem(message);
					this.CheckAndReleaseSlotAssignmentIfNeeded(statusData.Status, message);
				}
				if (nextProcessTime != null)
				{
					MigrationHelperBase.SetExDateTimeProperty(message, MigrationBatchMessageSchema.MigrationNextProcessTime, new ExDateTime?(nextProcessTime.Value));
				}
				if (flags != null)
				{
					message[MigrationBatchMessageSchema.MigrationFlags] = flags.Value;
				}
				if (position != null)
				{
					position.WriteToMessageItem(message, true);
				}
				if (subscriptionLastChecked != null)
				{
					message[MigrationBatchMessageSchema.MigrationJobItemSubscriptionLastChecked] = subscriptionLastChecked.Value;
				}
				if (subscriptionQueuedTime != null)
				{
					message[MigrationBatchMessageSchema.MigrationJobItemSubscriptionQueuedTime] = subscriptionQueuedTime.Value;
				}
				if (provisionedTime != null)
				{
					message[MigrationBatchMessageSchema.MigrationProvisionedTime] = provisionedTime.Value;
				}
				if (mailboxData != null)
				{
					mailboxData.WriteToMessageItem(message, true);
				}
				if (stepSettings != null)
				{
					stepSettings.WriteToMessageItem(message, true);
				}
				if (stepSnapshot != null && stepSnapshot is IMigrationSerializable)
				{
					((IMigrationSerializable)stepSnapshot).WriteToMessageItem(message, true);
					if (stepSnapshot.Id != null && stepSnapshot.Id is IMigrationSerializable)
					{
						((IMigrationSerializable)stepSnapshot.Id).WriteToMessageItem(message, true);
					}
				}
				if (subscriptionSettingsLastUpdatedTime != null)
				{
					MigrationHelperBase.SetExDateTimeProperty(message, MigrationBatchMessageSchema.MigrationJobItemSubscriptionSettingsLastUpdatedTime, new ExDateTime?(subscriptionSettingsLastUpdatedTime.Value));
				}
			});
			if (stepSnapshot != null)
			{
				if (stepSnapshot is ISubscriptionStatistics)
				{
					this.SubscriptionStatistics = (ISubscriptionStatistics)stepSnapshot;
					this.SubscriptionId = (ISubscriptionId)stepSnapshot.Id;
				}
				else if (stepSnapshot is ProvisioningSnapshot)
				{
					this.ProvisioningStatistics = (ProvisioningSnapshot)stepSnapshot;
				}
			}
			if (stepSettings != null)
			{
				if (stepSettings is JobItemSubscriptionSettingsBase)
				{
					this.SubscriptionSettings = (JobItemSubscriptionSettingsBase)stepSettings;
				}
				else if (stepSettings is ProvisioningDataStorageBase)
				{
					this.ProvisioningData = (ProvisioningDataStorageBase)stepSettings;
				}
			}
			if (mailboxData != null)
			{
				this.LocalMailbox = mailboxData;
			}
			if (subscriptionLastChecked != null)
			{
				this.SubscriptionLastChecked = new ExDateTime?(subscriptionLastChecked.Value);
			}
			if (subscriptionQueuedTime != null)
			{
				this.SubscriptionQueuedTime = new ExDateTime?(subscriptionQueuedTime.Value);
			}
			if (provisionedTime != null)
			{
				this.ProvisionedTime = new ExDateTime?(provisionedTime.Value);
			}
			if (flags != null)
			{
				this.Flags = flags.Value;
			}
			if (position != null)
			{
				this.WorkflowPosition = position;
			}
			if (nextProcessTime != null)
			{
				this.NextProcessTime = nextProcessTime;
			}
			if (subscriptionSettingsLastUpdatedTime != null)
			{
				this.SubscriptionSettingsLastUpdatedTime = new ExDateTime?(subscriptionSettingsLastUpdatedTime.Value);
			}
			MigrationUserStatus migrationUserStatus = this.Status;
			this.StatusData = statusData;
			if (this.Status != migrationUserStatus)
			{
				this.LogStatusEvent();
			}
			if (exception != null)
			{
				MigrationFailureFlags failureFlags;
				switch (state)
				{
				case MigrationState.Failed:
					failureFlags = MigrationFailureFlags.Fatal;
					break;
				case MigrationState.Corrupted:
					failureFlags = MigrationFailureFlags.Corruption;
					break;
				default:
					failureFlags = MigrationFailureFlags.None;
					break;
				}
				MigrationFailureLog.LogFailureEvent(this, exception, failureFlags, null);
			}
			if (state == MigrationState.Failed && this.MigrationJob != null && this.MigrationJob.ReportData != null)
			{
				this.MigrationJob.ReportData.Append(Strings.MigrationReportJobItemFailed(this.Identifier, exception.LocalizedString), exception, ReportEntryFlags.Failure | ReportEntryFlags.Fatal | ReportEntryFlags.Target);
				provider.FlushReport(this.MigrationJob.ReportData);
			}
		}

		public List<string> GetGroupMembersInfo(IMigrationDataProvider provider)
		{
			MigrationUtil.ThrowOnNullArgument(provider, "provider");
			MigrationUtil.AssertOrThrow(base.StoreObjectId != null, "Should only work on an item that's been persisted", new object[0]);
			MigrationUtil.AssertOrThrow(this.ShouldProvision, "Does not support provisioning!", new object[0]);
			MigrationUtil.AssertOrThrow(this.ProvisioningData != null, "Provisioning Data missing!", new object[0]);
			MigrationUtil.AssertOrThrow(this.ProvisioningData is ExchangeProvisioningDataStorage, "Provisioning data wrong type!", new object[0]);
			MigrationUtil.AssertOrThrow(((ExchangeProvisioningDataStorage)this.ProvisioningData).ExchangeRecipient != null, "Provisioning Recipient Data missing!", new object[0]);
			MigrationUtil.AssertOrThrow(this.RecipientType == MigrationUserRecipientType.Group, "ExchangeRecipient has to be type of Group", new object[0]);
			List<string> members = null;
			this.UpdatePersistedMessage(provider, MigrationJobItem.GroupProperties, delegate(IMigrationMessageItem message)
			{
				members = ((LegacyExchangeMigrationGroupRecipient)((ExchangeProvisioningDataStorage)this.ProvisioningData).ExchangeRecipient).GetNextBatchOfMembers(provider, message);
			});
			return members;
		}

		public void SetGroupProperties(IMigrationDataProvider provider, MigrationUserStatus? status, ExDateTime? provisionedTime, string[] members, int membersProvisioned, int membersSkipped)
		{
			MigrationUtil.AssertOrThrow(this.ShouldProvision, "Does not support provisioning!", new object[0]);
			MigrationUtil.AssertOrThrow(this.ProvisioningData != null, "Provisioning Data missing!", new object[0]);
			MigrationUtil.AssertOrThrow(this.ProvisioningData is ExchangeProvisioningDataStorage, "Provisioning data wrong type!", new object[0]);
			MigrationUtil.AssertOrThrow(((ExchangeProvisioningDataStorage)this.ProvisioningData).ExchangeRecipient != null, "Provisioning Recipient Data missing!", new object[0]);
			MigrationUtil.AssertOrThrow(this.RecipientType == MigrationUserRecipientType.Group, "ExchangeRecipient has to be type of Group", new object[0]);
			MigrationUtil.ThrowOnNullArgument(provider, "provider");
			MigrationStatusData<MigrationUserStatus> statusData = null;
			if (status != null)
			{
				statusData = new MigrationStatusData<MigrationUserStatus>(this.StatusData);
				statusData.UpdateStatus(status.Value, null);
			}
			this.UpdatePersistedMessage(provider, MigrationJobItem.GroupProperties, delegate(IMigrationMessageItem message)
			{
				if (statusData != null)
				{
					statusData.WriteToMessageItem(message, true);
					this.CheckAndReleaseSlotAssignmentIfNeeded(statusData.Status, message);
				}
				if (provisionedTime != null)
				{
					message[MigrationBatchMessageSchema.MigrationProvisionedTime] = provisionedTime.Value;
				}
				LegacyExchangeMigrationGroupRecipient legacyExchangeMigrationGroupRecipient = (LegacyExchangeMigrationGroupRecipient)((ExchangeProvisioningDataStorage)this.ProvisioningData).ExchangeRecipient;
				if (members != null)
				{
					legacyExchangeMigrationGroupRecipient.SetGroupMembersInfo(message, members);
					return;
				}
				legacyExchangeMigrationGroupRecipient.UpdateGroupMembersInfo(message, membersProvisioned, membersSkipped);
			});
			if (provisionedTime != null)
			{
				this.ProvisionedTime = provisionedTime;
			}
			if (statusData != null)
			{
				this.StatusData = statusData;
				this.LogStatusEvent();
			}
		}

		public void DisableMigration(IMigrationDataProvider provider, MigrationUserStatus status)
		{
			MigrationUtil.ThrowOnNullArgument(provider, "provider");
			if (!MigrationJobItem.IsFailedStatus(status) && !MigrationJobItem.IsStoppedStatus(status))
			{
				throw new ArgumentException("Expect a failed or stopped status");
			}
			MigrationStatusData<MigrationUserStatus> newStatusData = new MigrationStatusData<MigrationUserStatus>(this.StatusData);
			if ((!MigrationJobItem.IsFailedStatus(this.Status) && MigrationJobItem.IsFailedStatus(status)) || (!MigrationJobItem.IsStoppedStatus(this.Status) && MigrationJobItem.IsStoppedStatus(status)))
			{
				MigrationLogger.Log(MigrationEventType.Information, "MigrationJobItem.DisableMigration: jobitem {0}, updating status {1}", new object[]
				{
					this,
					status
				});
				newStatusData.UpdateStatus(status, new MigrationCancelledByUserRequestException(), "migration disabled", true, null);
			}
			PropertyDefinition[] propertiesToUpdate = MigrationHelper.AggregateProperties(new IList<PropertyDefinition>[]
			{
				MigrationJobItem.DisableMigrationProperties,
				this.MigrationJobSlotPropertyDefinitions
			});
			ExDateTime timeNow = ExDateTime.UtcNow;
			this.UpdatePersistedMessage(provider, propertiesToUpdate, delegate(IMigrationMessageItem message)
			{
				newStatusData.WriteToMessageItem(message, true);
				message[MigrationBatchMessageSchema.MigrationDisableTime] = timeNow;
				message[MigrationBatchMessageSchema.MigrationJobItemSubscriptionLastChecked] = MigrationJobItem.MaxDateTimeValue;
				this.CheckAndReleaseSlotAssignmentIfNeeded(newStatusData.Status, message);
			});
			this.StatusData = newStatusData;
			this.SubscriptionLastChecked = new ExDateTime?(MigrationJobItem.MaxDateTimeValue);
			this.SubscriptionDisableTime = new ExDateTime?(timeNow);
		}

		public void SetStatus(IMigrationDataProvider provider, MigrationUserStatus status)
		{
			if (MigrationJobItem.IsFailedStatus(status))
			{
				throw new ArgumentException("Use SetFailedStatus instead");
			}
			MigrationStatusData<MigrationUserStatus> migrationStatusData = new MigrationStatusData<MigrationUserStatus>(this.StatusData);
			migrationStatusData.UpdateStatus(status, null);
			MigrationLogger.Log(MigrationEventType.Error, "MigrationJobItem.SetStatus: jobitem {0}, statusData {1}", new object[]
			{
				this,
				migrationStatusData
			});
			this.SetStatusData(provider, migrationStatusData);
		}

		public void SetStatusAndSubscriptionLastChecked(IMigrationDataProvider provider, MigrationUserStatus? status, LocalizedException localizedError, ExDateTime? subscriptionLastChecked, ISubscriptionStatistics stats)
		{
			this.SetStatusAndSubscriptionLastChecked(provider, status, localizedError, subscriptionLastChecked, true, stats);
		}

		public void SetSubscriptionFailed(IMigrationDataProvider provider, MigrationUserStatus status, LocalizedException localizedError)
		{
			MigrationUtil.ThrowOnNullArgument(provider, "provider");
			MigrationUtil.ThrowOnNullArgument(localizedError, "localizedError");
			if (!MigrationJobItem.IsFailedStatus(status))
			{
				throw new ArgumentException("Failed state not supported for " + status.ToString());
			}
			this.SetStatusAndSubscriptionLastChecked(provider, new MigrationUserStatus?(status), localizedError, new ExDateTime?(ExDateTime.UtcNow), true, null);
		}

		public void SetStatusAndSubscriptionLastChecked(IMigrationDataProvider provider, MigrationUserStatus? status, LocalizedException localizedError, ExDateTime? subscriptionLastChecked, bool supportIncrementalSync, ISubscriptionStatistics stats)
		{
			MigrationUtil.ThrowOnNullArgument(provider, "provider");
			if (status == null && localizedError != null)
			{
				throw new ArgumentException("error should not be updated without updating status");
			}
			if (status != null && (status == MigrationUserStatus.Failed || status == MigrationUserStatus.IncrementalFailed || status == MigrationUserStatus.CompletedWithWarnings || status == MigrationUserStatus.CompletionFailed) && localizedError == null)
			{
				throw new ArgumentException("An error message must be provided if the status is failed, CompletionFailed or CompletedWithWarnings");
			}
			MigrationStatusData<MigrationUserStatus> migrationStatusData = null;
			if (status != null)
			{
				migrationStatusData = new MigrationStatusData<MigrationUserStatus>(this.StatusData);
				if (MigrationJobItem.IsFailedStatus(status.Value))
				{
					migrationStatusData.UpdateStatus(status.Value, localizedError, MigrationLogger.CombineInternalError("SetStatusAndSubscriptionLastChecked: failed status", localizedError), true, null);
				}
				else
				{
					if (status.Value == MigrationUserStatus.Synced || status.Value == MigrationUserStatus.Completed || status.Value == MigrationUserStatus.CompletedWithWarnings)
					{
						if (this.StateLastUpdated == null)
						{
							throw MigrationHelperBase.CreatePermanentExceptionWithInternalData<MigrationUnknownException>("if we've completed, we should have a state last updated");
						}
						TimeSpan timeSpan = ExDateTime.UtcNow - this.StateLastUpdated.Value;
						if (this.InitialSyncDuration == null)
						{
							this.InitialSyncDuration = new TimeSpan?(timeSpan);
						}
						else if (this.IncrementalSyncDuration == null)
						{
							this.IncrementalSyncDuration = new TimeSpan?(timeSpan);
						}
						else
						{
							this.IncrementalSyncDuration += timeSpan;
						}
					}
					if (localizedError != null)
					{
						migrationStatusData.UpdateStatus(status.Value, localizedError, MigrationLogger.CombineInternalError("SetStatusAndSubscriptionLastChecked: setting warning", localizedError), null);
					}
					else
					{
						migrationStatusData.UpdateStatus(status.Value, null);
					}
				}
			}
			else if (subscriptionLastChecked != null && !string.IsNullOrEmpty(this.StatusData.InternalError))
			{
				migrationStatusData = new MigrationStatusData<MigrationUserStatus>(this.StatusData);
				MigrationLogger.Log(MigrationEventType.Information, "MigrationJobItem.SetStatusAndSubscriptionLastChecked: jobitem {0}, clearing out error: {1}", new object[]
				{
					this,
					migrationStatusData
				});
				migrationStatusData.ClearError();
			}
			if (!supportIncrementalSync || !this.ShouldMigrate)
			{
				MigrationLogger.Log(MigrationEventType.Verbose, "setting subscription last check to max date time for recipient type: {0}, overriding {1}", new object[]
				{
					this.RecipientType,
					subscriptionLastChecked
				});
				subscriptionLastChecked = new ExDateTime?(MigrationJobItem.MaxDateTimeValue);
			}
			this.SetStatusAndSubscriptionLastChecked(provider, migrationStatusData, subscriptionLastChecked, stats);
		}

		public void Delete(IMigrationDataProvider provider)
		{
			MigrationUtil.ThrowOnNullArgument(provider, "provider");
			if (base.StoreObjectId != null)
			{
				provider.RemoveMessage(base.StoreObjectId);
				if (this.MigrationJob != null && this.MigrationJob.ReportData != null)
				{
					this.MigrationJob.ReportData.Append(Strings.MigrationReportJobItemRemovedInternal(this.Identifier));
				}
			}
		}

		public void SetStatusData(IMigrationDataProvider provider, MigrationStatusData<MigrationUserStatus> statusData)
		{
			this.SetStatusData(provider, statusData, false);
		}

		public void SetSubscriptionId(IMigrationDataProvider provider, ISubscriptionId subscriptionId, MigrationUserStatus? itemStatus)
		{
			MigrationUtil.ThrowOnNullArgument(provider, "provider");
			MigrationLogger.Log(MigrationEventType.Verbose, "MigrationJobItem.SetSubscriptionId: Setting SubscriptionId for migration item: {0}.", new object[]
			{
				subscriptionId
			});
			ISubscriptionId subscriptionIdToSave = subscriptionId ?? SubscriptionIdHelper.Create(this.MigrationType, null, this.IsPAW);
			ExDateTime timeNow = ExDateTime.UtcNow;
			MigrationStatusData<MigrationUserStatus> statusData = null;
			if (itemStatus != null)
			{
				statusData = new MigrationStatusData<MigrationUserStatus>(this.StatusData);
				statusData.UpdateStatus(itemStatus.Value, null);
			}
			PropertyDefinition[] propertiesToUpdate = MigrationHelper.AggregateProperties(new IList<PropertyDefinition>[]
			{
				this.SubscriptionIdPropertyDefinitions,
				MigrationStatusData<MigrationUserStatus>.StatusPropertyDefinition,
				new StorePropertyDefinition[]
				{
					MigrationBatchMessageSchema.MigrationJobItemSubscriptionLastChecked
				}
			});
			this.UpdatePersistedMessage(provider, propertiesToUpdate, delegate(IMigrationMessageItem message)
			{
				if (subscriptionIdToSave != null)
				{
					subscriptionIdToSave.WriteToMessageItem(message, true);
				}
				if (statusData != null)
				{
					statusData.WriteToMessageItem(message, true);
					this.CheckAndReleaseSlotAssignmentIfNeeded(statusData.Status, message);
				}
				MigrationHelperBase.SetExDateTimeProperty(message, MigrationBatchMessageSchema.MigrationJobItemSubscriptionLastChecked, new ExDateTime?(timeNow));
			});
			this.SubscriptionId = subscriptionId;
			this.SubscriptionLastChecked = new ExDateTime?(timeNow);
			if (statusData != null)
			{
				this.StatusData = statusData;
				this.LogStatusEvent();
			}
		}

		public void SetMigrationFlags(IMigrationDataProvider provider, MigrationFlags flags)
		{
			MigrationUtil.ThrowOnNullArgument(provider, "provider");
			MigrationLogger.Log(MigrationEventType.Verbose, "MigrationJobItem.SetMigrationFlags: job-item {0} flags {1}", new object[]
			{
				this,
				flags
			});
			PropertyDefinition[] array = new StorePropertyDefinition[]
			{
				MigrationBatchMessageSchema.MigrationFlags
			};
			MigrationStatusData<MigrationUserStatus> statusData = null;
			MigrationUserStatus? migrationUserStatus = MigrationJobItem.ResolveFlagStatus(flags);
			if (migrationUserStatus != null)
			{
				array = MigrationHelper.AggregateProperties(new IList<PropertyDefinition>[]
				{
					MigrationStatusData<MigrationUserStatus>.StatusPropertyDefinition,
					array
				});
				statusData = new MigrationStatusData<MigrationUserStatus>(this.StatusData);
				statusData.UpdateStatus(migrationUserStatus.Value, null);
			}
			this.UpdatePersistedMessage(provider, array, delegate(IMigrationMessageItem message)
			{
				message[MigrationBatchMessageSchema.MigrationFlags] = flags;
				if (statusData != null)
				{
					statusData.WriteToMessageItem(message, true);
				}
			});
			this.Flags = flags;
			if (statusData != null)
			{
				this.StatusData = statusData;
			}
		}

		public void SetSubscriptionSettings(IMigrationDataProvider provider, JobItemSubscriptionSettingsBase subscriptionSettings)
		{
			MigrationUtil.ThrowOnNullArgument(provider, "provider");
			MigrationUtil.AssertOrThrow(base.StoreObjectId != null, "Should only work on an item that's been persisted", new object[0]);
			MigrationLogger.Log(MigrationEventType.Verbose, "MigrationJobItem.SetSubscriptionSettings for migration item: {0}.", new object[]
			{
				this
			});
			JobItemSubscriptionSettingsBase subscriptionSettingsToSave = subscriptionSettings ?? JobItemSubscriptionSettingsBase.Create(this.MigrationType);
			this.UpdatePersistedMessage(provider, this.SubscriptionSettingsPropertyDefinitions, delegate(IMigrationMessageItem message)
			{
				if (subscriptionSettingsToSave != null)
				{
					subscriptionSettingsToSave.WriteToMessageItem(message, true);
				}
			});
			this.SubscriptionSettings = subscriptionSettings;
		}

		public void SetStatusData(IMigrationDataProvider provider, MigrationStatusData<MigrationUserStatus> statusData, bool setLastRestartTime)
		{
			MigrationUtil.ThrowOnNullArgument(provider, "provider");
			MigrationUtil.ThrowOnNullArgument(statusData, "statusData");
			MigrationUtil.AssertOrThrow(base.StoreObjectId != null, "Should only work on an item that's been persisted", new object[0]);
			PropertyDefinition[] propertiesToUpdate = MigrationHelper.AggregateProperties(new IList<PropertyDefinition>[]
			{
				MigrationJobItem.updatePropertyDefinitionsBase,
				new StorePropertyDefinition[]
				{
					MigrationBatchMessageSchema.MigrationJobLastRestartTime
				}
			});
			this.UpdateIncrementalSyncFailureCount(statusData);
			ExDateTime timeNowUtc = ExDateTime.UtcNow;
			this.UpdatePersistedMessage(provider, propertiesToUpdate, delegate(IMigrationMessageItem message)
			{
				statusData.WriteToMessageItem(message, true);
				this.CheckAndReleaseSlotAssignmentIfNeeded(statusData.Status, message);
				if (setLastRestartTime)
				{
					MigrationHelperBase.SetExDateTimeProperty(message, MigrationBatchMessageSchema.MigrationJobLastRestartTime, new ExDateTime?(timeNowUtc));
				}
			});
			if (setLastRestartTime)
			{
				this.LastRestartTime = new ExDateTime?(timeNowUtc);
			}
			MigrationUserStatus migrationUserStatus = this.Status;
			this.StatusData = statusData;
			if (this.Status != migrationUserStatus)
			{
				this.LogStatusEvent();
			}
		}

		public void SetTroubleshooterNotes(IMigrationDataProvider provider, string notes)
		{
			this.TroubleshooterNotes = notes;
			this.UpdatePersistedMessage(provider, MigrationPersistableBase.MigrationBaseDefinitions, SaveMode.ResolveConflicts, new Action<IMigrationMessageItem>(this.WriteExtendedPropertiesToMessageItem));
		}

		public void UpdateConsumedSlot(Guid providerGuid, MigrationSlotType slotType, IMigrationDataProvider dataProvider)
		{
			this.MigrationSlotProviderGuid = providerGuid;
			this.ConsumedSlotType = slotType;
			this.UpdatePersistedMessage(dataProvider, this.MigrationJobSlotPropertyDefinitions, new Action<IMigrationMessageItem>(this.SaveConsumedSlotInformationToMessage));
		}

		public void SetSubscriptionLastUpdatedTime(IMigrationDataProvider provider, ExDateTime? subscriptionSettingsLastUpdatedTime)
		{
			MigrationUtil.ThrowOnNullArgument(provider, "provider");
			MigrationLogger.Log(MigrationEventType.Verbose, "MigrationJobItem.SetLastSubscriptionUpdatedTime: Setting last subscription updated time for migration item to: {0}.", new object[]
			{
				subscriptionSettingsLastUpdatedTime
			});
			this.UpdatePersistedMessage(provider, new StorePropertyDefinition[]
			{
				MigrationBatchMessageSchema.MigrationJobItemSubscriptionSettingsLastUpdatedTime
			}, delegate(IMigrationMessageItem message)
			{
				MigrationHelperBase.SetExDateTimeProperty(message, MigrationBatchMessageSchema.MigrationJobItemSubscriptionSettingsLastUpdatedTime, subscriptionSettingsLastUpdatedTime);
			});
			this.SubscriptionSettingsLastUpdatedTime = subscriptionSettingsLastUpdatedTime;
		}

		public override XElement GetDiagnosticInfo(IMigrationDataProvider dataProvider, MigrationDiagnosticArgument argument)
		{
			XElement xelement = new XElement("MigrationJobItem", new object[]
			{
				new XAttribute("name", this.Identifier),
				new XAttribute("guid", this.JobItemGuid)
			});
			if (this.StatusData != null)
			{
				xelement.Add(this.StatusData.GetDiagnosticInfo(dataProvider, argument));
			}
			if (this.IsPAW)
			{
				xelement.Add(new object[]
				{
					new XElement("flags", this.Flags),
					new XElement("nextProcessTime", this.NextProcessTime ?? ExDateTime.MinValue)
				});
				if (this.WorkflowPosition != null)
				{
					xelement.Add(this.WorkflowPosition.GetDiagnosticInfo(dataProvider, argument));
				}
			}
			base.GetDiagnosticInfo(dataProvider, argument, xelement);
			if (!argument.HasArgument("verbose"))
			{
				return xelement;
			}
			xelement.Add(new XElement("localMailboxIdentifier", this.LocalMailboxIdentifier));
			xelement.Add(new XElement("remoteIdentifier", this.RemoteIdentifier));
			xelement.Add(new XElement("migrationJobItemType", this.MigrationType));
			xelement.Add(new XElement("recipientType", this.RecipientType));
			if (this.LocalMailbox != null)
			{
				xelement.Add(this.LocalMailbox.GetDiagnosticInfo(dataProvider, argument));
			}
			xelement.Add(new object[]
			{
				new XElement("version", base.Version),
				new XElement("messageId", base.StoreObjectId),
				new XElement("belongsToJob", this.MigrationJobId),
				new XElement("subscriptionLastChecked", this.SubscriptionLastChecked),
				new XElement("subscriptionQueuedTime", this.SubscriptionQueuedTime),
				new XElement("provisionedTime", this.ProvisionedTime),
				new XElement("SubscriptionSettingsLastUpdatedTime", this.SubscriptionSettingsLastUpdatedTime),
				new XElement("MigrationSlotProviderGuid", this.MigrationSlotProviderGuid),
				new XElement("ConsumedSlotType", this.ConsumedSlotType)
			});
			if (this.ProvisioningData != null)
			{
				xelement.Add(this.ProvisioningData.GetDiagnosticInfo(dataProvider, argument));
			}
			if (this.MigrationType == MigrationType.PublicFolder)
			{
				xelement.Add(new XElement("LastFinalizationAttempt", this.LastFinalizationAttempt));
				xelement.Add(new XElement("PublicFolderCompletionFailures", this.PublicFolderCompletionFailures));
			}
			if (this.IsPAW)
			{
				IMigrationSerializable migrationSerializable = this.ProvisioningStatistics as IMigrationSerializable;
				if (migrationSerializable != null)
				{
					xelement.Add(migrationSerializable.GetDiagnosticInfo(dataProvider, argument));
				}
			}
			if (this.SubscriptionId != null)
			{
				xelement.Add(this.SubscriptionId.GetDiagnosticInfo(dataProvider, argument));
			}
			if (this.SubscriptionSettings != null)
			{
				xelement.Add(this.SubscriptionSettings.GetDiagnosticInfo(dataProvider, argument));
			}
			if (this.SubscriptionStatistics != null)
			{
				xelement.Add(this.SubscriptionStatistics.GetDiagnosticInfo(dataProvider, argument));
			}
			return xelement;
		}

		public override string ToString()
		{
			if (this.IsPAW)
			{
				return string.Format("{0} ({1}\\{2}) {3} {4}", new object[]
				{
					this.Identifier,
					this.MigrationJobId,
					this.JobItemGuid,
					this.WorkflowPosition,
					this.State
				});
			}
			return string.Format("{0}:{1}:{2}:{3}", new object[]
			{
				this.JobItemGuid,
				this.MigrationJobId,
				this.Identifier,
				this.Status
			});
		}

		public override bool ReadFromMessageItem(IMigrationStoreObject message)
		{
			base.ReadFromMessageItem(message);
			if (this.IsPAW)
			{
				this.Flags = MigrationHelper.GetEnumProperty<MigrationFlags>(message, MigrationBatchMessageSchema.MigrationFlags);
				this.NextProcessTime = MigrationHelper.GetExDateTimePropertyOrNull(message, MigrationBatchMessageSchema.MigrationNextProcessTime);
				this.WorkflowPosition = MigrationWorkflowPosition.CreateFromMessage(message);
			}
			this.MigrationJobId = MigrationHelper.GetGuidProperty(message, MigrationBatchMessageSchema.MigrationJobId, true);
			this.Identifier = message.GetValueOrDefault<string>(MigrationBatchMessageSchema.MigrationJobItemIdentifier, null);
			this.LocalMailboxIdentifier = message.GetValueOrDefault<string>(MigrationBatchMessageSchema.MigrationJobItemLocalMailboxIdentifier, this.Identifier);
			this.JobItemGuid = MigrationHelper.GetGuidProperty(message, MigrationBatchMessageSchema.MigrationJobItemId, false);
			this.StatusData = MigrationStatusData<MigrationUserStatus>.Create(message, MigrationJobItem.StatusDataVersionMap[base.Version]);
			this.LocalMailbox = MailboxDataHelper.CreateFromMessage(message, this.MigrationType);
			if (this.LocalMailbox != null)
			{
				this.LocalMailbox.Update(this.LocalMailboxIdentifier, base.OrganizationId);
			}
			this.RecipientType = MigrationHelper.GetEnumProperty<MigrationUserRecipientType>(message, MigrationBatchMessageSchema.MigrationJobItemRecipientType);
			this.SubscriptionLastChecked = MigrationHelper.GetExDateTimePropertyOrNull(message, MigrationBatchMessageSchema.MigrationJobItemSubscriptionLastChecked);
			this.SubscriptionQueuedTime = MigrationHelper.GetExDateTimePropertyOrNull(message, MigrationBatchMessageSchema.MigrationJobItemSubscriptionQueuedTime);
			this.SubscriptionDisableTime = MigrationHelper.GetExDateTimePropertyOrNull(message, MigrationBatchMessageSchema.MigrationDisableTime);
			this.ProvisionedTime = MigrationHelper.GetExDateTimePropertyOrNull(message, MigrationBatchMessageSchema.MigrationProvisionedTime);
			this.LastRestartTime = MigrationHelper.GetExDateTimePropertyOrNull(message, MigrationBatchMessageSchema.MigrationJobLastRestartTime);
			this.SubscriptionSettingsLastUpdatedTime = MigrationHelper.GetExDateTimePropertyOrNull(message, MigrationBatchMessageSchema.MigrationJobItemSubscriptionSettingsLastUpdatedTime);
			this.CursorPosition = message.GetValueOrDefault<int>(this.CursorPositionPropertyDefinition, -1);
			this.ConsumedSlotType = (MigrationHelper.GetEnumPropertyOrNull<MigrationSlotType>(message, MigrationBatchMessageSchema.MigrationJobItemSlotType) ?? MigrationSlotType.None);
			this.MigrationSlotProviderGuid = MigrationHelper.GetGuidProperty(message, MigrationBatchMessageSchema.MigrationJobItemSlotProviderId, false);
			this.RemoteIdentifier = message.GetValueOrDefault<string>(MigrationBatchMessageSchema.MigrationJobItemIncomingUsername, null);
			this.LastFinalizationAttempt = message.GetValueOrDefault<int>(MigrationBatchMessageSchema.MigrationJobLastFinalizationAttempt, 0);
			if (this.ShouldProvision)
			{
				this.ProvisioningData = ProvisioningDataStorageBase.CreateFromMessage(message, this.MigrationType, this.RecipientType, this.IsPAW);
				if (this.IsPAW)
				{
					this.ProvisioningStatistics = ProvisioningSnapshot.CreateFromMessage(message, this.RecipientType);
				}
			}
			this.SubscriptionId = SubscriptionIdHelper.CreateFromMessage(message, this.MigrationType, this.LocalMailbox, this.IsPAW);
			this.SubscriptionSettings = JobItemSubscriptionSettingsBase.CreateFromMessage(message, this.MigrationType);
			this.SubscriptionStatistics = SubscriptionSnapshot.CreateFromMessage(message);
			return true;
		}

		public override void WriteToMessageItem(IMigrationStoreObject message, bool loaded)
		{
			message[MigrationBatchMessageSchema.MigrationType] = (int)this.MigrationType;
			message[MigrationBatchMessageSchema.MigrationVersion] = base.Version;
			message[StoreObjectSchema.ItemClass] = MigrationBatchMessageSchema.MigrationJobItemClass;
			message[MigrationBatchMessageSchema.MigrationJobId] = this.MigrationJobId;
			message[MigrationBatchMessageSchema.MigrationJobItemId] = this.JobItemGuid;
			message[MigrationBatchMessageSchema.MigrationJobItemRecipientType] = this.RecipientType;
			message[MigrationBatchMessageSchema.MigrationJobItemIdentifier] = this.Identifier;
			message[MigrationBatchMessageSchema.MigrationJobItemItemsSynced] = this.ItemsSynced;
			message[MigrationBatchMessageSchema.MigrationJobItemItemsSkipped] = this.ItemsSkipped;
			if (!string.Equals(this.Identifier, this.LocalMailboxIdentifier, StringComparison.OrdinalIgnoreCase))
			{
				message[MigrationBatchMessageSchema.MigrationJobItemLocalMailboxIdentifier] = this.LocalMailboxIdentifier;
			}
			if (this.IsPAW)
			{
				message[MigrationBatchMessageSchema.MigrationFlags] = this.Flags;
				MigrationHelperBase.SetExDateTimeProperty(message, MigrationBatchMessageSchema.MigrationNextProcessTime, this.NextProcessTime);
				if (this.WorkflowPosition != null)
				{
					this.WorkflowPosition.WriteToMessageItem(message, loaded);
				}
			}
			this.StatusData.WriteToMessageItem(message, loaded);
			if (this.LocalMailbox != null)
			{
				this.LocalMailbox.WriteToMessageItem(message, loaded);
			}
			if (this.SubscriptionLastChecked != null)
			{
				message[MigrationBatchMessageSchema.MigrationJobItemSubscriptionLastChecked] = this.SubscriptionLastChecked.Value;
			}
			if (this.SubscriptionDisableTime != null)
			{
				message[MigrationBatchMessageSchema.MigrationDisableTime] = this.SubscriptionDisableTime.Value;
			}
			MigrationHelperBase.SetExDateTimeProperty(message, MigrationBatchMessageSchema.MigrationJobLastRestartTime, this.LastRestartTime);
			MigrationHelperBase.SetExDateTimeProperty(message, MigrationBatchMessageSchema.MigrationJobItemSubscriptionSettingsLastUpdatedTime, this.SubscriptionSettingsLastUpdatedTime);
			this.SaveConsumedSlotInformationToMessage(message);
			if (this.RemoteIdentifier != null)
			{
				message[MigrationBatchMessageSchema.MigrationJobItemIncomingUsername] = this.RemoteIdentifier;
			}
			message[this.CursorPositionPropertyDefinition] = this.CursorPosition;
			if (this.ProvisioningData != null)
			{
				this.ProvisioningData.WriteToMessageItem(message, loaded);
			}
			if (this.MigrationType == MigrationType.PublicFolder)
			{
				message[MigrationBatchMessageSchema.MigrationJobLastFinalizationAttempt] = this.LastFinalizationAttempt;
			}
			if (this.IsPAW)
			{
				IMigrationSerializable migrationSerializable = this.ProvisioningStatistics as IMigrationSerializable;
				if (migrationSerializable != null)
				{
					migrationSerializable.WriteToMessageItem(message, loaded);
				}
			}
			if (this.SubscriptionId != null)
			{
				this.SubscriptionId.WriteToMessageItem(message, loaded);
			}
			if (this.SubscriptionSettings != null)
			{
				this.SubscriptionSettings.WriteToMessageItem(message, loaded);
			}
			base.WriteToMessageItem(message, loaded);
		}

		internal static MigrationJobItem Load(IMigrationDataProvider provider, StoreObjectId messageId, MigrationJobObjectCache jobCache, bool throwOnError = true)
		{
			MigrationUtil.ThrowOnNullArgument(provider, "provider");
			MigrationUtil.ThrowOnNullArgument(messageId, "messageId");
			MigrationUtil.ThrowOnNullArgument(jobCache, "jobCache");
			MigrationJobItem result;
			using (IMigrationMessageItem migrationMessageItem = provider.FindMessage(messageId, new StorePropertyDefinition[]
			{
				MigrationBatchMessageSchema.MigrationType,
				MigrationBatchMessageSchema.MigrationJobId
			}))
			{
				MigrationType valueOrDefault = migrationMessageItem.GetValueOrDefault<MigrationType>(MigrationBatchMessageSchema.MigrationType, MigrationType.None);
				Guid valueOrDefault2 = migrationMessageItem.GetValueOrDefault<Guid>(MigrationBatchMessageSchema.MigrationJobId, Guid.Empty);
				MigrationJobItem migrationJobItem = new MigrationJobItem(valueOrDefault);
				migrationJobItem.MigrationJob = jobCache.GetJob(valueOrDefault2);
				if (!migrationJobItem.TryLoad(provider, messageId))
				{
					if (throwOnError)
					{
						throw new CouldNotLoadMigrationPersistedItemTransientException(messageId.ToHexEntryId());
					}
					result = null;
				}
				else
				{
					result = migrationJobItem;
				}
			}
			return result;
		}

		internal static MigrationUserStatus? ResolveFlagStatus(MigrationFlags flags)
		{
			MigrationUserStatus? result = null;
			if (flags.HasFlag(MigrationFlags.Remove))
			{
				result = new MigrationUserStatus?(MigrationUserStatus.Removing);
			}
			else if (flags.HasFlag(MigrationFlags.Stop))
			{
				result = new MigrationUserStatus?(MigrationUserStatus.Stopping);
			}
			else if (flags.HasFlag(MigrationFlags.Start))
			{
				result = new MigrationUserStatus?(MigrationUserStatus.Starting);
			}
			return result;
		}

		internal static bool IsFailedStatus(MigrationUserStatus status)
		{
			foreach (MigrationUserStatus migrationUserStatus in MigrationJobItem.FailedStatuses)
			{
				if (status == migrationUserStatus)
				{
					return true;
				}
			}
			return false;
		}

		internal static bool IsStoppedStatus(MigrationUserStatus status)
		{
			foreach (MigrationUserStatus migrationUserStatus in MigrationJobItem.StoppedStatuses)
			{
				if (status == migrationUserStatus)
				{
					return true;
				}
			}
			return false;
		}

		internal static IEnumerable<StoreObjectId> GetIdsByState(IMigrationDataProvider provider, MigrationJob job, MigrationState state, ExDateTime? nextProcessTime = null, int? maxCount = null)
		{
			MigrationUtil.ThrowOnNullArgument(provider, "provider");
			MigrationUtil.ThrowOnNullArgument(job, "job");
			List<QueryFilter> list = new List<QueryFilter>
			{
				new ComparisonFilter(ComparisonOperator.Equal, StoreObjectSchema.ItemClass, MigrationBatchMessageSchema.MigrationJobItemClass),
				new ComparisonFilter(ComparisonOperator.Equal, MigrationBatchMessageSchema.MigrationJobId, job.JobId),
				new ComparisonFilter(ComparisonOperator.Equal, MigrationBatchMessageSchema.MigrationState, state)
			};
			List<PropertyDefinition> list2 = new List<PropertyDefinition>
			{
				StoreObjectSchema.ItemClass,
				MigrationBatchMessageSchema.MigrationJobId,
				MigrationBatchMessageSchema.MigrationState
			};
			if (nextProcessTime != null)
			{
				list.Add(new ComparisonFilter(ComparisonOperator.LessThanOrEqual, MigrationBatchMessageSchema.MigrationNextProcessTime, nextProcessTime));
				list2.Add(MigrationBatchMessageSchema.MigrationNextProcessTime);
			}
			return provider.FindMessageIds(QueryFilter.AndTogether(list.ToArray()), list2.ToArray(), MigrationJobItem.StateSort, delegate(IDictionary<PropertyDefinition, object> row)
			{
				if (!object.Equals(row[StoreObjectSchema.ItemClass], MigrationBatchMessageSchema.MigrationJobItemClass))
				{
					return MigrationRowSelectorResult.RejectRowStopProcessing;
				}
				if (!object.Equals(row[MigrationBatchMessageSchema.MigrationJobId], job.JobId))
				{
					return MigrationRowSelectorResult.RejectRowStopProcessing;
				}
				if ((MigrationState)row[MigrationBatchMessageSchema.MigrationState] != state)
				{
					return MigrationRowSelectorResult.RejectRowStopProcessing;
				}
				if (nextProcessTime != null && ExDateTime.Compare((ExDateTime)row[MigrationBatchMessageSchema.MigrationNextProcessTime], nextProcessTime.Value) > 0)
				{
					return MigrationRowSelectorResult.RejectRowStopProcessing;
				}
				return MigrationRowSelectorResult.AcceptRow;
			}, maxCount);
		}

		internal static IEnumerable<StoreObjectId> GetIdsByFlag(IMigrationDataProvider provider, MigrationJob job, MigrationFlags flag, MigrationState? state = null, int? maxCount = null)
		{
			List<QueryFilter> list = new List<QueryFilter>
			{
				new ComparisonFilter(ComparisonOperator.Equal, StoreObjectSchema.ItemClass, MigrationBatchMessageSchema.MigrationJobItemClass),
				new ComparisonFilter(ComparisonOperator.Equal, MigrationBatchMessageSchema.MigrationJobId, job.JobId),
				new ComparisonFilter(ComparisonOperator.Equal, MigrationBatchMessageSchema.MigrationFlags, flag)
			};
			List<PropertyDefinition> list2 = new List<PropertyDefinition>
			{
				StoreObjectSchema.ItemClass,
				MigrationBatchMessageSchema.MigrationJobId,
				MigrationBatchMessageSchema.MigrationFlags,
				MigrationBatchMessageSchema.MigrationState
			};
			if (state != null)
			{
				list.Add(new ComparisonFilter(ComparisonOperator.Equal, MigrationBatchMessageSchema.MigrationState, state.Value));
			}
			else
			{
				list.Add(new ComparisonFilter(ComparisonOperator.NotEqual, MigrationBatchMessageSchema.MigrationState, MigrationState.Disabled));
			}
			return provider.FindMessageIds(QueryFilter.AndTogether(list.ToArray()), list2.ToArray(), MigrationJobItem.FlagSort, delegate(IDictionary<PropertyDefinition, object> row)
			{
				if (!object.Equals(row[StoreObjectSchema.ItemClass], MigrationBatchMessageSchema.MigrationJobItemClass))
				{
					return MigrationRowSelectorResult.RejectRowStopProcessing;
				}
				if (!object.Equals(row[MigrationBatchMessageSchema.MigrationJobId], job.JobId))
				{
					return MigrationRowSelectorResult.RejectRowStopProcessing;
				}
				if (!object.Equals((MigrationFlags)row[MigrationBatchMessageSchema.MigrationFlags], flag))
				{
					return MigrationRowSelectorResult.RejectRowStopProcessing;
				}
				if ((state != null && (MigrationState)row[MigrationBatchMessageSchema.MigrationState] != state.Value) || (state == null && (MigrationState)row[MigrationBatchMessageSchema.MigrationState] == MigrationState.Disabled))
				{
					return MigrationRowSelectorResult.RejectRowContinueProcessing;
				}
				return MigrationRowSelectorResult.AcceptRow;
			}, maxCount);
		}

		internal static IEnumerable<StoreObjectId> GetIdsWithFlagPresence(IMigrationDataProvider provider, MigrationJob job, bool present, int? maxCount = null)
		{
			List<QueryFilter> list = new List<QueryFilter>
			{
				new ComparisonFilter(ComparisonOperator.Equal, StoreObjectSchema.ItemClass, MigrationBatchMessageSchema.MigrationJobItemClass),
				new ComparisonFilter(ComparisonOperator.Equal, MigrationBatchMessageSchema.MigrationJobId, job.JobId),
				new ComparisonFilter(present ? ComparisonOperator.NotEqual : ComparisonOperator.Equal, MigrationBatchMessageSchema.MigrationFlags, MigrationFlags.None),
				new ComparisonFilter(ComparisonOperator.NotEqual, MigrationBatchMessageSchema.MigrationState, MigrationState.Disabled)
			};
			List<PropertyDefinition> list2 = new List<PropertyDefinition>
			{
				StoreObjectSchema.ItemClass,
				MigrationBatchMessageSchema.MigrationJobId,
				MigrationBatchMessageSchema.MigrationFlags,
				MigrationBatchMessageSchema.MigrationState
			};
			return provider.FindMessageIds(QueryFilter.AndTogether(list.ToArray()), list2.ToArray(), MigrationJobItem.FlagSort, delegate(IDictionary<PropertyDefinition, object> row)
			{
				if (!object.Equals(row[StoreObjectSchema.ItemClass], MigrationBatchMessageSchema.MigrationJobItemClass))
				{
					return MigrationRowSelectorResult.RejectRowStopProcessing;
				}
				if (!object.Equals(row[MigrationBatchMessageSchema.MigrationJobId], job.JobId))
				{
					return MigrationRowSelectorResult.RejectRowStopProcessing;
				}
				bool flag = (MigrationFlags)row[MigrationBatchMessageSchema.MigrationFlags] != MigrationFlags.None;
				if (present != flag)
				{
					return MigrationRowSelectorResult.RejectRowStopProcessing;
				}
				if ((MigrationState)row[MigrationBatchMessageSchema.MigrationState] == MigrationState.Disabled)
				{
					return MigrationRowSelectorResult.RejectRowContinueProcessing;
				}
				return MigrationRowSelectorResult.AcceptRow;
			}, maxCount);
		}

		internal static IEnumerable<StoreObjectId> GetAllIds(IMigrationDataProvider provider, MigrationJob job, int? maxCount = null)
		{
			QueryFilter[] filters = new ComparisonFilter[]
			{
				new ComparisonFilter(ComparisonOperator.Equal, StoreObjectSchema.ItemClass, MigrationBatchMessageSchema.MigrationJobItemClass),
				new ComparisonFilter(ComparisonOperator.Equal, MigrationBatchMessageSchema.MigrationJobId, job.JobId)
			};
			return provider.FindMessageIds(QueryFilter.AndTogether(filters), new StorePropertyDefinition[]
			{
				StoreObjectSchema.ItemClass,
				MigrationBatchMessageSchema.MigrationJobId
			}, MigrationJobItem.FlagSort, delegate(IDictionary<PropertyDefinition, object> row)
			{
				if (!object.Equals(row[StoreObjectSchema.ItemClass], MigrationBatchMessageSchema.MigrationJobItemClass))
				{
					return MigrationRowSelectorResult.RejectRowStopProcessing;
				}
				if (!object.Equals(row[MigrationBatchMessageSchema.MigrationJobId], job.JobId))
				{
					return MigrationRowSelectorResult.RejectRowStopProcessing;
				}
				return MigrationRowSelectorResult.AcceptRow;
			}, maxCount);
		}

		internal static IEnumerable<MigrationJobItem> GetByFilter(IMigrationDataProvider provider, MigrationEqualityFilter primaryFilter, MigrationEqualityFilter[] secondaryFilters, SortBy[] additionalSorts, MigrationJobObjectCache jobCache, int? maxCount)
		{
			IEnumerable<StoreObjectId> messageIds = MigrationHelper.FindMessageIds(provider, primaryFilter, secondaryFilters, additionalSorts, maxCount);
			if (maxCount != null)
			{
				messageIds = new List<StoreObjectId>(messageIds);
			}
			foreach (StoreObjectId messageId in messageIds)
			{
				MigrationJobItem jobItem = MigrationJobItem.Load(provider, messageId, jobCache, true);
				yield return jobItem;
			}
			yield break;
		}

		internal static MigrationRowSelectorResult FilterJobItemsByColumnLastUpdated(IDictionary<PropertyDefinition, object> rowData, StorePropertyDefinition columnWithDateInfo, Guid? jobId, ExDateTime? lastCheckedTime, MigrationUserStatus? status)
		{
			if (!StringComparer.InvariantCultureIgnoreCase.Equals(rowData[StoreObjectSchema.ItemClass], MigrationBatchMessageSchema.MigrationJobItemClass))
			{
				return MigrationRowSelectorResult.RejectRowContinueProcessing;
			}
			if (status != null && status.Value != (MigrationUserStatus)rowData[MigrationBatchMessageSchema.MigrationUserStatus])
			{
				return MigrationRowSelectorResult.RejectRowStopProcessing;
			}
			if (jobId != null && !MigrationHelper.IsEqualXsoValues(jobId.Value, rowData[MigrationBatchMessageSchema.MigrationJobId]))
			{
				return MigrationRowSelectorResult.RejectRowContinueProcessing;
			}
			object obj;
			if (lastCheckedTime != null && rowData.TryGetValue(columnWithDateInfo, out obj) && obj is ExDateTime)
			{
				ExDateTime exDateTime = (ExDateTime)obj;
				if (exDateTime >= lastCheckedTime.Value)
				{
					MigrationLogger.Log(MigrationEventType.Information, "the stored time {0} the cutoff time {1}", new object[]
					{
						exDateTime,
						lastCheckedTime.Value
					});
					return MigrationRowSelectorResult.RejectRowStopProcessing;
				}
			}
			return MigrationRowSelectorResult.AcceptRow;
		}

		internal static IEnumerable<MigrationJobItem> LoadJobItemsWithStatus(IMigrationDataProvider provider, IEnumerable<StoreObjectId> messageIdList, MigrationUserStatus status, MigrationJobObjectCache jobCache)
		{
			foreach (StoreObjectId messageId in new List<StoreObjectId>(messageIdList))
			{
				MigrationJobItem jobItem = MigrationJobItem.Load(provider, messageId, jobCache, true);
				if (jobItem.Status == status)
				{
					yield return jobItem;
				}
			}
			yield break;
		}

		protected override bool InitializeFromMessageItem(IMigrationStoreObject message)
		{
			if (!base.InitializeFromMessageItem(message))
			{
				return false;
			}
			MigrationType migrationType = (MigrationType)message[MigrationBatchMessageSchema.MigrationType];
			if (this.MigrationType != migrationType)
			{
				throw new MigrationDataCorruptionException(string.Format("job type not set correctly.  expected {0}, found {1}", this.MigrationType, migrationType));
			}
			return true;
		}

		private static IEnumerable<MigrationJobItem> GetByColumnLastUpdated(IMigrationDataProvider provider, StorePropertyDefinition columnWithDateInfo, MigrationJob job, ExDateTime? lastCheckedTime, MigrationUserStatus status, int maxCount)
		{
			MigrationUtil.ThrowOnNullArgument(provider, "provider");
			MigrationUtil.ThrowOnNullArgument(job, "job");
			MigrationJobObjectCache migrationJobObjectCache = new MigrationJobObjectCache(provider);
			migrationJobObjectCache.PreSeed(job);
			MigrationEqualityFilter primaryFilter = new MigrationEqualityFilter(MigrationBatchMessageSchema.MigrationUserStatus, status);
			PropertyDefinition[] filterColumns = new PropertyDefinition[]
			{
				MigrationBatchMessageSchema.MigrationJobId,
				columnWithDateInfo,
				StoreObjectSchema.ItemClass
			};
			SortBy[] additionalSorts = new SortBy[]
			{
				new SortBy(MigrationBatchMessageSchema.MigrationJobId, SortOrder.Ascending),
				new SortBy(columnWithDateInfo, SortOrder.Ascending)
			};
			IEnumerable<StoreObjectId> messageIdList = provider.FindMessageIds(primaryFilter, filterColumns, additionalSorts, (IDictionary<PropertyDefinition, object> rowData) => MigrationJobItem.FilterJobItemsByColumnLastUpdated(rowData, columnWithDateInfo, new Guid?(job.JobId), lastCheckedTime, new MigrationUserStatus?(status)), new int?(maxCount));
			return MigrationJobItem.LoadJobItemsWithStatus(provider, messageIdList, status, migrationJobObjectCache);
		}

		private static StorePropertyDefinition GetCursorPositionProperty(MigrationType migrationType)
		{
			StorePropertyDefinition result;
			if (migrationType == MigrationType.ExchangeOutlookAnywhere)
			{
				result = MigrationBatchMessageSchema.MigrationJobItemExchangeRecipientIndex;
			}
			else
			{
				result = MigrationBatchMessageSchema.MigrationJobItemRowIndex;
			}
			return result;
		}

		private void SetUserMailboxProperties(IMigrationDataProvider provider, MigrationStatusData<MigrationUserStatus> statusData, MailboxData mailboxData, ExDateTime? provisionedTime)
		{
			MigrationUtil.ThrowOnNullArgument(provider, "provider");
			MigrationLogger.Log(MigrationEventType.Verbose, "MigrationJobItem.UpdateMailboxServer: jobitem {0}, status {1}, mailboxdata {2}", new object[]
			{
				this,
				statusData,
				mailboxData
			});
			PropertyDefinition[] propertiesToUpdate = MigrationHelper.AggregateProperties(new IList<PropertyDefinition>[]
			{
				MigrationJobItem.MailboxDataUpdateIndex,
				this.MigrationJobSlotPropertyDefinitions
			});
			this.UpdatePersistedMessage(provider, propertiesToUpdate, delegate(IMigrationMessageItem message)
			{
				if (mailboxData != null)
				{
					mailboxData.WriteToMessageItem(message, true);
				}
				if (statusData != null)
				{
					statusData.WriteToMessageItem(message, true);
					this.CheckAndReleaseSlotAssignmentIfNeeded(statusData.Status, message);
				}
				if (provisionedTime != null)
				{
					message[MigrationBatchMessageSchema.MigrationProvisionedTime] = provisionedTime.Value;
				}
			});
			if (mailboxData != null)
			{
				this.LocalMailbox = mailboxData;
			}
			if (provisionedTime != null)
			{
				this.ProvisionedTime = provisionedTime;
			}
			if (statusData != null)
			{
				MigrationUserStatus migrationUserStatus = this.Status;
				this.StatusData = statusData;
				if (this.Status != migrationUserStatus)
				{
					this.LogStatusEvent();
				}
			}
		}

		private void UpdateIncrementalSyncFailureCount(MigrationStatusData<MigrationUserStatus> newStatus)
		{
			if (newStatus.Status == MigrationUserStatus.IncrementalFailed)
			{
				this.IncrementalSyncFailures++;
				return;
			}
			if (newStatus.Status != MigrationUserStatus.IncrementalSyncing)
			{
				this.IncrementalSyncFailures = 0;
			}
		}

		private void SaveConsumedSlotInformationToMessage(IMigrationStoreObject message)
		{
			message[MigrationBatchMessageSchema.MigrationJobItemSlotProviderId] = this.MigrationSlotProviderGuid;
			message[MigrationBatchMessageSchema.MigrationJobItemSlotType] = this.ConsumedSlotType;
		}

		private void SetStatusAndSubscriptionLastChecked(IMigrationDataProvider provider, MigrationStatusData<MigrationUserStatus> statusData, ExDateTime? subscriptionLastChecked, ISubscriptionStatistics stats)
		{
			MigrationUtil.ThrowOnNullArgument(provider, "provider");
			MigrationUtil.AssertOrThrow(base.StoreObjectId != null, "Should only work on an item that's been persisted", new object[0]);
			if (statusData == null && subscriptionLastChecked == null && stats == null)
			{
				throw new ArgumentException("One or more of status,subcriptionCheckDate or stats should be specified");
			}
			if (stats != null)
			{
				MigrationUtil.ThrowOnLessThanZeroArgument(stats.NumItemsSynced, "itemsSynced");
				MigrationUtil.ThrowOnLessThanZeroArgument(stats.NumItemsSkipped, "itemsSkipped");
			}
			MigrationLogger.Log(MigrationEventType.Verbose, "MigrationJobItem.SetStatusAndCheckDate: jobitem {0}, status {1}, lastCheck {2}", new object[]
			{
				this,
				statusData,
				subscriptionLastChecked
			});
			if (statusData != null)
			{
				this.UpdateIncrementalSyncFailureCount(statusData);
			}
			PropertyDefinition[] propertiesToUpdate = MigrationHelper.AggregateProperties(new IList<PropertyDefinition>[]
			{
				MigrationJobItem.MigrationJobItemColumnsStatusIndex,
				this.MigrationJobSlotPropertyDefinitions
			});
			this.UpdatePersistedMessage(provider, propertiesToUpdate, delegate(IMigrationMessageItem message)
			{
				if (statusData != null)
				{
					statusData.WriteToMessageItem(message, true);
					this.WriteExtendedPropertiesToMessageItem(message);
					this.CheckAndReleaseSlotAssignmentIfNeeded(statusData.Status, message);
				}
				if (subscriptionLastChecked != null)
				{
					if (this.IncrementalSyncInterval != null)
					{
						ExDateTime exDateTime = subscriptionLastChecked.Value + this.IncrementalSyncInterval.Value;
						MigrationLogger.Log(MigrationEventType.Verbose, "changing subscription last checked from {0} to {1}", new object[]
						{
							subscriptionLastChecked.Value,
							exDateTime
						});
						subscriptionLastChecked = new ExDateTime?(exDateTime);
					}
					message[MigrationBatchMessageSchema.MigrationJobItemSubscriptionLastChecked] = subscriptionLastChecked.Value;
				}
				this.WriteLastFinalizationAttemptData(stats, statusData, message);
				if (stats != null)
				{
					stats.WriteToMessageItem(message, true);
				}
			});
			if (subscriptionLastChecked != null)
			{
				this.SubscriptionLastChecked = subscriptionLastChecked;
			}
			if (stats != null)
			{
				this.SubscriptionStatistics = stats;
			}
			if (statusData != null)
			{
				MigrationUserStatus migrationUserStatus = this.Status;
				this.StatusData = statusData;
				if (this.Status != migrationUserStatus)
				{
					this.LogStatusEvent();
				}
			}
		}

		private void WriteLastFinalizationAttemptData(ISubscriptionStatistics stats, MigrationStatusData<MigrationUserStatus> statusData, IMigrationMessageItem message)
		{
			if (this.MigrationType != MigrationType.PublicFolder)
			{
				return;
			}
			if (this.MigrationJob.Status == MigrationJobStatus.CompletionInitializing && this.MigrationJob.LastFinalizationAttempt + this.LastFinalizationAttempt == 0 && statusData != null)
			{
				if (statusData.Status == MigrationUserStatus.Synced && stats != null && stats.LastSyncTime >= this.MigrationJob.GetEffectiveFinalizationTime())
				{
					this.LastFinalizationAttempt = this.MigrationJob.LastFinalizationAttempt;
				}
				else if (MigrationJobItem.IsFailedStatus(statusData.Status))
				{
					this.PublicFolderCompletionFailures++;
					this.WriteExtendedPropertiesToMessageItem(message);
					int config = ConfigBase<MigrationServiceConfigSchema>.GetConfig<int>("MigrationPublicFolderCompletionFailureThreshold");
					if (this.PublicFolderCompletionFailures >= config)
					{
						this.LastFinalizationAttempt = this.MigrationJob.LastFinalizationAttempt;
					}
				}
				message[MigrationBatchMessageSchema.MigrationJobLastFinalizationAttempt] = this.LastFinalizationAttempt;
			}
		}

		private void Initialize(MigrationJob job, IMigrationDataRow dataRow, IMailboxData mailboxData, MigrationUserStatus status, LocalizedException localizedError, MigrationState? state = null, MigrationWorkflowPosition position = null)
		{
			MigrationUtil.ThrowOnNullArgument(job, "job");
			MigrationUtil.ThrowOnNullArgument(dataRow, "dataRow");
			bool isPAW = job.IsPAW;
			if (isPAW)
			{
				this.currentSupportedVersion = 4L;
			}
			else
			{
				this.currentSupportedVersion = 3L;
			}
			if (dataRow.MigrationType != this.MigrationType)
			{
				throw new ArgumentException(string.Format("DataRow should be of type {0}, but was {1}", this.MigrationType, dataRow.MigrationType));
			}
			this.Identifier = dataRow.Identifier;
			this.LocalMailboxIdentifier = dataRow.LocalMailboxIdentifier;
			this.JobItemGuid = Guid.NewGuid();
			if (dataRow.SupportsRemoteIdentifier)
			{
				this.RemoteIdentifier = dataRow.RemoteIdentifier;
			}
			this.MigrationJobId = job.JobId;
			this.BatchInputId = job.BatchInputId;
			if (MigrationJobItem.IsFailedStatus(status) || !isPAW)
			{
				this.LastRestartTime = new ExDateTime?(ExDateTime.UtcNow);
			}
			this.StatusData = new MigrationStatusData<MigrationUserStatus>(status, localizedError, MigrationJobItem.StatusDataVersionMap[this.currentSupportedVersion], state);
			this.RecipientType = dataRow.RecipientType;
			if (position != null)
			{
				this.WorkflowPosition = position;
			}
			this.JobName = job.JobName;
			this.IsStaged = job.IsStaged;
			if (mailboxData != null)
			{
				this.LocalMailbox = mailboxData;
			}
			this.CursorPosition = dataRow.CursorPosition;
			this.ProvisioningData = ProvisioningDataStorageBase.CreateFromDataRow(dataRow, isPAW);
			this.SubscriptionSettings = JobItemSubscriptionSettingsBase.CreateFromDataRow(dataRow);
			MigrationPreexistingDataRow migrationPreexistingDataRow = dataRow as MigrationPreexistingDataRow;
			if (migrationPreexistingDataRow != null)
			{
				this.SubscriptionId = migrationPreexistingDataRow.SubscriptionId;
			}
			if (!this.ShouldMigrate)
			{
				MigrationLogger.Log(MigrationEventType.Verbose, "setting subscription last check to max date time for recipient type: {0}", new object[]
				{
					this.RecipientType
				});
				this.SubscriptionLastChecked = new ExDateTime?(MigrationJobItem.MaxDateTimeValue);
			}
		}

		private void LogStatusEvent()
		{
			if (MigrationServiceFactory.Instance.ShouldLog)
			{
				MigrationJobItemLog.LogStatusEvent(this);
			}
		}

		private void UpdatePersistedMessage(IMigrationDataProvider dataProvider, PropertyDefinition[] propertiesToUpdate, Action<IMigrationMessageItem> updateAction)
		{
			this.UpdatePersistedMessage(dataProvider, propertiesToUpdate, SaveMode.NoConflictResolution, updateAction);
		}

		private void UpdatePersistedMessage(IMigrationDataProvider dataProvider, PropertyDefinition[] propertiesToUpdate, SaveMode saveMode, Action<IMigrationMessageItem> updateAction)
		{
			MigrationUtil.ThrowOnNullArgument(dataProvider, "dataProvider");
			MigrationUtil.ThrowOnNullArgument(updateAction, "updateAction");
			MigrationUtil.ThrowOnNullArgument(propertiesToUpdate, "propertiesToUpdate");
			HashSet<PropertyDefinition> hashSet = new HashSet<PropertyDefinition>(propertiesToUpdate);
			bool flag = hashSet.Contains(MigrationBatchMessageSchema.MigrationUserStatus);
			if (flag && !hashSet.Contains(MigrationBatchMessageSchema.MigrationJobItemSlotProviderId))
			{
				propertiesToUpdate = MigrationHelper.AggregateProperties(new IList<PropertyDefinition>[]
				{
					propertiesToUpdate,
					this.MigrationJobSlotPropertyDefinitions
				});
			}
			hashSet.Clear();
			MigrationUtil.AssertOrThrow(base.StoreObjectId != null, "Should only work on an item that's been persisted", new object[0]);
			using (IMigrationMessageItem migrationMessageItem = base.FindMessageItem(dataProvider, propertiesToUpdate))
			{
				migrationMessageItem.OpenAsReadWrite();
				updateAction(migrationMessageItem);
				if (flag)
				{
					MigrationStatusData<MigrationUserStatus> migrationStatusData = MigrationStatusData<MigrationUserStatus>.Create(migrationMessageItem, MigrationJobItem.StatusDataVersionMap[base.Version]);
					MigrationUserStatus migrationUserStatus = migrationStatusData.Status;
					if (migrationUserStatus != this.Status)
					{
						this.CheckAndReleaseSlotAssignmentIfNeeded(migrationUserStatus, migrationMessageItem);
					}
				}
				migrationMessageItem.Save(saveMode);
			}
		}

		private bool CheckAndReleaseSlotAssignmentIfNeeded(MigrationUserStatus newStatus, IMigrationMessageItem message)
		{
			if (newStatus == MigrationUserStatus.Completing || newStatus == MigrationUserStatus.IncrementalSyncing || newStatus == MigrationUserStatus.Syncing)
			{
				return false;
			}
			if (this.MigrationSlotProviderGuid != Guid.Empty || this.ConsumedSlotType != MigrationSlotType.None)
			{
				this.ConsumedSlotType = MigrationSlotType.None;
				this.MigrationSlotProviderGuid = Guid.Empty;
				this.SaveConsumedSlotInformationToMessage(message);
				return true;
			}
			return false;
		}

		public const long MigrationJobItemPAWVersion = 4L;

		private const long MigrationJobItemSupportPersistableBaseVersion = 3L;

		private const string BatchInputIdKey = "BatchInputId";

		private const string InitialSyncDurationKey = "InitialSyncDuration";

		private const string IncrementalSyncDurationKey = "IncrementalSyncDuration";

		private const string IncrementalSyncFailuresKey = "IncrementalSyncFailures";

		private const string PublicFolderCompletionFailuresKey = "PublicFolderCompletionFailures";

		private const string IncrementalSyncIntervalKey = "IncrementalSyncInterval";

		private const string TroubleshooterNotesKey = "TroubleshooterNotes";

		internal static readonly MigrationEqualityFilter MessageClassEqualityFilter = new MigrationEqualityFilter(StoreObjectSchema.ItemClass, MigrationBatchMessageSchema.MigrationJobItemClass);

		internal static readonly PropertyDefinition[] MigrationJobItemColumnsIndex = MigrationHelper.AggregateProperties(new IList<PropertyDefinition>[]
		{
			new PropertyDefinition[]
			{
				StoreObjectSchema.ItemClass,
				MigrationBatchMessageSchema.MigrationJobId,
				MigrationBatchMessageSchema.MigrationJobItemIdentifier,
				MigrationBatchMessageSchema.MigrationJobItemMailboxLegacyDN,
				MigrationBatchMessageSchema.MigrationJobItemSubscriptionLastChecked,
				MigrationBatchMessageSchema.MigrationType,
				MigrationBatchMessageSchema.MigrationJobItemItemsSynced,
				MigrationBatchMessageSchema.MigrationJobItemItemsSkipped,
				MigrationBatchMessageSchema.MigrationJobItemRecipientType,
				MigrationBatchMessageSchema.MigrationDisableTime,
				MigrationBatchMessageSchema.MigrationProvisionedTime,
				MigrationBatchMessageSchema.MigrationLastSuccessfulSyncTime,
				MigrationBatchMessageSchema.MigrationJobLastRestartTime,
				MigrationBatchMessageSchema.MigrationJobItemId,
				MigrationBatchMessageSchema.MigrationJobItemSubscriptionSettingsLastUpdatedTime,
				MigrationBatchMessageSchema.MigrationJobItemIncomingUsername,
				MigrationBatchMessageSchema.MigrationFlags,
				MigrationBatchMessageSchema.MigrationNextProcessTime,
				MigrationBatchMessageSchema.MigrationJobItemLocalMailboxIdentifier,
				MigrationBatchMessageSchema.MigrationJobItemSubscriptionQueuedTime,
				MigrationBatchMessageSchema.MigrationJobLastFinalizationAttempt
			},
			MigrationStatusData<MigrationUserStatus>.StatusPropertyDefinition,
			MigrationWorkflowPosition.MigrationWorkflowPositionProperties,
			MigrationPersistableBase.MigrationBaseDefinitions
		});

		internal static readonly MigrationUserStatus[] FailedStatuses = new MigrationUserStatus[]
		{
			MigrationUserStatus.Failed,
			MigrationUserStatus.IncrementalFailed,
			MigrationUserStatus.CompletionFailed
		};

		internal static readonly MigrationUserStatus[] StoppedStatuses = new MigrationUserStatus[]
		{
			MigrationUserStatus.Stopped,
			MigrationUserStatus.IncrementalStopped
		};

		internal static readonly MigrationUserStatus[] ErrorStatuses = new MigrationUserStatus[]
		{
			MigrationUserStatus.Failed,
			MigrationUserStatus.IncrementalFailed,
			MigrationUserStatus.CompletionFailed,
			MigrationUserStatus.Corrupted
		};

		internal static readonly MigrationUserStatus[] PreventPublicFolderCompletionErrorStatuses = new MigrationUserStatus[]
		{
			MigrationUserStatus.Failed,
			MigrationUserStatus.CompletionFailed,
			MigrationUserStatus.Corrupted
		};

		internal static readonly PropertyDefinition[] MigrationJobItemColumnsTypeIndex = new PropertyDefinition[]
		{
			StoreObjectSchema.ItemClass,
			MigrationBatchMessageSchema.MigrationVersion,
			MigrationBatchMessageSchema.MigrationType
		};

		internal static readonly PropertyDefinition[] MailboxDataUpdateIndex = MigrationHelper.AggregateProperties(new IList<PropertyDefinition>[]
		{
			new PropertyDefinition[]
			{
				StoreObjectSchema.ItemClass,
				MigrationBatchMessageSchema.MigrationProvisionedTime
			},
			MailboxData.MailboxDataPropertyDefinition,
			MigrationStatusData<MigrationUserStatus>.StatusPropertyDefinition
		});

		internal static readonly PropertyDefinition[] DisableMigrationProperties = MigrationHelper.AggregateProperties(new IList<PropertyDefinition>[]
		{
			new PropertyDefinition[]
			{
				MigrationBatchMessageSchema.MigrationJobItemSubscriptionLastChecked,
				MigrationBatchMessageSchema.MigrationDisableTime
			},
			MigrationStatusData<MigrationUserStatus>.StatusPropertyDefinition
		});

		internal static readonly ComparisonFilter MigrationJobItemMessageClassFilter = new ComparisonFilter(ComparisonOperator.Equal, StoreObjectSchema.ItemClass, MigrationBatchMessageSchema.MigrationJobItemClass);

		private static readonly SortBy[] StateSort = new SortBy[]
		{
			new SortBy(StoreObjectSchema.ItemClass, SortOrder.Ascending),
			new SortBy(MigrationBatchMessageSchema.MigrationJobId, SortOrder.Ascending),
			new SortBy(MigrationBatchMessageSchema.MigrationState, SortOrder.Ascending),
			new SortBy(MigrationBatchMessageSchema.MigrationNextProcessTime, SortOrder.Ascending)
		};

		private static readonly SortBy[] FlagSort = new SortBy[]
		{
			new SortBy(StoreObjectSchema.ItemClass, SortOrder.Ascending),
			new SortBy(MigrationBatchMessageSchema.MigrationJobId, SortOrder.Ascending),
			new SortBy(MigrationBatchMessageSchema.MigrationFlags, SortOrder.Ascending),
			new SortBy(MigrationBatchMessageSchema.MigrationState, SortOrder.Ascending)
		};

		private static readonly PropertyDefinition[] GroupProperties = MigrationHelper.AggregateProperties(new IList<PropertyDefinition>[]
		{
			LegacyExchangeMigrationGroupRecipient.GroupPropertyDefinitions,
			MigrationStatusData<MigrationUserStatus>.StatusPropertyDefinition
		});

		private static readonly ExDateTime MaxDateTimeValue = ExDateTime.MaxValue;

		private static readonly Dictionary<long, long> StatusDataVersionMap = new Dictionary<long, long>
		{
			{
				3L,
				1L
			},
			{
				4L,
				2L
			}
		};

		private static readonly PropertyDefinition[] MigrationJobItemSubscriptionStateLastUpdated = new PropertyDefinition[]
		{
			StoreObjectSchema.ItemClass,
			MigrationBatchMessageSchema.MigrationJobId,
			MigrationBatchMessageSchema.MigrationJobItemStateLastUpdated,
			MigrationBatchMessageSchema.MigrationJobItemSubscriptionLastChecked
		};

		private static readonly PropertyDefinition[] MigrationJobItemColumnsStatusIndex = MigrationHelper.AggregateProperties(new IList<PropertyDefinition>[]
		{
			new PropertyDefinition[]
			{
				MigrationBatchMessageSchema.MigrationJobItemSubscriptionLastChecked,
				MigrationBatchMessageSchema.MigrationJobItemItemsSynced,
				MigrationBatchMessageSchema.MigrationJobItemItemsSkipped,
				MigrationBatchMessageSchema.MigrationLastSuccessfulSyncTime
			},
			MigrationStatusData<MigrationUserStatus>.StatusPropertyDefinition,
			MigrationPersistableBase.MigrationBaseDefinitions
		});

		private static readonly PropertyDefinition[] updatePropertyDefinitionsBase = MigrationHelper.AggregateProperties(new IList<PropertyDefinition>[]
		{
			new PropertyDefinition[]
			{
				MigrationBatchMessageSchema.MigrationJobId,
				MigrationBatchMessageSchema.MigrationJobItemRecipientType
			},
			MigrationStatusData<MigrationUserStatus>.StatusPropertyDefinition
		});

		private static readonly ConcurrentDictionary<string, PropertyDefinition[]> PropertyDefinitionsHash = new ConcurrentDictionary<string, PropertyDefinition[]>();

		private static readonly MigrationEqualityFilter[] MigrationJobItemMessageClassFilterCollection = new MigrationEqualityFilter[]
		{
			MigrationJobItem.MessageClassEqualityFilter
		};

		private MigrationUserStatus status;

		private MigrationStatusData<MigrationUserStatus> statusData;

		private Guid migrationJobId;

		private IMailboxData localMailbox;

		private ExDateTime? subscriptionLastChecked;

		private long currentSupportedVersion;
	}
}
