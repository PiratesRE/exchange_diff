using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Security.Cryptography;
using Microsoft.Exchange.Common.Cache;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Data.ApplicationLogic;
using Microsoft.Exchange.Extensions;
using Microsoft.Exchange.Net;
using Microsoft.Exchange.Security.Dkm;

namespace Microsoft.Exchange.Data.ApplicationLogic
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class CachedPeopleConnectApplicationConfig
	{
		internal CachedPeopleConnectApplicationConfig(IPeopleConnectApplicationConfigCache cache, IPeopleConnectApplicationConfigADReader reader)
		{
			if (cache == null)
			{
				throw new ArgumentNullException("cache");
			}
			if (reader == null)
			{
				throw new ArgumentNullException("reader");
			}
			this.appADConfigCache = cache;
			this.appADConfigReader = reader;
		}

		public static CachedPeopleConnectApplicationConfig Instance
		{
			get
			{
				return CachedPeopleConnectApplicationConfig.instance;
			}
		}

		public IPeopleConnectApplicationConfig ReadLinkedIn()
		{
			return this.ReadProvider("linkedin");
		}

		public IPeopleConnectApplicationConfig ReadFacebook()
		{
			return this.ReadProvider("facebook");
		}

		public IPeopleConnectApplicationConfig ReadProvider(string provider)
		{
			if (string.IsNullOrEmpty(provider))
			{
				throw new ArgumentNullException("provider");
			}
			try
			{
				string a;
				if ((a = provider.Trim().ToLowerInvariant()) != null)
				{
					if (a == "facebook")
					{
						return this.RetrieveCachedFacebookADConfig().OverrideWith(this.RetrieveCachedWebProxyADConfig()).OverrideWith(this.ReadFacebookConfigFromRegistry());
					}
					if (a == "linkedin")
					{
						return this.RetrieveCachedLinkedInADConfig().OverrideWith(this.RetrieveCachedWebProxyADConfig()).OverrideWith(this.ReadLinkedInConfigFromRegistry());
					}
				}
				throw new ArgumentOutOfRangeException("provider");
			}
			catch (AuthServerNotFoundException innerException)
			{
				throw new ExchangeConfigurationException(Strings.FailedToReadProviderConfigurationSeeInnerException(provider), innerException);
			}
			catch (InvalidAuthConfigurationException innerException2)
			{
				throw new ExchangeConfigurationException(Strings.FailedToReadProviderConfigurationSeeInnerException(provider), innerException2);
			}
			catch (ServiceEndpointNotFoundException innerException3)
			{
				throw new ExchangeConfigurationException(Strings.FailedToReadProviderConfigurationSeeInnerException(provider), innerException3);
			}
			catch (EndpointContainerNotFoundException innerException4)
			{
				throw new ExchangeConfigurationException(Strings.FailedToReadProviderConfigurationSeeInnerException(provider), innerException4);
			}
			catch (ADTransientException innerException5)
			{
				throw new ExchangeConfigurationException(Strings.FailedToReadProviderConfigurationSeeInnerException(provider), innerException5);
			}
			catch (ADOperationException innerException6)
			{
				throw new ExchangeConfigurationException(Strings.FailedToReadProviderConfigurationSeeInnerException(provider), innerException6);
			}
			catch (CryptographicException innerException7)
			{
				throw new ExchangeConfigurationException(Strings.FailedToReadProviderConfigurationSeeInnerException(provider), innerException7);
			}
			catch (ArgumentOutOfRangeException)
			{
				throw;
			}
			catch (ExchangeConfigurationException)
			{
				throw;
			}
			catch (Exception ex)
			{
				if (CachedPeopleConnectApplicationConfig.Dkm.IsDkmException(ex))
				{
					throw new ExchangeConfigurationException(Strings.FailedToReadProviderConfigurationSeeInnerException(provider), ex);
				}
				throw;
			}
			IPeopleConnectApplicationConfig result;
			return result;
		}

		private IPeopleConnectApplicationConfig RetrieveCachedLinkedInADConfig()
		{
			if (PeopleConnectRegistryReader.Read().DogfoodInEnterprise)
			{
				return new CachedPeopleConnectApplicationConfig.PeopleConnectApplicationConfig();
			}
			IPeopleConnectApplicationConfig peopleConnectApplicationConfig;
			if (this.appADConfigCache.TryGetValue("linkedin", out peopleConnectApplicationConfig))
			{
				return peopleConnectApplicationConfig;
			}
			peopleConnectApplicationConfig = this.ReadLinkedInConfigFromAD();
			this.ValidateLinkedInConfig(peopleConnectApplicationConfig);
			this.appADConfigCache.Add("linkedin", peopleConnectApplicationConfig);
			return peopleConnectApplicationConfig;
		}

		private IPeopleConnectApplicationConfig RetrieveCachedFacebookADConfig()
		{
			if (PeopleConnectRegistryReader.Read().DogfoodInEnterprise)
			{
				return new CachedPeopleConnectApplicationConfig.PeopleConnectApplicationConfig();
			}
			IPeopleConnectApplicationConfig peopleConnectApplicationConfig;
			if (this.appADConfigCache.TryGetValue("facebook", out peopleConnectApplicationConfig))
			{
				return peopleConnectApplicationConfig;
			}
			peopleConnectApplicationConfig = this.ReadFacebookConfigFromAD();
			this.ValidateFacebookConfig(peopleConnectApplicationConfig);
			this.appADConfigCache.Add("facebook", peopleConnectApplicationConfig);
			return peopleConnectApplicationConfig;
		}

		private CachedPeopleConnectApplicationConfig.PeopleConnectApplicationConfig ReadLinkedInConfigFromAD()
		{
			AuthServer authServer = this.appADConfigReader.ReadLinkedInAuthServer();
			return new CachedPeopleConnectApplicationConfig.PeopleConnectApplicationConfig
			{
				AccessTokenEndpoint = authServer.TokenIssuingEndpoint,
				AppId = authServer.ApplicationIdentifier,
				AppSecretEncrypted = authServer.CurrentEncryptedAppSecret,
				DecryptAppSecret = new Func<string, string>(CachedPeopleConnectApplicationConfig.DecryptAppSecretWithDkm),
				RequestTokenEndpoint = authServer.AuthorizationEndpoint,
				ProfileEndpoint = this.appADConfigReader.ReadLinkedInProfileEndpoint(),
				ConnectionsEndpoint = this.appADConfigReader.ReadLinkedInConnectionsEndpoint(),
				RemoveAppEndpoint = this.appADConfigReader.ReadLinkedInInvalidateTokenEndpoint(),
				ReadTimeUtc = DateTime.UtcNow
			};
		}

		private CachedPeopleConnectApplicationConfig.PeopleConnectApplicationConfig ReadFacebookConfigFromAD()
		{
			AuthServer authServer = this.appADConfigReader.ReadFacebookAuthServer();
			return new CachedPeopleConnectApplicationConfig.PeopleConnectApplicationConfig
			{
				AppId = authServer.ApplicationIdentifier,
				AppSecretEncrypted = authServer.CurrentEncryptedAppSecret,
				DecryptAppSecret = new Func<string, string>(CachedPeopleConnectApplicationConfig.DecryptAppSecretWithDkm),
				AuthorizationEndpoint = authServer.AuthorizationEndpoint,
				GraphTokenEndpoint = authServer.TokenIssuingEndpoint,
				GraphApiEndpoint = this.appADConfigReader.ReadFacebookGraphApiEndpoint(),
				ReadTimeUtc = DateTime.UtcNow
			};
		}

		private CachedPeopleConnectApplicationConfig.PeopleConnectApplicationConfig ReadLinkedInConfigFromRegistry()
		{
			LinkedInRegistryReader linkedInRegistryReader = LinkedInRegistryReader.Read();
			return new CachedPeopleConnectApplicationConfig.PeopleConnectApplicationConfig
			{
				AccessTokenEndpoint = linkedInRegistryReader.AccessTokenEndpoint,
				AppId = linkedInRegistryReader.AppId,
				AppSecretEncrypted = linkedInRegistryReader.AppSecret,
				DecryptAppSecret = new Func<string, string>(CachedPeopleConnectApplicationConfig.NoOpDecrypt),
				ConnectionsEndpoint = linkedInRegistryReader.ConnectionsEndpoint,
				ConsentRedirectEndpoint = linkedInRegistryReader.ConsentRedirectEndpoint,
				ProfileEndpoint = linkedInRegistryReader.ProfileEndpoint,
				RequestTokenEndpoint = linkedInRegistryReader.RequestTokenEndpoint,
				WebRequestTimeout = linkedInRegistryReader.WebRequestTimeout,
				RemoveAppEndpoint = linkedInRegistryReader.RemoveAppEndpoint,
				WebProxyUri = linkedInRegistryReader.WebProxyUri,
				ReadTimeUtc = DateTime.UtcNow
			};
		}

		private CachedPeopleConnectApplicationConfig.PeopleConnectApplicationConfig ReadFacebookConfigFromRegistry()
		{
			FacebookRegistryReader facebookRegistryReader = FacebookRegistryReader.Read();
			return new CachedPeopleConnectApplicationConfig.PeopleConnectApplicationConfig
			{
				AppId = facebookRegistryReader.AppId,
				AppSecretEncrypted = facebookRegistryReader.AppSecret,
				ConsentRedirectEndpoint = facebookRegistryReader.ConsentRedirectEndpoint,
				DecryptAppSecret = new Func<string, string>(CachedPeopleConnectApplicationConfig.NoOpDecrypt),
				AuthorizationEndpoint = facebookRegistryReader.AuthorizationEndpoint,
				GraphApiEndpoint = facebookRegistryReader.GraphApiEndpoint,
				GraphTokenEndpoint = facebookRegistryReader.GraphTokenEndpoint,
				WebRequestTimeout = facebookRegistryReader.WebRequestTimeout,
				SkipContactUpload = facebookRegistryReader.SkipContactUpload,
				ContinueOnContactUploadFailure = facebookRegistryReader.ContinueOnContactUploadFailure,
				WaitForContactUploadCommit = facebookRegistryReader.WaitForContactUploadCommit,
				NotifyOnEachContactUpload = facebookRegistryReader.NotifyOnEachContactUpload,
				MaximumContactsToUpload = facebookRegistryReader.MaximumContactsToUpload,
				ReadTimeUtc = DateTime.UtcNow
			};
		}

		private CachedPeopleConnectApplicationConfig.PeopleConnectApplicationConfig RetrieveCachedWebProxyADConfig()
		{
			return new CachedPeopleConnectApplicationConfig.PeopleConnectApplicationConfig
			{
				WebProxyUri = this.appADConfigReader.ReadWebProxyUri()
			};
		}

		private void ValidateLinkedInConfig(IPeopleConnectApplicationConfig config)
		{
			CachedPeopleConnectApplicationConfig.ThrowExchangeConfigurationExceptionIfBlank(config.AccessTokenEndpoint, Strings.InvalidConfigurationMissingLinkedInAccessTokenEndpoint);
			CachedPeopleConnectApplicationConfig.ThrowExchangeConfigurationExceptionIfBlank(config.AppId, Strings.InvalidConfigurationMissingLinkedInAppId);
			CachedPeopleConnectApplicationConfig.ThrowExchangeConfigurationExceptionIfMatches(config.AppId, CachedPeopleConnectApplicationConfig.KnownInvalidAppIds, StringComparer.OrdinalIgnoreCase, Strings.InvalidConfigurationLinkedInAppId);
			CachedPeopleConnectApplicationConfig.ThrowExchangeConfigurationExceptionIfBlank(config.AppSecretEncrypted, Strings.InvalidConfigurationMissingLinkedInAppSecret);
			CachedPeopleConnectApplicationConfig.ThrowExchangeConfigurationExceptionIfMatches(config.AppSecretClearText, CachedPeopleConnectApplicationConfig.KnownInvalidAppSecrets, StringComparer.OrdinalIgnoreCase, Strings.InvalidConfigurationLinkedInAppSecret);
			CachedPeopleConnectApplicationConfig.ThrowExchangeConfigurationExceptionIfBlank(config.RequestTokenEndpoint, Strings.InvalidConfigurationMissingLinkedInRequestTokenEndpoint);
			CachedPeopleConnectApplicationConfig.ThrowExchangeConfigurationExceptionIfBlank(config.ConnectionsEndpoint, Strings.InvalidConfigurationMissingLinkedInConnectionsEndpoint);
			CachedPeopleConnectApplicationConfig.ThrowExchangeConfigurationExceptionIfBlank(config.ProfileEndpoint, Strings.InvalidConfigurationMissingLinkedInProfileEndpoint);
			CachedPeopleConnectApplicationConfig.ThrowExchangeConfigurationExceptionIfBlank(config.RemoveAppEndpoint, Strings.InvalidConfigurationMissingLinkedInInvalidateTokenEndpoint);
		}

		private void ValidateFacebookConfig(IPeopleConnectApplicationConfig config)
		{
			CachedPeopleConnectApplicationConfig.ThrowExchangeConfigurationExceptionIfBlank(config.AppId, Strings.InvalidConfigurationMissingFacebookAppId);
			CachedPeopleConnectApplicationConfig.ThrowExchangeConfigurationExceptionIfMatches(config.AppId, CachedPeopleConnectApplicationConfig.KnownInvalidAppIds, StringComparer.OrdinalIgnoreCase, Strings.InvalidConfigurationFacebookAppId);
			CachedPeopleConnectApplicationConfig.ThrowExchangeConfigurationExceptionIfBlank(config.AppSecretEncrypted, Strings.InvalidConfigurationMissingFacebookAppSecret);
			CachedPeopleConnectApplicationConfig.ThrowExchangeConfigurationExceptionIfMatches(config.AppSecretClearText, CachedPeopleConnectApplicationConfig.KnownInvalidAppSecrets, StringComparer.OrdinalIgnoreCase, Strings.InvalidConfigurationFacebookAppSecret);
			CachedPeopleConnectApplicationConfig.ThrowExchangeConfigurationExceptionIfBlank(config.AuthorizationEndpoint, Strings.InvalidConfigurationMissingFacebookAuthorizationEndpoint);
			CachedPeopleConnectApplicationConfig.ThrowExchangeConfigurationExceptionIfBlank(config.GraphTokenEndpoint, Strings.InvalidConfigurationMissingFacebookGraphTokenEndpoint);
			CachedPeopleConnectApplicationConfig.ThrowExchangeConfigurationExceptionIfBlank(config.GraphApiEndpoint, Strings.InvalidConfigurationMissingFacebookGraphApiEndpoint);
		}

		private static void ThrowExchangeConfigurationExceptionIfBlank(string s, LocalizedString errorMessage)
		{
			if (string.IsNullOrWhiteSpace(s))
			{
				throw new ExchangeConfigurationException(errorMessage);
			}
		}

		private static void ThrowExchangeConfigurationExceptionIfMatches(string s, IEnumerable<string> matches, StringComparer comparer, LocalizedString errorMessage)
		{
			if (matches == null)
			{
				return;
			}
			if (matches.Contains(s, comparer))
			{
				throw new ExchangeConfigurationException(errorMessage);
			}
		}

		private static string DecryptAppSecretWithDkm(string encryptedAppSecret)
		{
			string result;
			using (SecureString secureString = CachedPeopleConnectApplicationConfig.Dkm.EncryptedStringToSecureString(encryptedAppSecret))
			{
				result = secureString.AsUnsecureString();
			}
			return result;
		}

		private static string NoOpDecrypt(string secret)
		{
			return secret;
		}

		private const string FacebookLowerCase = "facebook";

		private const string LinkedInLowerCase = "linkedin";

		private static readonly Trace Tracer = ExTraceGlobals.PeopleConnectConfigurationTracer;

		private static readonly CachedPeopleConnectApplicationConfig instance = new CachedPeopleConnectApplicationConfig(new CachedPeopleConnectApplicationConfig.PeopleConnectApplicationConfigCacheImpl(), new CachedPeopleConnectApplicationConfig.PeopleConnectApplicationConfigADReaderImpl());

		private static readonly IExchangeGroupKey Dkm = PeopleConnectExchangeGroupKeyFactory.Create();

		private static readonly string[] KnownInvalidAppIds = new string[]
		{
			"0"
		};

		private static readonly string[] KnownInvalidAppSecrets = new string[]
		{
			"%Prod_PeopleConnectFacebookAppSecret%",
			"%Prod_PeopleConnectLinkedInAppSecret%",
			"%Gallatin_PeopleConnectLinkedInAppSecret%"
		};

		private readonly IPeopleConnectApplicationConfigCache appADConfigCache;

		private readonly IPeopleConnectApplicationConfigADReader appADConfigReader;

		private sealed class PeopleConnectApplicationConfig : CachableItem, IPeopleConnectApplicationConfig
		{
			public string AppId { get; internal set; }

			public string AppSecretEncrypted { get; internal set; }

			public Func<string, string> DecryptAppSecret { get; internal set; }

			public string AppSecretClearText
			{
				get
				{
					return this.DecryptAppSecret(this.AppSecretEncrypted);
				}
			}

			public string AuthorizationEndpoint { get; internal set; }

			public string GraphTokenEndpoint { get; internal set; }

			public string GraphApiEndpoint { get; internal set; }

			public string RequestTokenEndpoint { get; internal set; }

			public string AccessTokenEndpoint { get; internal set; }

			public string ProfileEndpoint { get; internal set; }

			public string ConnectionsEndpoint { get; internal set; }

			public string RemoveAppEndpoint { get; internal set; }

			public string ConsentRedirectEndpoint { get; internal set; }

			public TimeSpan WebRequestTimeout { get; internal set; }

			public string WebProxyUri { get; internal set; }

			public bool SkipContactUpload { get; internal set; }

			public bool ContinueOnContactUploadFailure { get; internal set; }

			public bool WaitForContactUploadCommit { get; internal set; }

			public bool NotifyOnEachContactUpload { get; internal set; }

			public int MaximumContactsToUpload { get; internal set; }

			public DateTime ReadTimeUtc { get; internal set; }

			public override long ItemSize
			{
				get
				{
					return 1L;
				}
			}

			public IPeopleConnectApplicationConfig OverrideWith(IPeopleConnectApplicationConfig other)
			{
				if (other == null)
				{
					return this;
				}
				if (this.Equals(other))
				{
					return this;
				}
				return new CachedPeopleConnectApplicationConfig.PeopleConnectApplicationConfig
				{
					AccessTokenEndpoint = CachedPeopleConnectApplicationConfig.PeopleConnectApplicationConfig.OverrideWith(this.AccessTokenEndpoint, other.AccessTokenEndpoint),
					AppId = CachedPeopleConnectApplicationConfig.PeopleConnectApplicationConfig.OverrideWith(this.AppId, other.AppId),
					AppSecretEncrypted = CachedPeopleConnectApplicationConfig.PeopleConnectApplicationConfig.OverrideWith<Func<string, string>>(Tuple.Create<string, Func<string, string>>(this.AppSecretEncrypted, this.DecryptAppSecret), Tuple.Create<string, Func<string, string>>(other.AppSecretEncrypted, other.DecryptAppSecret)).Item1,
					DecryptAppSecret = CachedPeopleConnectApplicationConfig.PeopleConnectApplicationConfig.OverrideWith<Func<string, string>>(Tuple.Create<string, Func<string, string>>(this.AppSecretEncrypted, this.DecryptAppSecret), Tuple.Create<string, Func<string, string>>(other.AppSecretEncrypted, other.DecryptAppSecret)).Item2,
					AuthorizationEndpoint = CachedPeopleConnectApplicationConfig.PeopleConnectApplicationConfig.OverrideWith(this.AuthorizationEndpoint, other.AuthorizationEndpoint),
					ProfileEndpoint = CachedPeopleConnectApplicationConfig.PeopleConnectApplicationConfig.OverrideWith(this.ProfileEndpoint, other.ProfileEndpoint),
					ConnectionsEndpoint = CachedPeopleConnectApplicationConfig.PeopleConnectApplicationConfig.OverrideWith(this.ConnectionsEndpoint, other.ConnectionsEndpoint),
					RemoveAppEndpoint = CachedPeopleConnectApplicationConfig.PeopleConnectApplicationConfig.OverrideWith(this.RemoveAppEndpoint, other.RemoveAppEndpoint),
					ConsentRedirectEndpoint = CachedPeopleConnectApplicationConfig.PeopleConnectApplicationConfig.OverrideWith(this.ConsentRedirectEndpoint, other.ConsentRedirectEndpoint),
					GraphApiEndpoint = CachedPeopleConnectApplicationConfig.PeopleConnectApplicationConfig.OverrideWith(this.GraphApiEndpoint, other.GraphApiEndpoint),
					GraphTokenEndpoint = CachedPeopleConnectApplicationConfig.PeopleConnectApplicationConfig.OverrideWith(this.GraphTokenEndpoint, other.GraphTokenEndpoint),
					RequestTokenEndpoint = CachedPeopleConnectApplicationConfig.PeopleConnectApplicationConfig.OverrideWith(this.RequestTokenEndpoint, other.RequestTokenEndpoint),
					WebProxyUri = CachedPeopleConnectApplicationConfig.PeopleConnectApplicationConfig.OverrideWith(this.WebProxyUri, other.WebProxyUri),
					WebRequestTimeout = CachedPeopleConnectApplicationConfig.PeopleConnectApplicationConfig.OverrideWith(this.WebRequestTimeout, other.WebRequestTimeout),
					ReadTimeUtc = CachedPeopleConnectApplicationConfig.PeopleConnectApplicationConfig.OverrideWith(this.ReadTimeUtc, other.ReadTimeUtc),
					SkipContactUpload = CachedPeopleConnectApplicationConfig.PeopleConnectApplicationConfig.OverrideWith<bool>(new bool?(this.SkipContactUpload), new bool?(other.SkipContactUpload)),
					ContinueOnContactUploadFailure = CachedPeopleConnectApplicationConfig.PeopleConnectApplicationConfig.OverrideWith<bool>(new bool?(this.ContinueOnContactUploadFailure), new bool?(other.ContinueOnContactUploadFailure)),
					WaitForContactUploadCommit = CachedPeopleConnectApplicationConfig.PeopleConnectApplicationConfig.OverrideWith<bool>(new bool?(this.WaitForContactUploadCommit), new bool?(other.WaitForContactUploadCommit)),
					NotifyOnEachContactUpload = CachedPeopleConnectApplicationConfig.PeopleConnectApplicationConfig.OverrideWith<bool>(new bool?(this.NotifyOnEachContactUpload), new bool?(other.NotifyOnEachContactUpload)),
					MaximumContactsToUpload = CachedPeopleConnectApplicationConfig.PeopleConnectApplicationConfig.OverrideWith<int>(new int?(this.MaximumContactsToUpload), new int?(other.MaximumContactsToUpload))
				};
			}

			private static string OverrideWith(string first, string second)
			{
				if (!string.IsNullOrWhiteSpace(second))
				{
					return second;
				}
				return first;
			}

			private static T OverrideWith<T>(T? first, T? second) where T : struct
			{
				if (second != null)
				{
					return second.Value;
				}
				T? t = first;
				if (t == null)
				{
					return default(T);
				}
				return t.GetValueOrDefault();
			}

			private static TimeSpan OverrideWith(TimeSpan first, TimeSpan second)
			{
				if (!TimeSpan.Zero.Equals(second))
				{
					return second;
				}
				return first;
			}

			private static DateTime OverrideWith(DateTime first, DateTime second)
			{
				if (!DateTime.MinValue.Equals(second))
				{
					return second;
				}
				return first;
			}

			private static Tuple<string, T> OverrideWith<T>(Tuple<string, T> first, Tuple<string, T> second)
			{
				if (!string.IsNullOrWhiteSpace(second.Item1))
				{
					return second;
				}
				return first;
			}
		}

		private sealed class PeopleConnectApplicationConfigCacheImpl : IPeopleConnectApplicationConfigCache
		{
			public bool TryGetValue(string key, out IPeopleConnectApplicationConfig value)
			{
				CachedPeopleConnectApplicationConfig.PeopleConnectApplicationConfig peopleConnectApplicationConfig;
				bool result = this.cache.TryGetValue(key, out peopleConnectApplicationConfig);
				value = peopleConnectApplicationConfig;
				return result;
			}

			public void Add(string key, IPeopleConnectApplicationConfig value)
			{
				this.cache.Add(key, (CachedPeopleConnectApplicationConfig.PeopleConnectApplicationConfig)value);
			}

			private readonly Cache<string, CachedPeopleConnectApplicationConfig.PeopleConnectApplicationConfig> cache = new Cache<string, CachedPeopleConnectApplicationConfig.PeopleConnectApplicationConfig>(64L, TimeSpan.FromHours(8.0), TimeSpan.Zero);
		}

		private sealed class PeopleConnectApplicationConfigADReaderImpl : IPeopleConnectApplicationConfigADReader
		{
			public AuthServer ReadFacebookAuthServer()
			{
				return OAuthConfigHelper.GetFacebookAuthServer();
			}

			public AuthServer ReadLinkedInAuthServer()
			{
				return OAuthConfigHelper.GetLinkedInAuthServer();
			}

			public string ReadWebProxyUri()
			{
				Server localServer = LocalServerCache.LocalServer;
				if (localServer == null)
				{
					CachedPeopleConnectApplicationConfig.Tracer.TraceError(0L, "Could not read proxy configuration from server object because there was a problem.");
					throw new ExchangeConfigurationException(Strings.FailedToReadWebProxyConfigurationFromAD);
				}
				if (!(localServer.InternetWebProxy != null))
				{
					return string.Empty;
				}
				return localServer.InternetWebProxy.AbsoluteUri;
			}

			public string ReadFacebookGraphApiEndpoint()
			{
				return CachedPeopleConnectApplicationConfig.PeopleConnectApplicationConfigADReaderImpl.GetEndpoint(ServiceEndpointId.FacebookGraphApi);
			}

			public string ReadLinkedInProfileEndpoint()
			{
				return CachedPeopleConnectApplicationConfig.PeopleConnectApplicationConfigADReaderImpl.GetEndpoint(ServiceEndpointId.LinkedInProfile);
			}

			public string ReadLinkedInConnectionsEndpoint()
			{
				return CachedPeopleConnectApplicationConfig.PeopleConnectApplicationConfigADReaderImpl.GetEndpoint(ServiceEndpointId.LinkedInConnections);
			}

			public string ReadLinkedInInvalidateTokenEndpoint()
			{
				return CachedPeopleConnectApplicationConfig.PeopleConnectApplicationConfigADReaderImpl.GetEndpoint(ServiceEndpointId.LinkedInInvalidateToken);
			}

			private static string GetEndpoint(string endpointCommonName)
			{
				ITopologyConfigurationSession topologyConfigurationSession = DirectorySessionFactory.Default.CreateTopologyConfigurationSession(ConsistencyMode.IgnoreInvalid, ADSessionSettings.FromRootOrgScopeSet(), 859, "GetEndpoint", "f:\\15.00.1497\\sources\\dev\\data\\src\\ApplicationLogic\\PeopleConnect\\CachedPeopleConnectApplicationConfig.cs");
				Uri uri = topologyConfigurationSession.GetEndpointContainer().GetEndpoint(endpointCommonName).Uri;
				if (!(uri != null))
				{
					return string.Empty;
				}
				return uri.ToString();
			}
		}
	}
}
