using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Forefront.Monitoring.ActiveMonitoring.MicrosoftOnline
{
	[XmlType(Namespace = "http://schemas.microsoft.com/online/directoryservices/change/2008/11")]
	[GeneratedCode("wsdl", "2.0.50727.1432")]
	[DebuggerStepThrough]
	[DesignerCategory("code")]
	[Serializable]
	public class CompanyUnverifiedDomainValue : CompanyDomainValue
	{
		public CompanyUnverifiedDomainValue()
		{
			this.pendingDeletionField = false;
		}

		[XmlAttribute]
		[DefaultValue(false)]
		public bool PendingDeletion
		{
			get
			{
				return this.pendingDeletionField;
			}
			set
			{
				this.pendingDeletionField = value;
			}
		}

		[XmlAttribute]
		public string VerificationCode
		{
			get
			{
				return this.verificationCodeField;
			}
			set
			{
				this.verificationCodeField = value;
			}
		}

		private bool pendingDeletionField;

		private string verificationCodeField;
	}
}
