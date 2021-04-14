using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Data.Storage;
using Microsoft.Mapi;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal abstract class PersistablePropertyBag : PropertyBag, ICorePropertyBag, ILocationIdentifierSetter, IDisposeTrackable, IDisposable
	{
		protected PersistablePropertyBag()
		{
			StorageGlobals.TraceConstructIDisposable(this);
			this.disposeTracker = this.GetDisposeTracker();
		}

		public DisposeTracker DisposeTracker
		{
			get
			{
				return this.disposeTracker;
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

		protected void CheckDisposed(string methodName)
		{
			if (this.isDisposed)
			{
				StorageGlobals.TraceFailedCheckDisposed(this, methodName);
				throw new ObjectDisposedException(base.GetType().ToString());
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
			if (disposing && this.disposeTracker != null)
			{
				this.disposeTracker.Dispose();
			}
		}

		public abstract ICollection<PropertyDefinition> AllFoundProperties { get; }

		public virtual void Reload()
		{
			throw new NotSupportedException("Currently this is only supported by AcrPropertyBag and StoreObjectPropertyBag.");
		}

		internal virtual ICollection<PropertyDefinition> PrefetchPropertyArray
		{
			get
			{
				this.CheckDisposed("PrefetchPropertyArray::get");
				return this.prefetchProperties;
			}
			set
			{
				this.CheckDisposed("PrefetchPropertyArray::set");
				this.prefetchProperties = (value ?? PersistablePropertyBag.empty);
			}
		}

		internal abstract void FlushChanges();

		internal abstract void SaveChanges(bool force);

		internal abstract ICollection<NativeStorePropertyDefinition> AllNativeProperties { get; }

		public abstract bool HasAllPropertiesLoaded { get; }

		public abstract void Clear();

		internal abstract PropertyBagSaveFlags SaveFlags { get; set; }

		internal abstract void SetUpdateImapIdFlag();

		public abstract Stream OpenPropertyStream(PropertyDefinition propertyDefinition, PropertyOpenMode openMode);

		internal abstract MapiProp MapiProp { get; }

		T ICorePropertyBag.GetValueOrDefault<T>(StorePropertyDefinition propertyDefinition)
		{
			return base.GetValueOrDefault<T>(propertyDefinition, default(T));
		}

		T ICorePropertyBag.GetValueOrDefault<T>(StorePropertyDefinition propertyDefinition, T defaultValue)
		{
			return PropertyBag.CheckPropertyValue<T>(propertyDefinition, base.TryGetProperty(propertyDefinition), defaultValue);
		}

		T? ICorePropertyBag.GetValueAsNullable<T>(StorePropertyDefinition propertyDefinition)
		{
			return PropertyBag.CheckNullablePropertyValue<T>(propertyDefinition, base.TryGetProperty(propertyDefinition));
		}

		internal static void CopyProperty(PersistablePropertyBag source, PropertyDefinition property, PersistablePropertyBag destination)
		{
			object obj = source.TryGetProperty(property);
			PropertyError propertyError = obj as PropertyError;
			if (propertyError == null)
			{
				destination[property] = obj;
				return;
			}
			if (PropertyError.IsPropertyValueTooBig(propertyError))
			{
				Stream stream;
				Stream readStream = stream = source.OpenPropertyStream(property, PropertyOpenMode.ReadOnly);
				try
				{
					Stream stream2;
					Stream writeStream = stream2 = destination.OpenPropertyStream(property, PropertyOpenMode.Create);
					try
					{
						Util.StreamHandler.CopyStreamData(readStream, writeStream);
					}
					finally
					{
						if (stream2 != null)
						{
							((IDisposable)stream2).Dispose();
						}
					}
				}
				finally
				{
					if (stream != null)
					{
						((IDisposable)stream).Dispose();
					}
				}
			}
		}

		internal static void CopyProperties(PersistablePropertyBag source, PersistablePropertyBag destination, params PropertyDefinition[] properties)
		{
			foreach (PropertyDefinition property in properties)
			{
				PersistablePropertyBag.CopyProperty(source, property, destination);
			}
		}

		internal static PersistablePropertyBag GetPersistablePropertyBag(ICorePropertyBag corePropertyBag)
		{
			return (PersistablePropertyBag)corePropertyBag;
		}

		internal byte[] GetLargeBinaryProperty(PropertyDefinition propertyDefinition)
		{
			object obj = base.TryGetProperty(propertyDefinition);
			byte[] array = obj as byte[];
			if (array != null)
			{
				return array;
			}
			if (PropertyError.IsPropertyValueTooBig(obj))
			{
				ExTraceGlobals.StorageTracer.Information<PropertyDefinition>((long)this.GetHashCode(), "PersitablePropertyBag::GetLargeBinaryProperty, {0} too big to fit in GetProp, streaming it", propertyDefinition);
				using (Stream stream = this.OpenPropertyStream(propertyDefinition, PropertyOpenMode.ReadOnly))
				{
					return Util.ReadByteArray(stream);
				}
			}
			PropertyError propertyError = obj as PropertyError;
			if (propertyError != null && propertyError.PropertyErrorCode == PropertyErrorCode.NotFound)
			{
				ExTraceGlobals.StorageTracer.Information<PropertyDefinition>((long)this.GetHashCode(), "PersitablePropertyBag::GetLargeBinaryProperty, {0} not found", propertyDefinition);
				return null;
			}
			ExTraceGlobals.StorageTracer.TraceError<PropertyDefinition>((long)this.GetHashCode(), "PersitablePropertyBag::GetLargeBinaryProperty, Error when accessing {0}", propertyDefinition);
			throw new CorruptDataException(ServerStrings.ErrorAccessingLargeProperty);
		}

		private static ICollection<PropertyDefinition> empty = Array<PropertyDefinition>.Empty;

		private ICollection<PropertyDefinition> prefetchProperties = PersistablePropertyBag.empty;

		private bool isDisposed;

		private readonly DisposeTracker disposeTracker;
	}
}
