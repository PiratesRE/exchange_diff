using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Exchange.EDiscovery.Export.EwsProxy
{
	[DesignerCategory("code")]
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
	[GeneratedCode("wsdl", "4.0.30319.17627")]
	[DebuggerStepThrough]
	[Serializable]
	public class CompleteFindInGALSpeechRecognitionResponseMessageType : ResponseMessageType
	{
		public RecognitionResultType RecognitionResult
		{
			get
			{
				return this.recognitionResultField;
			}
			set
			{
				this.recognitionResultField = value;
			}
		}

		private RecognitionResultType recognitionResultField;
	}
}
