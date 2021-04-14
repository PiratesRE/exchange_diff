using System;
using System.Data.Linq.Mapping;
using System.Data.Services.Common;
using Microsoft.Exchange.Management.FfoReporting.Common;

namespace Microsoft.Exchange.Management.FfoReporting
{
	[DataServiceKey("ConnectorName")]
	[Serializable]
	public class ServiceDeliveryReport : FfoReportObject
	{
		[Column(Name = "Organization")]
		[DalConversion("OrganizationFromTask", "Organization", new string[]
		{

		})]
		public string Organization { get; internal set; }

		[Column(Name = "IsAcceptedDomain")]
		[DalConversion("DefaultSerializer", "IsAcceptedDomain", new string[]
		{

		})]
		public bool IsAcceptedDomain { get; internal set; }

		[Column(Name = "IsOnPremMailbox")]
		[DalConversion("DefaultSerializer", "IsOnPremMailbox", new string[]
		{

		})]
		public bool IsOnPremMailbox { get; internal set; }

		[DalConversion("DefaultSerializer", "ConnectorName", new string[]
		{

		})]
		[Column(Name = "ConnectorName")]
		public string ConnectorName { get; internal set; }

		[Column(Name = "SmartHost")]
		[DalConversion("DefaultSerializer", "IPAddress", new string[]
		{

		})]
		public string SmartHost { get; internal set; }

		[Column(Name = "IsListeningOnPort25")]
		[DalConversion("DefaultSerializer", "IsListeningOnPort25", new string[]
		{

		})]
		public bool IsListeningOnPort25 { get; internal set; }

		[DalConversion("DefaultSerializer", "IsSuccessfullyReceivingMail", new string[]
		{

		})]
		[Column(Name = "IsSuccessfullyReceivingMail")]
		public bool IsSuccessfullyReceivingMail { get; internal set; }

		[DalConversion("ValueFromTask", "Recipient", new string[]
		{

		})]
		[ODataInput("Recipient")]
		public string Recipient { get; internal set; }
	}
}
