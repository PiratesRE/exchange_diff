using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.IdentityModel.Metadata;
using System.IdentityModel.Tokens;
using System.IO;
using System.Linq;
using System.Net;
using System.Security;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.ServiceModel.Security;
using System.Xml;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics.Components.Security;

namespace Microsoft.Exchange.Security.OAuth
{
	internal sealed class TrustedIssuer
	{
		private TrustedIssuer(IssuerMetadata metadata, Dictionary<string, X509Certificate2> x5tCertMap, TrustedIssuer.OnlineCertificateProvider onlineCertificateProvider)
		{
			OAuthCommon.VerifyNonNullArgument("metadata", metadata);
			OAuthCommon.VerifyNonNullArgument("x5tCertMap", x5tCertMap);
			this.metadata = metadata;
			this.x5tCertMap = x5tCertMap;
			this.onlineCertificateProvider = onlineCertificateProvider;
		}

		public static TrustedIssuer CreateFromExchangeCallback(LocalConfiguration localConfiguration, string realm)
		{
			return new TrustedIssuer(new IssuerMetadata(IssuerKind.PartnerApp, localConfiguration.ApplicationId, realm), TrustedIssuer.GetOfflineCertMap("self", localConfiguration.Certificates), null);
		}

		public static bool TryCreateFromAuthServer(AuthServer authServer, out TrustedIssuer trustedIssuer)
		{
			trustedIssuer = null;
			IssuerKind kind;
			switch (authServer.Type)
			{
			case AuthServerType.MicrosoftACS:
				kind = IssuerKind.ACS;
				goto IL_3C;
			case AuthServerType.ADFS:
				kind = IssuerKind.ADFS;
				goto IL_3C;
			case AuthServerType.AzureAD:
				kind = IssuerKind.AzureAD;
				goto IL_3C;
			}
			throw new InvalidOperationException();
			IL_3C:
			IssuerMetadata issuerMetadata = new IssuerMetadata(kind, authServer.IssuerIdentifier, authServer.Realm);
			TrustedIssuer.OnlineCertificateProvider onlineCertificateProvider = TrustedIssuer.GetOnlineCertificateProvider(authServer.AuthMetadataUrl);
			Dictionary<string, X509Certificate2> dictionary = TrustedIssuer.CombineCertificates(authServer.Name, authServer.CertificateBytes, onlineCertificateProvider);
			if (dictionary.Count == 0)
			{
				trustedIssuer = null;
				OAuthCommon.EventLogger.LogEvent(SecurityEventLogConstants.Tuple_OAuthAuthServerMissingCertificates, authServer.Name, new object[]
				{
					authServer.Name,
					authServer.ApplicationIdentifier
				});
				return false;
			}
			trustedIssuer = new TrustedIssuer(issuerMetadata, dictionary, onlineCertificateProvider);
			return true;
		}

		public static bool TryCreateFromPartnerApplication(PartnerApplication partnerApp, out TrustedIssuer trustedIssuer)
		{
			trustedIssuer = null;
			IssuerMetadata issuerMetadata = new IssuerMetadata(IssuerKind.PartnerApp, string.IsNullOrEmpty(partnerApp.IssuerIdentifier) ? partnerApp.ApplicationIdentifier : partnerApp.IssuerIdentifier, partnerApp.Realm);
			TrustedIssuer.OnlineCertificateProvider onlineCertificateProvider = TrustedIssuer.GetOnlineCertificateProvider(partnerApp.AuthMetadataUrl);
			Dictionary<string, X509Certificate2> dictionary = TrustedIssuer.CombineCertificates(partnerApp.Name, partnerApp.CertificateBytes, onlineCertificateProvider);
			if (dictionary.Count == 0)
			{
				OAuthCommon.EventLogger.LogEvent(SecurityEventLogConstants.Tuple_OAuthPartnerApplicationMissingCertificates, partnerApp.Name, new object[]
				{
					partnerApp.Name,
					partnerApp.ApplicationIdentifier
				});
				return false;
			}
			trustedIssuer = new TrustedIssuer(issuerMetadata, dictionary, onlineCertificateProvider);
			trustedIssuer.application = partnerApp;
			return true;
		}

