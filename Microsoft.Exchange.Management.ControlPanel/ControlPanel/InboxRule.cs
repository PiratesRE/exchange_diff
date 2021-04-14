using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Management.Common;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[KnownType(typeof(InboxRule))]
	[DataContract]
	public class InboxRule : RuleRow
	{
		public InboxRule(InboxRule rule) : base(rule)
		{
			this.Rule = rule;
			base.DescriptionObject = rule.Description;
			base.ConditionDescriptions = base.DescriptionObject.ConditionDescriptions.ToArray();
			base.ActionDescriptions = base.DescriptionObject.ActionDescriptions.ToArray();
			base.ExceptionDescriptions = base.DescriptionObject.ExceptionDescriptions.ToArray();
		}

		public InboxRule Rule { get; private set; }

		[DataMember(EmitDefaultValue = false)]
		public PeopleIdentity[] From
		{
			get
			{
				return this.Rule.From.ToPeopleIdentityArray();
			}
			private set
			{
				throw new NotSupportedException();
			}
		}

		[DataMember(EmitDefaultValue = false)]
		public PeopleIdentity[] SentTo
		{
			get
			{
				return this.Rule.SentTo.ToPeopleIdentityArray();
			}
			private set
			{
				throw new NotSupportedException();
			}
		}

		[DataMember(EmitDefaultValue = false)]
		public Identity FromSubscription
		{
			get
			{
				return this.Rule.FromSubscription.ToIdentity(this.Rule);
			}
			private set
			{
				throw new NotSupportedException();
			}
		}

		[DataMember(EmitDefaultValue = false)]
		public string[] SubjectContainsWords
		{
			get
			{
				return this.Rule.SubjectContainsWords.ToStringArray();
			}
			private set
			{
				throw new NotImplementedException();
			}
		}

		[DataMember(EmitDefaultValue = false)]
		public string[] SubjectOrBodyContainsWords
		{
			get
			{
				return this.Rule.SubjectOrBodyContainsWords.ToStringArray();
			}
			private set
			{
				throw new NotImplementedException();
			}
		}

		[DataMember(EmitDefaultValue = false)]
		public string[] FromAddressContainsWords
		{
			get
			{
				return this.Rule.FromAddressContainsWords.ToStringArray();
			}
			private set
			{
				throw new NotImplementedException();
			}
		}

		[DataMember(EmitDefaultValue = false)]
		public bool? MyNameInToOrCcBox
		{
			get
			{
				if (!this.Rule.MyNameInToOrCcBox)
				{
					return null;
				}
				return new bool?(true);
			}
			private set
			{
				throw new NotImplementedException();
			}
		}

		[DataMember(EmitDefaultValue = false)]
		public bool? SentOnlyToMe
		{
			get
			{
				if (!this.Rule.SentOnlyToMe)
				{
					return null;
				}
				return new bool?(true);
			}
			private set
			{
				throw new NotImplementedException();
			}
		}

		[DataMember(EmitDefaultValue = false)]
		public bool? MyNameInToBox
		{
			get
			{
				if (!this.Rule.MyNameInToBox)
				{
					return null;
				}
				return new bool?(true);
			}
			private set
			{
				throw new NotImplementedException();
			}
		}

		[DataMember(EmitDefaultValue = false)]
		public bool? MyNameInCcBox
		{
			get
			{
				if (!this.Rule.MyNameInCcBox)
				{
					return null;
				}
				return new bool?(true);
			}
			private set
			{
				throw new NotImplementedException();
			}
		}

		[DataMember(EmitDefaultValue = false)]
		public bool? MyNameNotInToBox
		{
			get
			{
				if (!this.Rule.MyNameNotInToBox)
				{
					return null;
				}
				return new bool?(true);
			}
			private set
			{
				throw new NotImplementedException();
			}
		}

		[DataMember(EmitDefaultValue = false)]
		public string[] BodyContainsWords
		{
			get
			{
				return this.Rule.BodyContainsWords.ToStringArray();
			}
			private set
			{
				throw new NotImplementedException();
			}
		}

		[DataMember(EmitDefaultValue = false)]
		public string[] RecipientAddressContainsWords
		{
			get
			{
				return this.Rule.RecipientAddressContainsWords.ToStringArray();
			}
			private set
			{
				throw new NotImplementedException();
			}
		}

		[DataMember(EmitDefaultValue = false)]
		public string[] HeaderContainsWords
		{
			get
			{
				return this.Rule.HeaderContainsWords.ToStringArray();
			}
			private set
			{
				throw new NotImplementedException();
			}
		}

		[DataMember(EmitDefaultValue = false)]
		public string WithImportance
		{
			get
			{
				return this.Rule.WithImportance.ToStringWithNull();
			}
			private set
			{
				throw new NotImplementedException();
			}
		}

		[DataMember(EmitDefaultValue = false)]
		public string WithSensitivity
		{
			get
			{
				return this.Rule.WithSensitivity.ToStringWithNull();
			}
			private set
			{
				throw new NotImplementedException();
			}
		}

		[DataMember(EmitDefaultValue = false)]
		public bool? HasAttachment
		{
			get
			{
				if (!this.Rule.HasAttachment)
				{
					return null;
				}
				return new bool?(true);
			}
			private set
			{
				throw new NotImplementedException();
			}
		}

		[DataMember(EmitDefaultValue = false)]
		public string MessageTypeMatches
		{
			get
			{
				return this.Rule.MessageTypeMatches.ToStringWithNull();
			}
			private set
			{
				throw new NotImplementedException();
			}
		}

		[DataMember(EmitDefaultValue = false)]
		public Identity HasClassification
		{
			get
			{
				return this.Rule.HasClassification.ToIdentity();
			}
			private set
			{
				throw new NotImplementedException();
			}
		}

		[DataMember(EmitDefaultValue = false)]
		public string FlaggedForAction
		{
			get
			{
				return this.Rule.FlaggedForAction;
			}
			private set
			{
				throw new NotImplementedException();
			}
		}

		[DataMember(EmitDefaultValue = false)]
		public NumberRange WithinSizeRange
		{
			get
			{
				NumberRange numberRange = new NumberRange();
				numberRange.AtMost = this.Rule.WithinSizeRangeMaximum.ToKB();
				numberRange.AtLeast = this.Rule.WithinSizeRangeMinimum.ToKB();
				if (numberRange.AtMost == 0 && numberRange.AtLeast == 0)
				{
					return null;
				}
				return numberRange;
			}
			private set
			{
				throw new NotImplementedException();
			}
		}

		[DataMember(EmitDefaultValue = false)]
		public DateRange WithinDateRange
		{
			get
			{
				DateRange dateRange = new DateRange();
				dateRange.BeforeDate = this.Rule.ReceivedBeforeDate.ToIdentity();
				dateRange.AfterDate = this.Rule.ReceivedAfterDate.ToIdentity();
				if (dateRange.BeforeDate == null && dateRange.AfterDate == null)
				{
					return null;
				}
				return dateRange;
			}
			private set
			{
				throw new NotImplementedException();
			}
		}

		[DataMember(EmitDefaultValue = false)]
		public Identity MoveToFolder
		{
			get
			{
				if (this.Rule.MoveToFolder != null)
				{
					return this.Rule.MoveToFolder.ToIdentity();
				}
				return null;
			}
			private set
			{
				throw new NotImplementedException();
			}
		}

		[DataMember(EmitDefaultValue = false)]
		public Identity CopyToFolder
		{
			get
			{
				if (this.Rule.CopyToFolder != null)
				{
					return this.Rule.CopyToFolder.ToIdentity();
				}
				return null;
			}
			private set
			{
				throw new NotImplementedException();
			}
		}

		[DataMember(EmitDefaultValue = false)]
		public bool? DeleteMessage
		{
			get
			{
				if (!this.Rule.DeleteMessage)
				{
					return null;
				}
				return new bool?(true);
			}
			private set
			{
				throw new NotImplementedException();
			}
		}

		[DataMember(EmitDefaultValue = false)]
		public bool? StopProcessingRules
		{
			get
			{
				if (!this.Rule.StopProcessingRules)
				{
					return null;
				}
				return new bool?(true);
			}
			private set
			{
				throw new NotImplementedException();
			}
		}

		[DataMember(EmitDefaultValue = false)]
		public Identity ApplyCategory
		{
			get
			{
				if (MultiValuedPropertyBase.IsNullOrEmpty(this.Rule.ApplyCategory))
				{
					return null;
				}
				string text = this.Rule.ApplyCategory.ToStringArray().ToCommaSeperatedString();
				return new Identity(text, text);
			}
			private set
			{
				throw new NotImplementedException();
			}
		}

		[DataMember(EmitDefaultValue = false)]
		public bool? MarkAsRead
		{
			get
			{
				if (!this.Rule.MarkAsRead)
				{
					return null;
				}
				return new bool?(true);
			}
			private set
			{
				throw new NotImplementedException();
			}
		}

		[DataMember(EmitDefaultValue = false)]
		public PeopleIdentity[] ForwardTo
		{
			get
			{
				return this.Rule.ForwardTo.ToPeopleIdentityArray();
			}
			private set
			{
				throw new NotSupportedException();
			}
		}

		[DataMember(EmitDefaultValue = false)]
		public string SendTextMessageNotificationTo
		{
			get
			{
				if (this.Rule.SendTextMessageNotificationTo != null && this.Rule.SendTextMessageNotificationTo.Count > 0)
				{
					return this.Rule.SendTextMessageNotificationTo[0].ToString();
				}
				return null;
			}
			private set
			{
				throw new NotSupportedException();
			}
		}

		[DataMember(EmitDefaultValue = false)]
		public PeopleIdentity[] RedirectTo
		{
			get
			{
				return this.Rule.RedirectTo.ToPeopleIdentityArray();
			}
			private set
			{
				throw new NotSupportedException();
			}
		}

		[DataMember(EmitDefaultValue = false)]
		public PeopleIdentity[] ForwardAsAttachmentTo
		{
			get
			{
				return this.Rule.ForwardAsAttachmentTo.ToPeopleIdentityArray();
			}
			private set
			{
				throw new NotSupportedException();
			}
		}

		[DataMember(EmitDefaultValue = false)]
		public string MarkImportance
		{
			get
			{
				return this.Rule.MarkImportance.ToStringWithNull();
			}
			private set
			{
				throw new NotSupportedException();
			}
		}

		[DataMember(EmitDefaultValue = false)]
		public PeopleIdentity[] ExceptIfFrom
		{
			get
			{
				return this.Rule.ExceptIfFrom.ToPeopleIdentityArray();
			}
			private set
			{
				throw new NotSupportedException();
			}
		}

		[DataMember(EmitDefaultValue = false)]
		public PeopleIdentity[] ExceptIfSentTo
		{
			get
			{
				return this.Rule.ExceptIfSentTo.ToPeopleIdentityArray();
			}
			private set
			{
				throw new NotSupportedException();
			}
		}

		[DataMember(EmitDefaultValue = false)]
		public Identity ExceptIfFromSubscription
		{
			get
			{
				return this.Rule.ExceptIfFromSubscription.ToIdentity(this.Rule);
			}
			private set
			{
				throw new NotSupportedException();
			}
		}

		[DataMember(EmitDefaultValue = false)]
		public string[] ExceptIfSubjectContainsWords
		{
			get
			{
				return this.Rule.ExceptIfSubjectContainsWords.ToStringArray();
			}
			private set
			{
				throw new NotImplementedException();
			}
		}

		[DataMember(EmitDefaultValue = false)]
		public string[] ExceptIfSubjectOrBodyContainsWords
		{
			get
			{
				return this.Rule.ExceptIfSubjectOrBodyContainsWords.ToStringArray();
			}
			private set
			{
				throw new NotImplementedException();
			}
		}

		[DataMember(EmitDefaultValue = false)]
		public string[] ExceptIfFromAddressContainsWords
		{
			get
			{
				return this.Rule.ExceptIfFromAddressContainsWords.ToStringArray();
			}
			private set
			{
				throw new NotImplementedException();
			}
		}

		[DataMember(EmitDefaultValue = false)]
		public bool? ExceptIfMyNameInToOrCcBox
		{
			get
			{
				if (!this.Rule.ExceptIfMyNameInToOrCcBox)
				{
					return null;
				}
				return new bool?(true);
			}
			private set
			{
				throw new NotImplementedException();
			}
		}

		[DataMember(EmitDefaultValue = false)]
		public bool? ExceptIfSentOnlyToMe
		{
			get
			{
				if (!this.Rule.ExceptIfSentOnlyToMe)
				{
					return null;
				}
				return new bool?(true);
			}
			private set
			{
				throw new NotImplementedException();
			}
		}

		[DataMember(EmitDefaultValue = false)]
		public bool? ExceptIfMyNameInToBox
		{
			get
			{
				if (!this.Rule.ExceptIfMyNameInToBox)
				{
					return null;
				}
				return new bool?(true);
			}
			private set
			{
				throw new NotImplementedException();
			}
		}

		[DataMember(EmitDefaultValue = false)]
		public bool? ExceptIfMyNameInCcBox
		{
			get
			{
				if (!this.Rule.ExceptIfMyNameInCcBox)
				{
					return null;
				}
				return new bool?(true);
			}
			private set
			{
				throw new NotImplementedException();
			}
		}

		[DataMember(EmitDefaultValue = false)]
		public bool? ExceptIfMyNameNotInToBox
		{
			get
			{
				if (!this.Rule.ExceptIfMyNameNotInToBox)
				{
					return null;
				}
				return new bool?(true);
			}
			private set
			{
				throw new NotImplementedException();
			}
		}

		[DataMember(EmitDefaultValue = false)]
		public string[] ExceptIfBodyContainsWords
		{
			get
			{
				return this.Rule.ExceptIfBodyContainsWords.ToStringArray();
			}
			private set
			{
				throw new NotImplementedException();
			}
		}

		[DataMember(EmitDefaultValue = false)]
		public string[] ExceptIfRecipientAddressContainsWords
		{
			get
			{
				return this.Rule.ExceptIfRecipientAddressContainsWords.ToStringArray();
			}
			private set
			{
				throw new NotImplementedException();
			}
		}

		[DataMember(EmitDefaultValue = false)]
		public string[] ExceptIfHeaderContainsWords
		{
			get
			{
				return this.Rule.ExceptIfHeaderContainsWords.ToStringArray();
			}
			private set
			{
				throw new NotImplementedException();
			}
		}

		[DataMember(EmitDefaultValue = false)]
		public string ExceptIfWithImportance
		{
			get
			{
				return this.Rule.ExceptIfWithImportance.ToStringWithNull();
			}
			private set
			{
				throw new NotImplementedException();
			}
		}

		[DataMember(EmitDefaultValue = false)]
		public string ExceptIfWithSensitivity
		{
			get
			{
				return this.Rule.ExceptIfWithSensitivity.ToStringWithNull();
			}
			private set
			{
				throw new NotImplementedException();
			}
		}

		[DataMember(EmitDefaultValue = false)]
		public bool? ExceptIfHasAttachment
		{
			get
			{
				if (!this.Rule.ExceptIfHasAttachment)
				{
					return null;
				}
				return new bool?(true);
			}
			private set
			{
				throw new NotImplementedException();
			}
		}

		[DataMember(EmitDefaultValue = false)]
		public string ExceptIfMessageTypeMatches
		{
			get
			{
				return this.Rule.ExceptIfMessageTypeMatches.ToStringWithNull();
			}
			private set
			{
				throw new NotImplementedException();
			}
		}

		[DataMember(EmitDefaultValue = false)]
		public Identity ExceptIfHasClassification
		{
			get
			{
				return this.Rule.ExceptIfHasClassification.ToIdentity();
			}
			private set
			{
				throw new NotImplementedException();
			}
		}

		[DataMember(EmitDefaultValue = false)]
		public string ExceptIfFlaggedForAction
		{
			get
			{
				return this.Rule.ExceptIfFlaggedForAction;
			}
			private set
			{
				throw new NotImplementedException();
			}
		}

		[DataMember(EmitDefaultValue = false)]
		public NumberRange ExceptIfWithinSizeRange
		{
			get
			{
				NumberRange numberRange = new NumberRange();
				numberRange.AtMost = this.Rule.ExceptIfWithinSizeRangeMaximum.ToKB();
				numberRange.AtLeast = this.Rule.ExceptIfWithinSizeRangeMinimum.ToKB();
				if (numberRange.AtMost == 0 && numberRange.AtLeast == 0)
				{
					return null;
				}
				return numberRange;
			}
			private set
			{
				throw new NotImplementedException();
			}
		}

		[DataMember(EmitDefaultValue = false)]
		public DateRange ExceptIfWithinDateRange
		{
			get
			{
				DateRange dateRange = new DateRange();
				dateRange.BeforeDate = this.Rule.ExceptIfReceivedBeforeDate.ToIdentity();
				dateRange.AfterDate = this.Rule.ExceptIfReceivedAfterDate.ToIdentity();
				if (dateRange.BeforeDate == null && dateRange.AfterDate == null)
				{
					return null;
				}
				return dateRange;
			}
			private set
			{
				throw new NotImplementedException();
			}
		}
	}
}
