using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Exchange.EDiscovery.Export.EwsProxy
{
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[GeneratedCode("wsdl", "4.0.30319.17627")]
	[DebuggerStepThrough]
	[DesignerCategory("code")]
	[Serializable]
	public class ImItemListType
	{
		[XmlArrayItem("ImGroup", IsNullable = false)]
		public ImGroupType[] Groups
		{
			get
			{
				return this.groupsField;
			}
			set
			{
				this.groupsField = value;
			}
		}

		[XmlArrayItem("Persona", IsNullable = false)]
		public PersonaType[] Personas
		{
			get
			{
				return this.personasField;
			}
			set
			{
				this.personasField = value;
			}
		}

		private ImGroupType[] groupsField;

		private PersonaType[] personasField;
	}
}
