using System;
using System.Xml.Serialization;
using Microsoft.Exchange.Data.Directory.ResourceHealth;

namespace Microsoft.Exchange.Services.Core.Types
{
	[XmlType("SetTeamMailboxRequestType", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
	public class SetTeamMailboxRequest : BaseRequest
	{
		internal override ServiceCommandBase GetServiceCommand(CallContext callContext)
		{
			return new SetTeamMailbox(callContext, this);
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

		public string SharePointSiteUrl;

		public TeamMailboxLifecycleState State;
	}
}
