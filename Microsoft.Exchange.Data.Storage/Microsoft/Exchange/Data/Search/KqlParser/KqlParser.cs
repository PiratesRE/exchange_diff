using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;
using Microsoft.Ceres.InteractionEngine.Processing.BuiltIn.Parsing;
using Microsoft.Ceres.InteractionEngine.Processing.BuiltIn.Parsing.Kql;
using Microsoft.Ceres.NlpBase.RichTypes.QueryTree;
using Microsoft.Exchange.Data.Search.AqsParser;
using Microsoft.Exchange.Data.Search.Common;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Search.KqlParser
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class KqlParser
	{
		public static QueryFilter ParseAndBuildQuery(string query, KqlParser.ParseOption parseOption, CultureInfo cultureInfo, IRecipientResolver recipientResolver, IPolicyTagProvider policyTagProvider)
		{
			return KqlParser.ParseAndBuildQuery(query, parseOption, cultureInfo, RescopedAll.Default, recipientResolver, policyTagProvider);
		}

		public static QueryFilter ParseAndBuildQuery(string query, KqlParser.ParseOption parseOption, CultureInfo cultureInfo, RescopedAll rescopedAll, IRecipientResolver recipientResolver, IPolicyTagProvider policyTagProvider)
		{
			KqlParser kqlParser = new KqlParser();
			LocalizedKeywordMapping mapping = LocalizedKeywordMapping.GetMapping(cultureInfo);
			TreeNode treeNode = kqlParser.Parse(query, parseOption, cultureInfo, mapping);
			QueryFilterBuilder queryFilterBuilder = new QueryFilterBuilder(mapping, parseOption, rescopedAll, recipientResolver, policyTagProvider, cultureInfo);
			queryFilterBuilder.AllowedKeywords = KqlParser.GetSupportedProperties(parseOption);
			QueryFilter queryFilter = queryFilterBuilder.Build(treeNode);
			if (queryFilter == null)
			{
				if ((parseOption & KqlParser.ParseOption.SuppressError) == KqlParser.ParseOption.None)
				{
					throw new ParserException(new ParserErrorInfo(ParserErrorCode.ParserError));
				}
				queryFilter = queryFilterBuilder.BuildAllFilter(query);
			}
			return queryFilter;
		}

		internal TreeNode Parse(string query, KqlParser.ParseOption parseOption, CultureInfo cultureInfo, LocalizedKeywordMapping keywordMapping)
		{
			if (string.IsNullOrEmpty(query))
			{
				throw new ArgumentNullException("query");
			}
			if (cultureInfo == null)
			{
				throw new ArgumentNullException("cultureInfo");
			}
			if ((parseOption & KqlParser.ParseOption.UseBasicKeywordsOnly) != KqlParser.ParseOption.None && (parseOption & KqlParser.ParseOption.UseCiKeywordOnly) != KqlParser.ParseOption.None)
			{
				throw new ArgumentException("GetBasicKeywordOnly can not be combined with UseCIKeywordOnly");
			}
			List<ParserErrorInfo> list = ((parseOption & KqlParser.ParseOption.SuppressError) != KqlParser.ParseOption.None) ? null : new List<ParserErrorInfo>();
			KqlParser kqlParser = new KqlParser();
			ParsingContext parsingContext = new ParsingContext
			{
				CultureInfo = cultureInfo,
				ImplicitAndBehavior = ((parseOption & KqlParser.ParseOption.ImplicitOr) == KqlParser.ParseOption.None),
				PropertyLookup = new PropertyLookup(keywordMapping, KqlParser.GetSupportedProperties(parseOption), list),
				SpecialPropertyHandler = new SpecialPropertyHandler(keywordMapping, list)
			};
			TreeNode treeNode = null;
			try
			{
				KqlParser.VerifyQuery(query);
				query = KqlParser.NormalizeQuery(query);
				treeNode = kqlParser.Parse(query, parsingContext);
			}
			catch (FormatException innerException)
			{
				if ((parseOption & KqlParser.ParseOption.SuppressError) == KqlParser.ParseOption.None)
				{
					throw new ParserException(new ParserErrorInfo(ParserErrorCode.KqlParseException), innerException);
				}
			}
			catch (ParseException innerException2)
			{
				if ((parseOption & KqlParser.ParseOption.SuppressError) == KqlParser.ParseOption.None)
				{
					throw new ParserException(new ParserErrorInfo(ParserErrorCode.KqlParseException), innerException2);
				}
			}
			catch (ParserException ex)
			{
				if ((parseOption & KqlParser.ParseOption.SuppressError) == KqlParser.ParseOption.None)
				{
					throw ex;
				}
			}
			if (list != null && list.Count > 0)
			{
				throw new ParserException(list);
			}
			if ((parseOption & KqlParser.ParseOption.SuppressError) == KqlParser.ParseOption.None && treeNode == null)
			{
				throw new ParserException(new ParserErrorInfo(ParserErrorCode.ParserError));
			}
			return treeNode;
		}

		private static void VerifyQuery(string query)
		{
			try
			{
				Match match = KqlParser.asteriskPattern.Match(query);
				if (match.Success)
				{
					TokenInfo errorToken = null;
					if (match.Groups != null)
					{
						if (match.Groups.Count == 1)
						{
							errorToken = new TokenInfo(match.Groups[0].Index, match.Groups[0].Length);
						}
						else if (match.Groups.Count > 1)
						{
							errorToken = new TokenInfo(match.Groups[1].Index, match.Groups[1].Length);
						}
					}
					throw new ParserException(new ParserErrorInfo(ParserErrorCode.UnexpectedToken, errorToken));
				}
			}
			catch (RegexMatchTimeoutException innerException)
			{
				throw new ParserException(new ParserErrorInfo(ParserErrorCode.KqlParseException, ServerStrings.KqlParserTimeout, null), innerException);
			}
		}

		private static string NormalizeQuery(string query)
		{
			query = KqlParser.ReplaceQuotes(query);
			query = KqlParser.ReplaceWhiteSpaces(query);
			return query;
		}

		private static string ReplaceWhiteSpaces(string query)
		{
			return Regex.Replace(query, "\\s+", " ");
		}

		private static string ReplaceQuotes(string query)
		{
			char[] array = query.ToCharArray();
			for (int i = 0; i < array.Length; i++)
			{
				char c = array[i];
				if (c == '“' || c == '”' || c == '„' || c == '‟' || c == '〝' || c == '〞' || c == '〟' || c == '＂')
				{
					array[i] = '"';
				}
			}
			return new string(array);
		}

		private static HashSet<PropertyKeyword> GetSupportedProperties(KqlParser.ParseOption parseOption)
		{
			if ((parseOption & KqlParser.ParseOption.UseCiKeywordOnly) != KqlParser.ParseOption.None)
			{
				return PropertyKeywordHelper.CiPropertyKeywords;
			}
			if ((parseOption & KqlParser.ParseOption.UseBasicKeywordsOnly) != KqlParser.ParseOption.None)
			{
				return PropertyKeywordHelper.BasicPropertyKeywords;
			}
			return PropertyKeywordHelper.AllPropertyKeywords;
		}

		private static Regex asteriskPattern = new Regex("(?:^|[\\W-[\\*]]+)(\\*+)(?:[^\"]*\"[^\"]*\")*[^\"]*$", RegexOptions.Compiled, TimeSpan.FromSeconds(10.0));

		[Flags]
		public enum ParseOption
		{
			None = 0,
			SuppressError = 1,
			ImplicitOr = 2,
			UseCiKeywordOnly = 4,
			DisablePrefixMatch = 8,
			UseBasicKeywordsOnly = 16,
			ContentIndexingDisabled = 32,
			QueryPreserving = 64,
			AllowShortWildcards = 128,
			EDiscoveryMode = 256
		}
	}
}
