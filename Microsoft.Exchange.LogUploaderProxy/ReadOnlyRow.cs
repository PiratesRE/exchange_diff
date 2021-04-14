using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.LogUploaderProxy
{
	public class ReadOnlyRow
	{
		public ReadOnlyRow(CsvFieldCache cursor, long position)
		{
			this.schema = cursor.Schema;
			this.readOnlyRowImpl = new ReadOnlyRow(cursor.CsvFieldCacheImpl, position);
		}

		public long Position
		{
			get
			{
				return this.readOnlyRowImpl.Position;
			}
		}

		public long EndPosition
		{
			get
			{
				return this.readOnlyRowImpl.EndPosition;
			}
		}

		public CsvTable Schema
		{
			get
			{
				return this.schema;
			}
		}

		public T GetField<T>(int field)
		{
			return this.readOnlyRowImpl.GetField<T>(field);
		}

		public T GetField<T>(string fieldName)
		{
			return this.readOnlyRowImpl.GetField<T>(fieldName);
		}

		public byte[] GetFieldRaw(string fieldName)
		{
			return this.readOnlyRowImpl.GetFieldRaw(fieldName);
		}

		private ReadOnlyRow readOnlyRowImpl;

		private CsvTable schema;
	}
}
