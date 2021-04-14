using System;
using System.Data.Services.Common;
using Microsoft.Exchange.Management.FfoReporting.Common;

namespace Microsoft.Exchange.Management.FfoReporting
{
	[DataServiceKey("TotalDevicesCount")]
	[Serializable]
	public sealed class MobileDeviceDashboardSummaryReport : FfoReportObject
	{
		[DalConversion("DefaultSerializer", "Platform", new string[]
		{

		})]
		public string Platform { get; set; }

		[DalConversion("DefaultSerializer", "TotalDevicesCount", new string[]
		{

		})]
		public int TotalDevicesCount { get; set; }

		[DalConversion("DefaultSerializer", "AllowedDevicesCount", new string[]
		{

		})]
		public int AllowedDevicesCount { get; set; }

		[DalConversion("DefaultSerializer", "BlockedDevicesCount", new string[]
		{

		})]
		public int BlockedDevicesCount { get; set; }

		[DalConversion("DefaultSerializer", "QuarantinedDevicesCount", new string[]
		{

		})]
		public int QuarantinedDevicesCount { get; set; }

		[DalConversion("DefaultSerializer", "UnknownDevicesCount", new string[]
		{

		})]
		public int UnknownDevicesCount { get; set; }

		[DalConversion("DefaultSerializer", "LastUpdatedTime", new string[]
		{

		})]
		public DateTime LastUpdatedTime { get; set; }

		[ODataInput("StartDate")]
		public DateTime? StartDate { get; set; }

		[ODataInput("EndDate")]
		public DateTime? EndDate { get; set; }
	}
}
