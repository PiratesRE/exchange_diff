using System;
using System.IO;
using Microsoft.Exchange.Clients.Owa.Core;

namespace Microsoft.Exchange.Clients.Owa.Premium.Controls
{
	internal sealed class WorkDurationDropDownList : DropDownList
	{
		public WorkDurationDropDownList(string id, DurationUnit duration)
		{
			int num = (int)duration;
			base..ctor(id, num.ToString(), null);
			this.duration = duration;
		}

		protected override void RenderSelectedValue(TextWriter writer)
		{
			writer.Write(LocalizedStrings.GetHtmlEncoded(TaskUtilities.GetWorkDurationUnitString(this.duration)));
		}

		protected override DropDownListItem[] CreateListItems()
		{
			DropDownListItem[] array = new DropDownListItem[WorkDurationDropDownList.durationUnitTypes.Length];
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = new DropDownListItem((int)WorkDurationDropDownList.durationUnitTypes[i], TaskUtilities.GetWorkDurationUnitString(WorkDurationDropDownList.durationUnitTypes[i]));
			}
			return array;
		}

		private static readonly DurationUnit[] durationUnitTypes = new DurationUnit[]
		{
			DurationUnit.Minutes,
			DurationUnit.Hours,
			DurationUnit.Days,
			DurationUnit.Weeks
		};

		private DurationUnit duration;
	}
}
