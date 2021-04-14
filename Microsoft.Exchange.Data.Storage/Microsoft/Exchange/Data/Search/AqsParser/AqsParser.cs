using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.StructuredQuery;

namespace Microsoft.Exchange.Data.Search.AqsParser
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class AqsParser
	{
		internal static Dictionary<string, PropertyKeyword> PropertyKeywordMap
		{
			get
			{
				return AqsParser.propertyKeywordMap;
			}
		}

		internal static Dictionary<string, KindKeyword> KindKeywordMap
		{
			get
			{
				return AqsParser.kindKeywordMap;
			}
		}

		internal HashSet<PropertyKeyword> AllowedKeywords { get; private set; }

		private bool SuppressError
		{
			get
			{
				return (this.parseOption & AqsParser.ParseOption.SuppressError) != AqsParser.ParseOption.None;
			}
		}

		private bool SupportNqs
		{
			get
			{
				return (this.parseOption & AqsParser.ParseOption.SupportNqs) != AqsParser.ParseOption.None;
			}
		}

		private bool UseCiKeywordOnly
		{
			get
			{
				return (this.parseOption & AqsParser.ParseOption.UseCiKeywordOnly) != AqsParser.ParseOption.None;
			}
		}

		private bool UseBasicKeywordOnly
		{
			get
			{
				return (this.parseOption & AqsParser.ParseOption.UseBasicKeywordsOnly) != AqsParser.ParseOption.None;
			}
		}

		private bool DisablePrefixMatch
		{
			get
			{
				return (this.parseOption & AqsParser.ParseOption.DisablePrefixMatch) != AqsParser.ParseOption.None;
			}
		}

		private bool SplitWords
		{
			get
			{
				return (this.parseOption & AqsParser.ParseOption.SplitWords) != AqsParser.ParseOption.None;
			}
		}

		private static bool IsOsVersionOrLater(int dwMajor, int dwMinor)
		{
			Version version = Environment.OSVersion.Version;
			return version.Major > dwMajor || (version.Major == dwMajor && version.Minor >= dwMinor);
		}

		private void ValidatePredicateOperator(Solution solution, LeafCondition leaf, List<ParserErrorInfo> errors)
		{
			if (leaf.Operation == null)
			{
				return;
			}
			TokenInfo tokenInfo = (leaf.OperationTermInfo == null) ? null : new TokenInfo(solution.Tokens[leaf.OperationTermInfo.FirstToken]);
			if (tokenInfo == null)
			{
				int num = -1;
				if (leaf.PropertyTermInfo != null)
				{
					num = leaf.PropertyTermInfo.FirstToken + leaf.PropertyTermInfo.Length;
				}
				int num2 = -1;
				if (leaf.ValueTermInfo != null)
				{
					num2 = leaf.ValueTermInfo.FirstToken - 1;
				}
				if (num != -1 && num2 != -1 && num <= num2)
				{
					tokenInfo = new TokenInfo(solution.Tokens[num2]);
				}
				else if (leaf.ValueTermInfo != null)
				{
					tokenInfo = new TokenInfo(solution.Tokens[leaf.ValueTermInfo.FirstToken]);
				}
				else if (leaf.PropertyTermInfo != null)
				{
					tokenInfo = new TokenInfo(solution.Tokens[leaf.PropertyTermInfo.FirstToken]);
				}
			}
			PropertyKeyword? propertyKeyword = null;
			if (leaf.PropertyTermInfo != null && AqsParser.PropertyKeywordMap.ContainsKey(leaf.PropertyName))
			{
				propertyKeyword = new PropertyKeyword?(AqsParser.PropertyKeywordMap[leaf.PropertyName]);
			}
			if (propertyKeyword == PropertyKeyword.Sent || propertyKeyword == PropertyKeyword.Received || propertyKeyword == PropertyKeyword.Importance || propertyKeyword == PropertyKeyword.Size)
			{
				switch (leaf.Operation)
				{
				case 1:
				case 3:
				case 4:
				case 5:
				case 6:
					break;
				default:
					errors.Add(new ParserErrorInfo(ParserErrorCode.InvalidOperator, tokenInfo));
					return;
				}
			}
			else
			{
				switch (leaf.Operation)
				{
				case 1:
				case 7:
				case 11:
				case 12:
				case 13:
					return;
				case 8:
				case 9:
					errors.Add(new ParserErrorInfo(ParserErrorCode.SuffixMatchNotSupported, tokenInfo));
					return;
				}
				errors.Add(new ParserErrorInfo(ParserErrorCode.InvalidOperator, tokenInfo));
			}
		}

		private void ValidatePredicate(Solution solution, LeafCondition leaf, List<ParserErrorInfo> errors, bool postResolving)
		{
			this.ValidatePredicateOperator(solution, leaf, errors);
			TokenInfo errorToken = (leaf.PropertyTermInfo == null) ? null : new TokenInfo(solution.Tokens[leaf.PropertyTermInfo.FirstToken]);
			PropertyKeyword propertyKeyword;
			if (postResolving)
			{
				if (!AqsParser.PropertyKeywordMap.TryGetValue(leaf.PropertyName, out propertyKeyword))
				{
					propertyKeyword = PropertyKeyword.All;
				}
			}
			else if (leaf.PropertyTermInfo != null)
			{
				if (!AqsParser.PropertyKeywordMap.TryGetValue(leaf.PropertyName, out propertyKeyword))
				{
					errors.Add(new ParserErrorInfo(ParserErrorCode.InvalidPropertyKey, errorToken));
					return;
				}
				if (!this.AllowedKeywords.Contains(propertyKeyword))
				{
					errors.Add(new ParserErrorInfo(ParserErrorCode.InvalidPropertyKey, errorToken));
					return;
				}
			}
			else
			{
				propertyKeyword = PropertyKeyword.All;
				if (leaf.ValueTermInfo == null)
				{
					errors.Add(new ParserErrorInfo(ParserErrorCode.UnexpectedToken));
					return;
				}
			}
			if (!postResolving && leaf.ValueTermInfo == null)
			{
				ParserErrorCode parserErrorCode = ParserErrorCode.MissingPropertyValue;
				PropertyKeyword propertyKeyword2 = propertyKeyword;
				switch (propertyKeyword2)
				{
				case PropertyKeyword.Sent:
				case PropertyKeyword.Received:
					parserErrorCode = ParserErrorCode.InvalidDateTimeFormat;
					break;
				default:
					if (propertyKeyword2 == PropertyKeyword.Kind)
					{
						parserErrorCode = ParserErrorCode.InvalidKindFormat;
					}
					break;
				}
				if (parserErrorCode != ParserErrorCode.MissingPropertyValue)
				{
					int num = leaf.PropertyTermInfo.FirstToken + leaf.PropertyTermInfo.Length + 1;
					if (leaf.OperationTermInfo != null)
					{
						num += leaf.PropertyTermInfo.Length;
					}
					if (num < solution.Tokens.Count)
					{
						errorToken = new TokenInfo(solution.Tokens[num]);
					}
				}
				errors.Add(new ParserErrorInfo(parserErrorCode, errorToken));
				return;
			}
			if (leaf.ValueTermInfo != null)
			{
				int firstChar = solution.Tokens[leaf.ValueTermInfo.FirstToken].FirstChar;
				errorToken = new TokenInfo(firstChar, leaf.ValueTermInfo.Text.Length);
			}
			if (propertyKeyword == PropertyKeyword.Kind)
			{
				if (!AqsParser.KindKeywordMap.ContainsKey((string)leaf.Value))
				{
					errors.Add(new ParserErrorInfo(ParserErrorCode.InvalidKindFormat, errorToken));
					return;
				}
			}
			else
			{
				if (propertyKeyword == PropertyKeyword.Sent || propertyKeyword == PropertyKeyword.Received)
				{
					if (postResolving || !(leaf.Value is string[]) || ((string[])leaf.Value).Length != 2)
					{
						return;
					}
					using (Condition condition = solution.Resolve(leaf, 0, (DateTime)ExDateTime.Now))
					{
						LeafCondition leafCondition = (LeafCondition)((CompoundCondition)condition).Children[0];
						LeafCondition leafCondition2 = (LeafCondition)((CompoundCondition)condition).Children[1];
						ExDateTime t = (ExDateTime)((DateTime)leafCondition.Value);
						ExDateTime t2 = (ExDateTime)((DateTime)leafCondition2.Value);
						if (t > t2)
						{
							errors.Add(new ParserErrorInfo(ParserErrorCode.InvalidDateTimeRange, errorToken));
						}
						return;
					}
				}
				if (postResolving && leaf.Value is string)
				{
					string text = (string)leaf.Value;
					if (!AqsParser.ContainsAlphanumericChars(text))
					{
						errors.Add(new ParserErrorInfo(ParserErrorCode.UnexpectedToken, errorToken));
					}
					int num2 = text.IndexOf('*');
					if (num2 >= 0)
					{
						string s = text.Substring(0, num2);
						if (!AqsParser.ContainsAlphanumericChars(s))
						{
							errors.Add(new ParserErrorInfo(ParserErrorCode.SuffixMatchNotSupported, errorToken));
						}
					}
				}
			}
		}

		private static bool ContainsAlphanumericChars(string s)
		{
			foreach (char c in s)
			{
				if (char.IsLetterOrDigit(c))
				{
					return true;
				}
			}
			return false;
		}

		private void ValidateConditionTree(Solution solution, Condition condition, List<ParserErrorInfo> errors, bool postResolving)
		{
			switch (condition.Type)
			{
			case 0:
			case 1:
				using (List<Condition>.Enumerator enumerator = ((CompoundCondition)condition).Children.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						Condition condition2 = enumerator.Current;
						this.ValidateConditionTree(solution, condition2, errors, postResolving);
					}
					return;
				}
				break;
			case 2:
				break;
			case 3:
			{
				LeafCondition leaf = (LeafCondition)condition;
				this.ValidatePredicate(solution, leaf, errors, postResolving);
				return;
			}
			default:
				throw new InvalidOperationException("Invalid condition type");
			}
			this.ValidateConditionTree(solution, ((NegationCondition)condition).Child, errors, postResolving);
		}

		private void PreResolvingValidate(Solution solution, List<ParserErrorInfo> errors)
		{
			foreach (ParseErrorInfo parseErrorInfo in solution.ParseErrors)
			{
				TokenInfo errorToken = new TokenInfo(solution.Tokens[parseErrorInfo.FirstToken]);
				switch (parseErrorInfo.Kind)
				{
				case 1:
				case 2:
					errors.Add(new ParserErrorInfo(ParserErrorCode.UnbalancedParenthesis, errorToken));
					break;
				case 3:
					errors.Add(new ParserErrorInfo(ParserErrorCode.InvalidModifier, errorToken));
					break;
				case 4:
					errors.Add(new ParserErrorInfo(ParserErrorCode.MissingOperand, errorToken));
					break;
				default:
					errors.Add(new ParserErrorInfo(ParserErrorCode.UnexpectedToken, errorToken));
					break;
				}
			}
			this.ValidateConditionTree(solution, solution.Condition, errors, false);
		}

		static AqsParser()
		{
			bool flag = AqsParser.IsOsVersionOrLater(6, 1);
			if (flag)
			{
				AqsParser.CanonicalKeywords = new Dictionary<PropertyKeyword, string>
				{
					{
						PropertyKeyword.From,
						"System.StructuredQuery.Virtual.From"
					},
					{
						PropertyKeyword.To,
						"System.StructuredQuery.Virtual.To"
					},
					{
						PropertyKeyword.Cc,
						"System.StructuredQuery.Virtual.Cc"
					},
					{
						PropertyKeyword.Bcc,
						"System.StructuredQuery.Virtual.Bcc"
					},
					{
						PropertyKeyword.Participants,
						"System.ItemParticipants"
					},
					{
						PropertyKeyword.Subject,
						"System.Subject"
					},
					{
						PropertyKeyword.Body,
						"System.Search.Contents"
					},
					{
						PropertyKeyword.Sent,
						"System.Message.DateSent"
					},
					{
						PropertyKeyword.Received,
						"System.Message.DateReceived"
					},
					{
						PropertyKeyword.Attachment,
						"System.Message.AttachmentContents"
					},
					{
						PropertyKeyword.PolicyTag,
						"System.Communication.PolicyTag"
					},
					{
						PropertyKeyword.Expires,
						"System.Communication.DateItemExpires"
					},
					{
						PropertyKeyword.HasAttachment,
						"System.Message.HasAttachments"
					},
					{
						PropertyKeyword.Category,
						"System.Category"
					},
					{
						PropertyKeyword.IsFlagged,
						"System.IsFlagged"
					},
					{
						PropertyKeyword.IsRead,
						"System.StructuredQuery.Virtual.IsRead"
					},
					{
						PropertyKeyword.Importance,
						"System.Importance"
					},
					{
						PropertyKeyword.Size,
						"System.Size"
					},
					{
						PropertyKeyword.Kind,
						"System.Kind"
					}
				};
				AqsParser.CanonicalKindKeys = AqsParser.KindKeywordMap.ToDictionary((KeyValuePair<string, KindKeyword> x) => x.Value, (KeyValuePair<string, KindKeyword> x) => string.Format("System.Kind#{0}", x.Key));
			}
			else
			{
				AqsParser.CanonicalKeywords = (from x in Enum.GetValues(typeof(PropertyKeyword)).OfType<PropertyKeyword>()
				where x != PropertyKeyword.All
				select x).ToDictionary((PropertyKeyword x) => x, (PropertyKeyword x) => Enum.GetName(typeof(PropertyKeyword), x));
				AqsParser.CanonicalKindKeys = Enum.GetValues(typeof(KindKeyword)).OfType<KindKeyword>().ToDictionary((KindKeyword x) => x, (KindKeyword x) => Enum.GetName(typeof(KindKeyword), x));
			}
			using (ParserManager parserManager = new ParserManager())
			{
				using (Parser parser = parserManager.CreateLoadedParser(CultureInfo.CurrentCulture))
				{
					parserManager.InitializeOptions(false, true, parser);
					parser.Parse("foo").Dispose();
				}
			}
		}

		public AqsParser()
		{
			this.AllowedKeywords = PropertyKeywordHelper.AllPropertyKeywords;
		}

		internal Condition Parse(string query, AqsParser.ParseOption parseOption, CultureInfo cultureInfo)
		{
			if (query == null)
			{
				throw new ArgumentNullException("query");
			}
			if (cultureInfo == null)
			{
				throw new ArgumentNullException("cultureInfo");
			}
			this.parseOption = parseOption;
			if (this.SupportNqs && !this.SuppressError)
			{
				throw new ArgumentException("parserOption: NQS must be combined with SuppressError");
			}
			if (this.UseCiKeywordOnly && this.UseBasicKeywordOnly)
			{
				throw new ArgumentException("UseBasicKeywordOnly can not be combined with UseCIKeywordOnly");
			}
			if (this.UseCiKeywordOnly)
			{
				this.AllowedKeywords = PropertyKeywordHelper.CiPropertyKeywords;
			}
			else if (this.UseBasicKeywordOnly)
			{
				this.AllowedKeywords = PropertyKeywordHelper.BasicPropertyKeywords;
			}
			Condition result;
			try
			{
				int num = 0;
				int i = 0;
				while (i < query.Length)
				{
					char c = query[i];
					if (c == '(' && ++num > 50)
					{
						if (this.SuppressError)
						{
							return null;
						}
						throw new ParserException(new ParserErrorInfo(ParserErrorCode.ParserError));
					}
					else
					{
						i++;
					}
				}
				using (ParserManager parserManager = new ParserManager())
				{
					using (Parser parser = parserManager.CreateLoadedParser(cultureInfo))
					{
						parserManager.InitializeOptions(this.SupportNqs, !this.DisablePrefixMatch, parser);
						using (Solution solution = parser.Parse(query))
						{
							List<ParserErrorInfo> list = new List<ParserErrorInfo>();
							if (!this.SuppressError)
							{
								this.PreResolvingValidate(solution, list);
								if (list.Count > 0)
								{
									throw new ParserException(list);
								}
							}
							ResolveOptions resolveOptions = 0;
							if (!this.SplitWords)
							{
								resolveOptions |= 64;
							}
							Condition condition = solution.Resolve(solution.Condition, resolveOptions, (DateTime)ExDateTime.Now);
							if (condition == null && !this.SuppressError)
							{
								throw new ParserException(new ParserErrorInfo(ParserErrorCode.ParserError));
							}
							if (!this.SuppressError)
							{
								this.ValidateConditionTree(solution, condition, list, true);
								if (list.Count > 0)
								{
									throw new ParserException(list);
								}
							}
							result = condition;
						}
					}
				}
			}
			catch (StructuredQueryException innerException)
			{
				if (!this.SuppressError)
				{
					throw new ParserException(new ParserErrorInfo(ParserErrorCode.StructuredQueryException), innerException);
				}
				result = null;
			}
			return result;
		}

		public static QueryFilter ParseAndBuildQuery(string query, AqsParser.ParseOption parseOption, CultureInfo culture, IRecipientResolver recipientResolver, IPolicyTagProvider policyTagProvider)
		{
			return AqsParser.ParseAndBuildQuery(query, parseOption, culture, RescopedAll.Default, recipientResolver, policyTagProvider);
		}

		public static QueryFilter ParseAndBuildQuery(string query, AqsParser.ParseOption parseOption, CultureInfo culture, RescopedAll rescopedAll, IRecipientResolver recipientResolver, IPolicyTagProvider policyTagProvider)
		{
			AqsParser aqsParser = new AqsParser();
			QueryFilter queryFilter = null;
			using (Condition condition = aqsParser.Parse(query, parseOption, culture))
			{
				QueryFilterBuilder queryFilterBuilder = new QueryFilterBuilder(culture, parseOption);
				queryFilterBuilder.RescopedAll = rescopedAll;
				queryFilterBuilder.RecipientResolver = recipientResolver;
				queryFilterBuilder.PolicyTagProvider = policyTagProvider;
				queryFilterBuilder.AllowedKeywords = aqsParser.AllowedKeywords;
				if (condition != null)
				{
					queryFilter = queryFilterBuilder.Build(condition);
				}
				if (queryFilter == null)
				{
					if (!aqsParser.SuppressError)
					{
						throw new ParserException(new ParserErrorInfo(ParserErrorCode.ParserError));
					}
					queryFilter = queryFilterBuilder.BuildAllFilter(1, query);
				}
			}
			return queryFilter;
		}

		public static ICollection<QueryFilter> FlattenQueryFilter(QueryFilter filter)
		{
			List<QueryFilter> list = new List<QueryFilter>();
			TextFilter textFilter = filter as TextFilter;
			if (textFilter != null)
			{
				list.Add(textFilter);
			}
			AndFilter andFilter = filter as AndFilter;
			if (andFilter != null)
			{
				list.Add(andFilter);
			}
			NotFilter notFilter = filter as NotFilter;
			if (notFilter != null)
			{
				list.Add(notFilter);
			}
			NearFilter nearFilter = filter as NearFilter;
			if (nearFilter != null)
			{
				list.Add(nearFilter);
			}
			OrFilter orFilter = filter as OrFilter;
			if (orFilter != null)
			{
				foreach (QueryFilter filter2 in orFilter.Filters)
				{
					list.AddRange(AqsParser.FlattenQueryFilter(filter2));
				}
			}
			return list;
		}

		internal static void HightlightParserErrors(string queryString, List<ParserErrorInfo> parserErrors, AqsParser.HandlePartitionedTokenDelegate handlePartitionedToken)
		{
			List<ParserErrorInfo> list = new List<ParserErrorInfo>();
			list.AddRange(parserErrors);
			list.Sort(delegate(ParserErrorInfo e1, ParserErrorInfo e2)
			{
				if (e1.ErrorToken == null && e2.ErrorToken == null)
				{
					return 0;
				}
				if (e1.ErrorToken == null)
				{
					return 1;
				}
				if (e2.ErrorToken == null)
				{
					return -1;
				}
				if (e1.ErrorToken.FirstChar != e2.ErrorToken.FirstChar)
				{
					return e1.ErrorToken.FirstChar - e2.ErrorToken.FirstChar;
				}
				return -(e1.ErrorToken.Length - e2.ErrorToken.Length);
			});
			int num = 0;
			foreach (ParserErrorInfo parserErrorInfo in list)
			{
				if (parserErrorInfo.ErrorToken != null)
				{
					if (parserErrorInfo.ErrorToken.FirstChar > num)
					{
						handlePartitionedToken(num, parserErrorInfo.ErrorToken.FirstChar - num, false);
						num = parserErrorInfo.ErrorToken.FirstChar;
					}
					int num2 = parserErrorInfo.ErrorToken.FirstChar + parserErrorInfo.ErrorToken.Length;
					if (num2 > num)
					{
						handlePartitionedToken(num, num2 - num, true);
						num = num2;
					}
				}
			}
			if (num < queryString.Length)
			{
				handlePartitionedToken(num, queryString.Length - num, false);
			}
		}

		private const int MaxExpressions = 50;

		private static Dictionary<string, PropertyKeyword> propertyKeywordMap = new Dictionary<string, PropertyKeyword>(StringComparer.OrdinalIgnoreCase)
		{
			{
				"System.Message.FromAddress",
				PropertyKeyword.From
			},
			{
				"System.Message.FromName",
				PropertyKeyword.From
			},
			{
				"System.Message.SenderAddress",
				PropertyKeyword.From
			},
			{
				"System.Message.SenderName",
				PropertyKeyword.From
			},
			{
				"System.StructuredQuery.Virtual.From",
				PropertyKeyword.From
			},
			{
				"System.Message.ToAddress",
				PropertyKeyword.To
			},
			{
				"System.Message.ToName",
				PropertyKeyword.To
			},
			{
				"System.StructuredQuery.Virtual.To",
				PropertyKeyword.To
			},
			{
				"System.Message.CcAddress",
				PropertyKeyword.Cc
			},
			{
				"System.Message.CcName",
				PropertyKeyword.Cc
			},
			{
				"System.StructuredQuery.Virtual.Cc",
				PropertyKeyword.Cc
			},
			{
				"System.Message.BccAddress",
				PropertyKeyword.Bcc
			},
			{
				"System.Message.BccName",
				PropertyKeyword.Bcc
			},
			{
				"System.StructuredQuery.Virtual.Bcc",
				PropertyKeyword.Bcc
			},
			{
				"System.ItemParticipants",
				PropertyKeyword.Participants
			},
			{
				"System.Subject",
				PropertyKeyword.Subject
			},
			{
				"System.Search.Contents",
				PropertyKeyword.Body
			},
			{
				"System.Message.DateSent",
				PropertyKeyword.Sent
			},
			{
				"System.Message.DateReceived",
				PropertyKeyword.Received
			},
			{
				"System.Message.AttachmentContents",
				PropertyKeyword.Attachment
			},
			{
				"System.Message.AttachmentNames",
				PropertyKeyword.AttachmentNames
			},
			{
				"System.StructuredQuery.Compound.0397",
				PropertyKeyword.Attachment
			},
			{
				"System.Communication.PolicyTag",
				PropertyKeyword.PolicyTag
			},
			{
				"System.Communication.DateItemExpires",
				PropertyKeyword.Expires
			},
			{
				"System.Message.HasAttachments",
				PropertyKeyword.HasAttachment
			},
			{
				"System.Category",
				PropertyKeyword.Category
			},
			{
				"System.IsFlagged",
				PropertyKeyword.IsFlagged
			},
			{
				"System.StructuredQuery.Virtual.IsRead",
				PropertyKeyword.IsRead
			},
			{
				"System.IsRead",
				PropertyKeyword.IsRead
			},
			{
				"System.Importance",
				PropertyKeyword.Importance
			},
			{
				"System.Size",
				PropertyKeyword.Size
			},
			{
				"System.Kind",
				PropertyKeyword.Kind
			},
			{
				"System.FileName",
				PropertyKeyword.All
			},
			{
				"*",
				PropertyKeyword.All
			}
		};

		private static Dictionary<string, KindKeyword> kindKeywordMap = new Dictionary<string, KindKeyword>(StringComparer.OrdinalIgnoreCase)
		{
			{
				"email",
				KindKeyword.email
			},
			{
				"calendar",
				KindKeyword.meetings
			},
			{
				"task",
				KindKeyword.tasks
			},
			{
				"note",
				KindKeyword.notes
			},
			{
				"document",
				KindKeyword.docs
			},
			{
				"journal",
				KindKeyword.journals
			},
			{
				"contact",
				KindKeyword.contacts
			},
			{
				"instantmessage",
				KindKeyword.im
			},
			{
				"voicemail",
				KindKeyword.voicemail
			},
			{
				"fax",
				KindKeyword.faxes
			},
			{
				"rssfeed",
				KindKeyword.rssfeeds
			}
		};

		internal static Dictionary<PropertyKeyword, string> CanonicalKeywords = null;

		internal static Dictionary<KindKeyword, string> CanonicalKindKeys = null;

		private AqsParser.ParseOption parseOption;

		[Flags]
		public enum ParseOption
		{
			None = 0,
			SuppressError = 1,
			SupportNqs = 2,
			SplitWords = 4,
			UseCiKeywordOnly = 16,
			DisablePrefixMatch = 32,
			UseBasicKeywordsOnly = 64,
			ContentIndexingDisabled = 128,
			QueryConverting = 256,
			AllowShortWildcards = 512
		}

		internal delegate void HandlePartitionedTokenDelegate(int start, int length, bool isErrorToken);
	}
}
