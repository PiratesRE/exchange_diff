using System;
using System.CodeDom.Compiler;
using System.Collections.ObjectModel;
using System.Data.Services.Common;

namespace Microsoft.WindowsAzure.ActiveDirectoryV142
{
	[DataServiceKey("objectId")]
	public class TenantDetail : DirectoryObject
	{
		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		public static TenantDetail CreateTenantDetail(string objectId, Collection<AssignedPlan> assignedPlans, Collection<string> companyTags, Collection<string> marketingNotificationEmails, Collection<ProvisionedPlan> provisionedPlans, Collection<ProvisioningError> provisioningErrors, Collection<string> technicalNotificationMails, Collection<VerifiedDomain> verifiedDomains)
		{
			TenantDetail tenantDetail = new TenantDetail();
			tenantDetail.objectId = objectId;
			if (assignedPlans == null)
			{
				throw new ArgumentNullException("assignedPlans");
			}
			tenantDetail.assignedPlans = assignedPlans;
			if (companyTags == null)
			{
				throw new ArgumentNullException("companyTags");
			}
			tenantDetail.companyTags = companyTags;
			if (marketingNotificationEmails == null)
			{
				throw new ArgumentNullException("marketingNotificationEmails");
			}
			tenantDetail.marketingNotificationEmails = marketingNotificationEmails;
			if (provisionedPlans == null)
			{
				throw new ArgumentNullException("provisionedPlans");
			}
			tenantDetail.provisionedPlans = provisionedPlans;
			if (provisioningErrors == null)
			{
				throw new ArgumentNullException("provisioningErrors");
			}
			tenantDetail.provisioningErrors = provisioningErrors;
			if (technicalNotificationMails == null)
			{
				throw new ArgumentNullException("technicalNotificationMails");
			}
			tenantDetail.technicalNotificationMails = technicalNotificationMails;
			if (verifiedDomains == null)
			{
				throw new ArgumentNullException("verifiedDomains");
			}
			tenantDetail.verifiedDomains = verifiedDomains;
			return tenantDetail;
		}

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		public Collection<AssignedPlan> assignedPlans
		{
			get
			{
				return this._assignedPlans;
			}
			set
			{
				this._assignedPlans = value;
			}
		}

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		public string city
		{
			get
			{
				return this._city;
			}
			set
			{
				this._city = value;
			}
		}

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		public DateTime? companyLastDirSyncTime
		{
			get
			{
				return this._companyLastDirSyncTime;
			}
			set
			{
				this._companyLastDirSyncTime = value;
			}
		}

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		public Collection<string> companyTags
		{
			get
			{
				return this._companyTags;
			}
			set
			{
				this._companyTags = value;
			}
		}

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		public string country
		{
			get
			{
				return this._country;
			}
			set
			{
				this._country = value;
			}
		}

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		public string countryLetterCode
		{
			get
			{
				return this._countryLetterCode;
			}
			set
			{
				this._countryLetterCode = value;
			}
		}

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		public bool? dirSyncEnabled
		{
			get
			{
				return this._dirSyncEnabled;
			}
			set
			{
				this._dirSyncEnabled = value;
			}
		}

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		public string displayName
		{
			get
			{
				return this._displayName;
			}
			set
			{
				this._displayName = value;
			}
		}

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		public Collection<string> marketingNotificationEmails
		{
			get
			{
				return this._marketingNotificationEmails;
			}
			set
			{
				this._marketingNotificationEmails = value;
			}
		}

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		public string postalCode
		{
			get
			{
				return this._postalCode;
			}
			set
			{
				this._postalCode = value;
			}
		}

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		public string preferredLanguage
		{
			get
			{
				return this._preferredLanguage;
			}
			set
			{
				this._preferredLanguage = value;
			}
		}

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		public Collection<ProvisionedPlan> provisionedPlans
		{
			get
			{
				return this._provisionedPlans;
			}
			set
			{
				this._provisionedPlans = value;
			}
		}

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		public Collection<ProvisioningError> provisioningErrors
		{
			get
			{
				return this._provisioningErrors;
			}
			set
			{
				this._provisioningErrors = value;
			}
		}

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		public SelfServePasswordResetPolicy selfServePasswordResetPolicy
		{
			get
			{
				if (this._selfServePasswordResetPolicy == null && !this._selfServePasswordResetPolicyInitialized)
				{
					this._selfServePasswordResetPolicy = new SelfServePasswordResetPolicy();
					this._selfServePasswordResetPolicyInitialized = true;
				}
				return this._selfServePasswordResetPolicy;
			}
			set
			{
				this._selfServePasswordResetPolicy = value;
				this._selfServePasswordResetPolicyInitialized = true;
			}
		}

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		public string state
		{
			get
			{
				return this._state;
			}
			set
			{
				this._state = value;
			}
		}

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		public string street
		{
			get
			{
				return this._street;
			}
			set
			{
				this._street = value;
			}
		}

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		public Collection<string> technicalNotificationMails
		{
			get
			{
				return this._technicalNotificationMails;
			}
			set
			{
				this._technicalNotificationMails = value;
			}
		}

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		public string telephoneNumber
		{
			get
			{
				return this._telephoneNumber;
			}
			set
			{
				this._telephoneNumber = value;
			}
		}

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		public string tenantType
		{
			get
			{
				return this._tenantType;
			}
			set
			{
				this._tenantType = value;
			}
		}

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		public Collection<VerifiedDomain> verifiedDomains
		{
			get
			{
				return this._verifiedDomains;
			}
			set
			{
				this._verifiedDomains = value;
			}
		}

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		public Collection<ServiceInfo> serviceInfo
		{
			get
			{
				return this._serviceInfo;
			}
			set
			{
				if (value != null)
				{
					this._serviceInfo = value;
				}
			}
		}

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		private Collection<AssignedPlan> _assignedPlans = new Collection<AssignedPlan>();

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		private string _city;

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		private DateTime? _companyLastDirSyncTime;

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		private Collection<string> _companyTags = new Collection<string>();

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		private string _country;

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		private string _countryLetterCode;

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		private bool? _dirSyncEnabled;

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		private string _displayName;

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		private Collection<string> _marketingNotificationEmails = new Collection<string>();

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		private string _postalCode;

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		private string _preferredLanguage;

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		private Collection<ProvisionedPlan> _provisionedPlans = new Collection<ProvisionedPlan>();

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		private Collection<ProvisioningError> _provisioningErrors = new Collection<ProvisioningError>();

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		private SelfServePasswordResetPolicy _selfServePasswordResetPolicy;

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		private bool _selfServePasswordResetPolicyInitialized;

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		private string _state;

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		private string _street;

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		private Collection<string> _technicalNotificationMails = new Collection<string>();

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		private string _telephoneNumber;

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		private string _tenantType;

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		private Collection<VerifiedDomain> _verifiedDomains = new Collection<VerifiedDomain>();

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		private Collection<ServiceInfo> _serviceInfo = new Collection<ServiceInfo>();
	}
}
