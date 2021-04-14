using System;
using System.Data.Linq.Mapping;
using System.Data.Services.Common;
using Microsoft.Exchange.Management.FfoReporting.Common;

namespace Microsoft.Exchange.Management.FfoReporting
{
	[DataServiceKey("PointsToService")]
	[Serializable]
	public class MxRecordReport : FfoReportObject
	{
		[DalConversion("OrganizationFromTask", "Organization", new string[]
		{

		})]
		[Column(Name = "Organization")]
		public string Organization { get; internal set; }

		[DalConversion("DefaultSerializer", "IsAcceptedDomain", new string[]
		{

		})]
		[Column(Name = "IsAcceptedDomain")]
		public bool IsAcceptedDomain { get; internal set; }

		[Column(Name = "RecordExists")]
		[DalConversion("DefaultSerializer", "MxRecordExists", new string[]
		{

		})]
		public bool RecordExists { get; internal set; }

		[Column(Name = "PointsToService")]
		[DalConversion("DefaultSerializer", "IsMxRecordPointingToService", new string[]
		{

		})]
		public bool PointsToService { get; internal set; }

		[Column(Name = "HighestPriorityMailhost")]
		[DalConversion("DefaultSerializer", "HighestPriorityMailhost", new string[]
		{

		})]
		public string HighestPriorityMailhost { get; internal set; }

		[Column(Name = "HighestPriorityMailhostIpAddress")]
		[DalConversion("DefaultSerializer", "HighestPriorityMailhostIpAddress", new string[]
		{

		})]
		public string HighestPriorityMailhostIpAddress { get; internal set; }

		[ODataInput("Domain")]
		[DalConversion("ValueFromTask", "Domain", new string[]
		{

		})]
		public string Domain { get; internal set; }
	}
}
