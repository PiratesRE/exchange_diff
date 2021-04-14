using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Management.Tasks;
using Microsoft.Exchange.Win32;
using Microsoft.Win32;

namespace Microsoft.Exchange.Management.TenantMonitoring
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal static class Utils
	{
		internal static EventLogEntryType ExtractEntryTypeFromInstanceId(uint eventId)
		{
			uint num = eventId & 3221225472U;
			if (num <= 1073741824U)
			{
				if (num != 0U && num != 1073741824U)
				{
				}
			}
			else
			{
				if (num == 2147483648U)
				{
					return EventLogEntryType.Warning;
				}
				if (num == 3221225472U)
				{
					return EventLogEntryType.Error;
				}
			}
			return EventLogEntryType.Information;
		}

		internal static int ExtractCodeFromEventIdentifier(int eventId)
		{
			return (int)((long)eventId & 65535L);
		}

		internal static string GetResourcesFilePath(string eventSource)
		{
			string name = Path.Combine("SYSTEM\\CurrentControlSet\\Services\\Eventlog\\Application", eventSource);
			string result;
			using (RegistryKey registryKey = Registry.LocalMachine.OpenSubKey(name, false))
			{
				if (registryKey == null)
				{
					result = string.Empty;
				}
				else
				{
					string text = (string)registryKey.GetValue("EventMessageFile", string.Empty);
					if (string.IsNullOrEmpty(text))
					{
						result = string.Empty;
					}
					else
					{
						string[] array = text.Split(new char[]
						{
							';'
						}, StringSplitOptions.RemoveEmptyEntries);
						if (array == null || array.Length == 0)
						{
							result = string.Empty;
						}
						else
						{
							result = array[0];
						}
					}
				}
			}
			return result;
		}

		internal static string GetLocalizedEventMessageAndCategory(string resourcesFilePath, uint messageId, uint categoryId, IList<string> insertionStrings, CultureInfo language, TextWriter debugWriter, out string localizedCategory)
		{
			if (string.IsNullOrEmpty(resourcesFilePath))
			{
				throw new ArgumentNullException("resourcesFilePath");
			}
			if (debugWriter == null)
			{
				throw new ArgumentNullException("debugWriter");
			}
			string result;
			using (SafeLibraryHandle safeLibraryHandle = NativeMethods.LoadLibraryEx(resourcesFilePath, IntPtr.Zero, 34U))
			{
				if (safeLibraryHandle == null || safeLibraryHandle.IsInvalid)
				{
					debugWriter.WriteLine(Strings.TenantNotificationDebugLoadLibraryFailed(resourcesFilePath, Marshal.GetLastWin32Error()));
					localizedCategory = string.Empty;
					result = string.Empty;
				}
				else
				{
					localizedCategory = Utils.GetLocalizedEventMessage(safeLibraryHandle, categoryId, null, language, debugWriter);
					result = Utils.GetLocalizedEventMessage(safeLibraryHandle, messageId, insertionStrings, language, debugWriter);
				}
			}
			return result;
		}

		private static string GetLocalizedEventMessage(SafeLibraryHandle resourcesModule, uint messageId, IList<string> insertionStrings, CultureInfo language, TextWriter debugWriter)
		{
			if (resourcesModule == null)
			{
				throw new ArgumentNullException("resourcesModule");
			}
			if (resourcesModule.IsInvalid)
			{
				throw new ArgumentException("invalid resources module handle.", "resourcesModule");
			}
			if (debugWriter == null)
			{
				throw new ArgumentNullException("debugWriter");
			}
			uint num = 2304U;
			string[] array = new string[100];
			if (insertionStrings == null || insertionStrings.Count == 0)
			{
				num |= 512U;
			}
			else
			{
				num |= 8192U;
				for (int i = 0; i < insertionStrings.Count; i++)
				{
					array[i] = insertionStrings[i];
				}
			}
			StringBuilder stringBuilder;
			int num2 = NativeMethods.FormatMessage(num, resourcesModule, messageId, (uint)((language != null) ? language.LCID : 0), out stringBuilder, 0U, array);
			if (num2 == 0 || stringBuilder == null)
			{
				if (language != null && (long)Marshal.GetLastWin32Error() == 1815L)
				{
					debugWriter.WriteLine(Strings.TenantNotificationDebugFormatMessageFailed(language));
					num2 = NativeMethods.FormatMessage(num, resourcesModule, messageId, 0U, out stringBuilder, 0U, array);
					if (num2 != 0 && stringBuilder != null)
					{
						return stringBuilder.ToString().TrimEnd(Utils.CharsToTrim);
					}
				}
				debugWriter.WriteLine(Strings.TenantNotificationDebugFormatMessageFailedSystemLang(Marshal.GetLastWin32Error()));
				return string.Empty;
			}
			return stringBuilder.ToString().TrimEnd(Utils.CharsToTrim);
		}

		internal const string FallbackHelpUrl = "http://help.outlook.com/ms.exch.evt.default.aspx";

		private const uint SeverityMask = 3221225472U;

		private const uint CodeMask = 65535U;

		private const uint SuccessSeverity = 0U;

		private const uint InformationalSeverity = 1073741824U;

		private const uint WarningSeverity = 2147483648U;

		private const uint ErrorSeverity = 3221225472U;

		private static readonly char[] CharsToTrim = new char[]
		{
			'\r',
			'\n',
			' '
		};
	}
}
