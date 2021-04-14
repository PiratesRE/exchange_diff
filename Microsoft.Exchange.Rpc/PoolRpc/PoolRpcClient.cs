using System;
using System.Runtime.ExceptionServices;
using System.Runtime.InteropServices;
using <CppImplementationDetails>;

namespace Microsoft.Exchange.Rpc.PoolRpc
{
	internal sealed class PoolRpcClient : RpcClientBase
	{
		public PoolRpcClient(string machineName, ValueType clientObjectGuid) : base(machineName, null, null, null, RpcClientFlags.AllowImpersonation | RpcClientFlags.UseEncryptedConnection | RpcClientFlags.UniqueBinding | RpcClientFlags.ExplicitEndpointLookup, <Module>.emsmdbpool_v0_1_s_ifspec, clientObjectGuid, null, HttpAuthenticationScheme.Basic, AuthenticationService.Negotiate)
		{
		}

		public IRpcAsyncResult BeginEcPoolConnect(uint flags, Guid poolGuid, ArraySegment<byte> auxIn, AsyncCallback callback, object state)
		{
			int num = 0;
			byte ptr;
			if (auxIn.Count > 0)
			{
				num = auxIn.Count;
				if (num > 0)
				{
					ptr = ref auxIn.Array[auxIn.Offset];
					goto IL_3B;
				}
			}
			ptr = ref (new byte[1])[0];
			IL_3B:
			ref byte pbAuxIn = ref ptr;
			return this.InternalBeginEcPoolConnect(flags, poolGuid, num, ref pbAuxIn, callback, state);
		}

		public int EndEcPoolConnect(IRpcAsyncResult asyncResult, out IntPtr contextHandle, out uint flags, out uint maxPoolSize, out Guid poolGuid, out ArraySegment<byte> auxOut)
		{
			if (null == asyncResult)
			{
				throw new ArgumentNullException("asyncResult");
			}
			EcPoolConnectAsyncResult ecPoolConnectAsyncResult = asyncResult as EcPoolConnectAsyncResult;
			if (null == ecPoolConnectAsyncResult)
			{
				throw new ArgumentException("Invalid type.", "asyncResult");
			}
			int result;
			try
			{
				ecPoolConnectAsyncResult.AsyncWaitHandle.WaitOne();
				result = ecPoolConnectAsyncResult.Complete(out contextHandle, out flags, out maxPoolSize, out poolGuid, out auxOut);
			}
			finally
			{
				if (ecPoolConnectAsyncResult != null)
				{
					((IDisposable)ecPoolConnectAsyncResult).Dispose();
				}
			}
			return result;
		}

		[HandleProcessCorruptedStateExceptions]
		public unsafe int EcPoolDisconnect(IntPtr contextHandle)
		{
			if (IntPtr.Zero != contextHandle)
			{
				try
				{
					void* ptr = contextHandle.ToPointer();
					<Module>.RpcSsDestroyClientContext(&ptr);
				}
				catch when (endfilter(true))
				{
				}
			}
			return 0;
		}

		public IRpcAsyncResult BeginEcPoolCreateSession(IntPtr contextHandle, byte[] sessionSecurityContext, uint flags, string userDn, uint connectionMode, uint codePageId, uint localeIdString, uint localeIdSort, short[] clientVersion, ArraySegment<byte> auxIn, AsyncCallback callback, object state)
		{
			int num = 0;
			byte ptr;
			if (auxIn.Count > 0)
			{
				num = auxIn.Count;
				if (num > 0)
				{
					ptr = ref auxIn.Array[auxIn.Offset];
					goto IL_3B;
				}
			}
			ptr = ref (new byte[1])[0];
			IL_3B:
			ref byte pbAuxIn = ref ptr;
			return this.InternalBeginEcPoolCreateSession(contextHandle, sessionSecurityContext, flags, userDn, connectionMode, codePageId, localeIdString, localeIdSort, clientVersion, num, ref pbAuxIn, callback, state);
		}

