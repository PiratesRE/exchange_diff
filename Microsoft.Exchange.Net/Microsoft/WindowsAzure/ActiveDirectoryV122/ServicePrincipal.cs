using System;
using System.CodeDom.Compiler;
using System.Collections.ObjectModel;
using System.Data.Services.Common;

namespace Microsoft.WindowsAzure.ActiveDirectoryV122
{
	[DataServiceKey("objectId")]
	public class ServicePrincipal : DirectoryObject
	{
		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		public static ServicePrincipal CreateServicePrincipal(string objectId, Collection<KeyCredential> keyCredentials, Collection<PasswordCredential> passwordCredentials, Collection<string> replyUrls, Collection<string> servicePrincipalNames, Collection<string> tags)
		{
			ServicePrincipal servicePrincipal = new ServicePrincipal();
			servicePrincipal.objectId = objectId;
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
		public string preferredTokenSigningKeyThumbprint
		{
			get
			{
				return this._preferredTokenSigningKeyThumbprint;
			}
			set
			{
				this._preferredTokenSigningKeyThumbprint = value;
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
		public Collection<Permission> permissions
		{
			get
			{
				return this._permissions;
			}
			set
			{
				if (value != null)
				{
					this._permissions = value;
				}
			}
		}

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		private bool? _accountEnabled;

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		private Guid? _appId;

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		private Guid? _appOwnerTenantId;

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		private ServicePrincipalAuthenticationPolicy _authenticationPolicy;

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		private bool _authenticationPolicyInitialized;

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		private string _displayName;

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		private string _errorUrl;

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		private string _homepage;

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		private Collection<KeyCredential> _keyCredentials = new Collection<KeyCredential>();

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		private string _logoutUrl;

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		private Collection<PasswordCredential> _passwordCredentials = new Collection<PasswordCredential>();

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		private string _preferredTokenSigningKeyThumbprint;

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		private string _publisherName;

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		private Collection<string> _replyUrls = new Collection<string>();

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		private string _samlMetadataUrl;

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		private Collection<string> _servicePrincipalNames = new Collection<string>();

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		private Collection<string> _tags = new Collection<string>();

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		private Collection<Permission> _permissions = new Collection<Permission>();
	}
}
