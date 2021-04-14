using System;
using System.Globalization;
using System.Security.Permissions;
using Microsoft.Exchange.Cluster.ReplicaSeeder;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Cluster.Replay;
using Microsoft.Isam.Esent.Interop;

namespace Microsoft.Exchange.Cluster.Replay
{
	[SecurityPermission(SecurityAction.LinkDemand, UnmanagedCode = true)]
	internal class EseDatabaseReader : SafeBackupContextHandle, IEseDatabaseReader, IDisposable
	{
		internal static IEseDatabaseReader GetEseDatabaseReader(string serverName, Guid dbGuid, string dbName, string dbPath)
		{
			return EseDatabaseReader.hookableFactory.Value(serverName, dbGuid, dbName, dbPath);
		}

		internal static IDisposable SetTestHook(Func<string, Guid, string, string, IEseDatabaseReader> newFactory)
		{
			return EseDatabaseReader.hookableFactory.SetTestHook(newFactory);
		}

		private EseDatabaseReader(string dbName, Guid dbGuid) : base(dbName, dbGuid)
		{
		}

		private static EseDatabaseReader BuildEseDatabaseReader(string serverName, Guid dbGuid, string dbName, string dbPath)
		{
			EseDatabaseReader eseDatabaseReader = new EseDatabaseReader(dbName, dbGuid);
			SafeBackupContextHandle safeBackupContextHandle = eseDatabaseReader;
			SafeBackupContextHandle.GetAndSetIntPtrInCER(serverName, dbName, null, ref safeBackupContextHandle);
			bool flag = false;
			try
			{
				JET_DBINFOMISC jet_DBINFOMISC;
				int databaseInfo = CReplicaSeederInterop.GetDatabaseInfo(eseDatabaseReader.handle, dbPath, out jet_DBINFOMISC);
				if (databaseInfo != 0)
				{
					throw new FailedToGetDatabaseInfo(databaseInfo);
				}
				eseDatabaseReader.m_pageSize = (long)jet_DBINFOMISC.cbPageSize;
				eseDatabaseReader.m_dbPath = dbPath;
				if (jet_DBINFOMISC.cbPageSize != 4096 && jet_DBINFOMISC.cbPageSize != 8192 && jet_DBINFOMISC.cbPageSize != 32768)
				{
					throw new UnExpectedPageSizeException(dbPath, (long)jet_DBINFOMISC.cbPageSize);
				}
				flag = true;
			}
			finally
			{
				if (!flag)
				{
					eseDatabaseReader.ReleaseHandle();
				}
			}
			return eseDatabaseReader;
		}

		public static IEseDatabaseReader GetRemoteEseDatabaseReader(string serverName, Guid databaseGuid, bool useClassicEseback)
		{
			string text;
			string text2;
			SeedHelper.GetDatabaseNameAndPath(databaseGuid, out text, out text2);
			if (useClassicEseback)
			{
				return EseDatabaseReader.GetEseDatabaseReader(serverName, databaseGuid, text, text2);
			}
			return new RemoteEseDatabaseReader(serverName, databaseGuid, text, text2);
		}

		public void ForceNewLog()
		{
			ExTraceGlobals.IncrementalReseederTracer.TraceDebug((long)this.GetHashCode(), "rolling a log file");
			CReplicaSeederInterop.ForceNewLog(this.handle);
		}

		public byte[] ReadOnePage(long pageNumber, out long lowGen, out long highGen)
		{
			if (pageNumber < 1L)
			{
				throw new ArgumentOutOfRangeException(string.Format(CultureInfo.CurrentCulture, "pageNumber is {0}, must be >= 1 ", new object[]
				{
					pageNumber
				}));
			}
			byte[] result = new byte[this.m_pageSize];
			long num2;
			int num = CReplicaSeederInterop.OnlineGetDatabasePages(this.handle, this.m_dbPath, (ulong)pageNumber, 1UL, (ulong)this.m_pageSize, out result, out num2, out lowGen, out highGen);
			if (num == -4001)
			{
				throw new JetErrorFileIOBeyondEOFException(pageNumber.ToString());
			}
			if (num != 0)
			{
				throw new FailedToReadDatabasePage(num);
			}
			DiagCore.RetailAssert(num2 == this.m_pageSize, "cRead {0} != m_pageSize {1}", new object[]
			{
				num2,
				this.m_pageSize
			});
			return result;
		}

		public long PageSize
		{
			get
			{
				return this.m_pageSize;
			}
		}

		public long ReadPageSize()
		{
			return this.m_pageSize;
		}

		private static Hookable<Func<string, Guid, string, string, IEseDatabaseReader>> hookableFactory = Hookable<Func<string, Guid, string, string, IEseDatabaseReader>>.Create(false, new Func<string, Guid, string, string, IEseDatabaseReader>(EseDatabaseReader.BuildEseDatabaseReader));

		private long m_pageSize;

		private string m_dbPath;
	}
}
