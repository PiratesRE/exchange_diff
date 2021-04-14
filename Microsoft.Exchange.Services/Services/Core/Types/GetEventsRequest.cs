using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Services.Core.Types
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	[XmlType("GetEventsType", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
	public class GetEventsRequest : BasePullRequest
	{
		[XmlElement]
		[DataMember(IsRequired = true)]
		public string Watermark { get; set; }

		internal override ServiceCommandBase GetServiceCommand(CallContext callContext)
		{
			return new GetEvents(callContext, this);
		}
	}
}
