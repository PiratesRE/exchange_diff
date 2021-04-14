using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using Microsoft.Exchange.Collections;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Data.Storage;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Mapi;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal sealed class AcrPropertyBag : PersistablePropertyBag
	{
		internal AcrPropertyBag(PersistablePropertyBag propertyBag, AcrProfile profile, StoreObjectId itemId, IPropertyBagFactory propertyBagFactory, byte[] openChangeKey)
		{
			this.propertyBag = propertyBag;
			base.PrefetchPropertyArray = this.propertyBag.PrefetchPropertyArray;
			this.profile = profile;
			this.itemId = itemId;
			this.propertyBagFactory = propertyBagFactory;
			this.openChangeKey = openChangeKey;
			if (propertyBag.DisposeTracker != null)
			{
				propertyBag.DisposeTracker.AddExtraDataWithStackTrace("AcrPropertyBag owns PersistablePropertyBag propertyBag at");
			}
		}

		protected override void InternalDispose(bool disposing)
		{
			base.InternalDispose(disposing);
			if (this.propertyBag != null && this.propertyBag.DisposeTracker != null)
			{
				this.propertyBag.DisposeTracker.AddExtraDataWithStackTrace(string.Format(CultureInfo.InvariantCulture, "AcrPropertyBag.InternalDispose({0}) called with stack", new object[]
				{
					disposing
				}));
			}
			if (disposing)
			{
				this.propertyBag.Dispose();
			}
			this.propertyBag = null;
			this.propertyTrackingCache = null;
			this.propertiesWrittenAsStream = null;
		}

		public void OpenAsReadWrite()
		{
			base.CheckDisposed("PropertyBag::OpenAsReadWrite()");
			ExTraceGlobals.StorageTracer.Information<int>((long)this.GetHashCode(), "AcrPropertyBag::OpenAsReadWrite HashCode = {0}.", this.GetHashCode());
			if (this.Mode != AcrPropertyBag.AcrMode.ReadOnly)
			{
				ExTraceGlobals.StorageTracer.Information<int>((long)this.GetHashCode(), "AcrPropertyBag::Reopen HashCode = {0}. Reopen called when Item was already opened with ItemId.", this.GetHashCode());
				return;
			}
			object obj = this.propertyBag.TryGetProperty(InternalSchema.ChangeKey);
			if (PropertyError.IsPropertyNotFound(obj))
			{
				obj = new byte[]
				{
					1
				};
			}
			if (obj is byte[])
			{
				this.openChangeKey = (this.currentChangeKey = (byte[])obj);
				this.acrModeHint = AcrPropertyBag.AcrMode.Passive;
			}
		}

		public override DisposeTracker GetDisposeTracker()
		{
			return DisposeTracker.Get<AcrPropertyBag>(this);
		}

		protected override void SetValidatedStoreProperty(StorePropertyDefinition propertyDefinition, object propertyValue)
		{
			base.CheckDisposed("SetValidatedStoreProperty");
			if (propertyValue == null)
			{
				ExTraceGlobals.StorageTracer.TraceError<int>((long)this.GetHashCode(), "AcrPropertyBag::SetProperties {0}, propertyValues passed as null", this.GetHashCode());
				throw new ArgumentNullException(ServerStrings.ExNullParameter("propertyValue", 2));
			}
			switch (this.Mode)
			{
			case AcrPropertyBag.AcrMode.ReadOnly:
				ExTraceGlobals.StorageTracer.TraceError<int>((long)this.GetHashCode(), "AcrPropertyBag::SetProperties {0}, SetProperties called for readonly AcrPropertyBag", this.GetHashCode());
				throw new AccessDeniedException(ServerStrings.ExItemIsOpenedInReadOnlyMode);
			case AcrPropertyBag.AcrMode.Active:
			case AcrPropertyBag.AcrMode.Passive:
				this.ModifyPropertyWithBookKeeping(propertyDefinition, propertyValue);
				return;
			case (AcrPropertyBag.AcrMode)4:
				return;
			case AcrPropertyBag.AcrMode.NewItem:
				((IDirectPropertyBag)this.propertyBag).SetValue(propertyDefinition, propertyValue);
				return;
			default:
				return;
			}
		}

		protected override object TryGetStoreProperty(StorePropertyDefinition propertyDefinition)
		{
			return ((IDirectPropertyBag)this.propertyBag).GetValue(propertyDefinition);
		}

		public override void Load(ICollection<PropertyDefinition> properties)
		{
			this.propertyBag.Load(properties);
		}

		public override void Clear()
		{
			this.propertyBag.Clear();
		}

		internal override ICollection<NativeStorePropertyDefinition> AllNativeProperties
		{
			get
			{
				return this.propertyBag.AllNativeProperties;
			}
		}

		public override bool HasAllPropertiesLoaded
		{
			get
			{
				base.CheckDisposed("HasAllPropertiesLoaded::get");
				return this.propertyBag.HasAllPropertiesLoaded;
			}
		}

		public override bool CanIgnoreUnchangedProperties
		{
			get
			{
				base.CheckDisposed("CanIgnoreUnchangedProperties::get");
				return this.propertyBag.CanIgnoreUnchangedProperties;
			}
		}

		private void SetOriginalProperty(PropertyDefinition propertyDefinition)
		{
			Dictionary<PropertyDefinition, PropertyDefinition> dictionary = new Dictionary<PropertyDefinition, PropertyDefinition>();
			List<PropertyDefinition> list = new List<PropertyDefinition>();
			if (this.profile[propertyDefinition] != null && (this.profile[propertyDefinition].RequireChangeTracking || this.Mode == AcrPropertyBag.AcrMode.Active))
			{
				foreach (PropertyDefinition propertyDefinition2 in this.profile[propertyDefinition].AllProperties)
				{
					if ((!this.propertyTrackingCache.ContainsKey(propertyDefinition2) || this.propertyTrackingCache[propertyDefinition2].OriginalValue == null) && !dictionary.ContainsKey(propertyDefinition2))
					{
						dictionary.Add(propertyDefinition2, propertyDefinition2);
						list.Add(propertyDefinition2);
					}
				}
			}
			else if (this.Mode == AcrPropertyBag.AcrMode.Active && !dictionary.ContainsKey(propertyDefinition))
			{
				dictionary.Add(propertyDefinition, propertyDefinition);
				list.Add(propertyDefinition);
			}
			if (list.Count > 0)
			{
				this.propertyBag.Load(list);
				foreach (PropertyDefinition propertyDefinition3 in list)
				{
					object obj = this.propertyBag.TryGetProperty(propertyDefinition3);
					switch (this.Mode)
					{
					case AcrPropertyBag.AcrMode.Active:
						if (!this.propertyTrackingCache.ContainsKey(propertyDefinition3))
						{
							this.propertyTrackingCache.Add(propertyDefinition3, new AcrPropertyBag.TrackingInfo(false, obj, null));
						}
						break;
					case AcrPropertyBag.AcrMode.Passive:
						if (!this.propertyTrackingCache.ContainsKey(propertyDefinition3))
						{
							this.propertyTrackingCache.Add(propertyDefinition3, new AcrPropertyBag.TrackingInfo(false, null, obj));
						}
						break;
					}
				}
			}
		}

		private void MarkPropertyAsDirty(PropertyDefinition propertyDefinition)
		{
			if (this.propertyTrackingCache.ContainsKey(propertyDefinition))
			{
				this.propertyTrackingCache[propertyDefinition].Dirty = true;
				return;
			}
			this.propertyTrackingCache.Add(propertyDefinition, new AcrPropertyBag.TrackingInfo(true));
		}

		private void ModifyPropertyWithBookKeeping(StorePropertyDefinition propertyDefinition, object propertyValue)
		{
			if (!this.propertiesWrittenAsStream.ContainsKey(propertyDefinition))
			{
				this.SetOriginalProperty(propertyDefinition);
			}
			if (propertyValue == null)
			{
				this.propertyBag.Delete(propertyDefinition);
			}
			else
			{
				((IDirectPropertyBag)this.propertyBag).SetValue(propertyDefinition, propertyValue);
			}
			this.MarkPropertyAsDirty(propertyDefinition);
		}

		protected override void DeleteStoreProperty(StorePropertyDefinition propertyDefinition)
		{
			base.CheckDisposed("DeleteStoreProperty");
			if (propertyDefinition == null)
			{
				ExTraceGlobals.StorageTracer.TraceError<int>((long)this.GetHashCode(), "AcrPropertyBag::Delete {0}, propertyDefinition passed as null", this.GetHashCode());
				throw new ArgumentNullException(ServerStrings.ExNullParameter("propertyDefinition", 1));
			}
			switch (this.Mode)
			{
			case AcrPropertyBag.AcrMode.ReadOnly:
				ExTraceGlobals.StorageTracer.TraceError<int>((long)this.GetHashCode(), "AcrPropertyBag::Delete {0}, Delete called for readonly AcrPropertyBag", this.GetHashCode());
				throw new AccessDeniedException(ServerStrings.ExItemIsOpenedInReadOnlyMode);
			case AcrPropertyBag.AcrMode.Active:
			case AcrPropertyBag.AcrMode.Passive:
				this.ModifyPropertyWithBookKeeping(propertyDefinition, null);
				return;
			case (AcrPropertyBag.AcrMode)4:
				return;
			case AcrPropertyBag.AcrMode.NewItem:
				((IDirectPropertyBag)this.propertyBag).Delete(propertyDefinition);
				return;
			default:
				return;
			}
		}

		public override Stream OpenPropertyStream(PropertyDefinition propertyDefinition, PropertyOpenMode openMode)
		{
			base.CheckDisposed("AcrPropertyBag::OpenPropertyStream");
			EnumValidator.AssertValid<PropertyOpenMode>(openMode);
			if (openMode != PropertyOpenMode.ReadOnly)
			{
				if (this.Mode == AcrPropertyBag.AcrMode.ReadOnly)
				{
					ExTraceGlobals.StorageTracer.TraceError<int, PropertyOpenMode>((long)this.GetHashCode(), "AcrPropertyBag::SetProperties {0}, OpenPropertyStream called for readonly AcrPropertyBag with opemMode = {1}", this.GetHashCode(), openMode);
					throw new AccessDeniedException(ServerStrings.ExItemIsOpenedInReadOnlyMode);
				}
				if (!this.propertiesWrittenAsStream.ContainsKey(propertyDefinition))
				{
					if (this.propertyTrackingCache.ContainsKey(propertyDefinition))
					{
						ExTraceGlobals.StorageTracer.Information<int, string>((long)this.GetHashCode(), "AcrPropertyBag::OpenPropertyStream HashCode[{0}]: PropertyRemoved from acr {1}", this.GetHashCode(), propertyDefinition.Name);
						this.propertyTrackingCache.Remove(propertyDefinition);
					}
					this.propertiesWrittenAsStream.Add(propertyDefinition, propertyDefinition);
				}
			}
			return this.propertyBag.OpenPropertyStream(propertyDefinition, openMode);
		}

		public override PropertyValueTrackingData GetOriginalPropertyInformation(PropertyDefinition propertyDefinition)
		{
			return this.propertyBag.GetOriginalPropertyInformation(propertyDefinition);
		}

		internal void SetIrresolvableChange()
		{
			this.irresolvableChanges = true;
		}

		public override ICollection<PropertyDefinition> AllFoundProperties
		{
			get
			{
				base.CheckDisposed("AcrPropertyBag::AllFoundProperties");
				return this.propertyBag.AllFoundProperties;
			}
		}

		public override void Reload()
		{
			this.propertyBag.Reload();
			this.propertyTrackingCache.Clear();
			this.propertiesWrittenAsStream.Clear();
		}

		private Dictionary<PropertyDefinition, AcrPropertyProfile.ValuesToResolve> GetValuesToResolve(PropertyBag acrPropertyBag)
		{
			HashSet<PropertyDefinition> hashSet = new HashSet<PropertyDefinition>(this.propertyTrackingCache.Count);
			foreach (KeyValuePair<PropertyDefinition, AcrPropertyBag.TrackingInfo> keyValuePair in this.propertyTrackingCache)
			{
				if (keyValuePair.Value.Dirty)
				{
					foreach (PropertyDefinition item in this.profile.GetPropertiesNeededForResolution(keyValuePair.Key))
					{
						hashSet.TryAdd(item);
					}
				}
			}
			Dictionary<PropertyDefinition, AcrPropertyProfile.ValuesToResolve> dictionary = new Dictionary<PropertyDefinition, AcrPropertyProfile.ValuesToResolve>();
			if (hashSet.Count > 0)
			{
				this.propertyBag.Load(hashSet);
				acrPropertyBag.Load(hashSet);
				foreach (PropertyDefinition propertyDefinition in hashSet)
				{
					AcrPropertyBag.TrackingInfo trackingInfo;
					this.propertyTrackingCache.TryGetValue(propertyDefinition, out trackingInfo);
					object clientValue = this.propertyBag.TryGetProperty(propertyDefinition);
					if (acrPropertyBag == this.propertyBag)
					{
						dictionary.Add(propertyDefinition, new AcrPropertyProfile.ValuesToResolve(clientValue, trackingInfo.ServerValue, trackingInfo.OriginalValue));
					}
					else
					{
						object serverValue = acrPropertyBag.TryGetProperty(propertyDefinition);
						dictionary.Add(propertyDefinition, new AcrPropertyProfile.ValuesToResolve(clientValue, serverValue, (trackingInfo != null) ? trackingInfo.OriginalValue : null));
					}
				}
			}
			return dictionary;
		}

		private ConflictResolutionResult ApplyAcr(PropertyBag acrPropBag, SaveMode saveMode)
		{
			Dictionary<PropertyDefinition, AcrPropertyProfile.ValuesToResolve> valuesToResolve = this.GetValuesToResolve(acrPropBag);
			string valueOrDefault = this.PropertyBag.GetValueOrDefault<string>(InternalSchema.ItemClass, string.Empty);
			if (ObjectClass.IsCalendarItemCalendarItemOccurrenceOrRecurrenceException(valueOrDefault) || ObjectClass.IsMeetingMessage(valueOrDefault))
			{
				LocationIdentifierHelper locationIdentifierHelper = new LocationIdentifierHelper();
				AcrPropertyProfile.ValuesToResolve valuesToResolve2;
				object serverValue;
				if (valuesToResolve.TryGetValue(InternalSchema.ChangeList, out valuesToResolve2))
				{
					locationIdentifierHelper.ChangeBuffer = (byte[])valuesToResolve2.ClientValue;
					serverValue = valuesToResolve2.ServerValue;
				}
				else
				{
					serverValue = new PropertyError(InternalSchema.ChangeList, PropertyErrorCode.NotFound);
				}
				locationIdentifierHelper.SetLocationIdentifier(53909U, LastChangeAction.AcrPerformed);
				valuesToResolve2 = new AcrPropertyProfile.ValuesToResolve(locationIdentifierHelper.ChangeBuffer, serverValue, null);
				valuesToResolve[InternalSchema.ChangeList] = valuesToResolve2;
			}
			ConflictResolutionResult conflictResolutionResult = this.profile.ResolveConflicts(valuesToResolve);
			if (this.propertiesWrittenAsStream.Count > 0)
			{
				List<PropertyConflict> list = new List<PropertyConflict>(conflictResolutionResult.PropertyConflicts);
				foreach (PropertyDefinition propertyDefinition in this.propertiesWrittenAsStream.Keys)
				{
					PropertyConflict item = new PropertyConflict(propertyDefinition, null, null, null, null, false);
					list.Add(item);
				}
				conflictResolutionResult = new ConflictResolutionResult(SaveResult.IrresolvableConflict, list.ToArray());
			}
			if (this.irresolvableChanges || saveMode == SaveMode.FailOnAnyConflict)
			{
				conflictResolutionResult = new ConflictResolutionResult(SaveResult.IrresolvableConflict, conflictResolutionResult.PropertyConflicts);
			}
			if (conflictResolutionResult.SaveStatus != SaveResult.IrresolvableConflict)
			{
				List<PropertyDefinition> list2 = new List<PropertyDefinition>();
				List<PropertyDefinition> list3 = new List<PropertyDefinition>();
				List<object> list4 = new List<object>();
				if (this.propertyBag == acrPropBag)
				{
					foreach (PropertyConflict propertyConflict in conflictResolutionResult.PropertyConflicts)
					{
						if (propertyConflict.ResolvedValue is PropertyError)
						{
							if (PropertyError.IsPropertyNotFound(propertyConflict.ResolvedValue) && (!PropertyError.IsPropertyError(propertyConflict.ClientValue) || !PropertyError.IsPropertyNotFound(propertyConflict.ClientValue)))
							{
								list2.Add(propertyConflict.PropertyDefinition);
							}
						}
						else if (propertyConflict.ResolvedValue != propertyConflict.ClientValue)
						{
							list3.Add(propertyConflict.PropertyDefinition);
							list4.Add(propertyConflict.ResolvedValue);
						}
					}
				}
				else
				{
					foreach (PropertyConflict propertyConflict2 in conflictResolutionResult.PropertyConflicts)
					{
						if (propertyConflict2.ResolvedValue is PropertyError)
						{
							if (PropertyError.IsPropertyNotFound(propertyConflict2.ResolvedValue))
							{
								list2.Add(propertyConflict2.PropertyDefinition);
							}
						}
						else if (propertyConflict2.ServerValue != propertyConflict2.ResolvedValue)
						{
							list3.Add(propertyConflict2.PropertyDefinition);
							list4.Add(propertyConflict2.ResolvedValue);
						}
					}
				}
				for (int k = 0; k < list2.Count; k++)
				{
					acrPropBag.Delete(list2[k]);
				}
				for (int l = 0; l < list3.Count; l++)
				{
					acrPropBag[list3[l]] = list4[l];
				}
			}
			return conflictResolutionResult;
		}

		private bool CanSaveBeNoOp()
		{
			return !this.IsDirty && this.Mode != AcrPropertyBag.AcrMode.NewItem && this.Mode != AcrPropertyBag.AcrMode.ReadOnly;
		}

		internal ConflictResolutionResult FlushChangesWithAcr(SaveMode saveMode)
		{
			base.CheckDisposed("AcrPropertyBag::FlushChangesWithAcr");
			ConflictResolutionResult conflictResolutionResult = null;
			if (this.CanSaveBeNoOp())
			{
				return ConflictResolutionResult.Success;
			}
			switch (this.Mode)
			{
			case AcrPropertyBag.AcrMode.ReadOnly:
				throw new AccessDeniedException(ServerStrings.ExItemIsOpenedInReadOnlyMode);
			case AcrPropertyBag.AcrMode.Active:
				conflictResolutionResult = this.ApplyAcr(this.propertyBag, saveMode);
				if (conflictResolutionResult.SaveStatus == SaveResult.IrresolvableConflict)
				{
					return conflictResolutionResult;
				}
				break;
			}
			this.propertyBag.FlushChanges();
			this.uncommitted = true;
			if (conflictResolutionResult == null)
			{
				conflictResolutionResult = ConflictResolutionResult.Success;
			}
			return conflictResolutionResult;
		}

		internal ConflictResolutionResult SaveChangesWithAcr(SaveMode saveMode)
		{
			base.CheckDisposed("AcrPropertyBag::SaveChangesWithAcr");
			ConflictResolutionResult conflictResolutionResult = null;
			if (this.CanSaveBeNoOp() && !this.uncommitted)
			{
				return ConflictResolutionResult.Success;
			}
			try
			{
				this.propertyBag.SaveChanges(false);
				this.uncommitted = false;
			}
			catch (SaveConflictException)
			{
				PersistablePropertyBag persistablePropertyBag = this.propertyBagFactory.CreateStorePropertyBag(this.propertyBag, this.PrefetchPropertyArray);
				persistablePropertyBag.Context.Copy(this.propertyBag.Context);
				try
				{
					conflictResolutionResult = this.ApplyAcr(persistablePropertyBag, saveMode);
					if (conflictResolutionResult.SaveStatus == SaveResult.Success || conflictResolutionResult.SaveStatus == SaveResult.SuccessWithConflictResolution)
					{
						persistablePropertyBag.FlushChanges();
						persistablePropertyBag.SaveChanges(false);
						this.PropertyBag = persistablePropertyBag;
						persistablePropertyBag = null;
						this.uncommitted = false;
					}
				}
				finally
				{
					if (persistablePropertyBag != null)
					{
						persistablePropertyBag.Dispose();
					}
				}
			}
			if (conflictResolutionResult == null)
			{
				conflictResolutionResult = ConflictResolutionResult.Success;
			}
			if (conflictResolutionResult.SaveStatus != SaveResult.IrresolvableConflict)
			{
				this.RefreshCacheAfterSave(conflictResolutionResult);
			}
			return conflictResolutionResult;
		}

		private void RefreshCacheAfterSave(ConflictResolutionResult resolutionResults)
		{
			if (this.Mode != AcrPropertyBag.AcrMode.NewItem)
			{
				this.propertyTrackingCache.Clear();
			}
			this.currentChangeKey = (this.openChangeKey = Guid.NewGuid().ToByteArray());
			this.acrModeHint = AcrPropertyBag.AcrMode.Passive;
			this.propertiesWrittenAsStream.Clear();
			this.irresolvableChanges = false;
		}

		internal override void FlushChanges()
		{
			base.CheckDisposed("AcrPropertyBag::FlushChanges");
			if (this.Mode == AcrPropertyBag.AcrMode.ReadOnly)
			{
				throw new AccessDeniedException(ServerStrings.ExItemIsOpenedInReadOnlyMode);
			}
			if (this.CanSaveBeNoOp())
			{
				return;
			}
			this.propertyBag.FlushChanges();
			this.uncommitted = true;
		}

		internal override void SaveChanges(bool force)
		{
			base.CheckDisposed("AcrPropertyBag::SaveChanges");
			if (this.Mode == AcrPropertyBag.AcrMode.ReadOnly)
			{
				throw new AccessDeniedException(ServerStrings.ExItemIsOpenedInReadOnlyMode);
			}
			if (this.CanSaveBeNoOp() && !this.uncommitted)
			{
				return;
			}
			this.propertyBag.SaveChanges(force);
			this.uncommitted = false;
			this.RefreshCacheAfterSave(null);
		}

		internal override MapiProp MapiProp
		{
			get
			{
				return this.propertyBag.MapiProp;
			}
		}

		internal PersistablePropertyBag PropertyBag
		{
			get
			{
				return this.propertyBag;
			}
			private set
			{
				if (this.propertyBag != null)
				{
					this.propertyBag.Dispose();
				}
				this.propertyBag = value;
				if (this.propertyBag != null)
				{
					this.propertyBag.PrefetchPropertyArray = this.PrefetchPropertyArray;
				}
			}
		}

		internal override ICollection<PropertyDefinition> PrefetchPropertyArray
		{
			get
			{
				return base.PrefetchPropertyArray;
			}
			set
			{
				PersistablePropertyBag persistablePropertyBag = this.propertyBag;
				base.PrefetchPropertyArray = value;
				persistablePropertyBag.PrefetchPropertyArray = value;
			}
		}

		public override bool IsDirty
		{
			get
			{
				base.CheckDisposed("IsDirty::get");
				return this.propertyBag.IsDirty || this.propertyTrackingCache.Count != 0 || this.propertiesWrittenAsStream.Count != 0 || this.irresolvableChanges;
			}
		}

		internal bool IsReadOnly
		{
			get
			{
				return this.Mode == AcrPropertyBag.AcrMode.ReadOnly;
			}
		}

		private AcrPropertyBag.AcrMode Mode
		{
			get
			{
				if (this.acrModeHint != AcrPropertyBag.AcrMode.Unknown)
				{
					return this.acrModeHint;
				}
				if (this.itemId == null)
				{
					this.acrModeHint = AcrPropertyBag.AcrMode.NewItem;
				}
				else if (this.openChangeKey == null)
				{
					this.acrModeHint = AcrPropertyBag.AcrMode.ReadOnly;
				}
				else if (this.currentChangeKey == null)
				{
					object obj = this.propertyBag.TryGetProperty(InternalSchema.ChangeKey);
					if (obj is byte[])
					{
						this.currentChangeKey = (byte[])obj;
						if (!ArrayComparer<byte>.Comparer.Equals(this.currentChangeKey, this.openChangeKey))
						{
							this.acrModeHint = AcrPropertyBag.AcrMode.Active;
						}
						else
						{
							this.acrModeHint = AcrPropertyBag.AcrMode.Passive;
						}
					}
					else
					{
						this.currentChangeKey = this.openChangeKey;
						this.acrModeHint = AcrPropertyBag.AcrMode.Passive;
					}
				}
				return this.acrModeHint;
			}
		}

		internal override ExTimeZone ExTimeZone
		{
			get
			{
				return this.PropertyBag.ExTimeZone;
			}
			set
			{
				this.PropertyBag.ExTimeZone = value;
			}
		}

		internal override PropertyBagSaveFlags SaveFlags
		{
			get
			{
				base.CheckDisposed("AcrPropertyBag.SaveFlags.get");
				return this.propertyBag.SaveFlags;
			}
			set
			{
				base.CheckDisposed("AcrPropertyBag.SaveFlags.set");
				EnumValidator.ThrowIfInvalid<PropertyBagSaveFlags>(value, "value");
				this.propertyBag.SaveFlags = value;
			}
		}

		internal override void SetUpdateImapIdFlag()
		{
			base.CheckDisposed("AcrPropertyBag::SetUpdateImapIdFlag");
			this.propertyBag.SetUpdateImapIdFlag();
		}

		protected override bool InternalIsPropertyDirty(AtomicStorePropertyDefinition propertyDefinition)
		{
			base.CheckDisposed("InternalIsPropertyDirty");
			return ((IDirectPropertyBag)this.propertyBag).IsDirty(propertyDefinition);
		}

		protected override bool IsLoaded(NativeStorePropertyDefinition propertyDefinition)
		{
			return ((IDirectPropertyBag)this.PropertyBag).IsLoaded(propertyDefinition);
		}

		internal override PropertyBagContext Context
		{
			get
			{
				return this.propertyBag.Context;
			}
		}

		private AcrPropertyBag.AcrMode acrModeHint;

		private PersistablePropertyBag propertyBag;

		private readonly AcrProfile profile;

		private readonly IPropertyBagFactory propertyBagFactory;

		private StoreObjectId itemId;

		private byte[] openChangeKey;

		private byte[] currentChangeKey;

		private bool irresolvableChanges;

		private Dictionary<PropertyDefinition, PropertyDefinition> propertiesWrittenAsStream = new Dictionary<PropertyDefinition, PropertyDefinition>();

		private Dictionary<PropertyDefinition, AcrPropertyBag.TrackingInfo> propertyTrackingCache = new Dictionary<PropertyDefinition, AcrPropertyBag.TrackingInfo>();

		private bool uncommitted;

		private enum AcrMode
		{
			Unknown,
			ReadOnly,
			Active,
			Passive,
			NewItem = 5
		}

		private class TrackingInfo
		{
			public TrackingInfo(bool dirty) : this(dirty, null, null)
			{
			}

			public TrackingInfo(bool dirty, object serverValue, object originalValue)
			{
				this.Dirty = dirty;
				this.ServerValue = serverValue;
				this.OriginalValue = originalValue;
			}

			public object OriginalValue;

			public object ServerValue;

			public bool Dirty;
		}
	}
}
