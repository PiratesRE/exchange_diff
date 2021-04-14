using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Mapi;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class ServerReplyMessageAction : IdAction
	{
		private ServerReplyMessageAction(StoreObjectId id, Guid guidTemplate, Rule rule) : base(ActionType.ServerReplyMessageAction, id, rule)
		{
			this.guidTemplate = guidTemplate;
		}

		public static ServerReplyMessageAction Create(StoreObjectId messageId, Guid guidTemplate, Rule rule)
		{
			ActionBase.CheckParams(new object[]
			{
				rule,
				messageId
			});
			if (!IdConverter.IsMessageId(messageId))
			{
				throw new ArgumentException("messageId");
			}
			return new ServerReplyMessageAction(messageId, guidTemplate, rule);
		}

		internal override RuleAction BuildRuleAction()
		{
			return new RuleAction.Reply(base.Id.ProviderLevelItemId, this.GuidTemplate, RuleAction.Reply.ActionFlags.None);
		}

		public Guid GuidTemplate
		{
			get
			{
				return this.guidTemplate;
			}
		}

		private readonly Guid guidTemplate;
	}
}
