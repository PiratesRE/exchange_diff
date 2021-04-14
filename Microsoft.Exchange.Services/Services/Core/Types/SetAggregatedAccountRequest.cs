using System;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Services.Core.Types
{
	[XmlType("SetAggregatedAccountRequestType", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
	public class SetAggregatedAccountRequest : BaseAggregatedAccountRequest
	{
		[XmlElement]
		public string Authentication { get; set; }

		[XmlElement]
		public string EmailAddress { get; set; }

		[XmlElement]
		public string UserName { get; set; }

		[XmlElement]
		public string Password { get; set; }

		[XmlElement]
		public string IncomingServer { get; set; }

		[XmlElement]
		public string IncomingPort { get; set; }

		[XmlElement]
		public string IncrementalSyncInterval { get; set; }

		[XmlElement]
		public string Security { get; set; }

		internal override ServiceCommandBase GetServiceCommand(CallContext callContext)
		{
			return new SetAggregatedAccount(callContext, this);
		}
	}
}
