using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using Microsoft.Exchange.Data.Directory.ResourceHealth;

namespace Microsoft.Exchange.Services.Core.Types
{
	[XmlType("RemoveImGroupRequestType", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	public class RemoveImGroupRequest : BaseRequest
	{
		[DataMember(Name = "GroupId", IsRequired = true, Order = 1)]
		[XmlElement(ElementName = "GroupId")]
		public ItemId GroupId { get; set; }

		internal override ServiceCommandBase GetServiceCommand(CallContext callContext)
		{
			return new RemoveImGroupCommand(callContext, this);
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
