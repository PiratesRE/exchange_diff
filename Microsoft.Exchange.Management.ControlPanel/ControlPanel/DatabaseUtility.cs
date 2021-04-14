using System;
using System.ServiceModel;
using System.Web;

namespace Microsoft.Exchange.Management.ControlPanel
{
	public static class DatabaseUtility
	{
		public static string PageTitle
		{
			get
			{
				if (DatabasePageId.MaintenanceSchedule == DatabaseUtility.GetDatabasePageId())
				{
					return Strings.DatabaseMaintenanceSchedulePageTitle;
				}
				if (DatabasePageId.QuotaNotificationSchedule == DatabaseUtility.GetDatabasePageId())
				{
					return Strings.DatabaseQuotaNotificationSchedulePageTitle;
				}
				return string.Empty;
			}
		}

		public static string LegendForTrueValue
		{
			get
			{
				if (DatabasePageId.MaintenanceSchedule == DatabaseUtility.GetDatabasePageId())
				{
					return Strings.DatabaseSchedulerMaintenanceHours;
				}
				if (DatabasePageId.QuotaNotificationSchedule == DatabaseUtility.GetDatabasePageId())
				{
					return Strings.DatabaseSchedulerQuotaNotificationHours;
				}
				return string.Empty;
			}
		}

		public static string LegendForFalseValue
		{
			get
			{
				if (DatabasePageId.MaintenanceSchedule == DatabaseUtility.GetDatabasePageId())
				{
					return Strings.DatabasechedulerNonMaintenanceHours;
				}
				if (DatabasePageId.QuotaNotificationSchedule == DatabaseUtility.GetDatabasePageId())
				{
					return Strings.DatabasechedulerNonQuotaNotificationHours;
				}
				return string.Empty;
			}
		}

		private static DatabasePageId GetDatabasePageId()
		{
			string text = HttpContext.Current.Request.QueryString["functionaltype"];
			int num;
			DatabasePageId result;
			if (!int.TryParse(text, out num) && Enum.TryParse<DatabasePageId>(text, out result))
			{
				return result;
			}
			throw new FaultException("The query string functionaltype = " + HttpContext.Current.Request.QueryString["functionaltype"] + " could not be understood.");
		}
	}
}
