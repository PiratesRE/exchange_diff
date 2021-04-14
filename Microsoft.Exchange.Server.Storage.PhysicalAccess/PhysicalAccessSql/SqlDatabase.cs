using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.ManagedStore.PhysicalAccess;
using Microsoft.Exchange.Server.Storage.Common;
using Microsoft.Exchange.Server.Storage.PhysicalAccess;

namespace Microsoft.Exchange.Server.Storage.PhysicalAccessSql
{
	internal class SqlDatabase : Database
	{
		internal SqlDatabase(string mdbName, string logPath, string filePath, string fileName, DatabaseFlags databaseFlags, DatabaseOptions databaseOptions) : base(mdbName, logPath, filePath, fileName, databaseFlags, databaseOptions)
		{
			this.connectionString = DatabaseLocation.GetConnectionString(base.DisplayName);
		}

		public override int PageSize
		{
			get
			{
				return 8192;
			}
		}

		public override DatabaseType DatabaseType
		{
			get
			{
				return DatabaseType.Sql;
			}
		}

		internal string ConnectionString
		{
			get
			{
				return this.connectionString;
			}
		}

		public override ILogReplayStatus LogReplayStatus
		{
			get
			{
				throw new StoreException((LID)56776U, ErrorCodeValue.NotSupported, "LogReplayStatus.Get is not supported for SQL database.");
			}
		}

		public override void Configure()
		{
			this.DoFirstMountProcessing();
		}

		public override bool TryOpen(bool lossyMount)
		{
			try
			{
				using (LockManager.Lock(SqlDatabase.configurationLockObject))
				{
					using (SqlConnection sqlConnection = new SqlConnection(null, "SqlDatabase.TryOpen"))
					{
						if (!this.DatabaseExists(sqlConnection))
						{
							return false;
						}
					}
					using (SqlConnection sqlConnection2 = new SqlConnection(this, "SqlDatabase.TryOpen"))
					{
						this.BeforeMountDatabase(sqlConnection2);
					}
					this.isOpen = true;
				}
			}
			catch (SqlException ex)
			{
				Microsoft.Exchange.Server.Storage.Common.Globals.LogEvent(MSExchangeISEventLogConstants.Tuple_SQLExceptionDetected, new object[]
				{
					ex.Message,
					ex.StackTrace,
					"SqlDatabase.TryOpen"
				});
				throw new CouldNotCreateDatabaseException(ex.Message, ex);
			}
			return true;
		}

		public override bool TryCreate(bool force)
		{
			try
			{
				using (LockManager.Lock(SqlDatabase.configurationLockObject))
				{
					using (SqlConnection sqlConnection = new SqlConnection(null, "SqlDatabase.TryCreate"))
					{
						string text = Path.Combine(base.FilePath, base.DisplayName + ".mdf");
						if (this.DatabaseExists(sqlConnection))
						{
							if (!force)
							{
								return false;
							}
							this.Delete(true);
						}
						else if (File.Exists(text) && !force)
						{
							return false;
						}
						string text2 = Path.Combine(base.FilePath, base.DisplayName + ".ndf");
						string text3 = Path.Combine(base.LogPath, base.DisplayName + ".ldf");
						string directoryName = Path.GetDirectoryName(typeof(Database).Assembly.Location);
						string text4 = Path.Combine(Path.GetDirectoryName(directoryName), "Logging");
						base.CreateDirectory(base.FilePath);
						base.CreateDirectory(base.LogPath);
						base.CreateDirectory(text4);
						base.DeleteFileOrDirectory(text);
						base.DeleteFileOrDirectory(text3);
						int num = 1;
						for (;;)
						{
							string text5 = text2 + num.ToString();
							if (!File.Exists(text5) && !Directory.Exists(text5))
							{
								break;
							}
							base.DeleteFileOrDirectory(text5);
							num++;
						}
						this.CreateDatabase(sqlConnection, text, text3, text2, directoryName, text4);
					}
					using (SqlConnection sqlConnection2 = new SqlConnection(this, "SqlDatabase.TryCreate"))
					{
						this.BeforeMountDatabase(sqlConnection2);
					}
					this.isOpen = true;
				}
			}
			catch (SqlException ex)
			{
				Microsoft.Exchange.Server.Storage.Common.Globals.LogEvent(MSExchangeISEventLogConstants.Tuple_SQLExceptionDetected, new object[]
				{
					ex.Message,
					ex.StackTrace,
					"SqlDatabase.TryCreate"
				});
				throw new CouldNotCreateDatabaseException(ex.Message, ex);
			}
			return true;
		}

		public override void Close()
		{
			if (this.isOpen)
			{
				this.isOpen = false;
			}
		}

