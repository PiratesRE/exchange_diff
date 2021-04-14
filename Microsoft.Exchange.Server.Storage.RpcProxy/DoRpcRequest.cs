using System;
using Microsoft.Exchange.Rpc;

namespace Microsoft.Exchange.Server.Storage.RpcProxy
{
	internal struct DoRpcRequest
	{
		public uint Flags;

		public uint MaximumResponseSize;

		public ArraySegment<byte> Request;

		public ArraySegment<byte> AuxiliaryIn;

		public DoRpcCompleteCallback CompletionCallback;

		public Action<RpcException> ExceptionCallback;
	}
}
