using System;
using System.Collections.Generic;
using System.Net;
using System.Runtime.ExceptionServices;
using System.Runtime.InteropServices;
using <CppImplementationDetails>;

namespace Microsoft.Exchange.Rpc.ExchangeClient
{
	internal class ExchangeRpcClient : RpcClientBase
	{
		[HandleProcessCorruptedStateExceptions]
		private unsafe int EcDoConnect_Wrapped([MarshalAs(UnmanagedType.U1)] bool isEx, void** pcxh, byte* szUserDN, uint ulFlags, uint ulConMod, uint cbLimit, uint ulCpid, uint ulLcidString, uint ulLcidSort, uint ulIcxrLink, ushort usFCanConvertCodePages, uint* pcmsPollsMax, uint* pcRetry, uint* pcmsRetryDelay, ushort* picxr, byte** pszDNPrefix, byte** pszDisplayName, ushort* rgwClientVersion, ushort* rgwServerVersion, ushort* rgwBestVersion, uint* pulTimeStamp, byte* rgbAuxIn, uint cbAuxIn, byte* rgbAuxOut, uint* pcbAuxOut)
		{
			int result = 0;
			base.ResetRetryCounter();
			for (;;)
			{
				if (!this.fIsHTTP)
				{
					<Module>.RpcBindingReset(base.BindingHandle);
				}
				DateTime utcNow = DateTime.UtcNow;
				try
				{
					if (isEx)
					{
						result = <Module>.cli_EcDoConnectEx(base.BindingHandle, pcxh, szUserDN, ulFlags, ulConMod, cbLimit, ulCpid, ulLcidString, ulLcidSort, ulIcxrLink, usFCanConvertCodePages, pcmsPollsMax, pcRetry, pcmsRetryDelay, picxr, pszDNPrefix, pszDisplayName, rgwClientVersion, rgwServerVersion, rgwBestVersion, pulTimeStamp, rgbAuxIn, cbAuxIn, rgbAuxOut, pcbAuxOut);
					}
					else
					{
						result = <Module>.cli_EcDoConnect(base.BindingHandle, pcxh, szUserDN, ulFlags, ulConMod, cbLimit, ulCpid, ulLcidString, ulLcidSort, ulIcxrLink, usFCanConvertCodePages, pcmsPollsMax, pcRetry, pcmsRetryDelay, picxr, pszDNPrefix, pszDisplayName, rgwClientVersion, rgwServerVersion, rgwBestVersion, pulTimeStamp);
					}
					this.totalRpcCounter++;
					DateTime utcNow2 = DateTime.UtcNow;
					TimeSpan ts = utcNow2 - utcNow;
					TimeSpan timeSpan = this.totalRpcTime.Add(ts);
					this.totalRpcTime = timeSpan;
				}
				catch when (endfilter(true))
				{
					this.totalRpcCounter++;
					TimeSpan ts2 = DateTime.UtcNow - utcNow;
					TimeSpan timeSpan2 = this.totalRpcTime.Add(ts2);
					this.totalRpcTime = timeSpan2;
					int exceptionCode = Marshal.GetExceptionCode();
					if (this.allowRpcRetry && base.RetryRpcCall(exceptionCode, RpcRetryType.CallCancelled | RpcRetryType.ServerBusy | RpcRetryType.ServerUnavailable | RpcRetryType.AccessDenied) != 0)
					{
						continue;
					}
					string routineName;
					if (isEx)
					{
						routineName = "EcDoConnectEx";
					}
					else
					{
						routineName = "EcDoConnect";
					}
					<Module>.Microsoft.Exchange.Rpc.ThrowRpcExceptionWithEEInfo(exceptionCode, routineName);
				}
				break;
			}
			return result;
		}

		[HandleProcessCorruptedStateExceptions]
		private unsafe int EcDoDisconnect_Wrapped(void** pcxh)
		{
			int result = 0;
			base.ResetRetryCounter();
			for (;;)
			{
				DateTime utcNow = DateTime.UtcNow;
				try
				{
					result = <Module>.cli_EcDoDisconnect(pcxh);
					this.totalRpcCounter++;
					DateTime utcNow2 = DateTime.UtcNow;
					TimeSpan ts = utcNow2 - utcNow;
					TimeSpan timeSpan = this.totalRpcTime.Add(ts);
					this.totalRpcTime = timeSpan;
				}
				catch when (endfilter(true))
				{
					this.totalRpcCounter++;
					TimeSpan ts2 = DateTime.UtcNow - utcNow;
					TimeSpan timeSpan2 = this.totalRpcTime.Add(ts2);
					this.totalRpcTime = timeSpan2;
					int exceptionCode = Marshal.GetExceptionCode();
					if (this.allowRpcRetry && base.RetryRpcCall(exceptionCode, RpcRetryType.ServerBusy) != 0)
					{
						continue;
					}
					<Module>.Microsoft.Exchange.Rpc.ThrowRpcExceptionWithEEInfo(exceptionCode, "EcDoDisconnect");
				}
				break;
			}
			return result;
		}

