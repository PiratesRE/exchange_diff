using System;
using System.Collections.Concurrent;
using System.Net;
using System.ServiceModel;
using Microsoft.Exchange.Security.Cryptography.X509Certificates;

namespace Microsoft.Exchange.Data.Directory.GlobalLocatorService
{
	internal class ServiceProxyPool<T> : IServiceProxyPool<T>
	{
		internal ServiceProxyPool(WSHttpBinding binding, ServiceEndpoint serviceEndpoint)
		{
			this.pool = new ConcurrentQueue<T>();
			this.channelFactory = new ChannelFactory<T>(binding, serviceEndpoint.Uri.ToString());
			try
			{
				this.channelFactory.Credentials.ClientCertificate.Certificate = TlsCertificateInfo.FindFirstCertWithSubjectDistinguishedName(serviceEndpoint.CertificateSubject);
			}
			catch (ArgumentException ex)
			{
				throw new GlsPermanentException(DirectoryStrings.PermanentGlsError(ex.Message));
			}
			ServicePointManager.DefaultConnectionLimit = Math.Max(ServicePointManager.DefaultConnectionLimit, 8 * Environment.ProcessorCount);
		}

		public T Acquire()
		{
			T t;
			if (!this.pool.TryDequeue(out t) || t == null)
			{
				t = this.CreateServiceProxy();
			}
			return t;
		}

		public void Release(T serviceProxy)
		{
			this.pool.Enqueue(serviceProxy);
		}

		private T CreateServiceProxy()
		{
			return this.channelFactory.CreateChannel();
		}

		private ConcurrentQueue<T> pool;

		private ChannelFactory<T> channelFactory;
	}
}
