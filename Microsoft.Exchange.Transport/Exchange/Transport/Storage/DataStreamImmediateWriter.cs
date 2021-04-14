using System;
using Microsoft.Exchange.Diagnostics.Components.Transport;
using Microsoft.Isam.Esent.Interop;

namespace Microsoft.Exchange.Transport.Storage
{
	internal class DataStreamImmediateWriter : DataStreamImmediate
	{
		internal DataStreamImmediateWriter(DataColumn column, DataTableCursor cursor, DataRow dataRow, bool update, Func<bool> checkpointCallback, int sequence) : base(column, cursor, dataRow, sequence)
		{
			this.checkpointCallback = checkpointCallback;
			if (update)
			{
				try
				{
					this.length = (long)(Api.RetrieveColumnSize(cursor.Session, cursor.TableId, column.ColumnId, base.Sequence, RetrieveColumnGrbit.None) ?? 0);
					return;
				}
				catch (EsentErrorException ex)
				{
					this.inError = true;
					if (!DataSource.HandleIsamException(ex, cursor.Connection.Source))
					{
						throw;
					}
					return;
				}
			}
			this.length = -1L;
			this.SetColumnLength(0L);
		}

		public override bool CanWrite
		{
			get
			{
				return true;
			}
		}

		public override bool CanSeek
		{
			get
			{
				return true;
			}
		}

		public override bool CanRead
		{
			get
			{
				return false;
			}
		}

		public override bool CanTimeout
		{
			get
			{
				return false;
			}
		}

		public override int Read(byte[] buffer, int offset, int count)
		{
			throw new NotSupportedException();
		}

		public override void Flush()
		{
		}

		public override void SetLength(long value)
		{
			this.SetColumnLength(value);
		}

		public override void Write(byte[] buffer, int offset, int count)
		{
			if (count == 0)
			{
				return;
			}
			if (offset == 0 && buffer.Length == count)
			{
				this.Write(buffer);
				return;
			}
			byte[] array = new byte[count];
			for (int i = 0; i < count; i++)
			{
				array[i] = buffer[offset + i];
			}
			this.Write(array);
		}

		private void Write(byte[] buffer)
		{
			this.Write(this.Position, buffer);
			this.position += (long)buffer.Length;
		}

		private void Write(long position, byte[] data)
		{
			this.ThrowIfDataRowNotInUpdate();
			if (this.inError)
			{
				ExTraceGlobals.ExpoTracer.TraceError(0L, "Attempt to write to the database stream after it encountered an exception was skipped.");
				return;
			}
			JET_SETINFO jet_SETINFO = new JET_SETINFO();
			jet_SETINFO.ibLongValue = (int)position;
			jet_SETINFO.itagSequence = base.Sequence;
			SetColumnGrbit setColumnGrbit = (position >= this.length) ? SetColumnGrbit.AppendLV : SetColumnGrbit.None;
			if (this.Column.IntrinsicLV)
			{
				setColumnGrbit |= SetColumnGrbit.IntrinsicLV;
			}
			try
			{
				if (Api.JetSetColumn(this.cursor.Session, this.cursor.TableId, this.Column.ColumnId, data, data.Length, setColumnGrbit, jet_SETINFO) != JET_wrn.Success)
				{
					this.inError = true;
					throw new InvalidOperationException(Strings.StreamStateInvalid);
				}
			}
			catch (EsentErrorException ex)
			{
				this.inError = true;
				if (!DataSource.HandleIsamException(ex, this.cursor.Connection.Source))
				{
					throw;
				}
			}
			this.length = Math.Max(this.length, position + (long)data.Length);
			base.DataRow.PerfCounters.StreamWrites.Increment();
			if (data != null)
			{
				base.DataRow.PerfCounters.StreamBytesWritten.IncrementBy((long)data.Length);
				this.bytesWritten += (long)data.Length;
			}
			if (this.checkpointCallback != null && base.DataRow.Updating && this.bytesWritten > 131072L)
			{
				this.checkpointCallback();
				this.bytesWritten = 0L;
			}
		}

		private void ThrowIfDataRowNotInUpdate()
		{
			if (!this.row.Updating)
			{
				throw new InvalidOperationException();
			}
		}

		private void SetColumnLength(long value)
		{
			this.ThrowIfDataRowNotInUpdate();
			if (this.length == value)
			{
				return;
			}
			if (this.inError)
			{
				ExTraceGlobals.ExpoTracer.TraceError(0L, "Attempt to modify the length of the database stream after it encountered an exception was skipped.");
				return;
			}
			try
			{
				JET_SETINFO setinfo = new JET_SETINFO
				{
					itagSequence = base.Sequence
				};
				if (value == 0L)
				{
					Api.JetSetColumn(this.cursor.Session, base.Cursor.TableId, this.Column.ColumnId, null, 0, SetColumnGrbit.ZeroLength, setinfo);
				}
				else
				{
					byte[] bytes = BitConverter.GetBytes(value);
					Api.JetSetColumn(this.cursor.Session, base.Cursor.TableId, this.Column.ColumnId, bytes, bytes.Length, SetColumnGrbit.SizeLV, setinfo);
				}
			}
			catch (EsentErrorException ex)
			{
				this.inError = true;
				if (!DataSource.HandleIsamException(ex, this.cursor.Connection.Source))
				{
					throw;
				}
			}
			this.length = value;
			base.DataRow.PerfCounters.StreamSetLen.Increment();
		}

		protected override void Dispose(bool disposing)
		{
			base.Dispose(disposing);
		}

		private const int CheckPointChunkSize = 131072;

		private Func<bool> checkpointCallback;

		private long bytesWritten;

		private bool inError;
	}
}
