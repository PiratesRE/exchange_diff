using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using Microsoft.Exchange.Data.Directory.ResourceHealth;

namespace Microsoft.Exchange.Services.Core.Types
{
	[XmlType("GetPeopleConnectStateType", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
	public sealed class GetPeopleConnectStateRequest : BaseRequest
	{
		[XmlElement]
		[DataMember(Name = "Provider", IsRequired = true)]
		public string Provider { get; set; }

		internal override ServiceCommandBase GetServiceCommand(CallContext callContext)
		{
			return new GetPeopleConnectState(callContext, this);
		}

		internal override BaseServerIdInfo GetProxyInfo(CallContext callContext)
		{
			return null;
		}

		internal override ResourceKey[] GetResources(CallContext callContext, int taskStep)
		{
			return base.GetResourceKeysFromProxyInfo(false, callContext);
		}
	}
}
