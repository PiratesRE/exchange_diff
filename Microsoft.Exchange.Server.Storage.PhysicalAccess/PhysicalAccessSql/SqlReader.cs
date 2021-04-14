using System;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Text;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.ManagedStore.PhysicalAccess;
using Microsoft.Exchange.Server.Storage.Common;
using Microsoft.Exchange.Server.Storage.Common.ExtensionMethods;
using Microsoft.Exchange.Server.Storage.PhysicalAccess;

namespace Microsoft.Exchange.Server.Storage.PhysicalAccessSql
{
	internal sealed class SqlReader : Reader
	{
		internal SqlReader(ISqlDataReader reader, Connection connection, int skipTo, SimpleQueryOperator simpleQueryOperator, bool disposeQueryOperator) : base(connection, simpleQueryOperator, disposeQueryOperator)
		{
			this.reader = reader;
			this.skipTo = skipTo;
		}

		public override bool IsClosed
		{
			get
			{
				return this.reader.IsClosed;
			}
		}

		public override bool Read(out int rowsSkipped)
		{
			rowsSkipped = 0;
			bool result;
			try
			{
				bool flag;
				using (base.Connection.TrackDbOperationExecution(base.SimpleQueryOperator))
				{
					for (;;)
					{
						flag = this.reader.Read();
						if (!flag || this.skipTo <= 0)
						{
							break;
						}
						this.skipTo--;
						rowsSkipped++;
					}
				}
				this.UpdateRowsRead();
				if (flag)
				{
					this.TraceReadRecord(this.reader);
				}
				result = flag;
			}
			catch (SqlException ex)
			{
				base.Connection.OnExceptionCatch(ex);
				SqlConnection.LogSQLError("Reader", "Read", ex);
				throw ((SqlConnection)base.Connection).ProcessSqlError(ex);
			}
			return result;
		}

		public override bool? GetNullableBoolean(Column column)
		{
			int ordinal = this.GetOrdinal(column.Name);
			bool? result;
			try
			{
				bool? flag = null;
				if (!this.reader.IsDBNull(ordinal))
				{
					flag = new bool?(this.reader.GetBoolean(ordinal));
					this.UpdateBytesRead(2L);
				}
				else
				{
					this.UpdateBytesRead(1L);
				}
				result = flag;
			}
			catch (SqlException ex)
			{
				base.Connection.OnExceptionCatch(ex);
				SqlConnection.LogSQLError("Reader", "GetNullableBoolean", ex);
				throw ((SqlConnection)base.Connection).ProcessSqlError(ex);
			}
			return result;
		}

		public override bool GetBoolean(Column column)
		{
			int ordinal = this.GetOrdinal(column.Name);
			bool boolean;
			try
			{
				this.UpdateBytesRead(1L);
				boolean = this.reader.GetBoolean(ordinal);
			}
			catch (SqlException ex)
			{
				base.Connection.OnExceptionCatch(ex);
				SqlConnection.LogSQLError("Reader", "GetBoolean", ex);
				throw ((SqlConnection)base.Connection).ProcessSqlError(ex);
			}
			return boolean;
		}

		public override long? GetNullableInt64(Column column)
		{
			int ordinal = this.GetOrdinal(column.Name);
			long? result;
			try
			{
				long? num = null;
				if (!this.reader.IsDBNull(ordinal))
				{
					num = new long?(this.reader.GetInt64(ordinal));
					this.UpdateBytesRead(9L);
				}
				else
				{
					this.UpdateBytesRead(1L);
				}
				result = num;
			}
			catch (SqlException ex)
			{
				base.Connection.OnExceptionCatch(ex);
				SqlConnection.LogSQLError("Reader", "GetNullableInt64", ex);
				throw ((SqlConnection)base.Connection).ProcessSqlError(ex);
			}
			return result;
		}

		public override long GetInt64(Column column)
		{
			int ordinal = this.GetOrdinal(column.Name);
			long @int;
			try
			{
				this.UpdateBytesRead(8L);
				@int = this.reader.GetInt64(ordinal);
			}
			catch (SqlException ex)
			{
				base.Connection.OnExceptionCatch(ex);
				SqlConnection.LogSQLError("Reader", "GetInt64", ex);
				throw ((SqlConnection)base.Connection).ProcessSqlError(ex);
			}
			return @int;
		}

