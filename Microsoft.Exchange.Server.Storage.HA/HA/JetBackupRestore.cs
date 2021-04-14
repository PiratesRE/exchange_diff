using System;
using Microsoft.Exchange.Diagnostics.Components.ManagedStore.HA;
using Microsoft.Isam.Esent.Interop;

namespace Microsoft.Exchange.Server.Storage.HA
{
	internal static class JetBackupRestore
	{
		public static int RegisterEseback(Guid? instanceId)
		{
			string text = "Microsoft Information Store";
			if (instanceId != null)
			{
				text = instanceId.Value.ToString();
			}
			ESEBACK_CALLBACKS callbacks = new ESEBACK_CALLBACKS
			{
				pfnPrepareInstance = new PfnErrESECBPrepareInstanceForBackup(EsebackCallbacks.PrepareInstanceForBackup),
				pfnDoneWithInstance = new PfnErrESECBDoneWithInstanceForBackup(EsebackCallbacks.DoneWithInstanceForBackup),
				pfnGetDatabasesInfo = new PfnErrESECBGetDatabasesInfo(EsebackCallbacks.GetDatabasesInfo),
				pfnIsSGReplicated = new PfnErrESECBIsSGReplicated(EsebackCallbacks.IsSGReplicated),
				pfnServerAccessCheck = new PfnErrESECBServerAccessCheck(EsebackCallbacks.ServerAccessCheck),
				pfnTrace = new PfnErrESECBTrace(EsebackCallbacks.Trace)
			};
			JetBackupRestore.eseback = new Eseback(callbacks);
			int num = JetBackupRestore.eseback.BackupRestoreRegister("Microsoft Exchange Server", BackupRegisterFlags.BACKUP_DATA_TRANSFER_SOCKETS | BackupRegisterFlags.BACKUP_DATA_TRANSFER_SHAREDMEM | BackupRegisterFlags.REGISTER_BACKUP_LOCAL | BackupRegisterFlags.REGISTER_ONLINE_RESTORE_LOCAL | BackupRegisterFlags.REGISTER_SURROGATE_BACKUP_LOCAL | BackupRegisterFlags.REGISTER_SURROGATE_BACKUP_REMOTE | BackupRegisterFlags.REGISTER_LOGSHIP_SEED_LOCAL | BackupRegisterFlags.REGISTER_LOGSHIP_SEED_REMOTE | BackupRegisterFlags.REGISTER_LOGSHIP_UPDATER_LOCAL | BackupRegisterFlags.REGISTER_LOGSHIP_UPDATER_REMOTE | BackupRegisterFlags.REGISTER_EXCHANGE_SERVER_LOCAL | BackupRegisterFlags.REGISTER_EXCHANGE_SERVER_REMOTE, text, JetBackupRestore.crimsonPublisher);
			ExTraceGlobals.EsebackTracer.TraceFunction(0L, "Registering Eseback({0},{1},{2}) = {3}", new object[]
			{
				"Microsoft Exchange Server",
				BackupRegisterFlags.BACKUP_DATA_TRANSFER_SOCKETS | BackupRegisterFlags.BACKUP_DATA_TRANSFER_SHAREDMEM | BackupRegisterFlags.REGISTER_BACKUP_LOCAL | BackupRegisterFlags.REGISTER_ONLINE_RESTORE_LOCAL | BackupRegisterFlags.REGISTER_SURROGATE_BACKUP_LOCAL | BackupRegisterFlags.REGISTER_SURROGATE_BACKUP_REMOTE | BackupRegisterFlags.REGISTER_LOGSHIP_SEED_LOCAL | BackupRegisterFlags.REGISTER_LOGSHIP_SEED_REMOTE | BackupRegisterFlags.REGISTER_LOGSHIP_UPDATER_LOCAL | BackupRegisterFlags.REGISTER_LOGSHIP_UPDATER_REMOTE | BackupRegisterFlags.REGISTER_EXCHANGE_SERVER_LOCAL | BackupRegisterFlags.REGISTER_EXCHANGE_SERVER_REMOTE,
				text,
				num
			});
			return num;
		}

		public static int UnregisterEseback()
		{
			int num = Eseback.BackupRestoreUnregister();
			ExTraceGlobals.EsebackTracer.TraceFunction<int>(0L, "Unregister Eseback = {0}", num);
			return num;
		}

		private const string DisplayName = "Microsoft Exchange Server";

		private const string EndpointAnnotation = "Microsoft Information Store";

		private const BackupRegisterFlags RegisterFlags = BackupRegisterFlags.BACKUP_DATA_TRANSFER_SOCKETS | BackupRegisterFlags.BACKUP_DATA_TRANSFER_SHAREDMEM | BackupRegisterFlags.REGISTER_BACKUP_LOCAL | BackupRegisterFlags.REGISTER_ONLINE_RESTORE_LOCAL | BackupRegisterFlags.REGISTER_SURROGATE_BACKUP_LOCAL | BackupRegisterFlags.REGISTER_SURROGATE_BACKUP_REMOTE | BackupRegisterFlags.REGISTER_LOGSHIP_SEED_LOCAL | BackupRegisterFlags.REGISTER_LOGSHIP_SEED_REMOTE | BackupRegisterFlags.REGISTER_LOGSHIP_UPDATER_LOCAL | BackupRegisterFlags.REGISTER_LOGSHIP_UPDATER_REMOTE | BackupRegisterFlags.REGISTER_EXCHANGE_SERVER_LOCAL | BackupRegisterFlags.REGISTER_EXCHANGE_SERVER_REMOTE;

		private static readonly Guid crimsonPublisher = new Guid("455E3D31-77EF-4853-94A1-032A399567EC");

		private static Eseback eseback;
	}
}