		public IRpcAsyncResult BeginEcPoolCreateSession(IntPtr contextHandle, uint flags, string userDn, uint connectionMode, uint codePageId, uint localeIdString, uint localeIdSort, short[] clientVersion, ArraySegment<byte> auxIn, AsyncCallback callback, object state)
		{
			return this.BeginEcPoolCreateSession(contextHandle, null, flags, userDn, connectionMode, codePageId, localeIdString, localeIdSort, clientVersion, auxIn, callback, state);
		}

		public int EndEcPoolCreateSession(IRpcAsyncResult asyncResult, out uint sessionHandle, out string displayName, out uint maximumPolls, out uint retryCount, out uint retryDelay, out uint timeStamp, out short[] serverVersion, out short[] bestVersion, out ArraySegment<byte> auxOut)
		{
			if (null == asyncResult)
			{
				throw new ArgumentNullException("asyncResult");
			}
			EcPoolCreateSessionAsyncResult ecPoolCreateSessionAsyncResult = asyncResult as EcPoolCreateSessionAsyncResult;
			if (null == ecPoolCreateSessionAsyncResult)
			{
				throw new ArgumentException("Invalid type.", "asyncResult");
			}
			int result;
			try
			{
				ecPoolCreateSessionAsyncResult.AsyncWaitHandle.WaitOne();
				result = ecPoolCreateSessionAsyncResult.Complete(out sessionHandle, out displayName, out maximumPolls, out retryCount, out retryDelay, out timeStamp, out serverVersion, out bestVersion, out auxOut);
			}
			finally
			{
				if (ecPoolCreateSessionAsyncResult != null)
				{
					((IDisposable)ecPoolCreateSessionAsyncResult).Dispose();
				}
			}
			return result;
		}

		[HandleProcessCorruptedStateExceptions]
		public unsafe IRpcAsyncResult BeginEcPoolCloseSession(IntPtr contextHandle, uint sessionHandle, AsyncCallback callback, object state)
		{
			EcPoolCloseSessionAsyncResult ecPoolCloseSessionAsyncResult = null;
			EcPoolCloseSessionAsyncResult result = null;
			GCHandle value = default(GCHandle);
			bool flag = false;
			try
			{
				ecPoolCloseSessionAsyncResult = new EcPoolCloseSessionAsyncResult(callback, state);
				value = GCHandle.Alloc(ecPoolCloseSessionAsyncResult);
				flag = true;
				int num = <Module>.RpcAsyncInitializeHandle((_RPC_ASYNC_STATE*)ecPoolCloseSessionAsyncResult.NativeState(), 112U);
				if (num != null)
				{
					<Module>.Microsoft.Exchange.Rpc.ThrowRpcExceptionWithEEInfo(num, "EcCloseSession, RpcAsyncInitializeHandle");
				}
				*(long*)(ecPoolCloseSessionAsyncResult.NativeState() + 24L / (long)sizeof(EcPoolCloseSessionAsyncState)) = 0L;
				*(int*)(ecPoolCloseSessionAsyncResult.NativeState() + 44L / (long)sizeof(EcPoolCloseSessionAsyncState)) = 1;
				IntPtr handle = ecPoolCloseSessionAsyncResult.AsyncWaitHandle.Handle;
				*(long*)(ecPoolCloseSessionAsyncResult.NativeState() + 48L / (long)sizeof(EcPoolCloseSessionAsyncState)) = handle.ToPointer();
				IntPtr rootedAsyncState = GCHandle.ToIntPtr(value);
				bool flag2 = ecPoolCloseSessionAsyncResult.RegisterWait(rootedAsyncState);
				try
				{
					<Module>.cli_EcPoolCloseSession((_RPC_ASYNC_STATE*)ecPoolCloseSessionAsyncResult.NativeState(), contextHandle.ToPointer(), sessionHandle);
					result = ecPoolCloseSessionAsyncResult;
					ecPoolCloseSessionAsyncResult = null;
					flag = (((!flag2) ? 1 : 0) != 0);
				}
				catch when (endfilter(true))
				{
					num = Marshal.GetExceptionCode();
					<Module>.Microsoft.Exchange.Rpc.ThrowRpcExceptionWithEEInfo(num, "EcCloseSession");
				}
			}
			finally
			{
				if (null != ecPoolCloseSessionAsyncResult)
				{
					((IDisposable)ecPoolCloseSessionAsyncResult).Dispose();
				}
				if (flag)
				{
					value.Free();
				}
			}
			return result;
		}

