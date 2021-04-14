using System;
using System.Collections.Generic;
using System.Globalization;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage.Auditing
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class AuditLogCollection : IAuditLogCollection
	{
		public AuditLogCollection(StoreSession storeSession, StoreId auditLogRootId, Trace tracer) : this(storeSession, auditLogRootId, tracer, null)
		{
		}

		public AuditLogCollection(StoreSession storeSession, StoreId auditLogRootId, Trace tracer, Func<IAuditLogRecord, MessageItem, int> recordFormatter)
		{
			this.storeSession = storeSession;
			this.auditLogRootId = auditLogRootId;
			this.tracer = tracer;
			this.recordFormatter = recordFormatter;
		}

		public IEnumerable<IAuditLog> GetAuditLogs()
		{
			return this.InternalGetAuditLogs(false, null);
		}

		public IEnumerable<AuditLog> GetExpiringAuditLogs(DateTime expirationTime)
		{
			return this.InternalGetAuditLogs(true, (AuditLogCollection.LogInfo logInfo) => logInfo.RangeEnd < expirationTime || logInfo.RangeStart <= expirationTime);
		}

		public bool FindLog(DateTime timestamp, bool createIfNotExists, out IAuditLog auditLog)
		{
			auditLog = null;
			using (Folder folder = Folder.Bind(this.storeSession, this.auditLogRootId))
			{
				AuditLogCollection.LogInfo? logInfo = null;
				foreach (AuditLogCollection.LogInfo value in this.FindLogFolders(folder, false))
				{
					if (value.RangeStart <= timestamp && timestamp < value.RangeEnd)
					{
						logInfo = new AuditLogCollection.LogInfo?(value);
						break;
					}
				}
				if (logInfo == null)
				{
					if (!createIfNotExists)
					{
						if (this.IsTraceEnabled)
						{
							this.tracer.TraceDebug<DateTime>((long)this.GetHashCode(), "No matching log subfolder found. Lookup time={0}", timestamp);
						}
						return false;
					}
					AuditLogCollection.LogInfo value2 = default(AuditLogCollection.LogInfo);
					string logFolderNameAndRange = AuditLogCollection.GetLogFolderNameAndRange(timestamp, out value2.RangeStart, out value2.RangeEnd);
					using (Folder folder2 = Folder.Create(this.storeSession, this.auditLogRootId, StoreObjectType.Folder, logFolderNameAndRange, CreateMode.OpenIfExists))
					{
						if (folder2.Id == null)
						{
							folder2.Save();
							folder2.Load();
						}
						value2.FolderId = folder2.Id;
						value2.ItemCount = folder2.ItemCount;
						logInfo = new AuditLogCollection.LogInfo?(value2);
						if (this.IsTraceEnabled)
						{
							this.tracer.TraceDebug<DateTime, DateTime, string>((long)this.GetHashCode(), "New log subfolder created. StartTime=[{0}],EndTime=[{1}],FolderId=[{2}]", value2.RangeStart, value2.RangeEnd, value2.FolderId.ToBase64String());
						}
						goto IL_1AA;
					}
				}
				if (this.IsTraceEnabled)
				{
					this.tracer.TraceDebug<DateTime, DateTime, string>((long)this.GetHashCode(), "Found matching log subfolder StartTime=[{0}],EndTime=[{1}],FolderId=[{2}]", logInfo.Value.RangeStart, logInfo.Value.RangeEnd, logInfo.Value.FolderId.ToBase64String());
				}
				IL_1AA:
				auditLog = new AuditLog(this.storeSession, logInfo.Value.FolderId, logInfo.Value.RangeStart, logInfo.Value.RangeEnd, logInfo.Value.ItemCount, this.recordFormatter);
			}
			return true;
		}

		public static string GetLogFolderNameAndRange(DateTime timestamp, out DateTime rangeStart, out DateTime rangeEnd)
		{
			string text = timestamp.ToString(AuditLogCollection.DailyLogFolderNameFormat);
			if (!AuditLogCollection.TryParseLogRange(text, out rangeStart, out rangeEnd))
			{
				throw new InvalidOperationException("Invalid log folder name");
			}
			return text;
		}

		public static bool TryParseLogRange(string logName, out DateTime rangeStart, out DateTime rangeEnd)
		{
			rangeStart = default(DateTime);
			rangeEnd = default(DateTime);
			if (string.IsNullOrEmpty(logName))
			{
				return false;
			}
			bool flag = DateTime.TryParseExact(logName, AuditLogCollection.DailyLogFolderNameFormat, CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal | DateTimeStyles.AssumeUniversal, out rangeStart);
			if (flag)
			{
				rangeEnd = rangeStart.AddDays(1.0);
			}
			return flag;
		}

		private bool IsTraceEnabled
		{
			get
			{
				return this.tracer != null && this.tracer.IsTraceEnabled(TraceType.DebugTrace);
			}
		}

		private IEnumerable<AuditLog> InternalGetAuditLogs(bool includeEmptyFolders, Func<AuditLogCollection.LogInfo, bool> predicate)
		{
			using (Folder rootFolder = Folder.Bind(this.storeSession, this.auditLogRootId))
			{
				foreach (AuditLogCollection.LogInfo logInfo in this.FindLogFolders(rootFolder, includeEmptyFolders))
				{
					if (predicate == null || predicate(logInfo))
					{
						yield return new AuditLog(this.storeSession, logInfo.FolderId, logInfo.RangeStart, logInfo.RangeEnd, logInfo.ItemCount, this.recordFormatter);
					}
				}
				if (rootFolder.ItemCount > 0)
				{
					yield return new AuditLog(this.storeSession, this.auditLogRootId, DateTime.MinValue, DateTime.MaxValue, rootFolder.ItemCount, this.recordFormatter);
				}
			}
			yield break;
		}

		private IEnumerable<AuditLogCollection.LogInfo> FindLogFolders(Folder rootFolder, bool includeEmptyFolders)
		{
			if (rootFolder.HasSubfolders)
			{
				using (QueryResult queryResult = rootFolder.FolderQuery(FolderQueryFlags.None, null, AuditLogCollection.SortByDisplayName, AuditLogCollection.FolderProperties))
				{
					object[][] rows;
					do
					{
						rows = queryResult.GetRows(100);
						if (rows != null && rows.Length > 0)
						{
							foreach (object[] row in rows)
							{
								DateTime logRangeStart;
								DateTime logRangeEnd;
								bool validLogSubfolder = AuditLogCollection.TryParseLogRange(row[0] as string, out logRangeStart, out logRangeEnd);
								int itemCount = AuditLogCollection.GetItemCount(row[2]);
								if (validLogSubfolder && (includeEmptyFolders || itemCount > 0))
								{
									yield return new AuditLogCollection.LogInfo
									{
										FolderId = (StoreId)row[1],
										RangeStart = logRangeStart,
										RangeEnd = logRangeEnd,
										ItemCount = itemCount
									};
								}
							}
						}
					}
					while (rows != null && rows.Length > 0);
				}
			}
			yield break;
		}

		private static int GetItemCount(object itemCountValue)
		{
			if (itemCountValue == null || !(itemCountValue is int))
			{
				return 0;
			}
			return (int)itemCountValue;
		}

		private const int FolderQueryBatchSize = 100;

		private static string DailyLogFolderNameFormat = "yyyy-MM-dd";

		private static readonly SortBy[] SortByDisplayName = new SortBy[]
		{
			new SortBy(FolderSchema.DisplayName, SortOrder.Ascending)
		};

		private static readonly PropertyDefinition[] FolderProperties = new PropertyDefinition[]
		{
			FolderSchema.DisplayName,
			FolderSchema.Id,
			FolderSchema.ItemCount
		};

		private StoreSession storeSession;

		private StoreId auditLogRootId;

		private Trace tracer;

		private Func<IAuditLogRecord, MessageItem, int> recordFormatter;

		private enum FolderPropertiesIndex
		{
			IdxDisplayName,
			IdxId,
			IdxItemCount,
			IdxLast
		}

		private struct LogInfo
		{
			public StoreId FolderId;

			public DateTime RangeStart;

			public DateTime RangeEnd;

			public int ItemCount;
		}
	}
}
