using System;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Entities.Calendaring;
using Microsoft.Exchange.Entities.EntitySets.Commands;

namespace Microsoft.Exchange.Entities.Calendaring.EntitySets.CalendarGroupCommands
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class DeleteCalendarGroup : DeleteEntityCommand<CalendarGroups>
	{
		protected override ITracer Trace
		{
			get
			{
				return ExTraceGlobals.DeleteCalendarGroupTracer;
			}
		}

		protected override VoidResult OnExecute()
		{
			this.Scope.CalendarGroupDataProvider.Delete(this.Scope.IdConverter.ToStoreObjectId(base.EntityKey), DeleteItemFlags.HardDelete);
			return VoidResult.Value;
		}
	}
}
