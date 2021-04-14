using System;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;

namespace Microsoft.Exchange.Security.Cryptography.X509Certificates
{
	internal class CertificateRevocationList
	{
		private CertificateRevocationList(CertificateRevocationList.CrlContext item)
		{
			this.certEncodingType = item.CertEncodingType;
			this.rawData = item.CrlRawData;
			this.store = item.Store;
		}

		public int CertEncodingType
		{
			get
			{
				return this.certEncodingType;
			}
		}

		public byte[] CrlRawData
		{
			get
			{
				return this.rawData;
			}
		}

		public X509Store Store
		{
			get
			{
				return this.store;
			}
		}

		internal static CertificateRevocationList Create(IntPtr bytes)
		{
			return new CertificateRevocationList((CertificateRevocationList.CrlContext)Marshal.PtrToStructure(bytes, typeof(CertificateRevocationList.CrlContext)));
		}

		private int certEncodingType;

		private byte[] rawData;

		private X509Store store;

		private struct CrlContext
		{
			public int CertEncodingType
			{
				get
				{
					return this.certEncodingType;
				}
			}

			public byte[] CrlRawData
			{
				get
				{
					if (this.crlEncodedSize <= 0)
					{
						return null;
					}
					byte[] array = new byte[this.crlEncodedSize];
					Marshal.Copy(this.crlEncoded, array, 0, this.crlEncodedSize);
					return array;
				}
			}

			public X509Store Store
			{
				get
				{
					if (!(this.certStore != IntPtr.Zero))
					{
						return null;
					}
					return new X509Store(this.certStore);
				}
			}

			private int certEncodingType;

			private IntPtr crlEncoded;

			private int crlEncodedSize;

			private IntPtr crlInfo;

			private IntPtr certStore;
		}
	}
}
