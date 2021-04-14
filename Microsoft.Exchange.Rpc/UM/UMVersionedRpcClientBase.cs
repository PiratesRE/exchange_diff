using System;
using System.Runtime.ExceptionServices;
using System.Runtime.InteropServices;

namespace Microsoft.Exchange.Rpc.UM
{
	internal class UMVersionedRpcClientBase : RpcClientBase
	{
		protected UMVersionedRpcClientBase(string serverFqdn) : base(serverFqdn)
		{
		}

		public string OperationName
		{
			get
			{
				return this.operationName;
			}
		}

		[HandleProcessCorruptedStateExceptions]
		public unsafe virtual byte[] ExecuteRequest(byte[] request)
		{
			int num = 0;
			int cBytes = 0;
			byte* ptr = null;
			byte* ptr2 = null;
			byte[] result = null;
			try
			{
				int num2;
				try
				{
					num2 = <Module>.MToUBytes(request, &num, &ptr);
					if (num2 >= 0)
					{
						num2 = calli(System.Int32 modopt(System.Runtime.CompilerServices.IsLong) modopt(System.Runtime.CompilerServices.CallConvCdecl)(System.Void*,System.Int32,System.Byte*,System.Int32*,System.Byte**), base.BindingHandle, num, ptr, ref cBytes, ref ptr2, this.executeRequestDelegate);
						if (num2 >= 0 && ptr2 != null)
						{
							result = <Module>.UToMBytes(cBytes, ptr2);
						}
					}
				}
				catch when (endfilter(true))
				{
					RpcClientBase.ThrowRpcException(Marshal.GetExceptionCode(), this.operationName);
				}
				if (num2 < 0)
				{
					RpcClientBase.ThrowRpcException(num2, this.operationName);
				}
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

		protected string operationName;

		protected method executeRequestDelegate;
	}
}
