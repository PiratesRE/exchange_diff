using System;
using System.Runtime.ExceptionServices;
using System.Runtime.InteropServices;
using Microsoft.Exchange.Net;

namespace Microsoft.Exchange.Rpc.ExchangeClient
{
	internal class ClientAsyncCallState_Execute : ClientAsyncCallState
	{
		private void Cleanup()
		{
			if (this.m_pulFlags != IntPtr.Zero)
			{
				Marshal.FreeHGlobal(this.m_pulFlags);
				this.m_pulFlags = IntPtr.Zero;
			}
			if (this.m_pbRopIn != IntPtr.Zero)
			{
				Marshal.FreeHGlobal(this.m_pbRopIn);
				this.m_pbRopIn = IntPtr.Zero;
			}
			if (this.m_pbRopOut != IntPtr.Zero)
			{
				Marshal.FreeHGlobal(this.m_pbRopOut);
				this.m_pbRopOut = IntPtr.Zero;
			}
			if (this.m_pcbRopOut != IntPtr.Zero)
			{
				Marshal.FreeHGlobal(this.m_pcbRopOut);
				this.m_pcbRopOut = IntPtr.Zero;
			}
			if (this.m_pbAuxIn != IntPtr.Zero)
			{
				Marshal.FreeHGlobal(this.m_pbAuxIn);
				this.m_pbAuxIn = IntPtr.Zero;
			}
			if (this.m_pbAuxOut != IntPtr.Zero)
			{
				Marshal.FreeHGlobal(this.m_pbAuxOut);
				this.m_pbAuxOut = IntPtr.Zero;
			}
			if (this.m_pcbAuxOut != IntPtr.Zero)
			{
				Marshal.FreeHGlobal(this.m_pcbAuxOut);
				this.m_pcbAuxOut = IntPtr.Zero;
			}
			if (this.m_pulTransTime != IntPtr.Zero)
			{
				Marshal.FreeHGlobal(this.m_pulTransTime);
				this.m_pulTransTime = IntPtr.Zero;
			}
			if (this.m_pExCXH != IntPtr.Zero)
			{
				Marshal.FreeHGlobal(this.m_pExCXH);
				this.m_pExCXH = IntPtr.Zero;
			}
		}

		public ClientAsyncCallState_Execute(CancelableAsyncCallback asyncCallback, object asyncState, IntPtr contextHandle, int flags, ArraySegment<byte> segmentExtendedRopIn, ArraySegment<byte> segmentExtendedRopOut, ArraySegment<byte> segmentExtendedAuxIn, ArraySegment<byte> segmentExtendedAuxOut) : base("Execute", asyncCallback, asyncState)
		{
			try
			{
				this.m_segmentExtendedRopOut = segmentExtendedRopOut;
				this.m_segmentExtendedAuxOut = segmentExtendedAuxOut;
				this.m_pulFlags = IntPtr.Zero;
				this.m_pbRopIn = IntPtr.Zero;
				this.m_pbRopOut = IntPtr.Zero;
				this.m_pcbRopOut = IntPtr.Zero;
				this.m_pbAuxIn = IntPtr.Zero;
				this.m_pbAuxOut = IntPtr.Zero;
				this.m_pcbAuxOut = IntPtr.Zero;
				this.m_pulTransTime = IntPtr.Zero;
				this.m_pExCXH = IntPtr.Zero;
				bool flag = false;
				try
				{
					IntPtr pulFlags = Marshal.AllocHGlobal(4);
					this.m_pulFlags = pulFlags;
					Marshal.WriteInt32(this.m_pulFlags, flags);
					int count = segmentExtendedRopIn.Count;
					this.m_cbRopIn = count;
					IntPtr pbRopIn = Marshal.AllocHGlobal(count + 8);
					this.m_pbRopIn = pbRopIn;
					int cbRopIn = this.m_cbRopIn;
					if (cbRopIn > 0)
					{
						Marshal.Copy(segmentExtendedRopIn.Array, segmentExtendedRopIn.Offset, this.m_pbRopIn, cbRopIn);
					}
					int count2 = segmentExtendedRopOut.Count;
					IntPtr pbRopOut = Marshal.AllocHGlobal(count2 + 8);
					this.m_pbRopOut = pbRopOut;
					IntPtr pcbRopOut = Marshal.AllocHGlobal(4);
					this.m_pcbRopOut = pcbRopOut;
					Marshal.WriteInt32(this.m_pcbRopOut, count2);
					int count3 = segmentExtendedAuxIn.Count;
					this.m_cbAuxIn = count3;
					IntPtr pbAuxIn = Marshal.AllocHGlobal(count3 + 8);
					this.m_pbAuxIn = pbAuxIn;
					int cbAuxIn = this.m_cbAuxIn;
					if (cbAuxIn > 0)
					{
						Marshal.Copy(segmentExtendedAuxIn.Array, segmentExtendedAuxIn.Offset, this.m_pbAuxIn, cbAuxIn);
					}
					int count4 = segmentExtendedAuxOut.Count;
					IntPtr pbAuxOut = Marshal.AllocHGlobal(count4 + 8);
					this.m_pbAuxOut = pbAuxOut;
					IntPtr pcbAuxOut = Marshal.AllocHGlobal(4);
					this.m_pcbAuxOut = pcbAuxOut;
					Marshal.WriteInt32(this.m_pcbAuxOut, count4);
					IntPtr pulTransTime = Marshal.AllocHGlobal(4);
					this.m_pulTransTime = pulTransTime;
					IntPtr pExCXH = Marshal.AllocHGlobal(IntPtr.Size);
					this.m_pExCXH = pExCXH;
					Marshal.WriteIntPtr(this.m_pExCXH, contextHandle);
					flag = true;
				}
				finally
				{
					if (!flag)
					{
						this.Cleanup();
					}
				}
			}
			catch
			{
				base.Dispose(true);
				throw;
			}
		}

