using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Exchange.SoapWebClient.EWS
{
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
	[DebuggerStepThrough]
	[DesignerCategory("code")]
	[GeneratedCode("wsdl", "4.0.30319.17627")]
	[Serializable]
	public class GetUMSubscriberCallAnsweringDataResponseMessageType : ResponseMessageType
	{
		public bool IsOOF;

		public UMMailboxTranscriptionEnabledSetting IsTranscriptionEnabledInMailboxConfig;

		public bool IsMailboxQuotaExceeded;

		[XmlElement(DataType = "base64Binary")]
		public byte[] Greeting;

		public string GreetingName;

		public bool TaskTimedOut;
	}
}
