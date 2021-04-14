using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.Exchange.Data.Search.Common;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.InfoWorker.Common;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.StructuredQuery;

namespace Microsoft.Exchange.Data.Search.AqsParser
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class QueryFilterBuilder
	{
		private QueryFilter BuildTextFilter(PropertyDefinition[] properties, string propertyValue, MatchOptions matchOption, MatchFlags matchFlags)
		{
			if (string.IsNullOrEmpty(propertyValue))
			{
				return null;
			}
			if (properties.Length == 1)
			{
				return new TextFilter(properties[0], propertyValue, matchOption, matchFlags);
			}
			if (properties.Length > 1)
			{
				QueryFilter[] array = new QueryFilter[properties.Length];
				for (int i = 0; i < properties.Length; i++)
				{
					array[i] = new TextFilter(properties[i], propertyValue, matchOption, matchFlags);
				}
				return new OrFilter(array);
			}
			return null;
		}

		private QueryFilter BuildTextFilter(PropertyDefinition[] properties, string propertyValue, ConditionOperation opt)
		{
			if (string.IsNullOrEmpty(propertyValue))
			{
				return null;
			}
			MatchOptions matchOption;
			switch (opt)
			{
			case 1:
			case 12:
				matchOption = MatchOptions.ExactPhrase;
				goto IL_69;
			case 7:
			case 13:
				matchOption = (this.IsContentIndexingEnabled ? MatchOptions.PrefixOnWords : MatchOptions.SubString);
				goto IL_69;
			case 8:
			case 9:
				matchOption = MatchOptions.SubString;
				goto IL_69;
			case 11:
				matchOption = MatchOptions.WildcardString;
				goto IL_69;
			}
			return null;
			IL_69:
			int num;
			if (!this.ShortWildcardsAllowed && (propertyValue.Length <= 1 || (propertyValue.Length <= 4 && int.TryParse(propertyValue, NumberStyles.Integer, this.culture, out num))))
			{
				matchOption = MatchOptions.ExactPhrase;
			}
			return this.BuildTextFilter(properties, propertyValue, matchOption, MatchFlags.Loose);
		}

		private QueryFilter BuildComparisonFilter(ComparisonOperator comparisionOpt, PropertyDefinition[] properties, object propertyValue)
		{
			if (this.IsQueryConverting && propertyValue != null && propertyValue is ExDateTime)
			{
				propertyValue = new ExDateTime(ExTimeZone.CurrentTimeZone, ((ExDateTime)propertyValue).UniversalTime);
			}
			if (properties.Length == 1)
			{
				return new ComparisonFilter(comparisionOpt, properties[0], propertyValue);
			}
			if (properties.Length > 1)
			{
				QueryFilter[] array = new QueryFilter[properties.Length];
				for (int i = 0; i < properties.Length; i++)
				{
					array[i] = new ComparisonFilter(comparisionOpt, properties[i], propertyValue);
				}
				return new OrFilter(array);
			}
			return null;
		}

		private QueryFilter BuildExistsFilter(PropertyDefinition[] properties)
		{
			if (properties.Length == 1)
			{
				return new ExistsFilter(properties[0]);
			}
			if (properties.Length > 1)
			{
				QueryFilter[] array = new QueryFilter[properties.Length];
				for (int i = 0; i < properties.Length; i++)
				{
					array[i] = new ExistsFilter(properties[i]);
				}
				return new OrFilter(array);
			}
			return null;
		}

		private QueryFilter BuildComparisonFilter(ConditionOperation opt, PropertyDefinition[] properties, object propertyValue)
		{
			ComparisonOperator comparisionOpt;
			switch (opt)
			{
			case 1:
				comparisionOpt = ComparisonOperator.Equal;
				goto IL_3C;
			case 3:
				comparisionOpt = ComparisonOperator.LessThan;
				goto IL_3C;
			case 4:
				comparisionOpt = ComparisonOperator.GreaterThan;
				goto IL_3C;
			case 5:
				comparisionOpt = ComparisonOperator.LessThanOrEqual;
				goto IL_3C;
			case 6:
				comparisionOpt = ComparisonOperator.GreaterThanOrEqual;
				goto IL_3C;
			}
			return null;
			IL_3C:
			return this.BuildComparisonFilter(comparisionOpt, properties, propertyValue);
		}

		private QueryFilter BuildRecipientFilter(PropertyDefinition[] propertyMapping, ConditionOperation opt, object propertyValue)
		{
			List<QueryFilter> list = new List<QueryFilter>();
			QueryFilter queryFilter = this.BuildTextFilter(propertyMapping, (string)propertyValue, opt);
			if (queryFilter != null)
			{
				list.Add(queryFilter);
			}
			if (this.RecipientResolver != null)
			{
				string[] array = this.RecipientResolver.Resolve(propertyValue as string);
				if (array != null)
				{
					foreach (string text in array)
					{
						if (!text.Equals((string)propertyValue, StringComparison.OrdinalIgnoreCase))
						{
							queryFilter = this.BuildTextFilter(propertyMapping, text, opt);
							if (queryFilter != null)
							{
								list.Add(queryFilter);
							}
						}
					}
				}
			}
			if (list.Count <= 0)
			{
				return null;
			}
			if (list.Count <= 1)
			{
				return list[0];
			}
			return new OrFilter(list.ToArray());
		}

		private QueryFilter BuildFromFilter(ConditionOperation opt, object value)
		{
			List<QueryFilter> list = new List<QueryFilter>();
			QueryFilter queryFilter = this.BuildTextFilter(this.fromMapping, (string)value, opt);
			if (queryFilter != null)
			{
				list.Add(queryFilter);
			}
			if (this.RecipientResolver != null)
			{
				string[] array = this.RecipientResolver.Resolve(value as string);
				if (array != null)
				{
					foreach (string text in array)
					{
						if (!text.Equals((string)value, StringComparison.OrdinalIgnoreCase))
						{
							queryFilter = this.BuildTextFilter(this.fromMapping, text, 1);
							if (queryFilter != null)
							{
								list.Add(queryFilter);
							}
						}
					}
				}
			}
			if (list.Count <= 0)
			{
				return null;
			}
			if (list.Count <= 1)
			{
				return list[0];
			}
			return new OrFilter(list.ToArray());
		}

		private QueryFilter BuildToFilter(ConditionOperation opt, object value)
		{
			return this.BuildRecipientFilter(this.toMapping, opt, value);
		}

		private QueryFilter BuildBccFilter(ConditionOperation opt, object value)
		{
			return this.BuildRecipientFilter(this.bccMapping, opt, value);
		}

		private QueryFilter BuildCcFilter(ConditionOperation opt, object value)
		{
			return this.BuildRecipientFilter(this.ccMapping, opt, value);
		}

		private QueryFilter BuildParticipantsFilter(ConditionOperation opt, object value)
		{
			return this.BuildRecipientFilter(this.IsQueryConverting ? new PropertyDefinition[]
			{
				this.participantsMapping[0]
			} : this.participantsMapping, opt, value);
		}

		private QueryFilter BuildSubjectFilter(ConditionOperation opt, object value)
		{
			return this.BuildTextFilter(this.subjectMapping, (string)value, opt);
		}

		private QueryFilter BuildBodyFilter(ConditionOperation opt, object value)
		{
			return this.BuildTextFilter(this.bodyMapping, (string)value, opt);
		}

		private QueryFilter BuildAttachmentFilter(ConditionOperation opt, object value)
		{
			QueryFilter queryFilter = this.BuildTextFilter(this.attachmentMapping, (string)value, opt);
			if (queryFilter != null && !this.IsContentIndexingEnabled)
			{
				QueryFilter queryFilter2 = this.BuildTextFilter(this.attachmentSubMapping, (string)value, opt);
				if (queryFilter2 != null)
				{
					return new OrFilter(new QueryFilter[]
					{
						queryFilter,
						new SubFilter(SubFilterProperties.Attachments, queryFilter2)
					});
				}
			}
			return queryFilter;
		}

		private QueryFilter BuildAttachmentNamesFilter(ConditionOperation opt, object value)
		{
			return this.BuildTextFilter(this.attachmentNamesMapping, (string)value, opt);
		}

		private QueryFilter BuildSentFilter(ConditionOperation opt, object value)
		{
			ExDateTime exDateTime = new ExDateTime(ExTimeZone.UtcTimeZone, ((DateTime)value).ToUniversalTime());
			return this.BuildComparisonFilter(opt, this.sentMapping, exDateTime);
		}

		private QueryFilter BuildReceivedFilter(ConditionOperation opt, object value)
		{
			ExDateTime exDateTime = new ExDateTime(ExTimeZone.UtcTimeZone, ((DateTime)value).ToUniversalTime());
			return this.BuildComparisonFilter(opt, this.receivedMapping, exDateTime);
		}

		private QueryFilter BuildKindFilter(ConditionOperation opt, object value)
		{
			if (AqsParser.KindKeywordMap.ContainsKey((string)value))
			{
				KindKeyword kindKeyword = AqsParser.KindKeywordMap[(string)value];
				if (this.IsQueryConverting)
				{
					return this.BuildTextFilter(this.kindMapping, kindKeyword.ToString(), 1);
				}
				List<QueryFilter> list = new List<QueryFilter>();
				string[] array = Globals.KindKeywordToClassMap[kindKeyword];
				foreach (string propertyValue in array)
				{
					QueryFilter queryFilter = this.BuildTextFilter(this.kindMapping, propertyValue, 13);
					if (queryFilter != null)
					{
						list.Add(queryFilter);
					}
				}
				if (list.Count > 0)
				{
					if (list.Count <= 1)
					{
						return list[0];
					}
					return new OrFilter(list.ToArray());
				}
			}
			return null;
		}

		private QueryFilter BuildHasAttachmentFilter(ConditionOperation opt, object value)
		{
			if (value is bool)
			{
				return this.BuildComparisonFilter(1, this.hasAttachmentMapping, (bool)value);
			}
			return null;
		}

		private QueryFilter BuildIsFlaggedFilter(ConditionOperation opt, object value)
		{
			if (!(value is bool))
			{
				return null;
			}
			if ((bool)value)
			{
				return this.BuildComparisonFilter(1, this.isFlaggedMapping, FlagStatus.Flagged);
			}
			return new OrFilter(new QueryFilter[]
			{
				this.BuildComparisonFilter(1, this.isFlaggedMapping, FlagStatus.NotFlagged),
				new NotFilter(this.BuildExistsFilter(this.isFlaggedMapping))
			});
		}

		private QueryFilter BuildIsReadFilter(ConditionOperation opt, object value)
		{
			if (value is bool)
			{
				return this.BuildComparisonFilter(1, this.isReadMapping, (bool)value);
			}
			return null;
		}

		private QueryFilter BuildCategoryFilter(ConditionOperation opt, object value)
		{
			return this.BuildTextFilter(this.categoryMapping, (string)value, opt);
		}

		private QueryFilter BuildImportanceFilter(ConditionOperation opt, object value)
		{
			int? num = null;
			if (value is long)
			{
				num = new int?((int)((long)value));
			}
			else if (value is int)
			{
				num = new int?((int)value);
			}
			if (num != null)
			{
				QueryFilter queryFilter = this.BuildComparisonFilter(opt, this.importanceMapping, num.Value / 2);
				if (this.IsQueryConverting && queryFilter.Equals(QueryFilterBuilder.importanceHighFilter))
				{
					queryFilter = this.BuildTextFilter(this.importanceMapping, Importance.High.ToString(), 1);
				}
				return queryFilter;
			}
			return null;
		}

		private QueryFilter BuildSizeFilter(ConditionOperation opt, object value)
		{
			int? num = null;
			if (value is long)
			{
				num = new int?((int)((long)value));
			}
			else if (value is int)
			{
				num = new int?((int)value);
			}
			if (num != null)
			{
				return this.BuildComparisonFilter(opt, this.sizeMapping, num.Value);
			}
			return null;
		}

		private QueryFilter BuildPolicyTagFilter(ConditionOperation opt, object value)
		{
			string tagName = value as string;
			if (string.IsNullOrEmpty(tagName))
			{
				return null;
			}
			List<QueryFilter> list = new List<QueryFilter>();
			if (QueryFilterBuilder.guidRegex.Match(tagName).Success)
			{
				Guid guid = Guid.Empty;
				try
				{
					guid = new Guid(tagName);
					list.Add(this.BuildComparisonFilter(ComparisonOperator.Equal, this.policyTagMapping, guid.ToByteArray()));
				}
				catch (FormatException)
				{
				}
			}
			IEnumerable<PolicyTag> enumerable = null;
			if (this.PolicyTagProvider != null)
			{
				PolicyTag[] policyTags = this.PolicyTagProvider.PolicyTags;
				if (policyTags != null)
				{
					switch (opt)
					{
					case 1:
					case 12:
						enumerable = from x in policyTags
						where x.Name.Equals(tagName, StringComparison.OrdinalIgnoreCase)
						select x;
						break;
					case 7:
					case 8:
					case 9:
					case 11:
					case 13:
					{
						Regex regex = new Regex(tagName, RegexOptions.IgnoreCase | RegexOptions.Compiled | RegexOptions.CultureInvariant);
						enumerable = from x in policyTags
						where regex.Match(x.Name).Success
						select x;
						break;
					}
					}
				}
			}
			if (enumerable != null)
			{
				foreach (PolicyTag policyTag in enumerable)
				{
					list.Add(this.BuildComparisonFilter(ComparisonOperator.Equal, this.policyTagMapping, policyTag.PolicyGuid.ToByteArray()));
				}
			}
			if (list.Count <= 0)
			{
				return null;
			}
			if (list.Count <= 1)
			{
				return list[0];
			}
			return new OrFilter(list.ToArray());
		}

		private QueryFilter BuildExpiresFilter(ConditionOperation opt, object value)
		{
			ExDateTime exDateTime = new ExDateTime(ExTimeZone.UtcTimeZone, ((DateTime)value).ToUniversalTime());
			return this.BuildComparisonFilter(opt, this.expiresMapping, exDateTime);
		}

		internal QueryFilter BuildAllFilter(ConditionOperation opt, object value)
		{
			PropertyKeyword[] array2;
			switch (this.RescopedAll)
			{
			case RescopedAll.Default:
				return this.BuildTextFilter(this.allMapping, (string)value, opt);
			case RescopedAll.Subject:
			{
				PropertyKeyword[] array = new PropertyKeyword[1];
				array2 = array;
				break;
			}
			case RescopedAll.Body:
				array2 = new PropertyKeyword[]
				{
					PropertyKeyword.Body
				};
				break;
			case RescopedAll.BodyAndSubject:
				array2 = new PropertyKeyword[]
				{
					PropertyKeyword.Subject,
					PropertyKeyword.Body
				};
				break;
			case RescopedAll.From:
				array2 = new PropertyKeyword[]
				{
					PropertyKeyword.From
				};
				break;
			case RescopedAll.Participants:
				array2 = new PropertyKeyword[]
				{
					PropertyKeyword.Participants
				};
				break;
			default:
				throw new ArgumentException("RescopedAll");
			}
			List<QueryFilter> list = new List<QueryFilter>();
			foreach (PropertyKeyword key in array2)
			{
				QueryFilter queryFilter = this.filterBuilderMap[key](opt, value);
				if (queryFilter != null)
				{
					list.Add(queryFilter);
				}
			}
			if (list.Count <= 0)
			{
				return null;
			}
			if (list.Count <= 1)
			{
				return list[0];
			}
			return new OrFilter(list.ToArray());
		}

		private QueryFilter PredicateToQueryFilter(LeafCondition leafCond)
		{
			if (!AqsParser.PropertyKeywordMap.ContainsKey(leafCond.PropertyName))
			{
				return null;
			}
			PropertyKeyword? propertyKeyword = new PropertyKeyword?(AqsParser.PropertyKeywordMap[leafCond.PropertyName]);
			if (propertyKeyword == null)
			{
				return null;
			}
			if (!this.AllowedKeywords.Contains(propertyKeyword.Value))
			{
				return null;
			}
			if (leafCond.Operation == 2)
			{
				QueryFilter queryFilter = this.filterBuilderMap[propertyKeyword.Value](1, leafCond.Value);
				if (queryFilter != null)
				{
					return new NotFilter(queryFilter);
				}
			}
			else
			{
				if (leafCond.Operation != 10)
				{
					return this.filterBuilderMap[propertyKeyword.Value](leafCond.Operation, leafCond.Value);
				}
				QueryFilter queryFilter2 = this.filterBuilderMap[propertyKeyword.Value](9, leafCond.Value);
				if (queryFilter2 != null)
				{
					return new NotFilter(queryFilter2);
				}
			}
			return null;
		}

		private QueryFilter ConditionToQueryFilter(Condition condition)
		{
			if (condition.Type == null)
			{
				HashSet<QueryFilter> hashSet = new HashSet<QueryFilter>();
				foreach (Condition condition2 in ((CompoundCondition)condition).Children)
				{
					QueryFilter queryFilter = this.ConditionToQueryFilter(condition2);
					if (queryFilter == null)
					{
						return null;
					}
					hashSet.Add(queryFilter);
				}
				if (hashSet.Count > 0)
				{
					QueryFilter queryFilter2 = (hashSet.Count > 1) ? new AndFilter(hashSet.ToArray<QueryFilter>()) : hashSet.First<QueryFilter>();
					if (this.IsQueryConverting)
					{
						if (queryFilter2.Equals(QueryFilterBuilder.importanceNormalFilter))
						{
							queryFilter2 = this.BuildTextFilter(this.importanceMapping, Importance.Normal.ToString(), 1);
						}
						else if (queryFilter2.Equals(QueryFilterBuilder.importanceLowFilter))
						{
							queryFilter2 = this.BuildTextFilter(this.importanceMapping, Importance.Low.ToString(), 1);
						}
					}
					return queryFilter2;
				}
			}
			else if (condition.Type == 1)
			{
				HashSet<QueryFilter> hashSet2 = new HashSet<QueryFilter>();
				foreach (Condition condition3 in ((CompoundCondition)condition).Children)
				{
					QueryFilter queryFilter3 = this.ConditionToQueryFilter(condition3);
					if (queryFilter3 == null)
					{
						return null;
					}
					hashSet2.Add(queryFilter3);
				}
				if (hashSet2.Count > 0)
				{
					if (hashSet2.Count <= 1)
					{
						return hashSet2.First<QueryFilter>();
					}
					return new OrFilter(hashSet2.ToArray<QueryFilter>());
				}
			}
			else if (condition.Type == 2)
			{
				QueryFilter queryFilter4 = this.ConditionToQueryFilter(((NegationCondition)condition).Child);
				if (queryFilter4 != null)
				{
					return new NotFilter(queryFilter4);
				}
			}
			else
			{
				if (condition.Type == 3)
				{
					return this.PredicateToQueryFilter((LeafCondition)condition);
				}
				throw new ArgumentException("No condition node other than NOT, AND, OR and Leaf is allowed.");
			}
			return null;
		}

		public IRecipientResolver RecipientResolver { get; set; }

		public IPolicyTagProvider PolicyTagProvider { get; set; }

		public bool IsContentIndexingEnabled
		{
			get
			{
				return (this.options & AqsParser.ParseOption.ContentIndexingDisabled) == AqsParser.ParseOption.None;
			}
		}

		public bool ShortWildcardsAllowed
		{
			get
			{
				return (this.options & AqsParser.ParseOption.AllowShortWildcards) != AqsParser.ParseOption.None;
			}
		}

		public RescopedAll RescopedAll
		{
			get
			{
				return this.rescopedAll;
			}
			set
			{
				this.rescopedAll = value;
			}
		}

		public bool IsQueryConverting
		{
			get
			{
				return (this.options & AqsParser.ParseOption.QueryConverting) != AqsParser.ParseOption.None;
			}
		}

		public HashSet<PropertyKeyword> AllowedKeywords { get; set; }

		public QueryFilterBuilder(CultureInfo culture, AqsParser.ParseOption options)
		{
			this.culture = culture;
			this.options = options;
			this.AllowedKeywords = PropertyKeywordHelper.AllPropertyKeywords;
			this.filterBuilderMap = new Dictionary<PropertyKeyword, QueryFilterBuilder.FilterBuildDelegate>();
			this.filterBuilderMap.Add(PropertyKeyword.From, new QueryFilterBuilder.FilterBuildDelegate(this.BuildFromFilter));
			this.filterBuilderMap.Add(PropertyKeyword.To, new QueryFilterBuilder.FilterBuildDelegate(this.BuildToFilter));
			this.filterBuilderMap.Add(PropertyKeyword.Bcc, new QueryFilterBuilder.FilterBuildDelegate(this.BuildBccFilter));
			this.filterBuilderMap.Add(PropertyKeyword.Cc, new QueryFilterBuilder.FilterBuildDelegate(this.BuildCcFilter));
			this.filterBuilderMap.Add(PropertyKeyword.Participants, new QueryFilterBuilder.FilterBuildDelegate(this.BuildParticipantsFilter));
			this.filterBuilderMap.Add(PropertyKeyword.Subject, new QueryFilterBuilder.FilterBuildDelegate(this.BuildSubjectFilter));
			this.filterBuilderMap.Add(PropertyKeyword.Body, new QueryFilterBuilder.FilterBuildDelegate(this.BuildBodyFilter));
			this.filterBuilderMap.Add(PropertyKeyword.Sent, new QueryFilterBuilder.FilterBuildDelegate(this.BuildSentFilter));
			this.filterBuilderMap.Add(PropertyKeyword.Received, new QueryFilterBuilder.FilterBuildDelegate(this.BuildReceivedFilter));
			this.filterBuilderMap.Add(PropertyKeyword.Attachment, new QueryFilterBuilder.FilterBuildDelegate(this.BuildAttachmentFilter));
			this.filterBuilderMap.Add(PropertyKeyword.AttachmentNames, new QueryFilterBuilder.FilterBuildDelegate(this.BuildAttachmentNamesFilter));
			this.filterBuilderMap.Add(PropertyKeyword.Kind, new QueryFilterBuilder.FilterBuildDelegate(this.BuildKindFilter));
			this.filterBuilderMap.Add(PropertyKeyword.PolicyTag, new QueryFilterBuilder.FilterBuildDelegate(this.BuildPolicyTagFilter));
			this.filterBuilderMap.Add(PropertyKeyword.Expires, new QueryFilterBuilder.FilterBuildDelegate(this.BuildExpiresFilter));
			this.filterBuilderMap.Add(PropertyKeyword.IsFlagged, new QueryFilterBuilder.FilterBuildDelegate(this.BuildIsFlaggedFilter));
			this.filterBuilderMap.Add(PropertyKeyword.IsRead, new QueryFilterBuilder.FilterBuildDelegate(this.BuildIsReadFilter));
			this.filterBuilderMap.Add(PropertyKeyword.Category, new QueryFilterBuilder.FilterBuildDelegate(this.BuildCategoryFilter));
			this.filterBuilderMap.Add(PropertyKeyword.Importance, new QueryFilterBuilder.FilterBuildDelegate(this.BuildImportanceFilter));
			this.filterBuilderMap.Add(PropertyKeyword.Size, new QueryFilterBuilder.FilterBuildDelegate(this.BuildSizeFilter));
			this.filterBuilderMap.Add(PropertyKeyword.HasAttachment, new QueryFilterBuilder.FilterBuildDelegate(this.BuildHasAttachmentFilter));
			this.filterBuilderMap.Add(PropertyKeyword.All, new QueryFilterBuilder.FilterBuildDelegate(this.BuildAllFilter));
		}

		public QueryFilter Build(Condition condRoot)
		{
			if (condRoot == null)
			{
				throw new ArgumentNullException("condRoot");
			}
			QueryFilter queryFilter = this.ConditionToQueryFilter(condRoot);
			QueryFilterBuilder.Tracer.TraceDebug<QueryFilter>((long)this.GetHashCode(), "QueryFilterBuilder.Build creates a filter of {0}", queryFilter);
			return queryFilter;
		}

		protected static readonly Trace Tracer = ExTraceGlobals.SearchTracer;

		private readonly CultureInfo culture;

		private Dictionary<PropertyKeyword, QueryFilterBuilder.FilterBuildDelegate> filterBuilderMap;

		private RescopedAll rescopedAll;

		private static Regex guidRegex = new Regex("^(\\{){0,1}[0-9a-fA-F]{8}\\-[0-9a-fA-F]{4}\\-[0-9a-fA-F]{4}\\-[0-9a-fA-F]{4}\\-[0-9a-fA-F]{12}(\\}){0,1}$", RegexOptions.Compiled);

		private readonly AqsParser.ParseOption options;

		private static readonly QueryFilter importanceHighFilter = new ComparisonFilter(ComparisonOperator.GreaterThanOrEqual, ItemSchema.Importance, 2);

		private static readonly QueryFilter importanceNormalFilter = new AndFilter(new QueryFilter[]
		{
			new ComparisonFilter(ComparisonOperator.GreaterThanOrEqual, ItemSchema.Importance, 1),
			new ComparisonFilter(ComparisonOperator.LessThan, ItemSchema.Importance, 2)
		});

		private static readonly QueryFilter importanceLowFilter = new AndFilter(new QueryFilter[]
		{
			new ComparisonFilter(ComparisonOperator.GreaterThanOrEqual, ItemSchema.Importance, 0),
			new ComparisonFilter(ComparisonOperator.LessThan, ItemSchema.Importance, 1)
		});

		private PropertyDefinition[] fromMapping = new PropertyDefinition[]
		{
			ItemSchema.SearchSender
		};

		private PropertyDefinition[] toMapping = new PropertyDefinition[]
		{
			ItemSchema.SearchRecipientsTo
		};

		private PropertyDefinition[] ccMapping = new PropertyDefinition[]
		{
			ItemSchema.SearchRecipientsCc
		};

		private PropertyDefinition[] bccMapping = new PropertyDefinition[]
		{
			ItemSchema.SearchRecipientsBcc
		};

		private PropertyDefinition[] participantsMapping = new PropertyDefinition[]
		{
			ItemSchema.SearchRecipients,
			ItemSchema.SearchSender
		};

		private PropertyDefinition[] bodyMapping = new PropertyDefinition[]
		{
			ItemSchema.TextBody
		};

		private PropertyDefinition[] subjectMapping = new PropertyDefinition[]
		{
			ItemSchema.Subject
		};

		private PropertyDefinition[] attachmentMapping = new PropertyDefinition[]
		{
			ItemSchema.AttachmentContent
		};

		private PropertyDefinition[] attachmentSubMapping = new PropertyDefinition[]
		{
			AttachmentSchema.AttachFileName,
			AttachmentSchema.AttachLongFileName,
			AttachmentSchema.AttachExtension,
			AttachmentSchema.DisplayName
		};

		private PropertyDefinition[] attachmentNamesMapping = new PropertyDefinition[]
		{
			AttachmentSchema.AttachLongFileName
		};

		private PropertyDefinition[] sentMapping = new PropertyDefinition[]
		{
			ItemSchema.SentTime
		};

		private PropertyDefinition[] receivedMapping = new PropertyDefinition[]
		{
			ItemSchema.ReceivedTime
		};

		private PropertyDefinition[] kindMapping = new PropertyDefinition[]
		{
			StoreObjectSchema.ItemClass
		};

		private PropertyDefinition[] policyTagMapping = new PropertyDefinition[]
		{
			StoreObjectSchema.PolicyTag
		};

		private PropertyDefinition[] expiresMapping = new PropertyDefinition[]
		{
			ItemSchema.RetentionDate
		};

		private PropertyDefinition[] hasAttachmentMapping = new PropertyDefinition[]
		{
			ItemSchema.HasAttachment
		};

		private PropertyDefinition[] isFlaggedMapping = new PropertyDefinition[]
		{
			ItemSchema.FlagStatus
		};

		private PropertyDefinition[] isReadMapping = new PropertyDefinition[]
		{
			MessageItemSchema.IsRead
		};

		private PropertyDefinition[] categoryMapping = new PropertyDefinition[]
		{
			ItemSchema.Categories
		};

		private PropertyDefinition[] importanceMapping = new PropertyDefinition[]
		{
			ItemSchema.Importance
		};

		private PropertyDefinition[] sizeMapping = new PropertyDefinition[]
		{
			ItemSchema.Size
		};

		private PropertyDefinition[] allMapping = new PropertyDefinition[]
		{
			ItemSchema.SearchAllIndexedProps
		};

		private delegate QueryFilter FilterBuildDelegate(ConditionOperation opt, object value);
	}
}
