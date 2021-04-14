using System;
using System.Runtime.InteropServices;
using Microsoft.Exchange.Server.Storage.Common;

namespace Microsoft.Exchange.Server.Storage.HA
{
	internal static class StructConversion
	{
		private unsafe static _DATABASE_BACKUP_INFO* AllocateUnmanagedArray<_DATABASE_BACKUP_INFO>(Array managed)
		{
			ulong num = (ulong)((long)managed.Length);
			_DATABASE_BACKUP_INFO* ptr = <Module>.@new((num > 384307168202282325UL) ? ulong.MaxValue : (num * 48UL));
			if (0L == ptr)
			{
				throw new OutOfMemoryException("Failed to allocate unmanaged array");
			}
			initblk(ptr, 0, (long)managed.Length * 48L);
			return ptr;
		}

		private unsafe static _INSTANCE_BACKUP_INFO* AllocateUnmanagedArray<_INSTANCE_BACKUP_INFO>(Array managed)
		{
			ulong num = (ulong)((long)managed.Length);
			_INSTANCE_BACKUP_INFO* ptr = <Module>.@new((num > 384307168202282325UL) ? ulong.MaxValue : (num * 48UL));
			if (0L == ptr)
			{
				throw new OutOfMemoryException("Failed to allocate unmanaged array");
			}
			initblk(ptr, 0, (long)managed.Length * 48L);
			return ptr;
		}

		private unsafe static _LOGSHIP_INFO* AllocateUnmanagedArray<_LOGSHIP_INFO>(Array managed)
		{
			ulong num = (ulong)((long)managed.Length);
			_LOGSHIP_INFO* ptr = <Module>.@new((num > 1152921504606846975UL) ? ulong.MaxValue : (num * 16UL));
			if (0L == ptr)
			{
				throw new OutOfMemoryException("Failed to allocate unmanaged array");
			}
			initblk(ptr, 0, (long)managed.Length * 16L);
			return ptr;
		}

		private unsafe static void FreeUnmanagedArray<_DATABASE_BACKUP_INFO>(_DATABASE_BACKUP_INFO* rgT)
		{
			if (rgT != null)
			{
				<Module>.delete(rgT);
			}
		}

		private unsafe static void FreeUnmanagedArray<_INSTANCE_BACKUP_INFO>(_INSTANCE_BACKUP_INFO* rgT)
		{
			if (rgT != null)
			{
				<Module>.delete(rgT);
			}
		}

		private unsafe static void FreeUnmanagedArray<_LOGSHIP_INFO>(_LOGSHIP_INFO* rgT)
		{
			if (rgT != null)
			{
				<Module>.delete(rgT);
			}
		}

		private unsafe static _DATABASE_BACKUP_INFO MakeUnmanagedDatabaseBackupInfo(MDATABASE_BACKUP_INFO managed)
		{
			_DATABASE_BACKUP_INFO result = Marshal.StringToHGlobalUni(managed.Name).ToPointer();
			string text = string.Format("{0}\0\0\0", managed.Path);
			*(ref result + 8) = text.Length;
			IntPtr intPtr = Marshal.StringToHGlobalUni(text);
			*(ref result + 16) = intPtr.ToPointer();
			Guid guid = managed.Guid;
			ref void void& = ref guid;
			cpblk(ref result + 24, ref void&, 16);
			*(ref result + 40) = 0;
			*(ref result + 44) = (int)managed.Flags;
			return result;
		}

		private unsafe static void FreeUnmanagedDatabaseBackupInfo(_DATABASE_BACKUP_INFO databaseInfo)
		{
			if (databaseInfo != null)
			{
				IntPtr hglobal = new IntPtr(databaseInfo);
				Marshal.FreeHGlobal(hglobal);
			}
			if (*(ref databaseInfo + 16) != 0L)
			{
				IntPtr hglobal2 = new IntPtr(*(ref databaseInfo + 16));
				Marshal.FreeHGlobal(hglobal2);
			}
		}

