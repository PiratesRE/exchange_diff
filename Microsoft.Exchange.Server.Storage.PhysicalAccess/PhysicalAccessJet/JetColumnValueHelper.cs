using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.ManagedStore.PhysicalAccess;
using Microsoft.Exchange.Server.Storage.Common;
using Microsoft.Exchange.Server.Storage.PhysicalAccess;
using Microsoft.Isam.Esent.Interop;

namespace Microsoft.Exchange.Server.Storage.PhysicalAccessJet
{
	internal static class JetColumnValueHelper
	{
		public static object GetValueFromJetValue(PhysicalColumn column, object value)
		{
			if (value == null && !column.IsNullable)
			{
				Microsoft.Exchange.Server.Storage.Common.Globals.AssertRetail(false, string.Format("non-nullable column {0} in table {1} contains null", column.Name, column.Table.Name));
			}
			switch (column.ExtendedTypeCode)
			{
			case ExtendedTypeCode.Boolean:
			case ExtendedTypeCode.Int16:
			case ExtendedTypeCode.Int32:
			case ExtendedTypeCode.Int64:
			case ExtendedTypeCode.Single:
			case ExtendedTypeCode.Double:
			case ExtendedTypeCode.Guid:
			case ExtendedTypeCode.String:
			case ExtendedTypeCode.Binary:
				return value;
			case ExtendedTypeCode.DateTime:
				if (value != null)
				{
					DateTime dateTime;
					ParseSerialize.TryConvertFileTime((long)value, out dateTime);
					return dateTime;
				}
				return value;
			case ExtendedTypeCode.MVInt16:
			case ExtendedTypeCode.MVInt32:
			case ExtendedTypeCode.MVInt64:
			case ExtendedTypeCode.MVSingle:
			case ExtendedTypeCode.MVDouble:
			case ExtendedTypeCode.MVDateTime:
			case ExtendedTypeCode.MVGuid:
			case ExtendedTypeCode.MVString:
			case ExtendedTypeCode.MVBinary:
				if (value != null)
				{
					return SerializedValue.Parse((byte[])value);
				}
				return value;
			}
			throw new InvalidOperationException(string.Format("Unknown or unexpected extended type code {0} for a column {1} having type {2}", column.ExtendedTypeCode, column, column.Type));
		}

		public static Microsoft.Isam.Esent.Interop.ColumnValue[] GetColumnValues(JetConnection jetConnection, HashSet<PhysicalColumn> columns, BitArray retrieveFromPrimaryBookmarkMap, BitArray retrieveFromIndexMap)
		{
			Microsoft.Isam.Esent.Interop.ColumnValue[] array = new Microsoft.Isam.Esent.Interop.ColumnValue[columns.Count];
			int num = 0;
			foreach (PhysicalColumn physicalColumn in columns)
			{
				array[num++] = JetColumnValueHelper.CreateColumnValue(jetConnection, (JetPhysicalColumn)physicalColumn, retrieveFromPrimaryBookmarkMap, retrieveFromIndexMap);
			}
			Array.Sort<Microsoft.Isam.Esent.Interop.ColumnValue>(array, ColumnValueComparer.Instance);
			return array;
		}

		public static Microsoft.Isam.Esent.Interop.ColumnValue CreateColumnValue(JetConnection jetConnection, JetPhysicalColumn column, BitArray retrieveFromPrimaryBookmarkMap, BitArray retrieveFromIndexMap)
		{
			Microsoft.Isam.Esent.Interop.ColumnValue columnValue;
			switch (column.ExtendedTypeCode)
			{
			case ExtendedTypeCode.Boolean:
				columnValue = new BoolColumnValue();
				goto IL_EB;
			case ExtendedTypeCode.Int16:
				columnValue = new Int16ColumnValue();
				goto IL_EB;
			case ExtendedTypeCode.Int32:
				columnValue = new Int32ColumnValue();
				goto IL_EB;
			case ExtendedTypeCode.Int64:
				columnValue = new Int64ColumnValue();
				goto IL_EB;
			case ExtendedTypeCode.Single:
				columnValue = new FloatColumnValue();
				goto IL_EB;
			case ExtendedTypeCode.Double:
				columnValue = new DoubleColumnValue();
				goto IL_EB;
			case ExtendedTypeCode.DateTime:
				columnValue = new Int64ColumnValue();
				goto IL_EB;
			case ExtendedTypeCode.Guid:
				columnValue = new GuidColumnValue();
				goto IL_EB;
			case ExtendedTypeCode.String:
				columnValue = new StringColumnValue();
				goto IL_EB;
			case ExtendedTypeCode.Binary:
			case ExtendedTypeCode.MVInt16:
			case ExtendedTypeCode.MVInt32:
			case ExtendedTypeCode.MVInt64:
			case ExtendedTypeCode.MVSingle:
			case ExtendedTypeCode.MVDouble:
			case ExtendedTypeCode.MVDateTime:
			case ExtendedTypeCode.MVGuid:
			case ExtendedTypeCode.MVString:
			case ExtendedTypeCode.MVBinary:
				columnValue = new BytesColumnValue();
				goto IL_EB;
			}
			throw new InvalidOperationException(string.Format("Unknown or unexpected extended type code {0} for a column {1} having type {2}", column.ExtendedTypeCode, column, column.Type));
			IL_EB:
			columnValue.Columnid = column.GetJetColumnId(jetConnection);
			columnValue.RetrieveGrbit = JetRetrieveColumnHelper.GetColumnRetrieveGrbit(column, retrieveFromPrimaryBookmarkMap, retrieveFromIndexMap);
			return columnValue;
		}