		public static TrustedIssuer.OnlineCertificateProvider GetOnlineCertificateProvider(string metadataUrlString)
		{
			if (Uri.IsWellFormedUriString(metadataUrlString, UriKind.Absolute))
			{
				switch (AuthMetadataParser.DecideMetadataDocumentType(metadataUrlString))
				{
				case AuthMetadataParser.MetadataDocType.OAuthS2SV1Metadata:
					return TrustedIssuer.onlineCertProviders.GetOrAdd(metadataUrlString, (string url) => new TrustedIssuer.JsonOnlineCertificateProvider(url));
				case AuthMetadataParser.MetadataDocType.WSFedMetadata:
					return TrustedIssuer.onlineCertProviders.GetOrAdd(metadataUrlString, (string url) => new TrustedIssuer.XmlOnlineCertificateProvider(url));
				case AuthMetadataParser.MetadataDocType.OAuthOpenIdConnectMetadata:
					return TrustedIssuer.onlineCertProviders.GetOrAdd(metadataUrlString, (string url) => new TrustedIssuer.OpenIdConnectCertificateProvider(url));
				}
				return TrustedIssuer.onlineCertProviders.GetOrAdd(metadataUrlString, (string url) => new TrustedIssuer.JsonOnlineCertificateProvider(url));
			}
			return TrustedIssuer.OnlineCertificateProvider.NullProvider;
		}

		private static Dictionary<string, X509Certificate2> CombineCertificates(string name, MultiValuedProperty<byte[]> offlineCertBytes, TrustedIssuer.OnlineCertificateProvider onlineCertificateProvider)
		{
			Dictionary<string, X509Certificate2> offlineCertMap = TrustedIssuer.GetOfflineCertMap(name, offlineCertBytes);
			X509Certificate2[] certificates = onlineCertificateProvider.GetCertificates();
			if (certificates != null)
			{
				bool flag = false;
				foreach (X509Certificate2 x509Certificate in certificates)
				{
					string text = OAuthCommon.Base64UrlEncoder.EncodeBytes(x509Certificate.GetCertHash());
					if (!offlineCertMap.ContainsKey(text))
					{
						ExTraceGlobals.OAuthTracer.TraceDebug<string, string, string>(0L, "[TrustedIssuer:CombineCertificates] found new online certificates with x5t '{0}' for {1} from {2}", text, name, onlineCertificateProvider.MetadataUrl);
						offlineCertMap.Add(text, x509Certificate);
						flag = true;
					}
				}
				if (flag)
				{
					OAuthCommon.EventLogger.LogEvent(SecurityEventLogConstants.Tuple_OAuthNewCertificatesFromMetadataUrl, onlineCertificateProvider.MetadataUrl, new object[]
					{
						onlineCertificateProvider.MetadataUrl
					});
				}
			}
			return offlineCertMap;
		}

		private static Dictionary<string, X509Certificate2> GetOfflineCertMap(string name, MultiValuedProperty<byte[]> offlineCertBytes)
		{
			return TrustedIssuer.GetOfflineCertMap(name, (from certByte in offlineCertBytes
			select new X509Certificate2(certByte)).ToArray<X509Certificate2>());
		}

		private static Dictionary<string, X509Certificate2> GetOfflineCertMap(string name, X509Certificate2[] offlineCerts)
		{
			Dictionary<string, X509Certificate2> dictionary = new Dictionary<string, X509Certificate2>(4);
			foreach (X509Certificate2 x509Certificate in offlineCerts)
			{
				string text = OAuthCommon.Base64UrlEncoder.EncodeBytes(x509Certificate.GetCertHash());
				if (dictionary.ContainsKey(text))
				{
					ExTraceGlobals.OAuthTracer.TraceWarning<string, string>(0L, "[TrustedIssuer:CombineCertificates] duplicate certificates with x5t '{0}' were found for issuer {1}", text, name);
				}
				else
				{
					dictionary.Add(text, x509Certificate);
				}
			}
			return dictionary;
		}

		public IssuerMetadata IssuerMetadata
		{
			get
			{
				return this.metadata;
			}
		}

