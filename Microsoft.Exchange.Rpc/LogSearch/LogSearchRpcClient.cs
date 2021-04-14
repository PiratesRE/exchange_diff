using System;
using System.Runtime.ExceptionServices;
using System.Runtime.InteropServices;

namespace Microsoft.Exchange.Rpc.LogSearch
{
	internal class LogSearchRpcClient : RpcClientBase
	{
		public LogSearchRpcClient(string machineName) : base(machineName)
		{
		}

		[HandleProcessCorruptedStateExceptions]
		public unsafe int Search(string logName, byte[] query, [MarshalAs(UnmanagedType.U1)] bool continueInBackground, byte[] results, ref Guid sessionId, ref bool more, ref int progress)
		{
			byte* ptr = null;
			ushort* ptr2 = null;
			byte* ptr3 = null;
			int num = 0;
			int num2 = 0;
			try
			{
				ptr2 = <Module>.StringToUnmanaged(logName);
				int num3 = 0;
				ptr = <Module>.MToUBytesClient(query, &num3);
				int num4 = 0;
				bool flag;
				_GUID guid;
				int num6;
				int num7;
				do
				{
					num4++;
					flag = false;
					try
					{
						int num5 = continueInBackground ? 1 : 0;
						num2 = <Module>.cli_Search(base.BindingHandle, ptr2, num3, ptr, num5, &guid, results.Length, &ptr3, &num, &num6, &num7);
					}
					catch when (endfilter(true))
					{
						if (num4 == 1 && Marshal.GetExceptionCode() == 1727)
						{
							flag = true;
						}
						else
						{
							RpcClientBase.ThrowRpcException(Marshal.GetExceptionCode(), "Search");
						}
					}
				}
				while (flag);
				if (num2 >= 0 && num > results.Length)
				{
					num2 = LogSearchErrorCode.LOGSEARCH_E_RESPONSE_OVERFLOW;
				}
				if (num2 < 0)
				{
					throw new LogSearchException(num2);
				}
				if (ptr3 != null)
				{
					IntPtr source = new IntPtr((void*)ptr3);
					Marshal.Copy(source, results, 0, num);
				}
				Guid guid2 = <Module>.FromGUID(ref guid);
				sessionId = guid2;
				int num8 = (num6 == 1) ? 1 : 0;
				more = (num8 != 0);
				progress = num7;
			}
			finally
			{
				if (ptr2 != null)
				{
					<Module>.MIDL_user_free((void*)ptr2);
				}
				if (ptr != null)
				{
					<Module>.MIDL_user_free((void*)ptr);
				}
				if (ptr3 != null)
				{
					<Module>.MIDL_user_free((void*)ptr3);
				}
			}
			return num;
		}

		[HandleProcessCorruptedStateExceptions]
		public unsafe int SearchExtensibleSchema(string clientVersion, string logName, byte[] query, [MarshalAs(UnmanagedType.U1)] bool continueInBackground, byte[] results, ref Guid sessionId, ref bool more, ref int progress)
		{
			byte* ptr = null;
			ushort* ptr2 = null;
			byte* ptr3 = null;
			int num = 0;
			int num2 = 0;
			try
			{
				ptr2 = <Module>.StringToUnmanaged(logName);
				ushort* ptr4 = <Module>.StringToUnmanaged(clientVersion);
				int num3 = 0;
				ptr = <Module>.MToUBytesClient(query, &num3);
				int num4 = 0;
				bool flag;
				_GUID guid;
				int num6;
				int num7;
				do
				{
					num4++;
					flag = false;
					try
					{
						int num5 = continueInBackground ? 1 : 0;
						num2 = <Module>.cli_SearchExtensibleSchema(base.BindingHandle, ptr4, ptr2, num3, ptr, num5, &guid, results.Length, &ptr3, &num, &num6, &num7);
					}
					catch when (endfilter(true))
					{
						if (num4 == 1 && Marshal.GetExceptionCode() == 1727)
						{
							flag = true;
						}
						else
						{
							RpcClientBase.ThrowRpcException(Marshal.GetExceptionCode(), "Search");
						}
					}
				}
				while (flag);
				if (num2 >= 0 && num > results.Length)
				{
					num2 = LogSearchErrorCode.LOGSEARCH_E_RESPONSE_OVERFLOW;
				}
				if (num2 < 0)
				{
					throw new LogSearchException(num2);
				}
				if (ptr3 != null)
				{
					IntPtr source = new IntPtr((void*)ptr3);
					Marshal.Copy(source, results, 0, num);
				}
				Guid guid2 = <Module>.FromGUID(ref guid);
				sessionId = guid2;
				int num8 = (num6 == 1) ? 1 : 0;
				more = (num8 != 0);
				progress = num7;
			}
			finally
			{
				if (ptr2 != null)
				{
					<Module>.MIDL_user_free((void*)ptr2);
				}
				if (ptr != null)
				{
					<Module>.MIDL_user_free((void*)ptr);
				}
				if (ptr3 != null)
				{
					<Module>.MIDL_user_free((void*)ptr3);
				}
			}
			return num;
		}

		[HandleProcessCorruptedStateExceptions]
		public unsafe int Continue(Guid sessionId, [MarshalAs(UnmanagedType.U1)] bool continueInBackground, byte[] results, ref bool more, ref int progress)
		{
			byte* ptr = null;
			int num = 0;
			int num2 = 0;
			try
			{
				int num4;
				int num5;
				try
				{
					int num3 = continueInBackground ? 1 : 0;
					_GUID guid = <Module>.ToGUID(ref sessionId);
					num2 = <Module>.cli_Continue(base.BindingHandle, guid, num3, results.Length, &ptr, &num, &num4, &num5);
				}
				catch when (endfilter(true))
				{
					RpcClientBase.ThrowRpcException(Marshal.GetExceptionCode(), "Continue");
				}
				if (num2 >= 0 && num > results.Length)
				{
					num2 = LogSearchErrorCode.LOGSEARCH_E_RESPONSE_OVERFLOW;
				}
				if (num2 < 0)
				{
					throw new LogSearchException(num2);
				}
				if (ptr != null)
				{
					IntPtr source = new IntPtr((void*)ptr);
					Marshal.Copy(source, results, 0, num);
				}
				int num6 = (num4 == 1) ? 1 : 0;
				more = (num6 != 0);
				progress = num5;
			}
			finally
			{
				if (ptr != null)
				{
					<Module>.MIDL_user_free((void*)ptr);
				}
			}
			return num;
		}

		[HandleProcessCorruptedStateExceptions]
		public void Cancel(Guid sessionId)
		{
			int num = 0;
			try
			{
				_GUID guid = <Module>.ToGUID(ref sessionId);
				num = <Module>.cli_Cancel(base.BindingHandle, guid);
			}
			catch when (endfilter(true))
			{
				RpcClientBase.ThrowRpcException(Marshal.GetExceptionCode(), "Cancel");
			}
			if (num < 0)
			{
				throw new LogSearchException(num);
			}
		}
	}
}
