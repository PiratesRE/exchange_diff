using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.Tasks.UM
{
	internal static class Strings
	{
		static Strings()
		{
			Strings.stringIDs.Add(781067276U, "ConfirmationMessageStartUMPhoneSession");
			Strings.stringIDs.Add(3239495249U, "InstallUMWebServiceTask");
			Strings.stringIDs.Add(3779087329U, "InvalidMethodToDisableAA");
			Strings.stringIDs.Add(1689586553U, "EmptyCountryOrRegionCode");
			Strings.stringIDs.Add(1601426056U, "DefaultMailboxSettings");
			Strings.stringIDs.Add(530191348U, "ConfirmationMessageStopUMPhoneSession");
			Strings.stringIDs.Add(3015522797U, "InstallUmCallRouterTask");
			Strings.stringIDs.Add(1999730358U, "CannotCreateHuntGroupForHostedSipDialPlan");
			Strings.stringIDs.Add(3303439652U, "PasswordMailBody");
			Strings.stringIDs.Add(987112185U, "ExternalHostFqdnChanges");
			Strings.stringIDs.Add(3433711682U, "RemoteEndDisconnected");
			Strings.stringIDs.Add(887877668U, "SuccessfulLogonState");
			Strings.stringIDs.Add(1970930248U, "ValidCertRequiredForUM");
			Strings.stringIDs.Add(454845832U, "AttemptingToCreateHuntgroup");
			Strings.stringIDs.Add(2238046577U, "OperationSuccessful");
			Strings.stringIDs.Add(3572721584U, "CallRouterTransferFromTLStoTCPModeWarning");
			Strings.stringIDs.Add(505394326U, "UmServiceNotInstalled");
			Strings.stringIDs.Add(3094237923U, "UMStartupModeChanges");
			Strings.stringIDs.Add(2019001798U, "PromptProvisioningShareDescription");
			Strings.stringIDs.Add(3372841158U, "UmCallRouterName");
			Strings.stringIDs.Add(1223643872U, "UserProblem");
			Strings.stringIDs.Add(648648092U, "ConfigureGatewayToForwardCallsMsg");
			Strings.stringIDs.Add(939630945U, "GatewayAddressRequiresFqdn");
			Strings.stringIDs.Add(471142364U, "DNSEntryNotFound");
			Strings.stringIDs.Add(3757387627U, "ExceptionInvalidSipNameDomain");
			Strings.stringIDs.Add(4150015982U, "UninstallUmCallRouterTask");
			Strings.stringIDs.Add(1600651549U, "LogonError");
			Strings.stringIDs.Add(388296289U, "ConfirmationMessageDisableUMServerImmediately");
			Strings.stringIDs.Add(818779485U, "GatewayIPAddressFamilyInconsistentException");
			Strings.stringIDs.Add(2840915457U, "ConfirmationMessageDisableUMServer");
			Strings.stringIDs.Add(1625624441U, "NoMailboxServersFound");
			Strings.stringIDs.Add(1298209242U, "SrtpWithoutTls");
			Strings.stringIDs.Add(3114226637U, "ConfirmationMessageDisableUMIPGateway");
			Strings.stringIDs.Add(2388018184U, "UninstallUmServiceTask");
			Strings.stringIDs.Add(2187708795U, "UmServiceDescription");
			Strings.stringIDs.Add(3944137335U, "DefaultMailboxRequiredWhenForwardTrue");
			Strings.stringIDs.Add(4164812292U, "ConfirmationMessageTestUMConnectivityLocalLoop");
			Strings.stringIDs.Add(587456431U, "InvalidDefaultOutboundCallingLineId");
			Strings.stringIDs.Add(193099536U, "ErrorGeneratingDefaultPassword");
			Strings.stringIDs.Add(3703544840U, "InvalidDTMFSequenceReceived");
			Strings.stringIDs.Add(2970756378U, "UninstallUMWebServiceTask");
			Strings.stringIDs.Add(1263770381U, "ADError");
			Strings.stringIDs.Add(48881282U, "NotMailboxServer");
			Strings.stringIDs.Add(2410305798U, "LanguagesNotPassed");
			Strings.stringIDs.Add(1691564973U, "InstallUmServiceTask");
			Strings.stringIDs.Add(2718089772U, "WaitForFirstDiagnosticResponse");
			Strings.stringIDs.Add(940435338U, "InvalidTimeZoneParameters");
			Strings.stringIDs.Add(1355103499U, "CertNotFound");
			Strings.stringIDs.Add(2010047074U, "PilotNumberState");
			Strings.stringIDs.Add(1658738722U, "KeepProperties");
			Strings.stringIDs.Add(2919065030U, "WaitForDiagnosticResponse");
			Strings.stringIDs.Add(2675709228U, "UCMAPreReqException");
			Strings.stringIDs.Add(2660011992U, "DialPlanAssociatedWithPoliciesException");
			Strings.stringIDs.Add(1878527290U, "PinExpired");
			Strings.stringIDs.Add(1151155524U, "LockedOut");
			Strings.stringIDs.Add(100754582U, "GatewayFqdnNotInAcceptedDomain");
			Strings.stringIDs.Add(918425027U, "NoDTMFSwereReceived");
			Strings.stringIDs.Add(1885509224U, "PasswordMailSubject");
			Strings.stringIDs.Add(2380514321U, "InvalidIPAddressReceived");
			Strings.stringIDs.Add(943497584U, "InvalidALParameterException");
			Strings.stringIDs.Add(3527640475U, "MustSpecifyThumbprint");
			Strings.stringIDs.Add(3746187495U, "InvalidMailboxServerVersionForTUMCTask");
			Strings.stringIDs.Add(886805380U, "CannotCreateGatewayForHostedSipDialPlan");
			Strings.stringIDs.Add(2203312157U, "ConfirmationMessageDisableUMIPGatewayImmediately");
			Strings.stringIDs.Add(1602032641U, "PilotNumber");
			Strings.stringIDs.Add(1762496243U, "AttemptingToCreateIPGateway");
			Strings.stringIDs.Add(505861453U, "ExceptionUserNotAllowedForUMEnabled");
			Strings.stringIDs.Add(305424905U, "ExchangePrincipalError");
			Strings.stringIDs.Add(2568008289U, "InvalidExternalHostFqdn");
			Strings.stringIDs.Add(912893922U, "UCMAPreReqUpgradeException");
			Strings.stringIDs.Add(4252462372U, "AADisableConfirmationString");
			Strings.stringIDs.Add(3860731788U, "AAAlreadyDisabled");
			Strings.stringIDs.Add(3372908061U, "ConfirmationMessageTestUMConnectivityPinReset");
			Strings.stringIDs.Add(1942717475U, "CertWithoutTls");
			Strings.stringIDs.Add(1061463472U, "SendEmail");
			Strings.stringIDs.Add(3845881662U, "ExceptionSipResourceIdNotUnique");
			Strings.stringIDs.Add(55026498U, "PortChanges");
			Strings.stringIDs.Add(577419765U, "AANameTooLong");
			Strings.stringIDs.Add(1006009848U, "DefaultUMHuntGroupName");
			Strings.stringIDs.Add(1694996750U, "CouldnotRetreivePasswd");
			Strings.stringIDs.Add(1731989106U, "PINEnterState");
			Strings.stringIDs.Add(3858123826U, "BusinessHoursSettings");
			Strings.stringIDs.Add(2511055751U, "UmServiceStillInstalled");
			Strings.stringIDs.Add(1003963056U, "ConfirmationMessageSetUmCallRouterSettings");
			Strings.stringIDs.Add(160860353U, "ValidCertRequiredForUMCallRouter");
			Strings.stringIDs.Add(3650433099U, "DialPlanAssociatedWithUserException");
			Strings.stringIDs.Add(73835935U, "TransferFromTLStoTCPModeWarning");
			Strings.stringIDs.Add(4163764725U, "InvalidALParameter");
			Strings.stringIDs.Add(154856458U, "AfterHoursSettings");
			Strings.stringIDs.Add(3666462471U, "UmCallRouterDescription");
			Strings.stringIDs.Add(3969155231U, "InvalidAutoAttendantScopeSetting");
			Strings.stringIDs.Add(3358569313U, "TcpAndTlsPortsCannotBeSame");
			Strings.stringIDs.Add(3177175916U, "ConfirmationMessageTestUMConnectivityTUILocalLoop");
			Strings.stringIDs.Add(2342320894U, "CurrentTimeZoneIdNotFound");
			Strings.stringIDs.Add(975840932U, "AttemptingToStampFQDN");
			Strings.stringIDs.Add(339800695U, "Pin");
			Strings.stringIDs.Add(4175977607U, "NotifyEmail");
			Strings.stringIDs.Add(1074457952U, "UmServiceName");
			Strings.stringIDs.Add(2152868767U, "AAAlreadyEnabled");
			Strings.stringIDs.Add(1304023191U, "TransferFromTCPtoTLSModeWarning");
		}

		public static LocalizedString DisabledLinkedAutoAttendant(string autoAttendant, string linkedAutoAttendant)
		{
			return new LocalizedString("DisabledLinkedAutoAttendant", "", false, false, Strings.ResourceManager, new object[]
			{
				autoAttendant,
				linkedAutoAttendant
			});
		}

		public static LocalizedString MultipleAutoAttendantsWithSameId(string s)
		{
			return new LocalizedString("MultipleAutoAttendantsWithSameId", "Ex13CFAB", false, true, Strings.ResourceManager, new object[]
			{
				s
			});
		}

		public static LocalizedString ConfirmationMessageStartUMPhoneSession
		{
			get
			{
				return new LocalizedString("ConfirmationMessageStartUMPhoneSession", "ExEAF89A", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString UMServiceDisabledException(string serviceName, string serverName)
		{
			return new LocalizedString("UMServiceDisabledException", "", false, false, Strings.ResourceManager, new object[]
			{
				serviceName,
				serverName
			});
		}

		public static LocalizedString InstallUMWebServiceTask
		{
			get
			{
				return new LocalizedString("InstallUMWebServiceTask", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InvalidMethodToDisableAA
		{
			get
			{
				return new LocalizedString("InvalidMethodToDisableAA", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString EmptyCountryOrRegionCode
		{
			get
			{
				return new LocalizedString("EmptyCountryOrRegionCode", "Ex3AF75C", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString SourceFileOpenException(string fileName)
		{
			return new LocalizedString("SourceFileOpenException", "", false, false, Strings.ResourceManager, new object[]
			{
				fileName
			});
		}

		public static LocalizedString InvalidDtmfFallbackAutoAttendant(string s)
		{
			return new LocalizedString("InvalidDtmfFallbackAutoAttendant", "", false, false, Strings.ResourceManager, new object[]
			{
				s
			});
		}

		public static LocalizedString MultipleDialplansWithSameId(object s)
		{
			return new LocalizedString("MultipleDialplansWithSameId", "Ex33C279", false, true, Strings.ResourceManager, new object[]
			{
				s
			});
		}

		public static LocalizedString DefaultMailboxSettings
		{
			get
			{
				return new LocalizedString("DefaultMailboxSettings", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ConfirmationMessageExportUMMailboxPrompt(string promptName)
		{
			return new LocalizedString("ConfirmationMessageExportUMMailboxPrompt", "Ex6462C2", false, true, Strings.ResourceManager, new object[]
			{
				promptName
			});
		}

		public static LocalizedString ConfirmationMessageStopUMPhoneSession
		{
			get
			{
				return new LocalizedString("ConfirmationMessageStopUMPhoneSession", "Ex00E79A", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString CannotDisableAutoAttendant_KeyMapping(string s)
		{
			return new LocalizedString("CannotDisableAutoAttendant_KeyMapping", "", false, false, Strings.ResourceManager, new object[]
			{
				s
			});
		}

		public static LocalizedString ExceptionSIPResouceIdConflictWithExistingValue(string sipResId, string sipProxy)
		{
			return new LocalizedString("ExceptionSIPResouceIdConflictWithExistingValue", "", false, false, Strings.ResourceManager, new object[]
			{
				sipResId,
				sipProxy
			});
		}

		public static LocalizedString PINResetfailedToResetPin(string s)
		{
			return new LocalizedString("PINResetfailedToResetPin", "Ex4860CE", false, true, Strings.ResourceManager, new object[]
			{
				s
			});
		}

		public static LocalizedString RpcNotRegistered(string server)
		{
			return new LocalizedString("RpcNotRegistered", "", false, false, Strings.ResourceManager, new object[]
			{
				server
			});
		}

		public static LocalizedString InstallUmCallRouterTask
		{
			get
			{
				return new LocalizedString("InstallUmCallRouterTask", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InvalidUMServerStateOperationException(string s)
		{
			return new LocalizedString("InvalidUMServerStateOperationException", "Ex887580", false, true, Strings.ResourceManager, new object[]
			{
				s
			});
		}

		public static LocalizedString ErrorUMInvalidExtensionFormat(string s)
		{
			return new LocalizedString("ErrorUMInvalidExtensionFormat", "Ex82291E", false, true, Strings.ResourceManager, new object[]
			{
				s
			});
		}

		public static LocalizedString SavePINError(string user, LocalizedString reason)
		{
			return new LocalizedString("SavePINError", "Ex7152F3", false, true, Strings.ResourceManager, new object[]
			{
				user,
				reason
			});
		}

		public static LocalizedString AutoAttendantAlreadDisabledException(string s)
		{
			return new LocalizedString("AutoAttendantAlreadDisabledException", "ExB5F0B1", false, true, Strings.ResourceManager, new object[]
			{
				s
			});
		}

		public static LocalizedString UnableToSetMSSRegistryValue(string registryKey, string exceptionMessage)
		{
			return new LocalizedString("UnableToSetMSSRegistryValue", "ExA53D7C", false, true, Strings.ResourceManager, new object[]
			{
				registryKey,
				exceptionMessage
			});
		}

		public static LocalizedString CannotCreateHuntGroupForHostedSipDialPlan
		{
			get
			{
				return new LocalizedString("CannotCreateHuntGroupForHostedSipDialPlan", "ExF07F7A", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString PasswordMailBody
		{
			get
			{
				return new LocalizedString("PasswordMailBody", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ConfirmationMessageEnableUMAutoAttendant(string Identity)
		{
			return new LocalizedString("ConfirmationMessageEnableUMAutoAttendant", "ExB8396E", false, true, Strings.ResourceManager, new object[]
			{
				Identity
			});
		}

		public static LocalizedString DialPlanAssociatedWithIPGatewayException(string s)
		{
			return new LocalizedString("DialPlanAssociatedWithIPGatewayException", "ExA19A7E", false, true, Strings.ResourceManager, new object[]
			{
				s
			});
		}

		public static LocalizedString ExternalHostFqdnChanges
		{
			get
			{
				return new LocalizedString("ExternalHostFqdnChanges", "Ex6260BA", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RemoteEndDisconnected
		{
			get
			{
				return new LocalizedString("RemoteEndDisconnected", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString TUILogonSuccessful(string s)
		{
			return new LocalizedString("TUILogonSuccessful", "Ex4A52B6", false, true, Strings.ResourceManager, new object[]
			{
				s
			});
		}

		public static LocalizedString SIPFEServerConfigurationNotFound(string serverName)
		{
			return new LocalizedString("SIPFEServerConfigurationNotFound", "", false, false, Strings.ResourceManager, new object[]
			{
				serverName
			});
		}

		public static LocalizedString SuccessfulLogonState
		{
			get
			{
				return new LocalizedString("SuccessfulLogonState", "ExED26CE", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ValidCertRequiredForUM
		{
			get
			{
				return new LocalizedString("ValidCertRequiredForUM", "ExAD69E6", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ConfirmationMessageCopyUMCustomPromptDownloadAutoAttendantPrompts(string TargetPath, string UMAutoAttendant)
		{
			return new LocalizedString("ConfirmationMessageCopyUMCustomPromptDownloadAutoAttendantPrompts", "ExBC9FB1", false, true, Strings.ResourceManager, new object[]
			{
				TargetPath,
				UMAutoAttendant
			});
		}

		public static LocalizedString AttemptingToCreateHuntgroup
		{
			get
			{
				return new LocalizedString("AttemptingToCreateHuntgroup", "ExACF72F", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString AutoAttendantEnabledEvent(string s)
		{
			return new LocalizedString("AutoAttendantEnabledEvent", "", false, false, Strings.ResourceManager, new object[]
			{
				s
			});
		}

		public static LocalizedString DPLinkedGwNotFQDN(string address, string gateway)
		{
			return new LocalizedString("DPLinkedGwNotFQDN", "Ex4E8609", false, true, Strings.ResourceManager, new object[]
			{
				address,
				gateway
			});
		}

		public static LocalizedString ExceptionIPGatewayAlreadyExists(string ipaddress)
		{
			return new LocalizedString("ExceptionIPGatewayAlreadyExists", "", false, false, Strings.ResourceManager, new object[]
			{
				ipaddress
			});
		}

		public static LocalizedString ConfirmationMessageExportUMPromptAutoAttendantPrompts(string Path, string UMAutoAttendant)
		{
			return new LocalizedString("ConfirmationMessageExportUMPromptAutoAttendantPrompts", "Ex236E06", false, true, Strings.ResourceManager, new object[]
			{
				Path,
				UMAutoAttendant
			});
		}

		public static LocalizedString InvalidLanguageIdException(string l)
		{
			return new LocalizedString("InvalidLanguageIdException", "ExB6F513", false, true, Strings.ResourceManager, new object[]
			{
				l
			});
		}

		public static LocalizedString OperationSuccessful
		{
			get
			{
				return new LocalizedString("OperationSuccessful", "Ex469E17", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString CallRouterTransferFromTLStoTCPModeWarning
		{
			get
			{
				return new LocalizedString("CallRouterTransferFromTLStoTCPModeWarning", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InvalidAutoAttendant(string s)
		{
			return new LocalizedString("InvalidAutoAttendant", "", false, false, Strings.ResourceManager, new object[]
			{
				s
			});
		}

		public static LocalizedString RpcUnavailable(string server)
		{
			return new LocalizedString("RpcUnavailable", "", false, false, Strings.ResourceManager, new object[]
			{
				server
			});
		}

		public static LocalizedString UmServiceNotInstalled
		{
			get
			{
				return new LocalizedString("UmServiceNotInstalled", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString UMStartupModeChanges
		{
			get
			{
				return new LocalizedString("UMStartupModeChanges", "ExE8B015", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString FailedToEstablishMedia(int seconds)
		{
			return new LocalizedString("FailedToEstablishMedia", "ExE69203", false, true, Strings.ResourceManager, new object[]
			{
				seconds
			});
		}

		public static LocalizedString IPGatewayAlreadEnabledException(string s)
		{
			return new LocalizedString("IPGatewayAlreadEnabledException", "ExBA40F0", false, true, Strings.ResourceManager, new object[]
			{
				s
			});
		}

		public static LocalizedString DefaultLanguageNotAvailable(string val)
		{
			return new LocalizedString("DefaultLanguageNotAvailable", "", false, false, Strings.ResourceManager, new object[]
			{
				val
			});
		}

		public static LocalizedString MismatchedOrgInDPAndGW(string dp, string gw)
		{
			return new LocalizedString("MismatchedOrgInDPAndGW", "", false, false, Strings.ResourceManager, new object[]
			{
				dp,
				gw
			});
		}

		public static LocalizedString PromptProvisioningShareDescription
		{
			get
			{
				return new LocalizedString("PromptProvisioningShareDescription", "ExEE67A1", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ConfirmationMessageRemoveUMHuntGroup(string Identity)
		{
			return new LocalizedString("ConfirmationMessageRemoveUMHuntGroup", "ExFFB4D2", false, true, Strings.ResourceManager, new object[]
			{
				Identity
			});
		}

		public static LocalizedString ConfirmationMessageNewUMDialPlan(string Name, string NumberOfDigitsInExtension)
		{
			return new LocalizedString("ConfirmationMessageNewUMDialPlan", "Ex61C8AF", false, true, Strings.ResourceManager, new object[]
			{
				Name,
				NumberOfDigitsInExtension
			});
		}

		public static LocalizedString MaxAsrPhraseLengthExceeded(string menu)
		{
			return new LocalizedString("MaxAsrPhraseLengthExceeded", "", false, false, Strings.ResourceManager, new object[]
			{
				menu
			});
		}

		public static LocalizedString ErrorOrganizationNotUnique(string idStringValue)
		{
			return new LocalizedString("ErrorOrganizationNotUnique", "ExA3D209", false, true, Strings.ResourceManager, new object[]
			{
				idStringValue
			});
		}

		public static LocalizedString UmCallRouterName
		{
			get
			{
				return new LocalizedString("UmCallRouterName", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString OperationNotSupportedOnLegacMailboxException(string use)
		{
			return new LocalizedString("OperationNotSupportedOnLegacMailboxException", "ExCC7517", false, true, Strings.ResourceManager, new object[]
			{
				use
			});
		}

		public static LocalizedString UserProblem
		{
			get
			{
				return new LocalizedString("UserProblem", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString SipOptionsError(string targetUri, string error)
		{
			return new LocalizedString("SipOptionsError", "", false, false, Strings.ResourceManager, new object[]
			{
				targetUri,
				error
			});
		}

		public static LocalizedString ConfigureGatewayToForwardCallsMsg
		{
			get
			{
				return new LocalizedString("ConfigureGatewayToForwardCallsMsg", "Ex78168D", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ConfirmationMessageSetUmServer(string Identity)
		{
			return new LocalizedString("ConfirmationMessageSetUmServer", "ExBAA6AF", false, true, Strings.ResourceManager, new object[]
			{
				Identity
			});
		}

		public static LocalizedString NonExistantServer(string s)
		{
			return new LocalizedString("NonExistantServer", "Ex84E341", false, true, Strings.ResourceManager, new object[]
			{
				s
			});
		}

		public static LocalizedString GatewayAddressRequiresFqdn
		{
			get
			{
				return new LocalizedString("GatewayAddressRequiresFqdn", "Ex78EEA2", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString DNSEntryNotFound
		{
			get
			{
				return new LocalizedString("DNSEntryNotFound", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ExceptionInvalidSipNameDomain
		{
			get
			{
				return new LocalizedString("ExceptionInvalidSipNameDomain", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorContactAddressListNotUnique(string cal)
		{
			return new LocalizedString("ErrorContactAddressListNotUnique", "ExA27D04", false, true, Strings.ResourceManager, new object[]
			{
				cal
			});
		}

		public static LocalizedString UninstallUmCallRouterTask
		{
			get
			{
				return new LocalizedString("UninstallUmCallRouterTask", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString DefaultUMMailboxPolicyName(string dialPlan)
		{
			return new LocalizedString("DefaultUMMailboxPolicyName", "Ex23F363", false, true, Strings.ResourceManager, new object[]
			{
				dialPlan
			});
		}

		public static LocalizedString DefaultPolicyCreationNameTooLong(string dialPlan)
		{
			return new LocalizedString("DefaultPolicyCreationNameTooLong", "ExDB32C1", false, true, Strings.ResourceManager, new object[]
			{
				dialPlan
			});
		}

		public static LocalizedString CallAnsweringRuleNotFoundException(string identity)
		{
			return new LocalizedString("CallAnsweringRuleNotFoundException", "", false, false, Strings.ResourceManager, new object[]
			{
				identity
			});
		}

		public static LocalizedString EmptyASRPhrase(string menu)
		{
			return new LocalizedString("EmptyASRPhrase", "", false, false, Strings.ResourceManager, new object[]
			{
				menu
			});
		}

		public static LocalizedString ConfirmationMessageNewUMIPGateway(string Name, string IPAddress)
		{
			return new LocalizedString("ConfirmationMessageNewUMIPGateway", "ExE8280F", false, true, Strings.ResourceManager, new object[]
			{
				Name,
				IPAddress
			});
		}

		public static LocalizedString NonExistantDialPlan(object s)
		{
			return new LocalizedString("NonExistantDialPlan", "ExF33045", false, true, Strings.ResourceManager, new object[]
			{
				s
			});
		}

		public static LocalizedString LogonError
		{
			get
			{
				return new LocalizedString("LogonError", "ExE453E4", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ConfirmationMessageRemoveUMAutoAttendant(string Identity)
		{
			return new LocalizedString("ConfirmationMessageRemoveUMAutoAttendant", "ExF5A4B1", false, true, Strings.ResourceManager, new object[]
			{
				Identity
			});
		}

		public static LocalizedString UMServerAlreadDisabledException(string s)
		{
			return new LocalizedString("UMServerAlreadDisabledException", "Ex1FA46E", false, true, Strings.ResourceManager, new object[]
			{
				s
			});
		}

		public static LocalizedString ConfirmationMessageDisableUMServerImmediately
		{
			get
			{
				return new LocalizedString("ConfirmationMessageDisableUMServerImmediately", "Ex5524D9", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString GatewayIPAddressFamilyInconsistentException
		{
			get
			{
				return new LocalizedString("GatewayIPAddressFamilyInconsistentException", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ConfirmationMessageDisableUMServer
		{
			get
			{
				return new LocalizedString("ConfirmationMessageDisableUMServer", "Ex3CC24E", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString AutoAttendantDisabledEvent(string s)
		{
			return new LocalizedString("AutoAttendantDisabledEvent", "", false, false, Strings.ResourceManager, new object[]
			{
				s
			});
		}

		public static LocalizedString NoMailboxServersFound
		{
			get
			{
				return new LocalizedString("NoMailboxServersFound", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MakeCallError(string host, string error)
		{
			return new LocalizedString("MakeCallError", "", false, false, Strings.ResourceManager, new object[]
			{
				host,
				error
			});
		}

		public static LocalizedString ChangesTakeEffectAfterRestartingUmServer(string changedObject, string server, string extraData)
		{
			return new LocalizedString("ChangesTakeEffectAfterRestartingUmServer", "Ex2F1EC6", false, true, Strings.ResourceManager, new object[]
			{
				changedObject,
				server,
				extraData
			});
		}

		public static LocalizedString SrtpWithoutTls
		{
			get
			{
				return new LocalizedString("SrtpWithoutTls", "Ex7E56C5", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ServerIsPublishingPointException(string dialPlan)
		{
			return new LocalizedString("ServerIsPublishingPointException", "", false, false, Strings.ResourceManager, new object[]
			{
				dialPlan
			});
		}

		public static LocalizedString AutoAttendantAlreadEnabledException(string s)
		{
			return new LocalizedString("AutoAttendantAlreadEnabledException", "ExA84098", false, true, Strings.ResourceManager, new object[]
			{
				s
			});
		}

		public static LocalizedString AutoAttendantAlreadyExistsException(string name, string dialplan)
		{
			return new LocalizedString("AutoAttendantAlreadyExistsException", "", false, false, Strings.ResourceManager, new object[]
			{
				name,
				dialplan
			});
		}

		public static LocalizedString ConfirmationMessageDisableUMIPGateway
		{
			get
			{
				return new LocalizedString("ConfirmationMessageDisableUMIPGateway", "Ex46A84A", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString UninstallUmServiceTask
		{
			get
			{
				return new LocalizedString("UninstallUmServiceTask", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ConfirmationMessageRemoveUMDialPlan(string Identity)
		{
			return new LocalizedString("ConfirmationMessageRemoveUMDialPlan", "Ex1539C6", false, true, Strings.ResourceManager, new object[]
			{
				Identity
			});
		}

		public static LocalizedString UmServiceDescription
		{
			get
			{
				return new LocalizedString("UmServiceDescription", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString DiagnosticSequence(string dtmf)
		{
			return new LocalizedString("DiagnosticSequence", "Ex5BAAEA", false, true, Strings.ResourceManager, new object[]
			{
				dtmf
			});
		}

		public static LocalizedString TUILogonfailedToMakeCall(string s)
		{
			return new LocalizedString("TUILogonfailedToMakeCall", "Ex67DE1D", false, true, Strings.ResourceManager, new object[]
			{
				s
			});
		}

		public static LocalizedString DefaultMailboxRequiredWhenForwardTrue
		{
			get
			{
				return new LocalizedString("DefaultMailboxRequiredWhenForwardTrue", "Ex9F9BB7", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MultipleUMMailboxPolicyWithSameId(string s)
		{
			return new LocalizedString("MultipleUMMailboxPolicyWithSameId", "Ex95B89D", false, true, Strings.ResourceManager, new object[]
			{
				s
			});
		}

		public static LocalizedString ConfirmationMessageTestUMConnectivityLocalLoop
		{
			get
			{
				return new LocalizedString("ConfirmationMessageTestUMConnectivityLocalLoop", "Ex140AFE", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ConfirmationMessageRemoveUMIPGateway(string Identity)
		{
			return new LocalizedString("ConfirmationMessageRemoveUMIPGateway", "ExE8CFB2", false, true, Strings.ResourceManager, new object[]
			{
				Identity
			});
		}

		public static LocalizedString ErrorUMInvalidSipNameAddressFormat(string s)
		{
			return new LocalizedString("ErrorUMInvalidSipNameAddressFormat", "ExFEA28B", false, true, Strings.ResourceManager, new object[]
			{
				s
			});
		}

		public static LocalizedString NonExistantIPGateway(string s)
		{
			return new LocalizedString("NonExistantIPGateway", "Ex9184CF", false, true, Strings.ResourceManager, new object[]
			{
				s
			});
		}

		public static LocalizedString InvalidDefaultOutboundCallingLineId
		{
			get
			{
				return new LocalizedString("InvalidDefaultOutboundCallingLineId", "Ex983F08", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorContactAddressListNotFound(string cal)
		{
			return new LocalizedString("ErrorContactAddressListNotFound", "Ex79E0BB", false, true, Strings.ResourceManager, new object[]
			{
				cal
			});
		}

		public static LocalizedString DnsResolutionError(string hostName, string message)
		{
			return new LocalizedString("DnsResolutionError", "", false, false, Strings.ResourceManager, new object[]
			{
				hostName,
				message
			});
		}

		public static LocalizedString ErrorGeneratingDefaultPassword
		{
			get
			{
				return new LocalizedString("ErrorGeneratingDefaultPassword", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorObjectNotFound(string s)
		{
			return new LocalizedString("ErrorObjectNotFound", "ExF2BD41", false, true, Strings.ResourceManager, new object[]
			{
				s
			});
		}

		public static LocalizedString ErrorWeakPasswordHistorySingular(int minLength)
		{
			return new LocalizedString("ErrorWeakPasswordHistorySingular", "", false, false, Strings.ResourceManager, new object[]
			{
				minLength
			});
		}

		public static LocalizedString InvalidDTMFSequenceReceived
		{
			get
			{
				return new LocalizedString("InvalidDTMFSequenceReceived", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ConfirmationMessageNewUMHuntGroup(string Name, string PilotIdentifier, string UMDialPlan, string IPGateway)
		{
			return new LocalizedString("ConfirmationMessageNewUMHuntGroup", "Ex89A6FA", false, true, Strings.ResourceManager, new object[]
			{
				Name,
				PilotIdentifier,
				UMDialPlan,
				IPGateway
			});
		}

		public static LocalizedString UninstallUMWebServiceTask
		{
			get
			{
				return new LocalizedString("UninstallUMWebServiceTask", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ExceptionUserAlreadyUmEnabled(string s)
		{
			return new LocalizedString("ExceptionUserAlreadyUmEnabled", "", false, false, Strings.ResourceManager, new object[]
			{
				s
			});
		}

		public static LocalizedString ResetUMMailboxError(string user, LocalizedString reason)
		{
			return new LocalizedString("ResetUMMailboxError", "ExE75932", false, true, Strings.ResourceManager, new object[]
			{
				user,
				reason
			});
		}

		public static LocalizedString InvalidAutoAttendantInDialPlan(string s, string d)
		{
			return new LocalizedString("InvalidAutoAttendantInDialPlan", "", false, false, Strings.ResourceManager, new object[]
			{
				s,
				d
			});
		}

		public static LocalizedString ErrorUMInvalidE164AddressFormat(string s)
		{
			return new LocalizedString("ErrorUMInvalidE164AddressFormat", "Ex49FCC2", false, true, Strings.ResourceManager, new object[]
			{
				s
			});
		}

		public static LocalizedString ADError
		{
			get
			{
				return new LocalizedString("ADError", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString SendPINResetMailError(string user, LocalizedString reason)
		{
			return new LocalizedString("SendPINResetMailError", "Ex83904B", false, true, Strings.ResourceManager, new object[]
			{
				user,
				reason
			});
		}

		public static LocalizedString Confirm(string userId)
		{
			return new LocalizedString("Confirm", "Ex395C75", false, true, Strings.ResourceManager, new object[]
			{
				userId
			});
		}

		public static LocalizedString PINResetfailedToResetPasswd(string s)
		{
			return new LocalizedString("PINResetfailedToResetPasswd", "Ex3F7D35", false, true, Strings.ResourceManager, new object[]
			{
				s
			});
		}

		public static LocalizedString ErrorWeakPasswordNoHistory(int minLength)
		{
			return new LocalizedString("ErrorWeakPasswordNoHistory", "", false, false, Strings.ResourceManager, new object[]
			{
				minLength
			});
		}

		public static LocalizedString NotMailboxServer
		{
			get
			{
				return new LocalizedString("NotMailboxServer", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString LanguagesNotPassed
		{
			get
			{
				return new LocalizedString("LanguagesNotPassed", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InvalidUMUser(string phone, string dialplan)
		{
			return new LocalizedString("InvalidUMUser", "ExE5F49C", false, true, Strings.ResourceManager, new object[]
			{
				phone,
				dialplan
			});
		}

		public static LocalizedString ErrorUMInvalidSipNameDomain(string s)
		{
			return new LocalizedString("ErrorUMInvalidSipNameDomain", "Ex64C4F6", false, true, Strings.ResourceManager, new object[]
			{
				s
			});
		}

		public static LocalizedString InstallUmServiceTask
		{
			get
			{
				return new LocalizedString("InstallUmServiceTask", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InvalidServerVersionForUMRpcTask(string server)
		{
			return new LocalizedString("InvalidServerVersionForUMRpcTask", "Ex0916DC", false, true, Strings.ResourceManager, new object[]
			{
				server
			});
		}

		public static LocalizedString WaitForFirstDiagnosticResponse
		{
			get
			{
				return new LocalizedString("WaitForFirstDiagnosticResponse", "Ex9D6E1C", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MultipleIPGatewaysWithSameId(string s)
		{
			return new LocalizedString("MultipleIPGatewaysWithSameId", "Ex449BE0", false, true, Strings.ResourceManager, new object[]
			{
				s
			});
		}

		public static LocalizedString InvalidTimeZoneParameters
		{
			get
			{
				return new LocalizedString("InvalidTimeZoneParameters", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString CertNotFound
		{
			get
			{
				return new LocalizedString("CertNotFound", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString PilotNumberState
		{
			get
			{
				return new LocalizedString("PilotNumberState", "Ex034A59", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString KeepProperties
		{
			get
			{
				return new LocalizedString("KeepProperties", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString DuplicateMenuName(string menu)
		{
			return new LocalizedString("DuplicateMenuName", "", false, false, Strings.ResourceManager, new object[]
			{
				menu
			});
		}

		public static LocalizedString WaitForDiagnosticResponse
		{
			get
			{
				return new LocalizedString("WaitForDiagnosticResponse", "ExB5BEB1", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString FirewallCorrectlyConfigured(string server)
		{
			return new LocalizedString("FirewallCorrectlyConfigured", "ExF0465F", false, true, Strings.ResourceManager, new object[]
			{
				server
			});
		}

		public static LocalizedString UCMAPreReqException
		{
			get
			{
				return new LocalizedString("UCMAPreReqException", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ChangesTakeEffectAfterRestartingUmCallRouterService(string changedObject, string server, string extraData)
		{
			return new LocalizedString("ChangesTakeEffectAfterRestartingUmCallRouterService", "", false, false, Strings.ResourceManager, new object[]
			{
				changedObject,
				server,
				extraData
			});
		}

		public static LocalizedString DialPlanAssociatedWithPoliciesException
		{
			get
			{
				return new LocalizedString("DialPlanAssociatedWithPoliciesException", "ExF5AEBA", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString PinExpired
		{
			get
			{
				return new LocalizedString("PinExpired", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString LockedOut
		{
			get
			{
				return new LocalizedString("LockedOut", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString GatewayFqdnNotInAcceptedDomain
		{
			get
			{
				return new LocalizedString("GatewayFqdnNotInAcceptedDomain", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ScopeErrorOnAutoAttendant(string aa, string reason)
		{
			return new LocalizedString("ScopeErrorOnAutoAttendant", "Ex95FA7F", false, true, Strings.ResourceManager, new object[]
			{
				aa,
				reason
			});
		}

		public static LocalizedString DefaultPolicyCreation(string moreInfo)
		{
			return new LocalizedString("DefaultPolicyCreation", "Ex1513EC", false, true, Strings.ResourceManager, new object[]
			{
				moreInfo
			});
		}

		public static LocalizedString ConfirmationMessageCopyUMCustomPromptDownloadDialPlanPrompts(string TargetPath, string UMDialPlan)
		{
			return new LocalizedString("ConfirmationMessageCopyUMCustomPromptDownloadDialPlanPrompts", "Ex01C3E9", false, true, Strings.ResourceManager, new object[]
			{
				TargetPath,
				UMDialPlan
			});
		}

		public static LocalizedString NoDTMFSwereReceived
		{
			get
			{
				return new LocalizedString("NoDTMFSwereReceived", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString DialPlanAssociatedWithAutoAttendantException(string s)
		{
			return new LocalizedString("DialPlanAssociatedWithAutoAttendantException", "ExCD9DAC", false, true, Strings.ResourceManager, new object[]
			{
				s
			});
		}

		public static LocalizedString ExceptionUserAlreadyUmDisabled(string s)
		{
			return new LocalizedString("ExceptionUserAlreadyUmDisabled", "", false, false, Strings.ResourceManager, new object[]
			{
				s
			});
		}

		public static LocalizedString AADisableWhatifString(string s)
		{
			return new LocalizedString("AADisableWhatifString", "", false, false, Strings.ResourceManager, new object[]
			{
				s
			});
		}

		public static LocalizedString InitUMMailboxError(string user, LocalizedString reason)
		{
			return new LocalizedString("InitUMMailboxError", "ExBC88DA", false, true, Strings.ResourceManager, new object[]
			{
				user,
				reason
			});
		}

		public static LocalizedString SipUriError(string field)
		{
			return new LocalizedString("SipUriError", "", false, false, Strings.ResourceManager, new object[]
			{
				field
			});
		}

		public static LocalizedString PasswordMailSubject
		{
			get
			{
				return new LocalizedString("PasswordMailSubject", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InvalidIPAddressReceived
		{
			get
			{
				return new LocalizedString("InvalidIPAddressReceived", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InvalidALParameterException
		{
			get
			{
				return new LocalizedString("InvalidALParameterException", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ConfirmationMessageCopyUMCustomPromptUploadAutoAttendantPrompts(string Path, string UMAutoAttendant)
		{
			return new LocalizedString("ConfirmationMessageCopyUMCustomPromptUploadAutoAttendantPrompts", "Ex9F7491", false, true, Strings.ResourceManager, new object[]
			{
				Path,
				UMAutoAttendant
			});
		}

		public static LocalizedString MustSpecifyThumbprint
		{
			get
			{
				return new LocalizedString("MustSpecifyThumbprint", "Ex8B33C9", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InvalidMailboxServerVersionForTUMCTask
		{
			get
			{
				return new LocalizedString("InvalidMailboxServerVersionForTUMCTask", "Ex4B2908", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString CannotCreateGatewayForHostedSipDialPlan
		{
			get
			{
				return new LocalizedString("CannotCreateGatewayForHostedSipDialPlan", "Ex27F54C", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InvalidMailbox(string mailbox, string setting)
		{
			return new LocalizedString("InvalidMailbox", "", false, false, Strings.ResourceManager, new object[]
			{
				mailbox,
				setting
			});
		}

		public static LocalizedString ConfirmationMessageExportUMCallDataRecord(string date)
		{
			return new LocalizedString("ConfirmationMessageExportUMCallDataRecord", "ExE568C5", false, true, Strings.ResourceManager, new object[]
			{
				date
			});
		}

		public static LocalizedString UMMailboxPolicyNotPresent(string user)
		{
			return new LocalizedString("UMMailboxPolicyNotPresent", "ExA64168", false, true, Strings.ResourceManager, new object[]
			{
				user
			});
		}

		public static LocalizedString InvalidDtmfChar(char c)
		{
			return new LocalizedString("InvalidDtmfChar", "Ex3255FC", false, true, Strings.ResourceManager, new object[]
			{
				c
			});
		}

		public static LocalizedString ConfirmationMessageDisableUMIPGatewayImmediately
		{
			get
			{
				return new LocalizedString("ConfirmationMessageDisableUMIPGatewayImmediately", "Ex856DF0", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorWeakPasswordHistoryPlural(int minLength, int history)
		{
			return new LocalizedString("ErrorWeakPasswordHistoryPlural", "", false, false, Strings.ResourceManager, new object[]
			{
				minLength,
				history
			});
		}

		public static LocalizedString PilotNumber
		{
			get
			{
				return new LocalizedString("PilotNumber", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString AttemptingToCreateIPGateway
		{
			get
			{
				return new LocalizedString("AttemptingToCreateIPGateway", "Ex92EE52", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ExceptionUserNotAllowedForUMEnabled
		{
			get
			{
				return new LocalizedString("ExceptionUserNotAllowedForUMEnabled", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ExceptionIPGatewayIPAddressAlreadyExists(string ipaddress)
		{
			return new LocalizedString("ExceptionIPGatewayIPAddressAlreadyExists", "", false, false, Strings.ResourceManager, new object[]
			{
				ipaddress
			});
		}

		public static LocalizedString IPGatewayAlreadDisabledException(string s)
		{
			return new LocalizedString("IPGatewayAlreadDisabledException", "ExC437B0", false, true, Strings.ResourceManager, new object[]
			{
				s
			});
		}

		public static LocalizedString ExchangePrincipalError
		{
			get
			{
				return new LocalizedString("ExchangePrincipalError", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorWeakPasswordWithNoCommonPatterns(LocalizedString baseText)
		{
			return new LocalizedString("ErrorWeakPasswordWithNoCommonPatterns", "", false, false, Strings.ResourceManager, new object[]
			{
				baseText
			});
		}

		public static LocalizedString InvalidExternalHostFqdn
		{
			get
			{
				return new LocalizedString("InvalidExternalHostFqdn", "Ex63572E", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString UCMAPreReqUpgradeException
		{
			get
			{
				return new LocalizedString("UCMAPreReqUpgradeException", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString AADisableConfirmationString
		{
			get
			{
				return new LocalizedString("AADisableConfirmationString", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString AAAlreadyDisabled
		{
			get
			{
				return new LocalizedString("AAAlreadyDisabled", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MaxAsrPhraseCountExceeded(string menu)
		{
			return new LocalizedString("MaxAsrPhraseCountExceeded", "", false, false, Strings.ResourceManager, new object[]
			{
				menu
			});
		}

		public static LocalizedString InvalidAAFileExtension(string s)
		{
			return new LocalizedString("InvalidAAFileExtension", "", false, false, Strings.ResourceManager, new object[]
			{
				s
			});
		}

		public static LocalizedString ConfirmationMessageTestUMConnectivityPinReset
		{
			get
			{
				return new LocalizedString("ConfirmationMessageTestUMConnectivityPinReset", "Ex152E23", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString SendSequenceError(string error)
		{
			return new LocalizedString("SendSequenceError", "", false, false, Strings.ResourceManager, new object[]
			{
				error
			});
		}

		public static LocalizedString ConfirmationMessageImportUMPromptAutoAttendantPrompts(string Path, string UMAutoAttendant)
		{
			return new LocalizedString("ConfirmationMessageImportUMPromptAutoAttendantPrompts", "Ex406BE4", false, true, Strings.ResourceManager, new object[]
			{
				Path,
				UMAutoAttendant
			});
		}

		public static LocalizedString MailboxNotLocal(string userName, string mailboxServer)
		{
			return new LocalizedString("MailboxNotLocal", "", false, false, Strings.ResourceManager, new object[]
			{
				userName,
				mailboxServer
			});
		}

		public static LocalizedString DuplicateASRPhrase(string phrase)
		{
			return new LocalizedString("DuplicateASRPhrase", "", false, false, Strings.ResourceManager, new object[]
			{
				phrase
			});
		}

		public static LocalizedString ErrorServerNotFound(object idStringValue)
		{
			return new LocalizedString("ErrorServerNotFound", "", false, false, Strings.ResourceManager, new object[]
			{
				idStringValue
			});
		}

		public static LocalizedString ErrorWeakPassword(string details)
		{
			return new LocalizedString("ErrorWeakPassword", "", false, false, Strings.ResourceManager, new object[]
			{
				details
			});
		}

		public static LocalizedString CertWithoutTls
		{
			get
			{
				return new LocalizedString("CertWithoutTls", "Ex76FB5C", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString SendEmail
		{
			get
			{
				return new LocalizedString("SendEmail", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ConfirmationMessageSetUMMailboxPIN(string Identity)
		{
			return new LocalizedString("ConfirmationMessageSetUMMailboxPIN", "Ex14B69B", false, true, Strings.ResourceManager, new object[]
			{
				Identity
			});
		}

		public static LocalizedString InvalidIPGatewayStateOperationException(string s)
		{
			return new LocalizedString("InvalidIPGatewayStateOperationException", "ExDF0034", false, true, Strings.ResourceManager, new object[]
			{
				s
			});
		}

		public static LocalizedString TUILogonfailedToLogon(string s)
		{
			return new LocalizedString("TUILogonfailedToLogon", "Ex9FCD6D", false, true, Strings.ResourceManager, new object[]
			{
				s
			});
		}

		public static LocalizedString DesktopExperienceRequiredException(string serverFqdn)
		{
			return new LocalizedString("DesktopExperienceRequiredException", "Ex61B65E", false, true, Strings.ResourceManager, new object[]
			{
				serverFqdn
			});
		}

		public static LocalizedString NewPublishingPointException(string shareName, string moreInfo)
		{
			return new LocalizedString("NewPublishingPointException", "", false, false, Strings.ResourceManager, new object[]
			{
				shareName,
				moreInfo
			});
		}

		public static LocalizedString ExceptionIPGatewayInvalid(string ipaddress)
		{
			return new LocalizedString("ExceptionIPGatewayInvalid", "", false, false, Strings.ResourceManager, new object[]
			{
				ipaddress
			});
		}

		public static LocalizedString ConfirmationMessageSetUMAutoAttendant(string Identity)
		{
			return new LocalizedString("ConfirmationMessageSetUMAutoAttendant", "ExC46D25", false, true, Strings.ResourceManager, new object[]
			{
				Identity
			});
		}

		public static LocalizedString ExceptionSipResourceIdNotUnique
		{
			get
			{
				return new LocalizedString("ExceptionSipResourceIdNotUnique", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString DuplicateE164PilotIdentifierListEntry(string objectName)
		{
			return new LocalizedString("DuplicateE164PilotIdentifierListEntry", "Ex1893DE", false, true, Strings.ResourceManager, new object[]
			{
				objectName
			});
		}

		public static LocalizedString PortChanges
		{
			get
			{
				return new LocalizedString("PortChanges", "Ex49211B", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString AANameTooLong
		{
			get
			{
				return new LocalizedString("AANameTooLong", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorOrganizationalUnitNotUnique(string ou)
		{
			return new LocalizedString("ErrorOrganizationalUnitNotUnique", "ExDA52FD", false, true, Strings.ResourceManager, new object[]
			{
				ou
			});
		}

		public static LocalizedString DefaultUMHuntGroupName
		{
			get
			{
				return new LocalizedString("DefaultUMHuntGroupName", "Ex28450E", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorUMPilotIdentifierInUse(string pilotIdentifier, string aa, string dp)
		{
			return new LocalizedString("ErrorUMPilotIdentifierInUse", "Ex33BB1F", false, true, Strings.ResourceManager, new object[]
			{
				pilotIdentifier,
				aa,
				dp
			});
		}

		public static LocalizedString ConfirmationMessageEnableUMIPGateway(string Identity)
		{
			return new LocalizedString("ConfirmationMessageEnableUMIPGateway", "Ex016190", false, true, Strings.ResourceManager, new object[]
			{
				Identity
			});
		}

		public static LocalizedString ChangingMSSMaxDiskCacheSize(string maxDiskCacheSize)
		{
			return new LocalizedString("ChangingMSSMaxDiskCacheSize", "Ex0B61A3", false, true, Strings.ResourceManager, new object[]
			{
				maxDiskCacheSize
			});
		}

		public static LocalizedString ConfirmationMessageImportUMPromptDialPlanPrompts(string Path, string UMDialPlan)
		{
			return new LocalizedString("ConfirmationMessageImportUMPromptDialPlanPrompts", "ExCFE423", false, true, Strings.ResourceManager, new object[]
			{
				Path,
				UMDialPlan
			});
		}

		public static LocalizedString DuplicateKeys(string key)
		{
			return new LocalizedString("DuplicateKeys", "", false, false, Strings.ResourceManager, new object[]
			{
				key
			});
		}

		public static LocalizedString DefaultAutoAttendantInDialPlanException(string s)
		{
			return new LocalizedString("DefaultAutoAttendantInDialPlanException", "", false, false, Strings.ResourceManager, new object[]
			{
				s
			});
		}

		public static LocalizedString GenericRPCError(string msg, string server)
		{
			return new LocalizedString("GenericRPCError", "", false, false, Strings.ResourceManager, new object[]
			{
				msg,
				server
			});
		}

		public static LocalizedString CouldnotRetreivePasswd
		{
			get
			{
				return new LocalizedString("CouldnotRetreivePasswd", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString TopologyDiscoveryProblem(string s)
		{
			return new LocalizedString("TopologyDiscoveryProblem", "", false, false, Strings.ResourceManager, new object[]
			{
				s
			});
		}

		public static LocalizedString NonExistantAutoAttendant(string s)
		{
			return new LocalizedString("NonExistantAutoAttendant", "ExD0D0CA", false, true, Strings.ResourceManager, new object[]
			{
				s
			});
		}

		public static LocalizedString MailboxMustBeSpecifiedException(string parameter)
		{
			return new LocalizedString("MailboxMustBeSpecifiedException", "", false, false, Strings.ResourceManager, new object[]
			{
				parameter
			});
		}

		public static LocalizedString PINEnterState
		{
			get
			{
				return new LocalizedString("PINEnterState", "ExA22288", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InvalidSpeechEnabledAutoAttendant(string s)
		{
			return new LocalizedString("InvalidSpeechEnabledAutoAttendant", "", false, false, Strings.ResourceManager, new object[]
			{
				s
			});
		}

		public static LocalizedString BusinessHoursSettings
		{
			get
			{
				return new LocalizedString("BusinessHoursSettings", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString UmServiceStillInstalled
		{
			get
			{
				return new LocalizedString("UmServiceStillInstalled", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InitializeError(string error)
		{
			return new LocalizedString("InitializeError", "", false, false, Strings.ResourceManager, new object[]
			{
				error
			});
		}

		public static LocalizedString OperationFailed(string operation)
		{
			return new LocalizedString("OperationFailed", "", false, false, Strings.ResourceManager, new object[]
			{
				operation
			});
		}

		public static LocalizedString ConfirmationMessageSetUmCallRouterSettings
		{
			get
			{
				return new LocalizedString("ConfirmationMessageSetUmCallRouterSettings", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ValidateGeneratePINError(string user, LocalizedString reason)
		{
			return new LocalizedString("ValidateGeneratePINError", "ExEFEEDF", false, true, Strings.ResourceManager, new object[]
			{
				user,
				reason
			});
		}

		public static LocalizedString ErrorOrganizationNotFound(string idStringValue)
		{
			return new LocalizedString("ErrorOrganizationNotFound", "Ex5948C4", false, true, Strings.ResourceManager, new object[]
			{
				idStringValue
			});
		}

		public static LocalizedString TUILogonfailedToGetPin(string s)
		{
			return new LocalizedString("TUILogonfailedToGetPin", "Ex9BC15A", false, true, Strings.ResourceManager, new object[]
			{
				s
			});
		}

		public static LocalizedString ErrorOrganizationalUnitNotFound(string ou)
		{
			return new LocalizedString("ErrorOrganizationalUnitNotFound", "Ex05F75C", false, true, Strings.ResourceManager, new object[]
			{
				ou
			});
		}

		public static LocalizedString NotifyEmailPilotNumberField(string s)
		{
			return new LocalizedString("NotifyEmailPilotNumberField", "", false, false, Strings.ResourceManager, new object[]
			{
				s
			});
		}

		public static LocalizedString ConfirmationMessageSetUMDialPlan(string Identity)
		{
			return new LocalizedString("ConfirmationMessageSetUMDialPlan", "Ex175B50", false, true, Strings.ResourceManager, new object[]
			{
				Identity
			});
		}

		public static LocalizedString ValidCertRequiredForUMCallRouter
		{
			get
			{
				return new LocalizedString("ValidCertRequiredForUMCallRouter", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InvalidDtmfFallbackAutoAttendantDialPlan(string s)
		{
			return new LocalizedString("InvalidDtmfFallbackAutoAttendantDialPlan", "", false, false, Strings.ResourceManager, new object[]
			{
				s
			});
		}

		public static LocalizedString ConfirmationMessageTestUMConnectivityEndToEnd(string IPGateway, string Phone)
		{
			return new LocalizedString("ConfirmationMessageTestUMConnectivityEndToEnd", "Ex887C22", false, true, Strings.ResourceManager, new object[]
			{
				IPGateway,
				Phone
			});
		}

		public static LocalizedString DialPlanAssociatedWithUserException
		{
			get
			{
				return new LocalizedString("DialPlanAssociatedWithUserException", "ExFEC83F", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString TransferFromTLStoTCPModeWarning
		{
			get
			{
				return new LocalizedString("TransferFromTLStoTCPModeWarning", "Ex79C6E3", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InvalidALParameter
		{
			get
			{
				return new LocalizedString("InvalidALParameter", "Ex13D141", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ConfirmationMessageSetUMMailboxConfiguration(string Identity)
		{
			return new LocalizedString("ConfirmationMessageSetUMMailboxConfiguration", "Ex92BDFC", false, true, Strings.ResourceManager, new object[]
			{
				Identity
			});
		}

		public static LocalizedString InvalidUMUserName(string email)
		{
			return new LocalizedString("InvalidUMUserName", "ExE9089C", false, true, Strings.ResourceManager, new object[]
			{
				email
			});
		}

		public static LocalizedString ExceptionDialPlanNotFound(string s)
		{
			return new LocalizedString("ExceptionDialPlanNotFound", "", false, false, Strings.ResourceManager, new object[]
			{
				s
			});
		}

		public static LocalizedString InvalidDtmfFallbackAutoAttendant_Disabled(string s)
		{
			return new LocalizedString("InvalidDtmfFallbackAutoAttendant_Disabled", "", false, false, Strings.ResourceManager, new object[]
			{
				s
			});
		}

		public static LocalizedString OperationTimedOutInTestUMConnectivityTask(string operation, string timeout)
		{
			return new LocalizedString("OperationTimedOutInTestUMConnectivityTask", "", false, false, Strings.ResourceManager, new object[]
			{
				operation,
				timeout
			});
		}

		public static LocalizedString UMServerAlreadEnabledException(string s)
		{
			return new LocalizedString("UMServerAlreadEnabledException", "Ex212695", false, true, Strings.ResourceManager, new object[]
			{
				s
			});
		}

		public static LocalizedString AfterHoursSettings
		{
			get
			{
				return new LocalizedString("AfterHoursSettings", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ConfirmationMessageDisableUMAutoAttendant(string Identity)
		{
			return new LocalizedString("ConfirmationMessageDisableUMAutoAttendant", "Ex9FFD26", false, true, Strings.ResourceManager, new object[]
			{
				Identity
			});
		}

		public static LocalizedString ConfirmationMessageCopyUMCustomPromptUploadDialPlanPrompts(string Path, string UMDialPlan)
		{
			return new LocalizedString("ConfirmationMessageCopyUMCustomPromptUploadDialPlanPrompts", "ExE1969C", false, true, Strings.ResourceManager, new object[]
			{
				Path,
				UMDialPlan
			});
		}

		public static LocalizedString UmCallRouterDescription
		{
			get
			{
				return new LocalizedString("UmCallRouterDescription", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InvalidAutoAttendantScopeSetting
		{
			get
			{
				return new LocalizedString("InvalidAutoAttendantScopeSetting", "ExFD440E", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString TcpAndTlsPortsCannotBeSame
		{
			get
			{
				return new LocalizedString("TcpAndTlsPortsCannotBeSame", "Ex174B88", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString DialPlanAssociatedWithServerException(string s)
		{
			return new LocalizedString("DialPlanAssociatedWithServerException", "Ex3A40FB", false, true, Strings.ResourceManager, new object[]
			{
				s
			});
		}

		public static LocalizedString ConfirmationMessageTestUMConnectivityTUILocalLoop
		{
			get
			{
				return new LocalizedString("ConfirmationMessageTestUMConnectivityTUILocalLoop", "Ex4E3109", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString CurrentTimeZoneIdNotFound
		{
			get
			{
				return new LocalizedString("CurrentTimeZoneIdNotFound", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ConfirmationMessageRemoveUMPublishingPoint(string shareName, string hostName)
		{
			return new LocalizedString("ConfirmationMessageRemoveUMPublishingPoint", "Ex8E8159", false, true, Strings.ResourceManager, new object[]
			{
				shareName,
				hostName
			});
		}

		public static LocalizedString ConfirmationMessageExportUMPromptDialPlanPrompts(string Path, string UMDialPlan)
		{
			return new LocalizedString("ConfirmationMessageExportUMPromptDialPlanPrompts", "ExEDDCE6", false, true, Strings.ResourceManager, new object[]
			{
				Path,
				UMDialPlan
			});
		}

		public static LocalizedString MultipleServersWithSameId(string s)
		{
			return new LocalizedString("MultipleServersWithSameId", "ExDF8F7B", false, true, Strings.ResourceManager, new object[]
			{
				s
			});
		}

		public static LocalizedString RemovePublishingPointException(string shareName, string moreInfo)
		{
			return new LocalizedString("RemovePublishingPointException", "", false, false, Strings.ResourceManager, new object[]
			{
				shareName,
				moreInfo
			});
		}

		public static LocalizedString ConfirmationMessageRemoveUMMailboxPrompt(string Identity)
		{
			return new LocalizedString("ConfirmationMessageRemoveUMMailboxPrompt", "Ex6C326C", false, true, Strings.ResourceManager, new object[]
			{
				Identity
			});
		}

		public static LocalizedString UnableToCreateGatewayObjectException(string msg)
		{
			return new LocalizedString("UnableToCreateGatewayObjectException", "ExCD819E", false, true, Strings.ResourceManager, new object[]
			{
				msg
			});
		}

		public static LocalizedString ConfirmationMessageEnableUMServer(string Identity)
		{
			return new LocalizedString("ConfirmationMessageEnableUMServer", "ExFBC3B9", false, true, Strings.ResourceManager, new object[]
			{
				Identity
			});
		}

		public static LocalizedString DialPlanChangeException(string s)
		{
			return new LocalizedString("DialPlanChangeException", "", false, false, Strings.ResourceManager, new object[]
			{
				s
			});
		}

		public static LocalizedString InvalidServerVersionInDialPlan(string dialplan)
		{
			return new LocalizedString("InvalidServerVersionInDialPlan", "ExAFF016", false, true, Strings.ResourceManager, new object[]
			{
				dialplan
			});
		}

		public static LocalizedString ErrorServerNotUnique(object idStringValue)
		{
			return new LocalizedString("ErrorServerNotUnique", "", false, false, Strings.ResourceManager, new object[]
			{
				idStringValue
			});
		}

		public static LocalizedString GetPINInfoError(string user, LocalizedString reason)
		{
			return new LocalizedString("GetPINInfoError", "Ex2AA5F2", false, true, Strings.ResourceManager, new object[]
			{
				user,
				reason
			});
		}

		public static LocalizedString ConfirmationMessageNewUMAutoAttendant(string Name, string PilotIdentifierList, string UMDialPlan)
		{
			return new LocalizedString("ConfirmationMessageNewUMAutoAttendant", "ExC3F82B", false, true, Strings.ResourceManager, new object[]
			{
				Name,
				PilotIdentifierList,
				UMDialPlan
			});
		}

		public static LocalizedString InvalidDtmfFallbackAutoAttendantSelf(string s)
		{
			return new LocalizedString("InvalidDtmfFallbackAutoAttendantSelf", "", false, false, Strings.ResourceManager, new object[]
			{
				s
			});
		}

		public static LocalizedString ExceptionNumericArgumentLengthInvalid(string value, string argument, int maxSize)
		{
			return new LocalizedString("ExceptionNumericArgumentLengthInvalid", "", false, false, Strings.ResourceManager, new object[]
			{
				value,
				argument,
				maxSize
			});
		}

		public static LocalizedString AttemptingToStampFQDN
		{
			get
			{
				return new LocalizedString("AttemptingToStampFQDN", "Ex789BB0", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString PINResetSuccessful(string s)
		{
			return new LocalizedString("PINResetSuccessful", "Ex246F83", false, true, Strings.ResourceManager, new object[]
			{
				s
			});
		}

		public static LocalizedString InvalidTimeZone(string tzKeyName)
		{
			return new LocalizedString("InvalidTimeZone", "Ex095AE6", false, true, Strings.ResourceManager, new object[]
			{
				tzKeyName
			});
		}

		public static LocalizedString Pin
		{
			get
			{
				return new LocalizedString("Pin", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString CannotDisableAutoAttendant(string s)
		{
			return new LocalizedString("CannotDisableAutoAttendant", "", false, false, Strings.ResourceManager, new object[]
			{
				s
			});
		}

		public static LocalizedString SubmitWelcomeMailError(string user, LocalizedString reason)
		{
			return new LocalizedString("SubmitWelcomeMailError", "Ex7A9A91", false, true, Strings.ResourceManager, new object[]
			{
				user,
				reason
			});
		}

		public static LocalizedString NotifyEmail
		{
			get
			{
				return new LocalizedString("NotifyEmail", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ConfirmationMessageSetUMIPGateway(string Identity)
		{
			return new LocalizedString("ConfirmationMessageSetUMIPGateway", "Ex0A1D7E", false, true, Strings.ResourceManager, new object[]
			{
				Identity
			});
		}

		public static LocalizedString ExceptionHuntGroupAlreadyExists(string ipGateway, string pilotIdentifier)
		{
			return new LocalizedString("ExceptionHuntGroupAlreadyExists", "", false, false, Strings.ResourceManager, new object[]
			{
				ipGateway,
				pilotIdentifier
			});
		}

		public static LocalizedString CannotAddNonSipNameDialplanToCallRouter(string s)
		{
			return new LocalizedString("CannotAddNonSipNameDialplanToCallRouter", "", false, false, Strings.ResourceManager, new object[]
			{
				s
			});
		}

		public static LocalizedString CannotAddNonSipNameDialplan(string s)
		{
			return new LocalizedString("CannotAddNonSipNameDialplan", "", false, false, Strings.ResourceManager, new object[]
			{
				s
			});
		}

		public static LocalizedString ConfirmationMessageDisableUMMailbox(string Identity)
		{
			return new LocalizedString("ConfirmationMessageDisableUMMailbox", "Ex372C0B", false, true, Strings.ResourceManager, new object[]
			{
				Identity
			});
		}

		public static LocalizedString ConfirmationMessageEnableUMMailbox(string Identity, string UMMailboxPolicy)
		{
			return new LocalizedString("ConfirmationMessageEnableUMMailbox", "Ex5A93BD", false, true, Strings.ResourceManager, new object[]
			{
				Identity,
				UMMailboxPolicy
			});
		}

		public static LocalizedString UmServiceName
		{
			get
			{
				return new LocalizedString("UmServiceName", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString AAAlreadyEnabled
		{
			get
			{
				return new LocalizedString("AAAlreadyEnabled", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString StatusChangeException(string s)
		{
			return new LocalizedString("StatusChangeException", "", false, false, Strings.ResourceManager, new object[]
			{
				s
			});
		}

		public static LocalizedString UMMailboxPolicyIdNotFound(string id)
		{
			return new LocalizedString("UMMailboxPolicyIdNotFound", "Ex6386DA", false, true, Strings.ResourceManager, new object[]
			{
				id
			});
		}

		public static LocalizedString ShouldUpgradeObjectVersion(string objtype)
		{
			return new LocalizedString("ShouldUpgradeObjectVersion", "ExD7A795", false, true, Strings.ResourceManager, new object[]
			{
				objtype
			});
		}

		public static LocalizedString InvalidServerVersionInGateway(string gateway)
		{
			return new LocalizedString("InvalidServerVersionInGateway", "Ex5C141E", false, true, Strings.ResourceManager, new object[]
			{
				gateway
			});
		}

		public static LocalizedString CallNotAnsweredInTestUMConnectivityTask(string timeout)
		{
			return new LocalizedString("CallNotAnsweredInTestUMConnectivityTask", "", false, false, Strings.ResourceManager, new object[]
			{
				timeout
			});
		}

		public static LocalizedString PropertyNotSupportedOnLegacyObjectException(string user, string propname)
		{
			return new LocalizedString("PropertyNotSupportedOnLegacyObjectException", "Ex87FD02", false, true, Strings.ResourceManager, new object[]
			{
				user,
				propname
			});
		}

		public static LocalizedString TransferFromTCPtoTLSModeWarning
		{
			get
			{
				return new LocalizedString("TransferFromTCPtoTLSModeWarning", "Ex48BB9D", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ServiceNotStarted(string serviceName)
		{
			return new LocalizedString("ServiceNotStarted", "", false, false, Strings.ResourceManager, new object[]
			{
				serviceName
			});
		}

		public static LocalizedString GetLocalizedString(Strings.IDs key)
		{
			return new LocalizedString(Strings.stringIDs[(uint)key], Strings.ResourceManager, new object[0]);
		}

		private static Dictionary<uint, string> stringIDs = new Dictionary<uint, string>(100);

		private static ExchangeResourceManager ResourceManager = ExchangeResourceManager.GetResourceManager("Microsoft.Exchange.Management.Tasks.UM.Strings", typeof(Strings).GetTypeInfo().Assembly);

		public enum IDs : uint
		{
			ConfirmationMessageStartUMPhoneSession = 781067276U,
			InstallUMWebServiceTask = 3239495249U,
			InvalidMethodToDisableAA = 3779087329U,
			EmptyCountryOrRegionCode = 1689586553U,
			DefaultMailboxSettings = 1601426056U,
			ConfirmationMessageStopUMPhoneSession = 530191348U,
			InstallUmCallRouterTask = 3015522797U,
			CannotCreateHuntGroupForHostedSipDialPlan = 1999730358U,
			PasswordMailBody = 3303439652U,
			ExternalHostFqdnChanges = 987112185U,
			RemoteEndDisconnected = 3433711682U,
			SuccessfulLogonState = 887877668U,
			ValidCertRequiredForUM = 1970930248U,
			AttemptingToCreateHuntgroup = 454845832U,
			OperationSuccessful = 2238046577U,
			CallRouterTransferFromTLStoTCPModeWarning = 3572721584U,
			UmServiceNotInstalled = 505394326U,
			UMStartupModeChanges = 3094237923U,
			PromptProvisioningShareDescription = 2019001798U,
			UmCallRouterName = 3372841158U,
			UserProblem = 1223643872U,
			ConfigureGatewayToForwardCallsMsg = 648648092U,
			GatewayAddressRequiresFqdn = 939630945U,
			DNSEntryNotFound = 471142364U,
			ExceptionInvalidSipNameDomain = 3757387627U,
			UninstallUmCallRouterTask = 4150015982U,
			LogonError = 1600651549U,
			ConfirmationMessageDisableUMServerImmediately = 388296289U,
			GatewayIPAddressFamilyInconsistentException = 818779485U,
			ConfirmationMessageDisableUMServer = 2840915457U,
			NoMailboxServersFound = 1625624441U,
			SrtpWithoutTls = 1298209242U,
			ConfirmationMessageDisableUMIPGateway = 3114226637U,
			UninstallUmServiceTask = 2388018184U,
			UmServiceDescription = 2187708795U,
			DefaultMailboxRequiredWhenForwardTrue = 3944137335U,
			ConfirmationMessageTestUMConnectivityLocalLoop = 4164812292U,
			InvalidDefaultOutboundCallingLineId = 587456431U,
			ErrorGeneratingDefaultPassword = 193099536U,
			InvalidDTMFSequenceReceived = 3703544840U,
			UninstallUMWebServiceTask = 2970756378U,
			ADError = 1263770381U,
			NotMailboxServer = 48881282U,
			LanguagesNotPassed = 2410305798U,
			InstallUmServiceTask = 1691564973U,
			WaitForFirstDiagnosticResponse = 2718089772U,
			InvalidTimeZoneParameters = 940435338U,
			CertNotFound = 1355103499U,
			PilotNumberState = 2010047074U,
			KeepProperties = 1658738722U,
			WaitForDiagnosticResponse = 2919065030U,
			UCMAPreReqException = 2675709228U,
			DialPlanAssociatedWithPoliciesException = 2660011992U,
			PinExpired = 1878527290U,
			LockedOut = 1151155524U,
			GatewayFqdnNotInAcceptedDomain = 100754582U,
			NoDTMFSwereReceived = 918425027U,
			PasswordMailSubject = 1885509224U,
			InvalidIPAddressReceived = 2380514321U,
			InvalidALParameterException = 943497584U,
			MustSpecifyThumbprint = 3527640475U,
			InvalidMailboxServerVersionForTUMCTask = 3746187495U,
			CannotCreateGatewayForHostedSipDialPlan = 886805380U,
			ConfirmationMessageDisableUMIPGatewayImmediately = 2203312157U,
			PilotNumber = 1602032641U,
			AttemptingToCreateIPGateway = 1762496243U,
			ExceptionUserNotAllowedForUMEnabled = 505861453U,
			ExchangePrincipalError = 305424905U,
			InvalidExternalHostFqdn = 2568008289U,
			UCMAPreReqUpgradeException = 912893922U,
			AADisableConfirmationString = 4252462372U,
			AAAlreadyDisabled = 3860731788U,
			ConfirmationMessageTestUMConnectivityPinReset = 3372908061U,
			CertWithoutTls = 1942717475U,
			SendEmail = 1061463472U,
			ExceptionSipResourceIdNotUnique = 3845881662U,
			PortChanges = 55026498U,
			AANameTooLong = 577419765U,
			DefaultUMHuntGroupName = 1006009848U,
			CouldnotRetreivePasswd = 1694996750U,
			PINEnterState = 1731989106U,
			BusinessHoursSettings = 3858123826U,
			UmServiceStillInstalled = 2511055751U,
			ConfirmationMessageSetUmCallRouterSettings = 1003963056U,
			ValidCertRequiredForUMCallRouter = 160860353U,
			DialPlanAssociatedWithUserException = 3650433099U,
			TransferFromTLStoTCPModeWarning = 73835935U,
			InvalidALParameter = 4163764725U,
			AfterHoursSettings = 154856458U,
			UmCallRouterDescription = 3666462471U,
			InvalidAutoAttendantScopeSetting = 3969155231U,
			TcpAndTlsPortsCannotBeSame = 3358569313U,
			ConfirmationMessageTestUMConnectivityTUILocalLoop = 3177175916U,
			CurrentTimeZoneIdNotFound = 2342320894U,
			AttemptingToStampFQDN = 975840932U,
			Pin = 339800695U,
			NotifyEmail = 4175977607U,
			UmServiceName = 1074457952U,
			AAAlreadyEnabled = 2152868767U,
			TransferFromTCPtoTLSModeWarning = 1304023191U
		}

		private enum ParamIDs
		{
			DisabledLinkedAutoAttendant,
			MultipleAutoAttendantsWithSameId,
			UMServiceDisabledException,
			SourceFileOpenException,
			InvalidDtmfFallbackAutoAttendant,
			MultipleDialplansWithSameId,
			ConfirmationMessageExportUMMailboxPrompt,
			CannotDisableAutoAttendant_KeyMapping,
			ExceptionSIPResouceIdConflictWithExistingValue,
			PINResetfailedToResetPin,
			RpcNotRegistered,
			InvalidUMServerStateOperationException,
			ErrorUMInvalidExtensionFormat,
			SavePINError,
			AutoAttendantAlreadDisabledException,
			UnableToSetMSSRegistryValue,
			ConfirmationMessageEnableUMAutoAttendant,
			DialPlanAssociatedWithIPGatewayException,
			TUILogonSuccessful,
			SIPFEServerConfigurationNotFound,
			ConfirmationMessageCopyUMCustomPromptDownloadAutoAttendantPrompts,
			AutoAttendantEnabledEvent,
			DPLinkedGwNotFQDN,
			ExceptionIPGatewayAlreadyExists,
			ConfirmationMessageExportUMPromptAutoAttendantPrompts,
			InvalidLanguageIdException,
			InvalidAutoAttendant,
			RpcUnavailable,
			FailedToEstablishMedia,
			IPGatewayAlreadEnabledException,
			DefaultLanguageNotAvailable,
			MismatchedOrgInDPAndGW,
			ConfirmationMessageRemoveUMHuntGroup,
			ConfirmationMessageNewUMDialPlan,
			MaxAsrPhraseLengthExceeded,
			ErrorOrganizationNotUnique,
			OperationNotSupportedOnLegacMailboxException,
			SipOptionsError,
			ConfirmationMessageSetUmServer,
			NonExistantServer,
			ErrorContactAddressListNotUnique,
			DefaultUMMailboxPolicyName,
			DefaultPolicyCreationNameTooLong,
			CallAnsweringRuleNotFoundException,
			EmptyASRPhrase,
			ConfirmationMessageNewUMIPGateway,
			NonExistantDialPlan,
			ConfirmationMessageRemoveUMAutoAttendant,
			UMServerAlreadDisabledException,
			AutoAttendantDisabledEvent,
			MakeCallError,
			ChangesTakeEffectAfterRestartingUmServer,
			ServerIsPublishingPointException,
			AutoAttendantAlreadEnabledException,
			AutoAttendantAlreadyExistsException,
			ConfirmationMessageRemoveUMDialPlan,
			DiagnosticSequence,
			TUILogonfailedToMakeCall,
			MultipleUMMailboxPolicyWithSameId,
			ConfirmationMessageRemoveUMIPGateway,
			ErrorUMInvalidSipNameAddressFormat,
			NonExistantIPGateway,
			ErrorContactAddressListNotFound,
			DnsResolutionError,
			ErrorObjectNotFound,
			ErrorWeakPasswordHistorySingular,
			ConfirmationMessageNewUMHuntGroup,
			ExceptionUserAlreadyUmEnabled,
			ResetUMMailboxError,
			InvalidAutoAttendantInDialPlan,
			ErrorUMInvalidE164AddressFormat,
			SendPINResetMailError,
			Confirm,
			PINResetfailedToResetPasswd,
			ErrorWeakPasswordNoHistory,
			InvalidUMUser,
			ErrorUMInvalidSipNameDomain,
			InvalidServerVersionForUMRpcTask,
			MultipleIPGatewaysWithSameId,
			DuplicateMenuName,
			FirewallCorrectlyConfigured,
			ChangesTakeEffectAfterRestartingUmCallRouterService,
			ScopeErrorOnAutoAttendant,
			DefaultPolicyCreation,
			ConfirmationMessageCopyUMCustomPromptDownloadDialPlanPrompts,
			DialPlanAssociatedWithAutoAttendantException,
			ExceptionUserAlreadyUmDisabled,
			AADisableWhatifString,
			InitUMMailboxError,
			SipUriError,
			ConfirmationMessageCopyUMCustomPromptUploadAutoAttendantPrompts,
			InvalidMailbox,
			ConfirmationMessageExportUMCallDataRecord,
			UMMailboxPolicyNotPresent,
			InvalidDtmfChar,
			ErrorWeakPasswordHistoryPlural,
			ExceptionIPGatewayIPAddressAlreadyExists,
			IPGatewayAlreadDisabledException,
			ErrorWeakPasswordWithNoCommonPatterns,
			MaxAsrPhraseCountExceeded,
			InvalidAAFileExtension,
			SendSequenceError,
			ConfirmationMessageImportUMPromptAutoAttendantPrompts,
			MailboxNotLocal,
			DuplicateASRPhrase,
			ErrorServerNotFound,
			ErrorWeakPassword,
			ConfirmationMessageSetUMMailboxPIN,
			InvalidIPGatewayStateOperationException,
			TUILogonfailedToLogon,
			DesktopExperienceRequiredException,
			NewPublishingPointException,
			ExceptionIPGatewayInvalid,
			ConfirmationMessageSetUMAutoAttendant,
			DuplicateE164PilotIdentifierListEntry,
			ErrorOrganizationalUnitNotUnique,
			ErrorUMPilotIdentifierInUse,
			ConfirmationMessageEnableUMIPGateway,
			ChangingMSSMaxDiskCacheSize,
			ConfirmationMessageImportUMPromptDialPlanPrompts,
			DuplicateKeys,
			DefaultAutoAttendantInDialPlanException,
			GenericRPCError,
			TopologyDiscoveryProblem,
			NonExistantAutoAttendant,
			MailboxMustBeSpecifiedException,
			InvalidSpeechEnabledAutoAttendant,
			InitializeError,
			OperationFailed,
			ValidateGeneratePINError,
			ErrorOrganizationNotFound,
			TUILogonfailedToGetPin,
			ErrorOrganizationalUnitNotFound,
			NotifyEmailPilotNumberField,
			ConfirmationMessageSetUMDialPlan,
			InvalidDtmfFallbackAutoAttendantDialPlan,
			ConfirmationMessageTestUMConnectivityEndToEnd,
			ConfirmationMessageSetUMMailboxConfiguration,
			InvalidUMUserName,
			ExceptionDialPlanNotFound,
			InvalidDtmfFallbackAutoAttendant_Disabled,
			OperationTimedOutInTestUMConnectivityTask,
			UMServerAlreadEnabledException,
			ConfirmationMessageDisableUMAutoAttendant,
			ConfirmationMessageCopyUMCustomPromptUploadDialPlanPrompts,
			DialPlanAssociatedWithServerException,
			ConfirmationMessageRemoveUMPublishingPoint,
			ConfirmationMessageExportUMPromptDialPlanPrompts,
			MultipleServersWithSameId,
			RemovePublishingPointException,
			ConfirmationMessageRemoveUMMailboxPrompt,
			UnableToCreateGatewayObjectException,
			ConfirmationMessageEnableUMServer,
			DialPlanChangeException,
			InvalidServerVersionInDialPlan,
			ErrorServerNotUnique,
			GetPINInfoError,
			ConfirmationMessageNewUMAutoAttendant,
			InvalidDtmfFallbackAutoAttendantSelf,
			ExceptionNumericArgumentLengthInvalid,
			PINResetSuccessful,
			InvalidTimeZone,
			CannotDisableAutoAttendant,
			SubmitWelcomeMailError,
			ConfirmationMessageSetUMIPGateway,
			ExceptionHuntGroupAlreadyExists,
			CannotAddNonSipNameDialplanToCallRouter,
			CannotAddNonSipNameDialplan,
			ConfirmationMessageDisableUMMailbox,
			ConfirmationMessageEnableUMMailbox,
			StatusChangeException,
			UMMailboxPolicyIdNotFound,
			ShouldUpgradeObjectVersion,
			InvalidServerVersionInGateway,
			CallNotAnsweredInTestUMConnectivityTask,
			PropertyNotSupportedOnLegacyObjectException,
			ServiceNotStarted
		}
	}
}