		[HandleProcessCorruptedStateExceptions]
		private int EcDoDummy_Wrapped()
		{
			int result = 0;
			base.ResetRetryCounter();
			for (;;)
			{
				<Module>.RpcBindingReset(base.BindingHandle);
				DateTime utcNow = DateTime.UtcNow;
				try
				{
					result = <Module>.cli_EcDummyRpc(base.BindingHandle);
					this.totalRpcCounter++;
					DateTime utcNow2 = DateTime.UtcNow;
					TimeSpan ts = utcNow2 - utcNow;
					TimeSpan timeSpan = this.totalRpcTime.Add(ts);
					this.totalRpcTime = timeSpan;
				}
				catch when (endfilter(true))
				{
					this.totalRpcCounter++;
					TimeSpan ts2 = DateTime.UtcNow - utcNow;
					TimeSpan timeSpan2 = this.totalRpcTime.Add(ts2);
					this.totalRpcTime = timeSpan2;
					int exceptionCode = Marshal.GetExceptionCode();
					if (this.allowRpcRetry && base.RetryRpcCall(exceptionCode, RpcRetryType.ServerBusy) != 0)
					{
						continue;
					}
					<Module>.Microsoft.Exchange.Rpc.ThrowRpcExceptionWithEEInfo(exceptionCode, "EcDoDummy");
				}
				break;
			}
			return result;
		}

		[HandleProcessCorruptedStateExceptions]
		private unsafe int EcDoRpc_Wrapped(void** pcxh, byte* rgb, ushort* pusLength, ushort usSize)
		{
			int result = 0;
			base.ResetRetryCounter();
			for (;;)
			{
				DateTime utcNow = DateTime.UtcNow;
				try
				{
					result = <Module>.cli_EcDoRpc(pcxh, rgb, pusLength, usSize);
					this.totalRpcCounter++;
					DateTime utcNow2 = DateTime.UtcNow;
					TimeSpan ts = utcNow2 - utcNow;
					TimeSpan timeSpan = this.totalRpcTime.Add(ts);
					this.totalRpcTime = timeSpan;
				}
				catch when (endfilter(true))
				{
					this.totalRpcCounter++;
					TimeSpan ts2 = DateTime.UtcNow - utcNow;
					TimeSpan timeSpan2 = this.totalRpcTime.Add(ts2);
					this.totalRpcTime = timeSpan2;
					int exceptionCode = Marshal.GetExceptionCode();
					if (this.allowRpcRetry && base.RetryRpcCall(exceptionCode, RpcRetryType.ServerBusy) != 0)
					{
						continue;
					}
					<Module>.Microsoft.Exchange.Rpc.ThrowRpcExceptionWithEEInfo(exceptionCode, "EcDoRpc");
				}
				break;
			}
			return result;
		}

		[HandleProcessCorruptedStateExceptions]
		private unsafe int EcDoRpcExt2_Wrapped(void** pcxh, uint* pulFlags, byte* rgbIn, uint cbIn, byte* rgbOut, uint* pcbOut, byte* rgbAuxIn, uint cbAuxIn, byte* rgbAuxOut, uint* pcbAuxOut, uint* pulTransTime)
		{
			int result = 0;
			base.ResetRetryCounter();
			for (;;)
			{
				DateTime utcNow = DateTime.UtcNow;
				try
				{
					result = <Module>.cli_EcDoRpcExt2(pcxh, pulFlags, rgbIn, cbIn, rgbOut, pcbOut, rgbAuxIn, cbAuxIn, rgbAuxOut, pcbAuxOut, pulTransTime);
					this.totalRpcCounter++;
					DateTime utcNow2 = DateTime.UtcNow;
					TimeSpan ts = utcNow2 - utcNow;
					TimeSpan timeSpan = this.totalRpcTime.Add(ts);
					this.totalRpcTime = timeSpan;
				}
				catch when (endfilter(true))
				{
					this.totalRpcCounter++;
					TimeSpan ts2 = DateTime.UtcNow - utcNow;
					TimeSpan timeSpan2 = this.totalRpcTime.Add(ts2);
					this.totalRpcTime = timeSpan2;
					int exceptionCode = Marshal.GetExceptionCode();
					if (this.allowRpcRetry && base.RetryRpcCall(exceptionCode, RpcRetryType.ServerBusy) != 0)
					{
						continue;
					}
					<Module>.Microsoft.Exchange.Rpc.ThrowRpcExceptionWithEEInfo(exceptionCode, "EcDoRpcExt2");
				}
				break;
			}
			return result;
		}

