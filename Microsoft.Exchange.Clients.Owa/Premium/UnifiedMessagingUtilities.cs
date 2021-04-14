using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using Microsoft.Exchange.Clients.Owa.Core;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics.Components.Clients;
using Microsoft.Exchange.InfoWorker.Common.Availability;
using Microsoft.Exchange.UM.PersonalAutoAttendant;

namespace Microsoft.Exchange.Clients.Owa.Premium
{
	internal static class UnifiedMessagingUtilities
	{
		public static void RenderSender(UserContext userContext, TextWriter writer, MessageItem message)
		{
			if (writer == null)
			{
				throw new ArgumentNullException("writer");
			}
			if (message == null)
			{
				throw new ArgumentNullException("message");
			}
			if (Utilities.IsOnBehalfOf(message.Sender, message.From))
			{
				writer.Write(LocalizedStrings.GetHtmlEncoded(-165544498), RenderingUtilities.GetSender(userContext, message.Sender, "spnSender", null), UnifiedMessagingUtilities.GetUMSender(userContext, message, "spnFrom", SenderDisplayMode.NoPhoto));
				return;
			}
			writer.Write(UnifiedMessagingUtilities.GetUMSender(userContext, message, "spnFrom"));
		}

		public static SanitizedHtmlString GetUMSender(UserContext userContext, MessageItem message, string id)
		{
			return UnifiedMessagingUtilities.GetUMSender(userContext, message, id, SenderDisplayMode.DefaultDisplay);
		}

		public static SanitizedHtmlString GetUMSender(UserContext userContext, MessageItem message, string id, SenderDisplayMode senderDisplayMode)
		{
			if (message == null)
			{
				throw new ArgumentNullException("message");
			}
			Participant from = message.From;
			if (from == null)
			{
				return SanitizedHtmlString.Empty;
			}
			string text = message.TryGetProperty(MessageItemSchema.SenderTelephoneNumber) as string;
			if (string.IsNullOrEmpty(text) && string.IsNullOrEmpty(from.DisplayName))
			{
				return RenderingUtilities.GetSender(userContext, from, id, LocalizedStrings.GetNonEncoded(786269816), true, senderDisplayMode);
			}
			if (!string.IsNullOrEmpty(from.DisplayName))
			{
				return RenderingUtilities.GetSender(userContext, from, id, from.DisplayName, true, senderDisplayMode);
			}
			if (string.IsNullOrEmpty(Utilities.NormalizePhoneNumber(text)))
			{
				return RenderingUtilities.GetSender(userContext, from, id, text, true, senderDisplayMode);
			}
			return RenderingUtilities.GetSender(userContext, from, id, text, text, true, senderDisplayMode);
		}

		internal static string ValidatePhoneNumbers(UnifiedMessagingUtilities.ValidatePhoneNumber validatePhoneNumber, params string[] phoneNumbers)
		{
			IDataValidationResult dataValidationResult = new DataValidationResult();
			string data = string.Empty;
			string result = null;
			foreach (string text in phoneNumbers)
			{
				if (!validatePhoneNumber(text, out dataValidationResult))
				{
					data = text;
					break;
				}
			}
			if (dataValidationResult.PAAValidationResult != PAAValidationResult.Valid)
			{
				result = UnifiedMessagingUtilities.GetErrorResourceId(dataValidationResult.PAAValidationResult, data);
			}
			return result;
		}

