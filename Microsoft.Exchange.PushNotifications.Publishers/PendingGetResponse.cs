using System;
using System.Web;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.PushNotifications.Publishers
{
	internal class PendingGetResponse : IPendingGetResponse
	{
		internal PendingGetResponse(HttpResponse response)
		{
			ArgumentValidator.ThrowIfNull("response", response);
			this.response = response;
		}

		public void Write(string payload)
		{
			ArgumentValidator.ThrowIfNullOrEmpty("payload", payload);
			this.response.Write(payload);
		}

		private HttpResponse response;
	}
}
