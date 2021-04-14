using System;
using System.Diagnostics;
using System.Globalization;

namespace Microsoft.Isam.Esent.Interop
{
	[Serializable]
	public class JET_SNPROG : IEquatable<JET_SNPROG>
	{
		public int cunitDone
		{
			[DebuggerStepThrough]
			get
			{
				return this.completedUnits;
			}
			internal set
			{
				this.completedUnits = value;
			}
		}

		public int cunitTotal
		{
			[DebuggerStepThrough]
			get
			{
				return this.totalUnits;
			}
			internal set
			{
				this.totalUnits = value;
			}
		}

		public override bool Equals(object obj)
		{
			return obj != null && !(base.GetType() != obj.GetType()) && this.Equals((JET_SNPROG)obj);
		}

		public override string ToString()
		{
			return string.Format(CultureInfo.InvariantCulture, "JET_SNPROG({0}/{1})", new object[]
			{
				this.cunitDone,
				this.cunitTotal
			});
		}

		public override int GetHashCode()
		{
			return this.cunitDone * 31 ^ this.cunitTotal;
		}

		public bool Equals(JET_SNPROG other)
		{
			return other != null && this.cunitDone == other.cunitDone && this.cunitTotal == other.cunitTotal;
		}

		internal void SetFromNative(NATIVE_SNPROG native)
		{
			checked
			{
				this.cunitDone = (int)native.cunitDone;
				this.cunitTotal = (int)native.cunitTotal;
			}
		}

		private int completedUnits;

		private int totalUnits;
	}
}
