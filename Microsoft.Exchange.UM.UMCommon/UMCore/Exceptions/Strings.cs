using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.UM.UMCore.Exceptions
{
	internal static class Strings
	{
		static Strings()
		{
			Strings.stringIDs.Add(3474105973U, "ResolveCallerStage");
			Strings.stringIDs.Add(4197870756U, "NoDialPlanFound");
			Strings.stringIDs.Add(170519939U, "DialPlanNotFound_RetireTime");
			Strings.stringIDs.Add(537191075U, "SpeechServiceNotRunning");
			Strings.stringIDs.Add(1647771799U, "WatsoningDueToWorkerProcessNotTerminating");
			Strings.stringIDs.Add(172656460U, "MediaEdgeResourceAllocationFailed");
			Strings.stringIDs.Add(1109759235U, "MediaEdgeAuthenticationServiceDiscoveryFailed");
			Strings.stringIDs.Add(2357624513U, "PartnerGatewayNotFoundError");
			Strings.stringIDs.Add(1237921669U, "FailedQueueingWorkItemException");
			Strings.stringIDs.Add(2767459964U, "NonFunctionalAsrAA");
			Strings.stringIDs.Add(567544794U, "MobileRecoDispatcherStopping");
			Strings.stringIDs.Add(2663630261U, "SearchFolderVerificationStage");
			Strings.stringIDs.Add(2606993156U, "WatsoningDueToTimeout");
			Strings.stringIDs.Add(3371889563U, "WorkItemNeedsToBeRequeued");
			Strings.stringIDs.Add(3952240266U, "PingNoResponse");
			Strings.stringIDs.Add(3373645747U, "DialPlanObjectInvalid");
			Strings.stringIDs.Add(485458581U, "UMWorkerProcessNotAvailableError");
			Strings.stringIDs.Add(898152416U, "HeavyBlockingOperationException");
			Strings.stringIDs.Add(452693103U, "TCPOnly");
			Strings.stringIDs.Add(1961615598U, "MediaEdgeAuthenticationServiceCredentialsAcquisitionFailed");
			Strings.stringIDs.Add(1039614757U, "SipEndpointStartFailure");
			Strings.stringIDs.Add(1272750593U, "TwoExpressions");
			Strings.stringIDs.Add(2320007644U, "TransferTargetPhone");
			Strings.stringIDs.Add(189751189U, "MobileRecoDispatcherNotInitialized");
			Strings.stringIDs.Add(4029710960U, "BusinessLocationDefaultMenuName");
			Strings.stringIDs.Add(4262408002U, "NoValidResultsException");
			Strings.stringIDs.Add(320703422U, "NoSpeechDetectedException");
			Strings.stringIDs.Add(3953874673U, "UMServerDisabled");
			Strings.stringIDs.Add(3024498600U, "DisabledAA");
			Strings.stringIDs.Add(3421894090U, "ConfigurationStage");
			Strings.stringIDs.Add(1403090333U, "IPv6Only");
			Strings.stringIDs.Add(1395597532U, "ExpressionUnaryOp");
			Strings.stringIDs.Add(3380825748U, "SIPAccessServiceNotSet");
			Strings.stringIDs.Add(752583827U, "SIPSessionBorderControllerNotSet");
			Strings.stringIDs.Add(2059145634U, "InvalidSyntax");
			Strings.stringIDs.Add(339680743U, "TLSOnly");
			Strings.stringIDs.Add(1513784523U, "MediaEdgeFipsEncryptionNegotiationFailure");
			Strings.stringIDs.Add(3959337510U, "InvalidRequest");
			Strings.stringIDs.Add(697886779U, "NonFunctionalDtmfAA");
			Strings.stringIDs.Add(446476468U, "InvalidDefaultMailboxAA");
			Strings.stringIDs.Add(1862730821U, "MediaEdgeCredentialsRejected");
			Strings.stringIDs.Add(1630784093U, "AVAuthenticationServiceNotSet");
			Strings.stringIDs.Add(2954555563U, "IllegalVoipProvider");
			Strings.stringIDs.Add(1691255536U, "OperatorBinaryOp");
			Strings.stringIDs.Add(3923867977U, "PipelineCleanupGeneratedWatson");
			Strings.stringIDs.Add(95483037U, "MediaEdgeChannelEstablishmentUnknown");
			Strings.stringIDs.Add(3197238929U, "MediaEdgeResourceAllocationUnknown");
			Strings.stringIDs.Add(3039625362U, "MediaEdgeDnsResolutionFailure");
			Strings.stringIDs.Add(3718706677U, "ExpressionLeftParen");
			Strings.stringIDs.Add(2450261225U, "OutboundCallCancelled");
			Strings.stringIDs.Add(1928440680U, "UnknownNode");
			Strings.stringIDs.Add(3612559634U, "TransferTargetHost");
			Strings.stringIDs.Add(255737285U, "CacheRefreshInitialization");
			Strings.stringIDs.Add(3342577741U, "Blind");
			Strings.stringIDs.Add(2537385312U, "OperatorRightParen");
			Strings.stringIDs.Add(1382974549U, "NotificationEventFormatException");
			Strings.stringIDs.Add(861740637U, "SmtpSubmissionFailed");
			Strings.stringIDs.Add(3990320669U, "WatsoningDueToRecycling");
			Strings.stringIDs.Add(681134424U, "PipelineInitialization");
			Strings.stringIDs.Add(2918939034U, "TCPnTLS");
			Strings.stringIDs.Add(352018919U, "IPv4Only");
			Strings.stringIDs.Add(3801336377U, "MediaEdgeConnectionFailure");
			Strings.stringIDs.Add(2520682044U, "Supervised");
			Strings.stringIDs.Add(890268669U, "SourceStringInvalid");
		}

		public static LocalizedString LegacyMailboxesNotSupported(string organizationId, string callId)
		{
			return new LocalizedString("LegacyMailboxesNotSupported", Strings.ResourceManager, new object[]
			{
				organizationId,
				callId
			});
		}

		public static LocalizedString InvalidFileInVoiceMailSubmissionFolder(string file, string error)
		{
			return new LocalizedString("InvalidFileInVoiceMailSubmissionFolder", Strings.ResourceManager, new object[]
			{
				file,
				error
			});
		}

		public static LocalizedString ResolveCallerStage
		{
			get
			{
				return new LocalizedString("ResolveCallerStage", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString NoDialPlanFound
		{
			get
			{
				return new LocalizedString("NoDialPlanFound", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString PingSummaryLine(string peer, int responseCode, string responseText, string diagnostics)
		{
			return new LocalizedString("PingSummaryLine", Strings.ResourceManager, new object[]
			{
				peer,
				responseCode,
				responseText,
				diagnostics
			});
		}

		public static LocalizedString DialPlanNotFound_RetireTime
		{
			get
			{
				return new LocalizedString("DialPlanNotFound_RetireTime", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString FsmModuleNotFound(string module, string file)
		{
			return new LocalizedString("FsmModuleNotFound", Strings.ResourceManager, new object[]
			{
				module,
				file
			});
		}

		public static LocalizedString UnknownFirstActivityId(string manager)
		{
			return new LocalizedString("UnknownFirstActivityId", Strings.ResourceManager, new object[]
			{
				manager
			});
		}

		public static LocalizedString InvalidObjectGuidException(string smtpAddress)
		{
			return new LocalizedString("InvalidObjectGuidException", Strings.ResourceManager, new object[]
			{
				smtpAddress
			});
		}

		public static LocalizedString InvalidPromptResourceId(string statementId)
		{
			return new LocalizedString("InvalidPromptResourceId", Strings.ResourceManager, new object[]
			{
				statementId
			});
		}

		public static LocalizedString InvalidRecoEventDeclaration(string path, string rule)
		{
			return new LocalizedString("InvalidRecoEventDeclaration", Strings.ResourceManager, new object[]
			{
				path,
				rule
			});
		}

		public static LocalizedString UnableToInitializeResource(string reason)
		{
			return new LocalizedString("UnableToInitializeResource", Strings.ResourceManager, new object[]
			{
				reason
			});
		}

		public static LocalizedString SpeechServiceNotRunning
		{
			get
			{
				return new LocalizedString("SpeechServiceNotRunning", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ExpressionSyntaxException(string error)
		{
			return new LocalizedString("ExpressionSyntaxException", Strings.ResourceManager, new object[]
			{
				error
			});
		}

		public static LocalizedString WatsoningDueToWorkerProcessNotTerminating
		{
			get
			{
				return new LocalizedString("WatsoningDueToWorkerProcessNotTerminating", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InvalidVariable(string varName)
		{
			return new LocalizedString("InvalidVariable", Strings.ResourceManager, new object[]
			{
				varName
			});
		}

		public static LocalizedString InvalidCondition(string conditionName)
		{
			return new LocalizedString("InvalidCondition", Strings.ResourceManager, new object[]
			{
				conditionName
			});
		}

		public static LocalizedString MediaEdgeResourceAllocationFailed
		{
			get
			{
				return new LocalizedString("MediaEdgeResourceAllocationFailed", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MediaEdgeAuthenticationServiceDiscoveryFailed
		{
			get
			{
				return new LocalizedString("MediaEdgeAuthenticationServiceDiscoveryFailed", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString PartnerGatewayNotFoundError
		{
			get
			{
				return new LocalizedString("PartnerGatewayNotFoundError", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString PipelineFull(string user)
		{
			return new LocalizedString("PipelineFull", Strings.ResourceManager, new object[]
			{
				user
			});
		}

		public static LocalizedString InvalidTCPPort(string port)
		{
			return new LocalizedString("InvalidTCPPort", Strings.ResourceManager, new object[]
			{
				port
			});
		}

		public static LocalizedString DelayedPingResponse(double pingTime)
		{
			return new LocalizedString("DelayedPingResponse", Strings.ResourceManager, new object[]
			{
				pingTime
			});
		}

		public static LocalizedString FailedQueueingWorkItemException
		{
			get
			{
				return new LocalizedString("FailedQueueingWorkItemException", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString UnableToFindCertificate(string thumbprint, string server)
		{
			return new LocalizedString("UnableToFindCertificate", Strings.ResourceManager, new object[]
			{
				thumbprint,
				server
			});
		}

		public static LocalizedString SpeechGrammarFetchErrorException(string grammar)
		{
			return new LocalizedString("SpeechGrammarFetchErrorException", Strings.ResourceManager, new object[]
			{
				grammar
			});
		}

		public static LocalizedString CallFromInvalidGateway(string gatewayAddress)
		{
			return new LocalizedString("CallFromInvalidGateway", Strings.ResourceManager, new object[]
			{
				gatewayAddress
			});
		}

		public static LocalizedString UndeclaredRecoEventName(string path, string rule, string name)
		{
			return new LocalizedString("UndeclaredRecoEventName", Strings.ResourceManager, new object[]
			{
				path,
				rule,
				name
			});
		}

		public static LocalizedString FreeDiskSpaceLimitExceeded(long available, long limit)
		{
			return new LocalizedString("FreeDiskSpaceLimitExceeded", Strings.ResourceManager, new object[]
			{
				available,
				limit
			});
		}

		public static LocalizedString FileNotFound(string path)
		{
			return new LocalizedString("FileNotFound", Strings.ResourceManager, new object[]
			{
				path
			});
		}

		public static LocalizedString PromptParameterCondition(string statementId)
		{
			return new LocalizedString("PromptParameterCondition", Strings.ResourceManager, new object[]
			{
				statementId
			});
		}

		public static LocalizedString UnKnownManager(string manager)
		{
			return new LocalizedString("UnKnownManager", Strings.ResourceManager, new object[]
			{
				manager
			});
		}

		public static LocalizedString NonFunctionalAsrAA
		{
			get
			{
				return new LocalizedString("NonFunctionalAsrAA", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MinDtmfNotZeroWithNoKey(string id)
		{
			return new LocalizedString("MinDtmfNotZeroWithNoKey", Strings.ResourceManager, new object[]
			{
				id
			});
		}

		public static LocalizedString MobileRecoDispatcherStopping
		{
			get
			{
				return new LocalizedString("MobileRecoDispatcherStopping", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString SearchFolderVerificationStage
		{
			get
			{
				return new LocalizedString("SearchFolderVerificationStage", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString CacheRefreshADDeleteNotification(string name)
		{
			return new LocalizedString("CacheRefreshADDeleteNotification", Strings.ResourceManager, new object[]
			{
				name
			});
		}

		public static LocalizedString WatsoningDueToTimeout
		{
			get
			{
				return new LocalizedString("WatsoningDueToTimeout", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString DuplicateGrammarRule(string path, string rule)
		{
			return new LocalizedString("DuplicateGrammarRule", Strings.ResourceManager, new object[]
			{
				path,
				rule
			});
		}

		public static LocalizedString SpeechGrammarFetchTimeoutException(string grammar)
		{
			return new LocalizedString("SpeechGrammarFetchTimeoutException", Strings.ResourceManager, new object[]
			{
				grammar
			});
		}

		public static LocalizedString OCFeatureInvalidItemId(string itemId)
		{
			return new LocalizedString("OCFeatureInvalidItemId", Strings.ResourceManager, new object[]
			{
				itemId
			});
		}

		public static LocalizedString WorkItemNeedsToBeRequeued
		{
			get
			{
				return new LocalizedString("WorkItemNeedsToBeRequeued", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString PingNoResponse
		{
			get
			{
				return new LocalizedString("PingNoResponse", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString DialPlanObjectInvalid
		{
			get
			{
				return new LocalizedString("DialPlanObjectInvalid", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MobileRecoRPCShutdownException(Guid id)
		{
			return new LocalizedString("MobileRecoRPCShutdownException", Strings.ResourceManager, new object[]
			{
				id
			});
		}

		public static LocalizedString MaxCallsLimitReached(int numCalls)
		{
			return new LocalizedString("MaxCallsLimitReached", Strings.ResourceManager, new object[]
			{
				numCalls
			});
		}

		public static LocalizedString InvalidPerfCounterException(string counterName)
		{
			return new LocalizedString("InvalidPerfCounterException", Strings.ResourceManager, new object[]
			{
				counterName
			});
		}

		public static LocalizedString UMWorkerProcessNotAvailableError
		{
			get
			{
				return new LocalizedString("UMWorkerProcessNotAvailableError", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorChangingCertificates(string service, string server)
		{
			return new LocalizedString("ErrorChangingCertificates", Strings.ResourceManager, new object[]
			{
				service,
				server
			});
		}

		public static LocalizedString HeavyBlockingOperationException
		{
			get
			{
				return new LocalizedString("HeavyBlockingOperationException", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MailboxUnavailableException(string messageType, string database, string exceptionMessage)
		{
			return new LocalizedString("MailboxUnavailableException", Strings.ResourceManager, new object[]
			{
				messageType,
				database,
				exceptionMessage
			});
		}

		public static LocalizedString MissingMainPrompts(string id)
		{
			return new LocalizedString("MissingMainPrompts", Strings.ResourceManager, new object[]
			{
				id
			});
		}

		public static LocalizedString OCFeatureInvalidLocalResourcePath(string resourcePath)
		{
			return new LocalizedString("OCFeatureInvalidLocalResourcePath", Strings.ResourceManager, new object[]
			{
				resourcePath
			});
		}

		public static LocalizedString TCPOnly
		{
			get
			{
				return new LocalizedString("TCPOnly", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString CallFromUnknownTcpGateway(string gatewayAddress)
		{
			return new LocalizedString("CallFromUnknownTcpGateway", Strings.ResourceManager, new object[]
			{
				gatewayAddress
			});
		}

		public static LocalizedString NoValidLegacyServer(string callId, string user)
		{
			return new LocalizedString("NoValidLegacyServer", Strings.ResourceManager, new object[]
			{
				callId,
				user
			});
		}

		public static LocalizedString FaxRequestActivityWithoutFaxRequestAccepted(string id)
		{
			return new LocalizedString("FaxRequestActivityWithoutFaxRequestAccepted", Strings.ResourceManager, new object[]
			{
				id
			});
		}

		public static LocalizedString ObjectPromptsNotConsistent(string identity)
		{
			return new LocalizedString("ObjectPromptsNotConsistent", Strings.ResourceManager, new object[]
			{
				identity
			});
		}

		public static LocalizedString CallFromUnknownTlsGateway(string remoteEndpoint, string certThumbPrint, string certFqdns)
		{
			return new LocalizedString("CallFromUnknownTlsGateway", Strings.ResourceManager, new object[]
			{
				remoteEndpoint,
				certThumbPrint,
				certFqdns
			});
		}

		public static LocalizedString MediaEdgeAuthenticationServiceCredentialsAcquisitionFailed
		{
			get
			{
				return new LocalizedString("MediaEdgeAuthenticationServiceCredentialsAcquisitionFailed", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString UnknownGrammarRule(string path, string rule)
		{
			return new LocalizedString("UnknownGrammarRule", Strings.ResourceManager, new object[]
			{
				path,
				rule
			});
		}

		public static LocalizedString SipEndpointStartFailure
		{
			get
			{
				return new LocalizedString("SipEndpointStartFailure", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString TwoExpressions
		{
			get
			{
				return new LocalizedString("TwoExpressions", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorLookingUpActiveMailboxServer(string user, string callId)
		{
			return new LocalizedString("ErrorLookingUpActiveMailboxServer", Strings.ResourceManager, new object[]
			{
				user,
				callId
			});
		}

		public static LocalizedString TransferTargetPhone
		{
			get
			{
				return new LocalizedString("TransferTargetPhone", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InvalidAction(string action)
		{
			return new LocalizedString("InvalidAction", Strings.ResourceManager, new object[]
			{
				action
			});
		}

		public static LocalizedString UnknownTransitionId(string id, string source)
		{
			return new LocalizedString("UnknownTransitionId", Strings.ResourceManager, new object[]
			{
				id,
				source
			});
		}

		public static LocalizedString UnexpectedToken(string token)
		{
			return new LocalizedString("UnexpectedToken", Strings.ResourceManager, new object[]
			{
				token
			});
		}

		public static LocalizedString HeaderFileArgumentInvalid(string argName)
		{
			return new LocalizedString("HeaderFileArgumentInvalid", Strings.ResourceManager, new object[]
			{
				argName
			});
		}

		public static LocalizedString DuplicateCondition(string eventId)
		{
			return new LocalizedString("DuplicateCondition", Strings.ResourceManager, new object[]
			{
				eventId
			});
		}

		public static LocalizedString MobileRecoDispatcherNotInitialized
		{
			get
			{
				return new LocalizedString("MobileRecoDispatcherNotInitialized", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InvalidDiversionReceived(string diversion)
		{
			return new LocalizedString("InvalidDiversionReceived", Strings.ResourceManager, new object[]
			{
				diversion
			});
		}

		public static LocalizedString BusinessLocationDefaultMenuName
		{
			get
			{
				return new LocalizedString("BusinessLocationDefaultMenuName", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString NoValidResultsException
		{
			get
			{
				return new LocalizedString("NoValidResultsException", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString NoSpeechDetectedException
		{
			get
			{
				return new LocalizedString("NoSpeechDetectedException", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MissingRequiredTransition(string id, string transition)
		{
			return new LocalizedString("MissingRequiredTransition", Strings.ResourceManager, new object[]
			{
				id,
				transition
			});
		}

		public static LocalizedString FreeDiskSpaceLimitWarning(long available, long limit, long warning)
		{
			return new LocalizedString("FreeDiskSpaceLimitWarning", Strings.ResourceManager, new object[]
			{
				available,
				limit,
				warning
			});
		}

		public static LocalizedString MinDtmfZeroWithoutNoKey(string id)
		{
			return new LocalizedString("MinDtmfZeroWithoutNoKey", Strings.ResourceManager, new object[]
			{
				id
			});
		}

		public static LocalizedString UMServerDisabled
		{
			get
			{
				return new LocalizedString("UMServerDisabled", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MinNumericGreaterThanMax(string id)
		{
			return new LocalizedString("MinNumericGreaterThanMax", Strings.ResourceManager, new object[]
			{
				id
			});
		}

		public static LocalizedString InputTimeoutLessThanInterdigit(string id)
		{
			return new LocalizedString("InputTimeoutLessThanInterdigit", Strings.ResourceManager, new object[]
			{
				id
			});
		}

		public static LocalizedString DuplicateRecoRequestId(Guid id)
		{
			return new LocalizedString("DuplicateRecoRequestId", Strings.ResourceManager, new object[]
			{
				id
			});
		}

		public static LocalizedString DisabledAA
		{
			get
			{
				return new LocalizedString("DisabledAA", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ConfigurationStage
		{
			get
			{
				return new LocalizedString("ConfigurationStage", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ServerNotAssociatedWithDialPlan(string dialplan)
		{
			return new LocalizedString("ServerNotAssociatedWithDialPlan", Strings.ResourceManager, new object[]
			{
				dialplan
			});
		}

		public static LocalizedString IPv6Only
		{
			get
			{
				return new LocalizedString("IPv6Only", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ReachMaxProcessedTimes(string argName)
		{
			return new LocalizedString("ReachMaxProcessedTimes", Strings.ResourceManager, new object[]
			{
				argName
			});
		}

		public static LocalizedString ExpressionUnaryOp
		{
			get
			{
				return new LocalizedString("ExpressionUnaryOp", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MinDtmfGreaterThanMax(string id)
		{
			return new LocalizedString("MinDtmfGreaterThanMax", Strings.ResourceManager, new object[]
			{
				id
			});
		}

		public static LocalizedString SIPAccessServiceNotSet
		{
			get
			{
				return new LocalizedString("SIPAccessServiceNotSet", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString SIPSessionBorderControllerNotSet
		{
			get
			{
				return new LocalizedString("SIPSessionBorderControllerNotSet", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString CallFromInvalidHuntGroup(string huntGroup, string gatewayAddress)
		{
			return new LocalizedString("CallFromInvalidHuntGroup", Strings.ResourceManager, new object[]
			{
				huntGroup,
				gatewayAddress
			});
		}

		public static LocalizedString OCFeatureDataValidation(LocalizedString info)
		{
			return new LocalizedString("OCFeatureDataValidation", Strings.ResourceManager, new object[]
			{
				info
			});
		}

		public static LocalizedString InvalidNestedPrompt(string promptName, string promptTYpe)
		{
			return new LocalizedString("InvalidNestedPrompt", Strings.ResourceManager, new object[]
			{
				promptName,
				promptTYpe
			});
		}

		public static LocalizedString InvalidSyntax
		{
			get
			{
				return new LocalizedString("InvalidSyntax", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString TLSOnly
		{
			get
			{
				return new LocalizedString("TLSOnly", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MediaEdgeFipsEncryptionNegotiationFailure
		{
			get
			{
				return new LocalizedString("MediaEdgeFipsEncryptionNegotiationFailure", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InvalidRequest
		{
			get
			{
				return new LocalizedString("InvalidRequest", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InvalidSIPHeader(string request, string header, string value)
		{
			return new LocalizedString("InvalidSIPHeader", Strings.ResourceManager, new object[]
			{
				request,
				header,
				value
			});
		}

		public static LocalizedString DuplicateTransition(string id, string eventId)
		{
			return new LocalizedString("DuplicateTransition", Strings.ResourceManager, new object[]
			{
				id,
				eventId
			});
		}

		public static LocalizedString NoGrammarCapableMailbox(string organizationId, string callId)
		{
			return new LocalizedString("NoGrammarCapableMailbox", Strings.ResourceManager, new object[]
			{
				organizationId,
				callId
			});
		}

		public static LocalizedString InvalidGrammarResourceId(string statementId)
		{
			return new LocalizedString("InvalidGrammarResourceId", Strings.ResourceManager, new object[]
			{
				statementId
			});
		}

		public static LocalizedString NonFunctionalDtmfAA
		{
			get
			{
				return new LocalizedString("NonFunctionalDtmfAA", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InvalidDefaultMailboxAA
		{
			get
			{
				return new LocalizedString("InvalidDefaultMailboxAA", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString UnexpectedSwitchValueException(string enumValue)
		{
			return new LocalizedString("UnexpectedSwitchValueException", Strings.ResourceManager, new object[]
			{
				enumValue
			});
		}

		public static LocalizedString MissingRecoEventDeclaration(string path, string ruleName)
		{
			return new LocalizedString("MissingRecoEventDeclaration", Strings.ResourceManager, new object[]
			{
				path,
				ruleName
			});
		}

		public static LocalizedString StateMachineHalted(string id)
		{
			return new LocalizedString("StateMachineHalted", Strings.ResourceManager, new object[]
			{
				id
			});
		}

		public static LocalizedString SpeechGrammarLoadException(string grammar)
		{
			return new LocalizedString("SpeechGrammarLoadException", Strings.ResourceManager, new object[]
			{
				grammar
			});
		}

		public static LocalizedString InvalidParseState(string id, string node, string state)
		{
			return new LocalizedString("InvalidParseState", Strings.ResourceManager, new object[]
			{
				id,
				node,
				state
			});
		}

		public static LocalizedString KillWorkItemInvalidGuid(string filename)
		{
			return new LocalizedString("KillWorkItemInvalidGuid", Strings.ResourceManager, new object[]
			{
				filename
			});
		}

		public static LocalizedString MediaEdgeCredentialsRejected
		{
			get
			{
				return new LocalizedString("MediaEdgeCredentialsRejected", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString Ports(int port1, int port2)
		{
			return new LocalizedString("Ports", Strings.ResourceManager, new object[]
			{
				port1,
				port2
			});
		}

		public static LocalizedString EDiscoveryMailboxFull(string name, string exception)
		{
			return new LocalizedString("EDiscoveryMailboxFull", Strings.ResourceManager, new object[]
			{
				name,
				exception
			});
		}

		public static LocalizedString AVAuthenticationServiceNotSet
		{
			get
			{
				return new LocalizedString("AVAuthenticationServiceNotSet", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString DiagnosticCallFromRemoteHost(string gatewayAddress)
		{
			return new LocalizedString("DiagnosticCallFromRemoteHost", Strings.ResourceManager, new object[]
			{
				gatewayAddress
			});
		}

		public static LocalizedString IllegalVoipProvider
		{
			get
			{
				return new LocalizedString("IllegalVoipProvider", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString OperatorBinaryOp
		{
			get
			{
				return new LocalizedString("OperatorBinaryOp", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString UMServerNotFoundinAD(string serverFqdn)
		{
			return new LocalizedString("UMServerNotFoundinAD", Strings.ResourceManager, new object[]
			{
				serverFqdn
			});
		}

		public static LocalizedString ExpressionException(string error)
		{
			return new LocalizedString("ExpressionException", Strings.ResourceManager, new object[]
			{
				error
			});
		}

		public static LocalizedString PersonalContactsSpeechGrammarTimeoutException(string user)
		{
			return new LocalizedString("PersonalContactsSpeechGrammarTimeoutException", Strings.ResourceManager, new object[]
			{
				user
			});
		}

		public static LocalizedString UnexpectedSymbol(string symbol)
		{
			return new LocalizedString("UnexpectedSymbol", Strings.ResourceManager, new object[]
			{
				symbol
			});
		}

		public static LocalizedString KillWorkItemHeaderFileNotExist(string headerfile)
		{
			return new LocalizedString("KillWorkItemHeaderFileNotExist", Strings.ResourceManager, new object[]
			{
				headerfile
			});
		}

		public static LocalizedString GrammarFileNotFoundException(string grammarFile)
		{
			return new LocalizedString("GrammarFileNotFoundException", Strings.ResourceManager, new object[]
			{
				grammarFile
			});
		}

		public static LocalizedString ExpiredCertificate(string thumbprint, string server)
		{
			return new LocalizedString("ExpiredCertificate", Strings.ResourceManager, new object[]
			{
				thumbprint,
				server
			});
		}

		public static LocalizedString DuplicateScopedId(string id)
		{
			return new LocalizedString("DuplicateScopedId", Strings.ResourceManager, new object[]
			{
				id
			});
		}

		public static LocalizedString PersonalContactsSpeechGrammarErrorException(string user)
		{
			return new LocalizedString("PersonalContactsSpeechGrammarErrorException", Strings.ResourceManager, new object[]
			{
				user
			});
		}

		public static LocalizedString PipelineCleanupGeneratedWatson
		{
			get
			{
				return new LocalizedString("PipelineCleanupGeneratedWatson", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MediaEdgeChannelEstablishmentUnknown
		{
			get
			{
				return new LocalizedString("MediaEdgeChannelEstablishmentUnknown", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MediaEdgeResourceAllocationUnknown
		{
			get
			{
				return new LocalizedString("MediaEdgeResourceAllocationUnknown", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InvalidEvent(string tevent)
		{
			return new LocalizedString("InvalidEvent", Strings.ResourceManager, new object[]
			{
				tevent
			});
		}

		public static LocalizedString MediaEdgeDnsResolutionFailure
		{
			get
			{
				return new LocalizedString("MediaEdgeDnsResolutionFailure", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MaxGreetingLengthExceededException(int greetingLength)
		{
			return new LocalizedString("MaxGreetingLengthExceededException", Strings.ResourceManager, new object[]
			{
				greetingLength
			});
		}

		public static LocalizedString ExpressionLeftParen
		{
			get
			{
				return new LocalizedString("ExpressionLeftParen", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ConfigurationException(string msg)
		{
			return new LocalizedString("ConfigurationException", Strings.ResourceManager, new object[]
			{
				msg
			});
		}

		public static LocalizedString UMServiceBaseException(string exceptionText)
		{
			return new LocalizedString("UMServiceBaseException", Strings.ResourceManager, new object[]
			{
				exceptionText
			});
		}

		public static LocalizedString UnableToRemovePermissions(string service, int errorCode)
		{
			return new LocalizedString("UnableToRemovePermissions", Strings.ResourceManager, new object[]
			{
				service,
				errorCode
			});
		}

		public static LocalizedString UnableToStopListening(string service)
		{
			return new LocalizedString("UnableToStopListening", Strings.ResourceManager, new object[]
			{
				service
			});
		}

		public static LocalizedString MaxCallsLimitReachedWarning(int currentCalls, int maxCalls)
		{
			return new LocalizedString("MaxCallsLimitReachedWarning", Strings.ResourceManager, new object[]
			{
				currentCalls,
				maxCalls
			});
		}

		public static LocalizedString OCFeatureCAMustHaveDiversion(string feature)
		{
			return new LocalizedString("OCFeatureCAMustHaveDiversion", Strings.ResourceManager, new object[]
			{
				feature
			});
		}

		public static LocalizedString InvalidResultTypeException(string resultType)
		{
			return new LocalizedString("InvalidResultTypeException", Strings.ResourceManager, new object[]
			{
				resultType
			});
		}

		public static LocalizedString OutboundCallCancelled
		{
			get
			{
				return new LocalizedString("OutboundCallCancelled", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RecognizerNotInstalled(string engineType, string language)
		{
			return new LocalizedString("RecognizerNotInstalled", Strings.ResourceManager, new object[]
			{
				engineType,
				language
			});
		}

		public static LocalizedString ToHeaderDoesNotContainTenantGuid(string callId, string toUri)
		{
			return new LocalizedString("ToHeaderDoesNotContainTenantGuid", Strings.ResourceManager, new object[]
			{
				callId,
				toUri
			});
		}

		public static LocalizedString InvalidQualifiedName(string name)
		{
			return new LocalizedString("InvalidQualifiedName", Strings.ResourceManager, new object[]
			{
				name
			});
		}

		public static LocalizedString UnknownNode
		{
			get
			{
				return new LocalizedString("UnknownNode", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString UnableToCreateCallerPropertiesException(string typeA)
		{
			return new LocalizedString("UnableToCreateCallerPropertiesException", Strings.ResourceManager, new object[]
			{
				typeA
			});
		}

		public static LocalizedString RecordMissingTransitions(string id)
		{
			return new LocalizedString("RecordMissingTransitions", Strings.ResourceManager, new object[]
			{
				id
			});
		}

		public static LocalizedString RuleNotPublic(string path, string rule)
		{
			return new LocalizedString("RuleNotPublic", Strings.ResourceManager, new object[]
			{
				path,
				rule
			});
		}

		public static LocalizedString NotificationEventSignalingException(string msg)
		{
			return new LocalizedString("NotificationEventSignalingException", Strings.ResourceManager, new object[]
			{
				msg
			});
		}

		public static LocalizedString MissingResourcePrompt(string statementId, int lcid)
		{
			return new LocalizedString("MissingResourcePrompt", Strings.ResourceManager, new object[]
			{
				statementId,
				lcid
			});
		}

		public static LocalizedString TransferTargetHost
		{
			get
			{
				return new LocalizedString("TransferTargetHost", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString CacheRefreshInitialization
		{
			get
			{
				return new LocalizedString("CacheRefreshInitialization", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InvalidActivityManager(string name)
		{
			return new LocalizedString("InvalidActivityManager", Strings.ResourceManager, new object[]
			{
				name
			});
		}

		public static LocalizedString Blind
		{
			get
			{
				return new LocalizedString("Blind", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MaxMobileRecoRequestsReached(int current, int max)
		{
			return new LocalizedString("MaxMobileRecoRequestsReached", Strings.ResourceManager, new object[]
			{
				current,
				max
			});
		}

		public static LocalizedString GlobalGatewayWithNoMatch(string gatewayAddress, string pilotnumber)
		{
			return new LocalizedString("GlobalGatewayWithNoMatch", Strings.ResourceManager, new object[]
			{
				gatewayAddress,
				pilotnumber
			});
		}

		public static LocalizedString OperatorRightParen
		{
			get
			{
				return new LocalizedString("OperatorRightParen", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString OCFeatureSACannotHaveDiversion(string feature)
		{
			return new LocalizedString("OCFeatureSACannotHaveDiversion", Strings.ResourceManager, new object[]
			{
				feature
			});
		}

		public static LocalizedString CacheRefreshADUpdateNotification(string name)
		{
			return new LocalizedString("CacheRefreshADUpdateNotification", Strings.ResourceManager, new object[]
			{
				name
			});
		}

		public static LocalizedString NotificationEventFormatException
		{
			get
			{
				return new LocalizedString("NotificationEventFormatException", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MissingSuffixRule(string ruleName)
		{
			return new LocalizedString("MissingSuffixRule", Strings.ResourceManager, new object[]
			{
				ruleName
			});
		}

		public static LocalizedString EmptyRecoRequestId(Guid id)
		{
			return new LocalizedString("EmptyRecoRequestId", Strings.ResourceManager, new object[]
			{
				id
			});
		}

		public static LocalizedString SmtpSubmissionFailed
		{
			get
			{
				return new LocalizedString("SmtpSubmissionFailed", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InvalidPromptType(string name)
		{
			return new LocalizedString("InvalidPromptType", Strings.ResourceManager, new object[]
			{
				name
			});
		}

		public static LocalizedString ConfigFileInitializationError(string file)
		{
			return new LocalizedString("ConfigFileInitializationError", Strings.ResourceManager, new object[]
			{
				file
			});
		}

		public static LocalizedString CallCouldNotBeHandled(string callId, string remoteEndpoint)
		{
			return new LocalizedString("CallCouldNotBeHandled", Strings.ResourceManager, new object[]
			{
				callId,
				remoteEndpoint
			});
		}

		public static LocalizedString InvalidRecoRequestId(Guid id)
		{
			return new LocalizedString("InvalidRecoRequestId", Strings.ResourceManager, new object[]
			{
				id
			});
		}

		public static LocalizedString WatsoningDueToRecycling
		{
			get
			{
				return new LocalizedString("WatsoningDueToRecycling", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString PipelineInitialization
		{
			get
			{
				return new LocalizedString("PipelineInitialization", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString GrammarFetcherException(string msg)
		{
			return new LocalizedString("GrammarFetcherException", Strings.ResourceManager, new object[]
			{
				msg
			});
		}

		public static LocalizedString TCPnTLS
		{
			get
			{
				return new LocalizedString("TCPnTLS", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString UserNotFoundException(Guid id)
		{
			return new LocalizedString("UserNotFoundException", Strings.ResourceManager, new object[]
			{
				id
			});
		}

		public static LocalizedString IPv4Only
		{
			get
			{
				return new LocalizedString("IPv4Only", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InvalidTLSPort(string port)
		{
			return new LocalizedString("InvalidTLSPort", Strings.ResourceManager, new object[]
			{
				port
			});
		}

		public static LocalizedString InvalidAudioStreamException(string msg)
		{
			return new LocalizedString("InvalidAudioStreamException", Strings.ResourceManager, new object[]
			{
				msg
			});
		}

		public static LocalizedString MediaEdgeConnectionFailure
		{
			get
			{
				return new LocalizedString("MediaEdgeConnectionFailure", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString Supervised
		{
			get
			{
				return new LocalizedString("Supervised", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString PipelineFullWithCDRMessages(string name, string dbName)
		{
			return new LocalizedString("PipelineFullWithCDRMessages", Strings.ResourceManager, new object[]
			{
				name,
				dbName
			});
		}

		public static LocalizedString SourceStringInvalid
		{
			get
			{
				return new LocalizedString("SourceStringInvalid", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InvalidOperator(string op)
		{
			return new LocalizedString("InvalidOperator", Strings.ResourceManager, new object[]
			{
				op
			});
		}

		public static LocalizedString GetLocalizedString(Strings.IDs key)
		{
			return new LocalizedString(Strings.stringIDs[(uint)key], Strings.ResourceManager, new object[0]);
		}

		private static Dictionary<uint, string> stringIDs = new Dictionary<uint, string>(64);

		private static ExchangeResourceManager ResourceManager = ExchangeResourceManager.GetResourceManager("Microsoft.Exchange.UM.UMCore.Exceptions.Strings", typeof(Strings).GetTypeInfo().Assembly);

		public enum IDs : uint
		{
			ResolveCallerStage = 3474105973U,
			NoDialPlanFound = 4197870756U,
			DialPlanNotFound_RetireTime = 170519939U,
			SpeechServiceNotRunning = 537191075U,
			WatsoningDueToWorkerProcessNotTerminating = 1647771799U,
			MediaEdgeResourceAllocationFailed = 172656460U,
			MediaEdgeAuthenticationServiceDiscoveryFailed = 1109759235U,
			PartnerGatewayNotFoundError = 2357624513U,
			FailedQueueingWorkItemException = 1237921669U,
			NonFunctionalAsrAA = 2767459964U,
			MobileRecoDispatcherStopping = 567544794U,
			SearchFolderVerificationStage = 2663630261U,
			WatsoningDueToTimeout = 2606993156U,
			WorkItemNeedsToBeRequeued = 3371889563U,
			PingNoResponse = 3952240266U,
			DialPlanObjectInvalid = 3373645747U,
			UMWorkerProcessNotAvailableError = 485458581U,
			HeavyBlockingOperationException = 898152416U,
			TCPOnly = 452693103U,
			MediaEdgeAuthenticationServiceCredentialsAcquisitionFailed = 1961615598U,
			SipEndpointStartFailure = 1039614757U,
			TwoExpressions = 1272750593U,
			TransferTargetPhone = 2320007644U,
			MobileRecoDispatcherNotInitialized = 189751189U,
			BusinessLocationDefaultMenuName = 4029710960U,
			NoValidResultsException = 4262408002U,
			NoSpeechDetectedException = 320703422U,
			UMServerDisabled = 3953874673U,
			DisabledAA = 3024498600U,
			ConfigurationStage = 3421894090U,
			IPv6Only = 1403090333U,
			ExpressionUnaryOp = 1395597532U,
			SIPAccessServiceNotSet = 3380825748U,
			SIPSessionBorderControllerNotSet = 752583827U,
			InvalidSyntax = 2059145634U,
			TLSOnly = 339680743U,
			MediaEdgeFipsEncryptionNegotiationFailure = 1513784523U,
			InvalidRequest = 3959337510U,
			NonFunctionalDtmfAA = 697886779U,
			InvalidDefaultMailboxAA = 446476468U,
			MediaEdgeCredentialsRejected = 1862730821U,
			AVAuthenticationServiceNotSet = 1630784093U,
			IllegalVoipProvider = 2954555563U,
			OperatorBinaryOp = 1691255536U,
			PipelineCleanupGeneratedWatson = 3923867977U,
			MediaEdgeChannelEstablishmentUnknown = 95483037U,
			MediaEdgeResourceAllocationUnknown = 3197238929U,
			MediaEdgeDnsResolutionFailure = 3039625362U,
			ExpressionLeftParen = 3718706677U,
			OutboundCallCancelled = 2450261225U,
			UnknownNode = 1928440680U,
			TransferTargetHost = 3612559634U,
			CacheRefreshInitialization = 255737285U,
			Blind = 3342577741U,
			OperatorRightParen = 2537385312U,
			NotificationEventFormatException = 1382974549U,
			SmtpSubmissionFailed = 861740637U,
			WatsoningDueToRecycling = 3990320669U,
			PipelineInitialization = 681134424U,
			TCPnTLS = 2918939034U,
			IPv4Only = 352018919U,
			MediaEdgeConnectionFailure = 3801336377U,
			Supervised = 2520682044U,
			SourceStringInvalid = 890268669U
		}

		private enum ParamIDs
		{
			LegacyMailboxesNotSupported,
			InvalidFileInVoiceMailSubmissionFolder,
			PingSummaryLine,
			FsmModuleNotFound,
			UnknownFirstActivityId,
			InvalidObjectGuidException,
			InvalidPromptResourceId,
			InvalidRecoEventDeclaration,
			UnableToInitializeResource,
			ExpressionSyntaxException,
			InvalidVariable,
			InvalidCondition,
			PipelineFull,
			InvalidTCPPort,
			DelayedPingResponse,
			UnableToFindCertificate,
			SpeechGrammarFetchErrorException,
			CallFromInvalidGateway,
			UndeclaredRecoEventName,
			FreeDiskSpaceLimitExceeded,
			FileNotFound,
			PromptParameterCondition,
			UnKnownManager,
			MinDtmfNotZeroWithNoKey,
			CacheRefreshADDeleteNotification,
			DuplicateGrammarRule,
			SpeechGrammarFetchTimeoutException,
			OCFeatureInvalidItemId,
			MobileRecoRPCShutdownException,
			MaxCallsLimitReached,
			InvalidPerfCounterException,
			ErrorChangingCertificates,
			MailboxUnavailableException,
			MissingMainPrompts,
			OCFeatureInvalidLocalResourcePath,
			CallFromUnknownTcpGateway,
			NoValidLegacyServer,
			FaxRequestActivityWithoutFaxRequestAccepted,
			ObjectPromptsNotConsistent,
			CallFromUnknownTlsGateway,
			UnknownGrammarRule,
			ErrorLookingUpActiveMailboxServer,
			InvalidAction,
			UnknownTransitionId,
			UnexpectedToken,
			HeaderFileArgumentInvalid,
			DuplicateCondition,
			InvalidDiversionReceived,
			MissingRequiredTransition,
			FreeDiskSpaceLimitWarning,
			MinDtmfZeroWithoutNoKey,
			MinNumericGreaterThanMax,
			InputTimeoutLessThanInterdigit,
			DuplicateRecoRequestId,
			ServerNotAssociatedWithDialPlan,
			ReachMaxProcessedTimes,
			MinDtmfGreaterThanMax,
			CallFromInvalidHuntGroup,
			OCFeatureDataValidation,
			InvalidNestedPrompt,
			InvalidSIPHeader,
			DuplicateTransition,
			NoGrammarCapableMailbox,
			InvalidGrammarResourceId,
			UnexpectedSwitchValueException,
			MissingRecoEventDeclaration,
			StateMachineHalted,
			SpeechGrammarLoadException,
			InvalidParseState,
			KillWorkItemInvalidGuid,
			Ports,
			EDiscoveryMailboxFull,
			DiagnosticCallFromRemoteHost,
			UMServerNotFoundinAD,
			ExpressionException,
			PersonalContactsSpeechGrammarTimeoutException,
			UnexpectedSymbol,
			KillWorkItemHeaderFileNotExist,
			GrammarFileNotFoundException,
			ExpiredCertificate,
			DuplicateScopedId,
			PersonalContactsSpeechGrammarErrorException,
			InvalidEvent,
			MaxGreetingLengthExceededException,
			ConfigurationException,
			UMServiceBaseException,
			UnableToRemovePermissions,
			UnableToStopListening,
			MaxCallsLimitReachedWarning,
			OCFeatureCAMustHaveDiversion,
			InvalidResultTypeException,
			RecognizerNotInstalled,
			ToHeaderDoesNotContainTenantGuid,
			InvalidQualifiedName,
			UnableToCreateCallerPropertiesException,
			RecordMissingTransitions,
			RuleNotPublic,
			NotificationEventSignalingException,
			MissingResourcePrompt,
			InvalidActivityManager,
			MaxMobileRecoRequestsReached,
			GlobalGatewayWithNoMatch,
			OCFeatureSACannotHaveDiversion,
			CacheRefreshADUpdateNotification,
			MissingSuffixRule,
			EmptyRecoRequestId,
			InvalidPromptType,
			ConfigFileInitializationError,
			CallCouldNotBeHandled,
			InvalidRecoRequestId,
			GrammarFetcherException,
			UserNotFoundException,
			InvalidTLSPort,
			InvalidAudioStreamException,
			PipelineFullWithCDRMessages,
			InvalidOperator
		}
	}
}
