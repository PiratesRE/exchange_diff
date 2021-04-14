using System;
using System.Globalization;

namespace Microsoft.Isam.Esent.Interop
{
	public struct JET_LS : IEquatable<JET_LS>, IFormattable
	{
		public bool IsInvalid
		{
			get
			{
				return this.Value == IntPtr.Zero || this.Value == new IntPtr(-1);
			}
		}

		public IntPtr Value { get; set; }

		public static bool operator ==(JET_LS lhs, JET_LS rhs)
		{
			return lhs.Value == rhs.Value;
		}

		public static bool operator !=(JET_LS lhs, JET_LS rhs)
		{
			return !(lhs == rhs);
		}

		public override string ToString()
		{
			return string.Format(CultureInfo.InvariantCulture, "JET_LS(0x{0:x})", new object[]
			{
				this.Value.ToInt64()
			});
		}

		public string ToString(string format, IFormatProvider formatProvider)
		{
			if (!string.IsNullOrEmpty(format) && !("G" == format))
			{
				return this.Value.ToInt64().ToString(format, formatProvider);
			}
			return this.ToString();
		}

		public override bool Equals(object obj)
		{
			return obj != null && !(base.GetType() != obj.GetType()) && this.Equals((JET_LS)obj);
		}

		public override int GetHashCode()
		{
			return this.Value.GetHashCode();
		}

		public bool Equals(JET_LS other)
		{
			return this.Value.Equals(other.Value);
		}

		public static readonly JET_LS Nil = new JET_LS
		{
			Value = new IntPtr(-1)
		};
	}
}
