using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using Microsoft.Exchange.Data.Directory.ResourceHealth;

namespace Microsoft.Exchange.Services.Core.Types
{
	[XmlType("RemoveContactFromImListRequestType", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	public class RemoveContactFromImListRequest : BaseRequest
	{
		[XmlElement(ElementName = "ContactId")]
		[DataMember(Name = "ContactId", IsRequired = true, Order = 1)]
		public ItemId ContactId { get; set; }

		internal override ServiceCommandBase GetServiceCommand(CallContext callContext)
		{
			return new RemoveContactFromImListCommand(callContext, this);
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
