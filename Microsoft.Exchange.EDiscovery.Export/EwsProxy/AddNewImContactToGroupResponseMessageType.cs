using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Exchange.EDiscovery.Export.EwsProxy
{
	[GeneratedCode("wsdl", "4.0.30319.17627")]
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
	[DebuggerStepThrough]
	[DesignerCategory("code")]
	[Serializable]
	public class AddNewImContactToGroupResponseMessageType : ResponseMessageType
	{
		public PersonaType Persona
		{
			get
			{
				return this.personaField;
			}
			set
			{
				this.personaField = value;
			}
		}

		private PersonaType personaField;
	}
}
