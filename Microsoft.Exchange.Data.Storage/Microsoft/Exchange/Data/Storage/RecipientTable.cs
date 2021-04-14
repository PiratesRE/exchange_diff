using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Data.Storage;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Mapi;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal sealed class RecipientTable : DisposableObject
	{
		internal RecipientTable(ICoreItem item)
		{
			bool flag = false;
			try
			{
				StorageGlobals.TraceConstructIDisposable(this);
				if (item.Session == null)
				{
					this.recipientTable = null;
				}
				else
				{
					this.mapiMessage = (MapiMessage)CoreObject.GetPersistablePropertyBag(item).MapiProp;
					this.storeSession = item.Session;
					this.timeZone = CoreObject.GetPersistablePropertyBag(item).ExTimeZone;
					StoreSession storeSession = this.storeSession;
					bool flag2 = false;
					try
					{
						if (storeSession != null)
						{
							storeSession.BeginMapiCall();
							storeSession.BeginServerHealthCall();
							flag2 = true;
						}
						if (StorageGlobals.MapiTestHookBeforeCall != null)
						{
							StorageGlobals.MapiTestHookBeforeCall(MethodBase.GetCurrentMethod());
						}
						this.recipientTable = this.mapiMessage.GetRecipientTable();
					}
					catch (MapiPermanentException ex)
					{
						throw StorageGlobals.TranslateMapiException(ServerStrings.MapiCannotGetRecipientTable, ex, storeSession, this, "{0}. MapiException = {1}.", new object[]
						{
							string.Format("RecipientTable::Storage.RecipientTable.", new object[0]),
							ex
						});
					}
					catch (MapiRetryableException ex2)
					{
						throw StorageGlobals.TranslateMapiException(ServerStrings.MapiCannotGetRecipientTable, ex2, storeSession, this, "{0}. MapiException = {1}.", new object[]
						{
							string.Format("RecipientTable::Storage.RecipientTable.", new object[0]),
							ex2
						});
					}
					finally
					{
						try
						{
							if (storeSession != null)
							{
								storeSession.EndMapiCall();
								if (flag2)
								{
									storeSession.EndServerHealthCall();
								}
							}
						}
						finally
						{
							if (StorageGlobals.MapiTestHookAfterCall != null)
							{
								StorageGlobals.MapiTestHookAfterCall(MethodBase.GetCurrentMethod());
							}
						}
					}
				}
				flag = true;
			}
			finally
			{
				if (!flag)
				{
					base.Dispose();
				}
			}
		}

		protected override DisposeTracker GetDisposeTracker()
		{
			return DisposeTracker.Get<RecipientTable>(this);
		}

		protected override void InternalDispose(bool disposing)
		{
			if (disposing && this.recipientTable != null)
			{
				this.recipientTable.Dispose();
			}
			base.InternalDispose(disposing);
		}

		internal bool IsDirty
		{
			get
			{
				return this.recipientChangeTracker.IsDirty;
			}
		}

		internal void Save()
		{
			this.CheckDisposed(null);
			if (this.storeSession == null)
			{
				return;
			}
			this.maxMapiTableRowCount = Math.Max(this.maxMapiTableRowCount, this.GetMapiTableRowCount());
			this.RemoveRecipients();
			this.ModifyRecipients();
			this.AddRecipients();
		}

		internal void BuildRecipientCollection(Action<IList<NativeStorePropertyDefinition>, object[]> recipientCollectionBuilder)
		{
			IList<NativeStorePropertyDefinition> arg = null;
			using (QueryResult recipientProperties = this.GetRecipientProperties(out arg))
			{
				object[][] rows;
				while ((rows = recipientProperties.GetRows(2147483647)).Length > 0)
				{
					foreach (object[] array2 in rows)
					{
						if (array2 != null)
						{
							recipientCollectionBuilder(arg, array2);
						}
					}
				}
			}
		}

		internal bool FindRemovedRecipient(Participant participant, out CoreRecipient recipient)
		{
			return this.recipientChangeTracker.FindRemovedRecipient(participant, out recipient);
		}

		internal IRecipientChangeTracker RecipientChangeTracker
		{
			get
			{
				return this.recipientChangeTracker;
			}
		}

		internal ExTimeZone ExTimeZone
		{
			get
			{
				return this.timeZone;
			}
		}

		private AdrEntry GetAdrEntry(CoreRecipient coreRecipient)
		{
			ICollection<PropertyDefinition> allFoundProperties = coreRecipient.PropertyBag.AllFoundProperties;
			ICollection<PropTag> collection = PropertyTagCache.Cache.PropTagsFromPropertyDefinitions<PropertyDefinition>(this.storeSession.Mailbox.MapiStore, this.storeSession, false, allFoundProperties);
			List<PropValue> list = new List<PropValue>(collection.Count);
			int num = 0;
			using (IEnumerator<PropTag> enumerator = collection.GetEnumerator())
			{
				foreach (PropertyDefinition propertyDefinition in allFoundProperties)
				{
					if (!enumerator.MoveNext())
					{
						throw new InvalidOperationException("PropTag enumerator out of step with definitions");
					}
					object obj = coreRecipient.PropertyBag.TryGetProperty(propertyDefinition);
					object obj2;
					if (obj is ExDateTime)
					{
						obj2 = (DateTime)((ExDateTime)obj).ToUtc();
					}
					else
					{
						obj2 = obj;
					}
					if (!(obj2 is PropertyError))
					{
						list.Add(new PropValue(enumerator.Current, obj2));
					}
					num++;
				}
			}
			return new AdrEntry(list.ToArray());
		}

		private void RemoveRecipients()
		{
			if (this.recipientChangeTracker.RemovedRecipientIds.Count > 0)
			{
				if (this.GetMapiTableRowCount() == this.recipientChangeTracker.RemovedRecipientIds.Count)
				{
					this.ModifyParticipants((ModifyRecipientsFlags)0, RecipientTable.EmptyParticipantArray);
				}
				else
				{
					AdrEntry[] array = new AdrEntry[this.recipientChangeTracker.RemovedRecipientIds.Count];
					int num = 0;
					foreach (int num2 in this.recipientChangeTracker.RemovedRecipientIds)
					{
						array[num++] = new AdrEntry(new PropValue[]
						{
							new PropValue(PropTag.RowId, num2)
						});
					}
					this.ModifyParticipants(ModifyRecipientsFlags.RemoveRecipients, array);
				}
				this.recipientChangeTracker.ClearRemovedRecipients();
			}
		}

		private void AddRecipients()
		{
			int num = this.maxMapiTableRowCount;
			if (this.recipientChangeTracker.AddedRecipients.Count > 0)
			{
				AdrEntry[] array = new AdrEntry[this.recipientChangeTracker.AddedRecipients.Count];
				int num2 = 0;
				foreach (CoreRecipient coreRecipient in this.recipientChangeTracker.AddedRecipients)
				{
					coreRecipient.SetRowId(num++);
					array[num2++] = this.GetAdrEntry(coreRecipient);
					coreRecipient.SetUnchanged();
				}
				this.ModifyParticipants(ModifyRecipientsFlags.AddRecipients, array);
				this.recipientChangeTracker.ClearAddedRecipients();
			}
		}

		private void ModifyRecipients()
		{
			if (this.recipientChangeTracker.ModifiedRecipients.Count > 0)
			{
				AdrEntry[] array = new AdrEntry[this.recipientChangeTracker.ModifiedRecipients.Count];
				for (int i = 0; i < this.recipientChangeTracker.ModifiedRecipients.Count; i++)
				{
					array[i] = this.GetAdrEntry(this.recipientChangeTracker.ModifiedRecipients[i]);
					this.recipientChangeTracker.ModifiedRecipients[i].SetUnchanged();
				}
				this.ModifyParticipants(ModifyRecipientsFlags.ModifyRecipients, array);
				this.recipientChangeTracker.ModifiedRecipients.Clear();
			}
		}

		private void ModifyParticipants(ModifyRecipientsFlags flags, params AdrEntry[] adrEntries)
		{
			ExTraceGlobals.StorageTracer.Information((long)this.GetHashCode(), "RecipientTable::ModifyParticipants.");
			StoreSession storeSession = this.storeSession;
			bool flag = false;
			try
			{
				if (storeSession != null)
				{
					storeSession.BeginMapiCall();
					storeSession.BeginServerHealthCall();
					flag = true;
				}
				if (StorageGlobals.MapiTestHookBeforeCall != null)
				{
					StorageGlobals.MapiTestHookBeforeCall(MethodBase.GetCurrentMethod());
				}
				this.mapiMessage.ModifyRecipients(flags, adrEntries);
			}
			catch (MapiPermanentException ex)
			{
				throw StorageGlobals.TranslateMapiException(ServerStrings.MapiCannotModifyRecipients, ex, storeSession, this, "{0}. MapiException = {1}.", new object[]
				{
					string.Format("RecipientTable::ModifyParticipant. The modify flags = {0}.", flags),
					ex
				});
			}
			catch (MapiRetryableException ex2)
			{
				throw StorageGlobals.TranslateMapiException(ServerStrings.MapiCannotModifyRecipients, ex2, storeSession, this, "{0}. MapiException = {1}.", new object[]
				{
					string.Format("RecipientTable::ModifyParticipant. The modify flags = {0}.", flags),
					ex2
				});
			}
			finally
			{
				try
				{
					if (storeSession != null)
					{
						storeSession.EndMapiCall();
						if (flag)
						{
							storeSession.EndServerHealthCall();
						}
					}
				}
				finally
				{
					if (StorageGlobals.MapiTestHookAfterCall != null)
					{
						StorageGlobals.MapiTestHookAfterCall(MethodBase.GetCurrentMethod());
					}
				}
			}
		}

		private QueryResult GetRecipientProperties(out IList<NativeStorePropertyDefinition> recipientProperties)
		{
			this.CheckDisposed(null);
			bool flag = false;
			NativeStorePropertyDefinition[] recipientPropertyDefinitionsFromMapiTable = this.GetRecipientPropertyDefinitionsFromMapiTable(out flag);
			if (flag)
			{
				this.SetRecipientColumnsAndSortOrderOnMapiTable(recipientPropertyDefinitionsFromMapiTable);
			}
			recipientProperties = recipientPropertyDefinitionsFromMapiTable;
			return new QueryResult(this.recipientTable, recipientPropertyDefinitionsFromMapiTable, null, this.storeSession, false);
		}

		private NativeStorePropertyDefinition[] GetRecipientPropertyDefinitionsFromMapiTable(out bool originalColumnsChanged)
		{
			return InternalSchema.RemoveNullPropertyDefinions(PropertyTagCache.Cache.PropertyDefinitionsFromPropTags(NativeStorePropertyDefinition.TypeCheckingFlag.DoNotCreateInvalidType, this.storeSession.Mailbox.MapiStore, this.storeSession, this.recipientTable.QueryColumns(QueryColumnsFlags.AllColumns)), out originalColumnsChanged);
		}

		private void SetRecipientColumnsAndSortOrderOnMapiTable(NativeStorePropertyDefinition[] recipientPropertyDefinitions)
		{
			ICollection<PropTag> columns = PropertyTagCache.Cache.PropTagsFromPropertyDefinitions(this.mapiMessage, this.storeSession, recipientPropertyDefinitions);
			StoreSession storeSession = this.storeSession;
			bool flag = false;
			try
			{
				if (storeSession != null)
				{
					storeSession.BeginMapiCall();
					storeSession.BeginServerHealthCall();
					flag = true;
				}
				if (StorageGlobals.MapiTestHookBeforeCall != null)
				{
					StorageGlobals.MapiTestHookBeforeCall(MethodBase.GetCurrentMethod());
				}
				this.recipientTable.SetColumns(columns);
				this.recipientTable.SortTable(new SortOrder(PropTag.RowId, SortFlags.Ascend), SortTableFlags.None);
			}
			catch (MapiPermanentException ex)
			{
				throw StorageGlobals.TranslateMapiException(ServerStrings.MapiCannotGetRecipientTable, ex, storeSession, this, "{0}. MapiException = {1}.", new object[]
				{
					string.Format("RecipientTable::Storage.RecipientTable.", new object[0]),
					ex
				});
			}
			catch (MapiRetryableException ex2)
			{
				throw StorageGlobals.TranslateMapiException(ServerStrings.MapiCannotGetRecipientTable, ex2, storeSession, this, "{0}. MapiException = {1}.", new object[]
				{
					string.Format("RecipientTable::Storage.RecipientTable.", new object[0]),
					ex2
				});
			}
			finally
			{
				try
				{
					if (storeSession != null)
					{
						storeSession.EndMapiCall();
						if (flag)
						{
							storeSession.EndServerHealthCall();
						}
					}
				}
				finally
				{
					if (StorageGlobals.MapiTestHookAfterCall != null)
					{
						StorageGlobals.MapiTestHookAfterCall(MethodBase.GetCurrentMethod());
					}
				}
			}
		}

		private int GetMapiTableRowCount()
		{
			StoreSession storeSession = this.storeSession;
			bool flag = false;
			int rowCount;
			try
			{
				if (storeSession != null)
				{
					storeSession.BeginMapiCall();
					storeSession.BeginServerHealthCall();
					flag = true;
				}
				if (StorageGlobals.MapiTestHookBeforeCall != null)
				{
					StorageGlobals.MapiTestHookBeforeCall(MethodBase.GetCurrentMethod());
				}
				rowCount = this.recipientTable.GetRowCount();
			}
			catch (MapiPermanentException ex)
			{
				throw StorageGlobals.TranslateMapiException(ServerStrings.MapiCannotGetRecipientTable, ex, storeSession, this, "{0}. MapiException = {1}.", new object[]
				{
					string.Format("RecipientTable::Storage.RecipientTable.", new object[0]),
					ex
				});
			}
			catch (MapiRetryableException ex2)
			{
				throw StorageGlobals.TranslateMapiException(ServerStrings.MapiCannotGetRecipientTable, ex2, storeSession, this, "{0}. MapiException = {1}.", new object[]
				{
					string.Format("RecipientTable::Storage.RecipientTable.", new object[0]),
					ex2
				});
			}
			finally
			{
				try
				{
					if (storeSession != null)
					{
						storeSession.EndMapiCall();
						if (flag)
						{
							storeSession.EndServerHealthCall();
						}
					}
				}
				finally
				{
					if (StorageGlobals.MapiTestHookAfterCall != null)
					{
						StorageGlobals.MapiTestHookAfterCall(MethodBase.GetCurrentMethod());
					}
				}
			}
			return rowCount;
		}

		private readonly StoreSession storeSession;

		private readonly MapiMessage mapiMessage;

		private readonly MapiTable recipientTable;

		private readonly ExTimeZone timeZone = ExTimeZone.UtcTimeZone;

		private readonly RecipientChangeTracker recipientChangeTracker = new RecipientChangeTracker();

		private static readonly AdrEntry[] EmptyParticipantArray = Array<AdrEntry>.Empty;

		private int maxMapiTableRowCount;
	}
}
