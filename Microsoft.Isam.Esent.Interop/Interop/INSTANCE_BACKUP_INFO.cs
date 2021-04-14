using System;
using System.Globalization;
using System.Runtime.InteropServices;

namespace Microsoft.Isam.Esent.Interop
{
	[StructLayout(LayoutKind.Sequential)]
	public class INSTANCE_BACKUP_INFO
	{
		public long hInstanceId { get; set; }

		public string wszInstanceName { get; set; }

		public int ulIconIndexInstance { get; set; }

		public DATABASE_BACKUP_INFO[] rgDatabase { get; set; }

		public ESE_ICON_DESCRIPTION[] rgIconDescription { get; set; }

		public override string ToString()
		{
			return string.Format(CultureInfo.InvariantCulture, "INSTANCE_BACKUP_INFO({0})", new object[]
			{
				this.wszInstanceName
			});
		}

		internal NATIVE_INSTANCE_BACKUP_INFO GetNativeInstanceBackupInfo()
		{
			NATIVE_ESE_ICON_DESCRIPTION[] nativeEseIconDescriptions = INSTANCE_BACKUP_INFO.GetNativeEseIconDescriptions(this.rgIconDescription);
			NATIVE_DATABASE_BACKUP_INFO[] nativeDatabaseBackupInfos = INSTANCE_BACKUP_INFO.GetNativeDatabaseBackupInfos(this.rgDatabase);
			int cDatabase = (nativeDatabaseBackupInfos == null) ? 0 : nativeDatabaseBackupInfos.Length;
			int cIconDescription = (nativeEseIconDescriptions == null) ? 0 : nativeEseIconDescriptions.Length;
			IntPtr rgIconDescription = Eseback.ConvertStructArrayToIntPtr<NATIVE_ESE_ICON_DESCRIPTION>(nativeEseIconDescriptions);
			IntPtr rgDatabase = Eseback.ConvertStructArrayToIntPtr<NATIVE_DATABASE_BACKUP_INFO>(nativeDatabaseBackupInfos);
			return new NATIVE_INSTANCE_BACKUP_INFO
			{
				hInstanceId = this.hInstanceId,
				rgIconDescription = rgIconDescription,
				cDatabase = (uint)cDatabase,
				rgDatabase = rgDatabase,
				wszInstanceName = Marshal.StringToHGlobalUni(this.wszInstanceName),
				cIconDescription = (uint)cIconDescription,
				ulIconIndexInstance = (uint)this.ulIconIndexInstance
			};
		}

		private static NATIVE_DATABASE_BACKUP_INFO[] GetNativeDatabaseBackupInfos(DATABASE_BACKUP_INFO[] databaseBackupInfos)
		{
			NATIVE_DATABASE_BACKUP_INFO[] array = null;
			if (databaseBackupInfos != null)
			{
				array = new NATIVE_DATABASE_BACKUP_INFO[databaseBackupInfos.Length];
				for (int i = 0; i < array.Length; i++)
				{
					array[i] = databaseBackupInfos[i].GetNativeDatabaseBackupInfo();
				}
			}
			return array;
		}

		private static NATIVE_ESE_ICON_DESCRIPTION[] GetNativeEseIconDescriptions(ESE_ICON_DESCRIPTION[] eseIconDescriptions)
		{
			NATIVE_ESE_ICON_DESCRIPTION[] array = null;
			if (eseIconDescriptions != null)
			{
				array = new NATIVE_ESE_ICON_DESCRIPTION[eseIconDescriptions.Length];
				for (int i = 0; i < array.Length; i++)
				{
					array[i] = eseIconDescriptions[i].GetNativeEseIconDescription();
				}
			}
			return array;
		}
	}
}
