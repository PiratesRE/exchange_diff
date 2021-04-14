using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.Cluster.ReplicaVssWriter
{
	internal class BackupInstance
	{
		public BackupInstance(Guid vssIdCurrentSnapshotSeId)
		{
			this.m_vssIdCurrentSnapshotSetId = vssIdCurrentSnapshotSeId;
			this.m_vssbackuptype = 0;
			this.m_fNoComponents = false;
			this.m_fFrozen = false;
			this.m_fSnapPrepared = false;
			this.m_fPostSnapshot = false;
			this.m_fBackupPrepared = false;
			this.m_StorageGroupBackups = new List<StorageGroupBackup>();
		}

		public Guid m_vssIdCurrentSnapshotSetId;

		public int m_vssbackuptype;

		public bool m_fNoComponents;

		public bool m_fFrozen;

		public bool m_fSnapPrepared;

		public bool m_fPostSnapshot;

		public bool m_fBackupPrepared;

		public List<StorageGroupBackup> m_StorageGroupBackups;
	}
}
