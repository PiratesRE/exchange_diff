using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;

namespace Microsoft.Exchange.Clients.Owa.Core
{
	public static class LocalizedStrings
	{
		public static string GetHtmlEncoded(Strings.IDs localizedID)
		{
			string name = Culture.GetUserCulture().Name;
			return LocalizedStrings.GetHtmlEncodedInternal(name, localizedID);
		}

		public static string GetHtmlEncoded(Strings.IDs localizedID, CultureInfo userCulture)
		{
			string name = userCulture.Name;
			return LocalizedStrings.GetHtmlEncodedInternal(name, localizedID);
		}

		public static string GetHtmlEncodedFromKey(string key, Strings.IDs localizedId)
		{
			return LocalizedStrings.GetHtmlEncodedInternal(key, localizedId);
		}

		public static string GetNonEncoded(Strings.IDs localizedId)
		{
			return Strings.GetLocalizedString(localizedId);
		}

		public static string GetJavascriptEncoded(Strings.IDs localizedId)
		{
			string name = Culture.GetUserCulture().Name;
			return LocalizedStrings.GetJavascriptEncodedInternal(name, localizedId);
		}

		public static string GetJavascriptEncodedFromKey(string key, Strings.IDs localizedID)
		{
			return LocalizedStrings.GetJavascriptEncodedInternal(key, localizedID);
		}

		public static string GetJavascriptEncodedInternal(string key, Strings.IDs localizedID)
		{
			Dictionary<uint, string> dictionary = null;
			object obj = LocalizedStrings.javascriptEncodedStringsCollection[key];
			if (obj == null)
			{
				lock (LocalizedStrings.javascriptEncodedStringsCollection)
				{
					if (LocalizedStrings.javascriptEncodedStringsCollection[key] == null)
					{
						Strings.IDs[] array = (Strings.IDs[])Enum.GetValues(typeof(Strings.IDs));
						dictionary = new Dictionary<uint, string>(array.Length);
						for (int i = 0; i < array.Length; i++)
						{
							dictionary[array[i]] = Utilities.JavascriptEncode(Strings.GetLocalizedString(array[i]));
						}
						LocalizedStrings.javascriptEncodedStringsCollection[key] = dictionary;
					}
					else
					{
						dictionary = (Dictionary<uint, string>)LocalizedStrings.javascriptEncodedStringsCollection[key];
					}
					goto IL_A9;
				}
			}
			dictionary = (Dictionary<uint, string>)obj;
			IL_A9:
			return dictionary[localizedID];
		}

		internal static string GetHtmlEncodedInternal(string key, Strings.IDs localizedID)
		{
			Dictionary<uint, string> dictionary = null;
			object obj = LocalizedStrings.htmlEncodedStringsCollection[key];
			if (obj == null)
			{
				lock (LocalizedStrings.htmlEncodedStringsCollection)
				{
					if (LocalizedStrings.htmlEncodedStringsCollection[key] == null)
					{
						Strings.IDs[] array = (Strings.IDs[])Enum.GetValues(typeof(Strings.IDs));
						dictionary = new Dictionary<uint, string>(array.Length);
						for (int i = 0; i < array.Length; i++)
						{
							dictionary[array[i]] = Utilities.HtmlEncode(Strings.GetLocalizedString(array[i]));
						}
						LocalizedStrings.htmlEncodedStringsCollection[key] = dictionary;
					}
					else
					{
						dictionary = (Dictionary<uint, string>)LocalizedStrings.htmlEncodedStringsCollection[key];
					}
					goto IL_A9;
				}
			}
			dictionary = (Dictionary<uint, string>)obj;
			IL_A9:
			return dictionary[localizedID];
		}

		private static Hashtable htmlEncodedStringsCollection = new Hashtable();

		private static Hashtable javascriptEncodedStringsCollection = new Hashtable();
	}
}
