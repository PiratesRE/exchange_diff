using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Storage.Management;
using Microsoft.Exchange.Data.Storage.Principal;
using Microsoft.Exchange.Data.Storage.StoreConfigurableType;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Data.Storage;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Win32;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal static class TimeZoneHelper
	{
		public static REG_TIMEZONE_INFO RegTimeZoneInfoFromExTimeZone(ExTimeZone timeZone)
		{
			return TimeZoneHelper.RegTimeZoneInfoFromExTimeZone(timeZone, ExDateTime.Now);
		}

		public static REG_TIMEZONE_INFO RegTimeZoneInfoFromExTimeZone(ExTimeZone timeZone, ExDateTime effectiveTime)
		{
			DateTime t = (DateTime)effectiveTime.ToUtc();
			ExTimeZoneRuleGroup exTimeZoneRuleGroup = null;
			foreach (ExTimeZoneRuleGroup exTimeZoneRuleGroup2 in timeZone.TimeZoneInformation.Groups)
			{
				if (exTimeZoneRuleGroup == null)
				{
					exTimeZoneRuleGroup = exTimeZoneRuleGroup2;
				}
				if (exTimeZoneRuleGroup2.EffectiveUtcStart <= t && exTimeZoneRuleGroup2.EffectiveUtcEnd > t)
				{
					exTimeZoneRuleGroup = exTimeZoneRuleGroup2;
					break;
				}
			}
			if (exTimeZoneRuleGroup.Rules.Count > 2)
			{
				throw new NotImplementedException();
			}
			return TimeZoneHelper.RegTimeZoneInfoFromExTimeZoneRuleGroup(exTimeZoneRuleGroup);
		}

		internal static REG_TIMEZONE_INFO RegTimeZoneInfoFromExTimeZoneRuleGroup(ExTimeZoneRuleGroup group)
		{
			REG_TIMEZONE_INFO result = default(REG_TIMEZONE_INFO);
			ExTimeZoneRule exTimeZoneRule = group.Rules[0];
			ExTimeZoneRule exTimeZoneRule2 = (group.Rules.Count > 1) ? group.Rules[1] : null;
			if (exTimeZoneRule2 != null && exTimeZoneRule.Bias > exTimeZoneRule2.Bias)
			{
				ExTimeZoneRule exTimeZoneRule3 = exTimeZoneRule;
				exTimeZoneRule = exTimeZoneRule2;
				exTimeZoneRule2 = exTimeZoneRule3;
			}
			result.Bias = (int)(-(int)exTimeZoneRule.Bias.TotalMinutes);
			result.StandardBias = 0;
			if (exTimeZoneRule2 != null)
			{
				result.DaylightBias = (int)(exTimeZoneRule.Bias.TotalMinutes - exTimeZoneRule2.Bias.TotalMinutes);
				result.StandardDate = TimeZoneHelper.Win32SystemTimeFromRecurringTime(exTimeZoneRule2.ObservanceEnd);
				result.DaylightDate = TimeZoneHelper.Win32SystemTimeFromRecurringTime(exTimeZoneRule.ObservanceEnd);
			}
			return result;
		}

		private static NativeMethods.SystemTime Win32SystemTimeFromRecurringTime(ExYearlyRecurringTime recurring)
		{
			NativeMethods.SystemTime result = default(NativeMethods.SystemTime);
			ExYearlyRecurringDate exYearlyRecurringDate = recurring as ExYearlyRecurringDate;
			ExYearlyRecurringDay exYearlyRecurringDay = recurring as ExYearlyRecurringDay;
			if (exYearlyRecurringDate != null)
			{
				result.Year = (ushort)ExDateTime.Now.Year;
				result.Month = (ushort)exYearlyRecurringDate.Month;
				result.Day = (ushort)exYearlyRecurringDate.Day;
				result.Hour = (ushort)exYearlyRecurringDate.Hour;
				result.Minute = (ushort)exYearlyRecurringDate.Minute;
				result.Second = (ushort)exYearlyRecurringDate.Second;
				result.Milliseconds = (ushort)exYearlyRecurringDate.Milliseconds;
			}
			else
			{
				if (exYearlyRecurringDay == null)
				{
					throw new InvalidOperationException();
				}
				result.Year = 0;
				result.Month = (ushort)exYearlyRecurringDay.Month;
				result.Day = (ushort)((exYearlyRecurringDay.Occurrence == -1) ? 5 : exYearlyRecurringDay.Occurrence);
				result.DayOfWeek = (ushort)exYearlyRecurringDay.DayOfWeek;
				result.Hour = (ushort)exYearlyRecurringDay.Hour;
				result.Minute = (ushort)exYearlyRecurringDay.Minute;
				result.Second = (ushort)exYearlyRecurringDay.Second;
				result.Milliseconds = (ushort)exYearlyRecurringDay.Milliseconds;
			}
			return result;
		}

		public static ExTimeZone GetExTimeZoneFromItem(Item item)
		{
			ExTimeZone exTimeZone = TimeZoneHelper.GetRecurringTimeZoneFromPropertyBag(item.PropertyBag);
			if (exTimeZone == null)
			{
				exTimeZone = TimeZoneHelper.GetTimeZoneFromProperties("Customized Time Zone", null, item.GetValueOrDefault<byte[]>(InternalSchema.TimeZoneDefinitionStart));
			}
			if (exTimeZone == null)
			{
				if (item.Session != null && item.Session.ExTimeZone != ExTimeZone.UtcTimeZone)
				{
					exTimeZone = item.Session.ExTimeZone;
				}
				else
				{
					ExTimeZoneHelperForMigrationOnly.CheckValidationLevel(false, ExTimeZoneHelperForMigrationOnly.ValidationLevel.Low, "TimeZoneHelper.GetTimeZoneFromItem: no time zone", new object[0]);
					exTimeZone = ExTimeZone.CurrentTimeZone;
				}
			}
			return exTimeZone;
		}

		public static ExTimeZone GetPromotedTimeZoneFromItem(Item item)
		{
			ExTimeZone exTimeZoneFromItem = TimeZoneHelper.GetExTimeZoneFromItem(item);
			ExTimeZone exTimeZone = null;
			if (exTimeZoneFromItem != null)
			{
				exTimeZone = TimeZoneHelper.PromoteCustomizedTimeZone(exTimeZoneFromItem);
			}
			return exTimeZone ?? exTimeZoneFromItem;
		}

		public static ExDateTime NormalizeUtcTime(ExDateTime utcTime, ExTimeZone legacyTimeZone)
		{
			if (legacyTimeZone == null || !legacyTimeZone.IsCustomTimeZone)
			{
				return utcTime.ToUtc();
			}
			DateTime localTime = legacyTimeZone.ConvertDateTime(utcTime).LocalTime;
			ExTimeZone exTimeZone = TimeZoneHelper.PromoteCustomizedTimeZone(legacyTimeZone);
			if (exTimeZone != null)
			{
				return new ExDateTime(exTimeZone, localTime);
			}
			return utcTime.ToUtc();
		}

		public static ExDateTime DeNormalizeToUtcTime(ExDateTime time, ExTimeZone legacyTimeZone)
		{
			if (legacyTimeZone == null || !legacyTimeZone.IsCustomTimeZone)
			{
				return time.ToUtc();
			}
			ExTimeZone exTimeZone = TimeZoneHelper.PromoteCustomizedTimeZone(legacyTimeZone);
			if (exTimeZone != null)
			{
				DateTime localTime = exTimeZone.ConvertDateTime(time).LocalTime;
				return new ExDateTime(legacyTimeZone, localTime).ToUtc();
			}
			return time;
		}

		public static ExTimeZone GetUserTimeZone(MailboxSession mailboxSession)
		{
			ExTimeZone result = null;
			UserConfiguration mailboxConfiguration = UserConfigurationHelper.GetMailboxConfiguration(mailboxSession, "OWA.UserOptions", UserConfigurationTypes.Dictionary, false);
			if (mailboxConfiguration == null)
			{
				ExTraceGlobals.StorageTracer.TraceDebug<IExchangePrincipal>(0L, "{0}: UserOption doesn't exist.", mailboxSession.MailboxOwner);
			}
			else
			{
				using (mailboxConfiguration)
				{
					IDictionary dictionary = null;
					try
					{
						dictionary = mailboxConfiguration.GetDictionary();
					}
					catch (CorruptDataException)
					{
						ExTraceGlobals.StorageTracer.TraceError<IExchangePrincipal>(0L, "{0}: Dictionary exists but is corrupt.", mailboxSession.MailboxOwner);
					}
					catch (InvalidOperationException)
					{
						ExTraceGlobals.StorageTracer.TraceError<IExchangePrincipal>(0L, "{0}: Dictionary is invalid.", mailboxSession.MailboxOwner);
					}
					if (dictionary != null)
					{
						string text = dictionary[MailboxRegionalConfigurationSchema.TimeZone.Name] as string;
						ExTraceGlobals.StorageTracer.TraceDebug<IExchangePrincipal, string>(0L, "{0}: Get timezone from dictionary of configuration. KeyName = {1}", mailboxSession.MailboxOwner, text);
						if (string.IsNullOrEmpty(text) || !ExTimeZoneEnumerator.Instance.TryGetTimeZoneByName(text, out result))
						{
							ExTraceGlobals.StorageTracer.TraceError<IExchangePrincipal, string>(0L, "{0}: The KeyName of TimeZone is invalid. KeyName = {1}", mailboxSession.MailboxOwner, text);
						}
					}
				}
			}
			return result;
		}

		internal static ExDateTime AssignLocalTimeToUtc(ExDateTime timeToReserve)
		{
			DateTime dateTime = DateTime.SpecifyKind(timeToReserve.LocalTime, DateTimeKind.Unspecified);
			return new ExDateTime(ExTimeZone.UtcTimeZone, dateTime);
		}

		internal static ExDateTime AssignUtcTimeToLocal(ExDateTime timeToRestore, ExTimeZone localTimeZone)
		{
			DateTime dateTime = DateTime.SpecifyKind(timeToRestore.UniversalTime, DateTimeKind.Unspecified);
			return new ExDateTime(localTimeZone, dateTime);
		}

		public static ExTimeZone GetRecurringTimeZoneFromPropertyBag(PropertyBag propertyBag)
		{
			return TimeZoneHelper.GetRecurringTimeZoneFromPropertyBag(propertyBag.AsIStorePropertyBag());
		}

		public static ExTimeZone GetRecurringTimeZoneFromPropertyBag(IStorePropertyBag propertyBag)
		{
			string valueOrDefault = propertyBag.GetValueOrDefault<string>(InternalSchema.TimeZone, null);
			byte[] valueOrDefault2 = propertyBag.GetValueOrDefault<byte[]>(InternalSchema.TimeZoneBlob, null);
			byte[] valueOrDefault3 = propertyBag.GetValueOrDefault<byte[]>(InternalSchema.TimeZoneDefinitionRecurring, null);
			return TimeZoneHelper.GetTimeZoneFromProperties(valueOrDefault, valueOrDefault2, valueOrDefault3);
		}

		internal static ExTimeZone GetTimeZoneFromProperties(string timeZoneDisplayName, byte[] o11TimeZoneBlob, byte[] o12TimeZoneBlob)
		{
			string text = timeZoneDisplayName ?? string.Empty;
			ExTimeZone result;
			if (O12TimeZoneFormatter.TryParseTimeZoneBlob(o12TimeZoneBlob, text, out result))
			{
				return result;
			}
			ExTimeZone result2;
			if (O11TimeZoneFormatter.TryParseTimeZoneBlob(o11TimeZoneBlob, text, out result2))
			{
				return result2;
			}
			return null;
		}

		internal static ExTimeZone PromoteCustomizedTimeZone(ExTimeZone customizedTimeZone)
		{
			ExTimeZone exTimeZone = null;
			if (customizedTimeZone.IsCustomTimeZone && !ExTimeZoneEnumerator.Instance.TryGetTimeZoneByName(customizedTimeZone.AlternativeId, out exTimeZone))
			{
				string displayName = customizedTimeZone.LocalizableDisplayName.ToString();
				List<ExTimeZone> list = TimeZoneHelper.MatchCustomTimeZoneByEffectiveRule(customizedTimeZone);
				exTimeZone = TimeZoneHelper.GetUniqueTimeZoneMatch(list, displayName);
				if (exTimeZone == null && list.Count > 1)
				{
					List<ExTimeZone> timeZones = TimeZoneHelper.MatchCustomTimeZoneByRules(customizedTimeZone, list);
					exTimeZone = TimeZoneHelper.GetUniqueTimeZoneMatch(timeZones, displayName);
				}
			}
			return exTimeZone;
		}

		private static List<ExTimeZone> MatchCustomTimeZoneByEffectiveRule(ExTimeZone customTimeZone)
		{
			List<ExTimeZone> list = new List<ExTimeZone>();
			REG_TIMEZONE_INFO v = TimeZoneHelper.RegTimeZoneInfoFromExTimeZone(customTimeZone);
			foreach (ExTimeZone exTimeZone in ExTimeZoneEnumerator.Instance)
			{
				if (v == TimeZoneHelper.RegTimeZoneInfoFromExTimeZone(exTimeZone))
				{
					list.Add(exTimeZone);
				}
			}
			return list;
		}

		private static List<ExTimeZone> MatchCustomTimeZoneByRules(ExTimeZone customTimeZone, List<ExTimeZone> candidates)
		{
			List<ExTimeZone> list = new List<ExTimeZone>();
			byte[] timeZoneBlob = O12TimeZoneFormatter.GetTimeZoneBlob(customTimeZone);
			foreach (ExTimeZone exTimeZone in candidates)
			{
				byte[] timeZoneBlob2 = O12TimeZoneFormatter.GetTimeZoneBlob(exTimeZone);
				if (O12TimeZoneFormatter.CompareBlob(timeZoneBlob, timeZoneBlob2, false))
				{
					list.Add(exTimeZone);
				}
			}
			return list;
		}

		private static ExTimeZone GetUniqueTimeZoneMatch(List<ExTimeZone> timeZones, string displayName)
		{
			ExTimeZone exTimeZone = null;
			if (timeZones.Count == 1)
			{
				exTimeZone = timeZones[0];
			}
			else if (timeZones.Count > 1 && !string.IsNullOrEmpty(displayName))
			{
				foreach (ExTimeZone exTimeZone2 in timeZones)
				{
					if (displayName.Equals(exTimeZone2.LocalizableDisplayName.ToString(), StringComparison.OrdinalIgnoreCase))
					{
						if (exTimeZone != null)
						{
							exTimeZone = null;
							break;
						}
						exTimeZone = exTimeZone2;
					}
				}
			}
			return exTimeZone;
		}

		public static ExTimeZone CreateExTimeZoneFromRegTimeZoneInfo(REG_TIMEZONE_INFO regInfo, string keyName)
		{
			ExTimeZone exTimeZone = TimeZoneHelper.CreateCustomExTimeZoneFromRegTimeZoneInfo(regInfo, keyName, keyName);
			return TimeZoneHelper.PromoteCustomizedTimeZone(exTimeZone) ?? exTimeZone;
		}

		public static ExTimeZone CreateCustomExTimeZoneFromRegTimeZoneInfo(REG_TIMEZONE_INFO regInfo, string keyName, string displayName)
		{
			return TimeZoneHelper.CreateCustomExTimeZoneFromRegRules(regInfo, keyName, displayName, new List<RegistryTimeZoneRule>(1)
			{
				new RegistryTimeZoneRule(DateTime.MinValue.Year, regInfo)
			});
		}

		internal static ExTimeZone CreateCustomExTimeZoneFromRegRules(REG_TIMEZONE_INFO regInfo, string keyName, string displayName, List<RegistryTimeZoneRule> regRules)
		{
			RegistryTimeZoneInformation registryTimeZoneInformation = new RegistryTimeZoneInformation(keyName, displayName, string.Empty, string.Empty, string.Empty, regInfo);
			foreach (RegistryTimeZoneRule item in regRules)
			{
				registryTimeZoneInformation.Rules.Add(item);
			}
			ExTimeZone timeZoneWithIdFromRegistryInfo = ExRegistryTimeZoneProvider.GetTimeZoneWithIdFromRegistryInfo("tzone://Microsoft/Custom", registryTimeZoneInformation);
			timeZoneWithIdFromRegistryInfo.TimeZoneInformation.AlternativeId = keyName;
			return timeZoneWithIdFromRegistryInfo;
		}

		internal static class TestAccess
		{
			internal static REG_TIMEZONE_INFO RegTimeZoneInfoFromExTimeZone(ExTimeZone timeZone)
			{
				return TimeZoneHelper.RegTimeZoneInfoFromExTimeZone(timeZone);
			}
		}
	}
}
