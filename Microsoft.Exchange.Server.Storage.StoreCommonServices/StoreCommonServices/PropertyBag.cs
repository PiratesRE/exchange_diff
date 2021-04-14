using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Server.Storage.Common;
using Microsoft.Exchange.Server.Storage.PhysicalAccess;
using Microsoft.Exchange.Server.Storage.PropertyBlob;
using Microsoft.Exchange.Server.Storage.PropTags;

namespace Microsoft.Exchange.Server.Storage.StoreCommonServices
{
	public abstract class PropertyBag : ISimplePropertyBagWithChangeTracking, ISimplePropertyBag, ISimpleReadOnlyPropertyBag, ISimplePropertyStorageWithChangeTracking, ISimplePropertyStorage, ISimpleReadOnlyPropertyStorage, ITWIR, IInstanceNumberOverride
	{
		protected PropertyBag(bool changeTrackingEnabled)
		{
			this.changeTrackingEnabled = changeTrackingEnabled;
		}

		public static string PropertyBlobToString(byte[] buffer)
		{
			StringBuilder stringBuilder = new StringBuilder(100);
			PropertyBlob.BlobReader blobReader = new PropertyBlob.BlobReader(buffer, 0);
			for (int i = 0; i < blobReader.PropertyCount; i++)
			{
				StorePropTag tag = StorePropTag.CreateWithoutInfo(blobReader.GetPropertyTag(i));
				object propertyValue = blobReader.GetPropertyValue(i);
				Property property = new Property(tag, propertyValue);
				property.AppendToString(stringBuilder);
				stringBuilder.Append(Environment.NewLine);
			}
			return stringBuilder.ToString();
		}

		public virtual bool IsDirty
		{
			get
			{
				return this.PropertiesDirty;
			}
		}

		public bool ChangeTrackingEnabled
		{
			get
			{
				return this.changeTrackingEnabled;
			}
		}

		public virtual bool IsChanged
		{
			get
			{
				return this.originalProperties != null && this.originalProperties.Count != 0;
			}
		}

		public IOriginalPropertyBag OriginalBag
		{
			get
			{
				if (this.originalBag == null)
				{
					this.originalBag = new PropertyBag.OriginalPropertyBag(this);
				}
				return this.originalBag;
			}
		}

		public abstract Context CurrentOperationContext { get; }

		public abstract ObjectPropertySchema Schema { get; }

		public abstract ReplidGuidMap ReplidGuidMap { get; }

		public bool NoReplicateOperationInProgress { get; set; }

		public int CountOfBlobProperties
		{
			get
			{
				if (this.Properties == null)
				{
					return 0;
				}
				return this.Properties.Count;
			}
		}

		protected abstract Dictionary<ushort, KeyValuePair<StorePropTag, object>> Properties { get; }

		protected abstract bool PropertiesDirty { get; set; }

		protected abstract void AssignPropertiesToUse(Dictionary<ushort, KeyValuePair<StorePropTag, object>> properties);

		public virtual object GetPropertyValue(Context context, StorePropTag propTag)
		{
			if (this.Schema == null)
			{
				return this.GetBlobPropertyValue(context, propTag);
			}
			return this.Schema.GetPropertyValue(context, this, propTag);
		}

		public virtual bool TryGetProperty(Context context, ushort propId, out StorePropTag propTag, out object value)
		{
			if (this.Schema == null)
			{
				return this.TryGetBlobProperty(context, propId, out propTag, out value);
			}
			return this.Schema.TryGetProperty(context, this, propId, out propTag, out value);
		}

		public virtual void EnumerateProperties(Context context, Func<StorePropTag, object, bool> action, bool showValue)
		{
			if (this.Schema == null)
			{
				this.EnumerateBlobProperties(context, action, showValue);
				return;
			}
			this.Schema.EnumerateProperties(context, this, action, showValue);
		}

		IReplidGuidMap ISimplePropertyBag.ReplidGuidMap
		{
			get
			{
				return this.ReplidGuidMap;
			}
		}

		public virtual ErrorCode SetProperty(Context context, StorePropTag propTag, object value)
		{
			if (this.Schema == null)
			{
				this.SetBlobProperty(context, propTag, value);
				return ErrorCode.NoError;
			}
			return this.Schema.SetProperty(context, this, propTag, value);
		}

