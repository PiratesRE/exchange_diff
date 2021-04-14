using System;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Services.Core.Types
{
	[XmlType(TypeName = "GetEncryptionConfigurationResponseType", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
	public class GetEncryptionConfigurationResponse : ResponseMessage
	{
		public GetEncryptionConfigurationResponse()
		{
		}

		internal GetEncryptionConfigurationResponse(ServiceResultCode code, ServiceError error, GetEncryptionConfigurationResponse getEncryptionConfigurationResponse) : base(code, error)
		{
			if (getEncryptionConfigurationResponse != null)
			{
				this.ImageBase64 = getEncryptionConfigurationResponse.ImageBase64;
				this.EmailText = getEncryptionConfigurationResponse.EmailText;
				this.PortalText = getEncryptionConfigurationResponse.PortalText;
				this.DisclaimerText = getEncryptionConfigurationResponse.DisclaimerText;
				this.OTPEnabled = getEncryptionConfigurationResponse.OTPEnabled;
			}
		}

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

		public override ResponseType GetResponseType()
		{
			return ResponseType.GetEncryptionConfigurationResponseMessage;
		}
	}
}
