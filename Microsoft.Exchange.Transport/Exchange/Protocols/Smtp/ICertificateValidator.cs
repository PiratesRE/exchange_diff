using System;
using System.Security.Cryptography.X509Certificates;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Net;
using Microsoft.Exchange.Security.Cryptography.X509Certificates;
using Microsoft.Exchange.Transport.Logging;

namespace Microsoft.Exchange.Protocols.Smtp
{
	internal interface ICertificateValidator
	{
		bool MatchCertificateFqdns(SmtpDomainWithSubdomains domain, X509Certificate2 cert, MatchOptions options, IProtocolLogSession logSession);

		bool MatchCertificateFqdns(SmtpDomainWithSubdomains domain, IX509Certificate2 cert, MatchOptions options, IProtocolLogSession logSession);

		string FindBestMatchingCertificateFqdn(MatchableDomain domain, X509Certificate2 cert, MatchOptions options, IProtocolLogSession logSession);

		bool FindBestMatchingCertificateFqdn<T>(MatchableDomainMap<Tuple<X500DistinguishedName, T>> domains, X509Certificate2 cert, MatchOptions options, IProtocolLogSession logSession, out T matchedEntry, out string matchedCertName);

		bool FindBestMatchingCertificateFqdn<T>(MatchableDomainMap<Tuple<X500DistinguishedName, T>> domains, IX509Certificate2 cert, MatchOptions options, IProtocolLogSession logSession, out T matchedEntry, out string matchedCertName);

		ChainValidityStatus ChainValidateAsAnonymous(IX509Certificate2 cert, bool cacheOnlyUrlRetrieval);

		ChainValidityStatus ChainValidateAsAnonymous(X509Certificate2 cert, bool cacheOnlyUrlRetrieval);

		bool ShouldTreatValidationResultAsSuccess(ChainValidityStatus status);
	}
}
