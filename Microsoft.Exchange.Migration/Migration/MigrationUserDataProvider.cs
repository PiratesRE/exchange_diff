using System;
using System.Collections.Generic;
using System.Xml.Linq;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.Management;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.MailboxReplicationService;

namespace Microsoft.Exchange.Migration
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class MigrationUserDataProvider : XsoMailboxDataProviderBase
	{
		protected MigrationUserDataProvider(MigrationDataProvider dataProvider, string executingUserId, bool cleanSessionOnDispose = true) : base(dataProvider.MailboxSession)
		{
			this.dataProvider = dataProvider;
			this.diagnosticEnabled = false;
			this.IncludeReport = false;
			this.jobCache = new MigrationJobObjectCache(dataProvider);
			this.ExecutingUserId = executingUserId;
			this.cleanSessionOnDispose = cleanSessionOnDispose;
		}

		public int? LimitSkippedItemsTo { get; set; }

		public bool ForceRemoval { get; set; }

		public IMigrationDataProvider MailboxProvider
		{
			get
			{
				return this.dataProvider;
			}
		}

		public LocalizedException LastError { get; private set; }

		public bool IncludeReport { get; set; }

		public string ExecutingUserId { get; private set; }

		public static MigrationUserDataProvider CreateDataProvider(MigrationDataProvider dataProvider, string executingUserId = null)
		{
			MigrationUtil.ThrowOnNullArgument(dataProvider, "dataProvider");
			return new MigrationUserDataProvider(dataProvider, executingUserId, false);
		}

		public static MigrationUserDataProvider CreateDataProvider(string action, IRecipientSession recipientSession, ADUser partitionMailbox, string executingUserId = null)
		{
			MigrationUtil.ThrowOnNullOrEmptyArgument(action, "action");
			MigrationUtil.ThrowOnNullArgument(recipientSession, "recipientSession");
			MigrationUserDataProvider result;
			using (DisposeGuard disposeGuard = default(DisposeGuard))
			{
				MigrationDataProvider disposable = MigrationDataProvider.CreateProviderForMigrationMailbox(action, recipientSession, partitionMailbox);
				disposeGuard.Add<MigrationDataProvider>(disposable);
				MigrationUserDataProvider migrationUserDataProvider = new MigrationUserDataProvider(disposable, executingUserId, true);
				disposeGuard.Success();
				result = migrationUserDataProvider;
			}
			return result;
		}

		public MigrationJob GetJob(MigrationBatchId batch)
		{
			return this.jobCache.GetJob(batch);
		}

		public void EnableDiagnostics(string argument)
		{
			this.diagnosticEnabled = true;
			this.diagnosticArgument = new MigrationDiagnosticArgument(argument);
		}

		public IEnumerable<MigrationUser> GetByUserId(MigrationUserId userId, int pageSize)
		{
			return this.InternalFindPaged<MigrationUser>(null, userId, false, null, pageSize);
		}

		protected override IEnumerable<T> InternalFindPaged<T>(QueryFilter filter, ObjectId rootId, bool deepSearch, SortBy unused, int pageSize)
		{
			SortBy[] sort = null;
			QueryFilter[] internalFilters = null;
			this.DetermineInternalFiltering(filter, rootId, ref pageSize, out sort, out internalFilters);
			foreach (QueryFilter internalFilter in internalFilters)
			{
				foreach (object[] row in this.MailboxProvider.QueryRows(internalFilter, sort, MigrationUser.PropertyDefinitions, pageSize))
				{
					MigrationJobItemSummary summary = MigrationJobItemSummary.LoadFromRow(row);
					this.LastError = null;
					yield return (T)((object)this.ConvertStoreObjectToPresentationObject<T>(summary));
				}
			}
			yield break;
		}

		protected override void InternalSave(ConfigurableObject instance)
		{
			ObjectState objectState = instance.ObjectState;
			if (objectState != ObjectState.Deleted)
			{
				throw new NotSupportedException(string.Format("Save: MigrationUserDataProvider(objectState:{0})", instance.ObjectState));
			}
		}

		protected override void InternalDelete(ConfigurableObject instance)
		{
			MigrationUser user = (MigrationUser)instance;
			MigrationJob job = null;
			bool isPAW = false;
			MigrationHelper.RunUpdateOperation(delegate
			{
				job = this.GetJob(user.BatchId);
				if (job == null && !this.ForceRemoval)
				{
					throw new CannotRemoveUserWithoutBatchException(user.Identity.ToString());
				}
				MigrationJobItem byGuid = MigrationJobItem.GetByGuid(this.MailboxProvider, user.Identity.JobItemGuid);
				if (byGuid == null)
				{
					throw new MigrationUserAlreadyRemovedException(user.Identity.JobItemGuid.ToString());
				}
				isPAW = byGuid.IsPAW;
				if (isPAW && !this.ForceRemoval)
				{
					byGuid.SetMigrationFlags(this.MailboxProvider, MigrationFlags.Remove);
					return;
				}
				if (!this.ForceRemoval)
				{
					MigrationUtil.AssertOrThrow(!isPAW && job != null, "We should have thrown above if this is not the case!", new object[0]);
					MigrationUtil.AssertOrThrow(byGuid.MigrationJobId == job.JobId, "The job should be the owner of the job item.", new object[0]);
					using (ILegacySubscriptionHandler legacySubscriptionHandler = LegacySubscriptionHandlerBase.CreateSubscriptionHandler(this.MailboxProvider, job))
					{
						MigrationJobRemovingProcessor.RemoveJobItemSubscription(byGuid, legacySubscriptionHandler);
					}
				}
				byGuid.Delete(this.MailboxProvider);
			});
			if (!isPAW && job != null)
			{
				CommonUtils.CatchKnownExceptions(delegate
				{
					if (!this.ForceRemoval)
					{
						job.IncrementRemovedUserCount(this.MailboxProvider);
					}
					job.ReportData.Append(Strings.MigrationReportJobItemRemoved(this.ExecutingUserId, user.Identity.ToString()));
					this.MailboxProvider.FlushReport(job.ReportData);
				}, null);
			}
		}

		protected override void InternalDispose(bool disposing)
		{
			try
			{
				if (disposing)
				{
					if (this.cleanSessionOnDispose && this.dataProvider != null)
					{
						this.dataProvider.Dispose();
					}
					this.dataProvider = null;
				}
			}
			finally
			{
				base.InternalDispose(disposing);
			}
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<MigrationUserDataProvider>(this);
		}

		private T ConvertStoreObjectToPresentationObject<T>(MigrationJobItemSummary summary)
		{
			MigrationUser migrationUser;
			if (typeof(T).Equals(typeof(MigrationUserStatistics)))
			{
				migrationUser = new MigrationUserStatistics();
			}
			else
			{
				migrationUser = new MigrationUser();
			}
			if (summary.Identifier != null || summary.JobItemGuid != null)
			{
				migrationUser.Identity = new MigrationUserId(summary.Identifier, summary.JobItemGuid ?? Guid.Empty);
			}
			migrationUser.EmailAddress = SmtpAddress.Empty;
			SmtpAddress emailAddress;
			if (!string.IsNullOrEmpty(summary.LocalMailboxIdentifier))
			{
				migrationUser.EmailAddress = (SmtpAddress)summary.LocalMailboxIdentifier;
			}
			else if (MigrationServiceHelper.TryParseSmtpAddress(summary.Identifier, out emailAddress))
			{
				migrationUser.EmailAddress = emailAddress;
			}
			migrationUser.RecipientType = summary.RecipientType;
			migrationUser.SkippedItemCount = summary.ItemsSkipped;
			migrationUser.SyncedItemCount = summary.ItemsSynced;
			migrationUser.MailboxLegacyDN = summary.MailboxLegacyDN;
			ExTimeZone timeZone = null;
			MigrationJobSummary jobSummary = this.jobCache.GetJobSummary(summary.BatchGuid);
			if (jobSummary != null)
			{
				migrationUser.BatchId = jobSummary.BatchId;
				timeZone = jobSummary.UserTimeZone;
			}
			ExDateTime? universalDateTime = MigrationHelper.GetUniversalDateTime(summary.LastSubscriptionCheckTime);
			migrationUser.LastSubscriptionCheckTime = (DateTime?)MigrationHelper.GetLocalizedDateTime(universalDateTime, timeZone);
			ExDateTime? universalDateTime2 = MigrationHelper.GetUniversalDateTime(summary.LastSuccessfulSyncTime);
			migrationUser.LastSuccessfulSyncTime = (DateTime?)MigrationHelper.GetLocalizedDateTime(universalDateTime2, timeZone);
			if (summary.MailboxGuid != null)
			{
				migrationUser.MailboxGuid = summary.MailboxGuid.Value;
			}
			if (summary.MrsId != null)
			{
				migrationUser.RequestGuid = summary.MrsId.Value;
			}
			if (summary.Status != null)
			{
				migrationUser.Status = summary.Status.Value;
			}
			if (migrationUser is MigrationUserStatistics)
			{
				this.PopulateStatistics((MigrationUserStatistics)migrationUser);
			}
			return (T)((object)migrationUser);
		}

		private void PopulateStatistics(MigrationUserStatistics user)
		{
			MigrationUtil.ThrowOnNullArgument(user, "user");
			MigrationUtil.ThrowOnNullArgument(user.Identity, "Identity");
			MigrationUtil.ThrowOnNullArgument(this.MailboxProvider, "dataProvider");
			MigrationSession argument = MigrationSession.Get(this.MailboxProvider, false);
			MigrationUtil.ThrowOnNullArgument(argument, "session");
			MigrationJobItem migrationJobItem = null;
			if (user.Identity != null && user.Identity.JobItemGuid != Guid.Empty)
			{
				migrationJobItem = MigrationJobItem.GetByGuid(this.MailboxProvider, user.Identity.JobItemGuid);
			}
			if (migrationJobItem == null)
			{
				return;
			}
			if (migrationJobItem.StatusData != null)
			{
				user.Error = migrationJobItem.StatusData.FailureRecord;
			}
			user.MigrationType = migrationJobItem.MigrationType;
			if (this.diagnosticEnabled)
			{
				XElement diagnosticInfo = migrationJobItem.GetDiagnosticInfo(this.MailboxProvider, this.diagnosticArgument);
				if (diagnosticInfo != null)
				{
					user.DiagnosticInfo = diagnosticInfo.ToString();
				}
			}
			if (migrationJobItem.SubscriptionId != null)
			{
				SubscriptionSnapshot subscriptionSnapshot = null;
				try
				{
					SubscriptionAccessorBase subscriptionAccessorBase = SubscriptionAccessorBase.CreateAccessor(this.MailboxProvider, user.MigrationType, migrationJobItem.IsPAW);
					subscriptionAccessorBase.IncludeReport = true;
					subscriptionSnapshot = subscriptionAccessorBase.RetrieveSubscriptionSnapshot(migrationJobItem.SubscriptionId);
				}
				catch (LocalizedException lastError)
				{
					this.LastError = lastError;
				}
				if (subscriptionSnapshot != null)
				{
					user.TotalQueuedDuration = subscriptionSnapshot.TotalQueuedDuration;
					user.TotalInProgressDuration = subscriptionSnapshot.TotalInProgressDuration;
					user.TotalSyncedDuration = subscriptionSnapshot.TotalSyncedDuration;
					user.TotalStalledDuration = subscriptionSnapshot.TotalStalledDuration;
					user.EstimatedTotalTransferSize = subscriptionSnapshot.EstimatedTotalTransferSize;
					user.EstimatedTotalTransferCount = subscriptionSnapshot.EstimatedTotalTransferCount;
					user.BytesTransferred = subscriptionSnapshot.BytesTransferred;
					user.AverageBytesTransferredPerHour = subscriptionSnapshot.AverageBytesTransferredPerHour;
					user.CurrentBytesTransferredPerMinute = subscriptionSnapshot.CurrentBytesTransferredPerMinute;
					user.SyncedItemCount = subscriptionSnapshot.NumItemsSynced;
					user.TotalItemsInSourceMailboxCount = subscriptionSnapshot.NumTotalItemsInMailbox;
					user.SkippedItemCount = subscriptionSnapshot.NumItemsSkipped;
					user.PercentageComplete = subscriptionSnapshot.PercentageComplete;
					if (subscriptionSnapshot.Report != null)
					{
						user.SkippedItems = new MultiValuedProperty<MigrationUserSkippedItem>();
						this.LoadSkippedItems(user.SkippedItems, subscriptionSnapshot.Report.BadItems);
						this.LoadSkippedItems(user.SkippedItems, subscriptionSnapshot.Report.LargeItems);
						if (this.IncludeReport)
						{
							user.Report = subscriptionSnapshot.Report;
						}
					}
				}
			}
		}

		private void LoadSkippedItems(MultiValuedProperty<MigrationUserSkippedItem> skippedItems, List<BadMessageRec> badMessages)
		{
			int num = skippedItems.Count;
			if ((this.LimitSkippedItemsTo != null && this.LimitSkippedItemsTo.Value - num <= 0) || badMessages == null)
			{
				return;
			}
			foreach (BadMessageRec rec in badMessages)
			{
				skippedItems.Add(this.BadRecToSkippedItem(rec));
				if (this.LimitSkippedItemsTo != null && ++num >= this.LimitSkippedItemsTo.Value)
				{
					break;
				}
			}
		}

		private MigrationUserSkippedItem BadRecToSkippedItem(BadMessageRec rec)
		{
			MigrationUtil.ThrowOnNullArgument(rec, "rec");
			return new MigrationUserSkippedItem
			{
				Kind = rec.Kind.ToString(),
				FolderName = rec.FolderName,
				Sender = rec.Sender,
				Recipient = rec.Recipient,
				Subject = rec.Subject,
				MessageClass = rec.MessageClass,
				MessageSize = rec.MessageSize,
				DateSent = rec.DateSent,
				DateReceived = rec.DateReceived,
				Failure = ((rec.Failure != null) ? rec.Failure.ToString() : null)
			};
		}

		private void ParsePresentationFilter(QueryFilter presentationFilter, out Guid? mailboxGuid, out Guid? batchGuid, out MigrationUserStatus[] statuses)
		{
			statuses = null;
			mailboxGuid = null;
			batchGuid = null;
			List<QueryFilter> list = new List<QueryFilter>();
			if (presentationFilter != null)
			{
				list.Add(presentationFilter);
			}
			while (list.Count > 0)
			{
				QueryFilter queryFilter = list[0];
				list.RemoveAt(0);
				if (queryFilter is CompositeFilter)
				{
					list.AddRange(((CompositeFilter)queryFilter).Filters);
				}
				else if (queryFilter is ComparisonFilter)
				{
					ComparisonFilter comparisonFilter = (ComparisonFilter)queryFilter;
					PropertyDefinition property = comparisonFilter.Property;
					object propertyValue = comparisonFilter.PropertyValue;
					if (property == MigrationUserSchema.BatchId)
					{
						MigrationBatchId batchId = (MigrationBatchId)propertyValue;
						batchGuid = new Guid?(this.jobCache.GetBatchGuidById(batchId));
					}
					else if (property == MigrationUserSchema.Status)
					{
						statuses = new MigrationUserStatus[]
						{
							(MigrationUserStatus)propertyValue
						};
					}
					else if (property == MigrationUserSchema.StatusSummary)
					{
						statuses = MigrationUser.MapFromSummaryToStatus[(MigrationUserStatusSummary)propertyValue];
					}
					else if (property == MigrationUserSchema.MailboxGuid)
					{
						mailboxGuid = new Guid?((Guid)propertyValue);
					}
				}
			}
		}

		private void DetermineInternalFiltering(QueryFilter filter, ObjectId rootId, ref int pageSize, out SortBy[] sort, out QueryFilter[] internalFilters)
		{
			if (filter != null)
			{
				Guid? guid = null;
				Guid? guid2 = null;
				MigrationUserStatus[] array = null;
				this.ParsePresentationFilter(filter, out guid, out guid2, out array);
				if (guid != null)
				{
					internalFilters = new QueryFilter[]
					{
						QueryFilter.AndTogether(new QueryFilter[]
						{
							MigrationJobItem.MessageClassEqualityFilter.Filter,
							new ComparisonFilter(ComparisonOperator.Equal, MigrationUser.MailboxGuidPropertyDefinition, guid.Value)
						})
					};
					sort = new SortBy[]
					{
						new SortBy(StoreObjectSchema.ItemClass, SortOrder.Ascending),
						new SortBy(MigrationUser.MailboxGuidPropertyDefinition, SortOrder.Ascending)
					};
					pageSize = 2;
					return;
				}
				if (guid2 != null)
				{
					sort = new SortBy[]
					{
						new SortBy(StoreObjectSchema.ItemClass, SortOrder.Ascending),
						new SortBy(MigrationUser.BatchIdPropertyDefinition, SortOrder.Ascending),
						new SortBy(MigrationUser.StatusPropertyDefinition, SortOrder.Ascending)
					};
				}
				else
				{
					sort = new SortBy[]
					{
						new SortBy(StoreObjectSchema.ItemClass, SortOrder.Ascending),
						new SortBy(MigrationUser.StatusPropertyDefinition, SortOrder.Ascending)
					};
				}
				if (array != null)
				{
					internalFilters = new QueryFilter[array.Length];
					for (int i = 0; i < array.Length; i++)
					{
						MigrationUserStatus migrationUserStatus = array[i];
						if (guid2 != null)
						{
							internalFilters[i] = QueryFilter.AndTogether(new QueryFilter[]
							{
								MigrationJobItem.MessageClassEqualityFilter.Filter,
								new ComparisonFilter(ComparisonOperator.Equal, MigrationUser.BatchIdPropertyDefinition, guid2.Value),
								new ComparisonFilter(ComparisonOperator.Equal, MigrationUser.StatusPropertyDefinition, (int)migrationUserStatus)
							});
						}
						else
						{
							internalFilters[i] = QueryFilter.AndTogether(new QueryFilter[]
							{
								MigrationJobItem.MessageClassEqualityFilter.Filter,
								new ComparisonFilter(ComparisonOperator.Equal, MigrationUser.StatusPropertyDefinition, (int)migrationUserStatus)
							});
						}
					}
					return;
				}
				if (guid2 != null)
				{
					internalFilters = new QueryFilter[]
					{
						QueryFilter.AndTogether(new QueryFilter[]
						{
							MigrationJobItem.MessageClassEqualityFilter.Filter,
							new ComparisonFilter(ComparisonOperator.Equal, MigrationUser.BatchIdPropertyDefinition, guid2.Value)
						})
					};
					return;
				}
				sort = null;
			}
			MigrationUserId migrationUserId = rootId as MigrationUserId;
			if (migrationUserId != null)
			{
				if (migrationUserId.JobItemGuid != Guid.Empty)
				{
					internalFilters = new QueryFilter[]
					{
						QueryFilter.AndTogether(new QueryFilter[]
						{
							MigrationJobItem.MessageClassEqualityFilter.Filter,
							new ComparisonFilter(ComparisonOperator.Equal, MigrationUser.IdPropertyDefinition, migrationUserId.JobItemGuid)
						})
					};
					sort = MigrationUserDataProvider.JobItemIdSort;
				}
				else
				{
					internalFilters = new QueryFilter[]
					{
						QueryFilter.AndTogether(new QueryFilter[]
						{
							MigrationJobItem.MessageClassEqualityFilter.Filter,
							new ComparisonFilter(ComparisonOperator.Equal, MigrationUser.IdentifierPropertyDefinition, migrationUserId.Id)
						})
					};
					sort = MigrationUserDataProvider.DefaultSort;
				}
				pageSize = 2;
				return;
			}
			internalFilters = new QueryFilter[]
			{
				MigrationJobItem.MessageClassEqualityFilter.Filter
			};
			sort = MigrationUserDataProvider.DefaultSort;
		}

		public static readonly SortBy[] DefaultSort = new SortBy[]
		{
			new SortBy(StoreObjectSchema.ItemClass, SortOrder.Ascending),
			new SortBy(MigrationUser.IdentifierPropertyDefinition, SortOrder.Ascending)
		};

		public static readonly SortBy[] JobItemIdSort = new SortBy[]
		{
			new SortBy(StoreObjectSchema.ItemClass, SortOrder.Ascending),
			new SortBy(MigrationUser.IdPropertyDefinition, SortOrder.Ascending)
		};

		private readonly MigrationJobObjectCache jobCache;

		private MigrationDataProvider dataProvider;

		private bool diagnosticEnabled;

		private MigrationDiagnosticArgument diagnosticArgument;

		private readonly bool cleanSessionOnDispose;
	}
}
