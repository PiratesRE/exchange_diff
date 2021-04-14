using System;
using System.Xml.Serialization;
using Microsoft.Exchange.Data.Directory.ResourceHealth;

namespace Microsoft.Exchange.Services.Core.Types
{
	[XmlType("UnpinTeamMailboxRequestType", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
	public class UnpinTeamMailboxRequest : BaseRequest
	{
		internal override ServiceCommandBase GetServiceCommand(CallContext callContext)
		{
			return new UnpinTeamMailbox(callContext, this);
		}

		internal override ResourceKey[] GetResources(CallContext callContext, int taskStep)
		{
			return base.GetResourceKeysFromProxyInfo(true, callContext);
		}

		internal override BaseServerIdInfo GetProxyInfo(CallContext callContext)
		{
			return IdConverter.GetServerInfoForCallContext(callContext);
		}

		public EmailAddressWrapper EmailAddress;
	}
}
