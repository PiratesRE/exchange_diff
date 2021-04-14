using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Data.Storage;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Net;
using Microsoft.Mapi;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal sealed class InMemoryPersistablePropertyBag : PersistablePropertyBag
	{
		internal InMemoryPersistablePropertyBag(ICollection<PropertyDefinition> propsToReturn)
		{
			this.isDirty = false;
			this.memoryPropertyBag = new MemoryPropertyBag();
			if (propsToReturn != null)
			{
				this.Load(propsToReturn);
			}
		}

		public override DisposeTracker GetDisposeTracker()
		{
			return DisposeTracker.Get<InMemoryPersistablePropertyBag>(this);
		}

		public override Stream OpenPropertyStream(PropertyDefinition propertyDefinition, PropertyOpenMode openMode)
		{
			Stream stream = null;
			StorePropertyDefinition storePropertyDefinition = InternalSchema.ToStorePropertyDefinition(propertyDefinition);
			if (openMode != PropertyOpenMode.Create)
			{
				object value = ((IDirectPropertyBag)this.memoryPropertyBag).GetValue(storePropertyDefinition);
				PropertyError propertyError = value as PropertyError;
				if (propertyError == null)
				{
					stream = this.WrapValueInStream(value);
				}
				else if (propertyError.PropertyErrorCode == PropertyErrorCode.RequireStreamed)
				{
					stream = this.streamList[storePropertyDefinition];
				}
				else if (openMode == PropertyOpenMode.ReadOnly)
				{
					throw new ObjectNotFoundException(ServerStrings.StreamPropertyNotFound(storePropertyDefinition.ToString()));
				}
			}
			if (stream == null)
			{
				stream = new MemoryStream();
				((IDirectPropertyBag)this.memoryPropertyBag).SetValue(storePropertyDefinition, new PropertyError(storePropertyDefinition, PropertyErrorCode.RequireStreamed));
				this.streamList[storePropertyDefinition] = stream;
			}
			if (openMode != PropertyOpenMode.ReadOnly)
			{
				this.isDirty = true;
			}
			stream.Seek(0L, SeekOrigin.Begin);
			return new StreamWrapper(stream, false);
		}

		public override PropertyValueTrackingData GetOriginalPropertyInformation(PropertyDefinition propertyDefinition)
		{
			return new PropertyValueTrackingData(PropertyTrackingInformation.Unchanged, null);
		}

		internal override void FlushChanges()
		{
			this.isDirty = false;
			this.memoryPropertyBag.ClearChangeInfo();
		}

		internal override void SaveChanges(bool force)
		{
		}

		internal override PropertyBagSaveFlags SaveFlags
		{
			get
			{
				return this.saveFlags;
			}
			set
			{
				EnumValidator.ThrowIfInvalid<PropertyBagSaveFlags>(value, "value");
				this.saveFlags = value;
			}
		}

		internal override void SetUpdateImapIdFlag()
		{
		}

		internal override MapiProp MapiProp
		{
			get
			{
				return null;
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

		public override void Load(ICollection<PropertyDefinition> propsToLoad)
		{
			if (propsToLoad == InternalSchema.ContentConversionProperties)
			{
				this.memoryPropertyBag.SetAllPropertiesLoaded();
				return;
			}
			this.memoryPropertyBag.Load(propsToLoad);
		}

		public override void Clear()
		{
			this.memoryPropertyBag.Clear();
		}

		protected override void DeleteStoreProperty(StorePropertyDefinition propertyDefinition)
		{
			((IDirectPropertyBag)this.memoryPropertyBag).Delete(propertyDefinition);
			this.isDirty = true;
		}

		protected override void SetValidatedStoreProperty(StorePropertyDefinition propertyDefinition, object propertyValue)
		{
			((IDirectPropertyBag)this.memoryPropertyBag).SetValue(propertyDefinition, propertyValue);
			this.isDirty = true;
		}

		protected override object TryGetStoreProperty(StorePropertyDefinition propertyDefinition)
		{
			return ((IDirectPropertyBag)this.memoryPropertyBag).GetValue(propertyDefinition);
		}

		public override bool IsDirty
		{
			get
			{
				return this.isDirty;
			}
		}

		protected override bool InternalIsPropertyDirty(AtomicStorePropertyDefinition propertyDefinition)
		{
			return ((IDirectPropertyBag)this.memoryPropertyBag).IsDirty(propertyDefinition);
		}

		public override ICollection<PropertyDefinition> AllFoundProperties
		{
			get
			{
				return this.memoryPropertyBag.AllFoundProperties;
			}
		}

		private Stream WrapValueInStream(object value)
		{
			MemoryStream memoryStream = new MemoryStream();
			if (value != null)
			{
				byte[] array = value as byte[];
				if (array != null)
				{
					memoryStream.Write(array, 0, array.Length);
					memoryStream.Position = 0L;
				}
				else
				{
					string text = value as string;
					if (text == null)
					{
						ExTraceGlobals.StorageTracer.TraceError<Type>((long)this.GetHashCode(), "InMemoryPersistablePropertyBag::WrapValueInStream. The property value cannot be streamed. Type = {0}.", value.GetType());
						throw new InvalidOperationException(ServerStrings.NotStreamablePropertyValue(value.GetType()));
					}
					byte[] bytes = ConvertUtils.UnicodeEncoding.GetBytes(text);
					memoryStream.Write(bytes, 0, bytes.Length);
					memoryStream.Position = 0L;
				}
			}
			return memoryStream;
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
			}
		}

		protected override bool IsLoaded(NativeStorePropertyDefinition propertyDefinition)
		{
			return ((IDirectPropertyBag)this.memoryPropertyBag).IsLoaded(propertyDefinition);
		}

		private MemoryPropertyBag memoryPropertyBag;

		private readonly Dictionary<PropertyDefinition, Stream> streamList = new Dictionary<PropertyDefinition, Stream>();

		private bool isDirty;

		private PropertyBagSaveFlags saveFlags;
	}
}
