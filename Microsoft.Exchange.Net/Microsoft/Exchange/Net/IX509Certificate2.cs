using System;
using System.Security;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

namespace Microsoft.Exchange.Net
{
	internal interface IX509Certificate2 : IEquatable<IX509Certificate2>
	{
		X509Certificate2 Certificate { get; }

		IntPtr Handle { get; }

		string Issuer { get; }

		string Subject { get; }

		byte[] Export(X509ContentType contentType);

		byte[] Export(X509ContentType contentType, SecureString password);

		byte[] Export(X509ContentType contentType, string password);

		byte[] GetCertHash();

		string GetCertHashString();

		string GetEffectiveDateString();

		string GetExpirationDateString();

		string GetFormat();

		int GetHashCode();

		string GetKeyAlgorithm();

		byte[] GetKeyAlgorithmParameters();

		string GetKeyAlgorithmParametersString();

		byte[] GetPublicKey();

		string GetPublicKeyString();

		byte[] GetRawCertData();

		string GetRawCertDataString();

		byte[] GetSerialNumber();

		string GetSerialNumberString();

		void Import(byte[] rawData);

		void Import(string fileName);

		void Import(byte[] rawData, SecureString password, X509KeyStorageFlags keyStorageFlags);

		void Import(byte[] rawData, string password, X509KeyStorageFlags keyStorageFlags);

		void Import(string fileName, SecureString password, X509KeyStorageFlags keyStorageFlags);

		void Import(string fileName, string password, X509KeyStorageFlags keyStorageFlags);

		void Reset();

		string ToString();

		string ToString(bool fVerbose);

		bool Archived { get; set; }

		X509ExtensionCollection Extensions { get; }

		string FriendlyName { get; set; }

		bool HasPrivateKey { get; }

		X500DistinguishedName IssuerName { get; }

		DateTime NotAfter { get; }

		DateTime NotBefore { get; }

		AsymmetricAlgorithm PrivateKey { get; set; }

		PublicKey PublicKey { get; }

		byte[] RawData { get; }

		string SerialNumber { get; }

		Oid SignatureAlgorithm { get; }

		X500DistinguishedName SubjectName { get; }

		string Thumbprint { get; }

		int Version { get; }

		bool Verify();
	}
}
