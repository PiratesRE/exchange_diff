using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.MessagingPolicies.Rules
{
	internal static class TextMatchingStrings
	{
		static TextMatchingStrings()
		{
			TextMatchingStrings.stringIDs.Add(1233043159U, "RegexSyntaxForStar");
			TextMatchingStrings.stringIDs.Add(3196275601U, "RegexMismatchingParenthesis");
			TextMatchingStrings.stringIDs.Add(3281782444U, "RegexUnSupportedMetaCharacter");
			TextMatchingStrings.stringIDs.Add(1519177102U, "KeywordInternalParsingError");
			TextMatchingStrings.stringIDs.Add(1668465210U, "RegexInternalParsingError");
			TextMatchingStrings.stringIDs.Add(688185054U, "RegexSyntaxForBar");
		}

		public static LocalizedString RegexSyntaxForStar
		{
			get
			{
				return new LocalizedString("RegexSyntaxForStar", "", false, false, TextMatchingStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RegexPatternParsingError(string diagnostic)
		{
			return new LocalizedString("RegexPatternParsingError", "", false, false, TextMatchingStrings.ResourceManager, new object[]
			{
				diagnostic
			});
		}

		public static LocalizedString RegexMismatchingParenthesis
		{
			get
			{
				return new LocalizedString("RegexMismatchingParenthesis", "", false, false, TextMatchingStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RegexUnSupportedMetaCharacter
		{
			get
			{
				return new LocalizedString("RegexUnSupportedMetaCharacter", "", false, false, TextMatchingStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString KeywordInternalParsingError
		{
			get
			{
				return new LocalizedString("KeywordInternalParsingError", "", false, false, TextMatchingStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RegexInternalParsingError
		{
			get
			{
				return new LocalizedString("RegexInternalParsingError", "", false, false, TextMatchingStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RegexSyntaxForBar
		{
			get
			{
				return new LocalizedString("RegexSyntaxForBar", "", false, false, TextMatchingStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString GetLocalizedString(TextMatchingStrings.IDs key)
		{
			return new LocalizedString(TextMatchingStrings.stringIDs[(uint)key], TextMatchingStrings.ResourceManager, new object[0]);
		}

		private static Dictionary<uint, string> stringIDs = new Dictionary<uint, string>(6);

		private static ExchangeResourceManager ResourceManager = ExchangeResourceManager.GetResourceManager("Microsoft.Exchange.MessagingPolicies.Rules.TextMatchingStrings", typeof(TextMatchingStrings).GetTypeInfo().Assembly);

		public enum IDs : uint
		{
			RegexSyntaxForStar = 1233043159U,
			RegexMismatchingParenthesis = 3196275601U,
			RegexUnSupportedMetaCharacter = 3281782444U,
			KeywordInternalParsingError = 1519177102U,
			RegexInternalParsingError = 1668465210U,
			RegexSyntaxForBar = 688185054U
		}

		private enum ParamIDs
		{
			RegexPatternParsingError
		}
	}
}
