using System;

namespace Microsoft.Exchange.Diagnostics
{
	internal class ReadOnlyRow
	{
		public ReadOnlyRow(CsvFieldCache cursor, long position)
		{
			this.cursor = cursor;
			this.position = position;
		}

		public long Position
		{
			get
			{
				return this.position;
			}
		}

		public long EndPosition
		{
			get
			{
				return this.cursor.Position;
			}
		}

		public CsvTable Schema
		{
			get
			{
				return this.cursor.Schema;
			}
		}

		public T GetField<T>(int field)
		{
			return (T)((object)this.cursor.GetField(field));
		}

		public T GetField<T>(string fieldName)
		{
			int field = this.Schema.NameToIndex(fieldName);
			return this.GetField<T>(field);
		}

		public byte[] GetFieldRaw(string fieldName)
		{
			return this.cursor.GetFieldRaw(this.Schema.NameToIndex(fieldName));
		}

		private readonly long position;

		private CsvFieldCache cursor;
	}
}
