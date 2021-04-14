using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Data.ApplicationLogic.TextMessaging
{
	[XmlType(AnonymousType = true)]
	[DesignerCategory("code")]
	[GeneratedCode("xsd", "4.0.30319.17627")]
	[DebuggerStepThrough]
	[Serializable]
	public class TextMessagingHostingDataCarriers
	{
		[XmlElement("Carrier", Form = XmlSchemaForm.Unqualified)]
		public TextMessagingHostingDataCarriersCarrier[] Carrier
		{
			get
			{
				return this.carrierField;
			}
			set
			{
				this.carrierField = value;
			}
		}

		private TextMessagingHostingDataCarriersCarrier[] carrierField;
	}
}
