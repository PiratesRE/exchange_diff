using System;
using System.Globalization;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[DataContract]
	public class UMAAHolidaySchedule
	{
		public UMAAHolidaySchedule(HolidaySchedule holiday)
		{
			this.Name = holiday.Name;
			this.GreetingFileName = holiday.Greeting;
			this.StartDate = holiday.StartDate.ToString("yyyy/MM/dd", CultureInfo.InvariantCulture);
			this.EndDate = holiday.EndDate.ToString("yyyy/MM/dd", CultureInfo.InvariantCulture);
			this.StartDateDisplay = holiday.StartDate.ToString(EcpDateTimeHelper.GetUserDateFormat(), CultureInfo.CurrentCulture);
			this.EndDateDisplay = holiday.EndDate.ToString(EcpDateTimeHelper.GetUserDateFormat(), CultureInfo.CurrentCulture);
		}

		[DataMember]
		public string Name { get; private set; }

		[DataMember]
		public string StartDate { get; private set; }

		[DataMember]
		public string StartDateDisplay { get; private set; }

		[DataMember]
		public string EndDate { get; private set; }

		[DataMember]
		public string EndDateDisplay { get; private set; }

		[DataMember]
		public string GreetingFileName { get; private set; }

		public HolidaySchedule ToHolidaySchedule()
		{
			this.Validate();
			return new HolidaySchedule(this.Name, this.GreetingFileName, DateTime.ParseExact(this.StartDate, "yyyy/MM/dd", CultureInfo.InvariantCulture), DateTime.ParseExact(this.EndDate, "yyyy/MM/dd", CultureInfo.InvariantCulture));
		}

		private void Validate()
		{
			this.Name.FaultIfNullOrEmpty(Strings.UMHolidayScheduleNameNotSet);
			this.GreetingFileName.FaultIfNullOrEmpty(Strings.UMHolidayScheduleGreetingFileNameNotSet);
			this.StartDate.FaultIfNullOrEmpty(Strings.UMHolidayScheduleStartDateNotSet);
			this.EndDate.FaultIfNullOrEmpty(Strings.UMHolidayScheduleEndDateNotSet);
		}
	}
}
