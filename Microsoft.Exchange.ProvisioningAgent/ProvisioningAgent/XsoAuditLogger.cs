using System;
using System.Text;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.Auditing;
using Microsoft.Exchange.Data.Storage.Principal;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.ProvisioningAgent;
using Microsoft.Exchange.Management.SystemConfigurationTasks;
using Microsoft.Exchange.Provisioning.LoadBalancing;

namespace Microsoft.Exchange.ProvisioningAgent
{
	internal class XsoAuditLogger : IAuditLog
	{
		public XsoAuditLogger(ExchangePrincipal principal, bool canUsePartitionedLogs)
		{
			this.principal = principal;
			IMailboxLocation location = principal.MailboxInfo.Location;
			if (location != null && location.ServerSite != null)
			{
				this.isLocalSite = PhysicalResourceLoadBalancing.IsDatabaseInLocalSite(location.ServerSite, location.ServerFqdn, location.DatabaseName, delegate(string message)
				{
				});
			}
			this.canUsePartitionedLogs = canUsePartitionedLogs;
		}

		public bool IsLocalSite
		{
			get
			{
				return this.isLocalSite;
			}
			set
			{
				this.isLocalSite = value;
			}
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
			if (this.isLocalSite)
			{
				return this.LogLocal(auditRecord);
			}
			return this.LogRemote(auditRecord);
		}

		private int LogLocal(IAuditLogRecord auditRecord)
		{
			if (XsoAuditLogger.Tracer.IsTraceEnabled(TraceType.DebugTrace))
			{
				XsoAuditLogger.Tracer.TraceDebug<string>(0L, "Writing log to local site mailbox {0}.", this.principal.MailboxInfo.PrimarySmtpAddress.ToString());
			}
			int result;
			using (MailboxSession mailboxSession = MailboxSessionManager.CreateMailboxSession(this.principal))
			{
				result = this.LogWithSession(auditRecord, mailboxSession);
			}
			return result;
		}

		private int LogRemote(IAuditLogRecord auditRecord)
		{
			if (XsoAuditLogger.Tracer.IsTraceEnabled(TraceType.DebugTrace))
			{
				XsoAuditLogger.Tracer.TraceDebug<string>(0L, "Writing log to remote site mailbox {0}.", this.principal.MailboxInfo.PrimarySmtpAddress.ToString());
			}
			MailboxSession userMailboxSessionFromCache = MailboxSessionManager.GetUserMailboxSessionFromCache(this.principal);
			try
			{
				return this.LogWithSession(auditRecord, userMailboxSessionFromCache);
			}
			catch (StorageTransientException)
			{
				if (userMailboxSessionFromCache != null)
				{
					MailboxSessionManager.ReturnMailboxSessionToCache(ref userMailboxSessionFromCache, true);
					userMailboxSessionFromCache = MailboxSessionManager.GetUserMailboxSessionFromCache(this.principal);
					if (userMailboxSessionFromCache != null)
					{
						return this.LogWithSession(auditRecord, userMailboxSessionFromCache);
					}
				}
			}
			finally
			{
				if (userMailboxSessionFromCache != null)
				{
					MailboxSessionManager.ReturnMailboxSessionToCache(ref userMailboxSessionFromCache, false);
				}
			}
			return 0;
		}

		private int LogWithSession(IAuditLogRecord auditRecord, MailboxSession session)
		{
			IAuditLog auditLog = null;
			if (this.canUsePartitionedLogs && AuditFeatureManager.IsPartitionedAdminLogEnabled(this.principal))
			{
				AuditLogCollection auditLogCollection = new AuditLogCollection(session, this.GetLogFolderId(session), XsoAuditLogger.Tracer, (IAuditLogRecord record, MessageItem message) => AuditLogParseSerialize.SerializeAdminAuditRecord(record, message));
				if (!auditLogCollection.FindLog(auditRecord.CreationTime, true, out auditLog))
				{
					auditLog = null;
				}
			}
			if (auditLog == null)
			{
				auditLog = new AuditLog(session, this.GetLogFolderId(session), DateTime.MinValue, DateTime.MaxValue, 0, (IAuditLogRecord record, MessageItem message) => AuditLogParseSerialize.SerializeAdminAuditRecord(record, message));
			}
			return auditLog.WriteAuditRecord(auditRecord);
		}

		protected virtual StoreObjectId GetLogFolderId(MailboxSession mailboxSession)
		{
			return AdminAuditLogHelper.GetOrCreateAdminAuditLogsFolderId(mailboxSession);
		}

		private void SaveMessage(MessageItem message)
		{
			ConflictResolutionResult conflictResolutionResult = message.Save(SaveMode.ResolveConflicts);
			if (conflictResolutionResult.SaveStatus == SaveResult.IrresolvableConflict)
			{
				if (XsoAuditLogger.Tracer.IsTraceEnabled(TraceType.DebugTrace))
				{
					XsoAuditLogger.Tracer.TraceDebug<XsoAuditLogger, string>((long)this.GetHashCode(), "{0} Unable to save log in the AdminAuditLogs folder due to irresolvable conflict. Details: {1}", this, this.ConvertConflictResolutionResultToString(conflictResolutionResult));
				}
				throw new SaveConflictException(ServerStrings.ExSaveFailedBecauseOfConflicts(message.InternalObjectId), conflictResolutionResult);
			}
			if (XsoAuditLogger.Tracer.IsTraceEnabled(TraceType.DebugTrace))
			{
				XsoAuditLogger.Tracer.TraceDebug<XsoAuditLogger>((long)this.GetHashCode(), "{0} Successfully saved log in the AdminAuditLogs folder under discovery mailbox.", this);
				return;
			}
		}

		private string ConvertConflictResolutionResultToString(ConflictResolutionResult result)
		{
			if (result == null)
			{
				return string.Empty;
			}
			StringBuilder stringBuilder = new StringBuilder("Save results: ");
			stringBuilder.AppendLine(result.SaveStatus.ToString());
			if (result.PropertyConflicts != null && result.PropertyConflicts.Length > 0)
			{
				for (int i = 0; i < result.PropertyConflicts.Length; i++)
				{
					PropertyConflict propertyConflict = result.PropertyConflicts[i];
					stringBuilder.AppendFormat("Resolvable: {0} Property: {1}\n", propertyConflict.ConflictResolvable, propertyConflict.PropertyDefinition);
					stringBuilder.AppendFormat("\tOriginal value: {0}\n", propertyConflict.OriginalValue);
					stringBuilder.AppendFormat("\tClient value: {0}\n", propertyConflict.ClientValue);
					stringBuilder.AppendFormat("\tServer value: {0}\n", propertyConflict.ServerValue);
					stringBuilder.AppendFormat("\tResolved value: {0}\n", propertyConflict.ResolvedValue);
				}
			}
			else
			{
				stringBuilder.Append("Zero properties in conflict");
			}
			return stringBuilder.ToString();
		}

		private static readonly Trace Tracer = ExTraceGlobals.AdminAuditLogTracer;

		private readonly ExchangePrincipal principal;

		private readonly bool canUsePartitionedLogs;

		private bool isLocalSite;
	}
}