		private void ~ClientAsyncCallState_Execute()
		{
			this.Cleanup();
		}

		public unsafe override void InternalBegin()
		{
			<Module>.cli_Async_EcDoRpcExt2((_RPC_ASYNC_STATE*)base.RpcAsyncState().ToPointer(), (void**)this.m_pExCXH.ToPointer(), (uint*)this.m_pulFlags.ToPointer(), this.m_pbRopIn.ToPointer(), this.m_cbRopIn, this.m_pbRopOut.ToPointer(), (uint*)this.m_pcbRopOut.ToPointer(), this.m_pbAuxIn.ToPointer(), this.m_cbAuxIn, this.m_pbAuxOut.ToPointer(), (uint*)this.m_pcbAuxOut.ToPointer(), (uint*)this.m_pulTransTime.ToPointer());
		}

		public int End(out IntPtr contextHandle, out ArraySegment<byte> segmentExtendedRopOut, out ArraySegment<byte> segmentExtendedAuxOut)
		{
			int result;
			try
			{
				int num = base.CheckCompletion();
				int num2 = Marshal.ReadInt32(this.m_pcbRopOut);
				if (num2 > this.m_segmentExtendedRopOut.Count)
				{
					throw new Exception(string.Format("Server returned more data then requested; what? cbRopOut={0}, buffer.Count={1}", num2, this.m_segmentExtendedRopOut.Count));
				}
				if (num2 > 0)
				{
					Marshal.Copy(this.m_pbRopOut, this.m_segmentExtendedRopOut.Array, this.m_segmentExtendedRopOut.Offset, num2);
				}
				ArraySegment<byte> arraySegment = new ArraySegment<byte>(this.m_segmentExtendedRopOut.Array, this.m_segmentExtendedRopOut.Offset, num2);
				segmentExtendedRopOut = arraySegment;
				int num3 = Marshal.ReadInt32(this.m_pcbAuxOut);
				if (num3 > this.m_segmentExtendedAuxOut.Count)
				{
					throw new Exception(string.Format("Server returned more data then requested; what? cbAuxOut={0}, buffer.Count={1}", num3, this.m_segmentExtendedAuxOut.Count));
				}
				if (num3 > 0)
				{
					Marshal.Copy(this.m_pbAuxOut, this.m_segmentExtendedAuxOut.Array, this.m_segmentExtendedAuxOut.Offset, num3);
				}
				ArraySegment<byte> arraySegment2 = new ArraySegment<byte>(this.m_segmentExtendedAuxOut.Array, this.m_segmentExtendedAuxOut.Offset, num3);
				segmentExtendedAuxOut = arraySegment2;
				IntPtr intPtr = Marshal.ReadIntPtr(this.m_pExCXH);
				contextHandle = intPtr;
				result = num;
			}
			finally
			{
				this.Cleanup();
			}
			return result;
		}

		[HandleProcessCorruptedStateExceptions]
		protected override void Dispose([MarshalAs(UnmanagedType.U1)] bool A_0)
		{
			if (A_0)
			{
				try
				{
					this.Cleanup();
					return;
				}
				finally
				{
					base.Dispose(true);
				}
			}
			base.Dispose(false);
		}

		private ArraySegment<byte> m_segmentExtendedRopOut;

		private ArraySegment<byte> m_segmentExtendedAuxOut;

		private IntPtr m_pulFlags;

		private IntPtr m_pbRopIn;

		private int m_cbRopIn;

		private IntPtr m_pbRopOut;

		private IntPtr m_pcbRopOut;

		private IntPtr m_pbAuxIn;

		private int m_cbAuxIn;

		private IntPtr m_pbAuxOut;

		private IntPtr m_pcbAuxOut;

		private IntPtr m_pulTransTime;

		private IntPtr m_pExCXH;
	}
}