		private unsafe static _DATABASE_BACKUP_INFO* MakeUnmanagedDatabaseBackupInfos(MDATABASE_BACKUP_INFO[] databases)
		{
			_DATABASE_BACKUP_INFO* ptr = StructConversion.AllocateUnmanagedArray<_DATABASE_BACKUP_INFO>(databases);
			int num = 0;
			if (0 < databases.Length)
			{
				_DATABASE_BACKUP_INFO* ptr2 = ptr;
				do
				{
					_DATABASE_BACKUP_INFO database_BACKUP_INFO = StructConversion.MakeUnmanagedDatabaseBackupInfo(databases[num]);
					cpblk(ptr2, ref database_BACKUP_INFO, 48);
					num++;
					ptr2 += 48L;
				}
				while (num < databases.Length);
			}
			return ptr;
		}

		private unsafe static void FreeUnmanagedDatabaseBackupInfos(int cInfo, _DATABASE_BACKUP_INFO* rgDatabaseInfo)
		{
			if (rgDatabaseInfo != null)
			{
				if (0 < cInfo)
				{
					_DATABASE_BACKUP_INFO* ptr = rgDatabaseInfo;
					int num = cInfo;
					do
					{
						StructConversion.FreeUnmanagedDatabaseBackupInfo(*ptr);
						ptr += 48L;
						num += -1;
					}
					while (num > 0);
				}
				<Module>.delete(rgDatabaseInfo);
			}
		}

		private unsafe static _INSTANCE_BACKUP_INFO MakeUnmanagedBackupInfo(MINSTANCE_BACKUP_INFO managed)
		{
			_INSTANCE_BACKUP_INFO result = managed.Instance.Value.ToInt64();
			IntPtr intPtr = Marshal.StringToHGlobalUni(managed.Name);
			*(ref result + 8) = intPtr.ToPointer();
			*(ref result + 16) = 0;
			*(ref result + 20) = managed.Databases.Length;
			*(ref result + 24) = StructConversion.MakeUnmanagedDatabaseBackupInfos(managed.Databases);
			*(ref result + 32) = 0;
			*(ref result + 40) = 0L;
			return result;
		}

		private unsafe static void FreeUnmanagedBackupInfo(_INSTANCE_BACKUP_INFO instanceInfo)
		{
			if (*(ref instanceInfo + 8) != 0L)
			{
				IntPtr hglobal = new IntPtr(*(ref instanceInfo + 8));
				Marshal.FreeHGlobal(hglobal);
			}
			StructConversion.FreeUnmanagedDatabaseBackupInfos(*(ref instanceInfo + 20), *(ref instanceInfo + 24));
		}

		private unsafe static _LOGSHIP_INFO MakeUnmanagedLogshipInfo(MLOGSHIP_INFO managed)
		{
			_LOGSHIP_INFO type = managed.Type;
			*(ref type + 4) = managed.Name.Length;
			IntPtr intPtr = Marshal.StringToHGlobalUni(managed.Name);
			*(ref type + 8) = intPtr.ToPointer();
			return type;
		}

		private unsafe static void FreeUnmanagedLogshipInfo(_LOGSHIP_INFO logshipInfo)
		{
			if (*(ref logshipInfo + 8) != 0L)
			{
				IntPtr hglobal = new IntPtr(*(ref logshipInfo + 8));
				Marshal.FreeHGlobal(hglobal);
			}
		}

		public unsafe static ushort* WszFromString(string s)
		{
			return (ushort*)Marshal.StringToHGlobalUni(s).ToPointer();
		}

		public unsafe static void FreeWsz(ushort* wsz)
		{
			if (wsz != null)
			{
				IntPtr hglobal = new IntPtr(wsz);
				Marshal.FreeHGlobal(hglobal);
			}
		}

