using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics.Components.Security;
using Microsoft.Exchange.Security.Authentication;

namespace Microsoft.Exchange.Security.OAuth
{
	internal sealed class LocalConfiguration
	{
		public static LocalConfiguration Load(ValidationResultCollector resultCollector = null)
		{
			Exception ex = null;
			return LocalConfiguration.InternalLoadHelper(resultCollector, false, out ex);
		}

		internal static LocalConfiguration InternalLoadHelper(ValidationResultCollector resultCollector, bool loadTrustedIssuers, out Exception exception)
		{
			ExTraceGlobals.OAuthTracer.TraceDebug(0L, "[LocalConfiguration::InternalLoadHelper] entering");
			if (resultCollector == null)
			{
				resultCollector = ValidationResultCollector.NullInstance;
			}
			AuthServer[] authServers = null;
			PartnerApplication[] partnerApps = null;
			AuthConfig authConfig = null;
			exception = null;
			try
			{
				ADSessionSettings sessionSettings = ADSessionSettings.FromRootOrgScopeSet();
				ITopologyConfigurationSession configSession = DirectorySessionFactory.Default.CreateTopologyConfigurationSession(true, ConsistencyMode.IgnoreInvalid, sessionSettings, 76, "InternalLoadHelper", "f:\\15.00.1497\\sources\\dev\\Security\\src\\Authentication\\OAuth\\LocalConfiguration.cs");
				ADOperationResult adoperationResult = ADNotificationAdapter.TryRunADOperation(delegate()
				{
					authServers = configSession.Find<AuthServer>(null, QueryScope.SubTree, new OrFilter(new QueryFilter[]
					{
						new ComparisonFilter(ComparisonOperator.Equal, AuthServerSchema.Type, AuthServerType.MicrosoftACS),
						new ComparisonFilter(ComparisonOperator.Equal, AuthServerSchema.Type, AuthServerType.AzureAD),
						new ComparisonFilter(ComparisonOperator.Equal, AuthServerSchema.Type, AuthServerType.ADFS)
					}), null, ADGenericPagedReader<AuthServer>.DefaultPageSize);
				});
				if (adoperationResult != ADOperationResult.Success)
				{
					ExTraceGlobals.OAuthTracer.TraceDebug<ADOperationErrorCode, Exception>(0L, "[LocalConfiguration::InternalLoadHelper] fail to load AuthServer with error code '{0}', exception: {1}", adoperationResult.ErrorCode, adoperationResult.Exception);
					resultCollector.Add(SecurityStrings.LoadAuthServer, SecurityStrings.ResultADOperationFailure(adoperationResult.ErrorCode.ToString(), adoperationResult.Exception.Message), ResultType.Error);
					return null;
				}
				resultCollector.Add(SecurityStrings.LoadAuthServer, SecurityStrings.ResultAuthServersFound(authServers.Length), ResultType.Success);
				foreach (AuthServer authServer2 in authServers)
				{
					LocalizedString task = SecurityStrings.CheckAuthServer(authServer2.Name);
					if (authServer2.Enabled)
					{
						resultCollector.Add(task, SecurityStrings.ResultAuthServerOK, ResultType.Success);
					}
					else
					{
						resultCollector.Add(task, SecurityStrings.ResultAuthServerDisabled, ResultType.Warning);
					}
				}
				authServers = (from authServer in authServers
				where authServer.Enabled
				select authServer).OrderBy(delegate(AuthServer authServer)
				{
					if (!OAuthCommon.IsRealmEmpty(authServer.Realm))
					{
						return authServer.Realm;
					}
					return string.Empty;
				}).ToArray<AuthServer>();
				adoperationResult = ADNotificationAdapter.TryRunADOperation(delegate()
				{
					partnerApps = configSession.Find<PartnerApplication>(null, QueryScope.SubTree, null, null, ADGenericPagedReader<PartnerApplication>.DefaultPageSize);
				});
				if (adoperationResult != ADOperationResult.Success)
				{
					ExTraceGlobals.OAuthTracer.TraceDebug<ADOperationErrorCode, Exception>(0L, "[LocalConfiguration::InternalLoadHelper] fail to load PartnerApplication with error code '{0}', exception: {1}", adoperationResult.ErrorCode, adoperationResult.Exception);
					resultCollector.Add(SecurityStrings.LoadPartnerApplication, SecurityStrings.ResultADOperationFailure(adoperationResult.ErrorCode.ToString(), adoperationResult.Exception.Message), ResultType.Error);
					return null;
				}
				resultCollector.Add(SecurityStrings.LoadPartnerApplication, SecurityStrings.ResultPartnerApplicationsFound(partnerApps.Length), ResultType.Success);
				foreach (PartnerApplication partnerApplication in partnerApps)
				{
					LocalizedString task2 = SecurityStrings.CheckPartnerApplication(partnerApplication.Name);
					if (partnerApplication.Enabled)
					{
						resultCollector.Add(task2, SecurityStrings.ResultPartnerApplicationOK, ResultType.Success);
					}
					else
					{
						resultCollector.Add(task2, SecurityStrings.ResultPartnerApplicationDisabled, ResultType.Warning);
					}
				}
				partnerApps = (from pa in partnerApps
				where pa.Enabled
				select pa).ToArray<PartnerApplication>();
				adoperationResult = ADNotificationAdapter.TryRunADOperation(delegate()
				{
					authConfig = configSession.Read<AuthConfig>(configSession.GetOrgContainerId().GetChildId(AuthConfig.ContainerName));
				});
				if (adoperationResult == ADOperationResult.Success)
				{
					resultCollector.Add(SecurityStrings.LoadAuthConfig, SecurityStrings.ResultAuthConfigFound, ResultType.Success);
					return new LocalConfiguration(authConfig, authServers, partnerApps, resultCollector, loadTrustedIssuers);
				}
				ExTraceGlobals.OAuthTracer.TraceDebug<ADOperationErrorCode, Exception>(0L, "[LocalConfiguration::InternalLoadHelper] fail to load AuthConfig with error code '{0}', exception: {1}", adoperationResult.ErrorCode, adoperationResult.Exception);
				resultCollector.Add(SecurityStrings.LoadAuthConfig, SecurityStrings.ResultADOperationFailure(adoperationResult.ErrorCode.ToString(), adoperationResult.Exception.Message), ResultType.Error);
				return null;
			}
			catch (LocalizedException ex)
			{
				exception = ex;
				ExTraceGlobals.OAuthTracer.TraceWarning<Exception>(0L, "[LocalConfiguration::InternalLoadHelper] hit the exception {0}", exception);
				resultCollector.Add(SecurityStrings.LoadConfiguration, SecurityStrings.ResultLoadConfigurationException(exception), ResultType.Error);
			}
			return null;
		}

