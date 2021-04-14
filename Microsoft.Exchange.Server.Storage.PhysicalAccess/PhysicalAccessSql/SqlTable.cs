using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.ManagedStore.PhysicalAccess;
using Microsoft.Exchange.Server.Storage.Common;
using Microsoft.Exchange.Server.Storage.PhysicalAccess;

namespace Microsoft.Exchange.Server.Storage.PhysicalAccessSql
{
	public class SqlTable : Table
	{
		public SqlTable(string name, TableClass tableClass, CultureInfo culture, bool trackDirtyObjects, TableAccessHints tableAccessHints, bool readOnly, Visibility visibility, bool schemaExtension, SpecialColumns specialCols, Index[] indexes, PhysicalColumn[] computedColumns, PhysicalColumn[] columns) : base(name, tableClass, culture, trackDirtyObjects, tableAccessHints, readOnly, visibility, schemaExtension, specialCols, indexes, computedColumns, columns)
		{
		}

		public static void DeleteSqlTable(IConnectionProvider connectionProvider, string tableName)
		{
			using (SqlCommand sqlCommand = new SqlCommand(connectionProvider.GetConnection()))
			{
				sqlCommand.AppendStatement("DROP TABLE [Exchange].[", Connection.OperationType.Other);
				sqlCommand.Append(tableName);
				sqlCommand.Append("]");
				sqlCommand.ExecuteNonQuery();
			}
		}

		private static string GetSqlType(Type colType, int maxLength, bool fixedLength)
		{
			switch (ValueTypeHelper.GetExtendedTypeCode(colType))
			{
			case ExtendedTypeCode.Boolean:
				return "bit";
			case ExtendedTypeCode.Int16:
				return "smallint";
			case ExtendedTypeCode.Int32:
				return "int";
			case ExtendedTypeCode.Int64:
				return "bigint";
			case ExtendedTypeCode.Single:
				return "float(24)";
			case ExtendedTypeCode.Double:
				return "float(53)";
			case ExtendedTypeCode.DateTime:
				return "datetime2";
			case ExtendedTypeCode.Guid:
				return "uniqueidentifier";
			case ExtendedTypeCode.String:
				if (maxLength > 8000)
				{
					return "nvarchar(max)";
				}
				if (fixedLength)
				{
					return string.Format("nchar({0})", maxLength);
				}
				return string.Format("nvarchar({0})", maxLength);
			case ExtendedTypeCode.Binary:
				if (maxLength > 8000)
				{
					return "varbinary(max)";
				}
				if (fixedLength)
				{
					return string.Format("binary({0})", maxLength);
				}
				return string.Format("varbinary({0})", maxLength);
			case ExtendedTypeCode.MVInt16:
			case ExtendedTypeCode.MVInt32:
			case ExtendedTypeCode.MVInt64:
			case ExtendedTypeCode.MVSingle:
			case ExtendedTypeCode.MVDouble:
			case ExtendedTypeCode.MVDateTime:
			case ExtendedTypeCode.MVGuid:
			case ExtendedTypeCode.MVString:
			case ExtendedTypeCode.MVBinary:
				if (maxLength > 8000)
				{
					return "varbinary(max)";
				}
				if (fixedLength)
				{
					return string.Format("binary({0})", maxLength);
				}
				return string.Format("varbinary({0})", maxLength);
			}
			throw new InvalidOperationException("GetSqlType: type " + colType + " is not valid");
		}

