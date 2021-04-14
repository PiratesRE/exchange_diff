using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using Microsoft.Exchange.Collections;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.TenantMonitoring;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Management.TenantMonitoring
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal sealed class NotificationDataProvider : IConfigDataProvider
	{
		public NotificationDataProvider(ADUser adUser, ADSessionSettings sessionSettings)
		{
			if (adUser == null)
			{
				throw new ArgumentNullException("adUser");
			}
			if (sessionSettings == null)
			{
				throw new ArgumentNullException("sessionSettings");
			}
			this.adUser = adUser;
			this.sessionSettings = sessionSettings;
			this.source = adUser.ToString();
		}

		public string Source
		{
			get
			{
				return this.source ?? string.Empty;
			}
		}

		public IConfigurable Read<T>(ObjectId identity) where T : IConfigurable, new()
		{
			if (identity == null)
			{
				throw new ArgumentNullException("identity");
			}
			NotificationDataProvider.Tracer.TraceDebug<ObjectId>((long)this.GetHashCode(), "Reading notification with identity {0}", identity);
			IConfigurable result;
			using (NotificationDataProvider.NotificationDataStore notificationDataStore = this.CreateDataStore())
			{
				result = notificationDataStore.Read((NotificationIdentity)identity);
			}
			return result;
		}

		public IConfigurable[] Find<T>(QueryFilter filter, ObjectId rootId, bool deepSearch, SortBy sortBy) where T : IConfigurable, new()
		{
			throw new NotSupportedException();
		}

		public IEnumerable<T> FindPaged<T>(QueryFilter filter, ObjectId rootId, bool deepSearch, SortBy sortBy, int pageSize) where T : IConfigurable, new()
		{
			NotificationDataProvider.Tracer.TraceDebug<QueryFilter>((long)this.GetHashCode(), "Finding notifications matching filter {0}", filter);
			IEnumerable<T> result;
			using (NotificationDataProvider.NotificationDataStore notificationDataStore = this.CreateDataStore())
			{
				result = (IEnumerable<T>)notificationDataStore.Find(filter, null);
			}
			return result;
		}

		public void Save(IConfigurable instance)
		{
			Notification notification = NotificationDataProvider.ValidateNotNullAndCastArgument(instance, "instance");
			NotificationDataProvider.Tracer.TraceDebug<Notification>((long)this.GetHashCode(), "Saving notification {0}", notification);
			if (!notification.IsValid)
			{
				throw new ArgumentException("instance is invalid.", "instance");
			}
			switch (notification.ObjectState)
			{
			case ObjectState.New:
				this.NewNotification(notification);
				return;
			case ObjectState.Unchanged:
				return;
			case ObjectState.Changed:
			case ObjectState.Deleted:
				throw new NotSupportedException();
			default:
				return;
			}
		}

		public void Delete(IConfigurable instance)
		{
			throw new NotSupportedException();
		}

		internal RecentNotificationEmailTestResult RecentNotificationEmailExists(Notification notification)
		{
			if (notification == null)
			{
				throw new ArgumentNullException("notification");
			}
			RecentNotificationEmailTestResult result;
			using (NotificationDataProvider.NotificationDataStore notificationDataStore = this.CreateDataStore())
			{
				uint num = notificationDataStore.CountNotificationsSentPast24Hours(notification.CreationTimeUtc);
				if (num >= 50U)
				{
					result = RecentNotificationEmailTestResult.DailyCapReached;
				}
				else
				{
					QueryFilter criterion = QueryFilter.AndTogether(new QueryFilter[]
					{
						new ComparisonFilter(ComparisonOperator.Equal, TenantNotificationMessageSchema.MonitoringNotificationEmailSent, true),
						new ComparisonFilter(ComparisonOperator.GreaterThan, TenantNotificationMessageSchema.MonitoringCreationTimeUtc, notification.CreationTimeUtc.Subtract(TimeSpan.FromHours(24.0))),
						new ComparisonFilter(ComparisonOperator.Equal, TenantNotificationMessageSchema.MonitoringHashCodeForDuplicateDetection, notification.ComputeHashCodeForDuplicateDetection())
					});
					result = (notificationDataStore.Exists(criterion, notification.EventSource) ? RecentNotificationEmailTestResult.PastDay : RecentNotificationEmailTestResult.None);
				}
			}
			return result;
		}

		private static Notification ValidateNotNullAndCastArgument(IConfigurable argument, string argumentName)
		{
			if (argument == null)
			{
				throw new ArgumentNullException(argumentName);
			}
			Notification result = argument as Notification;
			if (argument == null)
			{
				throw new ArgumentException(ServerStrings.ObjectMustBeOfType(typeof(Notification).ToString()));
			}
			return result;
		}

		private void NewNotification(Notification notification)
		{
			using (NotificationDataProvider.NotificationDataStore notificationDataStore = this.CreateDataStore())
			{
				notificationDataStore.Save(notification);
			}
		}

		private NotificationDataProvider.NotificationDataStore CreateDataStore()
		{
			return new NotificationDataProvider.NotificationDataStore(this.adUser, this.sessionSettings);
		}

		private const uint MaxNotificationsPerDay = 50U;

		private static readonly Microsoft.Exchange.Diagnostics.Trace Tracer = ExTraceGlobals.DataProviderTracer;

		private readonly ADUser adUser;

		private readonly ADSessionSettings sessionSettings;

		private readonly string source;

		private sealed class NotificationDataStore : IDisposable
		{
			internal NotificationDataStore(ADUser user, ADSessionSettings sessionSettings)
			{
				this.mailboxSession = NotificationDataProvider.NotificationDataStore.OpenSession(user, sessionSettings);
			}

			~NotificationDataStore()
			{
				this.Dispose(false);
			}

			public void Dispose()
			{
				this.Dispose(true);
				GC.SuppressFinalize(this);
			}

			internal void Save(Notification notification)
			{
				this.CheckDisposed();
				using (Folder folder = Folder.Bind(this.mailboxSession, this.GetNotificationsFolderId(notification.EventSource, true)))
				{
					using (MessageItem messageItem = MessageItem.Create(this.mailboxSession, folder.Id))
					{
						messageItem[TenantNotificationMessageSchema.MonitoringUniqueId] = notification.Identity.GetBytes();
						messageItem[TenantNotificationMessageSchema.MonitoringEventInstanceId] = notification.EventInstanceId;
						messageItem[TenantNotificationMessageSchema.MonitoringEventSource] = notification.EventSource;
						messageItem[TenantNotificationMessageSchema.MonitoringEventCategoryId] = notification.EventCategoryId;
						messageItem[TenantNotificationMessageSchema.MonitoringEventTimeUtc] = notification.EventTimeUtc;
						messageItem[TenantNotificationMessageSchema.MonitoringEventEntryType] = notification.EntryType;
						messageItem[TenantNotificationMessageSchema.MonitoringInsertionStrings] = notification.InsertionStrings.ToArray<string>();
						messageItem[TenantNotificationMessageSchema.MonitoringNotificationEmailSent] = notification.EmailSent;
						messageItem[TenantNotificationMessageSchema.MonitoringCreationTimeUtc] = notification.CreationTimeUtc;
						messageItem[TenantNotificationMessageSchema.MonitoringNotificationRecipients] = notification.NotificationRecipients.ToArray();
						messageItem[TenantNotificationMessageSchema.MonitoringHashCodeForDuplicateDetection] = notification.ComputeHashCodeForDuplicateDetection();
						messageItem[TenantNotificationMessageSchema.MonitoringNotificationMessageIds] = notification.NotificationMessageIds.ToArray();
						messageItem[TenantNotificationMessageSchema.MonitoringEventPeriodicKey] = notification.PeriodicKey;
						messageItem[ItemSchema.Subject] = string.Format(CultureInfo.InvariantCulture, "creation-time:{0}; event-source:{1}; event-category-id:{2}; event-id:{3}; email-sent:{4}; periodic-key:{5};", new object[]
						{
							notification.CreationTimeUtc,
							notification.EventSource,
							notification.EventCategoryId,
							notification.EventInstanceId,
							notification.EmailSent,
							notification.PeriodicKey
						});
						PolicyTagHelper.SetRetentionProperties(messageItem, ExDateTime.UtcNow.AddDays(14.0), 14);
						messageItem.Save(SaveMode.NoConflictResolution);
					}
				}
				if (notification.EmailSent)
				{
					using (Folder folder2 = Folder.Bind(this.mailboxSession, this.GetTenantNotificationsFolderId(), NotificationDataProvider.NotificationDataStore.FolderColumns))
					{
						long valueOrDefault = folder2.GetValueOrDefault<long>(TenantNotificationMessageSchema.MonitoringCountOfNotificationsSentInPast24Hours, 0L);
						folder2[TenantNotificationMessageSchema.MonitoringCountOfNotificationsSentInPast24Hours] = NotificationDataProvider.NotificationDataStore.NotificationCountPropertyHelper.IncrementCount(valueOrDefault, notification.CreationTimeUtc);
						folder2.Save(SaveMode.NoConflictResolution);
					}
				}
			}

			internal Notification Read(NotificationIdentity identity)
			{
				this.CheckDisposed();
				if (identity == null)
				{
					throw new ArgumentNullException("identity");
				}
				byte[] bytes = identity.GetBytes();
				using (Folder folder = Folder.Bind(this.mailboxSession, this.GetNotificationsFolderId(identity.EventSource, true)))
				{
					using (QueryResult queryResult = folder.ItemQuery(ItemQueryType.None, null, NotificationDataProvider.NotificationDataStore.SortByCreationTime, NotificationDataProvider.NotificationDataStore.NotificationColumns))
					{
						queryResult.SeekToCondition(SeekReference.OriginBeginning, new ComparisonFilter(ComparisonOperator.Equal, TenantNotificationMessageSchema.MonitoringUniqueId, bytes));
						foreach (object[] array in queryResult.GetRows(10))
						{
							if (array != null && array.Length >= NotificationDataProvider.NotificationDataStore.NotificationColumns.Length && !(array[0] is PropertyError) && ArrayComparer<byte>.Comparer.Equals(bytes, (byte[])array[0]))
							{
								return NotificationDataProvider.NotificationDataStore.Deserialize(array[0], array[1], array[2], array[6], array[3], array[4], array[5], array[7], array[8], array[9], array[10], array[11]);
							}
						}
					}
				}
				return null;
			}

			internal IEnumerable<Notification> Find(QueryFilter filter, string eventSource)
			{
				this.CheckDisposed();
				StoreObjectId notificationsFolderId;
				bool includeSubfolders;
				if (string.IsNullOrEmpty(eventSource))
				{
					notificationsFolderId = this.GetTenantNotificationsFolderId();
					includeSubfolders = true;
				}
				else
				{
					notificationsFolderId = this.GetNotificationsFolderId(eventSource, false);
					includeSubfolders = false;
				}
				if (notificationsFolderId == null)
				{
					return NotificationDataProvider.NotificationDataStore.EmptyNotificationArray;
				}
				return this.Find(notificationsFolderId, includeSubfolders, filter);
			}

			internal uint CountNotificationsSentPast24Hours(ExDateTime now)
			{
				this.CheckDisposed();
				uint count;
				using (Folder folder = Folder.Bind(this.mailboxSession, this.GetTenantNotificationsFolderId(), NotificationDataProvider.NotificationDataStore.FolderColumns))
				{
					long valueOrDefault = folder.GetValueOrDefault<long>(TenantNotificationMessageSchema.MonitoringCountOfNotificationsSentInPast24Hours, 0L);
					count = NotificationDataProvider.NotificationDataStore.NotificationCountPropertyHelper.GetCount(valueOrDefault, now);
				}
				return count;
			}

			internal bool Exists(QueryFilter criterion, string eventSource)
			{
				this.CheckDisposed();
				if (string.IsNullOrEmpty(eventSource))
				{
					throw new ArgumentNullException("eventSource");
				}
				StoreObjectId notificationsFolderId = this.GetNotificationsFolderId(eventSource, false);
				if (notificationsFolderId == null)
				{
					return false;
				}
				bool result;
				using (Folder folder = Folder.Bind(this.mailboxSession, notificationsFolderId))
				{
					using (QueryResult queryResult = folder.ItemQuery(ItemQueryType.None, null, null, NotificationDataProvider.NotificationDataStore.NotificationColumns))
					{
						if (criterion != null)
						{
							queryResult.SeekToCondition(SeekReference.OriginBeginning, criterion, SeekToConditionFlags.AllowExtendedFilters);
						}
						result = (queryResult.GetRows(10).Length > 0);
					}
				}
				return result;
			}

			private static MailboxSession OpenSession(ADUser user, ADSessionSettings sessionSettings)
			{
				return MailboxSession.OpenAsAdmin(ExchangePrincipal.FromADUser(sessionSettings, user, RemotingOptions.AllowCrossSite), CultureInfo.InvariantCulture, "Client=Management;Action=Manage-TenantMonitoringNotifications");
			}

			private static Notification Deserialize(object uniqueIdOrPropertyError, object eventSourceOrPropertyError, object eventIdOrPropertyError, object eventEntryTypeOrPropertyError, object insertionStringsOrPropertyError, object creationTimeOrPropertyError, object emailSentOrPropertyError, object notificationRecipientsOrPropertyError, object eventCategoryIdOrPropertyError, object eventTimeOrPropertyError, object notificationMessageIdsOrPropertyError, object periodicKeyOrPropertyError)
			{
				object[] array = new object[]
				{
					uniqueIdOrPropertyError,
					eventSourceOrPropertyError,
					eventIdOrPropertyError,
					eventEntryTypeOrPropertyError,
					creationTimeOrPropertyError
				};
				foreach (object obj in array)
				{
					if (obj is PropertyError)
					{
						return null;
					}
				}
				return new Notification(new NotificationIdentity((byte[])uniqueIdOrPropertyError), (string)eventSourceOrPropertyError, (int)eventIdOrPropertyError, (eventCategoryIdOrPropertyError is PropertyError) ? 0 : ((int)eventCategoryIdOrPropertyError), (eventTimeOrPropertyError is PropertyError) ? ((ExDateTime)creationTimeOrPropertyError) : ((ExDateTime)eventTimeOrPropertyError), (EventLogEntryType)eventEntryTypeOrPropertyError, (insertionStringsOrPropertyError is PropertyError) ? null : ((string[])insertionStringsOrPropertyError), !(emailSentOrPropertyError is PropertyError) && (bool)emailSentOrPropertyError, (ExDateTime)creationTimeOrPropertyError, (notificationRecipientsOrPropertyError is PropertyError) ? null : ((string[])notificationRecipientsOrPropertyError), (notificationMessageIdsOrPropertyError is PropertyError) ? null : ((string[])notificationMessageIdsOrPropertyError), (periodicKeyOrPropertyError is PropertyError) ? string.Empty : ((string)periodicKeyOrPropertyError), ObjectState.Unchanged);
			}

			private IEnumerable<Notification> Find(StoreObjectId searchRootId, bool includeSubfolders, QueryFilter filter)
			{
				if (searchRootId == null)
				{
					throw new ArgumentNullException("searchRootId");
				}
				LinkedList<Notification> linkedList = new LinkedList<Notification>();
				Stack<StoreObjectId> stack = new Stack<StoreObjectId>(100);
				stack.Push(searchRootId);
				while (stack.Count > 0)
				{
					using (Folder folder = Folder.Bind(this.mailboxSession, stack.Pop()))
					{
						if (includeSubfolders)
						{
							using (QueryResult queryResult = folder.FolderQuery(FolderQueryFlags.None, null, null, NotificationDataProvider.NotificationDataStore.FolderColumns))
							{
								object[][] rows = queryResult.GetRows(100);
								while (rows.Length > 0)
								{
									foreach (object[] array2 in rows)
									{
										if (array2 != null && array2.Length >= NotificationDataProvider.NotificationDataStore.FolderColumns.Length && !(array2[0] is PropertyError))
										{
											stack.Push(((VersionedId)array2[0]).ObjectId);
										}
									}
									rows = queryResult.GetRows(100);
								}
							}
						}
						using (QueryResult queryResult2 = folder.ItemQuery(ItemQueryType.None, filter, NotificationDataProvider.NotificationDataStore.SortByCreationTime, NotificationDataProvider.NotificationDataStore.NotificationColumns))
						{
							object[][] rows2 = queryResult2.GetRows(100);
							while (rows2.Length > 0)
							{
								foreach (object[] array4 in rows2)
								{
									if (array4 != null && array4.Length >= NotificationDataProvider.NotificationDataStore.NotificationColumns.Length)
									{
										Notification notification = NotificationDataProvider.NotificationDataStore.Deserialize(array4[0], array4[1], array4[2], array4[6], array4[3], array4[4], array4[5], array4[7], array4[8], array4[9], array4[10], array4[11]);
										if (notification != null)
										{
											linkedList.AddLast(notification);
										}
									}
								}
								rows2 = queryResult2.GetRows(100);
							}
						}
					}
				}
				return linkedList;
			}

			private StoreObjectId GetNotificationsFolderId(string eventSource, bool create)
			{
				if (string.IsNullOrEmpty(eventSource))
				{
					throw new ArgumentNullException("eventSource");
				}
				return this.GetOrCreateFolder(this.GetTenantNotificationsFolderId(), eventSource, create);
			}

			private StoreObjectId GetTenantNotificationsFolderId()
			{
				if (this.tenantNotificationsFolderId == null)
				{
					this.tenantNotificationsFolderId = this.GetOrCreateFolder(this.mailboxSession.GetDefaultFolderId(DefaultFolderType.Root), "TenantNotifications", true);
				}
				return this.tenantNotificationsFolderId;
			}

			private StoreObjectId GetOrCreateFolder(StoreObjectId parentFolderId, string folderName, bool create)
			{
				if (parentFolderId == null)
				{
					throw new ArgumentNullException("parentFolderId");
				}
				if (string.IsNullOrEmpty(folderName))
				{
					throw new ArgumentNullException("folderName");
				}
				using (Folder folder = Folder.Bind(this.mailboxSession, parentFolderId))
				{
					using (QueryResult queryResult = folder.FolderQuery(FolderQueryFlags.None, new ComparisonFilter(ComparisonOperator.Equal, FolderSchema.DisplayName, folderName), null, NotificationDataProvider.NotificationDataStore.FolderColumns))
					{
						foreach (object[] array in queryResult.GetRows(10))
						{
							if (array != null && array.Length >= NotificationDataProvider.NotificationDataStore.FolderColumns.Length && !(array[0] is PropertyError) && !(array[1] is PropertyError) && folderName.Equals((string)array[1], StringComparison.OrdinalIgnoreCase))
							{
								return ((VersionedId)array[0]).ObjectId;
							}
						}
					}
				}
				if (!create)
				{
					return null;
				}
				StoreObjectId storeObjectId;
				using (Folder folder2 = Folder.Create(this.mailboxSession, parentFolderId, StoreObjectType.Folder))
				{
					folder2.DisplayName = folderName;
					folder2.Save();
					folder2.Load();
					storeObjectId = folder2.StoreObjectId;
				}
				return storeObjectId;
			}

			private void Dispose(bool disposing)
			{
				if (!this.disposed)
				{
					if (disposing && this.mailboxSession != null)
					{
						this.mailboxSession.Dispose();
					}
					this.disposed = true;
				}
			}

			private void CheckDisposed()
			{
				if (this.disposed)
				{
					throw new ObjectDisposedException("NotificationDataStore");
				}
			}

			private const string TenantNotificationsFolderDisplayName = "TenantNotifications";

			private const int NotificationTimeToLiveInDays = 14;

			private static readonly PropertyDefinition[] NotificationColumns = new PropertyDefinition[]
			{
				TenantNotificationMessageSchema.MonitoringUniqueId,
				TenantNotificationMessageSchema.MonitoringEventSource,
				TenantNotificationMessageSchema.MonitoringEventInstanceId,
				TenantNotificationMessageSchema.MonitoringInsertionStrings,
				TenantNotificationMessageSchema.MonitoringCreationTimeUtc,
				TenantNotificationMessageSchema.MonitoringNotificationEmailSent,
				TenantNotificationMessageSchema.MonitoringEventEntryType,
				TenantNotificationMessageSchema.MonitoringNotificationRecipients,
				TenantNotificationMessageSchema.MonitoringEventCategoryId,
				TenantNotificationMessageSchema.MonitoringEventTimeUtc,
				TenantNotificationMessageSchema.MonitoringNotificationMessageIds,
				TenantNotificationMessageSchema.MonitoringEventPeriodicKey
			};

			private static readonly PropertyDefinition[] FolderColumns = new PropertyDefinition[]
			{
				MessageItemSchema.FolderId,
				FolderSchema.DisplayName,
				TenantNotificationMessageSchema.MonitoringCountOfNotificationsSentInPast24Hours
			};

			private static readonly SortBy[] SortByCreationTime = new SortBy[]
			{
				new SortBy(TenantNotificationMessageSchema.MonitoringCreationTimeUtc, SortOrder.Descending)
			};

			private static readonly Notification[] EmptyNotificationArray = new Notification[0];

			private readonly MailboxSession mailboxSession;

			private bool disposed;

			private StoreObjectId tenantNotificationsFolderId;

			private static class ColumnIndices
			{
				internal const int UniqueId = 0;

				internal const int EventSource = 1;

				internal const int EventInstanceId = 2;

				internal const int InsertionStrings = 3;

				internal const int CreationTimeUtc = 4;

				internal const int NotificationEmailSent = 5;

				internal const int EntryType = 6;

				internal const int NotificationRecipients = 7;

				internal const int EventCategoryId = 8;

				internal const int EventTimeUtc = 9;

				internal const int NotificationMessageIds = 10;

				internal const int PeriodicKey = 11;
			}

			private static class FolderColumnIndices
			{
				internal const int FolderId = 0;

				internal const int DisplayName = 1;

				internal const int CountOfNotificationsInPast24Hours = 2;
			}

			private static class NotificationCountPropertyHelper
			{
				internal static uint GetCount(long propertyValue, ExDateTime now)
				{
					long num = now.UtcTicks / 600000000L;
					long num2 = (long)((ulong)NotificationDataProvider.NotificationDataStore.NotificationCountPropertyHelper.ExtractTimeStamp(propertyValue));
					if (num - num2 >= 1440L)
					{
						return 0U;
					}
					return NotificationDataProvider.NotificationDataStore.NotificationCountPropertyHelper.ExtractCounter(propertyValue);
				}

				internal static long IncrementCount(long propertyValue, ExDateTime now)
				{
					long num = now.UtcTicks / 600000000L;
					long num2 = (long)((ulong)NotificationDataProvider.NotificationDataStore.NotificationCountPropertyHelper.ExtractTimeStamp(propertyValue));
					if (num - num2 > 1440L)
					{
						return NotificationDataProvider.NotificationDataStore.NotificationCountPropertyHelper.ToBinaryForm(num, 1L);
					}
					uint num3 = NotificationDataProvider.NotificationDataStore.NotificationCountPropertyHelper.ExtractCounter(propertyValue);
					return NotificationDataProvider.NotificationDataStore.NotificationCountPropertyHelper.ToBinaryForm(num2, (long)((ulong)(num3 + 1U)));
				}

				private static uint ExtractTimeStamp(long propertyValue)
				{
					return (uint)((ulong)(propertyValue & -4294967296L) >> 32);
				}

				private static uint ExtractCounter(long propertyValue)
				{
					return (uint)(propertyValue & (long)((ulong)-1));
				}

				private static long ToBinaryForm(long timestamp, long counter)
				{
					return timestamp << 32 | counter;
				}

				private const int MinutesInADay = 1440;
			}
		}
	}
}
