using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.MailboxLoadBalance
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	internal class InvalidOrganizationIdentityException : MailboxLoadBalancePermanentException
	{
		public InvalidOrganizationIdentityException(string orgName, string externalId) : base(MigrationWorkflowServiceStrings.ErrorInvalidExternalOrganizationId(orgName, externalId))
		{
			this.orgName = orgName;
			this.externalId = externalId;
		}

		public InvalidOrganizationIdentityException(string orgName, string externalId, Exception innerException) : base(MigrationWorkflowServiceStrings.ErrorInvalidExternalOrganizationId(orgName, externalId), innerException)
		{
			this.orgName = orgName;
			this.externalId = externalId;
		}

		protected InvalidOrganizationIdentityException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.orgName = (string)info.GetValue("orgName", typeof(string));
			this.externalId = (string)info.GetValue("externalId", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("orgName", this.orgName);
			info.AddValue("externalId", this.externalId);
		}

		public string OrgName
		{
			get
			{
				return this.orgName;
			}
		}

		public string ExternalId
		{
			get
			{
				return this.externalId;
			}
		}

		private readonly string orgName;

		private readonly string externalId;
	}
}
