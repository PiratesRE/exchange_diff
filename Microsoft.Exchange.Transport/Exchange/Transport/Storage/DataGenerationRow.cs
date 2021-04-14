using System;

namespace Microsoft.Exchange.Transport.Storage
{
	internal class DataGenerationRow : DataRow
	{
		public DataGenerationRow(DataTable table) : base(table)
		{
		}

		public int GenerationId
		{
			get
			{
				return ((ColumnCache<int>)base.Columns[0]).Value;
			}
			set
			{
				((ColumnCache<int>)base.Columns[0]).Value = value;
			}
		}

		public DateTime StartTime
		{
			get
			{
				return ((ColumnCache<DateTime>)base.Columns[1]).Value;
			}
			set
			{
				((ColumnCache<DateTime>)base.Columns[1]).Value = value;
			}
		}

		public DateTime EndTime
		{
			get
			{
				return ((ColumnCache<DateTime>)base.Columns[2]).Value;
			}
			set
			{
				((ColumnCache<DateTime>)base.Columns[2]).Value = value;
			}
		}

		public int Category
		{
			get
			{
				return ((ColumnCache<int>)base.Columns[3]).Value;
			}
			set
			{
				((ColumnCache<int>)base.Columns[3]).Value = value;
			}
		}

		public string Name
		{
			get
			{
				return ((ColumnCache<string>)base.Columns[4]).Value;
			}
			set
			{
				((ColumnCache<string>)base.Columns[4]).Value = value;
			}
		}

		public static DataGenerationRow LoadFromRow(DataTableCursor cursor)
		{
			DataGenerationRow dataGenerationRow = new DataGenerationRow(cursor.Table);
			dataGenerationRow.LoadFromCurrentRow(cursor);
			return dataGenerationRow;
		}

		public void Commit(Transaction transaction)
		{
			base.Materialize(transaction);
		}

		public new void Commit()
		{
			base.Commit();
		}
	}
}
