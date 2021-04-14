using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Exchange.Data.QuickLogStrings;
using Microsoft.Exchange.Data.Storage.Principal;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.InfoWorker.Common;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Mapi;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class SingleInstanceItemHandler
	{
		public SingleInstanceItemHandler(string messageClass, DefaultFolderType defaultFolder)
		{
			if (messageClass == null)
			{
				throw new ArgumentNullException("messageClass");
			}
			EnumValidator.ThrowIfInvalid<DefaultFolderType>(defaultFolder, "defaultFolder");
			this.messageClass = messageClass;
			this.defaultFolder = defaultFolder;
		}

		public override string ToString()
		{
			if (this.toString == null)
			{
				this.toString = "Single instance item for MessageClass=" + this.messageClass + ", Folder=" + this.defaultFolder.ToString();
			}
			return this.toString;
		}

		public string GetItemContent(MailboxSession itemStore)
		{
			List<object[]> results = this.QueryItems(itemStore);
			VersionedId mostRecentItem = this.GetMostRecentItem(results);
			return this.GetItemContent(itemStore, mostRecentItem);
		}

		public string GetItemContent(MailboxSession itemStore, StoreId itemId)
		{
			if (itemId == null)
			{
				SingleInstanceItemHandler.Tracer.TraceDebug<SingleInstanceItemHandler, IExchangePrincipal>((long)this.GetHashCode(), "{0}: Item not found in {1}", this, itemStore.MailboxOwner);
				return null;
			}
			SingleInstanceItemHandler.Tracer.TraceDebug<SingleInstanceItemHandler, IExchangePrincipal>((long)this.GetHashCode(), "{0}: Item found in '{1}'", this, itemStore.MailboxOwner);
			string result;
			try
			{
				using (Item item = MessageItem.Bind(itemStore, itemId))
				{
					using (TextReader textReader = item.Body.OpenTextReader(BodyFormat.TextPlain))
					{
						result = textReader.ReadToEnd();
					}
				}
			}
			catch (ObjectNotFoundException)
			{
				SingleInstanceItemHandler.Tracer.TraceDebug<SingleInstanceItemHandler, IExchangePrincipal>((long)this.GetHashCode(), "{0}: Item no longer found in {1}", this, itemStore.MailboxOwner);
				result = null;
			}
			return result;
		}

		public VersionedId SetItemContent(MailboxSession itemStore, string body)
		{
			return this.Update(itemStore, false, (string existingContent) => body);
		}

		public void Delete(MailboxSession itemStore)
		{
			List<object[]> results = this.QueryItems(itemStore);
			VersionedId mostRecentItem = this.GetMostRecentItem(results);
			if (mostRecentItem != null)
			{
				try
				{
					itemStore.Delete(DeleteItemFlags.HardDelete, new StoreId[]
					{
						mostRecentItem
					});
				}
				catch (ObjectNotFoundException)
				{
					SingleInstanceItemHandler.Tracer.TraceError<SingleInstanceItemHandler, VersionedId, IExchangePrincipal>((long)this.GetHashCode(), "{0}: ObjectNotFoundException encountred while trying to delete object, itemId='{1}' on mailbox {2}", this, mostRecentItem, itemStore.MailboxOwner);
				}
			}
		}

		public void UpdateItemContent(MailboxSession itemStore, SingleInstanceItemHandler.ContentUpdater updater)
		{
			this.Update(itemStore, true, updater);
		}

		private VersionedId Update(MailboxSession itemStore, bool getExisting, SingleInstanceItemHandler.ContentUpdater updater)
		{
			int i = 0;
			while (i < 5)
			{
				i++;
				try
				{
					return this.UpdateRetryable(itemStore, getExisting, updater);
				}
				catch (MapiExceptionObjectChanged arg)
				{
					SingleInstanceItemHandler.Tracer.TraceError<SingleInstanceItemHandler, IExchangePrincipal, MapiExceptionObjectChanged>((long)this.GetHashCode(), "{0}: Exception updating item: Mailbox = {1}, exception = {2}, retrying", this, itemStore.MailboxOwner, arg);
					if (i == 5)
					{
						throw;
					}
				}
				catch (ObjectNotFoundException arg2)
				{
					SingleInstanceItemHandler.Tracer.TraceError<SingleInstanceItemHandler, IExchangePrincipal, ObjectNotFoundException>((long)this.GetHashCode(), "{0}: Exception updating item: Mailbox = {1}, exception = {2}, retrying", this, itemStore.MailboxOwner, arg2);
					if (i == 5)
					{
						throw;
					}
				}
				catch (SaveConflictException arg3)
				{
					SingleInstanceItemHandler.Tracer.TraceError<SingleInstanceItemHandler, IExchangePrincipal, SaveConflictException>((long)this.GetHashCode(), "{0}: Exception updating item: Mailbox = {1}, exception = {2}, retrying", this, itemStore.MailboxOwner, arg3);
					if (i == 5)
					{
						throw;
					}
				}
			}
			return null;
		}

		private VersionedId UpdateRetryable(MailboxSession itemStore, bool getExisting, SingleInstanceItemHandler.ContentUpdater updater)
		{
			List<object[]> list = this.QueryItems(itemStore);
			VersionedId versionedId = this.GetMostRecentItem(list);
			if (versionedId == null)
			{
				try
				{
					using (Folder folder = Folder.Bind(itemStore, this.defaultFolder))
					{
						using (Item item = MessageItem.Create(itemStore, folder.Id))
						{
							item.ClassName = this.messageClass;
							using (TextWriter textWriter = item.Body.OpenTextWriter(BodyFormat.TextPlain))
							{
								textWriter.Write(updater(null));
							}
							ConflictResolutionResult conflictResolutionResult = item.Save(SaveMode.ResolveConflicts);
							if (conflictResolutionResult.SaveStatus == SaveResult.IrresolvableConflict)
							{
								throw new SaveConflictException(ServerStrings.ExSaveFailedBecauseOfConflicts(item.Id), conflictResolutionResult);
							}
							item.Load();
							versionedId = item.Id;
						}
					}
					SingleInstanceItemHandler.Tracer.TraceDebug<SingleInstanceItemHandler, VersionedId, IExchangePrincipal>((long)this.GetHashCode(), "{0}: Created new item itemId={1} on mailbox {2}", this, versionedId, itemStore.MailboxOwner);
					goto IL_1CD;
				}
				catch (ObjectNotFoundException ex)
				{
					SingleInstanceItemHandler.Tracer.TraceError<SingleInstanceItemHandler, IExchangePrincipal, ObjectNotFoundException>((long)this.GetHashCode(), "{0}: bind to folder failed on mailbox {1}. Exception={2}", this, itemStore.MailboxOwner, ex);
					throw new SingleInstanceItemHandlerPermanentException(Strings.FailedToGetItem(this.messageClass, this.defaultFolder.ToString()), ex);
				}
			}
			using (Item item2 = MessageItem.Bind(itemStore, versionedId))
			{
				string existingContent = null;
				if (getExisting)
				{
					using (TextReader textReader = item2.Body.OpenTextReader(BodyFormat.TextPlain))
					{
						existingContent = textReader.ReadToEnd();
					}
				}
				using (TextWriter textWriter2 = item2.Body.OpenTextWriter(BodyFormat.TextPlain))
				{
					textWriter2.Write(updater(existingContent));
				}
				ConflictResolutionResult conflictResolutionResult2 = item2.Save(SaveMode.ResolveConflicts);
				if (conflictResolutionResult2.SaveStatus == SaveResult.IrresolvableConflict)
				{
					throw new SaveConflictException(ServerStrings.ExSaveFailedBecauseOfConflicts(item2.Id), conflictResolutionResult2);
				}
			}
			SingleInstanceItemHandler.Tracer.TraceDebug<SingleInstanceItemHandler, VersionedId, IExchangePrincipal>((long)this.GetHashCode(), "{0}: Updated item itemId={1} on mailbox {2}", this, versionedId, itemStore.MailboxOwner);
			IL_1CD:
			using (IEnumerator<object[]> enumerator = list.GetEnumerator())
			{
				List<VersionedId> list2 = new List<VersionedId>();
				for (;;)
				{
					bool flag = enumerator.MoveNext();
					if ((!flag || list2.Count == 100) && list2.Count > 0)
					{
						try
						{
							itemStore.Delete(DeleteItemFlags.HardDelete, list2.ToArray());
						}
						catch (ObjectNotFoundException)
						{
							SingleInstanceItemHandler.Tracer.TraceError<SingleInstanceItemHandler, IExchangePrincipal>((long)this.GetHashCode(), "{0}: ObjectNotFoundException encountred while trying to delete duplicate item on mailbox {1}", this, itemStore.MailboxOwner);
						}
						list2.Clear();
					}
					if (!flag)
					{
						break;
					}
					VersionedId versionedId2 = (VersionedId)enumerator.Current[0];
					if (versionedId != versionedId2)
					{
						list2.Add(versionedId2);
						SingleInstanceItemHandler.Tracer.TraceDebug<SingleInstanceItemHandler, VersionedId, IExchangePrincipal>((long)this.GetHashCode(), "{0}: Deleting extra item {1} on mailbox {2}", this, versionedId2, itemStore.MailboxOwner);
					}
				}
			}
			return versionedId;
		}

		private VersionedId GetMostRecentItem(List<object[]> results)
		{
			ExDateTime t = ExDateTime.MinValue;
			VersionedId result = null;
			for (int i = 0; i < results.Count; i++)
			{
				object[] array = results[i];
				object obj = array[0];
				object obj2 = array[1];
				if (!(obj is VersionedId))
				{
					throw new SingleInstanceItemHandlerTransientException(Strings.descMissingProperty("Id", (obj == null) ? "<null>" : obj.ToString()));
				}
				if (!(obj2 is ExDateTime))
				{
					throw new SingleInstanceItemHandlerTransientException(Strings.descMissingProperty("LastModifiedTime", (obj2 == null) ? "<null>" : obj2.ToString()));
				}
				VersionedId versionedId = (VersionedId)obj;
				ExDateTime exDateTime = (ExDateTime)obj2;
				if (exDateTime > t)
				{
					result = versionedId;
					t = exDateTime;
				}
			}
			return result;
		}

		private List<object[]> QueryItems(MailboxSession itemStore)
		{
			List<object[]> list = new List<object[]>();
			try
			{
				using (Folder folder = Folder.Bind(itemStore, this.defaultFolder))
				{
					using (QueryResult queryResult = folder.ItemQuery(ItemQueryType.None, null, null, SingleInstanceItemHandler.ItemQueryReturnColumns))
					{
						for (;;)
						{
							object[][] rows = queryResult.GetRows(100);
							if (rows == null || rows.Length == 0 || list.Count > 100000)
							{
								break;
							}
							foreach (object[] array in rows)
							{
								string value = array[2] as string;
								if (this.messageClass.Equals(value, StringComparison.OrdinalIgnoreCase))
								{
									list.Add(array);
								}
							}
						}
					}
				}
			}
			catch (ObjectNotFoundException ex)
			{
				SingleInstanceItemHandler.Tracer.TraceError<SingleInstanceItemHandler, IExchangePrincipal, ObjectNotFoundException>((long)this.GetHashCode(), "{0}: bind to folder failed on mailbox {1}. Exception={2}", this, itemStore.MailboxOwner, ex);
				throw new SingleInstanceItemHandlerPermanentException(Strings.FailedToGetItem(this.messageClass, this.defaultFolder.ToString()), ex);
			}
			return list;
		}

		private const int DeleteBatchSize = 100;

		private const int QueryBatchSize = 100;

		private const int MaximumQueryListSize = 100000;

		private const int MaxRetry = 5;

		private string messageClass;

		private DefaultFolderType defaultFolder;

		private string toString;

		private static readonly SortBy[] ItemQuerySortColumns = new SortBy[]
		{
			new SortBy(StoreObjectSchema.ItemClass, SortOrder.Ascending)
		};

		private static readonly PropertyDefinition[] ItemQueryReturnColumns = new PropertyDefinition[]
		{
			ItemSchema.Id,
			StoreObjectSchema.LastModifiedTime,
			StoreObjectSchema.ItemClass
		};

		private static readonly Trace Tracer = ExTraceGlobals.SingleInstanceItemTracer;

		public delegate string ContentUpdater(string existingContent);

		private enum ItemQueryReturnColumnIndex
		{
			Id,
			LastModifiedTime,
			ItemClass
		}
	}
}
