using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using Microsoft.Exchange.Data.Directory.ResourceHealth;
using Microsoft.Exchange.UM.Rpc;

namespace Microsoft.Exchange.Services.Core.Types
{
	[XmlType("SaveUMPinType", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
	public class SaveUMPinRequest : BaseRequest
	{
		[DataMember(Name = "PinInfo")]
		[XmlElement("PinInfo")]
		public PINInfo PinInfo { get; set; }

		[DataMember(Name = "UserUMMailboxPolicyGuid")]
		[XmlElement("UserUMMailboxPolicyGuid")]
		public Guid UserUMMailboxPolicyGuid { get; set; }

		internal override ServiceCommandBase GetServiceCommand(CallContext callContext)
		{
			return new SaveUMPin(callContext, this);
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
