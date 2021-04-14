using System;
using System.IO;
using Microsoft.Exchange.Clients.Owa.Core;

namespace Microsoft.Exchange.Clients.Owa.Premium.Controls
{
	internal sealed class DurationDropDownList : DropDownList
	{
		private DurationDropDownList(int duration, string id) : base(id, false, null, null)
		{
			this.duration = duration;
		}

		public static void RenderDurationPicker(TextWriter writer, int duration, string id)
		{
			DurationDropDownList durationDropDownList = new DurationDropDownList(duration, id);
			durationDropDownList.Render(writer);
		}

		protected override void RenderSelectedValue(TextWriter writer)
		{
			writer.Write("<input type=text id=\"txtInput\" value=\"");
			writer.Write(DateTimeUtilities.FormatDuration(this.duration));
			writer.Write("\">");
		}

		protected override void RenderListItems(TextWriter writer)
		{
		}

		protected override void RenderExpandoData(TextWriter writer)
		{
			base.RenderExpandoData(writer);
			writer.Write(" L_Dec=\"");
			writer.Write(SanitizedHtmlString.FromStringId(-1032346272));
			writer.Write("\"");
		}

		private int duration;
	}
}