		internal static string GetErrorResourceId(PAAValidationResult result, string data)
		{
			Strings.IDs? ds = null;
			bool flag = false;
			switch (result)
			{
			case PAAValidationResult.ParseError:
				ds = new Strings.IDs?(-1771439532);
				flag = true;
				goto IL_D8;
			case PAAValidationResult.SipUriInNonSipDialPlan:
				ds = new Strings.IDs?(-1315215359);
				flag = true;
				goto IL_D8;
			case PAAValidationResult.PermissionCheckFailure:
				ds = new Strings.IDs?(922375801);
				goto IL_D8;
			case PAAValidationResult.NonExistentContact:
				ds = new Strings.IDs?(2004848387);
				goto IL_D8;
			case PAAValidationResult.NoValidPhones:
				ds = new Strings.IDs?(839747470);
				flag = true;
				goto IL_D8;
			case PAAValidationResult.NonExistentDefaultContactsFolder:
				ds = new Strings.IDs?(2028770649);
				goto IL_D8;
			case PAAValidationResult.NonExistentDirectoryUser:
				ds = new Strings.IDs?(1890666003);
				goto IL_D8;
			case PAAValidationResult.NonMailboxDirectoryUser:
				ds = new Strings.IDs?(1357571235);
				flag = true;
				goto IL_D8;
			case PAAValidationResult.InvalidExtension:
				ds = new Strings.IDs?(-983837116);
				flag = true;
				goto IL_D8;
			}
			ds = new Strings.IDs?(-1018465893);
			IL_D8:
			string result2;
			if (flag)
			{
				result2 = Utilities.HtmlEncode(string.Format(CultureInfo.CurrentCulture, LocalizedStrings.GetNonEncoded(ds.Value), new object[]
				{
					data
				}));
			}
			else
			{
				result2 = LocalizedStrings.GetHtmlEncoded(ds.Value);
			}
			return result2;
		}

		internal static string CreatePAAPreviewString(PersonalAutoAttendant pAA, UserContext userContext)
		{
			StringBuilder stringBuilder = new StringBuilder();
			bool flag = true;
			stringBuilder.Append(LocalizedStrings.GetHtmlEncoded(-1942641301));
			if (pAA.ExtensionList != null && pAA.ExtensionList.Count > 0)
			{
				stringBuilder.Append(LocalizedStrings.GetHtmlEncoded(-1452720339));
				flag = false;
			}
			if (pAA.CallerIdList != null && pAA.CallerIdList.Count > 0)
			{
				UnifiedMessagingUtilities.ProcessCallerIdList(pAA, ref flag, stringBuilder, userContext);
			}
			if (pAA.TimeOfDay != TimeOfDayEnum.None)
			{
				UnifiedMessagingUtilities.ProcessTimeOfDay(pAA, ref flag, stringBuilder, userContext);
			}
			if (pAA.FreeBusy != FreeBusyStatusEnum.None)
			{
				UnifiedMessagingUtilities.ProcessFreeBusyStatus(pAA, ref flag, stringBuilder);
			}
			if (pAA.OutOfOffice != OutOfOfficeStatusEnum.None)
			{
				UnifiedMessagingUtilities.ProcessOutofOfficeStatus(pAA, ref flag, stringBuilder);
			}
			if (string.CompareOrdinal(stringBuilder.ToString(), LocalizedStrings.GetHtmlEncoded(-1942641301)) == 0)
			{
				stringBuilder.Length = 0;
				stringBuilder.Append(LocalizedStrings.GetHtmlEncoded(-194809129));
			}
			stringBuilder.Append("\r\n");
			stringBuilder.Append(UnifiedMessagingUtilities.CreatePAAPreviewActionString(pAA, userContext));
			return stringBuilder.ToString();
		}

		internal static Contact GetContact(StoreObjectId storeObjectId, UserContext userContext)
		{
			if (storeObjectId == null)
			{
				throw new ArgumentNullException("storeObjectId");
			}
			if (userContext == null)
			{
				throw new ArgumentNullException("userContext");
			}
			Contact result = null;
			try
			{
				result = Utilities.GetItem<Contact>(userContext.MailboxSession, storeObjectId, false, new PropertyDefinition[0]);
			}
			catch (StoragePermanentException ex)
			{
				ExTraceGlobals.CoreCallTracer.TraceDebug<string, string>(0L, "UnifiedMessagingUtilities::GetContact. Unable to get contact with StoreObjectId {0}. Exception: {1}.", storeObjectId.ToBase64String(), ex.Message);
			}
			return result;
		}

