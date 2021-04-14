using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using Microsoft.Exchange.Data.Directory.ResourceHealth;

namespace Microsoft.Exchange.Services.Core.Types
{
	[XmlType("AddImContactToGroupRequestType", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	public class AddImContactToGroupRequest : BaseRequest
	{
		[XmlElement(ElementName = "ContactId")]
		[DataMember(Name = "ContactId", IsRequired = true, Order = 1)]
		public ItemId ContactId { get; set; }

		[XmlElement(ElementName = "GroupId")]
		[DataMember(Name = "GroupId", IsRequired = false, Order = 2)]
		public ItemId GroupId { get; set; }

		internal override ServiceCommandBase GetServiceCommand(CallContext callContext)
		{
			return new AddImContactToGroupCommand(callContext, this);
		}

		internal override BaseServerIdInfo GetProxyInfo(CallContext callContext)
		{
			return BaseRequest.GetServerInfoForItemId(callContext, this.ContactId);
		}

		internal override ResourceKey[] GetResources(CallContext callContext, int taskStep)
		{
			return base.GetResourceKeysForItemId(true, callContext, this.ContactId);
		}
	}
}