		public LocalConfiguration(string applicationId, string realm, X509Certificate2 signingKey, X509Certificate2 previousSigningKey, AuthServer[] authServers, PartnerApplication[] partnerApps, bool loadTrustedIssuers = true) : this(applicationId, realm, signingKey, previousSigningKey, null, authServers, partnerApps, loadTrustedIssuers)
		{
		}

		public LocalConfiguration(string applicationId, string realm, X509Certificate2 signingKey, X509Certificate2 previousSigningKey, X509Certificate2 nextSigningKey, AuthServer[] authServers, PartnerApplication[] partnerApps, bool loadTrustedIssuers = true)
		{
			this.Initialize(applicationId, realm, signingKey, previousSigningKey, nextSigningKey, authServers, partnerApps, loadTrustedIssuers);
		}

		private LocalConfiguration(AuthConfig authConfig, AuthServer[] authServers, PartnerApplication[] partnerApps, ValidationResultCollector resultCollector, bool loadTrustedIssuers)
		{
			OAuthCommon.VerifyNonNullArgument("authConfig", authConfig);
			OAuthCommon.VerifyNonNullArgument("authServers", authServers);
			OAuthCommon.VerifyNonNullArgument("partnerApps", partnerApps);
			string serviceName = authConfig.ServiceName;
			if (resultCollector != null)
			{
				if (string.Equals(serviceName, WellknownPartnerApplicationIdentifiers.Exchange, StringComparison.OrdinalIgnoreCase))
				{
					resultCollector.Add(SecurityStrings.CheckServiceName, SecurityStrings.ResultMatchServiceNameDefaultValue(serviceName), ResultType.Success);
				}
				else
				{
					resultCollector.Add(SecurityStrings.CheckServiceName, SecurityStrings.ResultDidNotMatchServiceNameDefaultValue(serviceName, WellknownPartnerApplicationIdentifiers.Exchange), ResultType.Warning);
				}
			}
			string text = string.Empty;
			if (!AuthCommon.IsMultiTenancyEnabled)
			{
				if (!string.IsNullOrEmpty(authConfig.Realm))
				{
					text = authConfig.Realm;
					if (resultCollector != null)
					{
						resultCollector.Add(SecurityStrings.CheckAuthConfigRealm, SecurityStrings.ResultAuthConfigRealmOverwrote(text), ResultType.Warning);
					}
				}
				else
				{
					text = OAuthCommon.DefaultAcceptedDomain;
				}
			}
			X509Certificate2 x509Certificate = null;
			X509Certificate2 x509Certificate2 = null;
			X509Certificate2 x509Certificate3 = null;
			string currentCertificateThumbprint = authConfig.CurrentCertificateThumbprint;
			string previousCertificateThumbprint = authConfig.PreviousCertificateThumbprint;
			string nextCertificateThumbprint = authConfig.NextCertificateThumbprint;
			X509Store x509Store = null;
			try
			{
				x509Store = new X509Store(StoreLocation.LocalMachine);
				x509Store.Open(OpenFlags.ReadOnly);
				x509Certificate = LocalConfiguration.ReadCertificate(SecurityStrings.CheckCurrentCertificate, x509Store, currentCertificateThumbprint, resultCollector);
				x509Certificate2 = LocalConfiguration.ReadCertificate(SecurityStrings.CheckPreviousCertificate, x509Store, previousCertificateThumbprint, resultCollector);
				x509Certificate3 = LocalConfiguration.ReadCertificate(SecurityStrings.CheckNextCertificate, x509Store, nextCertificateThumbprint, resultCollector);
			}
			catch (CryptographicException ex)
			{
				if (resultCollector != null)
				{
					resultCollector.Add(SecurityStrings.ReadCertificates, SecurityStrings.ResultReadCertificatesException(ex), ResultType.Error);
				}
				OAuthCommon.EventLogger.LogEvent(SecurityEventLogConstants.Tuple_OAuthFailToReadSigningCertificates, string.Empty, new object[]
				{
					ex
				});
				ExTraceGlobals.OAuthTracer.TraceError<string, string, CryptographicException>(0L, "[LocalConfiguration::ctor] hitting CryptographicException when retrieving certs with thumbprint {0}, {1}. The exception detail: {2}", currentCertificateThumbprint, previousCertificateThumbprint, ex);
			}
			finally
			{
				if (x509Store != null)
				{
					x509Store.Close();
				}
			}
			if (x509Certificate == null && x509Certificate2 == null)
			{
				ExTraceGlobals.OAuthTracer.TraceWarning<string, string>(0L, "[LocalConfiguration::ctor] no signing cert was found, authconfig has thumbrpints {0}, {1}.", authConfig.CurrentCertificateThumbprint, authConfig.PreviousCertificateThumbprint);
			}
			this.Initialize(serviceName, text, x509Certificate, x509Certificate2, x509Certificate3, authServers, partnerApps, loadTrustedIssuers);
		}

