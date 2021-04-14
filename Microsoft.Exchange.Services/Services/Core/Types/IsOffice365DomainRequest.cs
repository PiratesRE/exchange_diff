using System;
using System.Xml.Serialization;
using Microsoft.Exchange.Data.Directory.ResourceHealth;

namespace Microsoft.Exchange.Services.Core.Types
{
	[XmlType("IsOffice365DomainRequest", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
	public class IsOffice365DomainRequest : BaseRequest
	{
		internal override BaseServerIdInfo GetProxyInfo(CallContext callContext)
		{
			return null;
		}

		internal override ResourceKey[] GetResources(CallContext callContext, int taskStep)
		{
			return null;
		}

		[XmlElement]
		public string EmailAddress { get; set; }

		internal override ServiceCommandBase GetServiceCommand(CallContext callContext)
		{
			return new IsOffice365Domain(callContext, this);
		}
	}
}
