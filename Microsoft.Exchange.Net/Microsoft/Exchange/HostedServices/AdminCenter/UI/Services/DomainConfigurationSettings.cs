using System;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.HostedServices.AdminCenter.UI.Services
{
	[DebuggerStepThrough]
	[DataContract(Name = "DomainConfigurationSettings", Namespace = "http://schemas.datacontract.org/2004/07/Microsoft.Exchange.HostedServices.AdminCenter.UI.Services")]
	[GeneratedCode("System.Runtime.Serialization", "3.0.0.0")]
	[Serializable]
	internal class DomainConfigurationSettings : ConfigurationSettings
	{
		[DataMember]
		internal int[] ConnectorId
		{
			get
			{
				return this.ConnectorIdField;
			}
			set
			{
				this.ConnectorIdField = value;
			}
		}

		[DataMember]
		internal Guid? DomainGuid
		{
			get
			{
				return this.DomainGuidField;
			}
			set
			{
				this.DomainGuidField = value;
			}
		}

		[DataMember]
		internal string DomainName
		{
			get
			{
				return this.DomainNameField;
			}
			set
			{
				this.DomainNameField = value;
			}
		}

		[DataMember]
		internal InheritanceSettings InheritFromCompany
		{
			get
			{
				return this.InheritFromCompanyField;
			}
			set
			{
				this.InheritFromCompanyField = value;
			}
		}

		[DataMember]
		internal DomainMailFlowType MailFlowType
		{
			get
			{
				return this.MailFlowTypeField;
			}
			set
			{
				this.MailFlowTypeField = value;
			}
		}

		[DataMember]
		internal string SmtpProfileName
		{
			get
			{
				return this.SmtpProfileNameField;
			}
			set
			{
				this.SmtpProfileNameField = value;
			}
		}

		[OptionalField]
		private int[] ConnectorIdField;

		[OptionalField]
		private Guid? DomainGuidField;

		[OptionalField]
		private string DomainNameField;

		[OptionalField]
		private InheritanceSettings InheritFromCompanyField;

		[OptionalField]
		private DomainMailFlowType MailFlowTypeField;

		[OptionalField]
		private string SmtpProfileNameField;
	}
}