		public override int? GetNullableInt32(Column column)
		{
			int ordinal = this.GetOrdinal(column.Name);
			int? result;
			try
			{
				int? num = null;
				if (!this.reader.IsDBNull(ordinal))
				{
					num = new int?(this.reader.GetInt32(ordinal));
					this.UpdateBytesRead(5L);
				}
				else
				{
					this.UpdateBytesRead(1L);
				}
				result = num;
			}
			catch (SqlException ex)
			{
				base.Connection.OnExceptionCatch(ex);
				SqlConnection.LogSQLError("Reader", "GetNullableInt32", ex);
				throw ((SqlConnection)base.Connection).ProcessSqlError(ex);
			}
			return result;
		}

		public override int GetInt32(Column column)
		{
			int ordinal = this.GetOrdinal(column.Name);
			int @int;
			try
			{
				this.UpdateBytesRead(4L);
				@int = this.reader.GetInt32(ordinal);
			}
			catch (SqlException ex)
			{
				base.Connection.OnExceptionCatch(ex);
				SqlConnection.LogSQLError("Reader", "GetInt32", ex);
				throw ((SqlConnection)base.Connection).ProcessSqlError(ex);
			}
			return @int;
		}

		public override short? GetNullableInt16(Column column)
		{
			int ordinal = this.GetOrdinal(column.Name);
			short? result;
			try
			{
				short? num = null;
				if (!this.reader.IsDBNull(ordinal))
				{
					num = new short?(this.reader.GetInt16(ordinal));
					this.UpdateBytesRead(3L);
				}
				else
				{
					this.UpdateBytesRead(1L);
				}
				result = num;
			}
			catch (SqlException ex)
			{
				base.Connection.OnExceptionCatch(ex);
				SqlConnection.LogSQLError("Reader", "GetNullableInt16", ex);
				throw ((SqlConnection)base.Connection).ProcessSqlError(ex);
			}
			return result;
		}

		public override short GetInt16(Column column)
		{
			int ordinal = this.GetOrdinal(column.Name);
			short @int;
			try
			{
				this.UpdateBytesRead(2L);
				@int = this.reader.GetInt16(ordinal);
			}
			catch (SqlException ex)
			{
				base.Connection.OnExceptionCatch(ex);
				SqlConnection.LogSQLError("Reader", "GetInt16", ex);
				throw ((SqlConnection)base.Connection).ProcessSqlError(ex);
			}
			return @int;
		}

		public override Guid? GetNullableGuid(Column column)
		{
			int ordinal = this.GetOrdinal(column.Name);
			Guid? result;
			try
			{
				Guid? guid = null;
				if (!this.reader.IsDBNull(ordinal))
				{
					guid = new Guid?(this.reader.GetGuid(ordinal));
					this.UpdateBytesRead(17L);
				}
				else
				{
					this.UpdateBytesRead(1L);
				}
				result = guid;
			}
			catch (SqlException ex)
			{
				base.Connection.OnExceptionCatch(ex);
				SqlConnection.LogSQLError("Reader", "GetNullableGuid", ex);
				throw ((SqlConnection)base.Connection).ProcessSqlError(ex);
			}
			return result;
		}

		public override Guid GetGuid(Column column)
		{
			int ordinal = this.GetOrdinal(column.Name);
			Guid guid;
			try
			{
				this.UpdateBytesRead(16L);
				guid = this.reader.GetGuid(ordinal);
			}
			catch (SqlException ex)
			{
				base.Connection.OnExceptionCatch(ex);
				SqlConnection.LogSQLError("Reader", "GetGuid", ex);
				throw ((SqlConnection)base.Connection).ProcessSqlError(ex);
			}
			return guid;
		}

		public override DateTime GetDateTime(Column column)
		{
			int ordinal = this.GetOrdinal(column.Name);
			DateTime result;
			try
			{
				this.UpdateBytesRead(8L);
				DateTime dateTime = this.reader.GetDateTime(ordinal);
				result = dateTime;
			}
			catch (SqlException ex)
			{
				base.Connection.OnExceptionCatch(ex);
				SqlConnection.LogSQLError("Reader", "GetDateTime", ex);
				throw ((SqlConnection)base.Connection).ProcessSqlError(ex);
			}
			return result;
		}

