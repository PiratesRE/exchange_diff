using System;
using System.Globalization;

namespace Microsoft.Isam.Esent.Interop
{
	public struct JET_DBID : IEquatable<JET_DBID>, IFormattable
	{
		public static JET_DBID Nil
		{
			get
			{
				return new JET_DBID
				{
					Value = uint.MaxValue
				};
			}
		}

		public static bool operator ==(JET_DBID lhs, JET_DBID rhs)
		{
			return lhs.Value == rhs.Value;
		}

		public static bool operator !=(JET_DBID lhs, JET_DBID rhs)
		{
			return !(lhs == rhs);
		}

		public override string ToString()
		{
			return string.Format(CultureInfo.InvariantCulture, "JET_DBID({0})", new object[]
			{
				this.Value
			});
		}

		public string ToString(string format, IFormatProvider formatProvider)
		{
			if (!string.IsNullOrEmpty(format) && !("G" == format))
			{
				return this.Value.ToString(format, formatProvider);
			}
			return this.ToString();
		}

		public override bool Equals(object obj)
		{
			return obj != null && !(base.GetType() != obj.GetType()) && this.Equals((JET_DBID)obj);
		}

		public override int GetHashCode()
		{
			return this.Value.GetHashCode();
		}

		public bool Equals(JET_DBID other)
		{
			return this.Value.Equals(other.Value);
		}

		internal uint Value;
	}
}
