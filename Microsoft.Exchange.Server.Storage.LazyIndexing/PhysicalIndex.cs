using System;
using System.Globalization;
using System.Text;
using Microsoft.Exchange.Server.Storage.Common;
using Microsoft.Exchange.Server.Storage.PhysicalAccess;
using Microsoft.Exchange.Server.Storage.PropTags;
using Microsoft.Exchange.Server.Storage.StoreCommonServices;

namespace Microsoft.Exchange.Server.Storage.LazyIndexing
{
	public sealed class PhysicalIndex
	{
		private PhysicalIndex()
		{
		}

		public int PhysicalIndexNumber
		{
			get
			{
				return this.physicalIndexNumber;
			}
		}

		public Table Table
		{
			get
			{
				return this.table;
			}
		}

		private string IndexName
		{
			get
			{
				return this.indexName;
			}
		}

		private string TableName
		{
			get
			{
				return this.tableName;
			}
		}

		public long CreationTimeStamp { get; set; }

		public Connection ConnectionLimitVisibility
		{
			get
			{
				return this.connectionLimitVisibility;
			}
			set
			{
				this.connectionLimitVisibility = value;
			}
		}

		internal int IdentityColumnIndex
		{
			get
			{
				return (int)this.identityColumnIndex;
			}
		}

		public static int MaxSortColumnLength(Type type)
		{
			return SortColumn.MaxSortColumnLength(type);
		}

		public static int MaxSortColumnLength(PropertyType propType)
		{
			return SortColumn.MaxSortColumnLength(propType);
		}

		public static void AppendColumnName(int columnNumber, StringBuilder sb)
		{
			if (columnNumber < PhysicalIndex.columnNames.Length)
			{
				sb.Append(PhysicalIndex.columnNames[columnNumber]);
				return;
			}
			Statistics.Unsorted.MaxPIColumnIndex.Bump(columnNumber);
			sb.Append("C");
			sb.Append(columnNumber);
		}

		public static string GetColumnName(int columnNumber)
		{
			if (columnNumber < PhysicalIndex.columnNames.Length)
			{
				return PhysicalIndex.columnNames[columnNumber];
			}
			Statistics.Unsorted.MaxPIColumnIndex.Bump(columnNumber);
			return "C" + columnNumber.ToString();
		}

		public static string GetTableName(int physicalIndexNumber)
		{
			return "pi" + physicalIndexNumber.ToString();
		}

		public bool IndexIsVisibleForConnection(Connection connection)
		{
			return this.connectionLimitVisibility == null || object.ReferenceEquals(this.connectionLimitVisibility, connection);
		}

		public CultureInfo GetCulture()
		{
			return CultureHelper.TranslateLcid(this.lcid);
		}

		public Column GetColumn(int columnNumber)
		{
			return this.Table.Columns[columnNumber];
		}

		public int GetColumnCount()
		{
			return this.Table.Columns.Count;
		}

		public void AppendPiName(StringBuilder sb)
		{
			sb.Append("[Exchange].[");
			sb.Append(this.tableName);
			sb.Append("]");
		}

		internal static PhysicalIndex CreatePhysicalIndex(Context context, int keyColumnCount, short identityColumnIndex, PropertyType[] columnTypes, int[] columnMaxLengths, bool[] columnFixedLengths, bool[] columnAscendings)
		{
			PhysicalIndex physicalIndex = null;
			for (int i = 0; i < keyColumnCount; i++)
			{
			}
			PseudoIndexDefinitionTable pseudoIndexDefinitionTable = DatabaseSchema.PseudoIndexDefinitionTable(context.Database);
			IndexDefinitionBlob[] array = new IndexDefinitionBlob[columnTypes.Length];
			for (int j = 0; j < array.Length; j++)
			{
				int num = (int)columnTypes[j];
				if (columnTypes[j] == PropertyType.Unicode && j < keyColumnCount)
				{
					num |= int.MinValue;
				}
				array[j] = new IndexDefinitionBlob(num, columnMaxLengths[j], columnFixedLengths[j], columnAscendings[j]);
			}
			byte[] value = IndexDefinitionBlob.Serialize(keyColumnCount, CultureHelper.GetLcidFromCulture(context.Culture), identityColumnIndex, array);
			using (DataRow dataRow = Factory.CreateDataRow(context.Culture, context, pseudoIndexDefinitionTable.Table, true, new ColumnValue[]
			{
				new ColumnValue(pseudoIndexDefinitionTable.ColumnBlob, value)
			}))
			{
				dataRow.Flush(context);
				physicalIndex = new PhysicalIndex();
				physicalIndex.PopulateFromDataRow(context, dataRow, pseudoIndexDefinitionTable);
			}
			physicalIndex.Table.CreateTable(context, 0);
			return physicalIndex;
		}

