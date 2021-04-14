using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using Microsoft.Exchange.Common.HA;
using Microsoft.Exchange.Server.Storage.Common;

namespace Microsoft.Exchange.Server.Storage.PhysicalAccess
{
	public abstract class Database
	{
		protected Database(string displayName, string logPath, string filePath, string fileName, DatabaseFlags databaseFlags, DatabaseOptions databaseOptions)
		{
			this.displayName = displayName;
			this.logPath = logPath;
			this.filePath = filePath;
			this.fileName = fileName;
			this.databaseOptions = databaseOptions;
			this.databaseFlags = databaseFlags;
			if (string.IsNullOrEmpty(this.fileName))
			{
				this.fileName = this.displayName + ".EDB";
			}
			this.perfInstance = PhysicalAccessPerformanceCounters.GetInstance(this.DisplayName);
			this.tables = new LockFreeDictionary<string, Table>();
		}

		public string DisplayName
		{
			get
			{
				return this.displayName;
			}
		}

		public DatabaseFlags Flags
		{
			get
			{
				return this.databaseFlags;
			}
		}

		public abstract int PageSize { get; }

		public DatabaseOptions DatabaseOptions
		{
			get
			{
				return this.databaseOptions;
			}
		}

		internal PhysicalAccessPerformanceCountersInstance PerfInstance
		{
			get
			{
				return this.perfInstance;
			}
		}

		public string FilePath
		{
			get
			{
				return this.filePath;
			}
		}

		public string FileName
		{
			get
			{
				return this.fileName;
			}
		}

		public string LogPath
		{
			get
			{
				return this.logPath;
			}
		}

		public abstract DatabaseType DatabaseType { get; }

		public abstract ILogReplayStatus LogReplayStatus { get; }

		public virtual void Configure()
		{
		}

		public abstract bool TryOpen(bool lossyMount);

		public abstract bool TryCreate(bool force);

		public abstract void Close();

		public abstract void Delete(bool deleteFiles);

		public virtual void ResetDatabaseEngine()
		{
		}

		public virtual DatabaseHeaderInfo GetDatabaseHeaderInfo(IConnectionProvider connectionProvider)
		{
			return new DatabaseHeaderInfo(null, DateTime.MinValue, 0, 0);
		}

		public virtual bool IsDatabaseEngineBusy(out string highResourceUsageType, out long currentResourceUsage, out long maxResourceUsage)
		{
			highResourceUsageType = null;
			currentResourceUsage = 0L;
			maxResourceUsage = 0L;
			return false;
		}

		public virtual void VersionStoreCleanup(IConnectionProvider connectionProvider)
		{
		}

		public virtual void ExtendDatabase(IConnectionProvider connectionProvider)
		{
		}

		public virtual void ShrinkDatabase(IConnectionProvider connectionProvider)
		{
		}

		public virtual void StartBackgroundChecksumming(IConnectionProvider connectionProvider)
		{
		}

		internal virtual void PublishHaFailure(FailureTag failureTag)
		{
		}

		public abstract void CreatePhysicalSchemaObjects(IConnectionProvider connectionProvider);

		public void AddTableMetadata(Table table)
		{
			this.tables.Add(table.Name, table);
		}

		public void RemoveTableMetadata(string tableName)
		{
			this.tables.Remove(tableName);
		}

		public Table GetTableMetadata(string tableName)
		{
			Table result;
			if (this.tables.TryGetValue(tableName, out result))
			{
				return result;
			}
			return null;
		}

		public IEnumerable<Table> GetAllTableMetadata()
		{
			return this.tables.Values;
		}

		public virtual void PopulateTableMetadataFromDatabase()
		{
		}

		public abstract void GetDatabaseSize(IConnectionProvider connectionProvider, out uint totalPages, out uint availablePages, out uint pageSize);

		public abstract void ForceNewLog(IConnectionProvider connectionProvider);

		public abstract IEnumerable<string> GetTableNames(IConnectionProvider connectionProvider);

		public abstract void StartLogReplay(Func<bool, bool> passiveDatabaseAttachDetachHandler);

		public abstract void FinishLogReplay();

		public abstract void CancelLogReplay();

		public abstract bool CheckTableExists(string tableName);

		public abstract void GetSerializedDatabaseInformation(IConnectionProvider connectionProvider, out byte[] databaseInfo);

		public abstract void GetLastBackupInformation(IConnectionProvider connectionProvider, out DateTime fullBackupTime, out DateTime incrementalBackupTime, out DateTime differentialBackupTime, out DateTime copyBackupTime, out int snapFull, out int snapIncremental, out int snapDifferential, out int snapCopy);

		public abstract void SnapshotPrepare(uint flags);

		public abstract void SnapshotFreeze(uint flags);

		public abstract void SnapshotThaw(uint flags);

		public abstract void SnapshotTruncateLogInstance(uint flags);

		public abstract void SnapshotStop(uint flags);

		internal int GetMaxCachePages()
		{
			int num;
			if (this.databaseOptions != null && this.databaseOptions.MaxCachePages != null && this.databaseOptions.MaxCachePages > 0)
			{
				num = this.databaseOptions.MaxCachePages.Value;
			}
			else
			{
				num = this.GetCacheSize();
				if (this.databaseOptions != null && this.databaseOptions.MinCachePages != null && this.databaseOptions.MinCachePages > 0 && num < this.databaseOptions.MinCachePages)
				{
					num = this.databaseOptions.MinCachePages.Value;
				}
			}
			return num;
		}

