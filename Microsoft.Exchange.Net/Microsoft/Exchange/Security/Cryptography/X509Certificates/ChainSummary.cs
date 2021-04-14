using System;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using Microsoft.Exchange.Net;

namespace Microsoft.Exchange.Security.Cryptography.X509Certificates
{
	internal class ChainSummary
	{
		internal ChainSummary(CapiNativeMethods.CertChainPolicyStatus results)
		{
			this.data = results;
		}

		public ChainValidityStatus Status
		{
			get
			{
				return this.data.Status;
			}
		}

		internal static ChainSummary Validate(SafeChainContextHandle chainContext, ChainPolicyParameters options)
		{
			CapiNativeMethods.CertChainPolicyParameters certChainPolicyParameters = new CapiNativeMethods.CertChainPolicyParameters(options.Flags);
			IntPtr pszPolicyOID = IntPtr.Zero;
			SSLChainPolicyParameters sslchainPolicyParameters = options as SSLChainPolicyParameters;
			if (sslchainPolicyParameters != null)
			{
				pszPolicyOID = (IntPtr)4L;
				ChainSummary.SSLExtraChainPolicyParameter sslextraChainPolicyParameter = new ChainSummary.SSLExtraChainPolicyParameter(sslchainPolicyParameters.Type, sslchainPolicyParameters.Options, sslchainPolicyParameters.ServerName);
				CapiNativeMethods.CertChainPolicyStatus results = CapiNativeMethods.CertChainPolicyStatus.Create();
				IntPtr intPtr = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(ChainSummary.SSLExtraChainPolicyParameter)));
				Marshal.StructureToPtr(sslextraChainPolicyParameter, intPtr, false);
				certChainPolicyParameters.ExtraPolicy = intPtr;
				try
				{
					if (!CapiNativeMethods.CertVerifyCertificateChainPolicy(pszPolicyOID, chainContext, ref certChainPolicyParameters, ref results))
					{
						throw new CryptographicException(Marshal.GetLastWin32Error());
					}
				}
				finally
				{
					Marshal.FreeHGlobal(intPtr);
				}
				return new ChainSummary(results);
			}
			BaseChainPolicyParameters baseChainPolicyParameters = options as BaseChainPolicyParameters;
			if (baseChainPolicyParameters == null)
			{
				throw new ArgumentException(NetException.OnlySSLSupported);
			}
			pszPolicyOID = (IntPtr)1L;
			CapiNativeMethods.CertChainPolicyStatus results2 = CapiNativeMethods.CertChainPolicyStatus.Create();
			if (!CapiNativeMethods.CertVerifyCertificateChainPolicy(pszPolicyOID, chainContext, ref certChainPolicyParameters, ref results2))
			{
				throw new CryptographicException(Marshal.GetLastWin32Error());
			}
			return new ChainSummary(results2);
		}

		private CapiNativeMethods.CertChainPolicyStatus data = default(CapiNativeMethods.CertChainPolicyStatus);

		private struct SSLExtraChainPolicyParameter
		{
			internal SSLExtraChainPolicyParameter(SSLPolicyAuthorizationType type, SSLPolicyAuthorizationOptions checks, string name)
			{
				this.size = (uint)Marshal.SizeOf(typeof(ChainSummary.SSLExtraChainPolicyParameter));
				this.type = type;
				this.checks = checks;
				this.serverName = name;
			}

			private uint size;

			private SSLPolicyAuthorizationType type;

			private SSLPolicyAuthorizationOptions checks;

			[MarshalAs(UnmanagedType.LPWStr)]
			private string serverName;
		}
	}
}
