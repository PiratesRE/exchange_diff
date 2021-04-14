using System;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Net
{
	internal class CancellableNetworkConnection : NetworkConnection
	{
		public CancellableNetworkConnection(Socket socket, CancellationToken token, int bufferSize = 4096) : base(socket, 4096)
		{
			ArgumentValidator.ThrowIfNull("socket", socket);
			this.token = token;
			this.ctr = token.Register(new Action(base.Shutdown));
		}

		protected override void ReadComplete(IAsyncResult iar)
		{
			this.ReadOrReadLineCompleteInternal(iar, new AsyncCallback(base.ReadComplete));
		}

		protected override void ReadLineComplete(IAsyncResult iar)
		{
			this.ReadOrReadLineCompleteInternal(iar, new AsyncCallback(base.ReadLineComplete));
		}

		protected override void WriteComplete(IAsyncResult iar)
		{
			if (this.token.IsCancellationRequested)
			{
				this.breadcrumbs.Drop(NetworkConnection.Breadcrumb.AsyncOperationCancelled);
				TaskCompletionSource<object> taskCompletionSource = (TaskCompletionSource<object>)iar.AsyncState;
				taskCompletionSource.SetCanceled();
				return;
			}
			base.WriteComplete(iar);
		}

		protected override void InternalDispose(bool disposeTrueFinalizeFalse)
		{
			if (disposeTrueFinalizeFalse)
			{
				this.ctr.Dispose();
			}
			base.InternalDispose(disposeTrueFinalizeFalse);
		}

		private void ReadOrReadLineCompleteInternal(IAsyncResult iar, AsyncCallback completionMethod)
		{
			if (this.token.IsCancellationRequested)
			{
				this.breadcrumbs.Drop(NetworkConnection.Breadcrumb.AsyncOperationCancelled);
				TaskCompletionSource<NetworkConnection.LazyAsyncResultWithTimeout> taskCompletionSource = (TaskCompletionSource<NetworkConnection.LazyAsyncResultWithTimeout>)iar.AsyncState;
				taskCompletionSource.SetCanceled();
				return;
			}
			completionMethod(iar);
		}

		private CancellationToken token;

		private CancellationTokenRegistration ctr;
	}
}
