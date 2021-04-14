using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.ManagedStore.PhysicalAccess;
using Microsoft.Exchange.Server.Storage.Common;
using Microsoft.Exchange.Server.Storage.PhysicalAccess;
using Microsoft.Isam.Esent.Interop;

namespace Microsoft.Exchange.Server.Storage.PhysicalAccessJet
{
	public class JetTable : Microsoft.Exchange.Server.Storage.PhysicalAccess.Table
	{
		public JetTable(string name, TableClass tableClass, CultureInfo culture, bool trackDirtyObjects, TableAccessHints tableAccessHints, bool readOnly, Visibility visibility, bool schemaExtension, SpecialColumns specialCols, Index[] indexes, PhysicalColumn[] computedColumns, PhysicalColumn[] columns) : base(name, tableClass, culture, trackDirtyObjects, tableAccessHints, readOnly, visibility, schemaExtension, specialCols, indexes, computedColumns, columns)
		{
		}

		public static void DeleteJetTable(IConnectionProvider connectionProvider, string tableName)
		{
			JetConnection jetConnection = connectionProvider.GetConnection() as JetConnection;
			jetConnection.BeginTransactionIfNeeded(Connection.TransactionOption.NeedTransaction);
			jetConnection.DeleteTable(tableName);
		}

		internal static uint LCMapFlagsFromCompareOptions(CompareOptions compareOptions)
		{
			return Conversions.LCMapFlagsFromCompareOptions(CompareOptions.IgnoreCase | CompareOptions.IgnoreKanaType | CompareOptions.IgnoreWidth) | 134217728U;
		}

		public override void CreateTable(IConnectionProvider connectionProvider, int version)
		{
			this.TraceCreateTable(connectionProvider);
			JetConnection jetConnection = connectionProvider.GetConnection() as JetConnection;
			jetConnection.BeginTransactionIfNeeded(Connection.TransactionOption.NeedTransaction);
			JET_TABLECREATE jet_TABLECREATE = new JET_TABLECREATE
			{
				szTableName = base.Name,
				grbit = (base.IsPartitioned ? CreateTableColumnIndexGrbit.TemplateTable : CreateTableColumnIndexGrbit.None),
				rgcolumncreate = this.GetColumnCreates(version),
				rgindexcreate = this.GetIndexCreates(version)
			};
			jet_TABLECREATE.cColumns = jet_TABLECREATE.rgcolumncreate.Length;
			jet_TABLECREATE.cIndexes = jet_TABLECREATE.rgindexcreate.Length;
			jetConnection.CreateTable(jet_TABLECREATE);
		}

		public override void AddColumn(IConnectionProvider connectionProvider, PhysicalColumn column)
		{
			JetConnection jetConnection = connectionProvider.GetConnection() as JetConnection;
			Microsoft.Exchange.Server.Storage.Common.Globals.AssertRetail(jetConnection.TransactionStarted, "Transaction should already have been started");
			jetConnection.AddColumn(this, column);
		}

		public override void RemoveColumn(IConnectionProvider connectionProvider, PhysicalColumn column)
		{
			JetConnection jetConnection = connectionProvider.GetConnection() as JetConnection;
			Microsoft.Exchange.Server.Storage.Common.Globals.AssertRetail(jetConnection.TransactionStarted, "Transaction should already have been started");
			jetConnection.RemoveColumn(this, column);
		}

		public override void CreateIndex(IConnectionProvider connectionProvider, Index index, IList<object> partitionValues)
		{
			for (int i = 0; i < base.Indexes.Count; i++)
			{
				if (string.Compare(base.Indexes[i].Name, index.Name, StringComparison.OrdinalIgnoreCase) == 0)
				{
					return;
				}
			}
			string tableName = base.IsPartitioned ? JetPartitionHelper.GetPartitionName(this, partitionValues, base.NumberOfPartitioningColumns) : base.Name;
			JET_INDEXCREATE indexCreate = this.GetIndexCreate(index);
			JetConnection jetConnection = connectionProvider.GetConnection() as JetConnection;
			jetConnection.BeginTransactionIfNeeded(Connection.TransactionOption.NeedTransaction);
			jetConnection.CreateIndex(tableName, base.Name, base.TableClass, indexCreate);
		}

