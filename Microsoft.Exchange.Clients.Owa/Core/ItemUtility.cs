using System;
using System.IO;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.Clients.Owa.Core
{
	internal sealed class ItemUtility
	{
		private ItemUtility()
		{
		}

		public static T GetProperty<T>(IStorePropertyBag propertyBag, PropertyDefinition propertyDefinition, T defaultValue)
		{
			object obj = propertyBag.TryGetProperty(propertyDefinition);
			if (obj is PropertyError || obj == null)
			{
				return defaultValue;
			}
			return (T)((object)obj);
		}

		public static bool HasCategories(IStorePropertyBag storePropertyBag)
		{
			string[] array = null;
			if (OwaContext.Current.UserContext.CanActAsOwner)
			{
				array = ItemUtility.GetProperty<string[]>(storePropertyBag, ItemSchema.Categories, null);
			}
			return (array != null && 0 < array.Length) || ItemUtility.GetLegacyColoredFlag(storePropertyBag) != int.MinValue;
		}

		public static int GetLegacyColoredFlag(IStorePropertyBag storePropertyBag)
		{
			FlagStatus property = ItemUtility.GetProperty<FlagStatus>(storePropertyBag, ItemSchema.FlagStatus, FlagStatus.NotFlagged);
			int property2 = ItemUtility.GetProperty<int>(storePropertyBag, ItemSchema.ItemColor, int.MinValue);
			bool property3 = ItemUtility.GetProperty<bool>(storePropertyBag, ItemSchema.IsToDoItem, false);
			if (property == FlagStatus.Flagged && property2 != -2147483648 && !property3)
			{
				return property2;
			}
			return int.MinValue;
		}

		public static bool ShouldRenderSendAgain(Item item, bool isEmbeddedItem)
		{
			ReportMessage reportMessage = item as ReportMessage;
			return !isEmbeddedItem && reportMessage != null && reportMessage.IsSendAgainAllowed;
		}

		public static string GetCategoriesAsString(Item item)
		{
			string result = null;
			if (item == null)
			{
				throw new ArgumentNullException("item");
			}
			string[] property = ItemUtility.GetProperty<string[]>(item, ItemSchema.Categories, null);
			if (property != null)
			{
				result = string.Join("; ", property);
			}
			return result;
		}

		public static bool UserCanEditItem(Item item)
		{
			if (item == null)
			{
				throw new ArgumentNullException("item");
			}
			return ItemUtility.HasRight(item, EffectiveRights.Modify);
		}

		public static bool UserCanDeleteItem(Item item)
		{
			if (item == null)
			{
				throw new ArgumentNullException("item");
			}
			return ItemUtility.HasRight(item, EffectiveRights.Delete);
		}

		private static bool HasRight(Item item, EffectiveRights effectiveRightToCheck)
		{
			EffectiveRights property = ItemUtility.GetProperty<EffectiveRights>(item, StoreObjectSchema.EffectiveRights, EffectiveRights.None);
			return (property & effectiveRightToCheck) == effectiveRightToCheck;
		}

		internal static bool IsReplySupported(Item item)
		{
			CalendarItemBase calendarItemBase = item as CalendarItemBase;
			bool flag = calendarItemBase != null && calendarItemBase.IsMeeting;
			return item is MessageItem || flag || item is PostItem;
		}

		internal static bool IsForwardSupported(Item item)
		{
			bool flag = ObjectClass.IsOfClass(item.ClassName, "IPM.Note.Microsoft.Approval.Request");
			bool flag2 = ObjectClass.IsOfClass(item.ClassName, "IPM.Sharing");
			bool flag3 = false;
			CalendarItemBase calendarItemBase = item as CalendarItemBase;
			if (calendarItemBase != null && !calendarItemBase.IsMeeting)
			{
				if (calendarItemBase.IsCalendarItemTypeOccurrenceOrException)
				{
					flag3 = true;
				}
				else
				{
					CalendarItem calendarItem = calendarItemBase as CalendarItem;
					if (calendarItem != null)
					{
						flag3 = (calendarItem.Recurrence != null);
					}
				}
			}
			return !flag && !flag2 && !flag3;
		}

		internal static void SetItemBody(Item item, BodyFormat bodyFormat, string content)
		{
			if (item == null)
			{
				throw new ArgumentNullException("item");
			}
			if (content == null)
			{
				throw new ArgumentNullException("content");
			}
			Body body = item.Body;
			if (!OwaContext.Current.UserContext.IsBasicExperience && OwaContext.Current.UserContext.IsIrmEnabled && Utilities.IsIrmDecrypted(item))
			{
				body = ((RightsManagedMessageItem)item).ProtectedBody;
			}
			using (TextWriter textWriter = body.OpenTextWriter(bodyFormat))
			{
				textWriter.Write(content);
			}
		}

		internal static string GetItemBody(Item item, BodyFormat desiredFormat)
		{
			if (item == null)
			{
				throw new ArgumentNullException("item");
			}
			string result;
			using (TextReader textReader = item.Body.OpenTextReader(desiredFormat))
			{
				result = textReader.ReadToEnd();
			}
			return result;
		}

		public static bool HasDeletePolicy(IStorePropertyBag storePropertyBag)
		{
			byte[] property = ItemUtility.GetProperty<byte[]>(storePropertyBag, StoreObjectSchema.PolicyTag, null);
			return property != null;
		}
	}
}
