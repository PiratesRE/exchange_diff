using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Mapi;

namespace Microsoft.Exchange.Data.Storage.MailboxRules
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class DeferredActionWorkItem : WorkItem
	{
		public DeferredActionWorkItem(IRuleEvaluationContext context, string provider, int actionIndex) : base(context, actionIndex)
		{
			this.provider = provider;
			this.actions = new List<DeferredActionWorkItem.ActionInfo>();
			this.folder = context.CurrentFolder;
		}

		public bool HasMoveOrCopy
		{
			get
			{
				foreach (DeferredActionWorkItem.ActionInfo actionInfo in this.actions)
				{
					if (actionInfo.Action is RuleAction.MoveCopy)
					{
						return true;
					}
				}
				return false;
			}
		}

		public string Provider
		{
			get
			{
				return this.provider;
			}
		}

		public override ExecutionStage Stage
		{
			get
			{
				return ExecutionStage.OnCreatedMessage | ExecutionStage.OnDeliveredMessage;
			}
		}

		public byte[] MoveToFolderEntryId
		{
			get
			{
				return this.moveToFolderEntryId;
			}
			set
			{
				this.moveToFolderEntryId = value;
			}
		}

		public byte[] MoveToStoreEntryId
		{
			get
			{
				return this.moveToStoreEntryId;
			}
			set
			{
				this.moveToStoreEntryId = value;
			}
		}

		public void AddAction(RuleAction action)
		{
			this.actions.Add(new DeferredActionWorkItem.ActionInfo(action, base.Rule));
		}

		public override void Execute()
		{
			if (base.Context.DeliveredMessage == null)
			{
				base.Context.TraceDebug("Deferred action: Message was not delivered, deferred actions will be ignored.");
				return;
			}
			if (base.Context.ExecutionStage == ExecutionStage.OnDeliveredMessage)
			{
				this.CreateDAM();
				return;
			}
			if (base.Context.ExecutionStage == ExecutionStage.OnCreatedMessage)
			{
				this.UpdateDeliveredMessage();
			}
		}

		private void CreateDAM()
		{
			MailboxSession mailboxSession = base.Context.StoreSession as MailboxSession;
			base.Context.TraceDebug("Deferred action: Creating deferred action message.");
			using (DeferredAction deferredAction = RuleMessageUtils.CreateDAM(mailboxSession, this.folder.Id.ObjectId, this.provider))
			{
				foreach (DeferredActionWorkItem.ActionInfo actionInfo in this.actions)
				{
					base.Context.TraceDebug<RuleAction>("Deferred action: Adding deferred action {0}.", actionInfo.Action);
					deferredAction.AddAction(actionInfo.RuleId, actionInfo.Action);
				}
				StoreId defaultFolderId = mailboxSession.GetDefaultFolderId(DefaultFolderType.Inbox);
				bool flag = RuleUtil.EqualsStoreId(defaultFolderId, base.Context.FinalDeliveryFolderId);
				deferredAction.Message[ItemSchema.DeferredActionMessageBackPatched] = !flag;
				base.Context.DeliveredMessage.Load(DeferredError.EntryId);
				byte[] value = (byte[])base.Context.DeliveredMessage[StoreObjectSchema.EntryId];
				deferredAction.Message[ItemSchema.OriginalMessageEntryId] = value;
				base.Context.TraceDebug("Deferred action: Saving deferred action message.");
				deferredAction.SerializeActionsAndSave();
				base.Context.TraceDebug("Deferred action: Deferred action message saved.");
			}
		}

		private void UpdateDeliveredMessage()
		{
			base.Context.TraceDebug("Deferred action: Updating delivered message.");
			MessageItem deliveredMessage = base.Context.DeliveredMessage;
			if (!RuleUtil.IsNullOrEmpty(this.MoveToFolderEntryId) && !RuleUtil.IsNullOrEmpty(this.MoveToStoreEntryId))
			{
				deliveredMessage[ItemSchema.MoveToStoreEntryId] = this.MoveToStoreEntryId;
				deliveredMessage[ItemSchema.MoveToFolderEntryId] = this.MoveToFolderEntryId;
			}
			deliveredMessage[ItemSchema.HasDeferredActionMessage] = true;
		}

		private List<DeferredActionWorkItem.ActionInfo> actions;

		private string provider;

		private byte[] moveToStoreEntryId;

		private byte[] moveToFolderEntryId;

		private Folder folder;

		private struct ActionInfo
		{
			public ActionInfo(RuleAction action, Rule rule)
			{
				this.action = action;
				this.ruleId = rule.ID;
			}

			public RuleAction Action
			{
				get
				{
					return this.action;
				}
			}

			public long RuleId
			{
				get
				{
					return this.ruleId;
				}
			}

			private RuleAction action;

			private long ruleId;
		}
	}
}
