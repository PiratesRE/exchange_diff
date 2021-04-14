using System;
using System.Security;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Net
{
	internal class X509Certificate2Wrapper : IX509Certificate2, IEquatable<IX509Certificate2>
	{
		public X509Certificate2Wrapper(X509Certificate2 certificate)
		{
			ArgumentValidator.ThrowIfNull("certificate", certificate);
			this.certificate = certificate;
		}

		public X509Certificate2 Certificate
		{
			get
			{
				return this.certificate;
			}
		}

		public override bool Equals(object other)
		{
			return this.certificate.Equals(other as X509Certificate2Wrapper);
		}

		public bool Equals(IX509Certificate2 other)
		{
			return this.certificate.Equals(other.Certificate);
		}

		public override int GetHashCode()
		{
			return this.certificate.GetHashCode();
		}

		public IntPtr Handle
		{
			get
			{
				return this.certificate.Handle;
			}
		}

		public string Issuer
		{
			get
			{
				return this.certificate.Issuer;
			}
		}

		public string Subject
		{
			get
			{
				return this.certificate.Subject;
			}
		}

		public byte[] Export(X509ContentType contentType)
		{
			return this.certificate.Export(contentType);
		}

		public byte[] Export(X509ContentType contentType, SecureString password)
		{
			return this.certificate.Export(contentType, password);
		}

		public byte[] Export(X509ContentType contentType, string password)
		{
			return this.certificate.Export(contentType, password);
		}

		public byte[] GetCertHash()
		{
			return this.certificate.GetCertHash();
		}

		public string GetCertHashString()
		{
			return this.certificate.GetCertHashString();
		}

		public string GetEffectiveDateString()
		{
			return this.certificate.GetEffectiveDateString();
		}

		public string GetExpirationDateString()
		{
			return this.certificate.GetExpirationDateString();
		}

		public string GetFormat()
		{
			return this.certificate.GetFormat();
		}

		public string GetKeyAlgorithm()
		{
			return this.certificate.GetKeyAlgorithm();
		}

		public byte[] GetKeyAlgorithmParameters()
		{
			return this.certificate.GetKeyAlgorithmParameters();
		}

		public string GetKeyAlgorithmParametersString()
		{
			return this.certificate.GetKeyAlgorithmParametersString();
		}

		public byte[] GetPublicKey()
		{
			return this.certificate.GetPublicKey();
		}

		public string GetPublicKeyString()
		{
			return this.certificate.GetPublicKeyString();
		}

		public byte[] GetRawCertData()
		{
			return this.certificate.GetRawCertData();
		}

		public string GetRawCertDataString()
		{
			return this.certificate.GetRawCertDataString();
		}

		public byte[] GetSerialNumber()
		{
			return this.certificate.GetSerialNumber();
		}

		public string GetSerialNumberString()
		{
			return this.certificate.GetSerialNumberString();
		}

		public void Import(byte[] rawData)
		{
			this.certificate.Import(rawData);
		}

		public void Import(string fileName)
		{
			this.certificate.Import(fileName);
		}

		public void Import(byte[] rawData, SecureString password, X509KeyStorageFlags keyStorageFlags)
		{
			this.certificate.Import(rawData, password, keyStorageFlags);
		}

		public void Import(byte[] rawData, string password, X509KeyStorageFlags keyStorageFlags)
		{
			this.certificate.Import(rawData, password, keyStorageFlags);
		}

		public void Import(string fileName, SecureString password, X509KeyStorageFlags keyStorageFlags)
		{
			this.certificate.Import(fileName, password, keyStorageFlags);
		}

		public void Import(string fileName, string password, X509KeyStorageFlags keyStorageFlags)
		{
			this.certificate.Import(fileName, password, keyStorageFlags);
		}

		public void Reset()
		{
			this.certificate.Reset();
		}

		public string ToString(bool fVerbose)
		{
			return this.certificate.ToString(fVerbose);
		}

		public bool Archived
		{
			get
			{
				return this.certificate.Archived;
			}
			set
			{
				this.certificate.Archived = value;
			}
		}

		public X509ExtensionCollection Extensions
		{
			get
			{
				return this.certificate.Extensions;
			}
		}

		public string FriendlyName
		{
			get
			{
				return this.certificate.FriendlyName;
			}
			set
			{
				this.certificate.FriendlyName = value;
			}
		}

		public bool HasPrivateKey
		{
			get
			{
				return this.certificate.HasPrivateKey;
			}
		}

		public X500DistinguishedName IssuerName
		{
			get
			{
				return this.certificate.IssuerName;
			}
		}

		public DateTime NotAfter
		{
			get
			{
				return this.certificate.NotAfter;
			}
		}

		public DateTime NotBefore
		{
			get
			{
				return this.certificate.NotBefore;
			}
		}

		public AsymmetricAlgorithm PrivateKey
		{
			get
			{
				return this.certificate.PrivateKey;
			}
			set
			{
				this.certificate.PrivateKey = value;
			}
		}

		public PublicKey PublicKey
		{
			get
			{
				return this.certificate.PublicKey;
			}
		}

		public byte[] RawData
		{
			get
			{
				return this.certificate.RawData;
			}
		}

		public string SerialNumber
		{
			get
			{
				return this.certificate.SerialNumber;
			}
		}

		public Oid SignatureAlgorithm
		{
			get
			{
				return this.certificate.SignatureAlgorithm;
			}
		}

		public X500DistinguishedName SubjectName
		{
			get
			{
				return this.certificate.SubjectName;
			}
		}

		public string Thumbprint
		{
			get
			{
				return this.certificate.Thumbprint;
			}
		}

		public int Version
		{
			get
			{
				return this.certificate.Version;
			}
		}

		public bool Verify()
		{
			return this.certificate.Verify();
		}

		private readonly X509Certificate2 certificate;
	}
}
