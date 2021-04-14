using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using Microsoft.Exchange.Data.Directory.ResourceHealth;

namespace Microsoft.Exchange.Services.Core.Types
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	[XmlType("AddNewImContactToGroupRequestType", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
	public class AddNewImContactToGroupRequest : BaseRequest
	{
		[XmlElement(ElementName = "ImAddress")]
		[DataMember(Name = "ImAddress", IsRequired = true, Order = 1)]
		public string ImAddress { get; set; }

		[XmlElement(ElementName = "DisplayName")]
		[DataMember(Name = "DisplayName", IsRequired = false, Order = 2)]
		public string DisplayName { get; set; }

		[XmlElement(ElementName = "GroupId")]
		[DataMember(Name = "GroupId", IsRequired = false, Order = 3)]
		public ItemId GroupId { get; set; }

		internal override ServiceCommandBase GetServiceCommand(CallContext callContext)
		{
			return new AddNewImContactToGroupCommand(callContext, this);
		}

		internal override BaseServerIdInfo GetProxyInfo(CallContext callContext)
		{
			if (this.GroupId != null)
			{
				return BaseRequest.GetServerInfoForItemId(callContext, this.GroupId);
			}
			return IdConverter.GetServerInfoForCallContext(callContext);
		}

		internal override ResourceKey[] GetResources(CallContext callContext, int taskStep)
		{
			return base.GetResourceKeysFromProxyInfo(true, callContext);
		}
	}
}
