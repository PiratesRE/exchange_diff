using System;
using System.Diagnostics;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.Auditing;
using Microsoft.Exchange.Data.Storage.Infoworker.MailboxSearch;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.SoapWebClient.EWS;
using Microsoft.Office.Compliance.Audit.Schema;
using Microsoft.Office.Compliance.Audit.Schema.Admin;

namespace Microsoft.Exchange.Data.ApplicationLogic
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class AdminAuditWriter
	{
		public AdminAuditWriter(Microsoft.Exchange.Diagnostics.Trace tracer)
		{
			this.Tracer = tracer;
		}

		public void Write(ExchangeAdminAuditRecord record)
		{
			Stopwatch stopwatch = Stopwatch.StartNew();
			using (AdminAuditOpticsLogData adminAuditOpticsLogData = new AdminAuditOpticsLogData())
			{
				try
				{
					adminAuditOpticsLogData.Tenant = (string.IsNullOrEmpty(record.OrganizationName) ? "First Org" : record.OrganizationName);
					adminAuditOpticsLogData.CmdletName = record.Operation;
					adminAuditOpticsLogData.ExternalAccess = record.ExternalAccess;
					adminAuditOpticsLogData.OperationSucceeded = (record.Succeeded ?? false);
					adminAuditOpticsLogData.RecordId = record.Id;
					adminAuditOpticsLogData.Asynchronous = true;
					IAuditLog auditLog = this.GetAuditLog(record.OrganizationId ?? string.Empty, record);
					IAuditLogRecord auditRecord = this.ConvertAuditRecord(record);
					adminAuditOpticsLogData.RecordSize = auditLog.WriteAuditRecord(auditRecord);
					adminAuditOpticsLogData.LoggingError = null;
					adminAuditOpticsLogData.AuditSucceeded = true;
				}
				catch (Exception loggingError)
				{
					adminAuditOpticsLogData.LoggingError = loggingError;
					adminAuditOpticsLogData.AuditSucceeded = false;
					throw;
				}
				finally
				{
					adminAuditOpticsLogData.LoggingTime = stopwatch.ElapsedMilliseconds;
				}
			}
		}

		private IAuditLogRecord ConvertAuditRecord(AuditRecord record)
		{
			return new AuditLogRecord(record, this.Tracer);
		}

		private IAuditLog GetAuditLog(string organizationIdEncoded, AuditRecord auditRecord)
		{
			CacheEntry<IAuditLog> cacheEntry;
			IAuditLog auditLog = this.auditLogs.TryGetValue(organizationIdEncoded, DateTime.UtcNow, out cacheEntry) ? cacheEntry.Value : null;
			ExchangePrincipal exchangePrincipal = this.GetExchangePrincipal(organizationIdEncoded);
			EwsAuditClient ewsClient = null;
			FolderIdType folderIdType = null;
			if (AuditFeatureManager.IsPartitionedAdminLogEnabled(exchangePrincipal) && (auditLog == null || auditLog.EstimatedLogEndTime < auditRecord.CreationTime))
			{
				this.GetClientAndRootFolderId(exchangePrincipal, ref ewsClient, ref folderIdType);
				EwsAuditLogCollection ewsAuditLogCollection = new EwsAuditLogCollection(ewsClient, folderIdType);
				if (ewsAuditLogCollection.FindLog(auditRecord.CreationTime, true, out auditLog))
				{
					this.auditLogs.Set(organizationIdEncoded, DateTime.UtcNow, new CacheEntry<IAuditLog>(auditLog));
				}
				else
				{
					auditLog = null;
				}
			}
			if (auditLog == null)
			{
				this.GetClientAndRootFolderId(exchangePrincipal, ref ewsClient, ref folderIdType);
				auditLog = new EwsAuditLog(ewsClient, folderIdType, DateTime.MinValue, DateTime.MaxValue);
				this.auditLogs.Set(organizationIdEncoded, DateTime.UtcNow, new CacheEntry<IAuditLog>(auditLog));
			}
			return auditLog;
		}

		private void GetClientAndRootFolderId(ExchangePrincipal principal, ref EwsAuditClient ewsClient, ref FolderIdType auditRootFolderId)
		{
			ewsClient = (ewsClient ?? new EwsAuditClient(new EwsConnectionManager(principal, OpenAsAdminOrSystemServiceBudgetTypeType.Unthrottled, this.Tracer), EwsAuditClient.DefaultSoapClientTimeout, this.Tracer));
			auditRootFolderId = (auditRootFolderId ?? AdminAuditWriter.GetAuditRootFolderId(ewsClient));
		}

		private static FolderIdType GetAuditRootFolderId(EwsAuditClient ewsClient)
		{
			FolderIdType folderIdType;
			ewsClient.CheckAndCreateWellKnownFolder(DistinguishedFolderIdNameType.root, DistinguishedFolderIdNameType.recoverableitemsroot, out folderIdType);
			FolderIdType result;
			ewsClient.CheckAndCreateWellKnownFolder(DistinguishedFolderIdNameType.recoverableitemsroot, DistinguishedFolderIdNameType.adminauditlogs, out result);
			return result;
		}

		private ExchangePrincipal GetExchangePrincipal(string organizationIdEncoded)
		{
			CacheEntry<ExchangePrincipal> cacheEntry;
			ExchangePrincipal exchangePrincipal;
			if (this.principals.TryGetValue(organizationIdEncoded, DateTime.UtcNow, out cacheEntry))
			{
				exchangePrincipal = cacheEntry.Value;
			}
			else
			{
				OrganizationId organizationId = AuditRecordDatabaseWriterVisitor.GetOrganizationId(organizationIdEncoded);
				ADUser tenantArbitrationMailbox = AdminAuditWriter.GetTenantArbitrationMailbox(organizationId);
				exchangePrincipal = ExchangePrincipal.FromADUser(tenantArbitrationMailbox, null);
				this.principals.TryAdd(organizationIdEncoded, DateTime.UtcNow, new CacheEntry<ExchangePrincipal>(exchangePrincipal));
			}
			return exchangePrincipal;
		}

		private static ADUser GetTenantArbitrationMailbox(OrganizationId organizationId)
		{
			ADSessionSettings sessionSettings = ADSessionSettings.FromOrganizationIdWithoutRbacScopes(ADSystemConfigurationSession.GetRootOrgContainerIdForLocalForest(), organizationId, null, false);
			IRecipientSession tenantOrRootOrgRecipientSession = DirectorySessionFactory.Default.GetTenantOrRootOrgRecipientSession(true, ConsistencyMode.IgnoreInvalid, sessionSettings, 244, "GetTenantArbitrationMailbox", "f:\\15.00.1497\\sources\\dev\\data\\src\\ApplicationLogic\\AuditLog\\AdminAuditWriter.cs");
			return MailboxDataProvider.GetDiscoveryMailbox(tenantOrRootOrgRecipientSession);
		}

		public const int PrincipalCacheSize = 128;

		public const int AuditLogCacheSize = 16;

		public static TimeSpan PrincipalLifeTime = TimeSpan.FromMinutes(15.0);

		public static TimeSpan AuditLogLifeTime = TimeSpan.FromMinutes(5.0);

		private readonly Microsoft.Exchange.Diagnostics.Trace Tracer;

		private readonly CacheWithExpiration<string, CacheEntry<ExchangePrincipal>> principals = new CacheWithExpiration<string, CacheEntry<ExchangePrincipal>>(128, AdminAuditWriter.PrincipalLifeTime, null);

		private readonly CacheWithExpiration<string, CacheEntry<IAuditLog>> auditLogs = new CacheWithExpiration<string, CacheEntry<IAuditLog>>(16, AdminAuditWriter.AuditLogLifeTime, null);
	}
}
