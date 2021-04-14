using System;
using System.Runtime.ExceptionServices;
using System.Runtime.InteropServices;
using Microsoft.Exchange.Rpc.Nspi;

namespace Microsoft.Exchange.Rpc.MigrationService
{
	internal class MigrationProxyRpcClient : RpcClientBase, IMigrationProxyRpc
	{
		public MigrationProxyRpcClient(string machineName) : base(machineName)
		{
		}

		[HandleProcessCorruptedStateExceptions]
		public unsafe virtual int NspiQueryRows(int version, byte[] inBlob, out byte[] outBlob, out SafeRpcMemoryHandle rowsetHandle)
		{
			int result = -2147467259;
			_SRowSet_r* ptr = null;
			byte* ptr2 = null;
			byte* ptr3 = null;
			int cBytes = 0;
			int num = 0;
			outBlob = null;
			rowsetHandle = null;
			try
			{
				bool flag = false;
				do
				{
					try
					{
						ptr2 = <Module>.MToUBytesClient(inBlob, &num);
						result = <Module>.cli_ProxyNspiQueryRows(base.BindingHandle, version, num, ptr2, &cBytes, &ptr3, &ptr);
						flag = false;
					}
					catch when (endfilter(true))
					{
						int exceptionCode = Marshal.GetExceptionCode();
						if (1727 == exceptionCode && !flag)
						{
							flag = true;
						}
						else
						{
							RpcClientBase.ThrowRpcException(exceptionCode, "ProxyNspiQueryRows");
						}
					}
				}
				while (flag);
				outBlob = <Module>.UToMBytes(cBytes, ptr3);
				if (ptr != null)
				{
					IntPtr handle = new IntPtr((void*)ptr);
					rowsetHandle = new SafeSRowSetHandle(handle);
				}
			}
			finally
			{
				if (ptr2 != null)
				{
					<Module>.MIDL_user_free((void*)ptr2);
				}
				if (ptr3 != null)
				{
					<Module>.MIDL_user_free((void*)ptr3);
				}
			}
			return result;
		}

		[HandleProcessCorruptedStateExceptions]
		public unsafe virtual int NspiGetRecipient(int version, byte[] inBlob, out byte[] outBlob, out SafeRpcMemoryHandle rowsetHandle)
		{
			int result = -2147467259;
			_SRowSet_r* ptr = null;
			byte* ptr2 = null;
			byte* ptr3 = null;
			int cBytes = 0;
			int num = 0;
			outBlob = null;
			rowsetHandle = null;
			try
			{
				bool flag = false;
				do
				{
					try
					{
						ptr2 = <Module>.MToUBytesClient(inBlob, &num);
						result = <Module>.cli_ProxyNspiGetRecipient(base.BindingHandle, version, num, ptr2, &cBytes, &ptr3, &ptr);
						flag = false;
					}
					catch when (endfilter(true))
					{
						int exceptionCode = Marshal.GetExceptionCode();
						if (1727 == exceptionCode && !flag)
						{
							flag = true;
						}
						else
						{
							RpcClientBase.ThrowRpcException(exceptionCode, "ProxyNspiGetRecipient");
						}
					}
				}
				while (flag);
				outBlob = <Module>.UToMBytes(cBytes, ptr3);
				if (ptr != null)
				{
					IntPtr handle = new IntPtr((void*)ptr);
					rowsetHandle = new SafeSRowSetHandle(handle);
				}
			}
			finally
			{
				if (ptr2 != null)
				{
					<Module>.MIDL_user_free((void*)ptr2);
				}
				if (ptr3 != null)
				{
					<Module>.MIDL_user_free((void*)ptr3);
				}
			}
			return result;
		}

		[HandleProcessCorruptedStateExceptions]
		public unsafe virtual int NspiSetRecipient(int version, byte[] inBlob, out byte[] outBlob)
		{
			int result = -2147467259;
			byte* ptr = null;
			byte* ptr2 = null;
			int cBytes = 0;
			int num = 0;
			outBlob = null;
			try
			{
				bool flag = false;
				do
				{
					try
					{
						ptr = <Module>.MToUBytesClient(inBlob, &num);
						result = <Module>.cli_ProxyNspiSetRecipient(base.BindingHandle, version, num, ptr, &cBytes, &ptr2);
						flag = false;
					}
					catch when (endfilter(true))
					{
						int exceptionCode = Marshal.GetExceptionCode();
						if (1727 == exceptionCode && !flag)
						{
							flag = true;
						}
						else
						{
							RpcClientBase.ThrowRpcException(exceptionCode, "ProxyNspiSetRecipient");
						}
					}
				}
				while (flag);
				outBlob = <Module>.UToMBytes(cBytes, ptr2);
			}
			finally
			{
				if (ptr != null)
				{
					<Module>.MIDL_user_free((void*)ptr);
				}
				if (ptr2 != null)
				{
					<Module>.MIDL_user_free((void*)ptr2);
				}
			}
			return result;
		}

