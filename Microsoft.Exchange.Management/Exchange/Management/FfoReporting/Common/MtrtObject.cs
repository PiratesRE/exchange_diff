using System;
using System.Data.Services.Common;

namespace Microsoft.Exchange.Management.FfoReporting.Common
{
	[DataServiceKey("Index")]
	[Serializable]
	public class MtrtObject : FfoReportObject, IPageableObject
	{
		[ODataInput("StartDate")]
		[DalConversion("ValueFromTask", "StartDate", new string[]
		{

		})]
		public DateTime StartDate { get; internal set; }

		[DalConversion("ValueFromTask", "EndDate", new string[]
		{

		})]
		[ODataInput("EndDate")]
		public DateTime EndDate { get; internal set; }

		public int Index { get; set; }
	}
}
