using System;
using System.Globalization;
using System.IO;
using Microsoft.Exchange.Clients.Owa.Core;
using Microsoft.Exchange.Clients.Owa.Core.Controls;

namespace Microsoft.Exchange.Clients.Owa.Premium.Controls
{
	internal class PrintSchedulingAreaVisual : PrintCalendarVisual
	{
		public PrintSchedulingAreaVisual(ISessionContext sessionContext, SchedulingAreaVisual visual, ICalendarDataSource dataSource, int viewStartTime, int viewEndTime) : base(sessionContext, visual, dataSource)
		{
			this.viewStartTime = viewStartTime;
			TimeStripMode persistedTimeStripMode = DailyViewBase.GetPersistedTimeStripMode(base.SessionContext);
			int num = (persistedTimeStripMode == TimeStripMode.FifteenMinutes) ? 4 : 2;
			this.rowCount = (viewEndTime - viewStartTime) * num;
			if (base.Top < 0.0)
			{
				base.Height += base.Top;
				base.Top = 0.0;
			}
			if (base.Top + base.Height > (double)this.rowCount)
			{
				base.Height = (double)this.rowCount - base.Top;
			}
			if (base.Height < 1.0)
			{
				base.Height = 1.0;
			}
		}

		protected override string TimeDescription
		{
			get
			{
				if (base.StartTime.Hour < this.viewStartTime || base.StartTime.Minute != 0)
				{
					return base.StartTime.ToString(base.SessionContext.TimeFormat);
				}
				return string.Empty;
			}
		}

		protected override void RenderVisualPosition(TextWriter writer)
		{
			writer.Write("top: ");
			writer.Write(string.Format(CultureInfo.InvariantCulture, "{0:0.####}", new object[]
			{
				100.0 / (double)this.rowCount * base.Top
			}));
			writer.Write("%; height: ");
			writer.Write(string.Format(CultureInfo.InvariantCulture, "{0:0.####}", new object[]
			{
				100.0 / (double)this.rowCount * base.Height
			}));
			writer.Write("%; width: ");
			writer.Write(string.Format(CultureInfo.InvariantCulture, "{0:0.####}", new object[]
			{
				100.0 * base.Width
			}));
			writer.Write("%; left: ");
			writer.Write(string.Format(CultureInfo.InvariantCulture, "{0:0.####}", new object[]
			{
				100.0 * base.Left
			}));
			writer.Write("%;");
		}

		public const int IconWidth = 12;

		private int rowCount;

		private int viewStartTime;
	}
}
