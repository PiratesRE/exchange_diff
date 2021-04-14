using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Mapi;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal abstract class Condition
	{
		private static Dictionary<ConditionType, ConditionOrder> BuildOrder(Dictionary<ConditionType, ConditionOrder> order)
		{
			return new Dictionary<ConditionType, ConditionOrder>
			{
				{
					ConditionType.FromRecipientsCondition,
					ConditionOrder.FromRecipientsCondition
				},
				{
					ConditionType.ContainsSubjectStringCondition,
					ConditionOrder.ContainsSubjectStringCondition
				},
				{
					ConditionType.SentOnlyToMeCondition,
					ConditionOrder.SentOnlyToMeCondition
				},
				{
					ConditionType.SentToMeCondition,
					ConditionOrder.SentToMeCondition
				},
				{
					ConditionType.MarkedAsImportanceCondition,
					ConditionOrder.MarkedAsImportanceCondition
				},
				{
					ConditionType.MarkedAsSensitivityCondition,
					ConditionOrder.MarkedAsSensitivityCondition
				},
				{
					ConditionType.SentCcMeCondition,
					ConditionOrder.SentCcMeCondition
				},
				{
					ConditionType.SentToOrCcMeCondition,
					ConditionOrder.SentToOrCcMeCondition
				},
				{
					ConditionType.NotSentToMeCondition,
					ConditionOrder.NotSentToMeCondition
				},
				{
					ConditionType.SentToRecipientsCondition,
					ConditionOrder.SentToRecipientsCondition
				},
				{
					ConditionType.ContainsBodyStringCondition,
					ConditionOrder.ContainsBodyStringCondition
				},
				{
					ConditionType.ContainsSubjectOrBodyStringCondition,
					ConditionOrder.ContainsSubjectOrBodyStringCondition
				},
				{
					ConditionType.ContainsHeaderStringCondition,
					ConditionOrder.ContainsHeaderStringCondition
				},
				{
					ConditionType.ContainsSenderStringCondition,
					ConditionOrder.ContainsSenderStringCondition
				},
				{
					ConditionType.MarkedAsOofCondition,
					ConditionOrder.MarkedAsOofCondition
				},
				{
					ConditionType.HasAttachmentCondition,
					ConditionOrder.HasAttachmentCondition
				},
				{
					ConditionType.WithinSizeRangeCondition,
					ConditionOrder.WithinSizeRangeCondition
				},
				{
					ConditionType.WithinDateRangeCondition,
					ConditionOrder.WithinDateRangeCondition
				},
				{
					ConditionType.MeetingMessageCondition,
					ConditionOrder.MeetingMessageCondition
				},
				{
					ConditionType.MeetingResponseCondition,
					ConditionOrder.MeetingResponseCondition
				},
				{
					ConditionType.ContainsRecipientStringCondition,
					ConditionOrder.ContainsRecipientStringCondition
				},
				{
					ConditionType.AssignedCategoriesCondition,
					ConditionOrder.AssignedCategoriesCondition
				},
				{
					ConditionType.FormsCondition,
					ConditionOrder.FormsCondition
				},
				{
					ConditionType.MessageClassificationCondition,
					ConditionOrder.MessageClassificationCondition
				},
				{
					ConditionType.NdrCondition,
					ConditionOrder.NdrCondition
				},
				{
					ConditionType.AutomaticForwardCondition,
					ConditionOrder.AutomaticForwardCondition
				},
				{
					ConditionType.EncryptedCondition,
					ConditionOrder.EncryptedCondition
				},
				{
					ConditionType.SignedCondition,
					ConditionOrder.SignedCondition
				},
				{
					ConditionType.ReadReceiptCondition,
					ConditionOrder.ReadReceiptCondition
				},
				{
					ConditionType.PermissionControlledCondition,
					ConditionOrder.PermissionControlledCondition
				},
				{
					ConditionType.ApprovalRequestCondition,
					ConditionOrder.ApprovalRequestCondition
				},
				{
					ConditionType.VoicemailCondition,
					ConditionOrder.VoicemailCondition
				},
				{
					ConditionType.FlaggedForActionCondition,
					ConditionOrder.FlaggedForActionCondition
				},
				{
					ConditionType.FromSubscriptionCondition,
					ConditionOrder.FromSubscriptionCondition
				}
			};
		}

		private static Dictionary<ConditionType, ExceptionOrder> BuildExceptionOrder(Dictionary<ConditionType, ExceptionOrder> exceptionOrder)
		{
			return new Dictionary<ConditionType, ExceptionOrder>
			{
				{
					ConditionType.FromRecipientsCondition,
					ExceptionOrder.FromRecipientsCondition
				},
				{
					ConditionType.ContainsSubjectStringCondition,
					ExceptionOrder.ContainsSubjectStringCondition
				},
				{
					ConditionType.SentOnlyToMeCondition,
					ExceptionOrder.SentOnlyToMeCondition
				},
				{
					ConditionType.SentToMeCondition,
					ExceptionOrder.SentToMeCondition
				},
				{
					ConditionType.MarkedAsImportanceCondition,
					ExceptionOrder.MarkedAsImportanceCondition
				},
				{
					ConditionType.MarkedAsSensitivityCondition,
					ExceptionOrder.MarkedAsSensitivityCondition
				},
				{
					ConditionType.SentCcMeCondition,
					ExceptionOrder.SentCcMeCondition
				},
				{
					ConditionType.SentToOrCcMeCondition,
					ExceptionOrder.SentToOrCcMeCondition
				},
				{
					ConditionType.NotSentToMeCondition,
					ExceptionOrder.NotSentToMeCondition
				},
				{
					ConditionType.SentToRecipientsCondition,
					ExceptionOrder.SentToRecipientsCondition
				},
				{
					ConditionType.ContainsBodyStringCondition,
					ExceptionOrder.ContainsBodyStringCondition
				},
				{
					ConditionType.ContainsSubjectOrBodyStringCondition,
					ExceptionOrder.ContainsSubjectOrBodyStringCondition
				},
				{
					ConditionType.ContainsHeaderStringCondition,
					ExceptionOrder.ContainsHeaderStringCondition
				},
				{
					ConditionType.ContainsSenderStringCondition,
					ExceptionOrder.ContainsSenderStringCondition
				},
				{
					ConditionType.MarkedAsOofCondition,
					ExceptionOrder.MarkedAsOofCondition
				},
				{
					ConditionType.HasAttachmentCondition,
					ExceptionOrder.HasAttachmentCondition
				},
				{
					ConditionType.WithinSizeRangeCondition,
					ExceptionOrder.WithinSizeRangeCondition
				},
				{
					ConditionType.WithinDateRangeCondition,
					ExceptionOrder.WithinDateRangeCondition
				},
				{
					ConditionType.MeetingMessageCondition,
					ExceptionOrder.MeetingMessageCondition
				},
				{
					ConditionType.MeetingResponseCondition,
					ExceptionOrder.MeetingResponseCondition
				},
				{
					ConditionType.ContainsRecipientStringCondition,
					ExceptionOrder.ContainsRecipientStringCondition
				},
				{
					ConditionType.AssignedCategoriesCondition,
					ExceptionOrder.AssignedCategoriesCondition
				},
				{
					ConditionType.FormsCondition,
					ExceptionOrder.FormsCondition
				},
				{
					ConditionType.MessageClassificationCondition,
					ExceptionOrder.MessageClassificationCondition
				},
				{
					ConditionType.NdrCondition,
					ExceptionOrder.NdrCondition
				},
				{
					ConditionType.AutomaticForwardCondition,
					ExceptionOrder.AutomaticForwardCondition
				},
				{
					ConditionType.EncryptedCondition,
					ExceptionOrder.EncryptedCondition
				},
				{
					ConditionType.SignedCondition,
					ExceptionOrder.SignedCondition
				},
				{
					ConditionType.ReadReceiptCondition,
					ExceptionOrder.ReadReceiptCondition
				},
				{
					ConditionType.PermissionControlledCondition,
					ExceptionOrder.PermissionControlledCondition
				},
				{
					ConditionType.ApprovalRequestCondition,
					ExceptionOrder.ApprovalRequestCondition
				},
				{
					ConditionType.VoicemailCondition,
					ExceptionOrder.VoicemailCondition
				},
				{
					ConditionType.FlaggedForActionCondition,
					ExceptionOrder.FlaggedForActionCondition
				},
				{
					ConditionType.FromSubscriptionCondition,
					ExceptionOrder.FromSubscriptionCondition
				}
			};
		}

		protected Condition(ConditionType conditionType, Rule rule)
		{
			EnumValidator.ThrowIfInvalid<ConditionType>(conditionType, "conditionType");
			this.conditionType = conditionType;
			this.rule = rule;
		}

		public ConditionType ConditionType
		{
			get
			{
				return this.conditionType;
			}
		}

		public Rule Rule
		{
			get
			{
				return this.rule;
			}
		}

		public virtual Rule.ProviderIdEnum ProviderId
		{
			get
			{
				return Rule.ProviderIdEnum.OL98Plus;
			}
		}

		internal ConditionOrder ConditionOrder
		{
			get
			{
				return Condition.ConditionsOrder[this.ConditionType];
			}
		}

		internal ExceptionOrder ExceptionOrder
		{
			get
			{
				return Condition.ExceptionsOrder[this.ConditionType];
			}
		}

		internal abstract Restriction BuildRestriction();

		protected static Restriction CreatePropertyRestriction<T>(PropTag propertyTag, T value)
		{
			Restriction result;
			if ((PropTag)4096U == ((PropTag)4096U & propertyTag))
			{
				PropTag tag = propertyTag & (PropTag)4294963199U;
				result = new Restriction.PropertyRestriction(Restriction.RelOp.Equal, tag, true, value);
			}
			else
			{
				result = Restriction.EQ(propertyTag, value);
			}
			return result;
		}

		protected static Restriction CreateAndStringPropertyRestriction(PropTag propertyTag, string[] values)
		{
			Restriction[] array = new Restriction.PropertyRestriction[values.Length];
			for (int i = 0; i < values.Length; i++)
			{
				array[i] = Condition.CreatePropertyRestriction<string>(propertyTag, values[i]);
			}
			return Condition.CreateAndRestriction(array);
		}

		protected static Restriction CreateAndRestriction(Restriction[] subRestrictions)
		{
			return Restriction.And(subRestrictions);
		}

		protected static Restriction CreateSearchKeyContentRestriction(PropTag propertyTag, byte[] value, ContentFlags flags)
		{
			return Restriction.Content(propertyTag, value, flags);
		}

		protected static Restriction CreateORSearchKeyContentRestriction(byte[][] values, PropTag propertyTag, ContentFlags flags)
		{
			if (values.Length > 1)
			{
				Restriction[] array = new Restriction.ContentRestriction[values.Length];
				for (int i = 0; i < values.Length; i++)
				{
					array[i] = Restriction.Content(propertyTag, values[i], flags);
				}
				return Restriction.Or(array);
			}
			if (values.Length == 1)
			{
				return Condition.CreateSearchKeyContentRestriction(propertyTag, values[0], flags);
			}
			return null;
		}

		protected static Restriction CreateORStringContentRestriction(string[] values, PropTag propertyTag, ContentFlags flags)
		{
			if (values.Length > 1)
			{
				Restriction[] array = new Restriction.ContentRestriction[values.Length];
				for (int i = 0; i < values.Length; i++)
				{
					array[i] = Restriction.Content(propertyTag, values[i], flags);
				}
				return Restriction.Or(array);
			}
			if (values.Length == 1)
			{
				return Condition.CreateStringContentRestriction(propertyTag, values[0], flags);
			}
			return null;
		}

		protected static Restriction CreateORGuidContentRestriction(Guid[] values, PropTag propertyTag)
		{
			if (values.Length > 1)
			{
				Restriction[] array = new Restriction.PropertyRestriction[values.Length];
				for (int i = 0; i < values.Length; i++)
				{
					array[i] = Condition.CreatePropertyRestriction<Guid>(propertyTag, values[i]);
				}
				return Restriction.Or(array);
			}
			if (values.Length == 1)
			{
				return Condition.CreatePropertyRestriction<Guid>(propertyTag, values[0]);
			}
			return null;
		}

		protected static Restriction CreateStringContentRestriction(PropTag propertyTag, string value, ContentFlags flags)
		{
			return Restriction.Content(propertyTag, value, flags);
		}

		protected static Restriction CreateIntPropertyRestriction(PropTag propertyTag, int value, Restriction.RelOp relop)
		{
			return new Restriction.PropertyRestriction(relop, propertyTag, value);
		}

		protected static Restriction CreateBooleanPropertyRestriction(PropTag propertyTag, bool value, Restriction.RelOp relop)
		{
			return new Restriction.PropertyRestriction(relop, propertyTag, value);
		}

		protected static Restriction CreateDateTimePropertyRestriction(PropTag propertyTag, ExDateTime dateTime, Restriction.RelOp relop)
		{
			return new Restriction.PropertyRestriction(relop, propertyTag, (DateTime)dateTime.ToUtc());
		}

		protected static Restriction CreateSubjectOrBodyRestriction(string[] values)
		{
			Restriction[] array = new Restriction.ContentRestriction[values.Length * 2];
			if (values.Length > 0)
			{
				int i = 0;
				int num = 0;
				while (i < values.Length)
				{
					array[num] = Condition.CreateStringContentRestriction(PropTag.Subject, values[i], ContentFlags.SubString | ContentFlags.IgnoreCase);
					array[num + 1] = Condition.CreateStringContentRestriction(PropTag.Body, values[i], ContentFlags.SubString | ContentFlags.IgnoreCase);
					i++;
					num += 2;
				}
				return Restriction.Or(array);
			}
			return null;
		}

		protected static Restriction CreateFromRestriction(IList<Participant> participants)
		{
			if (participants.Count > 0)
			{
				return Rule.OrAddressList(participants, PropTag.SenderName);
			}
			return null;
		}

		protected static Restriction CreateRecipientRestriction(IList<Participant> participants)
		{
			if (participants.Count > 0)
			{
				Restriction restriction = Rule.OrAddressList(participants, PropTag.DisplayName);
				return new Restriction.RecipientRestriction(restriction);
			}
			return null;
		}

		protected static Restriction CreateOnlyToMeRestriction()
		{
			Restriction[] array = new Restriction[3];
			array[0] = Condition.CreateBooleanPropertyRestriction(PropTag.MessageToMe, true, Restriction.RelOp.Equal);
			Restriction restriction = Condition.CreateStringContentRestriction(PropTag.DisplayTo, ";", ContentFlags.SubString);
			array[1] = Restriction.Not(restriction);
			array[2] = Condition.CreatePropertyRestriction<string>(PropTag.DisplayCc, string.Empty);
			return Restriction.And(array);
		}

		protected static Restriction CreateCcToMeRestriction()
		{
			return Restriction.And(new Restriction[]
			{
				Condition.CreateBooleanPropertyRestriction(PropTag.MessageCcMe, true, Restriction.RelOp.Equal),
				Condition.CreateBooleanPropertyRestriction(PropTag.MessageRecipMe, true, Restriction.RelOp.Equal),
				Condition.CreateBooleanPropertyRestriction(PropTag.MessageToMe, false, Restriction.RelOp.Equal)
			});
		}

		protected static Restriction CreateHasAttachmentRestriction()
		{
			return Restriction.BitMaskNonZero(PropTag.MessageFlags, 16);
		}

		protected static Restriction CreateSizeRestriction(int? atLeast, int? atMost)
		{
			Restriction[] array = null;
			if (atLeast != null && atMost != null)
			{
				array = new Restriction[2];
			}
			if (atLeast != null)
			{
				Restriction restriction = Condition.CreateIntPropertyRestriction(PropTag.MessageSize, (atLeast * 1024).Value, Restriction.RelOp.GreaterThan);
				if (atMost == null)
				{
					return restriction;
				}
				array[0] = restriction;
			}
			if (atMost != null)
			{
				Restriction restriction2 = Condition.CreateIntPropertyRestriction(PropTag.MessageSize, (atMost * 1024).Value, Restriction.RelOp.LessThan);
				if (atLeast == null)
				{
					return restriction2;
				}
				array[1] = restriction2;
			}
			return Restriction.And(array);
		}

		protected static Restriction CreateOneOrTwoTimesRestrictions(ExDateTime? before, ExDateTime? after)
		{
			Restriction[] array = null;
			if (before != null && after != null)
			{
				array = new Restriction[2];
			}
			if (before != null)
			{
				Restriction restriction = Condition.CreateDateTimePropertyRestriction(PropTag.MessageDeliveryTime, before.Value, Restriction.RelOp.LessThan);
				if (after == null)
				{
					return restriction;
				}
				array[0] = restriction;
			}
			if (after != null)
			{
				Restriction restriction2 = Condition.CreateDateTimePropertyRestriction(PropTag.MessageDeliveryTime, after.Value, Restriction.RelOp.GreaterThan);
				if (before == null)
				{
					return restriction2;
				}
				array[1] = restriction2;
			}
			return Restriction.And(array);
		}

		protected static Restriction CreateIsNdrRestrictions()
		{
			return Condition.CreateAndRestriction(new Restriction[]
			{
				Condition.CreateStringContentRestriction(PropTag.MessageClass, "REPORT", ContentFlags.Prefix | ContentFlags.IgnoreCase),
				Condition.CreateStringContentRestriction(PropTag.MessageClass, ".NDR", ContentFlags.SubString | ContentFlags.IgnoreCase)
			});
		}

		protected static void CheckParams(params object[] parameters)
		{
			Rule rule = Rule.CheckRuleParameter(parameters);
			int i;
			for (i = 1; i < parameters.Length; i++)
			{
				if (parameters[i] == null)
				{
					rule.ThrowValidateException(delegate
					{
						throw new ArgumentNullException("parameter " + i);
					}, "parameter " + i);
				}
				string[] array = parameters[i] as string[];
				if (array != null)
				{
					if (array.Length == 0)
					{
						rule.ThrowValidateException(delegate
						{
							throw new ArgumentException("parameter " + i);
						}, "parameter " + i);
					}
				}
				else
				{
					string text = parameters[i] as string;
					if (text != null && text.Length == 0)
					{
						rule.ThrowValidateException(delegate
						{
							throw new ArgumentException("parameter " + i);
						}, "parameter " + i);
					}
				}
			}
		}

		internal const PropTag MultiValueTag = (PropTag)4096U;

		internal static Dictionary<ConditionType, ConditionOrder> ConditionsOrder = Condition.BuildOrder(Condition.ConditionsOrder);

		internal static Dictionary<ConditionType, ExceptionOrder> ExceptionsOrder = Condition.BuildExceptionOrder(Condition.ExceptionsOrder);

		internal static ConditionOrderComparer ConditionOrderComparer = new ConditionOrderComparer();

		internal static ExceptionOrderComparer ExceptionOrderComparer = new ExceptionOrderComparer();

		private readonly ConditionType conditionType;

		private readonly Rule rule;
	}
}
