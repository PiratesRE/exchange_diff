using System;
using System.Net.Sockets;

namespace Microsoft.Exchange.EseRepl
{
	internal class SocketStreamAsyncArgs : SocketAsyncEventArgs, IPoolableObject
	{
		public bool Preallocated { get; private set; }

		public SimpleBuffer InternalBuffer { get; set; }

		public EventHandler<SocketAsyncEventArgs> CompletionRtn { get; set; }

		public SocketStreamAsyncArgs(bool preallocated = false)
		{
			this.Preallocated = preallocated;
		}
	}
}
