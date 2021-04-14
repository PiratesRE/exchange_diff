using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Rpc.Cluster
{
	internal class UncaughtClrOverRpcBarrierException : RpcException
	{
		public UncaughtClrOverRpcBarrierException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public UncaughtClrOverRpcBarrierException(string message, int hr) : base(message, hr)
		{
		}

		public UncaughtClrOverRpcBarrierException(string message) : base(message)
		{
		}

		public static void ThrowIfNecessary(int status, string routineName)
		{
			if (-532459699 == status)
			{
				throw new UncaughtClrOverRpcBarrierException(string.Format("Uncaught CLR exception on other side of '{0}'.", routineName));
			}
		}

		private static uint ClrExceptionCode = 3762507597U;
	}
}
