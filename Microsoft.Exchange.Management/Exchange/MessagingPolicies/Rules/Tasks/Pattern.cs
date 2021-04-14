using System;
using System.Linq;
using System.Runtime.Serialization;
using System.Security.Permissions;
using System.Text.RegularExpressions;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Management.Tasks;
using Microsoft.Exchange.TextMatching;
using Microsoft.Exchange.TextProcessing;

namespace Microsoft.Exchange.MessagingPolicies.Rules.Tasks
{
	[Serializable]
	public struct Pattern : IComparable, ISerializable
	{
		public Pattern(string input)
		{
			this = default(Pattern);
			this.isLegacyRegex = true;
			this.ignoreMaxLength = false;
			if (this.IsValid(input))
			{
				try
				{
					Pattern.ValidatePattern(input, this.isLegacyRegex, false);
					this.value = input;
				}
				catch (ArgumentException)
				{
				}
				if (string.IsNullOrEmpty(this.value))
				{
					this.isLegacyRegex = false;
					Pattern.ValidatePattern(input, this.isLegacyRegex, false);
				}
				this.value = input;
				return;
			}
			throw new ValidationArgumentException(Strings.ErrorPatternIsTooLong(input, 128), null);
		}

		private Pattern(SerializationInfo info, StreamingContext context)
		{
			this = new Pattern((string)info.GetValue("value", typeof(string)), false, false);
		}

		internal Pattern(string input, bool useLegacyRegex, bool ignoreMaxLength = false)
		{
			this = default(Pattern);
			this.isLegacyRegex = useLegacyRegex;
			this.ignoreMaxLength = ignoreMaxLength;
			if (this.IsValid(input))
			{
				Pattern.ValidatePattern(input, this.isLegacyRegex, false);
				this.value = input;
				return;
			}
			throw new ValidationArgumentException(Strings.ErrorPatternIsTooLong(input, 128), null);
		}

		public static Pattern Empty
		{
			get
			{
				return default(Pattern);
			}
		}

		public string Value
		{
			get
			{
				if (this.IsValid(this.value))
				{
					Pattern.ValidatePattern(this.value, this.isLegacyRegex, false);
					return this.value;
				}
				throw new ValidationArgumentException(Strings.ErrorPatternIsTooLong(this.value, 128), null);
			}
		}

		public static Pattern Parse(string s, bool useLegacyRegex = true)
		{
			return new Pattern(s, useLegacyRegex, false);
		}

		public static bool operator ==(Pattern a, Pattern b)
		{
			return a.Value == b.Value;
		}

		public static bool operator !=(Pattern a, Pattern b)
		{
			return a.Value != b.Value;
		}

		[SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.SerializationFormatter)]
		[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
		public void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			info.AddValue("value", this.value);
		}

		public override string ToString()
		{
			if (this.value == null)
			{
				return string.Empty;
			}
			return this.value;
		}

		public override int GetHashCode()
		{
			if (this.value == null)
			{
				return string.Empty.GetHashCode();
			}
			return this.value.GetHashCode();
		}

		public override bool Equals(object obj)
		{
			return obj is Pattern && this.Equals((Pattern)obj);
		}

		public bool Equals(Pattern obj)
		{
			return this.value == obj.Value;
		}

		public int CompareTo(object obj)
		{
			if (!(obj is Pattern))
			{
				throw new ArgumentException("Parameter is not of type Pattern.");
			}
			return string.Compare(this.value, ((Pattern)obj).Value, StringComparison.OrdinalIgnoreCase);
		}

		private bool IsValid(string input)
		{
			return input != null && (this.ignoreMaxLength || input.Length <= 128);
		}

