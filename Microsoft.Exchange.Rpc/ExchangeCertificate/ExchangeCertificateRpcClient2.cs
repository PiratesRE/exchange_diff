using System;
using System.Runtime.ExceptionServices;
using System.Runtime.InteropServices;
using System.Security;

namespace Microsoft.Exchange.Rpc.ExchangeCertificate
{
	internal class ExchangeCertificateRpcClient2 : RpcClientBase
	{
		public ExchangeCertificateRpcClient2(string machineName) : base(machineName)
		{
		}

		[HandleProcessCorruptedStateExceptions]
		public unsafe byte[] GetCertificate2(int version, byte[] inBlob)
		{
			byte[] result = null;
			byte* ptr = null;
			byte* ptr2 = null;
			int cBytes = 0;
			try
			{
				try
				{
					int num = 0;
					ptr = <Module>.MToUBytesClient(inBlob, &num);
					int num2 = <Module>.cli_GetCertificate2(base.BindingHandle, version, num, ptr, &cBytes, &ptr2);
					if (num2 < 0)
					{
						RpcClientBase.ThrowRpcException(num2, "cli_GetCertificate2");
					}
				}
				catch when (endfilter(true))
				{
					RpcClientBase.ThrowRpcException(Marshal.GetExceptionCode(), "cli_GetCertificate2");
				}
				result = <Module>.UToMBytes(cBytes, ptr2);
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
		public unsafe byte[] CreateCertificate2(int version, byte[] inBlob)
		{
			byte[] result = null;
			byte* ptr = null;
			byte* ptr2 = null;
			int cBytes = 0;
			try
			{
				try
				{
					int num = 0;
					ptr = <Module>.MToUBytesClient(inBlob, &num);
					int num2 = <Module>.cli_CreateCertificate2(base.BindingHandle, version, num, ptr, &cBytes, &ptr2);
					if (num2 < 0)
					{
						RpcClientBase.ThrowRpcException(num2, "cli_CreateCertificate2");
					}
				}
				catch when (endfilter(true))
				{
					RpcClientBase.ThrowRpcException(Marshal.GetExceptionCode(), "cli_CreateCertificate2");
				}
				result = <Module>.UToMBytes(cBytes, ptr2);
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
		public unsafe byte[] RemoveCertificate2(int version, byte[] inBlob)
		{
			byte[] result = null;
			byte* ptr = null;
			byte* ptr2 = null;
			int cBytes = 0;
			try
			{
				try
				{
					int num = 0;
					ptr = <Module>.MToUBytesClient(inBlob, &num);
					int num2 = <Module>.cli_RemoveCertificate2(base.BindingHandle, version, num, ptr, &cBytes, &ptr2);
					if (num2 < 0)
					{
						RpcClientBase.ThrowRpcException(num2, "cli_RemoveCertificate2");
					}
				}
				catch when (endfilter(true))
				{
					RpcClientBase.ThrowRpcException(Marshal.GetExceptionCode(), "cli_RemoveCertificate2");
				}
				result = <Module>.UToMBytes(cBytes, ptr2);
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
		public unsafe byte[] ExportCertificate2(int version, byte[] inBlob, SecureString password)
		{
			byte[] result = null;
			byte* ptr = null;
			IntPtr intPtr = IntPtr.Zero;
			byte* ptr2 = null;
			int cBytes = 0;
			try
			{
				try
				{
					int num = 0;
					ptr = <Module>.MToUBytesClient(inBlob, &num);
					ushort* ptr3;
					if (password != null)
					{
						intPtr = Marshal.SecureStringToBSTR(password);
						ptr3 = (ushort*)intPtr.ToPointer();
					}
					else
					{
						ptr3 = (ushort*)(&<Module>.??_C@_11LOCGONAA@?$AA?$AA@);
					}
					int num2 = <Module>.cli_ExportCertificate2(base.BindingHandle, version, num, ptr, ptr3, &cBytes, &ptr2);
					if (num2 < 0)
					{
						RpcClientBase.ThrowRpcException(num2, "cli_ExportCertificate2");
					}
				}
				catch when (endfilter(true))
				{
					RpcClientBase.ThrowRpcException(Marshal.GetExceptionCode(), "cli_ExportCertificate2");
				}
				result = <Module>.UToMBytes(cBytes, ptr2);
			}
			finally
			{
				if (intPtr != IntPtr.Zero)
				{
					Marshal.ZeroFreeBSTR(intPtr);
				}
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
		public unsafe byte[] ImportCertificate2(int version, byte[] inBlob, SecureString password)
		{
			byte[] result = null;
			byte* ptr = null;
			IntPtr intPtr = IntPtr.Zero;
			byte* ptr2 = null;
			int cBytes = 0;
			try
			{
				try
				{
					int num = 0;
					ptr = <Module>.MToUBytesClient(inBlob, &num);
					ushort* ptr3;
					if (password != null)
					{
						intPtr = Marshal.SecureStringToBSTR(password);
						ptr3 = (ushort*)intPtr.ToPointer();
					}
					else
					{
						ptr3 = (ushort*)(&<Module>.??_C@_11LOCGONAA@?$AA?$AA@);
					}
					int num2 = <Module>.cli_ImportCertificate2(base.BindingHandle, version, num, ptr, ptr3, &cBytes, &ptr2);
					if (num2 < 0)
					{
						RpcClientBase.ThrowRpcException(num2, "cli_ImportCertificate2");
					}
				}
				catch when (endfilter(true))
				{
					RpcClientBase.ThrowRpcException(Marshal.GetExceptionCode(), "cli_ImportCertificate2");
				}
				result = <Module>.UToMBytes(cBytes, ptr2);
			}
			finally
			{
				if (intPtr != IntPtr.Zero)
				{
					Marshal.ZeroFreeBSTR(intPtr);
				}
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
		public unsafe byte[] EnableCertificate2(int version, byte[] inBlob)
		{
			byte[] result = null;
			byte* ptr = null;
			byte* ptr2 = null;
			int cBytes = 0;
			try
			{
				try
				{
					int num = 0;
					ptr = <Module>.MToUBytesClient(inBlob, &num);
					int num2 = <Module>.cli_EnableCertificate2(base.BindingHandle, version, num, ptr, &cBytes, &ptr2);
					if (num2 < 0)
					{
						RpcClientBase.ThrowRpcException(num2, "cli_EnableCertificate2");
					}
				}
				catch when (endfilter(true))
				{
					RpcClientBase.ThrowRpcException(Marshal.GetExceptionCode(), "cli_EnableCertificate2");
				}
				result = <Module>.UToMBytes(cBytes, ptr2);
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
	}
}
