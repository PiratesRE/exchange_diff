using System;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.HostedServices.AdminCenter.UI.Services
{
	[DebuggerStepThrough]
	[DataContract(Name = "Company", Namespace = "http://schemas.datacontract.org/2004/07/Microsoft.Exchange.HostedServices.AdminCenter.UI.Services")]
	[GeneratedCode("System.Runtime.Serialization", "3.0.0.0")]
	[Serializable]
	internal class Company : IExtensibleDataObject
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
		internal bool ActivationComplete
		{
			get
			{
				return this.ActivationCompleteField;
			}
			set
			{
				this.ActivationCompleteField = value;
			}
		}

		[DataMember]
		internal Guid? CompanyGuid
		{
			get
			{
				return this.CompanyGuidField;
			}
			set
			{
				this.CompanyGuidField = value;
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
		internal int? ConfigurationId
		{
			get
			{
				return this.ConfigurationIdField;
			}
			set
			{
				this.ConfigurationIdField = value;
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
		internal InheritanceSettings InheritFromParent
		{
			get
			{
				return this.InheritFromParentField;
			}
			set
			{
				this.InheritFromParentField = value;
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
		internal bool IsReseller
		{
			get
			{
				return this.IsResellerField;
			}
			set
			{
				this.IsResellerField = value;
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
		internal int ParentCompanyId
		{
			get
			{
				return this.ParentCompanyIdField;
			}
			set
			{
				this.ParentCompanyIdField = value;
			}
		}

		[DataMember]
		internal CompanyConfigurationSettings Settings
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
		internal TimeZone TimeZone
		{
			get
			{
				return this.TimeZoneField;
			}
			set
			{
				this.TimeZoneField = value;
			}
		}

		[NonSerialized]
		private ExtensionDataObject extensionDataField;

		[OptionalField]
		private bool ActivationCompleteField;

		[OptionalField]
		private Guid? CompanyGuidField;

		[OptionalField]
		private int CompanyIdField;

		[OptionalField]
		private int? ConfigurationIdField;

		[OptionalField]
		private DateTime DateCreatedField;

		[OptionalField]
		private InheritanceSettings InheritFromParentField;

		[OptionalField]
		private bool IsEnabledField;

		[OptionalField]
		private bool IsResellerField;

		[OptionalField]
		private string NameField;

		[OptionalField]
		private int ParentCompanyIdField;

		[OptionalField]
		private CompanyConfigurationSettings SettingsField;

		[OptionalField]
		private TimeZone TimeZoneField;
	}
}
