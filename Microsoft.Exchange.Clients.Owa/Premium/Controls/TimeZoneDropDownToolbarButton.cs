using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Exchange.Clients.Owa.Core;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Clients.Owa.Premium.Controls
{
	internal sealed class TimeZoneDropDownToolbarButton : ToolbarButton
	{
		public TimeZoneDropDownToolbarButton() : base("noaction", ToolbarButtonFlags.NoAction | ToolbarButtonFlags.HasInnerControl, -1018465893, ThemeFileId.None)
		{
		}

		public override void RenderControl(TextWriter writer)
		{
			List<DropDownListItem> list = new List<DropDownListItem>();
			ExTimeZone timeZone = OwaContext.Current.SessionContext.TimeZone;
			string selectedValue = string.Empty;
			foreach (ExTimeZone exTimeZone in ExTimeZoneEnumerator.Instance)
			{
				if (string.Equals(exTimeZone.Id, timeZone.Id, StringComparison.OrdinalIgnoreCase))
				{
					selectedValue = exTimeZone.Id;
				}
				list.Add(new DropDownListItem(exTimeZone.Id, exTimeZone.LocalizableDisplayName.ToString()));
			}
			DropDownList dropDownList = new DropDownList("divTimeZoneList", selectedValue, list.ToArray());
			dropDownList.Render(writer);
			writer.Write("<span id=\"divMeasure\" class=\"tbLh tbBefore tbAfter fltAfter\">");
			writer.Write(SanitizedHtmlString.FromStringId(2126414109));
			writer.Write("</span>");
		}
	}
}