		public override void Delete(bool deleteFiles)
		{
			this.Close();
			using (LockManager.Lock(SqlDatabase.configurationLockObject))
			{
				using (SqlConnection sqlConnection = new SqlConnection(null, "DeleteDatabase"))
				{
					if (this.DatabaseExists(sqlConnection))
					{
						sqlConnection.Commit();
						using (SqlCommand sqlCommand = new SqlCommand(sqlConnection))
						{
							sqlCommand.StartNewStatement(Connection.OperationType.Other);
							sqlCommand.Append("ALTER DATABASE [");
							sqlCommand.Append(base.DisplayName);
							sqlCommand.Append("] SET OFFLINE WITH ROLLBACK IMMEDIATE");
							sqlCommand.StartNewStatement(Connection.OperationType.Other);
							sqlCommand.Append("ALTER DATABASE [");
							sqlCommand.Append(base.DisplayName);
							sqlCommand.Append("] SET ONLINE");
							sqlCommand.StartNewStatement(Connection.OperationType.Other);
							sqlCommand.Append("DROP DATABASE [");
							sqlCommand.Append(base.DisplayName);
							sqlCommand.Append("]");
							sqlCommand.ExecuteNonQuery(Connection.TransactionOption.NoTransaction);
						}
					}
				}
				SqlConnection.ClearAllPools();
				if (deleteFiles)
				{
					string path = base.FilePath + "\\" + base.DisplayName + ".mdf";
					string path2 = base.FilePath + "\\" + base.DisplayName + ".ndf1";
					string path3 = base.LogPath + "\\" + base.DisplayName + ".ldf";
					File.Delete(path);
					File.Delete(path2);
					File.Delete(path3);
				}
			}
		}

		public override void CreatePhysicalSchemaObjects(IConnectionProvider connectionProvider)
		{
		}

		public override void PopulateTableMetadataFromDatabase()
		{
		}

		public override bool CheckTableExists(string tableName)
		{
			throw new StoreException((LID)45052U, ErrorCodeValue.NotSupported, "CheckTableExists is not supported for SQL database.");
		}

		public override void GetDatabaseSize(IConnectionProvider connectionProvider, out uint totalPages, out uint availablePages, out uint pageSize)
		{
			throw new StoreException((LID)65032U, ErrorCodeValue.NotSupported, "GetDatabaseSize is not supported for SQL database.");
		}

		public override void GetSerializedDatabaseInformation(IConnectionProvider connectionProvider, out byte[] databaseInfo)
		{
			throw new StoreException((LID)40528U, ErrorCodeValue.NotSupported, "GetDatabaseInformation is not supported for SQL database.");
		}

		public override void GetLastBackupInformation(IConnectionProvider connectionProvider, out DateTime fullBackupTime, out DateTime incrementalBackupTime, out DateTime differentialBackupTime, out DateTime copyBackupTime, out int snapFull, out int snapIncremental, out int snapDifferential, out int snapCopy)
		{
			throw new StoreException((LID)56360U, ErrorCodeValue.NotSupported, "GetDatabaseBackupInformation is not supported for SQL database.");
		}

		public override void SnapshotPrepare(uint flags)
		{
			throw new StoreException((LID)62504U, ErrorCodeValue.NotSupported, "SnapshotPrepare is not supported for SQL database.");
		}

		public override void SnapshotFreeze(uint flags)
		{
			throw new StoreException((LID)37928U, ErrorCodeValue.NotSupported, "SnapshotFreeze is not supported for SQL database.");
		}

		public override void SnapshotThaw(uint flags)
		{
			throw new StoreException((LID)54312U, ErrorCodeValue.NotSupported, "SnapshotThaw is not supported for SQL database.");
		}

		public override void SnapshotTruncateLogInstance(uint flags)
		{
			throw new StoreException((LID)42024U, ErrorCodeValue.NotSupported, "SnapshotTruncateLogInstance is not supported for SQL database.");
		}

		public override void SnapshotStop(uint flags)
		{
			throw new StoreException((LID)58408U, ErrorCodeValue.NotSupported, "SnapshotStop is not supported for SQL database.");
		}

		public override void StartLogReplay(Func<bool, bool> passiveDatabaseAttachDetachHandler)
		{
			throw new StoreException((LID)44488U, ErrorCodeValue.NotSupported, "StartLogReplay is not supported for SQL database.");
		}

		public override void FinishLogReplay()
		{
			throw new StoreException((LID)60872U, ErrorCodeValue.NotSupported, "FinishLogReplay is not supported for SQL database.");
		}

		public override void CancelLogReplay()
		{
			throw new StoreException((LID)36296U, ErrorCodeValue.NotSupported, "CancelLogReplay is not supported for SQL database.");
		}

		public override void ForceNewLog(IConnectionProvider connectionProvider)
		{
			throw new StoreException((LID)42624U, ErrorCodeValue.NotSupported, "ForceNewLog is not supported for SQL database.");
		}

		public override IEnumerable<string> GetTableNames(IConnectionProvider connectionProvider)
		{
			throw new StoreException((LID)56220U, ErrorCodeValue.NotSupported, "GetTableNames is not supported for SQL database.");
		}

		private static void ExecuteAtStartCommand(Connection masterConnection, string statement)
		{
			using (SqlCommand sqlCommand = new SqlCommand(masterConnection))
			{
				sqlCommand.StartNewStatement(Connection.OperationType.Other);
				sqlCommand.Append(statement);
				sqlCommand.ExecuteScalar(Connection.TransactionOption.NoTransaction);
			}
		}