		public override DateTime? GetNullableDateTime(Column column)
		{
			int ordinal = this.GetOrdinal(column.Name);
			DateTime? result;
			try
			{
				DateTime? dateTime = null;
				if (!this.reader.IsDBNull(ordinal))
				{
					dateTime = new DateTime?(this.GetDateTime(column));
					this.UpdateBytesRead(8L);
				}
				else
				{
					this.UpdateBytesRead(1L);
				}
				result = dateTime;
			}
			catch (SqlException ex)
			{
				base.Connection.OnExceptionCatch(ex);
				SqlConnection.LogSQLError("Reader", "GetNullableDateTime", ex);
				throw ((SqlConnection)base.Connection).ProcessSqlError(ex);
			}
			return result;
		}

		public override byte[] GetBinary(Column column)
		{
			int ordinal = this.GetOrdinal(column.Name);
			byte[] result;
			try
			{
				SqlBinary sqlBinary = null;
				byte[] array = null;
				if (!this.reader.IsDBNull(ordinal))
				{
					array = this.reader.GetSqlBinary(ordinal).Value;
					this.UpdateBytesRead((long)(array.Length + 1));
				}
				else
				{
					this.UpdateBytesRead(1L);
				}
				result = array;
			}
			catch (SqlException ex)
			{
				base.Connection.OnExceptionCatch(ex);
				SqlConnection.LogSQLError("Reader", "GetBinary", ex);
				throw ((SqlConnection)base.Connection).ProcessSqlError(ex);
			}
			return result;
		}

		public override string GetString(Column column)
		{
			int ordinal = this.GetOrdinal(column.Name);
			string result;
			try
			{
				string text = null;
				if (!this.reader.IsDBNull(ordinal))
				{
					text = this.reader.GetString(ordinal);
					this.UpdateBytesRead((long)(text.Length * 2 + 1));
				}
				else
				{
					this.UpdateBytesRead(1L);
				}
				result = text;
			}
			catch (SqlException ex)
			{
				base.Connection.OnExceptionCatch(ex);
				SqlConnection.LogSQLError("Reader", "GetString", ex);
				throw ((SqlConnection)base.Connection).ProcessSqlError(ex);
			}
			return result;
		}

		public override object GetValue(Column column)
		{
			int ordinal = this.GetOrdinal(column.Name);
			object result;
			try
			{
				object obj = null;
				if (!this.reader.IsDBNull(ordinal))
				{
					obj = this.reader.GetValue(ordinal);
					if (obj != null && obj is byte[] && column.Type != typeof(byte[]))
					{
						obj = SerializedValue.Parse((byte[])obj);
					}
					this.UpdateBytesRead(10L);
				}
				else
				{
					this.UpdateBytesRead(1L);
				}
				result = obj;
			}
			catch (SqlException ex)
			{
				base.Connection.OnExceptionCatch(ex);
				SqlConnection.LogSQLError("Reader", "GetValue", ex);
				throw ((SqlConnection)base.Connection).ProcessSqlError(ex);
			}
			return result;
		}

		public override long GetChars(Column column, long dataIndex, char[] outBuffer, int bufferIndex, int length)
		{
			int ordinal = this.GetOrdinal(column.Name);
			long result;
			try
			{
				long num = 0L;
				if (!this.reader.IsDBNull(ordinal))
				{
					num = this.reader.GetChars(ordinal, dataIndex, outBuffer, bufferIndex, length);
					this.UpdateBytesRead(num + 1L);
				}
				else
				{
					this.UpdateBytesRead(1L);
				}
				result = num;
			}
			catch (SqlException ex)
			{
				base.Connection.OnExceptionCatch(ex);
				SqlConnection.LogSQLError("Reader", "GetChars", ex);
				throw ((SqlConnection)base.Connection).ProcessSqlError(ex);
			}
			return result;
		}

		public override long GetBytes(Column column, long dataIndex, byte[] outBuffer, int bufferIndex, int length)
		{
			int ordinal = this.GetOrdinal(column.Name);
			long result;
			try
			{
				long bytes = this.reader.GetBytes(ordinal, dataIndex, outBuffer, bufferIndex, length);
				this.UpdateBytesRead(bytes);
				result = bytes;
			}
			catch (SqlException ex)
			{
				base.Connection.OnExceptionCatch(ex);
				SqlConnection.LogSQLError("Reader", "GetBytes", ex);
				throw ((SqlConnection)base.Connection).ProcessSqlError(ex);
			}
			return result;
		}

