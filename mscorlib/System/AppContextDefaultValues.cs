using System;
using System.Runtime.Versioning;
using System.Security;
using System.Text;
using Microsoft.Win32;
using Microsoft.Win32.SafeHandles;

namespace System
{
	internal static class AppContextDefaultValues
	{
		public static void PopulateDefaultValues()
		{
			string platformIdentifier;
			string profile;
			int version;
			AppContextDefaultValues.ParseTargetFrameworkName(out platformIdentifier, out profile, out version);
			AppContextDefaultValues.PopulateDefaultValuesPartial(platformIdentifier, profile, version);
		}

		private static void ParseTargetFrameworkName(out string identifier, out string profile, out int version)
		{
			string targetFrameworkName = AppDomain.CurrentDomain.SetupInformation.TargetFrameworkName;
			if (!AppContextDefaultValues.TryParseFrameworkName(targetFrameworkName, out identifier, out version, out profile))
			{
				identifier = ".NETFramework";
				version = 40000;
				profile = string.Empty;
			}
		}

		private static bool TryParseFrameworkName(string frameworkName, out string identifier, out int version, out string profile)
		{
			string empty;
			profile = (empty = string.Empty);
			identifier = empty;
			version = 0;
			if (frameworkName == null || frameworkName.Length == 0)
			{
				return false;
			}
			string[] array = frameworkName.Split(new char[]
			{
				','
			});
			version = 0;
			if (array.Length < 2 || array.Length > 3)
			{
				return false;
			}
			identifier = array[0].Trim();
			if (identifier.Length == 0)
			{
				return false;
			}
			bool result = false;
			profile = null;
			for (int i = 1; i < array.Length; i++)
			{
				string[] array2 = array[i].Split(new char[]
				{
					'='
				});
				if (array2.Length != 2)
				{
					return false;
				}
				string text = array2[0].Trim();
				string text2 = array2[1].Trim();
				if (text.Equals("Version", StringComparison.OrdinalIgnoreCase))
				{
					result = true;
					if (text2.Length > 0 && (text2[0] == 'v' || text2[0] == 'V'))
					{
						text2 = text2.Substring(1);
					}
					Version version2 = new Version(text2);
					version = version2.Major * 10000;
					if (version2.Minor > 0)
					{
						version += version2.Minor * 100;
					}
					if (version2.Build > 0)
					{
						version += version2.Build;
					}
				}
				else
				{
					if (!text.Equals("Profile", StringComparison.OrdinalIgnoreCase))
					{
						return false;
					}
					if (!string.IsNullOrEmpty(text2))
					{
						profile = text2;
					}
				}
			}
			return result;
		}

		[SecuritySafeCritical]
		private static void TryGetSwitchOverridePartial(string switchName, ref bool overrideFound, ref bool overrideValue)
		{
			string text = null;
			overrideFound = false;
			if (!AppContextDefaultValues.s_errorReadingRegistry)
			{
				text = AppContextDefaultValues.GetSwitchValueFromRegistry(switchName);
			}
			if (text == null)
			{
				text = CompatibilitySwitch.GetValue(switchName);
			}
			bool flag;
			if (text != null && bool.TryParse(text, out flag))
			{
				overrideValue = flag;
				overrideFound = true;
			}
		}

		private static void PopulateDefaultValuesPartial(string platformIdentifier, string profile, int version)
		{
			if (!(platformIdentifier == ".NETCore") && !(platformIdentifier == ".NETFramework"))
			{
				if (platformIdentifier == "WindowsPhone" || platformIdentifier == "WindowsPhoneApp")
				{
					if (version <= 80100)
					{
						AppContext.DefineSwitchDefault(AppContextDefaultValues.SwitchNoAsyncCurrentCulture, true);
						AppContext.DefineSwitchDefault(AppContextDefaultValues.SwitchThrowExceptionIfDisposedCancellationTokenSource, true);
						AppContext.DefineSwitchDefault(AppContextDefaultValues.SwitchUseLegacyPathHandling, true);
						AppContext.DefineSwitchDefault(AppContextDefaultValues.SwitchBlockLongPaths, true);
						AppContext.DefineSwitchDefault(AppContextDefaultValues.SwitchDoNotAddrOfCspParentWindowHandle, true);
						AppContext.DefineSwitchDefault(AppContextDefaultValues.SwitchIgnorePortablePDBsInStackTraces, true);
					}
				}
			}
			else
			{
				if (version <= 40502)
				{
					AppContext.DefineSwitchDefault(AppContextDefaultValues.SwitchNoAsyncCurrentCulture, true);
					AppContext.DefineSwitchDefault(AppContextDefaultValues.SwitchThrowExceptionIfDisposedCancellationTokenSource, true);
				}
				if (version <= 40601)
				{
					AppContext.DefineSwitchDefault(AppContextDefaultValues.SwitchUseLegacyPathHandling, true);
					AppContext.DefineSwitchDefault(AppContextDefaultValues.SwitchBlockLongPaths, true);
					AppContext.DefineSwitchDefault(AppContextDefaultValues.SwitchSetActorAsReferenceWhenCopyingClaimsIdentity, true);
				}
				if (version <= 40602)
				{
					AppContext.DefineSwitchDefault(AppContextDefaultValues.SwitchDoNotAddrOfCspParentWindowHandle, true);
				}
				if (version <= 40701)
				{
					AppContext.DefineSwitchDefault(AppContextDefaultValues.SwitchIgnorePortablePDBsInStackTraces, true);
				}
				if (version <= 40702)
				{
					AppContext.DefineSwitchDefault(AppContextDefaultValues.SwitchCryptographyUseLegacyFipsThrow, true);
					AppContext.DefineSwitchDefault(AppContextDefaultValues.SwitchDoNotMarshalOutByrefSafeArrayOnInvoke, true);
				}
			}
			AppContextDefaultValues.PopulateOverrideValuesPartial();
		}

