using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Data.Storage;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Mapi;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class StoreObjectPropertyBag : PersistablePropertyBag
	{
		internal StoreObjectPropertyBag(StoreSession session, MapiProp mapiProp, ICollection<PropertyDefinition> autoloadProperties) : this(session, mapiProp, autoloadProperties, true)
		{
		}

		internal StoreObjectPropertyBag(StoreSession session, MapiProp mapiProp, ICollection<PropertyDefinition> autoloadProperties, bool canSaveOrDisposeMapiProp)
		{
			this.trackingPropertyExistence = autoloadProperties.Contains(InternalSchema.PropertyExistenceTracker);
			bool flag = false;
			try
			{
				this.canSaveOrDisposeMapiProp = canSaveOrDisposeMapiProp;
				if (mapiProp != null)
				{
					this.MapiPropertyBag = new MapiPropertyBag(session, mapiProp);
					if (this.mapiPropertyBag != null && this.mapiPropertyBag.DisposeTracker != null)
					{
						this.mapiPropertyBag.DisposeTracker.AddExtraDataWithStackTrace("StoreObjectPropertyBag owns mapiPropertyBag at");
					}
				}
				this.ExTimeZone = session.ExTimeZone;
				this.PrefetchPropertyArray = autoloadProperties;
				this.Load(null);
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

		internal override ICollection<NativeStorePropertyDefinition> AllNativeProperties
		{
			get
			{
				return this.memoryPropertyBag.AllNativeProperties;
			}
		}

		public override bool HasAllPropertiesLoaded
		{
			get
			{
				base.CheckDisposed("HasAllPropertiesLoaded::get");
				return this.memoryPropertyBag.HasAllPropertiesLoaded;
			}
		}

		protected override object TryGetStoreProperty(StorePropertyDefinition propertyDefinition)
		{
			if (propertyDefinition == null)
			{
				throw new ArgumentNullException(ServerStrings.ExNullParameter("propertyDefinition", 1));
			}
			return ((IDirectPropertyBag)this.memoryPropertyBag).GetValue(propertyDefinition);
		}

		protected override void SetValidatedStoreProperty(StorePropertyDefinition propertyDefinition, object propertyValue)
		{
			if (propertyDefinition == null)
			{
				throw new ArgumentNullException(ServerStrings.ExNullParameter("propertyDefinition", 1));
			}
			if (propertyValue == null)
			{
				throw PropertyError.ToException(new PropertyError[]
				{
					new PropertyError(propertyDefinition, PropertyErrorCode.NullValue)
				});
			}
			((IDirectPropertyBag)this.memoryPropertyBag).SetValue(propertyDefinition, propertyValue);
			this.TrackProperty(propertyDefinition, true);
		}

		protected override void DeleteStoreProperty(StorePropertyDefinition propertyDefinition)
		{
			((IDirectPropertyBag)this.MemoryPropertyBag).Delete(propertyDefinition);
			this.TrackProperty(propertyDefinition, false);
		}

		public override void Load(ICollection<PropertyDefinition> properties)
		{
			this.BindToMapiPropertyBag();
			if (this.HasAllPropertiesLoaded)
			{
				return;
			}
			if (properties == InternalSchema.ContentConversionProperties || this.PrefetchPropertyArray == InternalSchema.ContentConversionProperties)
			{
				this.InternalLoadAllPropertiesOnItemForContentConversion();
				return;
			}
			this.InternalLoad(properties);
		}

		public override void Reload()
		{
			this.Clear();
			this.InternalLoad(null);
		}

		internal void ForceReload(ICollection<PropertyDefinition> propsToLoad)
		{
			if (base.IsNew)
			{
				throw new InvalidOperationException(ServerStrings.NoServerValueAvailable);
			}
			foreach (PropertyDefinition propertyDefinition in propsToLoad)
			{
				this.MemoryPropertyBag.Unload(propertyDefinition);
			}
			this.InternalLoad(propsToLoad);
		}

		public override void Clear()
		{
			this.memoryPropertyBag.Clear();
			this.propertyExistenceBitMap = null;
		}

		private void InternalLoad(ICollection<PropertyDefinition> extraProperties)
		{
			ICollection<PropertyDefinition> collection;
			if (!this.IsCacheValid)
			{
				collection = this.PrefetchPropertyArray.Concat(extraProperties);
			}
			else
			{
				collection = extraProperties;
			}
			if (collection == null || collection.Count == 0)
			{
				return;
			}
			if (this.mapiPropertyBag != null)
			{
				IList<NativeStorePropertyDefinition> nativePropertyDefinitions = StorePropertyDefinition.GetNativePropertyDefinitions<PropertyDefinition>(PropertyDependencyType.AllRead, collection, (NativeStorePropertyDefinition native) => !((IDirectPropertyBag)this.MemoryPropertyBag).IsLoaded(native));
				if (nativePropertyDefinitions.Count > 0)
				{
					object[] properties = this.MapiPropertyBag.GetProperties(nativePropertyDefinitions);
					this.MemoryPropertyBag.PreLoadStoreProperty<NativeStorePropertyDefinition>(nativePropertyDefinitions, properties);
					return;
				}
			}
			else
			{
				this.MemoryPropertyBag.Load(collection);
			}
		}

		private void InternalLoadAllPropertiesOnItemForContentConversion()
		{
			if (this.mapiPropertyBag != null)
			{
				ICollection<PropValue> allProperties = this.MapiPropertyBag.GetAllProperties();
				if (allProperties.Count > 0)
				{
					this.MemoryPropertyBag.PreLoadStoreProperties(allProperties);
				}
			}
			this.MemoryPropertyBag.SetAllPropertiesLoaded();
		}

		public override DisposeTracker GetDisposeTracker()
		{
			return DisposeTracker.Get<StoreObjectPropertyBag>(this);
		}

		protected override void InternalDispose(bool disposing)
		{
			base.InternalDispose(disposing);
			if (this.mapiPropertyBag != null && this.mapiPropertyBag.DisposeTracker != null)
			{
				this.mapiPropertyBag.DisposeTracker.AddExtraDataWithStackTrace(string.Format(CultureInfo.InvariantCulture, "StoreObjectPropertyBag.InternalDispose({0}) called with stack", new object[]
				{
					disposing
				}));
			}
			if (disposing)
			{
				if (this.mapiPropertyBag != null)
				{
					if (!this.canSaveOrDisposeMapiProp)
					{
						this.MapiPropertyBag.DetachMapiProp();
					}
					this.MapiPropertyBag.Dispose();
				}
				foreach (StoreObjectStream storeObjectStream in this.listOfStreams)
				{
					storeObjectStream.DetachPropertyBag();
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
				base.PrefetchPropertyArray = (value ?? ((ICollection<PropertyDefinition>)Array<PropertyDefinition>.Empty));
			}
		}

		internal override MapiProp MapiProp
		{
			get
			{
				return this.MapiPropertyBag.MapiProp;
			}
		}

		public override Stream OpenPropertyStream(PropertyDefinition propertyDefinition, PropertyOpenMode openMode)
		{
			base.CheckDisposed("OpenPropertyStream");
			EnumValidator.AssertValid<PropertyOpenMode>(openMode);
			NativeStorePropertyDefinition nativeStorePropertyDefinition = propertyDefinition as NativeStorePropertyDefinition;
			if (nativeStorePropertyDefinition == null)
			{
				throw new InvalidOperationException(ServerStrings.ExPropertyNotStreamable(propertyDefinition.ToString()));
			}
			Stream result;
			using (DisposeGuard disposeGuard = default(DisposeGuard))
			{
				StoreObjectStream storeObjectStream = new StoreObjectStream(this, nativeStorePropertyDefinition, openMode);
				disposeGuard.Add<StoreObjectStream>(storeObjectStream);
				this.listOfStreams.Add(storeObjectStream);
				disposeGuard.Success();
				if (openMode == PropertyOpenMode.Create || openMode == PropertyOpenMode.Modify)
				{
					this.TrackProperty(nativeStorePropertyDefinition, true);
				}
				result = storeObjectStream;
			}
			return result;
		}

		internal override void FlushChanges()
		{
			base.CheckDisposed("FlushChanges");
			List<PropertyError> list = new List<PropertyError>();
			this.BindToMapiPropertyBag();
			this.MapiPropertyBag.SaveFlags = this.saveFlags;
			foreach (StoreObjectStream storeObjectStream in this.listOfStreams)
			{
				storeObjectStream.Flush();
			}
			list.AddRange(this.FlushDeleteProperties());
			list.AddRange(this.FlushSetProperties());
			if ((this.saveFlags & PropertyBagSaveFlags.IgnoreMapiComputedErrors) == PropertyBagSaveFlags.IgnoreMapiComputedErrors)
			{
				for (int i = list.Count - 1; i >= 0; i--)
				{
					if (list[i].PropertyErrorCode == PropertyErrorCode.SetStoreComputedPropertyError)
					{
						list.RemoveAt(i);
					}
				}
			}
			if ((this.saveFlags & PropertyBagSaveFlags.IgnoreAccessDeniedErrors) == PropertyBagSaveFlags.IgnoreAccessDeniedErrors)
			{
				for (int j = list.Count - 1; j >= 0; j--)
				{
					if (list[j].PropertyErrorCode == PropertyErrorCode.AccessDenied)
					{
						list.RemoveAt(j);
					}
				}
			}
			if (list.Count > 0)
			{
				ExTraceGlobals.StorageTracer.TraceDebug<int>((long)this.GetHashCode(), "StoreObjectPropertyBag::FlushChanges. Property was not set or deleted successfully. Error Count = {0}.", list.Count);
				throw PropertyError.ToException(list.ToArray());
			}
			this.memoryPropertyBag.ClearChangeInfo();
		}

		internal override void SaveChanges(bool force)
		{
			base.CheckDisposed("SaveChanges");
			if (this.canSaveOrDisposeMapiProp)
			{
				this.mapiPropertyBag.SaveChanges(force);
			}
			this.Clear();
		}

		protected override bool InternalIsPropertyDirty(AtomicStorePropertyDefinition propertyDefinition)
		{
			base.CheckDisposed("InternalIsPropertyDirty");
			return ((IDirectPropertyBag)this.memoryPropertyBag).IsDirty(propertyDefinition);
		}

		public override bool IsDirty
		{
			get
			{
				base.CheckDisposed("IsDirty::get");
				return this.canSaveOrDisposeMapiProp && this.memoryPropertyBag.IsDirty;
			}
		}

		public override PropertyValueTrackingData GetOriginalPropertyInformation(PropertyDefinition propertyDefinition)
		{
			return this.memoryPropertyBag.GetOriginalPropertyInformation(propertyDefinition);
		}

		protected override bool IsLoaded(NativeStorePropertyDefinition propertyDefinition)
		{
			return ((IDirectPropertyBag)this.memoryPropertyBag).IsLoaded(propertyDefinition);
		}

		public override ICollection<PropertyDefinition> AllFoundProperties
		{
			get
			{
				if (this.Context.CoreState.Origin == Origin.Existing && !this.HasAllPropertiesLoaded)
				{
					return InternalSchema.Combine<PropertyDefinition>(this.MapiPropertyBag.GetAllFoundProperties(), this.MemoryPropertyBag.AllFoundProperties);
				}
				return this.MemoryPropertyBag.AllFoundProperties;
			}
		}

		protected PropertyError[] FlushSetProperties()
		{
			this.BindToMapiPropertyBag();
			if (this.memoryPropertyBag.ChangeList.Count > 0)
			{
				List<PropertyDefinition> list = new List<PropertyDefinition>();
				List<object> list2 = new List<object>();
				List<PropertyError> list3 = new List<PropertyError>();
				this.AddSubjectPropertiesToList(list, list2);
				foreach (PropertyDefinition propertyDefinition in this.memoryPropertyBag.ChangeList)
				{
					if (!propertyDefinition.Equals(InternalSchema.MapiSubject) && !propertyDefinition.Equals(InternalSchema.SubjectPrefix) && !propertyDefinition.Equals(InternalSchema.NormalizedSubjectInternal))
					{
						object obj = this.memoryPropertyBag.TryGetProperty(propertyDefinition);
						if (!(obj is PropertyError))
						{
							PropertyErrorCode? propertyErrorCode;
							if (this.ShouldSkipProperty(propertyDefinition, out propertyErrorCode))
							{
								list3.Add(new PropertyError(propertyDefinition, (propertyErrorCode != null) ? propertyErrorCode.Value : PropertyErrorCode.NotFound));
							}
							else
							{
								list.Add(propertyDefinition);
								list2.Add(obj);
							}
						}
					}
				}
				if (list2.Count > 0)
				{
					if ((this.saveFlags & PropertyBagSaveFlags.SaveFolderPropertyBagConditional) == PropertyBagSaveFlags.SaveFolderPropertyBagConditional)
					{
						list3.AddRange(this.MapiPropertyBag.SetPropertiesWithChangeKeyCheck(base.TryGetProperty(InternalSchema.ChangeKey) as byte[], list.ToArray(), list2.ToArray()));
					}
					else
					{
						list3.AddRange(this.MapiPropertyBag.SetProperties(list.ToArray(), list2.ToArray()));
					}
				}
				return list3.ToArray();
			}
			return MapiPropertyBag.EmptyPropertyErrorArray;
		}

		protected virtual bool ShouldSkipProperty(PropertyDefinition property, out PropertyErrorCode? error)
		{
			error = null;
			return false;
		}

		private void AddSubjectPropertiesToList(List<PropertyDefinition> propertyDefinitions, List<object> propertyValues)
		{
			object changedProperty = this.GetChangedProperty(InternalSchema.MapiSubject);
			object changedProperty2 = this.GetChangedProperty(InternalSchema.SubjectPrefixInternal);
			object changedProperty3 = this.GetChangedProperty(InternalSchema.NormalizedSubjectInternal);
			if (changedProperty2 != null)
			{
				propertyDefinitions.Add(InternalSchema.SubjectPrefixInternal);
				propertyValues.Add(changedProperty2);
			}
			if (changedProperty3 != null)
			{
				propertyDefinitions.Add(InternalSchema.NormalizedSubjectInternal);
				propertyValues.Add(changedProperty3);
			}
			if (changedProperty != null && (changedProperty2 == null || changedProperty3 == null))
			{
				propertyDefinitions.Add(InternalSchema.MapiSubject);
				propertyValues.Add(changedProperty);
			}
		}

		protected PropertyError[] FlushDeleteProperties()
		{
			this.BindToMapiPropertyBag();
			if (this.memoryPropertyBag.DeleteList.Count > 0)
			{
				List<PropertyDefinition> list = new List<PropertyDefinition>();
				List<PropertyError> list2 = new List<PropertyError>();
				foreach (PropertyDefinition propertyDefinition in this.memoryPropertyBag.DeleteList)
				{
					PropertyErrorCode? propertyErrorCode;
					if (this.ShouldSkipProperty(propertyDefinition, out propertyErrorCode))
					{
						list2.Add(new PropertyError(propertyDefinition, (propertyErrorCode != null) ? propertyErrorCode.Value : PropertyErrorCode.NotFound));
					}
					else
					{
						list.Add(propertyDefinition);
					}
				}
				if (list.Count > 0)
				{
					list2.AddRange(this.MapiPropertyBag.DeleteProperties(list));
				}
				return list2.ToArray();
			}
			return MapiPropertyBag.EmptyPropertyErrorArray;
		}

		protected virtual void LazyCreateMapiPropertyBag()
		{
			throw new InvalidOperationException("The property bag is not bound to a MapiProp object");
		}

		protected void BindToMapiPropertyBag()
		{
			if (this.mapiPropertyBag == null)
			{
				this.LazyCreateMapiPropertyBag();
			}
		}

		private bool IsCacheValid
		{
			get
			{
				return this.MemoryPropertyBag.HasAllPropertiesLoaded || this.MemoryPropertyBag.Count > 0;
			}
		}

		private object GetChangedProperty(PropertyDefinition property)
		{
			if (!this.memoryPropertyBag.ChangeList.Contains(property))
			{
				return null;
			}
			object obj = this.memoryPropertyBag.TryGetProperty(property);
			if (obj is PropertyError)
			{
				return null;
			}
			return obj;
		}

		private void SyncPropertyBagParameters()
		{
			if (this.mapiPropertyBag != null)
			{
				this.MapiPropertyBag.ExTimeZone = this.memoryPropertyBag.ExTimeZone;
			}
		}

		private void TrackProperty(StorePropertyDefinition propertyDefinition, bool isAdded)
		{
			if (!this.trackingPropertyExistence)
			{
				return;
			}
			long bitFlag = PropertyExistenceTracker.GetBitFlag(propertyDefinition);
			if (bitFlag == -1L)
			{
				return;
			}
			if (this.propertyExistenceBitMap == null)
			{
				object obj = base.TryGetProperty(InternalSchema.PropertyExistenceTracker);
				if (obj is long)
				{
					this.propertyExistenceBitMap = new long?((long)obj);
				}
				else
				{
					this.propertyExistenceBitMap = new long?(0L);
				}
			}
			if (isAdded)
			{
				this.propertyExistenceBitMap = new long?(this.propertyExistenceBitMap.Value | bitFlag);
			}
			else
			{
				this.propertyExistenceBitMap &= ~bitFlag;
			}
			if (this.propertyExistenceBitMap.Value != 0L)
			{
				((IDirectPropertyBag)this.memoryPropertyBag).SetValue(InternalSchema.PropertyExistenceTracker, this.propertyExistenceBitMap.Value);
				return;
			}
			((IDirectPropertyBag)this.memoryPropertyBag).Delete(InternalSchema.PropertyExistenceTracker);
		}

		internal static StoreObjectPropertyBag CreatePropertyBag(StoreSession storeSession, StoreObjectId id, ICollection<PropertyDefinition> prefetchPropertyArray)
		{
			Util.ThrowOnNullArgument(id, "id");
			MapiProp mapiProp = null;
			StoreObjectPropertyBag storeObjectPropertyBag = null;
			bool flag = false;
			StoreObjectPropertyBag result;
			try
			{
				mapiProp = storeSession.GetMapiProp(id);
				storeObjectPropertyBag = new StoreObjectPropertyBag(storeSession, mapiProp, prefetchPropertyArray);
				flag = true;
				result = storeObjectPropertyBag;
			}
			finally
			{
				if (!flag)
				{
					Util.DisposeIfPresent(storeObjectPropertyBag);
					Util.DisposeIfPresent(mapiProp);
				}
			}
			return result;
		}

		internal static PropertyError[] MapiPropProblemsToPropertyErrors(PropertyDefinition[] propertyDefinitions, PropProblem[] problems)
		{
			PropertyError[] array = new PropertyError[problems.Length];
			for (int i = 0; i < problems.Length; i++)
			{
				string errorDescription;
				PropertyErrorCode error = MapiPropertyHelper.MapiErrorToXsoError(problems[i].Scode, out errorDescription);
				array[i] = new PropertyError(propertyDefinitions[problems[i].Index], error, errorDescription);
			}
			return array;
		}

		internal static PropertyError[] MapiPropProblemsToPropertyErrors(StoreSession storeSession, MapiProp mapiProp, PropProblem[] problems)
		{
			PropTag[] array = new PropTag[problems.Length];
			PropProblem[] array2 = new PropProblem[problems.Length];
			for (int i = 0; i < problems.Length; i++)
			{
				array[i] = problems[i].PropTag;
				array2[i] = new PropProblem(i, problems[i].PropTag, problems[i].Scode);
			}
			NativeStorePropertyDefinition[] propertyDefinitions = PropertyTagCache.Cache.PropertyDefinitionsFromPropTags(NativeStorePropertyDefinition.TypeCheckingFlag.DisableTypeCheck, mapiProp, storeSession, array);
			return StoreObjectPropertyBag.MapiPropProblemsToPropertyErrors(propertyDefinitions, array2);
		}

		internal override PropertyBagSaveFlags SaveFlags
		{
			get
			{
				base.CheckDisposed("StoreObjectPropertyBag.SaveFlags.get");
				return this.saveFlags;
			}
			set
			{
				base.CheckDisposed("StoreObjectPropertyBag.SaveFlags.set");
				EnumValidator.ThrowIfInvalid<PropertyBagSaveFlags>(value, "value");
				this.saveFlags = value;
			}
		}

		internal override void SetUpdateImapIdFlag()
		{
			base.CheckDisposed("SetUpdateImapIdFlag");
			this.MapiPropertyBag.SetUpdateImapIdFlag();
		}

		[Conditional("DEBUG")]
		internal static void CheckNativePropertyDefinition(PropertyDefinition propertyDefinition)
		{
		}

		internal override ExTimeZone ExTimeZone
		{
			get
			{
				return this.memoryPropertyBag.ExTimeZone;
			}
			set
			{
				this.memoryPropertyBag.ExTimeZone = value;
				this.SyncPropertyBagParameters();
			}
		}

		internal void OnStreamClose(StoreObjectStream stream)
		{
			this.listOfStreams.Remove(stream);
		}

		internal MapiPropertyBag MapiPropertyBag
		{
			get
			{
				if (this.mapiPropertyBag == null)
				{
					throw new InvalidOperationException("The property bag is not bound to a MapiProp object");
				}
				return this.mapiPropertyBag;
			}
			set
			{
				Util.ThrowOnNullArgument(value, "value");
				if (this.mapiPropertyBag != null)
				{
					throw new InvalidOperationException("MapiPropertyBag has been already assigned");
				}
				this.mapiPropertyBag = value;
				this.SyncPropertyBagParameters();
			}
		}

		internal MemoryPropertyBag MemoryPropertyBag
		{
			get
			{
				return this.memoryPropertyBag;
			}
		}

		private MapiPropertyBag mapiPropertyBag;

		private readonly MemoryPropertyBag memoryPropertyBag = new MemoryPropertyBag();

		private readonly bool canSaveOrDisposeMapiProp = true;

		private PropertyBagSaveFlags saveFlags;

		private readonly List<StoreObjectStream> listOfStreams = new List<StoreObjectStream>();

		private readonly bool trackingPropertyExistence;

		private long? propertyExistenceBitMap;

		internal enum PropertyAccessMode
		{
			Stream,
			Value
		}
	}
}
