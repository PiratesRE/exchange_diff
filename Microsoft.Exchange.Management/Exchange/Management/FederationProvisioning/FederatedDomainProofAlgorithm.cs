using System;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace Microsoft.Exchange.Management.FederationProvisioning
{
	internal static class FederatedDomainProofAlgorithm
	{
		public static byte[] GetSignature(X509Certificate2 certificate, string domain)
		{
			byte[] buffer = FederatedDomainProofAlgorithm.Canonicalize(domain);
			byte[] result;
			using (SHA1CryptoServiceProvider sha1CryptoServiceProvider = new SHA1CryptoServiceProvider())
			{
				RSACryptoServiceProvider rsacryptoServiceProvider = certificate.PrivateKey as RSACryptoServiceProvider;
				result = rsacryptoServiceProvider.SignData(buffer, sha1CryptoServiceProvider);
			}
			return result;
		}

		private static byte[] Canonicalize(string domain)
		{
			return Encoding.UTF8.GetBytes(domain.Trim().ToLowerInvariant());
		}

		public const string HashAlgorithmName = "SHA-512";
	}
}