		private void DoFirstMountProcessing()
		{
			if (!SqlDatabase.didFirstMount)
			{
				using (LockManager.Lock(SqlDatabase.configurationLockObject))
				{
					if (!SqlDatabase.didFirstMount)
					{
						using (SqlConnection sqlConnection = new SqlConnection(null, "SqlDatabase.Configure"))
						{
							SqlDatabase.ExecuteAtStartCommand(sqlConnection, "sp_configure 'remote login timeout', '60000'");
							SqlDatabase.ExecuteAtStartCommand(sqlConnection, "sp_configure 'remote query timeout', '60000'");
							SqlDatabase.ExecuteAtStartCommand(sqlConnection, "sp_configure 'show advanced option', '1'");
							SqlDatabase.ExecuteAtStartCommand(sqlConnection, "reconfigure with override");
							SqlDatabase.ExecuteAtStartCommand(sqlConnection, "sp_configure 'max degree of parallelism', '1'");
							SqlDatabase.ExecuteAtStartCommand(sqlConnection, "sys.sp_configure 'recovery interval (min)', '10000'");
							SqlDatabase.ExecuteAtStartCommand(sqlConnection, "sp_configure 'query wait', '60000'");
							SqlDatabase.ExecuteAtStartCommand(sqlConnection, "sp_configure 'max server memory', " + ((long)base.GetMaxCachePages() * (long)this.PageSize / 1048576L).ToString());
							SqlDatabase.ExecuteAtStartCommand(sqlConnection, "sp_configure 'min server memory', " + ((long)base.GetMinCachePages() * (long)this.PageSize / 1048576L).ToString());
							SqlDatabase.ExecuteAtStartCommand(sqlConnection, "reconfigure with override");
						}
						SqlDatabase.didFirstMount = true;
					}
				}
			}
		}

		private void BeforeMountDatabase(SqlConnection connection)
		{
			using (SqlCommand sqlCommand = new SqlCommand(connection, "exec [Exchange].[sp_start_trace]", Connection.OperationType.Other))
			{
				sqlCommand.ExecuteNonQuery(Connection.TransactionOption.NoTransaction);
			}
			if ((base.Flags & DatabaseFlags.CircularLoggingEnabled) != DatabaseFlags.None)
			{
				int num;
				using (SqlCommand sqlCommand2 = new SqlCommand(connection, Connection.OperationType.Query))
				{
					sqlCommand2.Append("SELECT COUNT(*) FROM msdb..backupset WHERE database_name = ");
					sqlCommand2.AppendParameter(base.DisplayName);
					num = (int)sqlCommand2.ExecuteScalar(Connection.TransactionOption.NoTransaction);
				}
				if (num == 0)
				{
					using (SqlCommand sqlCommand3 = new SqlCommand(connection, Connection.OperationType.Other))
					{
						sqlCommand3.Append("BACKUP DATABASE [");
						sqlCommand3.Append(base.DisplayName);
						sqlCommand3.Append("] TO DISK = 'nul'");
						sqlCommand3.ExecuteNonQuery(Connection.TransactionOption.NoTransaction);
					}
				}
			}
		}

		private bool DatabaseExists(Connection masterConnection)
		{
			object obj;
			using (SqlCommand sqlCommand = new SqlCommand(masterConnection, "select count(*) from dbo.sysdatabases where name = '" + base.DisplayName + "'", Connection.OperationType.Query))
			{
				obj = sqlCommand.ExecuteScalar(Connection.TransactionOption.NoTransaction);
			}
			return (int)obj != 0;
		}

		private void CreateDatabase(SqlConnection masterConnection, string primaryFileName, string logName, string dataName, string binaryPath, string diagnosticsPath)
		{
			masterConnection.Commit();
			for (int i = 0; i < ParameterizedSQL.CreateDatabase.Length; i++)
			{
				string text = string.Format(ParameterizedSQL.CreateDatabase[i], new object[]
				{
					base.DisplayName,
					primaryFileName,
					logName,
					dataName,
					binaryPath,
					diagnosticsPath
				});
				try
				{
					using (SqlCommand sqlCommand = new SqlCommand(masterConnection, text, Connection.OperationType.Other))
					{
						sqlCommand.ExecuteNonQuery(Connection.TransactionOption.NoTransaction);
					}
				}
				catch (SqlException ex)
				{
					Microsoft.Exchange.Server.Storage.Common.Globals.LogEvent(MSExchangeISEventLogConstants.Tuple_SQLExceptionDetected, new object[]
					{
						ex.Message,
						ex.StackTrace,
						text.ToString()
					});
					ExTraceGlobals.DbInteractionSummaryTracer.TraceError<string, string>(0L, "CreateDatabase error: Message is {0} Stack trace is {1}", ex.Message, ex.StackTrace);
					throw masterConnection.ProcessSqlError(ex);
				}
			}
		}

		private static bool didFirstMount;

		private static object configurationLockObject = new object();

		private string connectionString;

		private bool isOpen;
	}
}
