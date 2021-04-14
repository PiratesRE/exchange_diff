using System;
using System.Runtime.InteropServices;
using Microsoft.Isam.Esent.Interop;

namespace Microsoft.Exchange.Cluster.ReplicaVssWriter
{
	internal class StorageGroupBackup
	{
		public StorageGroupBackup(string serverName, Guid guidReplica, [MarshalAs(UnmanagedType.U1)] bool fIsPassive)
		{
			this.m_serverName = serverName;
			this.m_guidSGIdentityGuid = guidReplica;
			this.m_fIsPassive = fIsPassive;
			this.m_fComponentBackup = false;
			this.m_fFrozen = false;
			this.m_fSurrogateBackupBegun = false;
			this.m_hccx = null;
		}

		public StorageGroupBackup(string serverName, Guid guidReplica, JET_SIGNATURE logfileSignature, long lowestGenerationRequired, long highestGenerationRequired, string destinationLogPath, string logFilePrefix, string logExtension, [MarshalAs(UnmanagedType.U1)] bool fIsPassive)
		{
			this.m_serverName = serverName;
			this.m_guidSGIdentityGuid = guidReplica;
			this.m_fComponentBackup = true;
			this.m_logfileSignature = logfileSignature;
			this.m_lowestGenerationRequired = lowestGenerationRequired;
			this.m_highestGenerationRequired = highestGenerationRequired;
			this.m_fFrozen = false;
			this.m_fSurrogateBackupBegun = false;
			this.m_hccx = null;
			this.m_destinationLogPath = destinationLogPath;
			this.m_logFilePrefix = logFilePrefix;
			this.m_logExtension = logExtension;
			this.m_fIsPassive = fIsPassive;
		}

		public string m_serverName;

		public Guid m_guidSGIdentityGuid;

		public bool m_fComponentBackup;

		public JET_SIGNATURE m_logfileSignature;

		public long m_lowestGenerationRequired;

		public long m_highestGenerationRequired;

		public bool m_fFrozen;

		public bool m_fSurrogateBackupBegun;

		public unsafe void* m_hccx;

		public string m_destinationLogPath;

		public string m_logFilePrefix;

		public string m_logExtension;

		public bool m_fIsPassive;
	}
}