		public override void DeleteIndex(IConnectionProvider connectionProvider, Index index, IList<object> partitionValues)
		{
			for (int i = 0; i < base.Indexes.Count; i++)
			{
				if (string.Compare(base.Indexes[i].Name, index.Name, StringComparison.OrdinalIgnoreCase) == 0)
				{
					return;
				}
			}
			string partitionName = JetPartitionHelper.GetPartitionName(this, partitionValues, base.NumberOfPartitioningColumns);
			JET_INDEXCREATE indexCreate = this.GetIndexCreate(index);
			JetConnection jetConnection = connectionProvider.GetConnection() as JetConnection;
			jetConnection.BeginTransactionIfNeeded(Connection.TransactionOption.NeedTransaction);
			jetConnection.DeleteIndex(partitionName, base.TableClass, indexCreate.szIndexName);
		}

		public override bool IsIndexCreated(IConnectionProvider connectionProvider, Index index, IList<object> partitionValues)
		{
			string partitionName = JetPartitionHelper.GetPartitionName(this, partitionValues, base.NumberOfPartitioningColumns);
			JET_INDEXCREATE indexCreate = this.GetIndexCreate(index);
			JetConnection jetConnection = connectionProvider.GetConnection() as JetConnection;
			jetConnection.BeginTransactionIfNeeded(Connection.TransactionOption.DontNeedTransaction);
			return jetConnection.IsIndexCreated(this, partitionName, base.TableClass, indexCreate.szIndexName);
		}

		public override bool ValidateLocaleVersion(IConnectionProvider connectionProvider, IList<object> partitionValues)
		{
			JetConnection jetConnection = connectionProvider.GetConnection() as JetConnection;
			string tableName = base.IsPartitioned ? JetPartitionHelper.GetPartitionName(this, partitionValues, partitionValues.Count) : base.Name;
			return jetConnection.ValidateLocaleVersion(this, tableName, partitionValues);
		}

		public override void GetTableSize(IConnectionProvider connectionProvider, IList<object> partitionValues, out int totalPages, out int availablePages)
		{
			JetConnection jetConnection = connectionProvider.GetConnection() as JetConnection;
			string tableName = base.IsPartitioned ? JetPartitionHelper.GetPartitionName(this, partitionValues, partitionValues.Count) : base.Name;
			jetConnection.GetTableSize(tableName, out totalPages, out availablePages);
		}

		public override VirtualColumn VirtualColumn(string column)
		{
			VirtualColumnDefinition virtualColumnDefinition;
			if (JetTable.SupportedVirtualColumns.TryGetValue(column, out virtualColumnDefinition))
			{
				return new JetVirtualColumn(virtualColumnDefinition.Id, column, virtualColumnDefinition.Type, false, Visibility.Public, 0, virtualColumnDefinition.Size, this);
			}
			return null;
		}

		private JET_INDEXCREATE[] GetIndexCreates(int version)
		{
			List<JET_INDEXCREATE> list = new List<JET_INDEXCREATE>(base.Indexes.Count);
			for (int i = 0; i < base.Indexes.Count; i++)
			{
				if (base.Indexes[i].MinVersion <= version && version <= base.Indexes[i].MaxVersion)
				{
					list.Add(this.GetIndexCreate(base.Indexes[i]));
				}
			}
			return list.ToArray();
		}

