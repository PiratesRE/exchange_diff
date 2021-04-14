using System;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Services.Core.Types
{
	[XmlType("RemoveAggregatedAccountRequestType", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
	public class RemoveAggregatedAccountRequest : BaseAggregatedAccountRequest
	{
		[XmlElement]
		public string EmailAddress { get; set; }

		internal override ServiceCommandBase GetServiceCommand(CallContext callContext)
		{
			return new RemoveAggregatedAccount(callContext, this);
		}
	}
}
