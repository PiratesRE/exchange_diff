using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Storage.Auditing;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.SoapWebClient.EWS;

namespace Microsoft.Exchange.Data.ApplicationLogic
{
	internal class EwsAuditLogCollection : IAuditLogCollection
	{
		public EwsAuditLogCollection(EwsAuditClient ewsClient, FolderIdType auditRootFolderId)
		{
			this.ewsClient = ewsClient;
			this.auditRootFolderId = auditRootFolderId;
		}

		public Trace Tracer
		{
			get
			{
				return this.ewsClient.Tracer;
			}
		}

		public IEnumerable<IAuditLog> GetAuditLogs()
		{
			BaseFolderType[] subfolders = this.ewsClient.GetSubFolders(this.auditRootFolderId, null);
			foreach (BaseFolderType subfolder in subfolders)
			{
				DateTime logRangeStart;
				DateTime logRangeEnd;
				bool validLogSubfolder = AuditLogCollection.TryParseLogRange(subfolder.DisplayName, out logRangeStart, out logRangeEnd);
				if (validLogSubfolder && subfolder.TotalCount > 0)
				{
					yield return new EwsAuditLog(this.ewsClient, subfolder.FolderId, logRangeStart, logRangeEnd);
				}
			}
			yield return new EwsAuditLog(this.ewsClient, this.auditRootFolderId, DateTime.MinValue, DateTime.MaxValue);
			yield break;
		}

		public bool FindLog(DateTime timestamp, bool createIfNotExists, out IAuditLog auditLog)
		{
			auditLog = null;
			DateTime dateTime;
			DateTime dateTime2;
			string logFolderNameAndRange = AuditLogCollection.GetLogFolderNameAndRange(timestamp, out dateTime, out dateTime2);
			FolderIdType folderIdType = null;
			if (!this.ewsClient.FindFolder(logFolderNameAndRange, this.auditRootFolderId, out folderIdType))
			{
				if (!createIfNotExists)
				{
					if (this.IsTraceEnabled(TraceType.DebugTrace))
					{
						this.Tracer.TraceDebug<DateTime>((long)this.GetHashCode(), "No matching log subfolder found. Lookup time={0}", timestamp);
					}
					return false;
				}
				bool flag = this.ewsClient.CreateFolder(logFolderNameAndRange, this.auditRootFolderId, out folderIdType);
				if (!flag)
				{
					flag = this.ewsClient.FindFolder(logFolderNameAndRange, this.auditRootFolderId, out folderIdType);
				}
				if (!flag && this.IsTraceEnabled(TraceType.DebugTrace))
				{
					this.Tracer.TraceDebug<DateTime, DateTime>((long)this.GetHashCode(), "Failed to create audit log folder for log range [{0}, {1})", dateTime, dateTime2);
				}
			}
			if (folderIdType != null)
			{
				auditLog = new EwsAuditLog(this.ewsClient, folderIdType, dateTime, dateTime2);
			}
			return folderIdType != null;
		}

		private bool IsTraceEnabled(TraceType traceType)
		{
			return this.Tracer != null && this.Tracer.IsTraceEnabled(TraceType.DebugTrace);
		}

		private EwsAuditClient ewsClient;

		private FolderIdType auditRootFolderId;
	}
}