		private JET_INDEXCREATE GetIndexCreate(Index index)
		{
			int num = 0;
			int num2 = 0;
			StringBuilder stringBuilder = new StringBuilder(20);
			List<JET_CONDITIONALCOLUMN> list = new List<JET_CONDITIONALCOLUMN>();
			for (int i = 0; i < index.Columns.Count; i++)
			{
				PhysicalColumn physicalColumn = (PhysicalColumn)index.Columns[i];
				if (physicalColumn.Index >= base.SpecialCols.NumberOfPartioningColumns)
				{
					if (index.Ascending[i])
					{
						stringBuilder.AppendFormat("+{0}\0", physicalColumn.PhysicalName);
					}
					else
					{
						stringBuilder.AppendFormat("-{0}\0", physicalColumn.PhysicalName);
					}
					if (index.Conditional[i])
					{
						list.Add(new JET_CONDITIONALCOLUMN
						{
							szColumnName = physicalColumn.PhysicalName,
							grbit = ConditionalColumnGrbit.ColumnMustBeNonNull
						});
					}
					if (physicalColumn.MaxLength != 0 || physicalColumn.ExtendedTypeCode == ExtendedTypeCode.String || physicalColumn.Size > 32)
					{
						num++;
					}
					else
					{
						num2 += 1 + physicalColumn.Size;
					}
				}
			}
			CreateIndexGrbit createIndexGrbit = (CreateIndexGrbit)262144;
			if (index.PrimaryKey)
			{
				createIndexGrbit |= CreateIndexGrbit.IndexPrimary;
			}
			if (index.Unique)
			{
				createIndexGrbit |= CreateIndexGrbit.IndexUnique;
			}
			JET_INDEXCREATE jet_INDEXCREATE = new JET_INDEXCREATE();
			jet_INDEXCREATE.szIndexName = index.Name;
			jet_INDEXCREATE.szKey = stringBuilder.ToString();
			jet_INDEXCREATE.cbKey = jet_INDEXCREATE.szKey.Length + 1;
			if (list.Count > 0)
			{
				jet_INDEXCREATE.rgconditionalcolumn = list.ToArray();
				jet_INDEXCREATE.cConditionalColumn = list.Count;
			}
			jet_INDEXCREATE.pidxUnicode = new JET_UNICODEINDEX();
			jet_INDEXCREATE.pidxUnicode.dwMapFlags = JetTable.LCMapFlagsFromCompareOptions(CompareOptions.IgnoreCase | CompareOptions.IgnoreKanaType | CompareOptions.IgnoreWidth);
			jet_INDEXCREATE.pidxUnicode.szLocaleName = base.Culture.CompareInfo.Name;
			if (num > 0)
			{
				jet_INDEXCREATE.cbVarSegMac = (2000 - num2) / num;
				jet_INDEXCREATE.cbKeyMost = 2000;
			}
			jet_INDEXCREATE.grbit = createIndexGrbit;
			if (base.TableAccessHints != TableAccessHints.None)
			{
				jet_INDEXCREATE.pSpaceHints = new JET_SPACEHINTS
				{
					grbit = SpaceHintsGrbit.RetrieveHintTableScanForward
				};
			}
			return jet_INDEXCREATE;
		}

		private JET_COLUMNCREATE[] GetColumnCreates(int version)
		{
			List<JET_COLUMNCREATE> list = new List<JET_COLUMNCREATE>(base.Columns.Count);
			for (int i = base.SpecialCols.NumberOfPartioningColumns; i < base.Columns.Count; i++)
			{
				if (base.Columns[i].MinVersion <= version && version <= base.Columns[i].MaxVersion)
				{
					list.Add(JetConnection.GetColumnCreate(base.Columns[i]));
				}
			}
			return list.ToArray();
		}

