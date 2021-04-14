using System;
using System.Collections.Generic;
using System.Data;
using System.ServiceModel;
using Microsoft.Exchange.Common;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Management.ControlPanel;
using Microsoft.Exchange.Management.ControlPanel.WebControls;

namespace Microsoft.Exchange.Management.DDIService
{
	public class UMAutoAttendantService : DDICodeBehind
	{
		public static void OnPostGetObject(DataRow inputRow, DataTable dataTable, DataObjectStore store)
		{
			if (dataTable.Rows.Count == 0 || store == null)
			{
				return;
			}
			DataRow dataRow = dataTable.Rows[0];
			UMAutoAttendant umautoAttendant = (UMAutoAttendant)store.GetDataObject("UMAutoAttendant");
			if (umautoAttendant != null)
			{
				UMDialPlan dialPlan = umautoAttendant.GetDialPlan();
				dataRow["ExtensionLength"] = dialPlan.NumberOfDigitsInExtension;
				dataRow["IsTelex"] = (dialPlan.URIType == UMUriType.TelExtn);
				List<UMAAMenuKeyMapping> value;
				List<UMAAMenuKeyMapping> value2;
				UMAAMenuKeyMapping.CreateMappings((MultiValuedProperty<CustomMenuKeyMapping>)dataRow["BusinessHoursKeyMapping"], (MultiValuedProperty<CustomMenuKeyMapping>)dataRow["AfterHoursKeyMapping"], out value, out value2);
				dataRow["BusinessHoursKeyMapping"] = value;
				dataRow["AfterHoursKeyMapping"] = value2;
				dataRow["BusinessHoursSchedule"] = new ScheduleBuilder(new Schedule(umautoAttendant.BusinessHoursSchedule)).GetEntireState();
			}
		}

		public static void OnPostGetObjectForNew(DataRow inputRow, DataTable dataTable, DataObjectStore store)
		{
			if (dataTable.Rows.Count == 0)
			{
				return;
			}
			DataRow dataRow = dataTable.Rows[0];
			UMDialPlan umdialPlan = (UMDialPlan)store.GetDataObject("UMDialPlan");
			dataRow["UMDialPlan"] = umdialPlan.Name;
			dataRow["ExtensionLength"] = umdialPlan.NumberOfDigitsInExtension;
			dataRow["IsTelex"] = (umdialPlan.URIType == UMUriType.TelExtn);
		}

		public static List<DropDownItemData> GetAvailableUmLanguages()
		{
			return UMDialPlanService.GetAvailableUmLanguages();
		}

		public static List<DropDownItemData> GetAllTimeZones()
		{
			List<DropDownItemData> list = new List<DropDownItemData>();
			foreach (ExTimeZone exTimeZone in ExTimeZoneEnumerator.Instance)
			{
				list.Add(new DropDownItemData(exTimeZone.DisplayName, exTimeZone.Id));
			}
			return list;
		}

		public static object GetInfoAnnouncementEnabled(string infoAnnouncementFilename, bool isInfoAnnouncementInterruptible)
		{
			return UMDialPlanService.GetInfoAnnouncementEnabled(infoAnnouncementFilename, isInfoAnnouncementInterruptible);
		}

		public static CustomMenuKeyMapping[] UMAAMenuKeyMappingToTask(object value)
		{
			if (!DDIHelper.IsEmptyValue(value))
			{
				return Array.ConvertAll<object, CustomMenuKeyMapping>((object[])value, (object x) => ((UMAAMenuKeyMapping)x).ToCustomKeyMapping());
			}
			return null;
		}

		public static HolidaySchedule[] UMAAHolidayScheduleToTask(object value)
		{
			if (!DDIHelper.IsEmptyValue(value))
			{
				return Array.ConvertAll<object, HolidaySchedule>((object[])value, (object x) => ((UMAAHolidaySchedule)x).ToHolidaySchedule());
			}
			return null;
		}

		public static IEnumerable<ScheduleInterval> BoolArrayToScheduleInterval(object value)
		{
			if (DDIHelper.IsEmptyValue(value))
			{
				return null;
			}
			bool[] array = Array.ConvertAll<object, bool>((object[])value, (object x) => (bool)x);
			if (array.Length != 672)
			{
				throw new FaultException(Strings.EditUMAutoAttendantBusinessHoursScheduleFault(array.Length));
			}
			ScheduleBuilder scheduleBuilder = new ScheduleBuilder();
			scheduleBuilder.SetEntireState(array);
			return scheduleBuilder.Schedule.Intervals;
		}

		private const string UMDialPlanName = "UMDialPlan";

		private const string ExtensionLengthName = "ExtensionLength";

		private const string IsTelexName = "IsTelex";

		private const string UMAutoAttendantName = "UMAutoAttendant";

		private const string BusinessHoursScheduleName = "BusinessHoursSchedule";

		private const string BusinessHoursKeyMappingName = "BusinessHoursKeyMapping";

		private const string AfterHoursKeyMappingName = "AfterHoursKeyMapping";

		private const int BusinessHoursScheduleStructureLength = 672;
	}
}
