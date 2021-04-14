using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using Microsoft.Exchange.Data.Directory.ResourceHealth;

namespace Microsoft.Exchange.Services.Core.Types
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	[XmlType("GetClientAccessTokenType", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
	public class GetClientAccessTokenRequest : BaseRequest
	{
		[XmlArrayItem(ElementName = "TokenRequest", Namespace = "http://schemas.microsoft.com/exchange/services/2006/types", Type = typeof(ClientAccessTokenRequestType))]
		[DataMember]
		public ClientAccessTokenRequestType[] TokenRequests { get; set; }

		internal override ServiceCommandBase GetServiceCommand(CallContext callContext)
		{
			return new GetClientAccessToken(callContext, this);
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
