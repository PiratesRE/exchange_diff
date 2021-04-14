using System;
using System.Collections.Generic;
using Microsoft.Exchange.Server.Storage.Common;
using Microsoft.Exchange.Server.Storage.PhysicalAccess;
using Microsoft.Exchange.Server.Storage.PropTags;

namespace Microsoft.Exchange.Server.Storage.StoreCommonServices
{
	internal class PropertySchema
	{
		private PropertySchema()
		{
			this.objectSchemas = new Dictionary<int, ObjectPropertySchema>(10);
		}

		internal static void Initialize()
		{
			if (PropertySchema.dataSlot == -1)
			{
				PropertySchema.dataSlot = StoreDatabase.AllocateComponentDataSlot();
			}
		}

		internal static void MountEventHandler(StoreDatabase database)
		{
			PropertySchema value = new PropertySchema();
			database.ComponentData[PropertySchema.dataSlot] = value;
		}

		internal static void DismountEventHandler(StoreDatabase database)
		{
			database.ComponentData[PropertySchema.dataSlot] = null;
		}

		internal static PropertySchema GetSchema(StoreDatabase database)
		{
			return database.ComponentData[PropertySchema.dataSlot] as PropertySchema;
		}

		public static void AddObjectSchema(StoreDatabase database, ObjectType objectType, ObjectPropertySchema objectSchema)
		{
			PropertySchema schema = PropertySchema.GetSchema(database);
			schema.AddObjectSchema(objectType, objectSchema);
		}

		internal static ObjectPropertySchema GetObjectSchema(StoreDatabase database, ObjectType objectType)
		{
			PropertySchema schema = PropertySchema.GetSchema(database);
			return schema.GetObjectSchema(objectType);
		}

		public static PropertyMapping FindMapping(StoreDatabase database, ObjectType objectType, StorePropTag propertyTag)
		{
			ObjectPropertySchema objectSchema = PropertySchema.GetObjectSchema(database, objectType);
			if (objectSchema == null)
			{
				return null;
			}
			return objectSchema.FindMapping(propertyTag);
		}

		public static Column MapToColumn(StoreDatabase database, ObjectType objectType, StorePropTag propertyTag)
		{
			ObjectPropertySchema objectSchema = PropertySchema.GetObjectSchema(database, objectType);
			if (objectSchema == null)
			{
				return null;
			}
			return PropertySchema.MapToColumnHelper(objectSchema, propertyTag);
		}

		public static bool IsMultiValueInstanceColumn(Column column)
		{
			ExtendedPropertyColumn extendedPropertyColumn;
			return PropertySchema.IsMultiValueInstanceColumn(column, out extendedPropertyColumn);
		}

		public static bool IsMultiValueInstanceColumn(Column column, out ExtendedPropertyColumn baseMultiValueColumn)
		{
			baseMultiValueColumn = null;
			MappedPropertyColumn mappedPropertyColumn = column as MappedPropertyColumn;
			if (mappedPropertyColumn == null)
			{
				return false;
			}
			if (!mappedPropertyColumn.StorePropTag.IsMultiValueInstance)
			{
				return false;
			}
			FunctionColumn functionColumn = mappedPropertyColumn.ActualColumn as FunctionColumn;
			baseMultiValueColumn = (functionColumn.ArgumentColumns[1] as ExtendedPropertyColumn);
			return true;
		}

		private static Column MapToColumnHelper(ObjectPropertySchema objectSchema, StorePropTag propertyTag)
		{
			PropertyMapping propertyMapping = objectSchema.FindMapping(propertyTag);
			if (propertyMapping != null)
			{
				return propertyMapping.Column;
			}
			ObjectPropertySchema baseSchema = objectSchema.BaseSchema;
			if (baseSchema != null)
			{
				return PropertySchema.MapToColumnHelper(baseSchema, propertyTag);
			}
			if (propertyTag.IsMultiValueInstance)
			{
				return PropertySchema.ConstructMVIFunctionColumn(objectSchema, propertyTag);
			}
			return PropertySchemaPopulation.ConstructPropertyColumn(objectSchema.Table, propertyTag, objectSchema.RowPropBagCreator, null);
		}

