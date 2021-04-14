using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Web.Services.Protocols;
using System.Xml.Serialization;

namespace Microsoft.Exchange.EDiscovery.Export.EwsProxy
{
	[GeneratedCode("wsdl", "4.0.30319.17627")]
	[XmlRoot("ManagementRole", Namespace = "http://schemas.microsoft.com/exchange/services/2006/types", IsNullable = false)]
	[DebuggerStepThrough]
	[DesignerCategory("code")]
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[Serializable]
	public class ManagementRoleType : SoapHeader
	{
		[XmlArrayItem("Role", IsNullable = false)]
		public string[] UserRoles
		{
			get
			{
				return this.userRolesField;
			}
			set
			{
				this.userRolesField = value;
			}
		}

		[XmlArrayItem("Role", IsNullable = false)]
		public string[] ApplicationRoles
		{
			get
			{
				return this.applicationRolesField;
			}
			set
			{
				this.applicationRolesField = value;
			}
		}

		private string[] userRolesField;

		private string[] applicationRolesField;
	}
}
