using System;
using Microsoft.Exchange.Management.FfoReporting.Common;

namespace Microsoft.Exchange.Management.FfoReporting
{
	[Serializable]
	public class MailTrafficReport : TrafficObject
	{
		[DalConversion("OrganizationFromTask", "Organization", new string[]
		{

		})]
		public string Organization { get; private set; }

		[DalConversion("DefaultSerializer", "Domain", new string[]
		{

		})]
		[Redact]
		[ODataInput("Domain")]
		public string Domain { get; private set; }

		[DalConversion("DateFromIntSerializer", "DateKey", new string[]
		{
			"HourKey"
		})]
		public DateTime Date { get; private set; }

		[DalConversion("DefaultSerializer", "EventType", new string[]
		{

		})]
		[ODataInput("EventType")]
		public string EventType { get; private set; }

		[DalConversion("DefaultSerializer", "Direction", new string[]
		{

		})]
		[ODataInput("Direction")]
		public string Direction { get; private set; }

		[ODataInput("Action")]
		[DalConversion("DefaultSerializer", "Action", new string[]
		{

		})]
		public string Action { get; private set; }

		[DalConversion("DefaultSerializer", "MessageCount", new string[]
		{

		})]
		public int MessageCount { get; private set; }

		[ODataInput("SummarizeBy")]
		public string SummarizeBy { get; private set; }
	}
}
