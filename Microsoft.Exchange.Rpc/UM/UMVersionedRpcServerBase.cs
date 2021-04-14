using System;

namespace Microsoft.Exchange.Rpc.UM
{
	internal abstract class UMVersionedRpcServerBase : RpcServerBase
	{
		public abstract int ExecuteRequest(byte[] request, out byte[] response);

		public UMVersionedRpcServerBase()
		{
		}

		public static IntPtr UMPlayOnPhoneRpcIntfHandle = (IntPtr)<Module>.IUMPlayOnPhone_v1_0_s_ifspec;

		public static IntPtr UMPartnerMessageRpcIntfHandle = (IntPtr)<Module>.IUMPartnerMessage_v1_0_s_ifspec;

		public static IntPtr UMRecipientTasksRpcIntfHandle = (IntPtr)<Module>.IUMRecipientTasks_v1_0_s_ifspec;

		public static IntPtr UMPromptPreviewRpcIntfHandle = (IntPtr)<Module>.IUMPromptPreview_v1_0_s_ifspec;
	}
}
