using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Data.Storage;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Mapi;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal abstract class StoreObject : IDisposeTrackable, ILocationIdentifierController, IStoreObject, IStorePropertyBag, IPropertyBag, IReadOnlyPropertyBag, IDisposable
	{
		internal StoreObject(ICoreObject coreObject, bool shallowDispose = false)
		{
			using (DisposeGuard disposeGuard = this.Guard())
			{
				StorageGlobals.TraceConstructIDisposable(this);
				this.disposeTracker = this.GetDisposeTracker();
				this.shallowDispose = shallowDispose;
				DisposableObject disposableObject = coreObject as DisposableObject;
				if (disposableObject != null && disposableObject.DisposeTracker != null)
				{
					disposableObject.DisposeTracker.AddExtraDataWithStackTrace("StoreObject (e.g. MessageItem) owns coreObject (e.g. CoreItem) at");
				}
				this.AttachCoreObject(coreObject);
				((IDirectPropertyBag)this.PropertyBag).Context.StoreObject = this;
				disposeGuard.Success();
			}
		}

		public object[] GetProperties(ICollection<PropertyDefinition> propertyDefinitionArray)
		{
			this.CheckDisposed("GetProperties");
			if (propertyDefinitionArray == null || propertyDefinitionArray.Count == 0)
			{
				return Array<object>.Empty;
			}
			object[] array = new object[propertyDefinitionArray.Count];
			int num = 0;
			foreach (PropertyDefinition propertyDefinition in propertyDefinitionArray)
			{
				array[num++] = this.TryGetProperty(propertyDefinition);
			}
			return array;
		}

		public void SetProperties(ICollection<PropertyDefinition> propertyDefinitions, object[] propertyValues)
		{
			this.CheckDisposed("SetProperties");
			if (propertyDefinitions == null)
			{
				return;
			}
			int num = 0;
			foreach (PropertyDefinition propertyDefinition in propertyDefinitions)
			{
				this[propertyDefinition] = propertyValues[num++];
			}
		}

		public virtual bool IsDirty
		{
			get
			{
				return this.coreObject != null && this.coreObject.IsDirty;
			}
		}

		public bool IsPropertyDirty(PropertyDefinition propertyDefinition)
		{
			this.CheckDisposed("IsPropertyDirty");
			return this.PropertyBag.IsPropertyDirty(propertyDefinition);
		}

		public void Load()
		{
			this.Load(null);
		}

		public void Load(params PropertyDefinition[] properties)
		{
			this.Load((ICollection<PropertyDefinition>)properties);
		}

		public void Load(ICollection<PropertyDefinition> properties)
		{
			this.CheckDisposed("Load");
			ExTraceGlobals.StorageTracer.Information<int>((long)this.GetHashCode(), "StoreObject:Load. Hashcode = {0}.", this.GetHashCode());
			this.PropertyBag.Load(properties);
		}

		public virtual object this[PropertyDefinition propertyDefinition]
		{
			get
			{
				this.CheckDisposed("this::get[PropertyDefinition]");
				return this.GetProperty(propertyDefinition);
			}
			set
			{
				this.CheckDisposed("this::set[PropertyDefinition]");
				this.SetProperty(propertyDefinition, value);
			}
		}

		public object TryGetProperty(PropertyDefinition propertyDefinition)
		{
			this.CheckDisposed("TryGetProperty");
			if (propertyDefinition is SimpleVirtualPropertyDefinition)
			{
				return new PropertyError(propertyDefinition, PropertyErrorCode.NotFound);
			}
			return this.PropertyBag.TryGetProperty(propertyDefinition);
		}

		public object TryGetProperty(StorePropertyDefinition propertyDefinition)
		{
			this.CheckDisposed("TryGetProperty");
			if (propertyDefinition is SimpleVirtualPropertyDefinition)
			{
				return new PropertyError(propertyDefinition, PropertyErrorCode.NotFound);
			}
			return this.PropertyBag.TryGetProperty(propertyDefinition);
		}

		public void Delete(PropertyDefinition propertyDefinition)
		{
			this.CheckDisposed("Delete");
			this.PropertyBag.Delete(propertyDefinition);
		}

		public Stream OpenPropertyStream(PropertyDefinition propertyDefinition, PropertyOpenMode openMode)
		{
			this.CheckDisposed("OpenPropertyStream");
			if (propertyDefinition == null)
			{
				throw new ArgumentNullException("propertyDefinition");
			}
			EnumValidator.ThrowIfInvalid<PropertyOpenMode>(openMode, "openMode");
			InternalSchema.ToStorePropertyDefinition(propertyDefinition);
			if (!(propertyDefinition is NativeStorePropertyDefinition))
			{
				throw new NotImplementedException(ServerStrings.ExCalculatedPropertyStreamAccessNotSupported(propertyDefinition.Name));
			}
			return this.PropertyBag.OpenPropertyStream(propertyDefinition, openMode);
		}

		protected virtual void CheckDisposed(string methodName)
		{
			if (this.isDisposed)
			{
				StorageGlobals.TraceFailedCheckDisposed(this, methodName);
				throw new ObjectDisposedException(base.GetType().ToString());
			}
			if (this.coreObject == null)
			{
				throw new InvalidOperationException("The core object is null");
			}
		}

		public void SetDisposeTrackerStacktraceToCurrentLocation()
		{
			if (this.disposeTracker != null)
			{
				this.disposeTracker.SetReportedStacktraceToCurrentLocation();
			}
		}

		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		private void Dispose(bool disposing)
		{
			StorageGlobals.TraceDispose(this, this.isDisposed, disposing);
			if (!this.isDisposed)
			{
				this.isDisposed = true;
				this.InternalDispose(disposing);
			}
		}

		protected virtual void InternalDispose(bool disposing)
		{
			if (this.shallowDispose)
			{
				this.coreObject = null;
			}
			if (this.coreObject != null)
			{
				DisposableObject disposableObject = this.coreObject as DisposableObject;
				if (disposableObject != null && disposableObject.DisposeTracker != null)
				{
					disposableObject.DisposeTracker.AddExtraDataWithStackTrace(string.Format(CultureInfo.InvariantCulture, "StoreObject.InternalDispose({0}) called with stack", new object[]
					{
						disposing
					}));
				}
			}
			if (disposing)
			{
				Util.DisposeIfPresent(this.coreObject);
				this.coreObject = null;
				Util.DisposeIfPresent(this.disposeTracker);
			}
		}

		internal bool IsDisposed
		{
			get
			{
				return this.isDisposed;
			}
		}

		public abstract DisposeTracker GetDisposeTracker();

		public void SuppressDisposeTracker()
		{
			if (this.disposeTracker != null)
			{
				this.disposeTracker.Suppress();
			}
		}

		public abstract string ClassName { get; set; }

		public virtual Schema Schema
		{
			get
			{
				this.CheckDisposed("Schema::get");
				return StoreObjectSchema.Instance;
			}
		}

		public StoreSession Session
		{
			get
			{
				this.CheckDisposed("Session::get");
				return this.CoreObject.Session;
			}
		}

		IStoreSession IStoreObject.Session
		{
			get
			{
				return this.Session;
			}
		}

		public VersionedId Id
		{
			get
			{
				this.CheckDisposed("Id::get");
				return this.CoreObject.Id;
			}
		}

		public byte[] RecordKey
		{
			get
			{
				return this.GetValueOrDefault<byte[]>(StoreObjectSchema.RecordKey);
			}
		}

		public StoreObjectId ParentId
		{
			get
			{
				this.CheckDisposed("ParentId::get");
				StoreObjectId valueOrDefault = this.GetValueOrDefault<StoreObjectId>(InternalSchema.ParentItemId);
				if (valueOrDefault == null)
				{
					ExTraceGlobals.StorageTracer.TraceError<LocalizedString>((long)this.GetHashCode(), "StoreObject::ParentId::get. Error: {0}", ServerStrings.ExItemNoParentId);
					throw new InvalidOperationException(ServerStrings.ExItemNoParentId);
				}
				return valueOrDefault;
			}
		}

		protected bool IsInMemoryObject
		{
			get
			{
				return this.Session == null;
			}
		}

		public virtual ObjectState ObjectState
		{
			get
			{
				if (this.IsNew)
				{
					return ObjectState.New;
				}
				if (this.IsDirty)
				{
					return ObjectState.Changed;
				}
				return ObjectState.Unchanged;
			}
		}

		public ExDateTime CreationTime
		{
			get
			{
				this.CheckDisposed("CreationTime::get");
				return this.GetValueOrDefault<ExDateTime>(InternalSchema.CreationTime, ExDateTime.MinValue);
			}
		}

		public ExDateTime LastModifiedTime
		{
			get
			{
				this.CheckDisposed("LastModifiedTime::get");
				return this.GetValueOrDefault<ExDateTime>(InternalSchema.LastModifiedTime, ExDateTime.MinValue);
			}
		}

		public StoreObjectValidationError[] Validate()
		{
			this.CheckDisposed("Validate");
			ValidationContext context = new ValidationContext(this.Session);
			return Validation.CreateStoreObjectValiationErrorArray(this.CoreObject, context);
		}

		public bool EnableFullValidation
		{
			get
			{
				this.CheckDisposed("EnableFullValidation::get");
				return this.CoreObject.ValidateAllProperties;
			}
			set
			{
				this.CheckDisposed("EnableFullValidation::set");
				this.CoreObject.SetEnableFullValidation(value);
			}
		}

		public DisposeTracker DisposeTracker
		{
			get
			{
				this.CheckDisposed("DisposeTracker::get");
				return this.disposeTracker;
			}
		}

		public StoreObjectId InternalObjectId
		{
			get
			{
				this.CheckDisposed("InternalObjectId::get");
				return this.CoreObject.InternalStoreObjectId;
			}
		}

		public StoreObjectId StoreObjectId
		{
			get
			{
				this.CheckDisposed("StoreObjectId::get");
				return this.CoreObject.StoreObjectId;
			}
		}

		public ICollection<PropertyDefinition> PrefetchPropertyArray
		{
			get
			{
				this.CheckDisposed("PrefetchProperties::get");
				return this.PropertyBag.PrefetchPropertyArray;
			}
		}

		public bool IsNew
		{
			get
			{
				return this.CoreObject.Origin == Origin.New;
			}
		}

		internal ICollection<NativeStorePropertyDefinition> AllNativeProperties
		{
			get
			{
				return this.PropertyBag.AllNativeProperties;
			}
		}

		internal bool IsAttached
		{
			get
			{
				return this.CoreObject.ItemLevel == ItemLevel.Attached;
			}
		}

		private void AttachCoreObject(ICoreObject coreObject)
		{
			this.DetachCoreObject();
			this.coreObject = coreObject;
			if (this.coreObject != null)
			{
				this.OnCoreObjectAttached();
			}
		}

		private ICoreObject DetachCoreObject()
		{
			ICoreObject coreObject = this.coreObject;
			if (coreObject != null)
			{
				this.OnCoreObjectDetaching();
				this.coreObject = null;
			}
			return coreObject;
		}

		public void DeleteProperties(params PropertyDefinition[] propertyDefinitions)
		{
			this.CheckDisposed("DeleteProperties");
			if (propertyDefinitions == null)
			{
				throw new ArgumentNullException("propertyDefinitions");
			}
			foreach (PropertyDefinition propertyDefinition in propertyDefinitions)
			{
				this.Delete(propertyDefinition);
			}
		}

		private void SetProperty(PropertyDefinition propertyDefinition, object propertyValue)
		{
			this.CheckDisposed("SetProperty");
			this.PropertyBag.SetProperty(propertyDefinition, propertyValue);
		}

		private object GetProperty(PropertyDefinition propertyDefinition)
		{
			this.CheckDisposed("GetProperty");
			if (propertyDefinition is SimpleVirtualPropertyDefinition)
			{
				return new PropertyError(propertyDefinition, PropertyErrorCode.NotFound);
			}
			return this.PropertyBag[propertyDefinition];
		}

		public void SetOrDeleteProperty(PropertyDefinition propertyDefinition, object propertyValue)
		{
			this.CheckDisposed("SetOrDeleteProperty");
			this.PropertyBag.SetOrDeleteProperty(propertyDefinition, propertyValue);
		}

		protected virtual bool CanDoObjectUpdate
		{
			get
			{
				return true;
			}
		}

		public PersistablePropertyBag PropertyBag
		{
			get
			{
				this.CheckDisposed("PropertyBag::get");
				return Microsoft.Exchange.Data.Storage.CoreObject.GetPersistablePropertyBag(this.CoreObject);
			}
		}

		public ICoreObject CoreObject
		{
			get
			{
				this.CheckDisposed("CoreObject::get");
				return this.coreObject;
			}
			set
			{
				this.CheckDisposed("CoreObject::set");
				this.AttachCoreObject(value);
			}
		}

		internal MapiProp MapiProp
		{
			get
			{
				this.CheckDisposed("MapiProp::get");
				return this.PropertyBag.MapiProp;
			}
		}

		public T GetValueOrDefault<T>(PropertyDefinition propertyDefinition)
		{
			StorePropertyDefinition propertyDefinition2 = InternalSchema.ToStorePropertyDefinition(propertyDefinition);
			return this.GetValueOrDefault<T>(propertyDefinition2);
		}

		public T GetValueOrDefault<T>(StorePropertyDefinition propertyDefinition)
		{
			return this.GetValueOrDefault<T>(propertyDefinition, default(T));
		}

		public T GetValueOrDefault<T>(PropertyDefinition propertyDefinition, T defaultValue)
		{
			StorePropertyDefinition propertyDefinition2 = InternalSchema.ToStorePropertyDefinition(propertyDefinition);
			return this.GetValueOrDefault<T>(propertyDefinition2, defaultValue);
		}

		public T GetValueOrDefault<T>(StorePropertyDefinition propertyDefinition, T defaultValue)
		{
			this.CheckDisposed("GetValueOrDefault");
			return Microsoft.Exchange.Data.Storage.PropertyBag.CheckPropertyValue<T>(propertyDefinition, this.TryGetProperty(propertyDefinition), defaultValue);
		}

		public T? GetValueAsNullable<T>(PropertyDefinition propertyDefinition) where T : struct
		{
			StorePropertyDefinition propertyDefinition2 = InternalSchema.ToStorePropertyDefinition(propertyDefinition);
			return this.GetValueAsNullable<T>(propertyDefinition2);
		}

		public T? GetValueAsNullable<T>(StorePropertyDefinition propertyDefinition) where T : struct
		{
			this.CheckDisposed("GetValueAsNullable");
			return Microsoft.Exchange.Data.Storage.PropertyBag.CheckNullablePropertyValue<T>(propertyDefinition, this.TryGetProperty(propertyDefinition));
		}

		public void SafeSetProperty(PropertyDefinition propertyDefinition, object propValue)
		{
			this.CheckDisposed("SafeSetProperty");
			if (propValue != null && !(propValue is PropertyError))
			{
				this.SetProperty(propertyDefinition, propValue);
			}
		}

		internal static object SafePropertyValue(object propValue, Type type, object defaultValue)
		{
			if (propValue != null && propValue.GetType() == type)
			{
				return propValue;
			}
			return defaultValue;
		}

		internal T DownCastStoreObject<T>() where T : StoreObject
		{
			T t = this as T;
			if (t == null)
			{
				throw new WrongObjectTypeException(ServerStrings.BindToWrongObjectType(this.ClassName, typeof(T).ToString()));
			}
			return t;
		}

		protected virtual void OnCoreObjectAttached()
		{
		}

		protected virtual void OnCoreObjectDetaching()
		{
		}

		public LocationIdentifierHelper LocationIdentifierHelperInstance
		{
			get
			{
				this.CheckDisposed("get_LocationIdentifierHelperInstance");
				if (this.coreObject == null || !(this.coreObject is ILocationIdentifierController))
				{
					return null;
				}
				return ((ILocationIdentifierController)this.coreObject).LocationIdentifierHelperInstance;
			}
		}

		private readonly DisposeTracker disposeTracker;

		private readonly bool shallowDispose;

		private ICoreObject coreObject;

		private bool isDisposed;
	}
}
