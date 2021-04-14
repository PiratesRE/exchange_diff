using System;
using Microsoft.Exchange.Management.FfoReporting.Common;

namespace Microsoft.Exchange.Management.FfoReporting
{
	[Serializable]
	public class FfoMigrationReport : FfoReportObject
	{
		[DalConversion("OrganizationFromTask", "Organization", new string[]
		{

		})]
		public string Organization { get; private set; }

		[DalConversion("DefaultSerializer", "Report", new string[]
		{

		})]
		public string Report { get; private set; }
	}
}