		public PartnerApplication PartnerApplication
		{
			get
			{
				if (this.application == null)
				{
					throw new InvalidOperationException();
				}
				return this.application;
			}
		}

		public SecurityKeyIdentifier GetSecurityKeyIdentifier(string x5tHint)
		{
			X509Certificate2 x509Certificate = null;
			if (!string.IsNullOrEmpty(x5tHint) && this.x5tCertMap.TryGetValue(x5tHint, out x509Certificate))
			{
				ExTraceGlobals.OAuthTracer.TraceDebug<string, string>((long)this.GetHashCode(), "[TrustedIssuer:GetSecurityKeyIdentifier] found cert based on x5t value '{0}': {1}", x5tHint, x509Certificate.Subject);
				return new SecurityKeyIdentifier(new SecurityKeyIdentifierClause[]
				{
					new X509RawDataKeyIdentifierClause(x509Certificate)
				});
			}
			ExTraceGlobals.OAuthTracer.TraceDebug<string>((long)this.GetHashCode(), "[TrustedIssuer:GetSecurityKeyIdentifier] did not find cert based on x5t value '{0}', returning all certs", x5tHint);
			return new SecurityKeyIdentifier((from cert in this.x5tCertMap.Values
			select new X509RawDataKeyIdentifierClause(cert)).ToArray<X509RawDataKeyIdentifierClause>());
		}

		public void SetSigningTokens(string x5tHint, TokenValidationParameters tokenValidationParameters)
		{
			X509Certificate2 x509Certificate = null;
			if (!string.IsNullOrEmpty(x5tHint) && this.x5tCertMap.TryGetValue(x5tHint, out x509Certificate))
			{
				ExTraceGlobals.OAuthTracer.TraceDebug<string, string>((long)this.GetHashCode(), "[TrustedIssuer:GetSecurityKeyIdentifier] found cert based on x5t value '{0}': {1}", x5tHint, x509Certificate.Subject);
				tokenValidationParameters.SigningToken = new X509SecurityToken(x509Certificate);
				return;
			}
			ExTraceGlobals.OAuthTracer.TraceDebug<string>((long)this.GetHashCode(), "[TrustedIssuer:GetSecurityKeyIdentifier] did not find cert based on x5t value '{0}', returning all certs", x5tHint);
			List<SecurityToken> list = new List<SecurityToken>();
			foreach (X509Certificate2 certificate in this.x5tCertMap.Values)
			{
				list.Add(new X509SecurityToken(certificate));
			}
			tokenValidationParameters.SigningTokens = list;
		}

		public void PokeOnlineCertificateProvider()
		{
			this.onlineCertificateProvider.NotifyValidationFailure();
		}

		public override string ToString()
		{
			return string.Format(CultureInfo.InvariantCulture, "TrustedIssuer ({0})", new object[]
			{
				this.IssuerMetadata
			});
		}

		private static ConcurrentDictionary<string, TrustedIssuer.OnlineCertificateProvider> onlineCertProviders = new ConcurrentDictionary<string, TrustedIssuer.OnlineCertificateProvider>();

		private readonly Dictionary<string, X509Certificate2> x5tCertMap;

		private readonly IssuerMetadata metadata;

		private readonly TrustedIssuer.OnlineCertificateProvider onlineCertificateProvider;

		private PartnerApplication application;

		internal abstract class OnlineCertificateProvider
		{
			protected OnlineCertificateProvider()
			{
			}

			protected OnlineCertificateProvider(string metadataUrl) : this(metadataUrl, TrustedIssuer.OnlineCertificateProvider.DefaultRefreshInterval)
			{
			}

			protected OnlineCertificateProvider(string metadataUrl, TimeSpan refreshInterval)
			{
				this.metadataUrl = metadataUrl;
				this.refreshInterval = refreshInterval;
				this.signatureValidateFailed = false;
				this.fetchCount = 0;
				this.successfulFetchTimes = new DateTime[TrustedIssuer.OnlineCertificateProvider.MaxSuccessfulFetchCountPerInterval];
				this.oldestSuccessfulFetchTimeIndex = 0;
			}

