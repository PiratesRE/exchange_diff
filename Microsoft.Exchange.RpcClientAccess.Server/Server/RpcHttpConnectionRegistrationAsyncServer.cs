using System;
using System.Security.AccessControl;
using System.Security.Principal;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Rpc;
using Microsoft.Exchange.Rpc.RpcHttpConnectionRegistration;
using Microsoft.Exchange.RpcClientAccess.Messages;

namespace Microsoft.Exchange.RpcClientAccess.Server
{
	internal sealed class RpcHttpConnectionRegistrationAsyncServer : RpcHttpConnectionRegistrationAsyncRpcServer
	{
		internal static void Initialize(IRpcHttpConnectionRegistrationAsyncDispatch rpcHttpConnectionRegistrationAsyncDispatch, int maximumConcurrentCalls, ExEventLog eventLog)
		{
			Util.ThrowOnNullArgument(rpcHttpConnectionRegistrationAsyncDispatch, "rpcHttpConnectionRegistrationAsyncDispatch");
			Util.ThrowOnNullArgument(eventLog, "eventLog");
			RpcHttpConnectionRegistrationAsyncServer.rpcHttpConnectionRegistrationAsyncDispatch = rpcHttpConnectionRegistrationAsyncDispatch;
			RpcHttpConnectionRegistrationAsyncServer.eventLog = eventLog;
			RpcHttpConnectionRegistrationAsyncServer.maximumConcurrentCalls = maximumConcurrentCalls;
		}

		internal static void Start()
		{
			bool flag = false;
			if (RpcHttpConnectionRegistrationAsyncServer.server == null)
			{
				try
				{
					RpcHttpConnectionRegistrationAsyncServer.server = (RpcHttpConnectionRegistrationAsyncServer)RpcServerBase.RegisterAutoListenInterface(typeof(RpcHttpConnectionRegistrationAsyncServer), RpcHttpConnectionRegistrationAsyncServer.CreateSecurityDescriptor(), RpcHttpConnectionRegistrationAsyncServer.maximumConcurrentCalls, true, true, null, true, false, false);
					flag = true;
				}
				catch (DuplicateRpcEndpointException ex)
				{
					RpcHttpConnectionRegistrationAsyncServer.eventLog.LogEvent(RpcClientAccessServiceEventLogConstants.Tuple_DuplicateRpcEndpoint, string.Empty, new object[]
					{
						ex.Message
					});
					throw new RpcServiceAbortException("RpcHttpConnectionRegistrationAsyncServer is being aborted the service due to DuplicateRpcEndpointException", ex);
				}
				finally
				{
					if (!flag)
					{
						RpcHttpConnectionRegistrationAsyncServer.Stop();
						RpcHttpConnectionRegistrationAsyncServer.rpcHttpConnectionRegistrationAsyncDispatch = null;
					}
				}
			}
		}

		internal static void Stop()
		{
			if (RpcHttpConnectionRegistrationAsyncServer.server != null)
			{
				RpcServerBase.UnregisterInterface(RpcHttpConnectionRegistrationAsyncRpcServer.RpcIntfHandle, true);
				RpcHttpConnectionRegistrationAsyncServer.server = null;
				RpcHttpConnectionRegistration.Instance.Clear();
			}
		}

		private static FileSecurity CreateSecurityDescriptor()
		{
			FileSecurity fileSecurity = new FileSecurity();
			SecurityIdentifier securityIdentifier = new SecurityIdentifier(WellKnownSidType.BuiltinAdministratorsSid, null);
			SecurityIdentifier identity = new SecurityIdentifier(WellKnownSidType.LocalSystemSid, null);
			FileSystemAccessRule rule = new FileSystemAccessRule(securityIdentifier, FileSystemRights.ReadData, AccessControlType.Allow);
			FileSystemAccessRule rule2 = new FileSystemAccessRule(identity, FileSystemRights.ReadData, AccessControlType.Allow);
			fileSecurity.SetOwner(securityIdentifier);
			fileSecurity.AddAccessRule(rule);
			fileSecurity.AddAccessRule(rule2);
			return fileSecurity;
		}

		public override IRpcHttpConnectionRegistrationAsyncDispatch GetRpcHttpConnectionRegistrationAsyncDispatch()
		{
			return RpcHttpConnectionRegistrationAsyncServer.rpcHttpConnectionRegistrationAsyncDispatch;
		}

		public override bool IsShuttingDown()
		{
			return RpcClientAccessService.IsShuttingDown;
		}

		private static RpcHttpConnectionRegistrationAsyncServer server = null;

		private static IRpcHttpConnectionRegistrationAsyncDispatch rpcHttpConnectionRegistrationAsyncDispatch = null;

		private static int maximumConcurrentCalls;

		private static ExEventLog eventLog;
	}
}
