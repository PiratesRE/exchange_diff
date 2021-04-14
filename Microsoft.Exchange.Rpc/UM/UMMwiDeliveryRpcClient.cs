using System;
using System.Net;
using System.Runtime.ExceptionServices;
using System.Runtime.InteropServices;

namespace Microsoft.Exchange.Rpc.UM
{
	internal class UMMwiDeliveryRpcClient : RpcClientBase
	{
		public string OperationName
		{
			get
			{
				return this.operationName;
			}
		}

		public UMMwiDeliveryRpcClient(string machineName, NetworkCredential nc) : base(machineName, null, nc, AuthenticationService.Negotiate, null, true)
		{
			try
			{
				this.operationName = string.Format("{0}(IUMMwiDelivery)", machineName);
			}
			catch
			{
				base.Dispose(true);
				throw;
			}
		}

		public UMMwiDeliveryRpcClient(string machineName) : base(machineName)
		{
			try
			{
				this.operationName = string.Format("{0}(IUMMwiDelivery)", machineName);
			}
			catch
			{
				base.Dispose(true);
				throw;
			}
		}

		[HandleProcessCorruptedStateExceptions]
		public unsafe virtual void SendMwiMessage(Guid mailboxGuid, Guid dialPlanGuid, string userExtension, string userName, int unreadVoicemailCount, int totalVoicemailCount, int assistantLatencyMsec, Guid tenantGuid)
		{
			int num = 0;
			IntPtr intPtr = IntPtr.Zero;
			IntPtr intPtr2 = IntPtr.Zero;
			try
			{
				try
				{
					intPtr = Marshal.StringToHGlobalUni(userExtension);
					intPtr2 = Marshal.StringToHGlobalUni(userName);
					_GUID guid = <Module>.ToGUID(ref tenantGuid);
					_GUID guid2 = <Module>.ToGUID(ref dialPlanGuid);
					_GUID guid3 = <Module>.ToGUID(ref mailboxGuid);
					num = <Module>.cli_SendMwiMessage_v2_0(base.BindingHandle, guid3, guid2, (ushort*)intPtr.ToPointer(), (ushort*)intPtr2.ToPointer(), unreadVoicemailCount, totalVoicemailCount, assistantLatencyMsec, guid);
				}
				catch when (endfilter(true))
				{
					RpcClientBase.ThrowRpcException(Marshal.GetExceptionCode(), "SendMwiMessage");
				}
				if (num < 0)
				{
					RpcClientBase.ThrowRpcException(num, "SendMwiMessage");
				}
			}
			finally
			{
				if (IntPtr.Zero != intPtr)
				{
					Marshal.FreeHGlobal(intPtr);
				}
				if (IntPtr.Zero != intPtr2)
				{
					Marshal.FreeHGlobal(intPtr2);
				}
			}
		}

		protected string operationName;
	}
}
