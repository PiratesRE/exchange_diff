using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;

namespace Microsoft.Exchange.Data.ApplicationLogic
{
	internal static class Strings
	{
		static Strings()
		{
			Strings.stringIDs.Add(1416971879U, "DatacenterSecretIsMissingOrInvalid");
			Strings.stringIDs.Add(358167539U, "InvalidConfigurationMissingLinkedInAccessTokenEndpoint");
			Strings.stringIDs.Add(507132958U, "FailedToCreateAuditEwsConnection");
			Strings.stringIDs.Add(425380821U, "UserPhotoStoreNotFound");
			Strings.stringIDs.Add(1386497625U, "ErrorReasonNoMailboxInHosts");
			Strings.stringIDs.Add(2430608892U, "ErrorReasonIgnoreCaseWithoutRegExInEntitiesRules");
			Strings.stringIDs.Add(2258791064U, "ErrorReasonManifestSchemaUnknown");
			Strings.stringIDs.Add(1262244671U, "ErrorCannotDisableMandatoryExtension");
			Strings.stringIDs.Add(1644547849U, "InvalidConfigurationFacebookAppId");
			Strings.stringIDs.Add(1736107127U, "WelcomeMailSubject");
			Strings.stringIDs.Add(3597849903U, "ErrorExtensionAlreadyInstalled");
			Strings.stringIDs.Add(3654277798U, "ErrorReasonDefaultVersionIsInvalid");
			Strings.stringIDs.Add(2929113449U, "InvalidConfigurationMissingLinkedInConnectionsEndpoint");
			Strings.stringIDs.Add(1132961834U, "ErrorExtensionVersionInvalid");
			Strings.stringIDs.Add(3473955170U, "ErrorReasonItemTypeInvalid");
			Strings.stringIDs.Add(1902437266U, "FailureReasonSourceLocationTagMissing");
			Strings.stringIDs.Add(1272553825U, "ErrorExtensionWithIdAlreadyInstalledForOrg");
			Strings.stringIDs.Add(809039298U, "InvalidConfigurationMissingFacebookAppSecret");
			Strings.stringIDs.Add(1528417217U, "FailedToReadWebProxyConfigurationFromAD");
			Strings.stringIDs.Add(693971404U, "UserPhotoNotFound");
			Strings.stringIDs.Add(3286161403U, "InvalidConfigurationMissingLinkedInAppId");
			Strings.stringIDs.Add(224007719U, "PhotoHasBeenDeleted");
			Strings.stringIDs.Add(994301062U, "FailedToAccessAuditLogWithInnerException");
			Strings.stringIDs.Add(980865226U, "InvalidConfigurationMissingLinkedInRequestTokenEndpoint");
			Strings.stringIDs.Add(895214716U, "ErrorExtensionAlreadyInstalledForOrg");
			Strings.stringIDs.Add(2103946832U, "AuditLogAccessDenied");
			Strings.stringIDs.Add(2566788983U, "ErrorReasonItemTypeAllTypes");
			Strings.stringIDs.Add(1057073101U, "ErrorMasterTableSaveConflict");
			Strings.stringIDs.Add(3960918573U, "InvalidConfigurationMissingFacebookAppId");
			Strings.stringIDs.Add(3353268377U, "ErrorCantOverwriteDefaultExtension");
			Strings.stringIDs.Add(4017558222U, "ErrorReasonOnlySelectedEntitiesInRestricted");
			Strings.stringIDs.Add(1445064246U, "UserPhotoAccessDenied");
			Strings.stringIDs.Add(3896524780U, "ErrorReasonItemIsRuleAttributesNotValidForEdit");
			Strings.stringIDs.Add(1303673802U, "ErrorReasonNoRegexRuleInRestricted");
			Strings.stringIDs.Add(2340791895U, "ErrorManifestSignatureInvalidExtension");
			Strings.stringIDs.Add(921746356U, "InvalidConfigurationMissingLinkedInAppSecret");
			Strings.stringIDs.Add(103323614U, "ErrorReasonRegExNameAndValueRequiredInEntitiesRules");
			Strings.stringIDs.Add(531489075U, "SecureStringParameter");
			Strings.stringIDs.Add(3364919108U, "InvalidConfigurationFacebookAppSecret");
			Strings.stringIDs.Add(163140049U, "InvalidConfigurationMissingFacebookGraphTokenEndpoint");
			Strings.stringIDs.Add(3689335943U, "InvalidConfigurationLinkedInAppId");
			Strings.stringIDs.Add(3369915033U, "ErrorOrgLevelAppMustBeSiteLicense");
			Strings.stringIDs.Add(2727153851U, "ErrorMarketplaceWebServicesUnavailable");
			Strings.stringIDs.Add(2267978298U, "InvalidConfigurationMissingFacebookGraphApiEndpoint");
			Strings.stringIDs.Add(3571146374U, "InvalidConfigurationLinkedInAppSecret");
			Strings.stringIDs.Add(3104695745U, "InvalidConfigurationMissingFacebookAuthorizationEndpoint");
			Strings.stringIDs.Add(3368339595U, "InvalidConfigurationMissingLinkedInProfileEndpoint");
			Strings.stringIDs.Add(1184760770U, "FailureReasonNoAttributes");
			Strings.stringIDs.Add(3864317541U, "ErrorReasonRegexRuleInvalidValue");
			Strings.stringIDs.Add(3958377408U, "InvalidConfigurationMissingLinkedInInvalidateTokenEndpoint");
			Strings.stringIDs.Add(1579586375U, "ErrorReasonMinVersionIsInvalid");
			Strings.stringIDs.Add(190956125U, "ErrorReasonMissingOfficeApp");
			Strings.stringIDs.Add(1368134268U, "ErrorReasonOnlyMailboxSetAllowedInRequirement");
			Strings.stringIDs.Add(2777862026U, "ErrorReasonInvalidID");
			Strings.stringIDs.Add(330725840U, "ErrorReasonFormInFormSettingsNotUnique");
			Strings.stringIDs.Add(2661873235U, "WelcomeMailBodyNote");
			Strings.stringIDs.Add(1979709530U, "ErrorExtensionWithIdAlreadyInstalled");
		}

