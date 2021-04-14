using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Exchange.Clients.Owa.Core;

namespace Microsoft.Exchange.Clients.Owa.Premium
{
	internal class NavigationBar
	{
		public NavigationBar(UserContext userContext)
		{
			this.userContext = userContext;
			this.itemList.Add(new StandardNavigationBarItem(NavigationModule.Mail, userContext, LocalizedStrings.GetNonEncoded(405905481), "Inbx", "navMail();", ThemeFileId.EMail2, ThemeFileId.EMail2Small));
			if (userContext.IsFeatureEnabled(Feature.Calendar))
			{
				this.itemList.Add(new StandardNavigationBarItem(NavigationModule.Calendar, userContext, LocalizedStrings.GetNonEncoded(1292798904), "Cal", "navCal();", ThemeFileId.Calendar2, ThemeFileId.Calendar2Small));
			}
			if (userContext.IsFeatureEnabled(Feature.Contacts))
			{
				this.itemList.Add(new StandardNavigationBarItem(NavigationModule.Contacts, userContext, LocalizedStrings.GetNonEncoded(1716044995), "Ctcts", "navCtcts();", ThemeFileId.Contact2, ThemeFileId.Contact2Small));
			}
			if (userContext.IsFeatureEnabled(Feature.Tasks))
			{
				this.itemList.Add(new StandardNavigationBarItem(NavigationModule.Tasks, userContext, LocalizedStrings.GetNonEncoded(-1328808356), "Tsks", "navTsks();", ThemeFileId.Task2, ThemeFileId.Task));
			}
			if (DocumentLibraryUtilities.IsDocumentsAccessEnabled(userContext))
			{
				this.itemList.Add(new StandardNavigationBarItem(NavigationModule.Documents, userContext, LocalizedStrings.GetNonEncoded(-406393320), "Docs", "navDocs();", ThemeFileId.Documents, ThemeFileId.DocumentsSmall));
			}
			if (userContext.IsPublicFolderEnabled)
			{
				this.itemList.Add(new StandardNavigationBarItem(NavigationModule.PublicFolders, userContext, LocalizedStrings.GetNonEncoded(-1116491328), "PFs", "navPFs();", ThemeFileId.PublicFolder, ThemeFileId.PublicFolderSmall));
			}
			using (List<UIExtensionManager.NavigationExtensionItem>.Enumerator navigationBarEnumerator = UIExtensionManager.GetNavigationBarEnumerator())
			{
				while (navigationBarEnumerator.MoveNext())
				{
					UIExtensionManager.NavigationExtensionItem navigationExtensionItem = navigationBarEnumerator.Current;
					this.itemList.Add(new CustomNavigationBarItem(userContext, navigationExtensionItem.GetTextByLanguage(userContext.UserCulture.Name), navigationExtensionItem.TargetUrl, navigationExtensionItem.LargeIcon, navigationExtensionItem.SmallIcon));
				}
			}
		}

		public void Render(TextWriter writer, NavigationModule currentModule, NavigationBarType type)
		{
			int num = Math.Min(6, this.itemList.Count);
			int num2 = (this.itemList.Count > 6) ? 6 : this.itemList.Count;
			int[] array = new int[num2];
			int num3 = 0;
			for (int i = 0; i < num2; i++)
			{
				array[i] = 99 / num2;
				num3 += array[i];
			}
			int num4 = 0;
			while (num3 < 99 && num4 < num2)
			{
				array[num4]++;
				num3++;
				num4++;
			}
			if (type == NavigationBarType.WunderBar)
			{
				writer.Write("<div class=\"nbWunderBar\">");
			}
			for (int j = 0; j < num; j++)
			{
				this.itemList[j].Render(writer, currentModule, (type == NavigationBarType.WunderBar) ? array[j] : 0, type == NavigationBarType.WunderBar && this.itemList.Count > 4);
			}
			if (type == NavigationBarType.WunderBar)
			{
				writer.Write("</div>");
			}
			if (this.itemList.Count > 6)
			{
				writer.Write("<div class=\"nbWunderBar\">");
				for (int k = 6; k < 12; k++)
				{
					if (k < this.itemList.Count)
					{
						this.itemList[k].Render(writer, currentModule, array[k % 6], true);
					}
					else
					{
						writer.Write("<div class=\"nbMnuItm nbMnuItmWF\" style=\"width:");
						writer.Write(array[k % 6]);
						writer.Write("%\">");
						this.userContext.RenderThemeImage(writer, ThemeFileId.Clear1x1, "nbMnuImgWS", new object[0]);
						writer.Write("</div>");
					}
				}
				writer.Write("</div>");
			}
		}

		public int GetNavigationBarHeight(NavigationBarType type)
		{
			if (type == NavigationBarType.WunderBar)
			{
				return 5 + 31 * ((this.itemList.Count > 6) ? 2 : 1);
			}
			if (type == NavigationBarType.ExpandBar)
			{
				return 5 + 31 * ((this.itemList.Count > 6) ? 7 : this.itemList.Count);
			}
			throw new ArgumentException("Wrong navigation bar type");
		}

		private const int MaxItemCountOneLine = 6;

		private const int MaxLargeIconCount = 4;

		private const int ToggleBarHeight = 5;

		private const int NavigationBarItemHeight = 31;

		private readonly UserContext userContext;

		private List<NavigationBarItemBase> itemList = new List<NavigationBarItemBase>();
	}
}
