using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Microsoft.Exchange.Server.Storage.Common;
using Microsoft.Exchange.Server.Storage.Common.ExtensionMethods.Linq;
using Microsoft.Exchange.Server.Storage.PhysicalAccessJet;

namespace Microsoft.Exchange.Server.Storage.PhysicalAccess
{
	public class StoreQueryTargets
	{
		private StoreQueryTargets()
		{
			this.targets = new Dictionary<string, TableFunction>(5, StringComparer.OrdinalIgnoreCase);
		}

		public static IDictionary<string, TableFunction> Targets
		{
			get
			{
				return StoreQueryTargets.Instance.targets;
			}
		}

		private static StoreQueryTargets Instance
		{
			get
			{
				if (StoreQueryTargets.instance == null)
				{
					StoreQueryTargets.instance = new StoreQueryTargets();
				}
				return StoreQueryTargets.instance;
			}
		}

		public static void Register<T>(IStoreSimpleQueryTarget<T> collection, Visibility visibility)
		{
			TableFunction tableFunction = StoreQueryTargets.BuildTableFunction<T>(collection, visibility);
			if (tableFunction != null)
			{
				StoreQueryTargets.Targets[tableFunction.Name] = tableFunction;
			}
		}

		public static void Register<T>(IStoreDatabaseQueryTarget<T> collection, Database database, Visibility visibility)
		{
			TableFunction tableFunction = StoreQueryTargets.BuildTableFunction<T>(collection, visibility);
			if (tableFunction != null)
			{
				database.AddTableMetadata(tableFunction);
			}
		}

		private static TableFunction BuildTableFunction<T>(IStoreQueryTargetBase<T> collection, Visibility visibility)
		{
			Func<Tuple<int, PhysicalColumn, bool, bool>, bool> func = null;
			Func<Tuple<int, PhysicalColumn, bool, bool>, bool> func2 = null;
			Func<Tuple<int, PhysicalColumn, bool, bool>, int> func3 = null;
			Func<Tuple<int, PhysicalColumn, bool, bool>, PhysicalColumn> func4 = null;
			IList<Tuple<PropertyInfo, QueryableAttribute>> list = new List<Tuple<PropertyInfo, QueryableAttribute>>(20);
			IList<PropertyInfo> list2 = new List<PropertyInfo>(20);
			IList<Tuple<int, PhysicalColumn, bool, bool>> list3 = new List<Tuple<int, PhysicalColumn, bool, bool>>(3);
			IDictionary<string, PropertyInfo> getters = new Dictionary<string, PropertyInfo>(20);
			Type typeFromHandle = typeof(T);
			foreach (PropertyInfo propertyInfo in typeFromHandle.GetProperties())
			{
				list2.Add(propertyInfo);
				foreach (object obj in propertyInfo.GetCustomAttributes(false))
				{
					QueryableAttribute queryableAttribute = obj as QueryableAttribute;
					if (queryableAttribute != null)
					{
						list.Add(new Tuple<PropertyInfo, QueryableAttribute>(propertyInfo, queryableAttribute));
					}
				}
			}
			IList<PhysicalColumn> list4;
			if (list.Count > 0)
			{
				list4 = new List<PhysicalColumn>(list.Count);
				using (IEnumerator<Tuple<PropertyInfo, QueryableAttribute>> enumerator = list.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						Tuple<PropertyInfo, QueryableAttribute> tuple2 = enumerator.Current;
						PhysicalColumn column3 = StoreQueryTargets.GetColumn(tuple2.Item1, tuple2.Item2);
						list4.Add(column3);
						getters[tuple2.Item1.Name] = tuple2.Item1;
						if (tuple2.Item2.Index >= 0)
						{
							list3.Add(new Tuple<int, PhysicalColumn, bool, bool>(tuple2.Item2.Index, column3, false, true));
						}
					}
					goto IL_1BC;
				}
			}
			list4 = new List<PhysicalColumn>(list2.Count);
			foreach (PropertyInfo propertyInfo2 in list2)
			{
				PhysicalColumn column2 = StoreQueryTargets.GetColumn(propertyInfo2, null);
				list4.Add(column2);
				getters[propertyInfo2.Name] = propertyInfo2;
			}
			IL_1BC:
			if (list4.Count == 0)
			{
				throw new DiagnosticQueryException("No Queryable columns on collection type");
			}
			Index index;
			if (list3.Count > 0)
			{
				string name = "PrimaryKey";
				bool primaryKey = true;
				bool unique = true;
				bool schemaExtension = false;
				IEnumerable<Tuple<int, PhysicalColumn, bool, bool>> source = list3;
				if (func == null)
				{
					func = ((Tuple<int, PhysicalColumn, bool, bool> tuple) => tuple.Item3);
				}
				bool[] conditional = source.Select(func).ToArray<bool>();
				IEnumerable<Tuple<int, PhysicalColumn, bool, bool>> source2 = list3;
				if (func2 == null)
				{
					func2 = ((Tuple<int, PhysicalColumn, bool, bool> tuple) => tuple.Item4);
				}
				bool[] ascending = source2.Select(func2).ToArray<bool>();
				IEnumerable<Tuple<int, PhysicalColumn, bool, bool>> source3 = list3;
				if (func3 == null)
				{
					func3 = ((Tuple<int, PhysicalColumn, bool, bool> tuple) => tuple.Item1);
				}
				IEnumerable<Tuple<int, PhysicalColumn, bool, bool>> source4 = source3.OrderBy(func3);
				if (func4 == null)
				{
					func4 = ((Tuple<int, PhysicalColumn, bool, bool> tuple) => tuple.Item2);
				}
				index = new Index(name, primaryKey, unique, schemaExtension, conditional, ascending, source4.Select(func4).ToArray<PhysicalColumn>());
			}
			else
			{
				string name2 = "PrimaryKey";
				bool primaryKey2 = true;
				bool unique2 = true;
				bool schemaExtension2 = false;
				bool[] conditional2 = new bool[1];
				index = new Index(name2, primaryKey2, unique2, schemaExtension2, conditional2, new bool[]
				{
					true
				}, new PhysicalColumn[]
				{
					list4[0]
				});
			}
			TableFunction tableFunction = Factory.CreateTableFunction(collection.Name, delegate(IConnectionProvider connectionProvider, object[] parameters)
			{
				if (collection is IStoreSimpleQueryTarget<T>)
				{
					return ((IStoreSimpleQueryTarget<T>)collection).GetRows(parameters);
				}
				if (collection is IStoreDatabaseQueryTarget<T>)
				{
					return ((IStoreDatabaseQueryTarget<T>)collection).GetRows(connectionProvider, parameters);
				}
				throw new DiagnosticQueryException("Unexpected query target type");
			}, delegate(IConnectionProvider connectionProvider, object row, PhysicalColumn column)
			{
				PropertyInfo propertyInfo3;
				if (!getters.TryGetValue(column.Name, out propertyInfo3))
				{
					return null;
				}
				object value = propertyInfo3.GetValue(row, null);
				string text = value as string;
				IEnumerable enumerable = value as IEnumerable;
				byte[] array = value as byte[];
				if (column.ExtendedTypeCode == ExtendedTypeCode.String && text == null && value != null)
				{
					return StoreQueryTargets.ConvertValueToString(value, 0);
				}
				if (column.ExtendedTypeCode == ExtendedTypeCode.MVString && enumerable != null && array == null && value != null)
				{
					return StoreQueryTargets.ConvertCollectionToStrings(enumerable, 0);
				}
				return value;
			}, visibility, collection.ParameterTypes, new Index[]
			{
				index
			}, list4.ToArray<PhysicalColumn>());
			return tableFunction as JetTableFunction;
		}