		public static void GetPhysicalColumns(Microsoft.Exchange.Server.Storage.PhysicalAccess.Table table, IEnumerable<Column> columns, HashSet<PhysicalColumn> excludeColumns, IColumnResolver columnResolver, HashSet<PhysicalColumn> columnValues)
		{
			if (columns != null)
			{
				IList<Column> list = columns as IList<Column>;
				if (list != null)
				{
					Action<Column, object> action = null;
					for (int i = 0; i < list.Count; i++)
					{
						JetColumnValueHelper.ResolveAndAddPhysicalColumn(table, columnValues, excludeColumns, list[i], columnResolver, ref action);
					}
					return;
				}
				Action<Column, object> action2 = null;
				foreach (Column unresolvedColumn in columns)
				{
					JetColumnValueHelper.ResolveAndAddPhysicalColumn(table, columnValues, excludeColumns, unresolvedColumn, columnResolver, ref action2);
				}
			}
		}

		internal static bool CanReuseColumnValueBuffer(Column column)
		{
			return column.Size > 0 && column.Size <= 16 && column.Type != JetColumnValueHelper.TypeOfByteArray && !column.Type.IsArray;
		}

		internal static byte[] AllocateColumnValueBuffer()
		{
			return new byte[16];
		}

		internal static object GetAsObject(ArraySegment<byte> bytes, Column column)
		{
			switch (column.ExtendedTypeCode)
			{
			case ExtendedTypeCode.Boolean:
				if (bytes.Array[bytes.Offset] == 0)
				{
					return SerializedValue.BoxedFalse;
				}
				return SerializedValue.BoxedTrue;
			case ExtendedTypeCode.Int16:
				return BitConverter.ToInt16(bytes.Array, bytes.Offset);
			case ExtendedTypeCode.Int32:
				return SerializedValue.GetBoxedInt32(BitConverter.ToInt32(bytes.Array, bytes.Offset));
			case ExtendedTypeCode.Int64:
				return BitConverter.ToInt64(bytes.Array, bytes.Offset);
			case ExtendedTypeCode.Single:
				return BitConverter.ToSingle(bytes.Array, bytes.Offset);
			case ExtendedTypeCode.Double:
				return BitConverter.ToDouble(bytes.Array, bytes.Offset);
			case ExtendedTypeCode.DateTime:
				return ParseSerialize.ParseFileTime(bytes.Array, bytes.Offset);
			case ExtendedTypeCode.Guid:
				return new Guid(bytes.Array);
			case ExtendedTypeCode.String:
				return Encoding.Unicode.GetString(bytes.Array, bytes.Offset, bytes.Count);
			case ExtendedTypeCode.Binary:
				return bytes.Array;
			case ExtendedTypeCode.MVInt16:
			case ExtendedTypeCode.MVInt32:
			case ExtendedTypeCode.MVInt64:
			case ExtendedTypeCode.MVSingle:
			case ExtendedTypeCode.MVDouble:
			case ExtendedTypeCode.MVDateTime:
			case ExtendedTypeCode.MVGuid:
			case ExtendedTypeCode.MVString:
			case ExtendedTypeCode.MVBinary:
				return SerializedValue.Parse(bytes.Array);
			}
			throw new InvalidOperationException(string.Format("Unknown or unexpected extended type code {0} for a column {1} having type {2}", column.ExtendedTypeCode, column, column.Type));
		}

