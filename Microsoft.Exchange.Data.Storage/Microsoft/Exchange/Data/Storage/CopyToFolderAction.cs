using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Mapi;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class CopyToFolderAction : IdAction
	{
		private CopyToFolderAction(StoreObjectId id, Rule rule) : base(ActionType.CopyToFolderAction, id, rule)
		{
		}

		public static CopyToFolderAction Create(StoreObjectId folderId, Rule rule)
		{
			ActionBase.CheckParams(new object[]
			{
				rule,
				folderId
			});
			if (!IdConverter.IsFolderId(folderId))
			{
				throw new ArgumentException("folderId");
			}
			return new CopyToFolderAction(folderId, rule);
		}

		internal override RuleAction BuildRuleAction()
		{
			MailboxSession mailboxSession = base.Rule.Folder.Session as MailboxSession;
			if (mailboxSession != null)
			{
				byte[] providerLevelItemId = base.Id.ProviderLevelItemId;
				return new RuleAction.InMailboxCopy(providerLevelItemId);
			}
			throw new NotSupportedException();
		}
	}
}
