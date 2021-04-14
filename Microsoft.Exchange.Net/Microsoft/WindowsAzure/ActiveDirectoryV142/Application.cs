using System;
using System.CodeDom.Compiler;
using System.Collections.ObjectModel;
using System.Data.Services.Client;
using System.Data.Services.Common;

namespace Microsoft.WindowsAzure.ActiveDirectoryV142
{
	[DataServiceKey("objectId")]
	public class Application : DirectoryObject
	{
		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		public static Application CreateApplication(string objectId, Collection<AppPermission> appPermissions, Collection<string> identifierUris, Collection<KeyCredential> keyCredentials, Collection<Guid> knownClientApplications, DataServiceStreamLink mainLogo, Collection<PasswordCredential> passwordCredentials, Collection<string> replyUrls, Collection<RequiredResourceAccess> requiredResourceAccess)
		{
			Application application = new Application();
			application.objectId = objectId;
			if (appPermissions == null)
			{
				throw new ArgumentNullException("appPermissions");
			}
			application.appPermissions = appPermissions;
			if (identifierUris == null)
			{
				throw new ArgumentNullException("identifierUris");
			}
			application.identifierUris = identifierUris;
			if (keyCredentials == null)
			{
				throw new ArgumentNullException("keyCredentials");
			}
			application.keyCredentials = keyCredentials;
			if (knownClientApplications == null)
			{
				throw new ArgumentNullException("knownClientApplications");
			}
			application.knownClientApplications = knownClientApplications;
			application.mainLogo = mainLogo;
			if (passwordCredentials == null)
			{
				throw new ArgumentNullException("passwordCredentials");
			}
			application.passwordCredentials = passwordCredentials;
			if (replyUrls == null)
			{
				throw new ArgumentNullException("replyUrls");
			}
			application.replyUrls = replyUrls;
			if (requiredResourceAccess == null)
			{
				throw new ArgumentNullException("requiredResourceAccess");
			}
			application.requiredResourceAccess = requiredResourceAccess;
			return application;
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
		public bool? availableToOtherTenants
		{
			get
			{
				return this._availableToOtherTenants;
			}
			set
			{
				this._availableToOtherTenants = value;
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
		public Collection<string> identifierUris
		{
			get
			{
				return this._identifierUris;
			}
			set
			{
				this._identifierUris = value;
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
		public Collection<Guid> knownClientApplications
		{
			get
			{
				return this._knownClientApplications;
			}
			set
			{
				this._knownClientApplications = value;
			}
		}

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		public DataServiceStreamLink mainLogo
		{
			get
			{
				return this._mainLogo;
			}
			set
			{
				this._mainLogo = value;
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
		public bool? publicClient
		{
			get
			{
				return this._publicClient;
			}
			set
			{
				this._publicClient = value;
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
		public Collection<RequiredResourceAccess> requiredResourceAccess
		{
			get
			{
				return this._requiredResourceAccess;
			}
			set
			{
				this._requiredResourceAccess = value;
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
		public Collection<Notification> notifications
		{
			get
			{
				return this._notifications;
			}
			set
			{
				if (value != null)
				{
					this._notifications = value;
				}
			}
		}

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		public Collection<DirectoryObject> defaultPolicy
		{
			get
			{
				return this._defaultPolicy;
			}
			set
			{
				if (value != null)
				{
					this._defaultPolicy = value;
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
		private Guid? _appId;

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		private AppMetadata _appMetadata;

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		private bool _appMetadataInitialized;

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		private Collection<AppPermission> _appPermissions = new Collection<AppPermission>();

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		private bool? _availableToOtherTenants;

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		private string _displayName;

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		private string _errorUrl;

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		private string _homepage;

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		private Collection<string> _identifierUris = new Collection<string>();

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		private Collection<KeyCredential> _keyCredentials = new Collection<KeyCredential>();

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		private Collection<Guid> _knownClientApplications = new Collection<Guid>();

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		private DataServiceStreamLink _mainLogo;

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		private string _logoutUrl;

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		private Collection<PasswordCredential> _passwordCredentials = new Collection<PasswordCredential>();

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		private bool? _publicClient;

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		private Collection<string> _replyUrls = new Collection<string>();

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		private Collection<RequiredResourceAccess> _requiredResourceAccess = new Collection<RequiredResourceAccess>();

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		private string _resourceApplicationSet;

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		private string _samlMetadataUrl;

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		private bool? _webApi;

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		private bool? _webApp;

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		private Collection<Notification> _notifications = new Collection<Notification>();

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		private Collection<DirectoryObject> _defaultPolicy = new Collection<DirectoryObject>();

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		private Collection<ServiceEndpoint> _serviceEndpoints = new Collection<ServiceEndpoint>();
	}
}
