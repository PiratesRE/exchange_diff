using System;
using System.CodeDom.Compiler;
using System.Collections.ObjectModel;
using System.Data.Services.Common;

namespace Microsoft.WindowsAzure.ActiveDirectory
{
	[DataServiceKey("objectId")]
	public class ServicePrincipal : DirectoryObject
	{
		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		public static ServicePrincipal CreateServicePrincipal(string objectId, Collection<AppPermission> appPermissions, bool explicitAccessGrantRequired, Collection<KeyCredential> keyCredentials, Collection<PasswordCredential> passwordCredentials, Collection<string> replyUrls, Collection<string> servicePrincipalNames, Collection<string> tags)
		{
			ServicePrincipal servicePrincipal = new ServicePrincipal();
			servicePrincipal.objectId = objectId;
			if (appPermissions == null)
			{
				throw new ArgumentNullException("appPermissions");
			}
			servicePrincipal.appPermissions = appPermissions;
			servicePrincipal.explicitAccessGrantRequired = explicitAccessGrantRequired;
			if (keyCredentials == null)
			{
				throw new ArgumentNullException("keyCredentials");
			}
			servicePrincipal.keyCredentials = keyCredentials;
			if (passwordCredentials == null)
			{
				throw new ArgumentNullException("passwordCredentials");
			}
			servicePrincipal.passwordCredentials = passwordCredentials;
			if (replyUrls == null)
			{
				throw new ArgumentNullException("replyUrls");
			}
			servicePrincipal.replyUrls = replyUrls;
			if (servicePrincipalNames == null)
			{
				throw new ArgumentNullException("servicePrincipalNames");
			}
			servicePrincipal.servicePrincipalNames = servicePrincipalNames;
			if (tags == null)
			{
				throw new ArgumentNullException("tags");
			}
			servicePrincipal.tags = tags;
			return servicePrincipal;
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
		public Guid? appId
		{
			get
			{
				return this._appId;
			}
			set
			{
				this._appId = value;
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
		public Guid? appOwnerTenantId
		{
			get
			{
				return this._appOwnerTenantId;
			}
			set
			{
				this._appOwnerTenantId = value;
			}
		}

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		public Collection<AppPermission> appPermissions
		{
			get
			{
				return this._appPermissions;
			}
			set
			{
				this._appPermissions = value;
			}
		}

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		public ServicePrincipalAuthenticationPolicy authenticationPolicy
		{
			get
			{
				if (this._authenticationPolicy == null && !this._authenticationPolicyInitialized)
				{
					this._authenticationPolicy = new ServicePrincipalAuthenticationPolicy();
					this._authenticationPolicyInitialized = true;
				}
				return this._authenticationPolicy;
			}
			set
			{
				this._authenticationPolicy = value;
				this._authenticationPolicyInitialized = true;
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
		public string errorUrl
		{
			get
			{
				return this._errorUrl;
			}
			set
			{
				this._errorUrl = value;
			}
		}

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		public bool explicitAccessGrantRequired
		{
			get
			{
				return this._explicitAccessGrantRequired;
			}
			set
			{
				this._explicitAccessGrantRequired = value;
			}
		}

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		public string homepage
		{
			get
			{
				return this._homepage;
			}
			set
			{
				this._homepage = value;
			}
		}

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		public Collection<KeyCredential> keyCredentials
		{
			get
			{
				return this._keyCredentials;
			}
			set
			{
				this._keyCredentials = value;
			}
		}

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		public string logoutUrl
		{
			get
			{
				return this._logoutUrl;
			}
			set
			{
				this._logoutUrl = value;
			}
		}

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		public Collection<PasswordCredential> passwordCredentials
		{
			get
			{
				return this._passwordCredentials;
			}
			set
			{
				this._passwordCredentials = value;
			}
		}

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		public string publisherName
		{
			get
			{
				return this._publisherName;
			}
			set
			{
				this._publisherName = value;
			}
		}

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		public Collection<string> replyUrls
		{
			get
			{
				return this._replyUrls;
			}
			set
			{
				this._replyUrls = value;
			}
		}

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		public string resourceApplicationSet
		{
			get
			{
				return this._resourceApplicationSet;
			}
			set
			{
				this._resourceApplicationSet = value;
			}
		}

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		public string samlMetadataUrl
		{
			get
			{
				return this._samlMetadataUrl;
			}
			set
			{
				this._samlMetadataUrl = value;
			}
		}

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		public Collection<string> servicePrincipalNames
		{
			get
			{
				return this._servicePrincipalNames;
			}
			set
			{
				this._servicePrincipalNames = value;
			}
		}

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		public Collection<string> tags
		{
			get
			{
				return this._tags;
			}
			set
			{
				this._tags = value;
			}
		}

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		public bool? webApi
		{
			get
			{
				return this._webApi;
			}
			set
			{
				this._webApi = value;
			}
		}

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		public bool? webApp
		{
			get
			{
				return this._webApp;
			}
			set
			{
				this._webApp = value;
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
		public Collection<DirectAccessGrant> directAccessGrantedTo
		{
			get
			{
				return this._directAccessGrantedTo;
			}
			set
			{
				if (value != null)
				{
					this._directAccessGrantedTo = value;
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
		private bool? _accountEnabled;

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		private Guid? _appId;

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		private AppMetadata _appMetadata;

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		private bool _appMetadataInitialized;

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		private Guid? _appOwnerTenantId;

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		private Collection<AppPermission> _appPermissions = new Collection<AppPermission>();

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		private ServicePrincipalAuthenticationPolicy _authenticationPolicy;

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		private bool _authenticationPolicyInitialized;

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		private string _displayName;

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		private string _errorUrl;

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		private bool _explicitAccessGrantRequired;

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		private string _homepage;

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		private Collection<KeyCredential> _keyCredentials = new Collection<KeyCredential>();

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		private string _logoutUrl;

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		private Collection<PasswordCredential> _passwordCredentials = new Collection<PasswordCredential>();

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		private string _publisherName;

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		private Collection<string> _replyUrls = new Collection<string>();

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		private string _resourceApplicationSet;

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		private string _samlMetadataUrl;

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		private Collection<string> _servicePrincipalNames = new Collection<string>();

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		private Collection<string> _tags = new Collection<string>();

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		private bool? _webApi;

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		private bool? _webApp;

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		private Collection<ImpersonationAccessGrant> _impersonationAccessGrants = new Collection<ImpersonationAccessGrant>();

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		private Collection<DirectAccessGrant> _directAccessGrants = new Collection<DirectAccessGrant>();

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		private Collection<DirectAccessGrant> _directAccessGrantedTo = new Collection<DirectAccessGrant>();

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		private Collection<ServiceEndpoint> _serviceEndpoints = new Collection<ServiceEndpoint>();
	}
}
