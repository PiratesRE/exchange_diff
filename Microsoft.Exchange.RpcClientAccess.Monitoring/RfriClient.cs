using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Net;
using Microsoft.Exchange.Nspi.Rfri;
using Microsoft.Exchange.Rpc;
using Microsoft.Exchange.Rpc.RfriClient;

namespace Microsoft.Exchange.RpcClientAccess.Monitoring
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class RfriClient : BaseRpcClient<RfriAsyncRpcClient>, IRfriClient, IRpcClient, IDisposable
	{
		public RfriClient(RpcBindingInfo bindingInfo) : base(new RfriAsyncRpcClient(bindingInfo))
		{
		}

		public IAsyncResult BeginGetNewDsa(string userLegacyDN, TimeSpan timeout, AsyncCallback asyncCallback, object asyncState)
		{
			return new RfriClient.GetNewDsaRpcCallContext(base.RpcClient, userLegacyDN, timeout, asyncCallback, asyncState).Begin();
		}

		public RfriCallResult EndGetNewDsa(IAsyncResult asyncResult, out string server)
		{
			return ((RfriClient.GetNewDsaRpcCallContext)asyncResult).End(asyncResult, out server);
		}

		public IAsyncResult BeginGetFqdnFromLegacyDn(string serverDn, TimeSpan timeout, AsyncCallback asyncCallback, object asyncState)
		{
			return new RfriClient.GetFqdnRpcCallContext(base.RpcClient, serverDn, timeout, asyncCallback, asyncState).Begin();
		}

		public RfriCallResult EndGetFqdnFromLegacyDn(IAsyncResult asyncResult, out string serverFqdn)
		{
			return ((RfriClient.GetFqdnRpcCallContext)asyncResult).End(asyncResult, out serverFqdn);
		}

		protected sealed override DisposeTracker GetDisposeTracker()
		{
			return DisposeTracker.Get<RfriClient>(this);
		}

		private class GetNewDsaRpcCallContext : RpcCallContext<RfriCallResult>
		{
			public GetNewDsaRpcCallContext(RfriAsyncRpcClient rpcClient, string userDn, TimeSpan timeout, AsyncCallback asyncCallback, object asyncState) : base(timeout, asyncCallback, asyncState)
			{
				Util.ThrowOnNullArgument(rpcClient, "rpcClient");
				Util.ThrowOnNullOrEmptyArgument(userDn, "userDn");
				this.rpcClient = rpcClient;
				this.userDn = userDn;
			}

			public RfriCallResult End(IAsyncResult asyncResult, out string serverDn)
			{
				RfriCallResult result = base.GetResult();
				serverDn = this.serverDn;
				return result;
			}

			protected override ICancelableAsyncResult OnBegin(CancelableAsyncCallback asyncCallback, object asyncState)
			{
				ICancelableAsyncResult result = this.rpcClient.BeginGetNewDSA(null, null, RfriGetNewDSAFlags.None, this.userDn, asyncCallback, asyncState);
				base.StartRpcTimer();
				return result;
			}

			protected override RfriCallResult OnEnd(ICancelableAsyncResult asyncResult)
			{
				RfriCallResult result;
				try
				{
					RfriStatus rfriStatus = this.rpcClient.EndGetNewDSA(asyncResult, out this.serverDn);
					result = new RfriCallResult(rfriStatus);
				}
				finally
				{
					base.StopAndCleanupRpcTimer();
				}
				return result;
			}

			protected override RfriCallResult OnRpcException(RpcException rpcException)
			{
				return new RfriCallResult(rpcException);
			}

			private readonly RfriAsyncRpcClient rpcClient;

			private readonly string userDn;

			private string serverDn;
		}

		private class GetFqdnRpcCallContext : RpcCallContext<RfriCallResult>
		{
			public GetFqdnRpcCallContext(RfriAsyncRpcClient rpcClient, string serverDn, TimeSpan timeout, AsyncCallback asyncCallback, object asyncState) : base(timeout, asyncCallback, asyncState)
			{
				Util.ThrowOnNullArgument(rpcClient, "rpcClient");
				Util.ThrowOnNullOrEmptyArgument(serverDn, "serverDn");
				this.rpcClient = rpcClient;
				this.serverDn = serverDn;
			}

			public RfriCallResult End(IAsyncResult asyncResult, out string serverFqdn)
			{
				RfriCallResult result = base.GetResult();
				serverFqdn = this.serverFqdn;
				return result;
			}

			protected override ICancelableAsyncResult OnBegin(CancelableAsyncCallback asyncCallback, object asyncState)
			{
				ICancelableAsyncResult result = this.rpcClient.BeginGetFQDNFromLegacyDN(null, null, RfriGetFQDNFromLegacyDNFlags.None, this.serverDn, asyncCallback, asyncState);
				base.StartRpcTimer();
				return result;
			}

			protected override RfriCallResult OnEnd(ICancelableAsyncResult asyncResult)
			{
				RfriCallResult result;
				try
				{
					RfriStatus rfriStatus = this.rpcClient.EndGetFQDNFromLegacyDN(asyncResult, out this.serverFqdn);
					result = new RfriCallResult(rfriStatus);
				}
				finally
				{
					base.StopAndCleanupRpcTimer();
				}
				return result;
			}

			protected override RfriCallResult OnRpcException(RpcException rpcException)
			{
				return new RfriCallResult(rpcException);
			}

			private readonly RfriAsyncRpcClient rpcClient;

			private readonly string serverDn;

			private string serverFqdn;
		}
	}
}
