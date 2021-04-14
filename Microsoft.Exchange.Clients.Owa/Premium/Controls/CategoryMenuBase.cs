using System;
using System.IO;
using Microsoft.Exchange.Clients.Owa.Core;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.Clients.Owa.Premium.Controls
{
	public abstract class CategoryMenuBase : ContextMenu
	{
		public CategoryMenuBase(string id, UserContext userContext) : base(id, userContext)
		{
		}

		internal void RenderCategoryMenuItem(TextWriter output, Category category, string id)
		{
			this.currentCategory = category;
			base.RenderMenuItem(output, null, ThemeFileId.CheckUnchecked, id, Utilities.HtmlEncode("cat:" + category.Name), false, null, null, null, new ContextMenu.RenderMenuItemHtml(this.RenderCategoryMenuItemHtml));
		}

		private void RenderCategoryMenuItemHtml(TextWriter output)
		{
			string text = this.currentCategory.Name;
			bool flag = false;
			if (35 < text.Length)
			{
				text = text.Substring(0, 35) + "...";
				flag = true;
			}
			if (flag)
			{
				output.Write("<span title=\"");
				Utilities.HtmlEncode(this.currentCategory.Name, output);
				output.Write("\">");
			}
			CategorySwatch.RenderSwatch(output, this.currentCategory);
			output.Write(" ");
			Utilities.HtmlEncode(text, output);
			if (flag)
			{
				output.Write("</span>");
			}
		}

		private const int MaximumCharactersToDisplayCategoryName = 35;

		protected const string CategoryPrefix = "cat:";

		private Category currentCategory;

		public enum CategoryState
		{
			UnChecked,
			PartialCheck,
			Checked
		}
	}
}