		private static void ProcessCallerIdList(PersonalAutoAttendant pAA, ref bool isFirst, StringBuilder builder, UserContext userContext)
		{
			if (!isFirst)
			{
				builder.Append("\r\n");
				builder.Append(LocalizedStrings.GetHtmlEncoded(-56156833));
			}
			else
			{
				isFirst = false;
			}
			bool flag = false;
			List<string> list = new List<string>();
			List<string> list2 = new List<string>();
			List<string> list3 = new List<string>();
			List<string> list4 = new List<string>();
			foreach (CallerIdBase callerIdBase in pAA.CallerIdList)
			{
				switch (callerIdBase.CallerIdType)
				{
				case CallerIdTypeEnum.Number:
				{
					string phoneNumberString = ((PhoneNumberCallerId)callerIdBase).PhoneNumberString;
					if (!string.IsNullOrEmpty(phoneNumberString))
					{
						list.Add(phoneNumberString);
						continue;
					}
					continue;
				}
				case CallerIdTypeEnum.ContactItem:
					using (Contact contact = UnifiedMessagingUtilities.GetContact(((ContactItemCallerId)callerIdBase).StoreObjectId, userContext))
					{
						string item = (contact != null && !string.IsNullOrEmpty(contact.DisplayName)) ? contact.DisplayName : LocalizedStrings.GetNonEncoded(-1626952556);
						list2.Add(item);
						continue;
					}
					break;
				case CallerIdTypeEnum.DefaultContactFolder:
					flag = true;
					continue;
				case CallerIdTypeEnum.ADContact:
					break;
				default:
					continue;
				}
				list3.Add(((ADContactCallerId)callerIdBase).LegacyExchangeDN);
			}
			builder.Append(LocalizedStrings.GetHtmlEncoded(235538885));
			bool flag2 = false;
			bool flag3 = false;
			if (list.Count > 0)
			{
				builder.Append(Utilities.HtmlEncode(string.Format(CultureInfo.CurrentCulture, LocalizedStrings.GetNonEncoded(1695555698), new object[]
				{
					string.Join(", ", list.ToArray())
				})));
				flag2 = true;
			}
			if (list3.Count > 0)
			{
				list4 = UnifiedMessagingUtilities.GetContactNamesFromLegacyDN(list3, userContext);
				flag3 = true;
			}
			if (list2.Count > 0 || flag3)
			{
				string[] array = new string[list2.Count + list3.Count];
				if (flag3)
				{
					Array.Copy(list4.ToArray(), array, list4.Count);
				}
				if (list2.Count > 0)
				{
					Array.Copy(list2.ToArray(), 0, array, list4.Count, list2.Count);
				}
				if (flag2)
				{
					builder.Append(LocalizedStrings.GetHtmlEncoded(53305273));
				}
				builder.Append(Utilities.HtmlEncode(string.Format(CultureInfo.CurrentCulture, LocalizedStrings.GetNonEncoded(435094942), new object[]
				{
					string.Join(", ", array)
				})));
				flag3 = true;
			}
			if (flag)
			{
				if (flag2 || flag3)
				{
					builder.Append(LocalizedStrings.GetHtmlEncoded(53305273));
				}
				builder.Append(LocalizedStrings.GetHtmlEncoded(623331318));
			}
		}

		private static void ProcessTimeOfDay(PersonalAutoAttendant pAA, ref bool isFirst, StringBuilder builder, UserContext userContext)
		{
			if (!isFirst)
			{
				builder.Append("\r\n");
				builder.Append(LocalizedStrings.GetHtmlEncoded(-56156833));
			}
			else
			{
				isFirst = false;
			}
			string s = string.Empty;
			switch (pAA.TimeOfDay)
			{
			case TimeOfDayEnum.WorkingHours:
				s = LocalizedStrings.GetNonEncoded(1604545240);
				break;
			case TimeOfDayEnum.NonWorkingHours:
				s = LocalizedStrings.GetNonEncoded(955250317);
				break;
			case TimeOfDayEnum.Custom:
				s = UnifiedMessagingUtilities.GetCustomWorkingHours(pAA, userContext);
				break;
			}
			builder.Append(Utilities.HtmlEncode(s));
		}

