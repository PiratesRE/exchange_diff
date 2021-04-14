using System;
using System.Xml.Serialization;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Data.Storage;
using Microsoft.Exchange.Diagnostics.Components.InfoWorker.Common;
using Microsoft.Exchange.Win32;

namespace Microsoft.Exchange.Data.Storage
{
	[Serializable]
	public class ChangeDate
	{
		[XmlElement]
		public string Time
		{
			get
			{
				return string.Concat(new string[]
				{
					this.systemTime.Hour.ToString("00"),
					":",
					this.systemTime.Minute.ToString("00"),
					":",
					this.systemTime.Second.ToString("00")
				});
			}
			set
			{
				if (value == null)
				{
					Microsoft.Exchange.Diagnostics.Components.Data.Storage.ExTraceGlobals.WorkHoursTracer.TraceError((long)this.GetHashCode(), "Invalid Time: <null>");
					throw new WorkingHoursXmlMalformedException(ServerStrings.NullTimeInChangeDate);
				}
				string[] array = value.Split(new char[]
				{
					':'
				});
				if (array == null || array.Length != 3)
				{
					Microsoft.Exchange.Diagnostics.Components.Data.Storage.ExTraceGlobals.WorkHoursTracer.TraceError<string>((long)this.GetHashCode(), "Invalid Time: {0}", value);
					throw new WorkingHoursXmlMalformedException(ServerStrings.BadTimeFormatInChangeDate(value));
				}
				Exception ex = null;
				try
				{
					this.systemTime.Hour = Convert.ToUInt16(array[0]);
					this.systemTime.Minute = Convert.ToUInt16(array[1]);
					this.systemTime.Second = Convert.ToUInt16(array[2]);
				}
				catch (ArgumentException ex2)
				{
					ex = ex2;
				}
				catch (OverflowException ex3)
				{
					ex = ex3;
				}
				catch (FormatException ex4)
				{
					ex = ex4;
				}
				if (ex != null)
				{
					Microsoft.Exchange.Diagnostics.Components.Data.Storage.ExTraceGlobals.WorkHoursTracer.TraceError<string, Exception>((long)this.GetHashCode(), "Invalid Time: {0}. Exception={1}", value, ex);
					throw new WorkingHoursXmlMalformedException(ServerStrings.BadDateTimeFormatInChangeDate, ex);
				}
			}
		}

		[XmlElement]
		public string Date
		{
			get
			{
				return string.Concat(new string[]
				{
					this.systemTime.Year.ToString("00"),
					"/",
					this.systemTime.Month.ToString("00"),
					"/",
					this.systemTime.Day.ToString("00")
				});
			}
			set
			{
				if (value == null)
				{
					Microsoft.Exchange.Diagnostics.Components.Data.Storage.ExTraceGlobals.WorkHoursTracer.TraceError((long)this.GetHashCode(), "Invalid Date: <null>");
					throw new WorkingHoursXmlMalformedException(ServerStrings.NullDateInChangeDate);
				}
				string[] array = value.Split(new char[]
				{
					'/'
				});
				if (array == null || array.Length != 3)
				{
					Microsoft.Exchange.Diagnostics.Components.Data.Storage.ExTraceGlobals.WorkHoursTracer.TraceError<string>((long)this.GetHashCode(), "Invalid Date: {0}", value);
					throw new WorkingHoursXmlMalformedException(ServerStrings.BadDateFormatInChangeDate);
				}
				Exception ex = null;
				try
				{
					this.systemTime.Year = Convert.ToUInt16(array[0]);
					this.systemTime.Month = Convert.ToUInt16(array[1]);
					this.systemTime.Day = Convert.ToUInt16(array[2]);
				}
				catch (ArgumentException ex2)
				{
					ex = ex2;
				}
				catch (OverflowException ex3)
				{
					ex = ex3;
				}
				catch (FormatException ex4)
				{
					ex = ex4;
				}
				if (ex != null)
				{
					Microsoft.Exchange.Diagnostics.Components.Data.Storage.ExTraceGlobals.WorkHoursTracer.TraceError<string, Exception>((long)this.GetHashCode(), "Invalid Date: {0}. Exception={1}", value, ex);
					throw new WorkingHoursXmlMalformedException(ServerStrings.BadDateTimeFormatInChangeDate, ex);
				}
			}
		}

		[XmlElement]
		public short DayOfWeek
		{
			get
			{
				return (short)this.systemTime.DayOfWeek;
			}
			set
			{
				this.systemTime.DayOfWeek = (ushort)value;
			}
		}

		public ChangeDate()
		{
		}

		public override string ToString()
		{
			if (this.toString == null)
			{
				this.toString = string.Concat(new object[]
				{
					"Time=",
					this.Time,
					", Date=",
					this.Date,
					", DayOfWeek=",
					this.DayOfWeek
				});
			}
			return this.toString;
		}

		internal ChangeDate(NativeMethods.SystemTime systemTime)
		{
			this.systemTime = systemTime;
		}

		[XmlIgnore]
		internal NativeMethods.SystemTime SystemTime
		{
			get
			{
				return this.systemTime;
			}
			set
			{
				this.systemTime = value;
			}
		}

		[NonSerialized]
		private NativeMethods.SystemTime systemTime;

		private string toString;

		private static readonly Trace Tracer = Microsoft.Exchange.Diagnostics.Components.InfoWorker.Common.ExTraceGlobals.WorkingHoursTracer;
	}
}
