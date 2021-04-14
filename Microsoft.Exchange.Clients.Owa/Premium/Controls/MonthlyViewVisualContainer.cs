using System;
using Microsoft.Exchange.Clients.Owa.Core.Controls;

namespace Microsoft.Exchange.Clients.Owa.Premium.Controls
{
	internal class MonthlyViewVisualContainer : CalendarVisualContainer
	{
		public MonthlyViewVisualContainer(CalendarViewBase parentView) : base(null)
		{
			if (parentView == null)
			{
				throw new ArgumentNullException("parentView");
			}
			this.mapper = new MonthlyViewVisualMapper(parentView, new MonthlyViewVisualComparer(parentView.DataSource), this);
		}

		public override void MapVisuals()
		{
			this.mapper.MapVisuals();
		}

		public MonthlyViewVisualMapper Mapper
		{
			get
			{
				return this.mapper;
			}
		}

		private MonthlyViewVisualMapper mapper;
	}
}
