using System;
using System.IdentityModel.Selectors;
using System.IdentityModel.Tokens;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using Microsoft.Exchange.Diagnostics.Components.Services;
using Microsoft.Exchange.Services.Core;
using Microsoft.Exchange.Services.EventLogs;

namespace Microsoft.Exchange.Services.WCF
{
	public class EWSX509CertificateValidator : X509CertificateValidator
	{
		public EWSX509CertificateValidator()
		{
			this.validator = X509CertificateValidator.CreateChainTrustValidator(true, new X509ChainPolicy
			{
				RevocationMode = X509RevocationMode.Online
			});
		}

		public override void Validate(X509Certificate2 certificate)
		{
			Exception ex = null;
			try
			{
				this.validator.Validate(certificate);
			}
			catch (CryptographicException ex2)
			{
				ex = ex2;
			}
			catch (SecurityTokenValidationException ex3)
			{
				ex = ex3;
			}
			if (ex != null)
			{
				ServiceDiagnostics.LogExceptionWithTrace(ServicesEventLogConstants.Tuple_X509CerticateValidatorException, ex.Message, ExTraceGlobals.AuthenticationTracer, this, "[EWSX509CertificateValidator.Validate] hit Exception: {0}.", ex);
				throw ex;
			}
		}

		private readonly X509CertificateValidator validator;
	}
}
