using System;
using System.Collections.Specialized;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	internal static class RemoteRequestProcessor
	{
		internal static bool IsRemoteRequest(NameValueCollection headers)
		{
			return headers["X-OWA-RemoteUserId"] != null;
		}

		internal static string GetRemoteUserId(NameValueCollection headers)
		{
			return headers["X-OWA-RemoteUserId"];
		}

		internal static string GetRequesterUserId(NameValueCollection headers)
		{
			return headers["X-OWA-SelfId"];
		}

		internal const string RemoteUserIdHeaderName = "X-OWA-RemoteUserId";

		internal const string UserIdHeaderName = "X-OWA-SelfId";
	}
}
