using System;
using System.Runtime.InteropServices;

namespace Microsoft.Exchange.Rpc.LogSearch
{
	internal abstract class LogSearchRpcServer : RpcServerBase
	{
		public abstract byte[] Search(string logName, byte[] queryData, [MarshalAs(UnmanagedType.U1)] bool continueInBackground, int resultLimit, ref int resultSize, ref Guid sessionId, ref bool more, ref int progress, string clientName);

		public abstract byte[] SearchExtensibleSchema(string clientVersion, string logName, byte[] queryData, [MarshalAs(UnmanagedType.U1)] bool continueInBackground, int resultLimit, ref int resultSize, ref Guid sessionId, ref bool more, ref int progress, string clientName);

		public abstract byte[] Continue(Guid sessionId, [MarshalAs(UnmanagedType.U1)] bool continueInBackground, int resultLimit, ref int resultSize, ref bool more, ref int progress);

		public abstract void Cancel(Guid sessionId);

		public LogSearchRpcServer()
		{
		}

		public static IntPtr RpcIntfHandle = (IntPtr)<Module>.ILogSearch_v1_0_s_ifspec;
	}
}