		private void TraceCreateTable(IConnectionProvider connectionProvider)
		{
			if (ExTraceGlobals.DbInteractionSummaryTracer.IsTraceEnabled(TraceType.DebugTrace))
			{
				StringBuilder stringBuilder = new StringBuilder(100);
				if (ExTraceGlobals.DbInteractionDetailTracer.IsTraceEnabled(TraceType.DebugTrace))
				{
					stringBuilder.Append("cn:[");
					stringBuilder.Append(connectionProvider.GetConnection().GetHashCode());
					stringBuilder.Append("] ");
				}
				stringBuilder.Append("Create table [");
				stringBuilder.Append(base.Name);
				stringBuilder.Append("]");
				ExTraceGlobals.DbInteractionSummaryTracer.TraceDebug(0L, stringBuilder.ToString());
			}
		}

		public const int LongBinaryThreshold = 32;

		public const int DefaultKeySize = 255;

		public const int MaxKeySize = 2000;

		public static Dictionary<string, VirtualColumnDefinition> SupportedVirtualColumns = new Dictionary<string, VirtualColumnDefinition>(StringComparer.OrdinalIgnoreCase)
		{
			{
				"DatabasePageNumber",
				new VirtualColumnDefinition(VirtualColumnId.PageNumber, typeof(int), 4, "DatabasePageNumber", "Logical page number on which the current row resides.")
			},
			{
				"RecordDataSize",
				new VirtualColumnDefinition(VirtualColumnId.DataSize, typeof(long), 8, "RecordDataSize", "The size in bytes of the user data set in the record.")
			},
			{
				"RecordLongValueDataSize",
				new VirtualColumnDefinition(VirtualColumnId.LongValueDataSize, typeof(long), 8, "RecordLongValueDataSize", "The size in bytes of the user data set in the record, but stored in the long-value tree.")
			},
			{
				"RecordOverheadSize",
				new VirtualColumnDefinition(VirtualColumnId.OverheadSize, typeof(long), 8, "RecordOverheadSize", "The size in bytes of the overhead of the ESENT record structure for this record. This includes the record's key size.")
			},
			{
				"RecordLongValueOverheadSize",
				new VirtualColumnDefinition(VirtualColumnId.LongValueOverheadSize, typeof(long), 8, "RecordLongValueOverheadSize", "The size in bytes of the overhead of the long-value data.")
			},
			{
				"RecordNonTaggedColumnCount",
				new VirtualColumnDefinition(VirtualColumnId.NonTaggedColumnCount, typeof(long), 8, "RecordNonTaggedColumnCount", "The total number of fixed and variable columns set in this record.")
			},
			{
				"RecordTaggedColumnCount",
				new VirtualColumnDefinition(VirtualColumnId.TaggedColumnCount, typeof(long), 8, "RecordTaggedColumnCount", "The total number of tagged columns set in this record.")
			},
			{
				"RecordLongValueCount",
				new VirtualColumnDefinition(VirtualColumnId.LongValueCount, typeof(long), 8, "RecordLongValueCount", "The total number of long values stored in the long-value tree for this record. This does not include intrinsic long values.")
			},
			{
				"RecordMultiValueCount",
				new VirtualColumnDefinition(VirtualColumnId.MultiValueCount, typeof(long), 8, "RecordMultiValueCount", "The accumulation of the total number of values beyond the first for all columns in the record.")
			},
			{
				"RecordCompressedColumnCount",
				new VirtualColumnDefinition(VirtualColumnId.CompressedColumnCount, typeof(long), 8, "RecordCompressedColumnCount", "The total number of columns in the record which are compressed.")
			},
			{
				"RecordCompressedDataSize",
				new VirtualColumnDefinition(VirtualColumnId.CompressedDataSize, typeof(long), 8, "RecordCompressedDataSize", "The size in bytes of the compressed size of user data in record. This is the same as DataSize if no intrinsic long-values are compressed.")
			},
			{
				"RecordCompressedLongValueDataSize",
				new VirtualColumnDefinition(VirtualColumnId.CompressedLongValueDataSize, typeof(long), 8, "RecordCompressedLongValueDataSize", "The size in bytes of the compressed size of user data in the long-value tree. This is the same as LongValueDataSize if no separated long values.")
			}
		};
	}
}
