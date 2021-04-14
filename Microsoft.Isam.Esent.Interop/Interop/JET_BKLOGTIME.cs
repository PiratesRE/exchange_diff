using System;
using System.Globalization;

namespace Microsoft.Isam.Esent.Interop
{
	[Serializable]
	public struct JET_BKLOGTIME : IEquatable<JET_BKLOGTIME>, IJET_LOGTIME, INullableJetStruct
	{
		internal JET_BKLOGTIME(DateTime time, bool isSnapshot)
		{
			checked
			{
				this.bSeconds = (byte)time.Second;
				this.bMinutes = (byte)time.Minute;
				this.bHours = (byte)time.Hour;
				this.bDays = (byte)time.Day;
				this.bMonth = (byte)time.Month;
				this.bYear = (byte)(time.Year - 1900);
				this.bFiller1 = ((time.Kind == DateTimeKind.Utc) ? 1 : 0);
				this.bFiller1 |= (byte)((time.Millisecond & 127) << 1);
				this.bFiller2 = (isSnapshot ? 1 : 0);
				this.bFiller2 |= (byte)((time.Millisecond & 896) >> 6);
			}
		}

		public bool HasValue
		{
			get
			{
				return this.bMonth != 0 && 0 != this.bDays;
			}
		}

		public bool fTimeIsUTC
		{
			get
			{
				return 0 != (this.bFiller1 & 1);
			}
		}

		public bool fOSSnapshot
		{
			get
			{
				return 0 != (this.bFiller2 & 1);
			}
		}

		public static bool operator ==(JET_BKLOGTIME lhs, JET_BKLOGTIME rhs)
		{
			return lhs.Equals(rhs);
		}

		public static bool operator !=(JET_BKLOGTIME lhs, JET_BKLOGTIME rhs)
		{
			return !(lhs == rhs);
		}

		public DateTime? ToDateTime()
		{
			if (!this.HasValue)
			{
				return null;
			}
			return new DateTime?(new DateTime((int)this.bYear + 1900, (int)this.bMonth, (int)this.bDays, (int)this.bHours, (int)this.bMinutes, (int)this.bSeconds, (int)(this.bFiller2 & 14) << 6 | (int)((uint)(this.bFiller1 & 254) >> 1), this.fTimeIsUTC ? DateTimeKind.Utc : DateTimeKind.Local));
		}

		public override string ToString()
		{
			return string.Format(CultureInfo.InvariantCulture, "JET_BKLOGTIME({0}:{1}:{2}:{3}:{4}:{5}:0x{6:x}:0x{7:x})", new object[]
			{
				this.bSeconds,
				this.bMinutes,
				this.bHours,
				this.bDays,
				this.bMonth,
				this.bYear,
				this.bFiller1,
				this.bFiller2
			});
		}

		public override bool Equals(object obj)
		{
			return obj != null && !(base.GetType() != obj.GetType()) && this.Equals((JET_BKLOGTIME)obj);
		}

		public override int GetHashCode()
		{
			return this.bSeconds.GetHashCode() ^ (int)this.bMinutes << 6 ^ (int)this.bHours << 12 ^ (int)this.bDays << 17 ^ (int)this.bMonth << 22 ^ (int)this.bYear << 24 ^ (int)this.bFiller1 ^ (int)this.bFiller2 << 8;
		}

		public bool Equals(JET_BKLOGTIME other)
		{
			return this.bSeconds == other.bSeconds && this.bMinutes == other.bMinutes && this.bHours == other.bHours && this.bDays == other.bDays && this.bMonth == other.bMonth && this.bYear == other.bYear && this.bFiller1 == other.bFiller1 && this.bFiller2 == other.bFiller2;
		}

		private readonly byte bSeconds;

		private readonly byte bMinutes;

		private readonly byte bHours;

		private readonly byte bDays;

		private readonly byte bMonth;

		private readonly byte bYear;

		private readonly byte bFiller1;

		private readonly byte bFiller2;
	}
}
