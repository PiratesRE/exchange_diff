using System;
using System.Globalization;

namespace Microsoft.Isam.Esent.Interop
{
	public sealed class JET_COLUMNBASE : IEquatable<JET_COLUMNBASE>
	{
		internal JET_COLUMNBASE()
		{
		}

		internal JET_COLUMNBASE(NATIVE_COLUMNBASE value)
		{
			this.coltyp = (JET_coltyp)value.coltyp;
			this.cp = (JET_CP)value.cp;
			this.cbMax = checked((int)value.cbMax);
			this.grbit = (ColumndefGrbit)value.grbit;
			this.columnid = new JET_COLUMNID
			{
				Value = value.columnid
			};
			this.szBaseTableName = StringCache.TryToIntern(value.szBaseTableName);
			this.szBaseColumnName = StringCache.TryToIntern(value.szBaseColumnName);
		}

		internal JET_COLUMNBASE(NATIVE_COLUMNBASE_WIDE value)
		{
			this.coltyp = (JET_coltyp)value.coltyp;
			this.cp = (JET_CP)value.cp;
			this.cbMax = checked((int)value.cbMax);
			this.grbit = (ColumndefGrbit)value.grbit;
			this.columnid = new JET_COLUMNID
			{
				Value = value.columnid
			};
			this.szBaseTableName = StringCache.TryToIntern(value.szBaseTableName);
			this.szBaseColumnName = StringCache.TryToIntern(value.szBaseColumnName);
		}

		public JET_coltyp coltyp { get; internal set; }

		public JET_CP cp { get; internal set; }

		public int cbMax { get; internal set; }

		public ColumndefGrbit grbit { get; internal set; }

		public JET_COLUMNID columnid { get; internal set; }

		public string szBaseTableName { get; internal set; }

		public string szBaseColumnName { get; internal set; }

		public override string ToString()
		{
			return string.Format(CultureInfo.InvariantCulture, "JET_COLUMNBASE({0},{1})", new object[]
			{
				this.coltyp,
				this.grbit
			});
		}

		public override int GetHashCode()
		{
			int[] hashes = new int[]
			{
				this.coltyp.GetHashCode(),
				this.cp.GetHashCode(),
				this.cbMax,
				this.grbit.GetHashCode(),
				this.columnid.GetHashCode(),
				this.szBaseTableName.GetHashCode(),
				this.szBaseColumnName.GetHashCode()
			};
			return Util.CalculateHashCode(hashes);
		}

		public override bool Equals(object obj)
		{
			return obj != null && !(base.GetType() != obj.GetType()) && this.Equals((JET_COLUMNBASE)obj);
		}

		public bool Equals(JET_COLUMNBASE other)
		{
			return other != null && (this.coltyp == other.coltyp && this.cp == other.cp && this.cbMax == other.cbMax && this.columnid == other.columnid && this.grbit == other.grbit && string.Equals(this.szBaseTableName, other.szBaseTableName, StringComparison.Ordinal)) && string.Equals(this.szBaseColumnName, other.szBaseColumnName, StringComparison.Ordinal);
		}
	}
}
