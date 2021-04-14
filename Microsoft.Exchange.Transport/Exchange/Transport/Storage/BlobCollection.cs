using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Isam.Esent.Interop;

namespace Microsoft.Exchange.Transport.Storage
{
	internal class BlobCollection
	{
		public BlobCollection(DataColumn column, DataRow row)
		{
			if (column == null)
			{
				throw new ArgumentNullException("column");
			}
			if (row == null)
			{
				throw new ArgumentNullException("row");
			}
			if (!column.MultiValued)
			{
				throw new ArgumentException("Column is not MultiValued.", "column");
			}
			if (column.JetColType != JET_coltyp.LongBinary)
			{
				throw new ArgumentException("Column is not of type LongBinary.", "column");
			}
			this.column = column;
			this.row = row;
		}

		public Stream OpenWriter(byte blobKey, DataTableCursor cursor, bool update, bool cached = false, Func<bool> checkpointCallback = null)
		{
			Stream result;
			lock (this)
			{
				int sequence = this.GetSequence(blobKey, true, cursor);
				Stream stream = cached ? this.column.OpenCachingWriter(cursor, this.row, update, checkpointCallback, sequence) : this.column.OpenImmediateWriter(cursor, this.row, update, sequence);
				result = stream;
			}
			return result;
		}

		public Stream OpenReader(byte blobKey, DataTableCursor cursor, bool lazy = false)
		{
			Stream result;
			lock (this)
			{
				int sequence = this.GetSequence(blobKey, false, cursor);
				if (sequence > 0)
				{
					result = (lazy ? this.column.OpenLazyReader(cursor, this.row, sequence) : this.column.OpenImmediateReader(cursor, this.row, sequence));
				}
				else
				{
					result = Stream.Null;
				}
			}
			return result;
		}

		private int GetSequence(byte blobKey, bool create, DataTableCursor cursor)
		{
			this.LazyInitMap(cursor);
			int num = this.map.IndexOf(blobKey);
			if (num >= 0 || !create)
			{
				return num + 1;
			}
			this.map.Add(blobKey);
			this.SaveMap(cursor);
			return this.map.Count;
		}

		private void LazyInitMap(DataTableCursor cursor)
		{
			if (this.map == null)
			{
				this.map = new List<byte>();
				byte[] array = this.column.BytesFromCursor(cursor, true, 1);
				if (array == null || array.Length == 0)
				{
					this.map.Add(byte.MaxValue);
					return;
				}
				this.map.AddRange(array);
			}
		}

		private void SaveMap(DataTableCursor cursor)
		{
			this.column.SaveToCursor(cursor, this.map.ToArray(), 1, false, -1);
		}

		private const byte BlobCollectionMapKey = 255;

		private readonly DataColumn column;

		private readonly DataRow row;

		private List<byte> map;
	}
}
