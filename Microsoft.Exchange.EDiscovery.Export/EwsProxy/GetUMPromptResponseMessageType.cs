using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Exchange.EDiscovery.Export.EwsProxy
{
	[DebuggerStepThrough]
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
	[GeneratedCode("wsdl", "4.0.30319.17627")]
	[DesignerCategory("code")]
	[Serializable]
	public class GetUMPromptResponseMessageType : ResponseMessageType
	{
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

		private byte[] audioDataField;
	}
}
