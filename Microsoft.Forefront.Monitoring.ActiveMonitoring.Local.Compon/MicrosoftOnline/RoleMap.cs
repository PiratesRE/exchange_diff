using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Forefront.Monitoring.ActiveMonitoring.MicrosoftOnline
{
	[XmlInclude(typeof(PartnerRoleMap))]
	[GeneratedCode("wsdl", "2.0.50727.1432")]
	[DebuggerStepThrough]
	[DesignerCategory("code")]
	[XmlType(Namespace = "http://www.ccs.com/TestServices/")]
	[Serializable]
	public class RoleMap
	{
		public string PartnerGroup
		{
			get
			{
				return this.partnerGroupField;
			}
			set
			{
				this.partnerGroupField = value;
			}
		}

		public string OnBehalfRole
		{
			get
			{
				return this.onBehalfRoleField;
			}
			set
			{
				this.onBehalfRoleField = value;
			}
		}

		private string partnerGroupField;

		private string onBehalfRoleField;
	}
}
