using System;
using System.Collections.Generic;
using System.Globalization;
using System.Management.Automation;
using System.Text.RegularExpressions;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Mapi;

namespace Microsoft.Exchange.MailboxReplicationService
{
	internal static class ContentFilterBuilder
	{
		public static void ProcessContentFilter(string contentFilter, int contentFilterLCID, PSCmdlet cmdlet, IFilterBuilderHelper mapper, out RestrictionData restriction, out string normalizedFilter)
		{
			Exception ex = null;
			try
			{
				QueryParser queryParser = new QueryParser(contentFilter, ObjectSchema.GetInstance<ContentFilterSchema>(), QueryParser.Capabilities.All, (cmdlet != null) ? new QueryParser.EvaluateVariableDelegate(cmdlet.GetVariableValue) : null, new QueryParser.ConvertValueFromStringDelegate(ContentFilterBuilder.ConvertValueFromString));
				QueryFilter parseTree = queryParser.ParseTree;
				Restriction restriction2 = ContentFilterBuilder.BuildRestriction(parseTree, mapper);
				restriction = RestrictionData.GetRestrictionData(restriction2);
				restriction.LCID = contentFilterLCID;
				normalizedFilter = parseTree.GenerateInfixString(FilterLanguage.Monad);
				return;
			}
			catch (InvalidCastException ex2)
			{
				ex = ex2;
			}
			catch (ParsingException ex3)
			{
				ex = ex3;
			}
			catch (ArgumentOutOfRangeException ex4)
			{
				ex = ex4;
			}
			throw new InvalidContentFilterPermanentException(ex.Message, ex);
		}

		internal static Restriction BuildPropertyRestriction(SinglePropertyFilter filter, IFilterBuilderHelper mapper)
		{
			return ContentFilterBuilder.BuildRestrictionWrapper(filter, PropTag.Null, mapper, new ContentFilterBuilder.BuildRestrictionDelegate(ContentFilterBuilder.BuildPropertyRestrictionInternal));
		}

		internal static Restriction BuildRecipientRestriction(SinglePropertyFilter filter, IFilterBuilderHelper mapper)
		{
			return ContentFilterBuilder.BuildRestrictionWrapper(filter, PropTag.Null, mapper, new ContentFilterBuilder.BuildRestrictionDelegate(ContentFilterBuilder.BuildRecipientRestrictionInternal));
		}

		internal static Restriction BuildAttachmentRestriction(SinglePropertyFilter filter, IFilterBuilderHelper mapper)
		{
			Restriction restriction = ContentFilterBuilder.BuildRestrictionWrapper(filter, PropTag.SearchAttachments, mapper, new ContentFilterBuilder.BuildRestrictionDelegate(ContentFilterBuilder.BuildPropertyRestrictionInternal));
			Restriction restriction2 = Restriction.Or(new Restriction[]
			{
				ContentFilterBuilder.BuildRestrictionWrapper(filter, PropTag.AttachFileName, mapper, new ContentFilterBuilder.BuildRestrictionDelegate(ContentFilterBuilder.BuildPropertyRestrictionInternal)),
				ContentFilterBuilder.BuildRestrictionWrapper(filter, PropTag.AttachLongFileName, mapper, new ContentFilterBuilder.BuildRestrictionDelegate(ContentFilterBuilder.BuildPropertyRestrictionInternal)),
				ContentFilterBuilder.BuildRestrictionWrapper(filter, PropTag.AttachExtension, mapper, new ContentFilterBuilder.BuildRestrictionDelegate(ContentFilterBuilder.BuildPropertyRestrictionInternal)),
				ContentFilterBuilder.BuildRestrictionWrapper(filter, PropTag.DisplayName, mapper, new ContentFilterBuilder.BuildRestrictionDelegate(ContentFilterBuilder.BuildPropertyRestrictionInternal))
			});
			return Restriction.Or(new Restriction[]
			{
				restriction,
				Restriction.Sub(PropTag.MessageAttachments, restriction2)
			});
		}

