using System;
using System.Runtime.InteropServices;

namespace Microsoft.Exchange.Rpc.PoolRpc
{
	internal abstract class PoolRpcServerCommonBase : RpcServerBase
	{
		protected unsafe override void RegisterInterface(void* ifSpec, ValueType mgrTypeGuid, _GUID* pMgrTypeUuid, void* pMgrEpv, uint flags, uint maxCalls)
		{
			if (null == mgrTypeGuid)
			{
				throw new RpcException("mgrTypeGuid is null");
			}
			Guid? mgrTypeGuid2 = (Guid)mgrTypeGuid;
			this.m_mgrTypeGuid = mgrTypeGuid2;
			base.RegisterInterface(ifSpec, null, null, null, flags, maxCalls);
		}

		protected unsafe override void UnregisterInterface(void* ifSpec, _GUID* pMgrTypeUuid, uint waitForCallsToComplete)
		{
			if (this.m_mgrTypeGuid != null)
			{
				int num = <Module>.RpcServerUnregisterIf(ifSpec, null, waitForCallsToComplete);
				if (num != null)
				{
					RpcServerBase.ThrowRpcException("Could not unregister interface", num);
				}
			}
			else
			{
				int num2 = <Module>.RpcServerUnregisterIf(ifSpec, pMgrTypeUuid, waitForCallsToComplete);
				if (num2 != null)
				{
					RpcServerBase.ThrowRpcException("Could not unregister interface", num2);
				}
			}
		}

		protected unsafe override void RegisterEp(void* ifSpec, _RPC_BINDING_VECTOR* pBindingVector, _UUID_VECTOR* pUuidVector, ushort* wszAnnotation)
		{
			_RPC_IF_ID rpc_IF_ID;
			initblk(ref rpc_IF_ID, 0, 20L);
			int num = <Module>.RpcIfInqId(ifSpec, &rpc_IF_ID);
			if (num != null)
			{
				RpcServerBase.TraceError("RpcIfInqId failed with status {2} in file {0} line {1}", new object[]
				{
					"f:\\15.00.1497\\sources\\dev\\common\\src\\rpc\\dll\\poolrpcserver.cpp",
					297,
					num
				});
				RpcServerBase.ThrowRpcException("RpcIfInqId", num);
			}
			if (this.m_mgrTypeGuid != null)
			{
				_GUID guid = <Module>.Microsoft.Exchange.Rpc.?A0xc2782e68.GUIDFromGuid(this.m_mgrTypeGuid.Value);
				_UUID_VECTOR uuid_VECTOR = 1;
				*(ref uuid_VECTOR + 8) = ref guid;
				RpcServerBase.CheckDuplicateEndpoint(ref rpc_IF_ID, ref guid, true);
				int num2 = <Module>.RpcEpRegisterW(ifSpec, pBindingVector, &uuid_VECTOR, wszAnnotation);
				if (num2 != null)
				{
					RpcServerBase.ThrowRpcException("RpcEpRegister", num2);
				}
			}
			else
			{
				_GUID guid2 = 0;
				initblk(ref guid2 + 4, 0, 12L);
				RpcServerBase.CheckDuplicateEndpoint(ref rpc_IF_ID, ref guid2, false);
				int num3 = <Module>.RpcEpRegisterW(ifSpec, pBindingVector, pUuidVector, wszAnnotation);
				if (num3 != null)
				{
					RpcServerBase.ThrowRpcException("RpcEpRegister", num3);
				}
			}
		}

		protected unsafe override void UnregisterEp(void* ifSpec, _RPC_BINDING_VECTOR* pBindingVector, _UUID_VECTOR* pUuidVector)
		{
			if (this.m_mgrTypeGuid != null)
			{
				_GUID guid = <Module>.Microsoft.Exchange.Rpc.?A0xc2782e68.GUIDFromGuid(this.m_mgrTypeGuid.Value);
				_UUID_VECTOR uuid_VECTOR = 1;
				*(ref uuid_VECTOR + 8) = ref guid;
				base.UnregisterEp(ifSpec, pBindingVector, &uuid_VECTOR);
			}
			else
			{
				base.UnregisterEp(ifSpec, pBindingVector, pUuidVector);
			}
		}

		public static RpcServerBase RegisterServerInstance(Type type, Guid? instanceGuid, [MarshalAs(UnmanagedType.U1)] bool isLocalOnly, string annotation)
		{
			if (instanceGuid != null)
			{
				Guid value = instanceGuid.Value;
				return RpcServerBase.RegisterServer(type, RpcServerBase.BuildDefaultSecurityDescriptor(), 1, value, null, annotation, isLocalOnly, false, 1234U);
			}
			return RpcServerBase.RegisterInterface(type, isLocalOnly, true, annotation);
		}

		public static RpcServerBase RegisterAutoListenServerInstance(Type type, Guid? instanceGuid, int maxCalls, [MarshalAs(UnmanagedType.U1)] bool isLocalOnly, string annotation)
		{
			if (instanceGuid != null)
			{
				Guid value = instanceGuid.Value;
				return RpcServerBase.RegisterServer(type, RpcServerBase.BuildDefaultSecurityDescriptor(), 1, value, null, annotation, isLocalOnly, true, (uint)maxCalls);
			}
			return RpcServerBase.RegisterAutoListenInterface(type, maxCalls, isLocalOnly, true, annotation);
		}

