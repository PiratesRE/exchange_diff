using System;
using System.CodeDom.Compiler;
using System.Collections.ObjectModel;
using System.Data.Services.Client;
using System.Data.Services.Common;

namespace Microsoft.WindowsAzure.ActiveDirectoryV142
{
	[DataServiceKey("objectId")]
	public class User : DirectoryObject
	{
		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		public static User CreateUser(string objectId, Collection<AlternativeSecurityId> alternativeSecurityIds, Collection<AssignedLicense> assignedLicenses, Collection<AssignedPlan> assignedPlans, Collection<KeyValue> inviteReplyUrl, Collection<string> inviteResources, Collection<InvitationTicket> inviteTicket, Collection<LogonIdentifier> logonIdentifiers, Collection<string> otherMails, Collection<ProvisionedPlan> provisionedPlans, Collection<ProvisioningError> provisioningErrors, Collection<string> proxyAddresses, Collection<string> smtpAddresses, DataServiceStreamLink thumbnailPhoto)
		{
			User user = new User();
			user.objectId = objectId;
			if (alternativeSecurityIds == null)
			{
				throw new ArgumentNullException("alternativeSecurityIds");
			}
			user.alternativeSecurityIds = alternativeSecurityIds;
			if (assignedLicenses == null)
			{
				throw new ArgumentNullException("assignedLicenses");
			}
			user.assignedLicenses = assignedLicenses;
			if (assignedPlans == null)
			{
				throw new ArgumentNullException("assignedPlans");
			}
			user.assignedPlans = assignedPlans;
			if (inviteReplyUrl == null)
			{
				throw new ArgumentNullException("inviteReplyUrl");
			}
			user.inviteReplyUrl = inviteReplyUrl;
			if (inviteResources == null)
			{
				throw new ArgumentNullException("inviteResources");
			}
			user.inviteResources = inviteResources;
			if (inviteTicket == null)
			{
				throw new ArgumentNullException("inviteTicket");
			}
			user.inviteTicket = inviteTicket;
			if (logonIdentifiers == null)
			{
				throw new ArgumentNullException("logonIdentifiers");
			}
			user.logonIdentifiers = logonIdentifiers;
			if (otherMails == null)
			{
				throw new ArgumentNullException("otherMails");
			}
			user.otherMails = otherMails;
			if (provisionedPlans == null)
			{
				throw new ArgumentNullException("provisionedPlans");
			}
			user.provisionedPlans = provisionedPlans;
			if (provisioningErrors == null)
			{
				throw new ArgumentNullException("provisioningErrors");
			}
			user.provisioningErrors = provisioningErrors;
			if (proxyAddresses == null)
			{
				throw new ArgumentNullException("proxyAddresses");
			}
			user.proxyAddresses = proxyAddresses;
			if (smtpAddresses == null)
			{
				throw new ArgumentNullException("smtpAddresses");
			}
			user.smtpAddresses = smtpAddresses;
			user.thumbnailPhoto = thumbnailPhoto;
			return user;
		}

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		public string acceptedAs
		{
			get
			{
				return this._acceptedAs;
			}
			set
			{
				this._acceptedAs = value;
			}
		}

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		public DateTime? acceptedOn
		{
			get
			{
				return this._acceptedOn;
			}
			set
			{
				this._acceptedOn = value;
			}
		}

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		public bool? accountEnabled
		{
			get
			{
				return this._accountEnabled;
			}
			set
			{
				this._accountEnabled = value;
			}
		}

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		public Collection<AlternativeSecurityId> alternativeSecurityIds
		{
			get
			{
				return this._alternativeSecurityIds;
			}
			set
			{
				this._alternativeSecurityIds = value;
			}
		}

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		public AppMetadata appMetadata
		{
			get
			{
				if (this._appMetadata == null && !this._appMetadataInitialized)
				{
					this._appMetadata = new AppMetadata();
					this._appMetadataInitialized = true;
				}
				return this._appMetadata;
			}
			set
			{
				this._appMetadata = value;
				this._appMetadataInitialized = true;
			}
		}

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		public Collection<AssignedLicense> assignedLicenses
		{
			get
			{
				return this._assignedLicenses;
			}
			set
			{
				this._assignedLicenses = value;
			}
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
		public string creationType
		{
			get
			{
				return this._creationType;
			}
			set
			{
				this._creationType = value;
			}
		}

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		public string department
		{
			get
			{
				return this._department;
			}
			set
			{
				this._department = value;
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
		public string extensionAttribute1
		{
			get
			{
				return this._extensionAttribute1;
			}
			set
			{
				this._extensionAttribute1 = value;
			}
		}

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		public string extensionAttribute2
		{
			get
			{
				return this._extensionAttribute2;
			}
			set
			{
				this._extensionAttribute2 = value;
			}
		}

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		public string extensionAttribute3
		{
			get
			{
				return this._extensionAttribute3;
			}
			set
			{
				this._extensionAttribute3 = value;
			}
		}

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		public string extensionAttribute4
		{
			get
			{
				return this._extensionAttribute4;
			}
			set
			{
				this._extensionAttribute4 = value;
			}
		}

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		public string extensionAttribute5
		{
			get
			{
				return this._extensionAttribute5;
			}
			set
			{
				this._extensionAttribute5 = value;
			}
		}

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		public string extensionAttribute6
		{
			get
			{
				return this._extensionAttribute6;
			}
			set
			{
				this._extensionAttribute6 = value;
			}
		}

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		public string extensionAttribute7
		{
			get
			{
				return this._extensionAttribute7;
			}
			set
			{
				this._extensionAttribute7 = value;
			}
		}

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		public string extensionAttribute8
		{
			get
			{
				return this._extensionAttribute8;
			}
			set
			{
				this._extensionAttribute8 = value;
			}
		}

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		public string extensionAttribute9
		{
			get
			{
				return this._extensionAttribute9;
			}
			set
			{
				this._extensionAttribute9 = value;
			}
		}

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		public string extensionAttribute10
		{
			get
			{
				return this._extensionAttribute10;
			}
			set
			{
				this._extensionAttribute10 = value;
			}
		}

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		public string extensionAttribute11
		{
			get
			{
				return this._extensionAttribute11;
			}
			set
			{
				this._extensionAttribute11 = value;
			}
		}

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		public string extensionAttribute12
		{
			get
			{
				return this._extensionAttribute12;
			}
			set
			{
				this._extensionAttribute12 = value;
			}
		}

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		public string extensionAttribute13
		{
			get
			{
				return this._extensionAttribute13;
			}
			set
			{
				this._extensionAttribute13 = value;
			}
		}

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		public string extensionAttribute14
		{
			get
			{
				return this._extensionAttribute14;
			}
			set
			{
				this._extensionAttribute14 = value;
			}
		}

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		public string extensionAttribute15
		{
			get
			{
				return this._extensionAttribute15;
			}
			set
			{
				this._extensionAttribute15 = value;
			}
		}

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		public string facsimileTelephoneNumber
		{
			get
			{
				return this._facsimileTelephoneNumber;
			}
			set
			{
				this._facsimileTelephoneNumber = value;
			}
		}

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		public string givenName
		{
			get
			{
				return this._givenName;
			}
			set
			{
				this._givenName = value;
			}
		}

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		public string immutableId
		{
			get
			{
				return this._immutableId;
			}
			set
			{
				this._immutableId = value;
			}
		}

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		public DateTime? invitedOn
		{
			get
			{
				return this._invitedOn;
			}
			set
			{
				this._invitedOn = value;
			}
		}

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		public Collection<KeyValue> inviteReplyUrl
		{
			get
			{
				return this._inviteReplyUrl;
			}
			set
			{
				this._inviteReplyUrl = value;
			}
		}

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		public Collection<string> inviteResources
		{
			get
			{
				return this._inviteResources;
			}
			set
			{
				this._inviteResources = value;
			}
		}

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		public Collection<InvitationTicket> inviteTicket
		{
			get
			{
				return this._inviteTicket;
			}
			set
			{
				this._inviteTicket = value;
			}
		}

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		public string jobTitle
		{
			get
			{
				return this._jobTitle;
			}
			set
			{
				this._jobTitle = value;
			}
		}

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		public DateTime? lastDirSyncTime
		{
			get
			{
				return this._lastDirSyncTime;
			}
			set
			{
				this._lastDirSyncTime = value;
			}
		}

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		public Collection<LogonIdentifier> logonIdentifiers
		{
			get
			{
				return this._logonIdentifiers;
			}
			set
			{
				this._logonIdentifiers = value;
			}
		}

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		public string mail
		{
			get
			{
				return this._mail;
			}
			set
			{
				this._mail = value;
			}
		}

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		public string mailNickname
		{
			get
			{
				return this._mailNickname;
			}
			set
			{
				this._mailNickname = value;
			}
		}

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		public string mobile
		{
			get
			{
				return this._mobile;
			}
			set
			{
				this._mobile = value;
			}
		}

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		public string netId
		{
			get
			{
				return this._netId;
			}
			set
			{
				this._netId = value;
			}
		}

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		public byte[] onPremiseSecurityIdentifier
		{
			get
			{
				if (this._onPremiseSecurityIdentifier != null)
				{
					return (byte[])this._onPremiseSecurityIdentifier.Clone();
				}
				return null;
			}
			set
			{
				this._onPremiseSecurityIdentifier = value;
			}
		}

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		public Collection<string> otherMails
		{
			get
			{
				return this._otherMails;
			}
			set
			{
				this._otherMails = value;
			}
		}

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		public string passwordPolicies
		{
			get
			{
				return this._passwordPolicies;
			}
			set
			{
				this._passwordPolicies = value;
			}
		}

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		public PasswordProfile passwordProfile
		{
			get
			{
				if (this._passwordProfile == null && !this._passwordProfileInitialized)
				{
					this._passwordProfile = new PasswordProfile();
					this._passwordProfileInitialized = true;
				}
				return this._passwordProfile;
			}
			set
			{
				this._passwordProfile = value;
				this._passwordProfileInitialized = true;
			}
		}

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		public string physicalDeliveryOfficeName
		{
			get
			{
				return this._physicalDeliveryOfficeName;
			}
			set
			{
				this._physicalDeliveryOfficeName = value;
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
		public string primarySMTPAddress
		{
			get
			{
				return this._primarySMTPAddress;
			}
			set
			{
				this._primarySMTPAddress = value;
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
		public Collection<string> proxyAddresses
		{
			get
			{
				return this._proxyAddresses;
			}
			set
			{
				this._proxyAddresses = value;
			}
		}

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		public SelfServePasswordResetData selfServePasswordResetData
		{
			get
			{
				if (this._selfServePasswordResetData == null && !this._selfServePasswordResetDataInitialized)
				{
					this._selfServePasswordResetData = new SelfServePasswordResetData();
					this._selfServePasswordResetDataInitialized = true;
				}
				return this._selfServePasswordResetData;
			}
			set
			{
				this._selfServePasswordResetData = value;
				this._selfServePasswordResetDataInitialized = true;
			}
		}

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		public string signInName
		{
			get
			{
				return this._signInName;
			}
			set
			{
				this._signInName = value;
			}
		}

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		public string sipProxyAddress
		{
			get
			{
				return this._sipProxyAddress;
			}
			set
			{
				this._sipProxyAddress = value;
			}
		}

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		public Collection<string> smtpAddresses
		{
			get
			{
				return this._smtpAddresses;
			}
			set
			{
				this._smtpAddresses = value;
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
		public string streetAddress
		{
			get
			{
				return this._streetAddress;
			}
			set
			{
				this._streetAddress = value;
			}
		}

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		public string surname
		{
			get
			{
				return this._surname;
			}
			set
			{
				this._surname = value;
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
		public DataServiceStreamLink thumbnailPhoto
		{
			get
			{
				return this._thumbnailPhoto;
			}
			set
			{
				this._thumbnailPhoto = value;
			}
		}

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		public string usageLocation
		{
			get
			{
				return this._usageLocation;
			}
			set
			{
				this._usageLocation = value;
			}
		}

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		public string userPrincipalName
		{
			get
			{
				return this._userPrincipalName;
			}
			set
			{
				this._userPrincipalName = value;
			}
		}

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		public string userState
		{
			get
			{
				return this._userState;
			}
			set
			{
				this._userState = value;
			}
		}

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		public DateTime? userStateChangedOn
		{
			get
			{
				return this._userStateChangedOn;
			}
			set
			{
				this._userStateChangedOn = value;
			}
		}

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		public string userType
		{
			get
			{
				return this._userType;
			}
			set
			{
				this._userType = value;
			}
		}

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		public Collection<ImpersonationAccessGrant> impersonationAccessGrants
		{
			get
			{
				return this._impersonationAccessGrants;
			}
			set
			{
				if (value != null)
				{
					this._impersonationAccessGrants = value;
				}
			}
		}

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		public Collection<ServiceEndpoint> serviceEndpoints
		{
			get
			{
				return this._serviceEndpoints;
			}
			set
			{
				if (value != null)
				{
					this._serviceEndpoints = value;
				}
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
		public Collection<DirectoryObject> registeredDevices
		{
			get
			{
				return this._registeredDevices;
			}
			set
			{
				if (value != null)
				{
					this._registeredDevices = value;
				}
			}
		}

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		public Collection<DirectoryObject> ownedDevices
		{
			get
			{
				return this._ownedDevices;
			}
			set
			{
				if (value != null)
				{
					this._ownedDevices = value;
				}
			}
		}

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		public Collection<DirectAccessGrant> directAccessGrants
		{
			get
			{
				return this._directAccessGrants;
			}
			set
			{
				if (value != null)
				{
					this._directAccessGrants = value;
				}
			}
		}

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		public Collection<DirectoryObject> pendingMemberOf
		{
			get
			{
				return this._pendingMemberOf;
			}
			set
			{
				if (value != null)
				{
					this._pendingMemberOf = value;
				}
			}
		}

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		public Collection<DirectoryObject> invitedBy
		{
			get
			{
				return this._invitedBy;
			}
			set
			{
				if (value != null)
				{
					this._invitedBy = value;
				}
			}
		}

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		public Collection<DirectoryObject> invitedUsers
		{
			get
			{
				return this._invitedUsers;
			}
			set
			{
				if (value != null)
				{
					this._invitedUsers = value;
				}
			}
		}

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		private string _acceptedAs;

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		private DateTime? _acceptedOn;

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		private bool? _accountEnabled;

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		private Collection<AlternativeSecurityId> _alternativeSecurityIds = new Collection<AlternativeSecurityId>();

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		private AppMetadata _appMetadata;

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		private bool _appMetadataInitialized;

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		private Collection<AssignedLicense> _assignedLicenses = new Collection<AssignedLicense>();

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		private Collection<AssignedPlan> _assignedPlans = new Collection<AssignedPlan>();

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		private string _city;

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		private string _country;

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		private string _creationType;

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		private string _department;

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		private bool? _dirSyncEnabled;

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		private string _displayName;

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		private string _extensionAttribute1;

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		private string _extensionAttribute2;

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		private string _extensionAttribute3;

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		private string _extensionAttribute4;

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		private string _extensionAttribute5;

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		private string _extensionAttribute6;

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		private string _extensionAttribute7;

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		private string _extensionAttribute8;

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		private string _extensionAttribute9;

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		private string _extensionAttribute10;

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		private string _extensionAttribute11;

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		private string _extensionAttribute12;

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		private string _extensionAttribute13;

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		private string _extensionAttribute14;

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		private string _extensionAttribute15;

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		private string _facsimileTelephoneNumber;

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		private string _givenName;

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		private string _immutableId;

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		private DateTime? _invitedOn;

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		private Collection<KeyValue> _inviteReplyUrl = new Collection<KeyValue>();

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		private Collection<string> _inviteResources = new Collection<string>();

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		private Collection<InvitationTicket> _inviteTicket = new Collection<InvitationTicket>();

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		private string _jobTitle;

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		private DateTime? _lastDirSyncTime;

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		private Collection<LogonIdentifier> _logonIdentifiers = new Collection<LogonIdentifier>();

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		private string _mail;

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		private string _mailNickname;

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		private string _mobile;

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		private string _netId;

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		private byte[] _onPremiseSecurityIdentifier;

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		private Collection<string> _otherMails = new Collection<string>();

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		private string _passwordPolicies;

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		private PasswordProfile _passwordProfile;

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		private bool _passwordProfileInitialized;

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		private string _physicalDeliveryOfficeName;

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		private string _postalCode;

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		private string _preferredLanguage;

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		private string _primarySMTPAddress;

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		private Collection<ProvisionedPlan> _provisionedPlans = new Collection<ProvisionedPlan>();

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		private Collection<ProvisioningError> _provisioningErrors = new Collection<ProvisioningError>();

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		private Collection<string> _proxyAddresses = new Collection<string>();

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		private SelfServePasswordResetData _selfServePasswordResetData;

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		private bool _selfServePasswordResetDataInitialized;

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		private string _signInName;

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		private string _sipProxyAddress;

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		private Collection<string> _smtpAddresses = new Collection<string>();

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		private string _state;

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		private string _streetAddress;

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		private string _surname;

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		private string _telephoneNumber;

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		private DataServiceStreamLink _thumbnailPhoto;

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		private string _usageLocation;

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		private string _userPrincipalName;

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		private string _userState;

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		private DateTime? _userStateChangedOn;

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		private string _userType;

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		private Collection<ImpersonationAccessGrant> _impersonationAccessGrants = new Collection<ImpersonationAccessGrant>();

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		private Collection<ServiceEndpoint> _serviceEndpoints = new Collection<ServiceEndpoint>();

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		private Collection<ServiceInfo> _serviceInfo = new Collection<ServiceInfo>();

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		private Collection<DirectoryObject> _registeredDevices = new Collection<DirectoryObject>();

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		private Collection<DirectoryObject> _ownedDevices = new Collection<DirectoryObject>();

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		private Collection<DirectAccessGrant> _directAccessGrants = new Collection<DirectAccessGrant>();

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		private Collection<DirectoryObject> _pendingMemberOf = new Collection<DirectoryObject>();

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		private Collection<DirectoryObject> _invitedBy = new Collection<DirectoryObject>();

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		private Collection<DirectoryObject> _invitedUsers = new Collection<DirectoryObject>();
	}
}
