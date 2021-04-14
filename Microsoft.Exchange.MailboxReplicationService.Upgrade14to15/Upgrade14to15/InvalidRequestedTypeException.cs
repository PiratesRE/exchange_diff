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
	internal class InvalidRequestedTypeException : MigrationTransientException
	{
		public InvalidRequestedTypeException(string orgId, UpgradeRequestTypes currentType, string requestedType) : base(UpgradeHandlerStrings.InvalidRequestedType(orgId, currentType, requestedType))
		{
			this.orgId = orgId;
			this.currentType = currentType;
			this.requestedType = requestedType;
		}

		public InvalidRequestedTypeException(string orgId, UpgradeRequestTypes currentType, string requestedType, Exception innerException) : base(UpgradeHandlerStrings.InvalidRequestedType(orgId, currentType, requestedType), innerException)
		{
			this.orgId = orgId;
			this.currentType = currentType;
			this.requestedType = requestedType;
		}

		protected InvalidRequestedTypeException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.orgId = (string)info.GetValue("orgId", typeof(string));
			this.currentType = (UpgradeRequestTypes)info.GetValue("currentType", typeof(UpgradeRequestTypes));
			this.requestedType = (string)info.GetValue("requestedType", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("orgId", this.orgId);
			info.AddValue("currentType", this.currentType);
			info.AddValue("requestedType", this.requestedType);
		}

		public string OrgId
		{
			get
			{
				return this.orgId;
			}
		}

		public UpgradeRequestTypes CurrentType
		{
			get
			{
				return this.currentType;
			}
		}

		public string RequestedType
		{
			get
			{
				return this.requestedType;
			}
		}

		private readonly string orgId;

		private readonly UpgradeRequestTypes currentType;

		private readonly string requestedType;
	}
}
