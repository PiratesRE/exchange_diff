using System;
using System.Collections.Generic;
using System.IdentityModel.Selectors;
using System.Security.Cryptography.X509Certificates;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Net.WSTrust
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal sealed class CustomX509CertificateValidator : X509CertificateValidator
	{
		internal CustomX509CertificateValidator(IEnumerable<X509Certificate2> trustedCertificates)
		{
			this.trustedCertificates = trustedCertificates;
		}

		public override void Validate(X509Certificate2 certificate)
		{
			foreach (X509Certificate2 b in this.trustedCertificates)
			{
				if (this.IsSameRawCertData(certificate, b))
				{
					return;
				}
			}
			X509CertificateValidator.ChainTrust.Validate(certificate);
		}

		private bool IsSameRawCertData(X509Certificate2 a, X509Certificate2 b)
		{
			byte[] rawCertData = a.GetRawCertData();
			byte[] rawCertData2 = b.GetRawCertData();
			if (rawCertData.Length != rawCertData2.Length)
			{
				return false;
			}
			for (int i = 0; i < rawCertData.Length; i++)
			{
				if (rawCertData[i] != rawCertData2[i])
				{
					return false;
				}
			}
			return true;
		}

		private IEnumerable<X509Certificate2> trustedCertificates;
	}
}