		internal static PhysicalIndex GetPhysicalIndex(Context context, int physicalIndexNumber)
		{
			PhysicalIndex physicalIndex = new PhysicalIndex();
			if (!physicalIndex.Configure(context, physicalIndexNumber))
			{
				physicalIndex = null;
			}
			return physicalIndex;
		}

		internal bool Ascending(int colNumber)
		{
			return this.indexDefinitionEntries[colNumber].Ascending;
		}

		internal bool IndexMatch(int lcid, int keyColumnCount, int identityColumnIndex, PropertyType[] columnTypes, int[] columnMaxLengths, bool[] columnFixedLengths, bool[] columnAscendings, bool permitReverseOrder)
		{
			bool result = true;
			bool flag = permitReverseOrder && this.indexDefinitionEntries[0].Ascending != columnAscendings[0];
			if (this.keyColumnCount != keyColumnCount)
			{
				result = false;
			}
			else if ((int)this.identityColumnIndex != identityColumnIndex)
			{
				result = false;
			}
			else if (this.indexDefinitionEntries.Length != columnTypes.Length)
			{
				result = false;
			}
			else
			{
				for (int i = 0; i < this.indexDefinitionEntries.Length; i++)
				{
					bool flag2 = (this.indexDefinitionEntries[i].ColumnType & int.MinValue) != 0;
					if ((PropertyType)this.indexDefinitionEntries[i].ColumnType != columnTypes[i] || this.indexDefinitionEntries[i].FixedLength != columnFixedLengths[i] || this.indexDefinitionEntries[i].MaxLength != columnMaxLengths[i])
					{
						result = false;
						break;
					}
					if (i < keyColumnCount)
					{
						if ((!flag && this.indexDefinitionEntries[i].Ascending != columnAscendings[i]) || (flag && this.indexDefinitionEntries[i].Ascending == columnAscendings[i]))
						{
							result = false;
							break;
						}
						if ((this.lcid != lcid || !flag2) && (ushort)this.indexDefinitionEntries[i].ColumnType == 31)
						{
							result = false;
							break;
						}
					}
				}
			}
			return result;
		}

		private bool Configure(Context context, int physicalIndexNumber)
		{
			this.tableName = PhysicalIndex.GetTableName(physicalIndexNumber);
			this.indexName = this.tableName + "PK";
			PseudoIndexDefinitionTable pseudoIndexDefinitionTable = DatabaseSchema.PseudoIndexDefinitionTable(context.Database);
			using (DataRow dataRow = Factory.OpenDataRow(context.Culture, context, pseudoIndexDefinitionTable.Table, true, new ColumnValue[]
			{
				new ColumnValue(pseudoIndexDefinitionTable.PhysicalIndexNumber, physicalIndexNumber)
			}))
			{
				if (dataRow != null)
				{
					this.PopulateFromDataRow(context, dataRow, pseudoIndexDefinitionTable);
					return true;
				}
			}
			return false;
		}

