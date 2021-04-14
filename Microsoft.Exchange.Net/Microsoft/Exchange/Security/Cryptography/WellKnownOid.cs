using System;
using System.Security.Cryptography;

namespace Microsoft.Exchange.Security.Cryptography
{
	internal static class WellKnownOid
	{
		public static readonly Oid X957Sha1Dsa = new Oid("1.2.840.10040.4.3");

		public static readonly Oid RsaRsa = new Oid("1.2.840.113549.1.1.1");

		public static readonly Oid RsaSha1Rsa = new Oid("1.2.840.113549.1.1.5");

		public static readonly Oid Sha256Rsa = new Oid("1.2.840.113549.1.1.11");

		public static readonly Oid Sha384Rsa = new Oid("1.2.840.113549.1.1.12");

		public static readonly Oid Sha512Rsa = new Oid("1.2.840.113549.1.1.13");

		public static readonly Oid ExchangeKPK = new Oid("1.2.840.113556.1.4.7000.102.50757", "MLS Server Certificate");

		public static readonly Oid ExchangeMLSKey = new Oid("1.2.840.113556.1.4.7000.102.50767", "MLS Key pair");

		public static readonly Oid PkixKpServerAuth = new Oid("1.3.6.1.5.5.7.3.1");

		public static readonly Oid PkixKpClientAuth = new Oid("1.3.6.1.5.5.7.3.2");

		public static readonly Oid CommonName = new Oid("2.5.4.3");

		public static readonly Oid KeyUsage = new Oid("2.5.29.15");

		public static readonly Oid BasicConstraints = new Oid("2.5.29.19");

		public static readonly Oid SubjectAltName = new Oid("2.5.29.17");

		public static readonly Oid EmailProtection = new Oid("1.3.6.1.5.5.7.3.4");

		public static readonly Oid SubjectKeyIdentifier = new Oid("2.5.29.14");

		public static readonly Oid AnyExtendedKeyUsage = new Oid("2.5.29.37.0");

		public static readonly Oid ECPublicKey = new Oid("1.2.840.10045.2.1");
	}
}
