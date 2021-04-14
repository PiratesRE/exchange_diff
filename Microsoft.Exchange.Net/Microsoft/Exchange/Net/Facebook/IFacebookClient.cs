using System;
using System.IO;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Net.Facebook
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal interface IFacebookClient : IDisposable
	{
		IAsyncResult BeginGetFriends(string accessToken, string fields, string limit, string offset, AsyncCallback callback, object state);

		FacebookUsersList EndGetFriends(IAsyncResult ar);

		IAsyncResult BeginGetUsers(string accessToken, string userIds, string fields, AsyncCallback callback, object state);

		FacebookUsersList EndGetUsers(IAsyncResult ar);

		FacebookUser GetProfile(string accessToken, string fields);

		FacebookImportContactsResult UploadContacts(string accessToken, bool continuous, bool async, string source, string contactsFormat, string contactsStreamContentType, Stream contacts);

		void RemoveApplication(string accessToken);

		void Cancel();

		void SubscribeDownloadCompletedEvent(EventHandler<DownloadCompleteEventArgs> eventHandler);
	}
}