		private void PopulateFromDataRow(Context context, DataRow dataRow, PseudoIndexDefinitionTable pseudoIndexDefinitionTable)
		{
			this.physicalIndexNumber = (int)dataRow.GetValue(context, pseudoIndexDefinitionTable.PhysicalIndexNumber);
			this.tableName = PhysicalIndex.GetTableName(this.physicalIndexNumber);
			this.indexName = this.tableName + "PK";
			byte[] theBlob = (byte[])dataRow.GetValue(context, pseudoIndexDefinitionTable.ColumnBlob);
			this.indexDefinitionEntries = IndexDefinitionBlob.Deserialize(out this.keyColumnCount, out this.lcid, out this.identityColumnIndex, theBlob);
			this.table = context.Database.PhysicalDatabase.GetTableMetadata(this.TableName);
			if (this.table == null)
			{
				PhysicalColumn[] array = new PhysicalColumn[this.indexDefinitionEntries.Length];
				PhysicalColumn[] array2 = new PhysicalColumn[this.keyColumnCount];
				bool[] array3 = new bool[this.keyColumnCount];
				bool[] array4 = new bool[this.keyColumnCount];
				Index index = null;
				for (int i = 0; i < this.indexDefinitionEntries.Length; i++)
				{
					int num = 0;
					int num2 = 0;
					bool flag = i < this.keyColumnCount;
					PropertyType propType = (PropertyType)this.indexDefinitionEntries[i].ColumnType;
					if (!this.indexDefinitionEntries[i].FixedLength)
					{
						num = this.indexDefinitionEntries[i].MaxLength;
					}
					else
					{
						num2 = this.indexDefinitionEntries[i].MaxLength;
					}
					bool flag2 = i == (int)this.identityColumnIndex;
					string columnName = PhysicalIndex.GetColumnName(i);
					array[i] = Factory.CreatePhysicalColumn(columnName, columnName, PropertyTypeHelper.ClrTypeFromPropType(propType), !flag2, flag2, false, false, Visibility.Public, num, num2, Math.Max(num, num2));
					if (flag)
					{
						array2[i] = array[i];
						array4[i] = this.indexDefinitionEntries[i].Ascending;
						array3[i] = false;
					}
					if (flag2 && context.Database.PhysicalDatabase.DatabaseType == DatabaseType.Jet && i != 2)
					{
						PhysicalColumn[] array5 = new PhysicalColumn[3];
						bool[] array6 = new bool[3];
						bool[] array7 = new bool[3];
						int j;
						for (j = 0; j < 2; j++)
						{
							array5[j] = array2[j];
							array6[j] = array4[j];
							array7[j] = false;
						}
						array5[j] = array[i];
						array6[j] = true;
						array7[j] = false;
						index = new Index(array[i].Name + " Identity Column Secondary Index", false, true, false, array7, array6, array5);
					}
				}
				Index index2 = new Index(this.IndexName, true, true, false, array3, array4, array2);
				SpecialColumns specialCols = default(SpecialColumns);
				specialCols.NumberOfPartioningColumns = 2;
				int num3 = (index != null) ? 2 : 1;
				Index[] array8 = new Index[num3];
				array8[0] = index2;
				if (index != null)
				{
					array8[1] = index;
				}
				this.table = Factory.CreateTable(this.TableName, TableClass.LazyIndex, this.GetCulture(), false, TableAccessHints.ForwardScan, false, Visibility.Redacted, false, specialCols, array8, Table.NoColumns, array);
				context.Database.PhysicalDatabase.AddTableMetadata(this.table);
			}
		}

		private const string ClusteringKeySuffix = "PK";

		public const string IndexNamePrefix = "pi";

		public const int LogicalIndexKeyPrefixLength = 2;

		public const string MailboxPartitionNumberColumnName = "MailboxPartitionNumber";

		public const string LogicalIndexNumberColumnName = "LogicalIndexNumber";

		private const int UseLinguisticCasingRulesFlag = -2147483648;

		private static string[] columnNames = new string[]
		{
			"MailboxPartitionNumber",
			"LogicalIndexNumber",
			"C1",
			"C2",
			"C3",
			"C4",
			"C5",
			"C6",
			"C7",
			"C8",
			"C9",
			"C10",
			"C11",
			"C12",
			"C13",
			"C14",
			"C15",
			"C16",
			"C17",
			"C18",
			"C19",
			"C20",
			"C21",
			"C22",
			"C23",
			"C24",
			"C25",
			"C26",
			"C27",
			"C28",
			"C29",
			"C30",
			"C31",
			"C32",
			"C33",
			"C34",
			"C35",
			"C36",
			"C37",
			"C38",
			"C39",
			"C40",
			"C41",
			"C42",
			"C43",
			"C44",
			"C45",
			"C46",
			"C47",
			"C48",
			"C49",
			"C50",
			"C51",
			"C52",
			"C53",
			"C54",
			"C55",
			"C56",
			"C57",
			"C58",
			"C59",
			"C60",
			"C61",
			"C62",
			"C63",
			"C64",
			"C65",
			"C66",
			"C67",
			"C68",
			"C69",
			"C70",
			"C71",
			"C72",
			"C73",
			"C74",
			"C75",
			"C76",
			"C77",
			"C78",
			"C79",
			"C80",
			"C81",
			"C82",
			"C83",
			"C84",
			"C85",
			"C86",
			"C87",
			"C88",
			"C89",
			"C90",
			"C91",
			"C92",
			"C93",
			"C94",
			"C95",
			"C96",
			"C97",
			"C98",
			"C99"
		};

		private int keyColumnCount;

		private int lcid;

		private IndexDefinitionBlob[] indexDefinitionEntries;

		private short identityColumnIndex;

		private int physicalIndexNumber;

		private Table table;

		private string indexName;

		private string tableName;

		private Connection connectionLimitVisibility;
	}
}
