using System;
using System.Data.Services.Common;

namespace Microsoft.Exchange.Management.FfoReporting.Common
{
	[DataServiceKey("Index")]
	[Serializable]
	public class TrafficObject : FfoReportObject, IPageableObject
	{
		[ODataInput("StartDate")]
		public DateTime StartDate { get; private set; }

		[ODataInput("EndDate")]
		public DateTime EndDate { get; private set; }

		[ODataInput("AggregateBy")]
		public string AggregateBy { get; private set; }

		public int Index { get; set; }
	}
}
