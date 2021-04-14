using System;
using System.Globalization;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Mapi;

namespace Microsoft.Exchange.Data.Storage.MailboxRules
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal static class RestrictionEvaluator
	{
		public static bool Evaluate(Restriction restriction, IRuleEvaluationContext context)
		{
			if (restriction == null)
			{
				context.TraceDebug<string>("Empty restriction evaluated to true in rule {0}", context.CurrentRule.Name);
				return true;
			}
			Restriction.ResType type = restriction.Type;
			switch (type)
			{
			case Restriction.ResType.And:
				return RestrictionEvaluator.EvaluateAndRestriction((Restriction.AndRestriction)restriction, context);
			case Restriction.ResType.Or:
				return RestrictionEvaluator.EvaluateOrRestriction((Restriction.OrRestriction)restriction, context);
			case Restriction.ResType.Not:
				return RestrictionEvaluator.EvaluateNotRestriction((Restriction.NotRestriction)restriction, context);
			case Restriction.ResType.Content:
				return RestrictionEvaluator.EvaluateContentRestriction((Restriction.ContentRestriction)restriction, context);
			case Restriction.ResType.Property:
				return RestrictionEvaluator.EvaluatePropertyRestriction((Restriction.PropertyRestriction)restriction, context);
			case Restriction.ResType.CompareProps:
				return RestrictionEvaluator.EvaluateComparePropertyRestriction((Restriction.ComparePropertyRestriction)restriction, context);
			case Restriction.ResType.BitMask:
				return RestrictionEvaluator.EvaluateBitMaskRestriction((Restriction.BitMaskRestriction)restriction, context);
			case Restriction.ResType.Size:
				return RestrictionEvaluator.EvaluateSizeRestriction((Restriction.SizeRestriction)restriction, context);
			case Restriction.ResType.Exist:
				return RestrictionEvaluator.EvaluateExistRestriction((Restriction.ExistRestriction)restriction, context);
			case Restriction.ResType.SubRestriction:
				if (restriction is Restriction.AttachmentRestriction)
				{
					return RestrictionEvaluator.EvaluateAttachmentRestriction((Restriction.AttachmentRestriction)restriction, context);
				}
				if (restriction is Restriction.RecipientRestriction)
				{
					return RestrictionEvaluator.EvaluateRecipientRestriction((Restriction.RecipientRestriction)restriction, context);
				}
				throw new InvalidRuleException(string.Format("SubRestriction Type {0} is not supported!", restriction.GetType()));
			case Restriction.ResType.Comment:
				return RestrictionEvaluator.EvaluateCommentRestriction((Restriction.CommentRestriction)restriction, context);
			case Restriction.ResType.Count:
				return RestrictionEvaluator.EvaluateCountRestriction((Restriction.CountRestriction)restriction, context);
			default:
				switch (type)
				{
				case Restriction.ResType.True:
					context.TraceFunction("True Restriction");
					return true;
				case Restriction.ResType.False:
					context.TraceFunction("False Restriction");
					return false;
				default:
					throw new InvalidRuleException(string.Format("Restriction Type {0} is not supported!", restriction.Type));
				}
				break;
			}
		}

		internal static int? CompareValue(CultureInfo cultureInfo, PropTag tag, object x, object y)
		{
			int? result = null;
			PropType propType = tag.ValueType();
			if (propType <= PropType.String)
			{
				switch (propType)
				{
				case PropType.Short:
					result = new int?(((short)x).CompareTo((short)y));
					break;
				case PropType.Int:
					result = new int?(((int)x).CompareTo((int)y));
					break;
				case PropType.Float:
					result = new int?(((float)x).CompareTo((float)y));
					break;
				case PropType.Double:
				case PropType.AppTime:
					result = new int?(((double)x).CompareTo((double)y));
					break;
				case PropType.Currency:
					result = new int?(((long)x).CompareTo((long)y));
					break;
				case (PropType)8:
				case (PropType)9:
				case PropType.Error:
					break;
				case PropType.Boolean:
					result = new int?(((bool)x).CompareTo((bool)y));
					break;
				default:
					if (propType != PropType.Long)
					{
						switch (propType)
						{
						case PropType.AnsiString:
						case PropType.String:
							result = new int?(cultureInfo.CompareInfo.Compare((string)x, (string)y, CompareOptions.IgnoreCase | CompareOptions.IgnoreNonSpace | CompareOptions.IgnoreKanaType | CompareOptions.IgnoreWidth));
							break;
						}
					}
					else if (RuleUtil.IsLongID(tag))
					{
						result = new int?(RuleUtil.CompareBytes((long)x, (long)y));
					}
					else
					{
						result = new int?(((long)x).CompareTo((long)y));
					}
					break;
				}
			}
			else if (propType != PropType.SysTime)
			{
				if (propType != PropType.Guid)
				{
					if (propType == PropType.Binary)
					{
						byte[] x2 = (byte[])x;
						byte[] y2 = (byte[])y;
						if (RuleUtil.IsEntryIdProp(tag) && RuleUtil.EqualsEntryId(x2, y2))
						{
							result = new int?(0);
						}
						else
						{
							result = new int?(RuleUtil.CompareBytes(x2, y2));
						}
					}
				}
				else
				{
					result = new int?(RuleUtil.CompareBytes((Guid)x, (Guid)y));
				}
			}
			else
			{
				result = new int?((x is ExDateTime) ? ((DateTime)((ExDateTime)x).ToUtc()).CompareTo((DateTime)y) : ((DateTime)x).CompareTo((DateTime)y));
			}
			return result;
		}

		internal static bool GetOperationResultFromOrder(Restriction.RelOp op, int order)
		{
			if (order < 0)
			{
				return op == Restriction.RelOp.LessThan || op == Restriction.RelOp.LessThanOrEqual || op == Restriction.RelOp.NotEqual;
			}
			if (order == 0)
			{
				return op == Restriction.RelOp.GreaterThanOrEqual || op == Restriction.RelOp.LessThanOrEqual || op == Restriction.RelOp.Equal;
			}
			return op == Restriction.RelOp.GreaterThanOrEqual || op == Restriction.RelOp.GreaterThan || op == Restriction.RelOp.NotEqual;
		}

		internal static bool IsOrderOperation(Restriction.RelOp op)
		{
			return op == Restriction.RelOp.Equal || op == Restriction.RelOp.NotEqual || op == Restriction.RelOp.LessThan || op == Restriction.RelOp.LessThanOrEqual || op == Restriction.RelOp.GreaterThan || op == Restriction.RelOp.GreaterThanOrEqual;
		}

		internal static bool IsSupportedOperation(Restriction.RelOp op)
		{
			return RestrictionEvaluator.IsOrderOperation(op) || op == Restriction.RelOp.MemberOfDL;
		}

		internal static bool MatchString(LimitChecker limitChecker, CultureInfo cultureInfo, string content, string pattern, ContentFlags flags)
		{
			if (!limitChecker.CheckAndIncrementContentRestrictionCount(1, content))
			{
				return false;
			}
			CompareOptions compareOptions = RestrictionEvaluator.GetCompareOptions(flags);
			ContentFlags contentFlags = flags & (ContentFlags.SubString | ContentFlags.Prefix);
			CompareInfo compareInfo = cultureInfo.CompareInfo;
			switch (contentFlags)
			{
			case ContentFlags.FullString:
				return compareInfo.Compare(content, pattern, compareOptions) == 0;
			case ContentFlags.SubString:
				return compareInfo.IndexOf(content, pattern, compareOptions) != -1;
			case ContentFlags.Prefix:
				return compareInfo.IsPrefix(content, pattern, compareOptions);
			default:
				throw new InvalidRuleException(string.Format("Not supported content flags {0}", flags));
			}
		}

		private static bool CompareMultiValueWithValue(IRuleEvaluationContext context, PropTag tag, Restriction.RelOp op, object messageValueObject, object ruleValue)
		{
			Array array = (Array)messageValueObject;
			bool flag = false;
			foreach (object x in array)
			{
				flag = context.CompareSingleValue(tag, op, x, ruleValue);
				if (flag)
				{
					if (op != Restriction.RelOp.NotEqual)
					{
						break;
					}
				}
				else if (op == Restriction.RelOp.NotEqual)
				{
					break;
				}
			}
			return flag;
		}

		private static bool CompareValueWithMultiValue(IRuleEvaluationContext context, PropTag tag, Restriction.RelOp op, object messageValue, object ruleValueObject)
		{
			Array array = (Array)ruleValueObject;
			bool flag = false;
			foreach (object y in array)
			{
				flag = context.CompareSingleValue(tag, Restriction.RelOp.Equal, messageValue, y);
				if (flag)
				{
					break;
				}
			}
			if (op == Restriction.RelOp.NotEqual)
			{
				flag = !flag;
			}
			return flag;
		}

		private static bool EvaluateAndRestriction(Restriction.AndRestriction restriction, IRuleEvaluationContext context)
		{
			bool flag = true;
			context.NestedLevel++;
			foreach (Restriction restriction2 in restriction.Restrictions)
			{
				if (!RestrictionEvaluator.Evaluate(restriction2, context))
				{
					flag = false;
					break;
				}
			}
			context.NestedLevel--;
			context.TraceFunction<bool>("AndRestriction evaluated to {0}", flag);
			return flag;
		}

		private static bool EvaluateAttachmentRestriction(Restriction.AttachmentRestriction restriction, IRuleEvaluationContext context)
		{
			context.TraceFunction("AttachmentRestriction");
			context.NestedLevel++;
			bool flag = false;
			foreach (AttachmentHandle handle in context.Message.AttachmentCollection)
			{
				using (Attachment attachment = context.Message.AttachmentCollection.Open(handle))
				{
					context.TraceFunction<string>("Attachment {0}", attachment.DisplayName);
					using (IRuleEvaluationContext attachmentContext = context.GetAttachmentContext(attachment))
					{
						if (RestrictionEvaluator.Evaluate(restriction.Restriction, attachmentContext))
						{
							flag = true;
							break;
						}
					}
				}
			}
			context.NestedLevel--;
			context.TraceFunction<bool>("AttachmentRestriction evaluates to {0}", flag);
			return flag;
		}

		private static bool EvaluateBitMaskRestriction(Restriction.BitMaskRestriction restriction, IRuleEvaluationContext context)
		{
			RestrictionEvaluator.ValidateBitMaskRestriction(restriction);
			object obj = context[restriction.Tag];
			if (obj is int)
			{
				int num = (int)obj;
				bool flag;
				if ((num & restriction.Mask) != 0)
				{
					flag = (restriction.Bmr == Restriction.RelBmr.NotEqualToZero);
				}
				else
				{
					flag = (restriction.Bmr == Restriction.RelBmr.EqualToZero);
				}
				context.TraceFunction<object, Restriction.RelBmr, int, bool>("BitMaskRestriction [{0}] {1} [{2}] evaluates to {3}", obj, restriction.Bmr, restriction.Mask, flag);
				return flag;
			}
			context.TraceError<object>("Invalid BitMaskRestriction value: {0}", obj);
			return false;
		}

		private static bool EvaluateCommentRestriction(Restriction.CommentRestriction restriction, IRuleEvaluationContext context)
		{
			context.NestedLevel++;
			bool flag = RestrictionEvaluator.Evaluate(restriction.Restriction, context);
			context.NestedLevel--;
			context.TraceFunction<bool>("CommentRestriction evaluated to {0}", flag);
			return flag;
		}

		private static bool EvaluateComparePropertyRestriction(Restriction.ComparePropertyRestriction restriction, IRuleEvaluationContext context)
		{
			RestrictionEvaluator.ValidateComparePropertyRestriction(restriction);
			object obj = context[restriction.TagLeft];
			object obj2 = context[restriction.TagRight];
			context.TraceFunction<PropTag, Restriction.RelOp, PropTag>("ComparePropertyRestriction [{0}] {1} [{2}]", restriction.TagLeft, restriction.Op, restriction.TagRight);
			bool flag = false;
			if (obj != null && obj2 != null)
			{
				flag = context.CompareSingleValue(restriction.TagLeft, restriction.Op, obj, obj2);
			}
			context.TraceFunction<object, Restriction.RelOp, object, bool>("[{0}] {1} [{2}] evaluated to {3}", obj, restriction.Op, obj2, flag);
			return flag;
		}

		private static bool EvaluateContentRestriction(Restriction.ContentRestriction restriction, IRuleEvaluationContext context)
		{
			RestrictionEvaluator.ValidateContentRestriction(restriction);
			PropTag propTag = restriction.PropTag;
			if (restriction.MultiValued)
			{
				propTag = RuleUtil.GetMultiValuePropTag(propTag);
			}
			context.TraceFunction<PropTag, ContentFlags, object>("ContentRestriction tag [{0}] flags [{1}] value [{2}]", propTag, restriction.Flags, restriction.PropValue.Value);
			object obj = context[propTag];
			bool flag;
			if (obj == null)
			{
				flag = false;
			}
			else if (restriction.MultiValued)
			{
				flag = RestrictionEvaluator.MatchMultiValueWithPattern(context, restriction.PropTag, obj, restriction.PropValue.Value, restriction.Flags);
			}
			else if (RuleUtil.IsTextProp(restriction.PropTag))
			{
				flag = RestrictionEvaluator.MatchString(context.LimitChecker, context.CultureInfo, (string)obj, (string)restriction.PropValue.Value, restriction.Flags);
			}
			else
			{
				if (!RuleUtil.IsBinaryProp(restriction.PropTag))
				{
					throw new InvalidRuleException(string.Format("Content restriction can't be used on tag {0}", restriction.PropTag));
				}
				flag = RestrictionEvaluator.MatchByteArray(context.LimitChecker, (byte[])obj, (byte[])restriction.PropValue.Value, restriction.Flags);
			}
			context.TraceFunction<bool, object>("ContentRestriction Evaluated to {0} with property value [{1}]", flag, obj);
			if (!flag && propTag == PropTag.SenderSearchKey)
			{
				object obj2 = context[PropTag.SenderSmtpAddress];
				if (obj2 != null)
				{
					string @string = CTSGlobals.AsciiEncoding.GetString((byte[])restriction.PropValue.Value);
					ContentFlags contentFlags = ContentFlags.SubString | ContentFlags.IgnoreCase;
					context.TraceFunction("No match found in SenderSearchKey, searching for string in SenderSmtpAddress...");
					context.TraceFunction<PropTag, ContentFlags, string>("ContentRestriction tag [{0}] flags [{1}] value [{2}]", PropTag.SenderSmtpAddress, contentFlags, @string);
					flag = RestrictionEvaluator.MatchString(context.LimitChecker, context.CultureInfo, (string)obj2, @string, contentFlags);
				}
			}
			return flag;
		}

		private static bool EvaluateCountRestriction(Restriction.CountRestriction restriction, IRuleEvaluationContext context)
		{
			context.NestedLevel++;
			bool flag = RestrictionEvaluator.Evaluate(restriction.Restriction, context);
			context.NestedLevel--;
			context.TraceFunction<bool>("Count Restriction evaluated to {0}", flag);
			return flag;
		}

		private static bool EvaluateExistRestriction(Restriction.ExistRestriction restriction, IRuleEvaluationContext context)
		{
			object obj = context[restriction.Tag];
			bool flag = obj != null;
			context.TraceFunction<PropTag, bool>("ExistRestriction on tag [{0}] evaluted to {1}", restriction.Tag, flag);
			return flag;
		}

		private static bool EvaluateNotRestriction(Restriction.NotRestriction restriction, IRuleEvaluationContext context)
		{
			context.NestedLevel++;
			bool flag = !RestrictionEvaluator.Evaluate(restriction.Restriction, context);
			context.NestedLevel--;
			context.TraceFunction<bool>("NotRestriction evaluated to {0}", flag);
			return flag;
		}

		private static bool EvaluateOrRestriction(Restriction.OrRestriction restriction, IRuleEvaluationContext context)
		{
			context.NestedLevel++;
			bool flag = false;
			foreach (Restriction restriction2 in restriction.Restrictions)
			{
				if (RestrictionEvaluator.Evaluate(restriction2, context))
				{
					flag = true;
					break;
				}
			}
			context.NestedLevel--;
			context.TraceFunction<bool>("OrRestriction evaluated to {0}", flag);
			return flag;
		}

		private static bool EvaluatePropertyRestriction(Restriction.PropertyRestriction restriction, IRuleEvaluationContext context)
		{
			RestrictionEvaluator.ValidatePropertyRestriction(restriction);
			PropTag propTag = restriction.PropTag;
			if (restriction.MultiValued)
			{
				propTag = RuleUtil.GetMultiValuePropTag(propTag);
			}
			context.TraceFunction<PropTag, Restriction.RelOp, object>("PropertyRestriction [{0}] {1} [{2}]", propTag, restriction.Op, restriction.PropValue.Value);
			object obj = context[propTag];
			bool flag;
			if (obj == null)
			{
				flag = false;
			}
			else if (restriction.MultiValued)
			{
				flag = RestrictionEvaluator.CompareMultiValueWithValue(context, RuleUtil.GetSingleValuePropTag(restriction.PropTag), restriction.Op, obj, restriction.PropValue.Value);
			}
			else if (RuleUtil.IsMultiValueTag(restriction.PropValue.PropTag))
			{
				flag = RestrictionEvaluator.CompareValueWithMultiValue(context, restriction.PropTag, restriction.Op, obj, restriction.PropValue.Value);
			}
			else
			{
				flag = context.CompareSingleValue(restriction.PropTag, restriction.Op, obj, restriction.PropValue.Value);
			}
			context.TraceFunction<bool, object>("PropertyRestriction evaluated to {0} with property value [{1}]", flag, obj);
			return flag;
		}

		private static bool EvaluateRecipientRestriction(Restriction.RecipientRestriction restriction, IRuleEvaluationContext context)
		{
			context.TraceFunction("RecipientRestriction");
			context.NestedLevel++;
			bool flag = false;
			foreach (Recipient recipient in context.Message.Recipients)
			{
				using (IRuleEvaluationContext recipientContext = context.GetRecipientContext(recipient))
				{
					context.TraceFunction<string>("Recipient {0}", recipient.Participant.DisplayName);
					if (RestrictionEvaluator.Evaluate(restriction.Restriction, recipientContext))
					{
						flag = true;
						break;
					}
				}
			}
			context.NestedLevel--;
			context.TraceFunction<bool>("RecipientRestriction evaluates to {0}", flag);
			return flag;
		}

		private static bool EvaluateSizeRestriction(Restriction.SizeRestriction restriction, IRuleEvaluationContext context)
		{
			RestrictionEvaluator.ValidateSizeRestriction(restriction);
			context.TraceFunction<PropTag, Restriction.RelOp, int>("SizeRestriction [{0}] {1} [{2}]", restriction.Tag, restriction.Op, restriction.Size);
			PropType type = restriction.Tag.ValueType();
			int num = 0;
			bool flag = false;
			object obj = null;
			if (RuleUtil.GetPropTypeSize(type, out num))
			{
				flag = RestrictionEvaluator.GetOperationResultFromOrder(restriction.Op, num.CompareTo(restriction.Size));
			}
			else if (RuleUtil.IsMultiValueTag(restriction.Tag))
			{
				obj = context[restriction.Tag];
				object[] array = obj as object[];
				if (array != null)
				{
					foreach (object value in array)
					{
						int size = RuleUtil.GetSize(value);
						if (RestrictionEvaluator.GetOperationResultFromOrder(restriction.Op, size.CompareTo(restriction.Size)))
						{
							flag = true;
							break;
						}
					}
				}
			}
			else
			{
				obj = context[restriction.Tag];
				int size2 = RuleUtil.GetSize(obj);
				flag = RestrictionEvaluator.GetOperationResultFromOrder(restriction.Op, size2.CompareTo(restriction.Size));
			}
			context.TraceFunction<bool, object>("SizeRestriction evaluated to {0} with property value [{1}]", flag, obj);
			return flag;
		}

		private static CompareOptions GetCompareOptions(ContentFlags flags)
		{
			if ((flags & (ContentFlags.IgnoreNonSpace | ContentFlags.Loose)) != ContentFlags.FullString)
			{
				return CompareOptions.IgnoreCase | CompareOptions.IgnoreNonSpace | CompareOptions.IgnoreKanaType | CompareOptions.IgnoreWidth;
			}
			if ((flags & (ContentFlags.IgnoreCase | ContentFlags.Loose)) != ContentFlags.FullString)
			{
				return CompareOptions.IgnoreCase | CompareOptions.IgnoreWidth;
			}
			return CompareOptions.IgnoreWidth;
		}

		private static bool MatchByteArray(LimitChecker limitChecker, byte[] content, byte[] pattern, ContentFlags flags)
		{
			if (!limitChecker.CheckAndIncrementContentRestrictionCount(1, content))
			{
				return false;
			}
			switch (flags & (ContentFlags.SubString | ContentFlags.Prefix))
			{
			case ContentFlags.FullString:
				return RuleUtil.EqualsByteArray(content, pattern);
			case ContentFlags.SubString:
				return RuleUtil.Contains(content, pattern);
			case ContentFlags.Prefix:
				return RuleUtil.IsPrefix(content, pattern);
			default:
				throw new InvalidRuleException(string.Format("Not supported content flags {0}", flags));
			}
		}

		private static bool MatchMultiValueWithPattern(IRuleEvaluationContext context, PropTag tag, object content, object pattern, ContentFlags flags)
		{
			if (RuleUtil.IsTextProp(tag))
			{
				string[] array = (string[])content;
				string pattern2 = (string)pattern;
				foreach (string content2 in array)
				{
					if (RestrictionEvaluator.MatchString(context.LimitChecker, context.CultureInfo, content2, pattern2, flags))
					{
						return true;
					}
				}
			}
			else if (RuleUtil.IsBinaryProp(tag))
			{
				byte[][] array3 = (byte[][])content;
				byte[] pattern3 = pattern as byte[];
				foreach (byte[] content3 in array3)
				{
					if (RestrictionEvaluator.MatchByteArray(context.LimitChecker, content3, pattern3, flags))
					{
						return true;
					}
				}
			}
			return false;
		}

		private static void ValidateBitMaskRestriction(Restriction.BitMaskRestriction restriction)
		{
			if (restriction.Tag.ValueType() != PropType.Int)
			{
				throw new InvalidRuleException("BitMaskRestriction can only be used on int property");
			}
			if (Restriction.RelBmr.EqualToZero > restriction.Bmr || restriction.Bmr > Restriction.RelBmr.NotEqualToZero)
			{
				throw new InvalidRuleException("Not supported bitmask operation");
			}
		}

		private static void ValidateComparePropertyRestriction(Restriction.ComparePropertyRestriction restriction)
		{
			if (!RestrictionEvaluator.IsSupportedOperation(restriction.Op))
			{
				throw new InvalidRuleException(string.Format("Operation {0} is not supported", restriction.Op));
			}
			if (!RuleUtil.IsSameType(restriction.TagLeft, restriction.TagRight))
			{
				throw new InvalidRuleException("Tag and value are of different type");
			}
			if (RuleUtil.IsBooleanTag(restriction.TagLeft) && restriction.Op != Restriction.RelOp.Equal && restriction.Op != Restriction.RelOp.NotEqual)
			{
				throw new InvalidRuleException("Boolean tag only support Equal or NotEqual operation");
			}
			if (RuleUtil.IsMultiValueTag(restriction.TagLeft) || RuleUtil.IsMultiValueTag(restriction.TagRight))
			{
				throw new InvalidRuleException("Multi-valued prop or value is not supported in CompareProperty Restriction");
			}
		}

		private static void ValidateContentRestriction(Restriction.ContentRestriction restriction)
		{
			if (!RuleUtil.IsTextProp(restriction.PropTag) && !RuleUtil.IsBinaryProp(restriction.PropTag))
			{
				throw new InvalidRuleException(string.Format("Content Restriction is not supported on tag {0}", restriction.PropTag));
			}
			if (!RuleUtil.IsSameType(restriction.PropTag, restriction.PropValue.PropTag))
			{
				throw new InvalidRuleException("Tag and value are of different type");
			}
			if (RuleUtil.IsMultiValueTag(restriction.PropValue.PropTag))
			{
				throw new InvalidRuleException("Content Restriction does not support multi-valued value");
			}
		}

		private static void ValidatePropertyRestriction(Restriction.PropertyRestriction restriction)
		{
			if (!RestrictionEvaluator.IsSupportedOperation(restriction.Op))
			{
				throw new InvalidRuleException(string.Format("Operation {0} is not supported", restriction.Op));
			}
			if (RuleUtil.IsMultiValueTag(restriction.PropValue.PropTag))
			{
				if (restriction.MultiValued)
				{
					throw new InvalidRuleException("At most one tag can be multi-valued");
				}
				if (restriction.Op != Restriction.RelOp.Equal && restriction.Op != Restriction.RelOp.NotEqual)
				{
					throw new InvalidRuleException("Restriction has multi-valued value only support Equal or NotEqual operation");
				}
			}
			if (!RuleUtil.IsSameType(restriction.PropTag, restriction.PropValue.PropTag))
			{
				throw new InvalidRuleException("Tag and value are of different type");
			}
			if (RuleUtil.IsBooleanTag(restriction.PropTag) && restriction.Op != Restriction.RelOp.Equal && restriction.Op != Restriction.RelOp.NotEqual)
			{
				throw new InvalidRuleException("Boolean tag only support Equal or NotEqual operation");
			}
			RestrictionEvaluator.ValidateRestrictionValue(restriction.PropValue);
		}

		private static void ValidateRestrictionValue(PropValue value)
		{
			if (value.Value == null)
			{
				throw new InvalidRuleException("Value stored in restriction is null");
			}
			RuleUtil.CheckValueType(value.Value, value.PropTag);
		}

		private static void ValidateSizeRestriction(Restriction.SizeRestriction restriction)
		{
			if (!RestrictionEvaluator.IsOrderOperation(restriction.Op))
			{
				throw new InvalidRuleException(string.Format("Operation {0} is not supported in SizeRestriction", restriction.Op));
			}
		}

		private const CompareOptions StringComparisonOptions = CompareOptions.IgnoreCase | CompareOptions.IgnoreNonSpace | CompareOptions.IgnoreKanaType | CompareOptions.IgnoreWidth;
	}
}