		[HandleProcessCorruptedStateExceptions]
		[return: MarshalAs(UnmanagedType.U1)]
		private unsafe bool DoAsyncWaitExHelper_Guarded(method doAsyncWaitEx, _RPC_ASYNC_STATE* pasyncState, void* acxh, uint ulFlagsIn, uint* pulFlagsOut)
		{
			try
			{
				calli(System.Void modopt(System.Runtime.CompilerServices.CallConvCdecl)(_RPC_ASYNC_STATE*,System.Void*,System.UInt32 modopt(System.Runtime.CompilerServices.IsLong),System.UInt32 modopt(System.Runtime.CompilerServices.IsLong)*), pasyncState, acxh, ulFlagsIn, pulFlagsOut, doAsyncWaitEx);
			}
			catch when (endfilter(true))
			{
				int exceptionCode = Marshal.GetExceptionCode();
				if (this.allowRpcRetry && base.RetryRpcCall(exceptionCode, RpcRetryType.ServerBusy) != 0)
				{
					return true;
				}
				<Module>.Microsoft.Exchange.Rpc.ThrowRpcExceptionWithEEInfo(exceptionCode, "EcDoAsyncWait(Ex)");
			}
			return false;
		}

		private unsafe int EcDoAsyncWaitExHelper(method doAsyncWaitEx, void* acxh, uint ulFlagsIn, uint* pulFlagsOut)
		{
			int result = 0;
			int num;
			do
			{
				_RPC_ASYNC_STATE rpc_ASYNC_STATE;
				num = <Module>.RpcAsyncInitializeHandle(&rpc_ASYNC_STATE, 112U);
				if (num != null)
				{
					<Module>.Microsoft.Exchange.Rpc.ThrowRpcExceptionWithEEInfo(num, "EcDoAsyncWait(Ex), RpcAsyncInitializeHandle");
				}
				*(ref rpc_ASYNC_STATE + 24) = 0L;
				*(ref rpc_ASYNC_STATE + 44) = 1;
				try
				{
					*(ref rpc_ASYNC_STATE + 48) = <Module>.CreateEventW(null, 0, 0, null);
					if (*(ref rpc_ASYNC_STATE + 48) == 0L)
					{
						throw new Exception(string.Format("Unable to create Event, error: {0}", <Module>.GetLastError()));
					}
					if (this.DoAsyncWaitExHelper_Guarded(doAsyncWaitEx, &rpc_ASYNC_STATE, acxh, ulFlagsIn, pulFlagsOut))
					{
						continue;
					}
					if (<Module>.WaitForSingleObject(*(ref rpc_ASYNC_STATE + 48), -1) == -1)
					{
						throw new Exception(string.Format("Unable to wait for event, error: {0}", <Module>.GetLastError()));
					}
				}
				finally
				{
					if (*(ref rpc_ASYNC_STATE + 48) != 0L)
					{
						<Module>.CloseHandle(*(ref rpc_ASYNC_STATE + 48));
					}
				}
				num = <Module>.RpcAsyncCompleteCall(&rpc_ASYNC_STATE, (void*)(&result));
				if (num == null)
				{
					return result;
				}
			}
			while (this.allowRpcRetry && base.RetryRpcCall(num, RpcRetryType.ServerBusy) != 0);
			<Module>.Microsoft.Exchange.Rpc.ThrowRpcExceptionWithEEInfo(num, "EcDoAsyncWait(Ex), RpcAsyncCompleteCall");
			return result;
		}

		[HandleProcessCorruptedStateExceptions]
		private unsafe int EcDoAsyncConnect_Wrapped([MarshalAs(UnmanagedType.U1)] bool isEx, void* cxh, void** pacxh)
		{
			int result = 0;
			base.ResetRetryCounter();
			for (;;)
			{
				DateTime utcNow = DateTime.UtcNow;
				try
				{
					if (isEx)
					{
						result = <Module>.cli_EcDoAsyncConnectEx(cxh, pacxh);
					}
					else
					{
						result = <Module>.cli_EcDoAsyncConnect(cxh, pacxh);
					}
					this.totalRpcCounter++;
					DateTime utcNow2 = DateTime.UtcNow;
					TimeSpan ts = utcNow2 - utcNow;
					TimeSpan timeSpan = this.totalRpcTime.Add(ts);
					this.totalRpcTime = timeSpan;
				}
				catch when (endfilter(true))
				{
					this.totalRpcCounter++;
					TimeSpan ts2 = DateTime.UtcNow - utcNow;
					TimeSpan timeSpan2 = this.totalRpcTime.Add(ts2);
					this.totalRpcTime = timeSpan2;
					int exceptionCode = Marshal.GetExceptionCode();
					if (this.allowRpcRetry && base.RetryRpcCall(exceptionCode, RpcRetryType.ServerBusy) != 0)
					{
						continue;
					}
					<Module>.Microsoft.Exchange.Rpc.ThrowRpcExceptionWithEEInfo(exceptionCode, "EcDoAsyncConnect(Ex)");
				}
				break;
			}
			return result;
		}

