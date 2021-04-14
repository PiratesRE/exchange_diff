using System;
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Mapi.Unmanaged
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	[ComVisible(false)]
	internal class SafeExMemoryHandle : DisposeTrackableSafeHandleZeroOrMinusOneIsInvalid
	{
		protected SafeExMemoryHandle()
		{
		}

		internal SafeExMemoryHandle(IntPtr handle) : base(handle)
		{
		}

		public void CopyTo(byte[] destination, int startIndex, int length)
		{
			if (length == 0)
			{
				return;
			}
			if (this.IsInvalid)
			{
				throw new InvalidOperationException("Handle is invalid");
			}
			Marshal.Copy(this.handle, destination, startIndex, length);
		}

		public void CopyTo(short[] destination, int startIndex, int length)
		{
			if (length == 0)
			{
				return;
			}
			if (this.IsInvalid)
			{
				throw new InvalidOperationException("Handle is invalid");
			}
			Marshal.Copy(this.handle, destination, startIndex, length);
		}

		public void CopyTo(uint[] destination, int startIndex, int length)
		{
			if (length == 0)
			{
				return;
			}
			if (this.IsInvalid)
			{
				throw new InvalidOperationException("Handle is invalid");
			}
			int num = 0;
			for (int i = 0; i < length; i++)
			{
				destination[i] = (uint)Marshal.ReadInt32(this.handle, num);
				num += 4;
			}
		}

		public T[] ReadObjectArray<T>(int count, Func<SafeExMemoryHandle, IntPtr, int, T> createObjectDelegate)
		{
			if (this.IsInvalid)
			{
				return null;
			}
			if (count == 0)
			{
				return Array<T>.Empty;
			}
			T[] array = Array<T>.New(count);
			for (int i = 0; i < count; i++)
			{
				array[i] = createObjectDelegate(this, this.handle, i);
			}
			return array;
		}

		public unsafe MapiLtidNative[] ReadMapiLtidNativeArray(int cLtids)
		{
			return this.ReadObjectArray<MapiLtidNative>(cLtids, (SafeExMemoryHandle handle, IntPtr pointer, int index) => new MapiLtidNative((NativeLtid*)((byte*)pointer.ToPointer() + (IntPtr)index * (IntPtr)sizeof(NativeLtid))));
		}

		public unsafe MapiPUDNative[] ReadMapiPudNativeArray(int cpud)
		{
			return this.ReadObjectArray<MapiPUDNative>(cpud, (SafeExMemoryHandle handle, IntPtr pointer, int index) => new MapiPUDNative((NativePUD*)((byte*)pointer.ToPointer() + (IntPtr)index * (IntPtr)sizeof(NativePUD))));
		}

		public unsafe void CopyTo(Guid[] destination, int startIndex, int length)
		{
			if (length == 0)
			{
				return;
			}
			if (this.IsInvalid)
			{
				throw new InvalidOperationException("Handle is invalid");
			}
			Guid* ptr = (Guid*)base.DangerousGetHandle().ToPointer();
			for (int i = 0; i < length; i++)
			{
				destination[startIndex + i] = ptr[i];
			}
		}

		public void ForEach<T>(int count, Action<T> action)
		{
			if (count == 0)
			{
				return;
			}
			if (this.IsInvalid)
			{
				throw new InvalidOperationException("Handle is invalid");
			}
			IntPtr intPtr = base.DangerousGetHandle();
			long num = (long)Marshal.SizeOf(typeof(T));
			uint num2 = 0U;
			while ((ulong)num2 < (ulong)((long)count))
			{
				T obj = (T)((object)Marshal.PtrToStructure(intPtr, typeof(T)));
				action(obj);
				intPtr = (IntPtr)((long)intPtr + num);
				num2 += 1U;
			}
		}

		public override DisposeTracker GetDisposeTracker()
		{
			return DisposeTracker.Get<SafeExMemoryHandle>(this);
		}

		protected override bool ReleaseHandle()
		{
			if (!this.IsInvalid)
			{
				SafeExMemoryHandle.FreePvFnEx(this.handle);
			}
			return true;
		}

		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
		[DllImport("exrpc32.dll", ExactSpelling = true)]
		internal static extern void FreePvFnEx(IntPtr lpBuffer);
	}
}
