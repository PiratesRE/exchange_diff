using System;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;

namespace Microsoft.Exchange.Security.Cryptography.X509Certificates
{
	internal class ChainElement
	{
		private ChainElement(ChainElement.CertChainElement item)
		{
			this.trust = item.TrustStatus;
			this.extendedErrorInfo = item.ExtendedErrorInfo;
			this.certificate = item.Certificate;
			this.revocationInfo = item.RevocationInformation;
		}

		public X509Certificate2 Certificate
		{
			get
			{
				return this.certificate;
			}
		}

		public TrustStatus Status
		{
			get
			{
				return this.trust.error;
			}
		}

		public string ExtendedError
		{
			get
			{
				return this.extendedErrorInfo;
			}
		}

		public TrustInformation TrustInformation
		{
			get
			{
				return this.trust.information;
			}
		}

		public RevocationInformation RevocationInfo
		{
			get
			{
				return this.revocationInfo;
			}
		}

		internal static ChainElement Create(IntPtr bytes)
		{
			return new ChainElement((ChainElement.CertChainElement)Marshal.PtrToStructure(bytes, typeof(ChainElement.CertChainElement)));
		}

		internal static bool IsSelfSigned(IntPtr bytes)
		{
			return (((ChainElement.CertChainElement)Marshal.PtrToStructure(bytes, typeof(ChainElement.CertChainElement))).TrustStatus.information & TrustInformation.IsSelfSigned) != TrustInformation.None;
		}

		private CapiNativeMethods.CertTrustStatus trust;

		private X509Certificate2 certificate;

		private string extendedErrorInfo;

		private RevocationInformation revocationInfo;

		private struct CertChainElement
		{
			public X509Certificate2 Certificate
			{
				get
				{
					return new X509Certificate2(this.certContext);
				}
			}

			public RevocationInformation RevocationInformation
			{
				get
				{
					if (!(this.revocationInfo != IntPtr.Zero))
					{
						return null;
					}
					return RevocationInformation.Create(this.revocationInfo);
				}
			}

			public string ExtendedErrorInfo
			{
				get
				{
					return this.extendedErrorInfo;
				}
			}

			public CapiNativeMethods.CertTrustStatus TrustStatus
			{
				get
				{
					return this.trustStatus;
				}
			}

			private uint size;

			private IntPtr certContext;

			private CapiNativeMethods.CertTrustStatus trustStatus;

			private IntPtr revocationInfo;

			private IntPtr issuanceUsage;

			private IntPtr applicationUsage;

			[MarshalAs(UnmanagedType.LPWStr)]
			private string extendedErrorInfo;
		}
	}
}
