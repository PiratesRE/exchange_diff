using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Services.Core.Types
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	[XmlType("AddAggregatedAccountRequestType", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
	public class AddAggregatedAccountRequest : BaseAggregatedAccountRequest
	{
		[XmlElement]
		[DataMember(IsRequired = false)]
		public string Authentication { get; set; }

		[DataMember(IsRequired = true)]
		[XmlElement]
		public string EmailAddress { get; set; }

		[DataMember(IsRequired = false)]
		[XmlElement]
		public string UserName { get; set; }

		[XmlElement]
		[DataMember(IsRequired = false)]
		public string Password { get; set; }

		[DataMember(IsRequired = false)]
		[XmlElement]
		public string IncomingServer { get; set; }

		[DataMember(IsRequired = false)]
		[XmlElement]
		public string IncomingPort { get; set; }

		[DataMember(IsRequired = false)]
		[XmlElement]
		public string IncrementalSyncInterval { get; set; }

		[XmlElement]
		[DataMember(IsRequired = false)]
		public string IncomingProtocol { get; set; }

		[DataMember(IsRequired = false)]
		[XmlElement]
		public string Security { get; set; }

		[XmlElement]
		[DataMember(IsRequired = false)]
		public string OutgoingServer { get; set; }

		[DataMember(IsRequired = false)]
		[XmlElement]
		public string OutgoingPort { get; set; }

		[XmlElement]
		[DataMember(IsRequired = false)]
		public string OutgoingProtocol { get; set; }

		internal override ServiceCommandBase GetServiceCommand(CallContext callContext)
		{
			return new AddAggregatedAccount(callContext, this);
		}
	}
}
