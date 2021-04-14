using System;
using System.Collections.Generic;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Management.Tasks;
using Microsoft.Exchange.Monitoring.Reporting;

namespace Microsoft.Exchange.Monitoring
{
	public abstract class GetAvailabilityReport<TIdentity> : ReportingTask<TIdentity> where TIdentity : IIdentityParameter, new()
	{
		[Parameter(ParameterSetName = "StartEndDateSet", Mandatory = true)]
		public DateTime StartDate
		{
			get
			{
				return (DateTime)base.Fields["StartDate"];
			}
			set
			{
				base.Fields["StartDate"] = value;
			}
		}

		[Parameter(ParameterSetName = "StartEndDateSet", Mandatory = true)]
		public DateTime EndDate
		{
			get
			{
				return (DateTime)base.Fields["EndDate"];
			}
			set
			{
				base.Fields["EndDate"] = value;
			}
		}

		[Parameter(ParameterSetName = "ReportingPeriodSet", Mandatory = false)]
		public ReportingPeriod ReportingPeriod
		{
			get
			{
				return (ReportingPeriod)(base.Fields["ReportingPeriod"] ?? ReportingPeriod.LastMonth);
			}
			set
			{
				base.Fields["ReportingPeriod"] = value;
			}
		}

		[Parameter(ParameterSetName = "ReportingPeriodSet", Mandatory = false)]
		[Parameter(ParameterSetName = "StartEndDateSet", Mandatory = false)]
		public SwitchParameter DailyStatistics
		{
			get
			{
				return (SwitchParameter)(base.Fields["DailyStatistics"] ?? new SwitchParameter(false));
			}
			set
			{
				base.Fields["DailyStatistics"] = value;
			}
		}

		protected override void InternalValidate()
		{
			TaskLogger.LogEnter();
			if (base.ParameterSetName == "StartEndDateSet")
			{
				if (this.EndDate < this.StartDate)
				{
					base.WriteError(new ArgumentException(Strings.InvalidReportingDateRange(this.StartDate, this.EndDate), "StartDate, EndDate"), (ErrorCategory)1000, null);
				}
				if (this.StartDate < new DateTime(1753, 1, 1))
				{
					base.WriteError(new ArgumentException(Strings.InvalidReportingStartDate(this.StartDate), "StartDate"), (ErrorCategory)1000, null);
				}
				this.utcStartDateTime = new ExDateTime(ExTimeZone.UtcTimeZone, this.StartDate.Date);
				this.utcEndDateTime = new ExDateTime(ExTimeZone.UtcTimeZone, this.EndDate.Date);
			}
			else if (base.ParameterSetName == "ReportingPeriodSet")
			{
				this.GetStartEndDateForReportingPeriod((ReportingPeriod)base.Fields["ReportingPeriod"], out this.utcStartDateTime, out this.utcEndDateTime);
			}
			base.InternalValidate();
			TaskLogger.LogExit();
		}

		protected override void WriteResult(IConfigurable dataObject)
		{
			TaskLogger.LogEnter();
			if (dataObject != null)
			{
				ADOrganizationalUnit adorganizationalUnit = (ADOrganizationalUnit)dataObject;
				if (adorganizationalUnit.OrganizationId.OrganizationalUnit != null && adorganizationalUnit.OrganizationId.ConfigurationUnit != null)
				{
					this.GetTenantAvailabilityReport(adorganizationalUnit.OrganizationId.OrganizationalUnit);
				}
			}
			TaskLogger.LogExit();
		}

		protected override void WriteResult<T>(IEnumerable<T> dataObjects)
		{
			TaskLogger.LogEnter();
			if (dataObjects != null)
			{
				if (this.Identity != null)
				{
					base.WriteResult<T>(dataObjects);
				}
				else if (base.CurrentOrganizationId.Equals(OrganizationId.ForestWideOrgId))
				{
					this.GetTenantAvailabilityReport(null);
				}
				else
				{
					if (base.CurrentOrganizationId.OrganizationalUnit != null && base.CurrentOrganizationId.ConfigurationUnit != null)
					{
						this.GetTenantAvailabilityReport(base.CurrentOrganizationId.OrganizationalUnit);
					}
					base.WriteResult<T>(dataObjects);
				}
			}
			TaskLogger.LogExit();
		}

