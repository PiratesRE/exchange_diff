using System;
using System.DirectoryServices;
using Microsoft.Exchange.Rpc;
using Microsoft.Exchange.Rpc.UM;
using Microsoft.Exchange.UM.Rpc;
using Microsoft.Exchange.UM.UMCommon;

namespace Microsoft.Exchange.UM.UMCore
{
	internal sealed class UMRecipientTasksServerComponent : UMRPCComponentBase
	{
		internal static UMRecipientTasksServerComponent Instance
		{
			get
			{
				return UMRecipientTasksServerComponent.instance;
			}
		}

		internal override void RegisterServer()
		{
			ActiveDirectorySecurity sd = null;
			Util.GetServerSecurityDescriptor(ref sd);
			RpcServerBase.RegisterServer(typeof(UMRecipientTasksServerComponent.UMRecipientTasksServer), sd, 131220);
		}

		private static UMRecipientTasksServerComponent instance = new UMRecipientTasksServerComponent();

		internal sealed class UMRecipientTasksServer : UMVersionedRpcServer
		{
			protected override UMRPCComponentBase Component
			{
				get
				{
					return UMRecipientTasksServerComponent.Instance;
				}
			}

			protected override void PrepareRequest(UMVersionedRpcRequest request)
			{
			}

			public static IntPtr RpcIntfHandle = UMVersionedRpcServerBase.UMRecipientTasksRpcIntfHandle;
		}
	}
}
