using System;

namespace Microsoft.Exchange.MailboxReplicationService
{
	internal class MailboxMoveSourceResource : MailboxMoveResource
	{
		private MailboxMoveSourceResource(Guid mailboxGuid) : base(mailboxGuid)
		{
		}

		public override string ResourceType
		{
			get
			{
				return "MailboxMoveSource";
			}
		}

		public static readonly ResourceCache<MailboxMoveSourceResource> Cache = new ResourceCache<MailboxMoveSourceResource>((Guid id) => new MailboxMoveSourceResource(id));
	}
}
