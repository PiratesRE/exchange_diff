using System;
using System.Runtime.InteropServices;

namespace Microsoft.Exchange.Security.Cryptography.X509Certificates
{
	internal class RevocationCrlInformation
	{
		private RevocationCrlInformation(RevocationCrlInformation.CertRevocationCrlInfo item)
		{
			this.deltaEntry = item.IsDeltaCRLEntry;
			this.baseCRLContext = item.BaseCRLList;
			this.deltaCRLContext = item.DeltaCRLList;
		}

		public bool IsDelta
		{
			get
			{
				return this.deltaEntry;
			}
		}

		public CertificateRevocationList BaseCRL
		{
			get
			{
				return this.baseCRLContext;
			}
		}

		public CertificateRevocationList DeltaCRL
		{
			get
			{
				return this.deltaCRLContext;
			}
		}

		internal static RevocationCrlInformation Create(IntPtr bytes)
		{
			return new RevocationCrlInformation((RevocationCrlInformation.CertRevocationCrlInfo)Marshal.PtrToStructure(bytes, typeof(RevocationCrlInformation.CertRevocationCrlInfo)));
		}

		private bool deltaEntry;

		private CertificateRevocationList baseCRLContext;

		private CertificateRevocationList deltaCRLContext;

		private struct CertRevocationCrlInfo
		{
			public bool IsDeltaCRLEntry
			{
				get
				{
					return this.deltaCrlEntry;
				}
			}

			public CertificateRevocationList BaseCRLList
			{
				get
				{
					if (!(this.baseCRLContext != IntPtr.Zero))
					{
						return null;
					}
					return CertificateRevocationList.Create(this.baseCRLContext);
				}
			}

			public CertificateRevocationList DeltaCRLList
			{
				get
				{
					if (!(this.deltaCRLContext != IntPtr.Zero))
					{
						return null;
					}
					return CertificateRevocationList.Create(this.deltaCRLContext);
				}
			}

			private uint size;

			private IntPtr baseCRLContext;

			private IntPtr deltaCRLContext;

			private IntPtr crlEntry;

			[MarshalAs(UnmanagedType.Bool)]
			private bool deltaCrlEntry;
		}
	}
}