		public int EndEcPoolCloseSession(IRpcAsyncResult asyncResult)
		{
			if (null == asyncResult)
			{
				throw new ArgumentNullException("asyncResult");
			}
			EcPoolCloseSessionAsyncResult ecPoolCloseSessionAsyncResult = asyncResult as EcPoolCloseSessionAsyncResult;
			if (null == ecPoolCloseSessionAsyncResult)
			{
				throw new ArgumentException("Invalid type.", "asyncResult");
			}
			int result;
			try
			{
				ecPoolCloseSessionAsyncResult.AsyncWaitHandle.WaitOne();
				result = ecPoolCloseSessionAsyncResult.Complete();
			}
			finally
			{
				if (ecPoolCloseSessionAsyncResult != null)
				{
					((IDisposable)ecPoolCloseSessionAsyncResult).Dispose();
				}
			}
			return result;
		}

		public IRpcAsyncResult BeginEcPoolSessionDoRpc(IntPtr contextHandle, uint sessionHandle, uint flags, uint maximumResponseSize, ArraySegment<byte> request, ArraySegment<byte> auxIn, AsyncCallback callback, object state)
		{
			int num = 0;
			int num2 = 0;
			if (request.Count > 0)
			{
				num = request.Count;
			}
			if (auxIn.Count > 0)
			{
				num2 = auxIn.Count;
			}
			ref byte pbIn = ref (num <= 0) ? ref (new byte[1])[0] : ref request.Array[request.Offset];
			ref byte pbAuxIn = ref (num2 <= 0) ? ref (new byte[1])[0] : ref auxIn.Array[auxIn.Offset];
			return this.InternalBeginEcPoolSessionDoRpc(contextHandle, sessionHandle, flags, maximumResponseSize, num, ref pbIn, num2, ref pbAuxIn, callback, state);
		}

		public int EndEcPoolSessionDoRpc(IRpcAsyncResult asyncResult, out uint flags, out ArraySegment<byte> response, out ArraySegment<byte> auxOut)
		{
			if (null == asyncResult)
			{
				throw new ArgumentNullException("asyncResult");
			}
			EcPoolSessionDoRpcAsyncResult ecPoolSessionDoRpcAsyncResult = asyncResult as EcPoolSessionDoRpcAsyncResult;
			if (null == ecPoolSessionDoRpcAsyncResult)
			{
				throw new ArgumentException("Invalid type.", "asyncResult");
			}
			int result;
			try
			{
				ecPoolSessionDoRpcAsyncResult.AsyncWaitHandle.WaitOne();
				result = ecPoolSessionDoRpcAsyncResult.Complete(out flags, out response, out auxOut);
			}
			finally
			{
				if (ecPoolSessionDoRpcAsyncResult != null)
				{
					((IDisposable)ecPoolSessionDoRpcAsyncResult).Dispose();
				}
			}
			return result;
		}

