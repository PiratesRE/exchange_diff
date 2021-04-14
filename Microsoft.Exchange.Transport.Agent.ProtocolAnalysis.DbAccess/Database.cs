using System;
using System.Globalization;
using System.IO;
using System.Reflection;
using Microsoft.Exchange.Configuration.Common;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Sts;
using Microsoft.Exchange.Transport.Storage;
using Microsoft.Isam.Esent.Interop;

namespace Microsoft.Exchange.Transport.Agent.ProtocolAnalysis.DbAccess
{
	internal class Database
	{
		public static ProtocolAnalysisTable ProtocolAnalysisTable
		{
			get
			{
				return Database.protocolAnalysisTable;
			}
		}

		public static SenderReputationTable SenderReputationTable
		{
			get
			{
				return Database.senderReputationTable;
			}
		}

		public static OpenProxyStatusTable OpenProxyStatusTable
		{
			get
			{
				return Database.openProxyStatusTable;
			}
		}

		public static ConfigurationDataTable ConfigurationDataTable
		{
			get
			{
				return Database.configurationDataTable;
			}
		}

		public static DataSource DataSource
		{
			get
			{
				return Database.dataSource;
			}
		}

		public static string DatabasePath
		{
			get
			{
				return Database.databasePath + Database.databaseName;
			}
			set
			{
				Database.databaseName = Path.GetFileName(value);
				Database.databasePath = Path.GetDirectoryName(value);
				if (string.IsNullOrEmpty(Database.databaseName))
				{
					Database.databaseName = "pasettings.edb";
				}
				if (Database.databasePath[Database.databasePath.Length - 1] != Path.DirectorySeparatorChar)
				{
					Database.databasePath += Path.DirectorySeparatorChar;
				}
			}
		}

		protected static bool IsDbClosed
		{
			get
			{
				return Database.refCount == 0;
			}
		}

		public static void Detach()
		{
			lock (Database.syncObject)
			{
				Database.refCount--;
				if (Database.refCount == 0)
				{
					if (Database.dataSource == null)
					{
						ExTraceGlobals.DatabaseTracer.TraceError(0L, "DbAccess.Detach: refCount == 0 && dataSource == null");
						throw new LocalizedException(DbStrings.DetachRefCountFailed);
					}
					Database.ProtocolAnalysisTable.Detach();
					Database.SenderReputationTable.Detach();
					Database.OpenProxyStatusTable.Detach();
					Database.ConfigurationDataTable.Detach();
					Database.dataSource.CloseDatabase(false);
					Database.dataSource = null;
				}
			}
		}

		public static void Attach()
		{
			try
			{
				lock (Database.syncObject)
				{
					if (Database.refCount != 0 && Database.dataSource != null)
					{
						Database.refCount++;
					}
					else
					{
						if (Database.refCount != 0 || Database.dataSource != null)
						{
							ExTraceGlobals.DatabaseTracer.TraceError(0L, "DbAccess.Attach: refCount != 0 && dataSource == null");
							throw new LocalizedException(DbStrings.AttachRefCountFailed);
						}
						Database.SetDefaultDatabasePath();
						Database.dataSource = new DataSource(DbStrings.DatabaseInstanceName, Database.databasePath, Database.databaseName, Database.maxConnections, Database.perfCounterInstanceName, Database.logFilePath, null);
						Database.dataSource.OpenDatabase();
						using (DataConnection dataConnection = Database.dataSource.DemandNewConnection())
						{
							Database.ProtocolAnalysisTable.Attach(Database.dataSource, dataConnection);
							Database.SenderReputationTable.Attach(Database.dataSource, dataConnection);
							Database.OpenProxyStatusTable.Attach(Database.dataSource, dataConnection);
							Database.ConfigurationDataTable.Attach(Database.dataSource, dataConnection);
						}
						Database.refCount++;
					}
				}
			}
			catch (EsentFileAccessDeniedException innerException)
			{
				throw new ExchangeConfigurationException(Strings.DataBaseInUse("ProtocolAnalysis/Sts Database"), innerException);
			}
			catch (SchemaException innerException2)
			{
				throw new ExchangeConfigurationException(Strings.DatabaseAttachFailed("ProtocolAnalysis/Sts Database"), innerException2);
			}
		}

