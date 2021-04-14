using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using Microsoft.Exchange.Data.Directory.ResourceHealth;

namespace Microsoft.Exchange.Services.Core.Types
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	[XmlType("AddImGroupRequestType", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
	public class AddImGroupRequest : BaseRequest
	{
		[XmlElement(ElementName = "DisplayName")]
		[DataMember(Name = "DisplayName", IsRequired = true, Order = 1)]
		public string DisplayName { get; set; }

		internal override ServiceCommandBase GetServiceCommand(CallContext callContext)
		{
			return new AddImGroupCommand(callContext, this);
		}

		internal override BaseServerIdInfo GetProxyInfo(CallContext callContext)
		{
			return IdConverter.GetServerInfoForCallContext(callContext);
		}

		internal override ResourceKey[] GetResources(CallContext callContext, int taskStep)
		{
			return base.GetResourceKeysFromProxyInfo(true, callContext);
		}
	}
}
