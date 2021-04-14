using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Server.Storage.Common;
using Microsoft.Exchange.Server.Storage.PhysicalAccess;
using Microsoft.Exchange.Server.Storage.PropTags;

namespace Microsoft.Exchange.Server.Storage.StoreCommonServices
{
	public class ObjectPropertySchema
	{
		public ObjectPropertySchema()
		{
		}

		public ObjectPropertySchema(ObjectType objectType, Table table, Dictionary<StorePropTag, PropertyMapping> mappedProperties, Func<IRowAccess, IRowPropertyBag> rowPropBagCreator)
		{
			this.Initialize(objectType, table, mappedProperties, rowPropBagCreator, null);
		}

		public void Initialize(ObjectType objectType, Table table, Dictionary<StorePropTag, PropertyMapping> mappedProperties, Func<IRowAccess, IRowPropertyBag> rowPropBagCreator, ObjectPropertySchema baseSchema)
		{
			this.objectType = objectType;
			this.table = table;
			this.rowPropBagCreator = rowPropBagCreator;
			this.mappedProperties = mappedProperties;
			this.baseSchema = baseSchema;
			this.propertyIdMapping = new Dictionary<ushort, PropertyMapping>(Math.Max(mappedProperties.Count, 1));
			foreach (PropertyMapping propertyMapping in this.mappedProperties.Values)
			{
				if (propertyMapping.IsPrimary)
				{
					this.propertyIdMapping.Add(propertyMapping.PropertyTag.PropId, propertyMapping);
				}
			}
			foreach (PropertyMapping propertyMapping2 in this.mappedProperties.Values)
			{
				if (!this.propertyIdMapping.ContainsKey(propertyMapping2.PropertyTag.PropId))
				{
					this.propertyIdMapping.Add(propertyMapping2.PropertyTag.PropId, propertyMapping2);
				}
			}
			if (objectType == ObjectType.Message)
			{
				this.columnGroups = new ulong[table.Columns.Count];
				foreach (PropertyMapping propertyMapping3 in this.mappedProperties.Values)
				{
					if (propertyMapping3.MappingKind == PropertyMappingKind.PhysicalColumn)
					{
						PhysicalColumn physicalColumn = (PhysicalColumn)propertyMapping3.Column.ActualColumn;
						if (physicalColumn.Index >= 0)
						{
							this.columnGroups[physicalColumn.Index] = propertyMapping3.PropertyTag.GroupMask;
						}
					}
				}
			}
		}

		public Func<IRowAccess, IRowPropertyBag> RowPropBagCreator
		{
			get
			{
				return this.rowPropBagCreator;
			}
		}

		public Table Table
		{
			get
			{
				return this.table;
			}
		}

		public ObjectType ObjectType
		{
			get
			{
				return this.objectType;
			}
		}

		public ObjectType BaseObjectType
		{
			get
			{
				return WellKnownProperties.BaseObjectType[(int)this.objectType];
			}
		}

		public ulong[] ColumnGroups
		{
			get
			{
				return this.columnGroups;
			}
		}

		public PropertyMapping FindMapping(StorePropTag propertyTag)
		{
			PropertyMapping result;
			if (!this.mappedProperties.TryGetValue(propertyTag, out result))
			{
				result = null;
				if (this.BaseSchema != null && this.BaseSchema.Table == this.Table)
				{
					result = this.BaseSchema.FindMapping(propertyTag);
				}
			}
			return result;
		}

		public bool TryGetPrimaryMapping(ushort propId, out PropertyMapping primaryMapping)
		{
			return this.propertyIdMapping.TryGetValue(propId, out primaryMapping);
		}

		public void EnumerateMappings(Action<PropertyMapping> action)
		{
			foreach (PropertyMapping obj in this.mappedProperties.Values)
			{
				action(obj);
			}
		}

		public object GetPropertyValue(Context context, ISimpleReadOnlyPropertyBag bag, StorePropTag propTag)
		{
			PropertyMapping propertyMapping = this.FindMapping(propTag);
			PropertyMapping propertyMapping2;
			if (propertyMapping == null && this.TryGetPrimaryMapping(propTag.PropId, out propertyMapping2) && propertyMapping2.IsReservedPropId)
			{
				return null;
			}
			if (propertyMapping == null)
			{
				return bag.GetBlobPropertyValue(context, propTag);
			}
			return propertyMapping.GetPropertyValue(context, bag);
		}

		public bool TryGetProperty(Context context, ISimpleReadOnlyPropertyBag bag, ushort propId, out StorePropTag propTag, out object value)
		{
			PropertyMapping propertyMapping;
			bool flag;
			if (this.TryGetPrimaryMapping(propId, out propertyMapping) && propertyMapping.IsReservedPropId)
			{
				propTag = propertyMapping.PropertyTag;
				value = propertyMapping.GetPropertyValue(context, bag);
				flag = (value != null);
			}
			else
			{
				flag = bag.TryGetBlobProperty(context, propId, out propTag, out value);
				if (!flag && propertyMapping != null && propertyMapping.MappingKind != PropertyMappingKind.Default)
				{
					propTag = propertyMapping.PropertyTag;
					value = propertyMapping.GetPropertyValue(context, bag);
					flag = (value != null);
				}
			}
			return flag;
		}