		public virtual ErrorCode OpenPropertyReadStream(Context context, StorePropTag propTag, out Stream stream)
		{
			if (this.Schema == null)
			{
				stream = null;
				return ErrorCode.CreateNotSupported((LID)36632U, propTag.PropTag);
			}
			return this.Schema.OpenPropertyReadStream(context, this, propTag, out stream);
		}

		public virtual ErrorCode OpenPropertyWriteStream(Context context, StorePropTag propTag, out Stream stream)
		{
			if (this.Schema == null)
			{
				stream = null;
				return ErrorCode.CreateNotSupported((LID)38680U, propTag.PropTag);
			}
			return this.Schema.OpenPropertyWriteStream(context, this, propTag, out stream);
		}

		public virtual bool IsPropertyChanged(Context context, StorePropTag propTag)
		{
			if (this.Schema == null)
			{
				return this.IsBlobPropertyChanged(context, propTag);
			}
			return this.Schema.IsPropertyChanged(context, this, propTag);
		}

		public virtual object GetOriginalPropertyValue(Context context, StorePropTag propTag)
		{
			return this.OriginalBag.GetPropertyValue(context, propTag);
		}

		public virtual object GetBlobPropertyValue(Context context, StorePropTag propTag)
		{
			if (this.Properties != null)
			{
				KeyValuePair<StorePropTag, object> keyValuePair;
				bool flag = this.Properties.TryGetValue(propTag.PropId, out keyValuePair);
				if (flag && keyValuePair.Key == propTag)
				{
					return keyValuePair.Value;
				}
			}
			return null;
		}

		public virtual bool TryGetBlobProperty(Context context, ushort propId, out StorePropTag propTag, out object value)
		{
			KeyValuePair<StorePropTag, object> keyValuePair;
			if (this.Properties != null && this.Properties.TryGetValue(propId, out keyValuePair))
			{
				propTag = keyValuePair.Key;
				value = keyValuePair.Value;
				return true;
			}
			propTag = StorePropTag.Invalid;
			value = null;
			return false;
		}

		public virtual object GetPhysicalColumnValue(Context context, PhysicalColumn column)
		{
			return null;
		}

		public virtual void EnumerateBlobProperties(Context context, Func<StorePropTag, object, bool> action, bool showValue)
		{
			if (this.Properties != null)
			{
				foreach (KeyValuePair<ushort, KeyValuePair<StorePropTag, object>> keyValuePair in this.Properties)
				{
					if (!action(keyValuePair.Value.Key, showValue ? keyValuePair.Value.Value : null))
					{
						break;
					}
				}
			}
		}

		public virtual void SetBlobProperty(Context context, StorePropTag propTag, object value)
		{
			this.SetBlobProperty(context, propTag, value, false);
		}

		protected void SetBlobProperty(Context context, StorePropTag propTag, object value, bool keepNulls)
		{
			KeyValuePair<StorePropTag, object> keyValuePair = default(KeyValuePair<StorePropTag, object>);
			bool flag = this.Properties != null && this.Properties.TryGetValue(propTag.PropId, out keyValuePair);
			if ((flag && propTag == keyValuePair.Key && ValueHelper.ValuesEqual(keyValuePair.Value, value)) || (value == null && (!flag || keyValuePair.Value == null)))
			{
				return;
			}
			if (value == null && !keepNulls)
			{
				this.Properties.Remove(propTag.PropId);
			}
			else
			{
				if (this.Properties == null)
				{
					this.AssignPropertiesToUse(new Dictionary<ushort, KeyValuePair<StorePropTag, object>>(20));
				}
				this.Properties[propTag.PropId] = new KeyValuePair<StorePropTag, object>(propTag, value);
			}
			if (this.changeTrackingEnabled)
			{
				if (this.originalProperties == null)
				{
					this.originalProperties = new Dictionary<StorePropTag, object>(20);
				}
				if (!flag || keyValuePair.Value == null || propTag != keyValuePair.Key)
				{
					object y;
					if (!this.originalProperties.TryGetValue(propTag, out y))
					{
						this.originalProperties.Add(propTag, null);
					}
					else if (ValueHelper.ValuesEqual(value, y))
					{
						this.originalProperties.Remove(propTag);
					}
				}
				if (flag && keyValuePair.Value != null)
				{
					object x = (propTag == keyValuePair.Key) ? value : null;
					object y;
					if (!this.originalProperties.TryGetValue(keyValuePair.Key, out y))
					{
						this.originalProperties.Add(keyValuePair.Key, keyValuePair.Value);
					}
					else if (ValueHelper.ValuesEqual(x, y))
					{
						this.originalProperties.Remove(keyValuePair.Key);
					}
				}
			}
			if (!this.IsDirty)
			{
				this.OnDirty(context);
			}
			this.PropertiesDirty = true;
			if (!flag || keyValuePair.Value == null)
			{
				this.OnPropertyChanged(propTag, PropertyBag.GetPropertySize(propTag, value));
				return;
			}
			if (propTag == keyValuePair.Key || value == null)
			{
				long deltaSize = PropertyBag.GetPropertySize(propTag, value) - PropertyBag.GetPropertySize(keyValuePair.Key, keyValuePair.Value);
				this.OnPropertyChanged(keyValuePair.Key, deltaSize);
				return;
			}
			this.OnPropertyChanged(keyValuePair.Key, -PropertyBag.GetPropertySize(keyValuePair.Key, keyValuePair.Value));
			this.OnPropertyChanged(propTag, PropertyBag.GetPropertySize(propTag, value));
		}

