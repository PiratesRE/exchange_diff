using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.MessagingPolicies
{
	internal static class TransportRulesStrings
	{
		static TransportRulesStrings()
		{
			TransportRulesStrings.stringIDs.Add(1496915101U, "No");
			TransportRulesStrings.stringIDs.Add(3611143976U, "IncidentReportFalsePositiveLine");
			TransportRulesStrings.stringIDs.Add(3658274981U, "RuleNotInAd");
			TransportRulesStrings.stringIDs.Add(2679419348U, "IncidentReportBccLine");
			TransportRulesStrings.stringIDs.Add(1273971392U, "IncidentReportDataClassificationLine");
			TransportRulesStrings.stringIDs.Add(2547444043U, "InvalidPriority");
			TransportRulesStrings.stringIDs.Add(1190951733U, "InvalidFilteringServiceResult");
			TransportRulesStrings.stringIDs.Add(104102616U, "IncidentReportSubjectLine");
			TransportRulesStrings.stringIDs.Add(497835657U, "FailedToLoadAttachmentFilteringConfigOnStartup");
			TransportRulesStrings.stringIDs.Add(3336243037U, "IncidentReportDataClassifications");
			TransportRulesStrings.stringIDs.Add(3453011747U, "IncidentReportSender");
			TransportRulesStrings.stringIDs.Add(4028858231U, "InvalidDataClassification");
			TransportRulesStrings.stringIDs.Add(3712824958U, "SenderAddressLocationHeaderOrEnvelope");
			TransportRulesStrings.stringIDs.Add(101970590U, "IncidentReportDlpPolicyLine");
			TransportRulesStrings.stringIDs.Add(1778028774U, "IncidentReportActionLine");
			TransportRulesStrings.stringIDs.Add(1905970586U, "IncidentReportBcc");
			TransportRulesStrings.stringIDs.Add(3729543170U, "IncidentReportOverride");
			TransportRulesStrings.stringIDs.Add(3959763416U, "IncidentReportOverrideLine");
			TransportRulesStrings.stringIDs.Add(429892252U, "IncidentReportCc");
			TransportRulesStrings.stringIDs.Add(2422663214U, "IncidentReportIdMatchLine");
			TransportRulesStrings.stringIDs.Add(90811307U, "SenderAddressLocationHeader");
			TransportRulesStrings.stringIDs.Add(29398792U, "IncidentReportDoNotIncludeOriginalMail");
			TransportRulesStrings.stringIDs.Add(3226449092U, "IncidentReportIdMatch");
			TransportRulesStrings.stringIDs.Add(1864226526U, "IncidentReportCcLine");
			TransportRulesStrings.stringIDs.Add(750089942U, "IncidentReportJustificationLine");
			TransportRulesStrings.stringIDs.Add(3384738962U, "IncidentReportIdContext");
			TransportRulesStrings.stringIDs.Add(3955406766U, "IncidentReportRecipients");
			TransportRulesStrings.stringIDs.Add(1755226983U, "IncidentReportSeverity");
			TransportRulesStrings.stringIDs.Add(3488352230U, "IncidentReportFalsePositive");
			TransportRulesStrings.stringIDs.Add(1389339898U, "IncidentReportIncludeOriginalMail");
			TransportRulesStrings.stringIDs.Add(2794317127U, "IncidentReportValue");
			TransportRulesStrings.stringIDs.Add(414074715U, "UnableToUpdateRuleInAd");
			TransportRulesStrings.stringIDs.Add(1090675464U, "IncidentReportRuleDetections");
			TransportRulesStrings.stringIDs.Add(3021629903U, "Yes");
			TransportRulesStrings.stringIDs.Add(3700793673U, "IncidentReportToLine");
			TransportRulesStrings.stringIDs.Add(2156460713U, "IncidentReportSeverityLine");
			TransportRulesStrings.stringIDs.Add(492774025U, "IncidentReportRuleHitLine");
			TransportRulesStrings.stringIDs.Add(594567544U, "IncidentReportConfidenceLine");
			TransportRulesStrings.stringIDs.Add(4096407337U, "IncidentReportSenderLine");
			TransportRulesStrings.stringIDs.Add(863777729U, "IncidentReportRecommendedMinimumConfidenceLine");
			TransportRulesStrings.stringIDs.Add(234820282U, "IncidentReportMessageIdLine");
			TransportRulesStrings.stringIDs.Add(3943613787U, "IncidentReportAttachOriginalMail");
			TransportRulesStrings.stringIDs.Add(4271028853U, "IncidentReportMoreRecipients");
			TransportRulesStrings.stringIDs.Add(2110678538U, "IncidentReportSubject");
			TransportRulesStrings.stringIDs.Add(1898574625U, "IncidentReportCountLine");
			TransportRulesStrings.stringIDs.Add(4151534495U, "IncidentReportDisclaimer");
			TransportRulesStrings.stringIDs.Add(2536257678U, "SenderAddressLocationEnvelope");
			TransportRulesStrings.stringIDs.Add(4076992413U, "IncidentReportIdMatchValue");
			TransportRulesStrings.stringIDs.Add(3534659841U, "IncidentReportContextLine");
		}

		public static LocalizedString FailedToRegisterForConfigChangeNotification(string agentName)
		{
			return new LocalizedString("FailedToRegisterForConfigChangeNotification", "Ex07B14C", false, true, TransportRulesStrings.ResourceManager, new object[]
			{
				agentName
			});
		}

		public static LocalizedString No
		{
			get
			{
				return new LocalizedString("No", "", false, false, TransportRulesStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString IncidentReportFalsePositiveLine
		{
			get
			{
				return new LocalizedString("IncidentReportFalsePositiveLine", "", false, false, TransportRulesStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RuleNotInAd
		{
			get
			{
				return new LocalizedString("RuleNotInAd", "ExC86500", false, true, TransportRulesStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorInvokingFilteringService(int errorCode, string errorDescription)
		{
			return new LocalizedString("ErrorInvokingFilteringService", "", false, false, TransportRulesStrings.ResourceManager, new object[]
			{
				errorCode,
				errorDescription
			});
		}

		public static LocalizedString IncidentReportBccLine
		{
			get
			{
				return new LocalizedString("IncidentReportBccLine", "", false, false, TransportRulesStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InvalidReportDestinationArgument(object destination)
		{
			return new LocalizedString("InvalidReportDestinationArgument", "", false, false, TransportRulesStrings.ResourceManager, new object[]
			{
				destination
			});
		}

		public static LocalizedString IncidentReportDataClassificationLine
		{
			get
			{
				return new LocalizedString("IncidentReportDataClassificationLine", "", false, false, TransportRulesStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InvalidPriority
		{
			get
			{
				return new LocalizedString("InvalidPriority", "ExDE496A", false, true, TransportRulesStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InvalidFilteringServiceResult
		{
			get
			{
				return new LocalizedString("InvalidFilteringServiceResult", "", false, false, TransportRulesStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InvalidPropertyValueType(string name)
		{
			return new LocalizedString("InvalidPropertyValueType", "", false, false, TransportRulesStrings.ResourceManager, new object[]
			{
				name
			});
		}

		public static LocalizedString IncidentReportSubjectLine
		{
			get
			{
				return new LocalizedString("IncidentReportSubjectLine", "", false, false, TransportRulesStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString FailedToLoadAttachmentFilteringConfigOnStartup
		{
			get
			{
				return new LocalizedString("FailedToLoadAttachmentFilteringConfigOnStartup", "Ex0A986B", false, true, TransportRulesStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString IncidentReportDataClassifications
		{
			get
			{
				return new LocalizedString("IncidentReportDataClassifications", "", false, false, TransportRulesStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString DataClassificationPropertyRequired(string name)
		{
			return new LocalizedString("DataClassificationPropertyRequired", "", false, false, TransportRulesStrings.ResourceManager, new object[]
			{
				name
			});
		}

		public static LocalizedString IncidentReportSender
		{
			get
			{
				return new LocalizedString("IncidentReportSender", "", false, false, TransportRulesStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InvalidDataClassification
		{
			get
			{
				return new LocalizedString("InvalidDataClassification", "", false, false, TransportRulesStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString FailedToReadTransportSettingsConfiguration(string agentName)
		{
			return new LocalizedString("FailedToReadTransportSettingsConfiguration", "Ex00F967", false, true, TransportRulesStrings.ResourceManager, new object[]
			{
				agentName
			});
		}

		public static LocalizedString SenderAddressLocationHeaderOrEnvelope
		{
			get
			{
				return new LocalizedString("SenderAddressLocationHeaderOrEnvelope", "", false, false, TransportRulesStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString IncidentReportDlpPolicyLine
		{
			get
			{
				return new LocalizedString("IncidentReportDlpPolicyLine", "", false, false, TransportRulesStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InvalidKeywordInTransportRule(string keyword)
		{
			return new LocalizedString("InvalidKeywordInTransportRule", "", false, false, TransportRulesStrings.ResourceManager, new object[]
			{
				keyword
			});
		}

		public static LocalizedString RuleCollectionNotInAd(string name)
		{
			return new LocalizedString("RuleCollectionNotInAd", "Ex736C93", false, true, TransportRulesStrings.ResourceManager, new object[]
			{
				name
			});
		}

		public static LocalizedString CannotRemoveHeader(string name)
		{
			return new LocalizedString("CannotRemoveHeader", "Ex8D72A0", false, true, TransportRulesStrings.ResourceManager, new object[]
			{
				name
			});
		}

		public static LocalizedString InvalidAddress(string address)
		{
			return new LocalizedString("InvalidAddress", "ExF18033", false, true, TransportRulesStrings.ResourceManager, new object[]
			{
				address
			});
		}

		public static LocalizedString IncidentReportActionLine
		{
			get
			{
				return new LocalizedString("IncidentReportActionLine", "", false, false, TransportRulesStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString IpMatchPropertyRequired(string name)
		{
			return new LocalizedString("IpMatchPropertyRequired", "", false, false, TransportRulesStrings.ResourceManager, new object[]
			{
				name
			});
		}

		public static LocalizedString IncidentReportBcc
		{
			get
			{
				return new LocalizedString("IncidentReportBcc", "", false, false, TransportRulesStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString IncidentReportOverride
		{
			get
			{
				return new LocalizedString("IncidentReportOverride", "", false, false, TransportRulesStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString DomainIsPropertyRequired(string name)
		{
			return new LocalizedString("DomainIsPropertyRequired", "", false, false, TransportRulesStrings.ResourceManager, new object[]
			{
				name
			});
		}

		public static LocalizedString JournalingTargetDGEmptyDescription(string distributionGroup)
		{
			return new LocalizedString("JournalingTargetDGEmptyDescription", "", false, false, TransportRulesStrings.ResourceManager, new object[]
			{
				distributionGroup
			});
		}

		public static LocalizedString IncidentReportOverrideLine
		{
			get
			{
				return new LocalizedString("IncidentReportOverrideLine", "", false, false, TransportRulesStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString AttachmentReadError(string error)
		{
			return new LocalizedString("AttachmentReadError", "", false, false, TransportRulesStrings.ResourceManager, new object[]
			{
				error
			});
		}

		public static LocalizedString MessageBodyReadFailure(string error)
		{
			return new LocalizedString("MessageBodyReadFailure", "", false, false, TransportRulesStrings.ResourceManager, new object[]
			{
				error
			});
		}

		public static LocalizedString IncidentReportCc
		{
			get
			{
				return new LocalizedString("IncidentReportCc", "", false, false, TransportRulesStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString CannotSetHeader(string name, string value)
		{
			return new LocalizedString("CannotSetHeader", "Ex92ED94", false, true, TransportRulesStrings.ResourceManager, new object[]
			{
				name,
				value
			});
		}

		public static LocalizedString InvalidAttachmentPropertyParameter(string name)
		{
			return new LocalizedString("InvalidAttachmentPropertyParameter", "", false, false, TransportRulesStrings.ResourceManager, new object[]
			{
				name
			});
		}

		public static LocalizedString IncidentReportIdMatchLine
		{
			get
			{
				return new LocalizedString("IncidentReportIdMatchLine", "", false, false, TransportRulesStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString SenderAddressLocationHeader
		{
			get
			{
				return new LocalizedString("SenderAddressLocationHeader", "", false, false, TransportRulesStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString IncidentReportDoNotIncludeOriginalMail
		{
			get
			{
				return new LocalizedString("IncidentReportDoNotIncludeOriginalMail", "", false, false, TransportRulesStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString IncidentReportIdMatch
		{
			get
			{
				return new LocalizedString("IncidentReportIdMatch", "", false, false, TransportRulesStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString IncidentReportCcLine
		{
			get
			{
				return new LocalizedString("IncidentReportCcLine", "", false, false, TransportRulesStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InvalidHeaderName(string name)
		{
			return new LocalizedString("InvalidHeaderName", "ExC37D6F", false, true, TransportRulesStrings.ResourceManager, new object[]
			{
				name
			});
		}

		public static LocalizedString IncidentReportJustificationLine
		{
			get
			{
				return new LocalizedString("IncidentReportJustificationLine", "", false, false, TransportRulesStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString FailedToInitializeRMEnvironment(string agentName)
		{
			return new LocalizedString("FailedToInitializeRMEnvironment", "Ex668D19", false, true, TransportRulesStrings.ResourceManager, new object[]
			{
				agentName
			});
		}

		public static LocalizedString IncidentReportIdContext
		{
			get
			{
				return new LocalizedString("IncidentReportIdContext", "", false, false, TransportRulesStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString IncidentReportRecipients
		{
			get
			{
				return new LocalizedString("IncidentReportRecipients", "", false, false, TransportRulesStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString FipsMatchingContentRetrievalFailure(uint contentId)
		{
			return new LocalizedString("FipsMatchingContentRetrievalFailure", "", false, false, TransportRulesStrings.ResourceManager, new object[]
			{
				contentId
			});
		}

		public static LocalizedString IncidentReportSeverity
		{
			get
			{
				return new LocalizedString("IncidentReportSeverity", "", false, false, TransportRulesStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString JournalingTargetDGNotFoundDescription(string distributionGroup)
		{
			return new LocalizedString("JournalingTargetDGNotFoundDescription", "", false, false, TransportRulesStrings.ResourceManager, new object[]
			{
				distributionGroup
			});
		}

		public static LocalizedString BodyReadError(string error)
		{
			return new LocalizedString("BodyReadError", "", false, false, TransportRulesStrings.ResourceManager, new object[]
			{
				error
			});
		}

		public static LocalizedString InvalidReconciliationGuid(string guid)
		{
			return new LocalizedString("InvalidReconciliationGuid", "ExAD165A", false, true, TransportRulesStrings.ResourceManager, new object[]
			{
				guid
			});
		}

		public static LocalizedString IncidentReportFalsePositive
		{
			get
			{
				return new LocalizedString("IncidentReportFalsePositive", "", false, false, TransportRulesStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString IncidentReportIncludeOriginalMail
		{
			get
			{
				return new LocalizedString("IncidentReportIncludeOriginalMail", "", false, false, TransportRulesStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString IFilterProcessingExceptionMessage(string fileName)
		{
			return new LocalizedString("IFilterProcessingExceptionMessage", "Ex70F2FF", false, true, TransportRulesStrings.ResourceManager, new object[]
			{
				fileName
			});
		}

		public static LocalizedString InvalidRegexInTransportRule(string regex)
		{
			return new LocalizedString("InvalidRegexInTransportRule", "", false, false, TransportRulesStrings.ResourceManager, new object[]
			{
				regex
			});
		}

		public static LocalizedString IncidentReportValue
		{
			get
			{
				return new LocalizedString("IncidentReportValue", "", false, false, TransportRulesStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString UnableToUpdateRuleInAd
		{
			get
			{
				return new LocalizedString("UnableToUpdateRuleInAd", "ExF9ADCB", false, true, TransportRulesStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString IncidentReportRuleDetections
		{
			get
			{
				return new LocalizedString("IncidentReportRuleDetections", "", false, false, TransportRulesStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InvalidNotifySenderTypeArgument(object destination)
		{
			return new LocalizedString("InvalidNotifySenderTypeArgument", "", false, false, TransportRulesStrings.ResourceManager, new object[]
			{
				destination
			});
		}

		public static LocalizedString Yes
		{
			get
			{
				return new LocalizedString("Yes", "", false, false, TransportRulesStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString IncidentReportToLine
		{
			get
			{
				return new LocalizedString("IncidentReportToLine", "", false, false, TransportRulesStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString IncidentReportSeverityLine
		{
			get
			{
				return new LocalizedString("IncidentReportSeverityLine", "", false, false, TransportRulesStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString FileNameAndExtentionRetrievalFailure(string error)
		{
			return new LocalizedString("FileNameAndExtentionRetrievalFailure", "", false, false, TransportRulesStrings.ResourceManager, new object[]
			{
				error
			});
		}

		public static LocalizedString IncidentReportRuleHitLine
		{
			get
			{
				return new LocalizedString("IncidentReportRuleHitLine", "", false, false, TransportRulesStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString IncidentReportConfidenceLine
		{
			get
			{
				return new LocalizedString("IncidentReportConfidenceLine", "", false, false, TransportRulesStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString IncidentReportSenderLine
		{
			get
			{
				return new LocalizedString("IncidentReportSenderLine", "", false, false, TransportRulesStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString IncidentReportRecommendedMinimumConfidenceLine
		{
			get
			{
				return new LocalizedString("IncidentReportRecommendedMinimumConfidenceLine", "", false, false, TransportRulesStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString IncidentReportMessageIdLine
		{
			get
			{
				return new LocalizedString("IncidentReportMessageIdLine", "", false, false, TransportRulesStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString IncidentReportAttachOriginalMail
		{
			get
			{
				return new LocalizedString("IncidentReportAttachOriginalMail", "", false, false, TransportRulesStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InvalidAuditSeverityLevel(string name)
		{
			return new LocalizedString("InvalidAuditSeverityLevel", "", false, false, TransportRulesStrings.ResourceManager, new object[]
			{
				name
			});
		}

		public static LocalizedString InvalidTransportRuleEventSourceType(string typeName)
		{
			return new LocalizedString("InvalidTransportRuleEventSourceType", "", false, false, TransportRulesStrings.ResourceManager, new object[]
			{
				typeName
			});
		}

		public static LocalizedString IncidentReportMoreRecipients
		{
			get
			{
				return new LocalizedString("IncidentReportMoreRecipients", "", false, false, TransportRulesStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString IncidentReportSubject
		{
			get
			{
				return new LocalizedString("IncidentReportSubject", "", false, false, TransportRulesStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString IncidentReportCountLine
		{
			get
			{
				return new LocalizedString("IncidentReportCountLine", "", false, false, TransportRulesStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString IncidentReportDisclaimer
		{
			get
			{
				return new LocalizedString("IncidentReportDisclaimer", "", false, false, TransportRulesStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString SenderAddressLocationEnvelope
		{
			get
			{
				return new LocalizedString("SenderAddressLocationEnvelope", "", false, false, TransportRulesStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString IncidentReportIdMatchValue
		{
			get
			{
				return new LocalizedString("IncidentReportIdMatchValue", "", false, false, TransportRulesStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString FailedToLoadRuleCollection(string agentName)
		{
			return new LocalizedString("FailedToLoadRuleCollection", "ExE802B6", false, true, TransportRulesStrings.ResourceManager, new object[]
			{
				agentName
			});
		}

		public static LocalizedString IncidentReportContextLine
		{
			get
			{
				return new LocalizedString("IncidentReportContextLine", "", false, false, TransportRulesStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString IncidentReportMessageSubject(string ruleName)
		{
			return new LocalizedString("IncidentReportMessageSubject", "", false, false, TransportRulesStrings.ResourceManager, new object[]
			{
				ruleName
			});
		}

		public static LocalizedString GetLocalizedString(TransportRulesStrings.IDs key)
		{
			return new LocalizedString(TransportRulesStrings.stringIDs[(uint)key], TransportRulesStrings.ResourceManager, new object[0]);
		}

		private static Dictionary<uint, string> stringIDs = new Dictionary<uint, string>(49);

		private static ExchangeResourceManager ResourceManager = ExchangeResourceManager.GetResourceManager("Microsoft.Exchange.MessagingPolicies.TransportRulesStrings", typeof(TransportRulesStrings).GetTypeInfo().Assembly);

		public enum IDs : uint
		{
			No = 1496915101U,
			IncidentReportFalsePositiveLine = 3611143976U,
			RuleNotInAd = 3658274981U,
			IncidentReportBccLine = 2679419348U,
			IncidentReportDataClassificationLine = 1273971392U,
			InvalidPriority = 2547444043U,
			InvalidFilteringServiceResult = 1190951733U,
			IncidentReportSubjectLine = 104102616U,
			FailedToLoadAttachmentFilteringConfigOnStartup = 497835657U,
			IncidentReportDataClassifications = 3336243037U,
			IncidentReportSender = 3453011747U,
			InvalidDataClassification = 4028858231U,
			SenderAddressLocationHeaderOrEnvelope = 3712824958U,
			IncidentReportDlpPolicyLine = 101970590U,
			IncidentReportActionLine = 1778028774U,
			IncidentReportBcc = 1905970586U,
			IncidentReportOverride = 3729543170U,
			IncidentReportOverrideLine = 3959763416U,
			IncidentReportCc = 429892252U,
			IncidentReportIdMatchLine = 2422663214U,
			SenderAddressLocationHeader = 90811307U,
			IncidentReportDoNotIncludeOriginalMail = 29398792U,
			IncidentReportIdMatch = 3226449092U,
			IncidentReportCcLine = 1864226526U,
			IncidentReportJustificationLine = 750089942U,
			IncidentReportIdContext = 3384738962U,
			IncidentReportRecipients = 3955406766U,
			IncidentReportSeverity = 1755226983U,
			IncidentReportFalsePositive = 3488352230U,
			IncidentReportIncludeOriginalMail = 1389339898U,
			IncidentReportValue = 2794317127U,
			UnableToUpdateRuleInAd = 414074715U,
			IncidentReportRuleDetections = 1090675464U,
			Yes = 3021629903U,
			IncidentReportToLine = 3700793673U,
			IncidentReportSeverityLine = 2156460713U,
			IncidentReportRuleHitLine = 492774025U,
			IncidentReportConfidenceLine = 594567544U,
			IncidentReportSenderLine = 4096407337U,
			IncidentReportRecommendedMinimumConfidenceLine = 863777729U,
			IncidentReportMessageIdLine = 234820282U,
			IncidentReportAttachOriginalMail = 3943613787U,
			IncidentReportMoreRecipients = 4271028853U,
			IncidentReportSubject = 2110678538U,
			IncidentReportCountLine = 1898574625U,
			IncidentReportDisclaimer = 4151534495U,
			SenderAddressLocationEnvelope = 2536257678U,
			IncidentReportIdMatchValue = 4076992413U,
			IncidentReportContextLine = 3534659841U
		}

		private enum ParamIDs
		{
			FailedToRegisterForConfigChangeNotification,
			ErrorInvokingFilteringService,
			InvalidReportDestinationArgument,
			InvalidPropertyValueType,
			DataClassificationPropertyRequired,
			FailedToReadTransportSettingsConfiguration,
			InvalidKeywordInTransportRule,
			RuleCollectionNotInAd,
			CannotRemoveHeader,
			InvalidAddress,
			IpMatchPropertyRequired,
			DomainIsPropertyRequired,
			JournalingTargetDGEmptyDescription,
			AttachmentReadError,
			MessageBodyReadFailure,
			CannotSetHeader,
			InvalidAttachmentPropertyParameter,
			InvalidHeaderName,
			FailedToInitializeRMEnvironment,
			FipsMatchingContentRetrievalFailure,
			JournalingTargetDGNotFoundDescription,
			BodyReadError,
			InvalidReconciliationGuid,
			IFilterProcessingExceptionMessage,
			InvalidRegexInTransportRule,
			InvalidNotifySenderTypeArgument,
			FileNameAndExtentionRetrievalFailure,
			InvalidAuditSeverityLevel,
			InvalidTransportRuleEventSourceType,
			FailedToLoadRuleCollection,
			IncidentReportMessageSubject
		}
	}
}
