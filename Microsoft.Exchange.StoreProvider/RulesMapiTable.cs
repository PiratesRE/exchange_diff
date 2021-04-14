using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Mapi.Unmanaged;

namespace Microsoft.Mapi
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class RulesMapiTable : MapiTable
	{
		internal RulesMapiTable(IExMapiTable iMAPITable, MapiFolder folder, MapiStore mapiStore) : base(iMAPITable, mapiStore)
		{
			this.folderEntryId = (byte[])folder.GetProp(PropTag.EntryId).Value;
			base.Restrict(Rule.MiddleTierRule, RestrictFlags.Batch);
			base.SortTable(RulesMapiTable.sortSequenceAscending, SortTableFlags.Batch);
		}

		public override MapiNotificationHandle Advise(MapiNotificationHandler handler)
		{
			throw MapiExceptionHelper.NotSupportedException("Notifications are not supported on rules table.");
		}

		public override MapiNotificationHandle Advise(MapiNotificationHandler handler, NotificationCallbackMode mode)
		{
			throw MapiExceptionHelper.NotSupportedException("Notifications are not supported on rules table.");
		}

		public override void SetColumns(ICollection<PropTag> propTags)
		{
			base.CheckDisposed();
			ICollection<PropTag> columns = RulesMapiTable.ConvertPropertyTagsFromRuleToMessage(propTags);
			base.SetColumns(columns);
		}

		public override bool FindRow(Restriction restriction, BookMark bookmark, FindRowFlag flag)
		{
			base.CheckDisposed();
			if (restriction == null)
			{
				throw MapiExceptionHelper.ArgumentNullException("restriction");
			}
			Restriction restriction2 = Restriction.And(new Restriction[]
			{
				Rule.MiddleTierRule,
				RulesMapiTable.ConvertRestrictionFromRuleToMessage(restriction)
			});
			return base.FindRow(restriction2, bookmark, flag);
		}

		public override void SortTable(SortOrder sortOrder, SortTableFlags flags)
		{
			throw MapiExceptionHelper.NotSupportedException("Sorting is not supported on rules table.");
		}

		public override void Restrict(Restriction restriction, RestrictFlags flags)
		{
			base.CheckDisposed();
			Restriction restriction2 = RulesMapiTable.ConvertRestrictionFromRuleToMessage(restriction);
			Restriction restriction3 = Restriction.And(new Restriction[]
			{
				restriction2,
				Rule.MiddleTierRule
			});
			base.Restrict(restriction3, flags);
		}

		public override PropValue[][] QueryRows(int crows, QueryRowsFlags flags)
		{
			base.CheckDisposed();
			PropValue[][] messagePropertyValues = base.QueryRows(crows, flags);
			PropValue[][] result;
			using (MapiFolder mapiFolder = (MapiFolder)base.MapiStore.OpenEntry(this.folderEntryId))
			{
				result = RulesMapiTable.ConvertPropertyValuesFromMessageToRule(mapiFolder, messagePropertyValues);
			}
			return result;
		}

		public override PropValue[][] ExpandRow(long categoryId, int maxRows, int flags, out int expandedRows)
		{
			throw MapiExceptionHelper.NotSupportedException("Expand/Collapse is not supported on rules table.");
		}

		public override int CollapseRow(long categoryId, int flags)
		{
			throw MapiExceptionHelper.NotSupportedException("Expand/Collapse is not supported on rules table.");
		}

		public override BookMark CreateBookmark()
		{
			throw MapiExceptionHelper.NotSupportedException("Bookmarks are not supported on rules table.");
		}

		public override void FreeBookmark(BookMark bookmark)
		{
			throw MapiExceptionHelper.NotSupportedException("Bookmarks are not supported on rules table.");
		}

		public override byte[] GetCollapseState(byte[] instanceKey)
		{
			throw MapiExceptionHelper.NotSupportedException("Expand/Collapse is not supported on rules table.");
		}

		public override BookMark SetCollapseState(byte[] collapseState)
		{
			throw MapiExceptionHelper.NotSupportedException("Expand/Collapse is not supported on rules table.");
		}

		public override PropTag[] QueryColumns(QueryColumnsFlags flags)
		{
			base.CheckDisposed();
			ICollection<PropTag> messagePropertyTags = base.QueryColumns(flags);
			return RulesMapiTable.ConvertPropertyTagsFromMessageToRule(messagePropertyTags).ToArray<PropTag>();
		}

		public override void Unadvise(MapiNotificationHandle handle)
		{
			throw MapiExceptionHelper.NotSupportedException("Notifications are not supported on rules table.");
		}

		private static PropTag ConvertPropertyTagFromRuleToMessage(PropTag ruleTag)
		{
			PropTag result;
			if (!RulesMapiTable.mapFromRuleToMessagePropTag.TryGetValue(ruleTag, out result))
			{
				result = ruleTag;
			}
			return result;
		}

		private static ICollection<PropTag> ConvertPropertyTagsFromRuleToMessage(ICollection<PropTag> rulePropertyTags)
		{
			List<PropTag> list = new List<PropTag>();
			foreach (PropTag ruleTag in rulePropertyTags)
			{
				list.Add(RulesMapiTable.ConvertPropertyTagFromRuleToMessage(ruleTag));
			}
			return list;
		}

		private static ICollection<PropTag> ConvertPropertyTagsFromMessageToRule(ICollection<PropTag> messagePropertyTags)
		{
			List<PropTag> list = new List<PropTag>();
			foreach (PropTag propTag in messagePropertyTags)
			{
				PropTag item;
				if (!RulesMapiTable.mapFromMessageToRulePropTag.TryGetValue(propTag, out item))
				{
					item = propTag;
				}
				list.Add(item);
			}
			return list;
		}

		private static Restriction ConvertRestrictionFromRuleToMessage(Restriction ruleRestriction)
		{
			ruleRestriction.EnumerateRestriction(delegate(Restriction restriction, object ctx)
			{
				switch (restriction.Type)
				{
				case Restriction.ResType.Content:
				{
					Restriction.ContentRestriction contentRestriction = (Restriction.ContentRestriction)restriction;
					contentRestriction.PropValue = RulesMapiTable.ConvertPropertyValueFromRuleToMessage(contentRestriction.PropValue);
					contentRestriction.PropTag = RulesMapiTable.ConvertPropertyTagFromRuleToMessage(contentRestriction.PropTag);
					return;
				}
				case Restriction.ResType.Property:
				{
					Restriction.PropertyRestriction propertyRestriction = (Restriction.PropertyRestriction)restriction;
					propertyRestriction.PropValue = RulesMapiTable.ConvertPropertyValueFromRuleToMessage(propertyRestriction.PropValue);
					propertyRestriction.PropTag = RulesMapiTable.ConvertPropertyTagFromRuleToMessage(propertyRestriction.PropTag);
					return;
				}
				case Restriction.ResType.CompareProps:
				{
					Restriction.ComparePropertyRestriction comparePropertyRestriction = (Restriction.ComparePropertyRestriction)restriction;
					comparePropertyRestriction.TagLeft = RulesMapiTable.ConvertPropertyTagFromRuleToMessage(comparePropertyRestriction.TagLeft);
					comparePropertyRestriction.TagRight = RulesMapiTable.ConvertPropertyTagFromRuleToMessage(comparePropertyRestriction.TagRight);
					return;
				}
				case Restriction.ResType.BitMask:
				{
					Restriction.BitMaskRestriction bitMaskRestriction = (Restriction.BitMaskRestriction)restriction;
					bitMaskRestriction.Tag = RulesMapiTable.ConvertPropertyTagFromRuleToMessage(bitMaskRestriction.Tag);
					return;
				}
				case Restriction.ResType.Size:
				{
					Restriction.SizeRestriction sizeRestriction = (Restriction.SizeRestriction)restriction;
					sizeRestriction.Tag = RulesMapiTable.ConvertPropertyTagFromRuleToMessage(sizeRestriction.Tag);
					return;
				}
				case Restriction.ResType.Exist:
				{
					Restriction.ExistRestriction existRestriction = (Restriction.ExistRestriction)restriction;
					existRestriction.Tag = RulesMapiTable.ConvertPropertyTagFromRuleToMessage(existRestriction.Tag);
					return;
				}
				case Restriction.ResType.SubRestriction:
					break;
				case Restriction.ResType.Comment:
				{
					Restriction.CommentRestriction commentRestriction = (Restriction.CommentRestriction)restriction;
					PropValue[] values = commentRestriction.Values;
					for (int i = 0; i < values.Length; i++)
					{
						values[i] = RulesMapiTable.ConvertPropertyValueFromRuleToMessage(values[i]);
					}
					break;
				}
				default:
					return;
				}
			}, null);
			return ruleRestriction;
		}

		private static PropValue[][] ConvertPropertyValuesFromMessageToRule(MapiFolder folder, PropValue[][] messagePropertyValues)
		{
			foreach (PropValue[] array in messagePropertyValues)
			{
				for (int j = 0; j < array.Length; j++)
				{
					PropTag propTag = array[j].PropTag;
					PropTag propTag2;
					if (RulesMapiTable.mapFromMessageToRulePropTag.TryGetValue(propTag, out propTag2))
					{
						object obj = array[j].Value;
						if (propTag2 == PropTag.RuleActions)
						{
							obj = folder.DeserializeActions((byte[])obj);
						}
						else if (propTag2 == PropTag.RuleCondition)
						{
							obj = folder.DeserializeRestriction((byte[])obj);
						}
						array[j] = new PropValue(propTag2, obj);
					}
					else if (array[j].PropTag.Id() == Rule.PR_EX_RULE_CONDITION.Id() && array[j].IsError() && array[j].GetErrorValue() == -2147221233)
					{
						array[j] = new PropValue(PropTag.RuleCondition, null);
					}
				}
			}
			return messagePropertyValues;
		}

		private static PropValue ConvertPropertyValueFromRuleToMessage(PropValue rulePropertyValue)
		{
			PropTag propTag = rulePropertyValue.PropTag;
			PropTag propTag2;
			if (RulesMapiTable.mapFromRuleToMessagePropTag.TryGetValue(propTag, out propTag2))
			{
				return new PropValue(propTag2, rulePropertyValue.Value);
			}
			return rulePropertyValue;
		}

		private static Dictionary<PropTag, PropTag> BuildPropTagMap(PropTag[] from, PropTag[] to)
		{
			Dictionary<PropTag, PropTag> dictionary = new Dictionary<PropTag, PropTag>();
			for (int i = 0; i < from.Length; i++)
			{
				dictionary.Add(from[i], to[i]);
			}
			return dictionary;
		}

		private static readonly PropTag[] rulePropTags = new PropTag[]
		{
			PropTag.RuleID,
			PropTag.RuleSequence,
			PropTag.RuleState,
			PropTag.RuleUserFlags,
			PropTag.RuleProvider,
			PropTag.RuleName,
			PropTag.RuleLevel,
			PropTag.RuleProviderData,
			PropTag.RuleActions,
			PropTag.RuleCondition
		};

		private static readonly PropTag[] messagePropTags = new PropTag[]
		{
			Rule.PR_EX_RULE_ID,
			Rule.PR_EX_RULE_SEQUENCE,
			Rule.PR_EX_RULE_STATE,
			Rule.PR_EX_RULE_USER_FLAGS,
			Rule.PR_EX_RULE_PROVIDER,
			Rule.PR_EX_RULE_NAME,
			Rule.PR_EX_RULE_LEVEL,
			Rule.PR_EX_RULE_PROVIDER_DATA,
			Rule.PR_EX_RULE_ACTIONS,
			Rule.PR_EX_RULE_CONDITION
		};

		private static Dictionary<PropTag, PropTag> mapFromRuleToMessagePropTag = RulesMapiTable.BuildPropTagMap(RulesMapiTable.rulePropTags, RulesMapiTable.messagePropTags);

		private static Dictionary<PropTag, PropTag> mapFromMessageToRulePropTag = RulesMapiTable.BuildPropTagMap(RulesMapiTable.messagePropTags, RulesMapiTable.rulePropTags);

		private static readonly SortOrder sortSequenceAscending = new SortOrder(Rule.PR_EX_RULE_SEQUENCE, SortFlags.Ascend);

		private readonly byte[] folderEntryId;
	}
}
