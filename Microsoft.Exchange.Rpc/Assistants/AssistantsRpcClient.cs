using System;
using System.Runtime.ExceptionServices;
using System.Runtime.InteropServices;

namespace Microsoft.Exchange.Rpc.Assistants
{
	internal class AssistantsRpcClient : RpcClientBase
	{
		public AssistantsRpcClient(string machineName) : base(machineName)
		{
		}

		[HandleProcessCorruptedStateExceptions]
		public unsafe void Start(string assistantName, ValueType mailboxGuid, ValueType mdbGuid)
		{
			IntPtr hglobal = 0;
			int num = 0;
			try
			{
				try
				{
					hglobal = Marshal.StringToHGlobalAnsi(assistantName);
					sbyte* ptr = (sbyte*)hglobal.ToPointer();
					_GUID guid = <Module>.Microsoft.Exchange.Rpc.?A0x2ff2afa6.GUIDFromGuid(mdbGuid);
					_GUID guid2 = <Module>.Microsoft.Exchange.Rpc.?A0x2ff2afa6.GUIDFromGuid(mailboxGuid);
					num = <Module>.cli_RunNowHR(base.BindingHandle, (sbyte*)ptr, guid2, guid);
				}
				catch when (endfilter(true))
				{
					RpcClientBase.ThrowRpcException(Marshal.GetExceptionCode(), "Start");
				}
				if (num < 0)
				{
					RpcClientBase.ThrowRpcException(num, "Start");
				}
			}
			finally
			{
				Marshal.FreeHGlobal(hglobal);
			}
		}

		[HandleProcessCorruptedStateExceptions]
		public unsafe void Stop(string assistantName)
		{
			IntPtr hglobal = 0;
			int num = 0;
			try
			{
				try
				{
					hglobal = Marshal.StringToHGlobalAnsi(assistantName);
					sbyte* ptr = (sbyte*)hglobal.ToPointer();
					num = <Module>.cli_HaltHR(base.BindingHandle, (sbyte*)ptr);
				}
				catch when (endfilter(true))
				{
					RpcClientBase.ThrowRpcException(Marshal.GetExceptionCode(), "Stop");
				}
				if (num < 0)
				{
					RpcClientBase.ThrowRpcException(num, "Stop");
				}
			}
			finally
			{
				Marshal.FreeHGlobal(hglobal);
			}
		}

		[HandleProcessCorruptedStateExceptions]
		public unsafe void StartWithParams(string assistantName, ValueType mailboxGuid, ValueType mdbGuid, string parameters)
		{
			IntPtr hglobal = 0;
			IntPtr hglobal2 = 0;
			int num = 0;
			try
			{
				try
				{
					hglobal = Marshal.StringToHGlobalAnsi(assistantName);
					hglobal2 = Marshal.StringToHGlobalAnsi(parameters);
					sbyte* ptr = (sbyte*)hglobal.ToPointer();
					sbyte* ptr2 = (sbyte*)hglobal2.ToPointer();
					_GUID guid = <Module>.Microsoft.Exchange.Rpc.?A0x2ff2afa6.GUIDFromGuid(mdbGuid);
					_GUID guid2 = <Module>.Microsoft.Exchange.Rpc.?A0x2ff2afa6.GUIDFromGuid(mailboxGuid);
					num = <Module>.cli_RunNowWithParamsHR(base.BindingHandle, (sbyte*)ptr, guid2, guid, (sbyte*)ptr2);
				}
				catch when (endfilter(true))
				{
					RpcClientBase.ThrowRpcException(Marshal.GetExceptionCode(), "StartWithParams");
				}
				if (num < 0)
				{
					RpcClientBase.ThrowRpcException(num, "StartWithParams");
				}
			}
			finally
			{
				Marshal.FreeHGlobal(hglobal);
				Marshal.FreeHGlobal(hglobal2);
			}
		}
	}
}
