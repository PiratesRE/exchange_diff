using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Storage.Management;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.MailboxReplicationService.Upgrade14to15
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	internal class OrganizationHasConstraintsException : MigrationPermanentException
	{
		public OrganizationHasConstraintsException(UpgradeRequestTypes requestedType, string orgId, string orgName, string constraints) : base(UpgradeHandlerStrings.OrganizationHasConstraints(requestedType, orgId, orgName, constraints))
		{
			this.requestedType = requestedType;
			this.orgId = orgId;
			this.orgName = orgName;
			this.constraints = constraints;
		}

		public OrganizationHasConstraintsException(UpgradeRequestTypes requestedType, string orgId, string orgName, string constraints, Exception innerException) : base(UpgradeHandlerStrings.OrganizationHasConstraints(requestedType, orgId, orgName, constraints), innerException)
		{
			this.requestedType = requestedType;
			this.orgId = orgId;
			this.orgName = orgName;
			this.constraints = constraints;
		}

		protected OrganizationHasConstraintsException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.requestedType = (UpgradeRequestTypes)info.GetValue("requestedType", typeof(UpgradeRequestTypes));
			this.orgId = (string)info.GetValue("orgId", typeof(string));
			this.orgName = (string)info.GetValue("orgName", typeof(string));
			this.constraints = (string)info.GetValue("constraints", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("requestedType", this.requestedType);
			info.AddValue("orgId", this.orgId);
			info.AddValue("orgName", this.orgName);
			info.AddValue("constraints", this.constraints);
		}

		public UpgradeRequestTypes RequestedType
		{
			get
			{
				return this.requestedType;
			}
		}

		public string OrgId
		{
			get
			{
				return this.orgId;
			}
		}

		public string OrgName
		{
			get
			{
				return this.orgName;
			}
		}

		public string Constraints
		{
			get
			{
				return this.constraints;
			}
		}

		private readonly UpgradeRequestTypes requestedType;

		private readonly string orgId;

		private readonly string orgName;

		private readonly string constraints;
	}
}
