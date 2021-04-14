using System;
using System.Globalization;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Clients.Owa.Core.Controls
{
	internal class InfobarMessageBuilderBase
	{
		protected static string GetImportance(IStorePropertyBag storePropertyBag)
		{
			if (storePropertyBag == null)
			{
				throw new ArgumentNullException("storePropertyBag");
			}
			switch (ItemUtility.GetProperty<Importance>(storePropertyBag, ItemSchema.Importance, Importance.Normal))
			{
			case Importance.Low:
				return LocalizedStrings.GetNonEncoded(-1193056027);
			case Importance.High:
				return LocalizedStrings.GetNonEncoded(-788473393);
			}
			return null;
		}

		protected static string GetSensitivity(IStorePropertyBag storePropertyBag)
		{
			if (storePropertyBag == null)
			{
				throw new ArgumentNullException("storePropertyBag");
			}
			switch (ItemUtility.GetProperty<Sensitivity>(storePropertyBag, ItemSchema.Sensitivity, Sensitivity.Normal))
			{
			case Sensitivity.Personal:
				return LocalizedStrings.GetNonEncoded(-1220985107);
			case Sensitivity.Private:
				return LocalizedStrings.GetNonEncoded(-171299332);
			case Sensitivity.CompanyConfidential:
				return LocalizedStrings.GetNonEncoded(-216368585);
			default:
				return null;
			}
		}

		protected static string GetFlag(IStorePropertyBag storePropertyBag, UserContext userContext)
		{
			if (storePropertyBag == null)
			{
				throw new ArgumentNullException("storePropertyBag");
			}
			if (userContext == null)
			{
				throw new ArgumentNullException("userContext");
			}
			FlagStatus property = ItemUtility.GetProperty<FlagStatus>(storePropertyBag, ItemSchema.FlagStatus, FlagStatus.NotFlagged);
			if (property == FlagStatus.NotFlagged)
			{
				return null;
			}
			string text = ItemUtility.GetProperty<string>(storePropertyBag, ItemSchema.FlagRequest, null);
			if (text == null)
			{
				return null;
			}
			bool flag = false;
			for (int i = 0; i < InfobarMessageBuilderBase.defaultFlagMessages.Length; i++)
			{
				if (string.Equals(LocalizedStrings.GetNonEncoded(InfobarMessageBuilderBase.defaultFlagMessages[i]), text, StringComparison.OrdinalIgnoreCase))
				{
					flag = true;
					break;
				}
			}
			if (!flag)
			{
				text = LocalizedStrings.GetNonEncoded(-1950847676);
			}
			string result;
			if (property == FlagStatus.Complete)
			{
				ExDateTime property2 = ItemUtility.GetProperty<ExDateTime>(storePropertyBag, ItemSchema.CompleteDate, ExDateTime.MinValue);
				if (property2 != ExDateTime.MinValue)
				{
					property2 = ItemUtility.GetProperty<ExDateTime>(storePropertyBag, ItemSchema.FlagCompleteTime, ExDateTime.MinValue);
				}
				if (property2 != ExDateTime.MinValue)
				{
					result = string.Format(LocalizedStrings.GetNonEncoded(910655284), text, property2.ToString(DateTimeFormatInfo.CurrentInfo.LongDatePattern));
				}
				else
				{
					result = text;
				}
			}
			else
			{
				string text2 = null;
				string text3 = null;
				string text4 = null;
				string text5 = null;
				ExDateTime property3 = ItemUtility.GetProperty<ExDateTime>(storePropertyBag, ItemSchema.UtcStartDate, ExDateTime.MinValue);
				if (property3.Year > 1601)
				{
					text2 = property3.ToString(DateTimeFormatInfo.CurrentInfo.LongDatePattern);
				}
				ExDateTime property4 = ItemUtility.GetProperty<ExDateTime>(storePropertyBag, ItemSchema.UtcDueDate, ExDateTime.MinValue);
				if (property4.Year > 1601)
				{
					text3 = property4.ToString(DateTimeFormatInfo.CurrentInfo.LongDatePattern);
				}
				ExDateTime property5 = ItemUtility.GetProperty<ExDateTime>(storePropertyBag, ItemSchema.ReminderDueBy, ExDateTime.MinValue);
				bool property6 = ItemUtility.GetProperty<bool>(storePropertyBag, ItemSchema.ReminderIsSet, false);
				if (property6 && property5.Year > 1601)
				{
					text4 = property5.ToString(DateTimeFormatInfo.CurrentInfo.LongDatePattern);
					text5 = property5.ToString(userContext.UserOptions.TimeFormat);
				}
				if (text2 != null && text3 != null)
				{
					if (text4 != null && text5 != null)
					{
						result = string.Format(LocalizedStrings.GetNonEncoded(-1537077628), new object[]
						{
							text,
							text2,
							text3,
							text4,
							text5
						});
					}
					else
					{
						result = string.Format(LocalizedStrings.GetNonEncoded(1424074078), text, text2, text3);
					}
				}
				else if (text3 != null)
				{
					if (text4 != null && text5 != null)
					{
						result = string.Format(LocalizedStrings.GetNonEncoded(1614879588), new object[]
						{
							text,
							text3,
							text4,
							text5
						});
					}
					else
					{
						result = string.Format(LocalizedStrings.GetNonEncoded(-66921782), text, text3);
					}
				}
				else if (text4 != null && text5 != null)
				{
					result = string.Format(LocalizedStrings.GetNonEncoded(1186773594), text, text4, text5);
				}
				else
				{
					ExDateTime property7 = ItemUtility.GetProperty<ExDateTime>(storePropertyBag, MessageItemSchema.ReplyTime, ExDateTime.MinValue);
					if (property7.Year > 1601)
					{
						result = string.Format(LocalizedStrings.GetNonEncoded(1979391403), text, property7.ToString(DateTimeFormatInfo.CurrentInfo.LongDatePattern), property7.ToString(userContext.UserOptions.TimeFormat));
					}
					else
					{
						result = text;
					}
				}
			}
			return result;
		}

		protected static string GetCompliance(UserContext userContext, IStorePropertyBag storePropertyBag, bool isComposeMessage)
		{
			if (userContext == null)
			{
				throw new ArgumentNullException("userContext");
			}
			if (storePropertyBag == null)
			{
				throw new ArgumentNullException("storePropertyBag");
			}
			bool property = ItemUtility.GetProperty<bool>(storePropertyBag, ItemSchema.IsClassified, false);
			if (!property)
			{
				return null;
			}
			string property2 = ItemUtility.GetProperty<string>(storePropertyBag, ItemSchema.ClassificationGuid, string.Empty);
			Guid empty = Guid.Empty;
			if (!GuidHelper.TryParseGuid(property2, out empty) || empty == Guid.Empty)
			{
				return null;
			}
			string text = userContext.ComplianceReader.MessageClassificationReader.GetDescription(empty, userContext.UserCulture, isComposeMessage);
			if (string.IsNullOrEmpty(text))
			{
				return null;
			}
			if (!isComposeMessage)
			{
				string property3 = ItemUtility.GetProperty<string>(storePropertyBag, ItemSchema.ClassificationDescription, string.Empty);
				if (string.IsNullOrEmpty(property3) || string.IsNullOrEmpty(property3.Trim()))
				{
					return null;
				}
				string property4 = ItemUtility.GetProperty<string>(storePropertyBag, ItemSchema.Classification, string.Empty);
				text = property4 + " - " + property3;
			}
			return text;
		}

		protected static string GetCompliance(UserContext userContext, Guid complianceId)
		{
			if (userContext == null)
			{
				throw new ArgumentNullException("userContext");
			}
			return userContext.ComplianceReader.GetDescription(complianceId, userContext.UserCulture);
		}

		internal static bool ShouldRenderReadReceiptNoticeInfobar(UserContext userContext, IStorePropertyBag storePropertyBag)
		{
			if (userContext == null)
			{
				throw new ArgumentNullException("userContext");
			}
			if (storePropertyBag == null)
			{
				throw new ArgumentNullException("storePropertyBag");
			}
			return userContext.UserOptions.ReadReceipt == ReadReceiptResponse.DoNotAutomaticallySend && ItemUtility.GetProperty<bool>(storePropertyBag, MessageItemSchema.IsReadReceiptPending, false) && !JunkEmailUtilities.IsInJunkEmailFolder(storePropertyBag, false, userContext);
		}

		private static readonly Strings.IDs[] defaultFlagMessages = new Strings.IDs[]
		{
			-1950847676,
			681902140,
			-960062607,
			1091649098,
			-419354937,
			-154654355,
			-595208036,
			-2121202340,
			1083853808,
			-1736239124
		};
	}
}
