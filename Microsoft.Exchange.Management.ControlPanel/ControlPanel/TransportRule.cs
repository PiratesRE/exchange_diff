using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage.Management;
using Microsoft.Exchange.Diagnostics.Components.Management.ControlPanel;
using Microsoft.Exchange.MessagingPolicies.Rules;
using Microsoft.Exchange.MessagingPolicies.Rules.Tasks;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[DataContract]
	[KnownType(typeof(TransportRule))]
	public class TransportRule : RuleRow
	{
		public TransportRule(Microsoft.Exchange.MessagingPolicies.Rules.Tasks.Rule rule) : base(rule)
		{
			this.Rule = rule;
			base.DescriptionObject = rule.Description;
			base.ConditionDescriptions = base.DescriptionObject.ConditionDescriptions.ToArray();
			base.ActionDescriptions = base.DescriptionObject.ActionDescriptions.ToArray();
			base.ExceptionDescriptions = base.DescriptionObject.ExceptionDescriptions.ToArray();
			base.ExpiryDateDescription = base.DescriptionObject.RuleDescriptionExpiry;
			base.ActivationDateDescription = base.DescriptionObject.RuleDescriptionActivation;
		}

		public Microsoft.Exchange.MessagingPolicies.Rules.Tasks.Rule Rule { get; private set; }

		[DataMember]
		public DateTime? ActivationDate
		{
			get
			{
				return this.Rule.ActivationDate;
			}
			private set
			{
				throw new NotSupportedException();
			}
		}

		[DataMember]
		public DateTime? ExpiryDate
		{
			get
			{
				return this.Rule.ExpiryDate;
			}
			private set
			{
				throw new NotSupportedException();
			}
		}

		[DataMember]
		public string Mode
		{
			get
			{
				return this.Rule.Mode.ToStringWithNull();
			}
			private set
			{
				throw new NotSupportedException();
			}
		}

		[DataMember]
		public string Comments
		{
			get
			{
				return this.Rule.Comments.ToStringWithNull();
			}
			private set
			{
				throw new NotSupportedException();
			}
		}

		[DataMember]
		public string RuleErrorAction
		{
			get
			{
				return this.Rule.RuleErrorAction.ToStringWithNull();
			}
			private set
			{
				throw new NotSupportedException();
			}
		}

		[DataMember]
		public string SenderAddressLocation
		{
			get
			{
				return this.Rule.SenderAddressLocation.ToStringWithNull();
			}
			private set
			{
				throw new NotSupportedException();
			}
		}

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
		public PeopleIdentity[] FromMemberOf
		{
			get
			{
				return this.Rule.FromMemberOf.ToPeopleIdentityArray();
			}
			private set
			{
				throw new NotSupportedException();
			}
		}

		[DataMember(EmitDefaultValue = false)]
		public string FromScope
		{
			get
			{
				return this.Rule.FromScope.ToStringWithNull();
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
		public PeopleIdentity[] SentToMemberOf
		{
			get
			{
				return this.Rule.SentToMemberOf.ToPeopleIdentityArray();
			}
			private set
			{
				throw new NotSupportedException();
			}
		}

		[DataMember(EmitDefaultValue = false)]
		public string SentToScope
		{
			get
			{
				return this.Rule.SentToScope.ToStringWithNull();
			}
			private set
			{
				throw new NotSupportedException();
			}
		}

		[DataMember(EmitDefaultValue = false)]
		public PeopleIdentity[] BetweenMemberOf1
		{
			get
			{
				return this.Rule.BetweenMemberOf1.ToPeopleIdentityArray();
			}
			private set
			{
				throw new NotSupportedException();
			}
		}

		[DataMember(EmitDefaultValue = false)]
		public PeopleIdentity[] BetweenMemberOf2
		{
			get
			{
				return this.Rule.BetweenMemberOf2.ToPeopleIdentityArray();
			}
			private set
			{
				throw new NotSupportedException();
			}
		}

		[DataMember(EmitDefaultValue = false)]
		public PeopleIdentity[] ManagerAddresses
		{
			get
			{
				return this.Rule.ManagerAddresses.ToPeopleIdentityArray();
			}
			private set
			{
				throw new NotSupportedException();
			}
		}

		[DataMember(EmitDefaultValue = false)]
		public string ManagerForEvaluatedUser
		{
			get
			{
				return this.Rule.ManagerForEvaluatedUser.ToStringWithNull();
			}
			private set
			{
				throw new NotSupportedException();
			}
		}

		[DataMember(EmitDefaultValue = false)]
		public string SenderManagementRelationship
		{
			get
			{
				return this.Rule.SenderManagementRelationship.ToStringWithNull();
			}
			private set
			{
				throw new NotSupportedException();
			}
		}

		[DataMember(EmitDefaultValue = false)]
		public string ADComparisonAttribute
		{
			get
			{
				return this.Rule.ADComparisonAttribute.ToStringWithNull();
			}
			private set
			{
				throw new NotSupportedException();
			}
		}

		[DataMember(EmitDefaultValue = false)]
		public string ADComparisonOperator
		{
			get
			{
				return this.Rule.ADComparisonOperator.ToStringWithNull();
			}
			private set
			{
				throw new NotSupportedException();
			}
		}

		[DataMember(EmitDefaultValue = false)]
		public PeopleIdentity[] AnyOfToHeader
		{
			get
			{
				return this.Rule.AnyOfToHeader.ToPeopleIdentityArray();
			}
			private set
			{
				throw new NotSupportedException();
			}
		}

		[DataMember(EmitDefaultValue = false)]
		public PeopleIdentity[] AnyOfToHeaderMemberOf
		{
			get
			{
				return this.Rule.AnyOfToHeaderMemberOf.ToPeopleIdentityArray();
			}
			private set
			{
				throw new NotImplementedException();
			}
		}

		[DataMember(EmitDefaultValue = false)]
		public PeopleIdentity[] AnyOfCcHeader
		{
			get
			{
				return this.Rule.AnyOfCcHeader.ToPeopleIdentityArray();
			}
			private set
			{
				throw new NotImplementedException();
			}
		}

		[DataMember(EmitDefaultValue = false)]
		public PeopleIdentity[] AnyOfCcHeaderMemberOf
		{
			get
			{
				return this.Rule.AnyOfCcHeaderMemberOf.ToPeopleIdentityArray();
			}
			private set
			{
				throw new NotImplementedException();
			}
		}

		[DataMember(EmitDefaultValue = false)]
		public PeopleIdentity[] AnyOfToCcHeader
		{
			get
			{
				return this.Rule.AnyOfToCcHeader.ToPeopleIdentityArray();
			}
			private set
			{
				throw new NotImplementedException();
			}
		}

		[DataMember(EmitDefaultValue = false)]
		public PeopleIdentity[] AnyOfToCcHeaderMemberOf
		{
			get
			{
				return this.Rule.AnyOfToCcHeaderMemberOf.ToPeopleIdentityArray();
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
		public Identity SenderInRecipientList
		{
			get
			{
				string[] array = this.Rule.SenderInRecipientList.ToStringArray();
				if (array == null)
				{
					return null;
				}
				string text = array.ToCommaSeperatedString();
				return new Identity(text, text);
			}
			private set
			{
				throw new NotImplementedException();
			}
		}

		[DataMember(EmitDefaultValue = false)]
		public Identity RecipientInSenderList
		{
			get
			{
				string[] array = this.Rule.RecipientInSenderList.ToStringArray();
				if (array == null)
				{
					return null;
				}
				string text = array.ToCommaSeperatedString();
				return new Identity(text, text);
			}
			private set
			{
				throw new NotImplementedException();
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
		public string HeaderContainsMessageHeader
		{
			get
			{
				return this.Rule.HeaderContainsMessageHeader.ToStringWithNull();
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
		public string[] AnyOfRecipientAddressContainsWords
		{
			get
			{
				return this.Rule.AnyOfRecipientAddressContainsWords.ToStringArray();
			}
			private set
			{
				throw new NotImplementedException();
			}
		}

		[DataMember(EmitDefaultValue = false)]
		public string[] AttachmentContainsWords
		{
			get
			{
				return this.Rule.AttachmentContainsWords.ToStringArray();
			}
			private set
			{
				throw new NotImplementedException();
			}
		}

		[DataMember(EmitDefaultValue = false)]
		public bool? HasNoClassification
		{
			get
			{
				if (!this.Rule.HasNoClassification)
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
		public bool? AttachmentIsUnsupported
		{
			get
			{
				if (!this.Rule.AttachmentIsUnsupported)
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
		public string[] SenderADAttributeContainsWords
		{
			get
			{
				return this.Rule.SenderADAttributeContainsWords.ToStringArray();
			}
			private set
			{
				throw new NotImplementedException();
			}
		}

		[DataMember(EmitDefaultValue = false)]
		public string[] RecipientADAttributeContainsWords
		{
			get
			{
				return this.Rule.RecipientADAttributeContainsWords.ToStringArray();
			}
			private set
			{
				throw new NotImplementedException();
			}
		}

		[DataMember(EmitDefaultValue = false)]
		public Hashtable[] MessageContainsDataClassifications
		{
			get
			{
				if (this.Rule.MessageContainsDataClassifications == null)
				{
					return null;
				}
				return this.ParseDataClassifications(this.Rule.MessageContainsDataClassifications.ToStringArray());
			}
			private set
			{
				throw new NotImplementedException();
			}
		}

		[DataMember(EmitDefaultValue = false)]
		public string[] SenderDomainIs
		{
			get
			{
				return this.Rule.SenderDomainIs.ToStringArray();
			}
			private set
			{
				throw new NotImplementedException();
			}
		}

		[DataMember(EmitDefaultValue = false)]
		public string[] RecipientDomainIs
		{
			get
			{
				return this.Rule.RecipientDomainIs.ToStringArray();
			}
			private set
			{
				throw new NotImplementedException();
			}
		}

		[DataMember(EmitDefaultValue = false)]
		public string[] ContentCharacterSetContainsWords
		{
			get
			{
				return this.Rule.ContentCharacterSetContainsWords.ToStringArray();
			}
			private set
			{
				throw new NotImplementedException();
			}
		}

		[DataMember(EmitDefaultValue = false)]
		public string[] SubjectMatchesPatterns
		{
			get
			{
				return this.Rule.SubjectMatchesPatterns.ToStringArray();
			}
			private set
			{
				throw new NotImplementedException();
			}
		}

		[DataMember(EmitDefaultValue = false)]
		public string[] SubjectOrBodyMatchesPatterns
		{
			get
			{
				return this.Rule.SubjectOrBodyMatchesPatterns.ToStringArray();
			}
			private set
			{
				throw new NotImplementedException();
			}
		}

		[DataMember(EmitDefaultValue = false)]
		public string HeaderMatchesMessageHeader
		{
			get
			{
				return this.Rule.HeaderMatchesMessageHeader.ToStringWithNull();
			}
			private set
			{
				throw new NotImplementedException();
			}
		}

		[DataMember(EmitDefaultValue = false)]
		public string[] HeaderMatchesPatterns
		{
			get
			{
				return this.Rule.HeaderMatchesPatterns.ToStringArray();
			}
			private set
			{
				throw new NotImplementedException();
			}
		}

		[DataMember(EmitDefaultValue = false)]
		public string[] FromAddressMatchesPatterns
		{
			get
			{
				return this.Rule.FromAddressMatchesPatterns.ToStringArray();
			}
			private set
			{
				throw new NotImplementedException();
			}
		}

		[DataMember(EmitDefaultValue = false)]
		public string[] AttachmentNameMatchesPatterns
		{
			get
			{
				return this.Rule.AttachmentNameMatchesPatterns.ToStringArray();
			}
			private set
			{
				throw new NotImplementedException();
			}
		}

		[DataMember(EmitDefaultValue = false)]
		public string[] AttachmentMatchesPatterns
		{
			get
			{
				return this.Rule.AttachmentMatchesPatterns.ToStringArray();
			}
			private set
			{
				throw new NotImplementedException();
			}
		}

		[DataMember(EmitDefaultValue = false)]
		public string[] RecipientAddressMatchesPatterns
		{
			get
			{
				return this.Rule.RecipientAddressMatchesPatterns.ToStringArray();
			}
			private set
			{
				throw new NotImplementedException();
			}
		}

		[DataMember(EmitDefaultValue = false)]
		public string[] AnyOfRecipientAddressMatchesPatterns
		{
			get
			{
				return this.Rule.AnyOfRecipientAddressMatchesPatterns.ToStringArray();
			}
			private set
			{
				throw new NotImplementedException();
			}
		}

		[DataMember(EmitDefaultValue = false)]
		public string[] SenderADAttributeMatchesPatterns
		{
			get
			{
				return this.Rule.SenderADAttributeMatchesPatterns.ToStringArray();
			}
			private set
			{
				throw new NotImplementedException();
			}
		}

		[DataMember(EmitDefaultValue = false)]
		public string[] RecipientADAttributeMatchesPatterns
		{
			get
			{
				return this.Rule.RecipientADAttributeMatchesPatterns.ToStringArray();
			}
			private set
			{
				throw new NotImplementedException();
			}
		}

		[DataMember(EmitDefaultValue = false)]
		public string[] AttachmentExtensionMatchesWords
		{
			get
			{
				return this.Rule.AttachmentExtensionMatchesWords.ToStringArray();
			}
			private set
			{
				throw new NotImplementedException();
			}
		}

		[DataMember(EmitDefaultValue = false)]
		public string[] SenderIpRanges
		{
			get
			{
				return this.Rule.SenderIpRanges.ToStringArray<IPRange>();
			}
			private set
			{
				throw new NotImplementedException();
			}
		}

		[DataMember(EmitDefaultValue = false)]
		public string SCLOver
		{
			get
			{
				if (this.Rule.SCLOver != null)
				{
					return this.Rule.SCLOver.Value.ToString();
				}
				return null;
			}
			private set
			{
				throw new NotImplementedException();
			}
		}

		[DataMember(EmitDefaultValue = false)]
		public long? AttachmentSizeOver
		{
			get
			{
				if (this.Rule.AttachmentSizeOver != null)
				{
					return new long?((long)this.Rule.AttachmentSizeOver.Value.ToKB());
				}
				return null;
			}
			private set
			{
				throw new NotImplementedException();
			}
		}

		[DataMember(EmitDefaultValue = false)]
		public bool AttachmentProcessingLimitExceeded
		{
			get
			{
				return this.Rule.AttachmentProcessingLimitExceeded;
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
		public long? MessageSizeOver
		{
			get
			{
				if (this.Rule.MessageSizeOver != null)
				{
					return new long?((long)this.Rule.MessageSizeOver.Value.ToKB());
				}
				return null;
			}
			private set
			{
				throw new NotImplementedException();
			}
		}

		[DataMember(EmitDefaultValue = false)]
		public bool HasSenderOverride
		{
			get
			{
				return this.Rule.HasSenderOverride;
			}
			private set
			{
				throw new NotImplementedException();
			}
		}

		[DataMember(EmitDefaultValue = false)]
		public bool AttachmentHasExecutableContent
		{
			get
			{
				return this.Rule.AttachmentHasExecutableContent;
			}
			private set
			{
				throw new NotImplementedException();
			}
		}

		[DataMember(EmitDefaultValue = false)]
		public bool AttachmentIsPasswordProtected
		{
			get
			{
				return this.Rule.AttachmentIsPasswordProtected;
			}
			private set
			{
				throw new NotImplementedException();
			}
		}

		[DataMember(EmitDefaultValue = false)]
		public string PrependSubject
		{
			get
			{
				return this.Rule.PrependSubject.ToStringWithNull();
			}
			private set
			{
				throw new NotImplementedException();
			}
		}

		[DataMember(EmitDefaultValue = false)]
		public Identity ApplyClassification
		{
			get
			{
				return this.Rule.ApplyClassification.ToIdentity();
			}
			private set
			{
				throw new NotImplementedException();
			}
		}

		[DataMember(EmitDefaultValue = false)]
		public string ApplyHtmlDisclaimerLocation
		{
			get
			{
				return this.Rule.ApplyHtmlDisclaimerLocation.ToStringWithNull();
			}
			private set
			{
				throw new NotImplementedException();
			}
		}

		[DataMember(EmitDefaultValue = false)]
		public string ApplyHtmlDisclaimerText
		{
			get
			{
				return this.Rule.ApplyHtmlDisclaimerText.ToStringWithNull();
			}
			private set
			{
				throw new NotImplementedException();
			}
		}

		[DataMember(EmitDefaultValue = false)]
		public string ApplyHtmlDisclaimerFallbackAction
		{
			get
			{
				return this.Rule.ApplyHtmlDisclaimerFallbackAction.ToStringWithNull();
			}
			private set
			{
				throw new NotImplementedException();
			}
		}

		[DataMember(EmitDefaultValue = false)]
		public string SetSCL
		{
			get
			{
				if (this.Rule.SetSCL != null)
				{
					return this.Rule.SetSCL.Value.ToString();
				}
				return null;
			}
			private set
			{
				throw new NotImplementedException();
			}
		}

		[DataMember(EmitDefaultValue = false)]
		public string SetHeaderName
		{
			get
			{
				return this.Rule.SetHeaderName.ToStringWithNull();
			}
			private set
			{
				throw new NotImplementedException();
			}
		}

		[DataMember(EmitDefaultValue = false)]
		public string SetHeaderValue
		{
			get
			{
				return this.Rule.SetHeaderValue.ToStringWithNull();
			}
			private set
			{
				throw new NotImplementedException();
			}
		}

		[DataMember(EmitDefaultValue = false)]
		public string RemoveHeader
		{
			get
			{
				return this.Rule.RemoveHeader.ToStringWithNull();
			}
			private set
			{
				throw new NotImplementedException();
			}
		}

		[DataMember(EmitDefaultValue = false)]
		public Identity ApplyRightsProtectionTemplate
		{
			get
			{
				if (this.Rule.ApplyRightsProtectionTemplate == null || string.IsNullOrEmpty(this.Rule.ApplyRightsProtectionTemplate.TemplateName))
				{
					return null;
				}
				return this.Rule.ApplyRightsProtectionTemplate.ToIdentity(this.Rule.ApplyRightsProtectionTemplate.TemplateName);
			}
			private set
			{
				throw new NotImplementedException();
			}
		}

		[DataMember(EmitDefaultValue = false)]
		public string SetAuditSeverity
		{
			get
			{
				return this.Rule.SetAuditSeverity.ToStringWithNull();
			}
			private set
			{
				throw new NotImplementedException();
			}
		}

		[DataMember(EmitDefaultValue = false)]
		public bool? StopRuleProcessing
		{
			get
			{
				if (!this.Rule.StopRuleProcessing)
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
		public PeopleIdentity[] AddToRecipients
		{
			get
			{
				return this.Rule.AddToRecipients.ToPeopleIdentityArray();
			}
			private set
			{
				throw new NotImplementedException();
			}
		}

		[DataMember(EmitDefaultValue = false)]
		public PeopleIdentity[] CopyTo
		{
			get
			{
				return this.Rule.CopyTo.ToPeopleIdentityArray();
			}
			private set
			{
				throw new NotImplementedException();
			}
		}

		[DataMember(EmitDefaultValue = false)]
		public PeopleIdentity[] BlindCopyTo
		{
			get
			{
				return this.Rule.BlindCopyTo.ToPeopleIdentityArray();
			}
			private set
			{
				throw new NotImplementedException();
			}
		}

		[DataMember(EmitDefaultValue = false)]
		public string AddManagerAsRecipientType
		{
			get
			{
				return this.Rule.AddManagerAsRecipientType.ToStringWithNull();
			}
			private set
			{
				throw new NotImplementedException();
			}
		}

		[DataMember(EmitDefaultValue = false)]
		public SenderNotifySettings SenderNotifySettings
		{
			get
			{
				if (this.Rule.SenderNotificationType == null)
				{
					return null;
				}
				return new SenderNotifySettings
				{
					NotifySender = this.Rule.SenderNotificationType.Value.ToStringWithNull(),
					RejectMessage = this.Rule.RejectMessageReasonText.ToStringWithNull()
				};
			}
			private set
			{
				throw new NotImplementedException();
			}
		}

		[DataMember(EmitDefaultValue = false)]
		public PeopleIdentity[] ModerateMessageByUser
		{
			get
			{
				return this.Rule.ModerateMessageByUser.ToPeopleIdentityArray();
			}
			private set
			{
				throw new NotImplementedException();
			}
		}

		[DataMember(EmitDefaultValue = false)]
		public bool? ModerateMessageByManager
		{
			get
			{
				if (!this.Rule.ModerateMessageByManager)
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
		public PeopleIdentity[] RedirectMessageTo
		{
			get
			{
				return this.Rule.RedirectMessageTo.ToPeopleIdentityArray();
			}
			private set
			{
				throw new NotImplementedException();
			}
		}

		[DataMember(EmitDefaultValue = false)]
		public string RejectMessageEnhancedStatusCode
		{
			get
			{
				bool flag = this.Rule.RejectMessageEnhancedStatusCode != null && this.Rule.RejectMessageEnhancedStatusCode.Value == Utils.DefaultEnhancedStatusCode.Value;
				bool flag2 = this.Rule.RejectMessageReasonText != null && string.Compare(this.Rule.RejectMessageReasonText.Value.Value, Utils.DefaultRejectText.Value, StringComparison.InvariantCultureIgnoreCase) != 0;
				if (this.Rule.SenderNotificationType != null || (flag && flag2))
				{
					return null;
				}
				return this.Rule.RejectMessageEnhancedStatusCode.ToStringWithNull();
			}
			private set
			{
				throw new NotImplementedException();
			}
		}

		[DataMember(EmitDefaultValue = false)]
		public string RejectMessageReasonText
		{
			get
			{
				if (this.Rule.SenderNotificationType != null || !string.IsNullOrEmpty(this.RejectMessageEnhancedStatusCode))
				{
					return null;
				}
				return this.Rule.RejectMessageReasonText.ToStringWithNull();
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
		public bool? RouteMessageOutboundRequireTls
		{
			get
			{
				if (!this.Rule.RouteMessageOutboundRequireTls)
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
		public bool? ApplyOME
		{
			get
			{
				if (!this.Rule.ApplyOME)
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
		public bool? RemoveOME
		{
			get
			{
				if (!this.Rule.RemoveOME)
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
		public Identity GenerateIncidentReport
		{
			get
			{
				if (this.Rule.GenerateIncidentReport != null)
				{
					ADRecipientOrAddress[] array = new ADRecipientOrAddress[]
					{
						this.Rule.GenerateIncidentReport.ToADRecipientOrAddress()
					};
					if (array.Length == 1)
					{
						PeopleIdentity peopleIdentity = array.ToPeopleIdentityArray()[0];
						return new Identity(peopleIdentity.SMTPAddress, peopleIdentity.DisplayName);
					}
				}
				return null;
			}
			private set
			{
				throw new NotImplementedException();
			}
		}

		[DataMember(EmitDefaultValue = false)]
		public string IncidentReportOriginalMail
		{
			get
			{
				if (this.Rule.IncidentReportOriginalMail == null)
				{
					return null;
				}
				return this.Rule.IncidentReportOriginalMail.Value.ToStringWithNull();
			}
			private set
			{
				throw new NotImplementedException();
			}
		}

		[DataMember(EmitDefaultValue = false)]
		public string[] IncidentReportContent
		{
			get
			{
				if (this.Rule.IncidentReportContent != null)
				{
					return (from s in this.Rule.IncidentReportContent
					select s.ToString()).ToArray<string>();
				}
				return null;
			}
			private set
			{
				throw new NotImplementedException();
			}
		}

		[DataMember(EmitDefaultValue = false)]
		public string GenerateNotification
		{
			get
			{
				return this.Rule.GenerateNotification.ToStringWithNull();
			}
			private set
			{
				throw new NotImplementedException();
			}
		}

		[DataMember(EmitDefaultValue = false)]
		public bool? Quarantine
		{
			get
			{
				if (!this.Rule.Quarantine)
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
		public Identity RouteMessageOutboundConnector
		{
			get
			{
				if (string.IsNullOrEmpty(this.Rule.RouteMessageOutboundConnector))
				{
					return null;
				}
				return new Identity(this.Rule.RouteMessageOutboundConnector);
			}
			private set
			{
				throw new NotImplementedException();
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
				throw new NotImplementedException();
			}
		}

		[DataMember(EmitDefaultValue = false)]
		public PeopleIdentity[] ExceptIfFromMemberOf
		{
			get
			{
				return this.Rule.ExceptIfFromMemberOf.ToPeopleIdentityArray();
			}
			private set
			{
				throw new NotImplementedException();
			}
		}

		[DataMember(EmitDefaultValue = false)]
		public string ExceptIfFromScope
		{
			get
			{
				return this.Rule.ExceptIfFromScope.ToStringWithNull();
			}
			private set
			{
				throw new NotImplementedException();
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
				throw new NotImplementedException();
			}
		}

		[DataMember(EmitDefaultValue = false)]
		public PeopleIdentity[] ExceptIfSentToMemberOf
		{
			get
			{
				return this.Rule.ExceptIfSentToMemberOf.ToPeopleIdentityArray();
			}
			private set
			{
				throw new NotImplementedException();
			}
		}

		[DataMember(EmitDefaultValue = false)]
		public string ExceptIfSentToScope
		{
			get
			{
				return this.Rule.ExceptIfSentToScope.ToStringWithNull();
			}
			private set
			{
				throw new NotImplementedException();
			}
		}

		[DataMember(EmitDefaultValue = false)]
		public PeopleIdentity[] ExceptIfBetweenMemberOf1
		{
			get
			{
				return this.Rule.ExceptIfBetweenMemberOf1.ToPeopleIdentityArray();
			}
			private set
			{
				throw new NotImplementedException();
			}
		}

		[DataMember(EmitDefaultValue = false)]
		public PeopleIdentity[] ExceptIfBetweenMemberOf2
		{
			get
			{
				return this.Rule.ExceptIfBetweenMemberOf2.ToPeopleIdentityArray();
			}
			private set
			{
				throw new NotImplementedException();
			}
		}

		[DataMember(EmitDefaultValue = false)]
		public PeopleIdentity[] ExceptIfManagerAddresses
		{
			get
			{
				return this.Rule.ExceptIfManagerAddresses.ToPeopleIdentityArray();
			}
			private set
			{
				throw new NotImplementedException();
			}
		}

		[DataMember(EmitDefaultValue = false)]
		public string ExceptIfManagerForEvaluatedUser
		{
			get
			{
				return this.Rule.ExceptIfManagerForEvaluatedUser.ToStringWithNull();
			}
			private set
			{
				throw new NotImplementedException();
			}
		}

		[DataMember(EmitDefaultValue = false)]
		public string ExceptIfSenderManagementRelationship
		{
			get
			{
				return this.Rule.ExceptIfSenderManagementRelationship.ToStringWithNull();
			}
			private set
			{
				throw new NotImplementedException();
			}
		}

		[DataMember(EmitDefaultValue = false)]
		public string ExceptIfADComparisonAttribute
		{
			get
			{
				return this.Rule.ExceptIfADComparisonAttribute.ToStringWithNull();
			}
			private set
			{
				throw new NotImplementedException();
			}
		}

		[DataMember(EmitDefaultValue = false)]
		public string ExceptIfADComparisonOperator
		{
			get
			{
				return this.Rule.ExceptIfADComparisonOperator.ToStringWithNull();
			}
			private set
			{
				throw new NotImplementedException();
			}
		}

		[DataMember(EmitDefaultValue = false)]
		public PeopleIdentity[] ExceptIfAnyOfToHeader
		{
			get
			{
				return this.Rule.ExceptIfAnyOfToHeader.ToPeopleIdentityArray();
			}
			private set
			{
				throw new NotImplementedException();
			}
		}

		[DataMember(EmitDefaultValue = false)]
		public PeopleIdentity[] ExceptIfAnyOfToHeaderMemberOf
		{
			get
			{
				return this.Rule.ExceptIfAnyOfToHeaderMemberOf.ToPeopleIdentityArray();
			}
			private set
			{
				throw new NotImplementedException();
			}
		}

		[DataMember(EmitDefaultValue = false)]
		public PeopleIdentity[] ExceptIfAnyOfCcHeader
		{
			get
			{
				return this.Rule.ExceptIfAnyOfCcHeader.ToPeopleIdentityArray();
			}
			private set
			{
				throw new NotImplementedException();
			}
		}

		[DataMember(EmitDefaultValue = false)]
		public PeopleIdentity[] ExceptIfAnyOfCcHeaderMemberOf
		{
			get
			{
				return this.Rule.ExceptIfAnyOfCcHeaderMemberOf.ToPeopleIdentityArray();
			}
			private set
			{
				throw new NotImplementedException();
			}
		}

		[DataMember(EmitDefaultValue = false)]
		public PeopleIdentity[] ExceptIfAnyOfToCcHeader
		{
			get
			{
				return this.Rule.ExceptIfAnyOfToCcHeader.ToPeopleIdentityArray();
			}
			private set
			{
				throw new NotImplementedException();
			}
		}

		[DataMember(EmitDefaultValue = false)]
		public PeopleIdentity[] ExceptIfAnyOfToCcHeaderMemberOf
		{
			get
			{
				return this.Rule.ExceptIfAnyOfToCcHeaderMemberOf.ToPeopleIdentityArray();
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
		public string ExceptIfHeaderContainsMessageHeader
		{
			get
			{
				return this.Rule.ExceptIfHeaderContainsMessageHeader.ToStringWithNull();
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
		public Identity ExceptIfSenderInRecipientList
		{
			get
			{
				string[] array = this.Rule.ExceptIfSenderInRecipientList.ToStringArray();
				if (array == null)
				{
					return null;
				}
				string text = array.ToCommaSeperatedString();
				return new Identity(text, text);
			}
			private set
			{
				throw new NotImplementedException();
			}
		}

		[DataMember(EmitDefaultValue = false)]
		public Identity ExceptIfRecipientInSenderList
		{
			get
			{
				string[] array = this.Rule.ExceptIfRecipientInSenderList.ToStringArray();
				if (array == null)
				{
					return null;
				}
				string text = array.ToCommaSeperatedString();
				return new Identity(text, text);
			}
			private set
			{
				throw new NotImplementedException();
			}
		}

		[DataMember(EmitDefaultValue = false)]
		public string[] ExceptIfAnyOfRecipientAddressContainsWords
		{
			get
			{
				return this.Rule.ExceptIfAnyOfRecipientAddressContainsWords.ToStringArray();
			}
			private set
			{
				throw new NotImplementedException();
			}
		}

		[DataMember(EmitDefaultValue = false)]
		public string[] ExceptIfAttachmentContainsWords
		{
			get
			{
				return this.Rule.ExceptIfAttachmentContainsWords.ToStringArray();
			}
			private set
			{
				throw new NotImplementedException();
			}
		}

		[DataMember(EmitDefaultValue = false)]
		public bool? ExceptIfHasNoClassification
		{
			get
			{
				if (!this.Rule.ExceptIfHasNoClassification)
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
		public bool? ExceptIfAttachmentIsUnsupported
		{
			get
			{
				if (!this.Rule.ExceptIfAttachmentIsUnsupported)
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
		public string[] ExceptIfSenderADAttributeContainsWords
		{
			get
			{
				return this.Rule.ExceptIfSenderADAttributeContainsWords.ToStringArray();
			}
			private set
			{
				throw new NotImplementedException();
			}
		}

		[DataMember(EmitDefaultValue = false)]
		public string[] ExceptIfRecipientADAttributeContainsWords
		{
			get
			{
				return this.Rule.ExceptIfRecipientADAttributeContainsWords.ToStringArray();
			}
			private set
			{
				throw new NotImplementedException();
			}
		}

		[DataMember(EmitDefaultValue = false)]
		public string[] ExceptIfSenderDomainIs
		{
			get
			{
				return this.Rule.ExceptIfSenderDomainIs.ToStringArray();
			}
			private set
			{
				throw new NotImplementedException();
			}
		}

		[DataMember(EmitDefaultValue = false)]
		public string[] ExceptIfRecipientDomainIs
		{
			get
			{
				return this.Rule.ExceptIfRecipientDomainIs.ToStringArray();
			}
			private set
			{
				throw new NotImplementedException();
			}
		}

		[DataMember(EmitDefaultValue = false)]
		public string[] ExceptIfContentCharacterSetContainsWords
		{
			get
			{
				return this.Rule.ExceptIfContentCharacterSetContainsWords.ToStringArray();
			}
			private set
			{
				throw new NotImplementedException();
			}
		}

		[DataMember(EmitDefaultValue = false)]
		public Hashtable[] ExceptIfMessageContainsDataClassifications
		{
			get
			{
				if (this.Rule.ExceptIfMessageContainsDataClassifications == null)
				{
					return null;
				}
				return this.ParseDataClassifications(this.Rule.ExceptIfMessageContainsDataClassifications.ToStringArray());
			}
			private set
			{
				throw new NotImplementedException();
			}
		}

		[DataMember(EmitDefaultValue = false)]
		public string[] ExceptIfSubjectMatchesPatterns
		{
			get
			{
				return this.Rule.ExceptIfSubjectMatchesPatterns.ToStringArray();
			}
			private set
			{
				throw new NotImplementedException();
			}
		}

		[DataMember(EmitDefaultValue = false)]
		public string[] ExceptIfSubjectOrBodyMatchesPatterns
		{
			get
			{
				return this.Rule.ExceptIfSubjectOrBodyMatchesPatterns.ToStringArray();
			}
			private set
			{
				throw new NotImplementedException();
			}
		}

		[DataMember(EmitDefaultValue = false)]
		public string ExceptIfHeaderMatchesMessageHeader
		{
			get
			{
				return this.Rule.ExceptIfHeaderMatchesMessageHeader.ToStringWithNull();
			}
			private set
			{
				throw new NotImplementedException();
			}
		}

		[DataMember(EmitDefaultValue = false)]
		public string[] ExceptIfHeaderMatchesPatterns
		{
			get
			{
				return this.Rule.ExceptIfHeaderMatchesPatterns.ToStringArray();
			}
			private set
			{
				throw new NotImplementedException();
			}
		}

		[DataMember(EmitDefaultValue = false)]
		public string[] ExceptIfFromAddressMatchesPatterns
		{
			get
			{
				return this.Rule.ExceptIfFromAddressMatchesPatterns.ToStringArray();
			}
			private set
			{
				throw new NotImplementedException();
			}
		}

		[DataMember(EmitDefaultValue = false)]
		public string[] ExceptIfAttachmentNameMatchesPatterns
		{
			get
			{
				return this.Rule.ExceptIfAttachmentNameMatchesPatterns.ToStringArray();
			}
			private set
			{
				throw new NotImplementedException();
			}
		}

		[DataMember(EmitDefaultValue = false)]
		public string[] ExceptIfAttachmentMatchesPatterns
		{
			get
			{
				return this.Rule.ExceptIfAttachmentMatchesPatterns.ToStringArray();
			}
			private set
			{
				throw new NotImplementedException();
			}
		}

		[DataMember(EmitDefaultValue = false)]
		public string[] ExceptIfRecipientAddressMatchesPatterns
		{
			get
			{
				return this.Rule.ExceptIfRecipientAddressMatchesPatterns.ToStringArray();
			}
			private set
			{
				throw new NotImplementedException();
			}
		}

		[DataMember(EmitDefaultValue = false)]
		public string[] ExceptIfAnyOfRecipientAddressMatchesPatterns
		{
			get
			{
				return this.Rule.ExceptIfAnyOfRecipientAddressMatchesPatterns.ToStringArray();
			}
			private set
			{
				throw new NotImplementedException();
			}
		}

		[DataMember(EmitDefaultValue = false)]
		public string[] ExceptIfSenderADAttributeMatchesPatterns
		{
			get
			{
				return this.Rule.ExceptIfSenderADAttributeMatchesPatterns.ToStringArray();
			}
			private set
			{
				throw new NotImplementedException();
			}
		}

		[DataMember(EmitDefaultValue = false)]
		public string[] ExceptIfRecipientADAttributeMatchesPatterns
		{
			get
			{
				return this.Rule.ExceptIfRecipientADAttributeMatchesPatterns.ToStringArray();
			}
			private set
			{
				throw new NotImplementedException();
			}
		}

		[DataMember(EmitDefaultValue = false)]
		public string[] ExceptIfAttachmentExtensionMatchesWords
		{
			get
			{
				return this.Rule.ExceptIfAttachmentExtensionMatchesWords.ToStringArray();
			}
			private set
			{
				throw new NotImplementedException();
			}
		}

		[DataMember(EmitDefaultValue = false)]
		public string[] ExceptIfSenderIpRanges
		{
			get
			{
				return this.Rule.ExceptIfSenderIpRanges.ToStringArray<IPRange>();
			}
			private set
			{
				throw new NotImplementedException();
			}
		}

		[DataMember(EmitDefaultValue = false)]
		public string ExceptIfSCLOver
		{
			get
			{
				if (this.Rule.ExceptIfSCLOver != null)
				{
					return this.Rule.ExceptIfSCLOver.Value.ToString();
				}
				return null;
			}
			private set
			{
				throw new NotImplementedException();
			}
		}

		[DataMember(EmitDefaultValue = false)]
		public long? ExceptIfAttachmentSizeOver
		{
			get
			{
				if (this.Rule.ExceptIfAttachmentSizeOver != null)
				{
					return new long?((long)this.Rule.ExceptIfAttachmentSizeOver.Value.ToKB());
				}
				return null;
			}
			private set
			{
				throw new NotImplementedException();
			}
		}

		[DataMember(EmitDefaultValue = false)]
		public bool ExceptIfAttachmentProcessingLimitExceeded
		{
			get
			{
				return this.Rule.ExceptIfAttachmentProcessingLimitExceeded;
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
		public long? ExceptIfMessageSizeOver
		{
			get
			{
				if (this.Rule.ExceptIfMessageSizeOver != null)
				{
					return new long?((long)this.Rule.ExceptIfMessageSizeOver.Value.ToKB());
				}
				return null;
			}
			private set
			{
				throw new NotImplementedException();
			}
		}

		[DataMember(EmitDefaultValue = false)]
		public bool ExceptIfHasSenderOverride
		{
			get
			{
				return this.Rule.ExceptIfHasSenderOverride;
			}
			private set
			{
				throw new NotImplementedException();
			}
		}

		[DataMember(EmitDefaultValue = false)]
		public bool ExceptIfAttachmentHasExecutableContent
		{
			get
			{
				return this.Rule.ExceptIfAttachmentHasExecutableContent;
			}
			private set
			{
				throw new NotImplementedException();
			}
		}

		[DataMember(EmitDefaultValue = false)]
		public bool ExceptIfAttachmentIsPasswordProtected
		{
			get
			{
				return this.Rule.ExceptIfAttachmentIsPasswordProtected;
			}
			private set
			{
				throw new NotImplementedException();
			}
		}

		internal void UpdateName(string newName)
		{
			base.Name = newName;
		}

		private Hashtable[] ParseDataClassifications(string[] classifications)
		{
			List<Hashtable> list = new List<Hashtable>();
			foreach (string text in classifications)
			{
				Hashtable hashtable = new Hashtable();
				int startIndex = 0;
				int length = text.Length - 1;
				if (text.StartsWith("{") && text.EndsWith("}"))
				{
					startIndex = 1;
					length = text.Length - 2;
				}
				try
				{
					string[] array = text.Substring(startIndex, length).Split(new char[]
					{
						','
					});
					for (int j = 0; j < array.Length; j++)
					{
						string[] array2 = array[j].Split(new char[]
						{
							':'
						});
						if (array2[1].StartsWith("\"") && array2[1].EndsWith("\""))
						{
							array2[1] = array2[1].Substring(1, array2[1].Length - 2);
						}
						string text2 = array2[1].Trim();
						if (text2.ToLower() != "infinity" && text2.ToLower() != "recommended")
						{
							hashtable[array2[0].Trim()] = text2;
						}
					}
					hashtable["name"] = hashtable["id"];
					hashtable.Remove("id");
					list.Add(hashtable);
				}
				catch (Exception ex)
				{
					ExTraceGlobals.WebServiceTracer.TraceInformation<string, string, string>(0, 0L, "Error parsing Data classifications: {0}, Exception: {1}\r\b{2}", classifications.StringArrayJoin(","), ex.GetFullMessage(), ex.StackTrace.ToString());
				}
			}
			return list.ToArray();
		}
	}
}
