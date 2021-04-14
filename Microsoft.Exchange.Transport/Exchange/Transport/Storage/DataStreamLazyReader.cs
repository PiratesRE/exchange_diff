using System;
using Microsoft.Isam.Esent.Interop;

namespace Microsoft.Exchange.Transport.Storage
{
	internal class DataStreamLazyReader : DataStream, ICloneable
	{
		internal DataStreamLazyReader(DataColumn column, DataTableCursor cursor, DataRow row, int sequence) : base(column, row, sequence)
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

		internal DataStreamLazyReader(DataStream rhs) : base(rhs)
		{
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

		object ICloneable.Clone()
		{
			if (this.row == null)
			{
				throw new ObjectDisposedException("DataStreamLazyReader");
			}
			return new DataStreamLazyReader(this);
		}

		public override int Read(byte[] buffer, int offset, int count)
		{
			if (this.Position == this.Length)
			{
				return 0;
			}
			int result;
			using (DataTableCursor cursor = this.row.Table.GetCursor())
			{
				using (cursor.BeginTransaction())
				{
					this.row.SeekCurrent(cursor);
					result = base.InternalRead(buffer, offset, count, cursor);
				}
			}
			return result;
		}

		protected override void Dispose(bool disposing)
		{
			base.Dispose(disposing);
		}
	}
}
