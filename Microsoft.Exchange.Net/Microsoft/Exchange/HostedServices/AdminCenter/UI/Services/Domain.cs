using System;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.HostedServices.AdminCenter.UI.Services
{
	[DataContract(Name = "Domain", Namespace = "http://schemas.datacontract.org/2004/07/Microsoft.Exchange.HostedServices.AdminCenter.UI.Services")]
	[DebuggerStepThrough]
	[GeneratedCode("System.Runtime.Serialization", "3.0.0.0")]
	[Serializable]
	internal class Domain : IExtensibleDataObject
	{
		public ExtensionDataObject ExtensionData
		{
			get
			{
				return this.extensionDataField;
			}
			set
			{
				this.extensionDataField = value;
			}
		}

		[DataMember]
		internal bool CatchAll
		{
			get
			{
				return this.CatchAllField;
			}
			set
			{
				this.CatchAllField = value;
			}
		}

		[DataMember]
		internal int CompanyId
		{
			get
			{
				return this.CompanyIdField;
			}
			set
			{
				this.CompanyIdField = value;
			}
		}

		[DataMember]
		internal DateTime DateCreated
		{
			get
			{
				return this.DateCreatedField;
			}
			set
			{
				this.DateCreatedField = value;
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
		internal int DomainId
		{
			get
			{
				return this.DomainIdField;
			}
			set
			{
				this.DomainIdField = value;
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
		internal bool IsEnabled
		{
			get
			{
				return this.IsEnabledField;
			}
			set
			{
				this.IsEnabledField = value;
			}
		}

		[DataMember]
		internal bool IsValid
		{
			get
			{
				return this.IsValidField;
			}
			set
			{
				this.IsValidField = value;
			}
		}

		[DataMember]
		internal string MailServer
		{
			get
			{
				return this.MailServerField;
			}
			set
			{
				this.MailServerField = value;
			}
		}

		[DataMember]
		internal MailServerType MailServerType
		{
			get
			{
				return this.MailServerTypeField;
			}
			set
			{
				this.MailServerTypeField = value;
			}
		}

		[DataMember]
		internal string Name
		{
			get
			{
				return this.NameField;
			}
			set
			{
				this.NameField = value;
			}
		}

		[DataMember]
		internal int RetentionPeriod
		{
			get
			{
				return this.RetentionPeriodField;
			}
			set
			{
				this.RetentionPeriodField = value;
			}
		}

		[DataMember]
		internal DomainConfigurationSettings Settings
		{
			get
			{
				return this.SettingsField;
			}
			set
			{
				this.SettingsField = value;
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

		[NonSerialized]
		private ExtensionDataObject extensionDataField;

		[OptionalField]
		private bool CatchAllField;

		[OptionalField]
		private int CompanyIdField;

		[OptionalField]
		private DateTime DateCreatedField;

		[OptionalField]
		private Guid? DomainGuidField;

		[OptionalField]
		private int DomainIdField;

		[OptionalField]
		private InheritanceSettings InheritFromCompanyField;

		[OptionalField]
		private bool IsEnabledField;

		[OptionalField]
		private bool IsValidField;

		[OptionalField]
		private string MailServerField;

		[OptionalField]
		private MailServerType MailServerTypeField;

		[OptionalField]
		private string NameField;

		[OptionalField]
		private int RetentionPeriodField;

		[OptionalField]
		private DomainConfigurationSettings SettingsField;

		[OptionalField]
		private string SmtpProfileNameField;
	}
}
