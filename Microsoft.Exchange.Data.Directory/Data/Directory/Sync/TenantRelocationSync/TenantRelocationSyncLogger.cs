using System;
using System.IO;
using System.Reflection;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Win32;

namespace Microsoft.Exchange.Data.Directory.Sync.TenantRelocationSync
{
	internal class TenantRelocationSyncLogger : DisposeTrackableBase
	{
		private TenantRelocationSyncLogger()
		{
			this.logSchema = new LogSchema("Microsoft Exchange", Assembly.GetExecutingAssembly().GetName().Version.ToString(), "Tenant Relocation Sync Log", Enum.GetNames(typeof(TenantRelocationSyncLogger.TenantRelocationSyncLogFields)));
			this.log = new Log("TenantRelocationSyncLog", new LogHeaderFormatter(this.logSchema, true), "TenantRelocationSyncLogger");
			this.Configure();
		}

		public static TenantRelocationSyncLogger Instance
		{
			get
			{
				return TenantRelocationSyncLogger.instance;
			}
		}

		public void Configure()
		{
			if (!base.IsDisposed)
			{
				lock (this.logLock)
				{
					this.log.Configure(Path.Combine(TenantRelocationSyncLogger.GetExchangeInstallPath(), "Logging\\TenantRelocationLogs\\SyncLog\\"), TimeSpan.FromDays(30.0), 104857600L, 104857600L, 10485760, TimeSpan.FromSeconds(10.0), true);
				}
			}
		}

		public void Close()
		{
			if (!base.IsDisposed && this.log != null)
			{
				this.log.Close();
				this.log = null;
			}
		}

		public void Log(TenantRelocationSyncData syncData, string messageType, string errCode = null, string errorMessage = null, byte[] data = null)
		{
			this.Log(syncData.Source.TenantOrganizationUnit.Rdn.UnescapedName, syncData.Source.PartitionRoot.Rdn.UnescapedName, syncData.Target.PartitionRoot.Rdn.UnescapedName, messageType, errCode, errorMessage, data);
		}

		public void Log(string tenantName, string sourceForest, string targetForest, string messageType, string errorCode, string errorMessage, byte[] data)
		{
			this.LogRow(tenantName, Environment.CurrentManagedThreadId, sourceForest, targetForest, messageType, errorCode, errorMessage, data);
		}

		protected override void InternalDispose(bool disposing)
		{
			if (disposing)
			{
				this.Close();
			}
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<TenantRelocationSyncLogger>(this);
		}

		private static string TruncateAndConvertByteArray(byte[] bytes, int maxLength)
		{
			if (bytes.Length > maxLength)
			{
				return Convert.ToBase64String(bytes, 0, maxLength) + "[Truncated]";
			}
			return Convert.ToBase64String(bytes);
		}

		private void LogRow(string tenantName, int threadId, string sourceForest, string targetForest, string messageType, string errorCode, string errorMessage, byte[] data)
		{
			LogRowFormatter logRowFormatter = this.NewLogRow();
			logRowFormatter[1] = tenantName;
			logRowFormatter[2] = threadId;
			logRowFormatter[3] = sourceForest;
			logRowFormatter[4] = targetForest;
			logRowFormatter[5] = messageType;
			logRowFormatter[6] = errorCode;
			logRowFormatter[7] = this.TruncateString(errorMessage, 10000);
			logRowFormatter[8] = ((data == null) ? null : Convert.ToBase64String(data));
			this.AppendLogRow(logRowFormatter);
		}

		private static string GetExchangeInstallPath()
		{
			string result;
			using (RegistryKey registryKey = Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\ExchangeServer\\v15\\Setup"))
			{
				if (registryKey == null)
				{
					result = string.Empty;
				}
				else
				{
					object value = registryKey.GetValue("MsiInstallPath");
					registryKey.Close();
					if (value == null)
					{
						result = string.Empty;
					}
					else
					{
						result = value.ToString();
					}
				}
			}
			return result;
		}

		private LogRowFormatter NewLogRow()
		{
			return new LogRowFormatter(this.logSchema);
		}

		private void AppendLogRow(LogRowFormatter row)
		{
			if (!base.IsDisposed)
			{
				this.log.Append(row, 0);
			}
		}

		private string TruncateString(string str, int maxLength)
		{
			if (str != null && str.Length > maxLength)
			{
				return str.Substring(0, maxLength) + "[Truncated]";
			}
			return str;
		}

		private const string LogTypeName = "Tenant Relocation Sync Log";

		private const string FileNamePrefixName = "TenantRelocationSyncLog";

		private const int MaxStringLength = 10000;

		private const int MaxByteArrayLength = 20;

		private const string LogComponentName = "TenantRelocationSyncLogger";

		private const string SoftwareName = "Microsoft Exchange";

		private const string TruncatedSuffix = "[Truncated]";

		private static readonly TenantRelocationSyncLogger instance = new TenantRelocationSyncLogger();

		private Log log;

		private LogSchema logSchema;

		private object logLock = new object();

		internal enum TenantRelocationSyncLogFields
		{
			Timestamp,
			TenantName,
			ThreadId,
			SourceForest,
			TargetForest,
			MessageType,
			ErrorCode,
			ErrorMessage,
			Data
		}
	}
}
