using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using Microsoft.Exchange.Data.Directory.ResourceHealth;

namespace Microsoft.Exchange.Services.Core.Types
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	[XmlType("GetImItemsRequestType", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
	public class GetImItemsRequest : BaseRequest
	{
		internal override ServiceCommandBase GetServiceCommand(CallContext callContext)
		{
			return new GetImItemsCommand(callContext, this);
		}

		internal override BaseServerIdInfo GetProxyInfo(CallContext callContext)
		{
			if (this.ContactIds != null)
			{
				return BaseRequest.GetServerInfoForItemIdList(callContext, this.ContactIds);
			}
			if (this.GroupIds != null)
			{
				return BaseRequest.GetServerInfoForItemIdList(callContext, this.GroupIds);
			}
			return IdConverter.GetServerInfoForCallContext(callContext);
		}

		internal override ResourceKey[] GetResources(CallContext callContext, int taskStep)
		{
			return base.GetResourceKeysFromProxyInfo(false, callContext);
		}

		[DataMember(Name = "ContactIds", IsRequired = false, Order = 1)]
		[XmlArray]
		[XmlArrayItem("ItemId", typeof(ItemId), Namespace = "http://schemas.microsoft.com/exchange/services/2006/types", IsNullable = false)]
		public ItemId[] ContactIds;

		[XmlArrayItem("ItemId", typeof(ItemId), Namespace = "http://schemas.microsoft.com/exchange/services/2006/types", IsNullable = false)]
		[XmlArray]
		[DataMember(Name = "GroupIds", IsRequired = false, Order = 2)]
		public ItemId[] GroupIds;

		[XmlArray]
		[DataMember(Name = "ExtendedProperties", IsRequired = false, Order = 3)]
		[XmlArrayItem("ExtendedProperty", typeof(ExtendedPropertyUri), Namespace = "http://schemas.microsoft.com/exchange/services/2006/types", IsNullable = false)]
		public ExtendedPropertyUri[] ExtendedProperties;
	}
}
