using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Entities.Calendaring;
using Microsoft.Exchange.Entities.DataModel.Calendaring;
using Microsoft.Exchange.Entities.EntitySets.Commands;
using Microsoft.Exchange.Entities.EntitySets.Linq.ExtensionMethods;

namespace Microsoft.Exchange.Entities.Calendaring.EntitySets.CalendarGroupCommands
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class FindCalendarGroups : FindEntitiesCommand<CalendarGroups, CalendarGroup>
	{
		protected override ITracer Trace
		{
			get
			{
				return ExTraceGlobals.FindCalendarGroupsTracer;
			}
		}

		protected IEnumerable<CalendarGroup> FindAllCalendarGroups()
		{
			bool expandCalendars = base.ShouldExpand("Calendars");
			foreach (CalendarGroupInfo group in this.GetCalendarGroupInfoList(expandCalendars))
			{
				CalendarGroup entity = this.Scope.IdConverter.CreateBasicEntity<CalendarGroup>(group.Id, this.Scope.StoreSession);
				entity.Name = group.GroupName;
				entity.ClassId = group.GroupClassId;
				if (expandCalendars)
				{
					entity.Calendars = from calendarInfo in @group.Calendars
					where calendarInfo is LocalCalendarGroupEntryInfo
					select this.ConvertCalendar(calendarInfo);
				}
				yield return entity;
			}
			yield break;
		}

		protected override IEnumerable<CalendarGroup> OnExecute()
		{
			IEnumerable<CalendarGroup> source = this.FindAllCalendarGroups();
			return base.QueryOptions.ApplyTo(source.AsQueryable<CalendarGroup>());
		}

		private Calendar ConvertCalendar(CalendarGroupEntryInfo calendarInfo)
		{
			Calendar calendar = this.Scope.IdConverter.CreateBasicEntity<Calendar>(calendarInfo.Id, this.Scope.StoreSession);
			calendar.CalendarFolderStoreId = calendarInfo.CalendarId;
			calendar.Name = calendarInfo.CalendarName;
			calendar.Color = calendarInfo.CalendarColor;
			return calendar;
		}

		private IEnumerable<CalendarGroupInfo> GetCalendarGroupInfoList(bool ensureCalendarInfoIsLoaded)
		{
			bool flag = false;
			int num = 0;
			CalendarGroupInfoList calendarGroupsView;
			do
			{
				num++;
				calendarGroupsView = this.Scope.XsoFactory.GetCalendarGroupsView(this.Scope.StoreSession);
				if (!calendarGroupsView.DefaultGroups.ContainsKey(CalendarGroupType.MyCalendars))
				{
					using (ICalendarGroup calendarGroup = this.Scope.XsoFactory.BindToCalendarGroup(this.Scope.StoreSession, CalendarGroupType.MyCalendars))
					{
						calendarGroupsView.Add(calendarGroup.GetCalendarGroupInfo());
						flag = true;
					}
				}
				if (!calendarGroupsView.DefaultGroups.ContainsKey(CalendarGroupType.OtherCalendars))
				{
					using (ICalendarGroup calendarGroup2 = this.Scope.XsoFactory.BindToCalendarGroup(this.Scope.StoreSession, CalendarGroupType.OtherCalendars))
					{
						calendarGroupsView.Add(calendarGroup2.GetCalendarGroupInfo());
						flag = true;
					}
				}
			}
			while (ensureCalendarInfoIsLoaded && flag && num < 2);
			return calendarGroupsView;
		}
	}
}