		private static void ProcessFreeBusyStatus(PersonalAutoAttendant pAA, ref bool isFirst, StringBuilder builder)
		{
			if (!isFirst)
			{
				builder.Append("\r\n");
				builder.Append(LocalizedStrings.GetHtmlEncoded(-56156833));
			}
			else
			{
				isFirst = false;
			}
			List<string> list = new List<string>();
			if ((pAA.FreeBusy & FreeBusyStatusEnum.Busy) != FreeBusyStatusEnum.None)
			{
				list.Add(LocalizedStrings.GetNonEncoded(1864090954));
			}
			if ((pAA.FreeBusy & FreeBusyStatusEnum.Free) != FreeBusyStatusEnum.None)
			{
				list.Add(LocalizedStrings.GetNonEncoded(-575819061));
			}
			if ((pAA.FreeBusy & FreeBusyStatusEnum.Tentative) != FreeBusyStatusEnum.None)
			{
				list.Add(LocalizedStrings.GetNonEncoded(-559850781));
			}
			if ((pAA.FreeBusy & FreeBusyStatusEnum.OutOfOffice) != FreeBusyStatusEnum.None)
			{
				list.Add(LocalizedStrings.GetNonEncoded(-856091954));
			}
			builder.Append(Utilities.HtmlEncode(string.Format(CultureInfo.CurrentCulture, LocalizedStrings.GetNonEncoded(1058580864), new object[]
			{
				string.Join(LocalizedStrings.GetNonEncoded(53305273), list.ToArray())
			})));
		}

		private static void ProcessOutofOfficeStatus(PersonalAutoAttendant pAA, ref bool isFirst, StringBuilder builder)
		{
			if (!isFirst)
			{
				builder.Append("\r\n");
				builder.Append(LocalizedStrings.GetHtmlEncoded(-56156833));
			}
			else
			{
				isFirst = false;
			}
			builder.Append(LocalizedStrings.GetHtmlEncoded(-2042562863));
			if (pAA.OutOfOffice == OutOfOfficeStatusEnum.Oof)
			{
				builder.Append(LocalizedStrings.GetHtmlEncoded(-856091954));
				return;
			}
			if (pAA.OutOfOffice == OutOfOfficeStatusEnum.NotOof)
			{
				builder.Append(LocalizedStrings.GetHtmlEncoded(727321927));
			}
		}

		private static string GetContactNameFromLegacyDN(string legacyDN, UserContext userContext)
		{
			if (legacyDN == null)
			{
				throw new ArgumentNullException("legacyDN");
			}
			if (userContext == null)
			{
				throw new ArgumentNullException("userContext");
			}
			AdRecipientBatchQuery adRecipientBatchQuery = new AdRecipientBatchQuery(userContext, new string[]
			{
				legacyDN
			});
			ADRecipient adRecipient = adRecipientBatchQuery.GetAdRecipient(legacyDN);
			if (adRecipient == null || string.IsNullOrEmpty(adRecipient.DisplayName))
			{
				return LocalizedStrings.GetNonEncoded(-1626952556);
			}
			return adRecipient.DisplayName;
		}

		private static List<string> GetContactNamesFromLegacyDN(List<string> legacyDNs, UserContext userContext)
		{
			if (legacyDNs == null)
			{
				throw new ArgumentNullException("legacyDNs");
			}
			if (userContext == null)
			{
				throw new ArgumentNullException("userContext");
			}
			List<string> list = new List<string>(legacyDNs.Count);
			AdRecipientBatchQuery adRecipientBatchQuery = new AdRecipientBatchQuery(userContext, legacyDNs.ToArray());
			for (int i = 0; i < legacyDNs.Count; i++)
			{
				ADRecipient adRecipient = adRecipientBatchQuery.GetAdRecipient(legacyDNs[i]);
				list.Add((adRecipient != null && !string.IsNullOrEmpty(adRecipient.DisplayName)) ? adRecipient.DisplayName : LocalizedStrings.GetNonEncoded(-1626952556));
			}
			return list;
		}

		private static string GetCustomWorkingHours(PersonalAutoAttendant pAA, UserContext userContext)
		{
			if (DateTimeUtilities.GetHoursFormat(userContext.UserOptions.TimeFormat) == null)
			{
			}
			string timeDisplayValue = UnifiedMessagingUtilities.GetTimeDisplayValue(pAA.WorkingPeriod.StartTimeInMinutes, userContext);
			string timeDisplayValue2 = UnifiedMessagingUtilities.GetTimeDisplayValue(pAA.WorkingPeriod.EndTimeInMinutes, userContext);
			string days = UnifiedMessagingUtilities.GetDays(pAA.WorkingPeriod.DayOfWeek);
			return string.Format(CultureInfo.CurrentCulture, LocalizedStrings.GetNonEncoded(-250514213), new object[]
			{
				timeDisplayValue,
				timeDisplayValue2,
				days
			});
		}

