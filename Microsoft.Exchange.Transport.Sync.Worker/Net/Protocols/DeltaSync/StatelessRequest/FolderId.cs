using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Net.Protocols.DeltaSync.StatelessRequest
{
	[GeneratedCode("xsd", "2.0.50727.3038")]
	[DebuggerStepThrough]
	[XmlType(AnonymousType = true, Namespace = "HMMAIL:")]
	[XmlRoot(Namespace = "HMMAIL:", IsNullable = false)]
	[DesignerCategory("code")]
	[Serializable]
	public class FolderId
	{
		public FolderId()
		{
			this.isClientIdField = 0;
		}

		[DefaultValue(typeof(byte), "0")]
		[XmlAttribute]
		public byte isClientId
		{
			get
			{
				return this.isClientIdField;
			}
			set
			{
				this.isClientIdField = value;
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

		private byte isClientIdField;

		private string valueField;
	}
}
