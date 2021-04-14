using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using Microsoft.Exchange.Data.Directory.ResourceHealth;

namespace Microsoft.Exchange.Services.Core.Types
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	[XmlType("GetImItemListRequestType", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
	public class GetImItemListRequest : BaseRequest
	{
		internal override ServiceCommandBase GetServiceCommand(CallContext callContext)
		{
			return new GetImItemListCommand(callContext, this);
		}

		internal override BaseServerIdInfo GetProxyInfo(CallContext callContext)
		{
			return IdConverter.GetServerInfoForCallContext(callContext);
		}

		internal override ResourceKey[] GetResources(CallContext callContext, int taskStep)
		{
			return base.GetResourceKeysFromProxyInfo(false, callContext);
		}

		[DataMember(Name = "ExtendedProperties", IsRequired = false, Order = 1)]
		[XmlArray]
		[XmlArrayItem("ExtendedProperty", typeof(ExtendedPropertyUri), Namespace = "http://schemas.microsoft.com/exchange/services/2006/types", IsNullable = false)]
		public ExtendedPropertyUri[] ExtendedProperties;
	}
}