		private static void GetSizeAndLength(Type columnType, int columnLength, out int size, out int length)
		{
			TypeCode typeCode = Type.GetTypeCode(columnType);
			if (typeCode == TypeCode.String || typeCode == TypeCode.Object)
			{
				if (columnType == typeof(Guid))
				{
					size = 16;
					length = 0;
					return;
				}
				size = 0;
				length = columnLength;
				return;
			}
			else
			{
				length = 0;
				TypeCode typeCode2 = typeCode;
				if (typeCode2 != TypeCode.Boolean)
				{
					switch (typeCode2)
					{
					case TypeCode.Int16:
						size = 2;
						return;
					case TypeCode.Int32:
						size = 4;
						return;
					case TypeCode.Int64:
						size = 8;
						return;
					case TypeCode.Single:
						size = 4;
						return;
					case TypeCode.Double:
						size = 8;
						return;
					case TypeCode.DateTime:
						size = 8;
						return;
					}
					throw new InvalidOperationException("Unsupported queryable property type");
				}
				size = 1;
				return;
			}
		}

		private static PhysicalColumn GetColumn(PropertyInfo info, QueryableAttribute qa)
		{
			ExtendedTypeCode extendedTypeCode;
			Type type = ValueTypeHelper.TryGetExtendedTypeCode(info.PropertyType, out extendedTypeCode) ? info.PropertyType : (info.PropertyType.IsArray ? typeof(string[]) : typeof(string));
			int columnLength = (qa != null) ? qa.Length : 1048576;
			Visibility visibility = (qa != null) ? qa.Visibility : Visibility.Public;
			Visibility visibility2 = StoreQueryTargets.GetVisibility(info, visibility);
			int size;
			int num;
			StoreQueryTargets.GetSizeAndLength(type, columnLength, out size, out num);
			return Factory.CreatePhysicalColumn(info.Name, info.Name, type, false, false, false, false, visibility2, num, size, num);
		}

