using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Forefront.Monitoring.ActiveMonitoring.MicrosoftOnline
{
	[DesignerCategory("code")]
	[DebuggerStepThrough]
	[GeneratedCode("wsdl", "2.0.50727.1432")]
	[XmlType(Namespace = "http://schemas.microsoft.com/online/directoryservices/change/2008/11")]
	[Serializable]
	public class SupportRoleValue
	{
		[XmlAttribute]
		public string RoleId
		{
			get
			{
				return this.roleIdField;
			}
			set
			{
				this.roleIdField = value;
			}
		}

		[XmlAttribute]
		public string ForeignPrincipalName
		{
			get
			{
				return this.foreignPrincipalNameField;
			}
			set
			{
				this.foreignPrincipalNameField = value;
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

		private string roleIdField;

		private string foreignPrincipalNameField;

		private string foreignPrincipalIdField;
	}
}
