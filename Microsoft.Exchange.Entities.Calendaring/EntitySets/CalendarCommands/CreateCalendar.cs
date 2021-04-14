using System;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Entities.Calendaring;
using Microsoft.Exchange.Entities.DataModel.Calendaring;
using Microsoft.Exchange.Entities.EntitySets.Commands;

namespace Microsoft.Exchange.Entities.Calendaring.EntitySets.CalendarCommands
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class CreateCalendar : CreateEntityCommand<Calendars, Calendar>
	{
		protected override ITracer Trace
		{
			get
			{
				return ExTraceGlobals.CreateCalendarTracer;
			}
		}

		protected override Calendar OnExecute()
		{
			this.Scope.CalendarGroupEntryDataProvider.Validate(base.Entity, true);
			StoreId storeId = this.Scope.CalendarGroupForNewCalendars.GetStoreId();
			Calendar result;
			using (ICalendarGroup calendarGroup = this.Scope.XsoFactory.BindToCalendarGroup(this.Scope.StoreSession, storeId, null))
			{
				Calendar calendar = this.Scope.CalendarFolderDataProvider.Create(base.Entity);
				base.Entity.RecordKey = calendar.RecordKey;
				base.Entity.CalendarFolderStoreId = calendar.StoreId;
				try
				{
					result = this.Scope.CalendarGroupEntryDataProvider.Create(base.Entity, calendarGroup);
				}
				catch (LocalizedException ex)
				{
					try
					{
						this.Scope.CalendarFolderDataProvider.Delete(calendar.StoreId, DeleteItemFlags.HardDelete);
					}
					catch (Exception ex2)
					{
						throw new AggregateException(new Exception[]
						{
							ex,
							ex2
						});
					}
					throw;
				}
			}
			return result;
		}
	}
}
