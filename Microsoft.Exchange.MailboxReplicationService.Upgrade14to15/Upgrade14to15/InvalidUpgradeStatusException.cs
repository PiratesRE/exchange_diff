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
	internal class InvalidUpgradeStatusException : MigrationTransientException
	{
		public InvalidUpgradeStatusException(string id, UpgradeStatusTypes currentStatus) : base(UpgradeHandlerStrings.InvalidUpgradeStatus(id, currentStatus))
		{
			this.id = id;
			this.currentStatus = currentStatus;
		}

		public InvalidUpgradeStatusException(string id, UpgradeStatusTypes currentStatus, Exception innerException) : base(UpgradeHandlerStrings.InvalidUpgradeStatus(id, currentStatus), innerException)
		{
			this.id = id;
			this.currentStatus = currentStatus;
		}

		protected InvalidUpgradeStatusException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.id = (string)info.GetValue("id", typeof(string));
			this.currentStatus = (UpgradeStatusTypes)info.GetValue("currentStatus", typeof(UpgradeStatusTypes));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("id", this.id);
			info.AddValue("currentStatus", this.currentStatus);
		}

		public string Id
		{
			get
			{
				return this.id;
			}
		}

		public UpgradeStatusTypes CurrentStatus
		{
			get
			{
				return this.currentStatus;
			}
		}

		private readonly string id;

		private readonly UpgradeStatusTypes currentStatus;
	}
}
