using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class AuditLogSearchArbitrationMailboxNotFoundException : LocalizedException
	{
		public AuditLogSearchArbitrationMailboxNotFoundException(string organization) : base(Strings.AuditLogSearchArbitrationMailboxNotFound(organization))
		{
			this.organization = organization;
		}

		public AuditLogSearchArbitrationMailboxNotFoundException(string organization, Exception innerException) : base(Strings.AuditLogSearchArbitrationMailboxNotFound(organization), innerException)
		{
			this.organization = organization;
		}

		protected AuditLogSearchArbitrationMailboxNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.organization = (string)info.GetValue("organization", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("organization", this.organization);
		}

		public string Organization
		{
			get
			{
				return this.organization;
			}
		}

		private readonly string organization;
	}
}
