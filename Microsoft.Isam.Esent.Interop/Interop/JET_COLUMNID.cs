using System;
using System.Diagnostics;
using System.Globalization;

namespace Microsoft.Isam.Esent.Interop
{
	public struct JET_COLUMNID : IEquatable<JET_COLUMNID>, IComparable<JET_COLUMNID>, IFormattable
	{
		public static JET_COLUMNID Nil
		{
			[DebuggerStepThrough]
			get
			{
				return default(JET_COLUMNID);
			}
		}

		public bool IsInvalid
		{
			get
			{
				return this.Value == 0U || this.Value == uint.MaxValue;
			}
		}

		public static bool operator ==(JET_COLUMNID lhs, JET_COLUMNID rhs)
		{
			return lhs.Value == rhs.Value;
		}

		public static bool operator !=(JET_COLUMNID lhs, JET_COLUMNID rhs)
		{
			return !(lhs == rhs);
		}

		public static bool operator <(JET_COLUMNID lhs, JET_COLUMNID rhs)
		{
			return lhs.CompareTo(rhs) < 0;
		}

		public static bool operator >(JET_COLUMNID lhs, JET_COLUMNID rhs)
		{
			return lhs.CompareTo(rhs) > 0;
		}

		public static bool operator <=(JET_COLUMNID lhs, JET_COLUMNID rhs)
		{
			return lhs.CompareTo(rhs) <= 0;
		}

		public static bool operator >=(JET_COLUMNID lhs, JET_COLUMNID rhs)
		{
			return lhs.CompareTo(rhs) >= 0;
		}

		public override string ToString()
		{
			return string.Format(CultureInfo.InvariantCulture, "JET_COLUMNID(0x{0:x})", new object[]
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
			return obj != null && !(base.GetType() != obj.GetType()) && this.Equals((JET_COLUMNID)obj);
		}

		public override int GetHashCode()
		{
			return this.Value.GetHashCode();
		}

		public bool Equals(JET_COLUMNID other)
		{
			return this.Value.Equals(other.Value);
		}

		public int CompareTo(JET_COLUMNID other)
		{
			return this.Value.CompareTo(other.Value);
		}

		internal static JET_COLUMNID CreateColumnidFromNativeValue(int nativeValue)
		{
			return new JET_COLUMNID
			{
				Value = (uint)nativeValue
			};
		}

		internal uint Value;
	}
}
