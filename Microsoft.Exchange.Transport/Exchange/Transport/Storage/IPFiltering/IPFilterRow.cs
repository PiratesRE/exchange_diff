using System;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.Transport.Storage.IPFiltering
{
	internal class IPFilterRow : DataRow
	{
		public IPFilterRow() : base(Database.Table)
		{
		}

		public IPFilterRow(int identity) : base(Database.Table)
		{
			this.Identity = identity;
		}

		public int Identity
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

		public IPRange.Format Format
		{
			get
			{
				return (IPRange.Format)(this.TypeFlags & 15);
			}
			set
			{
				this.TypeFlags = (this.TypeFlags & -16 & (int)(value & (IPRange.Format)15));
			}
		}

		public PolicyType Policy
		{
			get
			{
				return (PolicyType)((this.TypeFlags & 240) >> 4);
			}
			set
			{
				this.TypeFlags = (this.TypeFlags & -241 & (int)((int)(value & (PolicyType)15) << 4));
			}
		}

		public IPvxAddress LowerBound
		{
			get
			{
				return ((ColumnCache<IPvxAddress>)base.Columns[2]).Value;
			}
			set
			{
				((ColumnCache<IPvxAddress>)base.Columns[2]).Value = value;
			}
		}

		public IPvxAddress UpperBound
		{
			get
			{
				return ((ColumnCache<IPvxAddress>)base.Columns[3]).Value;
			}
			set
			{
				((ColumnCache<IPvxAddress>)base.Columns[3]).Value = value;
			}
		}

		public DateTime ExpiresOn
		{
			get
			{
				return ((ColumnCache<DateTime>)base.Columns[4]).Value;
			}
			set
			{
				((ColumnCache<DateTime>)base.Columns[4]).Value = value;
			}
		}

		public string Comment
		{
			get
			{
				return ((ColumnCache<string>)base.Columns[5]).Value;
			}
			set
			{
				((ColumnCache<string>)base.Columns[5]).Value = value;
			}
		}

		internal int TypeFlags
		{
			get
			{
				return ((ColumnCache<int>)base.Columns[1]).Value;
			}
			set
			{
				((ColumnCache<int>)base.Columns[1]).Value = value;
			}
		}

		public static IPFilterRow LoadFromRow(DataTableCursor cursor)
		{
			IPFilterRow ipfilterRow = new IPFilterRow();
			ipfilterRow.LoadFromCurrentRow(cursor);
			return ipfilterRow;
		}

		public void Commit(Transaction transaction, DataTableCursor cursor)
		{
			base.MaterializeToCursor(transaction, cursor);
		}

		public new void Commit()
		{
			base.Commit();
		}
	}
}