		internal static Restriction BuildMessageKindRestriction(SinglePropertyFilter filter, IFilterBuilderHelper mapper)
		{
			return ContentFilterBuilder.BuildRestrictionWrapper(filter, PropTag.MessageClass, mapper, new ContentFilterBuilder.BuildRestrictionDelegate(ContentFilterBuilder.BuildMessageKindRestrictionInternal));
		}

		internal static Restriction BuildPolicyTagRestriction(SinglePropertyFilter filter, IFilterBuilderHelper mapper)
		{
			return ContentFilterBuilder.BuildRestrictionWrapper(filter, PropTag.PolicyTag, mapper, new ContentFilterBuilder.BuildRestrictionDelegate(ContentFilterBuilder.BuildPolicyTagRestrictionInternal));
		}

		internal static Restriction BuildIsFlaggedRestriction(SinglePropertyFilter filter, IFilterBuilderHelper mapper)
		{
			ContentFilterSchema.ContentFilterPropertyDefinition contentFilterPropertyDefinition = (ContentFilterSchema.ContentFilterPropertyDefinition)filter.Property;
			PropTag propTagToSearch = contentFilterPropertyDefinition.PropTagToSearch;
			bool flag;
			ContentFilterBuilder.CheckFilterIsEQorNE(filter, out flag);
			bool flag2 = (bool)ContentFilterBuilder.GetPropertyValue(filter);
			if (flag)
			{
				flag2 = !flag2;
			}
			Restriction restriction = Restriction.EQ(propTagToSearch, 2);
			if (!flag2)
			{
				return Restriction.Not(restriction);
			}
			return restriction;
		}

		internal static Restriction BuildIsReadRestriction(SinglePropertyFilter filter, IFilterBuilderHelper mapper)
		{
			ContentFilterSchema.ContentFilterPropertyDefinition contentFilterPropertyDefinition = (ContentFilterSchema.ContentFilterPropertyDefinition)filter.Property;
			PropTag propTagToSearch = contentFilterPropertyDefinition.PropTagToSearch;
			bool flag;
			ContentFilterBuilder.CheckFilterIsEQorNE(filter, out flag);
			bool flag2 = (bool)ContentFilterBuilder.GetPropertyValue(filter);
			if (flag)
			{
				flag2 = !flag2;
			}
			if (!flag2)
			{
				return Restriction.BitMaskZero(propTagToSearch, 1);
			}
			return Restriction.BitMaskNonZero(propTagToSearch, 1);
		}

		private static Restriction BuildRestriction(QueryFilter filter, IFilterBuilderHelper helper)
		{
			CompositeFilter compositeFilter = filter as CompositeFilter;
			if (compositeFilter != null)
			{
				Restriction[] array = new Restriction[compositeFilter.FilterCount];
				int num = 0;
				foreach (QueryFilter filter2 in compositeFilter.Filters)
				{
					array[num++] = ContentFilterBuilder.BuildRestriction(filter2, helper);
				}
				if (compositeFilter is AndFilter)
				{
					return Restriction.And(array);
				}
				if (compositeFilter is OrFilter)
				{
					return Restriction.Or(array);
				}
				throw ContentFilterBuilder.UnexpectedFilterType(filter);
			}
			else
			{
				NotFilter notFilter = filter as NotFilter;
				if (notFilter != null)
				{
					return Restriction.Not(ContentFilterBuilder.BuildRestriction(notFilter.Filter, helper));
				}
				SinglePropertyFilter singlePropertyFilter = filter as SinglePropertyFilter;
				if (singlePropertyFilter == null)
				{
					throw ContentFilterBuilder.UnexpectedFilterType(filter);
				}
				ContentFilterSchema.ContentFilterPropertyDefinition contentFilterPropertyDefinition = singlePropertyFilter.Property as ContentFilterSchema.ContentFilterPropertyDefinition;
				if (contentFilterPropertyDefinition == null)
				{
					throw ContentFilterBuilder.UnexpectedFilterType(filter);
				}
				return contentFilterPropertyDefinition.ConvertToRestriction(singlePropertyFilter, helper);
			}
		}

