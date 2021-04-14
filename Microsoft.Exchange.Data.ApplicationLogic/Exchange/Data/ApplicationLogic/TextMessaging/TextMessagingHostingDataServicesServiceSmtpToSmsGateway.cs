using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Data.ApplicationLogic.TextMessaging
{
	[DebuggerStepThrough]
	[DesignerCategory("code")]
	[XmlType(AnonymousType = true)]
	[GeneratedCode("xsd", "4.0.30319.17627")]
	[Serializable]
	public class TextMessagingHostingDataServicesServiceSmtpToSmsGateway
	{
		[XmlElement(Form = XmlSchemaForm.Unqualified)]
		public TextMessagingHostingDataServicesServiceSmtpToSmsGatewayRecipientAddressing RecipientAddressing
		{
			get
			{
				return this.recipientAddressingField;
			}
			set
			{
				this.recipientAddressingField = value;
			}
		}

		[XmlElement(Form = XmlSchemaForm.Unqualified)]
		public TextMessagingHostingDataServicesServiceSmtpToSmsGatewayMessageRendering MessageRendering
		{
			get
			{
				return this.messageRenderingField;
			}
			set
			{
				this.messageRenderingField = value;
			}
		}

		private TextMessagingHostingDataServicesServiceSmtpToSmsGatewayRecipientAddressing recipientAddressingField;

		private TextMessagingHostingDataServicesServiceSmtpToSmsGatewayMessageRendering messageRenderingField;
	}
}
