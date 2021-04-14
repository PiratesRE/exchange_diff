using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.UM.UMCommon.Exceptions
{
	internal static class Strings
	{
		static Strings()
		{
			Strings.stringIDs.Add(1713941734U, "ValidateOrGeneratePINRequest");
			Strings.stringIDs.Add(2723036908U, "ExceptionInvalidSipNameResourceId");
			Strings.stringIDs.Add(50529680U, "InvalidADRecipientException");
			Strings.stringIDs.Add(437391379U, "GetCallInfoRequest");
			Strings.stringIDs.Add(456952916U, "PromptPreviewRpcRequest");
			Strings.stringIDs.Add(3129803840U, "DeleteFailed");
			Strings.stringIDs.Add(3755207777U, "InvalidCallIdException");
			Strings.stringIDs.Add(1007247767U, "ContentIndexingNotEnabled");
			Strings.stringIDs.Add(2554681784U, "ClientAccessException");
			Strings.stringIDs.Add(3560634689U, "UMRPCIncompatibleVersionException");
			Strings.stringIDs.Add(3827073469U, "InitializeUMMailboxRequest");
			Strings.stringIDs.Add(1575129900U, "UnSecured");
			Strings.stringIDs.Add(1171028813U, "InvalidPAA");
			Strings.stringIDs.Add(1404156949U, "PlayOnPhoneGreetingRequest");
			Strings.stringIDs.Add(3153985174U, "SubmitWelcomeMessageRequest");
			Strings.stringIDs.Add(1559723317U, "DisableUMMailboxRequest");
			Strings.stringIDs.Add(2262233287U, "ExceptionInvalidE164ResourceId");
			Strings.stringIDs.Add(2807255575U, "OverPlayOnPhoneCallLimitException");
			Strings.stringIDs.Add(2987232564U, "ExceptionSipResourceIdNotNeeded");
			Strings.stringIDs.Add(954056551U, "ProcessPartnerMessageRequest");
			Strings.stringIDs.Add(913692915U, "AutoAttendantPromptRequest");
			Strings.stringIDs.Add(2355518126U, "GetPINInfoRequest");
			Strings.stringIDs.Add(1929284865U, "DisconnectRequest");
			Strings.stringIDs.Add(4142400168U, "DialingRulesException");
			Strings.stringIDs.Add(1333967066U, "AutoAttendantBusinessHoursPromptRequest");
			Strings.stringIDs.Add(3615365293U, "ExceptionE164ResourceIdNeeded");
			Strings.stringIDs.Add(634056442U, "AutoAttendantBusinessLocationPromptRequest");
			Strings.stringIDs.Add(3099272000U, "PasswordDerivedBytesNeedNonNegNum");
			Strings.stringIDs.Add(1057562681U, "PlayOnPhoneAAGreetingRequest");
			Strings.stringIDs.Add(1298000829U, "UnsupportedCustomGreetingWaveFormat");
			Strings.stringIDs.Add(1479848360U, "ExceptionInvalidSipUri");
			Strings.stringIDs.Add(2382391216U, "ADAccessFailed");
			Strings.stringIDs.Add(2474709144U, "AutoAttendantCustomPromptRequest");
			Strings.stringIDs.Add(2965802745U, "AutoAttendantWelcomePromptRequest");
			Strings.stringIDs.Add(49929337U, "TamperedPin");
			Strings.stringIDs.Add(3060719176U, "SubmitPINResetMessageRequest");
			Strings.stringIDs.Add(3132314599U, "PlayOnPhoneMessageRequest");
			Strings.stringIDs.Add(2380102598U, "InvalidPrincipalException");
			Strings.stringIDs.Add(4275120716U, "RpcUMServerNotFoundException");
			Strings.stringIDs.Add(1158496135U, "SipResourceIdAndExtensionsNeeded");
			Strings.stringIDs.Add(1496370889U, "PlayOnPhonePAAGreetingRequest");
			Strings.stringIDs.Add(229602390U, "NoCallerIdToUseException");
			Strings.stringIDs.Add(2125881321U, "UndeleteFailed");
			Strings.stringIDs.Add(1651664428U, "UMDataStorageMailboxNotFound");
			Strings.stringIDs.Add(1738969940U, "InvalidObjectIdException");
			Strings.stringIDs.Add(1136924537U, "UMMailboxPromptRequest");
			Strings.stringIDs.Add(3151943388U, "EWSNoResponseReceived");
			Strings.stringIDs.Add(4186469259U, "CorruptedPAAStore");
			Strings.stringIDs.Add(2584780874U, "CallIdNull");
			Strings.stringIDs.Add(2625829937U, "ExceptionInvalidPhoneNumber");
			Strings.stringIDs.Add(1241597555U, "Secured");
			Strings.stringIDs.Add(267464401U, "IPGatewayNotFoundException");
			Strings.stringIDs.Add(1196599U, "UnsupportedCustomGreetingWmaFormat");
			Strings.stringIDs.Add(143404757U, "ExceptionCouldNotGenerateExtension");
			Strings.stringIDs.Add(396119749U, "UndeleteNotFound");
			Strings.stringIDs.Add(3947493446U, "InvalidUMAutoAttendantException");
			Strings.stringIDs.Add(2003854661U, "UMGray");
			Strings.stringIDs.Add(4078673759U, "SavePINRequest");
			Strings.stringIDs.Add(1064401785U, "ResetPINException");
			Strings.stringIDs.Add(4195133953U, "PasswordDerivedBytesTooManyBytes");
		}

		public static LocalizedString UMMbxPolicyNotFound(string policy, string user)
		{
			return new LocalizedString("UMMbxPolicyNotFound", Strings.ResourceManager, new object[]
			{
				policy,
				user
			});
		}

		public static LocalizedString ValidateOrGeneratePINRequest
		{
			get
			{
				return new LocalizedString("ValidateOrGeneratePINRequest", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ExceptionInvalidSipNameResourceId
		{
			get
			{
				return new LocalizedString("ExceptionInvalidSipNameResourceId", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InvalidTenantGuidException(Guid tenantGuid)
		{
			return new LocalizedString("InvalidTenantGuidException", Strings.ResourceManager, new object[]
			{
				tenantGuid
			});
		}

		public static LocalizedString AcmFailure(string failureMessage)
		{
			return new LocalizedString("AcmFailure", Strings.ResourceManager, new object[]
			{
				failureMessage
			});
		}

		public static LocalizedString InvalidFileNameException(int fileNameMaximumLength)
		{
			return new LocalizedString("InvalidFileNameException", Strings.ResourceManager, new object[]
			{
				fileNameMaximumLength
			});
		}

		public static LocalizedString InvalidADRecipientException
		{
			get
			{
				return new LocalizedString("InvalidADRecipientException", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ExceptionExchangeServerNotValid(string name)
		{
			return new LocalizedString("ExceptionExchangeServerNotValid", Strings.ResourceManager, new object[]
			{
				name
			});
		}

		public static LocalizedString ExceptionExchangeServerNotFound(string s)
		{
			return new LocalizedString("ExceptionExchangeServerNotFound", Strings.ResourceManager, new object[]
			{
				s
			});
		}

		public static LocalizedString TlsTlsNegotiationFailure(int i, string t)
		{
			return new LocalizedString("TlsTlsNegotiationFailure", Strings.ResourceManager, new object[]
			{
				i,
				t
			});
		}

		public static LocalizedString GetCallInfoRequest
		{
			get
			{
				return new LocalizedString("GetCallInfoRequest", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString UMRpcTransientError(string user, string server)
		{
			return new LocalizedString("UMRpcTransientError", Strings.ResourceManager, new object[]
			{
				user,
				server
			});
		}

		public static LocalizedString OpenRestrictedContentException(string reason)
		{
			return new LocalizedString("OpenRestrictedContentException", Strings.ResourceManager, new object[]
			{
				reason
			});
		}

		public static LocalizedString PromptPreviewRpcRequest
		{
			get
			{
				return new LocalizedString("PromptPreviewRpcRequest", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString DialPlanNotFoundForServer(string s)
		{
			return new LocalizedString("DialPlanNotFoundForServer", Strings.ResourceManager, new object[]
			{
				s
			});
		}

		public static LocalizedString PasswordDerivedBytesValuesFixed(string name)
		{
			return new LocalizedString("PasswordDerivedBytesValuesFixed", Strings.ResourceManager, new object[]
			{
				name
			});
		}

		public static LocalizedString DeleteFailed
		{
			get
			{
				return new LocalizedString("DeleteFailed", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString CorruptedConfigurationCouldNotBeDeleted(string userAddress)
		{
			return new LocalizedString("CorruptedConfigurationCouldNotBeDeleted", Strings.ResourceManager, new object[]
			{
				userAddress
			});
		}

		public static LocalizedString ExclusionListException(string msg)
		{
			return new LocalizedString("ExclusionListException", Strings.ResourceManager, new object[]
			{
				msg
			});
		}

		public static LocalizedString PermanentlyUnableToAccessUserConfiguration(string userAddress)
		{
			return new LocalizedString("PermanentlyUnableToAccessUserConfiguration", Strings.ResourceManager, new object[]
			{
				userAddress
			});
		}

		public static LocalizedString GrammarDirectoryNotFoundError(string s, string p1, string p2)
		{
			return new LocalizedString("GrammarDirectoryNotFoundError", Strings.ResourceManager, new object[]
			{
				s,
				p1,
				p2
			});
		}

		public static LocalizedString InvalidCallIdException
		{
			get
			{
				return new LocalizedString("InvalidCallIdException", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString FileNotFound(string path)
		{
			return new LocalizedString("FileNotFound", Strings.ResourceManager, new object[]
			{
				path
			});
		}

		public static LocalizedString UMRpcAccessDeniedError(string server)
		{
			return new LocalizedString("UMRpcAccessDeniedError", Strings.ResourceManager, new object[]
			{
				server
			});
		}

		public static LocalizedString InvalidResponseException(string channel, string error)
		{
			return new LocalizedString("InvalidResponseException", Strings.ResourceManager, new object[]
			{
				channel,
				error
			});
		}

		public static LocalizedString FsmConfigurationException(string exceptionText)
		{
			return new LocalizedString("FsmConfigurationException", Strings.ResourceManager, new object[]
			{
				exceptionText
			});
		}

		public static LocalizedString NoIPAddress(string hostName)
		{
			return new LocalizedString("NoIPAddress", Strings.ResourceManager, new object[]
			{
				hostName
			});
		}

		public static LocalizedString ContentIndexingNotEnabled
		{
			get
			{
				return new LocalizedString("ContentIndexingNotEnabled", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString UMMailboxOperationQuotaExceededError(string message)
		{
			return new LocalizedString("UMMailboxOperationQuotaExceededError", Strings.ResourceManager, new object[]
			{
				message
			});
		}

		public static LocalizedString UMMailboxNotFound(string user)
		{
			return new LocalizedString("UMMailboxNotFound", Strings.ResourceManager, new object[]
			{
				user
			});
		}

		public static LocalizedString ClientAccessException
		{
			get
			{
				return new LocalizedString("ClientAccessException", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString UnableToFindUMReportData(string mailboxOwner)
		{
			return new LocalizedString("UnableToFindUMReportData", Strings.ResourceManager, new object[]
			{
				mailboxOwner
			});
		}

		public static LocalizedString UMRPCIncompatibleVersionException
		{
			get
			{
				return new LocalizedString("UMRPCIncompatibleVersionException", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InvalidAcceptedDomainException(string organizationId)
		{
			return new LocalizedString("InvalidAcceptedDomainException", Strings.ResourceManager, new object[]
			{
				organizationId
			});
		}

		public static LocalizedString TlsCertificateExpired(int i, string t)
		{
			return new LocalizedString("TlsCertificateExpired", Strings.ResourceManager, new object[]
			{
				i,
				t
			});
		}

		public static LocalizedString ADOperationRetriesExceeded(string s)
		{
			return new LocalizedString("ADOperationRetriesExceeded", Strings.ResourceManager, new object[]
			{
				s
			});
		}

		public static LocalizedString InitializeUMMailboxRequest
		{
			get
			{
				return new LocalizedString("InitializeUMMailboxRequest", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString UnSecured
		{
			get
			{
				return new LocalizedString("UnSecured", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InvalidPAA
		{
			get
			{
				return new LocalizedString("InvalidPAA", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString PlayOnPhoneGreetingRequest
		{
			get
			{
				return new LocalizedString("PlayOnPhoneGreetingRequest", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString PersonalAutoAttendantNotFound(string identity)
		{
			return new LocalizedString("PersonalAutoAttendantNotFound", Strings.ResourceManager, new object[]
			{
				identity
			});
		}

		public static LocalizedString DuplicateReplacementStringError(string s)
		{
			return new LocalizedString("DuplicateReplacementStringError", Strings.ResourceManager, new object[]
			{
				s
			});
		}

		public static LocalizedString UnsupportedCustomGreetingSizeFormat(string minutes)
		{
			return new LocalizedString("UnsupportedCustomGreetingSizeFormat", Strings.ResourceManager, new object[]
			{
				minutes
			});
		}

		public static LocalizedString SubmitWelcomeMessageRequest
		{
			get
			{
				return new LocalizedString("SubmitWelcomeMessageRequest", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString DisableUMMailboxRequest
		{
			get
			{
				return new LocalizedString("DisableUMMailboxRequest", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ADFatalError(string s)
		{
			return new LocalizedString("ADFatalError", Strings.ResourceManager, new object[]
			{
				s
			});
		}

		public static LocalizedString ExceptionInvalidE164ResourceId
		{
			get
			{
				return new LocalizedString("ExceptionInvalidE164ResourceId", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString CorruptedPasswordField(string user)
		{
			return new LocalizedString("CorruptedPasswordField", Strings.ResourceManager, new object[]
			{
				user
			});
		}

		public static LocalizedString UserConfigurationException(string exceptionText)
		{
			return new LocalizedString("UserConfigurationException", Strings.ResourceManager, new object[]
			{
				exceptionText
			});
		}

		public static LocalizedString OverPlayOnPhoneCallLimitException
		{
			get
			{
				return new LocalizedString("OverPlayOnPhoneCallLimitException", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MowaGrammarException(string exceptionText)
		{
			return new LocalizedString("MowaGrammarException", Strings.ResourceManager, new object[]
			{
				exceptionText
			});
		}

		public static LocalizedString ADOperationFailure(string s)
		{
			return new LocalizedString("ADOperationFailure", Strings.ResourceManager, new object[]
			{
				s
			});
		}

		public static LocalizedString UMMailboxOperationTransientError(string message)
		{
			return new LocalizedString("UMMailboxOperationTransientError", Strings.ResourceManager, new object[]
			{
				message
			});
		}

		public static LocalizedString AudioDataIsOversizeException(int maxAudioDataMegabytes, long maxGreetingSizeMinutes)
		{
			return new LocalizedString("AudioDataIsOversizeException", Strings.ResourceManager, new object[]
			{
				maxAudioDataMegabytes,
				maxGreetingSizeMinutes
			});
		}

		public static LocalizedString PromptSynthesisException(string info)
		{
			return new LocalizedString("PromptSynthesisException", Strings.ResourceManager, new object[]
			{
				info
			});
		}

		public static LocalizedString ExceptionSipResourceIdNotNeeded
		{
			get
			{
				return new LocalizedString("ExceptionSipResourceIdNotNeeded", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString CDROperationTransientError(string message)
		{
			return new LocalizedString("CDROperationTransientError", Strings.ResourceManager, new object[]
			{
				message
			});
		}

		public static LocalizedString TransportException(string msg)
		{
			return new LocalizedString("TransportException", Strings.ResourceManager, new object[]
			{
				msg
			});
		}

		public static LocalizedString ProcessPartnerMessageRequest
		{
			get
			{
				return new LocalizedString("ProcessPartnerMessageRequest", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString AutoAttendantPromptRequest
		{
			get
			{
				return new LocalizedString("AutoAttendantPromptRequest", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString GetPINInfoRequest
		{
			get
			{
				return new LocalizedString("GetPINInfoRequest", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString XMLError(int line, int position, string message)
		{
			return new LocalizedString("XMLError", Strings.ResourceManager, new object[]
			{
				line,
				position,
				message
			});
		}

		public static LocalizedString DisconnectRequest
		{
			get
			{
				return new LocalizedString("DisconnectRequest", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString UnsupportedCustomGreetingLegacyFormat(string fileName)
		{
			return new LocalizedString("UnsupportedCustomGreetingLegacyFormat", Strings.ResourceManager, new object[]
			{
				fileName
			});
		}

		public static LocalizedString TransientlyUnableToAccessUserConfiguration(string userAddress)
		{
			return new LocalizedString("TransientlyUnableToAccessUserConfiguration", Strings.ResourceManager, new object[]
			{
				userAddress
			});
		}

		public static LocalizedString DialingRulesException
		{
			get
			{
				return new LocalizedString("DialingRulesException", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorMaxPAACountReached(int count)
		{
			return new LocalizedString("ErrorMaxPAACountReached", Strings.ResourceManager, new object[]
			{
				count
			});
		}

		public static LocalizedString AutoAttendantBusinessHoursPromptRequest
		{
			get
			{
				return new LocalizedString("AutoAttendantBusinessHoursPromptRequest", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ExceptionE164ResourceIdNeeded
		{
			get
			{
				return new LocalizedString("ExceptionE164ResourceIdNeeded", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString TlsRemoteCertificateRevoked(int i, string t)
		{
			return new LocalizedString("TlsRemoteCertificateRevoked", Strings.ResourceManager, new object[]
			{
				i,
				t
			});
		}

		public static LocalizedString EWSUrlDiscoveryFailed(string user)
		{
			return new LocalizedString("EWSUrlDiscoveryFailed", Strings.ResourceManager, new object[]
			{
				user
			});
		}

		public static LocalizedString AutoAttendantBusinessLocationPromptRequest
		{
			get
			{
				return new LocalizedString("AutoAttendantBusinessLocationPromptRequest", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorPerformingCDROperation(string moreInfo)
		{
			return new LocalizedString("ErrorPerformingCDROperation", Strings.ResourceManager, new object[]
			{
				moreInfo
			});
		}

		public static LocalizedString UMRecipientValidation(string recipient, string fieldName)
		{
			return new LocalizedString("UMRecipientValidation", Strings.ResourceManager, new object[]
			{
				recipient,
				fieldName
			});
		}

		public static LocalizedString PasswordDerivedBytesNeedNonNegNum
		{
			get
			{
				return new LocalizedString("PasswordDerivedBytesNeedNonNegNum", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ExceptionUserNotUmEnabled(string user)
		{
			return new LocalizedString("ExceptionUserNotUmEnabled", Strings.ResourceManager, new object[]
			{
				user
			});
		}

		public static LocalizedString TlsUntrustedRemoteCertificate(int i, string t)
		{
			return new LocalizedString("TlsUntrustedRemoteCertificate", Strings.ResourceManager, new object[]
			{
				i,
				t
			});
		}

		public static LocalizedString PlayOnPhoneAAGreetingRequest
		{
			get
			{
				return new LocalizedString("PlayOnPhoneAAGreetingRequest", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString UnsupportedCustomGreetingWaveFormat
		{
			get
			{
				return new LocalizedString("UnsupportedCustomGreetingWaveFormat", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString DialPlanNotFound(string s)
		{
			return new LocalizedString("DialPlanNotFound", Strings.ResourceManager, new object[]
			{
				s
			});
		}

		public static LocalizedString ExceptionInvalidSipUri
		{
			get
			{
				return new LocalizedString("ExceptionInvalidSipUri", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString UmAuthorizationException(string user, string activity)
		{
			return new LocalizedString("UmAuthorizationException", Strings.ResourceManager, new object[]
			{
				user,
				activity
			});
		}

		public static LocalizedString ADRetry(int i)
		{
			return new LocalizedString("ADRetry", Strings.ResourceManager, new object[]
			{
				i
			});
		}

		public static LocalizedString PublishingException(LocalizedString msg)
		{
			return new LocalizedString("PublishingException", Strings.ResourceManager, new object[]
			{
				msg
			});
		}

		public static LocalizedString ADAccessFailed
		{
			get
			{
				return new LocalizedString("ADAccessFailed", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString SerializationError(string mailboxOwner)
		{
			return new LocalizedString("SerializationError", Strings.ResourceManager, new object[]
			{
				mailboxOwner
			});
		}

		public static LocalizedString AutoAttendantCustomPromptRequest
		{
			get
			{
				return new LocalizedString("AutoAttendantCustomPromptRequest", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString UMRPCIncompatibleVersionError(string server)
		{
			return new LocalizedString("UMRPCIncompatibleVersionError", Strings.ResourceManager, new object[]
			{
				server
			});
		}

		public static LocalizedString TlsRemoteCertificateInvalidUsage(int i, string t)
		{
			return new LocalizedString("TlsRemoteCertificateInvalidUsage", Strings.ResourceManager, new object[]
			{
				i,
				t
			});
		}

		public static LocalizedString AutoAttendantWelcomePromptRequest
		{
			get
			{
				return new LocalizedString("AutoAttendantWelcomePromptRequest", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString TlsIncorrectNameInRemoteCertificate(int i, string t)
		{
			return new LocalizedString("TlsIncorrectNameInRemoteCertificate", Strings.ResourceManager, new object[]
			{
				i,
				t
			});
		}

		public static LocalizedString CDROperationQuotaExceededError(string message)
		{
			return new LocalizedString("CDROperationQuotaExceededError", Strings.ResourceManager, new object[]
			{
				message
			});
		}

		public static LocalizedString DeleteContentException(string moreInfo)
		{
			return new LocalizedString("DeleteContentException", Strings.ResourceManager, new object[]
			{
				moreInfo
			});
		}

		public static LocalizedString TamperedPin
		{
			get
			{
				return new LocalizedString("TamperedPin", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString SubmitPINResetMessageRequest
		{
			get
			{
				return new LocalizedString("SubmitPINResetMessageRequest", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString PromptDirectoryNotFoundError(string s, string p1, string p2)
		{
			return new LocalizedString("PromptDirectoryNotFoundError", Strings.ResourceManager, new object[]
			{
				s,
				p1,
				p2
			});
		}

		public static LocalizedString CorruptedGreetingCouldNotBeDeleted(string userAddress)
		{
			return new LocalizedString("CorruptedGreetingCouldNotBeDeleted", Strings.ResourceManager, new object[]
			{
				userAddress
			});
		}

		public static LocalizedString UMRpcError(string targetName, int responseCode, string responseText)
		{
			return new LocalizedString("UMRpcError", Strings.ResourceManager, new object[]
			{
				targetName,
				responseCode,
				responseText
			});
		}

		public static LocalizedString PlayOnPhoneMessageRequest
		{
			get
			{
				return new LocalizedString("PlayOnPhoneMessageRequest", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InvalidPrincipalException
		{
			get
			{
				return new LocalizedString("InvalidPrincipalException", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString descMwiMessageExpiredError(string userName)
		{
			return new LocalizedString("descMwiMessageExpiredError", Strings.ResourceManager, new object[]
			{
				userName
			});
		}

		public static LocalizedString MissingUserAttribute(string attribute, string extension)
		{
			return new LocalizedString("MissingUserAttribute", Strings.ResourceManager, new object[]
			{
				attribute,
				extension
			});
		}

		public static LocalizedString RpcUMServerNotFoundException
		{
			get
			{
				return new LocalizedString("RpcUMServerNotFoundException", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString SipResourceIdAndExtensionsNeeded
		{
			get
			{
				return new LocalizedString("SipResourceIdAndExtensionsNeeded", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString PlayOnPhonePAAGreetingRequest
		{
			get
			{
				return new LocalizedString("PlayOnPhonePAAGreetingRequest", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString EWSOperationFailed(string response, string details)
		{
			return new LocalizedString("EWSOperationFailed", Strings.ResourceManager, new object[]
			{
				response,
				details
			});
		}

		public static LocalizedString NoCallerIdToUseException
		{
			get
			{
				return new LocalizedString("NoCallerIdToUseException", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString DestinationAlreadyExists(string path)
		{
			return new LocalizedString("DestinationAlreadyExists", Strings.ResourceManager, new object[]
			{
				path
			});
		}

		public static LocalizedString InvalidUMProxyAddressException(string proxyAddress)
		{
			return new LocalizedString("InvalidUMProxyAddressException", Strings.ResourceManager, new object[]
			{
				proxyAddress
			});
		}

		public static LocalizedString UMServerNotFoundDialPlanException(string dialPlan)
		{
			return new LocalizedString("UMServerNotFoundDialPlanException", Strings.ResourceManager, new object[]
			{
				dialPlan
			});
		}

		public static LocalizedString UndeleteFailed
		{
			get
			{
				return new LocalizedString("UndeleteFailed", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString TlsOther(int i, string t)
		{
			return new LocalizedString("TlsOther", Strings.ResourceManager, new object[]
			{
				i,
				t
			});
		}

		public static LocalizedString UMDataStorageMailboxNotFound
		{
			get
			{
				return new LocalizedString("UMDataStorageMailboxNotFound", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InvalidObjectIdException
		{
			get
			{
				return new LocalizedString("InvalidObjectIdException", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString EWSUMMailboxAccessException(string reason)
		{
			return new LocalizedString("EWSUMMailboxAccessException", Strings.ResourceManager, new object[]
			{
				reason
			});
		}

		public static LocalizedString UMMailboxPromptRequest
		{
			get
			{
				return new LocalizedString("UMMailboxPromptRequest", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString EWSNoResponseReceived
		{
			get
			{
				return new LocalizedString("EWSNoResponseReceived", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString WmaToWavConversion(string wma, string wav)
		{
			return new LocalizedString("WmaToWavConversion", Strings.ResourceManager, new object[]
			{
				wma,
				wav
			});
		}

		public static LocalizedString TlsRemoteDisconnected(int i, string t)
		{
			return new LocalizedString("TlsRemoteDisconnected", Strings.ResourceManager, new object[]
			{
				i,
				t
			});
		}

		public static LocalizedString UmUserException(string exceptionText)
		{
			return new LocalizedString("UmUserException", Strings.ResourceManager, new object[]
			{
				exceptionText
			});
		}

		public static LocalizedString CorruptedPAAStore
		{
			get
			{
				return new LocalizedString("CorruptedPAAStore", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString CDROperationObjectNotFound(string message)
		{
			return new LocalizedString("CDROperationObjectNotFound", Strings.ResourceManager, new object[]
			{
				message
			});
		}

		public static LocalizedString UMRpcGenericError(string user, int hResult, string server)
		{
			return new LocalizedString("UMRpcGenericError", Strings.ResourceManager, new object[]
			{
				user,
				hResult,
				server
			});
		}

		public static LocalizedString MoreThanOneSearchFolder(int searchFolderCount, string searchFolderName)
		{
			return new LocalizedString("MoreThanOneSearchFolder", Strings.ResourceManager, new object[]
			{
				searchFolderCount,
				searchFolderName
			});
		}

		public static LocalizedString CallIdNull
		{
			get
			{
				return new LocalizedString("CallIdNull", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ExceptionInvalidPhoneNumber
		{
			get
			{
				return new LocalizedString("ExceptionInvalidPhoneNumber", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InvalidArgumentException(string s)
		{
			return new LocalizedString("InvalidArgumentException", Strings.ResourceManager, new object[]
			{
				s
			});
		}

		public static LocalizedString UMMailboxOperationSendEmailError(string message)
		{
			return new LocalizedString("UMMailboxOperationSendEmailError", Strings.ResourceManager, new object[]
			{
				message
			});
		}

		public static LocalizedString ErrorPerformingUMMailboxOperation(string moreInfo)
		{
			return new LocalizedString("ErrorPerformingUMMailboxOperation", Strings.ResourceManager, new object[]
			{
				moreInfo
			});
		}

		public static LocalizedString descMwiNoTargetsAvailableError(string userName)
		{
			return new LocalizedString("descMwiNoTargetsAvailableError", Strings.ResourceManager, new object[]
			{
				userName
			});
		}

		public static LocalizedString SourceFileNotFound(string path)
		{
			return new LocalizedString("SourceFileNotFound", Strings.ResourceManager, new object[]
			{
				path
			});
		}

		public static LocalizedString CallIdNotNull(string currentlySetCallId, string newCallId)
		{
			return new LocalizedString("CallIdNotNull", Strings.ResourceManager, new object[]
			{
				currentlySetCallId,
				newCallId
			});
		}

		public static LocalizedString Secured
		{
			get
			{
				return new LocalizedString("Secured", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InsufficientSendQuotaForUMEnablement(string user)
		{
			return new LocalizedString("InsufficientSendQuotaForUMEnablement", Strings.ResourceManager, new object[]
			{
				user
			});
		}

		public static LocalizedString IPGatewayNotFoundException
		{
			get
			{
				return new LocalizedString("IPGatewayNotFoundException", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString UnsupportedCustomGreetingWmaFormat
		{
			get
			{
				return new LocalizedString("UnsupportedCustomGreetingWmaFormat", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString UMInvalidPartnerMessageException(string fieldName)
		{
			return new LocalizedString("UMInvalidPartnerMessageException", Strings.ResourceManager, new object[]
			{
				fieldName
			});
		}

		public static LocalizedString InvalidRequestException(string server)
		{
			return new LocalizedString("InvalidRequestException", Strings.ResourceManager, new object[]
			{
				server
			});
		}

		public static LocalizedString TlsLocalCertificateNotFound(int i, string t)
		{
			return new LocalizedString("TlsLocalCertificateNotFound", Strings.ResourceManager, new object[]
			{
				i,
				t
			});
		}

		public static LocalizedString CorruptedPIN(string userAddress)
		{
			return new LocalizedString("CorruptedPIN", Strings.ResourceManager, new object[]
			{
				userAddress
			});
		}

		public static LocalizedString descTooManyOutstandingMwiRequestsError(string userName)
		{
			return new LocalizedString("descTooManyOutstandingMwiRequestsError", Strings.ResourceManager, new object[]
			{
				userName
			});
		}

		public static LocalizedString MultupleUMDataStorageMailboxFound(string id1, string id2)
		{
			return new LocalizedString("MultupleUMDataStorageMailboxFound", Strings.ResourceManager, new object[]
			{
				id1,
				id2
			});
		}

		public static LocalizedString ExceptionCouldNotGenerateExtension
		{
			get
			{
				return new LocalizedString("ExceptionCouldNotGenerateExtension", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString UndeleteNotFound
		{
			get
			{
				return new LocalizedString("UndeleteNotFound", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString UnableToRemoveCustomGreeting(string userAddress)
		{
			return new LocalizedString("UnableToRemoveCustomGreeting", Strings.ResourceManager, new object[]
			{
				userAddress
			});
		}

		public static LocalizedString InvalidUMAutoAttendantException
		{
			get
			{
				return new LocalizedString("InvalidUMAutoAttendantException", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString UMGray
		{
			get
			{
				return new LocalizedString("UMGray", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString SavePINRequest
		{
			get
			{
				return new LocalizedString("SavePINRequest", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString NoIPv4Address(string hostName)
		{
			return new LocalizedString("NoIPv4Address", Strings.ResourceManager, new object[]
			{
				hostName
			});
		}

		public static LocalizedString DuplicateClassNameError(string s)
		{
			return new LocalizedString("DuplicateClassNameError", Strings.ResourceManager, new object[]
			{
				s
			});
		}

		public static LocalizedString ResetPINException
		{
			get
			{
				return new LocalizedString("ResetPINException", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString LegacyUmUser(string legacyDN)
		{
			return new LocalizedString("LegacyUmUser", Strings.ResourceManager, new object[]
			{
				legacyDN
			});
		}

		public static LocalizedString PasswordDerivedBytesTooManyBytes
		{
			get
			{
				return new LocalizedString("PasswordDerivedBytesTooManyBytes", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString WavToWmaConversion(string wav, string wma)
		{
			return new LocalizedString("WavToWmaConversion", Strings.ResourceManager, new object[]
			{
				wav,
				wma
			});
		}

		public static LocalizedString ExceptionNoMailboxForUser(string user)
		{
			return new LocalizedString("ExceptionNoMailboxForUser", Strings.ResourceManager, new object[]
			{
				user
			});
		}

		public static LocalizedString ErrorAccessingPublishingPoint(string moreInfo)
		{
			return new LocalizedString("ErrorAccessingPublishingPoint", Strings.ResourceManager, new object[]
			{
				moreInfo
			});
		}

		public static LocalizedString OutboundCallFailure(string s)
		{
			return new LocalizedString("OutboundCallFailure", Strings.ResourceManager, new object[]
			{
				s
			});
		}

		public static LocalizedString ADUMUserInvalidUMMailboxPolicyException(string useraddress)
		{
			return new LocalizedString("ADUMUserInvalidUMMailboxPolicyException", Strings.ResourceManager, new object[]
			{
				useraddress
			});
		}

		public static LocalizedString GetLocalizedString(Strings.IDs key)
		{
			return new LocalizedString(Strings.stringIDs[(uint)key], Strings.ResourceManager, new object[0]);
		}

		private static Dictionary<uint, string> stringIDs = new Dictionary<uint, string>(60);

		private static ExchangeResourceManager ResourceManager = ExchangeResourceManager.GetResourceManager("Microsoft.Exchange.UM.UMCommon.Exceptions.Strings", typeof(Strings).GetTypeInfo().Assembly);

		public enum IDs : uint
		{
			ValidateOrGeneratePINRequest = 1713941734U,
			ExceptionInvalidSipNameResourceId = 2723036908U,
			InvalidADRecipientException = 50529680U,
			GetCallInfoRequest = 437391379U,
			PromptPreviewRpcRequest = 456952916U,
			DeleteFailed = 3129803840U,
			InvalidCallIdException = 3755207777U,
			ContentIndexingNotEnabled = 1007247767U,
			ClientAccessException = 2554681784U,
			UMRPCIncompatibleVersionException = 3560634689U,
			InitializeUMMailboxRequest = 3827073469U,
			UnSecured = 1575129900U,
			InvalidPAA = 1171028813U,
			PlayOnPhoneGreetingRequest = 1404156949U,
			SubmitWelcomeMessageRequest = 3153985174U,
			DisableUMMailboxRequest = 1559723317U,
			ExceptionInvalidE164ResourceId = 2262233287U,
			OverPlayOnPhoneCallLimitException = 2807255575U,
			ExceptionSipResourceIdNotNeeded = 2987232564U,
			ProcessPartnerMessageRequest = 954056551U,
			AutoAttendantPromptRequest = 913692915U,
			GetPINInfoRequest = 2355518126U,
			DisconnectRequest = 1929284865U,
			DialingRulesException = 4142400168U,
			AutoAttendantBusinessHoursPromptRequest = 1333967066U,
			ExceptionE164ResourceIdNeeded = 3615365293U,
			AutoAttendantBusinessLocationPromptRequest = 634056442U,
			PasswordDerivedBytesNeedNonNegNum = 3099272000U,
			PlayOnPhoneAAGreetingRequest = 1057562681U,
			UnsupportedCustomGreetingWaveFormat = 1298000829U,
			ExceptionInvalidSipUri = 1479848360U,
			ADAccessFailed = 2382391216U,
			AutoAttendantCustomPromptRequest = 2474709144U,
			AutoAttendantWelcomePromptRequest = 2965802745U,
			TamperedPin = 49929337U,
			SubmitPINResetMessageRequest = 3060719176U,
			PlayOnPhoneMessageRequest = 3132314599U,
			InvalidPrincipalException = 2380102598U,
			RpcUMServerNotFoundException = 4275120716U,
			SipResourceIdAndExtensionsNeeded = 1158496135U,
			PlayOnPhonePAAGreetingRequest = 1496370889U,
			NoCallerIdToUseException = 229602390U,
			UndeleteFailed = 2125881321U,
			UMDataStorageMailboxNotFound = 1651664428U,
			InvalidObjectIdException = 1738969940U,
			UMMailboxPromptRequest = 1136924537U,
			EWSNoResponseReceived = 3151943388U,
			CorruptedPAAStore = 4186469259U,
			CallIdNull = 2584780874U,
			ExceptionInvalidPhoneNumber = 2625829937U,
			Secured = 1241597555U,
			IPGatewayNotFoundException = 267464401U,
			UnsupportedCustomGreetingWmaFormat = 1196599U,
			ExceptionCouldNotGenerateExtension = 143404757U,
			UndeleteNotFound = 396119749U,
			InvalidUMAutoAttendantException = 3947493446U,
			UMGray = 2003854661U,
			SavePINRequest = 4078673759U,
			ResetPINException = 1064401785U,
			PasswordDerivedBytesTooManyBytes = 4195133953U
		}

		private enum ParamIDs
		{
			UMMbxPolicyNotFound,
			InvalidTenantGuidException,
			AcmFailure,
			InvalidFileNameException,
			ExceptionExchangeServerNotValid,
			ExceptionExchangeServerNotFound,
			TlsTlsNegotiationFailure,
			UMRpcTransientError,
			OpenRestrictedContentException,
			DialPlanNotFoundForServer,
			PasswordDerivedBytesValuesFixed,
			CorruptedConfigurationCouldNotBeDeleted,
			ExclusionListException,
			PermanentlyUnableToAccessUserConfiguration,
			GrammarDirectoryNotFoundError,
			FileNotFound,
			UMRpcAccessDeniedError,
			InvalidResponseException,
			FsmConfigurationException,
			NoIPAddress,
			UMMailboxOperationQuotaExceededError,
			UMMailboxNotFound,
			UnableToFindUMReportData,
			InvalidAcceptedDomainException,
			TlsCertificateExpired,
			ADOperationRetriesExceeded,
			PersonalAutoAttendantNotFound,
			DuplicateReplacementStringError,
			UnsupportedCustomGreetingSizeFormat,
			ADFatalError,
			CorruptedPasswordField,
			UserConfigurationException,
			MowaGrammarException,
			ADOperationFailure,
			UMMailboxOperationTransientError,
			AudioDataIsOversizeException,
			PromptSynthesisException,
			CDROperationTransientError,
			TransportException,
			XMLError,
			UnsupportedCustomGreetingLegacyFormat,
			TransientlyUnableToAccessUserConfiguration,
			ErrorMaxPAACountReached,
			TlsRemoteCertificateRevoked,
			EWSUrlDiscoveryFailed,
			ErrorPerformingCDROperation,
			UMRecipientValidation,
			ExceptionUserNotUmEnabled,
			TlsUntrustedRemoteCertificate,
			DialPlanNotFound,
			UmAuthorizationException,
			ADRetry,
			PublishingException,
			SerializationError,
			UMRPCIncompatibleVersionError,
			TlsRemoteCertificateInvalidUsage,
			TlsIncorrectNameInRemoteCertificate,
			CDROperationQuotaExceededError,
			DeleteContentException,
			PromptDirectoryNotFoundError,
			CorruptedGreetingCouldNotBeDeleted,
			UMRpcError,
			descMwiMessageExpiredError,
			MissingUserAttribute,
			EWSOperationFailed,
			DestinationAlreadyExists,
			InvalidUMProxyAddressException,
			UMServerNotFoundDialPlanException,
			TlsOther,
			EWSUMMailboxAccessException,
			WmaToWavConversion,
			TlsRemoteDisconnected,
			UmUserException,
			CDROperationObjectNotFound,
			UMRpcGenericError,
			MoreThanOneSearchFolder,
			InvalidArgumentException,
			UMMailboxOperationSendEmailError,
			ErrorPerformingUMMailboxOperation,
			descMwiNoTargetsAvailableError,
			SourceFileNotFound,
			CallIdNotNull,
			InsufficientSendQuotaForUMEnablement,
			UMInvalidPartnerMessageException,
			InvalidRequestException,
			TlsLocalCertificateNotFound,
			CorruptedPIN,
			descTooManyOutstandingMwiRequestsError,
			MultupleUMDataStorageMailboxFound,
			UnableToRemoveCustomGreeting,
			NoIPv4Address,
			DuplicateClassNameError,
			LegacyUmUser,
			WavToWmaConversion,
			ExceptionNoMailboxForUser,
			ErrorAccessingPublishingPoint,
			OutboundCallFailure,
			ADUMUserInvalidUMMailboxPolicyException
		}
	}
}