		private static object GetPropertyValue(QueryFilter filter)
		{
			TextFilter textFilter = filter as TextFilter;
			if (textFilter != null)
			{
				return textFilter.Text;
			}
			ComparisonFilter comparisonFilter = filter as ComparisonFilter;
			if (comparisonFilter == null)
			{
				throw ContentFilterBuilder.UnexpectedFilterType(filter);
			}
			object obj = comparisonFilter.PropertyValue;
			if (obj == null)
			{
				return null;
			}
			if (obj is DateTime)
			{
				obj = ((DateTime)obj).ToUniversalTime();
			}
			else if (obj is ByteQuantifiedSize)
			{
				obj = (int)((ulong)((ByteQuantifiedSize)obj));
			}
			else if (obj is CultureInfo)
			{
				obj = ((CultureInfo)obj).LCID;
			}
			else if (obj.GetType().IsEnum)
			{
				obj = (int)obj;
			}
			return obj;
		}

		private static Restriction BuildRestrictionWrapper(SinglePropertyFilter filter, PropTag ptagToSearch, IFilterBuilderHelper mapper, ContentFilterBuilder.BuildRestrictionDelegate restrictionBuilder)
		{
			ContentFilterSchema.ContentFilterPropertyDefinition contentFilterPropertyDefinition = (ContentFilterSchema.ContentFilterPropertyDefinition)filter.Property;
			if (ptagToSearch == PropTag.Null)
			{
				if (contentFilterPropertyDefinition.NamedPropToSearch != null)
				{
					ptagToSearch = mapper.MapNamedProperty(contentFilterPropertyDefinition.NamedPropToSearch, contentFilterPropertyDefinition.NamedPropType);
				}
				else
				{
					ptagToSearch = contentFilterPropertyDefinition.PropTagToSearch;
				}
			}
			if (filter is ExistsFilter)
			{
				return Restriction.Exist(ptagToSearch);
			}
			return restrictionBuilder(filter, mapper, ptagToSearch);
		}

		private static Restriction BuildPropertyRestrictionInternal(SinglePropertyFilter filter, IFilterBuilderHelper mapper, PropTag ptagToSearch)
		{
			return ContentFilterBuilder.BuildBasicRestriction(filter, ptagToSearch, ContentFilterBuilder.GetPropertyValue(filter));
		}

		private static Restriction BuildRecipientRestrictionInternal(SinglePropertyFilter filter, IFilterBuilderHelper mapper, PropTag ptagToSearch)
		{
			List<Restriction> list = new List<Restriction>();
			bool flag;
			ContentFilterBuilder.CheckFilterIsEQorNE(filter, out flag);
			object propertyValue = ContentFilterBuilder.GetPropertyValue(filter);
			list.Add(Restriction.Content(ptagToSearch, propertyValue, ContentFlags.Prefix | ContentFlags.IgnoreCase));
			string text = propertyValue as string;
			if (text != null)
			{
				string[] array = mapper.MapRecipient(text);
				if (array != null)
				{
					foreach (string text2 in array)
					{
						if (!StringComparer.OrdinalIgnoreCase.Equals(text2, text))
						{
							list.Add(Restriction.Content(ptagToSearch, text2, ContentFlags.Prefix | ContentFlags.IgnoreCase));
						}
					}
				}
			}
			Restriction restriction;
			if (list.Count == 1)
			{
				restriction = list[0];
			}
			else
			{
				restriction = Restriction.Or(list.ToArray());
			}
			if (!flag)
			{
				return restriction;
			}
			return Restriction.Not(restriction);
		}

