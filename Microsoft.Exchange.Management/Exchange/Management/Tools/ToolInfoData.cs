using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Management.Tools
{
	[DesignerCategory("code")]
	[GeneratedCode("xsd", "4.0.30319.17627")]
	[DebuggerStepThrough]
	[Serializable]
	public class ToolInfoData
	{
		[XmlElement(Form = XmlSchemaForm.Unqualified)]
		public SupportedVersion defaultSupportedVersion
		{
			get
			{
				return this.defaultSupportedVersionField;
			}
			set
			{
				this.defaultSupportedVersionField = value;
			}
		}

		[XmlElement("customSupportedVersion", Form = XmlSchemaForm.Unqualified)]
		public CustomSupportedVersion[] customSupportedVersion
		{
			get
			{
				return this.customSupportedVersionField;
			}
			set
			{
				this.customSupportedVersionField = value;
			}
		}

		[XmlAttribute]
		public ToolId id
		{
			get
			{
				return this.idField;
			}
			set
			{
				this.idField = value;
			}
		}

		private SupportedVersion defaultSupportedVersionField;

		private CustomSupportedVersion[] customSupportedVersionField;

		private ToolId idField;
	}
}
