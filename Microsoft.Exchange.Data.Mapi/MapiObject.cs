using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Mapi.Common;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Data.Mapi;
using Microsoft.Mapi;

namespace Microsoft.Exchange.Data.Mapi
{
	[Serializable]
	public abstract class MapiObject : ConfigurableObject, IDisposeTrackable, IDisposable
	{
		public MapiObjectId MapiIdentity
		{
			get
			{
				return (MapiObjectId)this.GetIdentity();
			}
			internal set
			{
				this.SetIdentity(value);
			}
		}

		internal static MapiEntryId GetEntryIdentity(MapiProp mapiObject)
		{
			MapiEntryId result;
			try
			{
				if (mapiObject == null)
				{
					throw new ArgumentNullException("mapiObject");
				}
				PropValue prop = mapiObject.GetProp(PropTag.EntryId);
				if (PropType.Error == prop.PropType)
				{
					result = null;
				}
				else
				{
					result = new MapiEntryId(prop.GetBytes());
				}
			}
			catch (MapiPermanentException)
			{
				result = null;
			}
			catch (MapiRetryableException)
			{
				result = null;
			}
			return result;
		}

		internal static string GetName(MapiProp mapiObject)
		{
			string result;
			try
			{
				if (mapiObject == null)
				{
					throw new ArgumentNullException("mapiObject");
				}
				PropValue prop = mapiObject.GetProp(PropTag.DisplayName);
				if (PropType.Error == prop.PropType)
				{
					result = null;
				}
				else
				{
					result = prop.GetString();
				}
			}
			catch (MapiPermanentException)
			{
				result = null;
			}
			catch (MapiRetryableException)
			{
				result = null;
			}
			return result;
		}

		internal void DisposeCheck()
		{
			if (this.disposed)
			{
				throw new ObjectDisposedException(base.GetType().ToString());
			}
		}

		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		internal virtual void ReleaseUnmanagedResources()
		{
		}

		internal void ResetChangeTrackingAndObjectState()
		{
			base.ResetChangeTracking(true);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (!this.disposed)
			{
				if (disposing)
				{
					this.ReleaseUnmanagedResources();
					if (this.disposeTracker != null)
					{
						this.disposeTracker.Dispose();
						this.disposeTracker = null;
					}
				}
				this.disposed = true;
			}
		}

		public virtual DisposeTracker GetDisposeTracker()
		{
			return DisposeTracker.Get<MapiObject>(this);
		}

		public void SuppressDisposeTracker()
		{
			if (this.disposeTracker != null)
			{
				this.disposeTracker.Suppress();
			}
		}

		protected void EnableDisposeTracking()
		{
			if (this.disposeTracker == null)
			{
				this.disposeTracker = this.GetDisposeTracker();
			}
			this.disposed = false;
		}

		protected abstract ObjectId GetIdentity();

		protected virtual void SetIdentity(ObjectId identity)
		{
			if (identity == null)
			{
				throw new MapiInvalidOperationException(Strings.ExceptionIdentityNull);
			}
			this[this.propertyBag.ObjectIdentityPropertyDefinition] = identity;
		}

		internal abstract void Save(bool keepUnmanagedResources);

		internal abstract void Read(bool keepUnmanagedResources);

		internal abstract void Delete();

		internal abstract T[] Find<T>(QueryFilter filter, MapiObjectId root, QueryScope scope, SortBy sort, int maximumResultsSize) where T : IConfigurable, new();

		internal abstract IEnumerable<T> FindPaged<T>(QueryFilter filter, MapiObjectId root, QueryScope scope, SortBy sort, int pageSize, int maximumResultsSize) where T : IConfigurable, new();

		internal abstract MapiProp RawMapiEntry { get; }

		internal virtual MapiProp GetRawMapiEntry(out MapiStore store)
		{
			throw new NotImplementedException("MapiObject.GetRawMapiEntry is not implemented.");
		}

		internal bool IsOriginatingServerRetrieved
		{
			get
			{
				return null != this.originatingServer;
			}
		}

		public Fqdn OriginatingServer
		{
			get
			{
				return this.originatingServer;
			}
			internal set
			{
				this.originatingServer = value;
			}
		}

		internal MapiSession MapiSession
		{
			get
			{
				return this.mapiSession;
			}
			set
			{
				if (value == null)
				{
					throw new ArgumentNullException("MapiSession");
				}
				this.mapiSession = value;
			}
		}

