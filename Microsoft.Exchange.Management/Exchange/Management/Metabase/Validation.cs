using System;
using System.Globalization;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.Metabase
{
	internal sealed class Validation
	{
		internal static void ValidateName(string name)
		{
			if (name.Length < 1 || name.Length > 260)
			{
				throw new IisTasksValidationStringLengthOutOfRangeException(Strings.IisTasksNameValidationProperty, 1, 260);
			}
			Validation.ValidateUnicode(name, Strings.IisTasksNameValidationProperty);
		}

		internal static void ValidateWebSite(string webSite)
		{
			if (webSite.Length < 1 || webSite.Length > 260)
			{
				throw new IisTasksValidationStringLengthOutOfRangeException(Strings.IisTasksWebSiteValidationProperty, 1, 260);
			}
			Validation.ValidateUnicode(webSite, Strings.IisTasksWebSiteValidationProperty);
		}

		internal static void ValidateVirtualDirectory(string virtualDirectory)
		{
			if (virtualDirectory.Length < 1 || virtualDirectory.Length > 240)
			{
				throw new IisTasksValidationStringLengthOutOfRangeException(Strings.IisTasksVirtualDirectoryValidationProperty, 1, 240);
			}
			Validation.ValidateUnicode(virtualDirectory, Strings.IisTasksVirtualDirectoryValidationProperty);
			int num = virtualDirectory.IndexOfAny(Validation.invalidVirtualDirectoryChars);
			if (num != -1)
			{
				throw new IisTasksValidationInvalidVirtualDirectoryCharException(virtualDirectory, virtualDirectory[num], num, Validation.invalidVirtualDirectoryChars);
			}
		}

		internal static void ValidateApplicationRoot(string applicationRoot)
		{
			if (applicationRoot.Length < 1 || applicationRoot.Length > 10000)
			{
				throw new IisTasksValidationStringLengthOutOfRangeException(Strings.IisTasksApplicationRootValidationProperty, 1, 10000);
			}
			Validation.ValidateUnicode(applicationRoot, Strings.IisTasksApplicationRootValidationProperty);
		}

		internal static void ValidateApplicationPool(string applicationPool)
		{
			if (applicationPool.Length < 1 || applicationPool.Length > 259)
			{
				throw new IisTasksValidationStringLengthOutOfRangeException(Strings.IisTasksApplicationPoolValidationProperty, 1, 259);
			}
			Validation.ValidateUnicode(applicationPool, Strings.IisTasksApplicationPoolValidationProperty);
		}

		private static void ValidateUnicode(string s, LocalizedString propertyName)
		{
			for (int i = 0; i < s.Length; i++)
			{
				for (int j = 0; j < Validation.invalidCategories.Length; j++)
				{
					if (char.GetUnicodeCategory(s, i) == Validation.invalidCategories[j])
					{
						throw new IisTasksValidationInvalidUnicodeException(propertyName, s, s[i], (int)s[i], i);
					}
				}
			}
		}

		internal const int MinNameLength = 1;

		internal const int MaxNameLength = 260;

		internal const int MinWebSiteLength = 1;

		internal const int MaxWebSiteLength = 260;

		internal const int MinVirtualDirectoryLength = 1;

		internal const int MaxVirtualDirectoryLength = 240;

		internal const int MinApplicationPoolNameLength = 1;

		internal const int MaxApplicationPoolNameLength = 259;

		internal const int MinApplicationRootLength = 1;

		internal const int MaxApplicationRootLength = 10000;

		private static readonly char[] invalidVirtualDirectoryChars = "/?\\%*".ToCharArray();

		private static readonly UnicodeCategory[] invalidCategories = new UnicodeCategory[]
		{
			UnicodeCategory.Control,
			UnicodeCategory.LineSeparator,
			UnicodeCategory.ParagraphSeparator
		};
	}
}