		private static X509Certificate2 ReadCertificate(LocalizedString task, X509Store store, string thumbprint, ValidationResultCollector resultCollector)
		{
			if (!string.IsNullOrEmpty(thumbprint))
			{
				X509Certificate2Collection x509Certificate2Collection = store.Certificates.Find(X509FindType.FindByThumbprint, thumbprint, false);
				if (x509Certificate2Collection.Count > 0)
				{
					if (x509Certificate2Collection[0].HasPrivateKey)
					{
						X509Certificate2 x509Certificate = x509Certificate2Collection[0];
						if (!(x509Certificate.NotAfter <= DateTime.UtcNow) && !(x509Certificate.NotBefore >= DateTime.UtcNow))
						{
							if (resultCollector != null)
							{
								resultCollector.Add(task, SecurityStrings.ResultCertValid(thumbprint), ResultType.Success);
							}
							return x509Certificate;
						}
						if (resultCollector != null)
						{
							resultCollector.Add(task, SecurityStrings.ResultCertInvalid(thumbprint, x509Certificate.NotBefore, x509Certificate.NotAfter), ResultType.Warning);
						}
					}
					else if (resultCollector != null)
					{
						resultCollector.Add(task, SecurityStrings.ResultCertHasNoPrivateKey(thumbprint), ResultType.Warning);
					}
				}
				else
				{
					if (resultCollector != null)
					{
						resultCollector.Add(task, SecurityStrings.ResultCertNotFound(thumbprint), ResultType.Warning);
					}
					OAuthCommon.EventLogger.LogEvent(SecurityEventLogConstants.Tuple_OAuthSigningCertificateNotFoundOrMissingPrivateKey, thumbprint, new object[]
					{
						thumbprint
					});
					ExTraceGlobals.OAuthTracer.TraceWarning<string>(0L, "[LocalConfiguration::ReadCertificate] The certificate with thumbprint {0} was not found or has no private key.", thumbprint);
				}
			}
			else if (resultCollector != null)
			{
				resultCollector.Add(task, SecurityStrings.ResultThumbprintNotSet, ResultType.Warning);
			}
			return null;
		}

