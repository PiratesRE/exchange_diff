using System;

namespace Microsoft.Exchange.Clients.Owa.Core.Controls
{
	internal class FreeBusyVisualContainer : CalendarVisualContainer
	{
		public FreeBusyVisualContainer(DailyViewBase parentView, DateRange dateRange) : base(dateRange)
		{
			if (parentView == null)
			{
				throw new ArgumentNullException("parentView");
			}
			this.mapper = new FreeBusyVisualMapper(parentView, this);
		}

		public override void MapVisuals()
		{
			this.mapper.MapVisuals();
		}

		private FreeBusyVisualMapper mapper;
	}
}
