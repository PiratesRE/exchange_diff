using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Exchange.SoapWebClient.EWS
{
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
	[DesignerCategory("code")]
	[GeneratedCode("wsdl", "4.0.30319.17627")]
	[DebuggerStepThrough]
	[Serializable]
	public class SetEncryptionConfigurationType : BaseRequestType
	{
		public string ImageBase64;

		public string EmailText;

		public string PortalText;

		public string DisclaimerText;

		public bool OTPEnabled;

		[XmlIgnore]
		public bool OTPEnabledSpecified;
	}
}