		public static void Initialize(string dbPath)
		{
			Database.databasePath = dbPath;
			if (!string.IsNullOrEmpty(Database.databasePath) && Database.databasePath[Database.databasePath.Length - 1] != Path.DirectorySeparatorChar)
			{
				Database.databasePath += Path.DirectorySeparatorChar;
			}
			Database.Attach();
		}

		public static void PurgeTable(TimeSpan paDeleteTimeSpan, TimeSpan opDeleteTimeSpan, Trace tracer)
		{
			DateTime cutoffTime = DateTime.UtcNow.Subtract(paDeleteTimeSpan);
			DateTime cutoffTime2 = DateTime.UtcNow.Subtract(opDeleteTimeSpan);
			try
			{
				lock (Database.syncObject)
				{
					if (!Database.IsDbClosed)
					{
						PurgingScanner<ProtocolAnalysisRowData, ProtocolAnalysisTable> purgingScanner = new PurgingScanner<ProtocolAnalysisRowData, ProtocolAnalysisTable>(cutoffTime);
						PurgingScanner<OpenProxyStatusRowData, OpenProxyStatusTable> purgingScanner2 = new PurgingScanner<OpenProxyStatusRowData, OpenProxyStatusTable>(cutoffTime2);
						purgingScanner.Scan();
						tracer.TraceDebug(0L, "Purge complete for ProtocolAnalysis table");
						purgingScanner2.Scan();
						tracer.TraceDebug(0L, "Purge complete for OpenProxyStatus table");
					}
				}
			}
			catch
			{
				tracer.TraceDebug(0L, "PurgeTable: Failed to delete data.");
				throw;
			}
		}

		public static void UpdateSenderReputationData(byte[] senderAddressHash, int senderReputationLevel, bool openProxy, DateTime expirationTime, Trace tracer)
		{
			tracer.TraceDebug<int, bool>(0L, "Update sender reputation> srl:{0}, openproxy:{1}.", senderReputationLevel, openProxy);
			try
			{
				lock (Database.syncObject)
				{
					if (!Database.IsDbClosed)
					{
						SenderReputationRowData senderReputationRowData = DataRowAccessBase<SenderReputationTable, SenderReputationRowData>.Find(senderAddressHash);
						if (senderReputationRowData == null)
						{
							senderReputationRowData = DataRowAccessBase<SenderReputationTable, SenderReputationRowData>.NewData(senderAddressHash);
						}
						senderReputationRowData.Srl = senderReputationLevel;
						senderReputationRowData.OpenProxy = openProxy;
						senderReputationRowData.ExpirationTime = expirationTime;
						senderReputationRowData.Commit();
					}
				}
			}
			catch
			{
				tracer.TraceDebug(0L, "UpdateSenderReputationData: Failed to update data.");
				throw;
			}
		}

		public static void DeleteSenderReputationData(byte[] senderAddressHash, Trace tracer)
		{
			tracer.TraceDebug(0L, "Remove sender reputation.");
			try
			{
				lock (Database.syncObject)
				{
					if (!Database.IsDbClosed)
					{
						SenderReputationRowData senderReputationRowData = DataRowAccessBase<SenderReputationTable, SenderReputationRowData>.Find(senderAddressHash);
						if (senderReputationRowData != null)
						{
							senderReputationRowData.MarkToDelete();
							senderReputationRowData.Commit();
						}
					}
				}
			}
			catch
			{
				tracer.TraceDebug(0L, "DeleteSenderReputationData: Failed to delete data.");
				throw;
			}
		}

		public static void TruncateSenderReputationTable(Trace tracer)
		{
			tracer.TraceDebug(0L, "Truncate sender reputation table.");
			try
			{
				lock (Database.syncObject)
				{
					if (!Database.IsDbClosed)
					{
						PurgingScanner<SenderReputationRowData, SenderReputationTable> purgingScanner = new PurgingScanner<SenderReputationRowData, SenderReputationTable>();
						purgingScanner.Scan();
					}
				}
			}
			catch
			{
				tracer.TraceDebug(0L, "TruncateSenderReputationTable: Failed to delete data.");
				throw;
			}
		}