		internal void Instantiate(PropValue[] propertyValues)
		{
			if (propertyValues == null)
			{
				throw new ArgumentNullException("propertyValues");
			}
			if (propertyValues.Length == 0)
			{
				return;
			}
			if (!(this.ObjectSchema is MapiObjectSchema))
			{
				throw new MapiInvalidOperationException(Strings.ExceptionSchemaInvalidCast(this.ObjectSchema.GetType().ToString()));
			}
			base.InstantiationErrors.Clear();
			MapiStore mapiStore = null;
			MapiProp mapiProp = null;
			try
			{
				foreach (PropertyDefinition propertyDefinition in this.ObjectSchema.AllProperties)
				{
					MapiPropertyDefinition mapiPropertyDefinition = (MapiPropertyDefinition)propertyDefinition;
					if (!mapiPropertyDefinition.IsCalculated && mapiPropertyDefinition.PropertyTag != PropTag.Null)
					{
						bool flag = false;
						PropValue propValue = new PropValue(PropTag.Null, null);
						foreach (PropValue propValue in propertyValues)
						{
							if (propValue.PropTag.Id() == mapiPropertyDefinition.PropertyTag.Id())
							{
								flag = true;
								break;
							}
						}
						if (!flag)
						{
							if (mapiPropertyDefinition.IsMandatory)
							{
								base.InstantiationErrors.Add(new PropertyValidationError(Strings.ErrorMandatoryPropertyMissing(mapiPropertyDefinition.Name), mapiPropertyDefinition, null));
							}
						}
						else
						{
							if (PropType.Error == propValue.PropType && propValue.GetErrorValue() == -2147024882)
							{
								if (mapiProp == null)
								{
									mapiProp = this.GetRawMapiEntry(out mapiStore);
								}
								propValue = mapiProp.GetProp(mapiPropertyDefinition.PropertyTag);
							}
							if (PropType.Error == propValue.PropType)
							{
								ExTraceGlobals.MapiObjectTracer.TraceError<PropTag>((long)this.GetHashCode(), "Retrieving PropTag '{0}' failed.", mapiPropertyDefinition.PropertyTag);
							}
							else
							{
								try
								{
									object value = mapiPropertyDefinition.Extractor(propValue, mapiPropertyDefinition);
									IList<ValidationError> list = mapiPropertyDefinition.ValidateProperty(value, this.propertyBag, false);
									if (list != null)
									{
										base.InstantiationErrors.AddRange(list);
									}
									this.propertyBag.SetField(mapiPropertyDefinition, value);
								}
								catch (MapiConvertingException ex)
								{
									base.InstantiationErrors.Add(new PropertyConversionError(ex.LocalizedString, mapiPropertyDefinition, propValue, ex));
								}
							}
						}
					}
				}
			}
			finally
			{
				if (mapiProp != null)
				{
					mapiProp.Dispose();
					mapiProp = null;
				}
				if (mapiStore != null)
				{
					mapiStore.Dispose();
					mapiStore = null;
				}
			}
		}

		internal virtual void AdjustPropertyTagsToRead(List<PropTag> propertyTags)
		{
		}

		internal PropTag[] GetPropertyTagsToRead()
		{
			if (!(this.ObjectSchema is MapiObjectSchema))
			{
				throw new MapiInvalidOperationException(Strings.ExceptionSchemaInvalidCast(this.ObjectSchema.GetType().ToString()));
			}
			List<PropTag> list = new List<PropTag>(this.ObjectSchema.AllProperties.Count);
			foreach (PropertyDefinition propertyDefinition in this.ObjectSchema.AllProperties)
			{
				MapiPropertyDefinition mapiPropertyDefinition = (MapiPropertyDefinition)propertyDefinition;
				if (mapiPropertyDefinition.PropertyTag != PropTag.Null && !mapiPropertyDefinition.IsCalculated)
				{
					list.Add(mapiPropertyDefinition.PropertyTag);
				}
			}
			this.AdjustPropertyTagsToRead(list);
			return list.ToArray();
		}

		internal virtual void AdjustPropertyTagsToDelete(List<PropTag> propertyTags)
		{
		}

		internal PropTag[] GetPropertyTagsToDelete()
		{
			if (!(this.ObjectSchema is MapiObjectSchema))
			{
				throw new MapiInvalidOperationException(Strings.ExceptionSchemaInvalidCast(this.ObjectSchema.GetType().ToString()));
			}
			List<PropTag> list = new List<PropTag>(this.ObjectSchema.AllProperties.Count);
			foreach (PropertyDefinition propertyDefinition in this.ObjectSchema.AllProperties)
			{
				MapiPropertyDefinition mapiPropertyDefinition = (MapiPropertyDefinition)propertyDefinition;
				if (mapiPropertyDefinition.PropertyTag != PropTag.Null && !mapiPropertyDefinition.IsCalculated && !mapiPropertyDefinition.IsFilterOnly && !mapiPropertyDefinition.IsReadOnly)
				{
					bool flag = false;
					if (this.propertyBag.IsChanged(mapiPropertyDefinition))
					{
						object obj = null;
						if (this.propertyBag.TryGetField(mapiPropertyDefinition, ref obj) && !mapiPropertyDefinition.PersistDefaultValue && mapiPropertyDefinition.DefaultValue == obj)
						{
							list.Add(mapiPropertyDefinition.PropertyTag);
							flag = true;
						}
					}
					if (mapiPropertyDefinition.IsMandatory && flag)
					{
						throw new DataValidationException(new PropertyValidationError(Strings.ErrorMandatoryPropertyMissing(mapiPropertyDefinition.Name), mapiPropertyDefinition, null));
					}
				}
			}
			this.AdjustPropertyTagsToDelete(list);
			return list.ToArray();
		}

