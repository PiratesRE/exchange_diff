using System;

namespace Microsoft.Exchange.Server.Storage.HA
{
	public static class Eseback
	{
		public unsafe static int BackupRestoreRegister(string displayName, Eseback.Flags flags, string endpointAnnotation, IEsebackCallbacks callbacks, Guid crimsonPublisher)
		{
			EsebackCallbacks.ManagedCallbacks = callbacks;
			_ESEBACK_CALLBACKS _unep@?PrepareInstanceForBackup@InteropCallbacks@HA@Storage@Server@Exchange@Microsoft@@$$FSAJPEAU_ESEBACK_CONTEXT@@_KPEAX@Z = <Module>.__unep@?PrepareInstanceForBackup@InteropCallbacks@HA@Storage@Server@Exchange@Microsoft@@$$FSAJPEAU_ESEBACK_CONTEXT@@_KPEAX@Z;
			*(ref _unep@?PrepareInstanceForBackup@InteropCallbacks@HA@Storage@Server@Exchange@Microsoft@@$$FSAJPEAU_ESEBACK_CONTEXT@@_KPEAX@Z + 8) = <Module>.__unep@?DoneWithInstanceForBackup@InteropCallbacks@HA@Storage@Server@Exchange@Microsoft@@$$FSAJPEAU_ESEBACK_CONTEXT@@_KKPEAX@Z;
			*(ref _unep@?PrepareInstanceForBackup@InteropCallbacks@HA@Storage@Server@Exchange@Microsoft@@$$FSAJPEAU_ESEBACK_CONTEXT@@_KPEAX@Z + 16) = <Module>.__unep@?GetDatabasesInfo@InteropCallbacks@HA@Storage@Server@Exchange@Microsoft@@$$FSAJPEAU_ESEBACK_CONTEXT@@PEAKPEAPEAU_INSTANCE_BACKUP_INFO@@K@Z;
			*(ref _unep@?PrepareInstanceForBackup@InteropCallbacks@HA@Storage@Server@Exchange@Microsoft@@$$FSAJPEAU_ESEBACK_CONTEXT@@_KPEAX@Z + 24) = <Module>.__unep@?FreeDatabasesInfo@InteropCallbacks@HA@Storage@Server@Exchange@Microsoft@@$$FSAJPEAU_ESEBACK_CONTEXT@@KPEAU_INSTANCE_BACKUP_INFO@@@Z;
			*(ref _unep@?PrepareInstanceForBackup@InteropCallbacks@HA@Storage@Server@Exchange@Microsoft@@$$FSAJPEAU_ESEBACK_CONTEXT@@_KPEAX@Z + 32) = <Module>.__unep@?IsSGReplicated@InteropCallbacks@HA@Storage@Server@Exchange@Microsoft@@$$FSAJPEAU_ESEBACK_CONTEXT@@_KPEAHKPEAGPEAKPEAPEAU_LOGSHIP_INFO@@@Z;
			*(ref _unep@?PrepareInstanceForBackup@InteropCallbacks@HA@Storage@Server@Exchange@Microsoft@@$$FSAJPEAU_ESEBACK_CONTEXT@@_KPEAX@Z + 40) = <Module>.__unep@?FreeShipLogInfo@InteropCallbacks@HA@Storage@Server@Exchange@Microsoft@@$$FSAJKPEAU_LOGSHIP_INFO@@@Z;
			*(ref _unep@?PrepareInstanceForBackup@InteropCallbacks@HA@Storage@Server@Exchange@Microsoft@@$$FSAJPEAU_ESEBACK_CONTEXT@@_KPEAX@Z + 48) = <Module>.__unep@?ServerAccessCheck@InteropCallbacks@HA@Storage@Server@Exchange@Microsoft@@$$FSAJXZ;
			*(ref _unep@?PrepareInstanceForBackup@InteropCallbacks@HA@Storage@Server@Exchange@Microsoft@@$$FSAJPEAU_ESEBACK_CONTEXT@@_KPEAX@Z + 56) = <Module>.__unep@?ErrESECBTrace@NativeCallbacks@HA@Storage@Server@Exchange@Microsoft@@$$FSAJQEBDZZ;
			Guid guid = crimsonPublisher;
			ref void void& = ref guid;
			_GUID guid2;
			cpblk(ref guid2, ref void&, 16);
			ushort* ptr = null;
			ushort* ptr2 = null;
			int result;
			try
			{
				ptr = StructConversion.WszFromString(displayName);
				ptr2 = StructConversion.WszFromString(endpointAnnotation);
				result = <Module>.HrESEBackupRestoreRegister2(ptr, flags, ptr2, ref _unep@?PrepareInstanceForBackup@InteropCallbacks@HA@Storage@Server@Exchange@Microsoft@@$$FSAJPEAU_ESEBACK_CONTEXT@@_KPEAX@Z, ref guid2);
			}
			finally
			{
				StructConversion.FreeWsz(ptr);
				StructConversion.FreeWsz(ptr2);
			}
			return result;
		}

		public static int BackupRestoreUnregister()
		{
			int result = <Module>.HrESEBackupRestoreUnregister();
			EsebackCallbacks.ManagedCallbacks = null;
			return result;
		}

		[Flags]
		public enum Flags : uint
		{
			BACKUP_DATA_TRANSFER_SHAREDMEM = 2048U,
			BACKUP_DATA_TRANSFER_SOCKETS = 1024U,
			REGISTER_BACKUP_LOCAL = 4096U,
			REGISTER_BACKUP_REMOTE = 8192U,
			REGISTER_ONLINE_RESTORE_LOCAL = 16384U,
			REGISTER_ONLINE_RESTORE_REMOTE = 32768U,
			REGISTER_SURROGATE_BACKUP_LOCAL = 4194304U,
			REGISTER_SURROGATE_BACKUP_REMOTE = 8388608U,
			REGISTER_LOGSHIP_SEED_LOCAL = 16777216U,
			REGISTER_LOGSHIP_SEED_REMOTE = 33554432U,
			REGISTER_LOGSHIP_UPDATER_LOCAL = 67108864U,
			REGISTER_LOGSHIP_UPDATER_REMOTE = 134217728U,
			REGISTER_EXCHANGE_SERVER_LOCAL = 268435456U,
			REGISTER_EXCHANGE_SERVER_REMOTE = 536870912U
		}
	}
}
