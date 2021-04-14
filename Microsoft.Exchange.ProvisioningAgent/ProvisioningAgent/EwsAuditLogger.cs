using System;
using Microsoft.Exchange.Data.ApplicationLogic;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.Auditing;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.ProvisioningAgent;
using Microsoft.Exchange.SoapWebClient.EWS;

namespace Microsoft.Exchange.ProvisioningAgent
{
	internal class EwsAuditLogger : IAuditLog
	{
		public EwsAuditLogger(ExchangePrincipal principal)
		{
			this.ewsClient = new EwsAuditClient(new EwsConnectionManager(principal, OpenAsAdminOrSystemServiceBudgetTypeType.Unthrottled, EwsAuditLogger.Tracer), EwsAuditClient.DefaultSoapClientTimeout, EwsAuditLogger.Tracer);
			this.exchangePrincipal = principal;
			this.InitializeAdminAuditLogsFolder();
		}

		public DateTime EstimatedLogStartTime
		{
			get
			{
				return DateTime.MinValue;
			}
		}

		public DateTime EstimatedLogEndTime
		{
			get
			{
				return DateTime.MaxValue;
			}
		}

		public bool IsAsynchronous
		{
			get
			{
				return false;
			}
		}

		public IAuditQueryContext<TFilter> CreateAuditQueryContext<TFilter>()
		{
			throw new InvalidOperationException();
		}

		public int WriteAuditRecord(IAuditLogRecord auditRecord)
		{
			if (AuditFeatureManager.IsPartitionedAdminLogEnabled(this.exchangePrincipal) && (this.auditLog == null || this.auditLog.EstimatedLogEndTime < auditRecord.CreationTime))
			{
				EwsAuditLogCollection ewsAuditLogCollection = new EwsAuditLogCollection(this.ewsClient, this.auditRootFolderId);
				if (!ewsAuditLogCollection.FindLog(auditRecord.CreationTime, true, out this.auditLog))
				{
					this.auditLog = null;
				}
			}
			if (this.auditLog == null)
			{
				this.auditLog = new EwsAuditLog(this.ewsClient, this.auditRootFolderId, DateTime.MinValue, DateTime.MaxValue);
			}
			return this.auditLog.WriteAuditRecord(auditRecord);
		}

		private void InitializeAdminAuditLogsFolder()
		{
			FolderIdType folderIdType = null;
			this.ewsClient.CheckAndCreateWellKnownFolder(DistinguishedFolderIdNameType.root, DistinguishedFolderIdNameType.recoverableitemsroot, out folderIdType);
			this.ewsClient.CheckAndCreateWellKnownFolder(DistinguishedFolderIdNameType.recoverableitemsroot, DistinguishedFolderIdNameType.adminauditlogs, out this.auditRootFolderId);
		}

		private static readonly Trace Tracer = ExTraceGlobals.AdminAuditLogTracer;

		private EwsAuditClient ewsClient;

		private FolderIdType auditRootFolderId;

		private IAuditLog auditLog;

		private ExchangePrincipal exchangePrincipal;
	}
}
