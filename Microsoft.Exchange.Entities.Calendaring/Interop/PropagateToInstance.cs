using System;
using Microsoft.Exchange.Entities.DataModel.Calendaring;

namespace Microsoft.Exchange.Entities.Calendaring.Interop
{
	internal class PropagateToInstance : SeriesPendingActionsInterop
	{
		public PropagateToInstance(ICalendarInteropLog logger, ISeriesActionParser parser = null) : base(logger, parser)
		{
		}

		public Event Instance { get; set; }

		protected override void PropagateToSeriesInstances()
		{
			this.ExecuteOnInstance(this.Instance);
		}

		protected override void TryCleanUp()
		{
		}
	}
}
