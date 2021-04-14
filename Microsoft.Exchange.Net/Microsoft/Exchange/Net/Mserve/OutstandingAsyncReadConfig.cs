using System;
using System.IO;

namespace Microsoft.Exchange.Net.Mserve
{
	internal sealed class OutstandingAsyncReadConfig
	{
		public HttpClient Client
		{
			get
			{
				return this.client;
			}
		}

		public HttpSessionConfig SessionConfig
		{
			get
			{
				return this.sessionConfig;
			}
		}

		public StreamWriter XmlStreamWriter
		{
			get
			{
				return this.xmlStreamWriter;
			}
		}

		public CancelableAsyncCallback ClientCallback
		{
			get
			{
				return this.clientCallback;
			}
		}

		public object ClientState
		{
			get
			{
				return this.clientState;
			}
		}

		public int CachePartnerId
		{
			get
			{
				return this.cachePartnerId;
			}
		}

		public OutstandingAsyncReadConfig(HttpClient client, HttpSessionConfig sessionConfig, StreamWriter xmlStreamWriter, CancelableAsyncCallback clientCallback, int cachePartnerId, object clientState)
		{
			if (client == null)
			{
				throw new ArgumentNullException("client");
			}
			if (sessionConfig == null)
			{
				throw new ArgumentNullException("sessionConfig");
			}
			if (xmlStreamWriter == null)
			{
				throw new ArgumentNullException("xmlStreamWriter");
			}
			this.client = client;
			this.sessionConfig = sessionConfig;
			this.xmlStreamWriter = xmlStreamWriter;
			this.clientCallback = clientCallback;
			this.cachePartnerId = cachePartnerId;
			this.clientState = clientState;
		}

		private readonly HttpClient client;

		private readonly HttpSessionConfig sessionConfig;

		private readonly StreamWriter xmlStreamWriter;

		private readonly CancelableAsyncCallback clientCallback;

		private readonly object clientState;

		private readonly int cachePartnerId;
	}
}
