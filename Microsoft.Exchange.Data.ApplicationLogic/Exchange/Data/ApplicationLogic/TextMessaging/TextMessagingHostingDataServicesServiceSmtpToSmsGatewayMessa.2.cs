using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Data.ApplicationLogic.TextMessaging
{
	[GeneratedCode("xsd", "4.0.30319.17627")]
	[DesignerCategory("code")]
	[XmlType(AnonymousType = true)]
	[DebuggerStepThrough]
	[Serializable]
	public class TextMessagingHostingDataServicesServiceSmtpToSmsGatewayMessageRenderingCapacity
	{
		[XmlAttribute]
		public TextMessagingHostingDataServicesServiceSmtpToSmsGatewayMessageRenderingCapacityCodingScheme CodingScheme
		{
			get
			{
				return this.codingSchemeField;
			}
			set
			{
				this.codingSchemeField = value;
			}
		}

		[XmlText]
		public int Value
		{
			get
			{
				return this.valueField;
			}
			set
			{
				this.valueField = value;
			}
		}

		private TextMessagingHostingDataServicesServiceSmtpToSmsGatewayMessageRenderingCapacityCodingScheme codingSchemeField;

		private int valueField;
	}
}