		public static void ValidatePattern(string input, bool useLegacyRegex, bool ignoreConstraints = false)
		{
			if (input == null)
			{
				throw new ValidationArgumentException(Strings.Pattern, null);
			}
			if (string.IsNullOrWhiteSpace(input))
			{
				throw new ValidationArgumentException(Strings.PatternIsWhiteSpace, null);
			}
			if (useLegacyRegex)
			{
				RegexParser regexParser = new RegexParser(input, true);
				try
				{
					regexParser.Parse();
					return;
				}
				catch (TextMatchingParsingException ex)
				{
					throw new ValidationArgumentException(ex.LocalizedString, null);
				}
			}
			try
			{
				new MatchFactory().CreateRegex(input, CaseSensitivityMode.Insensitive, MatchRegexOptions.ExplicitCaptures);
			}
			catch (ArgumentException innerException)
			{
				throw new ValidationArgumentException(Strings.Pattern, innerException);
			}
			if (!ignoreConstraints)
			{
				Pattern.ValidatePatternDoesNotBeginOrEndWithWildcards(input);
				Pattern.ValidatePatternDoesNotContainGroupsOrAssertionsWithWildcards(input);
				Pattern.ValidatePatternDoesNotContainMultiMatchOnGroupsOrAssertions(input);
				Pattern.ValidatePatternDoesNotHaveSequentialIdenticalMultiMatches(input);
				Pattern.ValidatePatternDoesNotContainEmptyAlternations(input);
			}
		}

		internal static void ValidatePatternDoesNotBeginOrEndWithWildcards(string input)
		{
			Regex regex = new Regex(Pattern.beginsWithWildcardMultiMatchPattern);
			Regex regex2 = new Regex("^\\^\\.(\\*|\\+)");
			if (regex.IsMatch(input) || regex2.IsMatch(input))
			{
				throw new ValidationArgumentException(Strings.ErrorPatternCannotBeginWithWildcardMultiMatch, null);
			}
			Regex regex3 = new Regex(Pattern.endsWithWildcardMultiMatchPattern);
			Regex regex4 = new Regex(Pattern.endsWithWildcardMultiMatchAnchoredPattern);
			Match match = regex3.Match(input);
			Match match2 = regex4.Match(input);
			if ((match.Success && !Pattern.IsSequenceLengthOdd(match.Groups["LeadingBackslashes"].Value)) || (match2.Success && !Pattern.IsSequenceLengthOdd(match2.Groups["LeadingBackslashes"].Value)))
			{
				throw new ValidationArgumentException(Strings.ErrorPatternCannotEndWithWildcardMultiMatch, null);
			}
		}

		internal static void ValidatePatternDoesNotContainGroupsOrAssertionsWithWildcards(string input)
		{
			if (Regex.IsMatch(Pattern.StripBackslashPairs(input), Pattern.groupOrAssertionContainsMultiMatchWildcard))
			{
				throw new ValidationArgumentException(Strings.ErrorPatternCannotContainGroupOrAssertionWithMultiMatchWildcard, null);
			}
		}

		internal static void ValidatePatternDoesNotContainMultiMatchOnGroupsOrAssertions(string input)
		{
			if (Regex.IsMatch(Pattern.StripBackslashPairs(input), Pattern.unboundedGroupRepeater))
			{
				throw new ValidationArgumentException(Strings.ErrorPatternCannotContainMultiMatchOnGroupOrAssertion, null);
			}
		}

		internal static void ValidatePatternDoesNotHaveSequentialIdenticalMultiMatches(string input)
		{
			Regex regex = new Regex(Pattern.sequentialIdenticalMultiMatches);
			MatchCollection matchCollection = regex.Matches(input);
			foreach (object obj in matchCollection)
			{
				Match match = (Match)obj;
				string text = match.Groups["FirstCharacter"].Value;
				if (Pattern.IsSequenceLengthOdd(match.Groups["LeadingBackslashes"].Value))
				{
					text = "\\" + text;
				}
				string text2 = match.Groups["SecondCharacter"].Value;
				if (text != "\\" && (text == text2 || text == "." || text2 == "."))
				{
					throw new ValidationArgumentException(Strings.ErrorPatternCannotContainSequentialIdenticalMultiMatchPatterns, null);
				}
			}
		}