		private void Initialize(string applicationId, string realm, X509Certificate2 signingKey, X509Certificate2 previousSigningKey, X509Certificate2 nextSigningKey, AuthServer[] authServers, PartnerApplication[] partnerApps, bool loadTrustedIssuers)
		{
			this.applicationId = applicationId;
			this.singleTenancyRealm = realm;
			this.authServers = authServers;
			this.partnerApps = partnerApps;
			this.exchangeApp = Array.Find<PartnerApplication>(partnerApps, (PartnerApplication partnerApp) => OAuthCommon.IsIdMatch(partnerApp.ApplicationIdentifier, WellknownPartnerApplicationIdentifiers.Exchange));
			this.signingKey = signingKey;
			this.previousSigningKey = previousSigningKey;
			this.nextSigningKey = nextSigningKey;
			List<X509Certificate2> list = new List<X509Certificate2>(3);
			if (signingKey != null)
			{
				list.Add(signingKey);
			}
			if (previousSigningKey != null)
			{
				list.Add(previousSigningKey);
			}
			if (nextSigningKey != null)
			{
				list.Add(nextSigningKey);
			}
			this.allSigningCertificates = list.ToArray<X509Certificate2>();
			if (loadTrustedIssuers)
			{
				List<AuthServer> list2 = new List<AuthServer>(4);
				List<TrustedIssuer> list3 = new List<TrustedIssuer>(20);
				TrustedIssuer item = null;
				foreach (AuthServer authServer in this.authServers)
				{
					if (TrustedIssuer.TryCreateFromAuthServer(authServer, out item))
					{
						list3.Add(item);
					}
					if (!string.IsNullOrEmpty(authServer.AuthorizationEndpoint) && (authServer.Type == AuthServerType.AzureAD || authServer.Type == AuthServerType.ADFS) && authServer.IsDefaultAuthorizationEndpoint)
					{
						list2.Add(authServer);
					}
				}
				foreach (PartnerApplication partnerApplication in this.partnerApps)
				{
					if (!partnerApplication.UseAuthServer && TrustedIssuer.TryCreateFromPartnerApplication(partnerApplication, out item))
					{
						list3.Add(item);
					}
				}
				this.trustedIssuers = list3.ToArray();
				this.trustedIssuersString = string.Join(",", from trustedIssuer in list3
				select trustedIssuer.IssuerMetadata.ToTrustedIssuerString());
				string arg = string.Join(",", from trustedIssuer in list3
				where trustedIssuer.IssuerMetadata.Kind == IssuerKind.PartnerApp || trustedIssuer.IssuerMetadata.Kind == IssuerKind.ACS
				where !AuthCommon.IsMultiTenancyEnabled || trustedIssuer.IssuerMetadata.Kind != IssuerKind.ACS || trustedIssuer.IssuerMetadata.HasEmptyRealm
				where trustedIssuer.IssuerMetadata.Kind != IssuerKind.PartnerApp || !(trustedIssuer.IssuerMetadata.Realm == Constants.MSExchangeSelfIssuingTokenRealm)
				select trustedIssuer.IssuerMetadata.ToTrustedIssuerString());
				string text = null;
				if (list2.Count == 1)
				{
					text = list2[0].AuthorizationEndpoint;
				}
				else if (list2.Count > 1)
				{
					string text2 = string.Join(",", from s in list2
					select s.Name);
					OAuthCommon.EventLogger.LogEvent(SecurityEventLogConstants.Tuple_OAuthMoreThanOneAuthServerWithAuthorizationEndpoint, text2, new object[]
					{
						text2
					});
				}
				StringBuilder stringBuilder = new StringBuilder(200);
				stringBuilder.Append(Constants.BearerAuthenticationType);
				stringBuilder.AppendFormat(" {0}=\"{1}\",", Constants.ChallengeTokens.ClientId, this.applicationId);
				stringBuilder.AppendFormat(" {0}=\"{1}\"", Constants.ChallengeTokens.TrustedIssuers, arg);
				this.challengeResponseString = stringBuilder.ToString();
				if (!string.IsNullOrEmpty(text))
				{
					stringBuilder.AppendFormat(", {0}=\"{1}\"", Constants.ChallengeTokens.AuthorizationUri, text);
				}
				this.challengeResponseStringWithClientProfileEnabled = stringBuilder.ToString();
			}
		}

