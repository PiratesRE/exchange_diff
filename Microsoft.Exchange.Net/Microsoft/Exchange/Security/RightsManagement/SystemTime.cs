using System;
using System.Runtime.InteropServices;
using System.Security;

namespace Microsoft.Exchange.Security.RightsManagement
{
	[SecurityCritical(SecurityCriticalScope.Everything)]
	[StructLayout(LayoutKind.Sequential)]
	internal class SystemTime
	{
		internal SystemTime(DateTime dateTime)
		{
			this.year = (ushort)dateTime.Year;
			this.month = (ushort)dateTime.Month;
			this.dayOfWeek = (ushort)dateTime.DayOfWeek;
			this.day = (ushort)dateTime.Day;
			this.hour = (ushort)dateTime.Hour;
			this.minute = (ushort)dateTime.Minute;
			this.second = (ushort)dateTime.Second;
			this.milliseconds = (ushort)dateTime.Millisecond;
		}

		internal static uint Size
		{
			get
			{
				return 16U;
			}
		}

		internal SystemTime(byte[] dataBuffer)
		{
			this.year = BitConverter.ToUInt16(dataBuffer, 0);
			this.month = BitConverter.ToUInt16(dataBuffer, 2);
			this.dayOfWeek = BitConverter.ToUInt16(dataBuffer, 4);
			this.day = BitConverter.ToUInt16(dataBuffer, 6);
			this.hour = BitConverter.ToUInt16(dataBuffer, 8);
			this.minute = BitConverter.ToUInt16(dataBuffer, 10);
			this.second = BitConverter.ToUInt16(dataBuffer, 12);
			this.milliseconds = BitConverter.ToUInt16(dataBuffer, 14);
		}

		internal DateTime GetDateTime(DateTime defaultValue)
		{
			if (this.year == 0 && this.month == 0 && this.day == 0 && this.hour == 0 && this.minute == 0 && this.second == 0 && this.milliseconds == 0)
			{
				return defaultValue;
			}
			return new DateTime((int)this.year, (int)this.month, (int)this.day, (int)this.hour, (int)this.minute, (int)this.second, (int)this.milliseconds);
		}

		private ushort year;

		private ushort month;

		private ushort dayOfWeek;

		private ushort day;

		private ushort hour;

		private ushort minute;

		private ushort second;

		private ushort milliseconds;
	}
}
