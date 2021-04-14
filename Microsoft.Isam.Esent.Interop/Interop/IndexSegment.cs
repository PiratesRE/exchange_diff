using System;
using System.Diagnostics;
using System.Globalization;

namespace Microsoft.Isam.Esent.Interop
{
	[Serializable]
	public class IndexSegment : IEquatable<IndexSegment>
	{
		internal IndexSegment(string name, JET_coltyp coltyp, bool isAscending, bool isASCII)
		{
			this.columnName = name;
			this.coltyp = coltyp;
			this.isAscending = isAscending;
			this.isASCII = isASCII;
		}

		public string ColumnName
		{
			[DebuggerStepThrough]
			get
			{
				return this.columnName;
			}
		}

		public JET_coltyp Coltyp
		{
			[DebuggerStepThrough]
			get
			{
				return this.coltyp;
			}
		}

		public bool IsAscending
		{
			[DebuggerStepThrough]
			get
			{
				return this.isAscending;
			}
		}

		public bool IsASCII
		{
			[DebuggerStepThrough]
			get
			{
				return this.isASCII;
			}
		}

		public override bool Equals(object obj)
		{
			return obj != null && !(base.GetType() != obj.GetType()) && this.Equals((IndexSegment)obj);
		}

		public override string ToString()
		{
			return string.Format(CultureInfo.InvariantCulture, "{0}{1}({2})", new object[]
			{
				this.isAscending ? "+" : "-",
				this.columnName,
				this.coltyp
			});
		}

		public override int GetHashCode()
		{
			return this.columnName.GetHashCode() ^ (int)(this.coltyp * (JET_coltyp)31) ^ (this.isAscending ? 65536 : 131072) ^ (this.isASCII ? 262144 : 524288);
		}

		public bool Equals(IndexSegment other)
		{
			return other != null && (this.columnName.Equals(other.columnName, StringComparison.OrdinalIgnoreCase) && this.coltyp == other.coltyp && this.isAscending == other.isAscending) && this.isASCII == other.isASCII;
		}

		private readonly string columnName;

		private readonly JET_coltyp coltyp;

		private readonly bool isAscending;

		private readonly bool isASCII;
	}
}
