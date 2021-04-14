using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Mapi;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal abstract class SynchronizationUploadContextBase<TMapiCollector> : DisposableObject, ISynchronizationUploadContextBase, IDisposable where TMapiCollector : MapiUnk
	{
		protected SynchronizationUploadContextBase(CoreFolder folder, StorageIcsState initialState)
		{
			Util.ThrowOnNullArgument(folder, "folder");
			this.folder = folder;
			StoreSession session = this.Session;
			bool flag = false;
			try
			{
				if (session != null)
				{
					session.BeginMapiCall();
					session.BeginServerHealthCall();
					flag = true;
				}
				if (StorageGlobals.MapiTestHookBeforeCall != null)
				{
					StorageGlobals.MapiTestHookBeforeCall(MethodBase.GetCurrentMethod());
				}
				this.mapiCollector = this.MapiCreateCollector(initialState);
			}
			catch (MapiPermanentException ex)
			{
				throw StorageGlobals.TranslateMapiException(ServerStrings.CannotCreateCollector(base.GetType()), ex, session, this, "{0}. MapiException = {1}.", new object[]
				{
					string.Format("SynchronizationUploadContext::MapiCreateCollector failed", new object[0]),
					ex
				});
			}
			catch (MapiRetryableException ex2)
			{
				throw StorageGlobals.TranslateMapiException(ServerStrings.CannotCreateCollector(base.GetType()), ex2, session, this, "{0}. MapiException = {1}.", new object[]
				{
					string.Format("SynchronizationUploadContext::MapiCreateCollector failed", new object[0]),
					ex2
				});
			}
			finally
			{
				try
				{
					if (session != null)
					{
						session.EndMapiCall();
						if (flag)
						{
							session.EndServerHealthCall();
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
			this.mapiCollector.AllowWarnings = true;
		}

		public StoreSession Session
		{
			get
			{
				this.CheckDisposed(null);
				return this.folder.Session;
			}
		}

		public StoreObjectId SyncRootFolderId
		{
			get
			{
				this.CheckDisposed(null);
				return this.folder.Id.ObjectId;
			}
		}

		public CoreFolder CoreFolder
		{
			get
			{
				this.CheckDisposed(null);
				return this.folder;
			}
		}

		public void GetCurrentState(ref StorageIcsState currentState)
		{
			this.CheckDisposed(null);
			StoreSession session = this.Session;
			bool flag = false;
			try
			{
				if (session != null)
				{
					session.BeginMapiCall();
					session.BeginServerHealthCall();
					flag = true;
				}
				if (StorageGlobals.MapiTestHookBeforeCall != null)
				{
					StorageGlobals.MapiTestHookBeforeCall(MethodBase.GetCurrentMethod());
				}
				this.MapiGetCurrentState(ref currentState);
			}
			catch (MapiPermanentException ex)
			{
				throw StorageGlobals.TranslateMapiException(ServerStrings.CannotObtainSynchronizationUploadState(typeof(TMapiCollector)), ex, session, this, "{0}. MapiException = {1}.", new object[]
				{
					string.Format("SynchronizationUploadContext::GetCurrentState failed", new object[0]),
					ex
				});
			}
			catch (MapiRetryableException ex2)
			{
				throw StorageGlobals.TranslateMapiException(ServerStrings.CannotObtainSynchronizationUploadState(typeof(TMapiCollector)), ex2, session, this, "{0}. MapiException = {1}.", new object[]
				{
					string.Format("SynchronizationUploadContext::GetCurrentState failed", new object[0]),
					ex2
				});
			}
			finally
			{
				try
				{
					if (session != null)
					{
						session.EndMapiCall();
						if (flag)
						{
							session.EndServerHealthCall();
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

		internal void ImportDeletes(DeleteItemFlags deleteItemFlags, byte[][] sourceKeys)
		{
			this.CheckDisposed(null);
			Util.ThrowOnNullArgument(sourceKeys, "sourceKeys");
			ImportDeletionFlags importDeletionFlags = ((deleteItemFlags & DeleteItemFlags.HardDelete) == DeleteItemFlags.HardDelete) ? ImportDeletionFlags.HardDelete : ImportDeletionFlags.None;
			PropValue[] array = new PropValue[sourceKeys.Length];
			for (int i = 0; i < sourceKeys.Length; i++)
			{
				array[i] = new PropValue(PropTag.SourceKey, sourceKeys[i]);
			}
			StoreSession session = this.Session;
			bool flag = false;
			try
			{
				if (session != null)
				{
					session.BeginMapiCall();
					session.BeginServerHealthCall();
					flag = true;
				}
				if (StorageGlobals.MapiTestHookBeforeCall != null)
				{
					StorageGlobals.MapiTestHookBeforeCall(MethodBase.GetCurrentMethod());
				}
				this.MapiImportDeletes(importDeletionFlags, array);
			}
			catch (MapiPermanentException ex)
			{
				throw StorageGlobals.TranslateMapiException(ServerStrings.CannotImportDeletion, ex, session, this, "{0}. MapiException = {1}.", new object[]
				{
					string.Format("Import of object deletions failed", new object[0]),
					ex
				});
			}
			catch (MapiRetryableException ex2)
			{
				throw StorageGlobals.TranslateMapiException(ServerStrings.CannotImportDeletion, ex2, session, this, "{0}. MapiException = {1}.", new object[]
				{
					string.Format("Import of object deletions failed", new object[0]),
					ex2
				});
			}
			finally
			{
				try
				{
					if (session != null)
					{
						session.EndMapiCall();
						if (flag)
						{
							session.EndServerHealthCall();
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

		protected override void InternalDispose(bool disposing)
		{
			if (disposing)
			{
				Util.DisposeIfPresent(this.mapiCollector);
			}
			base.InternalDispose(disposing);
		}

		protected TMapiCollector MapiCollector
		{
			get
			{
				return this.mapiCollector;
			}
		}

		protected MapiFolder MapiFolder
		{
			get
			{
				return (MapiFolder)CoreObject.GetPersistablePropertyBag(this.folder).MapiProp;
			}
		}

		protected override DisposeTracker GetDisposeTracker()
		{
			return DisposeTracker.Get<SynchronizationUploadContextBase<TMapiCollector>>(this);
		}

		protected List<PropValue> GetPropValuesFromValues(ExTimeZone exTimeZone, IList<PropertyDefinition> propertyDefinitions, IList<object> propertyValues)
		{
			ICollection<PropTag> collection = PropertyTagCache.Cache.PropTagsFromPropertyDefinitions<PropertyDefinition>(this.Session.Mailbox.MapiStore, this.Session, false, propertyDefinitions);
			List<PropValue> list = new List<PropValue>(propertyDefinitions.Count);
			int num = 0;
			foreach (PropTag propTag in collection)
			{
				InternalSchema.CheckPropertyValueType(propertyDefinitions[num], propertyValues[num]);
				list.Add(MapiPropertyBag.GetPropValueFromValue(this.Session, exTimeZone, propTag, propertyValues[num]));
				num++;
			}
			return list;
		}

		protected abstract TMapiCollector MapiCreateCollector(StorageIcsState initialState);

		protected abstract void MapiGetCurrentState(ref StorageIcsState finalState);

		protected abstract void MapiImportDeletes(ImportDeletionFlags importDeletionFlags, PropValue[] sourceKeys);

		private readonly TMapiCollector mapiCollector = default(TMapiCollector);

		private readonly CoreFolder folder;
	}
}