		[HandleProcessCorruptedStateExceptions]
		private unsafe int EcDoAsyncWait_Wrapped([MarshalAs(UnmanagedType.U1)] bool isEx, void* acxh, uint ulFlagsIn, uint* pulFlagsOut)
		{
			int result = 0;
			base.ResetRetryCounter();
			while (!isEx)
			{
				try
				{
					result = <Module>.cli_EcDoAsyncWait(acxh, ulFlagsIn, pulFlagsOut);
				}
				catch when (endfilter(true))
				{
					int exceptionCode = Marshal.GetExceptionCode();
					if (this.allowRpcRetry && base.RetryRpcCall(exceptionCode, RpcRetryType.ServerBusy) != 0)
					{
						continue;
					}
					<Module>.Microsoft.Exchange.Rpc.ThrowRpcExceptionWithEEInfo(exceptionCode, "EcDoAsyncWait(Ex)");
				}
				return result;
			}
			return this.EcDoAsyncWaitExHelper(<Module>.__unep@?cli_EcDoAsyncWaitEx@@$$J0YAXPEAU_RPC_ASYNC_STATE@@PEAXKPEAK@Z, acxh, ulFlagsIn, pulFlagsOut);
		}

		private unsafe int EcDoConnect_Internal([MarshalAs(UnmanagedType.U1)] bool isEx, out IntPtr contextHandle, string userDn, int flags, int sizeLimit, int conMod, int cpid, int lcidString, int lcidSort, int sessionIdLink, [MarshalAs(UnmanagedType.U1)] bool canConvertCodePages, out TimeSpan pollsMax, out int retryCount, out TimeSpan retryDelay, out string dnPrefix, out string displayName, short[] clientVersion, out short[] serverVersion, out short[] bestVersion, ref int timeStamp, byte[] auxIn, [MarshalAs(UnmanagedType.U1)] bool compressAuxIn, [MarshalAs(UnmanagedType.U1)] bool xorMagicAuxIn, out byte[] auxOut, int maxSizeAuxOut, out bool wasCompressedAuxOut, out bool wasXorMagicAuxOut)
		{
			int result = 0;
			uint num = 0;
			uint num2 = 0;
			uint num3 = 0;
			ushort num4 = 0;
			byte* ptr = null;
			byte* ptr2 = null;
			$ArrayType$$$BY02G $ArrayType$$$BY02G = 0;
			initblk(ref $ArrayType$$$BY02G + 2, 0, 4L);
			$ArrayType$$$BY02G $ArrayType$$$BY02G2 = 0;
			initblk(ref $ArrayType$$$BY02G2 + 2, 0, 4L);
			byte* ptr3 = null;
			byte* ptr4 = null;
			ushort usFCanConvertCodePages = canConvertCodePages ? 1 : 0;
			uint cbAuxIn = 0;
			byte* ptr5 = null;
			uint num5 = maxSizeAuxOut;
			void* value = (void*)contextHandle;
			uint num6 = timeStamp;
			dnPrefix = null;
			displayName = null;
			auxOut = null;
			wasCompressedAuxOut = false;
			wasXorMagicAuxOut = false;
			if (clientVersion.Length != 3)
			{
				throw new ArgumentException("clientVersion must be 3 bytes in size");
			}
			try
			{
				byte* ptr6;
				if (userDn != null)
				{
					ptr6 = (byte*)<Module>.StringToUnmanagedMultiByte(userDn, 0U);
				}
				else
				{
					ptr6 = null;
				}
				ptr4 = ptr6;
				$ArrayType$$$BY02G $ArrayType$$$BY02G3 = (ushort)clientVersion[0];
				*(ref $ArrayType$$$BY02G3 + 2) = (short)((ushort)clientVersion[1]);
				*(ref $ArrayType$$$BY02G3 + 4) = (short)((ushort)clientVersion[2]);
				if (isEx)
				{
					int size;
					if (auxIn != null)
					{
						size = auxIn.Length;
					}
					else
					{
						size = 0;
					}
					ptr5 = <Module>.PackExtBuffer(auxIn, 0, size, compressAuxIn, xorMagicAuxIn, &cbAuxIn, null, 0, true);
					ptr3 = <Module>.MIDL_user_allocate(num5);
				}
				result = this.EcDoConnect_Wrapped(isEx, &value, ptr6, flags, conMod, sizeLimit, cpid, lcidString, lcidSort, sessionIdLink, usFCanConvertCodePages, &num, &num2, &num3, &num4, &ptr, &ptr2, ref $ArrayType$$$BY02G3, ref $ArrayType$$$BY02G, ref $ArrayType$$$BY02G2, &num6, ptr5, cbAuxIn, ptr3, &num5);
				IntPtr intPtr = (IntPtr)value;
				contextHandle = intPtr;
				timeStamp = num6;
				sessionIdLink = (int)num4;
				string text;
				if (ptr != null)
				{
					IntPtr ptr7 = new IntPtr((void*)ptr);
					text = Marshal.PtrToStringAnsi(ptr7);
				}
				else
				{
					text = null;
				}
				dnPrefix = text;
				string text2;
				if (ptr2 != null)
				{
					IntPtr ptr8 = new IntPtr((void*)ptr2);
					text2 = Marshal.PtrToStringAnsi(ptr8);
				}
				else
				{
					text2 = null;
				}
				displayName = text2;
				TimeSpan timeSpan = TimeSpan.FromMilliseconds(num);
				pollsMax = timeSpan;
				retryCount = num2;
				TimeSpan timeSpan2 = TimeSpan.FromMilliseconds(num3);
				retryDelay = timeSpan2;
				short[] array = new short[3];
				serverVersion = array;
				array[0] = $ArrayType$$$BY02G;
				serverVersion[1] = (short)(*(ref $ArrayType$$$BY02G + 2));
				serverVersion[2] = (short)(*(ref $ArrayType$$$BY02G + 4));
				short[] array2 = new short[3];
				bestVersion = array2;
				array2[0] = $ArrayType$$$BY02G2;
				bestVersion[1] = (short)(*(ref $ArrayType$$$BY02G2 + 2));
				bestVersion[2] = (short)(*(ref $ArrayType$$$BY02G2 + 4));
				if (isEx && num5 > 0)
				{
					auxOut = <Module>.UnpackExtBuffer(ptr3, num5, out wasCompressedAuxOut, out wasXorMagicAuxOut, null, null);
				}
			}
			finally
			{
				if (ptr5 != null)
				{
					<Module>.MIDL_user_free((void*)ptr5);
				}
				if (ptr4 != null)
				{
					<Module>.MIDL_user_free((void*)ptr4);
				}
				if (ptr3 != null)
				{
					<Module>.MIDL_user_free((void*)ptr3);
				}
				if (ptr != null)
				{
					<Module>.MIDL_user_free((void*)ptr);
					ptr = null;
				}
				if (ptr2 != null)
				{
					<Module>.MIDL_user_free((void*)ptr2);
					ptr2 = null;
				}
			}
			return result;
		}

