using System;
using System.IO;
using Microsoft.Exchange.Clients.Owa.Core;

namespace Microsoft.Exchange.Clients.Owa.Premium.Controls
{
	internal sealed class CalendarItemShowTimeAsContextMenu : ContextMenu
	{
		public CalendarItemShowTimeAsContextMenu(UserContext userContext) : base("divCalShwTmAsCm", userContext)
		{
		}

		protected override void RenderMenuItems(TextWriter output)
		{
			if (output == null)
			{
				throw new ArgumentNullException("output");
			}
			base.RenderMenuItem(output, -971703552, ThemeFileId.Clear, "divShowTimeFree", "free");
			base.RenderMenuItem(output, 1797669216, ThemeFileId.Clear, "divShowTimeTentative", "tentative");
			base.RenderMenuItem(output, 2052801377, ThemeFileId.Clear, "divShowTimeBusy", "busy");
			base.RenderMenuItem(output, 2047193656, ThemeFileId.Clear, "divShowTimeOOF", "unavailable");
		}
	}
}
