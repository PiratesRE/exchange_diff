using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Microsoft.Exchange.Core;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Extensibility.Internal
{
	internal class DsnParamText
	{
		private static bool IsMessageSizeTextInKB(long maxSize, long currentSize, CultureInfo culture, out string maxSizeText, out string currentSizeText)
		{
			bool result = true;
			if (maxSize >= 1024L && currentSize >= 1024L)
			{
				maxSize >>= 10;
				currentSize >>= 10;
				result = false;
			}
			if (currentSize == maxSize)
			{
				currentSize += 1L;
			}
			maxSizeText = (culture.IsNeutralCulture ? maxSize.ToString() : maxSize.ToString(culture));
			currentSizeText = (culture.IsNeutralCulture ? currentSize.ToString() : currentSize.ToString(culture));
			return result;
		}

		private DsnParamText(DsnParamItem[] dsnParamItems)
		{
			this.dsnParamItems = dsnParamItems;
		}

		public string[] GenerateTexts(DsnParameters dsnParameters, CultureInfo culture, out bool overwriteDefault)
		{
			List<string> list = null;
			bool flag = false;
			if (dsnParameters != null)
			{
				foreach (DsnParamItem dsnParamItem in this.dsnParamItems)
				{
					string @string = dsnParamItem.GetString(dsnParameters, culture, out overwriteDefault);
					if (overwriteDefault)
					{
						flag = true;
					}
					if (!string.IsNullOrEmpty(@string))
					{
						if (list == null)
						{
							list = new List<string>();
						}
						list.Add(@string);
					}
				}
			}
			overwriteDefault = flag;
			if (list != null)
			{
				return list.ToArray();
			}
			return null;
		}

		// Note: this type is marked as 'beforefieldinit'.
		static DsnParamText()
		{
			DsnParamItem[] array = new DsnParamItem[2];
			array[0] = new DsnParamItem(new string[]
			{
				"MaxRecipMessageSizeInKB",
				"CurrentMessageSizeInKB"
			}, delegate(DsnParameters dsnParameters, CultureInfo culture, out bool overwriteDefault)
			{
				long maxSize = (long)dsnParameters["MaxRecipMessageSizeInKB"];
				long currentSize = (long)dsnParameters["CurrentMessageSizeInKB"];
				overwriteDefault = false;
				string maxSize2;
				string currentSize2;
				if (!DsnParamText.IsMessageSizeTextInKB(maxSize, currentSize, culture, out maxSize2, out currentSize2))
				{
					return SystemMessages.DsnParamTextMessageSizePerRecipientInMB(currentSize2, maxSize2).ToString(culture);
				}
				return SystemMessages.DsnParamTextMessageSizePerRecipientInKB(currentSize2, maxSize2).ToString(culture);
			});
			array[1] = new DsnParamItem(new string[]
			{
				"MapiMessageClass",
				"TextMessagingDeliveryPointType",
				"TextMessagingBodyText",
				"TextMessagingRecipientPhoneNumber",
				"TextMessagingRecipientCarrier",
				"TextMessagingRecipientExceptions"
			}, delegate(DsnParameters dsnParameters, CultureInfo culture, out bool overwriteDefault)
			{
				string text = (string)dsnParameters["MapiMessageClass"];
				string text2 = (string)dsnParameters["TextMessagingDeliveryPointType"];
				string text3 = (string)dsnParameters["TextMessagingBodyText"];
				string number = (string)dsnParameters["TextMessagingRecipientPhoneNumber"];
				string carrier = (string)dsnParameters["TextMessagingRecipientCarrier"];
				IList<Exception> list = (IList<Exception>)dsnParameters["TextMessagingRecipientExceptions"];
				overwriteDefault = true;
				StringBuilder stringBuilder = new StringBuilder();
				int num = 0;
				while (list.Count > num)
				{
					Exception ex = list[num];
					if (list.Count - 1 == num)
					{
						if (ex is LocalizedException)
						{
							stringBuilder.Append(((LocalizedException)ex).LocalizedString.ToString(culture));
						}
						else
						{
							stringBuilder.AppendLine(ex.Message);
						}
					}
					else if (ex is LocalizedException)
					{
						stringBuilder.Append("<BR><BR>" + ((LocalizedException)ex).LocalizedString.ToString(culture));
					}
					else
					{
						stringBuilder.AppendLine("<BR><BR>" + ex.Message);
					}
					num++;
				}
				string text4 = stringBuilder.ToString();
				if (text.StartsWith("IPM.Note.Mobile.SMS.Alert", StringComparison.OrdinalIgnoreCase))
				{
					if (string.IsNullOrEmpty(text4))
					{
						return SystemMessages.HumanTextFailedSmtpToSmsGatewayNotification(number, carrier, text3).ToString(culture);
					}
					if (string.IsNullOrEmpty(text3) && text2.Equals("ExchangeActiveSync", StringComparison.OrdinalIgnoreCase))
					{
						return text4;
					}
					return SystemMessages.HumanTextFailedOmsNotification(text3, text4).ToString(culture);
				}
				else
				{
					if (!text.StartsWith("IPM.Note.Mobile.SMS.Undercurrent", StringComparison.OrdinalIgnoreCase))
					{
						return text4;
					}
					if (string.IsNullOrEmpty(text4))
					{
						return SystemMessages.HumanTextFailedPasscodeWithoutReason(number);
					}
					return SystemMessages.HumanTextFailedPasscodeWithReason(number, text4);
				}
			});
			DsnParamText.PerRecipientItems = array;
			DsnParamItem[] array2 = new DsnParamItem[3];
			array2[0] = new DsnParamItem(new string[]
			{
				"MaxMessageSizeInKB",
				"CurrentMessageSizeInKB"
			}, delegate(DsnParameters dsnParameters, CultureInfo culture, out bool overwriteDefault)
			{
				long maxSize = (long)dsnParameters["MaxMessageSizeInKB"];
				long currentSize = (long)dsnParameters["CurrentMessageSizeInKB"];
				overwriteDefault = true;
				string maxSize2;
				string currentSize2;
				if (!DsnParamText.IsMessageSizeTextInKB(maxSize, currentSize, culture, out maxSize2, out currentSize2))
				{
					return SystemMessages.DsnParamTextMessageSizePerMessageInMB(currentSize2, maxSize2).ToString(culture);
				}
				return SystemMessages.DsnParamTextMessageSizePerMessageInKB(currentSize2, maxSize2).ToString(culture);
			});
			array2[1] = new DsnParamItem(new string[]
			{
				"MaxRecipientCount",
				"CurrentRecipientCount"
			}, delegate(DsnParameters dsnParameters, CultureInfo culture, out bool overwriteDefault)
			{
				int num = (int)dsnParameters["MaxRecipientCount"];
				int num2 = (int)dsnParameters["CurrentRecipientCount"];
				overwriteDefault = true;
				string maxRecipientCount = culture.IsNeutralCulture ? num.ToString() : num.ToString(culture);
				string currentRecipientCount = culture.IsNeutralCulture ? num2.ToString() : num2.ToString(culture);
				return SystemMessages.DsnParamTextRecipientCount(currentRecipientCount, maxRecipientCount).ToString(culture);
			});
			array2[2] = new DsnParamItem(new string[]
			{
				"MapiMessageClass"
			}, delegate(DsnParameters dsnParameters, CultureInfo culture, out bool overwriteDefault)
			{
				string text = (string)dsnParameters["MapiMessageClass"];
				overwriteDefault = true;
				if (text.StartsWith("IPM.Note.Mobile.SMS.Alert", StringComparison.OrdinalIgnoreCase))
				{
					return SystemMessages.FailedHumanReadableTopTextForTextMessageNotification.ToString(culture);
				}
				return SystemMessages.FailedHumanReadableTopTextForTextMessage.ToString(culture);
			});
			DsnParamText.PerMessageItems = array2;
			DsnParamText.PerMessageDsnParamText = new DsnParamText(DsnParamText.PerMessageItems);
			DsnParamText.PerRecipientDsnParamText = new DsnParamText(DsnParamText.PerRecipientItems);
		}

		public const string MaxRecipMesageSizeInKB = "MaxRecipMessageSizeInKB";

		public const string MaxMessageSizeInKB = "MaxMessageSizeInKB";

		public const string CurrentMessageSizeInKB = "CurrentMessageSizeInKB";

		public const string MaxRecipientCount = "MaxRecipientCount";

		public const string CurrentRecipientCount = "CurrentRecipientCount";

		public const string MapiMessageClass = "MapiMessageClass";

		public const string TextMessagingDeliveryPointType = "TextMessagingDeliveryPointType";

		public const string TextMessagingBodyText = "TextMessagingBodyText";

		public const string TextMessagingRecipientPhoneNumber = "TextMessagingRecipientPhoneNumber";

		public const string TextMessagingRecipientCarrier = "TextMessagingRecipientCarrier";

		public const string TextMessagingRecipientExceptions = "TextMessagingRecipientExceptions";

		private const string MessageClassSms = "IPM.Note.Mobile.SMS";

		private const string MessageClassSmsAlert = "IPM.Note.Mobile.SMS.Alert";

		private const string MessageClassSmsUndercurrent = "IPM.Note.Mobile.SMS.Undercurrent";

		private const string DeliveryPointTypeSmtpToSmsGateway = "SmtpToSmsGateway";

		private const string DeliveryPointTypeOutlookMobileService = "OutlookMobileService";

		private const string DeliveryPointTypeExchangeActiveSync = "ExchangeActiveSync";

		private static readonly DsnParamItem[] PerRecipientItems;

		private static readonly DsnParamItem[] PerMessageItems;

		public static readonly DsnParamText PerMessageDsnParamText;

		public static readonly DsnParamText PerRecipientDsnParamText;

		private DsnParamItem[] dsnParamItems;
	}
}
