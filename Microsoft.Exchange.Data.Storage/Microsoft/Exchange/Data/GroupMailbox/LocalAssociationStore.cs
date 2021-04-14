using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.Principal;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.GroupMailbox;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Data.GroupMailbox
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class LocalAssociationStore : IAssociationStore, IDisposeTrackable, IDisposable
	{
		public LocalAssociationStore(IMailboxLocator mailboxLocator, IMailboxSession session, bool shouldDisposeSession, IXSOFactory xsoFactory, IMailboxAssociationPerformanceTracker performanceTracker, IExtensibleLogger logger)
		{
			ArgumentValidator.ThrowIfNull("mailboxLocator", mailboxLocator);
			ArgumentValidator.ThrowIfNull("session", session);
			ArgumentValidator.ThrowIfNull("xsoFactory", xsoFactory);
			ArgumentValidator.ThrowIfNull("performanceTracker", performanceTracker);
			ArgumentValidator.ThrowIfNull("logger", logger);
			this.mailboxLocator = mailboxLocator;
			this.session = session;
			this.shouldDisposeSession = shouldDisposeSession;
			this.xsoFactory = xsoFactory;
			this.performanceTracker = performanceTracker;
			this.logger = logger;
			this.disposeTracker = this.GetDisposeTracker();
			this.mailboxAssociationFolder = new LazilyInitialized<IFolder>(new Func<IFolder>(this.GetOrCreateDefaultAssociationsFolder));
		}

		public IMailboxLocator MailboxLocator
		{
			get
			{
				return this.mailboxLocator;
			}
		}

		public string ServerFullyQualifiedDomainName
		{
			get
			{
				StoreSession storeSession = this.session as StoreSession;
				string text;
				if (storeSession != null)
				{
					text = storeSession.ServerFullyQualifiedDomainName;
					LocalAssociationStore.Tracer.TraceDebug<string>((long)this.GetHashCode(), "LocalAssociationStore::ServerFullyQualifiedDomainName. Returning server name found in session: {0}", text);
				}
				else
				{
					text = LocalServerCache.LocalServerFqdn;
					LocalAssociationStore.Tracer.TraceDebug<string>((long)this.GetHashCode(), "LocalAssociationStore::ServerFullyQualifiedDomainName. Unkown session type returning local server name. {0}", text);
				}
				return text;
			}
		}

		public MailboxAssociationProcessingFlags AssociationProcessingFlags
		{
			get
			{
				this.LoadReplicationState(false);
				return this.mailboxReplicationFlags;
			}
		}

		public ExDateTime MailboxNextSyncTime
		{
			get
			{
				this.LoadReplicationState(false);
				return this.mailboxNextSyncTime;
			}
		}

		public IExchangePrincipal MailboxOwner
		{
			get
			{
				this.CheckDisposed("MailboxOwner");
				return this.session.MailboxOwner;
			}
		}

		internal IMailboxSession Session
		{
			get
			{
				this.CheckDisposed("Session");
				return this.session;
			}
		}

		public static void SaveMailboxSyncStatus(IMailboxSession session, ExDateTime? nextReplicationTime, MailboxAssociationProcessingFlags? mailboxAssociationProcessingFlags)
		{
			ArgumentValidator.ThrowIfNull("session", session);
			bool flag = false;
			if (nextReplicationTime != null)
			{
				LocalAssociationStore.Tracer.TraceDebug<ExDateTime?>(0L, "LocalAssociationStore::SaveMailboxSyncStatus. Setting NextReplicationTime = {0}", nextReplicationTime);
				session.Mailbox[MailboxSchema.MailboxAssociationNextReplicationTime] = nextReplicationTime;
				flag = true;
			}
			if (mailboxAssociationProcessingFlags != null)
			{
				LocalAssociationStore.Tracer.TraceDebug<MailboxAssociationProcessingFlags?>(0L, "LocalAssociationStore::SaveMailboxSyncStatus. Setting ProcessingFlags = {0}", mailboxAssociationProcessingFlags);
				session.Mailbox[MailboxSchema.MailboxAssociationProcessingFlags] = mailboxAssociationProcessingFlags;
				flag = true;
			}
			if (flag)
			{
				LocalAssociationStore.Tracer.TraceDebug(0L, "LocalAssociationStore::SaveMailboxSyncStatus. Saving and reloading mailbox table.");
				session.Mailbox.Save();
				session.Mailbox.Load();
				return;
			}
			LocalAssociationStore.Tracer.TraceDebug(0L, "LocalAssociationStore::SaveMailboxSyncStatus. No changes were detected.");
		}

		public void SaveMailboxAsOutOfSync()
		{
			if (!this.mailboxMarkedAsOutOfSync)
			{
				LocalAssociationStore.Tracer.TraceDebug((long)this.GetHashCode(), "LocalAssociationStore::SaveMailboxAsOutOfSync. Marking mailbox with next replication date set to now");
				this.SaveMailboxSyncStatus(ExDateTime.UtcNow);
				this.mailboxMarkedAsOutOfSync = true;
				return;
			}
			LocalAssociationStore.Tracer.TraceWarning((long)this.GetHashCode(), "LocalAssociationStore::SaveMailboxAsOutOfSync. Mailbox has previously been marked as out-of-sync. Skiping");
		}

		public void SaveMailboxSyncStatus(ExDateTime nextReplicationTime)
		{
			this.SaveMailboxSyncStatusInternal(new ExDateTime?(nextReplicationTime), null);
		}

		public void SaveMailboxSyncStatus(ExDateTime nextReplicationTime, MailboxAssociationProcessingFlags mailboxAssociationProcessingFlags)
		{
			this.SaveMailboxSyncStatusInternal(new ExDateTime?(nextReplicationTime), new MailboxAssociationProcessingFlags?(mailboxAssociationProcessingFlags));
		}

		public IMailboxAssociationGroup CreateGroupAssociation()
		{
			this.CheckDisposed("CreateGroupAssociation");
			this.performanceTracker.IncrementAssociationsCreated();
			IFolder value = this.mailboxAssociationFolder.Value;
			return this.xsoFactory.CreateMailboxAssociationGroup(this.session, value.Id);
		}

		public IMailboxAssociationUser CreateUserAssociation()
		{
			this.CheckDisposed("CreateUserAssociation");
			this.performanceTracker.IncrementAssociationsCreated();
			IFolder value = this.mailboxAssociationFolder.Value;
			return this.xsoFactory.CreateMailboxAssociationUser(this.session, value.Id);
		}

		public void DeleteAssociation(IMailboxAssociationBaseItem associationItem)
		{
			ArgumentValidator.ThrowIfNull("associationItem", associationItem);
			this.CheckDisposed("DeleteAssociation");
			MailboxAssociationBaseItem mailboxAssociationBaseItem = (MailboxAssociationBaseItem)associationItem;
			this.DeleteAssociation(mailboxAssociationBaseItem.GetValueOrDefault<VersionedId>(ItemSchema.Id));
		}

		public void OpenAssociationAsReadWrite(IMailboxAssociationBaseItem associationItem)
		{
			ArgumentValidator.ThrowIfNull("associationItem", associationItem);
			this.CheckDisposed("OpenAssociationAsReadWrite");
			MailboxAssociationBaseItem mailboxAssociationBaseItem = (MailboxAssociationBaseItem)associationItem;
			mailboxAssociationBaseItem.OpenAsReadWrite();
		}

		public void SaveAssociation(IMailboxAssociationBaseItem association)
		{
			ArgumentValidator.ThrowIfNull("association", association);
			this.CheckDisposed("SaveAssociation");
			this.performanceTracker.IncrementAssociationsUpdated();
			MailboxAssociationBaseItem mailboxAssociationBaseItem = (MailboxAssociationBaseItem)association;
			mailboxAssociationBaseItem.Save(SaveMode.ResolveConflicts);
		}

		public IEnumerable<IPropertyBag> GetAssociationsByType(string associationItemClass, PropertyDefinition associationTypeProperty, params PropertyDefinition[] propertiesToRetrieve)
		{
			return this.GetAssociationsByType(associationItemClass, associationTypeProperty, null, propertiesToRetrieve);
		}

		public IEnumerable<IPropertyBag> GetAssociationsByType(string associationItemClass, PropertyDefinition associationTypeProperty, int? maxItems, params PropertyDefinition[] propertiesToRetrieve)
		{
			ArgumentValidator.ThrowIfNullOrEmpty("associationItemClass", associationItemClass);
			ArgumentValidator.ThrowIfNull("associationTypeProperty", associationTypeProperty);
			this.CheckDisposed("GetAssociationsByType");
			if (!typeof(bool).IsAssignableFrom(associationTypeProperty.Type))
			{
				throw new InvalidOperationException("LocalAssociationStore::GetItemByAssociationType. Parameter associationTypeProperty should be a boolean property.");
			}
			AndFilter filter = new AndFilter(new QueryFilter[]
			{
				new ComparisonFilter(ComparisonOperator.Equal, StoreObjectSchema.ItemClass, associationItemClass),
				new ComparisonFilter(ComparisonOperator.Equal, associationTypeProperty, true)
			});
			return this.QueryAssociationFolder(associationTypeProperty.Name, filter, maxItems, propertiesToRetrieve);
		}

		public IEnumerable<IPropertyBag> GetAssociationsWithMembershipChangedAfter(ExDateTime date, params PropertyDefinition[] properties)
		{
			ArgumentValidator.ThrowIfNull("date", date);
			QueryFilter seekFilter = QueryFilter.AndTogether(new QueryFilter[]
			{
				LocalAssociationStore.ItemClassUserFilter,
				new ComparisonFilter(ComparisonOperator.GreaterThanOrEqual, MailboxAssociationBaseSchema.JoinDate, date)
			});
			return this.SortAndSeekAssociationsFolder(LocalAssociationStore.JoinedAfterDateSortBys, seekFilter, properties);
		}

		public TValue GetValueOrDefault<TValue>(IPropertyBag propertyBag, PropertyDefinition propertyDefinition, TValue defaultValue)
		{
			IStorePropertyBag storePropertyBag = (IStorePropertyBag)propertyBag;
			return storePropertyBag.GetValueOrDefault<TValue>(propertyDefinition, defaultValue);
		}

		public IMailboxAssociationGroup GetGroupAssociationByIdProperty(PropertyDefinition idProperty, params object[] idValues)
		{
			ArgumentValidator.ThrowIfNull("idProperty", idProperty);
			ArgumentValidator.ThrowIfNull("idValues", idValues);
			this.CheckDisposed("GetGroupAssociationByIdProperty");
			return this.GetAssociationByIdProperty<IMailboxAssociationGroup>(new Func<VersionedId, IMailboxAssociationGroup>(this.GetGroupAssociationByItemId), "IPM.MailboxAssociation.Group", MailboxAssociationGroupSchema.Instance.AllProperties, idProperty, idValues);
		}

		public IMailboxAssociationGroup GetGroupAssociationByItemId(VersionedId itemId)
		{
			ArgumentValidator.ThrowIfNull("itemId", itemId);
			this.CheckDisposed("GetGroupAssociationByItemId");
			return this.xsoFactory.BindToMailboxAssociationGroup(this.session, itemId.ObjectId, MailboxAssociationGroupSchema.Instance.AllProperties);
		}

		public IMailboxAssociationUser GetUserAssociationByIdProperty(PropertyDefinition idProperty, params object[] idValues)
		{
			ArgumentValidator.ThrowIfNull("idProperty", idProperty);
			ArgumentValidator.ThrowIfNull("idValues", idValues);
			this.CheckDisposed("GetUserAssociationByIdProperty");
			return this.GetAssociationByIdProperty<IMailboxAssociationUser>(new Func<VersionedId, IMailboxAssociationUser>(this.GetUserAssociationByItemId), "IPM.MailboxAssociation.User", MailboxAssociationUserSchema.Instance.AllProperties, idProperty, idValues);
		}

		public IMailboxAssociationUser GetUserAssociationByItemId(VersionedId itemId)
		{
			ArgumentValidator.ThrowIfNull("itemId", itemId);
			this.CheckDisposed("GetUserAssociationByItemId");
			return this.xsoFactory.BindToMailboxAssociationUser(this.session, itemId.ObjectId, MailboxAssociationUserSchema.Instance.AllProperties);
		}

		public IEnumerable<IPropertyBag> GetAllAssociations(string associationItemClass, ICollection<PropertyDefinition> propertiesToRetrieve)
		{
			ArgumentValidator.ThrowIfNullOrWhiteSpace("associationItemClass", associationItemClass);
			LocalAssociationStore.Tracer.TraceDebug<string, Guid>((long)this.GetHashCode(), "LocalAssociationStore.GetAllAssociations: SeekInAssociationFolder for ItemClass={0}. Mailbox={1}", associationItemClass, this.session.MailboxGuid);
			ComparisonFilter seekFilter = new ComparisonFilter(ComparisonOperator.Equal, StoreObjectSchema.ItemClass, associationItemClass);
			return this.SortAndSeekAssociationsFolder(LocalAssociationStore.SortByItemClass, seekFilter, propertiesToRetrieve);
		}

		private IEnumerable<IPropertyBag> SortAndSeekAssociationsFolder(SortBy[] sortBys, QueryFilter seekFilter, ICollection<PropertyDefinition> propertiesToRetrieve)
		{
			IFolder folder = this.mailboxAssociationFolder.Value;
			using (IQueryResult queryResult = folder.IItemQuery(ItemQueryType.None, null, sortBys, propertiesToRetrieve))
			{
				IEnumerable<IPropertyBag> foundItems = this.SeekForValueInQueryResult(queryResult, seekFilter);
				foreach (IPropertyBag propertyBag in foundItems)
				{
					yield return propertyBag;
				}
			}
			yield break;
		}

		public DisposeTracker GetDisposeTracker()
		{
			return DisposeTracker.Get<LocalAssociationStore>(this);
		}

		public void SuppressDisposeTracker()
		{
			if (this.disposeTracker != null)
			{
				this.disposeTracker.Suppress();
			}
		}

		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		private void CheckDisposed(string methodName)
		{
			if (this.disposed)
			{
				LocalAssociationStore.Tracer.TraceError<string>((long)this.GetHashCode(), "LocalAssociationStore::{0}. Attempted to use disposed object.", methodName);
				throw new ObjectDisposedException(base.GetType().ToString());
			}
		}

		private void Dispose(bool disposing)
		{
			if (!this.disposed)
			{
				if (disposing)
				{
					if (this.mailboxAssociationFolder.IsInitialized)
					{
						this.mailboxAssociationFolder.Value.Dispose();
					}
					if (this.shouldDisposeSession)
					{
						this.session.Dispose();
					}
					if (this.disposeTracker != null)
					{
						this.disposeTracker.Dispose();
					}
				}
				this.disposed = true;
			}
		}

		private TMailboxAssociation GetAssociationByIdProperty<TMailboxAssociation>(Func<VersionedId, TMailboxAssociation> bindFunction, string associationItemClass, ICollection<PropertyDefinition> propertiesToRetrieve, PropertyDefinition idProperty, params object[] idValues) where TMailboxAssociation : class, IMailboxAssociationBaseItem
		{
			this.CheckDisposed("GetAssociationByIdProperty");
			if (idValues.Length == 0)
			{
				return default(TMailboxAssociation);
			}
			IPropertyBag[] array = this.SeekInAssociationFolder<object>(associationItemClass, idProperty, idValues, new PropertyDefinition[]
			{
				idProperty,
				StoreObjectSchema.ItemClass,
				ItemSchema.Id
			}).ToArray<IPropertyBag>();
			string text = null;
			if (LocalAssociationStore.Tracer.IsTraceEnabled(TraceType.DebugTrace) || LocalAssociationStore.Tracer.IsTraceEnabled(TraceType.ErrorTrace) || array.Length > 1)
			{
				text = string.Join(", ", idValues);
			}
			if (array.Length == 0)
			{
				LocalAssociationStore.Tracer.TraceDebug<string, string>((long)this.GetHashCode(), "LocalAssociationStore::GetAssociationByIdProperty. Found no association item searching by property {0} with values {1}.", idProperty.Name, text);
				this.performanceTracker.IncrementFailedAssociationsSearch();
				return default(TMailboxAssociation);
			}
			if (array.Length > 1)
			{
				this.performanceTracker.IncrementNonUniqueAssociationsFound();
				this.LogWarning("LocalAssociationStore::GetAssociationByIdProperty", string.Format("Found more than 1 association item searching by property {0} with values {1}.", idProperty.Name, text));
				this.LogWarning("LocalAssociationStore::GetAssociationByIdProperty", string.Format("Keeping association with ID: {0}.", array[0][ItemSchema.Id]));
				for (int i = 1; i < array.Length; i++)
				{
					VersionedId versionedId = array[i][ItemSchema.Id] as VersionedId;
					using (TMailboxAssociation tmailboxAssociation = bindFunction(versionedId))
					{
						if (tmailboxAssociation != null)
						{
							this.LogWarning("LocalAssociationStore::GetAssociationByIdProperty", string.Format("Keeping association with ID: {0}, Removing association {1}", array[0][ItemSchema.Id], tmailboxAssociation.ToString()));
							this.DeleteAssociation(versionedId);
						}
						else
						{
							this.LogError("LocalAssociationStore::GetAssociationByIdProperty", string.Format("Couldn't bind to association with ID: {0}.", versionedId.ToString()));
						}
					}
				}
			}
			VersionedId versionedId2 = array[0][ItemSchema.Id] as VersionedId;
			LocalAssociationStore.Tracer.TraceDebug((long)this.GetHashCode(), "LocalAssociationStore::GetAssociationByIdProperty. Found association item searching by property {0} with values {1}. ItemId: {2}. Found value: {3}.", new object[]
			{
				idProperty.Name,
				text,
				versionedId2,
				array[0][idProperty]
			});
			return bindFunction(versionedId2);
		}

		private IEnumerable<IPropertyBag> SeekInAssociationFolder<T>(string itemClass, PropertyDefinition seekProperty, IEnumerable<T> seekValues, params PropertyDefinition[] properties)
		{
			ArgumentValidator.ThrowIfNullOrWhiteSpace("itemClass", itemClass);
			ArgumentValidator.ThrowIfNull("seekProperty", seekProperty);
			ArgumentValidator.ThrowIfNull("seekValues", seekValues);
			SortBy[] sortBys = new SortBy[]
			{
				new SortBy(seekProperty, SortOrder.Ascending),
				new SortBy(StoreObjectSchema.ItemClass, SortOrder.Ascending),
				new SortBy(StoreObjectSchema.LastModifiedTime, SortOrder.Descending)
			};
			IFolder folder = this.mailboxAssociationFolder.Value;
			using (IQueryResult queryResult = folder.IItemQuery(ItemQueryType.None, null, sortBys, properties))
			{
				foreach (T value in seekValues)
				{
					IEnumerable<IPropertyBag> foundItems = this.SeekForValueInQueryResult<T>(queryResult, seekProperty, value, itemClass);
					foreach (IPropertyBag propertyBag in foundItems)
					{
						yield return propertyBag;
					}
				}
			}
			yield break;
		}

		private IEnumerable<IPropertyBag> SeekForValueInQueryResult<T>(IQueryResult queryResult, PropertyDefinition seekProperty, T seekValue, string itemClass)
		{
			LocalAssociationStore.Tracer.TraceDebug<string, T, Guid>((long)this.GetHashCode(), "LocalAssociationStore.SeekForValueInQueryResult: SeekInAssociationFolder for {0}={1}. Mailbox={2}", seekProperty.Name, seekValue, this.session.MailboxGuid);
			AndFilter seekFilter = new AndFilter(new QueryFilter[]
			{
				new ComparisonFilter(ComparisonOperator.Equal, seekProperty, seekValue),
				new ComparisonFilter(ComparisonOperator.Equal, StoreObjectSchema.ItemClass, itemClass)
			});
			return this.SeekForValueInQueryResult(queryResult, seekFilter);
		}

		private IEnumerable<IPropertyBag> SeekForValueInQueryResult(IQueryResult queryResult, QueryFilter seekFilter)
		{
			if (queryResult.SeekToCondition(SeekReference.OriginBeginning, seekFilter, SeekToConditionFlags.AllowExtendedFilters))
			{
				bool itemsRemaining = true;
				while (itemsRemaining)
				{
					LocalAssociationStore.Tracer.TraceDebug<Guid>((long)this.GetHashCode(), "LocalAssociationStore.SeekForValueInQueryResult: Retrieving more items. Mailbox={0}", this.session.MailboxGuid);
					IStorePropertyBag[] items = queryResult.GetPropertyBags(100);
					if (items != null && items.Length > 0)
					{
						foreach (IStorePropertyBag item in items)
						{
							if (EvaluatableFilter.Evaluate(seekFilter, item))
							{
								LocalAssociationStore.Tracer.TraceDebug<Guid>((long)this.GetHashCode(), "LocalAssociationStore.SeekForValueInQueryResult: Returning found property bag. Mailbox={0}", this.session.MailboxGuid);
								this.performanceTracker.IncrementAssociationsRead();
								yield return item;
							}
							else
							{
								itemsRemaining = false;
							}
						}
					}
					else
					{
						itemsRemaining = false;
					}
				}
			}
			LocalAssociationStore.Tracer.TraceDebug<Guid>((long)this.GetHashCode(), "LocalAssociationStore.SeekForValueInQueryResult: No more property bags found. Mailbox={0}", this.session.MailboxGuid);
			yield break;
		}

		private IEnumerable<IPropertyBag> QueryAssociationFolder(string context, QueryFilter filter, int? maxItems, params PropertyDefinition[] properties)
		{
			if (maxItems != null)
			{
				ArgumentValidator.ThrowIfZeroOrNegative("maxItems", maxItems.Value);
			}
			IFolder folder = this.mailboxAssociationFolder.Value;
			properties = PropertyDefinitionCollection.Merge<PropertyDefinition>(properties, new PropertyDefinition[]
			{
				ItemSchema.Id
			});
			using (IQueryResult queryResult = folder.IItemQuery(ItemQueryType.None, filter, null, properties))
			{
				int itemsNumber = 0;
				bool fetchAllItems = maxItems == null;
				while (fetchAllItems || itemsNumber < maxItems.Value)
				{
					LocalAssociationStore.Tracer.TraceDebug<Guid>((long)this.GetHashCode(), "LocalAssociationStore.QueryAssociationFolder: Retrieving mailbox associations in mailbox {0}.", this.session.MailboxGuid);
					int fetchRowCount = fetchAllItems ? 100 : Math.Min(maxItems.Value - itemsNumber, 100);
					IStorePropertyBag[] items = queryResult.GetPropertyBags(fetchRowCount);
					if (items == null || items.Length == 0)
					{
						LocalAssociationStore.Tracer.TraceDebug((long)this.GetHashCode(), "LocalAssociationStore.QueryAssociationFolder: No more property bags found.");
						yield break;
					}
					foreach (IStorePropertyBag item in items)
					{
						LocalAssociationStore.Tracer.TraceDebug((long)this.GetHashCode(), "LocalAssociationStore.QueryAssociationFolder: Returning property bag with Id {0}.", new object[]
						{
							item[ItemSchema.Id]
						});
						this.performanceTracker.IncrementAssociationsRead();
						if (this.ValidateItem(context, item))
						{
							yield return item;
						}
					}
					itemsNumber += items.Length;
				}
			}
			yield break;
		}

		private void LoadReplicationState(bool reload = false)
		{
			if (reload || !this.mailboxReplicationStateLoaded)
			{
				LocalAssociationStore.Tracer.TraceDebug<bool>((long)this.GetHashCode(), "LocalAssociationStore::LoadReplicationState. Loading mailbox properties. (Reload={0})", reload);
				this.session.Mailbox.Load(LocalAssociationStore.MailboxProperties);
				this.mailboxReplicationFlags = this.session.Mailbox.GetValueOrDefault<MailboxAssociationProcessingFlags>(MailboxSchema.MailboxAssociationProcessingFlags, MailboxAssociationProcessingFlags.None);
				this.mailboxNextSyncTime = this.session.Mailbox.GetValueOrDefault<ExDateTime>(MailboxSchema.MailboxAssociationNextReplicationTime, ExDateTime.MinValue);
				this.mailboxReplicationStateLoaded = true;
				return;
			}
			LocalAssociationStore.Tracer.TraceWarning<bool>((long)this.GetHashCode(), "LocalAssociationStore::LoadReplicationState. Skipping mailbox properties loading. (Reload={0})", reload);
		}

		private void SaveMailboxSyncStatusInternal(ExDateTime? nextReplicationTime, MailboxAssociationProcessingFlags? mailboxAssociationProcessingFlags)
		{
			LocalAssociationStore.SaveMailboxSyncStatus(this.session, nextReplicationTime, mailboxAssociationProcessingFlags);
			this.LoadReplicationState(true);
		}

		private IFolder GetOrCreateDefaultAssociationsFolder()
		{
			StoreObjectId storeObjectId = this.session.GetDefaultFolderId(DefaultFolderType.MailboxAssociation);
			if (storeObjectId == null)
			{
				LocalAssociationStore.Tracer.TraceDebug((long)this.GetHashCode(), "LocalAssociationStore.GetOrCreateDefaultAssociationsFolder: Default mailbox association folder was not found. Attempting to create it");
				storeObjectId = this.session.CreateDefaultFolder(DefaultFolderType.MailboxAssociation);
			}
			LocalAssociationStore.Tracer.TraceDebug<StoreObjectId>((long)this.GetHashCode(), "LocalAssociationStore.GetOrCreateDefaultAssociationsFolder: Mailbox association folder ID = {0}.", storeObjectId);
			IFolder result;
			try
			{
				result = this.xsoFactory.BindToFolder(this.session, storeObjectId);
			}
			catch (ObjectNotFoundException arg)
			{
				LocalAssociationStore.Tracer.TraceError<StoreObjectId, ObjectNotFoundException>((long)this.GetHashCode(), "LocalAssociationStore.GetOrCreateDefaultAssociationsFolder: Couldn't bind to Association folder {0}. Exception='{1}'", storeObjectId, arg);
				StoreObjectId folderId = null;
				if (!this.session.TryFixDefaultFolderId(DefaultFolderType.MailboxAssociation, out folderId))
				{
					this.LogError("LocalAssociationStore.GetOrCreateDefaultAssociationsFolder", string.Format("Failed to repair association folder for mailbox:{0}", this.session.MailboxGuid));
					throw;
				}
				IFolder folder = this.xsoFactory.BindToFolder(this.session, folderId);
				LocalAssociationStore.Tracer.TraceDebug((long)this.GetHashCode(), "Successfully repaired association folder");
				result = folder;
			}
			return result;
		}

		private void DeleteAssociation(VersionedId itemId)
		{
			if (itemId != null)
			{
				LocalAssociationStore.Tracer.TraceDebug<StoreObjectId>((long)this.GetHashCode(), "LocalAssociationStore::DeleteAssociation. Deleting association item with ID={0}.", itemId.ObjectId);
				this.session.Delete(DeleteItemFlags.HardDelete, new StoreId[]
				{
					itemId.ObjectId
				});
				this.performanceTracker.IncrementAssociationsDeleted();
				return;
			}
			LocalAssociationStore.Tracer.TraceDebug((long)this.GetHashCode(), "LocalAssociationStore::DeleteAssociation. Skipping association without item ID.");
		}

		private bool ValidateItem(string context, IStorePropertyBag item)
		{
			object obj = item[MailboxAssociationBaseSchema.LegacyDN];
			if (obj == null || PropertyError.IsPropertyError(obj) || string.IsNullOrEmpty(obj as string))
			{
				this.performanceTracker.IncrementMissingLegacyDns();
				string arg = item[ItemSchema.Id].ToString();
				string valueOrDefault = item.GetValueOrDefault<string>(MailboxAssociationBaseSchema.ExternalId, "empty");
				PropertyError propertyError = obj as PropertyError;
				string arg2 = (propertyError == null) ? "null" : propertyError.ToString();
				string errorMessage = string.Format("LocalAssociationStore.QueryAssociationFolder: Missing LegacyDn for property bag with Id {0}, ExternalId {1}, PropertyError {2}", arg, valueOrDefault, arg2);
				this.LogError(context, errorMessage);
				return false;
			}
			return true;
		}

		private void LogError(string context, string errorMessage)
		{
			LocalAssociationStore.Tracer.TraceError((long)this.GetHashCode(), errorMessage);
			this.logger.LogEvent(new SchemaBasedLogEvent<MailboxAssociationLogSchema.Error>
			{
				{
					MailboxAssociationLogSchema.Error.Context,
					context
				},
				{
					MailboxAssociationLogSchema.Error.Exception,
					errorMessage
				}
			});
		}

		private void LogWarning(string context, string warningMessage)
		{
			LocalAssociationStore.Tracer.TraceWarning((long)this.GetHashCode(), warningMessage);
			this.logger.LogEvent(new SchemaBasedLogEvent<MailboxAssociationLogSchema.Warning>
			{
				{
					MailboxAssociationLogSchema.Warning.Context,
					context
				},
				{
					MailboxAssociationLogSchema.Warning.Message,
					warningMessage
				}
			});
		}

		private const int ItemBatchSize = 100;

		private static readonly Trace Tracer = ExTraceGlobals.LocalAssociationStoreTracer;

		private static readonly PropertyDefinition[] MailboxProperties = new PropertyDefinition[]
		{
			MailboxSchema.MailboxAssociationNextReplicationTime,
			MailboxSchema.MailboxAssociationProcessingFlags
		};

		private static readonly SortBy[] SortByItemClass = new SortBy[]
		{
			new SortBy(StoreObjectSchema.ItemClass, SortOrder.Ascending)
		};

		private static readonly SortBy[] JoinedAfterDateSortBys = new SortBy[]
		{
			new SortBy(StoreObjectSchema.ItemClass, SortOrder.Ascending),
			new SortBy(MailboxAssociationBaseSchema.JoinDate, SortOrder.Ascending)
		};

		private static readonly ComparisonFilter ItemClassUserFilter = new ComparisonFilter(ComparisonOperator.Equal, StoreObjectSchema.ItemClass, "IPM.MailboxAssociation.User");

		private readonly IMailboxLocator mailboxLocator;

		private readonly IMailboxSession session;

		private readonly bool shouldDisposeSession;

		private readonly IXSOFactory xsoFactory;

		private readonly LazilyInitialized<IFolder> mailboxAssociationFolder;

		private readonly IExtensibleLogger logger;

		private readonly IMailboxAssociationPerformanceTracker performanceTracker;

		private readonly DisposeTracker disposeTracker;

		private bool disposed;

		private bool mailboxReplicationStateLoaded;

		private MailboxAssociationProcessingFlags mailboxReplicationFlags;

		private ExDateTime mailboxNextSyncTime;

		private bool mailboxMarkedAsOutOfSync;
	}
}
