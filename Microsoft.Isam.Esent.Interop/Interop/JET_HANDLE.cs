using System;
using System.Diagnostics;
using System.Globalization;

namespace Microsoft.Isam.Esent.Interop
{
	public struct JET_HANDLE : IEquatable<JET_HANDLE>, IFormattable
	{
		public static JET_HANDLE Nil
		{
			[DebuggerStepThrough]
			get
			{
				return default(JET_HANDLE);
			}
		}

		public bool IsInvalid
		{
			get
			{
				return this.Value == IntPtr.Zero || this.Value == new IntPtr(-1);
			}
		}

		public static bool operator ==(JET_HANDLE lhs, JET_HANDLE rhs)
		{
			return lhs.Value == rhs.Value;
		}

		public static bool operator !=(JET_HANDLE lhs, JET_HANDLE rhs)
		{
			return !(lhs == rhs);
		}

		public override string ToString()
		{
			return string.Format(CultureInfo.InvariantCulture, "JET_HANDLE(0x{0:x})", new object[]
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
			return obj != null && !(base.GetType() != obj.GetType()) && this.Equals((JET_HANDLE)obj);
		}

		public override int GetHashCode()
		{
			return this.Value.GetHashCode();
		}

		public bool Equals(JET_HANDLE other)
		{
			return this.Value.Equals(other.Value);
		}

		internal IntPtr Value;
	}
}
