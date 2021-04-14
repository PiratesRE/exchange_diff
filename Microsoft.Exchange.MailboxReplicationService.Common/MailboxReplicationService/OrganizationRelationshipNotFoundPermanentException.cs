using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.MailboxReplicationService
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class OrganizationRelationshipNotFoundPermanentException : MailboxReplicationPermanentException
	{
		public OrganizationRelationshipNotFoundPermanentException(string domain, string orgId) : base(MrsStrings.OrganizationRelationshipNotFound(domain, orgId))
		{
			this.domain = domain;
			this.orgId = orgId;
		}

		public OrganizationRelationshipNotFoundPermanentException(string domain, string orgId, Exception innerException) : base(MrsStrings.OrganizationRelationshipNotFound(domain, orgId), innerException)
		{
			this.domain = domain;
			this.orgId = orgId;
		}

		protected OrganizationRelationshipNotFoundPermanentException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.domain = (string)info.GetValue("domain", typeof(string));
			this.orgId = (string)info.GetValue("orgId", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("domain", this.domain);
			info.AddValue("orgId", this.orgId);
		}

		public string Domain
		{
			get
			{
				return this.domain;
			}
		}

		public string OrgId
		{
			get
			{
				return this.orgId;
			}
		}

		private readonly string domain;

		private readonly string orgId;
	}
}
