using System;
using System.IO;
using Microsoft.Exchange.Clients.Owa.Core;

namespace Microsoft.Exchange.Clients.Owa.Premium.Controls
{
	internal sealed class LanguageDropDownList : DropDownList
	{
		public LanguageDropDownList(string id, string selectedValue, LanguageDropDownListItem[] listItems) : base(id, selectedValue, listItems)
		{
			this.languageListItems = listItems;
		}

		protected override void RenderListItems(TextWriter writer)
		{
			foreach (LanguageDropDownListItem languageDropDownListItem in this.languageListItems)
			{
				DropDownList.RenderListItemHtml(writer, languageDropDownListItem.ItemValue, Utilities.SanitizeHtmlEncode(languageDropDownListItem.Display), languageDropDownListItem.ItemId, languageDropDownListItem.IsBold, languageDropDownListItem.IsRtl);
			}
		}

		private LanguageDropDownListItem[] languageListItems;
	}
}
