using System;
using System.Runtime.CompilerServices;
using System.Security.Permissions;
using Microsoft.Exchange.Cluster.ClusApi;
using Microsoft.Exchange.Cluster.ReplayEventLog;
using Microsoft.Exchange.Cluster.ReplicaSeeder;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Cluster.Replay;

namespace Microsoft.Exchange.Cluster.Replay
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[SecurityPermission(SecurityAction.LinkDemand, UnmanagedCode = true)]
	internal abstract class SafeBackupContextHandle : SafeDisposeTrackerHandleZeroOrMinusOneIsInvalid
	{
		protected SafeBackupContextHandle(string dbName, Guid dbGuid) : base(true)
		{
			this.m_dbName = dbName;
			this.m_dbGuid = dbGuid;
			base.SetHandle(IntPtr.Zero);
		}

		public string DatabaseName
		{
			get
			{
				return this.m_dbName;
			}
		}

		public Guid DatabaseGuid
		{
			get
			{
				return this.m_dbGuid;
			}
		}

		protected static void GetAndSetIntPtrInCER(string serverName, string dbName, string transferAddress, ref SafeBackupContextHandle backupHandle)
		{
			IntPtr zero = IntPtr.Zero;
			IntPtr value = new IntPtr(-1);
			bool flag = false;
			int num = 0;
			RuntimeHelpers.PrepareConstrainedRegions();
			try
			{
			}
			finally
			{
				num = backupHandle.GetBackupContextIntPtr(serverName, dbName, transferAddress, out zero);
				flag = (num == 0 && zero != IntPtr.Zero && zero != value);
				if (flag)
				{
					backupHandle.SetHandle(zero);
				}
			}
			if (!flag)
			{
				throw new FailedToOpenBackupFileHandleException(dbName, serverName, num, SeedHelper.TranslateSeederErrorCode((long)num, serverName));
			}
		}

		protected override bool ReleaseHandle()
		{
			int num = CReplicaSeederInterop.CloseBackupContext(this.handle, this.m_ecLast);
			if (num != 0)
			{
				ExTraceGlobals.SeederServerTracer.TraceError<string, int>((long)this.GetHashCode(), "CloseBackupContext() for database '{0}' failed with error code {1}", this.DatabaseName, num);
				ReplayCrimsonEvents.BackupHandleCloseFailed.Log<Guid, string, string>(this.DatabaseGuid, this.DatabaseName, string.Format("CloseBackupContext() failed with error code {0}", num));
			}
			return num == 0;
		}

		protected int GetBackupContextIntPtr(string serverName, string dbName, string transferAddress, out IntPtr hContext)
		{
			hContext = IntPtr.Zero;
			return CReplicaSeederInterop.OpenBackupContext(serverName, dbName, transferAddress, this.m_dbGuid, out hContext);
		}

		protected int m_ecLast;

		private readonly string m_dbName;

		private readonly Guid m_dbGuid;
	}
}
