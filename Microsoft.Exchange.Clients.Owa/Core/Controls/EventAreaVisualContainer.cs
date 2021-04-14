using System;

namespace Microsoft.Exchange.Clients.Owa.Core.Controls
{
	internal class EventAreaVisualContainer : CalendarVisualContainer
	{
		public EventAreaVisualContainer(DailyViewBase parentView, DateRange dateRange) : base(dateRange)
		{
			if (parentView == null)
			{
				throw new ArgumentNullException("parentView");
			}
			this.mapper = new EventAreaVisualMapper(parentView, this);
		}

		public override void MapVisuals()
		{
			this.mapper.MapVisuals();
		}

		public EventAreaVisualMapper Mapper
		{
			get
			{
				return this.mapper;
			}
		}

		private EventAreaVisualMapper mapper;
	}
}