		private static string GetTimeDisplayValue(int minutes, UserContext userContext)
		{
			return (DateTime.UtcNow.Date + TimeSpan.FromMinutes((double)minutes)).ToString(userContext.UserOptions.TimeFormat, CultureInfo.InvariantCulture);
		}

		private static string GetDays(Microsoft.Exchange.InfoWorker.Common.Availability.DaysOfWeek daysOfWeek)
		{
			string[] dayNames = CultureInfo.CurrentCulture.DateTimeFormat.DayNames;
			List<string> list = new List<string>();
			string text = null;
			if (daysOfWeek == (Microsoft.Exchange.InfoWorker.Common.Availability.DaysOfWeek.Sunday | Microsoft.Exchange.InfoWorker.Common.Availability.DaysOfWeek.Monday | Microsoft.Exchange.InfoWorker.Common.Availability.DaysOfWeek.Tuesday | Microsoft.Exchange.InfoWorker.Common.Availability.DaysOfWeek.Wednesday | Microsoft.Exchange.InfoWorker.Common.Availability.DaysOfWeek.Thursday | Microsoft.Exchange.InfoWorker.Common.Availability.DaysOfWeek.Friday | Microsoft.Exchange.InfoWorker.Common.Availability.DaysOfWeek.Saturday))
			{
				text = LocalizedStrings.GetNonEncoded(-403478885);
			}
			else
			{
				if ((daysOfWeek & Microsoft.Exchange.InfoWorker.Common.Availability.DaysOfWeek.Sunday) != (Microsoft.Exchange.InfoWorker.Common.Availability.DaysOfWeek)0)
				{
					list.Add(dayNames[0]);
				}
				if ((daysOfWeek & Microsoft.Exchange.InfoWorker.Common.Availability.DaysOfWeek.Monday) != (Microsoft.Exchange.InfoWorker.Common.Availability.DaysOfWeek)0)
				{
					list.Add(dayNames[1]);
				}
				if ((daysOfWeek & Microsoft.Exchange.InfoWorker.Common.Availability.DaysOfWeek.Tuesday) != (Microsoft.Exchange.InfoWorker.Common.Availability.DaysOfWeek)0)
				{
					list.Add(dayNames[2]);
				}
				if ((daysOfWeek & Microsoft.Exchange.InfoWorker.Common.Availability.DaysOfWeek.Wednesday) != (Microsoft.Exchange.InfoWorker.Common.Availability.DaysOfWeek)0)
				{
					list.Add(dayNames[3]);
				}
				if ((daysOfWeek & Microsoft.Exchange.InfoWorker.Common.Availability.DaysOfWeek.Thursday) != (Microsoft.Exchange.InfoWorker.Common.Availability.DaysOfWeek)0)
				{
					list.Add(dayNames[4]);
				}
				if ((daysOfWeek & Microsoft.Exchange.InfoWorker.Common.Availability.DaysOfWeek.Friday) != (Microsoft.Exchange.InfoWorker.Common.Availability.DaysOfWeek)0)
				{
					list.Add(dayNames[5]);
				}
				if ((daysOfWeek & Microsoft.Exchange.InfoWorker.Common.Availability.DaysOfWeek.Saturday) != (Microsoft.Exchange.InfoWorker.Common.Availability.DaysOfWeek)0)
				{
					list.Add(dayNames[6]);
				}
			}
			if (text != null)
			{
				return text;
			}
			return string.Join(LocalizedStrings.GetNonEncoded(53305273), list.ToArray());
		}