		[HandleProcessCorruptedStateExceptions]
		public unsafe IRpcAsyncResult BeginEcPoolWaitForNotificationsAsync(IntPtr contextHandle, AsyncCallback callback, object state)
		{
			EcPoolWaitForNotificationsAsyncResult ecPoolWaitForNotificationsAsyncResult = null;
			EcPoolWaitForNotificationsAsyncResult result = null;
			GCHandle value = default(GCHandle);
			bool flag = false;
			try
			{
				ecPoolWaitForNotificationsAsyncResult = new EcPoolWaitForNotificationsAsyncResult(callback, state);
				value = GCHandle.Alloc(ecPoolWaitForNotificationsAsyncResult);
				flag = true;
				int num = <Module>.RpcAsyncInitializeHandle((_RPC_ASYNC_STATE*)ecPoolWaitForNotificationsAsyncResult.NativeState(), 112U);
				if (num != null)
				{
					<Module>.Microsoft.Exchange.Rpc.ThrowRpcExceptionWithEEInfo(num, "EcPoolWaitForNotificationsAsync, RpcAsyncInitializeHandle");
				}
				*(long*)(ecPoolWaitForNotificationsAsyncResult.NativeState() + 24L / (long)sizeof(EcPoolWaitForNotificationsAsyncState)) = 0L;
				*(int*)(ecPoolWaitForNotificationsAsyncResult.NativeState() + 44L / (long)sizeof(EcPoolWaitForNotificationsAsyncState)) = 1;
				IntPtr handle = ecPoolWaitForNotificationsAsyncResult.AsyncWaitHandle.Handle;
				*(long*)(ecPoolWaitForNotificationsAsyncResult.NativeState() + 48L / (long)sizeof(EcPoolWaitForNotificationsAsyncState)) = handle.ToPointer();
				IntPtr rootedAsyncState = GCHandle.ToIntPtr(value);
				bool flag2 = ecPoolWaitForNotificationsAsyncResult.RegisterWait(rootedAsyncState);
				try
				{
					<Module>.cli_EcPoolWaitForNotificationsAsyncEx((_RPC_ASYNC_STATE*)ecPoolWaitForNotificationsAsyncResult.NativeState(), contextHandle.ToPointer(), (uint*)(ecPoolWaitForNotificationsAsyncResult.NativeState() + 112L / (long)sizeof(EcPoolWaitForNotificationsAsyncState)), (byte**)(ecPoolWaitForNotificationsAsyncResult.NativeState() + 120L / (long)sizeof(EcPoolWaitForNotificationsAsyncState)));
					result = ecPoolWaitForNotificationsAsyncResult;
					ecPoolWaitForNotificationsAsyncResult = null;
					flag = (((!flag2) ? 1 : 0) != 0);
				}
				catch when (endfilter(true))
				{
					num = Marshal.GetExceptionCode();
					<Module>.Microsoft.Exchange.Rpc.ThrowRpcExceptionWithEEInfo(num, "EcPoolWaitForNotificationsAsync");
				}
			}
			finally
			{
				if (null != ecPoolWaitForNotificationsAsyncResult)
				{
					((IDisposable)ecPoolWaitForNotificationsAsyncResult).Dispose();
				}
				if (flag)
				{
					value.Free();
				}
			}
			return result;
		}

		public int EndEcPoolWaitForNotificationsAsync(IRpcAsyncResult asyncResult, out uint[] sessionHandles)
		{
			if (null == asyncResult)
			{
				throw new ArgumentNullException("asyncResult");
			}
			EcPoolWaitForNotificationsAsyncResult ecPoolWaitForNotificationsAsyncResult = asyncResult as EcPoolWaitForNotificationsAsyncResult;
			if (null == ecPoolWaitForNotificationsAsyncResult)
			{
				throw new ArgumentException("Invalid type.", "asyncResult");
			}
			int result;
			try
			{
				ecPoolWaitForNotificationsAsyncResult.AsyncWaitHandle.WaitOne();
				result = ecPoolWaitForNotificationsAsyncResult.Complete(out sessionHandles);
			}
			finally
			{
				if (ecPoolWaitForNotificationsAsyncResult != null)
				{
					((IDisposable)ecPoolWaitForNotificationsAsyncResult).Dispose();
				}
			}
			return result;
		}

