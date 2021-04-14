using System;
using Microsoft.Exchange.Entities.Calendaring.EntitySets;
using Microsoft.Exchange.Entities.DataModel.Calendaring;

namespace Microsoft.Exchange.Entities.Calendaring.Interop
{
	internal interface ICalendarInteropSeriesAction
	{
		Guid CommandId { get; }

		void ExecuteOnInstance(Event seriesInstance);

		void RestoreExecutionContext(Events entitySet, SeriesInteropCommand seriesInteropCommand);

		Event CleanUp(Event master);

		Event GetInitialMasterValue();

		Event InitialMasterOperation(Event updateToMaster);
	}
}