		public static void UpdateConfiguration(string configName, string configValue, Trace tracer)
		{
			tracer.TraceDebug<string, string>(0L, "Update configuration, name:{0}, value:{1}.", configName, configValue);
			try
			{
				lock (Database.syncObject)
				{
					if (!Database.IsDbClosed)
					{
						ConfigurationDataRowData configurationDataRowData = DataRowAccessBase<ConfigurationDataTable, ConfigurationDataRowData>.Find(configName);
						if (configurationDataRowData == null)
						{
							configurationDataRowData = DataRowAccessBase<ConfigurationDataTable, ConfigurationDataRowData>.NewData(configName);
						}
						configurationDataRowData.ConfigValue = configValue;
						configurationDataRowData.Commit();
					}
				}
			}
			catch
			{
				tracer.TraceDebug(0L, "UpdateConfiguration: Failed to update configuration.");
				throw;
			}
		}

		public static string GetConfiguration(string configName, Trace tracer)
		{
			tracer.TraceDebug<string>(0L, "Lookup configuration, name:{0}.", configName);
			string result = string.Empty;
			try
			{
				lock (Database.syncObject)
				{
					ConfigurationDataRowData configurationDataRowData = DataRowAccessBase<ConfigurationDataTable, ConfigurationDataRowData>.Find(configName);
					if (Database.IsDbClosed)
					{
						return null;
					}
					if (configurationDataRowData == null)
					{
						tracer.TraceDebug<string>(0L, "GetConfiguration: Could not find parameter with name {0}.", configName);
					}
					else
					{
						result = configurationDataRowData.ConfigValue;
					}
				}
			}
			catch
			{
				tracer.TraceDebug<string>(0L, "GetConfiguration: Failed to get configuration value for {0}.", configName);
				throw;
			}
			return result;
		}

		public static PropertyBag ScanSrlConfiguration()
		{
			PropertyBag result;
			lock (Database.syncObject)
			{
				if (Database.IsDbClosed)
				{
					result = Database.propertyBag;
				}
				else
				{
					RowBaseScanner<ConfigurationDataRowData> rowBaseScanner = new RowBaseScanner<ConfigurationDataRowData>(-1, new NextMessage<ConfigurationDataRowData>(Database.HandleSrlSettingsRecord));
					rowBaseScanner.Scan();
					result = Database.propertyBag;
				}
			}
			return result;
		}

		public static void UpdateReverseDns(string senderAddress, string reverseDns, Trace tracer)
		{
			tracer.TraceDebug<string, string>(0L, "Update reverse DNS for sender {0}, reverseDNS {1}", senderAddress, reverseDns);
			try
			{
				lock (Database.syncObject)
				{
					if (!Database.IsDbClosed)
					{
						ProtocolAnalysisRowData protocolAnalysisRowData = DataRowAccessBase<ProtocolAnalysisTable, ProtocolAnalysisRowData>.Find(senderAddress);
						if (protocolAnalysisRowData == null)
						{
							tracer.TraceDebug<string, string>(0L, "Update reverse DNS for sender {0}, reverseDNS {1} failed: sender does not exist in ProtocolAnalysisTable", senderAddress, reverseDns);
						}
						else
						{
							protocolAnalysisRowData.LastQueryTime = DateTime.UtcNow;
							protocolAnalysisRowData.LastUpdateTime = DateTime.UtcNow;
							protocolAnalysisRowData.ReverseDns = reverseDns;
							protocolAnalysisRowData.Processing = false;
							protocolAnalysisRowData.Commit();
						}
					}
				}
			}
			catch
			{
				tracer.TraceDebug<string, string>(0L, "Update reverse DNS for sender {0}, reverseDNS {1} failed.", senderAddress, reverseDns);
				throw;
			}
		}

