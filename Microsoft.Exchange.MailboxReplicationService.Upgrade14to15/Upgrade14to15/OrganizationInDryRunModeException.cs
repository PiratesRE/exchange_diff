using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Storage.Management;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.MailboxReplicationService.Upgrade14to15
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	internal class OrganizationInDryRunModeException : MigrationTransientException
	{
		public OrganizationInDryRunModeException(string tenant, string requestedType) : base(UpgradeHandlerStrings.OrganizationInDryRunMode(tenant, requestedType))
		{
			this.tenant = tenant;
			this.requestedType = requestedType;
		}

		public OrganizationInDryRunModeException(string tenant, string requestedType, Exception innerException) : base(UpgradeHandlerStrings.OrganizationInDryRunMode(tenant, requestedType), innerException)
		{
			this.tenant = tenant;
			this.requestedType = requestedType;
		}

		protected OrganizationInDryRunModeException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.tenant = (string)info.GetValue("tenant", typeof(string));
			this.requestedType = (string)info.GetValue("requestedType", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("tenant", this.tenant);
			info.AddValue("requestedType", this.requestedType);
		}

		public string Tenant
		{
			get
			{
				return this.tenant;
			}
		}

		public string RequestedType
		{
			get
			{
				return this.requestedType;
			}
		}

		private readonly string tenant;

		private readonly string requestedType;
	}
}
