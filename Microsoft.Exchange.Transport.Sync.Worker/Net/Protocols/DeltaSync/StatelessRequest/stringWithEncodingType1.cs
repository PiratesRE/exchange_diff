using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Net.Protocols.DeltaSync.StatelessRequest
{
	[XmlRoot("ConversationTopic", Namespace = "HMMAIL:", IsNullable = false)]
	[DebuggerStepThrough]
	[XmlType(TypeName = "stringWithEncodingType", Namespace = "HMMAIL:")]
	[GeneratedCode("xsd", "2.0.50727.3038")]
	[DesignerCategory("code")]
	[Serializable]
	public class stringWithEncodingType1
	{
		public stringWithEncodingType1()
		{
			this.encodingField = "0";
		}

		[XmlAttribute]
		[DefaultValue("0")]
		public string encoding
		{
			get
			{
				return this.encodingField;
			}
			set
			{
				this.encodingField = value;
			}
		}

		[XmlText]
		public string Value
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

		private string encodingField;

		private string valueField;
	}
}
