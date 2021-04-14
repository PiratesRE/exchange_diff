using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Net;

namespace Microsoft.Exchange.PushNotifications.Publishers
{
	internal abstract class HttpClientBase : IDisposable
	{
		public HttpClientBase(HttpClient httpClient)
		{
			ArgumentValidator.ThrowIfNull("httpClient", httpClient);
			this.HttpClient = httpClient;
		}

		private protected HttpClient HttpClient { protected get; private set; }

		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (!this.isDisposed)
			{
				if (disposing && this.HttpClient != null)
				{
					this.HttpClient.Dispose();
					this.HttpClient = null;
				}
				this.isDisposed = true;
			}
		}

		private bool isDisposed;
	}
}
