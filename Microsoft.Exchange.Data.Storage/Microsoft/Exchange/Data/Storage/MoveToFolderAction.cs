using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Mapi;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class MoveToFolderAction : IdAction
	{
		private MoveToFolderAction(StoreObjectId id, Rule rule) : base(ActionType.MoveToFolderAction, id, rule)
		{
		}

		public static MoveToFolderAction Create(StoreObjectId folderId, Rule rule)
		{
			ActionBase.CheckParams(new object[]
			{
				rule,
				folderId
			});
			if (!IdConverter.IsFolderId(folderId))
			{
				rule.ThrowValidateException(delegate
				{
					throw new ArgumentNullException("folderId");
				}, "folderId");
			}
			return new MoveToFolderAction(folderId, rule);
		}

		internal override RuleAction BuildRuleAction()
		{
			StoreSession session = base.Rule.Folder.Session;
			byte[] providerLevelItemId = base.Id.ProviderLevelItemId;
			return new RuleAction.InMailboxMove(providerLevelItemId);
		}
	}
}
