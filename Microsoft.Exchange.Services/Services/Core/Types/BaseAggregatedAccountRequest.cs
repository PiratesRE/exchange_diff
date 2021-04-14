using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using Microsoft.Exchange.Data.Directory.ResourceHealth;

namespace Microsoft.Exchange.Services.Core.Types
{
	[XmlInclude(typeof(RemoveAggregatedAccountRequest))]
	[XmlType("BaseAggregatedAccountRequestType", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
	[XmlInclude(typeof(CreateUnifiedMailboxRequest))]
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	[XmlInclude(typeof(SetAggregatedAccountRequest))]
	[XmlInclude(typeof(GetAggregatedAccountRequest))]
	[XmlInclude(typeof(AddAggregatedAccountRequest))]
	public class BaseAggregatedAccountRequest : BaseRequest
	{
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