		public virtual void SetPhysicalColumn(Context context, PhysicalColumn column, object value)
		{
			throw new NotSupportedException("physical column mapping not supported by a property bag without a table");
		}

		public virtual ErrorCode OpenPhysicalColumnReadStream(Context context, PhysicalColumn column, out Stream stream)
		{
			stream = null;
			return ErrorCode.CreateNotSupported((LID)40728U);
		}

		public virtual ErrorCode OpenPhysicalColumnWriteStream(Context context, PhysicalColumn column, out Stream stream)
		{
			stream = null;
			return ErrorCode.CreateNotSupported((LID)57112U);
		}

		ISimpleReadOnlyPropertyBag ISimplePropertyStorageWithChangeTracking.OriginalBag
		{
			get
			{
				return this.OriginalBag;
			}
		}

		public virtual bool IsBlobPropertyChanged(Context context, StorePropTag propTag)
		{
			return this.originalProperties != null && this.originalProperties.ContainsKey(propTag);
		}

		public virtual bool IsPhysicalColumnChanged(Context context, PhysicalColumn column)
		{
			return false;
		}

		public virtual object GetOriginalBlobPropertyValue(Context context, StorePropTag propTag)
		{
			object result = null;
			if (this.originalProperties == null || !this.originalProperties.TryGetValue(propTag, out result))
			{
				result = this.GetBlobPropertyValue(context, propTag);
			}
			return result;
		}

		public virtual bool TryGetOriginalBlobProperty(Context context, ushort propId, out StorePropTag propTag, out object value)
		{
			throw new NotSupportedException("TryGetProperty by ID for original property is not supported");
		}

		public virtual object GetOriginalPhysicalColumnValue(Context context, PhysicalColumn column)
		{
			return null;
		}

		public void EnumerateOriginalBlobProperties(Context context, Func<StorePropTag, object, bool> action, bool showValue)
		{
			if (this.Properties != null)
			{
				foreach (KeyValuePair<ushort, KeyValuePair<StorePropTag, object>> keyValuePair in this.Properties)
				{
					object obj = null;
					if (this.originalProperties == null || !this.originalProperties.TryGetValue(keyValuePair.Value.Key, out obj))
					{
						obj = keyValuePair.Value.Value;
					}
					if (obj != null && !action(keyValuePair.Value.Key, showValue ? obj : null))
					{
						return;
					}
				}
			}
			if (this.originalProperties != null)
			{
				foreach (KeyValuePair<StorePropTag, object> keyValuePair2 in this.originalProperties)
				{
					KeyValuePair<StorePropTag, object> keyValuePair3;
					if ((this.Properties == null || !this.Properties.TryGetValue(keyValuePair2.Key.PropId, out keyValuePair3) || keyValuePair3.Key != keyValuePair2.Key) && keyValuePair2.Value != null && !action(keyValuePair2.Key, showValue ? keyValuePair2.Value : null))
					{
						break;
					}
				}
			}
		}

		public void SetInstanceNumber(Context context, object instanceNumber)
		{
			this.instanceNumber = instanceNumber;
		}

		public object GetInstanceNumberOverride()
		{
			return this.instanceNumber;
		}

		int ITWIR.GetColumnSize(Column column)
		{
			return ((IColumn)column).GetSize(this);
		}

		object ITWIR.GetColumnValue(Column column)
		{
			return ((IColumn)column).GetValue(this);
		}