		internal int GetMinCachePages()
		{
			int num;
			if (this.databaseOptions != null && this.databaseOptions.MinCachePages != null && this.databaseOptions.MinCachePages > 0)
			{
				num = this.databaseOptions.MinCachePages.Value;
			}
			else
			{
				num = this.GetCacheSize();
				if (this.databaseOptions != null && this.databaseOptions.MaxCachePages != null && this.databaseOptions.MaxCachePages > 0 && num > this.databaseOptions.MaxCachePages)
				{
					num = this.databaseOptions.MaxCachePages.Value;
				}
			}
			return num;
		}

		protected void OpenPerfCounterInstance()
		{
			if (!PhysicalAccessPerformanceCounters.InstanceExists(this.DisplayName))
			{
				PhysicalAccessPerformanceCounters.GetInstance(this.DisplayName);
				return;
			}
			PhysicalAccessPerformanceCounters.ResetInstance(this.DisplayName);
		}

		protected void ClosePerfCounterInstance()
		{
			if (PhysicalAccessPerformanceCounters.InstanceExists(this.DisplayName))
			{
				PhysicalAccessPerformanceCounters.CloseInstance(this.DisplayName);
				PhysicalAccessPerformanceCounters.RemoveInstance(this.DisplayName);
			}
		}

		protected void DeleteFileOrDirectory(string pathName)
		{
			try
			{
				if (File.Exists(pathName))
				{
					File.Delete(pathName);
					Globals.LogEvent(MSExchangeISEventLogConstants.Tuple_FileDeleted, new object[]
					{
						this.DisplayName,
						pathName
					});
				}
				else if (Directory.Exists(pathName))
				{
					Directory.Delete(pathName, true);
					Globals.LogEvent(MSExchangeISEventLogConstants.Tuple_DirectoryDeleted, new object[]
					{
						this.DisplayName,
						pathName
					});
				}
			}
			catch (IOException ex)
			{
				Globals.LogEvent(MSExchangeISEventLogConstants.Tuple_IOExceptionDetected, new object[]
				{
					ex.Message,
					ex.StackTrace,
					this.DisplayName
				});
				throw new CouldNotCreateDatabaseException(ex.Message, ex);
			}
			catch (UnauthorizedAccessException ex2)
			{
				Globals.LogEvent(MSExchangeISEventLogConstants.Tuple_IOExceptionDetected, new object[]
				{
					ex2.Message,
					ex2.StackTrace,
					this.DisplayName
				});
				throw new CouldNotCreateDatabaseException(ex2.Message, ex2);
			}
		}

		protected void CreateDirectory(string directoryPathName)
		{
			if (Database.IsFile(directoryPathName))
			{
				this.DeleteFileOrDirectory(directoryPathName);
			}
			try
			{
				Directory.CreateDirectory(directoryPathName);
			}
			catch (IOException ex)
			{
				Globals.LogEvent(MSExchangeISEventLogConstants.Tuple_IOExceptionDetected, new object[]
				{
					ex.Message,
					ex.StackTrace,
					this.DisplayName
				});
				throw new CouldNotCreateDatabaseException(ex.Message, ex);
			}
			catch (UnauthorizedAccessException ex2)
			{
				Globals.LogEvent(MSExchangeISEventLogConstants.Tuple_IOExceptionDetected, new object[]
				{
					ex2.Message,
					ex2.StackTrace,
					this.DisplayName
				});
				throw new CouldNotCreateDatabaseException(ex2.Message, ex2);
			}
		}

		private static long GetTotalMemory()
		{
			if (Database.TotalMemoryForTest != null)
			{
				return Database.TotalMemoryForTest.Value;
			}
			NativeMemoryMethods.MemoryStatusEx memoryStatusEx = default(NativeMemoryMethods.MemoryStatusEx);
			memoryStatusEx.Init();
			ulong result = 0UL;
			if (NativeMemoryMethods.GlobalMemoryStatusEx(ref memoryStatusEx))
			{
				result = memoryStatusEx.TotalPhys;
			}
			else
			{
				Marshal.GetLastWin32Error();
				Globals.AssertRetail(false, "Call to GlobalMemoryStatusEx failed");
			}
			return (long)result;
		}

		private static bool IsFile(string path)
		{
			return File.Exists(path);
		}

		private int GetCacheSize()
		{
			long totalMemory = Database.GetTotalMemory();
			int num = (int)(totalMemory / (long)this.PageSize);
			int databaseCacheSizePercentage = DefaultSettings.Get.DatabaseCacheSizePercentage;
			int num2 = 0;
			int num3 = num * databaseCacheSizePercentage / 100;
			int num4 = (int)((long)num2 * 1024L * 1024L * 1024L / (long)this.PageSize);
			num3 -= num4;
			return Math.Max(3200, num3);
		}

		internal const int MinimumExpectedDatabases = 4;

		internal const int MinimumPages = 3200;

		[ThreadStatic]
		internal static long? TotalMemoryForTest;

		private readonly PhysicalAccessPerformanceCountersInstance perfInstance;

		private readonly DatabaseFlags databaseFlags;

		private readonly string displayName;

		private readonly string logPath;

		private readonly string filePath;

		private readonly string fileName;

		private LockFreeDictionary<string, Table> tables;

		private DatabaseOptions databaseOptions;

		protected enum SnapshotState
		{
			Null,
			Prepared,
			Frozen,
			Thawed
		}
	}
}
