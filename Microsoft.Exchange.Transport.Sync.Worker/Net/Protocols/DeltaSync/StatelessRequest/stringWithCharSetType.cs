using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Net.Protocols.DeltaSync.StatelessRequest
{
	[DebuggerStepThrough]
	[XmlType(Namespace = "HMMAIL:")]
	[XmlRoot("Categories", Namespace = "HMMAIL:", IsNullable = false)]
	[GeneratedCode("xsd", "2.0.50727.3038")]
	[DesignerCategory("code")]
	[Serializable]
	public class stringWithCharSetType
	{
		public stringWithCharSetType()
		{
			this.encodingField = "0";
		}

		[XmlAttribute]
		public string charset
		{
			get
			{
				return this.charsetField;
			}
			set
			{
				this.charsetField = value;
			}
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

		private string charsetField;

		private string encodingField;

		private string valueField;
	}
}
