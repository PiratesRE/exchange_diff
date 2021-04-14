using System;
using System.Net;

namespace Microsoft.Exchange.Rpc.RpcHttpConnectionRegistration
{
	internal class RpcHttpConnectionRegistrationRpcClient : RpcClientBase, IRpcHttpConnectionRegistrationDispatch
	{
		public RpcHttpConnectionRegistrationRpcClient(string machineName, string proxyServer, string protocolSequence, NetworkCredential networkCredential) : base(machineName, proxyServer, protocolSequence, networkCredential, HttpAuthenticationScheme.Basic, AuthenticationService.Negotiate, true)
		{
		}

		public RpcHttpConnectionRegistrationRpcClient() : base("localhost", null, null, null, null, null, true, null, HttpAuthenticationScheme.Basic, AuthenticationService.Negotiate, false, false, true)
		{
		}

		public virtual int Register(Guid associationGroupId, string token, string serverTarget, string sessionCookie, string clientIp, Guid requestId, out string failureMessage, out string failureDetails)
		{
			ClientCallWrapper_Register clientCallWrapper_Register = null;
			int result;
			try
			{
				clientCallWrapper_Register = new ClientCallWrapper_Register(base.BindingHandle, associationGroupId, token, serverTarget, sessionCookie, clientIp, requestId);
				int num = clientCallWrapper_Register.Execute();
				failureMessage = clientCallWrapper_Register.FailureMessage;
				failureDetails = clientCallWrapper_Register.FailureDetails;
				result = num;
			}
			finally
			{
				if (clientCallWrapper_Register != null)
				{
					((IDisposable)clientCallWrapper_Register).Dispose();
				}
			}
			return result;
		}

		public virtual int Unregister(Guid associationGroupId, Guid requestId)
		{
			ClientCallWrapper_Unregister clientCallWrapper_Unregister = null;
			int result;
			try
			{
				clientCallWrapper_Unregister = new ClientCallWrapper_Unregister(base.BindingHandle, associationGroupId, requestId);
				result = clientCallWrapper_Unregister.Execute();
			}
			finally
			{
				if (clientCallWrapper_Unregister != null)
				{
					((IDisposable)clientCallWrapper_Unregister).Dispose();
				}
			}
			return result;
		}

		public virtual int Clear()
		{
			ClientCallWrapper_Clear clientCallWrapper_Clear = null;
			int result;
			try
			{
				clientCallWrapper_Clear = new ClientCallWrapper_Clear(base.BindingHandle);
				result = clientCallWrapper_Clear.Execute();
			}
			finally
			{
				if (clientCallWrapper_Clear != null)
				{
					((IDisposable)clientCallWrapper_Clear).Dispose();
				}
			}
			return result;
		}
	}
}
