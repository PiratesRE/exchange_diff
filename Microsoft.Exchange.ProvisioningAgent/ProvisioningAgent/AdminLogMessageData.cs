using System;
using System.Text;
using Microsoft.Exchange.Data.Storage.Auditing;

namespace Microsoft.Exchange.ProvisioningAgent
{
	internal class AdminLogMessageData
	{
		public string Subject { get; set; }

		public string Body { get; set; }

		public string Caller { get; set; }

		public string ObjectModified { get; set; }

		public AdminLogMessageData(IAuditLogRecord auditRecord)
		{
			this.Body = AuditLogParseSerialize.GetAsString(auditRecord);
			this.Subject = string.Format("{0} : {1}", auditRecord.UserId, auditRecord.Operation);
			this.Caller = string.Format("{0}{1}", auditRecord.UserId, AdminLogMessageData.LogID);
			this.ObjectModified = string.Format("{0}{1}", auditRecord.ObjectId, AdminLogMessageData.LogID);
		}

		public int GetSize()
		{
			return Encoding.Unicode.GetByteCount(this.Subject) + Encoding.Unicode.GetByteCount(this.Body) + Encoding.Unicode.GetByteCount(this.Caller) + Encoding.Unicode.GetByteCount(this.ObjectModified);
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendFormat("Subject: {0}", this.Subject);
			stringBuilder.AppendLine();
			stringBuilder.AppendLine("Body: ");
			stringBuilder.Append(this.Body);
			return stringBuilder.ToString();
		}

		private static readonly string LogID = "audit";
	}
}
