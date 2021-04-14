using System;
using System.DirectoryServices;
using Microsoft.Exchange.Rpc;
using Microsoft.Exchange.Rpc.UM;
using Microsoft.Exchange.UM.ClientAccess.Messages;
using Microsoft.Exchange.UM.Rpc;
using Microsoft.Exchange.UM.UMCommon;

namespace Microsoft.Exchange.UM.UMCore
{
	internal sealed class UMPlayOnPhoneRpcServerComponent : UMRPCComponentBase
	{
		internal static UMPlayOnPhoneRpcServerComponent Instance
		{
			get
			{
				return UMPlayOnPhoneRpcServerComponent.instance;
			}
		}

		internal override void RegisterServer()
		{
			ActiveDirectorySecurity sd = null;
			Util.GetServerSecurityDescriptor(ref sd);
			uint accessMask = 131220U;
			RpcServerBase.RegisterServer(typeof(UMPlayOnPhoneRpcServerComponent.UMPlayOnPhoneRpcServer), sd, accessMask);
		}

		private static UMPlayOnPhoneRpcServerComponent instance = new UMPlayOnPhoneRpcServerComponent();

		internal sealed class UMPlayOnPhoneRpcServer : UMVersionedRpcServer
		{
			protected override UMRPCComponentBase Component
			{
				get
				{
					return UMPlayOnPhoneRpcServerComponent.Instance;
				}
			}

			protected override bool ResponseIsMandatory
			{
				get
				{
					return true;
				}
			}

			protected override void PrepareRequest(UMVersionedRpcRequest request)
			{
				((RequestBase)request).ProcessRequest = new ProcessRequestDelegate(RequestHandler.ProcessRequest);
			}

			public static IntPtr RpcIntfHandle = UMVersionedRpcServerBase.UMPlayOnPhoneRpcIntfHandle;
		}
	}
}
