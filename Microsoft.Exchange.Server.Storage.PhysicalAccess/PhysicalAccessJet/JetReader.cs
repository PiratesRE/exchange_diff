using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.ManagedStore.PhysicalAccess;
using Microsoft.Exchange.Server.Storage.Common.ExtensionMethods;
using Microsoft.Exchange.Server.Storage.PhysicalAccess;

namespace Microsoft.Exchange.Server.Storage.PhysicalAccessJet
{
	internal sealed class JetReader : Reader
	{
		internal JetReader(IConnectionProvider connectionProvider, SimpleQueryOperator simpleQueryOperator, bool disposeQueryOperator) : base(connectionProvider, simpleQueryOperator, disposeQueryOperator)
		{
			this.readerOpen = true;
			this.operationData = base.Connection.RecordOperationImpl(simpleQueryOperator);
			this.savedOperationData = base.Connection.SetCurrentOperationStatisticsObject(this.operationData);
		}

		private IJetSimpleQueryOperator JetQueryOperator
		{
			get
			{
				return (IJetSimpleQueryOperator)base.SimpleQueryOperator;
			}
		}

		public override bool IsClosed
		{
			get
			{
				return !this.readerOpen;
			}
		}

		public override bool Interrupted
		{
			get
			{
				return this.JetQueryOperator.Interrupted;
			}
		}

		public override bool EnableInterrupts(IInterruptControl interruptControl)
		{
			bool flag = base.SimpleQueryOperator.EnableInterrupts(interruptControl);
			this.interruptible = (interruptControl != null && flag);
			return flag;
		}

		public override bool Read(out int rowsSkipped)
		{
			return this.Read(true, out rowsSkipped);
		}

		internal bool Read(bool traceRead, out int rowsSkipped)
		{
			rowsSkipped = 0;
			if (this.interruptible && this.Interrupted)
			{
				this.operationData = base.Connection.RecordOperationImpl(base.SimpleQueryOperator);
				this.savedOperationData = base.Connection.SetCurrentOperationStatisticsObject(this.operationData);
			}
			bool flag = false;
			if (this.readerOpen)
			{
				using (this.TrackReaderSubOperation())
				{
					using (base.Connection.TrackTimeInDatabase())
					{
						if (!this.fetchFirst)
						{
							flag = this.JetQueryOperator.MoveFirst(out rowsSkipped);
							this.fetchFirst = true;
						}
						else
						{
							flag = this.JetQueryOperator.MoveNext();
						}
						if (flag && this.interruptible && this.Interrupted)
						{
							this.operationData = null;
							this.savedOperationData = null;
						}
					}
				}
				if (flag)
				{
					if (traceRead && (!this.interruptible || !this.Interrupted))
					{
						this.TraceReadRecord();
					}
				}
				else
				{
					this.Close();
				}
			}
			return flag;
		}

		public override bool? GetNullableBoolean(Column column)
		{
			bool? result;
			using (this.TrackReaderSubOperation())
			{
				result = (bool?)this.JetQueryOperator.GetColumnValue(column);
			}
			return result;
		}

		public override bool GetBoolean(Column column)
		{
			bool result;
			using (this.TrackReaderSubOperation())
			{
				result = (bool)this.JetQueryOperator.GetColumnValue(column);
			}
			return result;
		}

		public override long? GetNullableInt64(Column column)
		{
			long? result;
			using (this.TrackReaderSubOperation())
			{
				result = (long?)this.JetQueryOperator.GetColumnValue(column);
			}
			return result;
		}

		public override long GetInt64(Column column)
		{
			long result;
			using (this.TrackReaderSubOperation())
			{
				result = (long)this.JetQueryOperator.GetColumnValue(column);
			}
			return result;
		}

		public override int? GetNullableInt32(Column column)
		{
			int? result;
			using (this.TrackReaderSubOperation())
			{
				result = (int?)this.JetQueryOperator.GetColumnValue(column);
			}
			return result;
		}

		public override int GetInt32(Column column)
		{
			int result;
			using (this.TrackReaderSubOperation())
			{
				result = (int)this.JetQueryOperator.GetColumnValue(column);
			}
			return result;
		}

		public override short? GetNullableInt16(Column column)
		{
			short? result;
			using (this.TrackReaderSubOperation())
			{
				result = (short?)this.JetQueryOperator.GetColumnValue(column);
			}
			return result;
		}

		public override short GetInt16(Column column)
		{
			short result;
			using (this.TrackReaderSubOperation())
			{
				result = (short)this.JetQueryOperator.GetColumnValue(column);
			}
			return result;
		}

		public override Guid? GetNullableGuid(Column column)
		{
			Guid? result;
			using (this.TrackReaderSubOperation())
			{
				result = (Guid?)this.JetQueryOperator.GetColumnValue(column);
			}
			return result;
		}

		public override Guid GetGuid(Column column)
		{
			Guid result;
			using (this.TrackReaderSubOperation())
			{
				result = (Guid)this.JetQueryOperator.GetColumnValue(column);
			}
			return result;
		}

		public override DateTime GetDateTime(Column column)
		{
			DateTime result;
			using (this.TrackReaderSubOperation())
			{
				object columnValue = this.JetQueryOperator.GetColumnValue(column);
				DateTime dateTime = (DateTime)columnValue;
				result = dateTime;
			}
			return result;
		}

		public override DateTime? GetNullableDateTime(Column column)
		{
			DateTime? result;
			using (this.TrackReaderSubOperation())
			{
				object columnValue = this.JetQueryOperator.GetColumnValue(column);
				if (columnValue != null)
				{
					DateTime value = (DateTime)columnValue;
					result = new DateTime?(value);
				}
				else
				{
					result = null;
				}
			}
			return result;
		}

