using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Mapi;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class DeleteAction : Action
	{
		private DeleteAction(Rule rule) : base(ActionType.DeleteAction, rule)
		{
		}

		public static DeleteAction Create(Rule rule)
		{
			ActionBase.CheckParams(new object[]
			{
				rule
			});
			return new DeleteAction(rule);
		}

		internal override RuleAction BuildRuleAction()
		{
			MailboxSession mailboxSession = base.Rule.Folder.Session as MailboxSession;
			if (mailboxSession != null)
			{
				byte[] providerLevelItemId = mailboxSession.GetDefaultFolderId(DefaultFolderType.DeletedItems).ProviderLevelItemId;
				return new RuleAction.InMailboxMove(providerLevelItemId);
			}
			throw new NotSupportedException();
		}
	}
}
