using System;
using System.Security.Cryptography.X509Certificates;
using Microsoft.Exchange.Win32;

namespace Microsoft.Exchange.Security.Cryptography.X509Certificates
{
	internal class ChainEngine : IDisposable
	{
		internal ChainEngine(IEnginePool pool, SafeChainEngineHandle handle)
		{
			this.parent = pool;
			this.engine = handle;
		}

		public ChainEngine()
		{
			this.engine = SafeChainEngineHandle.DefaultEngine;
		}

		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		public ChainContext Build(X509Certificate2 certificate, ChainBuildOptions options, ChainBuildParameter parameter)
		{
			SafeHGlobalHandle invalidHandle = SafeHGlobalHandle.InvalidHandle;
			CapiNativeMethods.CertUsageMatch certUsageMatch = parameter.Match.GetCertUsageMatch(ref invalidHandle);
			SafeChainContextHandle handle;
			using (invalidHandle)
			{
				CapiNativeMethods.CertChainParameter certChainParameter = new CapiNativeMethods.CertChainParameter(certUsageMatch, parameter.UrlRetrievalTimeout, parameter.OverrideRevocationTime, parameter.RevocationFreshnessDelta);
				SafeCertStoreHandle hAdditionalStore = new SafeCertStoreHandle();
				if (!CapiNativeMethods.CertGetCertificateChain(this.engine, certificate.Handle, IntPtr.Zero, hAdditionalStore, ref certChainParameter, options, IntPtr.Zero, out handle))
				{
					return null;
				}
			}
			return new ChainContext(handle);
		}

		public ChainContext Build(X509Certificate2 certificate, ChainBuildOptions options, ChainBuildParameter parameter, X509Store additionalStore)
		{
			SafeHGlobalHandle invalidHandle = SafeHGlobalHandle.InvalidHandle;
			CapiNativeMethods.CertUsageMatch certUsageMatch = parameter.Match.GetCertUsageMatch(ref invalidHandle);
			SafeChainContextHandle handle;
			bool flag;
			using (invalidHandle)
			{
				CapiNativeMethods.CertChainParameter certChainParameter = new CapiNativeMethods.CertChainParameter(certUsageMatch, parameter.UrlRetrievalTimeout, parameter.OverrideRevocationTime, parameter.RevocationFreshnessDelta);
				using (SafeCertStoreHandle safeCertStoreHandle = SafeCertStoreHandle.Clone(additionalStore))
				{
					flag = CapiNativeMethods.CertGetCertificateChain(this.engine, certificate.Handle, IntPtr.Zero, safeCertStoreHandle, ref certChainParameter, options, IntPtr.Zero, out handle);
				}
			}
			if (!flag)
			{
				return null;
			}
			return new ChainContext(handle);
		}

		public ChainContext BuildAsAnonymous(X509Certificate2 certificate, ChainBuildOptions options, ChainBuildParameter parameter)
		{
			SafeHGlobalHandle invalidHandle = SafeHGlobalHandle.InvalidHandle;
			CapiNativeMethods.CertUsageMatch certUsageMatch = parameter.Match.GetCertUsageMatch(ref invalidHandle);
			SafeChainContextHandle handle;
			using (invalidHandle)
			{
				CapiNativeMethods.CertChainParameter certChainParameter = new CapiNativeMethods.CertChainParameter(certUsageMatch, parameter.UrlRetrievalTimeout, parameter.OverrideRevocationTime, parameter.RevocationFreshnessDelta);
				using (SafeCertStoreHandle storeHandleFromCertificate = CapiNativeMethods.GetStoreHandleFromCertificate(certificate))
				{
					if (!CapiNativeMethods.CertGetCertificateChain(this.engine, certificate.Handle, IntPtr.Zero, storeHandleFromCertificate, ref certChainParameter, options, IntPtr.Zero, out handle))
					{
						return null;
					}
				}
			}
			return new ChainContext(handle);
		}

		private void Dispose(bool disposing)
		{
			if (disposing)
			{
				if (this.parent != null)
				{
					this.parent.ReturnTo(this.engine);
					this.parent = null;
					return;
				}
				this.engine.Close();
			}
		}

		public const ChainBuildOptions DefaultOptions = ChainBuildOptions.CacheEndCert | ChainBuildOptions.RevocationCheckChainExcludeRoot | ChainBuildOptions.RevocationAccumulativeTimeout;

		private SafeChainEngineHandle engine;

		private IEnginePool parent;
	}
}
