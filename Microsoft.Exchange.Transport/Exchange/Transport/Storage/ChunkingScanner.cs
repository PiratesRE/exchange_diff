using System;

namespace Microsoft.Exchange.Transport.Storage
{
	internal abstract class ChunkingScanner
	{
		public virtual void Scan(Transaction transaction, DataTableCursor cursor, bool forward)
		{
			if (ChunkingScanner.SeekStartRow(cursor, forward))
			{
				this.ScanFromCurrentPosition(transaction, cursor, forward);
			}
		}

		public void ScanFromCurrentPosition(Transaction transaction, DataTableCursor cursor, bool forward)
		{
			if (transaction == null)
			{
				throw new ArgumentNullException("transaction");
			}
			if (cursor == null)
			{
				throw new ArgumentNullException("cursor");
			}
			if (forward)
			{
				while (this.HandleRecord(cursor) == ChunkingScanner.ScanControl.Continue)
				{
					transaction.RestartIfStale(100);
					if (!cursor.TryMoveNext(false))
					{
						return;
					}
				}
				return;
			}
			while (this.HandleRecord(cursor) == ChunkingScanner.ScanControl.Continue)
			{
				transaction.RestartIfStale(100);
				if (!cursor.TryMovePrevious(false))
				{
					return;
				}
			}
		}

		protected abstract ChunkingScanner.ScanControl HandleRecord(DataTableCursor cursor);

		private static bool SeekStartRow(DataTableCursor cursor, bool forward)
		{
			if (forward)
			{
				cursor.PrereadForward();
				return cursor.TryMoveFirst();
			}
			return cursor.TryMoveLast();
		}

		protected enum ScanControl
		{
			Continue,
			Stop
		}
	}
}
