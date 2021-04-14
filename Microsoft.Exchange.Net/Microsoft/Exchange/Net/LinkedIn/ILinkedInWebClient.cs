using System;
using System.Net;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Net.LinkedIn
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal interface ILinkedInWebClient
	{
		void SubscribeDownloadCompletedEvent(EventHandler<DownloadCompleteEventArgs> eventHandler);

		void Abort(IAsyncResult ar);

		LinkedInResponse AuthenticateApplication(string url, string requestAuthorizationHeader, TimeSpan requestTimeout, IWebProxy proxy);

		LinkedInPerson GetProfile(string accessToken, string accessTokenSecret, string fields);

		IAsyncResult BeginGetLinkedInConnections(string url, string authorizationHeader, TimeSpan requestTimeout, IWebProxy proxy, AsyncCallback callback, object state);

		LinkedInConnections EndGetLinkedInConnections(IAsyncResult ar);

		HttpStatusCode RemoveApplicationPermissions(string accessToken, string accessSecret);
	}
}