		internal static byte[] GetAsByteArray(object val, Column column)
		{
			if (val == null)
			{
				if (!column.IsNullable)
				{
					Microsoft.Exchange.Server.Storage.Common.Globals.AssertRetail(false, string.Format("non-nullable column {0} in table {1} contains null", column.Name, column.Table.Name));
				}
				return null;
			}
			if (val is LargeValue)
			{
				throw new StoreException((LID)62256U, ErrorCodeValue.TooComplex, "Large column values cannot be retrieved by value and must be processed as streams.");
			}
			switch (column.ExtendedTypeCode)
			{
			case ExtendedTypeCode.Boolean:
				if (!(bool)val)
				{
					return JetColumnValueHelper.BytesFromFalse;
				}
				return JetColumnValueHelper.BytesFromTrue;
			case ExtendedTypeCode.Int16:
				return BitConverter.GetBytes((short)val);
			case ExtendedTypeCode.Int32:
				return BitConverter.GetBytes((int)val);
			case ExtendedTypeCode.Int64:
				return BitConverter.GetBytes((long)val);
			case ExtendedTypeCode.Single:
				return BitConverter.GetBytes((float)val);
			case ExtendedTypeCode.Double:
				return BitConverter.GetBytes((double)val);
			case ExtendedTypeCode.DateTime:
			{
				byte[] array = new byte[8];
				ParseSerialize.SerializeFileTime((DateTime)val, array, 0);
				return array;
			}
			case ExtendedTypeCode.Guid:
				return ((Guid)val).ToByteArray();
			case ExtendedTypeCode.String:
			{
				string s = (string)val;
				return Encoding.Unicode.GetBytes(s);
			}
			case ExtendedTypeCode.Binary:
				return (byte[])val;
			case ExtendedTypeCode.MVInt16:
			case ExtendedTypeCode.MVInt32:
			case ExtendedTypeCode.MVInt64:
			case ExtendedTypeCode.MVSingle:
			case ExtendedTypeCode.MVDouble:
			case ExtendedTypeCode.MVDateTime:
			case ExtendedTypeCode.MVGuid:
			case ExtendedTypeCode.MVString:
			case ExtendedTypeCode.MVBinary:
				return SerializedValue.Serialize(val);
			}
			throw new InvalidOperationException(string.Format("Unknown or unexpected extended type code {0} for a column {1} having type {2}", column.ExtendedTypeCode, column, column.Type));
		}

		internal static object LoadFullOrTruncatedValue(JetPhysicalColumn column, IColumnStreamAccess streamAccess)
		{
			int columnSize = streamAccess.GetColumnSize(column);
			int num = object.ReferenceEquals(column, column.Table.SpecialCols.OffPagePropertyBlob) ? 65536 : 8192;
			if (columnSize >= num)
			{
				if (ExTraceGlobals.DbInteractionDetailTracer.IsTraceEnabled(TraceType.DebugTrace))
				{
					ExTraceGlobals.DbInteractionDetailTracer.TraceDebug<JetPhysicalColumn>(0L, "Column {0} is large streamable column. Retrieving a truncated value.", column);
				}
				byte[] array = new byte[2048];
				streamAccess.ReadStream(column, 0L, array, 0, array.Length);
				return new LargeValue((long)columnSize, array);
			}
			if (columnSize > 0)
			{
				if (ExTraceGlobals.DbInteractionDetailTracer.IsTraceEnabled(TraceType.DebugTrace))
				{
					ExTraceGlobals.DbInteractionDetailTracer.TraceDebug<JetPhysicalColumn>(0L, "Column {0} is small streamable column. Retrieving a full value.", column);
				}
				byte[] array2 = new byte[columnSize];
				streamAccess.ReadStream(column, 0L, array2, 0, array2.Length);
				return array2;
			}
			return null;
		}

