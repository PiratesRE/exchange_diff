using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using Microsoft.Exchange.Data.Directory.ResourceHealth;

namespace Microsoft.Exchange.Services.Core.Types
{
	[XmlType("GetUMSubscriberCallAnsweringDataType", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
	public class GetUMSubscriberCallAnsweringDataRequest : BaseRequest
	{
		[XmlElement("Timeout")]
		[DataMember(Name = "Timeout")]
		public string Timeout { get; set; }

		internal override ServiceCommandBase GetServiceCommand(CallContext callContext)
		{
			return new GetUMSubscriberCallAnsweringData(callContext, this);
		}

		internal override BaseServerIdInfo GetProxyInfo(CallContext callContext)
		{
			return null;
		}

		internal override ResourceKey[] GetResources(CallContext callContext, int taskStep)
		{
			return null;
		}
	}
}