		public unsafe static void PackBuffer(ArraySegment<byte> source, ArraySegment<byte> destination, [MarshalAs(UnmanagedType.U1)] bool compress, [MarshalAs(UnmanagedType.U1)] bool xor, out uint bytesPacked)
		{
			ref byte pbDest = ref destination.Array[destination.Offset];
			uint num;
			<Module>.PackExtBuffer(source.Array, source.Offset, source.Count, compress, xor, &num, ref pbDest, destination.Count, true);
			bytesPacked = num;
		}

		public unsafe static ArraySegment<byte> UnpackBuffer(ArraySegment<byte> buffer, out ArraySegment<byte>? nextBuffer, out byte[] leasedBuffer)
		{
			byte[] array = null;
			ArraySegment<byte> result = default(ArraySegment<byte>);
			try
			{
				uint count = buffer.Count;
				nextBuffer = null;
				leasedBuffer = null;
				if (count == null)
				{
					throw new FailRpcException("cbBuffer is zero length", -2147221227);
				}
				if (count < 8)
				{
					throw new FailRpcException("cbBuffer needs to be at least sizeof(RpcHeaderExt)", -2147221227);
				}
				ref byte byte& = ref buffer.Array[buffer.Offset];
				try
				{
					tagRpcHeaderExt* ptr = ref byte&;
					byte* ptr2 = ref byte& + 8L;
					ptr2 = ptr2;
					ushort num = *(ushort*)(ptr + 4L / (long)sizeof(tagRpcHeaderExt));
					uint num2 = num;
					num2 = num2;
					ushort num3 = *(ushort*)(ptr + 2L / (long)sizeof(tagRpcHeaderExt));
					tagRpcHeaderExt* ptr3;
					if ((num3 & 1) != 0)
					{
						ptr3 = ptr + 6L / (long)sizeof(tagRpcHeaderExt);
						if (*(ushort*)ptr3 < num)
						{
							throw new FailRpcException("uncompressed size is smaller than compressed size", -2147221227);
						}
					}
					else
					{
						ptr3 = ptr + 6L / (long)sizeof(tagRpcHeaderExt);
						if (*(ushort*)ptr3 != num)
						{
							throw new FailRpcException("no compression but sizes aren't the same", -2147221227);
						}
					}
					ulong num4 = count - 8UL;
					if (num2 > num4)
					{
						if ((num3 & 3) != 0 || (num3 & 4) == 0 || num2 != count)
						{
							throw new FailRpcException("extended data larger than size of buffer", -2147221227);
						}
						num2 = (uint)num4;
					}
					if ((num3 & 4) == 0)
					{
						ulong num5 = num2;
						ArraySegment<byte> value = new ArraySegment<byte>(buffer.Array, (int)(num5 + (ulong)((long)buffer.Offset) + 8UL), (int)((long)buffer.Count - (long)num5 - 8L));
						ArraySegment<byte>? arraySegment = new ArraySegment<byte>?(value);
						nextBuffer = arraySegment;
					}
					if ((*(ushort*)(ptr + 2L / (long)sizeof(tagRpcHeaderExt)) & 2) != 0)
					{
						<Module>.DoCompressibleXorMagic(ptr2, num2);
						*(short*)(ptr + 2L / (long)sizeof(tagRpcHeaderExt)) = (short)(*(ushort*)(ptr + 2L / (long)sizeof(tagRpcHeaderExt)) & 65533);
					}
					if ((*(ushort*)(ptr + 2L / (long)sizeof(tagRpcHeaderExt)) & 1) != 0)
					{
						array = RpcBufferPool.GetBuffer((int)(*(ushort*)ptr3));
						ref byte pbOrig = ref array[0];
						try
						{
							if (<Module>.Decompress(ref pbOrig, *(ushort*)ptr3, ptr2, num2) == null)
							{
								throw new FailRpcException("failed to decompress buffer", -2147221227);
							}
							ArraySegment<byte> arraySegment2 = new ArraySegment<byte>(array, 0, (int)(*(ushort*)ptr3));
							result = arraySegment2;
							leasedBuffer = array;
							array = null;
						}
						catch
						{
							throw;
						}
					}
					else
					{
						ArraySegment<byte> arraySegment3 = new ArraySegment<byte>(buffer.Array, (int)((long)buffer.Offset + 8L), num2);
						result = arraySegment3;
					}
				}
				catch
				{
					throw;
				}
			}
			finally
			{
				if (array != null)
				{
					RpcBufferPool.ReleaseBuffer(array);
				}
			}
			return result;
		}

		public static byte[] UnpackBuffer(ArraySegment<byte> buffer)
		{
			ArraySegment<byte>? arraySegment = null;
			byte[] array = null;
			byte[] result;
			try
			{
				ArraySegment<byte> arraySegment2 = PoolRpcServerCommonBase.UnpackBuffer(buffer, out arraySegment, out array);
				if (arraySegment2.Count > 0)
				{
					byte[] array2 = new byte[arraySegment2.Count];
					Buffer.BlockCopy(arraySegment2.Array, arraySegment2.Offset, array2, 0, arraySegment2.Count);
					result = array2;
				}
				else
				{
					result = new byte[0];
				}
			}
			finally
			{
				if (array != null)
				{
					RpcBufferPool.ReleaseBuffer(array);
				}
			}
			return result;
		}

		public abstract int GetInterfaceInstance(Guid instanceGuid, out IPoolRpcServer instance);

		public abstract void ConnectionDropped(IntPtr contextHandle);

		public PoolRpcServerCommonBase()
		{
		}

		protected Guid? m_mgrTypeGuid;
	}
}
