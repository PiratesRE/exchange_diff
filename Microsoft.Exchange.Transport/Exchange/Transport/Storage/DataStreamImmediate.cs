using System;

namespace Microsoft.Exchange.Transport.Storage
{
	internal abstract class DataStreamImmediate : DataStream
	{
		internal DataStreamImmediate(DataColumn column, DataTableCursor cursor, DataRow row, int sequence) : base(column, row, sequence)
		{
			this.cursor = cursor;
		}

		public DataTableCursor Cursor
		{
			get
			{
				return this.cursor;
			}
		}

		protected override void Dispose(bool disposing)
		{
			this.cursor = null;
			base.Dispose(disposing);
		}

		protected DataTableCursor cursor;
	}
}
