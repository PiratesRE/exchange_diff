using System;
using System.Text;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Storage.Auditing;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Management.SystemConfigurationTasks;

namespace Microsoft.Exchange.ProvisioningAgent
{
	internal class FfoAuditLogger : IAuditLog
	{
		public FfoAuditLogger(OrganizationId organization)
		{
			this.session = AdminAuditLogHelper.CreateSession(organization, null);
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
			ConfigObjectId configObjectId = new ConfigObjectId(CombGuidGenerator.NewGuid().ToString());
			string asString = AuditLogParseSerialize.GetAsString(auditRecord);
			AdminAuditLogEvent adminAuditLogEvent = new AdminAuditLogEvent(new AdminAuditLogEventId(configObjectId), asString);
			this.session.Save(new AdminAuditLogEventFacade(configObjectId)
			{
				ObjectModified = adminAuditLogEvent.ObjectModified,
				ModifiedObjectResolvedName = adminAuditLogEvent.ModifiedObjectResolvedName,
				CmdletName = adminAuditLogEvent.CmdletName,
				CmdletParameters = adminAuditLogEvent.CmdletParameters,
				ModifiedProperties = adminAuditLogEvent.ModifiedProperties,
				Caller = adminAuditLogEvent.Caller,
				Succeeded = adminAuditLogEvent.Succeeded,
				Error = adminAuditLogEvent.Error,
				RunDate = adminAuditLogEvent.RunDate,
				OriginatingServer = adminAuditLogEvent.OriginatingServer
			});
			return Encoding.Unicode.GetByteCount(asString);
		}

		private IConfigurationSession session;
	}
}