		public override byte[] GetBinary(Column column)
		{
			byte[] result;
			using (this.TrackReaderSubOperation())
			{
				result = (byte[])this.JetQueryOperator.GetColumnValue(column);
			}
			return result;
		}

		public override string GetString(Column column)
		{
			string result;
			using (this.TrackReaderSubOperation())
			{
				result = (string)this.JetQueryOperator.GetColumnValue(column);
			}
			return result;
		}

		public override object GetValue(Column column)
		{
			object columnValue;
			using (this.TrackReaderSubOperation())
			{
				columnValue = this.JetQueryOperator.GetColumnValue(column);
			}
			return columnValue;
		}

		public override object[] GetValues(IEnumerable<Column> columns)
		{
			object[] values;
			using (this.TrackReaderSubOperation())
			{
				using (base.Connection.TrackTimeInDatabase())
				{
					IGetColumnValues getColumnValues = this.JetQueryOperator as IGetColumnValues;
					if (getColumnValues != null)
					{
						using (this.TrackReaderSubOperation())
						{
							return getColumnValues.GetColumnValues(columns);
						}
					}
					values = base.GetValues(columns);
				}
			}
			return values;
		}

		public override long GetChars(Column column, long dataIndex, char[] outBuffer, int bufferIndex, int length)
		{
			long result;
			using (this.TrackReaderSubOperation())
			{
				int num = 0;
				object columnValue = this.JetQueryOperator.GetColumnValue(column);
				if (columnValue != null)
				{
					string text = (string)columnValue;
					char[] array = text.ToCharArray((int)dataIndex, length);
					num = array.Length;
					Array.Copy(array, 0, outBuffer, bufferIndex, num);
				}
				result = (long)num;
			}
			return result;
		}

		public override long GetBytes(Column column, long dataIndex, byte[] outBuffer, int bufferIndex, int length)
		{
			long result;
			using (this.TrackReaderSubOperation())
			{
				object columnValue = this.JetQueryOperator.GetColumnValue(column);
				if (columnValue != null)
				{
					byte[] array = (byte[])columnValue;
					if ((long)array.Length - dataIndex > 0L)
					{
						int num = Math.Min(length, array.Length - (int)dataIndex);
						Array.Copy(array, (int)dataIndex, outBuffer, bufferIndex, num);
						return (long)num;
					}
				}
				result = 0L;
			}
			return result;
		}

		public override void Close()
		{
			Connection connection = base.Connection;
			if (connection != null)
			{
				base.Connection.SetCurrentOperationStatisticsObject(this.savedOperationData);
			}
			this.readerOpen = false;
			base.Close();
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<JetReader>(this);
		}

		protected override void InternalDispose(bool calledFromDispose)
		{
			if (calledFromDispose)
			{
				if (this.readerOpen)
				{
					this.Close();
				}
				if (base.DisposeQueryOperator && base.SimpleQueryOperator != null)
				{
					base.SimpleQueryOperator.Dispose();
				}
			}
		}

		private void TraceReadRecord()
		{
			if (ExTraceGlobals.DbInteractionIntermediateTracer.IsTraceEnabled(TraceType.DebugTrace))
			{
				this.DoTraceReadRecord();
			}
		}

		private void DoTraceReadRecord()
		{
			using (this.TrackReaderSubOperation())
			{
				StringBuilder stringBuilder = new StringBuilder(100);
				stringBuilder.Append(" <<< Read row  cols:[");
				for (int i = 0; i < base.SimpleQueryOperator.ColumnsToFetch.Count; i++)
				{
					if (i != 0)
					{
						stringBuilder.Append(", ");
					}
					base.SimpleQueryOperator.ColumnsToFetch[i].AppendToString(stringBuilder, StringFormatOptions.None);
					stringBuilder.Append("=[");
					Column column = base.SimpleQueryOperator.ColumnsToFetch[i];
					object columnValue = this.JetQueryOperator.GetColumnValue(column);
					if (ExTraceGlobals.DbInteractionDetailTracer.IsTraceEnabled(TraceType.DebugTrace) || !(columnValue is byte[]) || ((byte[])columnValue).Length <= 32)
					{
						stringBuilder.AppendAsString(columnValue);
					}
					else
					{
						stringBuilder.Append("<long_blob>");
					}
					stringBuilder.Append("]");
				}
				stringBuilder.Append("]");
				ExTraceGlobals.DbInteractionIntermediateTracer.TraceDebug(0L, stringBuilder.ToString());
			}
		}

		private JetReader.JetReaderSubOperationTrackingFrame TrackReaderSubOperation()
		{
			return new JetReader.JetReaderSubOperationTrackingFrame(this);
		}

		private bool fetchFirst;

		private bool readerOpen;

		private DatabaseOperationStatistics operationData;

		private DatabaseOperationStatistics savedOperationData;

		private bool interruptible;

		private struct JetReaderSubOperationTrackingFrame : IDisposable
		{
			internal JetReaderSubOperationTrackingFrame(JetReader reader)
			{
				this.connection = reader.Connection;
				this.savedOperationData = this.connection.SetCurrentOperationStatisticsObject(reader.operationData);
			}

			public void Dispose()
			{
				this.connection.SetCurrentOperationStatisticsObject(this.savedOperationData);
			}

			private readonly Connection connection;

			private DatabaseOperationStatistics savedOperationData;
		}
	}
}
