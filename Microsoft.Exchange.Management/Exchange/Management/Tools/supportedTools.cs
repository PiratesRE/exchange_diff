using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Management.Tools
{
	[DebuggerStepThrough]
	[DesignerCategory("code")]
	[GeneratedCode("xsd", "4.0.30319.17627")]
	[XmlType(AnonymousType = true)]
	[XmlRoot(Namespace = "", IsNullable = false)]
	[Serializable]
	public class supportedTools
	{
		[XmlElement("toolInformation", Form = XmlSchemaForm.Unqualified)]
		public ToolInfoData[] toolInformation
		{
			get
			{
				return this.toolInformationField;
			}
			set
			{
				this.toolInformationField = value;
			}
		}

		private ToolInfoData[] toolInformationField;
	}
}
