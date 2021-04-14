using System;
using System.Collections.Generic;
using Microsoft.Exchange.Collections;
using Microsoft.Exchange.Data.Storage.Principal;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Data.Storage;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal abstract class SharingSubscriptionManagerBase<TKey, TData> : SharingItemManagerBase<TData>, IDisposable where TData : class, ISharingSubscriptionData<TKey>
	{
		protected MailboxSession MailboxSession
		{
			get
			{
				return this.mailboxSession;
			}
		}

		protected SharingSubscriptionManagerBase(MailboxSession mailboxSession, string itemClass, PropertyDefinition[] additionalItemProperties)
		{
			Util.ThrowOnNullArgument(mailboxSession, "mailboxSession");
			Util.ThrowOnNullOrEmptyArgument(itemClass, "itemClass");
			Util.ThrowOnNullArgument(additionalItemProperties, "additionalItemProperties");
			this.mailboxSession = mailboxSession;
			this.folder = Folder.Bind(mailboxSession, DefaultFolderType.Sharing);
			this.itemClass = itemClass;
			this.itemClassFilter = new ComparisonFilter(ComparisonOperator.Equal, StoreObjectSchema.ItemClass, itemClass);
			this.itemProperties = Util.MergeArrays<PropertyDefinition>(new ICollection<PropertyDefinition>[]
			{
				SharingSubscriptionManagerBase<TKey, TData>.ItemPropertiesBase,
				additionalItemProperties
			});
		}

		public void Dispose()
		{
			this.Dispose(true);
		}

		public TData[] GetAll()
		{
			this.CheckDisposed("GetAll");
			object[][] array = this.FindAll();
			List<TData> list = new List<TData>(array.Length);
			for (int i = 0; i < array.Length; i++)
			{
				TData tdata = this.CreateDataObjectFromItem(array[i]);
				if (tdata != null)
				{
					list.Add(tdata);
				}
			}
			ExTraceGlobals.SharingTracer.TraceDebug<IExchangePrincipal, int>((long)this.GetHashCode(), "{0}: All {1} subscription(s) are returned.", this.mailboxSession.MailboxOwner, list.Count);
			return list.ToArray();
		}

		public TData GetExisting(TKey subscriptionKey)
		{
			this.CheckDisposed("GetExisting");
			ExTraceGlobals.SharingTracer.TraceDebug<IExchangePrincipal, TKey>((long)this.GetHashCode(), "{0}: looking for subscription {1}", this.mailboxSession.MailboxOwner, subscriptionKey);
			object[] array = this.FindFirstByKey(subscriptionKey);
			if (array != null)
			{
				TData tdata = this.CreateDataObjectFromItem(array);
				ExTraceGlobals.SharingTracer.TraceDebug<IExchangePrincipal, TData>((long)this.GetHashCode(), "{0}: found subscription {1}", this.mailboxSession.MailboxOwner, tdata);
				return tdata;
			}
			ExTraceGlobals.SharingTracer.TraceDebug<IExchangePrincipal, TKey>((long)this.GetHashCode(), "{0}: No subscription was found {1}", this.mailboxSession.MailboxOwner, subscriptionKey);
			return default(TData);
		}

		public TData GetByLocalFolderId(StoreObjectId localFolderId)
		{
			this.CheckDisposed("GetByLocalFolderId");
			Util.ThrowOnNullArgument(localFolderId, "localFolderId");
			ExTraceGlobals.SharingTracer.TraceDebug<IExchangePrincipal, StoreObjectId>((long)this.GetHashCode(), "{0}: looking for subscription by folderId: {1}", this.mailboxSession.MailboxOwner, localFolderId);
			byte[] localFolderIdBytes = localFolderId.GetBytes();
			object[] array = this.FindFirst((object[] properties) => ArrayComparer<byte>.Comparer.Equals(localFolderIdBytes, SharingItemManagerBase<TData>.TryGetPropertyRef<byte[]>(properties, 2)));
			if (array != null)
			{
				TData tdata = this.CreateDataObjectFromItem(array);
				ExTraceGlobals.SharingTracer.TraceDebug<IExchangePrincipal, TData>((long)this.GetHashCode(), "{0}: found subscription {1}", this.mailboxSession.MailboxOwner, tdata);
				return tdata;
			}
			ExTraceGlobals.SharingTracer.TraceDebug<IExchangePrincipal, StoreObjectId>((long)this.GetHashCode(), "{0}: No subscription was found for folder: {1}.", this.mailboxSession.MailboxOwner, localFolderId);
			return default(TData);
		}

		public TData CreateOrUpdate(TData subscriptionData, bool throwIfConflict)
		{
			this.CheckDisposed("CreateOrUpdate");
			Util.ThrowOnNullArgument(subscriptionData, "subscriptionData");
			TData tdata = default(TData);
			try
			{
				if (subscriptionData.Id == null)
				{
					ExTraceGlobals.SharingTracer.TraceDebug<IExchangePrincipal, TKey>((long)this.GetHashCode(), "{0}: creating subscription {1}", this.mailboxSession.MailboxOwner, subscriptionData.Key);
					using (MessageItem messageItem = MessageItem.Create(this.mailboxSession, this.folder.Id))
					{
						messageItem.ClassName = this.itemClass;
						this.SaveSubscriptionData(messageItem, subscriptionData);
						messageItem.Load();
						this.ResolveConflictAfterCreation(subscriptionData.Key, messageItem.Id);
						goto IL_11B;
					}
				}
				ExTraceGlobals.SharingTracer.TraceDebug<IExchangePrincipal, VersionedId, TData>((long)this.GetHashCode(), "{0}: updating subscription ItemId={1}, Data={2}", this.mailboxSession.MailboxOwner, subscriptionData.Id, subscriptionData);
				using (Item item = MessageItem.Bind(this.mailboxSession, subscriptionData.Id, this.itemProperties))
				{
					this.SaveSubscriptionData(item, subscriptionData);
				}
				IL_11B:;
			}
			catch (SharingConflictException)
			{
				tdata = this.GetExisting(subscriptionData.Key);
				if (tdata == null || !tdata.LocalFolderId.Equals(subscriptionData.LocalFolderId))
				{
					this.Rollback(subscriptionData);
				}
				if (throwIfConflict)
				{
					throw;
				}
			}
			TData result;
			if ((result = tdata) == null)
			{
				result = subscriptionData;
			}
			return result;
		}

		protected void CheckDisposed(string methodName)
		{
			if (this.isDisposed)
			{
				StorageGlobals.TraceFailedCheckDisposed(this, methodName);
				throw new ObjectDisposedException(this.ToString());
			}
		}

		private void Dispose(bool disposing)
		{
			StorageGlobals.TraceDispose(this, this.isDisposed, disposing);
			if (!this.isDisposed)
			{
				this.isDisposed = true;
				if (disposing)
				{
					this.folder.Dispose();
				}
			}
		}

		private void Rollback(TData subscription)
		{
			if (subscription.LocalFolderId != null)
			{
				ExTraceGlobals.SharingTracer.TraceDebug<IExchangePrincipal, TData>((long)this.GetHashCode(), "{0}: Delete the referring folder due to failure of saving subscription {1}", this.mailboxSession.MailboxOwner, subscription);
				this.mailboxSession.Delete(DeleteItemFlags.HardDelete, new StoreId[]
				{
					subscription.LocalFolderId
				});
			}
		}

		private void SaveSubscriptionData(Item item, TData subscriptionData)
		{
			this.StampItemFromDataObject(item, subscriptionData);
			ConflictResolutionResult conflictResolutionResult = item.Save(SaveMode.ResolveConflicts);
			if (conflictResolutionResult.SaveStatus == SaveResult.IrresolvableConflict)
			{
				ExTraceGlobals.SharingTracer.TraceDebug<IExchangePrincipal, TData>((long)this.GetHashCode(), "{0}: A race condition occurred when saving subscription {1}", this.mailboxSession.MailboxOwner, subscriptionData);
				throw new SharingConflictException(conflictResolutionResult);
			}
			ExTraceGlobals.SharingTracer.TraceDebug<IExchangePrincipal, TData>((long)this.GetHashCode(), "{0}: saved subscription {1}", this.mailboxSession.MailboxOwner, subscriptionData);
		}

		private void ResolveConflictAfterCreation(TKey subscriptionKey, VersionedId subscriptionId)
		{
			object[] array = this.FindFirstByKey(subscriptionKey);
			VersionedId versionedId = (VersionedId)array[1];
			if (!subscriptionId.ObjectId.Equals(versionedId.ObjectId))
			{
				ExTraceGlobals.SharingTracer.TraceDebug<IExchangePrincipal, VersionedId>((long)this.GetHashCode(), "{0}: A race condition occurred. This subscription is about to be deleted due to duplication: {0}", this.mailboxSession.MailboxOwner, subscriptionId);
				AggregateOperationResult aggregateOperationResult = this.mailboxSession.Delete(DeleteItemFlags.HardDelete, new StoreId[]
				{
					subscriptionId
				});
				if (aggregateOperationResult.OperationResult != OperationResult.Succeeded)
				{
					ExTraceGlobals.SharingTracer.TraceError<IExchangePrincipal, VersionedId, AggregateOperationResult>((long)this.GetHashCode(), "{0}: Failed to delete this subscription {1}: {2}", this.mailboxSession.MailboxOwner, subscriptionId, aggregateOperationResult);
				}
				throw new SharingConflictException();
			}
		}

		private object[][] FindAll()
		{
			List<object[]> list = new List<object[]>();
			using (QueryResult queryResult = this.folder.ItemQuery(ItemQueryType.None, null, SharingSubscriptionManagerBase<TKey, TData>.ItemQuerySorts, this.itemProperties))
			{
				if (queryResult.SeekToCondition(SeekReference.OriginBeginning, this.itemClassFilter))
				{
					object[][] rows;
					int num;
					for (;;)
					{
						rows = queryResult.GetRows(10000);
						if (rows.Length == 0)
						{
							goto IL_A2;
						}
						num = 0;
						foreach (object[] properties in rows)
						{
							string y = SharingItemManagerBase<TData>.TryGetPropertyRef<string>(properties, 0);
							if (!StringComparer.OrdinalIgnoreCase.Equals(this.itemClass, y))
							{
								break;
							}
							num++;
						}
						if (num != rows.Length)
						{
							break;
						}
						list.AddRange(rows);
					}
					if (num > 0)
					{
						Array.Resize<object[]>(ref rows, num);
						list.AddRange(rows);
					}
				}
				IL_A2:;
			}
			return list.ToArray();
		}

		protected object[] FindFirst(Predicate<object[]> match)
		{
			using (QueryResult queryResult = this.folder.ItemQuery(ItemQueryType.None, null, SharingSubscriptionManagerBase<TKey, TData>.ItemQuerySorts, this.itemProperties))
			{
				if (queryResult.SeekToCondition(SeekReference.OriginBeginning, this.itemClassFilter))
				{
					for (;;)
					{
						object[][] rows = queryResult.GetRows(10000);
						if (rows.Length == 0)
						{
							goto IL_89;
						}
						bool flag = false;
						foreach (object[] array2 in rows)
						{
							string y = SharingItemManagerBase<TData>.TryGetPropertyRef<string>(array2, 0);
							if (!StringComparer.OrdinalIgnoreCase.Equals(this.itemClass, y))
							{
								flag = true;
								break;
							}
							if (match(array2))
							{
								goto Block_5;
							}
						}
						if (flag)
						{
							goto IL_89;
						}
					}
					Block_5:
					object[] array2;
					return array2;
				}
				IL_89:;
			}
			return null;
		}

		protected abstract object[] FindFirstByKey(TKey subscriptionKey);

		private readonly MailboxSession mailboxSession;

		private readonly Folder folder;

		private readonly string itemClass;

		private readonly ComparisonFilter itemClassFilter;

		private readonly PropertyDefinition[] itemProperties;

		private static readonly PropertyDefinition[] ItemPropertiesBase = new PropertyDefinition[]
		{
			StoreObjectSchema.ItemClass,
			ItemSchema.Id,
			SharingSchema.ExternalSharingLocalFolderId
		};

		private static readonly SortBy[] ItemQuerySorts = new SortBy[]
		{
			new SortBy(InternalSchema.ItemClass, SortOrder.Ascending),
			new SortBy(InternalSchema.LastModifiedTime, SortOrder.Ascending),
			new SortBy(InternalSchema.CreationTime, SortOrder.Ascending),
			new SortBy(InternalSchema.MID, SortOrder.Ascending)
		};

		private bool isDisposed;

		protected enum ItemPropertiesIndexBase
		{
			ItemClass,
			Id,
			ExternalSharingLocalFolderId,
			Next
		}
	}
}
