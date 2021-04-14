using System;

namespace Microsoft.Exchange.MailboxReplicationService
{
	internal class MailboxMoveTargetResource : MailboxMoveResource
	{
		private MailboxMoveTargetResource(Guid mailboxGuid) : base(mailboxGuid)
		{
		}

		public override string ResourceType
		{
			get
			{
				return "MailboxMoveTarget";
			}
		}

		public static readonly ResourceCache<MailboxMoveTargetResource> Cache = new ResourceCache<MailboxMoveTargetResource>((Guid id) => new MailboxMoveTargetResource(id));
	}
}
