using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Exchange.EDiscovery.Export.EwsProxy
{
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
	[GeneratedCode("wsdl", "4.0.30319.17627")]
	[DebuggerStepThrough]
	[DesignerCategory("code")]
	[Serializable]
	public class GetPersonaType : BaseRequestType
	{
		public ItemIdType PersonaId
		{
			get
			{
				return this.personaIdField;
			}
			set
			{
				this.personaIdField = value;
			}
		}

		public EmailAddressType EmailAddress
		{
			get
			{
				return this.emailAddressField;
			}
			set
			{
				this.emailAddressField = value;
			}
		}

		private ItemIdType personaIdField;

		private EmailAddressType emailAddressField;
	}
}