		[HandleProcessCorruptedStateExceptions]
		public unsafe virtual int NspiGetGroupMembers(int version, byte[] inBlob, out byte[] outBlob, out SafeRpcMemoryHandle rowsetHandle)
		{
			int result = -2147467259;
			_SRowSet_r* ptr = null;
			byte* ptr2 = null;
			byte* ptr3 = null;
			int cBytes = 0;
			int num = 0;
			outBlob = null;
			rowsetHandle = null;
			try
			{
				bool flag = false;
				do
				{
					try
					{
						ptr2 = <Module>.MToUBytesClient(inBlob, &num);
						result = <Module>.cli_ProxyNspiGetGroupMembers(base.BindingHandle, version, num, ptr2, &cBytes, &ptr3, &ptr);
						flag = false;
					}
					catch when (endfilter(true))
					{
						int exceptionCode = Marshal.GetExceptionCode();
						if (1727 == exceptionCode && !flag)
						{
							flag = true;
						}
						else
						{
							RpcClientBase.ThrowRpcException(exceptionCode, "ProxyNspiGetGroupMembers");
						}
					}
				}
				while (flag);
				outBlob = <Module>.UToMBytes(cBytes, ptr3);
				if (ptr != null)
				{
					IntPtr handle = new IntPtr((void*)ptr);
					rowsetHandle = new SafeSRowSetHandle(handle);
				}
			}
			finally
			{
				if (ptr2 != null)
				{
					<Module>.MIDL_user_free((void*)ptr2);
				}
				if (ptr3 != null)
				{
					<Module>.MIDL_user_free((void*)ptr3);
				}
			}
			return result;
		}

		[HandleProcessCorruptedStateExceptions]
		public unsafe virtual int NspiRfrGetNewDSA(int version, byte[] inBlob, out byte[] outBlob)
		{
			int result = -2147467259;
			byte* ptr = null;
			byte* ptr2 = null;
			int cBytes = 0;
			int num = 0;
			outBlob = null;
			try
			{
				bool flag = false;
				do
				{
					try
					{
						ptr = <Module>.MToUBytesClient(inBlob, &num);
						result = <Module>.cli_ProxyNspiRfrGetNewDSA(base.BindingHandle, version, num, ptr, &cBytes, &ptr2);
						flag = false;
					}
					catch when (endfilter(true))
					{
						int exceptionCode = Marshal.GetExceptionCode();
						if (1727 == exceptionCode && !flag)
						{
							flag = true;
						}
						else
						{
							RpcClientBase.ThrowRpcException(exceptionCode, "ProxyNspiRfrGetNewDSA");
						}
					}
				}
				while (flag);
				outBlob = <Module>.UToMBytes(cBytes, ptr2);
			}
			finally
			{
				if (ptr != null)
				{
					<Module>.MIDL_user_free((void*)ptr);
				}
				if (ptr2 != null)
				{
					<Module>.MIDL_user_free((void*)ptr2);
				}
			}
			return result;
		}

		[HandleProcessCorruptedStateExceptions]
		public unsafe virtual void AutodiscoverGetUserSettings(int version, byte[] inBlob, out byte[] outBlob)
		{
			byte* ptr = null;
			byte* ptr2 = null;
			int cBytes = 0;
			int num = 0;
			outBlob = null;
			try
			{
				bool flag = false;
				do
				{
					try
					{
						ptr = <Module>.MToUBytesClient(inBlob, &num);
						<Module>.cli_ProxyAutodiscoverGetUserSettings(base.BindingHandle, version, num, ptr, &cBytes, &ptr2);
						flag = false;
					}
					catch when (endfilter(true))
					{
						int exceptionCode = Marshal.GetExceptionCode();
						if (1727 == exceptionCode && !flag)
						{
							flag = true;
						}
						else
						{
							RpcClientBase.ThrowRpcException(exceptionCode, "ProxyAutodiscoverGetUserSettings");
						}
					}
				}
				while (flag);
				outBlob = <Module>.UToMBytes(cBytes, ptr2);
			}
			finally
			{
				if (ptr != null)
				{
					<Module>.MIDL_user_free((void*)ptr);
				}
				if (ptr2 != null)
				{
					<Module>.MIDL_user_free((void*)ptr2);
				}
			}
		}

		public const int RpcErrorServerTooBusy = 1723;
	}
}
