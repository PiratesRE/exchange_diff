using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Rpc;

namespace Microsoft.Exchange.RpcClientAccess.Monitoring
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal abstract class BaseRpcClient<TClient> : BaseObject, IRpcClient, IDisposable where TClient : RpcClientBase
	{
		protected BaseRpcClient(TClient rpcClient)
		{
			this.RpcClient = rpcClient;
		}

		public string BindingString
		{
			get
			{
				TClient rpcClient = this.RpcClient;
				return rpcClient.GetBindingString();
			}
		}

		private protected TClient RpcClient { protected get; private set; }

		protected override void InternalDispose()
		{
			Util.DisposeIfPresent(this.RpcClient);
			base.InternalDispose();
		}
	}
}
