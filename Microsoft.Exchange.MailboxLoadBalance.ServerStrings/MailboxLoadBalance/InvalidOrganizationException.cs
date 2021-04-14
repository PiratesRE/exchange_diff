using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.MailboxLoadBalance
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	internal class InvalidOrganizationException : MailboxLoadBalancePermanentException
	{
		public InvalidOrganizationException(string orgName) : base(MigrationWorkflowServiceStrings.ErrorInvalidOrganization(orgName))
		{
			this.orgName = orgName;
		}

		public InvalidOrganizationException(string orgName, Exception innerException) : base(MigrationWorkflowServiceStrings.ErrorInvalidOrganization(orgName), innerException)
		{
			this.orgName = orgName;
		}

		protected InvalidOrganizationException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.orgName = (string)info.GetValue("orgName", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("orgName", this.orgName);
		}

		public string OrgName
		{
			get
			{
				return this.orgName;
			}
		}

		private readonly string orgName;
	}
}
