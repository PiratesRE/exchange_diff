using System;
using System.Collections;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[DataContract]
	public abstract class TransportRuleParameters : SetObjectProperties
	{
		[DataMember]
		public string Name
		{
			get
			{
				return (string)base["Name"];
			}
			set
			{
				base["Name"] = value;
			}
		}

		[DataMember]
		public string DlpPolicy
		{
			get
			{
				return (string)base["DlpPolicy"];
			}
			set
			{
				base["DlpPolicy"] = value;
			}
		}

		[DataMember]
		public string Mode
		{
			get
			{
				return (string)base["Mode"];
			}
			set
			{
				base["Mode"] = value;
			}
		}

		[DataMember]
		public bool? StopRuleProcessing
		{
			get
			{
				return (bool?)base["StopRuleProcessing"];
			}
			set
			{
				base["StopRuleProcessing"] = (value ?? false);
			}
		}

		[DataMember]
		public DateTime? ActivationDate
		{
			get
			{
				return (DateTime?)base["ActivationDate"];
			}
			set
			{
				if (value != null)
				{
					base["ActivationDate"] = value.Value;
					return;
				}
				base["ActivationDate"] = null;
			}
		}

		[DataMember]
		public DateTime? ExpiryDate
		{
			get
			{
				return (DateTime?)base["ExpiryDate"];
			}
			set
			{
				if (value != null)
				{
					base["ExpiryDate"] = value.Value;
					return;
				}
				base["ExpiryDate"] = null;
			}
		}

		[DataMember]
		public string Comments
		{
			get
			{
				return (string)base["Comments"];
			}
			set
			{
				base["Comments"] = value;
			}
		}

		[DataMember]
		public string RuleErrorAction
		{
			get
			{
				return (string)base["RuleErrorAction"];
			}
			set
			{
				base["RuleErrorAction"] = value;
			}
		}

		[DataMember]
		public string SenderAddressLocation
		{
			get
			{
				return (string)base["SenderAddressLocation"];
			}
			set
			{
				base["SenderAddressLocation"] = value;
			}
		}

		[DataMember]
		public int? Priority
		{
			get
			{
				return (int?)base["Priority"];
			}
			set
			{
				base["Priority"] = value;
			}
		}

		[DataMember]
		public PeopleIdentity[] From
		{
			get
			{
				return PeopleIdentity.FromIdParameters(base["From"]);
			}
			set
			{
				base["From"] = value.ToIdParameters();
			}
		}

		[DataMember]
		public PeopleIdentity[] FromMemberOf
		{
			get
			{
				return PeopleIdentity.FromIdParameters(base["FromMemberOf"]);
			}
			set
			{
				base["FromMemberOf"] = value.ToIdParameters();
			}
		}

		[DataMember]
		public string FromScope
		{
			get
			{
				return (string)base["FromScope"];
			}
			set
			{
				base["FromScope"] = value;
			}
		}

		[DataMember]
		public PeopleIdentity[] SentTo
		{
			get
			{
				return PeopleIdentity.FromIdParameters(base["SentTo"]);
			}
			set
			{
				base["SentTo"] = value.ToIdParameters();
			}
		}

		[DataMember]
		public PeopleIdentity[] SentToMemberOf
		{
			get
			{
				return PeopleIdentity.FromIdParameters(base["SentToMemberOf"]);
			}
			set
			{
				base["SentToMemberOf"] = value.ToIdParameters();
			}
		}

		[DataMember]
		public string SentToScope
		{
			get
			{
				return (string)base["SentToScope"];
			}
			set
			{
				base["SentToScope"] = value;
			}
		}

		[DataMember]
		public PeopleIdentity[] BetweenMemberOf1
		{
			get
			{
				return PeopleIdentity.FromIdParameters(base["BetweenMemberOf1"]);
			}
			set
			{
				base["BetweenMemberOf1"] = value.ToIdParameters();
			}
		}

		[DataMember]
		public PeopleIdentity[] BetweenMemberOf2
		{
			get
			{
				return PeopleIdentity.FromIdParameters(base["BetweenMemberOf2"]);
			}
			set
			{
				base["BetweenMemberOf2"] = value.ToIdParameters();
			}
		}

		[DataMember]
		public PeopleIdentity[] ManagerAddresses
		{
			get
			{
				return PeopleIdentity.FromIdParameters(base["ManagerAddresses"]);
			}
			set
			{
				base["ManagerAddresses"] = value.ToIdParameters();
			}
		}

		[DataMember]
		public string ManagerForEvaluatedUser
		{
			get
			{
				return (string)base["ManagerForEvaluatedUser"];
			}
			set
			{
				base["ManagerForEvaluatedUser"] = value;
			}
		}

		[DataMember]
		public string SenderManagementRelationship
		{
			get
			{
				return (string)base["SenderManagementRelationship"];
			}
			set
			{
				base["SenderManagementRelationship"] = value;
			}
		}

		[DataMember]
		public string ADComparisonAttribute
		{
			get
			{
				return (string)base["ADComparisonAttribute"];
			}
			set
			{
				base["ADComparisonAttribute"] = value;
			}
		}

		[DataMember]
		public string ADComparisonOperator
		{
			get
			{
				return (string)base["ADComparisonOperator"];
			}
			set
			{
				base["ADComparisonOperator"] = value;
			}
		}

		[DataMember]
		public PeopleIdentity[] AnyOfToHeader
		{
			get
			{
				return PeopleIdentity.FromIdParameters(base["AnyOfToHeader"]);
			}
			set
			{
				base["AnyOfToHeader"] = value.ToIdParameters();
			}
		}

		[DataMember]
		public PeopleIdentity[] AnyOfToHeaderMemberOf
		{
			get
			{
				return PeopleIdentity.FromIdParameters(base["AnyOfToHeaderMemberOf"]);
			}
			set
			{
				base["AnyOfToHeaderMemberOf"] = value.ToIdParameters();
			}
		}

		[DataMember]
		public PeopleIdentity[] AnyOfCcHeader
		{
			get
			{
				return PeopleIdentity.FromIdParameters(base["AnyOfCcHeader"]);
			}
			set
			{
				base["AnyOfCcHeader"] = value.ToIdParameters();
			}
		}

		[DataMember]
		public PeopleIdentity[] AnyOfCcHeaderMemberOf
		{
			get
			{
				return PeopleIdentity.FromIdParameters(base["AnyOfCcHeaderMemberOf"]);
			}
			set
			{
				base["AnyOfCcHeaderMemberOf"] = value.ToIdParameters();
			}
		}

		[DataMember]
		public PeopleIdentity[] AnyOfToCcHeader
		{
			get
			{
				return PeopleIdentity.FromIdParameters(base["AnyOfToCcHeader"]);
			}
			set
			{
				base["AnyOfToCcHeader"] = value.ToIdParameters();
			}
		}

		[DataMember]
		public PeopleIdentity[] AnyOfToCcHeaderMemberOf
		{
			get
			{
				return PeopleIdentity.FromIdParameters(base["AnyOfToCcHeaderMemberOf"]);
			}
			set
			{
				base["AnyOfToCcHeaderMemberOf"] = value.ToIdParameters();
			}
		}

		[DataMember]
		public Identity HasClassification
		{
			get
			{
				string value = ((string[])base["HasClassification"]).ToCommaSeperatedString();
				return Identity.FromIdParameter(value);
			}
			set
			{
				base["HasClassification"] = ((value != null) ? value.RawIdentity : null);
			}
		}

		[DataMember]
		public Identity SenderInRecipientList
		{
			get
			{
				string value = ((string[])base["SenderInRecipientList"]).ToCommaSeperatedString();
				return Identity.FromIdParameter(value);
			}
			set
			{
				base["SenderInRecipientList"] = value.ToTaskIdStringArray();
			}
		}

		[DataMember]
		public Identity RecipientInSenderList
		{
			get
			{
				string value = ((string[])base["RecipientInSenderList"]).ToCommaSeperatedString();
				return Identity.FromIdParameter(value);
			}
			set
			{
				base["RecipientInSenderList"] = value.ToTaskIdStringArray();
			}
		}

		[DataMember]
		public string[] SubjectContainsWords
		{
			get
			{
				return (string[])base["SubjectContainsWords"];
			}
			set
			{
				base["SubjectContainsWords"] = value;
			}
		}

		[DataMember]
		public string[] SubjectOrBodyContainsWords
		{
			get
			{
				return (string[])base["SubjectOrBodyContainsWords"];
			}
			set
			{
				base["SubjectOrBodyContainsWords"] = value;
			}
		}

		[DataMember]
		public string HeaderContainsMessageHeader
		{
			get
			{
				return (string)base["HeaderContainsMessageHeader"];
			}
			set
			{
				base["HeaderContainsMessageHeader"] = value;
			}
		}

		[DataMember]
		public string[] HeaderContainsWords
		{
			get
			{
				return (string[])base["HeaderContainsWords"];
			}
			set
			{
				base["HeaderContainsWords"] = value;
			}
		}

		[DataMember]
		public string[] FromAddressContainsWords
		{
			get
			{
				return (string[])base["FromAddressContainsWords"];
			}
			set
			{
				base["FromAddressContainsWords"] = value;
			}
		}

		[DataMember]
		public Hashtable[] MessageContainsDataClassifications
		{
			get
			{
				return (Hashtable[])base["MessageContainsDataClassifications"];
			}
			set
			{
				base["MessageContainsDataClassifications"] = value;
			}
		}

		[DataMember]
		public string[] RecipientAddressContainsWords
		{
			get
			{
				return (string[])base["RecipientAddressContainsWords"];
			}
			set
			{
				base["RecipientAddressContainsWords"] = value;
			}
		}

		[DataMember]
		public string[] AnyOfRecipientAddressContainsWords
		{
			get
			{
				return (string[])base["AnyOfRecipientAddressContainsWords"];
			}
			set
			{
				base["AnyOfRecipientAddressContainsWords"] = value;
			}
		}

		[DataMember]
		public string[] AttachmentContainsWords
		{
			get
			{
				return (string[])base["AttachmentContainsWords"];
			}
			set
			{
				base["AttachmentContainsWords"] = value;
			}
		}

		[DataMember]
		public bool? HasNoClassification
		{
			get
			{
				return (bool?)base["HasNoClassification"];
			}
			set
			{
				base["HasNoClassification"] = (value ?? false);
			}
		}

		[DataMember]
		public bool? AttachmentIsUnsupported
		{
			get
			{
				return (bool?)base["AttachmentIsUnsupported"];
			}
			set
			{
				base["AttachmentIsUnsupported"] = (value ?? false);
			}
		}

		[DataMember]
		public string[] SenderADAttributeContainsWords
		{
			get
			{
				return (string[])base["SenderADAttributeContainsWords"];
			}
			set
			{
				base["SenderADAttributeContainsWords"] = value;
			}
		}

		[DataMember]
		public string[] RecipientADAttributeContainsWords
		{
			get
			{
				return (string[])base["RecipientADAttributeContainsWords"];
			}
			set
			{
				base["RecipientADAttributeContainsWords"] = value;
			}
		}

		[DataMember]
		public bool? AttachmentHasExecutableContent
		{
			get
			{
				return (bool?)base["AttachmentHasExecutableContent"];
			}
			set
			{
				base["AttachmentHasExecutableContent"] = (value ?? false);
			}
		}

		[DataMember]
		public string[] SubjectMatchesPatterns
		{
			get
			{
				return (string[])base["SubjectMatchesPatterns"];
			}
			set
			{
				base["SubjectMatchesPatterns"] = value;
			}
		}

		[DataMember]
		public string[] SubjectOrBodyMatchesPatterns
		{
			get
			{
				return (string[])base["SubjectOrBodyMatchesPatterns"];
			}
			set
			{
				base["SubjectOrBodyMatchesPatterns"] = value;
			}
		}

		[DataMember]
		public string HeaderMatchesMessageHeader
		{
			get
			{
				return (string)base["HeaderMatchesMessageHeader"];
			}
			set
			{
				base["HeaderMatchesMessageHeader"] = value;
			}
		}

		[DataMember]
		public string[] HeaderMatchesPatterns
		{
			get
			{
				return (string[])base["HeaderMatchesPatterns"];
			}
			set
			{
				base["HeaderMatchesPatterns"] = value;
			}
		}

		[DataMember]
		public string[] FromAddressMatchesPatterns
		{
			get
			{
				return (string[])base["FromAddressMatchesPatterns"];
			}
			set
			{
				base["FromAddressMatchesPatterns"] = value;
			}
		}

		[DataMember]
		public string[] AttachmentNameMatchesPatterns
		{
			get
			{
				return (string[])base["AttachmentNameMatchesPatterns"];
			}
			set
			{
				base["AttachmentNameMatchesPatterns"] = value;
			}
		}

		[DataMember]
		public string[] AttachmentExtensionMatchesWords
		{
			get
			{
				return (string[])base["AttachmentExtensionMatchesWords"];
			}
			set
			{
				base["AttachmentExtensionMatchesWords"] = value;
			}
		}

		[DataMember]
		public string[] AttachmentMatchesPatterns
		{
			get
			{
				return (string[])base["AttachmentMatchesPatterns"];
			}
			set
			{
				base["AttachmentMatchesPatterns"] = value;
			}
		}

		[DataMember]
		public string[] RecipientAddressMatchesPatterns
		{
			get
			{
				return (string[])base["RecipientAddressMatchesPatterns"];
			}
			set
			{
				base["RecipientAddressMatchesPatterns"] = value;
			}
		}

		[DataMember]
		public string[] AnyOfRecipientAddressMatchesPatterns
		{
			get
			{
				return (string[])base["AnyOfRecipientAddressMatchesPatterns"];
			}
			set
			{
				base["AnyOfRecipientAddressMatchesPatterns"] = value;
			}
		}

		[DataMember]
		public string[] SenderADAttributeMatchesPatterns
		{
			get
			{
				return (string[])base["SenderADAttributeMatchesPatterns"];
			}
			set
			{
				base["SenderADAttributeMatchesPatterns"] = value;
			}
		}

		[DataMember]
		public string[] RecipientADAttributeMatchesPatterns
		{
			get
			{
				return (string[])base["RecipientADAttributeMatchesPatterns"];
			}
			set
			{
				base["RecipientADAttributeMatchesPatterns"] = value;
			}
		}

		[DataMember]
		public string[] SenderIpRanges
		{
			get
			{
				return (string[])base["SenderIpRanges"];
			}
			set
			{
				base["SenderIpRanges"] = value;
			}
		}

		[DataMember]
		public string[] RecipientDomainIs
		{
			get
			{
				return (string[])base["RecipientDomainIs"];
			}
			set
			{
				base["RecipientDomainIs"] = value;
			}
		}

		[DataMember]
		public string[] SenderDomainIs
		{
			get
			{
				return (string[])base["SenderDomainIs"];
			}
			set
			{
				base["SenderDomainIs"] = value;
			}
		}

		[DataMember]
		public string[] ContentCharacterSetContainsWords
		{
			get
			{
				return (string[])base["ContentCharacterSetContainsWords"];
			}
			set
			{
				base["ContentCharacterSetContainsWords"] = value;
			}
		}

		[DataMember]
		public int? SCLOver
		{
			get
			{
				return (int?)base["SCLOver"];
			}
			set
			{
				base["SCLOver"] = value;
			}
		}

		[DataMember]
		public long? AttachmentSizeOver
		{
			get
			{
				return (long?)base["AttachmentSizeOver"];
			}
			set
			{
				if (value != null)
				{
					base["AttachmentSizeOver"] = ByteQuantifiedSize.FromKB((ulong)value.Value);
					return;
				}
				base["AttachmentSizeOver"] = null;
			}
		}

		[DataMember]
		public bool? AttachmentProcessingLimitExceeded
		{
			get
			{
				return (bool?)base["AttachmentProcessingLimitExceeded"];
			}
			set
			{
				base["AttachmentProcessingLimitExceeded"] = (value ?? false);
			}
		}

		[DataMember]
		public long? MessageSizeOver
		{
			get
			{
				return (long?)base["MessageSizeOver"];
			}
			set
			{
				if (value != null)
				{
					base["MessageSizeOver"] = ByteQuantifiedSize.FromKB((ulong)value.Value);
					return;
				}
				base["MessageSizeOver"] = null;
			}
		}

		[DataMember]
		public string SetAuditSeverity
		{
			get
			{
				return (string)base["SetAuditSeverity"];
			}
			set
			{
				base["SetAuditSeverity"] = value;
			}
		}

		[DataMember]
		public string WithImportance
		{
			get
			{
				return (string)base["WithImportance"];
			}
			set
			{
				base["WithImportance"] = value;
			}
		}

		[DataMember]
		public string MessageTypeMatches
		{
			get
			{
				return (string)base["MessageTypeMatches"];
			}
			set
			{
				base["MessageTypeMatches"] = value;
			}
		}

		[DataMember]
		public bool? HasSenderOverride
		{
			get
			{
				return (bool?)base["HasSenderOverride"];
			}
			set
			{
				base["HasSenderOverride"] = (value ?? false);
			}
		}

		[DataMember]
		public bool? AttachmentIsPasswordProtected
		{
			get
			{
				return (bool?)base["AttachmentIsPasswordProtected"];
			}
			set
			{
				base["AttachmentIsPasswordProtected"] = (value ?? false);
			}
		}

		[DataMember]
		public string PrependSubject
		{
			get
			{
				return (string)base["PrependSubject"];
			}
			set
			{
				base["PrependSubject"] = value;
			}
		}

		[DataMember]
		public string ApplyHtmlDisclaimerLocation
		{
			get
			{
				return (string)base["ApplyHtmlDisclaimerLocation"];
			}
			set
			{
				base["ApplyHtmlDisclaimerLocation"] = value;
			}
		}

		[DataMember]
		public string ApplyHtmlDisclaimerText
		{
			get
			{
				return (string)base["ApplyHtmlDisclaimerText"];
			}
			set
			{
				base["ApplyHtmlDisclaimerText"] = value;
			}
		}

		[DataMember]
		public string ApplyHtmlDisclaimerFallbackAction
		{
			get
			{
				return (string)base["ApplyHtmlDisclaimerFallbackAction"];
			}
			set
			{
				base["ApplyHtmlDisclaimerFallbackAction"] = value;
			}
		}

		[DataMember]
		public string SetHeaderName
		{
			get
			{
				return (string)base["SetHeaderName"];
			}
			set
			{
				base["SetHeaderName"] = value;
			}
		}

		[DataMember]
		public string SetHeaderValue
		{
			get
			{
				return (string)base["SetHeaderValue"];
			}
			set
			{
				base["SetHeaderValue"] = value;
			}
		}

		[DataMember]
		public string RemoveHeader
		{
			get
			{
				return (string)base["RemoveHeader"];
			}
			set
			{
				base["RemoveHeader"] = value;
			}
		}

		[DataMember]
		public Identity ApplyClassification
		{
			get
			{
				string value = ((string[])base["ApplyClassification"]).ToCommaSeperatedString();
				return Identity.FromIdParameter(value);
			}
			set
			{
				base["ApplyClassification"] = ((value != null) ? value.RawIdentity : null);
			}
		}

		[DataMember]
		public int? SetSCL
		{
			get
			{
				return (int?)base["SetSCL"];
			}
			set
			{
				base["SetSCL"] = value;
			}
		}

		[DataMember]
		public Identity ApplyRightsProtectionTemplate
		{
			get
			{
				string value = (string)base["ApplyRightsProtectionTemplate"];
				return Identity.FromIdParameter(value);
			}
			set
			{
				base["ApplyRightsProtectionTemplate"] = value;
			}
		}

		[DataMember]
		public PeopleIdentity[] AddToRecipients
		{
			get
			{
				return PeopleIdentity.FromIdParameters(base["AddToRecipients"]);
			}
			set
			{
				base["AddToRecipients"] = value.ToIdParameters();
			}
		}

		[DataMember]
		public PeopleIdentity[] CopyTo
		{
			get
			{
				return PeopleIdentity.FromIdParameters(base["CopyTo"]);
			}
			set
			{
				base["CopyTo"] = value.ToIdParameters();
			}
		}

		[DataMember]
		public PeopleIdentity[] BlindCopyTo
		{
			get
			{
				return PeopleIdentity.FromIdParameters(base["BlindCopyTo"]);
			}
			set
			{
				base["BlindCopyTo"] = value.ToIdParameters();
			}
		}

		[DataMember]
		public string AddManagerAsRecipientType
		{
			get
			{
				return (string)base["AddManagerAsRecipientType"];
			}
			set
			{
				base["AddManagerAsRecipientType"] = value;
			}
		}

		[DataMember]
		public PeopleIdentity[] ModerateMessageByUser
		{
			get
			{
				return PeopleIdentity.FromIdParameters(base["ModerateMessageByUser"]);
			}
			set
			{
				base["ModerateMessageByUser"] = value.ToIdParameters();
			}
		}

		[DataMember]
		public bool? ModerateMessageByManager
		{
			get
			{
				return (bool?)base["ModerateMessageByManager"];
			}
			set
			{
				base["ModerateMessageByManager"] = (value ?? false);
			}
		}

		[DataMember]
		public PeopleIdentity[] RedirectMessageTo
		{
			get
			{
				return PeopleIdentity.FromIdParameters(base["RedirectMessageTo"]);
			}
			set
			{
				base["RedirectMessageTo"] = value.ToIdParameters();
			}
		}

		[DataMember]
		public string RejectMessageEnhancedStatusCode
		{
			get
			{
				if (this.SenderNotifySettings.NotifySender != null)
				{
					return null;
				}
				return (string)base["RejectMessageEnhancedStatusCode"];
			}
			set
			{
				if (!string.IsNullOrEmpty(value))
				{
					base["RejectMessageEnhancedStatusCode"] = value;
				}
			}
		}

		[DataMember]
		public string RejectMessageReasonText
		{
			get
			{
				if (this.SenderNotifySettings != null)
				{
					return null;
				}
				return (string)base["RejectMessageReasonText"];
			}
			set
			{
				if (!string.IsNullOrEmpty(value))
				{
					base["RejectMessageReasonText"] = value;
				}
			}
		}

		[DataMember]
		public bool? DeleteMessage
		{
			get
			{
				return (bool?)base["DeleteMessage"];
			}
			set
			{
				base["DeleteMessage"] = (value ?? false);
			}
		}

		[DataMember]
		public Identity GenerateIncidentReport
		{
			get
			{
				return (Identity)base["GenerateIncidentReport"];
			}
			set
			{
				base["GenerateIncidentReport"] = value;
			}
		}

		[DataMember]
		public string IncidentReportOriginalMail
		{
			get
			{
				return (string)base["IncidentReportOriginalMail"];
			}
			set
			{
				base["IncidentReportOriginalMail"] = value;
			}
		}

		[DataMember]
		public string[] IncidentReportContent
		{
			get
			{
				return (string[])base["IncidentReportContent"];
			}
			set
			{
				base["IncidentReportContent"] = value;
			}
		}

		[DataMember]
		public string GenerateNotification
		{
			get
			{
				return (string)base["GenerateNotification"];
			}
			set
			{
				base["GenerateNotification"] = value;
			}
		}

		[DataMember]
		public bool? RouteMessageOutboundRequireTls
		{
			get
			{
				return (bool?)base["RouteMessageOutboundRequireTls"];
			}
			set
			{
				base["RouteMessageOutboundRequireTls"] = (value ?? false);
			}
		}

		[DataMember]
		public bool? ApplyOME
		{
			get
			{
				return (bool?)base["ApplyOME"];
			}
			set
			{
				base["ApplyOME"] = (value ?? false);
			}
		}

		[DataMember]
		public bool? RemoveOME
		{
			get
			{
				return (bool?)base["RemoveOME"];
			}
			set
			{
				base["RemoveOME"] = (value ?? false);
			}
		}

		[DataMember]
		public Identity RouteMessageOutboundConnector
		{
			get
			{
				return (Identity)base["RouteMessageOutboundConnector"];
			}
			set
			{
				base["RouteMessageOutboundConnector"] = value;
			}
		}

		[DataMember]
		public SenderNotifySettings SenderNotifySettings
		{
			get
			{
				return new SenderNotifySettings
				{
					NotifySender = (string)base["NotifySender"],
					RejectMessage = (string)base["RejectMessageReasonText"]
				};
			}
			set
			{
				if (value != null)
				{
					base["NotifySender"] = value.NotifySender;
					if (value.RejectMessage.ToStringWithNull() != null)
					{
						base["RejectMessageReasonText"] = value.RejectMessage.ToStringWithNull();
						return;
					}
				}
				else
				{
					base["NotifySender"] = null;
					base["RejectMessageReasonText"] = null;
				}
			}
		}

		[DataMember]
		public bool? Quarantine
		{
			get
			{
				return (bool?)base["Quarantine"];
			}
			set
			{
				base["Quarantine"] = (value ?? false);
			}
		}

		[DataMember]
		public PeopleIdentity[] ExceptIfFrom
		{
			get
			{
				return PeopleIdentity.FromIdParameters(base["ExceptIfFrom"]);
			}
			set
			{
				base["ExceptIfFrom"] = value.ToIdParameters();
			}
		}

		[DataMember]
		public PeopleIdentity[] ExceptIfFromMemberOf
		{
			get
			{
				return PeopleIdentity.FromIdParameters(base["ExceptIfFromMemberOf"]);
			}
			set
			{
				base["ExceptIfFromMemberOf"] = value.ToIdParameters();
			}
		}

		[DataMember]
		public string ExceptIfFromScope
		{
			get
			{
				return (string)base["ExceptIfFromScope"];
			}
			set
			{
				base["ExceptIfFromScope"] = value;
			}
		}

		[DataMember]
		public PeopleIdentity[] ExceptIfSentTo
		{
			get
			{
				return PeopleIdentity.FromIdParameters(base["ExceptIfSentTo"]);
			}
			set
			{
				base["ExceptIfSentTo"] = value.ToIdParameters();
			}
		}

		[DataMember]
		public PeopleIdentity[] ExceptIfSentToMemberOf
		{
			get
			{
				return PeopleIdentity.FromIdParameters(base["ExceptIfSentToMemberOf"]);
			}
			set
			{
				base["ExceptIfSentToMemberOf"] = value.ToIdParameters();
			}
		}

		[DataMember]
		public string ExceptIfSentToScope
		{
			get
			{
				return (string)base["ExceptIfSentToScope"];
			}
			set
			{
				base["ExceptIfSentToScope"] = value;
			}
		}

		[DataMember]
		public PeopleIdentity[] ExceptIfBetweenMemberOf1
		{
			get
			{
				return PeopleIdentity.FromIdParameters(base["ExceptIfBetweenMemberOf1"]);
			}
			set
			{
				base["ExceptIfBetweenMemberOf1"] = value.ToIdParameters();
			}
		}

		[DataMember]
		public PeopleIdentity[] ExceptIfBetweenMemberOf2
		{
			get
			{
				return PeopleIdentity.FromIdParameters(base["ExceptIfBetweenMemberOf2"]);
			}
			set
			{
				base["ExceptIfBetweenMemberOf2"] = value.ToIdParameters();
			}
		}

		[DataMember]
		public PeopleIdentity[] ExceptIfManagerAddresses
		{
			get
			{
				return PeopleIdentity.FromIdParameters(base["ExceptIfManagerAddresses"]);
			}
			set
			{
				base["ExceptIfManagerAddresses"] = value.ToIdParameters();
			}
		}

		[DataMember]
		public string ExceptIfManagerForEvaluatedUser
		{
			get
			{
				return (string)base["ExceptIfManagerForEvaluatedUser"];
			}
			set
			{
				base["ExceptIfManagerForEvaluatedUser"] = value;
			}
		}

		[DataMember]
		public string ExceptIfSenderManagementRelationship
		{
			get
			{
				return (string)base["ExceptIfSenderManagementRelationship"];
			}
			set
			{
				base["ExceptIfSenderManagementRelationship"] = value;
			}
		}

		[DataMember]
		public string ExceptIfADComparisonAttribute
		{
			get
			{
				return (string)base["ExceptIfADComparisonAttribute"];
			}
			set
			{
				base["ExceptIfADComparisonAttribute"] = value;
			}
		}

		[DataMember]
		public string ExceptIfADComparisonOperator
		{
			get
			{
				return (string)base["ExceptIfADComparisonOperator"];
			}
			set
			{
				base["ExceptIfADComparisonOperator"] = value;
			}
		}

		[DataMember]
		public Identity ExceptIfSenderInRecipientList
		{
			get
			{
				string value = ((string[])base["ExceptIfSenderInRecipientList"]).ToCommaSeperatedString();
				return Identity.FromIdParameter(value);
			}
			set
			{
				base["ExceptIfSenderInRecipientList"] = value.ToTaskIdStringArray();
			}
		}

		[DataMember]
		public Identity ExceptIfRecipientInSenderList
		{
			get
			{
				string value = ((string[])base["ExceptIfRecipientInSenderList"]).ToCommaSeperatedString();
				return Identity.FromIdParameter(value);
			}
			set
			{
				base["ExceptIfRecipientInSenderList"] = value.ToTaskIdStringArray();
			}
		}

		[DataMember]
		public PeopleIdentity[] ExceptIfAnyOfToHeader
		{
			get
			{
				return PeopleIdentity.FromIdParameters(base["ExceptIfAnyOfToHeader"]);
			}
			set
			{
				base["ExceptIfAnyOfToHeader"] = value.ToIdParameters();
			}
		}

		[DataMember]
		public PeopleIdentity[] ExceptIfAnyOfToHeaderMemberOf
		{
			get
			{
				return PeopleIdentity.FromIdParameters(base["ExceptIfAnyOfToHeaderMemberOf"]);
			}
			set
			{
				base["ExceptIfAnyOfToHeaderMemberOf"] = value.ToIdParameters();
			}
		}

		[DataMember]
		public PeopleIdentity[] ExceptIfAnyOfCcHeader
		{
			get
			{
				return PeopleIdentity.FromIdParameters(base["ExceptIfAnyOfCcHeader"]);
			}
			set
			{
				base["ExceptIfAnyOfCcHeader"] = value.ToIdParameters();
			}
		}

		[DataMember]
		public PeopleIdentity[] ExceptIfAnyOfCcHeaderMemberOf
		{
			get
			{
				return PeopleIdentity.FromIdParameters(base["ExceptIfAnyOfCcHeaderMemberOf"]);
			}
			set
			{
				base["ExceptIfAnyOfCcHeaderMemberOf"] = value.ToIdParameters();
			}
		}

		[DataMember]
		public PeopleIdentity[] ExceptIfAnyOfToCcHeader
		{
			get
			{
				return PeopleIdentity.FromIdParameters(base["ExceptIfAnyOfToCcHeader"]);
			}
			set
			{
				base["ExceptIfAnyOfToCcHeader"] = value.ToIdParameters();
			}
		}

		[DataMember]
		public PeopleIdentity[] ExceptIfAnyOfToCcHeaderMemberOf
		{
			get
			{
				return PeopleIdentity.FromIdParameters(base["ExceptIfAnyOfToCcHeaderMemberOf"]);
			}
			set
			{
				base["ExceptIfAnyOfToCcHeaderMemberOf"] = value.ToIdParameters();
			}
		}

		[DataMember]
		public Identity ExceptIfHasClassification
		{
			get
			{
				string value = ((string[])base["ExceptIfHasClassification"]).ToCommaSeperatedString();
				return Identity.FromIdParameter(value);
			}
			set
			{
				base["ExceptIfHasClassification"] = ((value != null) ? value.RawIdentity : null);
			}
		}

		[DataMember]
		public string[] ExceptIfSubjectContainsWords
		{
			get
			{
				return (string[])base["ExceptIfSubjectContainsWords"];
			}
			set
			{
				base["ExceptIfSubjectContainsWords"] = value;
			}
		}

		[DataMember]
		public string[] ExceptIfSubjectOrBodyContainsWords
		{
			get
			{
				return (string[])base["ExceptIfSubjectOrBodyContainsWords"];
			}
			set
			{
				base["ExceptIfSubjectOrBodyContainsWords"] = value;
			}
		}

		[DataMember]
		public string ExceptIfHeaderContainsMessageHeader
		{
			get
			{
				return (string)base["ExceptIfHeaderContainsMessageHeader"];
			}
			set
			{
				base["ExceptIfHeaderContainsMessageHeader"] = value;
			}
		}

		[DataMember]
		public string[] ExceptIfHeaderContainsWords
		{
			get
			{
				return (string[])base["ExceptIfHeaderContainsWords"];
			}
			set
			{
				base["ExceptIfHeaderContainsWords"] = value;
			}
		}

		[DataMember]
		public string[] ExceptIfFromAddressContainsWords
		{
			get
			{
				return (string[])base["ExceptIfFromAddressContainsWords"];
			}
			set
			{
				base["ExceptIfFromAddressContainsWords"] = value;
			}
		}

		[DataMember]
		public string[] ExceptIfRecipientAddressContainsWords
		{
			get
			{
				return (string[])base["ExceptIfRecipientAddressContainsWords"];
			}
			set
			{
				base["ExceptIfRecipientAddressContainsWords"] = value;
			}
		}

		[DataMember]
		public string[] ExceptIfAnyOfRecipientAddressContainsWords
		{
			get
			{
				return (string[])base["ExceptIfAnyOfRecipientAddressContainsWords"];
			}
			set
			{
				base["ExceptIfAnyOfRecipientAddressContainsWords"] = value;
			}
		}

		[DataMember]
		public string[] ExceptIfAttachmentContainsWords
		{
			get
			{
				return (string[])base["ExceptIfAttachmentContainsWords"];
			}
			set
			{
				base["ExceptIfAttachmentContainsWords"] = value;
			}
		}

		[DataMember]
		public bool? ExceptIfHasNoClassification
		{
			get
			{
				return (bool?)base["ExceptIfHasNoClassification"];
			}
			set
			{
				base["ExceptIfHasNoClassification"] = (value ?? false);
			}
		}

		[DataMember]
		public bool? ExceptIfAttachmentIsUnsupported
		{
			get
			{
				return (bool?)base["ExceptIfAttachmentIsUnsupported"];
			}
			set
			{
				base["ExceptIfAttachmentIsUnsupported"] = (value ?? false);
			}
		}

		[DataMember]
		public string[] ExceptIfSenderADAttributeContainsWords
		{
			get
			{
				return (string[])base["ExceptIfSenderADAttributeContainsWords"];
			}
			set
			{
				base["ExceptIfSenderADAttributeContainsWords"] = value;
			}
		}

		[DataMember]
		public string[] ExceptIfRecipientADAttributeContainsWords
		{
			get
			{
				return (string[])base["ExceptIfRecipientADAttributeContainsWords"];
			}
			set
			{
				base["ExceptIfRecipientADAttributeContainsWords"] = value;
			}
		}

		[DataMember]
		public string[] ExceptIfRecipientDomainIs
		{
			get
			{
				return (string[])base["ExceptIfRecipientDomainIs"];
			}
			set
			{
				base["ExceptIfRecipientDomainIs"] = value;
			}
		}

		[DataMember]
		public string[] ExceptIfSenderDomainIs
		{
			get
			{
				return (string[])base["ExceptIfSenderDomainIs"];
			}
			set
			{
				base["ExceptIfSenderDomainIs"] = value;
			}
		}

		[DataMember]
		public string[] ExceptIfContentCharacterSetContainsWords
		{
			get
			{
				return (string[])base["ExceptIfContentCharacterSetContainsWords"];
			}
			set
			{
				base["ExceptIfContentCharacterSetContainsWords"] = value;
			}
		}

		[DataMember]
		public string[] ExceptIfSubjectMatchesPatterns
		{
			get
			{
				return (string[])base["ExceptIfSubjectMatchesPatterns"];
			}
			set
			{
				base["ExceptIfSubjectMatchesPatterns"] = value;
			}
		}

		[DataMember]
		public string[] ExceptIfSubjectOrBodyMatchesPatterns
		{
			get
			{
				return (string[])base["ExceptIfSubjectOrBodyMatchesPatterns"];
			}
			set
			{
				base["ExceptIfSubjectOrBodyMatchesPatterns"] = value;
			}
		}

		[DataMember]
		public string ExceptIfHeaderMatchesMessageHeader
		{
			get
			{
				return (string)base["ExceptIfHeaderMatchesMessageHeader"];
			}
			set
			{
				base["ExceptIfHeaderMatchesMessageHeader"] = value;
			}
		}

		[DataMember]
		public string[] ExceptIfHeaderMatchesPatterns
		{
			get
			{
				return (string[])base["ExceptIfHeaderMatchesPatterns"];
			}
			set
			{
				base["ExceptIfHeaderMatchesPatterns"] = value;
			}
		}

		[DataMember]
		public string[] ExceptIfFromAddressMatchesPatterns
		{
			get
			{
				return (string[])base["ExceptIfFromAddressMatchesPatterns"];
			}
			set
			{
				base["ExceptIfFromAddressMatchesPatterns"] = value;
			}
		}

		[DataMember]
		public string[] ExceptIfAttachmentNameMatchesPatterns
		{
			get
			{
				return (string[])base["ExceptIfAttachmentNameMatchesPatterns"];
			}
			set
			{
				base["ExceptIfAttachmentNameMatchesPatterns"] = value;
			}
		}

		[DataMember]
		public string[] ExceptIfAttachmentMatchesPatterns
		{
			get
			{
				return (string[])base["ExceptIfAttachmentMatchesPatterns"];
			}
			set
			{
				base["ExceptIfAttachmentMatchesPatterns"] = value;
			}
		}

		[DataMember]
		public string[] ExceptIfRecipientAddressMatchesPatterns
		{
			get
			{
				return (string[])base["ExceptIfRecipientAddressMatchesPatterns"];
			}
			set
			{
				base["ExceptIfRecipientAddressMatchesPatterns"] = value;
			}
		}

		[DataMember]
		public string[] ExceptIfAnyOfRecipientAddressMatchesPatterns
		{
			get
			{
				return (string[])base["ExceptIfAnyOfRecipientAddressMatchesPatterns"];
			}
			set
			{
				base["ExceptIfAnyOfRecipientAddressMatchesPatterns"] = value;
			}
		}

		[DataMember]
		public string[] ExceptIfSenderADAttributeMatchesPatterns
		{
			get
			{
				return (string[])base["ExceptIfSenderADAttributeMatchesPatterns"];
			}
			set
			{
				base["ExceptIfSenderADAttributeMatchesPatterns"] = value;
			}
		}

		[DataMember]
		public string[] ExceptIfRecipientADAttributeMatchesPatterns
		{
			get
			{
				return (string[])base["ExceptIfRecipientADAttributeMatchesPatterns"];
			}
			set
			{
				base["ExceptIfRecipientADAttributeMatchesPatterns"] = value;
			}
		}

		[DataMember]
		public string[] ExceptIfAttachmentExtensionMatchesWords
		{
			get
			{
				return (string[])base["ExceptIfAttachmentExtensionMatchesWords"];
			}
			set
			{
				base["ExceptIfAttachmentExtensionMatchesWords"] = value;
			}
		}

		[DataMember]
		public string[] ExceptIfSenderIpRanges
		{
			get
			{
				return (string[])base["ExceptIfSenderIpRanges"];
			}
			set
			{
				base["ExceptIfSenderIpRanges"] = value;
			}
		}

		[DataMember]
		public int? ExceptIfSCLOver
		{
			get
			{
				return (int?)base["ExceptIfSCLOver"];
			}
			set
			{
				base["ExceptIfSCLOver"] = value;
			}
		}

		[DataMember]
		public long? ExceptIfAttachmentSizeOver
		{
			get
			{
				return (long?)base["ExceptIfAttachmentSizeOver"];
			}
			set
			{
				if (value != null)
				{
					base["ExceptIfAttachmentSizeOver"] = ByteQuantifiedSize.FromKB((ulong)value.Value);
					return;
				}
				base["ExceptIfAttachmentSizeOver"] = null;
			}
		}

		[DataMember]
		public bool? ExceptIfAttachmentProcessingLimitExceeded
		{
			get
			{
				return (bool?)base["ExceptIfAttachmentProcessingLimitExceeded"];
			}
			set
			{
				base["ExceptIfAttachmentProcessingLimitExceeded"] = (value ?? false);
			}
		}

		[DataMember]
		public long? ExceptIfMessageSizeOver
		{
			get
			{
				return (long?)base["ExceptIfMessageSizeOver"];
			}
			set
			{
				if (value != null)
				{
					base["ExceptIfMessageSizeOver"] = ByteQuantifiedSize.FromKB((ulong)value.Value);
					return;
				}
				base["ExceptIfMessageSizeOver"] = null;
			}
		}

		[DataMember]
		public Hashtable[] ExceptIfMessageContainsDataClassifications
		{
			get
			{
				return (Hashtable[])base["ExceptIfMessageContainsDataClassifications"];
			}
			set
			{
				base["ExceptIfMessageContainsDataClassifications"] = value;
			}
		}

		[DataMember]
		public string ExceptIfWithImportance
		{
			get
			{
				return (string)base["ExceptIfWithImportance"];
			}
			set
			{
				base["ExceptIfWithImportance"] = value;
			}
		}

		[DataMember]
		public string ExceptIfMessageTypeMatches
		{
			get
			{
				return (string)base["ExceptIfMessageTypeMatches"];
			}
			set
			{
				base["ExceptIfMessageTypeMatches"] = value;
			}
		}

		[DataMember]
		public bool? ExceptIfHasSenderOverride
		{
			get
			{
				return (bool?)base["ExceptIfHasSenderOverride"];
			}
			set
			{
				base["ExceptIfHasSenderOverride"] = (value ?? false);
			}
		}

		[DataMember]
		public bool? ExceptIfAttachmentHasExecutableContent
		{
			get
			{
				return (bool?)base["ExceptIfAttachmentHasExecutableContent"];
			}
			set
			{
				base["ExceptIfAttachmentHasExecutableContent"] = (value ?? false);
			}
		}

		[DataMember]
		public bool? ExceptIfAttachmentIsPasswordProtected
		{
			get
			{
				return (bool?)base["ExceptIfAttachmentIsPasswordProtected"];
			}
			set
			{
				base["ExceptIfAttachmentIsPasswordProtected"] = (value ?? false);
			}
		}
	}
}
