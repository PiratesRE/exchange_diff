using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.DatacenterStrings
{
	internal static class Strings
	{
		static Strings()
		{
			Strings.stringIDs.Add(3442334529U, "BecErrorThrottling");
			Strings.stringIDs.Add(2921179735U, "ErrorSPFInvalidMemberName");
			Strings.stringIDs.Add(3165600047U, "BecAccessDenied");
			Strings.stringIDs.Add(534972034U, "BecIncorrectPassword");
			Strings.stringIDs.Add(2713690330U, "BecErrorNotFouond");
			Strings.stringIDs.Add(2548455642U, "ErrorManagedParameterSetPassedInForFederatedTenant");
			Strings.stringIDs.Add(457807320U, "IDSErrorMissingNamespaceIdNode");
			Strings.stringIDs.Add(3569539930U, "ErrorParameterIsIncorrect");
			Strings.stringIDs.Add(3398198740U, "ErrorCodeProfileDoesNotExists");
			Strings.stringIDs.Add(71190514U, "ErrorMemberLockedOutBecauseOfPasswordAttempts");
			Strings.stringIDs.Add(507263250U, "IDSErrorEmptyCredFlagsNode");
			Strings.stringIDs.Add(2457590018U, "ErrorPasswordMatchesAccountWithSameMemberName");
			Strings.stringIDs.Add(240550867U, "BecErrorDomainValidation");
			Strings.stringIDs.Add(2539075827U, "ErrorDomainDoesNotExist");
			Strings.stringIDs.Add(1628902957U, "ErrorSignInNameInCompleteOrInvalid");
			Strings.stringIDs.Add(746302685U, "ErrorUserBlocked");
			Strings.stringIDs.Add(2001068460U, "BecTransientError");
			Strings.stringIDs.Add(2511795258U, "ErrorFederatedParameterSetPassedInForManagedTenant");
			Strings.stringIDs.Add(2085485339U, "IDSErrorEmptyPuidNode");
			Strings.stringIDs.Add(1384815350U, "BecRedirection");
			Strings.stringIDs.Add(2259851215U, "InvalidNetId");
			Strings.stringIDs.Add(1908027994U, "ErrorNameContainsBlockedWord");
			Strings.stringIDs.Add(153681091U, "ErrorServiceUnavailableDueToInternalError");
			Strings.stringIDs.Add(1374323893U, "IDSErrorEmptyNetIdNode");
			Strings.stringIDs.Add(1890206562U, "ErrorSPFPasswordTooLong");
			Strings.stringIDs.Add(3875381480U, "BecErrorInvalidHeader");
			Strings.stringIDs.Add(2343187933U, "BecErrorUserExists");
			Strings.stringIDs.Add(1260215560U, "ErrorUserNameReserved");
			Strings.stringIDs.Add(286588848U, "ErrorFieldContainsInvalidCharacters");
			Strings.stringIDs.Add(2252511124U, "ErrorPasswordRequired");
			Strings.stringIDs.Add(3038881494U, "ErrorInputContainsForbiddenWord");
			Strings.stringIDs.Add(1170008863U, "ErrorFederatedAccountAlreadyExists");
			Strings.stringIDs.Add(2883915563U, "ErrorEmailNameContainsInvalidCharacters");
			Strings.stringIDs.Add(116822391U, "ErrorPasswordContainedInSQ");
			Strings.stringIDs.Add(3327865544U, "BecErrorInvalidContext");
			Strings.stringIDs.Add(2629176178U, "ErrorsRemovedMailboxHaveNoNetID");
			Strings.stringIDs.Add(3821098863U, "ErrorPasswordContainsInvalidCharacters");
			Strings.stringIDs.Add(24718424U, "ErrorInvalidPassportId");
			Strings.stringIDs.Add(3848807642U, "BecErrorQuota");
			Strings.stringIDs.Add(159495284U, "BecErrorInvalidLicense");
			Strings.stringIDs.Add(1626889520U, "ErrorSPFPasswordIsBlank");
			Strings.stringIDs.Add(4122120961U, "ErrorCannotRecoverLiveIdMismatchInstanceType");
			Strings.stringIDs.Add(2570238007U, "BecErrorSubscription");
			Strings.stringIDs.Add(2854433926U, "ErrorSecretAnswerContainsPassword");
			Strings.stringIDs.Add(344542475U, "ErrorWLCDInternal");
			Strings.stringIDs.Add(1112425574U, "ErrorManagedMemberExistsSPF");
			Strings.stringIDs.Add(2178303307U, "BecErrorUniquenessValidation");
			Strings.stringIDs.Add(3710614060U, "ErrorPasswordIsInvalid");
			Strings.stringIDs.Add(3791085944U, "BecErrorInvalidWeakPassword");
			Strings.stringIDs.Add(1520187672U, "ErrorIncompleteEmailAddress");
			Strings.stringIDs.Add(1810820280U, "ErrorSecretQuestionContainsPassword");
			Strings.stringIDs.Add(561051570U, "BecUserInRecycleState");
			Strings.stringIDs.Add(854137693U, "ErrorFederatedParameterSetPassedInForManagedNamespace");
			Strings.stringIDs.Add(3194642979U, "InternalError");
			Strings.stringIDs.Add(764417362U, "ErrorMemberNameAndFederatedIdentityNotMatch");
			Strings.stringIDs.Add(1746286164U, "ErrorArchiveOnly");
			Strings.stringIDs.Add(2776399702U, "BecErrorSyntaxValidation");
			Strings.stringIDs.Add(2366837181U, "IDSErrorEmptyNamespaceIdNode");
			Strings.stringIDs.Add(2439548284U, "ErrorMemberIsLocked");
			Strings.stringIDs.Add(1968969488U, "ErrorSecretQuestionContainsMemberName");
			Strings.stringIDs.Add(370795085U, "BecErrorAccountDisabled");
			Strings.stringIDs.Add(1787101328U, "BecErrorInvalidPassword");
			Strings.stringIDs.Add(2743094604U, "ErrorCannotRenameCredentialToSameName");
			Strings.stringIDs.Add(2646378639U, "ErrorEnableRoomMailboxAccountParameterFalseInDatacenter");
			Strings.stringIDs.Add(3978792718U, "ErrorAccountIsNotEASI");
			Strings.stringIDs.Add(1953245018U, "NotAuthorized");
			Strings.stringIDs.Add(888348176U, "WindowsLiveIdProvisioningHandlerException");
			Strings.stringIDs.Add(4169169063U, "OrganizationIsReadOnly");
			Strings.stringIDs.Add(1190886676U, "ErrorEmailNameStartAfterDot");
			Strings.stringIDs.Add(3074197328U, "ErrorNamespaceNotFound");
			Strings.stringIDs.Add(3693389389U, "BecIdentityInternalError");
			Strings.stringIDs.Add(2927230868U, "BecCompanyNotFound");
			Strings.stringIDs.Add(2733812604U, "ErrorNoRecordInDatabase");
			Strings.stringIDs.Add(2692387272U, "BecErrorPropertyNotSettable");
			Strings.stringIDs.Add(2602850872U, "ErrorEmailAddressTooLong");
			Strings.stringIDs.Add(1479442559U, "BecErrorServiceUnavailable");
			Strings.stringIDs.Add(2501723808U, "IDSErrorEmptySignInNamesForNetId");
			Strings.stringIDs.Add(4277226160U, "ErrorUserAlreadyInactive");
			Strings.stringIDs.Add(3142877869U, "BecErrorTooManyMappedTenants");
			Strings.stringIDs.Add(1684749577U, "BecErrorStringLength");
			Strings.stringIDs.Add(509547158U, "ErrorPartnerNotAuthorized");
			Strings.stringIDs.Add(1770558589U, "ErrorManagedParameterSetPassedInForFederatedNamespace");
			Strings.stringIDs.Add(312656184U, "BecDirectoryInternalError");
			Strings.stringIDs.Add(364820960U, "BecUnknownError");
			Strings.stringIDs.Add(829279753U, "ErrorPasswordContainsLastName");
			Strings.stringIDs.Add(3970665479U, "BecInternalError");
			Strings.stringIDs.Add(938622763U, "ErrorDomainIsManaged");
			Strings.stringIDs.Add(3197989195U, "ErrorPasswordContainsMemberName");
			Strings.stringIDs.Add(706972491U, "ErrorPartnerCannotModifyProtectedField");
			Strings.stringIDs.Add(1463063580U, "ErrorMissingNetIDWhenBypassWLID");
			Strings.stringIDs.Add(3151258897U, "ErrorEmailStartsAndEndsWithDot");
			Strings.stringIDs.Add(524928998U, "ErrorCodeInvalidNetId");
			Strings.stringIDs.Add(146893062U, "ErrorMemberExists");
			Strings.stringIDs.Add(1316852426U, "BecErrorRequiredProperty");
			Strings.stringIDs.Add(2872232368U, "ErrorInvalidBrandData");
			Strings.stringIDs.Add(3566184850U, "ErrorCanNotSetPasswordOrResetOnFederatedAccount");
			Strings.stringIDs.Add(840675049U, "ErrorSignInNameTooLong");
			Strings.stringIDs.Add(2454442314U, "ErrorNamespaceDoesNotExist");
			Strings.stringIDs.Add(2655490445U, "IDSErrorMissingCredFlagsNode");
			Strings.stringIDs.Add(3197291445U, "ErrorNonEASIMemberExists");
			Strings.stringIDs.Add(1384046745U, "ErrorPasswordContainsFirstName");
			Strings.stringIDs.Add(1475855582U, "ErrorDomainNameIsNull");
			Strings.stringIDs.Add(1895268805U, "BecErrorQuotaExceeded");
			Strings.stringIDs.Add(3873716657U, "ErrorDatabaseOnMaintenance");
			Strings.stringIDs.Add(3937658635U, "ErrorLiveIdWithFederatedIdentityExists");
			Strings.stringIDs.Add(2857272973U, "ErrorSignInNameTooShort");
			Strings.stringIDs.Add(173165206U, "ErrorSPFPasswordTooShort");
		}

		public static LocalizedString BecErrorThrottling
		{
			get
			{
				return new LocalizedString("BecErrorThrottling", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorSPFInvalidMemberName
		{
			get
			{
				return new LocalizedString("ErrorSPFInvalidMemberName", "Ex0D2954", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorCannotChangeMemberFederationState(string act, string liveId, string oldDomain, string newDomain)
		{
			return new LocalizedString("ErrorCannotChangeMemberFederationState", "ExB66C30", false, true, Strings.ResourceManager, new object[]
			{
				act,
				liveId,
				oldDomain,
				newDomain
			});
		}

		public static LocalizedString ErrorManagedMemberExists(string memberName)
		{
			return new LocalizedString("ErrorManagedMemberExists", "ExBD5DC4", false, true, Strings.ResourceManager, new object[]
			{
				memberName
			});
		}

		public static LocalizedString BecAccessDenied
		{
			get
			{
				return new LocalizedString("BecAccessDenied", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString BecIncorrectPassword
		{
			get
			{
				return new LocalizedString("BecIncorrectPassword", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorEvictLiveIdMemberExists(string memberName)
		{
			return new LocalizedString("ErrorEvictLiveIdMemberExists", "Ex55F706", false, true, Strings.ResourceManager, new object[]
			{
				memberName
			});
		}

		public static LocalizedString BecErrorNotFouond
		{
			get
			{
				return new LocalizedString("BecErrorNotFouond", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorManagedParameterSetPassedInForFederatedTenant
		{
			get
			{
				return new LocalizedString("ErrorManagedParameterSetPassedInForFederatedTenant", "Ex62E9A2", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorPasswordIncludesMemberName(string memberName)
		{
			return new LocalizedString("ErrorPasswordIncludesMemberName", "ExEEF0F9", false, true, Strings.ResourceManager, new object[]
			{
				memberName
			});
		}

		public static LocalizedString ErrorEvictLiveIdMemberNotExists(string memberName)
		{
			return new LocalizedString("ErrorEvictLiveIdMemberNotExists", "Ex47077E", false, true, Strings.ResourceManager, new object[]
			{
				memberName
			});
		}

		public static LocalizedString IDSErrorUnexpectedResultsForGetProfileByAttributes(string memberName, string length)
		{
			return new LocalizedString("IDSErrorUnexpectedResultsForGetProfileByAttributes", "Ex60C917", false, true, Strings.ResourceManager, new object[]
			{
				memberName,
				length
			});
		}

		public static LocalizedString IDSErrorMissingNamespaceIdNode
		{
			get
			{
				return new LocalizedString("IDSErrorMissingNamespaceIdNode", "Ex922F62", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorParameterIsIncorrect
		{
			get
			{
				return new LocalizedString("ErrorParameterIsIncorrect", "Ex3FD29F", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorCodeProfileDoesNotExists
		{
			get
			{
				return new LocalizedString("ErrorCodeProfileDoesNotExists", "ExE5AB42", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorMemberLockedOutBecauseOfPasswordAttempts
		{
			get
			{
				return new LocalizedString("ErrorMemberLockedOutBecauseOfPasswordAttempts", "ExC7BD17", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString IDSErrorEmptyCredFlagsNode
		{
			get
			{
				return new LocalizedString("IDSErrorEmptyCredFlagsNode", "Ex8AFBBB", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorMemberNameUnavailableUsedForEASI(string memberName)
		{
			return new LocalizedString("ErrorMemberNameUnavailableUsedForEASI", "Ex8F202B", false, true, Strings.ResourceManager, new object[]
			{
				memberName
			});
		}

		public static LocalizedString ErrorPasswordMatchesAccountWithSameMemberName
		{
			get
			{
				return new LocalizedString("ErrorPasswordMatchesAccountWithSameMemberName", "Ex7B4616", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString BecErrorDomainValidation
		{
			get
			{
				return new LocalizedString("BecErrorDomainValidation", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorDomainDoesNotExist
		{
			get
			{
				return new LocalizedString("ErrorDomainDoesNotExist", "Ex8C4E74", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorPasswordIncludesInvalidChars(string memberName)
		{
			return new LocalizedString("ErrorPasswordIncludesInvalidChars", "ExAEADDF", false, true, Strings.ResourceManager, new object[]
			{
				memberName
			});
		}

		public static LocalizedString ErrorSignInNameInCompleteOrInvalid
		{
			get
			{
				return new LocalizedString("ErrorSignInNameInCompleteOrInvalid", "Ex36606C", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorUserBlocked
		{
			get
			{
				return new LocalizedString("ErrorUserBlocked", "ExD8AEE7", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString BecTransientError
		{
			get
			{
				return new LocalizedString("BecTransientError", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString VerboseEvictMember(string memberName)
		{
			return new LocalizedString("VerboseEvictMember", "Ex6058C9", false, true, Strings.ResourceManager, new object[]
			{
				memberName
			});
		}

		public static LocalizedString ErrorUnknown(string memberName)
		{
			return new LocalizedString("ErrorUnknown", "ExD7EB39", false, true, Strings.ResourceManager, new object[]
			{
				memberName
			});
		}

		public static LocalizedString ErrorFederatedParameterSetPassedInForManagedTenant
		{
			get
			{
				return new LocalizedString("ErrorFederatedParameterSetPassedInForManagedTenant", "Ex9C2EB5", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString IDSErrorEmptyPuidNode
		{
			get
			{
				return new LocalizedString("IDSErrorEmptyPuidNode", "Ex71D3CC", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorCannotUseReservedLiveId(string windowsLiveId)
		{
			return new LocalizedString("ErrorCannotUseReservedLiveId", "Ex8B95DC", false, true, Strings.ResourceManager, new object[]
			{
				windowsLiveId
			});
		}

		public static LocalizedString BecRedirection
		{
			get
			{
				return new LocalizedString("BecRedirection", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InvalidNetId
		{
			get
			{
				return new LocalizedString("InvalidNetId", "Ex76CC69", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorNameContainsBlockedWord
		{
			get
			{
				return new LocalizedString("ErrorNameContainsBlockedWord", "Ex3056EF", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorServiceUnavailableDueToInternalError
		{
			get
			{
				return new LocalizedString("ErrorServiceUnavailableDueToInternalError", "Ex276B63", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString IDSErrorEmptyNetIdNode
		{
			get
			{
				return new LocalizedString("IDSErrorEmptyNetIdNode", "ExF49CA6", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorSPFPasswordTooLong
		{
			get
			{
				return new LocalizedString("ErrorSPFPasswordTooLong", "Ex9CFCE7", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString BecErrorInvalidHeader
		{
			get
			{
				return new LocalizedString("BecErrorInvalidHeader", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorWindowsLiveIdRequired(string user)
		{
			return new LocalizedString("ErrorWindowsLiveIdRequired", "Ex63E9B9", false, true, Strings.ResourceManager, new object[]
			{
				user
			});
		}

		public static LocalizedString BecErrorUserExists
		{
			get
			{
				return new LocalizedString("BecErrorUserExists", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorUserNameReserved
		{
			get
			{
				return new LocalizedString("ErrorUserNameReserved", "Ex1F6050", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorFieldContainsInvalidCharacters
		{
			get
			{
				return new LocalizedString("ErrorFieldContainsInvalidCharacters", "Ex60F178", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorRedirectionEntryManagerException(string message)
		{
			return new LocalizedString("ErrorRedirectionEntryManagerException", "Ex0DD6C6", false, true, Strings.ResourceManager, new object[]
			{
				message
			});
		}

		public static LocalizedString ErrorPasswordRequired
		{
			get
			{
				return new LocalizedString("ErrorPasswordRequired", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorInputContainsForbiddenWord
		{
			get
			{
				return new LocalizedString("ErrorInputContainsForbiddenWord", "Ex357ED3", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorFederatedAccountAlreadyExists
		{
			get
			{
				return new LocalizedString("ErrorFederatedAccountAlreadyExists", "Ex24CDBC", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorEmailNameContainsInvalidCharacters
		{
			get
			{
				return new LocalizedString("ErrorEmailNameContainsInvalidCharacters", "Ex7DBC42", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorCannotImportForNamespaceType(string memberName)
		{
			return new LocalizedString("ErrorCannotImportForNamespaceType", "Ex1FFDD4", false, true, Strings.ResourceManager, new object[]
			{
				memberName
			});
		}

		public static LocalizedString ErrorPasswordContainedInSQ
		{
			get
			{
				return new LocalizedString("ErrorPasswordContainedInSQ", "ExEE8BF8", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorUnmanagedMemberNotExists(string memberName)
		{
			return new LocalizedString("ErrorUnmanagedMemberNotExists", "ExE08325", false, true, Strings.ResourceManager, new object[]
			{
				memberName
			});
		}

		public static LocalizedString ErrorMemberNameUnavailableUsedForDL(string memberName)
		{
			return new LocalizedString("ErrorMemberNameUnavailableUsedForDL", "Ex28E0C9", false, true, Strings.ResourceManager, new object[]
			{
				memberName
			});
		}

		public static LocalizedString BecErrorInvalidContext
		{
			get
			{
				return new LocalizedString("BecErrorInvalidContext", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorsRemovedMailboxHaveNoNetID
		{
			get
			{
				return new LocalizedString("ErrorsRemovedMailboxHaveNoNetID", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorLiveIdDoesNotExist(string windowsLiveId)
		{
			return new LocalizedString("ErrorLiveIdDoesNotExist", "Ex20CA8B", false, true, Strings.ResourceManager, new object[]
			{
				windowsLiveId
			});
		}

		public static LocalizedString ErrorPasswordContainsInvalidCharacters
		{
			get
			{
				return new LocalizedString("ErrorPasswordContainsInvalidCharacters", "Ex46338B", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString IDSErrorUnexpectedResultsForCreatePassports(string memberName, string length)
		{
			return new LocalizedString("IDSErrorUnexpectedResultsForCreatePassports", "ExF34284", false, true, Strings.ResourceManager, new object[]
			{
				memberName,
				length
			});
		}

		public static LocalizedString ErrorConfigurationUnitNotFound(string configunit)
		{
			return new LocalizedString("ErrorConfigurationUnitNotFound", "Ex2F6EA5", false, true, Strings.ResourceManager, new object[]
			{
				configunit
			});
		}

		public static LocalizedString ErrorInvalidPassportId
		{
			get
			{
				return new LocalizedString("ErrorInvalidPassportId", "Ex679B02", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString BecErrorQuota
		{
			get
			{
				return new LocalizedString("BecErrorQuota", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorPasswordTooShort(string memberName)
		{
			return new LocalizedString("ErrorPasswordTooShort", "ExDC236B", false, true, Strings.ResourceManager, new object[]
			{
				memberName
			});
		}

		public static LocalizedString BecErrorInvalidLicense
		{
			get
			{
				return new LocalizedString("BecErrorInvalidLicense", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorSPFPasswordIsBlank
		{
			get
			{
				return new LocalizedString("ErrorSPFPasswordIsBlank", "Ex8A7A81", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorCannotRemoveWindowsLiveIDFromProxyAddresses(string windowsLiveId)
		{
			return new LocalizedString("ErrorCannotRemoveWindowsLiveIDFromProxyAddresses", "ExBE4789", false, true, Strings.ResourceManager, new object[]
			{
				windowsLiveId
			});
		}

		public static LocalizedString ErrorCannotRecoverLiveIdMismatchInstanceType
		{
			get
			{
				return new LocalizedString("ErrorCannotRecoverLiveIdMismatchInstanceType", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorGuidNotParsable(string propertyName)
		{
			return new LocalizedString("ErrorGuidNotParsable", "", false, false, Strings.ResourceManager, new object[]
			{
				propertyName
			});
		}

		public static LocalizedString BecErrorSubscription
		{
			get
			{
				return new LocalizedString("BecErrorSubscription", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorSecretAnswerContainsPassword
		{
			get
			{
				return new LocalizedString("ErrorSecretAnswerContainsPassword", "Ex73E2A4", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorImportLiveIdManagedMemberExists(string memberName)
		{
			return new LocalizedString("ErrorImportLiveIdManagedMemberExists", "ExED7DB1", false, true, Strings.ResourceManager, new object[]
			{
				memberName
			});
		}

		public static LocalizedString ErrorWLCDInternal
		{
			get
			{
				return new LocalizedString("ErrorWLCDInternal", "Ex01378C", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorManagedMemberDoesNotExistForByolid(string memberName)
		{
			return new LocalizedString("ErrorManagedMemberDoesNotExistForByolid", "Ex22C8A9", false, true, Strings.ResourceManager, new object[]
			{
				memberName
			});
		}

		public static LocalizedString ErrorCannotRenameAccrossNamespaceTypes(string domain1, string domain2)
		{
			return new LocalizedString("ErrorCannotRenameAccrossNamespaceTypes", "Ex71D6DB", false, true, Strings.ResourceManager, new object[]
			{
				domain1,
				domain2
			});
		}

		public static LocalizedString ErrorManagedMemberExistsSPF
		{
			get
			{
				return new LocalizedString("ErrorManagedMemberExistsSPF", "ExC0D31D", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString BecErrorUniquenessValidation
		{
			get
			{
				return new LocalizedString("BecErrorUniquenessValidation", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorPasswordIsInvalid
		{
			get
			{
				return new LocalizedString("ErrorPasswordIsInvalid", "Ex33980C", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString OrganizationIsImmutable(string cu)
		{
			return new LocalizedString("OrganizationIsImmutable", "", false, false, Strings.ResourceManager, new object[]
			{
				cu
			});
		}

		public static LocalizedString BecErrorInvalidWeakPassword
		{
			get
			{
				return new LocalizedString("BecErrorInvalidWeakPassword", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorCannotDetermineLiveInstance(string identity)
		{
			return new LocalizedString("ErrorCannotDetermineLiveInstance", "Ex756F8E", false, true, Strings.ResourceManager, new object[]
			{
				identity
			});
		}

		public static LocalizedString ErrorIncompleteEmailAddress
		{
			get
			{
				return new LocalizedString("ErrorIncompleteEmailAddress", "Ex9AA701", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorSecretQuestionContainsPassword
		{
			get
			{
				return new LocalizedString("ErrorSecretQuestionContainsPassword", "ExCBA529", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorFailedToEvictMember(string memberName, string message)
		{
			return new LocalizedString("ErrorFailedToEvictMember", "Ex4E574D", false, true, Strings.ResourceManager, new object[]
			{
				memberName,
				message
			});
		}

		public static LocalizedString BecUserInRecycleState
		{
			get
			{
				return new LocalizedString("BecUserInRecycleState", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorParameterRequired(string parameterName)
		{
			return new LocalizedString("ErrorParameterRequired", "Ex7A03BB", false, true, Strings.ResourceManager, new object[]
			{
				parameterName
			});
		}

		public static LocalizedString ErrorOnGetProfile(string netId, string errorCode)
		{
			return new LocalizedString("ErrorOnGetProfile", "ExB02F66", false, true, Strings.ResourceManager, new object[]
			{
				netId,
				errorCode
			});
		}

		public static LocalizedString ErrorFederatedParameterSetPassedInForManagedNamespace
		{
			get
			{
				return new LocalizedString("ErrorFederatedParameterSetPassedInForManagedNamespace", "Ex821DDF", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InternalError
		{
			get
			{
				return new LocalizedString("InternalError", "Ex3E461F", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorMemberNameAndFederatedIdentityNotMatch
		{
			get
			{
				return new LocalizedString("ErrorMemberNameAndFederatedIdentityNotMatch", "ExE6878D", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorArchiveOnly
		{
			get
			{
				return new LocalizedString("ErrorArchiveOnly", "Ex9EC78C", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString BecErrorSyntaxValidation
		{
			get
			{
				return new LocalizedString("BecErrorSyntaxValidation", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString IDSErrorEmptyNamespaceIdNode
		{
			get
			{
				return new LocalizedString("IDSErrorEmptyNamespaceIdNode", "Ex0728C0", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorMemberIsLocked
		{
			get
			{
				return new LocalizedString("ErrorMemberIsLocked", "Ex776892", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorSecretQuestionContainsMemberName
		{
			get
			{
				return new LocalizedString("ErrorSecretQuestionContainsMemberName", "Ex24C70E", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString BecErrorAccountDisabled
		{
			get
			{
				return new LocalizedString("BecErrorAccountDisabled", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorMemberNameBlocked(string memberName)
		{
			return new LocalizedString("ErrorMemberNameBlocked", "Ex6221CE", false, true, Strings.ResourceManager, new object[]
			{
				memberName
			});
		}

		public static LocalizedString BecErrorInvalidPassword
		{
			get
			{
				return new LocalizedString("BecErrorInvalidPassword", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString SPFInternalError(string additionalMessage)
		{
			return new LocalizedString("SPFInternalError", "Ex026526", false, true, Strings.ResourceManager, new object[]
			{
				additionalMessage
			});
		}

		public static LocalizedString ErrorCannotRenameCredentialToSameName
		{
			get
			{
				return new LocalizedString("ErrorCannotRenameCredentialToSameName", "ExFEC65C", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorEnableRoomMailboxAccountParameterFalseInDatacenter
		{
			get
			{
				return new LocalizedString("ErrorEnableRoomMailboxAccountParameterFalseInDatacenter", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorAccountIsNotEASI
		{
			get
			{
				return new LocalizedString("ErrorAccountIsNotEASI", "Ex5CD86F", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString NotAuthorized
		{
			get
			{
				return new LocalizedString("NotAuthorized", "Ex4036C4", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString WindowsLiveIdProvisioningHandlerException
		{
			get
			{
				return new LocalizedString("WindowsLiveIdProvisioningHandlerException", "ExEDB8A7", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString OrganizationIsReadOnly
		{
			get
			{
				return new LocalizedString("OrganizationIsReadOnly", "ExF12C78", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorEmailNameStartAfterDot
		{
			get
			{
				return new LocalizedString("ErrorEmailNameStartAfterDot", "Ex79E561", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorMemberNameUnavailable(string memberName)
		{
			return new LocalizedString("ErrorMemberNameUnavailable", "Ex4070F8", false, true, Strings.ResourceManager, new object[]
			{
				memberName
			});
		}

		public static LocalizedString ErrorNamespaceNotFound
		{
			get
			{
				return new LocalizedString("ErrorNamespaceNotFound", "ExF61989", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString BecIdentityInternalError
		{
			get
			{
				return new LocalizedString("BecIdentityInternalError", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorUnmanagedMemberExists(string memberName)
		{
			return new LocalizedString("ErrorUnmanagedMemberExists", "Ex312482", false, true, Strings.ResourceManager, new object[]
			{
				memberName
			});
		}

		public static LocalizedString BecCompanyNotFound
		{
			get
			{
				return new LocalizedString("BecCompanyNotFound", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString AdditionalDebugInfo(string request, string response, string info)
		{
			return new LocalizedString("AdditionalDebugInfo", "ExB01114", false, true, Strings.ResourceManager, new object[]
			{
				request,
				response,
				info
			});
		}

		public static LocalizedString ErrorNoRecordInDatabase
		{
			get
			{
				return new LocalizedString("ErrorNoRecordInDatabase", "ExCFCC64", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString BecErrorPropertyNotSettable
		{
			get
			{
				return new LocalizedString("BecErrorPropertyNotSettable", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorMaxMembershipLimit(string memberName)
		{
			return new LocalizedString("ErrorMaxMembershipLimit", "ExCC8AE5", false, true, Strings.ResourceManager, new object[]
			{
				memberName
			});
		}

		public static LocalizedString ErrorCannotFindAcceptedDomain(string domainName)
		{
			return new LocalizedString("ErrorCannotFindAcceptedDomain", "Ex550E91", false, true, Strings.ResourceManager, new object[]
			{
				domainName
			});
		}

		public static LocalizedString ErrorCannotGetNamespaceId(string domain)
		{
			return new LocalizedString("ErrorCannotGetNamespaceId", "Ex5FF1C3", false, true, Strings.ResourceManager, new object[]
			{
				domain
			});
		}

		public static LocalizedString ErrorUserOrganizationIsNull(string adminName)
		{
			return new LocalizedString("ErrorUserOrganizationIsNull", "Ex308959", false, true, Strings.ResourceManager, new object[]
			{
				adminName
			});
		}

		public static LocalizedString ErrorEmailAddressTooLong
		{
			get
			{
				return new LocalizedString("ErrorEmailAddressTooLong", "Ex8EFBF5", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorManagedMemberDoesNotExist(string memberName)
		{
			return new LocalizedString("ErrorManagedMemberDoesNotExist", "ExFE76C4", false, true, Strings.ResourceManager, new object[]
			{
				memberName
			});
		}

		public static LocalizedString BecErrorServiceUnavailable
		{
			get
			{
				return new LocalizedString("BecErrorServiceUnavailable", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString IDSErrorUnexpectedXmlForCreatePassports(string memberName, string result)
		{
			return new LocalizedString("IDSErrorUnexpectedXmlForCreatePassports", "ExC2254F", false, true, Strings.ResourceManager, new object[]
			{
				memberName,
				result
			});
		}

		public static LocalizedString IDSErrorEmptySignInNamesForNetId
		{
			get
			{
				return new LocalizedString("IDSErrorEmptySignInNamesForNetId", "ExDB6EC4", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorWLCDPartnerAccessException(string message)
		{
			return new LocalizedString("ErrorWLCDPartnerAccessException", "Ex44F3EF", false, true, Strings.ResourceManager, new object[]
			{
				message
			});
		}

		public static LocalizedString ErrorUserAlreadyInactive
		{
			get
			{
				return new LocalizedString("ErrorUserAlreadyInactive", "Ex52119D", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString BecErrorTooManyMappedTenants
		{
			get
			{
				return new LocalizedString("BecErrorTooManyMappedTenants", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorIDSReturnedNullNetID(string memberName, string federatedIdentity)
		{
			return new LocalizedString("ErrorIDSReturnedNullNetID", "Ex68D71B", false, true, Strings.ResourceManager, new object[]
			{
				memberName,
				federatedIdentity
			});
		}

		public static LocalizedString BecErrorStringLength
		{
			get
			{
				return new LocalizedString("BecErrorStringLength", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorPartnerNotAuthorized
		{
			get
			{
				return new LocalizedString("ErrorPartnerNotAuthorized", "Ex440094", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorManagedParameterSetPassedInForFederatedNamespace
		{
			get
			{
				return new LocalizedString("ErrorManagedParameterSetPassedInForFederatedNamespace", "Ex275CAB", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString BecDirectoryInternalError
		{
			get
			{
				return new LocalizedString("BecDirectoryInternalError", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString BecUnknownError
		{
			get
			{
				return new LocalizedString("BecUnknownError", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorRedirectionEntryExists(string windowsLiveId)
		{
			return new LocalizedString("ErrorRedirectionEntryExists", "Ex689A9A", false, true, Strings.ResourceManager, new object[]
			{
				windowsLiveId
			});
		}

		public static LocalizedString ErrorPasswordContainsLastName
		{
			get
			{
				return new LocalizedString("ErrorPasswordContainsLastName", "Ex63E272", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorInvalidNetId(string memberName)
		{
			return new LocalizedString("ErrorInvalidNetId", "Ex664B35", false, true, Strings.ResourceManager, new object[]
			{
				memberName
			});
		}

		public static LocalizedString BecInternalError
		{
			get
			{
				return new LocalizedString("BecInternalError", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorDomainIsManaged
		{
			get
			{
				return new LocalizedString("ErrorDomainIsManaged", "Ex38C789", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorPasswordContainsMemberName
		{
			get
			{
				return new LocalizedString("ErrorPasswordContainsMemberName", "Ex95C592", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorLiveIdAlreadyExistsAsManaged(string windowsLiveId)
		{
			return new LocalizedString("ErrorLiveIdAlreadyExistsAsManaged", "ExB10BE9", false, true, Strings.ResourceManager, new object[]
			{
				windowsLiveId
			});
		}

		public static LocalizedString ErrorPartnerCannotModifyProtectedField
		{
			get
			{
				return new LocalizedString("ErrorPartnerCannotModifyProtectedField", "ExA65C1B", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorMissingNetIDWhenBypassWLID
		{
			get
			{
				return new LocalizedString("ErrorMissingNetIDWhenBypassWLID", "Ex81D36A", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorEmailStartsAndEndsWithDot
		{
			get
			{
				return new LocalizedString("ErrorEmailStartsAndEndsWithDot", "ExD5B19B", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorLiveIdAlreadyExistsAsEASI(string windowsLiveId)
		{
			return new LocalizedString("ErrorLiveIdAlreadyExistsAsEASI", "Ex280342", false, true, Strings.ResourceManager, new object[]
			{
				windowsLiveId
			});
		}

		public static LocalizedString ErrorCodeInvalidNetId
		{
			get
			{
				return new LocalizedString("ErrorCodeInvalidNetId", "Ex94410E", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorMemberExists
		{
			get
			{
				return new LocalizedString("ErrorMemberExists", "ExB09BC6", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString BecErrorRequiredProperty
		{
			get
			{
				return new LocalizedString("BecErrorRequiredProperty", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString IDSErrorBlob(string errorBlob)
		{
			return new LocalizedString("IDSErrorBlob", "Ex43D0F9", false, true, Strings.ResourceManager, new object[]
			{
				errorBlob
			});
		}

		public static LocalizedString ErrorPasswordBlank(string memberName)
		{
			return new LocalizedString("ErrorPasswordBlank", "Ex090AAB", false, true, Strings.ResourceManager, new object[]
			{
				memberName
			});
		}

		public static LocalizedString ErrorInvalidBrandData
		{
			get
			{
				return new LocalizedString("ErrorInvalidBrandData", "Ex5952D7", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString IDSInternalError(string additionalMessage)
		{
			return new LocalizedString("IDSInternalError", "Ex7BFB37", false, true, Strings.ResourceManager, new object[]
			{
				additionalMessage
			});
		}

		public static LocalizedString ErrorCanNotSetPasswordOrResetOnFederatedAccount
		{
			get
			{
				return new LocalizedString("ErrorCanNotSetPasswordOrResetOnFederatedAccount", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorSignInNameTooLong
		{
			get
			{
				return new LocalizedString("ErrorSignInNameTooLong", "Ex5C1AC7", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorInvalidMemberName(string memberName)
		{
			return new LocalizedString("ErrorInvalidMemberName", "ExD68B92", false, true, Strings.ResourceManager, new object[]
			{
				memberName
			});
		}

		public static LocalizedString ErrorNamespaceDoesNotExist
		{
			get
			{
				return new LocalizedString("ErrorNamespaceDoesNotExist", "Ex11F39D", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorMemberNameInUse(string memberName)
		{
			return new LocalizedString("ErrorMemberNameInUse", "Ex9957D0", false, true, Strings.ResourceManager, new object[]
			{
				memberName
			});
		}

		public static LocalizedString IDSErrorMissingCredFlagsNode
		{
			get
			{
				return new LocalizedString("IDSErrorMissingCredFlagsNode", "Ex40C1B4", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorPasswordInvalid(string memberName)
		{
			return new LocalizedString("ErrorPasswordInvalid", "Ex816E42", false, true, Strings.ResourceManager, new object[]
			{
				memberName
			});
		}

		public static LocalizedString ErrorNonEASIMemberExists
		{
			get
			{
				return new LocalizedString("ErrorNonEASIMemberExists", "ExE53E32", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorPrefix(string identity)
		{
			return new LocalizedString("ErrorPrefix", "Ex589ADA", false, true, Strings.ResourceManager, new object[]
			{
				identity
			});
		}

		public static LocalizedString ErrorOnGetNamespaceId(string domain)
		{
			return new LocalizedString("ErrorOnGetNamespaceId", "Ex42CD0A", false, true, Strings.ResourceManager, new object[]
			{
				domain
			});
		}

		public static LocalizedString ErrorPasswordTooLong(string memberName)
		{
			return new LocalizedString("ErrorPasswordTooLong", "Ex52FA3C", false, true, Strings.ResourceManager, new object[]
			{
				memberName
			});
		}

		public static LocalizedString ErrorPasswordContainsFirstName
		{
			get
			{
				return new LocalizedString("ErrorPasswordContainsFirstName", "Ex8845A7", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorDomainNameIsNull
		{
			get
			{
				return new LocalizedString("ErrorDomainNameIsNull", "ExD819C0", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString BecErrorQuotaExceeded
		{
			get
			{
				return new LocalizedString("BecErrorQuotaExceeded", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorManagedMemberNotExists(string memberName)
		{
			return new LocalizedString("ErrorManagedMemberNotExists", "Ex0CE124", false, true, Strings.ResourceManager, new object[]
			{
				memberName
			});
		}

		public static LocalizedString ErrorDatabaseOnMaintenance
		{
			get
			{
				return new LocalizedString("ErrorDatabaseOnMaintenance", "ExC74032", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorMemberNameUnavailableUsedAlternateAlias(string memberName)
		{
			return new LocalizedString("ErrorMemberNameUnavailableUsedAlternateAlias", "Ex309929", false, true, Strings.ResourceManager, new object[]
			{
				memberName
			});
		}

		public static LocalizedString ErrorLiveIdWithFederatedIdentityExists
		{
			get
			{
				return new LocalizedString("ErrorLiveIdWithFederatedIdentityExists", "Ex48C427", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorSignInNameTooShort
		{
			get
			{
				return new LocalizedString("ErrorSignInNameTooShort", "ExBDC882", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorLiveServicesPartnerAccessException(string message, string info)
		{
			return new LocalizedString("ErrorLiveServicesPartnerAccessException", "ExF735F7", false, true, Strings.ResourceManager, new object[]
			{
				message,
				info
			});
		}

		public static LocalizedString ErrorSPFPasswordTooShort
		{
			get
			{
				return new LocalizedString("ErrorSPFPasswordTooShort", "ExFEBDFD", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString GetLocalizedString(Strings.IDs key)
		{
			return new LocalizedString(Strings.stringIDs[(uint)key], Strings.ResourceManager, new object[0]);
		}

		private static Dictionary<uint, string> stringIDs = new Dictionary<uint, string>(107);

		private static ExchangeResourceManager ResourceManager = ExchangeResourceManager.GetResourceManager("Microsoft.Exchange.DatacenterStrings.Strings", typeof(Strings).GetTypeInfo().Assembly);

		public enum IDs : uint
		{
			BecErrorThrottling = 3442334529U,
			ErrorSPFInvalidMemberName = 2921179735U,
			BecAccessDenied = 3165600047U,
			BecIncorrectPassword = 534972034U,
			BecErrorNotFouond = 2713690330U,
			ErrorManagedParameterSetPassedInForFederatedTenant = 2548455642U,
			IDSErrorMissingNamespaceIdNode = 457807320U,
			ErrorParameterIsIncorrect = 3569539930U,
			ErrorCodeProfileDoesNotExists = 3398198740U,
			ErrorMemberLockedOutBecauseOfPasswordAttempts = 71190514U,
			IDSErrorEmptyCredFlagsNode = 507263250U,
			ErrorPasswordMatchesAccountWithSameMemberName = 2457590018U,
			BecErrorDomainValidation = 240550867U,
			ErrorDomainDoesNotExist = 2539075827U,
			ErrorSignInNameInCompleteOrInvalid = 1628902957U,
			ErrorUserBlocked = 746302685U,
			BecTransientError = 2001068460U,
			ErrorFederatedParameterSetPassedInForManagedTenant = 2511795258U,
			IDSErrorEmptyPuidNode = 2085485339U,
			BecRedirection = 1384815350U,
			InvalidNetId = 2259851215U,
			ErrorNameContainsBlockedWord = 1908027994U,
			ErrorServiceUnavailableDueToInternalError = 153681091U,
			IDSErrorEmptyNetIdNode = 1374323893U,
			ErrorSPFPasswordTooLong = 1890206562U,
			BecErrorInvalidHeader = 3875381480U,
			BecErrorUserExists = 2343187933U,
			ErrorUserNameReserved = 1260215560U,
			ErrorFieldContainsInvalidCharacters = 286588848U,
			ErrorPasswordRequired = 2252511124U,
			ErrorInputContainsForbiddenWord = 3038881494U,
			ErrorFederatedAccountAlreadyExists = 1170008863U,
			ErrorEmailNameContainsInvalidCharacters = 2883915563U,
			ErrorPasswordContainedInSQ = 116822391U,
			BecErrorInvalidContext = 3327865544U,
			ErrorsRemovedMailboxHaveNoNetID = 2629176178U,
			ErrorPasswordContainsInvalidCharacters = 3821098863U,
			ErrorInvalidPassportId = 24718424U,
			BecErrorQuota = 3848807642U,
			BecErrorInvalidLicense = 159495284U,
			ErrorSPFPasswordIsBlank = 1626889520U,
			ErrorCannotRecoverLiveIdMismatchInstanceType = 4122120961U,
			BecErrorSubscription = 2570238007U,
			ErrorSecretAnswerContainsPassword = 2854433926U,
			ErrorWLCDInternal = 344542475U,
			ErrorManagedMemberExistsSPF = 1112425574U,
			BecErrorUniquenessValidation = 2178303307U,
			ErrorPasswordIsInvalid = 3710614060U,
			BecErrorInvalidWeakPassword = 3791085944U,
			ErrorIncompleteEmailAddress = 1520187672U,
			ErrorSecretQuestionContainsPassword = 1810820280U,
			BecUserInRecycleState = 561051570U,
			ErrorFederatedParameterSetPassedInForManagedNamespace = 854137693U,
			InternalError = 3194642979U,
			ErrorMemberNameAndFederatedIdentityNotMatch = 764417362U,
			ErrorArchiveOnly = 1746286164U,
			BecErrorSyntaxValidation = 2776399702U,
			IDSErrorEmptyNamespaceIdNode = 2366837181U,
			ErrorMemberIsLocked = 2439548284U,
			ErrorSecretQuestionContainsMemberName = 1968969488U,
			BecErrorAccountDisabled = 370795085U,
			BecErrorInvalidPassword = 1787101328U,
			ErrorCannotRenameCredentialToSameName = 2743094604U,
			ErrorEnableRoomMailboxAccountParameterFalseInDatacenter = 2646378639U,
			ErrorAccountIsNotEASI = 3978792718U,
			NotAuthorized = 1953245018U,
			WindowsLiveIdProvisioningHandlerException = 888348176U,
			OrganizationIsReadOnly = 4169169063U,
			ErrorEmailNameStartAfterDot = 1190886676U,
			ErrorNamespaceNotFound = 3074197328U,
			BecIdentityInternalError = 3693389389U,
			BecCompanyNotFound = 2927230868U,
			ErrorNoRecordInDatabase = 2733812604U,
			BecErrorPropertyNotSettable = 2692387272U,
			ErrorEmailAddressTooLong = 2602850872U,
			BecErrorServiceUnavailable = 1479442559U,
			IDSErrorEmptySignInNamesForNetId = 2501723808U,
			ErrorUserAlreadyInactive = 4277226160U,
			BecErrorTooManyMappedTenants = 3142877869U,
			BecErrorStringLength = 1684749577U,
			ErrorPartnerNotAuthorized = 509547158U,
			ErrorManagedParameterSetPassedInForFederatedNamespace = 1770558589U,
			BecDirectoryInternalError = 312656184U,
			BecUnknownError = 364820960U,
			ErrorPasswordContainsLastName = 829279753U,
			BecInternalError = 3970665479U,
			ErrorDomainIsManaged = 938622763U,
			ErrorPasswordContainsMemberName = 3197989195U,
			ErrorPartnerCannotModifyProtectedField = 706972491U,
			ErrorMissingNetIDWhenBypassWLID = 1463063580U,
			ErrorEmailStartsAndEndsWithDot = 3151258897U,
			ErrorCodeInvalidNetId = 524928998U,
			ErrorMemberExists = 146893062U,
			BecErrorRequiredProperty = 1316852426U,
			ErrorInvalidBrandData = 2872232368U,
			ErrorCanNotSetPasswordOrResetOnFederatedAccount = 3566184850U,
			ErrorSignInNameTooLong = 840675049U,
			ErrorNamespaceDoesNotExist = 2454442314U,
			IDSErrorMissingCredFlagsNode = 2655490445U,
			ErrorNonEASIMemberExists = 3197291445U,
			ErrorPasswordContainsFirstName = 1384046745U,
			ErrorDomainNameIsNull = 1475855582U,
			BecErrorQuotaExceeded = 1895268805U,
			ErrorDatabaseOnMaintenance = 3873716657U,
			ErrorLiveIdWithFederatedIdentityExists = 3937658635U,
			ErrorSignInNameTooShort = 2857272973U,
			ErrorSPFPasswordTooShort = 173165206U
		}

		private enum ParamIDs
		{
			ErrorCannotChangeMemberFederationState,
			ErrorManagedMemberExists,
			ErrorEvictLiveIdMemberExists,
			ErrorPasswordIncludesMemberName,
			ErrorEvictLiveIdMemberNotExists,
			IDSErrorUnexpectedResultsForGetProfileByAttributes,
			ErrorMemberNameUnavailableUsedForEASI,
			ErrorPasswordIncludesInvalidChars,
			VerboseEvictMember,
			ErrorUnknown,
			ErrorCannotUseReservedLiveId,
			ErrorWindowsLiveIdRequired,
			ErrorRedirectionEntryManagerException,
			ErrorCannotImportForNamespaceType,
			ErrorUnmanagedMemberNotExists,
			ErrorMemberNameUnavailableUsedForDL,
			ErrorLiveIdDoesNotExist,
			IDSErrorUnexpectedResultsForCreatePassports,
			ErrorConfigurationUnitNotFound,
			ErrorPasswordTooShort,
			ErrorCannotRemoveWindowsLiveIDFromProxyAddresses,
			ErrorGuidNotParsable,
			ErrorImportLiveIdManagedMemberExists,
			ErrorManagedMemberDoesNotExistForByolid,
			ErrorCannotRenameAccrossNamespaceTypes,
			OrganizationIsImmutable,
			ErrorCannotDetermineLiveInstance,
			ErrorFailedToEvictMember,
			ErrorParameterRequired,
			ErrorOnGetProfile,
			ErrorMemberNameBlocked,
			SPFInternalError,
			ErrorMemberNameUnavailable,
			ErrorUnmanagedMemberExists,
			AdditionalDebugInfo,
			ErrorMaxMembershipLimit,
			ErrorCannotFindAcceptedDomain,
			ErrorCannotGetNamespaceId,
			ErrorUserOrganizationIsNull,
			ErrorManagedMemberDoesNotExist,
			IDSErrorUnexpectedXmlForCreatePassports,
			ErrorWLCDPartnerAccessException,
			ErrorIDSReturnedNullNetID,
			ErrorRedirectionEntryExists,
			ErrorInvalidNetId,
			ErrorLiveIdAlreadyExistsAsManaged,
			ErrorLiveIdAlreadyExistsAsEASI,
			IDSErrorBlob,
			ErrorPasswordBlank,
			IDSInternalError,
			ErrorInvalidMemberName,
			ErrorMemberNameInUse,
			ErrorPasswordInvalid,
			ErrorPrefix,
			ErrorOnGetNamespaceId,
			ErrorPasswordTooLong,
			ErrorManagedMemberNotExists,
			ErrorMemberNameUnavailableUsedAlternateAlias,
			ErrorLiveServicesPartnerAccessException
		}
	}
}
