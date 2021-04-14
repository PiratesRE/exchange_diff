using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Mapi;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class PropertyTable : DisposableObject, IModifyTable, IDisposable
	{
		internal PropertyTable(CoreFolder coreFolder, NativeStorePropertyDefinition property, ModifyTableOptions options, IModifyTableRestriction modifyTableRestriction)
		{
			Util.ThrowOnNullArgument(coreFolder, "coreFolder");
			Util.ThrowOnNullArgument(property, "property");
			EnumValidator.ThrowIfInvalid<ModifyTableOptions>(options);
			using (DisposeGuard disposeGuard = this.Guard())
			{
				this.mapiModifyTable = PropertyTable.GetMapiModifyTable(coreFolder, property);
				this.session = coreFolder.Session;
				this.propertyReference = this.session.Mailbox.MapiStore;
				this.tableNameForTracing = property.Name;
				this.options = options;
				this.modifyTableRestriction = modifyTableRestriction;
				disposeGuard.Success();
			}
		}

		public void Clear()
		{
			this.CheckDisposed(null);
			this.replaceAllRows = true;
			this.pendingOperations.Clear();
		}

		public void AddRow(params PropValue[] propValues)
		{
			this.CheckDisposed(null);
			this.AddPendingOperation(new PropertyTable.Operation(ModifyTableOperationType.Add, propValues));
		}

		public void ModifyRow(params PropValue[] propValues)
		{
			this.CheckDisposed(null);
			this.AddPendingOperation(new PropertyTable.Operation(ModifyTableOperationType.Modify, propValues));
		}

		public void RemoveRow(params PropValue[] propValues)
		{
			this.CheckDisposed(null);
			this.AddPendingOperation(new PropertyTable.Operation(ModifyTableOperationType.Remove, propValues));
		}

		public IQueryResult GetQueryResult(QueryFilter queryFilter, ICollection<PropertyDefinition> columns)
		{
			this.CheckDisposed(null);
			Util.ThrowOnNullArgument(columns, "columns");
			GetTableFlags getTableFlags = GetTableFlags.None;
			if ((this.options & ModifyTableOptions.FreeBusyAware) == ModifyTableOptions.FreeBusyAware)
			{
				getTableFlags |= GetTableFlags.FreeBusy;
			}
			IQueryResult result;
			using (DisposeGuard disposeGuard = default(DisposeGuard))
			{
				MapiTable mapiTable = null;
				StoreSession storeSession = this.session;
				object thisObject = null;
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
					mapiTable = this.mapiModifyTable.GetTable(getTableFlags);
				}
				catch (MapiPermanentException ex)
				{
					throw StorageGlobals.TranslateMapiException(ServerStrings.MapiCannotGetMapiTable, ex, storeSession, thisObject, "{0}. MapiException = {1}.", new object[]
					{
						string.Format("PropertyTable.GetQueryResult. Failed to get the MapiTable from the MapiModifyTable.", new object[0]),
						ex
					});
				}
				catch (MapiRetryableException ex2)
				{
					throw StorageGlobals.TranslateMapiException(ServerStrings.MapiCannotGetMapiTable, ex2, storeSession, thisObject, "{0}. MapiException = {1}.", new object[]
					{
						string.Format("PropertyTable.GetQueryResult. Failed to get the MapiTable from the MapiModifyTable.", new object[0]),
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
				disposeGuard.Add<MapiTable>(mapiTable);
				QueryExecutor.SetTableFilter(this.session, this.propertyReference, mapiTable, queryFilter);
				QueryResult queryResult = new QueryResult(mapiTable, columns, null, this.session, true);
				disposeGuard.Success();
				result = queryResult;
			}
			return result;
		}

		public void ApplyPendingChanges()
		{
			this.CheckDisposed(null);
			ModifyTableFlags modifyTableFlags = ModifyTableFlags.None;
			if ((this.options & ModifyTableOptions.FreeBusyAware) == ModifyTableOptions.FreeBusyAware)
			{
				modifyTableFlags |= ModifyTableFlags.FreeBusy;
			}
			if (this.replaceAllRows)
			{
				modifyTableFlags |= ModifyTableFlags.RowListReplace;
			}
			ICollection<RowEntry> rowList = from operation in this.pendingOperations
			select operation.ToRowEntry(this.session, this.propertyReference);
			this.EnforceRestriction(this.pendingOperations);
			StoreSession storeSession = this.session;
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
				this.mapiModifyTable.ModifyTable(modifyTableFlags, rowList);
			}
			catch (MapiPermanentException ex)
			{
				throw StorageGlobals.TranslateMapiException(ServerStrings.MapiCannotModifyPropertyTable(this.tableNameForTracing), ex, storeSession, this, "{0}. MapiException = {1}.", new object[]
				{
					string.Format("PropertyTable::Modify. Unable to modify a table.", new object[0]),
					ex
				});
			}
			catch (MapiRetryableException ex2)
			{
				throw StorageGlobals.TranslateMapiException(ServerStrings.MapiCannotModifyPropertyTable(this.tableNameForTracing), ex2, storeSession, this, "{0}. MapiException = {1}.", new object[]
				{
					string.Format("PropertyTable::Modify. Unable to modify a table.", new object[0]),
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
			this.pendingOperations.Clear();
			this.replaceAllRows = false;
		}

		public void SuppressRestriction()
		{
			this.CheckDisposed(null);
			this.propertyTableRestrictionSuppressed = true;
		}

		public StoreSession Session
		{
			get
			{
				this.CheckDisposed(null);
				return this.session;
			}
		}

		protected override DisposeTracker GetDisposeTracker()
		{
			return DisposeTracker.Get<PropertyTable>(this);
		}

		protected override void InternalDispose(bool disposing)
		{
			if (disposing)
			{
				Util.DisposeIfPresent(this.mapiModifyTable);
			}
			base.InternalDispose(disposing);
		}

		private static MapiModifyTable GetMapiModifyTable(CoreFolder coreFolder, NativeStorePropertyDefinition property)
		{
			PersistablePropertyBag persistablePropertyBag = CoreObject.GetPersistablePropertyBag(coreFolder);
			PropTag propTag = PropertyTagCache.Cache.PropTagFromPropertyDefinition(persistablePropertyBag.MapiProp, coreFolder.Session, property);
			StoreSession storeSession = coreFolder.Session;
			object thisObject = null;
			bool flag = false;
			MapiModifyTable result;
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
				result = (MapiModifyTable)persistablePropertyBag.MapiProp.OpenProperty(propTag, PropertyTable.IExchangeModifyTableInterfaceId, 0, OpenPropertyFlags.DeferredErrors);
			}
			catch (MapiPermanentException ex)
			{
				throw StorageGlobals.TranslateMapiException(ServerStrings.MapiCannotGetMapiTable, ex, storeSession, thisObject, "{0}. MapiException = {1}.", new object[]
				{
					string.Format("PropertyTable.GetMapiModifyTable. Unable to get MapiModifyTable.", new object[0]),
					ex
				});
			}
			catch (MapiRetryableException ex2)
			{
				throw StorageGlobals.TranslateMapiException(ServerStrings.MapiCannotGetMapiTable, ex2, storeSession, thisObject, "{0}. MapiException = {1}.", new object[]
				{
					string.Format("PropertyTable.GetMapiModifyTable. Unable to get MapiModifyTable.", new object[0]),
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
			return result;
		}

		private void AddPendingOperation(PropertyTable.Operation pendingOperation)
		{
			this.pendingOperations.Add(pendingOperation);
		}

		private void EnforceRestriction(ICollection<PropertyTable.Operation> operations)
		{
			if (!this.propertyTableRestrictionSuppressed && this.modifyTableRestriction != null)
			{
				this.modifyTableRestriction.Enforce(this, from operation in operations
				select operation);
			}
		}

		private static readonly Guid IExchangeModifyTableInterfaceId = new Guid("2d734cb0-53fd-101b-b19d-08002b3056e3");

		private readonly StoreSession session;

		private readonly MapiProp propertyReference;

		private readonly MapiModifyTable mapiModifyTable;

		private readonly string tableNameForTracing;

		private readonly ModifyTableOptions options;

		private readonly IModifyTableRestriction modifyTableRestriction;

		private readonly ICollection<PropertyTable.Operation> pendingOperations = new List<PropertyTable.Operation>();

		private bool replaceAllRows;

		private bool propertyTableRestrictionSuppressed;

		private sealed class Operation : ModifyTableOperation
		{
			internal Operation(ModifyTableOperationType operationType, PropValue[] propValues) : base(operationType, propValues)
			{
			}

			internal RowEntry ToRowEntry(StoreSession session, MapiProp propertyMappingReference)
			{
				PropertyTable.Operation.MapiRowFactory mapiRowFactory = null;
				switch (base.Operation)
				{
				case ModifyTableOperationType.Add:
					mapiRowFactory = new PropertyTable.Operation.MapiRowFactory(RowEntry.Add);
					break;
				case ModifyTableOperationType.Modify:
					mapiRowFactory = new PropertyTable.Operation.MapiRowFactory(RowEntry.Modify);
					break;
				case ModifyTableOperationType.Remove:
					mapiRowFactory = new PropertyTable.Operation.MapiRowFactory(RowEntry.Remove);
					break;
				}
				ICollection<PropTag> collection = PropertyTagCache.Cache.PropTagsFromPropertyDefinitions(propertyMappingReference, session, from propValue in base.Properties
				select (NativeStorePropertyDefinition)propValue.Property);
				PropValue[] array = new PropValue[base.Properties.Length];
				int num = 0;
				foreach (PropTag propTag in collection)
				{
					array[num] = MapiPropertyBag.GetPropValueFromValue(session, session.ExTimeZone, propTag, base.Properties[num].Value);
					num++;
				}
				return mapiRowFactory(array);
			}

			internal delegate RowEntry MapiRowFactory(ICollection<PropValue> propValues);
		}
	}
}
