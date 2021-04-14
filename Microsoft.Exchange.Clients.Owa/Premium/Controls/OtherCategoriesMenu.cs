using System;
using System.Globalization;
using System.IO;
using Microsoft.Exchange.Clients.Owa.Core;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.Clients.Owa.Premium.Controls
{
	public sealed class OtherCategoriesMenu : CategoryMenuBase
	{
		private OtherCategoriesMenu(UserContext userContext, string menuId) : base(menuId, userContext)
		{
			this.shouldScroll = true;
		}

		internal static OtherCategoriesMenu Create(UserContext userContext, Category[] categories, ContextMenu parentMenu)
		{
			return new OtherCategoriesMenu(userContext, parentMenu.Id + "O")
			{
				categories = categories
			};
		}

		protected override void RenderMenuItems(TextWriter output)
		{
			for (int i = 0; i < this.categories.Length; i++)
			{
				base.RenderCategoryMenuItem(output, this.categories[i], string.Format(CultureInfo.InvariantCulture, "divCatOth{0}", new object[]
				{
					i
				}));
			}
		}

		private Category[] categories;
	}
}