		private static Visibility GetVisibility(PropertyInfo info, Visibility visibility)
		{
			if (Type.GetTypeCode(info.PropertyType) == TypeCode.Object)
			{
				foreach (PropertyInfo propertyInfo in info.PropertyType.GetProperties())
				{
					foreach (object obj in propertyInfo.GetCustomAttributes(false))
					{
						QueryableAttribute queryableAttribute = obj as QueryableAttribute;
						if (queryableAttribute != null)
						{
							visibility = VisibilityHelper.Select(visibility, queryableAttribute.Visibility);
						}
					}
				}
			}
			return visibility;
		}

		private static string ConvertValueToString(object value, int depth)
		{
			if (value == null)
			{
				return null;
			}
			if (value is byte[])
			{
				return StoreQueryTargets.GetHexString((byte[])value);
			}
			if (value is DateTime)
			{
				return ((DateTime)value).ToString("yyyy'-'MM'-'dd' 'HH':'mm':'ss'.'fffffff");
			}
			if (Type.GetTypeCode(value.GetType()) == TypeCode.Object && depth < 1)
			{
				if (StoreQueryTargets.IsToStringAvailable(value.GetType()))
				{
					return value.ToString();
				}
				IList<PropertyInfo> list = new List<PropertyInfo>(40);
				IList<PropertyInfo> list2 = new List<PropertyInfo>(40);
				foreach (PropertyInfo propertyInfo in value.GetType().GetProperties())
				{
					list2.Add(propertyInfo);
					foreach (object obj in propertyInfo.GetCustomAttributes(false))
					{
						QueryableAttribute queryableAttribute = obj as QueryableAttribute;
						if (queryableAttribute != null)
						{
							list.Add(propertyInfo);
						}
					}
				}
				if (list.Count == 0)
				{
					list = list2;
				}
				if (list.Count == 1 && (list[0].PropertyType.Equals(typeof(byte[])) || !list[0].PropertyType.IsArray))
				{
					return StoreQueryTargets.ConvertValueToString(list[0].GetValue(value, null), depth + 1);
				}
				if (list.Count > 0)
				{
					StringBuilder stringBuilder = new StringBuilder(1024);
					foreach (PropertyInfo propertyInfo2 in list)
					{
						stringBuilder.AppendFormat("{0}{1} = ", (stringBuilder.Length > 0) ? ", " : string.Empty, propertyInfo2.Name);
						object value2 = propertyInfo2.GetValue(value, null);
						IEnumerable enumerable = value2 as IEnumerable;
						if (enumerable != null && !(value is byte[]))
						{
							stringBuilder.Append("[ ");
							int length = stringBuilder.Length;
							foreach (string arg in StoreQueryTargets.ConvertCollectionToStrings(enumerable, depth + 1))
							{
								stringBuilder.AppendFormat("{0}{1}", (stringBuilder.Length > length) ? ", " : string.Empty, arg);
							}
							stringBuilder.Append(" ]");
						}
						else
						{
							stringBuilder.Append(StoreQueryTargets.ConvertValueToString(value2, depth + 1));
						}
					}
					return stringBuilder.ToString();
				}
			}
			return value.ToString();
		}

		private static string[] ConvertCollectionToStrings(IEnumerable collection, int depth)
		{
			IList<string> list = new List<string>(50);
			if (collection != null)
			{
				foreach (object value in collection)
				{
					list.Add(StoreQueryTargets.ConvertValueToString(value, depth));
				}
			}
			return list.ToArray<string>();
		}

		private static string GetHexString(byte[] value)
		{
			if (value == null)
			{
				return null;
			}
			if (value.Length > 0)
			{
				StringBuilder stringBuilder = new StringBuilder("0x", 2 * value.Length + 2);
				foreach (byte b in value)
				{
					stringBuilder.Append("0123456789ABCDEF"[(b & 240) >> 4]);
					stringBuilder.Append("0123456789ABCDEF"[(int)(b & 15)]);
				}
				return stringBuilder.ToString();
			}
			return string.Empty;
		}

		private static bool IsToStringAvailable(Type type)
		{
			if (type == null || type.Equals(typeof(object)))
			{
				return false;
			}
			foreach (MethodInfo methodInfo in type.GetMethods(BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public))
			{
				if (methodInfo != null && methodInfo.Name.Equals("ToString") && methodInfo.GetParameters().Length == 0 && methodInfo.ReturnType.Equals(typeof(string)))
				{
					return true;
				}
			}
			return StoreQueryTargets.IsToStringAvailable(type.BaseType);
		}

		private const int MaximumPropertyDepth = 1;

		private static StoreQueryTargets instance;

		private readonly IDictionary<string, TableFunction> targets;
	}
}
