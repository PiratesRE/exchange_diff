using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.MailboxLoadBalance.Directory;

namespace Microsoft.Exchange.MailboxLoadBalance.Provisioning
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	[DataContract]
	internal class MailboxProvisioningResult : IExtensibleDataObject
	{
		[DataMember]
		public DirectoryIdentity Database { get; set; }

		public ExtensionDataObject ExtensionData { get; set; }

		[DataMember]
		public IMailboxProvisioningConstraints MailboxProvisioningConstraints { get; set; }

		public MailboxProvisioningResultStatus Status { get; set; }

		[DataMember]
		private int StatusInt
		{
			get
			{
				return (int)this.Status;
			}
			set
			{
				this.Status = (MailboxProvisioningResultStatus)value;
			}
		}

		public void ValidateSelection()
		{
			switch (this.Status)
			{
			case MailboxProvisioningResultStatus.Valid:
				return;
			case MailboxProvisioningResultStatus.InsufficientCapacity:
				throw new InsufficientCapacityProvisioningException();
			case MailboxProvisioningResultStatus.ConstraintCouldNotBeSatisfied:
				throw new ConstraintCouldNotBeSatisfiedProvisioningException(string.Format("{0}", this.MailboxProvisioningConstraints));
			default:
				throw new UnknownProvisioningStatusException();
			}
		}
	}
}
