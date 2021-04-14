using System;
using System.Runtime.ExceptionServices;
using System.Runtime.InteropServices;
using System.Threading;

namespace Microsoft.Exchange.Rpc.Throttling
{
	internal class ThrottlingRpcClient : RpcClientBase
	{
		public ThrottlingRpcClient(string machineName) : base(machineName)
		{
			try
			{
				this.m_refCount = 1;
			}
			catch
			{
				base.Dispose(true);
				throw;
			}
		}

		[HandleProcessCorruptedStateExceptions]
		[return: MarshalAs(UnmanagedType.U1)]
		public bool ObtainSubmissionTokens(Guid mailboxGuid, int requestedTokenCount, int totalTokenCount, int submissionType)
		{
			int num = 1;
			try
			{
				_GUID guid = <Module>.ToGUID(ref mailboxGuid);
				num = <Module>.cli_ObtainSubmissionTokens(base.BindingHandle, guid, requestedTokenCount, totalTokenCount, submissionType);
				return ((num == 1) ? 1 : 0) != 0;
			}
			catch when (endfilter(true))
			{
				RpcClientBase.ThrowRpcException(Marshal.GetExceptionCode(), "ObtainSubmissionTokens");
			}
			finally
			{
			}
			return ((num == 1) ? 1 : 0) != 0;
		}

		[HandleProcessCorruptedStateExceptions]
		public unsafe byte[] ObtainTokens(byte[] inBytes)
		{
			byte[] result = null;
			IntPtr hglobal = IntPtr.Zero;
			byte* ptr = null;
			int cBytes = 0;
			try
			{
				bool flag = false;
				do
				{
					try
					{
						int num = 0;
						hglobal = <Module>.MToUBytes(inBytes, &num);
						<Module>.cli_ObtainTokens(base.BindingHandle, num, (byte*)hglobal.ToPointer(), &cBytes, &ptr);
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
							RpcClientBase.ThrowRpcException(Marshal.GetExceptionCode(), "ObtainTokens");
						}
					}
				}
				while (flag);
				result = <Module>.UToMBytes(cBytes, ptr);
			}
			finally
			{
				Marshal.FreeHGlobal(hglobal);
				if (ptr != null)
				{
					<Module>.MIDL_user_free((void*)ptr);
				}
			}
			return result;
		}

		public void AddRef()
		{
			Interlocked.Increment(ref this.m_refCount);
		}

		public int RemoveRef()
		{
			return Interlocked.Decrement(ref this.m_refCount);
		}

		private int m_refCount;

		public static int RpcServerTooBusy = 1723;
	}
}
