﻿using System;
using System.Net;
using System.Runtime.ExceptionServices;
using System.Runtime.InteropServices;

namespace Microsoft.Exchange.Rpc.MigrationService
{
	internal class MigrationNotificationRpcClient : RpcClientBase, IMigrationNotificationRpc
	{
		public MigrationNotificationRpcClient(string machineName, NetworkCredential nc) : base(machineName, nc)
		{
		}

		public MigrationNotificationRpcClient(string machineName) : base(machineName)
		{
		}

		[HandleProcessCorruptedStateExceptions]
		public unsafe virtual byte[] UpdateMigrationRequest(int version, byte[] inBlob)
		{
			byte[] result = null;
			byte* ptr = null;
			byte* ptr2 = null;
			int cBytes = 0;
			try
			{
				bool flag = false;
				do
				{
					try
					{
						int num = 0;
						ptr = <Module>.MToUBytesClient(inBlob, &num);
						<Module>.cli_UpdateMigrationRequest(base.BindingHandle, version, num, ptr, &cBytes, &ptr2);
						flag = false;
					}
					catch when (endfilter(true))
					{
						int exceptionCode = Marshal.GetExceptionCode();
						if (1727 == exceptionCode)
						{
							flag = (!flag || flag);
						}
						RpcClientBase.ThrowRpcException(exceptionCode, "UpdateMigrationRequest");
					}
				}
				while (flag);
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

		public static int RpcServerTooBusy = 1723;
	}
}