		private void Initialize(string protocolSequence, [MarshalAs(UnmanagedType.U1)] bool allowRpcRetry)
		{
			this.totalRpcCounter = 0;
			this.totalRpcTime = TimeSpan.Zero;
			this.allowRpcRetry = allowRpcRetry;
			if (string.Equals(protocolSequence, "ncacn_http", StringComparison.InvariantCultureIgnoreCase))
			{
				this.fIsHTTP = true;
			}
			else
			{
				this.fIsHTTP = false;
			}
		}

		public ExchangeRpcClient(RpcBindingInfo bindingInfo) : base(bindingInfo.UseKerberosSpn("exchangeMDB", null))
		{
			try
			{
				this.Initialize(bindingInfo.ProtocolSequence, bindingInfo.AllowRpcRetry);
			}
			catch
			{
				base.Dispose(true);
				throw;
			}
		}

		public ExchangeRpcClient(string machineName, NetworkCredential nc) : base(machineName, null, null, true, nc, HttpAuthenticationScheme.Basic, AuthenticationService.Negotiate, true, false, true)
		{
			try
			{
				this.Initialize(null, true);
			}
			catch
			{
				base.Dispose(true);
				throw;
			}
		}

		public int TotalRpcCounter
		{
			get
			{
				return this.totalRpcCounter;
			}
		}

		public TimeSpan TotalRpcTime
		{
			get
			{
				return this.totalRpcTime;
			}
		}

		public int EcDoConnect(out IntPtr contextHandle, string userDn, int flags, int sizeLimit, int conMod, int cpid, int lcidString, int lcidSort, int sessionIdLink, [MarshalAs(UnmanagedType.U1)] bool canConvertCodePages, out TimeSpan pollsMax, out int retryCount, out TimeSpan retryDelay, out string dnPrefix, out string displayName, short[] clientVersion, out short[] serverVersion, out short[] bestVersion, ref int timeStamp)
		{
			byte[] array = null;
			bool flag = false;
			bool flag2 = false;
			return this.EcDoConnect_Internal(false, out contextHandle, userDn, flags, sizeLimit, conMod, cpid, lcidString, lcidSort, sessionIdLink, canConvertCodePages, out pollsMax, out retryCount, out retryDelay, out dnPrefix, out displayName, clientVersion, out serverVersion, out bestVersion, ref timeStamp, null, false, false, out array, 0, out flag, out flag2);
		}

