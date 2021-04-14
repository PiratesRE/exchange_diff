using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.Exchange.Clients.Owa.Core;
using Microsoft.Exchange.Clients.Owa.Premium.Controls;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.Clients.Owa.Premium
{
	[OwaEventNamespace("Cat")]
	internal sealed class CategoryEventHandler : OwaEventHandlerBase
	{
		[OwaEvent("gtCM")]
		[OwaEventParameter("fId", typeof(OwaStoreObjectId), false, true)]
		[OwaEventVerb(OwaEventVerb.Get)]
		[OwaEventParameter("typ", typeof(StoreObjectType))]
		public void GetCategoryMenu()
		{
			this.ThrowIfWebPartsCannotActAsOwner();
			StoreObjectType storeObjectType = (StoreObjectType)base.GetParameter("typ");
			OwaStoreObjectId folderId = null;
			if (base.IsParameterSet("fId"))
			{
				folderId = (OwaStoreObjectId)base.GetParameter("fId");
			}
			StoreObjectType storeObjectType2 = storeObjectType;
			OutlookModule outlookModule;
			switch (storeObjectType2)
			{
			case StoreObjectType.ContactsFolder:
				break;
			case StoreObjectType.TasksFolder:
				goto IL_6C;
			default:
				switch (storeObjectType2)
				{
				case StoreObjectType.CalendarItem:
				case StoreObjectType.CalendarItemOccurrence:
					outlookModule = OutlookModule.Contacts;
					goto IL_76;
				case StoreObjectType.Contact:
				case StoreObjectType.DistributionList:
					break;
				case StoreObjectType.Task:
					goto IL_6C;
				default:
					outlookModule = OutlookModule.Mail;
					goto IL_76;
				}
				break;
			}
			outlookModule = OutlookModule.Contacts;
			goto IL_76;
			IL_6C:
			outlookModule = OutlookModule.Contacts;
			IL_76:
			CategoryContextMenu.Render(base.UserContext, this.Writer, outlookModule, folderId);
		}

		[OwaEventParameter("id", typeof(OwaStoreObjectId), false, true)]
		[OwaEventParameter("ck", typeof(string), false, true)]
		[OwaEventParameter("typ", typeof(StoreObjectType), false, true)]
		[OwaEvent("mdfy")]
		[OwaEventParameter("fId", typeof(OwaStoreObjectId), false, true)]
		[OwaEventParameter("catAdd", typeof(string), true, true)]
		[OwaEventParameter("catRem", typeof(string), true, true)]
		public void ModifyCategories()
		{
			this.ThrowIfWebPartsCannotActAsOwner();
			string[] addCategories = (string[])base.GetParameter("catAdd");
			string[] removeCategories = (string[])base.GetParameter("catRem");
			using (Item item = this.GetItem())
			{
				CategoryContextMenu.ModifyCategories(item, addCategories, removeCategories);
				MeetingMessage meetingMessage = item as MeetingMessage;
				if (meetingMessage != null)
				{
					CalendarItemBase calendarItemBase = MeetingUtilities.TryGetCorrelatedItem(meetingMessage);
					if (calendarItemBase != null)
					{
						CategoryContextMenu.ModifyCategories(calendarItemBase, addCategories, removeCategories);
						Utilities.SaveItem(calendarItemBase);
					}
				}
				Utilities.SaveItem(item, true, SaveMode.FailOnAnyConflict);
				item.Load();
				this.Writer.Write("var sCats = \"");
				StringBuilder stringBuilder = new StringBuilder();
				StringWriter stringWriter = new StringWriter(stringBuilder);
				CategorySwatch.RenderCategories(base.OwaContext, stringWriter, item);
				stringWriter.Close();
				Utilities.JavascriptEncode(stringBuilder.ToString(), this.Writer);
				this.Writer.Write("\";");
				this.Writer.Write("a_rgCats = ");
				CategorySwatch.RenderCategoriesJavascriptArray(this.SanitizingWriter, item);
				this.Writer.Write(";");
				this.Writer.Write("a_sId = \"");
				Utilities.JavascriptEncode(Utilities.GetIdAsString(item), this.Writer);
				this.Writer.Write("\";");
				this.Writer.Write("a_sCK = \"");
				Utilities.JavascriptEncode(item.Id.ChangeKeyAsBase64String(), this.Writer);
				this.Writer.Write("\";");
			}
		}

		[OwaEventParameter("ck", typeof(string))]
		[OwaEvent("clr")]
		[OwaEventParameter("id", typeof(OwaStoreObjectId))]
		public void ClearCategories()
		{
			base.ThrowIfCannotActAsOwner();
			using (Item item = this.GetItem())
			{
				item.OpenAsReadWrite();
				CategoryContextMenu.ClearCategories(item);
				item.Save(SaveMode.ResolveConflicts);
				item.Load();
				this.Writer.Write("a_sCK = \"");
				Utilities.JavascriptEncode(item.Id.ChangeKeyAsBase64String(), this.Writer);
				this.Writer.Write("\";");
			}
		}

		[OwaEvent("MngCtgs")]
		public void GetManageCategoriesDialog()
		{
			base.ThrowIfCannotActAsOwner();
			ManageCategoriesDialog manageCategoriesDialog = new ManageCategoriesDialog(base.UserContext);
			manageCategoriesDialog.Render(this.Writer);
		}

		[OwaEventParameter("clr", typeof(int))]
		[OwaEventParameter("nm", typeof(string))]
		[OwaEvent("CrtCtg")]
		public void CreateCategory()
		{
			base.ThrowIfCannotActAsOwner();
			string text = (string)base.GetParameter("nm");
			if (text.Length > 255)
			{
				throw new OwaInvalidRequestException("Category name cannot be longer than 255 characters.");
			}
			int color = (int)base.GetParameter("clr");
			if (!CategorySwatch.IsValidCategoryColor(color))
			{
				throw new OwaInvalidRequestException("Category color must be in the range [-1, 24].");
			}
			text = text.Trim();
			if (text.Length == 0)
			{
				RenderingUtilities.RenderError(base.UserContext, this.Writer, 1243373352);
				return;
			}
			if (text.Contains(",") || text.Contains(";") || text.Contains("؛") || text.Contains("﹔") || text.Contains("；"))
			{
				RenderingUtilities.RenderError(base.UserContext, this.Writer, 1243373352);
				return;
			}
			if (CategoryEventHandler.guidRegEx.IsMatch(text))
			{
				RenderingUtilities.RenderError(base.UserContext, this.Writer, 1243373352);
				return;
			}
			MasterCategoryList masterCategoryList = base.UserContext.GetMasterCategoryList(true);
			if (masterCategoryList.Contains(text))
			{
				RenderingUtilities.RenderError(base.UserContext, this.Writer, -210070156);
				return;
			}
			Category category = Category.Create(text, color, false);
			masterCategoryList.Add(category);
			ManageCategoriesDialog.RenderCategory(this.Writer, category);
			masterCategoryList.Save();
		}

		[OwaEventParameter("nm", typeof(string))]
		[OwaEventParameter("clr", typeof(int))]
		[OwaEvent("ChgCtg")]
		public void ChangeCategoryColor()
		{
			base.ThrowIfCannotActAsOwner();
			string text = (string)base.GetParameter("nm");
			if (string.IsNullOrEmpty(text))
			{
				throw new OwaInvalidRequestException("Category name cannot be null or empty.");
			}
			int color = (int)base.GetParameter("clr");
			if (!CategorySwatch.IsValidCategoryColor(color))
			{
				throw new OwaInvalidRequestException("Category color must be in the range [-1, 24].");
			}
			MasterCategoryList masterCategoryList = base.UserContext.GetMasterCategoryList(true);
			if (!masterCategoryList.Contains(text))
			{
				throw new OwaInvalidRequestException("Category does not exist");
			}
			masterCategoryList[text].Color = color;
			masterCategoryList.Save();
		}

		[OwaEvent("DltCtg")]
		[OwaEventParameter("nm", typeof(string))]
		public void DeleteCategory()
		{
			base.ThrowIfCannotActAsOwner();
			string categoryName = (string)base.GetParameter("nm");
			MasterCategoryList masterCategoryList = base.UserContext.GetMasterCategoryList(true);
			if (!masterCategoryList.Contains(categoryName))
			{
				return;
			}
			masterCategoryList.Remove(categoryName);
			masterCategoryList.Save();
		}

		private Item GetItem()
		{
			OwaStoreObjectId owaStoreObjectId = base.GetParameter("id") as OwaStoreObjectId;
			string text = base.GetParameter("ck") as string;
			Item result;
			if (owaStoreObjectId == null)
			{
				if (!base.IsParameterSet("typ"))
				{
					throw new OwaInvalidRequestException();
				}
				StoreObjectType itemType = (StoreObjectType)base.GetParameter("typ");
				result = Utilities.CreateImplicitDraftItem(itemType, base.GetParameter("fId") as OwaStoreObjectId);
			}
			else
			{
				if (text == null)
				{
					throw new OwaInvalidRequestException();
				}
				result = Utilities.GetItem<Item>(base.UserContext, owaStoreObjectId, text, new PropertyDefinition[0]);
			}
			return result;
		}

		private void ThrowIfWebPartsCannotActAsOwner()
		{
			if (base.UserContext.IsWebPartRequest && !base.UserContext.CanActAsOwner)
			{
				throw new OwaAccessDeniedException(LocalizedStrings.GetNonEncoded(1622692336), true);
			}
		}

		public const string EventNamespace = "Cat";

		public const string MethodGetCategoryMenu = "gtCM";

		public const string MethodClearCategories = "clr";

		public const string MethodModifyCategories = "mdfy";

		public const string MethodGetManageCategoriesDialog = "MngCtgs";

		public const string MethodCreateCategory = "CrtCtg";

		public const string MethodChangeCategoryColor = "ChgCtg";

		public const string MethodDeleteCategory = "DltCtg";

		public const string CategoryName = "nm";

		public const string CategoryColor = "clr";

		public const string Id = "id";

		public const string ChangeKey = "ck";

		public const string FolderId = "fId";

		public const string ItemType = "typ";

		public const string AddCategories = "catAdd";

		public const string RemoveCategories = "catRem";

		private const string HexDigit = "[0123456789ABCDEF]";

		private const string GuidExpression = "^\\{[0123456789ABCDEF]{8}-[0123456789ABCDEF]{4}-[0123456789ABCDEF]{4}-[0123456789ABCDEF]{4}-[0123456789ABCDEF]{12}\\}$";

		private static readonly Regex guidRegEx = new Regex("^\\{[0123456789ABCDEF]{8}-[0123456789ABCDEF]{4}-[0123456789ABCDEF]{4}-[0123456789ABCDEF]{4}-[0123456789ABCDEF]{12}\\}$", RegexOptions.Compiled);
	}
}
