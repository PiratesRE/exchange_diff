using System;
using System.Management.Automation;
using Microsoft.Exchange.Management.FfoReporting.Common;
using Microsoft.Office.Datacenter.ActiveMonitoring;

namespace Microsoft.Exchange.Management.FfoReporting
{
	[Cmdlet("Get", "MobileDeviceDetailsReport")]
	[OutputType(new Type[]
	{
		typeof(MobileDeviceDetailsReport)
	})]
	public sealed class GetMobileDeviceDetailsReport : FfoReportingDalTask<MobileDeviceDetailsReport>
	{
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
	}
}
