using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.Exchange.Core;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.ExchangeSystem
{
	public sealed class ExRegistryTimeZoneProvider : ExTimeZoneProviderBase
	{
		public static ExRegistryTimeZoneProvider Instance
		{
			get
			{
				if (ExRegistryTimeZoneProvider.instance == null)
				{
					lock (ExRegistryTimeZoneProvider.instanceLock)
					{
						if (ExRegistryTimeZoneProvider.instance == null)
						{
							ExRegistryTimeZoneProvider.instance = new ExRegistryTimeZoneProvider("WindowsRegistry");
						}
					}
				}
				return ExRegistryTimeZoneProvider.instance;
			}
		}

		public static ExRegistryTimeZoneProvider InstanceWithReload
		{
			get
			{
				lock (ExRegistryTimeZoneProvider.instanceLock)
				{
					ExRegistryTimeZoneProvider.instance = null;
					ExRegistryTimeZoneProvider.instance = new ExRegistryTimeZoneProvider("WindowsRegistry");
				}
				return ExRegistryTimeZoneProvider.instance;
			}
		}

		public ExRegistryTimeZoneProvider(string id) : base(id)
		{
			IList<RegistryTimeZoneInformation> list = ExRegistryReader.ReadTimeZones();
			foreach (RegistryTimeZoneInformation tzInfo in list)
			{
				try
				{
					ExTimeZone timeZoneFromRegistryInfo = ExRegistryTimeZoneProvider.GetTimeZoneFromRegistryInfo(tzInfo);
					if (timeZoneFromRegistryInfo != null)
					{
						base.AddTimeZone(timeZoneFromRegistryInfo);
					}
				}
				catch (InvalidTimeZoneException)
				{
				}
			}
		}

		internal static ExTimeZone GetTimeZoneFromRegistryInfo(RegistryTimeZoneInformation tzInfo)
		{
			string id = ExRegistryTimeZoneProvider.BuildRegistryTimeZoneIdFromName(tzInfo.KeyName);
			return ExRegistryTimeZoneProvider.GetTimeZoneWithIdFromRegistryInfo(id, tzInfo);
		}

		internal static ExTimeZone GetTimeZoneWithIdFromRegistryInfo(string id, RegistryTimeZoneInformation tzInfo)
		{
			ExTimeZone result;
			try
			{
				LocalizedString localizedDisplayName = (id == "tzone://Microsoft/Custom") ? new LocalizedString(tzInfo.DisplayName) : ExRegistryTimeZoneProvider.GetLocalizedDisplayName(id, tzInfo.DisplayName);
				ExTimeZoneInformation exTimeZoneInformation = new ExTimeZoneInformation(id, tzInfo.DisplayName, localizedDisplayName, tzInfo.MuiStandardName);
				for (int i = 0; i < tzInfo.Rules.Count; i++)
				{
					ExRegistryTimeZoneProvider.LoadTimeZoneRuleFromRegistryInfo(exTimeZoneInformation, tzInfo, i);
				}
				result = new ExTimeZone(exTimeZoneInformation);
			}
			catch (ArgumentOutOfRangeException ex)
			{
				throw new InvalidTimeZoneException(ex.Message, ex);
			}
			return result;
		}

		internal static void LoadTimeZoneRuleFromRegistryInfo(ExTimeZoneInformation exTzInfo, RegistryTimeZoneInformation tzInfo, int index)
		{
			try
			{
				REG_TIMEZONE_INFO regTimeZoneInfo = tzInfo.Rules[index].RegTimeZoneInfo;
				TimeSpan bias = TimeSpan.FromMinutes((double)(-(double)regTimeZoneInfo.Bias - regTimeZoneInfo.StandardBias));
				TimeSpan bias2 = TimeSpan.FromMinutes((double)(-(double)regTimeZoneInfo.Bias - regTimeZoneInfo.DaylightBias));
				string ruleId = tzInfo.Rules[index].Start.Year.ToString();
				ExTimeZoneRuleGroup exTimeZoneRuleGroup;
				if (index < tzInfo.Rules.Count - 1)
				{
					int year = tzInfo.Rules[index + 1].Start.Year;
					exTimeZoneRuleGroup = new ExTimeZoneRuleGroup(new DateTime?(DateTime.SpecifyKind(new DateTime(year, 1, 1), DateTimeKind.Unspecified)));
				}
				else
				{
					exTimeZoneRuleGroup = new ExTimeZoneRuleGroup(null);
				}
				exTzInfo.AddGroup(exTimeZoneRuleGroup);
				if (regTimeZoneInfo.StandardDate.Month == 0 != (regTimeZoneInfo.DaylightDate.Month == 0))
				{
					throw new InvalidTimeZoneException("Incompatible DST transitions");
				}
				if (regTimeZoneInfo.StandardDate.Month == 0)
				{
					ExTimeZoneRule ruleInfo = new ExTimeZoneRule(ExRegistryTimeZoneProvider.BuildRegistryTimeZoneRuleIdFromName(tzInfo.KeyName, ruleId, "Standard"), "Standard", bias, null);
					exTimeZoneRuleGroup.AddRule(ruleInfo);
				}
				else
				{
					ExYearlyRecurringTime observanceEnd;
					ExYearlyRecurringTime observanceEnd2;
					if (regTimeZoneInfo.StandardDate.Year != 0)
					{
						observanceEnd = new ExYearlyRecurringDate((int)regTimeZoneInfo.DaylightDate.Month, (int)regTimeZoneInfo.DaylightDate.Day, (int)regTimeZoneInfo.DaylightDate.Hour, (int)regTimeZoneInfo.DaylightDate.Minute, (int)regTimeZoneInfo.DaylightDate.Second, (int)regTimeZoneInfo.DaylightDate.Milliseconds);
						observanceEnd2 = new ExYearlyRecurringDate((int)regTimeZoneInfo.StandardDate.Month, (int)regTimeZoneInfo.StandardDate.Day, (int)regTimeZoneInfo.StandardDate.Hour, (int)regTimeZoneInfo.StandardDate.Minute, (int)regTimeZoneInfo.StandardDate.Second, (int)regTimeZoneInfo.StandardDate.Milliseconds);
					}
					else
					{
						int occurrence = (regTimeZoneInfo.DaylightDate.Day == 5) ? -1 : ((int)regTimeZoneInfo.DaylightDate.Day);
						int occurrence2 = (regTimeZoneInfo.StandardDate.Day == 5) ? -1 : ((int)regTimeZoneInfo.StandardDate.Day);
						observanceEnd = new ExYearlyRecurringDay(occurrence, (DayOfWeek)regTimeZoneInfo.DaylightDate.DayOfWeek, (int)regTimeZoneInfo.DaylightDate.Month, (int)regTimeZoneInfo.DaylightDate.Hour, (int)regTimeZoneInfo.DaylightDate.Minute, (int)regTimeZoneInfo.DaylightDate.Second, (int)regTimeZoneInfo.DaylightDate.Milliseconds);
						observanceEnd2 = new ExYearlyRecurringDay(occurrence2, (DayOfWeek)regTimeZoneInfo.StandardDate.DayOfWeek, (int)regTimeZoneInfo.StandardDate.Month, (int)regTimeZoneInfo.StandardDate.Hour, (int)regTimeZoneInfo.StandardDate.Minute, (int)regTimeZoneInfo.StandardDate.Second, (int)regTimeZoneInfo.StandardDate.Milliseconds);
					}
					ExTimeZoneRule ruleInfo2 = new ExTimeZoneRule(ExRegistryTimeZoneProvider.BuildRegistryTimeZoneRuleIdFromName(tzInfo.KeyName, ruleId, "Daylight"), "Daylight", bias2, observanceEnd2);
					exTimeZoneRuleGroup.AddRule(ruleInfo2);
					ExTimeZoneRule ruleInfo3 = new ExTimeZoneRule(ExRegistryTimeZoneProvider.BuildRegistryTimeZoneRuleIdFromName(tzInfo.KeyName, ruleId, "Standard"), "Standard", bias, observanceEnd);
					exTimeZoneRuleGroup.AddRule(ruleInfo3);
				}
			}
			catch (ArgumentOutOfRangeException ex)
			{
				throw new InvalidTimeZoneException(ex.Message, ex);
			}
		}

		private static string BuildRegistryTimeZoneIdFromName(string name)
		{
			return name;
		}

		private static string BuildRegistryTimeZoneRuleIdFromName(string name, string ruleId, string relativeName)
		{
			StringBuilder stringBuilder = new StringBuilder(name.Length + ruleId.Length + relativeName.Length + 30);
			stringBuilder.Append("trule:Microsoft/Registry/");
			stringBuilder.Append(name);
			stringBuilder.Append("/");
			stringBuilder.Append(ruleId);
			stringBuilder.Append("-");
			stringBuilder.Append(relativeName);
			return stringBuilder.ToString();
		}

		private static LocalizedString GetLocalizedDisplayName(string id, string unlocalizedDisplayName)
		{
			string id2 = "TimeZone" + Regex.Replace(id, "[\\.\\(\\)\\s-+]", string.Empty);
			LocalizedString result = new LocalizedString(id2, ExRegistryTimeZoneProvider.ResourceManager, new object[0]);
			if (string.IsNullOrEmpty(result.ToString()))
			{
				result = new LocalizedString(unlocalizedDisplayName);
			}
			return result;
		}

		private const string TimeZoneIdEscapePattern = "[\\.\\(\\)\\s-+]";

		private const string TimeZoneLocalizedStringIdPrefix = "TimeZone";

		private static readonly ExchangeResourceManager ResourceManager = new ExchangeResourceManager.Concurrent(ExchangeResourceManager.GetResourceManager("Microsoft.Exchange.Core.CoreStrings", typeof(CoreStrings).GetTypeInfo().Assembly));

		private static ExRegistryTimeZoneProvider instance;

		private static object instanceLock = new object();
	}
}