			public static TrustedIssuer.OnlineCertificateProvider NullProvider
			{
				get
				{
					return TrustedIssuer.OnlineCertificateProvider.nullProvider;
				}
			}

			public string MetadataUrl
			{
				get
				{
					return this.metadataUrl;
				}
			}

			public virtual X509Certificate2[] GetCertificates()
			{
				if (this.NeedFetchNow())
				{
					ExTraceGlobals.OAuthTracer.TraceDebug((long)this.GetHashCode(), "[OCP:GetCertificates] start fetching");
					X509Certificate2[] array = this.FetchCertsHelper();
					bool flag = array != null;
					if (flag)
					{
						ExTraceGlobals.OAuthTracer.TraceDebug((long)this.GetHashCode(), "[OCP:GetCertificates] new certificates are fetched");
						this.certificates = array;
					}
					this.PostFetch(flag);
				}
				else
				{
					ExTraceGlobals.OAuthTracer.TraceDebug((long)this.GetHashCode(), "[OCP:GetCertificates] skip fetching and going to reuse the saved certs");
				}
				this.ClearSignatureValidationFlag();
				return this.certificates;
			}

			internal void ClearSignatureValidationFlag()
			{
				this.signatureValidateFailed = false;
			}

			public virtual void NotifyValidationFailure()
			{
				this.signatureValidateFailed = true;
			}

			internal bool NeedFetchNow()
			{
				DateTime utcNow = DateTime.UtcNow;
				if (this.nextFetchTime < utcNow)
				{
					ExTraceGlobals.OAuthTracer.TraceDebug<DateTime, DateTime>((long)this.GetHashCode(), "[OCP:NeedFetchNow] return true since scheduled fetch time was {0} and now is {1}", this.nextFetchTime, utcNow);
					return true;
				}
				DateTime arg = this.successfulFetchTimes[this.oldestSuccessfulFetchTimeIndex];
				ExTraceGlobals.OAuthTracer.TraceDebug<DateTime, int>((long)this.GetHashCode(), "[OCP:NeedFetchNow] the last 3 successful fetching happened at {0}, current retry count: {1}", arg, this.fetchCount);
				if (this.signatureValidateFailed && arg.Add(this.refreshInterval) < utcNow)
				{
					ExTraceGlobals.OAuthTracer.TraceDebug((long)this.GetHashCode(), "[OCP:NeedFetchNow] return true since we were told the signature validation failed, and we did not fetch too many times. last 24 hours");
					return true;
				}
				if (0 < this.fetchCount && this.fetchCount <= 3)
				{
					ExTraceGlobals.OAuthTracer.TraceDebug((long)this.GetHashCode(), "[OCP:NeedFetchNow] return true since we failed to fetch last time");
					return true;
				}
				return false;
			}

			internal void PostFetch(bool succeed)
			{
				this.fetchCount++;
				DateTime utcNow = DateTime.UtcNow;
				if (!succeed)
				{
					if (this.fetchCount > 3)
					{
						this.nextFetchTime = utcNow.Add(this.refreshInterval);
						return;
					}
				}
				else
				{
					this.successfulFetchTimes[this.oldestSuccessfulFetchTimeIndex] = utcNow;
					this.oldestSuccessfulFetchTimeIndex = (this.oldestSuccessfulFetchTimeIndex + 1) % TrustedIssuer.OnlineCertificateProvider.MaxSuccessfulFetchCountPerInterval;
					this.nextFetchTime = utcNow.Add(this.refreshInterval);
					this.fetchCount = 0;
				}
			}

			private X509Certificate2[] FetchCertsHelper()
			{
				AuthMetadataClient authMetadataClient = new AuthMetadataClient(this.metadataUrl, true);
				string content = null;
				try
				{
					content = authMetadataClient.Acquire(false);
				}
				catch (WebException ex)
				{
					ExTraceGlobals.OAuthTracer.TraceDebug<WebException>((long)this.GetHashCode(), "[OCP:FetchCerts] hitting exception during acquiring: {0}", ex);
					OAuthCommon.EventLogger.LogEvent(SecurityEventLogConstants.Tuple_OAuthFailedWhileReadingMetadata, this.metadataUrl, new object[]
					{
						this.metadataUrl,
						ex
					});
					return null;
				}
				IList<X509Certificate2> list = this.ExtractCertificates(content);
				if (list.Count == 0)
				{
					return null;
				}
				return list.ToArray<X509Certificate2>();
			}

