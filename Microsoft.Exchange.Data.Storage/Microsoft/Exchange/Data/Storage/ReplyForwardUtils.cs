using System;
using System.Globalization;
using Microsoft.Exchange.Data.Globalization;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Data.Storage;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal static class ReplyForwardUtils
	{
		internal static void UpdateOriginalItemProperties(Item sentItem)
		{
			object[] array = ReplyForwardUtils.DecodeReplyForwardStatus(sentItem.TryGetProperty(InternalSchema.ReplyForwardStatus) as string);
			if (array == null)
			{
				return;
			}
			if ((int)array[0] < 0)
			{
				return;
			}
			PropertyDefinition[] array2 = new PropertyDefinition[]
			{
				InternalSchema.LastVerbExecuted,
				InternalSchema.LastVerbExecutionTime,
				InternalSchema.IconIndex,
				InternalSchema.MessageAnswered
			};
			object[] array3 = new object[array2.Length];
			VersionedId storeId = VersionedId.Deserialize((string)array[2]);
			try
			{
				using (Item item = Item.Bind(sentItem.Session, storeId))
				{
					if (!(item is CalendarItemBase))
					{
						array3[0] = (int)array[0];
						array3[1] = ExDateTime.GetNow(sentItem.Session.ExTimeZone);
						if (item is MeetingMessage)
						{
							array3[2] = item.GetValueOrDefault<int>(InternalSchema.IconIndex, -1);
						}
						else
						{
							array3[2] = (int)array[1];
						}
						array3[3] = true;
						item.SetProperties(array2, array3);
						ConflictResolutionResult conflictResolutionResult = item.Save(SaveMode.ResolveConflicts);
						if (conflictResolutionResult.SaveStatus != SaveResult.Success)
						{
							ExTraceGlobals.StorageTracer.TraceDebug<string>(0L, "ReplyForwardCommon::UpdateOriginalItemProperties. {0}", "The original item has been changed thus update on the original item may not be done accurately.");
						}
					}
				}
			}
			catch (StoragePermanentException arg)
			{
				ExTraceGlobals.StorageTracer.TraceDebug<StoragePermanentException>(0L, "ReplyForwardCommon::UpdateOriginalItemProperties. The parent item cannot be found. It might have been deleted. Exception = {0}.", arg);
			}
			catch (StorageTransientException arg2)
			{
				ExTraceGlobals.StorageTracer.TraceDebug<StorageTransientException>(0L, "ReplyForwardCommon::UpdateOriginalItemProperties. The parent item cannot be found. It might have been deleted. Exception = {0}.", arg2);
			}
		}

		internal static VersionedId GetAssociatedId(MessageItem message)
		{
			string valueOrDefault = message.GetValueOrDefault<string>(InternalSchema.ReplyForwardStatus, string.Empty);
			object[] array = ReplyForwardUtils.DecodeReplyForwardStatus(valueOrDefault);
			VersionedId result = null;
			if (array != null && array.Length == 3 && array[2] is string)
			{
				result = VersionedId.Deserialize(Convert.FromBase64String((string)array[2]));
			}
			return result;
		}

		internal static void SetAssociatedId(MessageItem message, VersionedId associatedId)
		{
			message.SafeSetProperty(InternalSchema.ReplyForwardStatus, ReplyForwardUtils.EncodeReplyForwardStatus((LastAction)(-1), IconIndex.Default, associatedId));
		}

		internal static object[] DecodeReplyForwardStatus(string value)
		{
			if (value == null)
			{
				return null;
			}
			string[] array = value.Split(new char[]
			{
				','
			});
			if (array.Length != 3)
			{
				ExTraceGlobals.StorageTracer.TraceDebug<int, string>(0L, "ReplyForwardCommon::DecodeMessageStatus. The message status data has been corrupted. Status count = {0}. Value = {1}.", array.Length, value);
				return null;
			}
			int num = 0;
			int num2 = 0;
			string text = null;
			try
			{
				num = int.Parse(array[0]);
				num2 = int.Parse(array[1]);
				text = array[2];
			}
			catch (FormatException arg)
			{
				ExTraceGlobals.StorageTracer.TraceDebug<FormatException, string>(0L, "ReplyForwardCommon::DecodeMessageStatus. The message status data has been corrupted. Reasons = {0}. Value = {1}.", arg, value);
				return null;
			}
			return new object[]
			{
				num,
				num2,
				text
			};
		}

		internal static string EncodeReplyForwardStatus(LastAction lastAction, IconIndex iconIndex, VersionedId id)
		{
			if (id == null)
			{
				return string.Empty;
			}
			return ReplyForwardUtils.EncodeReplyForwardStatus(lastAction, iconIndex, id.ToBase64String());
		}

		internal static CultureInfo CalculateReplyForwardCulture(CultureInfo culture, Item newItem)
		{
			CultureInfo cultureInfo = null;
			if (culture != null)
			{
				cultureInfo = culture;
			}
			else if (newItem.Session != null)
			{
				cultureInfo = newItem.Session.InternalCulture;
			}
			if (cultureInfo != null && LocaleMap.GetLcidFromCulture(cultureInfo) == 4100)
			{
				return ReplyForwardUtils.GetCultureEnglishUS();
			}
			return cultureInfo;
		}

		private static string EncodeReplyForwardStatus(LastAction lastAction, IconIndex iconIndex, string parentMessageItemIdBase64String)
		{
			EnumValidator.AssertValid<IconIndex>(iconIndex);
			return string.Format("{0}{1}{2}{3}{4}", new object[]
			{
				(int)lastAction,
				',',
				(int)iconIndex,
				',',
				parentMessageItemIdBase64String
			});
		}

		private static CultureInfo GetCultureEnglishUS()
		{
			if (ReplyForwardUtils.cultureEnglishUS == null)
			{
				ReplyForwardUtils.cultureEnglishUS = LocaleMap.GetCultureFromLcid(1033);
			}
			return ReplyForwardUtils.cultureEnglishUS;
		}

		internal const char Comma = ',';

		private const int LcidSingapore = 4100;

		private const int LcidEnglishUS = 1033;

		private static CultureInfo cultureEnglishUS;
	}
}
