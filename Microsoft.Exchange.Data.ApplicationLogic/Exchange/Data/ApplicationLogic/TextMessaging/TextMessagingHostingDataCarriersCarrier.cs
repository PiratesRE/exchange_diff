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
	[GeneratedCode("xsd", "4.0.30319.17627")]
	[XmlType(AnonymousType = true)]
	[Serializable]
	public class TextMessagingHostingDataCarriersCarrier
	{
		[XmlElement("LocalizedInfo", Form = XmlSchemaForm.Unqualified)]
		public TextMessagingHostingDataCarriersCarrierLocalizedInfo[] LocalizedInfo
		{
			get
			{
				return this.localizedInfoField;
			}
			set
			{
				this.localizedInfoField = value;
			}
		}

		[XmlAttribute]
		public int Identity
		{
			get
			{
				return this.identityField;
			}
			set
			{
				this.identityField = value;
			}
		}

		private TextMessagingHostingDataCarriersCarrierLocalizedInfo[] localizedInfoField;

		private int identityField;
	}
}
