using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Search;
using Microsoft.Exchange.Inference.MdbCommon;
using Microsoft.Exchange.Search.Core.Abstraction;
using Microsoft.Exchange.Search.Core.Common;
using Microsoft.Exchange.Search.Core.Diagnostics;

namespace Microsoft.Exchange.Inference.Mdb
{
	internal class MdbDocumentAdapter : Disposable, IPersistableDocumentAdapter, IDocumentAdapter
	{
		internal MdbDocumentAdapter(MdbCompositeItemIdentity id, PropertyDefinition[] propertiesToLoad, MailboxSession session, MdbPropertyMap propertyMap) : this(id, propertiesToLoad, null, session, propertyMap, true)
		{
		}

		internal MdbDocumentAdapter(MdbCompositeItemIdentity id, PropertyDefinition[] propertiesToLoad, IItem item, MdbPropertyMap propertyMap) : this(id, propertiesToLoad, item, null, propertyMap, true)
		{
		}

		internal MdbDocumentAdapter(MdbCompositeItemIdentity id, PropertyDefinition[] propertiesToLoad, IItem item, MailboxSession session, MdbPropertyMap propertyMap, bool allowItemBind = true)
		{
			Util.ThrowOnNullArgument(id, "id");
			Util.ThrowOnNullArgument(propertyMap, "propertyMap");
			this.id = id;
			this.propertyMap = propertyMap;
			this.mappedPropertiesToLoadOnBind = MdbDocumentAdapter.GetMappings(this.propertyMap, propertiesToLoad);
			ExAssert.RetailAssert(this.mappedPropertiesToLoadOnBind != null, "Store Properties to load is null");
			this.Item = item;
			this.Session = session;
			if (this.Item == null && this.Session == null)
			{
				throw new ArgumentException("session and item are both null");
			}
			this.shouldDisposeItem = false;
			this.allowItemBind = allowItemBind;
			this.diagnosticsSession = DiagnosticsSession.CreateComponentDiagnosticsSession("MdbDocumentAdapter", ComponentInstance.Globals.Search.ServiceName, ExTraceGlobals.MdbDocumentAdapterTracer, (long)this.GetHashCode());
		}

		internal MdbDocumentAdapter(IDictionary<StorePropertyDefinition, object> preloadedProperties, MdbPropertyMap propertyMap)
		{
			Util.ThrowOnNullOrEmptyArgument<KeyValuePair<StorePropertyDefinition, object>>(preloadedProperties, "preloadedProperties");
			Util.ThrowOnNullArgument(propertyMap, "propertyMap");
			this.preloadedProperties = preloadedProperties;
			this.propertyMap = propertyMap;
			this.diagnosticsSession = DiagnosticsSession.CreateComponentDiagnosticsSession("MdbDocumentAdapter", ComponentInstance.Globals.Search.ServiceName, ExTraceGlobals.MdbDocumentAdapterTracer, (long)this.GetHashCode());
		}

		public bool ContainsPropertyMapping(PropertyDefinition property)
		{
			Util.ThrowOnNullArgument(property, "property");
			return this.propertyMap.ContainsKey(property);
		}

		public bool TryGetProperty(PropertyDefinition property, out object result)
		{
			object property2 = this.GetProperty(property);
			result = property2;
			return property2 != null;
		}

		public object GetProperty(PropertyDefinition property)
		{
			Util.ThrowOnNullArgument(property, "property");
			MdbPropertyMapping mdbMapping;
			if (this.propertyMap.TryGetValue(property, out mdbMapping))
			{
				return this.InternalGetProperty(mdbMapping);
			}
			throw new OperationFailedException(Strings.PropertyMappingFailed(property.ToString()));
		}

		public void SetProperty(PropertyDefinition property, object value)
		{
			Util.ThrowOnNullArgument(property, "property");
			Util.ThrowOnNullArgument(value, "value");
			MdbPropertyMapping mdbPropertyMapping;
			if (!this.propertyMap.TryGetValue(property, out mdbPropertyMapping))
			{
				throw new OperationFailedException(Strings.PropertyMappingFailed(property.ToString()));
			}
			if (mdbPropertyMapping.IsReadOnly)
			{
				throw new OperationFailedException(Strings.SetPropertyFailed(property.ToString()));
			}
			this.InternalSetProperty(mdbPropertyMapping, value);
		}