		internal virtual void AdjustPropertyValuesToUpdate(List<PropValue> propertyValues)
		{
		}

		internal PropValue[] GetPropertyValuesToUpdate()
		{
			if (!(this.ObjectSchema is MapiObjectSchema))
			{
				throw new MapiInvalidOperationException(Strings.ExceptionSchemaInvalidCast(this.ObjectSchema.GetType().ToString()));
			}
			List<PropValue> list = new List<PropValue>(this.ObjectSchema.AllProperties.Count);
			foreach (PropertyDefinition propertyDefinition in this.ObjectSchema.AllProperties)
			{
				MapiPropertyDefinition mapiPropertyDefinition = (MapiPropertyDefinition)propertyDefinition;
				if (mapiPropertyDefinition.PropertyTag != PropTag.Null && !mapiPropertyDefinition.IsCalculated && !mapiPropertyDefinition.IsFilterOnly && !mapiPropertyDefinition.IsReadOnly)
				{
					bool flag = false;
					if (base.ObjectState == ObjectState.New && !this.propertyBag.IsChanged(mapiPropertyDefinition) && (mapiPropertyDefinition.DefaultValue != mapiPropertyDefinition.InitialValue || mapiPropertyDefinition.PersistDefaultValue))
					{
						this.propertyBag[mapiPropertyDefinition] = mapiPropertyDefinition.InitialValue;
					}
					if (((MapiPropertyBag)this.propertyBag).IsChangedOrInitialized(mapiPropertyDefinition))
					{
						object obj = null;
						if (this.propertyBag.TryGetField(mapiPropertyDefinition, ref obj) && (mapiPropertyDefinition.DefaultValue != obj || mapiPropertyDefinition.PersistDefaultValue))
						{
							list.Add(mapiPropertyDefinition.Packer(obj, mapiPropertyDefinition));
							flag = true;
						}
					}
					if (mapiPropertyDefinition.IsMandatory && base.ObjectState == ObjectState.New && !flag)
					{
						throw new DataValidationException(new PropertyValidationError(Strings.ErrorMandatoryPropertyMissing(mapiPropertyDefinition.Name), mapiPropertyDefinition, null));
					}
				}
			}
			this.AdjustPropertyValuesToUpdate(list);
			return list.ToArray();
		}

		protected virtual MapiObject.UpdateIdentityFlags UpdateIdentityFlagsForCreating
		{
			get
			{
				return MapiObject.UpdateIdentityFlags.Default;
			}
		}

		protected virtual MapiObject.UpdateIdentityFlags UpdateIdentityFlagsForReading
		{
			get
			{
				return MapiObject.UpdateIdentityFlags.Default;
			}
		}

		protected virtual MapiObject.UpdateIdentityFlags UpdateIdentityFlagsForFinding
		{
			get
			{
				return MapiObject.UpdateIdentityFlags.Default;
			}
		}

		protected abstract MapiObject.RetrievePropertiesScope RetrievePropertiesScopeForFinding { get; }

		protected abstract void UpdateIdentity(MapiObject.UpdateIdentityFlags flags);

		public MapiObject() : base(new MapiPropertyBag())
		{
			this.disposeTracker = null;
			this.disposed = false;
		}

		internal MapiObject(MapiObjectId mapiObjectId, MapiSession mapiSession) : base(new MapiPropertyBag())
		{
			this.SetIdentity(mapiObjectId);
			this.MapiSession = mapiSession;
			this.disposeTracker = null;
			this.disposed = false;
		}

		private bool disposed;

		[NonSerialized]
		private MapiSession mapiSession;

		private Fqdn originatingServer;

		[NonSerialized]
		private DisposeTracker disposeTracker;

		[Flags]
		protected enum UpdateIdentityFlags
		{
			Nop = 0,
			EntryIdentity = 1,
			LegacyDistinguishedName = 2,
			FolderPath = 4,
			MailboxGuid = 8,
			All = 255,
			SkipIfExists = 256,
			Offline = 512,
			Default = 1023
		}

		protected enum RetrievePropertiesScope
		{
			Instance,
			Hierarchy,
			Database
		}
	}
}
