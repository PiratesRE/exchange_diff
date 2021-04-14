using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.MailboxTransport.ContentAggregation;

namespace Microsoft.Exchange.Transport.Sync.Worker.Framework.Provider.IMAP.Client
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal interface ICommClient : IDisposeTrackable, IDisposable
	{
		long TotalBytesSent { get; }

		long TotalBytesReceived { get; }

		bool IsConnected { get; }

		IAsyncResult BeginConnect(IMAPClientState clientState, AsyncCallback callback, object asyncState, object syncPoisonContext);

		AsyncOperationResult<IMAPResultData> EndConnect(IAsyncResult asyncResult);

		IAsyncResult BeginNegotiateTlsAsClient(IMAPClientState clientState, AsyncCallback callback, object asyncState, object syncPoisonContext);

		AsyncOperationResult<IMAPResultData> EndNegotiateTlsAsClient(IAsyncResult asyncResult);

		IAsyncResult BeginCommand(IMAPCommand command, IMAPClientState clientState, AsyncCallback callback, object asyncState, object syncPoisonContext);

		IAsyncResult BeginCommand(IMAPCommand command, bool processResponse, IMAPClientState clientState, AsyncCallback callback, object asyncState, object syncPoisonContext);

		AsyncOperationResult<IMAPResultData> EndCommand(IAsyncResult asyncResult);

		void Cancel();
	}
}
