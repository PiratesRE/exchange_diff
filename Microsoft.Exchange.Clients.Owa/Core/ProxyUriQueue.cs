using System;

namespace Microsoft.Exchange.Clients.Owa.Core
{
	public class ProxyUriQueue
	{
		public ProxyUriQueue(string uriString) : this(ProxyUri.Create(uriString))
		{
		}

		public ProxyUriQueue(ProxyUri uri)
		{
			this.head = -1;
			base..ctor();
			if (uri == null)
			{
				throw new ArgumentNullException();
			}
			this.data = new ProxyUri[1];
			this.data[0] = uri;
		}

		public ProxyUriQueue(ProxyUri[] proxyUris)
		{
			this.head = -1;
			base..ctor();
			if (proxyUris == null)
			{
				throw new ArgumentNullException("proxyUris");
			}
			this.data = proxyUris;
		}

		public ProxyUri Pop()
		{
			this.head++;
			if (this.head >= this.data.Length)
			{
				this.head = 0;
			}
			ProxyUri proxyUri = this.data[this.head];
			if (!proxyUri.IsParsed)
			{
				proxyUri.Parse();
			}
			return proxyUri;
		}

		public ProxyUri Head
		{
			get
			{
				return this.data[this.head];
			}
		}

		public int Count
		{
			get
			{
				return this.data.Length;
			}
		}

		private int head;

		private ProxyUri[] data;
	}
}
