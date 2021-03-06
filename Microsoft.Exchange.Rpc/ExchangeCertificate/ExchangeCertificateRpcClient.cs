using System;
using System.Runtime.ExceptionServices;
using System.Runtime.InteropServices;
using System.Security;

namespace Microsoft.Exchange.Rpc.ExchangeCertificate
{
	internal class ExchangeCertificateRpcClient : RpcClientBase
	{
		public ExchangeCertificateRpcClient(string machineName) : base(machineName)
		{
		}

		[HandleProcessCorruptedStateExceptions]
		public unsafe byte[] GetCertificate(int version, byte[] inBlob)
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
					int num2 = <Module>.cli_GetCertificate(base.BindingHandle, version, num, ptr, &cBytes, &ptr2);
					if (num2 < 0)
					{
						RpcClientBase.ThrowRpcException(num2, "cli_GetCertificate");
					}
				}
				catch when (endfilter(true))
				{
					RpcClientBase.ThrowRpcException(Marshal.GetExceptionCode(), "cli_GetCertificate");
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
		public unsafe byte[] CreateCertificate(int version, byte[] inBlob)
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
					int num2 = <Module>.cli_CreateCertificate(base.BindingHandle, version, num, ptr, &cBytes, &ptr2);
					if (num2 < 0)
					{
						RpcClientBase.ThrowRpcException(num2, "cli_CreateCertificate");
					}
				}
				catch when (endfilter(true))
				{
					RpcClientBase.ThrowRpcException(Marshal.GetExceptionCode(), "cli_CreateCertificate");
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
		public unsafe byte[] RemoveCertificate(int version, byte[] inBlob)
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
					int num2 = <Module>.cli_RemoveCertificate(base.BindingHandle, version, num, ptr, &cBytes, &ptr2);
					if (num2 < 0)
					{
						RpcClientBase.ThrowRpcException(num2, "cli_RemoveCertificate");
					}
				}
				catch when (endfilter(true))
				{
					RpcClientBase.ThrowRpcException(Marshal.GetExceptionCode(), "cli_RemoveCertificate");
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
		public unsafe byte[] ExportCertificate(int version, byte[] inBlob, SecureString password)
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
					int num2 = <Module>.cli_ExportCertificate(base.BindingHandle, version, num, ptr, ptr3, &cBytes, &ptr2);
					if (num2 < 0)
					{
						RpcClientBase.ThrowRpcException(num2, "cli_ExportCertificate");
					}
				}
				catch when (endfilter(true))
				{
					RpcClientBase.ThrowRpcException(Marshal.GetExceptionCode(), "cli_ExportCertificate");
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
		public unsafe byte[] ImportCertificate(int version, byte[] inBlob, SecureString password)
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
					int num2 = <Module>.cli_ImportCertificate(base.BindingHandle, version, num, ptr, ptr3, &cBytes, &ptr2);
					if (num2 < 0)
					{
						RpcClientBase.ThrowRpcException(num2, "cli_ImportCertificate");
					}
				}
				catch when (endfilter(true))
				{
					RpcClientBase.ThrowRpcException(Marshal.GetExceptionCode(), "cli_ImportCertificate");
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
		public unsafe byte[] EnableCertificate(int version, byte[] inBlob)
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
					int num2 = <Module>.cli_EnableCertificate(base.BindingHandle, version, num, ptr, &cBytes, &ptr2);
					if (num2 < 0)
					{
						RpcClientBase.ThrowRpcException(num2, "cli_EnableCertificate");
					}
				}
				catch when (endfilter(true))
				{
					RpcClientBase.ThrowRpcException(Marshal.GetExceptionCode(), "cli_EnableCertificate");
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
