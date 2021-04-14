using System;
using System.IO;
using Microsoft.Exchange.Clients.Owa.Core;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.Clients.Owa.Premium.Controls
{
	internal sealed class StatusDropDownList : DropDownList
	{
		public StatusDropDownList(string id, TaskStatus statusMapping)
		{
			int num = (int)statusMapping;
			base..ctor(id, num.ToString(), null);
			this.statusMapping = statusMapping;
		}

		protected override void RenderSelectedValue(TextWriter writer)
		{
			writer.Write(LocalizedStrings.GetHtmlEncoded(TaskUtilities.GetStatusString(this.statusMapping)));
		}

		protected override DropDownListItem[] CreateListItems()
		{
			DropDownListItem[] array = new DropDownListItem[StatusDropDownList.taskStatusTypes.Length];
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = new DropDownListItem((int)StatusDropDownList.taskStatusTypes[i], TaskUtilities.GetStatusString(StatusDropDownList.taskStatusTypes[i]));
			}
			return array;
		}

		private static readonly TaskStatus[] taskStatusTypes = new TaskStatus[]
		{
			TaskStatus.NotStarted,
			TaskStatus.InProgress,
			TaskStatus.Completed,
			TaskStatus.WaitingOnOthers,
			TaskStatus.Deferred
		};

		private TaskStatus statusMapping;
	}
}
