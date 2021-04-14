using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Forefront.Monitoring.ActiveMonitoring.MicrosoftOnline
{
	[DebuggerStepThrough]
	[DesignerCategory("code")]
	[XmlType(Namespace = "http://schemas.microsoft.com/online/directoryservices/change/2008/11")]
	[GeneratedCode("wsdl", "2.0.50727.1432")]
	[Serializable]
	public class XmlValueSupportRole
	{
		public SupportRoleValue SupportRole
		{
			get
			{
				return this.supportRoleField;
			}
			set
			{
				this.supportRoleField = value;
			}
		}

		private SupportRoleValue supportRoleField;
	}
}