		public static void UpdateOpenProxy(string senderAddress, OPDetectionResult status, string message, Trace tracer)
		{
			tracer.TraceDebug<string, OPDetectionResult, string>(0L, "Update open proxy status for sender:{0}, status:{1}, comment:{2}", senderAddress, status, message);
			try
			{
				lock (Database.syncObject)
				{
					if (!Database.IsDbClosed)
					{
						OpenProxyStatusRowData openProxyStatusRowData = DataRowAccessBase<OpenProxyStatusTable, OpenProxyStatusRowData>.Find(senderAddress);
						if (openProxyStatusRowData == null)
						{
							tracer.TraceDebug<string>(0L, "Update open proxy for sender {0}, failed: sender does not exist in ProtocolAnalysisTable", senderAddress);
						}
						else
						{
							openProxyStatusRowData.LastDetectionTime = DateTime.UtcNow;
							openProxyStatusRowData.LastAccessTime = DateTime.UtcNow;
							openProxyStatusRowData.OpenProxyStatus = (int)status;
							openProxyStatusRowData.Processing = false;
							openProxyStatusRowData.Message = message;
							openProxyStatusRowData.Commit();
						}
					}
				}
			}
			catch
			{
				tracer.TraceDebug<string>(0L, "Update open proxyfor sender {0}, failed.", senderAddress);
				throw;
			}
		}

		private static void SetDefaultDatabasePath()
		{
			if (Database.databasePath == null)
			{
				string location = Assembly.GetExecutingAssembly().Location;
				string directoryName = Path.GetDirectoryName(location);
				Database.databasePath = Path.Combine(directoryName, "..\\TransportRoles\\data\\SenderReputation\\");
			}
			Database.logFilePath = Database.databasePath;
		}

		private static void HandleSrlSettingsRecord(ConfigurationDataRowData data)
		{
			if (Database.propertyBag == null)
			{
				Database.propertyBag = new PropertyBag();
			}
			try
			{
				string configName;
				switch (configName = data.ConfigName)
				{
				case "ZombieKeywords":
				{
					string configValue = data.ConfigValue;
					string[] value = configValue.Split(new char[]
					{
						';'
					});
					Database.propertyBag[data.ConfigName] = value;
					goto IL_148;
				}
				case "ConfigurationVersion":
					Database.propertyBag[data.ConfigName] = data.ConfigValue;
					goto IL_148;
				case "MinWinLen":
				case "MaxWinLen":
				case "GoodBehaviorPeriod":
				case "InitWinLen":
					Database.propertyBag[data.ConfigName] = Convert.ToInt32(data.ConfigValue, CultureInfo.InvariantCulture);
					goto IL_148;
				}
				Database.propertyBag[data.ConfigName] = Convert.ToDouble(data.ConfigValue, CultureInfo.InvariantCulture);
				IL_148:;
			}
			catch (FormatException ex)
			{
				ExTraceGlobals.DatabaseTracer.TraceError<string>(0L, "HandleSrlSettingsRecord: configuration value does not belong to SRL settings: {0}", ex.Message);
			}
			catch (OverflowException ex2)
			{
				ExTraceGlobals.DatabaseTracer.TraceError<string>(0L, "HandleSrlSettingsRecord: configuration value does not belong to SRL settings: {0}", ex2.Message);
			}
		}

		protected const string DefaultDatabaseFileName = "pasettings.edb";

		protected static string databasePath;

		protected static string databaseName = "pasettings.edb";

		protected static string logFilePath;

		protected static object syncObject = new object();

		protected static DataSource dataSource = null;

		private static int refCount = 0;

		private static string perfCounterInstanceName = "paagents";

		private static ProtocolAnalysisTable protocolAnalysisTable = new ProtocolAnalysisTable();

		private static SenderReputationTable senderReputationTable = new SenderReputationTable();

		private static OpenProxyStatusTable openProxyStatusTable = new OpenProxyStatusTable();

		private static ConfigurationDataTable configurationDataTable = new ConfigurationDataTable();

		private static PropertyBag propertyBag = null;

		private static int maxConnections = 100;
	}
}
