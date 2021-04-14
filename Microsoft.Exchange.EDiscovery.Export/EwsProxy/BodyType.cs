using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Exchange.EDiscovery.Export.EwsProxy
{
	[DesignerCategory("code")]
	[DebuggerStepThrough]
	[GeneratedCode("wsdl", "4.0.30319.17627")]
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[Serializable]
	public class BodyType
	{
		[XmlAttribute("BodyType")]
		public BodyTypeType BodyType1
		{
			get
			{
				return this.bodyType1Field;
			}
			set
			{
				this.bodyType1Field = value;
			}
		}

		[XmlAttribute]
		public bool IsTruncated
		{
			get
			{
				return this.isTruncatedField;
			}
			set
			{
				this.isTruncatedField = value;
			}
		}

		[XmlIgnore]
		public bool IsTruncatedSpecified
		{
			get
			{
				return this.isTruncatedFieldSpecified;
			}
			set
			{
				this.isTruncatedFieldSpecified = value;
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

		private BodyTypeType bodyType1Field;

		private bool isTruncatedField;

		private bool isTruncatedFieldSpecified;

		private string valueField;
	}
}