		public override void CreateTable(IConnectionProvider connectionProvider, int version)
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
				stringBuilder.Append("Create table ");
				stringBuilder.Append(base.Name);
				ExTraceGlobals.DbInteractionSummaryTracer.TraceDebug(0L, stringBuilder.ToString());
			}
			using (SqlCommand sqlCommand = new SqlCommand(connectionProvider.GetConnection()))
			{
				sqlCommand.AppendStatement("CREATE TABLE [Exchange].[", Connection.OperationType.Other);
				sqlCommand.Append(base.Name);
				sqlCommand.Append("](");
				for (int i = 0; i < base.Columns.Count; i++)
				{
					this.FormatSqlColumn(base.Columns[i], sqlCommand, i == 0);
				}
				sqlCommand.Append(")");
				sqlCommand.Append(" WITH (DATA_COMPRESSION = PAGE)");
				for (int j = 0; j < base.Indexes.Count; j++)
				{
					sqlCommand.StartNewStatement(Connection.OperationType.Other);
					sqlCommand.Append("CREATE ");
					if (base.Indexes[j].Unique)
					{
						sqlCommand.Append("UNIQUE ");
					}
					if (base.Indexes[j].PrimaryKey)
					{
						sqlCommand.Append("CLUSTERED ");
					}
					sqlCommand.Append("INDEX [");
					sqlCommand.Append(base.Indexes[j].Name);
					sqlCommand.Append("] ON [Exchange].[");
					sqlCommand.Append(base.Name);
					sqlCommand.Append("](");
					for (int k = 0; k < base.Indexes[j].Columns.Count; k++)
					{
						if (k != 0)
						{
							sqlCommand.Append(", ");
						}
						sqlCommand.Append(base.Indexes[j].Columns[k].Name);
						if (base.Indexes[j].Ascending[k])
						{
							sqlCommand.Append(" ASC");
						}
						else
						{
							sqlCommand.Append(" DESC");
						}
					}
					sqlCommand.Append(")");
				}
				sqlCommand.StartNewStatement(Connection.OperationType.Other);
				sqlCommand.Append("ALTER INDEX ALL ON [Exchange].[");
				sqlCommand.Append(base.Name);
				sqlCommand.Append("] SET (ALLOW_PAGE_LOCKS = OFF)");
				sqlCommand.ExecuteNonQuery();
			}
		}

		public override void AddColumn(IConnectionProvider connectionProvider, PhysicalColumn column)
		{
			throw new NotSupportedException("AddColumn not supported against SQL table");
		}

		public override void RemoveColumn(IConnectionProvider connectionProvider, PhysicalColumn column)
		{
			throw new NotSupportedException("RemoveColumn not supported against SQL table");
		}

		public override void CreateIndex(IConnectionProvider connectionProvider, Index index, IList<object> partitionValues)
		{
			throw new NotSupportedException("CreateIndex not supported against SQL table");
		}

		public override void DeleteIndex(IConnectionProvider connectionProvider, Index index, IList<object> partitionValues)
		{
			throw new NotSupportedException("DeleteIndex not supported against SQL table");
		}

		public override bool IsIndexCreated(IConnectionProvider connectionProvider, Index index, IList<object> partitionValues)
		{
			throw new NotSupportedException("IsIndexCreated not supported against SQL table");
		}

		public override bool ValidateLocaleVersion(IConnectionProvider connectionProvider, IList<object> partitionValues)
		{
			return true;
		}

		public override void GetTableSize(IConnectionProvider connectionProvider, IList<object> partitionValues, out int totalPages, out int availablePages)
		{
			totalPages = 0;
			availablePages = 0;
		}

		private void FormatSqlColumn(PhysicalColumn column, SqlCommand command, bool first)
		{
			if (!first)
			{
				command.Append(", ");
			}
			command.Append("[");
			command.Append(column.Name);
			command.Append("] ");
			if (column.MaxLength > 0)
			{
				command.Append(SqlTable.GetSqlType(column.Type, column.MaxLength, false));
			}
			else
			{
				command.Append(SqlTable.GetSqlType(column.Type, column.Size, true));
			}
			SqlCollationHelper.AppendCollation(column, base.Culture, command);
			if (column.IsNullable)
			{
				command.Append(" NULL");
			}
			else
			{
				command.Append(" NOT NULL");
			}
			if (column.IsIdentity)
			{
				command.Append(" IDENTITY");
			}
		}

		private const int NonMaxColumnLength = 8000;
	}
}
