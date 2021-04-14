using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Data.ApplicationLogic.TextMessaging
{
	[XmlType(AnonymousType = true)]
	[GeneratedCode("xsd", "4.0.30319.17627")]
	[DesignerCategory("code")]
	[DebuggerStepThrough]
	[Serializable]
	public class TextMessagingHostingDataServicesService
	{
		[XmlElement(Form = XmlSchemaForm.Unqualified)]
		public string RegionIso2
		{
			get
			{
				return this.regionIso2Field;
			}
			set
			{
				this.regionIso2Field = value;
			}
		}

		[XmlElement(Form = XmlSchemaForm.Unqualified)]
		public int CarrierIdentity
		{
			get
			{
				return this.carrierIdentityField;
			}
			set
			{
				this.carrierIdentityField = value;
			}
		}

		[XmlElement(Form = XmlSchemaForm.Unqualified)]
		public TextMessagingHostingDataServicesServiceType Type
		{
			get
			{
				return this.typeField;
			}
			set
			{
				this.typeField = value;
			}
		}

		[XmlElement(Form = XmlSchemaForm.Unqualified)]
		public TextMessagingHostingDataServicesServiceVoiceCallForwarding VoiceCallForwarding
		{
			get
			{
				return this.voiceCallForwardingField;
			}
			set
			{
				this.voiceCallForwardingField = value;
			}
		}

		[XmlElement(Form = XmlSchemaForm.Unqualified)]
		public TextMessagingHostingDataServicesServiceSmtpToSmsGateway SmtpToSmsGateway
		{
			get
			{
				return this.smtpToSmsGatewayField;
			}
			set
			{
				this.smtpToSmsGatewayField = value;
			}
		}

		private string regionIso2Field;

		private int carrierIdentityField;

		private TextMessagingHostingDataServicesServiceType typeField;

		private TextMessagingHostingDataServicesServiceVoiceCallForwarding voiceCallForwardingField;

		private TextMessagingHostingDataServicesServiceSmtpToSmsGateway smtpToSmsGatewayField;
	}
}
