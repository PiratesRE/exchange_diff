using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.ReliableActions;
using Microsoft.Exchange.Entities.Calendaring.TypeConversion.PropertyAccessors.StorageAccessors;
using Microsoft.Exchange.Entities.DataModel.ReliableActions;
using Microsoft.Exchange.Entities.TypeConversion;
using Microsoft.Exchange.Extensions;

namespace Microsoft.Exchange.Entities.Calendaring.TypeConversion.PropertyTranslationRules
{
	internal class ActionQueueRules : ITranslationRule<ICalendarItemSeries, IActionQueue>
	{
		public void FromLeftToRightType(ICalendarItemSeries calendarItemSeries, IActionQueue actionQueue)
		{
			ActionInfo[] originalActions;
			CalendarItemAccessors.CalendarInteropActionQueue.TryGetValue(calendarItemSeries, out originalActions);
			actionQueue.OriginalActions = originalActions;
			bool hasData;
			CalendarItemAccessors.CalendarInteropActionQueueHasData.TryGetValue(calendarItemSeries, out hasData);
			actionQueue.HasData = hasData;
		}

		public void FromRightToLeftType(ICalendarItemSeries calendarItemSeries, IActionQueue actionQueue)
		{
			bool flag = !actionQueue.ActionsToAdd.IsNullOrEmpty<ActionInfo>();
			bool flag2 = !actionQueue.ActionsToRemove.IsNullOrEmpty<Guid>();
			ActionInfo[] source;
			if ((flag || flag2) && CalendarItemAccessors.CalendarInteropActionQueue.TryGetValue(calendarItemSeries, out source))
			{
				List<ActionInfo> list = source.ToList<ActionInfo>();
				if (flag)
				{
					list.AddRange(actionQueue.ActionsToAdd);
				}
				if (flag2)
				{
					Guid[] actionsToRemove = actionQueue.ActionsToRemove;
					for (int i = 0; i < actionsToRemove.Length; i++)
					{
						Guid guid = actionsToRemove[i];
						list.RemoveAll((ActionInfo info) => info.Id == guid);
					}
				}
				CalendarItemAccessors.CalendarInteropActionQueue.Set(calendarItemSeries, list.ToArray());
			}
		}
	}
}
