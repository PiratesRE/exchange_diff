using System;
using System.Xml.Serialization;
using Microsoft.Exchange.Data.Directory.ResourceHealth;

namespace Microsoft.Exchange.Services.Core.Types
{
	[XmlType("SetEncryptionConfigurationRequestType", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
	[Serializable]
	public class SetEncryptionConfigurationRequest : BaseRequest
	{
		[XmlElement(ElementName = "ImageBase64", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
		public string ImageBase64 { get; set; }

		[XmlElement(ElementName = "EmailText", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
		public string EmailText { get; set; }

		[XmlElement(ElementName = "PortalText", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
		public string PortalText { get; set; }

		[XmlElement(ElementName = "DisclaimerText", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
		public string DisclaimerText { get; set; }

		[XmlElement(ElementName = "OTPEnabled", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
		public bool OTPEnabled { get; set; }

		internal override ServiceCommandBase GetServiceCommand(CallContext callContext)
		{
			return new SetEncryptionConfiguration(callContext, this);
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
