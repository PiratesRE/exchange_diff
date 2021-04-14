using System;
using System.Diagnostics;
using System.Globalization;

namespace Microsoft.Isam.Esent.Interop
{
	[Serializable]
	public struct JET_BKINFO : IEquatable<JET_BKINFO>, INullableJetStruct
	{
		public JET_LGPOS lgposMark
		{
			[DebuggerStepThrough]
			get
			{
				return this.logPosition;
			}
			internal set
			{
				this.logPosition = value;
			}
		}

		public JET_BKLOGTIME bklogtimeMark
		{
			[DebuggerStepThrough]
			get
			{
				return this.backupTime;
			}
			internal set
			{
				this.backupTime = value;
			}
		}

		public int genLow
		{
			[DebuggerStepThrough]
			get
			{
				return (int)this.lowGeneration;
			}
			internal set
			{
				this.lowGeneration = checked((uint)value);
			}
		}

		public int genHigh
		{
			[DebuggerStepThrough]
			get
			{
				return (int)this.highGeneration;
			}
			set
			{
				this.highGeneration = checked((uint)value);
			}
		}

		public bool HasValue
		{
			get
			{
				return this.lgposMark.HasValue && this.backupTime.HasValue && this.lowGeneration != 0U && 0U != this.highGeneration;
			}
		}

		public static bool operator ==(JET_BKINFO lhs, JET_BKINFO rhs)
		{
			return lhs.Equals(rhs);
		}

		public static bool operator !=(JET_BKINFO lhs, JET_BKINFO rhs)
		{
			return !(lhs == rhs);
		}

		public override string ToString()
		{
			return string.Format(CultureInfo.InvariantCulture, "JET_BKINFO({0}-{1}:{2}:{3})", new object[]
			{
				this.genLow,
				this.genHigh,
				this.lgposMark,
				this.bklogtimeMark
			});
		}

		public override bool Equals(object obj)
		{
			return obj != null && !(base.GetType() != obj.GetType()) && this.Equals((JET_BKINFO)obj);
		}

		public override int GetHashCode()
		{
			return this.logPosition.GetHashCode() ^ this.backupTime.GetHashCode() ^ (int)((int)this.lowGeneration << 16) ^ (int)this.lowGeneration >> 16 ^ (int)this.highGeneration;
		}

		public bool Equals(JET_BKINFO other)
		{
			return this.logPosition == other.logPosition && this.backupTime == other.backupTime && this.lowGeneration == other.lowGeneration && this.highGeneration == other.highGeneration;
		}

		private JET_LGPOS logPosition;

		private JET_BKLOGTIME backupTime;

		private uint lowGeneration;

		private uint highGeneration;
	}
}
