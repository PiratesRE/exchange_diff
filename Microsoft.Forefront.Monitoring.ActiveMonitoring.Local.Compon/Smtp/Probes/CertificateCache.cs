using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Transport;
using Microsoft.Exchange.Security.Cryptography.X509Certificates;

namespace Microsoft.Forefront.Monitoring.ActiveMonitoring.Smtp.Probes
{
	internal class CertificateCache
	{
		public CertificateCache(ChainEnginePool pool)
		{
			if (pool == null)
			{
				throw new ArgumentNullException("pool");
			}
			this.pool = pool;
		}

		public void Open(OpenFlags flags)
		{
			this.rootStore.Open(flags);
			this.certStore.Open(flags);
		}

		public void Close()
		{
			if (this.certStore != null)
			{
				this.certStore.Close();
				this.certStore = null;
				this.certStoreBookmark = -1;
			}
			if (this.rootStore != null)
			{
				this.rootStore.Close();
				this.rootStoreBookmark = -1;
			}
		}

		public void Reset()
		{
			this.cache.Clear();
			this.certStoreBookmark--;
			this.rootStoreBookmark--;
		}

		public X509Certificate2 Find(IEnumerable<string> names, bool wildcardAllowed)
		{
			return this.Find(names, wildcardAllowed, WildcardMatchType.MultiLevel);
		}

		public X509Certificate2 Find(IEnumerable<string> names, bool wildcardAllowed, WildcardMatchType wildcardMatchType)
		{
			if (this.certStore == null)
			{
				return null;
			}
			StringBuilder stringBuilder = new StringBuilder(256);
			int num = 0;
			foreach (string value in names)
			{
				if (num++ != 0)
				{
					stringBuilder.Append(',');
				}
				stringBuilder.Append(value);
			}
			string key = stringBuilder.ToString();
			X509Certificate2 x509Certificate = null;
			lock (this)
			{
				this.CheckStores();
				if (!this.cache.TryGetValue(key, out x509Certificate))
				{
					CertificateSelectionOption certificateSelectionOption = wildcardAllowed ? CertificateSelectionOption.WildcardAllowed : CertificateSelectionOption.None;
					certificateSelectionOption |= CertificateSelectionOption.PreferedNonSelfSigned;
					try
					{
						using (ChainEngine engine = this.pool.GetEngine())
						{
							x509Certificate = TlsCertificateInfo.FindCertificate(this.certStore.BaseStore, names, certificateSelectionOption, wildcardMatchType, engine);
						}
						this.cache.Add(key, x509Certificate);
					}
					catch (ArgumentException)
					{
						x509Certificate = null;
					}
				}
			}
			if (x509Certificate != null)
			{
				ExTraceGlobals.CertificateTracer.Information<string, EnumerableTracer<string>>((long)names.GetHashCode(), "Certificate search found [{0}] which has one of the following FQDN's : {1} ", x509Certificate.Thumbprint, new EnumerableTracer<string>(names));
			}
			else
			{
				ExTraceGlobals.CertificateTracer.Information<EnumerableTracer<string>>((long)names.GetHashCode(), "No certificate search found which has one of the following FQDN's : {0} ", new EnumerableTracer<string>(names));
			}
			return x509Certificate;
		}

		public X509Certificate2 Find(string thumbPrint)
		{
			if (this.certStore == null)
			{
				return null;
			}
			X509Certificate2 x509Certificate = null;
			lock (this)
			{
				this.CheckStores();
				if (!this.thumbprintBasedCache.TryGetValue(thumbPrint, out x509Certificate))
				{
					x509Certificate = TlsCertificateInfo.FindCertificate(this.certStore.BaseStore, X509FindType.FindByThumbprint, thumbPrint);
					this.thumbprintBasedCache.Add(thumbPrint, x509Certificate);
				}
			}
			if (x509Certificate != null)
			{
				ExTraceGlobals.CertificateTracer.Information<string>(0L, "A certificate with thumbprint [{0}] has been found.", thumbPrint);
			}
			else
			{
				ExTraceGlobals.CertificateTracer.TraceError<string>(0L, "A certificate with thumbprint [{0}] has not been found.", thumbPrint);
			}
			return x509Certificate;
		}

		private void Clear()
		{
			this.cache.Clear();
			this.thumbprintBasedCache.Clear();
			this.certStoreBookmark = -1;
		}

		private void CheckStores()
		{
			int bookmark = this.certStore.Bookmark;
			int bookmark2 = this.rootStore.Bookmark;
			if (bookmark != this.certStoreBookmark || bookmark2 != this.rootStoreBookmark)
			{
				ExTraceGlobals.CertificateTracer.Information(0L, "Certificate store My | Root has changed - flushing certificate cache");
				this.Clear();
				this.certStoreBookmark = bookmark;
				this.rootStoreBookmark = bookmark2;
			}
		}

		private CertificateStore certStore = new CertificateStore(StoreName.My, StoreLocation.LocalMachine);

		private int certStoreBookmark = -1;

		private CertificateStore rootStore = new CertificateStore(StoreName.Root, StoreLocation.LocalMachine);

		private int rootStoreBookmark = -1;

		private Dictionary<string, X509Certificate2> cache = new Dictionary<string, X509Certificate2>(StringComparer.OrdinalIgnoreCase);

		private Dictionary<string, X509Certificate2> thumbprintBasedCache = new Dictionary<string, X509Certificate2>(StringComparer.OrdinalIgnoreCase);

		private ChainEnginePool pool;
	}
}
