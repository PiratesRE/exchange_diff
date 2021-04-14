using System;
using System.Globalization;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.HttpProxy
{
	internal static class BackEndCookieEntryParser
	{
		public static BackEndCookieEntryBase Parse(string entryValue)
		{
			BackEndCookieEntryBase result = null;
			if (!BackEndCookieEntryParser.TryParse(entryValue, out result))
			{
				throw new InvalidBackEndCookieException();
			}
			return result;
		}

		public static bool TryParse(string entryValue, out BackEndCookieEntryBase cookieEntry)
		{
			string text = null;
			return BackEndCookieEntryParser.TryParse(entryValue, out cookieEntry, out text);
		}

		public static bool TryParse(string entryValue, out BackEndCookieEntryBase cookieEntry, out string clearCookie)
		{
			cookieEntry = null;
			clearCookie = null;
			if (string.IsNullOrEmpty(entryValue))
			{
				return false;
			}
			bool result;
			try
			{
				string text = BackEndCookieEntryParser.UnObscurify(entryValue);
				clearCookie = text;
				string[] array = text.Split(BackEndCookieEntryParser.CookieSeparators);
				if (array.Length < 4)
				{
					result = false;
				}
				else
				{
					BackEndCookieEntryType backEndCookieEntryType;
					if (!BackEndCookieEntryBase.TryGetBackEndCookieEntryTypeFromString(array[0], out backEndCookieEntryType))
					{
						backEndCookieEntryType = (BackEndCookieEntryType)Enum.Parse(typeof(BackEndCookieEntryType), array[0], true);
					}
					ExDateTime expiryTime;
					if (!BackEndCookieEntryParser.TryParseDateTime(array[3], out expiryTime))
					{
						result = false;
					}
					else
					{
						switch (backEndCookieEntryType)
						{
						case BackEndCookieEntryType.Server:
							cookieEntry = new BackEndServerCookieEntry(array[1], int.Parse(array[2]), expiryTime);
							result = true;
							break;
						case BackEndCookieEntryType.Database:
						{
							Guid database = new Guid(array[1]);
							string text2 = string.IsNullOrEmpty(array[2]) ? null : array[2];
							if (array.Length == 5)
							{
								string resourceForest = string.IsNullOrEmpty(array[4]) ? null : array[4];
								cookieEntry = new BackEndDatabaseResourceForestCookieEntry(database, text2, resourceForest, expiryTime);
							}
							else
							{
								cookieEntry = new BackEndDatabaseCookieEntry(database, text2, expiryTime);
							}
							result = true;
							break;
						}
						default:
							result = false;
							break;
						}
					}
				}
			}
			catch (ArgumentException)
			{
				result = false;
			}
			catch (FormatException)
			{
				result = false;
			}
			return result;
		}

		internal static string UnObscurify(string obscureString)
		{
			byte[] array = Convert.FromBase64String(obscureString);
			byte[] array2 = new byte[array.Length];
			for (int i = 0; i < array.Length; i++)
			{
				byte[] array3 = array2;
				int num = i;
				byte[] array4 = array;
				int num2 = i;
				array3[num] = (array4[num2] ^= BackEndCookieEntryBase.ObfuscateValue);
			}
			return BackEndCookieEntryBase.Encoding.GetString(array2);
		}

		private static bool TryParseDateTime(string dateTimeString, out ExDateTime dateTime)
		{
			if (!string.IsNullOrEmpty(dateTimeString))
			{
				try
				{
					dateTime = ExDateTime.Parse(dateTimeString);
					return true;
				}
				catch (ArgumentException)
				{
				}
				catch (FormatException)
				{
				}
				try
				{
					dateTime = ExDateTime.Parse(dateTimeString, CultureInfo.InvariantCulture);
					return true;
				}
				catch (ArgumentException)
				{
				}
				catch (FormatException)
				{
				}
			}
			dateTime = default(ExDateTime);
			return false;
		}

		private static readonly char[] CookieSeparators = new char[]
		{
			'~'
		};
	}
}