		public unsafe int EcDoDisconnect(ref IntPtr contextHandle)
		{
			int result = 0;
			void* ptr = (void*)contextHandle;
			try
			{
				result = this.EcDoDisconnect_Wrapped(&ptr);
			}
			finally
			{
				contextHandle = IntPtr.Zero;
			}
			return result;
		}

		public int EcDoDummy()
		{
			return this.EcDoDummy_Wrapped();
		}

		public int EcDoConnectEx(out IntPtr contextHandle, string userDn, int flags, int sizeLimit, int conMod, int cpid, int lcidString, int lcidSort, int sessionIdLink, [MarshalAs(UnmanagedType.U1)] bool canConvertCodePages, out TimeSpan pollsMax, out int retryCount, out TimeSpan retryDelay, out string dnPrefix, out string displayName, short[] clientVersion, out short[] serverVersion, out short[] bestVersion, ref int timeStamp, byte[] auxIn, [MarshalAs(UnmanagedType.U1)] bool compressAuxIn, [MarshalAs(UnmanagedType.U1)] bool xorMagicAuxIn, out byte[] auxOut, int maxSizeAuxOut, out bool wasCompressedAuxOut, out bool wasXorMagicAuxOut)
		{
			return this.EcDoConnect_Internal(true, out contextHandle, userDn, flags, sizeLimit, conMod, cpid, lcidString, lcidSort, sessionIdLink, canConvertCodePages, out pollsMax, out retryCount, out retryDelay, out dnPrefix, out displayName, clientVersion, out serverVersion, out bestVersion, ref timeStamp, auxIn, compressAuxIn, xorMagicAuxIn, out auxOut, maxSizeAuxOut, out wasCompressedAuxOut, out wasXorMagicAuxOut);
		}

		public unsafe int EcDoRpc(ref IntPtr contextHandle, ref byte[] ropBuffer, int maxSizeRopBuffer)
		{
			int result = 0;
			ushort num = (ushort)maxSizeRopBuffer;
			void* value = (void*)contextHandle;
			byte* ptr = null;
			try
			{
				byte[] array = ropBuffer;
				if (array == null)
				{
					throw new ArgumentNullException("ropBuffer");
				}
				int num2 = array.Length;
				if (num2 > maxSizeRopBuffer)
				{
					throw new ArgumentException("ropBuffer is greater than maxSizeRopBuffer");
				}
				if (maxSizeRopBuffer > <Module>.g_cbMaxBufferSize)
				{
					throw new ArgumentException("maxSizeRopBuffer too large for EcDoRpc");
				}
				ushort num3 = (ushort)num2;
				ulong num4 = (ulong)num;
				ptr = <Module>.MIDL_user_allocate(num4);
				if (ptr == null)
				{
					throw new OutOfMemoryException();
				}
				initblk(ptr, 0, num4);
				IntPtr destination = new IntPtr((void*)ptr);
				Marshal.Copy(ropBuffer, 0, destination, (int)num3);
				if (num3 > 0)
				{
					<Module>.DoCompressibleXorMagic(ptr, num3);
				}
				result = this.EcDoRpc_Wrapped(&value, ptr, &num3, num);
				IntPtr intPtr = (IntPtr)value;
				contextHandle = intPtr;
				byte[] array2;
				if (num3 > 0)
				{
					<Module>.DoCompressibleXorMagic(ptr, num3);
					if (num3 > 0)
					{
						IntPtr uPtrData = new IntPtr((void*)ptr);
						array2 = <Module>.UToMBytes((int)num3, uPtrData);
						goto IL_D9;
					}
				}
				array2 = new byte[0];
				IL_D9:
				ropBuffer = array2;
			}
			finally
			{
				if (ptr != null)
				{
					<Module>.MIDL_user_free((void*)ptr);
				}
			}
			return result;
		}

