using System;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.Services.Core.DataConverter
{
	internal class CalendarItemTypeConverter : EnumConverter
	{
		public static string ToString(CalendarItemType calendarItemType)
		{
			string result = null;
			switch (calendarItemType)
			{
			case CalendarItemType.Single:
				result = "Single";
				break;
			case CalendarItemType.Occurrence:
				result = "Occurrence";
				break;
			case CalendarItemType.Exception:
				result = "Exception";
				break;
			case CalendarItemType.RecurringMaster:
				result = "RecurringMaster";
				break;
			}
			return result;
		}

		public static CalendarItemType Parse(string calendarItemTypeString)
		{
			if (calendarItemTypeString != null)
			{
				CalendarItemType result;
				if (!(calendarItemTypeString == "Exception"))
				{
					if (!(calendarItemTypeString == "Occurrence"))
					{
						if (!(calendarItemTypeString == "RecurringMaster"))
						{
							if (!(calendarItemTypeString == "Single"))
							{
								goto IL_4D;
							}
							result = CalendarItemType.Single;
						}
						else
						{
							result = CalendarItemType.RecurringMaster;
						}
					}
					else
					{
						result = CalendarItemType.Occurrence;
					}
				}
				else
				{
					result = CalendarItemType.Exception;
				}
				return result;
			}
			IL_4D:
			throw new FormatException("Invalid calendarItemType string: " + calendarItemTypeString);
		}

		public override object ConvertToObject(string propertyString)
		{
			return CalendarItemTypeConverter.Parse(propertyString);
		}

		public override string ConvertToString(object propertyValue)
		{
			return CalendarItemTypeConverter.ToString((CalendarItemType)propertyValue);
		}

		private const string ExceptionStringValue = "Exception";

		private const string OccurrenceStringValue = "Occurrence";

		private const string RecurringMasterStringValue = "RecurringMaster";

		private const string SingleStringValue = "Single";
	}
}
