using System;

namespace Microsoft.Exchange.Rpc.Assistants
{
	internal abstract class AssistantsRpcServerBase : RpcServerBase
	{
		public abstract void RunNow(string assistantName, ValueType mailboxGuid, ValueType mdbGuid);

		public abstract void Halt(string assistantName);

		public abstract int RunNowWithParamsHR(string assistantName, ValueType mailboxGuid, ValueType mdbGuid, string parameters);

		public abstract int RunNowHR(string assistantName, ValueType mailboxGuid, ValueType mdbGuid);

		public abstract int HaltHR(string assistantName);

		public static void StopServer()
		{
			RpcServerBase.StopServer(AssistantsRpcServerBase.RpcIntfHandle);
		}

		public AssistantsRpcServerBase()
		{
		}

		internal static IntPtr RpcIntfHandle = (IntPtr)<Module>.AssistantsRpc_v1_0_s_ifspec;
	}
}