		[HandleProcessCorruptedStateExceptions]
		private unsafe IRpcAsyncResult InternalBeginEcPoolConnect(uint flags, Guid poolGuid, int cbAuxIn, byte* pbAuxIn, AsyncCallback callback, object state)
		{
			EcPoolConnectAsyncResult ecPoolConnectAsyncResult = null;
			EcPoolConnectAsyncResult result = null;
			GCHandle value = default(GCHandle);
			bool flag = false;
			$ArrayType$$$BY0BI@E $ArrayType$$$BY0BI@E = 0;
			initblk(ref $ArrayType$$$BY0BI@E + 1, 0, 23L);
			try
			{
				ecPoolConnectAsyncResult = new EcPoolConnectAsyncResult(callback, state);
				value = GCHandle.Alloc(ecPoolConnectAsyncResult);
				flag = true;
				int num = <Module>.RpcAsyncInitializeHandle((_RPC_ASYNC_STATE*)ecPoolConnectAsyncResult.NativeState(), 112U);
				if (num != null)
				{
					<Module>.Microsoft.Exchange.Rpc.ThrowRpcExceptionWithEEInfo(num, "EcPoolConnect, RpcAsyncInitializeHandle");
				}
				*(long*)(ecPoolConnectAsyncResult.NativeState() + 24L / (long)sizeof(EcPoolConnectAsyncState)) = 0L;
				*(int*)(ecPoolConnectAsyncResult.NativeState() + 44L / (long)sizeof(EcPoolConnectAsyncState)) = 1;
				IntPtr handle = ecPoolConnectAsyncResult.AsyncWaitHandle.Handle;
				*(long*)(ecPoolConnectAsyncResult.NativeState() + 48L / (long)sizeof(EcPoolConnectAsyncState)) = handle.ToPointer();
				IntPtr rootedAsyncState = GCHandle.ToIntPtr(value);
				bool flag2 = ecPoolConnectAsyncResult.RegisterWait(rootedAsyncState);
				$ArrayType$$$BY0BI@E = 20;
				*(ref $ArrayType$$$BY0BI@E + 2) = 1;
				*(ref $ArrayType$$$BY0BI@E + 4) = (int)flags;
				_GUID guid = <Module>.ToGUID(ref poolGuid);
				cpblk(ref $ArrayType$$$BY0BI@E + 8, ref guid, 16);
				try
				{
					<Module>.cli_EcPoolConnect((_RPC_ASYNC_STATE*)ecPoolConnectAsyncResult.NativeState(), base.BindingHandle, (void**)(ecPoolConnectAsyncResult.NativeState() + 112L / (long)sizeof(EcPoolConnectAsyncState)), 24, ref $ArrayType$$$BY0BI@E, (uint*)(ecPoolConnectAsyncResult.NativeState() + 120L / (long)sizeof(EcPoolConnectAsyncState)), (byte**)(ecPoolConnectAsyncResult.NativeState() + 128L / (long)sizeof(EcPoolConnectAsyncState)), cbAuxIn, pbAuxIn, (uint*)(ecPoolConnectAsyncResult.NativeState() + 136L / (long)sizeof(EcPoolConnectAsyncState)), (byte**)(ecPoolConnectAsyncResult.NativeState() + 144L / (long)sizeof(EcPoolConnectAsyncState)));
					result = ecPoolConnectAsyncResult;
					ecPoolConnectAsyncResult = null;
					flag = (((!flag2) ? 1 : 0) != 0);
				}
				catch when (endfilter(true))
				{
					num = Marshal.GetExceptionCode();
					<Module>.Microsoft.Exchange.Rpc.ThrowRpcExceptionWithEEInfo(num, "EcPoolConnect");
				}
			}
			finally
			{
				if (null != ecPoolConnectAsyncResult)
				{
					((IDisposable)ecPoolConnectAsyncResult).Dispose();
				}
				if (flag)
				{
					value.Free();
				}
			}
			return result;
		}

