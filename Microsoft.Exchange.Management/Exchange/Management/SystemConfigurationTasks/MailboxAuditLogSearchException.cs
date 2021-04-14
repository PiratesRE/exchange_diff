using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class MailboxAuditLogSearchException : LocalizedException
	{
		public MailboxAuditLogSearchException(string error, Exception ex) : base(Strings.ErrorMailboxAuditLogSearchFailed(error, ex))
		{
			this.error = error;
			this.ex = ex;
		}

		public MailboxAuditLogSearchException(string error, Exception ex, Exception innerException) : base(Strings.ErrorMailboxAuditLogSearchFailed(error, ex), innerException)
		{
			this.error = error;
			this.ex = ex;
		}

		protected MailboxAuditLogSearchException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.error = (string)info.GetValue("error", typeof(string));
			this.ex = (Exception)info.GetValue("ex", typeof(Exception));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("error", this.error);
			info.AddValue("ex", this.ex);
		}

		public string Error
		{
			get
			{
				return this.error;
			}
		}

		public Exception Ex
		{
			get
			{
				return this.ex;
			}
		}

		private readonly string error;

		private readonly Exception ex;
	}
}
