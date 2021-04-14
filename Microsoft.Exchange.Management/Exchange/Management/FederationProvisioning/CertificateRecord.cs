using System;

namespace Microsoft.Exchange.Management.FederationProvisioning
{
	internal sealed class CertificateRecord : IEquatable<CertificateRecord>
	{
		public bool Equals(CertificateRecord other)
		{
			return this.Thumbprint == other.Thumbprint;
		}

		public FederationCertificateType Type;

		public string Thumbprint;
	}
}
