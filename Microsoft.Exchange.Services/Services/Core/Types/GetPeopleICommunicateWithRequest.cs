using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.ResourceHealth;

namespace Microsoft.Exchange.Services.Core.Types
{
	[XmlType("GetPeopleICommunicateWithType", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	public sealed class GetPeopleICommunicateWithRequest : BaseRequest
	{
		public GetPeopleICommunicateWithRequest(IOutgoingWebResponseContext outgoingResponse)
		{
			this.OutgoingResponse = outgoingResponse;
		}

		internal ADObjectId AdObjectId { get; set; }

		internal IOutgoingWebResponseContext OutgoingResponse { get; set; }

		internal override ServiceCommandBase GetServiceCommand(CallContext callContext)
		{
			return new GetPeopleICommunicateWith(callContext, this);
		}

		internal override BaseServerIdInfo GetProxyInfo(CallContext callContext)
		{
			return null;
		}

		internal override ResourceKey[] GetResources(CallContext callContext, int taskStep)
		{
			return null;
		}
	}
}
