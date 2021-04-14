using System;
using Microsoft.Exchange.Management.FfoReporting.Common;

namespace Microsoft.Exchange.Management.FfoReporting
{
	[Serializable]
	public class MailTrafficTopReport : TrafficObject
	{
		[DalConversion("OrganizationFromTask", "Organization", new string[]
		{

		})]
		public string Organization { get; private set; }

		[ODataInput("Domain")]
		[DalConversion("DefaultSerializer", "Domain", new string[]
		{

		})]
		[Redact]
		public string Domain { get; private set; }

		[DalConversion("DateFromIntSerializer", "DateKey", new string[]
		{
			"HourKey"
		})]
		public DateTime Date { get; private set; }

		[DalConversion("DefaultSerializer", "Name", new string[]
		{

		})]
		[Redact]
		public string Name { get; private set; }

		[DalConversion("DefaultSerializer", "EventType", new string[]
		{

		})]
		[ODataInput("EventType")]
		public string EventType { get; private set; }

		[ODataInput("Direction")]
		[DalConversion("DefaultSerializer", "Direction", new string[]
		{

		})]
		public string Direction { get; private set; }

		[DalConversion("DefaultSerializer", "MessageCount", new string[]
		{

		})]
		public int MessageCount { get; private set; }

		[ODataInput("SummarizeBy")]
		public string SummarizeBy { get; private set; }
	}
}