		[HandleProcessCorruptedStateExceptions]
		private unsafe IRpcAsyncResult InternalBeginEcPoolCreateSession(IntPtr contextHandle, byte[] sessionSecurityContext, uint flags, string userDn, uint connectionMode, uint codePageId, uint localeIdString, uint localeIdSort, short[] clientVersion, int cbAuxIn, byte* pbAuxIn, AsyncCallback callback, object state)
		{
			EcPoolCreateSessionAsyncResult ecPoolCreateSessionAsyncResult = null;
			EcPoolCreateSessionAsyncResult result = null;
			byte* ptr = null;
			int num = 0;
			IntPtr intPtr = IntPtr.Zero;
			sbyte* ptr2 = null;
			int num2 = 0;
			tagRPC_BLOCK_HEADER tagRPC_BLOCK_HEADER = 0;
			initblk(ref tagRPC_BLOCK_HEADER + 2, 0, 2L);
			tagRPC_POOL_CREATE_SESSION_REQUEST_HEADER tagRPC_POOL_CREATE_SESSION_REQUEST_HEADER = 0;
			initblk(ref tagRPC_POOL_CREATE_SESSION_REQUEST_HEADER + 4, 0, 36L);
			GCHandle value = default(GCHandle);
			bool flag = false;
			if (sessionSecurityContext != null && sessionSecurityContext.Length > 65535)
			{
				throw new ArgumentOutOfRangeException("sessionSecurityContext");
			}
			try
			{
				ecPoolCreateSessionAsyncResult = new EcPoolCreateSessionAsyncResult(callback, state);
				value = GCHandle.Alloc(ecPoolCreateSessionAsyncResult);
				flag = true;
				int num3 = <Module>.RpcAsyncInitializeHandle((_RPC_ASYNC_STATE*)ecPoolCreateSessionAsyncResult.NativeState(), 112U);
				if (num3 != null)
				{
					<Module>.Microsoft.Exchange.Rpc.ThrowRpcExceptionWithEEInfo(num3, "EcPoolCreateSession, RpcAsyncInitializeHandle");
				}
				*(long*)(ecPoolCreateSessionAsyncResult.NativeState() + 24L / (long)sizeof(EcPoolCreateSessionAsyncState)) = 0L;
				*(int*)(ecPoolCreateSessionAsyncResult.NativeState() + 44L / (long)sizeof(EcPoolCreateSessionAsyncState)) = 1;
				IntPtr handle = ecPoolCreateSessionAsyncResult.AsyncWaitHandle.Handle;
				*(long*)(ecPoolCreateSessionAsyncResult.NativeState() + 48L / (long)sizeof(EcPoolCreateSessionAsyncState)) = handle.ToPointer();
				tagRPC_POOL_CREATE_SESSION_REQUEST_HEADER = flags;
				*(ref tagRPC_POOL_CREATE_SESSION_REQUEST_HEADER + 4) = (int)connectionMode;
				*(ref tagRPC_POOL_CREATE_SESSION_REQUEST_HEADER + 12) = (int)codePageId;
				*(ref tagRPC_POOL_CREATE_SESSION_REQUEST_HEADER + 16) = (int)localeIdString;
				*(ref tagRPC_POOL_CREATE_SESSION_REQUEST_HEADER + 20) = (int)localeIdSort;
				*(ref tagRPC_POOL_CREATE_SESSION_REQUEST_HEADER + 24) = -1;
				*(ref tagRPC_POOL_CREATE_SESSION_REQUEST_HEADER + 32) = 1;
				if (clientVersion != null)
				{
					if (clientVersion.Length != 3)
					{
						throw new ArgumentException("Invalid version.", "clientVersion");
					}
					*(ref tagRPC_POOL_CREATE_SESSION_REQUEST_HEADER + 34) = (short)((ushort)clientVersion[0]);
					*(ref tagRPC_POOL_CREATE_SESSION_REQUEST_HEADER + 36) = (short)((ushort)clientVersion[1]);
					*(ref tagRPC_POOL_CREATE_SESSION_REQUEST_HEADER + 38) = (short)((ushort)clientVersion[2]);
				}
				intPtr = Marshal.StringToHGlobalAnsi(userDn);
				ptr2 = (sbyte*)intPtr.ToPointer();
				int num4 = <Module>.EcPackData<char\u0020*>(null, 0, &num2, ref ptr2);
				if (0 == num4)
				{
					num = (int)(num2 + 44L);
					ulong num5 = (ulong)((long)num);
					tagRPC_BLOCK_HEADER = (uint)(num5 - 4UL);
					*(ref tagRPC_BLOCK_HEADER + 2) = 3;
					if (sessionSecurityContext != null)
					{
						num = (int)(num5 + (ulong)((long)sessionSecurityContext.Length) + 4UL);
					}
					ptr = <Module>.MIDL_user_allocate((ulong)((long)num));
					if (null == ptr)
					{
						throw new OutOfMemoryException();
					}
					num2 = 0;
					num4 = <Module>.EcPackData<struct\u0020tagRPC_BLOCK_HEADER>(ptr, num, &num2, ref tagRPC_BLOCK_HEADER);
					if (0 == num4)
					{
						num4 = <Module>.EcPackData<struct\u0020tagRPC_POOL_CREATE_SESSION_REQUEST_HEADER>(ptr, num, &num2, ref tagRPC_POOL_CREATE_SESSION_REQUEST_HEADER);
						if (0 == num4)
						{
							num4 = <Module>.EcPackData<char\u0020*>(ptr, num, &num2, ref ptr2);
							if (0 == num4 && sessionSecurityContext != null)
							{
								tagRPC_BLOCK_HEADER = sessionSecurityContext.Length;
								*(ref tagRPC_BLOCK_HEADER + 2) = 6;
								num4 = <Module>.EcPackData<struct\u0020tagRPC_BLOCK_HEADER>(ptr, num, &num2, ref tagRPC_BLOCK_HEADER);
								if (0 == num4)
								{
									if (num - num2 < sessionSecurityContext.Length)
									{
										num4 = 1149;
										goto IL_219;
									}
									IntPtr destination = new IntPtr((void*)((byte*)num2 + ptr));
									Marshal.Copy(sessionSecurityContext, 0, destination, sessionSecurityContext.Length);
									num2 += sessionSecurityContext.Length;
								}
							}
						}
					}
				}
				if (num4 == null)
				{
					goto IL_224;
				}
				IL_219:
				RpcClientBase.ThrowRpcException(num4, "EcPoolCreateSession, EcPackData");
				IL_224:
				IntPtr rootedAsyncState = GCHandle.ToIntPtr(value);
				bool flag2 = ecPoolCreateSessionAsyncResult.RegisterWait(rootedAsyncState);
				try
				{
					<Module>.cli_EcPoolCreateSession((_RPC_ASYNC_STATE*)ecPoolCreateSessionAsyncResult.NativeState(), contextHandle.ToPointer(), num, ptr, (uint*)(ecPoolCreateSessionAsyncResult.NativeState() + 112L / (long)sizeof(EcPoolCreateSessionAsyncState)), (byte**)(ecPoolCreateSessionAsyncResult.NativeState() + 120L / (long)sizeof(EcPoolCreateSessionAsyncState)), cbAuxIn, pbAuxIn, (uint*)(ecPoolCreateSessionAsyncResult.NativeState() + 128L / (long)sizeof(EcPoolCreateSessionAsyncState)), (byte**)(ecPoolCreateSessionAsyncResult.NativeState() + 136L / (long)sizeof(EcPoolCreateSessionAsyncState)));
					result = ecPoolCreateSessionAsyncResult;
					ecPoolCreateSessionAsyncResult = null;
					flag = (((!flag2) ? 1 : 0) != 0);
				}
				catch when (endfilter(true))
				{
					num3 = Marshal.GetExceptionCode();
					<Module>.Microsoft.Exchange.Rpc.ThrowRpcExceptionWithEEInfo(num3, "EcPoolCreateSession");
				}
			}
			finally
			{
				if (IntPtr.Zero != intPtr)
				{
					Marshal.FreeHGlobal(intPtr);
				}
				if (ptr != null)
				{
					<Module>.MIDL_user_free((void*)ptr);
				}
				if (null != ecPoolCreateSessionAsyncResult)
				{
					((IDisposable)ecPoolCreateSessionAsyncResult).Dispose();
				}
				if (flag)
				{
					value.Free();
				}
			}
			return result;
		}

