using System;
using System.CodeDom.Compiler;
using System.Collections.ObjectModel;
using System.Data.Services.Client;
using System.Data.Services.Common;

namespace Microsoft.WindowsAzure.ActiveDirectoryV122
{
	[DataServiceKey("objectId")]
	public class Application : DirectoryObject
	{
		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		public static Application CreateApplication(string objectId, Collection<string> identifierUris, Collection<KeyCredential> keyCredentials, DataServiceStreamLink mainLogo, Collection<PasswordCredential> passwordCredentials, Collection<string> replyUrls)
		{
			Application application = new Application();
			application.objectId = objectId;
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
			return application;
		}

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		public bool? allowActAsForAllClients
		{
			get
			{
				return this._allowActAsForAllClients;
			}
			set
			{
				this._allowActAsForAllClients = value;
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
		public string groupMembershipClaims
		{
			get
			{
				return this._groupMembershipClaims;
			}
			set
			{
				this._groupMembershipClaims = value;
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
		public Collection<AppLocalizedBranding> appLocalizedBranding
		{
			get
			{
				return this._appLocalizedBranding;
			}
			set
			{
				if (value != null)
				{
					this._appLocalizedBranding = value;
				}
			}
		}

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		public Collection<AppNonLocalizedBranding> appNonLocalizedBranding
		{
			get
			{
				return this._appNonLocalizedBranding;
			}
			set
			{
				if (value != null)
				{
					this._appNonLocalizedBranding = value;
				}
			}
		}

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		public Collection<ExtensionProperty> extensionProperties
		{
			get
			{
				return this._extensionProperties;
			}
			set
			{
				if (value != null)
				{
					this._extensionProperties = value;
				}
			}
		}

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		private bool? _allowActAsForAllClients;

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		private Guid? _appId;

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		private bool? _availableToOtherTenants;

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		private string _displayName;

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		private string _errorUrl;

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		private string _groupMembershipClaims;

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		private string _homepage;

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		private Collection<string> _identifierUris = new Collection<string>();

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		private Collection<KeyCredential> _keyCredentials = new Collection<KeyCredential>();

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
		private string _samlMetadataUrl;

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		private Collection<AppLocalizedBranding> _appLocalizedBranding = new Collection<AppLocalizedBranding>();

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		private Collection<AppNonLocalizedBranding> _appNonLocalizedBranding = new Collection<AppNonLocalizedBranding>();

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		private Collection<ExtensionProperty> _extensionProperties = new Collection<ExtensionProperty>();
	}
}
