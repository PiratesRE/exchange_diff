using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Net;
using Microsoft.Exchange.SecureMail;
using Microsoft.Exchange.Security.Cryptography.X509Certificates;
using Microsoft.Exchange.Transport;
using Microsoft.Exchange.Transport.Logging;

namespace Microsoft.Exchange.Protocols.Smtp
{
	internal class CertificateValidator : ICertificateValidator
	{
		public CertificateValidator(ChainEnginePool pool, CertificateValidationResultCache anonymousValidationResultCache, TransportAppConfig.SecureMailConfig config)
		{
			ArgumentValidator.ThrowIfNull("pool", pool);
			ArgumentValidator.ThrowIfNull("config", config);
			this.pool = pool;
			this.anonymousValidationResultCache = anonymousValidationResultCache;
			this.subjectAlternativeNameLimit = config.SubjectAlternativeNameLimit;
			this.treatTransientCRLFailuresAsSuccess = config.TreatCRLTransientFailuresAsSuccessEnabled;
		}

		public bool MatchCertificateFqdns(SmtpDomainWithSubdomains domain, X509Certificate2 cert, MatchOptions options, IProtocolLogSession logSession)
		{
			ArgumentValidator.ThrowIfNull("cert", cert);
			return CertificateValidator.MatchCertificateFqdns(new MatchableDomain(domain), this.GetFqdns(cert, logSession), options);
		}

		public bool MatchCertificateFqdns(SmtpDomainWithSubdomains domain, IX509Certificate2 cert, MatchOptions options, IProtocolLogSession logSession)
		{
			ArgumentValidator.ThrowIfNull("cert", cert);
			return CertificateValidator.MatchCertificateFqdns(new MatchableDomain(domain), this.GetFqdns(cert.Certificate, logSession), options);
		}

		public string FindBestMatchingCertificateFqdn(MatchableDomain domain, X509Certificate2 cert, MatchOptions options, IProtocolLogSession logSession)
		{
			ArgumentValidator.ThrowIfNull("cert", cert);
			int num = -1;
			return CertificateValidator.FindBestMatchingCertificateFqdn(domain, this.GetFqdns(cert, logSession), options, ref num);
		}

		public bool FindBestMatchingCertificateFqdn<T>(MatchableDomainMap<Tuple<X500DistinguishedName, T>> domains, X509Certificate2 cert, MatchOptions options, IProtocolLogSession logSession, out T matchedEntry, out string matchedCertName)
		{
			return CertificateValidator.FindBestMatchingCertificateFqdn<T>(domains, this.GetFqdns(cert, logSession), cert.IssuerName, options, out matchedEntry, out matchedCertName);
		}

		public bool FindBestMatchingCertificateFqdn<T>(MatchableDomainMap<Tuple<X500DistinguishedName, T>> domains, IX509Certificate2 cert, MatchOptions options, IProtocolLogSession logSession, out T matchedEntry, out string matchedCertName)
		{
			ArgumentValidator.ThrowIfNull("cert", cert);
			return CertificateValidator.FindBestMatchingCertificateFqdn<T>(domains, this.GetFqdns(cert.Certificate, logSession), cert.IssuerName, options, out matchedEntry, out matchedCertName);
		}

		public ChainValidityStatus ChainValidateAsAnonymous(IX509Certificate2 cert, bool cacheOnlyUrlRetrieval)
		{
			if (cert == null)
			{
				return ChainValidityStatus.EmptyCertificate;
			}
			return this.ChainValidateAsAnonymous(cert.Certificate, cacheOnlyUrlRetrieval);
		}

		public ChainValidityStatus ChainValidateAsAnonymous(X509Certificate2 cert, bool cacheOnlyUrlRetrieval)
		{
			if (cert == null)
			{
				return ChainValidityStatus.EmptyCertificate;
			}
			ChainValidityStatus chainValidityStatus;
			if (this.anonymousValidationResultCache != null && this.anonymousValidationResultCache.TryGetValue(cert, out chainValidityStatus))
			{
				return chainValidityStatus;
			}
			chainValidityStatus = this.ChainValidateInternal(null, cert, true, cacheOnlyUrlRetrieval);
			if (CertificateValidator.IsTransientCRLFailure(chainValidityStatus) && this.treatTransientCRLFailuresAsSuccess)
			{
				chainValidityStatus = this.ChainValidateInternal(null, cert, true, cacheOnlyUrlRetrieval);
			}
			if (this.anonymousValidationResultCache != null && !CertificateValidator.IsTransientCRLFailure(chainValidityStatus))
			{
				this.anonymousValidationResultCache.TryAdd(cert, chainValidityStatus);
			}
			return chainValidityStatus;
		}

		public bool ShouldTreatValidationResultAsSuccess(ChainValidityStatus status)
		{
			return status == ChainValidityStatus.Valid || (this.treatTransientCRLFailuresAsSuccess && CertificateValidator.IsTransientCRLFailure(status));
		}

		private static bool IsTransientCRLFailure(ChainValidityStatus status)
		{
			return status == (ChainValidityStatus)2148081683U || status == (ChainValidityStatus)2148204814U || status == (ChainValidityStatus)2148081682U;
		}

