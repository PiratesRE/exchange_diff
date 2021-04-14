using System;
using System.Data.Linq.Mapping;
using System.Data.Services.Common;
using Microsoft.Exchange.Management.FfoReporting.Common;

namespace Microsoft.Exchange.Management.FfoReporting
{
	[DataServiceKey("Name")]
	[Serializable]
	public class OutboundConnectorReport : FfoReportObject
	{
		[Column(Name = "Organization")]
		[DalConversion("OrganizationFromTask", "Organization", new string[]
		{

		})]
		public string Organization { get; internal set; }

		[Column(Name = "Name")]
		[DalConversion("DefaultSerializer", "Name", new string[]
		{

		})]
		public string Name { get; internal set; }

		[ODataInput("Domain")]
		[DalConversion("ValueFromTask", "Domain", new string[]
		{

		})]
		public string Domain { get; internal set; }

		[DalConversion("DefaultSerializer", "IsAcceptedDomain", new string[]
		{

		})]
		[Column(Name = "IsAcceptedDomain")]
		public bool IsAcceptedDomain { get; internal set; }
	}
}