		[HandleProcessCorruptedStateExceptions]
		private unsafe IRpcAsyncResult InternalBeginEcPoolSessionDoRpc(IntPtr contextHandle, uint sessionHandle, uint flags, uint maximumResponseSize, int cbIn, byte* pbIn, int cbAuxIn, byte* pbAuxIn, AsyncCallback callback, object state)
		{
			EcPoolSessionDoRpcAsyncResult ecPoolSessionDoRpcAsyncResult = null;
			EcPoolSessionDoRpcAsyncResult result = null;
			GCHandle value = default(GCHandle);
			bool flag = false;
			try
			{
				ecPoolSessionDoRpcAsyncResult = new EcPoolSessionDoRpcAsyncResult(callback, state);
				value = GCHandle.Alloc(ecPoolSessionDoRpcAsyncResult);
				flag = true;
				int num = <Module>.RpcAsyncInitializeHandle((_RPC_ASYNC_STATE*)ecPoolSessionDoRpcAsyncResult.NativeState(), 112U);
				if (num != null)
				{
					<Module>.Microsoft.Exchange.Rpc.ThrowRpcExceptionWithEEInfo(num, "EcPoolSessionDoRpc, RpcAsyncInitializeHandle");
				}
				*(long*)(ecPoolSessionDoRpcAsyncResult.NativeState() + 24L / (long)sizeof(EcPoolSessionDoRpcAsyncState)) = 0L;
				*(int*)(ecPoolSessionDoRpcAsyncResult.NativeState() + 44L / (long)sizeof(EcPoolSessionDoRpcAsyncState)) = 1;
				IntPtr handle = ecPoolSessionDoRpcAsyncResult.AsyncWaitHandle.Handle;
				*(long*)(ecPoolSessionDoRpcAsyncResult.NativeState() + 48L / (long)sizeof(EcPoolSessionDoRpcAsyncState)) = handle.ToPointer();
				*(int*)(ecPoolSessionDoRpcAsyncResult.NativeState() + 112L / (long)sizeof(EcPoolSessionDoRpcAsyncState)) = (int)flags;
				*(int*)(ecPoolSessionDoRpcAsyncResult.NativeState() + 116L / (long)sizeof(EcPoolSessionDoRpcAsyncState)) = (int)maximumResponseSize;
				IntPtr rootedAsyncState = GCHandle.ToIntPtr(value);
				bool flag2 = ecPoolSessionDoRpcAsyncResult.RegisterWait(rootedAsyncState);
				try
				{
					<Module>.cli_EcPoolSessionDoRpc((_RPC_ASYNC_STATE*)ecPoolSessionDoRpcAsyncResult.NativeState(), contextHandle.ToPointer(), sessionHandle, (uint*)(ecPoolSessionDoRpcAsyncResult.NativeState() + 112L / (long)sizeof(EcPoolSessionDoRpcAsyncState)), cbIn, pbIn, (uint*)(ecPoolSessionDoRpcAsyncResult.NativeState() + 116L / (long)sizeof(EcPoolSessionDoRpcAsyncState)), (byte**)(ecPoolSessionDoRpcAsyncResult.NativeState() + 120L / (long)sizeof(EcPoolSessionDoRpcAsyncState)), cbAuxIn, pbAuxIn, (uint*)(ecPoolSessionDoRpcAsyncResult.NativeState() + 128L / (long)sizeof(EcPoolSessionDoRpcAsyncState)), (byte**)(ecPoolSessionDoRpcAsyncResult.NativeState() + 136L / (long)sizeof(EcPoolSessionDoRpcAsyncState)));
					result = ecPoolSessionDoRpcAsyncResult;
					ecPoolSessionDoRpcAsyncResult = null;
					flag = (((!flag2) ? 1 : 0) != 0);
				}
				catch when (endfilter(true))
				{
					num = Marshal.GetExceptionCode();
					<Module>.Microsoft.Exchange.Rpc.ThrowRpcExceptionWithEEInfo(num, "EcPoolSessionDoRpc");
				}
			}
			finally
			{
				if (null != ecPoolSessionDoRpcAsyncResult)
				{
					((IDisposable)ecPoolSessionDoRpcAsyncResult).Dispose();
				}
				if (flag)
				{
					value.Free();
				}
			}
			return result;
		}
	}
}
