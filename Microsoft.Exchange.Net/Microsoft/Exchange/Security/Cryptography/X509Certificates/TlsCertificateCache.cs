using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using Microsoft.Exchange.Diagnostics.Components.Net;

namespace Microsoft.Exchange.Security.Cryptography.X509Certificates
{
	internal class TlsCertificateCache
	{
		public static ChainEnginePool ChainEnginePool
		{
			get
			{
				if (TlsCertificateCache.chainEnginePool == null)
				{
					TlsCertificateCache.ChainEnginePool = new ChainEnginePool();
				}
				return TlsCertificateCache.chainEnginePool;
			}
			set
			{
				TlsCertificateCache.chainEnginePool = value;
			}
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

		public X509Certificate2 Find(IEnumerable<string> names, CertificateSelectionOption options)
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
			string text = stringBuilder.ToString();
			X509Certificate2 x509Certificate = null;
			lock (this)
			{
				this.CheckStores();
				bool flag2 = this.cache.TryGetValue(text, out x509Certificate);
				if (flag2)
				{
					if (x509Certificate != null)
					{
						goto IL_13F;
					}
				}
				try
				{
					using (ChainEngine engine = TlsCertificateCache.ChainEnginePool.GetEngine())
					{
						x509Certificate = TlsCertificateInfo.FindCertificate(this.certStore.BaseStore, names, options, engine);
					}
					if (flag2 && x509Certificate != null)
					{
						ExTraceGlobals.CertificateTracer.TraceDebug<string>(0L, "Replacing null certificate for domains: {0}", text);
						this.cache[text] = x509Certificate;
					}
					else
					{
						this.cache.Add(text, x509Certificate);
						ExTraceGlobals.CertificateTracer.TraceDebug<string, string>(0L, "Adding newly found certificate {0} for domains: {1}", (x509Certificate == null) ? "NULL" : x509Certificate.Thumbprint, text);
					}
				}
				catch (ArgumentException ex)
				{
					x509Certificate = null;
					ExTraceGlobals.CertificateTracer.TraceDebug<string, string>(0L, "No certificate returned for domains: {0} due to exception reason: {1}", text, ex.Message);
				}
				IL_13F:;
			}
			return x509Certificate;
		}

		private void Clear()
		{
			this.cache.Clear();
			this.certStoreBookmark = -1;
		}

		private void CheckStores()
		{
			int bookmark = this.certStore.Bookmark;
			int bookmark2 = this.rootStore.Bookmark;
			if (bookmark != this.certStoreBookmark || bookmark2 != this.rootStoreBookmark)
			{
				this.Clear();
				this.certStoreBookmark = bookmark;
				this.rootStoreBookmark = bookmark2;
			}
		}

		private CertificateStore certStore = new CertificateStore(StoreName.My, StoreLocation.LocalMachine);

		private static ChainEnginePool chainEnginePool = new ChainEnginePool();

		private int certStoreBookmark = -1;

		private CertificateStore rootStore = new CertificateStore(StoreName.Root, StoreLocation.LocalMachine);

		private int rootStoreBookmark = -1;

		private Dictionary<string, X509Certificate2> cache = new Dictionary<string, X509Certificate2>(StringComparer.OrdinalIgnoreCase);
	}
}
