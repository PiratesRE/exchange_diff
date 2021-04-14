using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Mapi;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal abstract class ActionBase
	{
		private static Dictionary<ActionType, ActionOrder> BuildOrder(Dictionary<ActionType, ActionOrder> order)
		{
			return new Dictionary<ActionType, ActionOrder>(16)
			{
				{
					ActionType.MoveToFolderAction,
					ActionOrder.MoveToFolderAction
				},
				{
					ActionType.DeleteAction,
					ActionOrder.DeleteAction
				},
				{
					ActionType.CopyToFolderAction,
					ActionOrder.CopyToFolderAction
				},
				{
					ActionType.ForwardToRecipientsAction,
					ActionOrder.ForwardToRecipientsAction
				},
				{
					ActionType.ForwardAsAttachmentToRecipientsAction,
					ActionOrder.ForwardAsAttachmentToRecipientsAction
				},
				{
					ActionType.SendSmsAlertToRecipientsAction,
					ActionOrder.SendSmsAlertToRecipientsAction
				},
				{
					ActionType.RedirectToRecipientsAction,
					ActionOrder.RedirectToRecipientsAction
				},
				{
					ActionType.ServerReplyMessageAction,
					ActionOrder.ServerReplyMessageAction
				},
				{
					ActionType.MarkImportanceAction,
					ActionOrder.MarkImportanceAction
				},
				{
					ActionType.MarkSensitivityAction,
					ActionOrder.MarkSensitivityAction
				},
				{
					ActionType.AssignCategoriesAction,
					ActionOrder.AssignCategoriesAction
				},
				{
					ActionType.FlagMessageAction,
					ActionOrder.FlagMessageAction
				},
				{
					ActionType.MarkAsReadAction,
					ActionOrder.MarkAsReadAction
				},
				{
					ActionType.StopProcessingAction,
					ActionOrder.StopProcessingAction
				},
				{
					ActionType.PermanentDeleteAction,
					ActionOrder.PermanentDeleteAction
				}
			};
		}

		protected static void CheckParams(params object[] parameters)
		{
			Rule rule = Rule.CheckRuleParameter(parameters);
			int i;
			for (i = 1; i < parameters.Length; i++)
			{
				if (parameters[i] == null)
				{
					rule.ThrowValidateException(delegate
					{
						throw new ArgumentNullException("parameter " + i);
					}, "parameter " + i);
				}
				IList<Participant> list = parameters[i] as IList<Participant>;
				if (list != null && list.Count == 0)
				{
					rule.ThrowValidateException(delegate
					{
						throw new ArgumentException("participants");
					}, "participants");
				}
			}
		}

		protected ActionBase(ActionType actionType, Rule rule)
		{
			this.actionType = actionType;
			this.rule = rule;
		}

		public ActionType ActionType
		{
			get
			{
				return this.actionType;
			}
		}

		public virtual Rule.ProviderIdEnum ProviderId
		{
			get
			{
				return Rule.ProviderIdEnum.OL98Plus;
			}
		}

		internal Rule Rule
		{
			get
			{
				return this.rule;
			}
		}

		internal ActionOrder ActionOrder
		{
			get
			{
				return ActionBase.ActionBuildOrder[this.ActionType];
			}
		}

		internal abstract RuleAction BuildRuleAction();

		internal static Dictionary<ActionType, ActionOrder> ActionBuildOrder = ActionBase.BuildOrder(ActionBase.ActionBuildOrder);

		internal static ActionOrderComparer ActionOrderComparer = new ActionOrderComparer();

		private readonly ActionType actionType;

		private readonly Rule rule;
	}
}