		public string GetStringByOrdinal(int i)
		{
			string result;
			try
			{
				string text = null;
				if (!this.reader.IsDBNull(i))
				{
					text = this.reader.GetString(i);
					this.UpdateBytesRead((long)(text.Length * 2 + 1));
				}
				else
				{
					this.UpdateBytesRead(1L);
				}
				result = text;
			}
			catch (SqlException ex)
			{
				base.Connection.OnExceptionCatch(ex);
				SqlConnection.LogSQLError("Reader", "GetString", ex);
				throw ((SqlConnection)base.Connection).ProcessSqlError(ex);
			}
			return result;
		}

		public object GetValueByOrdinal(int i)
		{
			object result;
			try
			{
				object obj = null;
				if (!this.reader.IsDBNull(i))
				{
					obj = this.reader.GetValue(i);
					this.UpdateBytesRead(10L);
				}
				else
				{
					this.UpdateBytesRead(1L);
				}
				result = obj;
			}
			catch (SqlException ex)
			{
				base.Connection.OnExceptionCatch(ex);
				SqlConnection.LogSQLError("Reader", "GetValue", ex);
				throw ((SqlConnection)base.Connection).ProcessSqlError(ex);
			}
			return result;
		}

		public override void Close()
		{
			if (!this.IsClosed)
			{
				try
				{
					if (ExTraceGlobals.DbIOTracer.IsTraceEnabled(TraceType.PerformanceTrace))
					{
						while (this.reader.NextResult())
						{
						}
					}
					this.reader.Close();
				}
				catch (SqlException ex)
				{
					base.Connection.OnExceptionCatch(ex);
					SqlConnection.LogSQLError("Reader", "Close", ex);
					throw ((SqlConnection)base.Connection).ProcessSqlError(ex);
				}
			}
			base.Connection.AddRowStatsCounter(null, RowStatsCounterType.Read, this.rowsRead);
			base.Connection.AddRowStatsCounter(null, RowStatsCounterType.ReadBytes, (int)this.bytesRead);
			base.Close();
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<SqlReader>(this);
		}

		protected override void InternalDispose(bool calledFromDispose)
		{
			if (calledFromDispose)
			{
				this.Close();
				this.reader.Dispose();
			}
			if (base.DisposeQueryOperator && base.SimpleQueryOperator != null)
			{
				base.SimpleQueryOperator.Dispose();
			}
			this.reader = null;
		}

		private void UpdateBytesRead(long bytesRead)
		{
			this.bytesRead += bytesRead;
		}

		private void UpdateRowsRead()
		{
			this.rowsRead++;
		}

		private int GetOrdinal(string name)
		{
			int ordinal;
			try
			{
				ordinal = this.reader.GetOrdinal(name);
			}
			catch (SqlException ex)
			{
				base.Connection.OnExceptionCatch(ex);
				SqlConnection.LogSQLError("Reader", "GetOrdinal", ex);
				throw ((SqlConnection)base.Connection).ProcessSqlError(ex);
			}
			return ordinal;
		}

		private void TraceReadRecord(ISqlDataReader reader)
		{
			if (ExTraceGlobals.DbInteractionIntermediateTracer.IsTraceEnabled(TraceType.DebugTrace))
			{
				StringBuilder stringBuilder = new StringBuilder(100);
				stringBuilder.Append(" <<< Read row  cols:[");
				for (int i = 0; i < reader.FieldCount; i++)
				{
					if (i != 0)
					{
						stringBuilder.Append(", ");
					}
					stringBuilder.Append(reader.GetName(i));
					stringBuilder.Append("=[");
					object value = reader.GetValue(i);
					if (ExTraceGlobals.DbInteractionDetailTracer.IsTraceEnabled(TraceType.DebugTrace) || !(value is byte[]) || ((byte[])value).Length <= 32)
					{
						stringBuilder.AppendAsString(value);
					}
					else
					{
						stringBuilder.Append("<long_blob>");
					}
					stringBuilder.Append("]");
				}
				ExTraceGlobals.DbInteractionIntermediateTracer.TraceDebug(0L, stringBuilder.ToString());
			}
		}

		private ISqlDataReader reader;

		private int skipTo;

		private long bytesRead;

		private int rowsRead;
	}
}
