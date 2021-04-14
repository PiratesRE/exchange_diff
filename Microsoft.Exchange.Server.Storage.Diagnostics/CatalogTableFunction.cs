using System;
using System.Collections.Generic;
using Microsoft.Exchange.Server.Storage.Common;
using Microsoft.Exchange.Server.Storage.Common.ExtensionMethods.Linq;
using Microsoft.Exchange.Server.Storage.PhysicalAccess;
using Microsoft.Exchange.Server.Storage.StoreCommonServices;

namespace Microsoft.Exchange.Server.Storage.Diagnostics
{
	public sealed class CatalogTableFunction
	{
		internal CatalogTableFunction()
		{
			this.tableName = Factory.CreatePhysicalColumn("TableName", "TableName", typeof(string), false, false, false, false, false, Microsoft.Exchange.Server.Storage.Common.Visibility.Public, 0, 128, 128);
			this.tableType = Factory.CreatePhysicalColumn("TableType", "TableType", typeof(string), false, false, false, false, false, Microsoft.Exchange.Server.Storage.Common.Visibility.Public, 32, 0, 32);
			this.partitionKey = Factory.CreatePhysicalColumn("PartitionKey", "PartitionKey", typeof(string), false, false, false, false, false, Microsoft.Exchange.Server.Storage.Common.Visibility.Public, 32, 0, 32);
			this.parameterTypes = Factory.CreatePhysicalColumn("ParameterTypes", "ParameterTypes", typeof(string), false, false, false, false, false, Microsoft.Exchange.Server.Storage.Common.Visibility.Public, 128, 0, 128);
			this.visibility = Factory.CreatePhysicalColumn("Visibility", "Visibility", typeof(string), false, false, false, false, false, Microsoft.Exchange.Server.Storage.Common.Visibility.Public, 12, 0, 12);
			string name = "PrimaryKey";
			bool primaryKey = true;
			bool unique = true;
			bool schemaExtension = false;
			bool[] conditional = new bool[1];
			Index index = new Index(name, primaryKey, unique, schemaExtension, conditional, new bool[]
			{
				true
			}, new PhysicalColumn[]
			{
				this.TableName
			});
			Index[] indexes = new Index[]
			{
				index
			};
			this.tableFunction = Factory.CreateTableFunction("Catalog", new TableFunction.GetTableContentsDelegate(this.GetTableContents), new TableFunction.GetColumnFromRowDelegate(this.GetColumnFromRow), Microsoft.Exchange.Server.Storage.Common.Visibility.Public, new Type[]
			{
				typeof(byte[])
			}, indexes, new PhysicalColumn[]
			{
				this.TableName,
				this.TableType,
				this.PartitionKey,
				this.ParameterTypes,
				this.Visibility
			});
		}

		public TableFunction TableFunction
		{
			get
			{
				return this.tableFunction;
			}
		}

		public PhysicalColumn TableName
		{
			get
			{
				return this.tableName;
			}
		}

		public PhysicalColumn TableType
		{
			get
			{
				return this.tableType;
			}
		}

		public PhysicalColumn PartitionKey
		{
			get
			{
				return this.partitionKey;
			}
		}

		public PhysicalColumn ParameterTypes
		{
			get
			{
				return this.parameterTypes;
			}
		}

		public PhysicalColumn Visibility
		{
			get
			{
				return this.visibility;
			}
		}

		public object GetTableContents(IConnectionProvider connectionProvider, object[] parameters)
		{
			if (parameters.Length == 1 && parameters[0] is byte[])
			{
				return SerializableCatalog.Deserialize((byte[])parameters[0]);
			}
			return null;
		}

		public object GetColumnFromRow(IConnectionProvider connectionProvider, object row, PhysicalColumn columnToFetch)
		{
			if (row is SerializableCatalog)
			{
				SerializableCatalog serializableCatalog = (SerializableCatalog)row;
				if (columnToFetch == this.TableName)
				{
					return serializableCatalog.TableName;
				}
				if (columnToFetch == this.TableType)
				{
					return serializableCatalog.TableType;
				}
				if (columnToFetch == this.PartitionKey)
				{
					return serializableCatalog.PartitionKey;
				}
				if (columnToFetch == this.ParameterTypes)
				{
					if (serializableCatalog.TableName.Equals("Catalog", StringComparison.OrdinalIgnoreCase))
					{
						return string.Empty;
					}
					return serializableCatalog.ParameterTypes;
				}
				else if (columnToFetch == this.Visibility)
				{
					return serializableCatalog.Visibility;
				}
			}
			return null;
		}

