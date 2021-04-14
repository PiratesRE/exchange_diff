using System;
using Microsoft.Isam.Esent.Interop;

namespace Microsoft.Exchange.Transport.Storage
{
	internal class DataStreamImmediateReader : DataStreamImmediate
	{
		internal DataStreamImmediateReader(DataColumn column, DataTableCursor cursor, DataRow row, int sequence) : base(column, cursor, row, sequence)
		{
			try
			{
				this.length = (long)(Api.RetrieveColumnSize(cursor.Session, cursor.TableId, column.ColumnId, base.Sequence, RetrieveColumnGrbit.None) ?? 0);
			}
			catch (EsentErrorException ex)
			{
				if (!DataSource.HandleIsamException(ex, cursor.Connection.Source))
				{
					throw;
				}
			}
		}

		protected DataStreamImmediateReader(DataStreamImmediateWriter rhs) : base(rhs.Column, rhs.Cursor, rhs.DataRow, rhs.Sequence)
		{
			this.length = rhs.Length;
		}

		protected DataStreamImmediateReader(DataStreamImmediateReader rhs) : base(rhs.Column, rhs.Cursor, rhs.DataRow, rhs.Sequence)
		{
			this.length = rhs.Length;
		}

		public override bool CanRead
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

		public override bool CanWrite
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

		public override void Write(byte[] buffer, int offset, int count)
		{
			throw new NotSupportedException();
		}

		public override void Flush()
		{
			throw new NotSupportedException();
		}

		public override void SetLength(long value)
		{
			throw new NotSupportedException();
		}

		public override int Read(byte[] buffer, int offset, int count)
		{
			return base.InternalRead(buffer, offset, count, this.cursor);
		}

		protected override void Dispose(bool disposing)
		{
			base.Dispose(disposing);
		}
	}
}
