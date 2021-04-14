using System;
using System.Collections.Generic;
using System.Net;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Security.ExternalAuthentication;
using Microsoft.Exchange.Net.WSTrust;

namespace Microsoft.Exchange.Data.Storage.Authentication
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class ExternalAuthentication
	{
		private ExternalAuthentication(ExternalAuthentication.ExternalAuthenticationFailureType failureType, ExternalAuthentication.ExternalAuthenticationSubFailureType subFailureType)
		{
			this.failureType = failureType;
			this.subFailureType = subFailureType;
		}

		private ExternalAuthentication(Dictionary<ADObjectId, SecurityTokenService> securityTokenServices, TokenValidator tokenValidator, List<X509Certificate2> certificates, Uri applicationUri) : this(ExternalAuthentication.ExternalAuthenticationFailureType.NoFailure, ExternalAuthentication.ExternalAuthenticationSubFailureType.NoFailure)
		{
			this.securityTokenServices = securityTokenServices;
			this.tokenValidator = tokenValidator;
			this.certificates = certificates;
			this.applicationUri = applicationUri;
		}

		private ExternalAuthentication(Dictionary<ADObjectId, SecurityTokenService> securityTokenServices, TokenValidator tokenValidator, List<X509Certificate2> certificates, Uri applicationUri, ExternalAuthentication.ExternalAuthenticationFailureType failureType, ExternalAuthentication.ExternalAuthenticationSubFailureType subFailureType) : this(failureType, subFailureType)
		{
			this.securityTokenServices = securityTokenServices;
			this.tokenValidator = tokenValidator;
			this.certificates = certificates;
			this.applicationUri = applicationUri;
		}

		public Uri ApplicationUri
		{
			get
			{
				this.ThrowIfNotEnabled();
				return this.applicationUri;
			}
		}

		public bool Enabled
		{
			get
			{
				return this.failureType == ExternalAuthentication.ExternalAuthenticationFailureType.NoFailure;
			}
		}

		public ExternalAuthentication.ExternalAuthenticationFailureType FailureType
		{
			get
			{
				return this.failureType;
			}
		}

		public ExternalAuthentication.ExternalAuthenticationSubFailureType SubFailureType
		{
			get
			{
				return this.subFailureType;
			}
		}

		public TokenValidator TokenValidator
		{
			get
			{
				this.ThrowIfNotEnabled();
				return this.tokenValidator;
			}
		}

		public IEnumerable<X509Certificate2> Certificates
		{
			get
			{
				this.ThrowIfNotEnabled();
				return this.certificates;
			}
		}

		public IEnumerable<SecurityTokenService> SecurityTokenServices
		{
			get
			{
				this.ThrowIfNotEnabled();
				return this.securityTokenServices.Values;
			}
		}

		public string SecurityTokenServicesIdentifiers
		{
			get
			{
				if (this.securityTokenServices == null)
				{
					return string.Empty;
				}
				StringBuilder stringBuilder = new StringBuilder();
				foreach (SecurityTokenService securityTokenService in this.securityTokenServices.Values)
				{
					stringBuilder.Append(securityTokenService.STSClientIdentity + ";");
				}
				return stringBuilder.ToString();
			}
		}

		private static ExEventLog EventLogger
		{
			get
			{
				if (ExternalAuthentication.eventLogger == null)
				{
					ExternalAuthentication.eventLogger = new ExEventLog(new Guid("{776208EC-5A13-4b8a-AA53-C84B72E40B86}"), "MSExchange Common");
				}
				return ExternalAuthentication.eventLogger;
			}
		}

		internal static void Initialize()
		{
			LocalServerCache.Initialize();
			FederationTrustCache.Initialize();
			ExternalAuthentication.instance = null;
		}

		public static void ForceReset()
		{
			ExternalAuthentication.ConfigurationTracer.TraceDebug(0L, "A Force Reset was requested.");
			ExternalAuthentication.instance = null;
		}

		public static ExternalAuthentication GetCurrent()
		{
			if (ExternalAuthentication.instance == null)
			{
				ExternalAuthentication.instance = ExternalAuthentication.CreateExternalAuthentication();
			}
			return ExternalAuthentication.instance;
		}

		public SecurityTokenService GetSecurityTokenService(OrganizationId organizationId)
		{
			this.ThrowIfNotEnabled();
			OrganizationIdCacheValue organizationIdCacheValue = OrganizationIdCache.Singleton.Get(organizationId);
			FederatedOrganizationId federatedOrganizationId = organizationIdCacheValue.FederatedOrganizationId;
			if (federatedOrganizationId == null)
			{
				ExternalAuthentication.ConfigurationTracer.TraceError<string>(0L, "Unable to find Federated Organization Identifier for organization {0}.", organizationId.ToString());
				return null;
			}
			if (federatedOrganizationId.DelegationTrustLink == null)
			{
				ExternalAuthentication.ConfigurationTracer.TraceError<string>(0L, "Unable to find configured delegation trust link for organization {0}.", organizationId.ToString());
				return null;
			}
			SecurityTokenService result;
			if (this.securityTokenServices.TryGetValue(federatedOrganizationId.DelegationTrustLink, out result))
			{
				return result;
			}
			ExternalAuthentication.ConfigurationTracer.TraceError<string, string>(0L, "Unable to find configured Security Token Service client for delegation trust link {0} for organization {1}.", federatedOrganizationId.DelegationTrustLink.DistinguishedName, organizationId.ToString());
			ExternalAuthentication.ConfigurationTracer.TraceError<string>(0L, "Current Security Token Service client list is {0}.", this.SecurityTokenServicesIdentifiers);
			return null;
		}

		private static ExternalAuthentication CreateExternalAuthentication()
		{
			ExternalAuthentication.InitializeNotificationsIfNeeded();
			ExternalAuthentication.ConfigurationTracer.TraceDebug(0L, "Searching for federation trust configuration in AD");
			WebProxy webProxy = null;
			Server localServer = LocalServerCache.LocalServer;
			Uri uri = null;
			if (localServer != null && localServer.InternetWebProxy != null)
			{
				ExternalAuthentication.ConfigurationTracer.TraceDebug<Uri>(0L, "Using custom InternetWebProxy {0}.", localServer.InternetWebProxy);
				uri = localServer.InternetWebProxy;
				webProxy = new WebProxy(localServer.InternetWebProxy);
			}
			ExternalAuthentication.currentWebProxy = uri;
			IEnumerable<FederationTrust> enumerable = null;
			try
			{
				enumerable = FederationTrustCache.GetFederationTrusts();
			}
			catch (LocalizedException arg)
			{
				ExternalAuthentication.ConfigurationTracer.TraceError<LocalizedException>(0L, "Unable to get federation trust. Exception: {0}", arg);
				return new ExternalAuthentication(ExternalAuthentication.ExternalAuthenticationFailureType.ErrorReadingFederationTrust, ExternalAuthentication.ExternalAuthenticationSubFailureType.DirectoryReadError);
			}
			Uri uri2 = null;
			List<X509Certificate2> list = new List<X509Certificate2>(4);
			List<X509Certificate2> list2 = new List<X509Certificate2>(4);
			Dictionary<ADObjectId, SecurityTokenService> dictionary = new Dictionary<ADObjectId, SecurityTokenService>(2);
			ExternalAuthentication.ExternalAuthenticationFailureType externalAuthenticationFailureType = ExternalAuthentication.ExternalAuthenticationFailureType.NoFederationTrust;
			ExternalAuthentication.ExternalAuthenticationSubFailureType externalAuthenticationSubFailureType = ExternalAuthentication.ExternalAuthenticationSubFailureType.NoFailure;
			int num = 0;
			bool flag = false;
			if (enumerable != null)
			{
				foreach (FederationTrust federationTrust in enumerable)
				{
					num++;
					ExternalAuthentication.FederationTrustResults federationTrustResults = ExternalAuthentication.TryCreateSecurityTokenService(federationTrust, webProxy);
					if (federationTrustResults.FailureType == ExternalAuthentication.ExternalAuthenticationFailureType.NoFailure)
					{
						dictionary.Add(federationTrust.Id, federationTrustResults.SecurityTokenService);
						list.AddRange(federationTrustResults.TokenSignatureCertificates);
						list2.AddRange(federationTrustResults.TokenDecryptionCertificates);
						if (uri2 == null)
						{
							uri2 = federationTrust.ApplicationUri;
							ExternalAuthentication.ConfigurationTracer.TraceDebug<Uri>(0L, "Using {0} as applicationUri", uri2);
						}
						else if (!federationTrust.ApplicationUri.Equals(uri2))
						{
							ExternalAuthentication.ConfigurationTracer.TraceError<Uri>(0L, "Cannot have multiple FederationTrust with different ApplicationUri values: {0}. Uri will be ignored", federationTrust.ApplicationUri);
							flag = true;
						}
					}
					else
					{
						externalAuthenticationFailureType = federationTrustResults.FailureType;
						externalAuthenticationSubFailureType = federationTrustResults.SubFailureType;
					}
				}
			}
			if (dictionary.Count == 0)
			{
				return new ExternalAuthentication(externalAuthenticationFailureType, externalAuthenticationSubFailureType);
			}
			if (dictionary.Count != num)
			{
				ExternalAuthentication.ConfigurationTracer.TraceError<string, string>(0L, "Number of Security Token Service clients {0} does not match number of federation trusts {1}", dictionary.Count.ToString(), num.ToString());
			}
			TokenValidator tokenValidator = new TokenValidator(uri2, list, list2);
			List<X509Certificate2> list3 = new List<X509Certificate2>(list.Count + list2.Count);
			list3.AddRange(list);
			list3.AddRange(list2);
			if (flag)
			{
				return new ExternalAuthentication(dictionary, tokenValidator, list3, uri2, ExternalAuthentication.ExternalAuthenticationFailureType.NoFailure, ExternalAuthentication.ExternalAuthenticationSubFailureType.WarningApplicationUriSkipped);
			}
			return new ExternalAuthentication(dictionary, tokenValidator, list3, uri2);
		}

		private static ExternalAuthentication.FederationTrustResults TryCreateSecurityTokenService(FederationTrust federationTrust, WebProxy webProxy)
		{
			if (!ExternalAuthentication.IsRequiredPropertyAvailable(federationTrust.TokenIssuerUri, "TokenIssuerUri"))
			{
				return new ExternalAuthentication.FederationTrustResults
				{
					FailureType = ExternalAuthentication.ExternalAuthenticationFailureType.MisconfiguredFederationTrust,
					SubFailureType = ExternalAuthentication.ExternalAuthenticationSubFailureType.MissingTokenIssuerUri
				};
			}
			if (!ExternalAuthentication.IsRequiredPropertyAvailable(federationTrust.TokenIssuerEpr, "TokenIssuerEpr"))
			{
				return new ExternalAuthentication.FederationTrustResults
				{
					FailureType = ExternalAuthentication.ExternalAuthenticationFailureType.MisconfiguredFederationTrust,
					SubFailureType = ExternalAuthentication.ExternalAuthenticationSubFailureType.MissingTokenIssuerEpr
				};
			}
			if (!ExternalAuthentication.IsRequiredPropertyAvailable(federationTrust.ApplicationUri, "ApplicationUri"))
			{
				return new ExternalAuthentication.FederationTrustResults
				{
					FailureType = ExternalAuthentication.ExternalAuthenticationFailureType.MisconfiguredFederationTrust,
					SubFailureType = ExternalAuthentication.ExternalAuthenticationSubFailureType.MissingApplicationUri
				};
			}
			if (!ExternalAuthentication.IsRequiredPropertyAvailable(federationTrust.TokenIssuerCertificate, "TokenIssuerCertificate"))
			{
				return new ExternalAuthentication.FederationTrustResults
				{
					FailureType = ExternalAuthentication.ExternalAuthenticationFailureType.MisconfiguredFederationTrust,
					SubFailureType = ExternalAuthentication.ExternalAuthenticationSubFailureType.MissingTokenIssuerCertificate
				};
			}
			X509Certificate2[] tokenSignatureCertificates = (federationTrust.TokenIssuerPrevCertificate != null) ? new X509Certificate2[]
			{
				federationTrust.TokenIssuerCertificate,
				federationTrust.TokenIssuerPrevCertificate
			} : new X509Certificate2[]
			{
				federationTrust.TokenIssuerCertificate
			};
			if (!ExternalAuthentication.HasAtLeastOneValidCertificate(tokenSignatureCertificates, federationTrust.Id, "TokenIssuerCertificate and TokenIssuerPrevCertificate"))
			{
				return new ExternalAuthentication.FederationTrustResults
				{
					FailureType = ExternalAuthentication.ExternalAuthenticationFailureType.InvalidTokenIssuerCertificate,
					SubFailureType = ExternalAuthentication.ExternalAuthenticationSubFailureType.NoSubCode
				};
			}
			if (!ExternalAuthentication.IsRequiredPropertyAvailable(federationTrust.OrgPrivCertificate, "OrgPrivCertificate"))
			{
				return new ExternalAuthentication.FederationTrustResults
				{
					FailureType = ExternalAuthentication.ExternalAuthenticationFailureType.MisconfiguredFederationTrust,
					SubFailureType = ExternalAuthentication.ExternalAuthenticationSubFailureType.MissingOrgPrivCertificate
				};
			}
			X509Store x509Store = new X509Store(StoreName.My, StoreLocation.LocalMachine);
			x509Store.Open(OpenFlags.ReadOnly);
			X509Certificate2 certificate;
			X509Certificate2[] tokenDecryptionCertificates;
			try
			{
				ExternalAuthentication.ExternalAuthenticationSubFailureType externalAuthenticationSubFailureType;
				certificate = ExternalAuthentication.GetCertificate(x509Store, federationTrust.OrgPrivCertificate, federationTrust.Id, "OrgPrivCertificate", true, out externalAuthenticationSubFailureType);
				if (certificate == null)
				{
					ExternalAuthentication.ConfigurationTracer.TraceError<string>(0L, "Federation trust is misconfigured. Unable to find certificate corresponding to OrgPrivCertificate={0}", federationTrust.OrgPrivCertificate);
					return new ExternalAuthentication.FederationTrustResults
					{
						FailureType = ExternalAuthentication.ExternalAuthenticationFailureType.MisconfiguredFederationTrust,
						SubFailureType = externalAuthenticationSubFailureType
					};
				}
				X509Certificate2 x509Certificate = null;
				if (!string.IsNullOrEmpty(federationTrust.OrgPrevPrivCertificate))
				{
					x509Certificate = ExternalAuthentication.GetCertificate(x509Store, federationTrust.OrgPrevPrivCertificate, federationTrust.Id, "OrgPrevPrivCertificate", false, out externalAuthenticationSubFailureType);
				}
				tokenDecryptionCertificates = ((x509Certificate != null) ? new X509Certificate2[]
				{
					certificate,
					x509Certificate
				} : new X509Certificate2[]
				{
					certificate
				});
			}
			finally
			{
				x509Store.Close();
			}
			SecurityTokenService securityTokenService = new SecurityTokenService(federationTrust.TokenIssuerEpr, webProxy, certificate, federationTrust.TokenIssuerUri, federationTrust.PolicyReferenceUri, federationTrust.ApplicationUri.OriginalString);
			ExternalAuthentication.ConfigurationTracer.TraceDebug(0L, "New instance of SecurityTokenService successfully built.");
			return new ExternalAuthentication.FederationTrustResults
			{
				FailureType = ExternalAuthentication.ExternalAuthenticationFailureType.NoFailure,
				SubFailureType = ExternalAuthentication.ExternalAuthenticationSubFailureType.NoFailure,
				SecurityTokenService = securityTokenService,
				TokenSignatureCertificates = tokenSignatureCertificates,
				TokenDecryptionCertificates = tokenDecryptionCertificates
			};
		}

		private static bool IsRequiredPropertyAvailable(X509Certificate2 certificate, string name)
		{
			return ExternalAuthentication.IsRequiredPropertyAvailable((certificate == null) ? null : certificate.Thumbprint, name);
		}

		private static bool IsRequiredPropertyAvailable(object property, string name)
		{
			if (property == null)
			{
				ExternalAuthentication.ConfigurationTracer.TraceError<string>(0L, "Property {0} is missing from federation trust configuration object.", name);
				return false;
			}
			ExternalAuthentication.ConfigurationTracer.TraceDebug<string, object>(0L, "Property {0}={1}", name, property);
			return true;
		}

		private static X509Certificate2 GetCertificate(X509Store store, string thumbprint, ObjectId objectId, string name, bool logExpired, out ExternalAuthentication.ExternalAuthenticationSubFailureType subFailureCode)
		{
			subFailureCode = ExternalAuthentication.ExternalAuthenticationSubFailureType.NoFailure;
			X509Certificate2Collection x509Certificate2Collection = store.Certificates.Find(X509FindType.FindByThumbprint, thumbprint, false);
			if (x509Certificate2Collection == null || x509Certificate2Collection.Count == 0 || x509Certificate2Collection[0] == null)
			{
				ExternalAuthentication.ConfigurationTracer.TraceError<string, string>(0L, "Certificate with thumprint {0} for {1} is not present in the certificate store.", thumbprint, name);
				ExternalAuthentication.EventLogger.LogEvent(CommonEventLogConstants.Tuple_FederationTrustOrganizationCertificateNotFound, thumbprint, new object[]
				{
					thumbprint,
					objectId.ToString()
				});
				subFailureCode = ExternalAuthentication.ExternalAuthenticationSubFailureType.CertificateNotInStore;
				return null;
			}
			X509Certificate2 x509Certificate = x509Certificate2Collection[0];
			if (!x509Certificate.HasPrivateKey)
			{
				ExternalAuthentication.ConfigurationTracer.TraceError<string, string>(0L, "Certificate with thumprint {0} for {1} does not have private key.", thumbprint, name);
				ExternalAuthentication.EventLogger.LogEvent(CommonEventLogConstants.Tuple_FederationTrustOrganizationCertificateNoPrivateKey, thumbprint, new object[]
				{
					thumbprint,
					objectId.ToString()
				});
				subFailureCode = ExternalAuthentication.ExternalAuthenticationSubFailureType.CertificateNoPrivateKey;
				return null;
			}
			DateTime utcNow = DateTime.UtcNow;
			if (utcNow > x509Certificate.NotAfter || utcNow < x509Certificate.NotBefore)
			{
				ExternalAuthentication.ConfigurationTracer.TraceError(0L, "Certificate with thumprint {0} for {1} is expired: NotBefore={2}, NotAfter={3}", new object[]
				{
					thumbprint,
					name,
					x509Certificate.NotBefore,
					x509Certificate.NotAfter
				});
				if (logExpired)
				{
					ExternalAuthentication.EventLogger.LogEvent(CommonEventLogConstants.Tuple_FederationTrustCertificateExpired, x509Certificate.Thumbprint, new object[]
					{
						x509Certificate.Thumbprint,
						objectId.ToString()
					});
				}
				subFailureCode = ExternalAuthentication.ExternalAuthenticationSubFailureType.CertificateExpirationTimeError;
				return null;
			}
			Exception ex = null;
			try
			{
				AsymmetricAlgorithm privateKey = x509Certificate.PrivateKey;
			}
			catch (CryptographicException ex2)
			{
				ex = ex2;
			}
			catch (NotSupportedException ex3)
			{
				ex = ex3;
			}
			if (ex != null)
			{
				ExternalAuthentication.ConfigurationTracer.TraceError<string, string>(0L, "Cannot access private key of certificate with thumprint {0} for {1}.", thumbprint, name);
				ExternalAuthentication.EventLogger.LogEvent(CommonEventLogConstants.Tuple_FederationTrustOrganizationCertificateNoPrivateKey, thumbprint, new object[]
				{
					thumbprint,
					objectId.ToString()
				});
				subFailureCode = ExternalAuthentication.ExternalAuthenticationSubFailureType.CertificatePrivateKeyCryptoError;
				return null;
			}
			ExternalAuthentication.ConfigurationTracer.TraceDebug<string, string, X509Certificate2>(0L, "Loaded certificate with thumprint {0} for {1}. Certificate details: {2}", thumbprint, name, x509Certificate);
			return x509Certificate;
		}

		private static bool HasAtLeastOneValidCertificate(X509Certificate2[] certificates, ObjectId objectId, string description)
		{
			bool flag = false;
			DateTime utcNow = DateTime.UtcNow;
			foreach (X509Certificate2 x509Certificate in certificates)
			{
				if (utcNow < x509Certificate.NotAfter && utcNow > x509Certificate.NotBefore)
				{
					flag = true;
				}
				else
				{
					ExternalAuthentication.ConfigurationTracer.TraceError<X509Certificate2>(0L, "Certificate is not valid: {0}", x509Certificate);
					ExternalAuthentication.EventLogger.LogEvent(CommonEventLogConstants.Tuple_FederationTrustCertificateExpired, x509Certificate.Thumbprint, new object[]
					{
						x509Certificate.Thumbprint,
						objectId.ToString()
					});
				}
			}
			if (!flag)
			{
				ExternalAuthentication.ConfigurationTracer.TraceError<string>(0L, "Federation trust is misconfigured. Unable to find at least one valid certificate in {0}", description);
			}
			return flag;
		}

		private static void InitializeNotificationsIfNeeded()
		{
			if (!ExternalAuthentication.subscribedForNotification)
			{
				lock (ExternalAuthentication.subscribedForNotificationLocker)
				{
					if (!ExternalAuthentication.subscribedForNotification)
					{
						LocalServerCache.Change += ExternalAuthentication.LocalServerNotificationHandler;
						FederationTrustCache.Change += ExternalAuthentication.FederationTrustNotificationHandler;
						ExternalAuthentication.subscribedForNotification = true;
					}
				}
			}
		}

		private static void LocalServerNotificationHandler(Server localServer)
		{
			ExternalAuthentication.ConfigurationTracer.TraceDebug(0L, "Changes detected in local server object in AD.");
			if (localServer == null)
			{
				ExternalAuthentication.ConfigurationTracer.TraceDebug(0L, "Ignoring notification because 'localServer' is null.");
				return;
			}
			if (ExternalAuthentication.currentWebProxy == localServer.InternetWebProxy)
			{
				ExternalAuthentication.ConfigurationTracer.TraceDebug(0L, "No need to update ExternalAuthentication because no changes were detected in InternetWebProxy.");
				return;
			}
			if (ExternalAuthentication.currentWebProxy != null && localServer.InternetWebProxy != null && ExternalAuthentication.currentWebProxy.Equals(localServer.InternetWebProxy))
			{
				ExternalAuthentication.ConfigurationTracer.TraceDebug(0L, "No need to update ExternalAuthentication because no changes were detected in InternetWebProxy.");
				return;
			}
			ExternalAuthentication.ConfigurationTracer.TraceDebug<Uri>(0L, "Need to update ExternalAuthentication with new web proxy: {0}", localServer.InternetWebProxy);
			ExternalAuthentication.instance = null;
		}

		private static void FederationTrustNotificationHandler()
		{
			ExternalAuthentication.ConfigurationTracer.TraceDebug(0L, "Changes detected in federation trust object in AD.");
			ExternalAuthentication.instance = null;
		}

		private void ThrowIfNotEnabled()
		{
			if (!this.Enabled)
			{
				ExternalAuthentication.ConfigurationTracer.TraceError(0L, "Throwing exception because callers is trying to use members of ExternalAuthentication that is not enabled.");
				throw new InvalidOperationException();
			}
		}

		private static readonly Trace ConfigurationTracer = ExTraceGlobals.ConfigurationTracer;

		private static ExternalAuthentication instance;

		private static ExEventLog eventLogger;

		private static bool subscribedForNotification;

		private static object subscribedForNotificationLocker = new object();

		private static Uri currentWebProxy;

		private Dictionary<ADObjectId, SecurityTokenService> securityTokenServices;

		private TokenValidator tokenValidator;

		private List<X509Certificate2> certificates;

		private Uri applicationUri;

		private ExternalAuthentication.ExternalAuthenticationFailureType failureType;

		private ExternalAuthentication.ExternalAuthenticationSubFailureType subFailureType;

		public enum ExternalAuthenticationFailureType
		{
			NoFailure,
			UnknownFailure,
			ErrorReadingFederationTrust,
			InvalidFederationCertificate,
			InvalidTokenIssuerCertificate,
			MisconfiguredFederationTrust,
			NoFederationTrust
		}

		public enum ExternalAuthenticationSubFailureType
		{
			NoFailure,
			NoSubCode,
			DirectoryReadError,
			MissingTokenIssuerUri,
			MissingTokenIssuerEpr,
			MissingApplicationUri,
			MissingTokenIssuerCertificate,
			MissingOrgPrivCertificate,
			CannotRetrieveOrgPrivCertificate,
			CertificateNotInStore,
			CertificateNoPrivateKey,
			CertificateExpirationTimeError,
			CertificatePrivateKeyCryptoError,
			WarningApplicationUriSkipped
		}

		private sealed class FederationTrustResults
		{
			public ExternalAuthentication.ExternalAuthenticationFailureType FailureType { get; set; }

			public ExternalAuthentication.ExternalAuthenticationSubFailureType SubFailureType { get; set; }

			public SecurityTokenService SecurityTokenService { get; set; }

			public IEnumerable<X509Certificate2> TokenSignatureCertificates { get; set; }

			public IEnumerable<X509Certificate2> TokenDecryptionCertificates { get; set; }
		}
	}
}
