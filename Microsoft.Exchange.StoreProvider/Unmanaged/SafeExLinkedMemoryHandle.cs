using System;
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Mapi.Unmanaged
{
	[ComVisible(false)]
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class SafeExLinkedMemoryHandle : SafeExMemoryHandle
	{
		internal SafeExLinkedMemoryHandle()
		{
		}

		internal SafeExLinkedMemoryHandle(IntPtr handle) : base(handle)
		{
		}

		public PropTag[] ReadPropTagArray()
		{
			if (this.IsInvalid)
			{
				return Array<PropTag>.Empty;
			}
			int size = Marshal.ReadInt32(this.handle, 0);
			PropTag[] array = Array<PropTag>.New(size);
			int num = 4;
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = (PropTag)Marshal.ReadInt32(this.handle, num);
				num += 4;
			}
			return array;
		}

		public unsafe PropProblem[] ReadPropProblemArray()
		{
			if (this.IsInvalid)
			{
				return null;
			}
			SPropProblemArray* ptr = (SPropProblemArray*)this.handle.ToPointer();
			PropProblem[] array = null;
			if (ptr->cProblem > 0)
			{
				SPropProblem* ptr2 = &ptr->aProblem;
				array = new PropProblem[ptr->cProblem];
				int i = 0;
				while (i < array.Length)
				{
					array[i] = new PropProblem(ptr2);
					i++;
					ptr2++;
				}
			}
			return array;
		}

		public int[] ReadInt32Array()
		{
			if (this.IsInvalid)
			{
				return null;
			}
			int size = Marshal.ReadInt32(this.handle, 0);
			int[] array = Array<int>.New(size);
			int num = 4;
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = Marshal.ReadInt32(this.handle, num);
				num += 4;
			}
			return array;
		}

		public unsafe void CopyTo(NamedProp[] destination, int startIndex, int length)
		{
			if (length == 0)
			{
				return;
			}
			if (this.IsInvalid)
			{
				throw new InvalidOperationException("Handle is invalid");
			}
			SNameId** ptr = (SNameId**)this.handle.ToPointer();
			for (int i = 0; i < length; i++)
			{
				if (*(IntPtr*)(ptr + (IntPtr)i * (IntPtr)sizeof(SNameId*) / (IntPtr)sizeof(SNameId*)) != (IntPtr)((UIntPtr)0))
				{
					destination[startIndex + i] = NamedProp.MarshalFromNative(*(IntPtr*)(ptr + (IntPtr)i * (IntPtr)sizeof(SNameId*) / (IntPtr)sizeof(SNameId*)));
				}
			}
		}

		public unsafe FastTransferBlock[] ReadFastTransferBlockArray(int cBlocks)
		{
			return base.ReadObjectArray<FastTransferBlock>(cBlocks, (SafeExMemoryHandle handle, IntPtr pointer, int index) => new FastTransferBlock((FxBlock*)((byte*)pointer.ToPointer() + (IntPtr)index * (IntPtr)sizeof(FxBlock))));
		}

		public override DisposeTracker GetDisposeTracker()
		{
			return DisposeTracker.Get<SafeExLinkedMemoryHandle>(this);
		}

		protected override bool ReleaseHandle()
		{
			if (!this.IsInvalid)
			{
				SafeExLinkedMemoryHandle.FreeLinkedFnEx(this.handle);
			}
			return true;
		}

		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
		[DllImport("exrpc32.dll", ExactSpelling = true)]
		private static extern void FreeLinkedFnEx(IntPtr lpBuffer);
	}
}