		private static string CreatePAAPreviewActionString(PersonalAutoAttendant pAA, UserContext userContext)
		{
			StringBuilder stringBuilder = new StringBuilder();
			bool flag = false;
			stringBuilder.Append("\r\n");
			stringBuilder.Append(LocalizedStrings.GetHtmlEncoded(1230987283));
			if (pAA.KeyMappingList.Count == 0)
			{
				return LocalizedStrings.GetHtmlEncoded(-1306184482);
			}
			foreach (KeyMappingBase keyMappingBase in pAA.KeyMappingList.SortedMenu)
			{
				switch (keyMappingBase.KeyMappingType)
				{
				case KeyMappingTypeEnum.TransferToNumber:
					UnifiedMessagingUtilities.TransferToPhoneNumber(keyMappingBase, stringBuilder, flag);
					if (!flag)
					{
						flag = true;
					}
					break;
				case KeyMappingTypeEnum.TransferToADContactMailbox:
					UnifiedMessagingUtilities.TransferToContact(keyMappingBase, userContext, stringBuilder, KeyMappingTypeEnum.TransferToADContactMailbox, flag);
					if (!flag)
					{
						flag = true;
					}
					break;
				case KeyMappingTypeEnum.TransferToADContactPhone:
					UnifiedMessagingUtilities.TransferToContact(keyMappingBase, userContext, stringBuilder, KeyMappingTypeEnum.TransferToADContactPhone, flag);
					if (!flag)
					{
						flag = true;
					}
					break;
				case KeyMappingTypeEnum.TransferToVoicemail:
					if (flag)
					{
						stringBuilder.Append("\r\n");
					}
					stringBuilder.Append(LocalizedStrings.GetHtmlEncoded(-246907567));
					if (!flag)
					{
						flag = true;
					}
					break;
				case KeyMappingTypeEnum.FindMe:
				{
					if (flag)
					{
						stringBuilder.Append("\r\n");
					}
					string text = UnifiedMessagingUtilities.AddContext(keyMappingBase);
					stringBuilder.Append(Utilities.HtmlEncode(string.Format(CultureInfo.CurrentCulture, LocalizedStrings.GetNonEncoded(-578565389), new object[]
					{
						text,
						keyMappingBase.Key
					})));
					if (!flag)
					{
						flag = true;
					}
					break;
				}
				}
			}
			return stringBuilder.ToString();
		}

		private static string AddContext(KeyMappingBase keyMappingBase)
		{
			string result = string.Empty;
			if (!string.IsNullOrEmpty(keyMappingBase.Context))
			{
				result = string.Format(CultureInfo.CurrentCulture, LocalizedStrings.GetNonEncoded(828545169), new object[]
				{
					keyMappingBase.Context
				});
			}
			return result;
		}

		private static void TransferToContact(KeyMappingBase keyMappingBase, UserContext userContext, StringBuilder builder, KeyMappingTypeEnum keyMappingType, bool addNewLine)
		{
			string text = UnifiedMessagingUtilities.AddContext(keyMappingBase);
			string contactNameFromLegacyDN = UnifiedMessagingUtilities.GetContactNameFromLegacyDN(((TransferToADContact)keyMappingBase).LegacyExchangeDN, userContext);
			Strings.IDs? ds = null;
			if (keyMappingType == KeyMappingTypeEnum.TransferToADContactMailbox)
			{
				ds = new Strings.IDs?(1644624696);
			}
			else
			{
				ds = new Strings.IDs?(-348119052);
			}
			if (addNewLine)
			{
				builder.Append("\r\n");
			}
			builder.Append(Utilities.HtmlEncode(string.Format(CultureInfo.CurrentCulture, LocalizedStrings.GetNonEncoded(ds.Value), new object[]
			{
				text,
				keyMappingBase.Key,
				contactNameFromLegacyDN
			})));
		}

		private static void TransferToPhoneNumber(KeyMappingBase keyMappingBase, StringBuilder builder, bool addNewLine)
		{
			string text = UnifiedMessagingUtilities.AddContext(keyMappingBase);
			if (addNewLine)
			{
				builder.Append("\r\n");
			}
			builder.Append(Utilities.HtmlEncode(string.Format(CultureInfo.CurrentCulture, LocalizedStrings.GetNonEncoded(-348119052), new object[]
			{
				text,
				keyMappingBase.Key,
				((TransferToNumber)keyMappingBase).PhoneNumberString
			})));
		}

		private const string NewLine = "\r\n";

		private const string CommaSpace = ", ";

		internal delegate bool ValidatePhoneNumber(string phoneNumber, out IDataValidationResult result);
	}
}
