using System;
using System.Xml.Serialization;
using Microsoft.Exchange.Data.Directory.ResourceHealth;
using Microsoft.Exchange.SoapWebClient.EWS;

namespace Microsoft.Exchange.Services.Core.Types
{
	[XmlType(TypeName = "UpdateMailboxAssociationType", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
	[Serializable]
	public class UpdateMailboxAssociationRequest : BaseRequest
	{
		[XmlElement("Association")]
		public MailboxAssociationType Association { get; set; }

		[XmlElement("Master")]
		public MasterMailboxType Master { get; set; }

		internal override ServiceCommandBase GetServiceCommand(CallContext callContext)
		{
			return new UpdateMailboxAssociation(callContext, this);
		}

		internal override BaseServerIdInfo GetProxyInfo(CallContext callContext)
		{
			return IdConverter.GetServerInfoForCallContext(callContext);
		}

		internal override ResourceKey[] GetResources(CallContext callContext, int currentStep)
		{
			return base.GetResourceKeysFromProxyInfo(true, callContext);
		}
	}
}
