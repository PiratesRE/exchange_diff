using System;
using System.IO;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Diagnostics.Components.Transport;
using Microsoft.Isam.Esent.Interop;
using Microsoft.Office.Datacenter.ActiveMonitoring;

namespace Microsoft.Exchange.Transport.Storage.IPFiltering
{
	internal class Database : ITransportComponent
	{
		public static DataSource DataSource
		{
			get
			{
				return Database.dataSource;
			}
		}

		public static IPFilteringTable Table
		{
			get
			{
				return Database.table;
			}
		}

		public static void Attach()
		{
			try
			{
				Database.SetDefaultDatabasePath();
				Database.LoadConfig();
				Database.dataSource = new DataSource(Strings.IPFilterDatabaseInstanceName, Database.databasePath, "IpFiltering.edb", 1, null, Database.logFilePath, null);
				Database.dataSource.LogBuffers = Database.logBufferSize;
				Database.dataSource.LogFileSize = Database.logFileSize;
				Database.dataSource.OpenDatabase();
				Database.VersionTable versionTable = new Database.VersionTable();
				using (DataConnection dataConnection = Database.dataSource.DemandNewConnection())
				{
					versionTable.Attach(Database.dataSource, dataConnection);
					Database.table.Attach(Database.dataSource, dataConnection);
				}
				versionTable.Detach();
				IPFilterLists.Load();
			}
			catch (EsentFileAccessDeniedException ex)
			{
				Components.EventLogger.LogEvent(TransportEventLogConstants.Tuple_DatabaseInUse, null, new object[]
				{
					Strings.IPFilterDatabaseInstanceName,
					ex
				});
				string notificationReason = string.Format("Database {0} is already in use. The service will be stopped. Exception details: {1}", Strings.IPFilterDatabaseInstanceName, ex);
				EventNotificationItem.Publish(ExchangeComponent.Transport.Name, "TransportServiceStartError", null, notificationReason, ResultSeverityLevel.Error, false);
				throw new TransportComponentLoadFailedException(Strings.DataBaseInUse(Strings.IPFilterDatabaseInstanceName), ex);
			}
			catch (SchemaException inner)
			{
				throw new TransportComponentLoadFailedException(Strings.DatabaseAttachFailed(Strings.IPFilterDatabaseInstanceName), inner);
			}
		}

		public static void Detach()
		{
			IPFilterLists.Cleanup();
			Database.Table.Detach();
			Database.dataSource.CloseDatabase(false);
			Database.dataSource = null;
		}

		public void Load()
		{
			Database.Attach();
		}

		public void Unload()
		{
			Database.Detach();
		}

		public string OnUnhandledException(Exception e)
		{
			return null;
		}

		private static void SetDefaultDatabasePath()
		{
			Database.databasePath = Path.Combine(ConfigurationContext.Setup.InstallPath, "TransportRoles\\data\\IpFilter\\");
			Database.logFilePath = Database.databasePath;
		}

		private static void LoadConfig()
		{
			if (!string.IsNullOrEmpty(Components.TransportAppConfig.IPFilteringDatabase.DatabasePath))
			{
				Database.databasePath = Components.TransportAppConfig.IPFilteringDatabase.DatabasePath;
			}
			if (!string.IsNullOrEmpty(Components.TransportAppConfig.IPFilteringDatabase.LogFilePath))
			{
				Database.logFilePath = Components.TransportAppConfig.IPFilteringDatabase.LogFilePath;
			}
			Database.logFileSize = Components.TransportAppConfig.IPFilteringDatabase.LogFileSize;
			Database.logBufferSize = Components.TransportAppConfig.IPFilteringDatabase.LogBufferSize;
		}

		private static DataSource dataSource;

		private static string databasePath;

		private static string logFilePath;

		private static uint logFileSize = 524288U;

		private static uint logBufferSize = 5120U;

		private static IPFilteringTable table = new IPFilteringTable();

		private class VersionTable : DataTable
		{
			protected override void AttachLoadInitValues(Transaction transaction, DataTableCursor cursor)
			{
				DataColumn<long> dataColumn = (DataColumn<long>)base.Schemas[0];
				if (cursor.Connection.Source.NewDatabase)
				{
					cursor.PrepareInsert(false, false);
					dataColumn.WriteToCursor(cursor, 2L);
					cursor.Update();
					return;
				}
				cursor.MoveFirst();
				long num = dataColumn.ReadFromCursor(cursor);
				ExTraceGlobals.GeneralTracer.TraceDebug<long, long>((long)this.GetHashCode(), "IP Filtering database opened with version: {0} required: {1}", num, 2L);
				if (num != 2L)
				{
					string text = string.Empty;
					string text2 = string.Empty;
					try
					{
						text = cursor.Connection.Source.DatabasePath;
						text2 = cursor.Connection.Source.LogFilePath;
					}
					catch (Exception)
					{
					}
					Components.EventLogger.LogEvent(TransportEventLogConstants.Tuple_DatabaseSchemaNotSupported, null, new object[]
					{
						Strings.IPFilterDatabaseInstanceName,
						num,
						2L,
						text,
						text2
					});
					throw new TransportComponentLoadFailedException(Strings.DatabaseSchemaNotSupported(Strings.IPFilterDatabaseInstanceName));
				}
			}

			[DataColumnDefinition(typeof(long), ColumnAccess.CachedProp, Required = true)]
			public const int Version = 0;

			private const long RequiredVersion = 2L;
		}
	}
}