		protected virtual void WriteReport(ConfigurableObject reportObject)
		{
			base.WriteResult(reportObject);
		}

		protected abstract void GetTenantAvailabilityReport(ADObjectId tenantId);

		private void GetStartEndDateForReportingPeriod(ReportingPeriod reportingPeriod, out ExDateTime startDateTime, out ExDateTime endDateTime)
		{
			startDateTime = ExDateTime.MinValue;
			endDateTime = ExDateTime.MaxValue;
			ExDateTime date = ExDateTime.UtcNow.Date;
			switch (reportingPeriod)
			{
			case ReportingPeriod.Today:
				startDateTime = date;
				endDateTime = startDateTime.Add(TimeSpan.FromDays(1.0)).Subtract(TimeSpan.FromMinutes(1.0));
				return;
			case ReportingPeriod.Yesterday:
				startDateTime = date.Subtract(TimeSpan.FromDays(1.0));
				endDateTime = startDateTime.AddDays(1.0).Subtract(TimeSpan.FromMinutes(1.0));
				return;
			case ReportingPeriod.LastWeek:
				startDateTime = date.Subtract(TimeSpan.FromDays((double)(7 + date.DayOfWeek)));
				endDateTime = date.Subtract(TimeSpan.FromDays((double)date.DayOfWeek)).Subtract(TimeSpan.FromMinutes(1.0));
				return;
			case ReportingPeriod.LastMonth:
				startDateTime = GetAvailabilityReport<TIdentity>.SubtractMonths(date, 1);
				endDateTime = GetAvailabilityReport<TIdentity>.GetLastMonthLastDate(date);
				return;
			case ReportingPeriod.Last3Months:
				startDateTime = GetAvailabilityReport<TIdentity>.SubtractMonths(date, 3);
				endDateTime = GetAvailabilityReport<TIdentity>.GetLastMonthLastDate(date);
				return;
			case ReportingPeriod.Last6Months:
				startDateTime = GetAvailabilityReport<TIdentity>.SubtractMonths(date, 6);
				endDateTime = GetAvailabilityReport<TIdentity>.GetLastMonthLastDate(date);
				return;
			case ReportingPeriod.Last12Months:
				startDateTime = GetAvailabilityReport<TIdentity>.SubtractMonths(date, 12);
				endDateTime = GetAvailabilityReport<TIdentity>.GetLastMonthLastDate(date);
				return;
			default:
				base.WriteError(new ArgumentException(Strings.InvalidReportingPeriod), (ErrorCategory)1000, null);
				return;
			}
		}

		private static ExDateTime SubtractMonths(ExDateTime dateTime, int monthsToSubtract)
		{
			int num = dateTime.Year;
			int num2 = dateTime.Month;
			num2 -= monthsToSubtract;
			if (num2 <= 0)
			{
				num--;
				num2 += 12;
			}
			return new ExDateTime(ExTimeZone.UtcTimeZone, num, num2, 1);
		}

		private static ExDateTime GetLastMonthLastDate(ExDateTime datetime)
		{
			return new ExDateTime(ExTimeZone.UtcTimeZone, datetime.Year, datetime.Month, 1).Subtract(TimeSpan.FromMinutes(1.0));
		}

		protected const string StartEndDateParameterSetName = "StartEndDateSet";

		protected const string ReportingPeriodParameterSetName = "ReportingPeriodSet";

		protected const ReportingPeriod DefaultReportingPeriod = ReportingPeriod.LastMonth;

		protected const int TotalDaysInWeek = 7;

		protected ExDateTime utcStartDateTime = ExDateTime.MinValue;

		protected ExDateTime utcEndDateTime = ExDateTime.MaxValue;
	}
}
