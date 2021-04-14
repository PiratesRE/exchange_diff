using System;
using Microsoft.Exchange.Rpc;
using Microsoft.Exchange.Rpc.RpcHttpConnectionRegistration;

namespace Microsoft.Exchange.RpcHttpModules
{
	public class RpcHttpConnectionRegistrationClient : IRpcHttpConnectionRegistration
	{
		public int Register(Guid associationGroupId, string token, string serverTarget, string sessionCookie, string clientIp, Guid requestId, out string failureMessage, out string failureDetails)
		{
			int result;
			try
			{
				using (RpcHttpConnectionRegistrationRpcClient registrationClient = RpcHttpConnectionRegistrationClient.GetRegistrationClient())
				{
					result = registrationClient.Register(associationGroupId, token, serverTarget, sessionCookie, clientIp, requestId, out failureMessage, out failureDetails);
				}
			}
			catch (RpcException ex)
			{
				throw new RpcHttpConnectionRegistrationInternalException(string.Format("RpcHttpConnectionRegistrationClient::Register RPC failed: {0}: ", ex.ErrorCode), ex);
			}
			return result;
		}

		public void Unregister(Guid associationGroupId, Guid requestId)
		{
			RpcHttpConnectionRegistrationClient.Execute("Unregister", (RpcHttpConnectionRegistrationRpcClient rpcHttpConnectionRegistration) => rpcHttpConnectionRegistration.Unregister(associationGroupId, requestId));
		}

		public void Clear()
		{
			RpcHttpConnectionRegistrationClient.Execute("Clear", (RpcHttpConnectionRegistrationRpcClient rpcHttpConnectionRegistration) => rpcHttpConnectionRegistration.Clear());
		}

		private static void Execute(string methodName, Func<RpcHttpConnectionRegistrationRpcClient, int> delegateFunc)
		{
			try
			{
				using (RpcHttpConnectionRegistrationRpcClient registrationClient = RpcHttpConnectionRegistrationClient.GetRegistrationClient())
				{
					int num = delegateFunc(registrationClient);
					if (num != 0)
					{
						throw new RpcHttpConnectionRegistrationException(string.Format("RpcHttpConnectionRegistrationClient::{0} call failed with error code = {1}", methodName, num), num);
					}
				}
			}
			catch (RpcException ex)
			{
				throw new RpcHttpConnectionRegistrationInternalException(string.Format("RpcHttpConnectionRegistrationClient::{0} RPC failed: {1}: ", methodName, ex.ErrorCode), ex);
			}
		}

		private static RpcHttpConnectionRegistrationRpcClient GetRegistrationClient()
		{
			return new RpcHttpConnectionRegistrationRpcClient();
		}
	}
}