		int ITWIR.GetPhysicalColumnSize(PhysicalColumn column)
		{
			return 0;
		}

		object ITWIR.GetPhysicalColumnValue(PhysicalColumn column)
		{
			return this.GetPhysicalColumnValue(this.CurrentOperationContext, column);
		}

		int ITWIR.GetPropertyColumnSize(PropertyColumn column)
		{
			return 0;
		}

		object ITWIR.GetPropertyColumnValue(PropertyColumn column)
		{
			return this.GetPropertyValue(this.CurrentOperationContext, column.StorePropTag);
		}

		public virtual void Scrub(Context context)
		{
			if (this.Properties != null && this.Properties.Count != 0)
			{
				StorePropTag[] array = new StorePropTag[this.Properties.Count];
				int num = 0;
				foreach (KeyValuePair<ushort, KeyValuePair<StorePropTag, object>> keyValuePair in this.Properties)
				{
					array[num++] = keyValuePair.Value.Key;
				}
				foreach (StorePropTag propTag in array)
				{
					this.SetBlobProperty(context, propTag, null);
				}
			}
		}

		public virtual void DiscardPrivateCache(Context context)
		{
			this.originalProperties = null;
			this.PropertiesDirty = false;
		}

		public virtual void MakeClean()
		{
			this.PropertiesDirty = false;
		}

		public virtual void CommitChanges()
		{
			this.MakeClean();
			if (this.changeTrackingEnabled && this.originalProperties != null)
			{
				this.originalProperties.Clear();
			}
		}

		public virtual ulong GetChangedPropertyGroups(Context context)
		{
			ulong num = 0UL;
			if (this.originalProperties != null)
			{
				foreach (StorePropTag storePropTag in this.originalProperties.Keys)
				{
					num |= storePropTag.GroupMask;
				}
			}
			return num;
		}

		protected bool TryGetBlobPropertyValue(Context context, StorePropTag propTag, out object propValue)
		{
			propValue = null;
			if (this.Properties != null)
			{
				KeyValuePair<StorePropTag, object> keyValuePair;
				bool flag = this.Properties.TryGetValue(propTag.PropId, out keyValuePair);
				if (flag)
				{
					if (keyValuePair.Key == propTag)
					{
						propValue = keyValuePair.Value;
					}
					return true;
				}
			}
			return false;
		}

		protected virtual void OnPropertyChanged(StorePropTag propTag, long deltaSize)
		{
		}

		protected virtual void OnDirty(Context context)
		{
		}

		protected void LoadFromPropertyBlob(Context context, byte[] propertyBlob)
		{
			Dictionary<ushort, KeyValuePair<StorePropTag, object>> properties = null;
			this.LoadPropertiesFromPropertyBlob(context, propertyBlob, ref properties);
			this.AssignPropertiesToUse(properties);
		}

		protected byte[] SaveToPropertyBlob(Context context)
		{
			return PropertyBlob.BuildBlob(this.Properties);
		}

		protected void LoadPropertiesFromPropertyBlob(Context context, byte[] propertyBlob, ref Dictionary<ushort, KeyValuePair<StorePropTag, object>> properties)
		{
			if (propertyBlob != null)
			{
				PropertyBlob.BlobReader blobReader = new PropertyBlob.BlobReader(propertyBlob, 0);
				for (int i = 0; i < blobReader.PropertyCount; i++)
				{
					StorePropTag key = this.MapPropTag(context, blobReader.GetPropertyTag(i));
					object propertyValue = blobReader.GetPropertyValue(i);
					if (properties == null)
					{
						properties = new Dictionary<ushort, KeyValuePair<StorePropTag, object>>(blobReader.PropertyCount);
					}
					KeyValuePair<StorePropTag, object> keyValuePair;
					if (!properties.TryGetValue(key.PropId, out keyValuePair) || keyValuePair.Value is ValueReference)
					{
						properties[key.PropId] = new KeyValuePair<StorePropTag, object>(key, propertyValue);
					}
				}
			}
		}

