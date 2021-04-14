using System;
using System.Globalization;
using System.IO;
using Microsoft.Exchange.Clients.Owa.Core;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics.Components.Clients;

namespace Microsoft.Exchange.Clients.Owa.Premium.Controls
{
	internal static class CategorySwatch
	{
		public static bool IsValidCategoryColor(int color)
		{
			return -1 <= color && color <= 24;
		}

		public static void RenderViewCategorySwatches(TextWriter writer, UserContext userContext, Item item, OwaStoreObjectId folderId)
		{
			if (writer == null)
			{
				throw new ArgumentNullException("writer");
			}
			if (userContext == null)
			{
				throw new ArgumentNullException("userContext");
			}
			if (item == null)
			{
				throw new ArgumentNullException("item");
			}
			string[] property = ItemUtility.GetProperty<string[]>(item, ItemSchema.Categories, null);
			FlagStatus property2 = ItemUtility.GetProperty<FlagStatus>(item, ItemSchema.FlagStatus, FlagStatus.NotFlagged);
			int property3 = ItemUtility.GetProperty<int>(item, ItemSchema.ItemColor, -1);
			bool property4 = ItemUtility.GetProperty<bool>(item, ItemSchema.IsToDoItem, false);
			CategorySwatch.RenderViewCategorySwatches(writer, userContext, property, property4, property2, property3, folderId);
		}

		public static void RenderViewCategorySwatches(TextWriter writer, UserContext userContext, string[] categories, bool isToDoItem, FlagStatus flagStatus, int itemColorInt, OwaStoreObjectId folderId)
		{
			CategorySwatch.RenderViewCategorySwatches(writer, userContext, categories, isToDoItem, flagStatus, itemColorInt, folderId, true);
		}

		public static void RenderViewCategorySwatches(TextWriter writer, UserContext userContext, string[] categories, bool isToDoItem, FlagStatus flagStatus, int itemColorInt, OwaStoreObjectId folderId, bool renderEmpty)
		{
			if (writer == null)
			{
				throw new ArgumentNullException("writer");
			}
			if (userContext == null)
			{
				throw new ArgumentNullException("userContext");
			}
			int num = 3;
			int num2 = 0;
			if (categories != null)
			{
				MasterCategoryList masterCategoryList = userContext.GetMasterCategoryList(folderId);
				if (masterCategoryList != null)
				{
					int num3 = 0;
					while (num3 < categories.Length && num2 < num)
					{
						Category category = masterCategoryList[categories[num3]];
						if (category != null && category.Color != -1)
						{
							CategorySwatch.RenderSwatch(writer, category);
							num2++;
						}
						num3++;
					}
				}
			}
			if (num2 == 0 && !isToDoItem && flagStatus == FlagStatus.Flagged && itemColorInt != -1)
			{
				CategorySwatch.RenderSwatch(writer, (ItemColor)itemColorInt);
				num2++;
			}
			if (renderEmpty && num2 == 0)
			{
				CategorySwatch.RenderSwatch(writer, null);
			}
		}

		public static string GetCategoryClassName(Category category)
		{
			return CategorySwatch.GetCategoryClassNameFromColor((category != null) ? category.Color : -1);
		}

		public static string GetCategoryClassNameFromColor(int color)
		{
			string result = "noClr";
			if (color != -1)
			{
				result = "cat" + color.ToString(CultureInfo.InvariantCulture);
			}
			return result;
		}

		public static void RenderSwatch(TextWriter writer, Category category)
		{
			CategorySwatch.RenderSwatch(writer, CategorySwatch.GetCategoryClassName(category));
		}

		public static void RenderSwatch(TextWriter writer, ItemColor itemColor)
		{
			string str = "catFlg";
			int num = (int)itemColor;
			CategorySwatch.RenderSwatch(writer, str + num.ToString(CultureInfo.InvariantCulture));
		}

		private static void RenderSwatch(TextWriter writer, string className)
		{
			writer.Write("<img class=\"catB ");
			writer.Write(className);
			writer.Write("\"/>");
		}

		public static void RenderCategories(OwaContext owaContext, TextWriter writer, Item item)
		{
			CategorySwatch.RenderCategories(owaContext, writer, item, item.Session);
		}

		public static void RenderCategories(OwaContext owaContext, TextWriter writer, IStorePropertyBag storePropertyBag, StoreSession storeSession)
		{
			if (owaContext == null)
			{
				throw new ArgumentNullException("owaContext");
			}
			if (writer == null)
			{
				throw new ArgumentNullException("writer");
			}
			if (storePropertyBag == null)
			{
				throw new ArgumentNullException("storePropertyBag");
			}
			string[] property = ItemUtility.GetProperty<string[]>(storePropertyBag, ItemSchema.Categories, null);
			string value = owaContext.UserContext.IsRtl ? "rtl" : "ltr";
			int num = 0;
			if (property != null && 0 < property.Length)
			{
				MasterCategoryList masterCategoryList = null;
				try
				{
					if (storeSession != null && owaContext.UserContext.IsOtherMailbox(storeSession))
					{
						masterCategoryList = owaContext.UserContext.GetMasterCategoryList(storeSession as MailboxSession);
					}
					else
					{
						masterCategoryList = owaContext.UserContext.GetMasterCategoryList();
					}
				}
				catch (QuotaExceededException ex)
				{
					ExTraceGlobals.CoreTracer.TraceDebug<string>(0L, "CategorySwatch.RenderCategories: Failed. Exception: {0}", ex.Message);
					return;
				}
				if (masterCategoryList != null)
				{
					for (int i = 0; i < property.Length; i++)
					{
						writer.Write("<span class=\"spanCatContainer\" dir=\"");
						writer.Write(value);
						writer.Write("\">");
						CategorySwatch.RenderSwatch(writer, masterCategoryList[property[i]]);
						writer.Write("&nbsp;");
						Utilities.SanitizeHtmlEncode(property[i], writer);
						if (i < property.Length - 1)
						{
							writer.Write("; ");
						}
						writer.Write("</span><wbr>");
						num++;
					}
				}
			}
			if (num == 0)
			{
				writer.Write("<span class=\"spanCatContainer catAfter\" dir=\"");
				writer.Write(value);
				writer.Write("\">");
				int legacyColoredFlag = ItemUtility.GetLegacyColoredFlag(storePropertyBag);
				if (0 < legacyColoredFlag && legacyColoredFlag < CategorySwatch.FlagCategory.Length)
				{
					CategorySwatch.RenderSwatch(writer, (ItemColor)legacyColoredFlag);
					writer.Write("<span id=\"vaM\">");
					writer.Write(SanitizedHtmlString.FromStringId(CategorySwatch.FlagCategory[legacyColoredFlag]));
					writer.Write("</span>");
				}
				writer.Write("</span>");
			}
		}

		public static void RenderCategoriesJavascriptArray(TextWriter writer, Item item)
		{
			writer.Write("new Array(");
			if (item != null)
			{
				bool flag = true;
				foreach (string s in item.Categories)
				{
					if (!flag)
					{
						writer.Write(",");
					}
					writer.Write("\"");
					Utilities.JavascriptEncode(s, writer);
					writer.Write("\"");
					flag = false;
				}
			}
			writer.Write(")");
		}

		public const int NoColor = -1;

		private static readonly Strings.IDs[] FlagCategory = new Strings.IDs[]
		{
			-1018465893,
			866727429,
			-1901339565,
			937307334,
			1294072875,
			596437217,
			1989367210
		};
	}
}