		private static Restriction BuildMessageKindRestrictionInternal(SinglePropertyFilter filter, IFilterBuilderHelper mapper, PropTag ptagToSearch)
		{
			List<string> list = new List<string>();
			bool flag;
			ContentFilterBuilder.CheckFilterIsEQorNE(filter, out flag);
			switch ((MessageKindEnum)ContentFilterBuilder.GetPropertyValue(filter))
			{
			case MessageKindEnum.Email:
				list.Add("IPM.Note");
				break;
			case MessageKindEnum.Calendar:
				list.Add("IPM.Schedule");
				list.Add("IPM.Appointment");
				break;
			case MessageKindEnum.Task:
				list.Add("IPM.Task");
				break;
			case MessageKindEnum.Note:
				list.Add("IPM.StickyNote");
				break;
			case MessageKindEnum.Doc:
				list.Add("IPM.Document");
				break;
			case MessageKindEnum.Journal:
				list.Add("IPM.Activity");
				break;
			case MessageKindEnum.Contact:
				list.Add("IPM.Contact");
				break;
			case MessageKindEnum.InstantMessage:
				list.Add("IPM.Note.Microsoft.Conversation");
				list.Add("IPM.Note.Microsoft.Missed");
				list.Add("IPM.Note.Microsoft.Conversation.Voice");
				list.Add("IPM.Note.Microsoft.Missed.Voice");
				break;
			case MessageKindEnum.Voicemail:
				list.Add("IPM.Note.Microsoft.Voicemail");
				break;
			case MessageKindEnum.Fax:
				list.Add("IPM.Note.Microsoft.Fax");
				break;
			case MessageKindEnum.Post:
				list.Add("IPM.Post");
				break;
			case MessageKindEnum.RSSFeed:
				list.Add("IPM.Post.RSS");
				break;
			default:
				throw ContentFilterBuilder.UnexpectedFilterType(filter);
			}
			Restriction[] array = new Restriction[list.Count];
			for (int i = 0; i < list.Count; i++)
			{
				array[i] = Restriction.EQ(ptagToSearch, list[i]);
			}
			Restriction restriction = (array.Length == 1) ? array[0] : Restriction.Or(array);
			if (!flag)
			{
				return restriction;
			}
			return Restriction.Not(restriction);
		}

		private static void CheckFilterIsEQorNE(SinglePropertyFilter filter, out bool isNE)
		{
			ComparisonFilter comparisonFilter = filter as ComparisonFilter;
			if (comparisonFilter == null || (comparisonFilter.ComparisonOperator != ComparisonOperator.Equal && comparisonFilter.ComparisonOperator != ComparisonOperator.NotEqual))
			{
				throw new FilterOperatorMustBeEQorNEPermanentException(filter.Property.Name);
			}
			isNE = (comparisonFilter.ComparisonOperator == ComparisonOperator.NotEqual);
		}

		private static Restriction BuildPolicyTagRestrictionInternal(SinglePropertyFilter filter, IFilterBuilderHelper mapper, PropTag ptagToSearch)
		{
			bool flag;
			ContentFilterBuilder.CheckFilterIsEQorNE(filter, out flag);
			List<Restriction> list = new List<Restriction>();
			string text = (string)ContentFilterBuilder.GetPropertyValue(filter);
			if (ContentFilterBuilder.guidRegex.Match(text).Success)
			{
				try
				{
					Guid guid = new Guid(text);
					list.Add(Restriction.EQ(ptagToSearch, guid.ToByteArray()));
				}
				catch (FormatException)
				{
				}
			}
			if (list.Count == 0)
			{
				Guid[] array = mapper.MapPolicyTag(text);
				if (array != null)
				{
					foreach (Guid guid2 in array)
					{
						list.Add(Restriction.EQ(ptagToSearch, guid2.ToByteArray()));
					}
				}
			}
			Restriction restriction = (list.Count == 1) ? list[0] : Restriction.Or(list.ToArray());
			if (!flag)
			{
				return restriction;
			}
			return Restriction.Not(restriction);
		}

		private static Restriction BuildBasicRestriction(QueryFilter filter, PropTag ptagToSearch, object propValue)
		{
			TextFilter textFilter = filter as TextFilter;
			if (textFilter != null)
			{
				return ContentFilterBuilder.BuildTextRestriction(textFilter, ptagToSearch, propValue);
			}
			ComparisonFilter comparisonFilter = filter as ComparisonFilter;
			if (comparisonFilter != null)
			{
				return ContentFilterBuilder.BuildComparisonRestriction(comparisonFilter, ptagToSearch, propValue);
			}
			return null;
		}

