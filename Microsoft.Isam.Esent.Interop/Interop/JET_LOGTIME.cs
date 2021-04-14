using System;
using System.Globalization;

namespace Microsoft.Isam.Esent.Interop
{
	[Serializable]
	public struct JET_LOGTIME : IEquatable<JET_LOGTIME>, IJET_LOGTIME, INullableJetStruct
	{
		internal JET_LOGTIME(ulong ui64Time)
		{
			this.bSeconds = (byte)(ui64Time & 255UL);
			ui64Time >>= 8;
			this.bMinutes = (byte)(ui64Time & 255UL);
			ui64Time >>= 8;
			this.bHours = (byte)(ui64Time & 255UL);
			ui64Time >>= 8;
			this.bDays = (byte)(ui64Time & 255UL);
			ui64Time >>= 8;
			this.bMonth = (byte)(ui64Time & 255UL);
			ui64Time >>= 8;
			this.bYear = (byte)(ui64Time & 255UL);
			ui64Time >>= 8;
			this.bFiller1 = (byte)(ui64Time & 255UL);
			ui64Time >>= 8;
			this.bFiller2 = (byte)(ui64Time & 255UL);
		}

		private JET_LOGTIME(byte year, byte month, byte day, byte hours, byte minutes, byte seconds, byte filler1, byte filler2)
		{
			this.bSeconds = seconds;
			this.bMinutes = minutes;
			this.bHours = hours;
			this.bDays = day;
			this.bMonth = month;
			this.bYear = year;
			this.bFiller1 = filler1;
			this.bFiller2 = filler2;
		}

		internal static JET_LOGTIME CreateArbitraryJetLogtimeForTestingOnly(byte year, byte month, byte day, byte hours, byte minutes, byte seconds, byte filler1, byte filler2)
		{
			return new JET_LOGTIME(year, month, day, hours, minutes, seconds, filler1, filler2);
		}

		internal ulong ToUint64()
		{
			ulong num = (ulong)this.bFiller2;
			num = (num << 8) + (ulong)this.bFiller1;
			num = (num << 8) + (ulong)this.bYear;
			num = (num << 8) + (ulong)this.bMonth;
			num = (num << 8) + (ulong)this.bDays;
			num = (num << 8) + (ulong)this.bHours;
			num = (num << 8) + (ulong)this.bMinutes;
			return (num << 8) + (ulong)this.bSeconds;
		}

		internal JET_LOGTIME(DateTime time)
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
				this.bFiller2 = (byte)((time.Millisecond & 896) >> 6);
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

		public static bool operator ==(JET_LOGTIME lhs, JET_LOGTIME rhs)
		{
			return lhs.Equals(rhs);
		}

		public static bool operator !=(JET_LOGTIME lhs, JET_LOGTIME rhs)
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
			return string.Format(CultureInfo.InvariantCulture, "JET_LOGTIME({0}:{1}:{2}:{3}:{4}:{5}:0x{6:x}:0x{7:x})", new object[]
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
			return obj != null && !(base.GetType() != obj.GetType()) && this.Equals((JET_LOGTIME)obj);
		}

		public override int GetHashCode()
		{
			return this.bSeconds.GetHashCode() ^ (int)this.bMinutes << 6 ^ (int)this.bHours << 12 ^ (int)this.bDays << 17 ^ (int)this.bMonth << 22 ^ (int)this.bYear << 24 ^ (int)this.bFiller1 ^ (int)this.bFiller2 << 8;
		}

		public bool Equals(JET_LOGTIME other)
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
