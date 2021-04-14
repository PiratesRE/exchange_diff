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
	internal class UnsupportedUpgradeRequestTypeException : MigrationTransientException
	{
		public UnsupportedUpgradeRequestTypeException(UpgradeRequestTypes upgradeRequest) : base(UpgradeHandlerStrings.UnsupportedUpgradeRequestType(upgradeRequest))
		{
			this.upgradeRequest = upgradeRequest;
		}

		public UnsupportedUpgradeRequestTypeException(UpgradeRequestTypes upgradeRequest, Exception innerException) : base(UpgradeHandlerStrings.UnsupportedUpgradeRequestType(upgradeRequest), innerException)
		{
			this.upgradeRequest = upgradeRequest;
		}

		protected UnsupportedUpgradeRequestTypeException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.upgradeRequest = (UpgradeRequestTypes)info.GetValue("upgradeRequest", typeof(UpgradeRequestTypes));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("upgradeRequest", this.upgradeRequest);
		}

		public UpgradeRequestTypes UpgradeRequest
		{
			get
			{
				return this.upgradeRequest;
			}
		}

		private readonly UpgradeRequestTypes upgradeRequest;
	}
}
