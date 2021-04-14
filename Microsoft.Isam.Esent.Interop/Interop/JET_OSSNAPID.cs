using System;
using System.Diagnostics;
using System.Globalization;

namespace Microsoft.Isam.Esent.Interop
{
	public struct JET_OSSNAPID : IEquatable<JET_OSSNAPID>, IFormattable
	{
		public static JET_OSSNAPID Nil
		{
			[DebuggerStepThrough]
			get
			{
				return default(JET_OSSNAPID);
			}
		}

		public bool IsInvalid
		{
			get
			{
				return this.Value == IntPtr.Zero || this.Value == new IntPtr(-1);
			}
		}

		public static bool operator ==(JET_OSSNAPID lhs, JET_OSSNAPID rhs)
		{
			return lhs.Value == rhs.Value;
		}

		public static bool operator !=(JET_OSSNAPID lhs, JET_OSSNAPID rhs)
		{
			return !(lhs == rhs);
		}

		public override string ToString()
		{
			return string.Format(CultureInfo.InvariantCulture, "JET_OSSNAPID(0x{0:x})", new object[]
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
			return obj != null && !(base.GetType() != obj.GetType()) && this.Equals((JET_OSSNAPID)obj);
		}

		public override int GetHashCode()
		{
			return this.Value.GetHashCode();
		}

		public bool Equals(JET_OSSNAPID other)
		{
			return this.Value.Equals(other.Value);
		}

		internal IntPtr Value;
	}
}
