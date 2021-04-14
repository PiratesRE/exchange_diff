using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Forefront.Monitoring.ActiveMonitoring.MicrosoftOnline
{
	[GeneratedCode("wsdl", "2.0.50727.1432")]
	[DebuggerStepThrough]
	[DesignerCategory("code")]
	[XmlType(Namespace = "http://schemas.microsoft.com/online/directoryservices/change/2008/11")]
	[Serializable]
	public class XmlValueCompanyPartnership
	{
		[XmlArrayItem("Partnership", IsNullable = false)]
		public PartnershipValue[] Partnerships
		{
			get
			{
				return this.partnershipsField;
			}
			set
			{
				this.partnershipsField = value;
			}
		}

		private PartnershipValue[] partnershipsField;
	}
}