		public string ApplicationId
		{
			get
			{
				return this.applicationId;
			}
		}

		public string SingleTenancyRealm
		{
			get
			{
				return this.singleTenancyRealm;
			}
		}

		public X509Certificate2 SigningKey
		{
			get
			{
				return this.signingKey;
			}
		}

		public X509Certificate2 PreviousSigningKey
		{
			get
			{
				return this.previousSigningKey;
			}
		}

		public X509Certificate2 NextSigningKey
		{
			get
			{
				return this.nextSigningKey;
			}
		}

		public AuthServer[] AuthServers
		{
			get
			{
				return this.authServers;
			}
		}

		public PartnerApplication[] PartnerApplications
		{
			get
			{
				return this.partnerApps;
			}
		}

		public PartnerApplication ExchangeApplication
		{
			get
			{
				return this.exchangeApp;
			}
		}

		public string TrustedIssuersAsString
		{
			get
			{
				return this.trustedIssuersString;
			}
		}

		public string ChallengeResponseString
		{
			get
			{
				return this.challengeResponseString;
			}
		}

		public string ChallengeResponseStringWithClientProfileEnabled
		{
			get
			{
				return this.challengeResponseStringWithClientProfileEnabled;
			}
		}

		public TrustedIssuer[] TrustedIssuers
		{
			get
			{
				return this.trustedIssuers;
			}
		}

		public X509Certificate2[] Certificates
		{
			get
			{
				return this.allSigningCertificates;
			}
		}

		private string applicationId;

		private string singleTenancyRealm;

		private X509Certificate2 signingKey;

		private X509Certificate2 previousSigningKey;

		private X509Certificate2 nextSigningKey;

		private X509Certificate2[] allSigningCertificates;

		private AuthServer[] authServers;

		private PartnerApplication[] partnerApps;

		private PartnerApplication exchangeApp;

		private TrustedIssuer[] trustedIssuers;

		private string trustedIssuersString;

		private string challengeResponseString;

		private string challengeResponseStringWithClientProfileEnabled;
	}
}
