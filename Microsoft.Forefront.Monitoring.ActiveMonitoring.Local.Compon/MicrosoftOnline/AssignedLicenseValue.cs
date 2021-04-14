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
	[Serializable]
	public class AssignedLicenseValue
	{
		public AssignedLicenseValue()
		{
			this.disabledField = new string[0];
		}

		[XmlAttribute]
		public string AccountId
		{
			get
			{
				return this.accountIdField;
			}
			set
			{
				this.accountIdField = value;
			}
		}

		[XmlAttribute]
		public string SkuId
		{
			get
			{
				return this.skuIdField;
			}
			set
			{
				this.skuIdField = value;
			}
		}

		[XmlAttribute]
		public string[] Disabled
		{
			get
			{
				return this.disabledField;
			}
			set
			{
				this.disabledField = value;
			}
		}

		private string accountIdField;

		private string skuIdField;

		private string[] disabledField;
	}
}
