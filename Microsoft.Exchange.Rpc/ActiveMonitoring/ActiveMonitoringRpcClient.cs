using System;
using System.Runtime.ExceptionServices;
using System.Runtime.InteropServices;
using Microsoft.Exchange.Diagnostics.Components.ActiveMonitoring;
using Microsoft.Exchange.Rpc.Cluster;

namespace Microsoft.Exchange.Rpc.ActiveMonitoring
{
	internal class ActiveMonitoringRpcClient : RpcClientBase
	{
		public ActiveMonitoringRpcClient(string machineName) : base(machineName)
		{
			try
			{
				base.SetTimeOut(5000);
				int num = <Module>.RpcMgmtSetCancelTimeout(0);
				if (num != null)
				{
					RpcClientBase.ThrowRpcException(num, "RpcMgmtSetCancelTimeout");
				}
			}
			catch
			{
				base.Dispose(true);
				throw;
			}
		}

		public void RequestMonitoring(Guid dbGuid)
		{
			RpcClientBase.ThrowRpcException(1764, "cli_RequestMonitoring");
		}

		public void CancelMonitoring(Guid dbGuid)
		{
			RpcClientBase.ThrowRpcException(1764, "cli_RequestMonitoring");
		}

		[HandleProcessCorruptedStateExceptions]
		public unsafe void Heartbeat(Guid dbGuid)
		{
			int num = 0;
			ushort* ptr = <Module>.StringToUnmanaged(<Module>.GetLocalComputerName((_COMPUTER_NAME_FORMAT)3));
			try
			{
				_GUID guid = <Module>.ToGUID(ref dbGuid);
				num = <Module>.cli_Heartbeat(base.BindingHandle, ptr, guid);
				if (num < 0)
				{
					RpcClientBase.ThrowRpcException(num, "cli_Heartbeat");
				}
			}
			catch when (endfilter(<Module>.DwExRpcExceptionFilter(Marshal.GetExceptionCode()) != null))
			{
				RpcClientBase.ThrowRpcException((num >= 0) ? Marshal.GetExceptionCode() : num, "cli_Heartbeat");
			}
			finally
			{
				if (ptr != null)
				{
					<Module>.MIDL_user_free((void*)ptr);
				}
			}
		}

		[HandleProcessCorruptedStateExceptions]
		public unsafe void RequestCredential(Guid dbGuid, string userPrincipalName, ref string credential)
		{
			int num = 0;
			ushort* ptr = <Module>.StringToUnmanaged(<Module>.GetLocalComputerName((_COMPUTER_NAME_FORMAT)3));
			ushort* ptr2 = <Module>.StringToUnmanaged(userPrincipalName);
			ushort* ptr3 = null;
			credential = null;
			try
			{
				try
				{
					_GUID guid = <Module>.ToGUID(ref dbGuid);
					num = <Module>.cli_RequestCredential(base.BindingHandle, ptr, guid, ptr2, &ptr3);
				}
				catch when (endfilter(<Module>.DwExRpcExceptionFilter(Marshal.GetExceptionCode()) != null))
				{
					RpcClientBase.ThrowRpcException(Marshal.GetExceptionCode(), "cli_RequestCredential");
				}
				if ((num == ActiveMonitoringRpcClient.WLIDException || num == ActiveMonitoringRpcClient.OtherManagedException) && null != ptr3)
				{
					string routineName = <Module>.UToMString(ptr3);
					RpcClientBase.ThrowRpcException(num, routineName);
				}
				else if (num < 0)
				{
					RpcClientBase.ThrowRpcException(num, "cli_RequestCredential");
				}
				if (null != ptr3)
				{
					credential = <Module>.UToMString(ptr3);
				}
			}
			finally
			{
				if (ptr != null)
				{
					<Module>.MIDL_user_free((void*)ptr);
				}
				if (ptr3 != null)
				{
					<Module>.MIDL_user_free((void*)ptr3);
				}
			}
		}

		[HandleProcessCorruptedStateExceptions]
		public unsafe RpcErrorExceptionInfo GenericRequest(RpcGenericRequestInfo requestInfo, out RpcGenericReplyInfo replyInfo)
		{
			int num = 0;
			tagErrorExceptionInfo tagErrorExceptionInfo;
			<Module>.InitErrorExceptionInfo(ref tagErrorExceptionInfo);
			tagGenericRequestInfo tagGenericRequestInfo;
			<Module>.InitGenericRequestInfo(ref tagGenericRequestInfo);
			tagGenericReplyInfo tagGenericReplyInfo;
			<Module>.InitGenericReplyInfo(ref tagGenericReplyInfo);
			try
			{
				num = <Module>.MToUGenericRequestInfo(requestInfo, &tagGenericRequestInfo);
				if (num < 0)
				{
					ReplayRpcException.ThrowRpcException(num, "GenericRequest.MToUGenericRequest");
				}
				num = <Module>.cli_GenericRequest(base.BindingHandle, &tagGenericRequestInfo, &tagGenericReplyInfo, &tagErrorExceptionInfo);
				if (num < 0)
				{
					ReplayRpcException.ThrowRpcException(num, "cli_GenericRequest");
				}
				object[] args = new object[]
				{
					tagGenericReplyInfo,
					*(ref tagGenericReplyInfo + 4),
					*(ref tagGenericReplyInfo + 8),
					*(ref tagGenericReplyInfo + 12),
					*(ref tagGenericReplyInfo + 16)
				};
				ExTraceGlobals.ActiveMonitoringRpcTracer.TraceDebug((long)this.GetHashCode(), "ReplayRpcClient::cli_GenericRequest() returned sver={0} cmd={1} majorver={2} minorver={3} dataSize={4}.", args);
				replyInfo = <Module>.UToMGenericReplyInfo(ref tagGenericReplyInfo);
				return <Module>.UToMErrorExceptionInfo(ref tagErrorExceptionInfo);
			}
			catch when (endfilter(<Module>.DwExRpcExceptionFilter(Marshal.GetExceptionCode()) != null))
			{
				ReplayRpcException.ThrowRpcException((num >= 0) ? Marshal.GetExceptionCode() : num, "cli_RpccGenericRequest");
			}
			finally
			{
				<Module>.FreeGenericRequestInfo(&tagGenericRequestInfo);
				<Module>.FreeGenericReplyInfo(&tagGenericReplyInfo);
				<Module>.FreeErrorExceptionInfo(&tagErrorExceptionInfo);
			}
			return null;
		}

		[HandleProcessCorruptedStateExceptions]
		public unsafe void CreateMonitoringMailbox(string displayName, Guid dbGuid)
		{
			int num = 0;
			ushort* ptr = null;
			base.SetTimeOut(110000);
			try
			{
				ptr = <Module>.StringToUnmanaged(displayName);
				try
				{
					_GUID guid = <Module>.ToGUID(ref dbGuid);
					num = <Module>.cli_CreateMonitoringMailbox(base.BindingHandle, ptr, guid);
					if (num < 0)
					{
						RpcClientBase.ThrowRpcException(num, "cli_CreateMonitoringMailbox");
					}
				}
				catch when (endfilter(<Module>.DwExRpcExceptionFilter(Marshal.GetExceptionCode()) != null))
				{
					RpcClientBase.ThrowRpcException((num >= 0) ? Marshal.GetExceptionCode() : num, "cli_CreateMonitoringMailbox");
				}
			}
			finally
			{
				if (ptr != null)
				{
					<Module>.MIDL_user_free((void*)ptr);
				}
			}
		}

		private static int WLIDException = -2147417337;

		private static int OtherManagedException = -2147417336;
	}
}
