using System;
using Microsoft.Isam.Esent.Interop;

namespace Microsoft.Exchange.Cluster.ReplicaVssWriter
{
	internal class ComponentInformation
	{
		public ComponentInformation()
		{
			this.m_uIndex = 0U;
			this.m_fPostRestore = false;
			this.m_fIncrementalBackupSet = false;
			this.m_fSelectedForRestore = false;
			this.m_fPrivateMdb = false;
			this.m_fSubComponentsExplicitlySelected = false;
			this.m_fLogsSelectedForRestore = false;
			this.m_fDatabaseFileSelectedForRestore = false;
			this.m_fRemappedGuid = false;
			this.m_fLogsRelocated = false;
			this.m_fCheckpointRelocated = false;
			this.m_fCircularLoggingInBackupSet = false;
			this.m_fCircularLoggingInDBTarget = false;
			this.m_fLegacyRequestor = false;
			this.m_fIncrementalRestore = false;
			this.m_fAdditionalRestores = false;
			this.m_fRunRecovery = false;
			this.m_fEDBRelocated = false;
			this.m_fEDBRenamed = false;
			this.m_status = (VSS_FILE_RESTORE_STATUS)0;
			this.m_rstscen = VssRestoreScenario.rstscenUnknown;
		}

		public static string File = "File";

		public static string Logs = "Logs";

		public static string LogsWildCard = "*.log";

		public static string ReservedLogsWildCard = "*.jrs";

		public static string LogsExtension = ".log";

		public static string CheckpointExtension = ".chk";

		public static string RestoreLogs = "_restoredLogs";

		public static string RestoreEnv = "restore.env";

		public static string TempLogfile = "tmp.log";

		public static string VersionStamp = "VERSION_STAMP";

		public static string SupportedVersion = "15";

		public static string DatabaseGuid = "DATABASE_GUID";

		public static string DatabaseGuidOriginal = "DATABASE_GUID_ORIGINAL";

		public static string DatabaseGuidTarget = "DATABASE_GUID_TARGET";

		public static string SystemPathOriginal = "SYSTEM_PATH_ORIGINAL";

		public static string SystemPathTarget = "SYSTEM_PATH_TARGET";

		public static string LogSignatureId = "LOG_SIGNATURE_ID";

		public static string LogSignatureTimestamp = "LOG_SIGNATURE_TIMESTAMP";

		public static string LogBaseName = "LOG_BASE_NAME";

		public static string LogPathOriginal = "LOG_PATH_ORIGINAL";

		public static string LogPathTarget = "LOG_PATH_TARGET";

		public static string CircularLogging = "CIRCULAR_LOGGING";

		public static string Recovery = "RECOVERY";

		public static string RestoreEnvironment = "<?xml version='1.0'?><DATABASE_RESTORE_ENVIRONMENT></DATABASE_RESTORE_ENVIRONMENT>";

		public static string PrivateMdb = "PRIVATE_MDB";

		public static string EdbLocationOriginal = "EDB_LOCATION_ORIGINAL";

		public static string EdbLocationTarget = "EDB_LOCATION_TARGET";

		public static string EdbFilenameOriginal = "EDB_FILENAME_ORIGINAL";

		public static string EdbFilenameTarget = "EDB_FILENAME_TARGET";

		public static string YES = "Yes";

		public static string NO = "No";

		public uint m_uIndex;

		public bool m_fPostRestore;

		public bool m_fIncrementalBackupSet;

		public bool m_fSelectedForRestore;

		public bool m_fPrivateMdb;

		public bool m_fSubComponentsExplicitlySelected;

		public bool m_fLogsSelectedForRestore;

		public bool m_fDatabaseFileSelectedForRestore;

		public bool m_fRemappedGuid;

		public bool m_fLogsRelocated;

		public bool m_fCheckpointRelocated;

		public bool m_fCircularLoggingInBackupSet;

		public bool m_fCircularLoggingInDBTarget;

		public bool m_fLegacyRequestor;

		public bool m_fIncrementalRestore;

		public bool m_fAdditionalRestores;

		public bool m_fRunRecovery;

		public bool m_fEDBRelocated;

		public bool m_fEDBRenamed;

		public VSS_FILE_RESTORE_STATUS m_status;

		public VssRestoreScenario m_rstscen;

		public ValueType m_guidDBOriginal;

		public ValueType m_guidDBTarget;

		public string m_logBaseName;

		public string m_logPathOriginal;

		public string m_logPathTarget;

		public string m_systemPathOriginal;

		public string m_systemPathTarget;

		public string m_edbFilenameOriginal;

		public string m_edbFilenameTarget;

		public string m_edbLocationOriginal;

		public string m_edbLocationTarget;

		public string m_displayNameTarget;

		public string m_restoreEnv;

		public string m_restoreEnvXml;

		public JET_SIGNATURE m_signLog;
	}
}
