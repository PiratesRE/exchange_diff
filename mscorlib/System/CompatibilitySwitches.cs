using System;
using System.Runtime.CompilerServices;

namespace System
{
	[FriendAccessAllowed]
	internal static class CompatibilitySwitches
	{
		public static bool IsCompatibilityBehaviorDefined
		{
			get
			{
				return CompatibilitySwitches.s_AreSwitchesSet;
			}
		}

		private static bool IsCompatibilitySwitchSet(string compatibilitySwitch)
		{
			bool? flag = AppDomain.CurrentDomain.IsCompatibilitySwitchSet(compatibilitySwitch);
			return flag != null && flag.Value;
		}

		internal static void InitializeSwitches()
		{
			CompatibilitySwitches.s_isNetFx40TimeSpanLegacyFormatMode = CompatibilitySwitches.IsCompatibilitySwitchSet("NetFx40_TimeSpanLegacyFormatMode");
			CompatibilitySwitches.s_isNetFx40LegacySecurityPolicy = CompatibilitySwitches.IsCompatibilitySwitchSet("NetFx40_LegacySecurityPolicy");
			CompatibilitySwitches.s_isNetFx45LegacyManagedDeflateStream = CompatibilitySwitches.IsCompatibilitySwitchSet("NetFx45_LegacyManagedDeflateStream");
			CompatibilitySwitches.s_AreSwitchesSet = true;
		}

		public static bool IsAppEarlierThanSilverlight4
		{
			get
			{
				return false;
			}
		}

		public static bool IsAppEarlierThanWindowsPhone8
		{
			get
			{
				return false;
			}
		}

		public static bool IsAppEarlierThanWindowsPhoneMango
		{
			get
			{
				return false;
			}
		}

		public static bool IsNetFx40TimeSpanLegacyFormatMode
		{
			get
			{
				return CompatibilitySwitches.s_isNetFx40TimeSpanLegacyFormatMode;
			}
		}

		public static bool IsNetFx40LegacySecurityPolicy
		{
			get
			{
				return CompatibilitySwitches.s_isNetFx40LegacySecurityPolicy;
			}
		}

		public static bool IsNetFx45LegacyManagedDeflateStream
		{
			get
			{
				return CompatibilitySwitches.s_isNetFx45LegacyManagedDeflateStream;
			}
		}

		private static bool s_AreSwitchesSet;

		private static bool s_isNetFx40TimeSpanLegacyFormatMode;

		private static bool s_isNetFx40LegacySecurityPolicy;

		private static bool s_isNetFx45LegacyManagedDeflateStream;
	}
}