		public static _GUID GetUnmanagedGuid(Guid managed)
		{
			ref void void& = ref managed;
			_GUID result;
			cpblk(ref result, ref void&, 16);
			return result;
		}

		public unsafe static _INSTANCE_BACKUP_INFO* MakeUnmanagedBackupInfos(MINSTANCE_BACKUP_INFO[] instances)
		{
			int num = instances.Length;
			_INSTANCE_BACKUP_INFO* ptr = null;
			_INSTANCE_BACKUP_INFO* result;
			try
			{
				ptr = StructConversion.AllocateUnmanagedArray<_INSTANCE_BACKUP_INFO>(instances);
				for (int i = 0; i < num; i++)
				{
					_INSTANCE_BACKUP_INFO instance_BACKUP_INFO = StructConversion.MakeUnmanagedBackupInfo(instances[i]);
					cpblk((long)i * 48L / (long)sizeof(_INSTANCE_BACKUP_INFO) + ptr, ref instance_BACKUP_INFO, 48);
				}
				result = ptr;
			}
			catch (object obj)
			{
				NullExecutionContext.Instance.Diagnostics.OnExceptionCatch(null);
				StructConversion.FreeUnmanagedBackupInfos(num, ptr);
				throw;
			}
			return result;
		}

		public unsafe static void FreeUnmanagedBackupInfos(int cInfo, _INSTANCE_BACKUP_INFO* rgInfo)
		{
			if (rgInfo != null)
			{
				if (0 < cInfo)
				{
					_INSTANCE_BACKUP_INFO* ptr = rgInfo;
					int num = cInfo;
					do
					{
						_INSTANCE_BACKUP_INFO instance_BACKUP_INFO = *ptr;
						if (*(ref instance_BACKUP_INFO + 8) != 0L)
						{
							IntPtr hglobal = new IntPtr(*(ref instance_BACKUP_INFO + 8));
							Marshal.FreeHGlobal(hglobal);
						}
						StructConversion.FreeUnmanagedDatabaseBackupInfos(*(ref instance_BACKUP_INFO + 20), *(ref instance_BACKUP_INFO + 24));
						ptr += 48L;
						num += -1;
					}
					while (num > 0);
				}
				<Module>.delete(rgInfo);
			}
		}

		public unsafe static _LOGSHIP_INFO* MakeUnmanagedLogshipInfos(MLOGSHIP_INFO[] infos)
		{
			int num = infos.Length;
			_LOGSHIP_INFO* ptr = null;
			_LOGSHIP_INFO* result;
			try
			{
				ptr = StructConversion.AllocateUnmanagedArray<_LOGSHIP_INFO>(infos);
				for (int i = 0; i < num; i++)
				{
					_LOGSHIP_INFO logship_INFO = StructConversion.MakeUnmanagedLogshipInfo(infos[i]);
					cpblk((long)i * 16L / (long)sizeof(_LOGSHIP_INFO) + ptr, ref logship_INFO, 16);
				}
				result = ptr;
			}
			catch (object obj)
			{
				NullExecutionContext.Instance.Diagnostics.OnExceptionCatch(null);
				StructConversion.FreeUnmanagedLogshipInfos(num, ptr);
				throw;
			}
			return result;
		}

		public unsafe static void FreeUnmanagedLogshipInfos(int cInfo, _LOGSHIP_INFO* rgInfo)
		{
			if (rgInfo != null)
			{
				if (0 < cInfo)
				{
					_LOGSHIP_INFO* ptr = rgInfo;
					int num = cInfo;
					do
					{
						_LOGSHIP_INFO logship_INFO = *ptr;
						if (*(ref logship_INFO + 8) != 0L)
						{
							IntPtr hglobal = new IntPtr(*(ref logship_INFO + 8));
							Marshal.FreeHGlobal(hglobal);
						}
						ptr += 16L;
						num += -1;
					}
					while (num > 0);
				}
				<Module>.delete(rgInfo);
			}
		}
	}
}