		public virtual void Save()
		{
			this.Save(true);
		}

		public virtual void Save(bool reload)
		{
			if (this.Item == null)
			{
				throw new OperationFailedException(Strings.SaveWithNoItemError);
			}
			if (this.Item.IsDirty)
			{
				MdbDocumentAdapter.CallXsoAndMapExceptions(this.diagnosticsSession, this.id.MailboxGuid, delegate
				{
					this.Item.Save(SaveMode.ResolveConflicts);
					if (reload)
					{
						this.Item.Load(this.mappedPropertiesToLoadOnBind);
					}
				});
				return;
			}
		}

		internal static IList<PropertyDefinition> GetMappings(MdbPropertyMap map, PropertyDefinition[] propertiesToLoad)
		{
			List<PropertyDefinition> list = new List<PropertyDefinition>();
			if (propertiesToLoad != null)
			{
				list = new List<PropertyDefinition>(propertiesToLoad.Length);
				foreach (PropertyDefinition propertyDefinition in propertiesToLoad)
				{
					MdbPropertyMapping mdbPropertyMapping;
					if (!map.TryGetValue(propertyDefinition, out mdbPropertyMapping))
					{
						throw new KeyNotFoundException(string.Format("Could not get mapping for {0}.", propertyDefinition));
					}
					list.AddRange(mdbPropertyMapping.StorePropertyDefinitions);
				}
			}
			return list;
		}

		internal static object CheckPropertyValue(object propertyValue)
		{
			PropertyError propertyError = propertyValue as PropertyError;
			if (propertyError == null)
			{
				return propertyValue;
			}
			if (propertyError.PropertyErrorCode == PropertyErrorCode.NotFound)
			{
				return null;
			}
			throw PropertyError.ToException(new PropertyError[]
			{
				propertyError
			});
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<MdbDocumentAdapter>(this);
		}

		protected override void InternalDispose(bool calledFromDispose)
		{
			if (calledFromDispose && this.Item != null)
			{
				if (this.shouldDisposeItem)
				{
					this.Item.Dispose();
				}
				this.Item = null;
			}
		}

		private static TReturnValue CallXsoAndMapExceptionsWithReturnValue<TReturnValue>(IDiagnosticsSession tracer, Guid mailboxGuid, MdbDocumentAdapter.CallXsoWithReturnValue<TReturnValue> xsoCall)
		{
			TReturnValue result = default(TReturnValue);
			MdbDocumentAdapter.CallXsoAndMapExceptions(tracer, mailboxGuid, delegate
			{
				result = xsoCall();
			});
			return result;
		}

		private static void CallXsoAndMapExceptions(IDiagnosticsSession tracer, Guid mailboxGuid, MdbDocumentAdapter.CallXso xsoCall)
		{
			try
			{
				xsoCall();
			}
			catch (ConnectionFailedTransientException ex)
			{
				tracer.TraceError<Guid, ConnectionFailedTransientException>("Failed to connect to mailbox {0}, exception: {1}", mailboxGuid, ex);
				throw new ComponentFailedTransientException(Strings.ConnectionToMailboxFailed(mailboxGuid), ex);
			}
			catch (ConnectionFailedPermanentException ex2)
			{
				tracer.TraceError<Guid, ConnectionFailedPermanentException>("Failed to connect to mailbox {0}, exception: {1}", mailboxGuid, ex2);
				throw new ComponentFailedPermanentException(Strings.ConnectionToMailboxFailed(mailboxGuid), ex2);
			}
			catch (ObjectNotFoundException ex3)
			{
				tracer.TraceDebug<Guid, ObjectNotFoundException>("Got exception from XSO (MDB: {0}): {1}", mailboxGuid, ex3);
				throw new DocumentFailureException(ex3);
			}
			catch (CorruptDataException ex4)
			{
				tracer.TraceDebug<Guid, CorruptDataException>("Got exception from XSO (MDB: {0}): {1}", mailboxGuid, ex4);
				throw new DocumentFailureException(ex4);
			}
			catch (PropertyErrorException ex5)
			{
				tracer.TraceDebug<Guid, PropertyErrorException>("Got exception from XSO (MDB: {0}): {1}", mailboxGuid, ex5);
				throw new DocumentFailureException(ex5);
			}
			catch (MailboxUnavailableException ex6)
			{
				tracer.TraceDebug<Guid, MailboxUnavailableException>("Got exception from XSO (MDB: {0}): {1}", mailboxGuid, ex6);
				throw new DocumentFailureException(ex6);
			}
			catch (StoragePermanentException innerException)
			{
				throw new ComponentFailedPermanentException(innerException);
			}
			catch (StorageTransientException innerException2)
			{
				throw new ComponentFailedTransientException(innerException2);
			}
		}

