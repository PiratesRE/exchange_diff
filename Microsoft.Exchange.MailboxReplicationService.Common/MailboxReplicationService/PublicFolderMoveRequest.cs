using System;
using Microsoft.Exchange.Data.Directory;

namespace Microsoft.Exchange.MailboxReplicationService
{
	[Serializable]
	public sealed class PublicFolderMoveRequest : RequestBase
	{
		public PublicFolderMoveRequest()
		{
		}

		internal PublicFolderMoveRequest(IRequestIndexEntry index) : base(index)
		{
		}

		public new ADObjectId SourceMailbox
		{
			get
			{
				return base.SourceMailbox;
			}
		}

		public new ADObjectId TargetMailbox
		{
			get
			{
				return base.TargetMailbox;
			}
		}

		public override string ToString()
		{
			if (base.Name != null && base.OrganizationId != null)
			{
				return string.Format("{0}\\{1}", base.OrganizationId.ToString(), base.Name);
			}
			return base.ToString();
		}
	}
}
