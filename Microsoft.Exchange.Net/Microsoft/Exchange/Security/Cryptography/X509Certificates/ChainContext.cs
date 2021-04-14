using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;
using Microsoft.Exchange.Diagnostics.Components.Net;

namespace Microsoft.Exchange.Security.Cryptography.X509Certificates
{
	internal class ChainContext : IDisposable
	{
		internal ChainContext(SafeChainContextHandle handle)
		{
			this.chainContext = handle;
			ChainContext.CertChainContext certChainContext = (ChainContext.CertChainContext)Marshal.PtrToStructure(this.chainContext.DangerousGetHandle(), typeof(ChainContext.CertChainContext));
			this.trust = certChainContext.TrustStatus;
			this.expires = (certChainContext.HasRevocationFreshnessTime ? (DateTime.UtcNow + TimeSpan.FromSeconds(certChainContext.RevocationFreshnessTime)) : DateTime.UtcNow);
			this.chainCount = certChainContext.Chains;
			this.chainData = certChainContext.ChainData;
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

		public DateTime Expires
		{
			get
			{
				return this.expires;
			}
		}

		public bool IsSelfSigned
		{
			get
			{
				if (this.chainCount != 1)
				{
					return false;
				}
				if (this.chains == null)
				{
					IntPtr bytes = Marshal.ReadIntPtr(this.chainData);
					return CertificateChain.IsSelfSigned(bytes);
				}
				if (this.chains.Count != 1 || this.chains[0].Elements.Count != 1)
				{
					return false;
				}
				TrustInformation trustInformation = this.chains[0].Elements[0].TrustInformation;
				return (trustInformation & TrustInformation.IsSelfSigned) != TrustInformation.None;
			}
		}

		public X509Certificate2 RootCertificate
		{
			get
			{
				if (this.Status == TrustStatus.IsUntrustedRoot)
				{
					return null;
				}
				IList<CertificateChain> list = this.GetChains();
				if (list.Count == 0)
				{
					ExTraceGlobals.CertificateTracer.TraceError(0L, "Marshalling error.  Chain segment count is zero.");
					throw new InvalidOperationException("Chain count is zero but we found results!");
				}
				CertificateChain certificateChain = list[list.Count - 1];
				if (certificateChain.Elements.Count == 0)
				{
					ExTraceGlobals.CertificateTracer.TraceError(0L, "Marshalling error.  Chain segment has zero elements.");
					throw new InvalidOperationException("Last chain count is zero but we found results!");
				}
				X509Certificate2 certificate = certificateChain.Elements[certificateChain.Elements.Count - 1].Certificate;
				if (certificate == null)
				{
					ExTraceGlobals.CertificateTracer.TraceError(0L, "Marshalling error.  Certificate in last element is null.");
					throw new InvalidOperationException("Root certificate was null!");
				}
				return certificate;
			}
		}

		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		public IList<CertificateChain> GetChains()
		{
			if (this.chains != null)
			{
				return this.chains;
			}
			List<CertificateChain> list = new List<CertificateChain>(this.chainCount);
			for (int i = 0; i < this.chainCount; i++)
			{
				IntPtr bytes = Marshal.ReadIntPtr(this.chainData, i * Marshal.SizeOf(typeof(IntPtr)));
				list.Add(CertificateChain.Create(bytes));
			}
			this.chains = list.AsReadOnly();
			return this.chains;
		}

		public ChainSummary Validate(ChainPolicyParameters options)
		{
			return ChainSummary.Validate(this.chainContext, options);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (disposing && this.chainContext != null)
			{
				this.chainContext.Dispose();
			}
		}

		private SafeChainContextHandle chainContext;

		private CapiNativeMethods.CertTrustStatus trust;

		private int chainCount;

		private IntPtr chainData;

		private IList<CertificateChain> chains;

		private DateTime expires;

		private struct CertChainContext
		{
			public CapiNativeMethods.CertTrustStatus TrustStatus
			{
				get
				{
					return this.trustStatus;
				}
			}

			public int Chains
			{
				get
				{
					return this.chains;
				}
			}

			public IntPtr ChainData
			{
				get
				{
					return this.chainData;
				}
			}

			public bool HasRevocationFreshnessTime
			{
				get
				{
					return this.hasRevocationFreshnessTime;
				}
			}

			public uint RevocationFreshnessTime
			{
				get
				{
					return this.revocationFreshnessTime;
				}
			}

			private int size;

			private CapiNativeMethods.CertTrustStatus trustStatus;

			private int chains;

			private IntPtr chainData;

			private int lowerChains;

			private IntPtr lowerChainData;

			[MarshalAs(UnmanagedType.Bool)]
			private bool hasRevocationFreshnessTime;

			private uint revocationFreshnessTime;
		}
	}
}
