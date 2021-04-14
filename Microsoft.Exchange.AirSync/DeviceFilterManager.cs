using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics.Components.AirSync;

namespace Microsoft.Exchange.AirSync
{
	public static class DeviceFilterManager
	{
		public static bool ContactsOnly
		{
			get
			{
				return DeviceFilterManager.IsDeviceInFilter("ContactsOnly");
			}
		}

		public static bool V25OnlyInOptions
		{
			get
			{
				return DeviceFilterManager.IsDeviceInFilter("V25OnlyInOptions");
			}
		}

		public static bool IsDeviceInFilter(string filterName)
		{
			if (Command.CurrentCommand != null && Command.CurrentCommand.Context != null)
			{
				IOrganizationSettingsData organizationSettingsData = ADNotificationManager.GetOrganizationSettingsData(Command.CurrentCommand.Context.User);
				if (organizationSettingsData == null || organizationSettingsData.DeviceFiltering == null || organizationSettingsData.DeviceFiltering.Count == 0)
				{
					AirSyncDiagnostics.TraceDebug(ExTraceGlobals.RequestsTracer, null, "There are no device filters or failed to get device filters");
					return false;
				}
				ActiveSyncDeviceFilter filter = organizationSettingsData.DeviceFiltering.GetFilter(filterName);
				if (filter == null)
				{
					AirSyncDiagnostics.TraceDebug<string>(ExTraceGlobals.RequestsTracer, null, "There are no device filters by given name:{0}", filterName);
					return false;
				}
				if (filter.ApplyForAllDevices)
				{
					AirSyncDiagnostics.TraceDebug<string>(ExTraceGlobals.RequestsTracer, null, "Device filter name:{0} has been set to apply for all devices", filterName);
					return true;
				}
				if (filter.Rules == null)
				{
					AirSyncDiagnostics.TraceDebug<string>(ExTraceGlobals.RequestsTracer, null, "There are no rules for the given device filters name:{0}", filterName);
					return false;
				}
				string text;
				string text2;
				string text3;
				string text4;
				string text5;
				DeviceFilterManager.GetActualValues(out text, out text2, out text3, out text4, out text5);
				foreach (ActiveSyncDeviceFilterRule activeSyncDeviceFilterRule in filter.Rules)
				{
					if ((activeSyncDeviceFilterRule.Characteristic == DeviceAccessCharacteristic.DeviceType && DeviceFilterManager.IsRuleValueMatch(activeSyncDeviceFilterRule, text)) || (activeSyncDeviceFilterRule.Characteristic == DeviceAccessCharacteristic.DeviceModel && DeviceFilterManager.IsRuleValueMatch(activeSyncDeviceFilterRule, text2)) || (activeSyncDeviceFilterRule.Characteristic == DeviceAccessCharacteristic.DeviceOS && DeviceFilterManager.IsRuleValueMatch(activeSyncDeviceFilterRule, text3)) || (activeSyncDeviceFilterRule.Characteristic == DeviceAccessCharacteristic.UserAgent && DeviceFilterManager.IsRuleValueMatch(activeSyncDeviceFilterRule, text4)) || (activeSyncDeviceFilterRule.Characteristic == DeviceAccessCharacteristic.XMSWLHeader && DeviceFilterManager.IsRuleValueMatch(activeSyncDeviceFilterRule, text5)))
					{
						AirSyncDiagnostics.TraceDebug(ExTraceGlobals.RequestsTracer, null, "The current request with DeviceType:{0} DeviceModel:{1} DeviceOS:{2} UserAgent:{3} XMSWLHeader:{4} has a matching filter:{5}", new object[]
						{
							text,
							text2,
							text3,
							text4,
							text5,
							filterName
						});
						return true;
					}
				}
				return false;
			}
			return false;
		}

		private static void GetActualValues(out string actualDeviceType, out string actualDeviceModel, out string actualDeviceOS, out string actualUserAgent, out string actualXMSWLHeader)
		{
			actualDeviceType = null;
			actualDeviceModel = null;
			actualDeviceOS = null;
			actualUserAgent = null;
			actualXMSWLHeader = null;
			if (Command.CurrentCommand.GlobalInfo != null)
			{
				actualDeviceModel = Command.CurrentCommand.GlobalInfo.DeviceModel;
				actualDeviceOS = Command.CurrentCommand.GlobalInfo.DeviceOS;
			}
			if (Command.CurrentCommand.DeviceIdentity != null)
			{
				actualDeviceType = Command.CurrentCommand.DeviceIdentity.DeviceType;
			}
			if (Command.CurrentCommand.Request != null)
			{
				actualUserAgent = Command.CurrentCommand.Request.UserAgent;
				actualXMSWLHeader = Command.CurrentCommand.Request.XMSWLHeader;
			}
		}

		private static bool IsRuleValueMatch(ActiveSyncDeviceFilterRule rule, string actualValue)
		{
			switch (rule.Operator)
			{
			case DeviceFilterOperator.Equals:
				return string.Equals(actualValue, rule.Value, StringComparison.OrdinalIgnoreCase);
			case DeviceFilterOperator.Contains:
				return AirSyncUtility.AreNotNullOrEmptyAndContains(actualValue, rule.Value);
			case DeviceFilterOperator.StartsWith:
				return AirSyncUtility.AreNotNullOrEmptyAndStartsWith(actualValue, rule.Value);
			case DeviceFilterOperator.Regex:
				return AirSyncUtility.AreNotNullOrEmptyAndRegexMatches(actualValue, rule.Value, RegexOptions.IgnoreCase | RegexOptions.Compiled | RegexOptions.CultureInvariant);
			default:
				throw new DeviceFilterInvalidException(EASServerStrings.InvalidDeviceFilterOperatorError(rule.Operator.ToString()));
			}
		}

		private static ActiveSyncDeviceFilter GetFilter(this Dictionary<string, ActiveSyncDeviceFilter> filters, string filterName)
		{
			if (filters != null && filters.ContainsKey(filterName))
			{
				return filters[filterName];
			}
			return null;
		}
	}
}
