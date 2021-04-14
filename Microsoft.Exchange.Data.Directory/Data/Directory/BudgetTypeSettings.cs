using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics.Components.Data.Directory;
using Microsoft.Exchange.Net;

namespace Microsoft.Exchange.Data.Directory
{
	internal static class BudgetTypeSettings
	{
		static BudgetTypeSettings()
		{
			BudgetTypeSettings.Initialize();
		}

		internal static void Initialize()
		{
			BudgetTypeSettings.CreateDefaultBudgetTypeSettingsMap();
			BudgetTypeSettings.LoadAppConfigSettings();
		}

		public static BudgetTypeSetting Get(BudgetType budgetType)
		{
			if (BudgetTypeSettings.GetSettingsTestHook != null)
			{
				return BudgetTypeSettings.GetSettingsTestHook(budgetType);
			}
			return BudgetTypeSettings.budgetTypeSettingsMap[budgetType];
		}

		internal static BudgetTypeSetting GetDefault(BudgetType budgetType)
		{
			if (BudgetTypeSettings.GetSettingsTestHook != null)
			{
				return BudgetTypeSettings.GetSettingsTestHook(budgetType);
			}
			BudgetTypeSetting result = null;
			if (BudgetTypeSettings.defaultBudgetTypeSettingsMap.TryGetValue(budgetType, out result))
			{
				return result;
			}
			return BudgetTypeSetting.OneMinuteSetting;
		}

		private static void CreateDefaultBudgetTypeSettingsMap()
		{
			BudgetTypeSettings.defaultBudgetTypeSettingsMap = new Dictionary<BudgetType, BudgetTypeSetting>();
			BudgetTypeSettings.defaultBudgetTypeSettingsMap.Add(BudgetType.OwaVoice, new BudgetTypeSetting(TimeSpan.FromSeconds(5.0), 3, 5));
			BudgetTypeSettings.defaultBudgetTypeSettingsMap.Add(BudgetType.Rca, new BudgetTypeSetting(TimeSpan.FromSeconds(5.0), 3, 5));
		}

		internal static string BuildMicroDelayMultiplierKey(BudgetType budgetType)
		{
			return string.Format("{0}.{1}", budgetType, "MaxMicroDelayMultiplier");
		}

		internal static string BuildMaxDelayKey(BudgetType budgetType)
		{
			return string.Format("{0}.{1}", budgetType, "MaxDelayInSeconds");
		}

		internal static string BuildMaxDelayedThreadsKey(BudgetType budgetType)
		{
			return string.Format("{0}.{1}", budgetType, "MaxDelayedThreadsPerProcessor");
		}

		private static void LoadAppConfigSettings()
		{
			BudgetTypeSettings.budgetTypeSettingsMap = new Dictionary<BudgetType, BudgetTypeSetting>();
			foreach (object obj in Enum.GetValues(typeof(BudgetType)))
			{
				BudgetType budgetType = (BudgetType)obj;
				string name = BudgetTypeSettings.BuildMicroDelayMultiplierKey(budgetType);
				string name2 = BudgetTypeSettings.BuildMaxDelayKey(budgetType);
				string name3 = BudgetTypeSettings.BuildMaxDelayedThreadsKey(budgetType);
				BudgetTypeSetting @default = BudgetTypeSettings.GetDefault(budgetType);
				IntAppSettingsEntry intAppSettingsEntry = new IntAppSettingsEntry(name, @default.MaxMicroDelayMultiplier, ExTraceGlobals.BudgetDelayTracer);
				TimeSpanAppSettingsEntry timeSpanAppSettingsEntry = new TimeSpanAppSettingsEntry(name2, TimeSpanUnit.Seconds, @default.MaxDelay, ExTraceGlobals.BudgetDelayTracer);
				IntAppSettingsEntry intAppSettingsEntry2 = new IntAppSettingsEntry(name3, @default.MaxDelayedThreadPerProcessor, ExTraceGlobals.BudgetDelayTracer);
				BudgetTypeSetting value = new BudgetTypeSetting(timeSpanAppSettingsEntry.Value, intAppSettingsEntry.Value, intAppSettingsEntry2.Value);
				BudgetTypeSettings.budgetTypeSettingsMap[budgetType] = value;
			}
		}

		public const string MaxMicroDelayMultiplierValueName = "MaxMicroDelayMultiplier";

		public const string MaxDelayValueName = "MaxDelayInSeconds";

		public const string MaxDelayedThreadsValuePerProcessorName = "MaxDelayedThreadsPerProcessor";

		public static Func<BudgetType, BudgetTypeSetting> GetSettingsTestHook = null;

		private static Dictionary<BudgetType, BudgetTypeSetting> defaultBudgetTypeSettingsMap;

		private static Dictionary<BudgetType, BudgetTypeSetting> budgetTypeSettingsMap;
	}
}
