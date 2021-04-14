using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Forefront.Monitoring.ActiveMonitoring.MicrosoftOnline
{
	[GeneratedCode("wsdl", "2.0.50727.1432")]
	[DesignerCategory("code")]
	[XmlType(Namespace = "http://schemas.microsoft.com/online/directoryservices/change/2008/11")]
	[DebuggerStepThrough]
	[XmlInclude(typeof(DirectoryPropertyXmlCompanyPartnershipSingle))]
	[Serializable]
	public class DirectoryPropertyXmlCompanyPartnership : DirectoryPropertyXml
	{
		[XmlElement("Value")]
		public XmlValueCompanyPartnership[] Value
		{
			get
			{
				return this.valueField;
			}
			set
			{
				this.valueField = value;
			}
		}

		private XmlValueCompanyPartnership[] valueField;
	}
}
