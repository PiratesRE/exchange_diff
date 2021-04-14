using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Net;

namespace Microsoft.Exchange.PushNotifications.Publishers
{
	internal class WnsResult<T> where T : class
	{
		public WnsResult(T response, Exception exception = null)
		{
			ArgumentValidator.ThrowIfNull("response", response);
			this.Response = response;
			this.Exception = exception;
		}

		public WnsResult(Exception exception)
		{
			ArgumentValidator.ThrowIfNull("exception", exception);
			this.Exception = exception;
		}

		public T Response { get; private set; }

		public Exception Exception { get; private set; }

		public bool IsTimeout
		{
			get
			{
				return this.Exception is DownloadTimeoutException;
			}
		}
	}
}
