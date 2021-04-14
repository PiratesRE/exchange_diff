using System;
using System.Runtime.InteropServices;

namespace Microsoft.Exchange.Rpc.Throttling
{
	internal abstract class ThrottlingRpcServer : RpcServerBase
	{
		[return: MarshalAs(UnmanagedType.U1)]
		public abstract bool ObtainSubmissionTokens(Guid mailboxGuid, int requestedTokenCount, int totalTokenCount, int submissionType);

		public abstract byte[] ObtainTokens(byte[] inBlob);

		public ThrottlingRpcServer()
		{
		}

		public static IntPtr RpcIntfHandle = (IntPtr)<Module>.IThrottling_v1_0_s_ifspec;
	}
}