		public static Column ConstructMVIFunctionColumn(Column multiValueColumn, Column instanceNumberColumn)
		{
			Type type = null;
			string functionName = null;
			switch (multiValueColumn.ExtendedTypeCode)
			{
			case ExtendedTypeCode.MVInt16:
				functionName = "Exchange.multiValueInstanceProperty_smallint";
				type = typeof(short);
				break;
			case ExtendedTypeCode.MVInt32:
				functionName = "Exchange.multiValueInstanceProperty_int";
				type = typeof(int);
				break;
			case ExtendedTypeCode.MVInt64:
				functionName = "Exchange.multiValueInstanceProperty_bigint";
				type = typeof(long);
				break;
			case ExtendedTypeCode.MVSingle:
				functionName = "Exchange.multiValueInstanceProperty_single";
				type = typeof(float);
				break;
			case ExtendedTypeCode.MVDouble:
				functionName = "Exchange.multiValueInstanceProperty_double";
				type = typeof(double);
				break;
			case ExtendedTypeCode.MVDateTime:
				functionName = "Exchange.multiValueInstanceProperty_datetime";
				type = typeof(DateTime);
				break;
			case ExtendedTypeCode.MVGuid:
				functionName = "Exchange.multiValueInstanceProperty_UNIQUEIDENTIFIER";
				type = typeof(Guid);
				break;
			case ExtendedTypeCode.MVString:
				functionName = "Exchange.multiValueInstanceProperty_nvarchar";
				type = typeof(string);
				break;
			case ExtendedTypeCode.MVBinary:
				functionName = "Exchange.multiValueInstanceProperty_binary";
				type = typeof(byte[]);
				break;
			default:
				Microsoft.Exchange.Server.Storage.Common.Globals.AssertRetail(false, string.Format("Unexpected type {0}", multiValueColumn.ExtendedTypeCode));
				break;
			}
			PropertyType inType = PropertyTypeHelper.PropTypeFromClrType(type);
			return Factory.CreateFunctionColumn("MultiValueInstanceProperty", type, PropertyTypeHelper.SizeFromPropType(inType), PropertyTypeHelper.MaxLengthFromPropType(inType), multiValueColumn.Table, PropertySchema.multiValueInstanceDelegate, functionName, new Column[]
			{
				instanceNumberColumn,
				multiValueColumn
			});
		}

		private static Column ConstructMVIFunctionColumn(ObjectPropertySchema objectSchema, StorePropTag propertyTag)
		{
			Column instanceNumberColumn = PropertySchema.MapToColumnHelper(objectSchema, PropTag.Message.InstanceNum);
			Column multiValueColumn = PropertySchema.MapToColumnHelper(objectSchema, propertyTag.ChangeType(propertyTag.PropType & (PropertyType)57343));
			return Factory.CreateMappedPropertyColumn(PropertySchema.ConstructMVIFunctionColumn(multiValueColumn, instanceNumberColumn), propertyTag);
		}

		private ObjectPropertySchema GetObjectSchema(ObjectType objectType)
		{
			ObjectPropertySchema result;
			this.objectSchemas.TryGetValue((int)objectType, out result);
			return result;
		}

		private void AddObjectSchema(ObjectType objectType, ObjectPropertySchema objectSchema)
		{
			this.objectSchemas.Add((int)objectType, objectSchema);
		}

		private static int dataSlot = -1;

		private static Func<object[], object> multiValueInstanceDelegate = delegate(object[] parameters)
		{
			if (parameters[0] == null || parameters[1] == null)
			{
				return null;
			}
			Array array = (Array)parameters[1];
			if (array.Length == 0)
			{
				return null;
			}
			int num = (int)parameters[0];
			if (num == 0 || num > array.Length)
			{
				return null;
			}
			return array.GetValue(num - 1);
		};

		private readonly Dictionary<int, ObjectPropertySchema> objectSchemas;
	}
}
