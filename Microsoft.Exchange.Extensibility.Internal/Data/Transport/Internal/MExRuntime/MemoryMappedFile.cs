using System;
using System.ComponentModel;
using System.IO;
using System.Runtime.InteropServices;
using Microsoft.Exchange.Win32;
using Microsoft.Win32.SafeHandles;

namespace Microsoft.Exchange.Data.Transport.Internal.MExRuntime
{
	internal sealed class MemoryMappedFile : IDisposable
	{
		static MemoryMappedFile()
		{
			ulong num;
			if (!NativeMethods.ConvertStringSecurityDescriptorToSecurityDescriptor("D:(A;;0x101fffff;;;SY)(A;;0x101fffff;;;BA)(A;;0x101fffff;;;CO)(D;;WDWO;;;WD)", 1U, out MemoryMappedFile.defaultSd, out num))
			{
				throw new Win32Exception();
			}
			MemoryMappedFile.defaultSecurityAttributes = new NativeMethods.SECURITY_ATTRIBUTES(MemoryMappedFile.defaultSd);
		}

		public MemoryMappedFile(string name, int size, bool writable)
		{
			this.size = (uint)((size > 0) ? size : 0);
			NativeMethods.MemoryAccessControl memoryAccessControl;
			if (writable)
			{
				memoryAccessControl = NativeMethods.MemoryAccessControl.ReadWrite;
				this.mapMode = NativeMethods.FileMapAccessControl.Write;
			}
			else
			{
				memoryAccessControl = NativeMethods.MemoryAccessControl.Readonly;
				this.mapMode = NativeMethods.FileMapAccessControl.Read;
			}
			this.fileHandle = NativeMethods.CreateFileMapping(new SafeFileHandle(IntPtr.Zero, false), ref MemoryMappedFile.defaultSecurityAttributes, memoryAccessControl, 0U, this.size, name);
			int lastWin32Error = Marshal.GetLastWin32Error();
			if (this.fileHandle.IsInvalid)
			{
				throw new IOException("CreateFileMapping(" + memoryAccessControl + ") failed", (lastWin32Error == 0) ? null : new Win32Exception(lastWin32Error));
			}
			if (memoryAccessControl == NativeMethods.MemoryAccessControl.Readonly && lastWin32Error != 183)
			{
				this.fileHandle.Dispose();
				throw new IOException("CreateFileMapping(READONLY) failed - '" + name + "' not found", (lastWin32Error == 0) ? null : new Win32Exception(lastWin32Error));
			}
			if (memoryAccessControl == NativeMethods.MemoryAccessControl.ReadWrite && lastWin32Error == 183)
			{
				this.fileHandle.Dispose();
				throw new IOException("CreateFileMapping(READWRITE) failed - '" + name + "' already exists", (lastWin32Error == 0) ? null : new Win32Exception(lastWin32Error));
			}
		}

		public MapFileStream CreateView(int offset, int size)
		{
			if (this.fileHandle == null)
			{
				throw new ObjectDisposedException("MemoryMappedFile");
			}
			if (this.fileHandle.IsInvalid)
			{
				throw new InvalidOperationException("MemoryMappedFile");
			}
			if ((long)(offset + size) > (long)((ulong)this.size))
			{
				throw new ArgumentException("size");
			}
			SafeViewOfFileHandle safeViewOfFileHandle = NativeMethods.MapViewOfFile(this.fileHandle, this.mapMode, 0U, (uint)offset, new UIntPtr((uint)size));
			int lastWin32Error = Marshal.GetLastWin32Error();
			if (safeViewOfFileHandle.IsInvalid)
			{
				throw new IOException("MapViewOfFile(" + this.mapMode + ") failed", (lastWin32Error == 0) ? null : new Win32Exception(lastWin32Error));
			}
			return new MapFileStream(safeViewOfFileHandle, size, this.mapMode == NativeMethods.FileMapAccessControl.Write);
		}

		public void Close()
		{
			if (this.fileHandle != null)
			{
				this.fileHandle.Dispose();
				this.fileHandle = null;
			}
		}

		public void Dispose()
		{
			this.Close();
		}

		private const string DefaultDacl = "D:(A;;0x101fffff;;;SY)(A;;0x101fffff;;;BA)(A;;0x101fffff;;;CO)(D;;WDWO;;;WD)";

		private static readonly SafeHGlobalHandle defaultSd;

		private static NativeMethods.SECURITY_ATTRIBUTES defaultSecurityAttributes;

		private uint size;

		private NativeMethods.FileMapAccessControl mapMode;

		private SafeFileHandle fileHandle;
	}
}