		public static LocalizedString DatacenterSecretIsMissingOrInvalid
		{
			get
			{
				return new LocalizedString("DatacenterSecretIsMissingOrInvalid", "ExE92D84", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InvalidConfigurationMissingLinkedInAccessTokenEndpoint
		{
			get
			{
				return new LocalizedString("InvalidConfigurationMissingLinkedInAccessTokenEndpoint", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorEtokenWithInvalidDeploymentId(string deploymentId)
		{
			return new LocalizedString("ErrorEtokenWithInvalidDeploymentId", "", false, false, Strings.ResourceManager, new object[]
			{
				deploymentId
			});
		}

		public static LocalizedString ErrorProtocolConfigurationMissing(string lastServer, string settingsType)
		{
			return new LocalizedString("ErrorProtocolConfigurationMissing", "", false, false, Strings.ResourceManager, new object[]
			{
				lastServer,
				settingsType
			});
		}

		public static LocalizedString ErrorCanNotReadInstalledList(string failureReason)
		{
			return new LocalizedString("ErrorCanNotReadInstalledList", "", false, false, Strings.ResourceManager, new object[]
			{
				failureReason
			});
		}

		public static LocalizedString FailureReasonAttributeMissing(string targetName)
		{
			return new LocalizedString("FailureReasonAttributeMissing", "", false, false, Strings.ResourceManager, new object[]
			{
				targetName
			});
		}

		public static LocalizedString FailedToCreateAuditEwsConnection
		{
			get
			{
				return new LocalizedString("FailedToCreateAuditEwsConnection", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString UserPhotoStoreNotFound
		{
			get
			{
				return new LocalizedString("UserPhotoStoreNotFound", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorReasonNoMailboxInHosts
		{
			get
			{
				return new LocalizedString("ErrorReasonNoMailboxInHosts", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ADUserNoPhoto(ADObjectId userId)
		{
			return new LocalizedString("ADUserNoPhoto", "", false, false, Strings.ResourceManager, new object[]
			{
				userId
			});
		}

		public static LocalizedString ErrorReasonIgnoreCaseWithoutRegExInEntitiesRules
		{
			get
			{
				return new LocalizedString("ErrorReasonIgnoreCaseWithoutRegExInEntitiesRules", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorReasonManifestSchemaUnknown
		{
			get
			{
				return new LocalizedString("ErrorReasonManifestSchemaUnknown", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorCannotDisableMandatoryExtension
		{
			get
			{
				return new LocalizedString("ErrorCannotDisableMandatoryExtension", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InvalidConfigurationFacebookAppId
		{
			get
			{
				return new LocalizedString("InvalidConfigurationFacebookAppId", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString WelcomeMailSubject
		{
			get
			{
				return new LocalizedString("WelcomeMailSubject", "ExC92635", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorExtensionAlreadyInstalled
		{
			get
			{
				return new LocalizedString("ErrorExtensionAlreadyInstalled", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorReasonDefaultVersionIsInvalid
		{
			get
			{
				return new LocalizedString("ErrorReasonDefaultVersionIsInvalid", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString CannotMapInvalidSmtpAddressToPhotoFile(string address)
		{
			return new LocalizedString("CannotMapInvalidSmtpAddressToPhotoFile", "", false, false, Strings.ResourceManager, new object[]
			{
				address
			});
		}

		public static LocalizedString InvalidConfigurationMissingLinkedInConnectionsEndpoint
		{
			get
			{
				return new LocalizedString("InvalidConfigurationMissingLinkedInConnectionsEndpoint", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorExtensionVersionInvalid
		{
			get
			{
				return new LocalizedString("ErrorExtensionVersionInvalid", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorReasonItemTypeInvalid
		{
			get
			{
				return new LocalizedString("ErrorReasonItemTypeInvalid", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString FailureReasonSourceLocationTagMissing
		{
			get
			{
				return new LocalizedString("FailureReasonSourceLocationTagMissing", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorExtensionWithIdAlreadyInstalledForOrg
		{
			get
			{
				return new LocalizedString("ErrorExtensionWithIdAlreadyInstalledForOrg", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorReasonTooManyRule(int maxRuleNumber)
		{
			return new LocalizedString("ErrorReasonTooManyRule", "", false, false, Strings.ResourceManager, new object[]
			{
				maxRuleNumber
			});
		}

		public static LocalizedString FailureReasonUserMasterTableInvalidScope(string scope, string id)
		{
			return new LocalizedString("FailureReasonUserMasterTableInvalidScope", "", false, false, Strings.ResourceManager, new object[]
			{
				scope,
				id
			});
		}

		public static LocalizedString ErrorInvalidManifestData(string ErrorReason)
		{
			return new LocalizedString("ErrorInvalidManifestData", "", false, false, Strings.ResourceManager, new object[]
			{
				ErrorReason
			});
		}

		public static LocalizedString InvalidConfigurationMissingFacebookAppSecret
		{
			get
			{
				return new LocalizedString("InvalidConfigurationMissingFacebookAppSecret", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorReasonInvalidXml(string xmlExceptionMessage)
		{
			return new LocalizedString("ErrorReasonInvalidXml", "", false, false, Strings.ResourceManager, new object[]
			{
				xmlExceptionMessage
			});
		}

		public static LocalizedString FailedToReadWebProxyConfigurationFromAD
		{
			get
			{
				return new LocalizedString("FailedToReadWebProxyConfigurationFromAD", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString UserPhotoNotFound
		{
			get
			{
				return new LocalizedString("UserPhotoNotFound", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorCouldNotRegisterComponent(string componentName)
		{
			return new LocalizedString("ErrorCouldNotRegisterComponent", "", false, false, Strings.ResourceManager, new object[]
			{
				componentName
			});
		}

		public static LocalizedString InvalidConfigurationMissingLinkedInAppId
		{
			get
			{
				return new LocalizedString("InvalidConfigurationMissingLinkedInAppId", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString PhotoHasBeenDeleted
		{
			get
			{
				return new LocalizedString("PhotoHasBeenDeleted", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString FailedToAccessAuditLogWithInnerException
		{
			get
			{
				return new LocalizedString("FailedToAccessAuditLogWithInnerException", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorReasonUrlMustBeAbsolute(string name, string value)
		{
			return new LocalizedString("ErrorReasonUrlMustBeAbsolute", "", false, false, Strings.ResourceManager, new object[]
			{
				name,
				value
			});
		}

		public static LocalizedString InvalidConfigurationMissingLinkedInRequestTokenEndpoint
		{
			get
			{
				return new LocalizedString("InvalidConfigurationMissingLinkedInRequestTokenEndpoint", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorExtensionAlreadyInstalledForOrg
		{
			get
			{
				return new LocalizedString("ErrorExtensionAlreadyInstalledForOrg", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString AuditLogAccessDenied
		{
			get
			{
				return new LocalizedString("AuditLogAccessDenied", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorMissingNodeInEtoken(string NodeName)
		{
			return new LocalizedString("ErrorMissingNodeInEtoken", "", false, false, Strings.ResourceManager, new object[]
			{
				NodeName
			});
		}

		public static LocalizedString ErrorReasonItemTypeAllTypes
		{
			get
			{
				return new LocalizedString("ErrorReasonItemTypeAllTypes", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorMasterTableSaveConflict
		{
			get
			{
				return new LocalizedString("ErrorMasterTableSaveConflict", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorReasonManifestVersionNotSupported(string schemaVersion, Version serverVersion)
		{
			return new LocalizedString("ErrorReasonManifestVersionNotSupported", "", false, false, Strings.ResourceManager, new object[]
			{
				schemaVersion,
				serverVersion
			});
		}

		public static LocalizedString InvalidConfigurationMissingFacebookAppId
		{
			get
			{
				return new LocalizedString("InvalidConfigurationMissingFacebookAppId", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString UserPhotoWithClassNotFound(string itemClass)
		{
			return new LocalizedString("UserPhotoWithClassNotFound", "", false, false, Strings.ResourceManager, new object[]
			{
				itemClass
			});
		}

		public static LocalizedString ErrorReasonManifestTooLarge(int maxSize)
		{
			return new LocalizedString("ErrorReasonManifestTooLarge", "", false, false, Strings.ResourceManager, new object[]
			{
				maxSize
			});
		}

		public static LocalizedString ErrorCantOverwriteDefaultExtension
		{
			get
			{
				return new LocalizedString("ErrorCantOverwriteDefaultExtension", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorReasonOnlySelectedEntitiesInRestricted
		{
			get
			{
				return new LocalizedString("ErrorReasonOnlySelectedEntitiesInRestricted", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorCannotUninstallProvidedExtension(string ExtensionID)
		{
			return new LocalizedString("ErrorCannotUninstallProvidedExtension", "", false, false, Strings.ResourceManager, new object[]
			{
				ExtensionID
			});
		}

		public static LocalizedString ErrorReasonMultipleRulesWithSameRegExName(string regExName)
		{
			return new LocalizedString("ErrorReasonMultipleRulesWithSameRegExName", "", false, false, Strings.ResourceManager, new object[]
			{
				regExName
			});
		}

		public static LocalizedString UserPhotoAccessDenied
		{
			get
			{
				return new LocalizedString("UserPhotoAccessDenied", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString UserPhotoThumbprintNotFound(bool preview)
		{
			return new LocalizedString("UserPhotoThumbprintNotFound", "", false, false, Strings.ResourceManager, new object[]
			{
				preview
			});
		}

		public static LocalizedString ErrorReasonItemIsRuleAttributesNotValidForEdit
		{
			get
			{
				return new LocalizedString("ErrorReasonItemIsRuleAttributesNotValidForEdit", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorReasonNoRegexRuleInRestricted
		{
			get
			{
				return new LocalizedString("ErrorReasonNoRegexRuleInRestricted", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorReasonInvalidUrlValue(string name, string value)
		{
			return new LocalizedString("ErrorReasonInvalidUrlValue", "", false, false, Strings.ResourceManager, new object[]
			{
				name,
				value
			});
		}

		public static LocalizedString FailedToFindEwsEndpoint(string mailbox)
		{
			return new LocalizedString("FailedToFindEwsEndpoint", "", false, false, Strings.ResourceManager, new object[]
			{
				mailbox
			});
		}

		public static LocalizedString ErrorManifestSignatureInvalidExtension
		{
			get
			{
				return new LocalizedString("ErrorManifestSignatureInvalidExtension", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorVDirConfigurationMissing(string lastServer, string urlType, string missingServiceType)
		{
			return new LocalizedString("ErrorVDirConfigurationMissing", "", false, false, Strings.ResourceManager, new object[]
			{
				lastServer,
				urlType,
				missingServiceType
			});
		}

		public static LocalizedString WelcomeMailBodyMain(string link, string note)
		{
			return new LocalizedString("WelcomeMailBodyMain", "Ex74CCEB", false, true, Strings.ResourceManager, new object[]
			{
				link,
				note
			});
		}

		public static LocalizedString InvalidConfigurationMissingLinkedInAppSecret
		{
			get
			{
				return new LocalizedString("InvalidConfigurationMissingLinkedInAppSecret", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString FailedToReadProviderConfigurationSeeInnerException(string provider)
		{
			return new LocalizedString("FailedToReadProviderConfigurationSeeInnerException", "", false, false, Strings.ResourceManager, new object[]
			{
				provider
			});
		}

		public static LocalizedString ErrorReasonMinApiVersionNotSupported(Version minApiVersion, Version serverVersion)
		{
			return new LocalizedString("ErrorReasonMinApiVersionNotSupported", "", false, false, Strings.ResourceManager, new object[]
			{
				minApiVersion,
				serverVersion
			});
		}

		public static LocalizedString ErrorReasonRegExNameAndValueRequiredInEntitiesRules
		{
			get
			{
				return new LocalizedString("ErrorReasonRegExNameAndValueRequiredInEntitiesRules", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString SecureStringParameter
		{
			get
			{
				return new LocalizedString("SecureStringParameter", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InvalidConfigurationFacebookAppSecret
		{
			get
			{
				return new LocalizedString("InvalidConfigurationFacebookAppSecret", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorExtensionUnableToUpgrade(string extensionName)
		{
			return new LocalizedString("ErrorExtensionUnableToUpgrade", "", false, false, Strings.ResourceManager, new object[]
			{
				extensionName
			});
		}

		public static LocalizedString FailureReasonOrgMasterTableInvalidScope(string scope, string id)
		{
			return new LocalizedString("FailureReasonOrgMasterTableInvalidScope", "", false, false, Strings.ResourceManager, new object[]
			{
				scope,
				id
			});
		}

		public static LocalizedString InvalidConfigurationMissingFacebookGraphTokenEndpoint
		{
			get
			{
				return new LocalizedString("InvalidConfigurationMissingFacebookGraphTokenEndpoint", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorReasonUnsupportedUrlScheme(string name, string value)
		{
			return new LocalizedString("ErrorReasonUnsupportedUrlScheme", "", false, false, Strings.ResourceManager, new object[]
			{
				name,
				value
			});
		}

		public static LocalizedString InvalidConfigurationLinkedInAppId
		{
			get
			{
				return new LocalizedString("InvalidConfigurationLinkedInAppId", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorOrgLevelAppMustBeSiteLicense
		{
			get
			{
				return new LocalizedString("ErrorOrgLevelAppMustBeSiteLicense", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorMarketplaceWebServicesUnavailable
		{
			get
			{
				return new LocalizedString("ErrorMarketplaceWebServicesUnavailable", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InvalidConfigurationMissingFacebookGraphApiEndpoint
		{
			get
			{
				return new LocalizedString("InvalidConfigurationMissingFacebookGraphApiEndpoint", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InvalidConfigurationLinkedInAppSecret
		{
			get
			{
				return new LocalizedString("InvalidConfigurationLinkedInAppSecret", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InvalidConfigurationMissingFacebookAuthorizationEndpoint
		{
			get
			{
				return new LocalizedString("InvalidConfigurationMissingFacebookAuthorizationEndpoint", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InvalidConfigurationMissingLinkedInProfileEndpoint
		{
			get
			{
				return new LocalizedString("InvalidConfigurationMissingLinkedInProfileEndpoint", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString FailureReasonNoAttributes
		{
			get
			{
				return new LocalizedString("FailureReasonNoAttributes", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorReasonRegexRuleInvalidValue
		{
			get
			{
				return new LocalizedString("ErrorReasonRegexRuleInvalidValue", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorReasonTooManyRegexRule(int maxRuleNumber)
		{
			return new LocalizedString("ErrorReasonTooManyRegexRule", "", false, false, Strings.ResourceManager, new object[]
			{
				maxRuleNumber
			});
		}

		public static LocalizedString FailureReasonAttributeValueInvalid(string targetName, string value)
		{
			return new LocalizedString("FailureReasonAttributeValueInvalid", "", false, false, Strings.ResourceManager, new object[]
			{
				targetName,
				value
			});
		}

		public static LocalizedString InvalidConfigurationMissingLinkedInInvalidateTokenEndpoint
		{
			get
			{
				return new LocalizedString("InvalidConfigurationMissingLinkedInInvalidateTokenEndpoint", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorReasonMinVersionIsInvalid
		{
			get
			{
				return new LocalizedString("ErrorReasonMinVersionIsInvalid", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorExtensionNotFound(string extensionID)
		{
			return new LocalizedString("ErrorExtensionNotFound", "", false, false, Strings.ResourceManager, new object[]
			{
				extensionID
			});
		}

		public static LocalizedString ErrorReasonMissingOfficeApp
		{
			get
			{
				return new LocalizedString("ErrorReasonMissingOfficeApp", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorReasonOnlyMailboxSetAllowedInRequirement
		{
			get
			{
				return new LocalizedString("ErrorReasonOnlyMailboxSetAllowedInRequirement", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorReasonInvalidID
		{
			get
			{
				return new LocalizedString("ErrorReasonInvalidID", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString UserPhotoTooManyItems(string itemClass)
		{
			return new LocalizedString("UserPhotoTooManyItems", "", false, false, Strings.ResourceManager, new object[]
			{
				itemClass
			});
		}

		public static LocalizedString ErrorReasonFormInFormSettingsNotUnique
		{
			get
			{
				return new LocalizedString("ErrorReasonFormInFormSettingsNotUnique", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString FailedToAccessAuditLog(string responseclass, string code, string error)
		{
			return new LocalizedString("FailedToAccessAuditLog", "", false, false, Strings.ResourceManager, new object[]
			{
				responseclass,
				code,
				error
			});
		}

		public static LocalizedString ErrorReasonMultipleRulesWithSameFilterName(string filterName)
		{
			return new LocalizedString("ErrorReasonMultipleRulesWithSameFilterName", "", false, false, Strings.ResourceManager, new object[]
			{
				filterName
			});
		}

		public static LocalizedString FailureReasonTagValueInvalid(string targetName, string value)
		{
			return new LocalizedString("FailureReasonTagValueInvalid", "", false, false, Strings.ResourceManager, new object[]
			{
				targetName,
				value
			});
		}

		public static LocalizedString WelcomeMailBodyNote
		{
			get
			{
				return new LocalizedString("WelcomeMailBodyNote", "Ex7B5215", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString FailureReasonTagMissing(string targetName)
		{
			return new LocalizedString("FailureReasonTagMissing", "", false, false, Strings.ResourceManager, new object[]
			{
				targetName
			});
		}

		public static LocalizedString ErrorReasonManifestValidationError(string exceptionMessage)
		{
			return new LocalizedString("ErrorReasonManifestValidationError", "", false, false, Strings.ResourceManager, new object[]
			{
				exceptionMessage
			});
		}

		public static LocalizedString ErrorExtensionWithIdAlreadyInstalled
		{
			get
			{
				return new LocalizedString("ErrorExtensionWithIdAlreadyInstalled", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorReasonInvalidRegEx(string ruleName, string attributeName)
		{
			return new LocalizedString("ErrorReasonInvalidRegEx", "", false, false, Strings.ResourceManager, new object[]
			{
				ruleName,
				attributeName
			});
		}

		public static LocalizedString ErrorAssetIdNotMatchInEtoken(string assetId, string assetIdInToken)
		{
			return new LocalizedString("ErrorAssetIdNotMatchInEtoken", "", false, false, Strings.ResourceManager, new object[]
			{
				assetId,
				assetIdInToken
			});
		}

		public static LocalizedString GetLocalizedString(Strings.IDs key)
		{
			return new LocalizedString(Strings.stringIDs[(uint)key], Strings.ResourceManager, new object[0]);
		}

		private static Dictionary<uint, string> stringIDs = new Dictionary<uint, string>(57);

		private static ExchangeResourceManager ResourceManager = ExchangeResourceManager.GetResourceManager("Microsoft.Exchange.Data.ApplicationLogic.Strings", typeof(Strings).GetTypeInfo().Assembly);

		public enum IDs : uint
		{
			DatacenterSecretIsMissingOrInvalid = 1416971879U,
			InvalidConfigurationMissingLinkedInAccessTokenEndpoint = 358167539U,
			FailedToCreateAuditEwsConnection = 507132958U,
			UserPhotoStoreNotFound = 425380821U,
			ErrorReasonNoMailboxInHosts = 1386497625U,
			ErrorReasonIgnoreCaseWithoutRegExInEntitiesRules = 2430608892U,
			ErrorReasonManifestSchemaUnknown = 2258791064U,
			ErrorCannotDisableMandatoryExtension = 1262244671U,
			InvalidConfigurationFacebookAppId = 1644547849U,
			WelcomeMailSubject = 1736107127U,
			ErrorExtensionAlreadyInstalled = 3597849903U,
			ErrorReasonDefaultVersionIsInvalid = 3654277798U,
			InvalidConfigurationMissingLinkedInConnectionsEndpoint = 2929113449U,
			ErrorExtensionVersionInvalid = 1132961834U,
			ErrorReasonItemTypeInvalid = 3473955170U,
			FailureReasonSourceLocationTagMissing = 1902437266U,
			ErrorExtensionWithIdAlreadyInstalledForOrg = 1272553825U,
			InvalidConfigurationMissingFacebookAppSecret = 809039298U,
			FailedToReadWebProxyConfigurationFromAD = 1528417217U,
			UserPhotoNotFound = 693971404U,
			InvalidConfigurationMissingLinkedInAppId = 3286161403U,
			PhotoHasBeenDeleted = 224007719U,
			FailedToAccessAuditLogWithInnerException = 994301062U,
			InvalidConfigurationMissingLinkedInRequestTokenEndpoint = 980865226U,
			ErrorExtensionAlreadyInstalledForOrg = 895214716U,
			AuditLogAccessDenied = 2103946832U,
			ErrorReasonItemTypeAllTypes = 2566788983U,
			ErrorMasterTableSaveConflict = 1057073101U,
			InvalidConfigurationMissingFacebookAppId = 3960918573U,
			ErrorCantOverwriteDefaultExtension = 3353268377U,
			ErrorReasonOnlySelectedEntitiesInRestricted = 4017558222U,
			UserPhotoAccessDenied = 1445064246U,
			ErrorReasonItemIsRuleAttributesNotValidForEdit = 3896524780U,
			ErrorReasonNoRegexRuleInRestricted = 1303673802U,
			ErrorManifestSignatureInvalidExtension = 2340791895U,
			InvalidConfigurationMissingLinkedInAppSecret = 921746356U,
			ErrorReasonRegExNameAndValueRequiredInEntitiesRules = 103323614U,
			SecureStringParameter = 531489075U,
			InvalidConfigurationFacebookAppSecret = 3364919108U,
			InvalidConfigurationMissingFacebookGraphTokenEndpoint = 163140049U,
			InvalidConfigurationLinkedInAppId = 3689335943U,
			ErrorOrgLevelAppMustBeSiteLicense = 3369915033U,
			ErrorMarketplaceWebServicesUnavailable = 2727153851U,
			InvalidConfigurationMissingFacebookGraphApiEndpoint = 2267978298U,
			InvalidConfigurationLinkedInAppSecret = 3571146374U,
			InvalidConfigurationMissingFacebookAuthorizationEndpoint = 3104695745U,
			InvalidConfigurationMissingLinkedInProfileEndpoint = 3368339595U,
			FailureReasonNoAttributes = 1184760770U,
			ErrorReasonRegexRuleInvalidValue = 3864317541U,
			InvalidConfigurationMissingLinkedInInvalidateTokenEndpoint = 3958377408U,
			ErrorReasonMinVersionIsInvalid = 1579586375U,
			ErrorReasonMissingOfficeApp = 190956125U,
			ErrorReasonOnlyMailboxSetAllowedInRequirement = 1368134268U,
			ErrorReasonInvalidID = 2777862026U,
			ErrorReasonFormInFormSettingsNotUnique = 330725840U,
			WelcomeMailBodyNote = 2661873235U,
			ErrorExtensionWithIdAlreadyInstalled = 1979709530U
		}

		private enum ParamIDs
		{
			ErrorEtokenWithInvalidDeploymentId,
			ErrorProtocolConfigurationMissing,
			ErrorCanNotReadInstalledList,
			FailureReasonAttributeMissing,
			ADUserNoPhoto,
			CannotMapInvalidSmtpAddressToPhotoFile,
			ErrorReasonTooManyRule,
			FailureReasonUserMasterTableInvalidScope,
			ErrorInvalidManifestData,
			ErrorReasonInvalidXml,
			ErrorCouldNotRegisterComponent,
			ErrorReasonUrlMustBeAbsolute,
			ErrorMissingNodeInEtoken,
			ErrorReasonManifestVersionNotSupported,
			UserPhotoWithClassNotFound,
			ErrorReasonManifestTooLarge,
			ErrorCannotUninstallProvidedExtension,
			ErrorReasonMultipleRulesWithSameRegExName,
			UserPhotoThumbprintNotFound,
			ErrorReasonInvalidUrlValue,
			FailedToFindEwsEndpoint,
			ErrorVDirConfigurationMissing,
			WelcomeMailBodyMain,
			FailedToReadProviderConfigurationSeeInnerException,
			ErrorReasonMinApiVersionNotSupported,
			ErrorExtensionUnableToUpgrade,
			FailureReasonOrgMasterTableInvalidScope,
			ErrorReasonUnsupportedUrlScheme,
			ErrorReasonTooManyRegexRule,
			FailureReasonAttributeValueInvalid,
			ErrorExtensionNotFound,
			UserPhotoTooManyItems,
			FailedToAccessAuditLog,
			ErrorReasonMultipleRulesWithSameFilterName,
			FailureReasonTagValueInvalid,
			FailureReasonTagMissing,
			ErrorReasonManifestValidationError,
			ErrorReasonInvalidRegEx,
			ErrorAssetIdNotMatchInEtoken
		}
	}
}