		private static int CompareTablesByName(SerializableCatalog x, SerializableCatalog y)
		{
			return x.TableName.CompareTo(y.TableName);
		}

		internal static object[] GetParameters(Context context)
		{
			List<SerializableCatalog> list = new List<SerializableCatalog>(20);
			if (context.Database.IsOnlineActive || context.Database.IsOnlinePassiveAttachedReadOnly)
			{
				using (IEnumerator<Table> enumerator = context.Database.PhysicalDatabase.GetAllTableMetadata().GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						Table table = enumerator.Current;
						if (context.Database.PhysicalDatabase.DatabaseType == DatabaseType.Jet || !StoreQueryTranslator.IsJetOnlyTable(table.Name))
						{
							TableFunction tableFunction = table as TableFunction;
							string text = (tableFunction == null) ? "Table" : "Table Function";
							string text2;
							if (!(tableFunction != null) || tableFunction.ParameterTypes == null)
							{
								text2 = string.Empty;
							}
							else
							{
								text2 = string.Join(",", from p in tableFunction.ParameterTypes
								select p.Name);
							}
							string text3 = text2;
							string text4;
							if (!table.IsPartitioned)
							{
								text4 = string.Empty;
							}
							else
							{
								text4 = string.Join(",", from c in table.Columns.Take(table.SpecialCols.NumberOfPartioningColumns)
								select c.Name);
							}
							string text5 = text4;
							string text6 = CatalogTableFunction.GetVisibility(table);
							list.Add(new SerializableCatalog(table.Name, text, text5, text3, text6));
						}
					}
					goto IL_192;
				}
			}
			if (context.Database.IsOnlinePassive)
			{
				list.Add(new SerializableCatalog("Catalog", "Table Function", string.Empty, string.Empty, "Public"));
			}
			IL_192:
			foreach (TableFunction tableFunction2 in StoreQueryTargets.Targets.Values)
			{
				string text7;
				if (tableFunction2.ParameterTypes == null)
				{
					text7 = string.Empty;
				}
				else
				{
					text7 = string.Join(",", from p in tableFunction2.ParameterTypes
					select p.Name);
				}
				string text8 = text7;
				string text9 = CatalogTableFunction.GetVisibility(tableFunction2);
				list.Add(new SerializableCatalog(tableFunction2.Name, "Table Function", string.Empty, text8, text9));
			}
			list.Sort(new Comparison<SerializableCatalog>(CatalogTableFunction.CompareTablesByName));
			return new object[]
			{
				SerializableCatalog.Serialize(list.ToArray())
			};
		}

		private static string GetVisibility(Table table)
		{
			if (table.Visibility == Microsoft.Exchange.Server.Storage.Common.Visibility.Private)
			{
				return "Private";
			}
			if (table.Visibility == Microsoft.Exchange.Server.Storage.Common.Visibility.Redacted)
			{
				return "Redacted";
			}
			foreach (Column column in table.Columns)
			{
				if (column.Visibility == Microsoft.Exchange.Server.Storage.Common.Visibility.Private || column.Visibility == Microsoft.Exchange.Server.Storage.Common.Visibility.Redacted)
				{
					return "Partial";
				}
			}
			if (table.Visibility == Microsoft.Exchange.Server.Storage.Common.Visibility.Public)
			{
				return "Public";
			}
			return "Unknown";
		}

		public const string TableNameName = "TableName";

		public const string TableTypeName = "TableType";

		public const string PartitionKeyName = "PartitionKey";

		public const string ParameterTypesName = "ParameterTypes";

		public const string VisibilityName = "Visibility";

		public const string TableFunctionName = "Catalog";

		private PhysicalColumn tableName;

		private PhysicalColumn tableType;

		private PhysicalColumn partitionKey;

		private PhysicalColumn parameterTypes;

		private PhysicalColumn visibility;

		private TableFunction tableFunction;
	}
}
