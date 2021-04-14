using System;
using System.Net;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.InfoWorker.Common.Availability
{
	internal sealed class TargetForestConfiguration
	{
		public string Id
		{
			get
			{
				return this.id;
			}
		}

		public NetworkCredential Credentials
		{
			get
			{
				return this.networkCredential;
			}
		}

		public string FullDnsDomainName
		{
			get
			{
				return this.fullDnsDomainName;
			}
		}

		public AvailabilityAccessMethod AccessMethod
		{
			get
			{
				return this.accessMethod;
			}
		}

		public bool IsPerUserAuthorizationSupported
		{
			get
			{
				return this.accessMethod == AvailabilityAccessMethod.PerUserFB;
			}
		}

		public Uri AutoDiscoverUrl
		{
			get
			{
				return this.autoDiscoverUrl;
			}
		}

		public AutodiscoverUrlSource AutodiscoverUrlSource
		{
			get
			{
				return this.autodiscoverUrlSource;
			}
		}

		public LocalizedException Exception
		{
			get
			{
				return this.exception;
			}
		}

		public void SetAutodiscoverUrl(Uri autoDiscoverUrl, AutodiscoverUrlSource autodiscoverUrlSource)
		{
			this.autoDiscoverUrl = autoDiscoverUrl;
			this.autodiscoverUrlSource = autodiscoverUrlSource;
		}

		internal TargetForestConfiguration(string id, string fullDnsDomainName, AvailabilityAccessMethod accessMethod, NetworkCredential networkCredential, Uri autoDiscoverUrl, AutodiscoverUrlSource autodiscoverUrlSource)
		{
			this.id = id;
			this.fullDnsDomainName = fullDnsDomainName;
			this.accessMethod = accessMethod;
			this.networkCredential = networkCredential;
			this.autoDiscoverUrl = autoDiscoverUrl;
			this.autodiscoverUrlSource = autodiscoverUrlSource;
		}

		internal TargetForestConfiguration(string id, string fullDnsDomainName, LocalizedException exception)
		{
			this.id = id;
			this.fullDnsDomainName = fullDnsDomainName;
			this.exception = exception;
		}

		internal CredentialCache GetCredentialCache(Uri uri)
		{
			CredentialCache credentialCache = null;
			if (this.AccessMethod == AvailabilityAccessMethod.OrgWideFBBasic && this.Credentials != null)
			{
				credentialCache = new CredentialCache();
				credentialCache.Add(uri, "Basic", this.Credentials);
			}
			return credentialCache;
		}

		internal ProxyAuthenticator GetProxyAuthenticatorForAutoDiscover(Uri uri, SerializedSecurityContext serializedSecurityContext, string messageId)
		{
			if (this.AccessMethod == AvailabilityAccessMethod.OrgWideFBBasic && this.Credentials != null)
			{
				return ProxyAuthenticator.Create(new CredentialCache
				{
					{
						uri,
						"Basic",
						this.Credentials
					}
				}, serializedSecurityContext, messageId);
			}
			return ProxyAuthenticator.Create(this.Credentials, serializedSecurityContext, messageId);
		}

		private LocalizedException exception;

		private string id;

		private string fullDnsDomainName;

		private AvailabilityAccessMethod accessMethod;

		private NetworkCredential networkCredential;

		private Uri autoDiscoverUrl;

		private AutodiscoverUrlSource autodiscoverUrlSource;
	}
}
