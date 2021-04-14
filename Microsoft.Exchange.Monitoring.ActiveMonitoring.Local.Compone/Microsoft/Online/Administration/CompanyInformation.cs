using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.Serialization;
using System.Xml;

namespace Microsoft.Online.Administration
{
	[DebuggerStepThrough]
	[GeneratedCode("System.Runtime.Serialization", "4.0.0.0")]
	[DataContract(Name = "CompanyInformation", Namespace = "http://schemas.datacontract.org/2004/07/Microsoft.Online.Administration")]
	public class CompanyInformation : IExtensibleDataObject
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
		public string[] AuthorizedServiceInstances
		{
			get
			{
				return this.AuthorizedServiceInstancesField;
			}
			set
			{
				this.AuthorizedServiceInstancesField = value;
			}
		}

		[DataMember]
		public AuthorizedService[] AuthorizedServices
		{
			get
			{
				return this.AuthorizedServicesField;
			}
			set
			{
				this.AuthorizedServicesField = value;
			}
		}

		[DataMember]
		public string City
		{
			get
			{
				return this.CityField;
			}
			set
			{
				this.CityField = value;
			}
		}

		[DataMember]
		public CompanyType CompanyType
		{
			get
			{
				return this.CompanyTypeField;
			}
			set
			{
				this.CompanyTypeField = value;
			}
		}

		[DataMember]
		public string Country
		{
			get
			{
				return this.CountryField;
			}
			set
			{
				this.CountryField = value;
			}
		}

		[DataMember]
		public string CountryLetterCode
		{
			get
			{
				return this.CountryLetterCodeField;
			}
			set
			{
				this.CountryLetterCodeField = value;
			}
		}

		[DataMember]
		public bool? DapEnabled
		{
			get
			{
				return this.DapEnabledField;
			}
			set
			{
				this.DapEnabledField = value;
			}
		}

		[DataMember]
		public bool? DirectorySynchronizationEnabled
		{
			get
			{
				return this.DirectorySynchronizationEnabledField;
			}
			set
			{
				this.DirectorySynchronizationEnabledField = value;
			}
		}

		[DataMember]
		public DirSyncStatus DirectorySynchronizationStatus
		{
			get
			{
				return this.DirectorySynchronizationStatusField;
			}
			set
			{
				this.DirectorySynchronizationStatusField = value;
			}
		}

		[DataMember]
		public string DisplayName
		{
			get
			{
				return this.DisplayNameField;
			}
			set
			{
				this.DisplayNameField = value;
			}
		}

		[DataMember]
		public string InitialDomain
		{
			get
			{
				return this.InitialDomainField;
			}
			set
			{
				this.InitialDomainField = value;
			}
		}

		[DataMember]
		public DateTime? LastDirSyncTime
		{
			get
			{
				return this.LastDirSyncTimeField;
			}
			set
			{
				this.LastDirSyncTimeField = value;
			}
		}

		[DataMember]
		public string[] MarketingNotificationEmails
		{
			get
			{
				return this.MarketingNotificationEmailsField;
			}
			set
			{
				this.MarketingNotificationEmailsField = value;
			}
		}

		[DataMember]
		public Guid? ObjectId
		{
			get
			{
				return this.ObjectIdField;
			}
			set
			{
				this.ObjectIdField = value;
			}
		}

		[DataMember]
		public XmlElement PortalSettings
		{
			get
			{
				return this.PortalSettingsField;
			}
			set
			{
				this.PortalSettingsField = value;
			}
		}

		[DataMember]
		public string PostalCode
		{
			get
			{
				return this.PostalCodeField;
			}
			set
			{
				this.PostalCodeField = value;
			}
		}

		[DataMember]
		public string PreferredLanguage
		{
			get
			{
				return this.PreferredLanguageField;
			}
			set
			{
				this.PreferredLanguageField = value;
			}
		}

		[DataMember]
		public bool SelfServePasswordResetEnabled
		{
			get
			{
				return this.SelfServePasswordResetEnabledField;
			}
			set
			{
				this.SelfServePasswordResetEnabledField = value;
			}
		}

		[DataMember]
		public ServiceInformation[] ServiceInformation
		{
			get
			{
				return this.ServiceInformationField;
			}
			set
			{
				this.ServiceInformationField = value;
			}
		}

		[DataMember]
		public ServiceInstanceInformation[] ServiceInstanceInformation
		{
			get
			{
				return this.ServiceInstanceInformationField;
			}
			set
			{
				this.ServiceInstanceInformationField = value;
			}
		}

		[DataMember]
		public string State
		{
			get
			{
				return this.StateField;
			}
			set
			{
				this.StateField = value;
			}
		}

		[DataMember]
		public string Street
		{
			get
			{
				return this.StreetField;
			}
			set
			{
				this.StreetField = value;
			}
		}

		[DataMember]
		public string[] TechnicalNotificationEmails
		{
			get
			{
				return this.TechnicalNotificationEmailsField;
			}
			set
			{
				this.TechnicalNotificationEmailsField = value;
			}
		}

		[DataMember]
		public string TelephoneNumber
		{
			get
			{
				return this.TelephoneNumberField;
			}
			set
			{
				this.TelephoneNumberField = value;
			}
		}

		[DataMember]
		public Dictionary<string, string> UIExtensibilityUris
		{
			get
			{
				return this.UIExtensibilityUrisField;
			}
			set
			{
				this.UIExtensibilityUrisField = value;
			}
		}

		private ExtensionDataObject extensionDataField;

		private string[] AuthorizedServiceInstancesField;

		private AuthorizedService[] AuthorizedServicesField;

		private string CityField;

		private CompanyType CompanyTypeField;

		private string CountryField;

		private string CountryLetterCodeField;

		private bool? DapEnabledField;

		private bool? DirectorySynchronizationEnabledField;

		private DirSyncStatus DirectorySynchronizationStatusField;

		private string DisplayNameField;

		private string InitialDomainField;

		private DateTime? LastDirSyncTimeField;

		private string[] MarketingNotificationEmailsField;

		private Guid? ObjectIdField;

		private XmlElement PortalSettingsField;

		private string PostalCodeField;

		private string PreferredLanguageField;

		private bool SelfServePasswordResetEnabledField;

		private ServiceInformation[] ServiceInformationField;

		private ServiceInstanceInformation[] ServiceInstanceInformationField;

		private string StateField;

		private string StreetField;

		private string[] TechnicalNotificationEmailsField;

		private string TelephoneNumberField;

		private Dictionary<string, string> UIExtensibilityUrisField;
	}
}
