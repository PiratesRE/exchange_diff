using System;

namespace Microsoft.Exchange.Rpc.JobQueue
{
	internal abstract class JobQueueRpcServer : RpcServerBase
	{
		public abstract byte[] EnqueueRequest(int version, int type, byte[] inputParameterBytes);

		public JobQueueRpcServer()
		{
		}

		public static IntPtr RpcIntfHandle = (IntPtr)<Module>.IJobQueue_v1_0_s_ifspec;
	}
}