		internal static void MakeJetKeyFromValue(JET_SESID sessionId, JET_TABLEID tableId, MakeKeyGrbit grbit, object value, Column column)
		{
			byte[] data = null;
			if (value != null)
			{
				if (column.MaxLength != 0)
				{
					bool flag;
					value = ValueHelper.TruncateValueIfNecessary(value, column.Type, column.MaxLength, out flag);
				}
				switch (column.ExtendedTypeCode)
				{
				case ExtendedTypeCode.Boolean:
					Api.MakeKey(sessionId, tableId, (bool)value, grbit);
					return;
				case ExtendedTypeCode.Int16:
					Api.MakeKey(sessionId, tableId, (short)value, grbit);
					return;
				case ExtendedTypeCode.Int32:
					Api.MakeKey(sessionId, tableId, (int)value, grbit);
					return;
				case ExtendedTypeCode.Int64:
					Api.MakeKey(sessionId, tableId, (long)value, grbit);
					return;
				case ExtendedTypeCode.Single:
					Api.MakeKey(sessionId, tableId, (float)value, grbit);
					return;
				case ExtendedTypeCode.Double:
					Api.MakeKey(sessionId, tableId, (double)value, grbit);
					return;
				case ExtendedTypeCode.Guid:
					Api.MakeKey(sessionId, tableId, (Guid)value, grbit);
					return;
				case ExtendedTypeCode.String:
					Api.MakeKey(sessionId, tableId, (string)value, Encoding.Unicode, grbit);
					return;
				case ExtendedTypeCode.Binary:
					Api.MakeKey(sessionId, tableId, (byte[])value, grbit);
					return;
				}
				data = JetColumnValueHelper.GetAsByteArray(value, column);
			}
			Api.MakeKey(sessionId, tableId, data, grbit);
		}

		private static void ResolveAndAddPhysicalColumn(Microsoft.Exchange.Server.Storage.PhysicalAccess.Table table, HashSet<PhysicalColumn> columnValues, HashSet<PhysicalColumn> excludeColumns, Column unresolvedColumn, IColumnResolver columnResolver, ref Action<Column, object> enumerateColumnsCallback)
		{
			Column actualColumn = columnResolver.Resolve(unresolvedColumn).ActualColumn;
			if (actualColumn.Table != table)
			{
				return;
			}
			if (actualColumn is PhysicalColumn)
			{
				PhysicalColumn physicalColumn = (PhysicalColumn)actualColumn;
				if (!JetPartitionHelper.IsPartitioningColumn(table, physicalColumn) && (excludeColumns == null || !excludeColumns.Contains(physicalColumn)) && !physicalColumn.StreamSupport && physicalColumn.Index != -1)
				{
					columnValues.Add(physicalColumn);
					return;
				}
			}
			else
			{
				if (enumerateColumnsCallback == null)
				{
					Microsoft.Exchange.Server.Storage.PhysicalAccess.Table localTable = table;
					HashSet<PhysicalColumn> localExcludeColumns = excludeColumns;
					enumerateColumnsCallback = delegate(Column x, object columnsSet)
					{
						if (x.Table == localTable)
						{
							PhysicalColumn physicalColumn2 = x.ActualColumn as PhysicalColumn;
							if (physicalColumn2 != null && !JetPartitionHelper.IsPartitioningColumn(localTable, physicalColumn2) && (localExcludeColumns == null || !localExcludeColumns.Contains(physicalColumn2)) && !physicalColumn2.StreamSupport && physicalColumn2.Index != -1)
							{
								((HashSet<PhysicalColumn>)columnsSet).Add(physicalColumn2);
								return;
							}
							if (x is PropertyColumn && null != localTable.SpecialCols.PropertyBlob && (localExcludeColumns == null || !localExcludeColumns.Contains(localTable.SpecialCols.PropertyBlob)))
							{
								((HashSet<PhysicalColumn>)columnsSet).Add(localTable.SpecialCols.PropertyBlob);
							}
						}
					};
				}
				actualColumn.EnumerateColumns(enumerateColumnsCallback, columnValues);
			}
		}

		// Note: this type is marked as 'beforefieldinit'.
		static JetColumnValueHelper()
		{
			byte[] bytesFromFalse = new byte[1];
			JetColumnValueHelper.BytesFromFalse = bytesFromFalse;
			JetColumnValueHelper.BytesFromTrue = new byte[]
			{
				byte.MaxValue
			};
		}

		private const int BufferReuseSize = 16;

		private static readonly Type TypeOfByteArray = typeof(byte[]);

		private static readonly Type TypeOfByteArrayArray = typeof(byte[][]);

		private static readonly Type TypeOfStringArray = typeof(string[]);

		private static readonly Type TypeOfShortArray = typeof(short[]);

		private static readonly Type TypeOfIntArray = typeof(int[]);

		private static readonly Type TypeOfLongArray = typeof(long[]);

		private static readonly Type TypeOfDoubleArray = typeof(double[]);

		private static readonly Type TypeOfFloatArray = typeof(float[]);

		private static readonly Type TypeOfGuid = typeof(Guid);

		private static readonly Type TypeOfGuidArray = typeof(Guid[]);

		private static readonly Type TypeOfDateTimeArray = typeof(DateTime[]);

		private static readonly byte[] BytesFromFalse;

		private static readonly byte[] BytesFromTrue;
	}
}