			protected abstract IList<X509Certificate2> ExtractCertificates(string content);

			public override string ToString()
			{
				return string.Format("NextFetch @ {0}, Retry# {1}, SucceedFetch @{2}", this.nextFetchTime, this.fetchCount, this.successfulFetchTimes[this.oldestSuccessfulFetchTimeIndex]);
			}

			private static readonly TimeSpan DefaultRefreshInterval = TimeSpan.FromHours(24.0);

			private static readonly TrustedIssuer.OnlineCertificateProvider nullProvider = new TrustedIssuer.NullOnlineCertificateProvider();

			private static readonly int MaxSuccessfulFetchCountPerInterval = 3;

			protected readonly string metadataUrl;

			private readonly TimeSpan refreshInterval;

			private X509Certificate2[] certificates;

			private DateTime nextFetchTime;

			private DateTime[] successfulFetchTimes;

			private int oldestSuccessfulFetchTimeIndex;

			private int fetchCount;

			private bool signatureValidateFailed;
		}

		internal sealed class XmlOnlineCertificateProvider : TrustedIssuer.OnlineCertificateProvider
		{
			public XmlOnlineCertificateProvider(string metadataUrl) : base(metadataUrl)
			{
			}

			public XmlOnlineCertificateProvider(string metadataUrl, TimeSpan refreshInterval) : base(metadataUrl, refreshInterval)
			{
			}

			protected override IList<X509Certificate2> ExtractCertificates(string content)
			{
				List<X509Certificate2> list = new List<X509Certificate2>();
				Exception ex = null;
				try
				{
					using (TextReader textReader = new StringReader(content))
					{
						using (XmlReader xmlReader = XmlReader.Create(textReader))
						{
							MetadataSerializer metadataSerializer = new MetadataSerializer
							{
								CertificateValidationMode = X509CertificateValidationMode.None
							};
							EntityDescriptor entityDescriptor = metadataSerializer.ReadMetadata(xmlReader) as EntityDescriptor;
							SecurityTokenServiceDescriptor securityTokenServiceDescriptor = entityDescriptor.RoleDescriptors.OfType<SecurityTokenServiceDescriptor>().First<SecurityTokenServiceDescriptor>();
							foreach (KeyDescriptor keyDescriptor in from k in securityTokenServiceDescriptor.Keys
							where k.Use == KeyType.Signing
							select k)
							{
								foreach (SecurityKeyIdentifierClause securityKeyIdentifierClause in keyDescriptor.KeyInfo)
								{
									if (securityKeyIdentifierClause is X509RawDataKeyIdentifierClause)
									{
										list.Add(new X509Certificate2((securityKeyIdentifierClause as X509RawDataKeyIdentifierClause).GetX509RawData()));
									}
								}
							}
						}
					}
				}
				catch (XmlException ex2)
				{
					ex = ex2;
				}
				catch (IOException ex3)
				{
					ex = ex3;
				}
				catch (SecurityException ex4)
				{
					ex = ex4;
				}
				catch (SystemException ex5)
				{
					ex = ex5;
				}
				if (ex != null)
				{
					ExTraceGlobals.OAuthTracer.TraceDebug<Exception>((long)this.GetHashCode(), "[OCP:FetchCerts] hitting exception during converting: {0}", ex);
					OAuthCommon.EventLogger.LogEvent(SecurityEventLogConstants.Tuple_OAuthFailedWhileReadingMetadata, this.metadataUrl, new object[]
					{
						this.metadataUrl,
						ex
					});
				}
				return list;
			}
		}

		internal sealed class JsonOnlineCertificateProvider : TrustedIssuer.OnlineCertificateProvider
		{
			public JsonOnlineCertificateProvider(string metadataUrl) : base(metadataUrl)
			{
			}

