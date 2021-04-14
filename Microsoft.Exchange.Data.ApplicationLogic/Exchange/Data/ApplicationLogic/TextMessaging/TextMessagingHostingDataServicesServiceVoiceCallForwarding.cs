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
	[DebuggerStepThrough]
	[DesignerCategory("code")]
	[Serializable]
	public class TextMessagingHostingDataServicesServiceVoiceCallForwarding
	{
		[XmlElement(Form = XmlSchemaForm.Unqualified)]
		public string Enable
		{
			get
			{
				return this.enableField;
			}
			set
			{
				this.enableField = value;
			}
		}

		[XmlElement(Form = XmlSchemaForm.Unqualified)]
		public string Disable
		{
			get
			{
				return this.disableField;
			}
			set
			{
				this.disableField = value;
			}
		}

		[XmlAttribute]
		public TextMessagingHostingDataServicesServiceVoiceCallForwardingType Type
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

		[XmlIgnore]
		public bool TypeSpecified
		{
			get
			{
				return this.typeFieldSpecified;
			}
			set
			{
				this.typeFieldSpecified = value;
			}
		}

		private string enableField;

		private string disableField;

		private TextMessagingHostingDataServicesServiceVoiceCallForwardingType typeField;

		private bool typeFieldSpecified;
	}
}
