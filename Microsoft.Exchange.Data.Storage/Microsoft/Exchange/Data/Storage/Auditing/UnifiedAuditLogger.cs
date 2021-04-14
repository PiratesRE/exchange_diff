using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Office.Compliance.Audit.Schema;

namespace Microsoft.Exchange.Data.Storage.Auditing
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class UnifiedAuditLogger : IDisposable
	{
		static UnifiedAuditLogger()
		{
			using (Process currentProcess = Process.GetCurrentProcess())
			{
				UnifiedAuditLogger.CurrentProcessId = currentProcess.Id;
			}
		}

		public UnifiedAuditLogger()
		{
			string[] fields = (from f in LocalQueueCsvFields.Fields
			select f.Name).ToArray<string>();
			this.logSchema = new LogSchema("Microsoft Exchange", "15.00.1497.012", "audit", fields);
			LogHeaderFormatter headerFormatter = new LogHeaderFormatter(this.logSchema, LogHeaderCsvOption.NotCsvCompatible);
			this.localQueue = new Log("audit", headerFormatter, "Exchange");
			UnifiedAuditLoggerSettings unifiedAuditLoggerSettings = UnifiedAuditLoggerSettings.Load();
			this.localQueue.Configure(Path.Combine(unifiedAuditLoggerSettings.DirectoryPath, "Exchange"), unifiedAuditLoggerSettings.MaxAge, (long)unifiedAuditLoggerSettings.MaxDirectorySize.ToBytes(), (long)unifiedAuditLoggerSettings.MaxFileSize.ToBytes(), (int)unifiedAuditLoggerSettings.CacheSize.ToBytes(), unifiedAuditLoggerSettings.FlushInterval, unifiedAuditLoggerSettings.FlushToDisk);
		}

		public int WriteAuditRecord(AuditRecord record)
		{
			LogRowFormatter row;
			int result = this.ConvertAuditRecord(record, out row);
			this.localQueue.Append(row, 0);
			return result;
		}

		public void Dispose()
		{
			this.localQueue.Flush();
			this.localQueue.Close();
		}

		public int ConvertAuditRecord(AuditRecord record, out LogRowFormatter logRowFormatter)
		{
			logRowFormatter = new LogRowFormatter(this.logSchema, true);
			int byteCount;
			using (RecordSerializer recordSerializer = new RecordSerializer())
			{
				string text = recordSerializer.Write(record, record.RecordType);
				byteCount = Encoding.UTF8.GetByteCount(text);
				logRowFormatter[0] = DateTime.UtcNow;
				logRowFormatter[1] = UnifiedAuditLogger.CurrentProcessId;
				logRowFormatter[2] = UnifiedAuditLogger.CurrentApplicationName;
				logRowFormatter[3] = record.RecordType;
				logRowFormatter[4] = text;
				logRowFormatter[5] = string.Empty;
			}
			return byteCount;
		}

		private const string FileNamePrefix = "audit";

		private const string ComponentName = "Exchange";

		private const string Software = "Microsoft Exchange";

		private const string Version = "15.00.1497.012";

		private const string LogType = "audit";

		private static readonly int CurrentProcessId;

		private static readonly string CurrentApplicationName = ApplicationName.Current.Name.ToLower();

		private readonly LogSchema logSchema;

		private readonly Log localQueue;
	}
}
