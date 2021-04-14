using System;
using System.Security.Cryptography.X509Certificates;
using Microsoft.Exchange.Diagnostics.Components.Transport;
using Microsoft.Exchange.Protocols.Smtp;
using Microsoft.Exchange.Security.Cryptography.X509Certificates;

namespace Microsoft.Exchange.Transport
{
	internal class CertificateComponent : ITransportComponent
	{
		public CertificateCache Cache
		{
			get
			{
				return this.cache;
			}
		}

		public CertificateValidator Validator
		{
			get
			{
				return this.validator;
			}
		}

		public void Load()
		{
			ChainEnginePool pool = new ChainEnginePool();
			this.anonymousValidationResultCache = new CertificateValidationResultCache(Components.Configuration.ProcessTransportRole, "AnonymousCertificateValidationResultCache", Components.TransportAppConfig.SecureMail, ExTraceGlobals.AnonymousCertificateValidationResultCacheTracer);
			this.validator = new CertificateValidator(pool, this.anonymousValidationResultCache, Components.TransportAppConfig.SecureMail);
			this.cache = new CertificateCache(pool);
			this.cache.Open(OpenFlags.ReadOnly);
		}

		public void Unload()
		{
			this.cache.Close();
			this.cache = null;
			this.validator = null;
			this.anonymousValidationResultCache.Dispose();
			this.anonymousValidationResultCache = null;
		}

		public string OnUnhandledException(Exception e)
		{
			return null;
		}

		private CertificateCache cache;

		private CertificateValidator validator;

		private CertificateValidationResultCache anonymousValidationResultCache;
	}
}