		public ErrorCode SetProperty(Context context, ISimplePropertyBag bag, StorePropTag propTag, object value)
		{
			PropertyMapping propertyMapping = this.FindMapping(propTag);
			PropertyMapping propertyMapping2;
			if (propertyMapping == null && this.TryGetPrimaryMapping(propTag.PropId, out propertyMapping2) && propertyMapping2.IsReservedPropId)
			{
				return ErrorCode.CreateUnexpectedType((LID)53016U, propTag.PropTag);
			}
			if (propertyMapping == null)
			{
				bag.SetBlobProperty(context, propTag, value);
				return ErrorCode.NoError;
			}
			if (!propertyMapping.CanBeSet)
			{
				return ErrorCode.CreateNoAccess((LID)46872U, propTag.PropTag);
			}
			return propertyMapping.SetPropertyValue(context, bag, value);
		}

		public bool IsPropertyChanged(Context context, ISimplePropertyBagWithChangeTracking bag, StorePropTag propTag)
		{
			PropertyMapping propertyMapping = this.FindMapping(propTag);
			PropertyMapping propertyMapping2;
			if (propertyMapping == null && this.TryGetPrimaryMapping(propTag.PropId, out propertyMapping2) && propertyMapping2.IsReservedPropId)
			{
				return false;
			}
			if (propertyMapping == null)
			{
				return bag.IsBlobPropertyChanged(context, propTag);
			}
			return propertyMapping.IsPropertyChanged(context, bag);
		}

		public void EnumerateProperties(Context context, ISimpleReadOnlyPropertyBag bag, Func<StorePropTag, object, bool> action, bool showValue)
		{
			foreach (PropertyMapping propertyMapping in this.mappedProperties.Values)
			{
				if (propertyMapping.ShouldBeListed && propertyMapping.MappingKind != PropertyMappingKind.Default)
				{
					object propertyValue = propertyMapping.GetPropertyValue(context, bag);
					if (propertyValue != null && !action(propertyMapping.PropertyTag, showValue ? propertyValue : null))
					{
						return;
					}
				}
			}
			bag.EnumerateBlobProperties(context, delegate(StorePropTag propTag, object propValue)
			{
				PropertyMapping propertyMapping2;
				return (this.mappedProperties.TryGetValue(propTag, out propertyMapping2) && (propertyMapping2.MappingKind != PropertyMappingKind.Default || !propertyMapping2.ShouldBeListed)) || action(propTag, propValue);
			}, showValue);
		}

		public ErrorCode OpenPropertyReadStream(Context context, ISimplePropertyBag bag, StorePropTag propTag, out Stream readStream)
		{
			PropertyMapping propertyMapping = this.FindMapping(propTag);
			PropertyMapping propertyMapping2;
			if (propertyMapping == null && this.TryGetPrimaryMapping(propTag.PropId, out propertyMapping2) && propertyMapping2.IsReservedPropId)
			{
				readStream = null;
				return ErrorCode.CreateNotFound((LID)63256U, propTag.PropTag);
			}
			if (propertyMapping == null)
			{
				readStream = null;
				return ErrorCode.CreateNotSupported((LID)55064U, propTag.PropTag);
			}
			return propertyMapping.OpenPropertyReadStream(context, bag, out readStream);
		}

		public ErrorCode OpenPropertyWriteStream(Context context, ISimplePropertyBag bag, StorePropTag propTag, out Stream writeStream)
		{
			PropertyMapping propertyMapping = this.FindMapping(propTag);
			PropertyMapping propertyMapping2;
			if (propertyMapping == null && this.TryGetPrimaryMapping(propTag.PropId, out propertyMapping2) && propertyMapping2.IsReservedPropId)
			{
				writeStream = null;
				return ErrorCode.CreateNoAccess((LID)42776U, propTag.PropTag);
			}
			if (propertyMapping == null)
			{
				writeStream = null;
				return ErrorCode.CreateNotSupported((LID)59160U, propTag.PropTag);
			}
			if (!propertyMapping.CanBeSet)
			{
				writeStream = null;
				return ErrorCode.CreateNoAccess((LID)50968U, propTag.PropTag);
			}
			return propertyMapping.OpenPropertyWriteStream(context, bag, out writeStream);
		}

		internal ObjectPropertySchema BaseSchema
		{
			get
			{
				return this.baseSchema;
			}
		}

		private Dictionary<StorePropTag, PropertyMapping> mappedProperties;

		private Dictionary<ushort, PropertyMapping> propertyIdMapping;

		private Func<IRowAccess, IRowPropertyBag> rowPropBagCreator;

		private Table table;

		private ulong[] columnGroups;

		private ObjectPropertySchema baseSchema;

		private ObjectType objectType;

		public static readonly ObjectPropertySchema Empty = new ObjectPropertySchema(ObjectType.Invalid, null, new Dictionary<StorePropTag, PropertyMapping>(1), null);
	}
}