			public JsonOnlineCertificateProvider(string metadataUrl, TimeSpan refreshInterval) : base(metadataUrl, refreshInterval)
			{
			}

			protected override IList<X509Certificate2> ExtractCertificates(string content)
			{
				List<X509Certificate2> list = new List<X509Certificate2>();
				JsonMetadataDocument jsonMetadataDocument = null;
				try
				{
					jsonMetadataDocument = AuthMetadataParser.GetDocument<JsonMetadataDocument>(content);
				}
				catch (AuthMetadataParserException ex)
				{
					ExTraceGlobals.OAuthTracer.TraceDebug<AuthMetadataParserException>((long)this.GetHashCode(), "[OCP:FetchCerts] hitting exception during parsing: {0}", ex);
					OAuthCommon.EventLogger.LogEvent(SecurityEventLogConstants.Tuple_OAuthFailedWhileReadingMetadata, this.metadataUrl, new object[]
					{
						this.metadataUrl,
						ex
					});
					return list;
				}
				if (jsonMetadataDocument != null && jsonMetadataDocument.keys != null)
				{
					foreach (JsonKey jsonKey in jsonMetadataDocument.keys)
					{
						JsonKeyValue keyvalue = jsonKey.keyvalue;
						if (keyvalue != null)
						{
							string value = keyvalue.value;
							if (!string.IsNullOrEmpty(value))
							{
								try
								{
									byte[] rawData = Convert.FromBase64String(value);
									list.Add(new X509Certificate2(rawData));
								}
								catch (FormatException ex2)
								{
									ExTraceGlobals.OAuthTracer.TraceDebug<FormatException>((long)this.GetHashCode(), "[OCP:FetchCerts] hitting exception during converting: {0}", ex2);
									OAuthCommon.EventLogger.LogEvent(SecurityEventLogConstants.Tuple_OAuthFailedWhileReadingMetadata, this.metadataUrl, new object[]
									{
										this.metadataUrl,
										ex2
									});
								}
								catch (CryptographicException ex3)
								{
									ExTraceGlobals.OAuthTracer.TraceDebug<CryptographicException>((long)this.GetHashCode(), "[OCP:FetchCerts] hitting exception during converting: {0}", ex3);
									OAuthCommon.EventLogger.LogEvent(SecurityEventLogConstants.Tuple_OAuthFailedWhileReadingMetadata, this.metadataUrl, new object[]
									{
										this.metadataUrl,
										ex3
									});
								}
							}
						}
					}
				}
				return list;
			}
		}

		internal sealed class OpenIdConnectCertificateProvider : TrustedIssuer.OnlineCertificateProvider
		{
			public OpenIdConnectCertificateProvider(string metadataUrl) : base(metadataUrl)
			{
			}

			public OpenIdConnectCertificateProvider(string metadataUrl, TimeSpan refreshInterval) : base(metadataUrl, refreshInterval)
			{
			}

