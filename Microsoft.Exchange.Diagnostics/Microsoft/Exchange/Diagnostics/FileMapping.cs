using System;
using System.ComponentModel;
using System.Globalization;
using System.Runtime.InteropServices;

namespace Microsoft.Exchange.Diagnostics
{
	public class FileMapping
	{
		public FileMapping(string name, bool writable)
		{
			this.fileMappingHandle = FileMapping.OpenFileMapping(name, writable);
			this.MapViewOfFile(this.fileMappingHandle, writable, name);
		}

		public FileMapping(string name, int size, bool writable)
		{
			this.fileMappingHandle = FileMapping.CreateFileMapping(name, size, writable);
			this.MapViewOfFile(this.fileMappingHandle, writable, name);
		}

		public IntPtr IntPtr
		{
			get
			{
				return this.mapViewFileHandle;
			}
		}

		[DllImport("Kernel32.dll", ExactSpelling = true, SetLastError = true)]
		public static extern bool CloseHandle(IntPtr handle);

		public void Dispose()
		{
			FileMapping.UnmapViewOfFile(this.mapViewFileHandle);
			FileMapping.CloseHandle(this.fileMappingHandle);
		}

		public void Close()
		{
			this.Dispose();
		}

		[DllImport("Kernel32.dll", BestFitMapping = false, SetLastError = true)]
		internal static extern IntPtr CreateFileMapping(IntPtr hFile, FileMapping.SECURITY_ATTRIBUTES lpFileMappingAttributes, int flProtect, int dwMaximumSizeHigh, int dwMaximumSizeLow, string lpName);

		[DllImport("Kernel32.dll", SetLastError = true)]
		internal static extern IntPtr OpenFileMapping(int desiredAccess, bool inheritHandle, string name);

		[DllImport("Kernel32.dll", SetLastError = true)]
		internal static extern IntPtr MapViewOfFile(IntPtr fileMappingObject, int desiredAccess, int fileOffsetHigh, int fileOffsetLow, UIntPtr numberOfBytesToMap);

		[DllImport("Advapi32.dll", SetLastError = true)]
		internal static extern bool ConvertStringSecurityDescriptorToSecurityDescriptor(string stringSecurityDescriptor, int stringSDRevision, out IntPtr pSecurityDescriptor, IntPtr securityDescriptorSize);

		private static IntPtr CreateFileMapping(string name, int size, bool writable)
		{
			IntPtr intPtr = IntPtr.Zero;
			IntPtr intPtr2 = IntPtr.Zero;
			int flProtect;
			if (writable)
			{
				flProtect = 4;
			}
			else
			{
				flProtect = 2;
			}
			IntPtr result;
			try
			{
				intPtr = FileMapping.GetSecurityDescriptor(name);
				FileMapping.SECURITY_ATTRIBUTES security_ATTRIBUTES = new FileMapping.SECURITY_ATTRIBUTES();
				security_ATTRIBUTES.SecurityDescriptor = intPtr;
				security_ATTRIBUTES.InheritHandle = false;
				intPtr2 = FileMapping.CreateFileMapping((IntPtr)(-1), security_ATTRIBUTES, flProtect, 0, size, name);
				if (intPtr2 == IntPtr.Zero)
				{
					throw new Win32Exception(Marshal.GetLastWin32Error(), "Could not create file mapping where file mapping name is : " + name);
				}
				result = intPtr2;
			}
			finally
			{
				if (intPtr != IntPtr.Zero)
				{
					FileMapping.LocalFree(intPtr);
				}
			}
			return result;
		}

		private static IntPtr GetSecurityDescriptor(string name)
		{
			string stringSecurityDescriptor = "D:(A;OICI;FRFWGRGW;;;AU)";
			IntPtr zero = IntPtr.Zero;
			if (!FileMapping.ConvertStringSecurityDescriptorToSecurityDescriptor(stringSecurityDescriptor, 1, out zero, IntPtr.Zero))
			{
				throw new Win32Exception(Marshal.GetLastWin32Error(), "Not able to get Security Descriptor where file mapping name is " + name);
			}
			return zero;
		}

		[DllImport("Kernel32.dll", SetLastError = true)]
		private static extern int LocalFree(IntPtr hMem);

		[DllImport("Kernel32.dll", SetLastError = true)]
		private static extern bool UnmapViewOfFile(IntPtr handle);

		private static IntPtr OpenFileMapping(string name, bool writable)
		{
			IntPtr intPtr = IntPtr.Zero;
			int desiredAccess;
			if (writable)
			{
				desiredAccess = 2;
			}
			else
			{
				desiredAccess = 4;
			}
			intPtr = FileMapping.OpenFileMapping(desiredAccess, false, name);
			if (intPtr == IntPtr.Zero)
			{
				int lastWin32Error = Marshal.GetLastWin32Error();
				throw new FileMappingNotFoundException(string.Format(CultureInfo.InvariantCulture, "Cound not open File mapping for name {0}. Error Details: {1}", new object[]
				{
					name,
					lastWin32Error
				}));
			}
			return intPtr;
		}

		private void MapViewOfFile(IntPtr fileMappingHandle, bool writable, string name)
		{
			int desiredAccess;
			if (writable)
			{
				desiredAccess = 2;
			}
			else
			{
				desiredAccess = 4;
			}
			this.mapViewFileHandle = FileMapping.MapViewOfFile(fileMappingHandle, desiredAccess, 0, 0, UIntPtr.Zero);
			if (this.mapViewFileHandle == IntPtr.Zero)
			{
				throw new Win32Exception(Marshal.GetLastWin32Error(), "Could not Get Map View of File Where file mapping name is : " + name);
			}
		}

		internal const int SDDLRevision1 = 1;

		private const int FileMapRead = 4;

		private const int FileMapWrite = 2;

		private const int PageReadonly = 2;

		private const int PageReadWrite = 4;

		private IntPtr mapViewFileHandle;

		private IntPtr fileMappingHandle;

		internal struct MEMORY_BASIC_INFORMATION
		{
			internal IntPtr BaseAddress;

			internal IntPtr AllocationBase;

			internal uint AllocationProtect;

			internal UIntPtr RegionSize;

			internal uint State;

			internal uint Protect;

			internal uint Type;
		}

		[StructLayout(LayoutKind.Sequential)]
		internal class SECURITY_ATTRIBUTES
		{
			public int Length = 12;

			public IntPtr SecurityDescriptor = IntPtr.Zero;

			public bool InheritHandle;
		}
	}
}