		private static Restriction BuildTextRestriction(TextFilter filter, PropTag ptagToSearch, object propValue)
		{
			ContentFlags contentFlags;
			switch (filter.MatchOptions)
			{
			case MatchOptions.FullString:
				contentFlags = ContentFlags.FullString;
				break;
			case MatchOptions.SubString:
			case MatchOptions.Suffix:
			case MatchOptions.WildcardString:
				contentFlags = ContentFlags.SubString;
				break;
			case MatchOptions.Prefix:
				contentFlags = ContentFlags.Prefix;
				break;
			case MatchOptions.PrefixOnWords:
				contentFlags = ContentFlags.PrefixOnWords;
				break;
			case MatchOptions.ExactPhrase:
				contentFlags = ContentFlags.ExactPhrase;
				break;
			default:
				contentFlags = ContentFlags.FullString;
				break;
			}
			if ((filter.MatchFlags & MatchFlags.IgnoreCase) != MatchFlags.Default)
			{
				contentFlags |= ContentFlags.IgnoreCase;
			}
			if ((filter.MatchFlags & MatchFlags.IgnoreNonSpace) != MatchFlags.Default)
			{
				contentFlags |= ContentFlags.IgnoreNonSpace;
			}
			if ((filter.MatchFlags & MatchFlags.Loose) != MatchFlags.Default)
			{
				contentFlags |= ContentFlags.Loose;
			}
			return Restriction.Content(ptagToSearch, propValue, contentFlags);
		}

		private static Restriction BuildComparisonRestriction(ComparisonFilter filter, PropTag ptagToSearch, object propValue)
		{
			Restriction.PropertyRestriction propertyRestriction;
			switch (filter.ComparisonOperator)
			{
			case ComparisonOperator.Equal:
				propertyRestriction = (Restriction.PropertyRestriction)Restriction.EQ(ptagToSearch, propValue);
				break;
			case ComparisonOperator.NotEqual:
				propertyRestriction = (Restriction.PropertyRestriction)Restriction.NE(ptagToSearch, propValue);
				break;
			case ComparisonOperator.LessThan:
				propertyRestriction = (Restriction.PropertyRestriction)Restriction.LT(ptagToSearch, propValue);
				break;
			case ComparisonOperator.LessThanOrEqual:
				propertyRestriction = (Restriction.PropertyRestriction)Restriction.LE(ptagToSearch, propValue);
				break;
			case ComparisonOperator.GreaterThan:
				propertyRestriction = (Restriction.PropertyRestriction)Restriction.GT(ptagToSearch, propValue);
				break;
			case ComparisonOperator.GreaterThanOrEqual:
				propertyRestriction = (Restriction.PropertyRestriction)Restriction.GE(ptagToSearch, propValue);
				break;
			default:
				throw ContentFilterBuilder.UnexpectedFilterType(filter);
			}
			if (ptagToSearch.IsMultiValued())
			{
				propertyRestriction.MultiValued = true;
			}
			return propertyRestriction;
		}

		private static Exception UnexpectedFilterType(QueryFilter filter)
		{
			return new UnexpectedFilterTypePermanentException(filter.GetType().Name);
		}

		private static object ConvertValueFromString(object valueToConvert, Type resultType)
		{
			string value = valueToConvert as string;
			bool flag;
			if (!string.IsNullOrEmpty(value) && resultType.Equals(typeof(bool)) && bool.TryParse(value, out flag))
			{
				return flag;
			}
			return MonadFilter.ConvertValueFromString(valueToConvert, resultType);
		}

		private static Regex guidRegex = new Regex("^(\\{){0,1}[0-9a-fA-F]{8}\\-[0-9a-fA-F]{4}\\-[0-9a-fA-F]{4}\\-[0-9a-fA-F]{4}\\-[0-9a-fA-F]{12}(\\}){0,1}$", RegexOptions.Compiled);

		private delegate Restriction BuildRestrictionDelegate(SinglePropertyFilter filter, IFilterBuilderHelper mapper, PropTag ptagToSearch);
	}
}
