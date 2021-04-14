using System;
using Microsoft.Exchange.Connections.Common;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Connections.Imap
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal interface INetworkFacade : IDisposeTrackable, IDisposable
	{
		long TotalBytesSent { get; }

		long TotalBytesReceived { get; }

		bool IsConnected { get; }

		string Server { get; }

		IAsyncResult BeginConnect(ImapConnectionContext connectionContext, AsyncCallback callback, object callbackState);

		AsyncOperationResult<ImapResultData> EndConnect(IAsyncResult asyncResult);

		IAsyncResult BeginNegotiateTlsAsClient(ImapConnectionContext connectionContext, AsyncCallback callback, object callbackState);

		AsyncOperationResult<ImapResultData> EndNegotiateTlsAsClient(IAsyncResult asyncResult);

		IAsyncResult BeginCommand(ImapCommand command, ImapConnectionContext connectionContext, AsyncCallback callback, object callbackState);

		IAsyncResult BeginCommand(ImapCommand command, bool processResponse, ImapConnectionContext connectionContext, AsyncCallback callback, object callbackState);

		AsyncOperationResult<ImapResultData> EndCommand(IAsyncResult asyncResult);

		void Cancel();
	}
}