		internal static void ValidatePatternDoesNotContainEmptyAlternations(string input)
		{
			string input2 = Pattern.StripBackslashPairs(input);
			if (Regex.IsMatch(input2, Pattern.beginsWithEmptyAlternation) || Regex.IsMatch(input2, Pattern.endsWithEmptyAlternation))
			{
				throw new ValidationArgumentException(Strings.ErrorPatternCannotContainEmptyAlternations, null);
			}
		}

		internal void ValidatePatternDoesNotExceedCpuTimeLimit(long limit)
		{
			if (!Utils.IsRegexExecutionTimeWithinLimit(this.value, limit, false))
			{
				throw new ValidationArgumentException(Strings.ErrorPatternIsTooExpensive(this.value), null);
			}
		}

		private static void ThrowValidationArgumentExceptionIfMatchesAndDoesNotLeadWithEscapeCharacter(Regex regex, string input, LocalizedString errorMessage)
		{
			if (regex.Matches(input).Cast<Match>().Any((Match match) => match.Success && !Pattern.IsSequenceLengthOdd(match.Groups["LeadingBackslashes"].Value)))
			{
				throw new ValidationArgumentException(errorMessage, null);
			}
		}

		private static string StripBackslashPairs(string input)
		{
			return Regex.Replace(input, Pattern.backslashPair, string.Empty);
		}

		internal static bool IsSequenceLengthOdd(string sequence)
		{
			return sequence.Length % 2 == 1;
		}

		internal void SuppressPiiData()
		{
			this.value = SuppressingPiiData.Redact(this.value);
		}

		public const int MaxLength = 128;

		public const string AllowedCharacters = ".";

		private const string leadingBackslashesGroupName = "LeadingBackslashes";

		private const string beginsWithWildcardMultiMatchAnchoredPattern = "^\\^\\.(\\*|\\+)";

		private string value;

		private readonly bool isLegacyRegex;

		private readonly bool ignoreMaxLength;

		private static readonly string leadingBackslashesGroupPattern = string.Format("(?<{0}>\\\\*)", "LeadingBackslashes");

		private static readonly string boundedRepeaterWithMinimalLowerBoundPattern = "((\\{(?<!\\\\\\{))[01],\\d*(\\}(?<!\\\\\\})))";

		private static readonly string beginsWithWildcardMultiMatchPattern = string.Format("^\\.(\\*|\\+|{0})", Pattern.boundedRepeaterWithMinimalLowerBoundPattern);

		private static readonly string endsWithWildcardMultiMatchPattern = string.Format("{0}\\.(\\*|\\+|{1})$", Pattern.leadingBackslashesGroupPattern, Pattern.boundedRepeaterWithMinimalLowerBoundPattern);

		private static readonly string endsWithWildcardMultiMatchAnchoredPattern = string.Format("{0}\\.(\\*|\\+)\\$$", Pattern.leadingBackslashesGroupPattern);

		private static string groupOrAssertionContainsMultiMatchWildcard = "\\((?<!\\\\\\().*?\\.(?<!\\\\\\.)" + string.Format("([\\*\\+]|{0})", Pattern.boundedRepeaterWithMinimalLowerBoundPattern) + ".*?\\)(?<!\\\\\\))";

		private static readonly string sequentialIdenticalMultiMatches = string.Format("{0}(?<FirstCharacter>.)(\\*|\\+)(?<SecondCharacter>\\\\?.)(\\*|\\+)", Pattern.leadingBackslashesGroupPattern);

		private static string backslashPair = "\\\\\\\\";

		private static readonly string beginsWithEmptyAlternation = "(\\(|^)(?<!\\\\\\()\\|(?<!\\\\\\|)";

		private static readonly string endsWithEmptyAlternation = "\\|(?<!\\\\\\|)(\\)|$)(?<!\\\\\\))";

		private static readonly string unboundedGroupRepeater = "\\((?<!\\\\\\().*?\\)(?<!\\\\\\))(\\*|\\+)";

		internal static readonly string BoundedRepeaterPattern = "((\\{(?<!\\\\\\{))\\d+(,\\d*)?(\\}(?<!\\\\\\})))";
	}
}