		protected void LoadPropertiesFromPropertyBlobStream(Context context, Stream propertyBlobStream, ref Dictionary<ushort, KeyValuePair<StorePropTag, object>> properties)
		{
			if (propertyBlobStream != null)
			{
				using (Stream stream = new BufferedStream(propertyBlobStream, 8192))
				{
					PropertyBlob.BlobStreamReader blobStreamReader = new PropertyBlob.BlobStreamReader(stream);
					foreach (KeyValuePair<uint, object> keyValuePair in blobStreamReader.LoadProperties(true, true))
					{
						StorePropTag key = this.MapPropTag(context, keyValuePair.Key);
						if (properties == null)
						{
							properties = new Dictionary<ushort, KeyValuePair<StorePropTag, object>>();
						}
						KeyValuePair<StorePropTag, object> keyValuePair2;
						if (!properties.TryGetValue(key.PropId, out keyValuePair2) || keyValuePair2.Value is ValueReference)
						{
							properties[key.PropId] = new KeyValuePair<StorePropTag, object>(key, keyValuePair.Value);
						}
					}
				}
			}
		}

		protected abstract StorePropTag MapPropTag(Context context, uint propertyTag);

		private static long GetPropertySize(StorePropTag propTag, object value)
		{
			if (value == null)
			{
				return 0L;
			}
			return (long)ValueTypeHelper.ValueSize(PropertyTypeHelper.GetExtendedTypeCode(propTag.PropType), value);
		}

		public const int PropertyBlobStreamBlockSize = 8192;

		private const int AverageCustomPropertyNumber = 20;

		private bool changeTrackingEnabled;

		private Dictionary<StorePropTag, object> originalProperties;

		private IOriginalPropertyBag originalBag;

		private object instanceNumber;

		private class OriginalPropertyBag : IOriginalPropertyBag, ISimpleReadOnlyPropertyBag, ISimpleReadOnlyPropertyStorage, ITWIR, IInstanceNumberOverride
		{
			public OriginalPropertyBag(PropertyBag propbag)
			{
				this.propbag = propbag;
			}

			public object GetInstanceNumberOverride()
			{
				return this.propbag.GetInstanceNumberOverride();
			}

			public object GetPropertyValue(Context context, StorePropTag propTag)
			{
				if (this.propbag.Schema == null)
				{
					return this.propbag.GetOriginalBlobPropertyValue(context, propTag);
				}
				return this.propbag.Schema.GetPropertyValue(context, this, propTag);
			}

			public bool TryGetProperty(Context context, ushort propId, out StorePropTag propTag, out object value)
			{
				if (this.propbag.Schema == null)
				{
					return this.propbag.TryGetOriginalBlobProperty(context, propId, out propTag, out value);
				}
				return this.propbag.Schema.TryGetProperty(context, this, propId, out propTag, out value);
			}

			public void EnumerateProperties(Context context, Func<StorePropTag, object, bool> action, bool showValue)
			{
				if (this.propbag.Schema == null)
				{
					this.propbag.EnumerateOriginalBlobProperties(context, action, showValue);
					return;
				}
				this.propbag.Schema.EnumerateProperties(context, this, action, showValue);
			}

			public object GetBlobPropertyValue(Context context, StorePropTag propTag)
			{
				return this.propbag.GetOriginalBlobPropertyValue(context, propTag);
			}

			public bool TryGetBlobProperty(Context context, ushort propId, out StorePropTag propTag, out object value)
			{
				return this.propbag.TryGetOriginalBlobProperty(context, propId, out propTag, out value);
			}

			public object GetPhysicalColumnValue(Context context, PhysicalColumn column)
			{
				return this.propbag.GetOriginalPhysicalColumnValue(context, column);
			}

			public void EnumerateBlobProperties(Context context, Func<StorePropTag, object, bool> action, bool showValue)
			{
				this.propbag.EnumerateOriginalBlobProperties(context, action, showValue);
			}

			int ITWIR.GetColumnSize(Column column)
			{
				return ((IColumn)column).GetSize(this);
			}

			object ITWIR.GetColumnValue(Column column)
			{
				return ((IColumn)column).GetValue(this);
			}

			int ITWIR.GetPhysicalColumnSize(PhysicalColumn column)
			{
				return 0;
			}

			object ITWIR.GetPhysicalColumnValue(PhysicalColumn column)
			{
				return this.GetPhysicalColumnValue(this.propbag.CurrentOperationContext, column);
			}

			int ITWIR.GetPropertyColumnSize(PropertyColumn column)
			{
				return 0;
			}

			object ITWIR.GetPropertyColumnValue(PropertyColumn column)
			{
				return this.GetPropertyValue(this.propbag.CurrentOperationContext, column.StorePropTag);
			}

			private PropertyBag propbag;
		}
	}
}
