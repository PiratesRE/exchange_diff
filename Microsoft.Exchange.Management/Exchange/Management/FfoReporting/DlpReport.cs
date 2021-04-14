using System;
using Microsoft.Exchange.Management.FfoReporting.Common;

namespace Microsoft.Exchange.Management.FfoReporting
{
	[Serializable]
	public class DlpReport : TrafficObject
	{
		[DalConversion("OrganizationFromTask", "Organization", new string[]
		{

		})]
		public string Organization { get; private set; }

		[DalConversion("DateFromIntSerializer", "DateKey", new string[]
		{
			"HourKey"
		})]
		public DateTime Date { get; private set; }

		[ODataInput("DlpPolicy")]
		[DalConversion("DefaultSerializer", "PolicyName", new string[]
		{

		})]
		public string DlpPolicy { get; private set; }

		[ODataInput("TransportRule")]
		[DalConversion("DefaultSerializer", "RuleName", new string[]
		{

		})]
		public string TransportRule { get; private set; }

		[DalConversion("DefaultSerializer", "Action", new string[]
		{

		})]
		[ODataInput("Action")]
		public string Action { get; private set; }

		[ODataInput("EventType")]
		[DalConversion("DefaultSerializer", "EventType", new string[]
		{

		})]
		public string EventType { get; private set; }

		[DalConversion("DefaultSerializer", "MessageCount", new string[]
		{

		})]
		public int MessageCount { get; private set; }

		[ODataInput("SummarizeBy")]
		public string SummarizeBy { get; private set; }

		[ODataInput("Source")]
		[DalConversion("DefaultSerializer", "DataSource", new string[]
		{

		})]
		public string Source { get; private set; }
	}
}