		public unsafe int EcDoRpcExt2(ref IntPtr contextHandle, ref int flags, byte[][] ropIn, [MarshalAs(UnmanagedType.U1)] bool compressRopIn, [MarshalAs(UnmanagedType.U1)] bool xorMagicRopIn, out byte[][] ropOut, int maxSizeRopOut, out bool[] wasCompressedRopOut, out bool[] wasXorMagicRopOut, byte[] auxIn, [MarshalAs(UnmanagedType.U1)] bool compressAuxIn, [MarshalAs(UnmanagedType.U1)] bool xorMagicAuxIn, out byte[] auxOut, int maxSizeAuxOut, out bool wasCompressedAuxOut, out bool wasXorMagicAuxOut, out int transTime)
		{
			if (ropIn != null && ropIn.Length > 128)
			{
				throw new ArgumentException("ropIn array has too many elements");
			}
			int result = 0;
			byte* ptr = null;
			byte* ptr2 = null;
			uint num = 0;
			byte* ptr3 = null;
			uint num2 = maxSizeRopOut;
			uint cbAuxIn = 0;
			byte* ptr4 = null;
			uint num3 = maxSizeAuxOut;
			$ArrayType$$$BY0IA@K $ArrayType$$$BY0IA@K = 0;
			initblk(ref $ArrayType$$$BY0IA@K + 4, 0, 508L);
			$ArrayType$$$BY0IA@PEAE $ArrayType$$$BY0IA@PEAE = 0L;
			initblk(ref $ArrayType$$$BY0IA@PEAE + 8, 0, 1016L);
			int num4 = 0;
			void* value = (void*)contextHandle;
			uint num5 = flags;
			ropOut = null;
			wasCompressedRopOut = null;
			wasXorMagicRopOut = null;
			auxOut = null;
			wasCompressedAuxOut = false;
			wasXorMagicAuxOut = false;
			try
			{
				uint num6 = 0;
				num4 = 0;
				for (;;)
				{
					int num7 = ropIn.Length;
					if (num4 >= num7)
					{
						break;
					}
					bool last = ((num4 >= num7 - 1) ? 1 : 0) != 0;
					uint num8 = 0;
					byte[] array = ropIn[num4];
					int size;
					if (array != null)
					{
						size = array.Length;
					}
					else
					{
						size = 0;
					}
					byte* ptr5 = <Module>.PackExtBuffer(array, 0, size, compressRopIn, xorMagicRopIn, &num8, null, 0, last);
					long num9 = (long)num4;
					*(num9 * 4L + ref $ArrayType$$$BY0IA@K) = num8;
					*(num9 * 8L + ref $ArrayType$$$BY0IA@PEAE) = ptr5;
					num6 += num8;
					num4++;
				}
				ptr3 = <Module>.MIDL_user_allocate(num6);
				uint num10 = 0;
				for (int i = 0; i < num4; i++)
				{
					long num11 = (long)i;
					uint num12 = (uint)(*(num11 * 4L + ref $ArrayType$$$BY0IA@K));
					<Module>.memcpy_s((void*)((byte*)num10 + ptr3), num6 - num10, *(num11 * 8L + ref $ArrayType$$$BY0IA@PEAE), (ulong)num12);
					num10 += num12;
				}
				ptr = <Module>.MIDL_user_allocate(num2);
				int size2;
				if (auxIn != null)
				{
					size2 = auxIn.Length;
				}
				else
				{
					size2 = 0;
				}
				ptr4 = <Module>.PackExtBuffer(auxIn, 0, size2, compressAuxIn, xorMagicAuxIn, &cbAuxIn, null, 0, true);
				ptr2 = <Module>.MIDL_user_allocate(num3);
				result = this.EcDoRpcExt2_Wrapped(&value, &num5, ptr3, num10, ptr, &num2, ptr4, cbAuxIn, ptr2, &num3, &num);
				IntPtr intPtr = (IntPtr)value;
				contextHandle = intPtr;
				flags = num5;
				transTime = num;
				if (num2 > 0)
				{
					ExchangeRpcClient.ShredChainedResponseBuffer(ptr, num2, out ropOut, out wasCompressedRopOut, out wasXorMagicRopOut);
				}
				if (num3 > 0)
				{
					auxOut = <Module>.UnpackExtBuffer(ptr2, num3, out wasCompressedAuxOut, out wasXorMagicAuxOut, null, null);
				}
			}
			finally
			{
				if (num4 > 0)
				{
					long num13 = 0L;
					long num14 = (long)num4;
					if (0L < num14)
					{
						do
						{
							byte** ptr6 = num13 * 8L + ref $ArrayType$$$BY0IA@PEAE;
							ulong num15 = (ulong)(*(long*)ptr6);
							if (num15 != 0UL)
							{
								<Module>.MIDL_user_free(num15);
								*(long*)ptr6 = 0L;
							}
							num13 += 1L;
						}
						while (num13 < num14);
					}
				}
				if (ptr3 != null)
				{
					<Module>.MIDL_user_free((void*)ptr3);
				}
				if (ptr4 != null)
				{
					<Module>.MIDL_user_free((void*)ptr4);
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

		public unsafe int EcDoAsyncConnect(IntPtr contextHandle, out IntPtr asynchronousContextHandle)
		{
			void* cxh = (void*)contextHandle;
			void* value = (void*)asynchronousContextHandle;
			int result = this.EcDoAsyncConnect_Wrapped(false, cxh, &value);
			IntPtr intPtr = (IntPtr)value;
			asynchronousContextHandle = intPtr;
			return result;
		}

		public unsafe int EcDoAsyncConnectEx(IntPtr contextHandle, out IntPtr asynchronousContextHandle)
		{
			void* cxh = (void*)contextHandle;
			void* value = (void*)asynchronousContextHandle;
			int result = this.EcDoAsyncConnect_Wrapped(true, cxh, &value);
			IntPtr intPtr = (IntPtr)value;
			asynchronousContextHandle = intPtr;
			return result;
		}

		public unsafe int EcDoAsyncWait(IntPtr asynchronousContextHandle, int flags, out int resultFlags)
		{
			void* acxh = (void*)asynchronousContextHandle;
			uint num;
			int result = this.EcDoAsyncWait_Wrapped(false, acxh, flags, &num);
			resultFlags = num;
			return result;
		}

		public unsafe int EcDoAsyncWaitEx(IntPtr asynchronousContextHandle, int flags, out int resultFlags)
		{
			void* acxh = (void*)asynchronousContextHandle;
			uint num;
			int result = this.EcDoAsyncWait_Wrapped(true, acxh, flags, &num);
			resultFlags = num;
			return result;
		}

		public unsafe static void ShredChainedResponseBuffer(byte* originalRopOut, int originalRopOutSize, out byte[][] shreddedRopOut, out bool[] wasCompressedRopOut, out bool[] wasXorMagicRopOut)
		{
			byte* ptr = originalRopOut;
			uint num = originalRopOutSize;
			List<byte[]> list = new List<byte[]>();
			List<bool> list2 = new List<bool>();
			List<bool> list3 = new List<bool>();
			if (num > 0)
			{
				while (ptr != null)
				{
					bool item = false;
					bool item2 = false;
					byte[] array = <Module>.UnpackExtBuffer(ptr, num, out item, out item2, &ptr, &num);
					if (array != null && array.Length > 0)
					{
						list.Add(array);
						list2.Add(item);
						list3.Add(item2);
					}
					if (num <= 0)
					{
						break;
					}
				}
			}
			if (list.Count > 0)
			{
				shreddedRopOut = list.ToArray();
				wasCompressedRopOut = list2.ToArray();
				wasXorMagicRopOut = list3.ToArray();
			}
		}

		public static void ShredChainedResponseBuffer(byte[] originalRopOut, out byte[][] shreddedRopOut, out bool[] wasCompressedRopOut, out bool[] wasXorMagicRopOut)
		{
			ref byte originalRopOut2 = ref originalRopOut[0];
			ExchangeRpcClient.ShredChainedResponseBuffer(ref originalRopOut2, originalRopOut.Length, out shreddedRopOut, out wasCompressedRopOut, out wasXorMagicRopOut);
		}

		public unsafe static byte[] PackChainedResponseBuffer(byte[][] shreddedRopOut, bool[] wasCompressedRopOut, bool[] wasXorMagicRopOut)
		{
			byte[] array = new byte[0];
			uint num = 0;
			if (shreddedRopOut == null)
			{
				throw new ArgumentException("null shreddedRopOut");
			}
			if (wasCompressedRopOut == null)
			{
				throw new ArgumentException("null wasCompressedRopOut");
			}
			if (wasXorMagicRopOut == null)
			{
				throw new ArgumentException("null wasXorMagicRopOut");
			}
			int num2 = wasCompressedRopOut.Length;
			if (shreddedRopOut.Length == num2 && num2 == wasXorMagicRopOut.Length)
			{
				for (int i = 0; i < shreddedRopOut.Length; i++)
				{
					byte* ptr = null;
					try
					{
						bool last = ((i >= shreddedRopOut.Length - 1) ? 1 : 0) != 0;
						byte[] array2 = shreddedRopOut[i];
						uint num3;
						ptr = <Module>.PackExtBuffer(array2, 0, array2.Length, wasCompressedRopOut[i], wasXorMagicRopOut[i], &num3, null, 0, last);
						IntPtr uPtrData = new IntPtr((void*)ptr);
						Array src = <Module>.UToMBytes(num3, uPtrData);
						byte[] array3 = new byte[num3 + num];
						Buffer.BlockCopy(array, 0, array3, 0, num);
						Buffer.BlockCopy(src, 0, array3, num, num3);
						array = array3;
						num += num3;
					}
					finally
					{
						if (ptr != null)
						{
							<Module>.MIDL_user_free((void*)ptr);
						}
					}
				}
				return array;
			}
			throw new ArgumentException("array length mismatch");
		}

		private bool fIsHTTP;

		private bool allowRpcRetry;

		private int totalRpcCounter;

		private TimeSpan totalRpcTime;
	}
}
