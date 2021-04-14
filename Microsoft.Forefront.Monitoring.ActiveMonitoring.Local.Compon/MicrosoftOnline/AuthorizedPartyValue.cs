using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Forefront.Monitoring.ActiveMonitoring.MicrosoftOnline
{
	[XmlType(Namespace = "http://schemas.microsoft.com/online/directoryservices/change/2008/11")]
	[DesignerCategory("code")]
	[GeneratedCode("wsdl", "2.0.50727.1432")]
	[DebuggerStepThrough]
	[Serializable]
	public class AuthorizedPartyValue
	{
		[XmlAttribute]
		public string ForeignContextId
		{
			get
			{
				return this.foreignContextIdField;
			}
			set
			{
				this.foreignContextIdField = value;
			}
		}

		[XmlAttribute]
		public string ForeignPrincipalId
		{
			get
			{
				return this.foreignPrincipalIdField;
			}
			set
			{
				this.foreignPrincipalIdField = value;
			}
		}

		private string foreignContextIdField;

		private string foreignPrincipalIdField;
	}
}
