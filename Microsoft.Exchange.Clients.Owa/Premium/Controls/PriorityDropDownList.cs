using System;
using System.IO;
using Microsoft.Exchange.Clients.Owa.Core;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.Clients.Owa.Premium.Controls
{
	internal sealed class PriorityDropDownList : DropDownList
	{
		public PriorityDropDownList(string id, Importance priority)
		{
			int num = (int)priority;
			base..ctor(id, num.ToString(), null);
			this.priority = priority;
		}

		protected override void RenderSelectedValue(TextWriter writer)
		{
			writer.Write(LocalizedStrings.GetHtmlEncoded(TaskUtilities.GetPriorityString(this.priority)));
		}

		protected override DropDownListItem[] CreateListItems()
		{
			DropDownListItem[] array = new DropDownListItem[PriorityDropDownList.importanceTypes.Length];
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = new DropDownListItem((int)PriorityDropDownList.importanceTypes[i], TaskUtilities.GetPriorityString(PriorityDropDownList.importanceTypes[i]));
			}
			return array;
		}

		private static readonly Importance[] importanceTypes = new Importance[]
		{
			Importance.Low,
			Importance.Normal,
			Importance.High
		};

		private Importance priority;
	}
}
