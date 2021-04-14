using System;
using System.Xml.Serialization;
using Microsoft.Exchange.Data.Directory.ResourceHealth;

namespace Microsoft.Exchange.Services.Core.Types
{
	[XmlType("GetClientExtensionRequestType", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
	[Serializable]
	public class GetClientExtensionRequest : BaseRequest
	{
		[XmlArrayItem("String", IsNullable = false, Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
		public string[] RequestedExtensionIds { get; set; }

		[XmlElement]
		public ClientExtensionUserParameters UserParameters { get; set; }

		[XmlElement]
		public bool IsDebug { get; set; }

		internal override ServiceCommandBase GetServiceCommand(CallContext callContext)
		{
			return new GetClientExtension(callContext, this);
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