			protected override IList<X509Certificate2> ExtractCertificates(string content)
			{
				List<X509Certificate2> list = new List<X509Certificate2>();
				OpenIdConnectJsonMetadataDocument openIdConnectJsonMetadataDocument = null;
				try
				{
					openIdConnectJsonMetadataDocument = AuthMetadataParser.GetDocument<OpenIdConnectJsonMetadataDocument>(content);
				}
				catch (AuthMetadataParserException ex)
				{
					ExTraceGlobals.OAuthTracer.TraceDebug<AuthMetadataParserException>((long)this.GetHashCode(), "[OCP:FetchCerts] hitting exception during parsing: {0}", ex);
					OAuthCommon.EventLogger.LogEvent(SecurityEventLogConstants.Tuple_OAuthFailedWhileReadingMetadata, this.metadataUrl, new object[]
					{
						this.metadataUrl,
						ex
					});
					return list;
				}
				if (string.IsNullOrEmpty(openIdConnectJsonMetadataDocument.jwks_uri))
				{
					ExTraceGlobals.OAuthTracer.TraceDebug<string>((long)this.GetHashCode(), "[OCP:FetchCerts] Cannot find Open Id Connect Key Url Path in metadata document at: {0}", this.metadataUrl);
					OAuthCommon.EventLogger.LogEvent(SecurityEventLogConstants.Tuple_OAuthFailedWhileReadingMetadata, this.metadataUrl, new object[]
					{
						this.metadataUrl,
						new AuthMetadataParserException(DirectoryStrings.ErrorAuthMetadataNoSigningKey)
					});
					return list;
				}
				AuthMetadataClient authMetadataClient = new AuthMetadataClient(openIdConnectJsonMetadataDocument.jwks_uri, true);
				string content2 = null;
				try
				{
					content2 = authMetadataClient.Acquire(false);
				}
				catch (WebException ex2)
				{
					ExTraceGlobals.OAuthTracer.TraceDebug<WebException>((long)this.GetHashCode(), "[OCP:FetchCerts] hitting exception during acquiring: {0}", ex2);
					OAuthCommon.EventLogger.LogEvent(SecurityEventLogConstants.Tuple_OAuthFailedWhileReadingMetadata, this.metadataUrl, new object[]
					{
						this.metadataUrl,
						ex2
					});
					return list;
				}
				OpenIdConnectKeysJsonMetadataDocument openIdConnectKeysJsonMetadataDocument = null;
				try
				{
					openIdConnectKeysJsonMetadataDocument = AuthMetadataParser.GetDocument<OpenIdConnectKeysJsonMetadataDocument>(content2);
				}
				catch (AuthMetadataParserException ex3)
				{
					ExTraceGlobals.OAuthTracer.TraceDebug<AuthMetadataParserException>((long)this.GetHashCode(), "[OCP:FetchCerts] hitting exception during parsing: {0}", ex3);
					OAuthCommon.EventLogger.LogEvent(SecurityEventLogConstants.Tuple_OAuthFailedWhileReadingMetadata, this.metadataUrl, new object[]
					{
						this.metadataUrl,
						ex3
					});
					return list;
				}
				if (openIdConnectKeysJsonMetadataDocument != null && openIdConnectKeysJsonMetadataDocument.keys != null)
				{
					foreach (OpenIdConnectKey openIdConnectKey in openIdConnectKeysJsonMetadataDocument.keys)
					{
						if (!string.IsNullOrEmpty(openIdConnectKey.use) && openIdConnectKey.use.Equals(AuthMetadataConstants.OpenIdConnectSigningKeyUsage, StringComparison.OrdinalIgnoreCase) && openIdConnectKey.x5c != null && openIdConnectKey.x5c.Length != 0)
						{
							string text = openIdConnectKey.x5c[0];
							if (!string.IsNullOrEmpty(text))
							{
								try
								{
									byte[] rawData = Convert.FromBase64String(text);
									list.Add(new X509Certificate2(rawData));
								}
								catch (FormatException ex4)
								{
									ExTraceGlobals.OAuthTracer.TraceDebug<FormatException>((long)this.GetHashCode(), "[OCP:FetchCerts] hitting exception during converting: {0}", ex4);
									OAuthCommon.EventLogger.LogEvent(SecurityEventLogConstants.Tuple_OAuthFailedWhileReadingMetadata, this.metadataUrl, new object[]
									{
										this.metadataUrl,
										ex4
									});
								}
								catch (CryptographicException ex5)
								{
									ExTraceGlobals.OAuthTracer.TraceDebug<CryptographicException>((long)this.GetHashCode(), "[OCP:FetchCerts] hitting exception during converting: {0}", ex5);
									OAuthCommon.EventLogger.LogEvent(SecurityEventLogConstants.Tuple_OAuthFailedWhileReadingMetadata, this.metadataUrl, new object[]
									{
										this.metadataUrl,
										ex5
									});
								}
							}
						}
					}
				}
				return list;
			}
		}

		internal sealed class NullOnlineCertificateProvider : TrustedIssuer.OnlineCertificateProvider
		{
			public override X509Certificate2[] GetCertificates()
			{
				return null;
			}

			public override void NotifyValidationFailure()
			{
			}

			protected override IList<X509Certificate2> ExtractCertificates(string content)
			{
				throw new NotImplementedException();
			}
		}
	}
}
