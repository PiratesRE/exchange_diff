using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using Microsoft.Exchange.Data.Directory.ResourceHealth;

namespace Microsoft.Exchange.Services.Core.Types
{
	[XmlType("AddNewTelUriContactToGroupRequestType", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	public class AddNewTelUriContactToGroupRequest : BaseRequest
	{
		[DataMember(Name = "TelUriAddress", IsRequired = true, Order = 1)]
		[XmlElement(ElementName = "TelUriAddress")]
		public string TelUriAddress { get; set; }

		[XmlElement(ElementName = "ImContactSipUriAddress")]
		[DataMember(Name = "ImContactSipUriAddress", IsRequired = true, Order = 2)]
		public string ImContactSipUriAddress { get; set; }

		[XmlElement(ElementName = "ImTelephoneNumber")]
		[DataMember(Name = "ImTelephoneNumber", IsRequired = false, Order = 3)]
		public string ImTelephoneNumber { get; set; }

		[DataMember(Name = "GroupId", IsRequired = false, Order = 4)]
		[XmlElement(ElementName = "GroupId")]
		public ItemId GroupId { get; set; }

		internal override ServiceCommandBase GetServiceCommand(CallContext callContext)
		{
			return new AddNewTelUriContactToGroupCommand(callContext, this);
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
