using System;
using System.Collections.Generic;
using System.Management.Automation;
using Microsoft.Exchange.Management.FfoReporting.Common;
using Microsoft.Office.Datacenter.ActiveMonitoring;

namespace Microsoft.Exchange.Management.FfoReporting
{
	[Cmdlet("Get", "MobileDeviceDashboardSummaryReport")]
	[OutputType(new Type[]
	{
		typeof(MobileDeviceDashboardSummaryReport)
	})]
	public sealed class GetMobileDeviceDashboardSummaryReport : FfoReportingDalTask<MobileDeviceDashboardSummaryReport>
	{
		public GetMobileDeviceDashboardSummaryReport() : base("Microsoft.Exchange.Hygiene.Data.DeviceSnapshot, Microsoft.Exchange.Hygiene.Data")
		{
		}

		public override string DataSessionTypeName
		{
			get
			{
				return "Microsoft.Exchange.Hygiene.Data.MobileDeviceSession";
			}
		}

		public override string DataSessionMethodName
		{
			get
			{
				return "GetDashboardSummary";
			}
		}

		public override string ComponentName
		{
			get
			{
				return ExchangeComponent.FfoMobileDevices.Name;
			}
		}

		public override string MonitorEventName
		{
			get
			{
				return "FFO Reporting Task Status Monitor";
			}
		}

		public override string DalMonitorEventName
		{
			get
			{
				return "FFO DAL Retrieval Status Monitor";
			}
		}

		protected override IReadOnlyList<MobileDeviceDashboardSummaryReport> AggregateOutput()
		{
			IReadOnlyList<MobileDeviceDashboardSummaryReport> readOnlyList = base.AggregateOutput();
			if (readOnlyList.Count == 0)
			{
				MobileDeviceDashboardSummaryReport item = new MobileDeviceDashboardSummaryReport
				{
					Platform = null,
					TotalDevicesCount = 0,
					AllowedDevicesCount = 0,
					BlockedDevicesCount = 0,
					QuarantinedDevicesCount = 0,
					UnknownDevicesCount = 0,
					LastUpdatedTime = DateTime.UtcNow,
					StartDate = new DateTime?(new DateTime(2014, 1, 1, 0, 0, 0)),
					EndDate = new DateTime?(new DateTime(2014, 12, 31, 23, 59, 59))
				};
				readOnlyList = new List<MobileDeviceDashboardSummaryReport>
				{
					item
				};
			}
			return readOnlyList;
		}

		protected override void CustomInternalValidate()
		{
			base.CustomInternalValidate();
			Schema.Utilities.CheckDates(this.StartDate, this.EndDate, new Schema.Utilities.NotifyNeedDefaultDatesDelegate(this.SetDefaultDates), new Schema.Utilities.ValidateDatesDelegate(Schema.Utilities.VerifyDateRange));
		}

		private void SetDefaultDates()
		{
			DateTime utcNow = DateTime.UtcNow;
			this.EndDate = new DateTime?(utcNow);
			this.StartDate = new DateTime?(utcNow.AddDays(-14.0));
		}

		[Parameter(Mandatory = false)]
		[QueryParameter("StartDateQueryDefinition", new string[]
		{

		})]
		public DateTime? StartDate { get; set; }

		[Parameter(Mandatory = false)]
		[QueryParameter("EndDateQueryDefinition", new string[]
		{

		})]
		public DateTime? EndDate { get; set; }

		private const string MobileDeviceDataSessionTypeName = "Microsoft.Exchange.Hygiene.Data.MobileDeviceSession";

		private const string MobileDeviceDataSessionMethodName = "GetDashboardSummary";

		private const string MobileDeviceDALTypeName = "Microsoft.Exchange.Hygiene.Data.DeviceSnapshot, Microsoft.Exchange.Hygiene.Data";
	}
}