		[SecuritySafeCritical]
		private static void PopulateOverrideValuesPartial()
		{
			string appContextOverridesInternalCall = CompatibilitySwitch.GetAppContextOverridesInternalCall();
			if (string.IsNullOrEmpty(appContextOverridesInternalCall))
			{
				return;
			}
			bool flag = false;
			bool flag2 = false;
			bool flag3 = false;
			int num = -1;
			int num2 = -1;
			for (int i = 0; i <= appContextOverridesInternalCall.Length; i++)
			{
				if (i == appContextOverridesInternalCall.Length || appContextOverridesInternalCall[i] == ';')
				{
					if (flag && flag2 && flag3)
					{
						int startIndex = num + 1;
						int length = num2 - num - 1;
						string switchName = appContextOverridesInternalCall.Substring(startIndex, length);
						int startIndex2 = num2 + 1;
						int length2 = i - num2 - 1;
						string value = appContextOverridesInternalCall.Substring(startIndex2, length2);
						bool isEnabled;
						if (bool.TryParse(value, out isEnabled))
						{
							AppContext.DefineSwitchOverride(switchName, isEnabled);
						}
					}
					num = i;
					flag3 = (flag2 = (flag = false));
				}
				else if (appContextOverridesInternalCall[i] == '=')
				{
					if (!flag)
					{
						flag = true;
						num2 = i;
					}
				}
				else if (flag)
				{
					flag3 = true;
				}
				else
				{
					flag2 = true;
				}
			}
		}

		public static bool TryGetSwitchOverride(string switchName, out bool overrideValue)
		{
			overrideValue = false;
			bool result = false;
			AppContextDefaultValues.TryGetSwitchOverridePartial(switchName, ref result, ref overrideValue);
			return result;
		}

		[SecuritySafeCritical]
		private static string GetSwitchValueFromRegistry(string switchName)
		{
			try
			{
				using (SafeRegistryHandle safeRegistryHandle = new SafeRegistryHandle((IntPtr)(-2147483646), true))
				{
					SafeRegistryHandle hKey = null;
					if (Win32Native.RegOpenKeyEx(safeRegistryHandle, "SOFTWARE\\Microsoft\\.NETFramework\\AppContext", 0, 131097, out hKey) == 0)
					{
						int capacity = 12;
						int num = 0;
						StringBuilder stringBuilder = new StringBuilder(capacity);
						if (Win32Native.RegQueryValueEx(hKey, switchName, null, ref num, stringBuilder, ref capacity) == 0)
						{
							return stringBuilder.ToString();
						}
					}
					else
					{
						AppContextDefaultValues.s_errorReadingRegistry = true;
					}
				}
			}
			catch
			{
				AppContextDefaultValues.s_errorReadingRegistry = true;
			}
			return null;
		}

		internal static readonly string SwitchNoAsyncCurrentCulture = "Switch.System.Globalization.NoAsyncCurrentCulture";

		internal static readonly string SwitchEnforceJapaneseEraYearRanges = "Switch.System.Globalization.EnforceJapaneseEraYearRanges";

		internal static readonly string SwitchFormatJapaneseFirstYearAsANumber = "Switch.System.Globalization.FormatJapaneseFirstYearAsANumber";

		internal static readonly string SwitchEnforceLegacyJapaneseDateParsing = "Switch.System.Globalization.EnforceLegacyJapaneseDateParsing";

		internal static readonly string SwitchThrowExceptionIfDisposedCancellationTokenSource = "Switch.System.Threading.ThrowExceptionIfDisposedCancellationTokenSource";

		internal static readonly string SwitchPreserveEventListnerObjectIdentity = "Switch.System.Diagnostics.EventSource.PreserveEventListnerObjectIdentity";

		internal static readonly string SwitchUseLegacyPathHandling = "Switch.System.IO.UseLegacyPathHandling";

		internal static readonly string SwitchBlockLongPaths = "Switch.System.IO.BlockLongPaths";

		internal static readonly string SwitchDoNotAddrOfCspParentWindowHandle = "Switch.System.Security.Cryptography.DoNotAddrOfCspParentWindowHandle";

		internal static readonly string SwitchSetActorAsReferenceWhenCopyingClaimsIdentity = "Switch.System.Security.ClaimsIdentity.SetActorAsReferenceWhenCopyingClaimsIdentity";

		internal static readonly string SwitchIgnorePortablePDBsInStackTraces = "Switch.System.Diagnostics.IgnorePortablePDBsInStackTraces";

		internal static readonly string SwitchUseNewMaxArraySize = "Switch.System.Runtime.Serialization.UseNewMaxArraySize";

		internal static readonly string SwitchUseConcurrentFormatterTypeCache = "Switch.System.Runtime.Serialization.UseConcurrentFormatterTypeCache";

		internal static readonly string SwitchUseLegacyExecutionContextBehaviorUponUndoFailure = "Switch.System.Threading.UseLegacyExecutionContextBehaviorUponUndoFailure";

		internal static readonly string SwitchCryptographyUseLegacyFipsThrow = "Switch.System.Security.Cryptography.UseLegacyFipsThrow";

		internal static readonly string SwitchDoNotMarshalOutByrefSafeArrayOnInvoke = "Switch.System.Runtime.InteropServices.DoNotMarshalOutByrefSafeArrayOnInvoke";

		internal static readonly string SwitchUseNetCoreTimer = "Switch.System.Threading.UseNetCoreTimer";

		private static volatile bool s_errorReadingRegistry;
	}
}
