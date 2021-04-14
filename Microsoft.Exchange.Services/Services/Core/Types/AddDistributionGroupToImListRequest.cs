using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using Microsoft.Exchange.Data.Directory.ResourceHealth;

namespace Microsoft.Exchange.Services.Core.Types
{
	[XmlType("AddDistributionGroupToImListRequestType", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	public class AddDistributionGroupToImListRequest : BaseRequest
	{
		[XmlElement(ElementName = "SmtpAddress")]
		[DataMember(Name = "SmtpAddress", IsRequired = true, Order = 1)]
		public string SmtpAddress { get; set; }

		[XmlElement(ElementName = "DisplayName")]
		[DataMember(Name = "DisplayName", IsRequired = true, Order = 2)]
		public string DisplayName { get; set; }

		internal override ServiceCommandBase GetServiceCommand(CallContext callContext)
		{
			return new AddDistributionGroupToImListCommand(callContext, this);
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
