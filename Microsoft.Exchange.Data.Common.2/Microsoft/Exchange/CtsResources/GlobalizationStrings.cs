using System;
using System.Collections.Generic;
using System.Reflection;
using System.Resources;

namespace Microsoft.Exchange.CtsResources
{
	internal static class GlobalizationStrings
	{
		static GlobalizationStrings()
		{
			GlobalizationStrings.stringIDs.Add(1308081499U, "MaxCharactersCannotBeNegative");
			GlobalizationStrings.stringIDs.Add(1590522975U, "CountOutOfRange");
			GlobalizationStrings.stringIDs.Add(1083457927U, "PriorityListIncludesNonDetectableCodePage");
			GlobalizationStrings.stringIDs.Add(3590683541U, "OffsetOutOfRange");
			GlobalizationStrings.stringIDs.Add(1226301788U, "IndexOutOfRange");
			GlobalizationStrings.stringIDs.Add(2746482960U, "CountTooLarge");
		}

		public static string InvalidCodePage(int codePage)
		{
			return string.Format(GlobalizationStrings.ResourceManager.GetString("InvalidCodePage"), codePage);
		}

		public static string NotInstalledCodePage(int codePage)
		{
			return string.Format(GlobalizationStrings.ResourceManager.GetString("NotInstalledCodePage"), codePage);
		}

		public static string MaxCharactersCannotBeNegative
		{
			get
			{
				return GlobalizationStrings.ResourceManager.GetString("MaxCharactersCannotBeNegative");
			}
		}

		public static string CountOutOfRange
		{
			get
			{
				return GlobalizationStrings.ResourceManager.GetString("CountOutOfRange");
			}
		}

		public static string PriorityListIncludesNonDetectableCodePage
		{
			get
			{
				return GlobalizationStrings.ResourceManager.GetString("PriorityListIncludesNonDetectableCodePage");
			}
		}

		public static string OffsetOutOfRange
		{
			get
			{
				return GlobalizationStrings.ResourceManager.GetString("OffsetOutOfRange");
			}
		}

		public static string InvalidCultureName(string cultureName)
		{
			return string.Format(GlobalizationStrings.ResourceManager.GetString("InvalidCultureName"), cultureName);
		}

		public static string NotInstalledCharsetCodePage(int codePage, string charsetName)
		{
			return string.Format(GlobalizationStrings.ResourceManager.GetString("NotInstalledCharsetCodePage"), codePage, charsetName);
		}

		public static string IndexOutOfRange
		{
			get
			{
				return GlobalizationStrings.ResourceManager.GetString("IndexOutOfRange");
			}
		}

		public static string CountTooLarge
		{
			get
			{
				return GlobalizationStrings.ResourceManager.GetString("CountTooLarge");
			}
		}

		public static string NotInstalledCharset(string charsetName)
		{
			return string.Format(GlobalizationStrings.ResourceManager.GetString("NotInstalledCharset"), charsetName);
		}

		public static string InvalidLocaleId(int localeId)
		{
			return string.Format(GlobalizationStrings.ResourceManager.GetString("InvalidLocaleId"), localeId);
		}

		public static string InvalidCharset(string charsetName)
		{
			return string.Format(GlobalizationStrings.ResourceManager.GetString("InvalidCharset"), charsetName);
		}

		public static string GetLocalizedString(GlobalizationStrings.IDs key)
		{
			return GlobalizationStrings.ResourceManager.GetString(GlobalizationStrings.stringIDs[(uint)key]);
		}

		private static Dictionary<uint, string> stringIDs = new Dictionary<uint, string>(6);

		private static ResourceManager ResourceManager = new ResourceManager("Microsoft.Exchange.CtsResources.GlobalizationStrings", typeof(GlobalizationStrings).GetTypeInfo().Assembly);

		public enum IDs : uint
		{
			MaxCharactersCannotBeNegative = 1308081499U,
			CountOutOfRange = 1590522975U,
			PriorityListIncludesNonDetectableCodePage = 1083457927U,
			OffsetOutOfRange = 3590683541U,
			IndexOutOfRange = 1226301788U,
			CountTooLarge = 2746482960U
		}

		private enum ParamIDs
		{
			InvalidCodePage,
			NotInstalledCodePage,
			InvalidCultureName,
			NotInstalledCharsetCodePage,
			NotInstalledCharset,
			InvalidLocaleId,
			InvalidCharset
		}
	}
}
