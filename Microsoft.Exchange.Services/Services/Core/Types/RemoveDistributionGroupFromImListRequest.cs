using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using Microsoft.Exchange.Data.Directory.ResourceHealth;

namespace Microsoft.Exchange.Services.Core.Types
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	[XmlType("RemoveDistributionGroupFromImListRequestType", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
	public class RemoveDistributionGroupFromImListRequest : BaseRequest
	{
		[DataMember(Name = "GroupId", IsRequired = true, Order = 1)]
		[XmlElement(ElementName = "GroupId")]
		public ItemId GroupId { get; set; }

		internal override ServiceCommandBase GetServiceCommand(CallContext callContext)
		{
			return new RemoveDistributionGroupFromImListCommand(callContext, this);
		}

		internal override BaseServerIdInfo GetProxyInfo(CallContext callContext)
		{
			return BaseRequest.GetServerInfoForItemId(callContext, this.GroupId);
		}

		internal override ResourceKey[] GetResources(CallContext callContext, int taskStep)
		{
			return base.GetResourceKeysForItemId(true, callContext, this.GroupId);
		}
	}
}
