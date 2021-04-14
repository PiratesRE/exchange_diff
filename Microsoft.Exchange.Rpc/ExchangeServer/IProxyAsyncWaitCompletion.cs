using System;
using System.Runtime.InteropServices;

namespace Microsoft.Exchange.Rpc.ExchangeServer
{
	public interface IProxyAsyncWaitCompletion
	{
		void CompleteAsyncCall([MarshalAs(UnmanagedType.U1)] bool notificationsPending, int errorCode);
	}
}
