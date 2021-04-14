using System;
using System.IO;
using System.Runtime.CompilerServices;
using Microsoft.Exchange.Cluster.ReplayEventLog;
using Microsoft.Exchange.Cluster.ReplicaSeeder;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Cluster.Replay;

namespace Microsoft.Exchange.Cluster.Replay
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class EseDatabaseBackupReader : SafeBackupContextHandle
	{
		public EseDatabaseBackupReader(string dbName, Guid dbGuid) : base(dbName, dbGuid)
		{
			this.m_backupFileHandle = IntPtr.Zero;
		}

		public long SourceFileSizeBytes
		{
			get
			{
				return this.m_sourceFileSizeBytes;
			}
		}

		public IntPtr BackupContextHandle
		{
			get
			{
				return this.handle;
			}
		}

		public IntPtr BackupFileHandle
		{
			get
			{
				return this.m_backupFileHandle;
			}
		}

		protected override bool ReleaseHandle()
		{
			bool result = true;
			if (!this.IsInvalid)
			{
				if (this.m_backupFileHandle != IntPtr.Zero)
				{
					int num = CReplicaSeederInterop.CloseBackupFileHandle(this.handle, this.m_backupFileHandle);
					this.m_backupFileHandle = IntPtr.Zero;
					if (num != 0)
					{
						ExTraceGlobals.SeederServerTracer.TraceError<string, int>((long)this.GetHashCode(), "CloseBackupFileHandle() for database '{0}' failed with error code {1}", base.DatabaseName, num);
						ReplayCrimsonEvents.BackupHandleCloseFailed.Log<Guid, string, string>(base.DatabaseGuid, base.DatabaseName, string.Format("CloseBackupFileHandle() failed with error code {0}", num));
					}
				}
				result = base.ReleaseHandle();
				base.SetHandleAsInvalid();
				EseDatabaseBackupReader.CleanupNativeLogger();
			}
			return result;
		}

		public void SetLastError(int ec)
		{
			this.m_ecLast = ec;
		}

		public int PerformDatabaseRead(long readOffset, byte[] buffer, int bytesToRead)
		{
			int num = bytesToRead;
			int num2 = CReplicaSeederInterop.PerformDatabaseRead(this.handle, this.m_backupFileHandle, readOffset, buffer, ref num);
			if (num2 != 0)
			{
				if (num2 == 38)
				{
					ExTraceGlobals.SeederServerTracer.TraceError<string, int>((long)this.GetHashCode(), "Reading the database '{0}' encountered the end of the file: {1}.", base.DatabaseName, num2);
				}
				else
				{
					ExTraceGlobals.SeederServerTracer.TraceError<string, int>((long)this.GetHashCode(), "Reading the database '{0}' encountered an error: {1}.", base.DatabaseName, num2);
				}
				throw new IOException(string.Format("EseDatabaseBackupReader: PerformDatabaseRead failed with error code 0x{0:X}. Expected {1} bytes read, but actually only {2} were read.", num2, bytesToRead, num));
			}
			return num;
		}

		public static EseDatabaseBackupReader GetESEDatabaseBackupReader(string serverName, string dbName, Guid dbGuid, string transferAddress, string sourceFileToBackupFullPath, uint readHintSizeBytes)
		{
			bool flag = false;
			int num = 0;
			IntPtr value = new IntPtr(-1);
			EseDatabaseBackupReader eseDatabaseBackupReader = new EseDatabaseBackupReader(dbName, dbGuid);
			SafeBackupContextHandle safeBackupContextHandle = eseDatabaseBackupReader;
			EseDatabaseBackupReader result;
			try
			{
				EseDatabaseBackupReader.SetupNativeLogger();
				SafeBackupContextHandle.GetAndSetIntPtrInCER(serverName, dbName, transferAddress, ref safeBackupContextHandle);
				flag = true;
				RuntimeHelpers.PrepareConstrainedRegions();
				try
				{
				}
				finally
				{
					num = CReplicaSeederInterop.OpenBackupFileHandle(eseDatabaseBackupReader.handle, sourceFileToBackupFullPath, readHintSizeBytes, out eseDatabaseBackupReader.m_backupFileHandle, out eseDatabaseBackupReader.m_sourceFileSizeBytes);
				}
				if (num != 0 || eseDatabaseBackupReader.m_backupFileHandle == IntPtr.Zero || eseDatabaseBackupReader.m_backupFileHandle == value)
				{
					eseDatabaseBackupReader.Close();
					throw new FailedToOpenBackupFileHandleException(dbName, serverName, num, SeedHelper.TranslateSeederErrorCode((long)num, serverName));
				}
				result = eseDatabaseBackupReader;
			}
			finally
			{
				if (!flag)
				{
					EseDatabaseBackupReader.CleanupNativeLogger();
				}
			}
			return result;
		}

		public static void SetupNativeLogger()
		{
			CReplicaSeederInterop.SetupNativeLogger();
		}

		public static void CleanupNativeLogger()
		{
			CReplicaSeederInterop.CleanupNativeLogger();
		}

		private IntPtr m_backupFileHandle;

		private long m_sourceFileSizeBytes;
	}
}
