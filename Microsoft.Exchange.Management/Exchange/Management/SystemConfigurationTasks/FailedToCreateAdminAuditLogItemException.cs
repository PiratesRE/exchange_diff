using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class FailedToCreateAdminAuditLogItemException : AdminAuditLogException
	{
		public FailedToCreateAdminAuditLogItemException(string responseclass, string code, string error) : base(Strings.FailedToCreateAdminAuditLogItem(responseclass, code, error))
		{
			this.responseclass = responseclass;
			this.code = code;
			this.error = error;
		}

		public FailedToCreateAdminAuditLogItemException(string responseclass, string code, string error, Exception innerException) : base(Strings.FailedToCreateAdminAuditLogItem(responseclass, code, error), innerException)
		{
			this.responseclass = responseclass;
			this.code = code;
			this.error = error;
		}

		protected FailedToCreateAdminAuditLogItemException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.responseclass = (string)info.GetValue("responseclass", typeof(string));
			this.code = (string)info.GetValue("code", typeof(string));
			this.error = (string)info.GetValue("error", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("responseclass", this.responseclass);
			info.AddValue("code", this.code);
			info.AddValue("error", this.error);
		}

		public string Responseclass
		{
			get
			{
				return this.responseclass;
			}
		}

		public string Code
		{
			get
			{
				return this.code;
			}
		}

		public string Error
		{
			get
			{
				return this.error;
			}
		}

		private readonly string responseclass;

		private readonly string code;

		private readonly string error;
	}
}
