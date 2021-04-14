using System;
using System.Runtime.Serialization;
using System.Text;
using System.Xml.Serialization;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.InfoWorker.Common.Availability
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	public class WorkingHours
	{
		internal static WorkingHours LoadFrom(MailboxSession session, StoreId folderId)
		{
			StorageWorkingHours storageWorkingHours = StorageWorkingHours.LoadFrom(session, folderId);
			if (storageWorkingHours == null)
			{
				return null;
			}
			return new WorkingHours
			{
				storageWorkingHours = storageWorkingHours
			};
		}

		internal static WorkingHours CreateDefaultWorkingHours(ExTimeZone timeZone)
		{
			return WorkingHours.Create(timeZone, DaysOfWeek.Monday | DaysOfWeek.Tuesday | DaysOfWeek.Wednesday | DaysOfWeek.Thursday | DaysOfWeek.Friday, 480, 1020);
		}

		internal static WorkingHours Create(ExTimeZone timeZone, DaysOfWeek daysOfWeek, int startTimeInMinutes, int endTimeInMinutes)
		{
			return new WorkingHours(timeZone, daysOfWeek, startTimeInMinutes, endTimeInMinutes);
		}

		internal void SaveTo(MailboxSession session, StoreId folderId)
		{
			if (this.WorkingPeriodArray == null || this.WorkingPeriodArray.Length != 1 || this.WorkingPeriodArray[0] == null)
			{
				throw new ArgumentException("WorkingPeriodArray", "WorkingPeriodArray must have one element");
			}
			WorkingPeriod workingPeriod = this.WorkingPeriodArray[0];
			this.storageWorkingHours.SaveTo(session, folderId);
		}

		private WorkingHours(ExTimeZone timeZone, DaysOfWeek daysOfWeek, int startTimeInMinutes, int endTimeInMinutes)
		{
			if (timeZone == null)
			{
				throw new ArgumentException("timeZone");
			}
			this.storageWorkingHours = StorageWorkingHours.Create(timeZone, (int)WorkingHours.ToStorageDaysOfWeek(daysOfWeek), startTimeInMinutes, endTimeInMinutes);
		}

		[DataMember]
		[XmlElement(IsNullable = false)]
		public SerializableTimeZone TimeZone
		{
			get
			{
				if (this.HasNullDelegate())
				{
					return null;
				}
				return new SerializableTimeZone(this.ExTimeZone);
			}
			set
			{
				if (value != null)
				{
					this.ExTimeZone = value.TimeZone;
				}
			}
		}

		[XmlArray(IsNullable = false)]
		[XmlArrayItem(Type = typeof(WorkingPeriod), IsNullable = false)]
		[DataMember]
		public WorkingPeriod[] WorkingPeriodArray
		{
			get
			{
				if (this.HasNullDelegate())
				{
					return null;
				}
				return new WorkingPeriod[]
				{
					new WorkingPeriod(this.DaysOfWeek, this.StartTimeInMinutes, this.EndTimeInMinutes)
				};
			}
			set
			{
				if (value == null || value.Length != 1)
				{
					throw new ArgumentException("WorkingPeriodArray can not be null or have more than one element in Version1.");
				}
				this.storageWorkingHours.UpdateWorkingPeriod((DaysOfWeek)value[0].DayOfWeek, value[0].StartTimeInMinutes, value[0].EndTimeInMinutes);
			}
		}

		[XmlIgnore]
		internal ExTimeZone ExTimeZone
		{
			get
			{
				if (this.storageWorkingHours == null)
				{
					return null;
				}
				return this.storageWorkingHours.TimeZone;
			}
			set
			{
				if (this.storageWorkingHours != null)
				{
					this.storageWorkingHours.TimeZone = value;
				}
			}
		}

		public override string ToString()
		{
			if (this.toString == null)
			{
				StringBuilder stringBuilder = new StringBuilder(100);
				stringBuilder.AppendFormat("TimeZone = {0}\n", this.ExTimeZone);
				if (this.WorkingPeriodArray == null || this.WorkingPeriodArray.Length == 0)
				{
					stringBuilder.Append("<no working hours>");
				}
				else
				{
					stringBuilder.AppendFormat("WorkingPeriods\n", new object[0]);
					foreach (WorkingPeriod arg in this.WorkingPeriodArray)
					{
						stringBuilder.AppendFormat("  WorkingPeriod : {0}\n", arg);
					}
				}
				this.toString = stringBuilder.ToString();
			}
			return this.toString;
		}

		public WorkingHours()
		{
			this.storageWorkingHours = StorageWorkingHours.Create(ExTimeZone.CurrentTimeZone);
		}

		public bool InWorkingHours(ExDateTime startUtc, ExDateTime endUtc)
		{
			DaysOfWeek daysOfWeek;
			int num;
			this.UtcToWorkTime(startUtc, out daysOfWeek, out num);
			DaysOfWeek daysOfWeek2;
			int num2;
			this.UtcToWorkTime(endUtc, out daysOfWeek2, out num2);
			if (this.WorkingPeriodArray == null || this.WorkingPeriodArray.Length != 1)
			{
				throw new ArgumentException("WorkingPeriodArray can not be null or have more than one element in Version1.");
			}
			WorkingPeriod workingPeriod = this.WorkingPeriodArray[0];
			if (daysOfWeek == daysOfWeek2 && (daysOfWeek & workingPeriod.DayOfWeek) != (DaysOfWeek)0 && num >= workingPeriod.StartTimeInMinutes && num <= workingPeriod.EndTimeInMinutes && num2 >= workingPeriod.StartTimeInMinutes && num2 <= workingPeriod.EndTimeInMinutes)
			{
				return true;
			}
			if (daysOfWeek != daysOfWeek2 && workingPeriod.StartTimeInMinutes == 0 && workingPeriod.EndTimeInMinutes >= 1439)
			{
				DaysOfWeek daysOfWeek3 = daysOfWeek;
				while ((daysOfWeek3 & workingPeriod.DayOfWeek) != (DaysOfWeek)0)
				{
					if (daysOfWeek3 == daysOfWeek2)
					{
						return true;
					}
					daysOfWeek3 = WorkingHours.NextDays(daysOfWeek3);
				}
				return false;
			}
			return false;
		}

		public static DaysOfWeek DayToDays(DayOfWeek dow)
		{
			switch (dow)
			{
			case DayOfWeek.Sunday:
				return DaysOfWeek.Sunday;
			case DayOfWeek.Monday:
				return DaysOfWeek.Monday;
			case DayOfWeek.Tuesday:
				return DaysOfWeek.Tuesday;
			case DayOfWeek.Wednesday:
				return DaysOfWeek.Wednesday;
			case DayOfWeek.Thursday:
				return DaysOfWeek.Thursday;
			case DayOfWeek.Friday:
				return DaysOfWeek.Friday;
			case DayOfWeek.Saturday:
				return DaysOfWeek.Saturday;
			default:
				throw new ArgumentException("dow");
			}
		}

		public static DaysOfWeek NextDays(DaysOfWeek d)
		{
			if (d <= DaysOfWeek.Wednesday)
			{
				switch (d)
				{
				case DaysOfWeek.Sunday:
					return DaysOfWeek.Monday;
				case DaysOfWeek.Monday:
					return DaysOfWeek.Tuesday;
				case DaysOfWeek.Sunday | DaysOfWeek.Monday:
					break;
				case DaysOfWeek.Tuesday:
					return DaysOfWeek.Wednesday;
				default:
					if (d == DaysOfWeek.Wednesday)
					{
						return DaysOfWeek.Thursday;
					}
					break;
				}
			}
			else
			{
				if (d == DaysOfWeek.Thursday)
				{
					return DaysOfWeek.Friday;
				}
				if (d == DaysOfWeek.Friday)
				{
					return DaysOfWeek.Saturday;
				}
				if (d == DaysOfWeek.Saturday)
				{
					return DaysOfWeek.Sunday;
				}
			}
			throw new ArgumentException("d");
		}

		public DaysOfWeek DaysOfWeek
		{
			get
			{
				if (this.HasNullDelegate())
				{
					return (DaysOfWeek)0;
				}
				return (DaysOfWeek)this.storageWorkingHours.DaysOfWeek;
			}
		}

		public int StartTimeInMinutes
		{
			get
			{
				if (this.HasNullDelegate())
				{
					return 0;
				}
				return this.storageWorkingHours.StartTimeInMinutes;
			}
		}

		public int EndTimeInMinutes
		{
			get
			{
				if (this.HasNullDelegate())
				{
					return 0;
				}
				return this.storageWorkingHours.EndTimeInMinutes;
			}
		}

		private static DaysOfWeek FromStorageDaysOfWeek(DaysOfWeek daysOfWeek)
		{
			return (DaysOfWeek)daysOfWeek;
		}

		private static DaysOfWeek ToStorageDaysOfWeek(DaysOfWeek daysOfWeek)
		{
			return (DaysOfWeek)daysOfWeek;
		}

		private bool HasNullDelegate()
		{
			return this.storageWorkingHours == null;
		}

		private void UtcToWorkTime(ExDateTime utcTime, out DaysOfWeek day, out int minuteOfDay)
		{
			ExDateTime exDateTime = (this.ExTimeZone ?? ExTimeZone.CurrentTimeZone).ConvertDateTime(utcTime);
			day = WorkingHours.DayToDays(exDateTime.DayOfWeek);
			minuteOfDay = exDateTime.Hour * 60 + exDateTime.Minute;
		}

		private StorageWorkingHours storageWorkingHours;

		private string toString;
	}
}