		protected virtual object InternalGetProperty(MdbPropertyMapping mdbMapping)
		{
			object result = null;
			if (this.allowItemBind)
			{
				this.InitializeItemIfNecessary();
				result = MdbDocumentAdapter.CallXsoAndMapExceptionsWithReturnValue<object>(this.diagnosticsSession, this.id.MailboxGuid, () => MdbDocumentAdapter.CheckPropertyValue(mdbMapping.GetPropertyValue(this.Item, new MdbDocumentAdapter.MdbDocAdapterPropertyContext
				{
					MailboxSession = this.Session
				})));
			}
			else if (this.AllPropertiesContained(mdbMapping.StorePropertyDefinitions))
			{
				return MdbDocumentAdapter.CheckPropertyValue(mdbMapping.GetPropertyValue(this.preloadedProperties));
			}
			return result;
		}

		private bool AllPropertiesContained(IEnumerable<StorePropertyDefinition> providerPropertyDefinitions)
		{
			foreach (StorePropertyDefinition key in providerPropertyDefinitions)
			{
				if (!this.preloadedProperties.ContainsKey(key))
				{
					return false;
				}
			}
			return true;
		}

		protected virtual void InternalSetProperty(MdbPropertyMapping mdbMapping, object value)
		{
			if (this.allowItemBind)
			{
				this.InitializeItemIfNecessary();
				MdbDocumentAdapter.CallXsoAndMapExceptions(this.diagnosticsSession, this.id.MailboxGuid, delegate
				{
					mdbMapping.SetPropertyValue(this.Item, value, new MdbDocumentAdapter.MdbDocAdapterPropertyContext
					{
						MailboxSession = this.Session
					});
				});
				if (this.mappedPropertiesToLoadOnBind != null)
				{
					foreach (StorePropertyDefinition item in mdbMapping.StorePropertyDefinitions)
					{
						if (!this.mappedPropertiesToLoadOnBind.Contains(item))
						{
							this.mappedPropertiesToLoadOnBind.Add(item);
						}
					}
					return;
				}
			}
			else
			{
				mdbMapping.SetPropertyValue(this.preloadedProperties, value);
			}
		}

		protected virtual IItem GetItem()
		{
			IItem item = null;
			MdbDocumentAdapter.CallXsoAndMapExceptions(this.diagnosticsSession, this.id.MailboxGuid, delegate
			{
				HashSet<PropertyDefinition> propsToReturn = new HashSet<PropertyDefinition>(this.mappedPropertiesToLoadOnBind);
				item = Microsoft.Exchange.Data.Storage.Item.Bind(this.Session, this.id.ItemId, ItemBindOption.None, propsToReturn);
				item.OpenAsReadWrite();
			});
			return item;
		}

		private void InitializeItemIfNecessary()
		{
			if (this.Item == null)
			{
				this.Item = this.GetItem();
				ExAssert.RetailAssert(this.Item != null, "Item is null");
				this.shouldDisposeItem = true;
			}
		}

		private readonly IDiagnosticsSession diagnosticsSession;

		private readonly bool allowItemBind;

		private readonly MdbPropertyMap propertyMap;

		protected IItem Item;

		private readonly MdbCompositeItemIdentity id;

		protected MailboxSession Session;

		private bool shouldDisposeItem;

		private readonly IList<PropertyDefinition> mappedPropertiesToLoadOnBind;

		private readonly IDictionary<StorePropertyDefinition, object> preloadedProperties;

		internal delegate void CallXso();

		internal delegate TReturnValue CallXsoWithReturnValue<TReturnValue>();

		private class MdbDocAdapterPropertyContext : IMdbPropertyMappingContext
		{
			public MailboxSession MailboxSession { get; internal set; }
		}
	}
}
