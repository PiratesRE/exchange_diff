using System;
using System.Globalization;
using System.IO;
using System.Text;
using Microsoft.Exchange.Clients.Owa.Core;
using Microsoft.Exchange.Clients.Owa.Core.Controls;

namespace Microsoft.Exchange.Clients.Owa.Premium.Controls
{
	internal class PrintEventAreaVisual : PrintCalendarVisual
	{
		public PrintEventAreaVisual(ISessionContext sessionContext, EventAreaVisual visual, ICalendarDataSource dataSource, int dayCount) : base(sessionContext, visual, dataSource)
		{
			this.dayCount = dayCount;
			this.leftBreak = visual.LeftBreak;
			if (sessionContext.IsRtl)
			{
				base.Left = (double)this.dayCount - base.Left - base.Width;
			}
		}

		protected override string TimeDescription
		{
			get
			{
				StringBuilder stringBuilder = new StringBuilder();
				if (this.leftBreak)
				{
					stringBuilder.Append(base.StartTime.ToString(base.SessionContext.DateFormat));
				}
				if (base.StartTime.Minute != 0 || base.StartTime.Hour != 0)
				{
					if (this.leftBreak)
					{
						stringBuilder.Append(" ");
					}
					stringBuilder.Append(base.StartTime.ToString(base.SessionContext.TimeFormat));
				}
				return stringBuilder.ToString();
			}
		}

		protected override void RenderVisualPosition(TextWriter writer)
		{
			writer.Write("left:");
			writer.Write(string.Format(CultureInfo.InvariantCulture, "{0:0.####}", new object[]
			{
				100.0 / (double)this.dayCount * base.Left
			}));
			writer.Write("%; top:");
			writer.Write(string.Format(CultureInfo.InvariantCulture, "{0:0.####}", new object[]
			{
				base.Top * 20.0
			}));
			writer.Write("px; width:");
			writer.Write(string.Format(CultureInfo.InvariantCulture, "{0:0.####}", new object[]
			{
				100.0 / (double)this.dayCount * base.Width
			}));
			writer.Write("%; height:");
			writer.Write(string.Format(CultureInfo.InvariantCulture, "{0:0.####}", new object[]
			{
				20
			}));
			writer.Write("px; ");
		}

		protected override void RenderExtraClasses(TextWriter writer)
		{
			writer.Write(" eventAreaVisual");
		}

		public const int RowHeight = 20;

		private int dayCount;

		private bool leftBreak;
	}
}
