using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.MailboxReplicationService
{
	internal class ReplayActionsQueue
	{
		internal ReplayActionsQueue(int capacity)
		{
			this.queue = new Queue<ReplayAction>(capacity);
			this.optimizeMap = new EntryIdMap<List<ReplayAction>>(capacity);
		}

		internal int Count
		{
			get
			{
				return this.queue.Count;
			}
		}

		internal void Enqueue(ReplayAction action)
		{
			MrsTracer.Provider.Function("ReplayActionsQueue.Enqueue", new object[0]);
			byte[] itemId = action.ItemId;
			List<ReplayAction> list;
			if (!this.optimizeMap.TryGetValue(itemId, out list))
			{
				this.optimizeMap.Add(itemId, new List<ReplayAction>
				{
					action
				});
			}
			else
			{
				foreach (ReplayAction replayAction in list)
				{
					ActionUpdateGroup actionUpdateGroup;
					ActionUpdateGroup actionUpdateGroup2;
					if (!replayAction.Ignored && ReplayActionsQueue.IsActionUpdate(replayAction, out actionUpdateGroup) && ReplayActionsQueue.IsActionUpdate(action, out actionUpdateGroup2) && actionUpdateGroup == actionUpdateGroup2)
					{
						MrsTracer.Service.Debug("Action {0} already existed in the batch. Replacing with action: {1}", new object[]
						{
							replayAction,
							action
						});
						replayAction.Ignored = true;
					}
				}
				list.Add(action);
			}
			this.queue.Enqueue(action);
		}

		internal bool TryQueue(out ReplayAction action)
		{
			if (this.queue.Count > 0)
			{
				action = this.queue.Dequeue();
				return true;
			}
			action = null;
			return false;
		}

		private static bool IsActionUpdate(ReplayAction action, out ActionUpdateGroup updateGroup)
		{
			updateGroup = ActionUpdateGroup.None;
			switch (action.Id)
			{
			case ActionId.MarkAsRead:
			case ActionId.MarkAsUnRead:
				updateGroup = ActionUpdateGroup.Read;
				return true;
			case ActionId.Flag:
			case ActionId.FlagClear:
			case ActionId.FlagComplete:
				updateGroup = ActionUpdateGroup.Flag;
				return true;
			case ActionId.UpdateCalendarEvent:
				updateGroup = ActionUpdateGroup.CalendarEvent;
				return true;
			}
			return false;
		}

		private readonly Queue<ReplayAction> queue;

		private readonly EntryIdMap<List<ReplayAction>> optimizeMap;
	}
}
