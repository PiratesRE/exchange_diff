﻿using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Exchange.SoapWebClient.EWS
{
	[GeneratedCode("wsdl", "4.0.30319.17627")]
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
	[DebuggerStepThrough]
	[DesignerCategory("code")]
	[Serializable]
	public class EncryptionConfigurationResponseType : ResponseMessageType
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