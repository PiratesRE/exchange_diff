using System;
using Microsoft.Exchange.Net;

namespace Microsoft.Exchange.RpcClientAccess.Server
{
	internal abstract class RpcHttpConnectionRegistrationDispatchTask : DispatchTask
	{
		public RpcHttpConnectionRegistrationDispatchTask(string description, RpcHttpConnectionRegistrationDispatch rpcHttpConnectionRegistrationDispatch, CancelableAsyncCallback asyncCallback, object asyncState) : base(description, asyncCallback, asyncState)
		{
			this.rpcHttpConnectionRegistrationDispatch = rpcHttpConnectionRegistrationDispatch;
		}

		internal RpcHttpConnectionRegistrationDispatch RpcHttpConnectionRegistrationDispatch
		{
			get
			{
				return this.rpcHttpConnectionRegistrationDispatch;
			}
		}

		internal override IntPtr ContextHandle
		{
			get
			{
				return IntPtr.Zero;
			}
		}

		private readonly RpcHttpConnectionRegistrationDispatch rpcHttpConnectionRegistrationDispatch;
	}
}
