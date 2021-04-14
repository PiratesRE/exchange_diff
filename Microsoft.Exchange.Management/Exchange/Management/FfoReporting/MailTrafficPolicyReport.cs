using System;
using Microsoft.Exchange.Management.FfoReporting.Common;

namespace Microsoft.Exchange.Management.FfoReporting
{
	[Serializable]
	public class MailTrafficPolicyReport : TrafficObject
	{
		[DalConversion("OrganizationFromTask", "Organization", new string[]
		{

		})]
		public string Organization { get; private set; }

		[Redact]
		[ODataInput("Domain")]
		[DalConversion("DefaultSerializer", "Domain", new string[]
		{

		})]
		public string Domain { get; private set; }

		[DalConversion("DateFromIntSerializer", "DateKey", new string[]
		{
			"HourKey"
		})]
		public DateTime Date { get; private set; }

		[DalConversion("DefaultSerializer", "PolicyName", new string[]
		{

		})]
		[ODataInput("DlpPolicy")]
		public string DlpPolicy { get; private set; }

		[ODataInput("TransportRule")]
		[DalConversion("DefaultSerializer", "RuleName", new string[]
		{

		})]
		public string TransportRule { get; private set; }

		[ODataInput("Action")]
		[DalConversion("DefaultSerializer", "Action", new string[]
		{

		})]
		public string Action { get; private set; }

		[ODataInput("EventType")]
		[DalConversion("DefaultSerializer", "EventType", new string[]
		{

		})]
		public string EventType { get; private set; }

		[DalConversion("DefaultSerializer", "Direction", new string[]
		{

		})]
		[ODataInput("Direction")]
		public string Direction { get; private set; }

		[DalConversion("DefaultSerializer", "MessageCount", new string[]
		{

		})]
		public int MessageCount { get; private set; }

		[ODataInput("SummarizeBy")]
		public string SummarizeBy { get; private set; }
	}
}
