using System;

namespace Microsoft.Exchange.Clients.Owa.Core.Controls
{
	internal class SchedulingAreaVisualContainer : CalendarVisualContainer
	{
		public SchedulingAreaVisualContainer(DailyViewBase parentView, DateRange dateRange) : base(dateRange)
		{
			if (parentView == null)
			{
				throw new ArgumentNullException("parentView");
			}
			this.mapper = new SchedulingAreaVisualMapper(parentView, this);
		}

		public override void MapVisuals()
		{
			this.mapper.MapVisuals();
		}

		public SchedulingAreaVisualMapper Mapper
		{
			get
			{
				return this.mapper;
			}
		}

		private SchedulingAreaVisualMapper mapper;
	}
}
