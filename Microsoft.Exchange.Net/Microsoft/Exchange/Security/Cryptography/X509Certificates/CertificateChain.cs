using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Microsoft.Exchange.Security.Cryptography.X509Certificates
{
	internal class CertificateChain
	{
		private CertificateChain(CertificateChain.CertSimpleChain item)
		{
			this.trust = item.Status;
			this.expires = item.Expires;
			this.elements = item.Elements;
		}

		public TrustStatus Status
		{
			get
			{
				return this.trust.error;
			}
		}

		public TrustInformation TrustInformation
		{
			get
			{
				return this.trust.information;
			}
		}

		public IList<ChainElement> Elements
		{
			get
			{
				return this.elements;
			}
		}

		public DateTime Expires
		{
			get
			{
				return this.expires;
			}
		}

		internal static bool IsSelfSigned(IntPtr bytes)
		{
			CertificateChain.CertSimpleChain certSimpleChain = (CertificateChain.CertSimpleChain)Marshal.PtrToStructure(bytes, typeof(CertificateChain.CertSimpleChain));
			if (certSimpleChain.ElementCount != 1)
			{
				return false;
			}
			IntPtr bytes2 = Marshal.ReadIntPtr(certSimpleChain.RawElements);
			return ChainElement.IsSelfSigned(bytes2);
		}

		internal static CertificateChain Create(IntPtr bytes)
		{
			return new CertificateChain((CertificateChain.CertSimpleChain)Marshal.PtrToStructure(bytes, typeof(CertificateChain.CertSimpleChain)));
		}

		private CapiNativeMethods.CertTrustStatus trust;

		private IList<ChainElement> elements;

		private DateTime expires;

		private struct CertSimpleChain
		{
			public CapiNativeMethods.CertTrustStatus Status
			{
				get
				{
					return this.trustStatus;
				}
			}

			public DateTime Expires
			{
				get
				{
					if (!this.hasRevocationFreshnessTime)
					{
						return DateTime.UtcNow;
					}
					return DateTime.UtcNow + TimeSpan.FromSeconds(this.revocationFreshnessTime);
				}
			}

			public int ElementCount
			{
				get
				{
					return this.elements;
				}
			}

			public IntPtr RawElements
			{
				get
				{
					return this.elementData;
				}
			}

			public IList<ChainElement> Elements
			{
				get
				{
					if (this.elements <= 0)
					{
						return null;
					}
					List<ChainElement> list = new List<ChainElement>(this.elements);
					for (int i = 0; i < this.elements; i++)
					{
						IntPtr bytes = Marshal.ReadIntPtr(this.elementData, i * Marshal.SizeOf(typeof(IntPtr)));
						list.Add(ChainElement.Create(bytes));
					}
					return list.AsReadOnly();
				}
			}

			public IntPtr TrustList
			{
				get
				{
					return this.trustListInfo;
				}
			}

			private uint size;

			private CapiNativeMethods.CertTrustStatus trustStatus;

			private int elements;

			private IntPtr elementData;

			private IntPtr trustListInfo;

			[MarshalAs(UnmanagedType.Bool)]
			private bool hasRevocationFreshnessTime;

			private uint revocationFreshnessTime;
		}
	}
}
