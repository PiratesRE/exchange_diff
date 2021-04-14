using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Forefront.Monitoring.ActiveMonitoring.MicrosoftOnline
{
	[GeneratedCode("wsdl", "2.0.50727.1432")]
	[XmlType(Namespace = "http://www.ccs.com/TestServices/")]
	[DesignerCategory("code")]
	[DebuggerStepThrough]
	[Serializable]
	public class Account
	{
		public Guid AccountId
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

		public string AccountName
		{
			get
			{
				return this.accountNameField;
			}
			set
			{
				this.accountNameField = value;
			}
		}

		private Guid accountIdField;

		private string accountNameField;
	}
}
