using System;
using System.IO;
using System.Text;
using Microsoft.Exchange.Clients.Owa.Core;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.Clients.Owa.Premium.Controls
{
	internal sealed class CategoryDropDownList : DropDownList
	{
		private CategoryDropDownList(OwaStoreObjectId folderId) : base("divCDd", null, null)
		{
			UserContext userContext = UserContextManager.GetUserContext();
			MasterCategoryList masterCategoryList = userContext.GetMasterCategoryList(folderId);
			if (masterCategoryList != null)
			{
				this.categories = masterCategoryList.ToArray();
				Array.Sort<Category>(this.categories, new MostRecentlyUsedCategories.CategoryNameComparer());
				if (0 < this.categories.Length)
				{
					this.selectedCategory = this.categories[0];
					base.SelectedValue = this.selectedCategory.Name;
					return;
				}
			}
			else
			{
				this.categories = new Category[0];
			}
		}

		public static void RenderCategoryDropDownList(TextWriter writer, OwaStoreObjectId folderId)
		{
			CategoryDropDownList categoryDropDownList = new CategoryDropDownList(folderId);
			categoryDropDownList.Render(writer);
		}

		protected override void RenderSelectedValue(TextWriter writer)
		{
			if (this.selectedCategory != null)
			{
				writer.Write(CategoryDropDownList.GetCategoryHtml(this.selectedCategory));
			}
		}

		private static string GetCategoryHtml(Category category)
		{
			string text = category.Name;
			bool flag = false;
			StringBuilder stringBuilder = new StringBuilder();
			StringWriter stringWriter = new StringWriter(stringBuilder);
			if (35 < text.Length)
			{
				text = text.Substring(0, 35) + "...";
				flag = true;
			}
			stringWriter.Write("<span class=\"listItm\"");
			if (flag)
			{
				stringWriter.Write(" title=\"");
				Utilities.HtmlEncode(category.Name, stringWriter);
				stringWriter.Write("\" ");
			}
			stringWriter.Write(">");
			CategorySwatch.RenderSwatch(stringWriter, category);
			stringWriter.Write("&nbsp;");
			Utilities.HtmlEncode(text, stringWriter);
			stringWriter.Write("</span>");
			stringWriter.Close();
			return stringBuilder.ToString();
		}

		protected override DropDownListItem[] CreateListItems()
		{
			DropDownListItem[] array = new DropDownListItem[this.categories.Length];
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = new DropDownListItem(this.categories[i].Name, CategoryDropDownList.GetCategoryHtml(this.categories[i]), true);
			}
			return array;
		}

		private const int MaximumCharactersToDisplayCategoryName = 35;

		private Category[] categories;

		private Category selectedCategory;
	}
}
