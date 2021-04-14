using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Mapi;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal abstract class ManifestProviderBase<TMapiManifest, TPhase> : DisposableObject where TMapiManifest : MapiUnk where TPhase : struct
	{
		protected ManifestProviderBase(CoreFolder folder, ManifestConfigFlags flags, QueryFilter filter, StorageIcsState initialState, PropertyDefinition[] includeProperties, PropertyDefinition[] excludeProperties)
		{
			this.folder = folder;
			this.disposeTracker = this.GetDisposeTracker();
			bool flag = false;
			try
			{
				PersistablePropertyBag persistablePropertyBag = CoreObject.GetPersistablePropertyBag(this.folder);
				Restriction restriction = null;
				if (filter != null)
				{
					restriction = FilterRestrictionConverter.CreateRestriction(folder.Session, persistablePropertyBag.ExTimeZone, persistablePropertyBag.MapiProp, filter);
				}
				ICollection<PropTag> includePropertyTags = PropertyTagCache.Cache.PropTagsFromPropertyDefinitions<PropertyDefinition>(this.MapiFolder, this.Session, true, includeProperties);
				ICollection<PropTag> excludePropertyTags = PropertyTagCache.Cache.PropTagsFromPropertyDefinitions<PropertyDefinition>(this.MapiFolder, this.Session, true, excludeProperties);
				StoreSession session = this.Session;
				object thisObject = this.folder;
				bool flag2 = false;
				try
				{
					if (session != null)
					{
						session.BeginMapiCall();
						session.BeginServerHealthCall();
						flag2 = true;
					}
					if (StorageGlobals.MapiTestHookBeforeCall != null)
					{
						StorageGlobals.MapiTestHookBeforeCall(MethodBase.GetCurrentMethod());
					}
					this.mapiManifest = this.MapiCreateManifest(flags, restriction, initialState, includePropertyTags, excludePropertyTags);
				}
				catch (MapiPermanentException ex)
				{
					throw StorageGlobals.TranslateMapiException(ServerStrings.CannotCreateManifestEx(base.GetType()), ex, session, thisObject, "{0}. MapiException = {1}.", new object[]
					{
						string.Format("ManifestProviderBase..ctor. Failed to create/configure HierarchyManifestEx.", new object[0]),
						ex
					});
				}
				catch (MapiRetryableException ex2)
				{
					throw StorageGlobals.TranslateMapiException(ServerStrings.CannotCreateManifestEx(base.GetType()), ex2, session, thisObject, "{0}. MapiException = {1}.", new object[]
					{
						string.Format("ManifestProviderBase..ctor. Failed to create/configure HierarchyManifestEx.", new object[0]),
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
							if (flag2)
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

		public void GetFinalState(ref StorageIcsState finalState)
		{
			this.CheckDisposed(null);
			if (this.nextChange == null && !this.finalStateReturned)
			{
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
					this.MapiGetFinalState(ref finalState);
				}
				catch (MapiPermanentException ex)
				{
					throw StorageGlobals.TranslateMapiException(ServerStrings.CannotSynchronizeManifestEx(typeof(TMapiManifest), this.clientPhase, this.manifestPhase), ex, session, this, "{0}. MapiException = {1}.", new object[]
					{
						string.Format("ManifestProviderBase::GetFinalState failed", new object[0]),
						ex
					});
				}
				catch (MapiRetryableException ex2)
				{
					throw StorageGlobals.TranslateMapiException(ServerStrings.CannotSynchronizeManifestEx(typeof(TMapiManifest), this.clientPhase, this.manifestPhase), ex2, session, this, "{0}. MapiException = {1}.", new object[]
					{
						string.Format("ManifestProviderBase::GetFinalState failed", new object[0]),
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
				this.finalStateReturned = true;
				return;
			}
			throw new InvalidOperationException("Consumers cannot get a final state twice or until they consumed all changes.");
		}

		protected static void TranslateFlag(ManifestConfigFlags sourceFlag, SyncConfigFlags destinationFlag, ManifestConfigFlags sourceFlags, ref SyncConfigFlags destinationFlags)
		{
			if ((sourceFlags & sourceFlag) == sourceFlag)
			{
				destinationFlags |= destinationFlag;
				return;
			}
			destinationFlags &= ~destinationFlag;
		}

		protected StoreSession Session
		{
			get
			{
				return this.folder.Session;
			}
		}

		protected TMapiManifest MapiManifest
		{
			get
			{
				return this.mapiManifest;
			}
		}

		protected MapiFolder MapiFolder
		{
			get
			{
				return (MapiFolder)CoreObject.GetPersistablePropertyBag(this.folder).MapiProp;
			}
		}

		protected override void InternalDispose(bool disposing)
		{
			if (disposing)
			{
				Util.DisposeIfPresent(this.mapiManifest);
				Util.DisposeIfPresent(this.disposeTracker);
			}
			base.InternalDispose(disposing);
		}

		protected PropValue[] FromMapiPropValueToXsoPropValue(PropValue[] propValues)
		{
			PropTag[] array = new PropTag[propValues.Length];
			for (int i = 0; i < propValues.Length; i++)
			{
				array[i] = propValues[i].PropTag;
			}
			NativeStorePropertyDefinition[] array2 = PropertyTagCache.Cache.PropertyDefinitionsFromPropTags(NativeStorePropertyDefinition.TypeCheckingFlag.DisableTypeCheck, this.MapiFolder, this.Session, array);
			PropValue[] array3 = new PropValue[propValues.Length];
			for (int j = 0; j < array2.Length; j++)
			{
				if (array2[j] == null)
				{
					throw new NotSupportedException(string.Format("The property tag cannot be resolved to a property definition. PropertyTag = {0}", array[j]));
				}
				object valueFromPropValue = MapiPropertyBag.GetValueFromPropValue(this.Session, CoreObject.GetPersistablePropertyBag(this.folder).ExTimeZone, array2[j], propValues[j]);
				array3[j] = new PropValue(array2[j], valueFromPropValue);
			}
			return array3;
		}

		protected abstract bool IsValidTransition(TPhase oldPhase, TPhase newPhase);

		protected abstract TMapiManifest MapiCreateManifest(ManifestConfigFlags flags, Restriction restriction, StorageIcsState initialState, ICollection<PropTag> includePropertyTags, ICollection<PropTag> excludePropertyTags);

		protected abstract void MapiGetFinalState(ref StorageIcsState finalState);

		protected abstract ManifestStatus MapiSynchronize();

		protected void SetChange(TPhase newPhase, ManifestChangeBase changeBase)
		{
			if (this.nextChange != null)
			{
				throw new InvalidOperationException("The change should have been retrieved by the consumer.");
			}
			this.nextChange = changeBase;
			this.SetPhase(ref this.manifestPhase, newPhase);
		}

		protected bool TryGetChange<T>(TPhase newClientPhase, out T change) where T : ManifestChangeBase
		{
			if (this.finalStateReturned)
			{
				throw new InvalidOperationException("Final state has been returned. No other data can be requested");
			}
			this.CacheNextChangeFromMapi();
			if (this.nextChange != null && !newClientPhase.Equals(this.manifestPhase) && !this.IsValidTransition(newClientPhase, this.manifestPhase))
			{
				throw new InvalidOperationException(string.Format("Client phase cannot be advanced from {0} to {1} when manifest phase is {2}", this.clientPhase, newClientPhase, this.manifestPhase));
			}
			this.SetPhase(ref this.clientPhase, newClientPhase);
			if (this.clientPhase.Equals(this.manifestPhase))
			{
				change = (T)((object)Interlocked.Exchange<ManifestChangeBase>(ref this.nextChange, null));
				return change != null;
			}
			change = default(T);
			return false;
		}

		private void CacheNextChangeFromMapi()
		{
			if (this.nextChange != null || this.noMoreData)
			{
				return;
			}
			StoreSession session = this.folder.Session;
			bool flag = false;
			ManifestStatus manifestStatus;
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
				manifestStatus = this.MapiSynchronize();
			}
			catch (MapiPermanentException ex)
			{
				throw StorageGlobals.TranslateMapiException(ServerStrings.CannotSynchronizeManifestEx(typeof(TMapiManifest), this.clientPhase, this.manifestPhase), ex, session, this, "{0}. MapiException = {1}.", new object[]
				{
					string.Format("ManifestProviderBase.CacheNextChangeFromMapi. Call to Synchronize failed.", new object[0]),
					ex
				});
			}
			catch (MapiRetryableException ex2)
			{
				throw StorageGlobals.TranslateMapiException(ServerStrings.CannotSynchronizeManifestEx(typeof(TMapiManifest), this.clientPhase, this.manifestPhase), ex2, session, this, "{0}. MapiException = {1}.", new object[]
				{
					string.Format("ManifestProviderBase.CacheNextChangeFromMapi. Call to Synchronize failed.", new object[0]),
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
			switch (manifestStatus)
			{
			case ManifestStatus.Done:
				this.noMoreData = true;
				return;
			case ManifestStatus.Yielded:
				if (this.nextChange == null)
				{
					throw new InvalidOperationException("Mapi reported that a callback was called, but no new changes got recorded");
				}
				return;
			}
			throw new InvalidOperationException(string.Format("By design, we should continue until we are done the whole changes. Status = {0}.", manifestStatus));
		}

		private void SetPhase(ref TPhase phase, TPhase newPhase)
		{
			if (!this.IsValidTransition(phase, newPhase))
			{
				throw new InvalidOperationException(string.Format("Change of phases from {0} to {1} is not supported", phase, newPhase));
			}
			phase = newPhase;
		}

		private readonly TMapiManifest mapiManifest;

		private readonly CoreFolder folder;

		private readonly DisposeTracker disposeTracker;

		private ManifestChangeBase nextChange;

		private TPhase manifestPhase;

		private TPhase clientPhase;

		private bool noMoreData;

		private bool finalStateReturned;
	}
}