		private static bool MatchCertificateFqdns(MatchableDomain domain, IEnumerable<string> certNames, MatchOptions options)
		{
			ArgumentValidator.ThrowIfNull("domain", domain);
			ArgumentValidator.ThrowIfNull("certNames", certNames);
			return certNames.Any((string certName) => -1 != domain.MatchCertName(certName, options, -1));
		}

		private static string FindBestMatchingCertificateFqdn(MatchableDomain domain, IEnumerable<string> certNames, MatchOptions options, ref int wildcardMatchDotCount)
		{
			ArgumentValidator.ThrowIfNull("domain", domain);
			ArgumentValidator.ThrowIfNull("certNames", certNames);
			string result = null;
			foreach (string text in certNames)
			{
				int num = domain.MatchCertName(text, options, wildcardMatchDotCount);
				int num2 = num;
				if (num2 != -1)
				{
					if (num2 == 2147483647)
					{
						wildcardMatchDotCount = int.MaxValue;
						return text;
					}
					if (num > wildcardMatchDotCount)
					{
						result = text;
						wildcardMatchDotCount = num;
					}
				}
			}
			return result;
		}

		private static bool FindBestMatchingCertificateFqdn<T>(IEnumerable<KeyValuePair<MatchableDomain, Tuple<X500DistinguishedName, T>>> domains, IList<string> certNames, X500DistinguishedName certIssuerDN, MatchOptions options, out T matchedEntry, out string matchedCertName)
		{
			ArgumentValidator.ThrowIfNull("domains", domains);
			matchedEntry = default(T);
			matchedCertName = null;
			int num = -1;
			foreach (KeyValuePair<MatchableDomain, Tuple<X500DistinguishedName, T>> keyValuePair in domains)
			{
				string text = CertificateValidator.FindBestMatchingCertificateFqdn(keyValuePair.Key, certNames, options, ref num);
				if (text != null && (keyValuePair.Value.Item1 == null || (certIssuerDN != null && string.Compare(certIssuerDN.Name, keyValuePair.Value.Item1.Name, StringComparison.OrdinalIgnoreCase) == 0)))
				{
					matchedCertName = text;
					matchedEntry = keyValuePair.Value.Item2;
					if (num == 2147483647)
					{
						return true;
					}
				}
			}
			return matchedCertName != null;
		}

		private ChainValidityStatus ChainValidateInternal(string domain, X509Certificate2 cert, bool validateAsAnonymous, bool cacheOnlyUrlRetrieval)
		{
			ArgumentValidator.ThrowIfNull("cert", cert);
			ChainBuildParameter parameter = new ChainBuildParameter(AndChainMatchIssuer.PkixKpServerAuth, TimeSpan.FromSeconds(10.0), false, TimeSpan.Zero);
			SSLChainPolicyParameters options = new SSLChainPolicyParameters(domain ?? "anydomain.com", ChainPolicyOptions.None, (domain == null) ? SSLPolicyAuthorizationOptions.IgnoreCertCNInvalid : SSLPolicyAuthorizationOptions.None, SSLPolicyAuthorizationType.Server);
			ChainBuildOptions options2 = ChainBuildOptions.CacheEndCert | ChainBuildOptions.RevocationCheckChainExcludeRoot | ChainBuildOptions.RevocationAccumulativeTimeout | (cacheOnlyUrlRetrieval ? ChainBuildOptions.CacheOnlyUrlRetrieval : ChainBuildOptions.DisableAia);
			ChainValidityStatus result;
			using (ChainEngine engine = this.pool.GetEngine())
			{
				ChainContext chainContext = validateAsAnonymous ? engine.BuildAsAnonymous(cert, options2, parameter) : engine.Build(cert, options2, parameter);
				if (chainContext == null)
				{
					result = (ChainValidityStatus)2148204810U;
				}
				else
				{
					using (chainContext)
					{
						ChainSummary chainSummary = chainContext.Validate(options);
						result = chainSummary.Status;
					}
				}
			}
			return result;
		}

		private IList<string> GetFqdns(X509Certificate2 cert, IProtocolLogSession logSession)
		{
			int num;
			IList<string> fqdns = TlsCertificateInfo.GetFQDNs(cert, this.subjectAlternativeNameLimit, out num);
			if (num > this.subjectAlternativeNameLimit)
			{
				if (logSession != null)
				{
					logSession.LogInformation(ProtocolLoggingLevel.Verbose, null, "Certificate '{0}' with {1} SANs exceeded SAN limit {2}; SANs ignored", new object[]
					{
						cert.Thumbprint,
						num,
						this.subjectAlternativeNameLimit
					});
				}
				Utils.EventLogger.LogEvent(TransportEventLogConstants.Tuple_SubjectAlternativeNameLimitExceeded, cert.Thumbprint, new object[]
				{
					cert.Thumbprint,
					num,
					this.subjectAlternativeNameLimit
				});
			}
			return fqdns;
		}

		public const int ExactMatchResult = 2147483647;

		public const int NoMatchResult = -1;

		private readonly bool treatTransientCRLFailuresAsSuccess;

		private readonly ChainEnginePool pool;

		private readonly CertificateValidationResultCache anonymousValidationResultCache;

		private readonly int subjectAlternativeNameLimit;
	}
}
