using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Exchange.EDiscovery.Export.EwsProxy
{
	[GeneratedCode("wsdl", "4.0.30319.17627")]
	[DesignerCategory("code")]
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
	[DebuggerStepThrough]
	[Serializable]
	public class CompleteFindInGALSpeechRecognitionType : BaseRequestType
	{
		public RecognitionIdType RecognitionId
		{
			get
			{
				return this.recognitionIdField;
			}
			set
			{
				this.recognitionIdField = value;
			}
		}

		[XmlElement(DataType = "base64Binary")]
		public byte[] AudioData
		{
			get
			{
				return this.audioDataField;
			}
			set
			{
				this.audioDataField = value;
			}
		}

		private RecognitionIdType recognitionIdField;

		private byte[] audioDataField;
	}
}
