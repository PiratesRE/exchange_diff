using System;
using System.IdentityModel.Selectors;
using System.IdentityModel.Tokens;
using System.Security.Cryptography.X509Certificates;
using System.Web;

namespace Microsoft.Exchange.Autodiscover.WCF
{
	public class AutodiscoverX509CertificateValidator : X509CertificateValidator
	{
		public override void Validate(X509Certificate2 certificate)
		{
			try
			{
				X509CertificateValidator.ChainTrust.Validate(certificate);
			}
			catch (SecurityTokenValidationException)
			{
				PerformanceCounters.UpdateCertAuthRequestsFailed(HttpContext.Current.Request.UserAgent);
				throw;
			}
		}
	}
}
