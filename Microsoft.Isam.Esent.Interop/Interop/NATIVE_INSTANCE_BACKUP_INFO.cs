using System;
using System.Runtime.InteropServices;

namespace Microsoft.Isam.Esent.Interop
{
	internal struct NATIVE_INSTANCE_BACKUP_INFO
	{
		public unsafe void FreeNativeInstanceInfo()
		{
			NATIVE_DATABASE_BACKUP_INFO* ptr = (NATIVE_DATABASE_BACKUP_INFO*)((void*)this.rgDatabase);
			int num = 0;
			while ((long)num < (long)((ulong)this.cDatabase))
			{
				ptr[num].FreeNativeDatabaseBackupInfo();
				num++;
			}
			NATIVE_ESE_ICON_DESCRIPTION* ptr2 = (NATIVE_ESE_ICON_DESCRIPTION*)((void*)this.rgIconDescription);
			int num2 = 0;
			while ((long)num2 < (long)((ulong)this.cIconDescription))
			{
				ptr2[num2].FreeNativeIconDescription();
				num2++;
			}
			Marshal.FreeHGlobal(this.wszInstanceName);
			this.wszInstanceName = IntPtr.Zero;
			Marshal.FreeHGlobal(this.rgDatabase);
			this.rgDatabase = IntPtr.Zero;
			Marshal.FreeHGlobal(this.rgIconDescription);
			this.rgIconDescription = IntPtr.Zero;
		}

		public long hInstanceId;

		public IntPtr wszInstanceName;

		public uint ulIconIndexInstance;

		public uint cDatabase;

		public IntPtr rgDatabase;

		public uint cIconDescription;

		public IntPtr rgIconDescription;
	}
}
