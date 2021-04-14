using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.Transport
{
	internal static class Strings
	{
		static Strings()
		{
			Strings.stringIDs.Add(2376986056U, "ErrorOnlyAllowInEop");
			Strings.stringIDs.Add(2845151070U, "InvalidContentDateFromAndContentDateToPredicate");
			Strings.stringIDs.Add(3231761178U, "PolicyNotifyErrorErrorMsg");
			Strings.stringIDs.Add(1183239240U, "CannotChangeDeviceConfigurationPolicyWorkload");
			Strings.stringIDs.Add(2160431466U, "CannotChangeDeviceConditionalAccessRuleName");
			Strings.stringIDs.Add(857423920U, "VerboseValidatingAddSharepointBinding");
			Strings.stringIDs.Add(629789999U, "UnexpectedConditionOrActionDetected");
			Strings.stringIDs.Add(107430575U, "SiteOutOfQuotaErrorMsg");
			Strings.stringIDs.Add(464180248U, "CanOnlyChangeDeviceConditionalAccessPolicy");
			Strings.stringIDs.Add(3201126371U, "InvalidCompliancePolicyWorkload");
			Strings.stringIDs.Add(178440487U, "VerboseValidatingExchangeBinding");
			Strings.stringIDs.Add(1412691609U, "InvalidAuditRuleWorkload");
			Strings.stringIDs.Add(4074170900U, "CanOnlyChangeDeviceConfigurationPolicy");
			Strings.stringIDs.Add(1306124383U, "VerboseValidatingRemoveSharepointBinding");
			Strings.stringIDs.Add(1508525573U, "CannotChangeDeviceConfigurationPolicyScenario");
			Strings.stringIDs.Add(3499233439U, "ShouldExpandGroups");
			Strings.stringIDs.Add(1830331352U, "InvalidDeviceRuleWorkload");
			Strings.stringIDs.Add(1930430118U, "ErrorInvalidPolicyCenterSiteOwner");
			Strings.stringIDs.Add(1713451535U, "CannotChangeAuditConfigurationRuleWorkload");
			Strings.stringIDs.Add(1005978434U, "FailedToOpenContainerErrorMsg");
			Strings.stringIDs.Add(2981356586U, "CannotChangeDeviceTenantRuleName");
			Strings.stringIDs.Add(228813078U, "CannotChangeDeviceConfigurationRuleName");
			Strings.stringIDs.Add(4091585218U, "InvalidAccessScopeIsPredicate");
			Strings.stringIDs.Add(2039724013U, "SpParserVersionNotSpecified");
			Strings.stringIDs.Add(1066947153U, "VerboseValidatingRemoveExchangeBinding");
			Strings.stringIDs.Add(317700209U, "InvalidHoldContentAction");
			Strings.stringIDs.Add(2522696186U, "CannotSetDeviceConfigurationPolicyWorkload");
			Strings.stringIDs.Add(167859032U, "CanOnlyManipulateDeviceConfigurationRule");
			Strings.stringIDs.Add(82360181U, "MulipleSpBindingObjectDetected");
			Strings.stringIDs.Add(605330666U, "ErrorNeedOrganizationId");
			Strings.stringIDs.Add(2976502162U, "CanOnlyChangeDeviceTenantPolicy");
			Strings.stringIDs.Add(631692221U, "SkippingInvalidTypeInGroupExpansion");
			Strings.stringIDs.Add(1853496069U, "FailedToGetExecutingUser");
			Strings.stringIDs.Add(1582525285U, "SensitiveInformationDoesNotContainId");
			Strings.stringIDs.Add(639724051U, "CannotChangeAuditConfigurationRuleName");
			Strings.stringIDs.Add(1614982109U, "PolicySyncTimeoutErrorMsg");
			Strings.stringIDs.Add(3781430831U, "MulipleExBindingObjectDetected");
			Strings.stringIDs.Add(766476708U, "CannotManipulateAuditConfigurationPolicy");
			Strings.stringIDs.Add(2985777218U, "CanOnlyManipulateDeviceTenantRule");
			Strings.stringIDs.Add(1450120914U, "ErrorSpBindingWithoutSpWorkload");
			Strings.stringIDs.Add(532439181U, "CannotManipulateDeviceConfigurationRule");
			Strings.stringIDs.Add(3642912594U, "ErrorExBindingWithoutExWorkload");
			Strings.stringIDs.Add(2717805860U, "CanOnlyManipulateDeviceConditionalAccessRule");
			Strings.stringIDs.Add(3278450928U, "DeploymentFailureWithNoImpact");
			Strings.stringIDs.Add(2628803538U, "FailedToGetSpSiteUrlForTenant");
			Strings.stringIDs.Add(1613362912U, "InvalidCombinationOfCompliancePolicyTypeAndWorkload");
			Strings.stringIDs.Add(912843721U, "FailedToGetCredentialsForTenant");
			Strings.stringIDs.Add(3928915344U, "InvalidContentPropertyContainsWordsPredicate");
			Strings.stringIDs.Add(518140498U, "VerboseValidatingAddExchangeBinding");
			Strings.stringIDs.Add(1502130987U, "CanOnlyManipulateAuditConfigurationPolicy");
			Strings.stringIDs.Add(574146044U, "VerboseRetryDistributionNotApplicable");
			Strings.stringIDs.Add(4099235009U, "UnknownErrorMsg");
			Strings.stringIDs.Add(1327345872U, "AuditConfigurationPolicyNotAllowed");
			Strings.stringIDs.Add(445162545U, "CanOnlyManipulateAuditConfigurationRule");
			Strings.stringIDs.Add(4174366101U, "DeviceConfigurationPolicyNotAllowed");
			Strings.stringIDs.Add(1615823280U, "CannotManipulateAuditConfigurationRule");
			Strings.stringIDs.Add(1905427409U, "SiteInReadonlyOrNotAccessibleErrorMsg");
			Strings.stringIDs.Add(2767511991U, "InvalidSensitiveInformationParameterValue");
		}

		public static LocalizedString SpParserInvalidVersionType(string version)
		{
			return new LocalizedString("SpParserInvalidVersionType", Strings.ResourceManager, new object[]
			{
				version
			});
		}

		public static LocalizedString ErrorCreateSiteTimeOut(string url)
		{
			return new LocalizedString("ErrorCreateSiteTimeOut", Strings.ResourceManager, new object[]
			{
				url
			});
		}

		public static LocalizedString VerboseBeginCalculatePolicyDistributionStatus(string name)
		{
			return new LocalizedString("VerboseBeginCalculatePolicyDistributionStatus", Strings.ResourceManager, new object[]
			{
				name
			});
		}

		public static LocalizedString ErrorMessageForNotificationFailure(string workload, string error)
		{
			return new LocalizedString("ErrorMessageForNotificationFailure", Strings.ResourceManager, new object[]
			{
				workload,
				error
			});
		}

		public static LocalizedString ErrorTaskRuleIsTooAdvancedToModify(string identity)
		{
			return new LocalizedString("ErrorTaskRuleIsTooAdvancedToModify", Strings.ResourceManager, new object[]
			{
				identity
			});
		}

		public static LocalizedString ErrorOnlyAllowInEop
		{
			get
			{
				return new LocalizedString("ErrorOnlyAllowInEop", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorCompliancePolicyHasNoObjectsToRetry(string name)
		{
			return new LocalizedString("ErrorCompliancePolicyHasNoObjectsToRetry", Strings.ResourceManager, new object[]
			{
				name
			});
		}

		public static LocalizedString ErrorCannotCreateRuleUnderPendingDeletionPolicy(string name)
		{
			return new LocalizedString("ErrorCannotCreateRuleUnderPendingDeletionPolicy", Strings.ResourceManager, new object[]
			{
				name
			});
		}

		public static LocalizedString InvalidContentDateFromAndContentDateToPredicate
		{
			get
			{
				return new LocalizedString("InvalidContentDateFromAndContentDateToPredicate", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString PolicyNotifyErrorErrorMsg
		{
			get
			{
				return new LocalizedString("PolicyNotifyErrorErrorMsg", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString VerbosePolicyStorageObjectLoadedForCommonRule(string storageObject, string policy)
		{
			return new LocalizedString("VerbosePolicyStorageObjectLoadedForCommonRule", Strings.ResourceManager, new object[]
			{
				storageObject,
				policy
			});
		}

		public static LocalizedString VerboseDeletePolicyStorageBaseObject(string name, string typeName)
		{
			return new LocalizedString("VerboseDeletePolicyStorageBaseObject", Strings.ResourceManager, new object[]
			{
				name,
				typeName
			});
		}

		public static LocalizedString CannotChangeDeviceConfigurationPolicyWorkload
		{
			get
			{
				return new LocalizedString("CannotChangeDeviceConfigurationPolicyWorkload", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString CannotChangeDeviceConditionalAccessRuleName
		{
			get
			{
				return new LocalizedString("CannotChangeDeviceConditionalAccessRuleName", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString VerboseValidatingAddSharepointBinding
		{
			get
			{
				return new LocalizedString("VerboseValidatingAddSharepointBinding", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString UnexpectedConditionOrActionDetected
		{
			get
			{
				return new LocalizedString("UnexpectedConditionOrActionDetected", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString SiteOutOfQuotaErrorMsg
		{
			get
			{
				return new LocalizedString("SiteOutOfQuotaErrorMsg", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString CanOnlyChangeDeviceConditionalAccessPolicy
		{
			get
			{
				return new LocalizedString("CanOnlyChangeDeviceConditionalAccessPolicy", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RemoveDeviceConditionalAccessRuleConfirmation(string ruleName)
		{
			return new LocalizedString("RemoveDeviceConditionalAccessRuleConfirmation", Strings.ResourceManager, new object[]
			{
				ruleName
			});
		}

		public static LocalizedString WarningTaskPolicyIsTooAdvancedToRead(string identity)
		{
			return new LocalizedString("WarningTaskPolicyIsTooAdvancedToRead", Strings.ResourceManager, new object[]
			{
				identity
			});
		}

		public static LocalizedString InvalidCompliancePolicyWorkload
		{
			get
			{
				return new LocalizedString("InvalidCompliancePolicyWorkload", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString DisplayBindingName(string workload)
		{
			return new LocalizedString("DisplayBindingName", Strings.ResourceManager, new object[]
			{
				workload
			});
		}

		public static LocalizedString VerboseTreatAsWarning(string endPoint, string objectType, string workload)
		{
			return new LocalizedString("VerboseTreatAsWarning", Strings.ResourceManager, new object[]
			{
				endPoint,
				objectType,
				workload
			});
		}

		public static LocalizedString SpParserInvalidSiteId(string siteId)
		{
			return new LocalizedString("SpParserInvalidSiteId", Strings.ResourceManager, new object[]
			{
				siteId
			});
		}

		public static LocalizedString ErrorMultipleObjectTypeForObjectLevelSync(string types)
		{
			return new LocalizedString("ErrorMultipleObjectTypeForObjectLevelSync", Strings.ResourceManager, new object[]
			{
				types
			});
		}

		public static LocalizedString VerboseValidatingExchangeBinding
		{
			get
			{
				return new LocalizedString("VerboseValidatingExchangeBinding", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString WarningDisabledRuleInEnabledPolicy(string ruleName)
		{
			return new LocalizedString("WarningDisabledRuleInEnabledPolicy", Strings.ResourceManager, new object[]
			{
				ruleName
			});
		}

		public static LocalizedString ErrorRuleContainsNoActions(string ruleName)
		{
			return new LocalizedString("ErrorRuleContainsNoActions", Strings.ResourceManager, new object[]
			{
				ruleName
			});
		}

		public static LocalizedString InvalidAuditRuleWorkload
		{
			get
			{
				return new LocalizedString("InvalidAuditRuleWorkload", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorCompliancePolicySyncNotificationClient(string workLoad, string reason)
		{
			return new LocalizedString("ErrorCompliancePolicySyncNotificationClient", Strings.ResourceManager, new object[]
			{
				workLoad,
				reason
			});
		}

		public static LocalizedString VerboseTrytoCheckSiteDeletedState(Uri siteUrl, string error)
		{
			return new LocalizedString("VerboseTrytoCheckSiteDeletedState", Strings.ResourceManager, new object[]
			{
				siteUrl,
				error
			});
		}

		public static LocalizedString CanOnlyChangeDeviceConfigurationPolicy
		{
			get
			{
				return new LocalizedString("CanOnlyChangeDeviceConfigurationPolicy", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString VerboseValidatingRemoveSharepointBinding
		{
			get
			{
				return new LocalizedString("VerboseValidatingRemoveSharepointBinding", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString CannotChangeDeviceConfigurationPolicyScenario
		{
			get
			{
				return new LocalizedString("CannotChangeDeviceConfigurationPolicyScenario", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString WarningInvalidTenant(string tenantId)
		{
			return new LocalizedString("WarningInvalidTenant", Strings.ResourceManager, new object[]
			{
				tenantId
			});
		}

		public static LocalizedString VerboseNotifyWorkloadWithChangesSuccess(string workload, string notificationIdentifier)
		{
			return new LocalizedString("VerboseNotifyWorkloadWithChangesSuccess", Strings.ResourceManager, new object[]
			{
				workload,
				notificationIdentifier
			});
		}

		public static LocalizedString ErrorRulesInPolicyIsTooAdvancedToModify(string policy, string rule)
		{
			return new LocalizedString("ErrorRulesInPolicyIsTooAdvancedToModify", Strings.ResourceManager, new object[]
			{
				policy,
				rule
			});
		}

		public static LocalizedString VerbosePolicyCenterSiteOwner(string address)
		{
			return new LocalizedString("VerbosePolicyCenterSiteOwner", Strings.ResourceManager, new object[]
			{
				address
			});
		}

		public static LocalizedString ShouldExpandGroups
		{
			get
			{
				return new LocalizedString("ShouldExpandGroups", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorCannotRemovePendingDeletionRule(string name)
		{
			return new LocalizedString("ErrorCannotRemovePendingDeletionRule", Strings.ResourceManager, new object[]
			{
				name
			});
		}

		public static LocalizedString ErrorSavingPolicyToWorkload(string policyName, string workloadName)
		{
			return new LocalizedString("ErrorSavingPolicyToWorkload", Strings.ResourceManager, new object[]
			{
				policyName,
				workloadName
			});
		}

		public static LocalizedString InvalidDeviceRuleWorkload
		{
			get
			{
				return new LocalizedString("InvalidDeviceRuleWorkload", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString VerbosePolicyStorageObjectLoaded(string storageObject)
		{
			return new LocalizedString("VerbosePolicyStorageObjectLoaded", Strings.ResourceManager, new object[]
			{
				storageObject
			});
		}

		public static LocalizedString ErrorInvalidPolicyCenterSiteOwner
		{
			get
			{
				return new LocalizedString("ErrorInvalidPolicyCenterSiteOwner", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString CannotChangeAuditConfigurationRuleWorkload
		{
			get
			{
				return new LocalizedString("CannotChangeAuditConfigurationRuleWorkload", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorCannotRemovePendingDeletionPolicy(string name)
		{
			return new LocalizedString("ErrorCannotRemovePendingDeletionPolicy", Strings.ResourceManager, new object[]
			{
				name
			});
		}

		public static LocalizedString FailedToOpenContainerErrorMsg
		{
			get
			{
				return new LocalizedString("FailedToOpenContainerErrorMsg", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InvalidSensitiveInformationParameterName(string invalidParameter)
		{
			return new LocalizedString("InvalidSensitiveInformationParameterName", Strings.ResourceManager, new object[]
			{
				invalidParameter
			});
		}

		public static LocalizedString CannotChangeDeviceTenantRuleName
		{
			get
			{
				return new LocalizedString("CannotChangeDeviceTenantRuleName", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString VerboseSpNotificationClientInfo(Uri spSiteUrl, Uri syncSvcUrl, string credentialType)
		{
			return new LocalizedString("VerboseSpNotificationClientInfo", Strings.ResourceManager, new object[]
			{
				spSiteUrl,
				syncSvcUrl,
				credentialType
			});
		}

		public static LocalizedString CannotChangeDeviceConfigurationRuleName
		{
			get
			{
				return new LocalizedString("CannotChangeDeviceConfigurationRuleName", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InvalidAccessScopeIsPredicate
		{
			get
			{
				return new LocalizedString("InvalidAccessScopeIsPredicate", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString VerboseTrytoCreatePolicyCenterSite(Uri siteUrl)
		{
			return new LocalizedString("VerboseTrytoCreatePolicyCenterSite", Strings.ResourceManager, new object[]
			{
				siteUrl
			});
		}

		public static LocalizedString ErrorPolicyNotFound(string policyParameter)
		{
			return new LocalizedString("ErrorPolicyNotFound", Strings.ResourceManager, new object[]
			{
				policyParameter
			});
		}

		public static LocalizedString SpParserVersionNotSpecified
		{
			get
			{
				return new LocalizedString("SpParserVersionNotSpecified", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorRuleNotUnique(string ruleParameter)
		{
			return new LocalizedString("ErrorRuleNotUnique", Strings.ResourceManager, new object[]
			{
				ruleParameter
			});
		}

		public static LocalizedString DeviceConfigurationRuleAlreadyExists(string ruleName)
		{
			return new LocalizedString("DeviceConfigurationRuleAlreadyExists", Strings.ResourceManager, new object[]
			{
				ruleName
			});
		}

		public static LocalizedString VerboseValidatingRemoveExchangeBinding
		{
			get
			{
				return new LocalizedString("VerboseValidatingRemoveExchangeBinding", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InvalidHoldContentAction
		{
			get
			{
				return new LocalizedString("InvalidHoldContentAction", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString CompliancePolicyCountExceedsLimit(int limit)
		{
			return new LocalizedString("CompliancePolicyCountExceedsLimit", Strings.ResourceManager, new object[]
			{
				limit
			});
		}

		public static LocalizedString ExCannotContainWideScopeBindings(string binding)
		{
			return new LocalizedString("ExCannotContainWideScopeBindings", Strings.ResourceManager, new object[]
			{
				binding
			});
		}

		public static LocalizedString RemoveDeviceTenantRuleConfirmation(string ruleName)
		{
			return new LocalizedString("RemoveDeviceTenantRuleConfirmation", Strings.ResourceManager, new object[]
			{
				ruleName
			});
		}

		public static LocalizedString ErrorCommonComplianceRuleIsDeleted(string name)
		{
			return new LocalizedString("ErrorCommonComplianceRuleIsDeleted", Strings.ResourceManager, new object[]
			{
				name
			});
		}

		public static LocalizedString CannotSetDeviceConfigurationPolicyWorkload
		{
			get
			{
				return new LocalizedString("CannotSetDeviceConfigurationPolicyWorkload", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorRuleNotFound(string ruleId)
		{
			return new LocalizedString("ErrorRuleNotFound", Strings.ResourceManager, new object[]
			{
				ruleId
			});
		}

		public static LocalizedString CanOnlyManipulateDeviceConfigurationRule
		{
			get
			{
				return new LocalizedString("CanOnlyManipulateDeviceConfigurationRule", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString VerboseValidatingSharepointBinding(string workload)
		{
			return new LocalizedString("VerboseValidatingSharepointBinding", Strings.ResourceManager, new object[]
			{
				workload
			});
		}

		public static LocalizedString MulipleSpBindingObjectDetected
		{
			get
			{
				return new LocalizedString("MulipleSpBindingObjectDetected", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RemoveCompliancePolicyConfirmation(string policyName)
		{
			return new LocalizedString("RemoveCompliancePolicyConfirmation", Strings.ResourceManager, new object[]
			{
				policyName
			});
		}

		public static LocalizedString ErrorInvalidDeltaSyncAndFullSyncType(string types)
		{
			return new LocalizedString("ErrorInvalidDeltaSyncAndFullSyncType", Strings.ResourceManager, new object[]
			{
				types
			});
		}

		public static LocalizedString ErrorNeedOrganizationId
		{
			get
			{
				return new LocalizedString("ErrorNeedOrganizationId", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorCannotInitializeNotificationClientToSharePoint(Uri spSiteUrl, Uri spAdminSiteUrl, Uri syncSvcUrl)
		{
			return new LocalizedString("ErrorCannotInitializeNotificationClientToSharePoint", Strings.ResourceManager, new object[]
			{
				spSiteUrl,
				spAdminSiteUrl,
				syncSvcUrl
			});
		}

		public static LocalizedString CanOnlyChangeDeviceTenantPolicy
		{
			get
			{
				return new LocalizedString("CanOnlyChangeDeviceTenantPolicy", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString SkippingInvalidTypeInGroupExpansion
		{
			get
			{
				return new LocalizedString("SkippingInvalidTypeInGroupExpansion", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString FailedToGetExecutingUser
		{
			get
			{
				return new LocalizedString("FailedToGetExecutingUser", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString SensitiveInformationDoesNotContainId
		{
			get
			{
				return new LocalizedString("SensitiveInformationDoesNotContainId", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString CannotChangeAuditConfigurationRuleName
		{
			get
			{
				return new LocalizedString("CannotChangeAuditConfigurationRuleName", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorAddRemoveExBindingsOverlapped(string bindings)
		{
			return new LocalizedString("ErrorAddRemoveExBindingsOverlapped", Strings.ResourceManager, new object[]
			{
				bindings
			});
		}

		public static LocalizedString WarningTaskRuleIsTooAdvancedToRead(string identity)
		{
			return new LocalizedString("WarningTaskRuleIsTooAdvancedToRead", Strings.ResourceManager, new object[]
			{
				identity
			});
		}

		public static LocalizedString VerboseNotifyWorkloadWithChanges(string workload, string changes)
		{
			return new LocalizedString("VerboseNotifyWorkloadWithChanges", Strings.ResourceManager, new object[]
			{
				workload,
				changes
			});
		}

		public static LocalizedString PolicySyncTimeoutErrorMsg
		{
			get
			{
				return new LocalizedString("PolicySyncTimeoutErrorMsg", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString DiagnoseMissingStatusForScope(DateTime whenChanged)
		{
			return new LocalizedString("DiagnoseMissingStatusForScope", Strings.ResourceManager, new object[]
			{
				whenChanged
			});
		}

		public static LocalizedString ErrorCannotInitializeNotificationClientToExchange(Uri pswsHostUrl, Uri syncSvcUrl)
		{
			return new LocalizedString("ErrorCannotInitializeNotificationClientToExchange", Strings.ResourceManager, new object[]
			{
				pswsHostUrl,
				syncSvcUrl
			});
		}

		public static LocalizedString SpLocationValidationFailed(string url)
		{
			return new LocalizedString("SpLocationValidationFailed", Strings.ResourceManager, new object[]
			{
				url
			});
		}

		public static LocalizedString VerboseEndCalculatePolicyDistributionStatus(string name, string state, int errorCount, int timeoutErrorCount)
		{
			return new LocalizedString("VerboseEndCalculatePolicyDistributionStatus", Strings.ResourceManager, new object[]
			{
				name,
				state,
				errorCount,
				timeoutErrorCount
			});
		}

		public static LocalizedString DiagnosePendingStatusTimeout(DateTime whenChanged, TimeSpan timeout)
		{
			return new LocalizedString("DiagnosePendingStatusTimeout", Strings.ResourceManager, new object[]
			{
				whenChanged,
				timeout
			});
		}

		public static LocalizedString WarningNotificationClientIsMissing(string workload)
		{
			return new LocalizedString("WarningNotificationClientIsMissing", Strings.ResourceManager, new object[]
			{
				workload
			});
		}

		public static LocalizedString SensitiveInformationNotFound(string identity)
		{
			return new LocalizedString("SensitiveInformationNotFound", Strings.ResourceManager, new object[]
			{
				identity
			});
		}

		public static LocalizedString MulipleExBindingObjectDetected
		{
			get
			{
				return new LocalizedString("MulipleExBindingObjectDetected", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString GroupsIsNotAllowedForHold(string group)
		{
			return new LocalizedString("GroupsIsNotAllowedForHold", Strings.ResourceManager, new object[]
			{
				group
			});
		}

		public static LocalizedString RemoveDeviceConfiguationRuleConfirmation(string ruleName)
		{
			return new LocalizedString("RemoveDeviceConfiguationRuleConfirmation", Strings.ResourceManager, new object[]
			{
				ruleName
			});
		}

		public static LocalizedString CannotManipulateAuditConfigurationPolicy
		{
			get
			{
				return new LocalizedString("CannotManipulateAuditConfigurationPolicy", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString DisplayRuleName(string name)
		{
			return new LocalizedString("DisplayRuleName", Strings.ResourceManager, new object[]
			{
				name
			});
		}

		public static LocalizedString RemoveComplianceRuleConfirmation(string ruleName)
		{
			return new LocalizedString("RemoveComplianceRuleConfirmation", Strings.ResourceManager, new object[]
			{
				ruleName
			});
		}

		public static LocalizedString VerboseLoadBindingStorageObjects(string bindingObject, string policy)
		{
			return new LocalizedString("VerboseLoadBindingStorageObjects", Strings.ResourceManager, new object[]
			{
				bindingObject,
				policy
			});
		}

		public static LocalizedString DeviceConditionalAccessRuleAlreadyExists(string ruleName)
		{
			return new LocalizedString("DeviceConditionalAccessRuleAlreadyExists", Strings.ResourceManager, new object[]
			{
				ruleName
			});
		}

		public static LocalizedString SpParserUnexpectedNumberOfTokens(int version, int expected, int actual)
		{
			return new LocalizedString("SpParserUnexpectedNumberOfTokens", Strings.ResourceManager, new object[]
			{
				version,
				expected,
				actual
			});
		}

		public static LocalizedString BindingCannotCombineAllWithIndividualBindings(string workLoad)
		{
			return new LocalizedString("BindingCannotCombineAllWithIndividualBindings", Strings.ResourceManager, new object[]
			{
				workLoad
			});
		}

		public static LocalizedString CompliancePolicyAlreadyExists(string policyName)
		{
			return new LocalizedString("CompliancePolicyAlreadyExists", Strings.ResourceManager, new object[]
			{
				policyName
			});
		}

		public static LocalizedString CanOnlyManipulateDeviceTenantRule
		{
			get
			{
				return new LocalizedString("CanOnlyManipulateDeviceTenantRule", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MulipleComplianceRulesFoundInPolicy(string policy)
		{
			return new LocalizedString("MulipleComplianceRulesFoundInPolicy", Strings.ResourceManager, new object[]
			{
				policy
			});
		}

		public static LocalizedString ErrorPolicyNotUnique(string policyParameter)
		{
			return new LocalizedString("ErrorPolicyNotUnique", Strings.ResourceManager, new object[]
			{
				policyParameter
			});
		}

		public static LocalizedString VerboseExNotificationClientInfo(Uri pswsHostUrl, Uri syncSvcUrl, string credentialType)
		{
			return new LocalizedString("VerboseExNotificationClientInfo", Strings.ResourceManager, new object[]
			{
				pswsHostUrl,
				syncSvcUrl,
				credentialType
			});
		}

		public static LocalizedString ErrorSpBindingWithoutSpWorkload
		{
			get
			{
				return new LocalizedString("ErrorSpBindingWithoutSpWorkload", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString CannotManipulateDeviceConfigurationRule
		{
			get
			{
				return new LocalizedString("CannotManipulateDeviceConfigurationRule", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString WarningNotifyWorkloadFailed(string workload)
		{
			return new LocalizedString("WarningNotifyWorkloadFailed", Strings.ResourceManager, new object[]
			{
				workload
			});
		}

		public static LocalizedString ErrorInvalidRecipientType(string recipientName, string recipientType)
		{
			return new LocalizedString("ErrorInvalidRecipientType", Strings.ResourceManager, new object[]
			{
				recipientName,
				recipientType
			});
		}

		public static LocalizedString ErrorInvalidPolicyCenterSiteUrl(string policyCenterSiteUrlStr)
		{
			return new LocalizedString("ErrorInvalidPolicyCenterSiteUrl", Strings.ResourceManager, new object[]
			{
				policyCenterSiteUrlStr
			});
		}

		public static LocalizedString ErrorInvalidObjectSyncType(string types)
		{
			return new LocalizedString("ErrorInvalidObjectSyncType", Strings.ResourceManager, new object[]
			{
				types
			});
		}

		public static LocalizedString ErrorExBindingWithoutExWorkload
		{
			get
			{
				return new LocalizedString("ErrorExBindingWithoutExWorkload", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString BindingCountExceedsLimit(string workLoad, int limit)
		{
			return new LocalizedString("BindingCountExceedsLimit", Strings.ResourceManager, new object[]
			{
				workLoad,
				limit
			});
		}

		public static LocalizedString CanOnlyManipulateDeviceConditionalAccessRule
		{
			get
			{
				return new LocalizedString("CanOnlyManipulateDeviceConditionalAccessRule", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString VerboseRetryDistributionNotificationDetails(string id, string objectType, string changeType)
		{
			return new LocalizedString("VerboseRetryDistributionNotificationDetails", Strings.ResourceManager, new object[]
			{
				id,
				objectType,
				changeType
			});
		}

		public static LocalizedString SpLocationHasMultipleSites(string url)
		{
			return new LocalizedString("SpLocationHasMultipleSites", Strings.ResourceManager, new object[]
			{
				url
			});
		}

		public static LocalizedString DeploymentFailureWithNoImpact
		{
			get
			{
				return new LocalizedString("DeploymentFailureWithNoImpact", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString PolicyAndIdentityParameterUsedTogether(string policy, string identity)
		{
			return new LocalizedString("PolicyAndIdentityParameterUsedTogether", Strings.ResourceManager, new object[]
			{
				policy,
				identity
			});
		}

		public static LocalizedString FailedToGetSpSiteUrlForTenant
		{
			get
			{
				return new LocalizedString("FailedToGetSpSiteUrlForTenant", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString VerboseTryLoadPolicyCenterSite(Uri siteUrl)
		{
			return new LocalizedString("VerboseTryLoadPolicyCenterSite", Strings.ResourceManager, new object[]
			{
				siteUrl
			});
		}

		public static LocalizedString ErrorMaxSiteLimit(int limit, int actual)
		{
			return new LocalizedString("ErrorMaxSiteLimit", Strings.ResourceManager, new object[]
			{
				limit,
				actual
			});
		}

		public static LocalizedString InvalidCombinationOfCompliancePolicyTypeAndWorkload
		{
			get
			{
				return new LocalizedString("InvalidCombinationOfCompliancePolicyTypeAndWorkload", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString SpParserVersionNotSupported(int version)
		{
			return new LocalizedString("SpParserVersionNotSupported", Strings.ResourceManager, new object[]
			{
				version
			});
		}

		public static LocalizedString WarningFailurePublishingStatus(string error)
		{
			return new LocalizedString("WarningFailurePublishingStatus", Strings.ResourceManager, new object[]
			{
				error
			});
		}

		public static LocalizedString SpParserInvalidSiteUrl(string siteUrl)
		{
			return new LocalizedString("SpParserInvalidSiteUrl", Strings.ResourceManager, new object[]
			{
				siteUrl
			});
		}

		public static LocalizedString FailedToGetCredentialsForTenant
		{
			get
			{
				return new LocalizedString("FailedToGetCredentialsForTenant", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString WarningPolicyContainsDisabledRules(string policyName)
		{
			return new LocalizedString("WarningPolicyContainsDisabledRules", Strings.ResourceManager, new object[]
			{
				policyName
			});
		}

		public static LocalizedString InvalidContentPropertyContainsWordsPredicate
		{
			get
			{
				return new LocalizedString("InvalidContentPropertyContainsWordsPredicate", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorAddRemoveSpBindingsOverlapped(string bindings)
		{
			return new LocalizedString("ErrorAddRemoveSpBindingsOverlapped", Strings.ResourceManager, new object[]
			{
				bindings
			});
		}

		public static LocalizedString VerboseCreateNotificationClient(string endPoint)
		{
			return new LocalizedString("VerboseCreateNotificationClient", Strings.ResourceManager, new object[]
			{
				endPoint
			});
		}

		public static LocalizedString ErrorUserObjectNotFound(string id)
		{
			return new LocalizedString("ErrorUserObjectNotFound", Strings.ResourceManager, new object[]
			{
				id
			});
		}

		public static LocalizedString ErrorMaxMailboxLimit(int limit, int actual)
		{
			return new LocalizedString("ErrorMaxMailboxLimit", Strings.ResourceManager, new object[]
			{
				limit,
				actual
			});
		}

		public static LocalizedString SpParserInvalidWebId(string webId)
		{
			return new LocalizedString("SpParserInvalidWebId", Strings.ResourceManager, new object[]
			{
				webId
			});
		}

		public static LocalizedString VerboseValidatingAddExchangeBinding
		{
			get
			{
				return new LocalizedString("VerboseValidatingAddExchangeBinding", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString VerboseDumpStatusObject(string workload, string objectType, string objectId, string objectVersion, string statusErrorCode, string statusVersion)
		{
			return new LocalizedString("VerboseDumpStatusObject", Strings.ResourceManager, new object[]
			{
				workload,
				objectType,
				objectId,
				objectVersion,
				statusErrorCode,
				statusVersion
			});
		}

		public static LocalizedString CanOnlyManipulateAuditConfigurationPolicy
		{
			get
			{
				return new LocalizedString("CanOnlyManipulateAuditConfigurationPolicy", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorMaxMailboxLimitReachedInGroupExpansion(int limit)
		{
			return new LocalizedString("ErrorMaxMailboxLimitReachedInGroupExpansion", Strings.ResourceManager, new object[]
			{
				limit
			});
		}

		public static LocalizedString ComplianceRuleAlreadyExists(string ruleName)
		{
			return new LocalizedString("ComplianceRuleAlreadyExists", Strings.ResourceManager, new object[]
			{
				ruleName
			});
		}

		public static LocalizedString VerboseRetryDistributionNotApplicable
		{
			get
			{
				return new LocalizedString("VerboseRetryDistributionNotApplicable", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString UnknownErrorMsg
		{
			get
			{
				return new LocalizedString("UnknownErrorMsg", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString AuditConfigurationPolicyNotAllowed
		{
			get
			{
				return new LocalizedString("AuditConfigurationPolicyNotAllowed", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString CanOnlyManipulateAuditConfigurationRule
		{
			get
			{
				return new LocalizedString("CanOnlyManipulateAuditConfigurationRule", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString VerboseSaveBindingStorageObjects(string bindingObject, string policy)
		{
			return new LocalizedString("VerboseSaveBindingStorageObjects", Strings.ResourceManager, new object[]
			{
				bindingObject,
				policy
			});
		}

		public static LocalizedString DeviceTenantRuleAlreadyExists(string ruleName)
		{
			return new LocalizedString("DeviceTenantRuleAlreadyExists", Strings.ResourceManager, new object[]
			{
				ruleName
			});
		}

		public static LocalizedString DeviceConfigurationPolicyNotAllowed
		{
			get
			{
				return new LocalizedString("DeviceConfigurationPolicyNotAllowed", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString DisplayPolicyName(string name)
		{
			return new LocalizedString("DisplayPolicyName", Strings.ResourceManager, new object[]
			{
				name
			});
		}

		public static LocalizedString ErrorUserObjectAmbiguous(string id)
		{
			return new LocalizedString("ErrorUserObjectAmbiguous", Strings.ResourceManager, new object[]
			{
				id
			});
		}

		public static LocalizedString SpParserUnexpectedContainerType(string expected, string actual)
		{
			return new LocalizedString("SpParserUnexpectedContainerType", Strings.ResourceManager, new object[]
			{
				expected,
				actual
			});
		}

		public static LocalizedString CannotManipulateAuditConfigurationRule
		{
			get
			{
				return new LocalizedString("CannotManipulateAuditConfigurationRule", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorSharePointCallFailed(string reason)
		{
			return new LocalizedString("ErrorSharePointCallFailed", Strings.ResourceManager, new object[]
			{
				reason
			});
		}

		public static LocalizedString ErrorTaskPolicyIsTooAdvancedToModify(string identity)
		{
			return new LocalizedString("ErrorTaskPolicyIsTooAdvancedToModify", Strings.ResourceManager, new object[]
			{
				identity
			});
		}

		public static LocalizedString VerboseTrytoCheckSiteExistence(Uri siteUrl, string error)
		{
			return new LocalizedString("VerboseTrytoCheckSiteExistence", Strings.ResourceManager, new object[]
			{
				siteUrl,
				error
			});
		}

		public static LocalizedString SiteInReadonlyOrNotAccessibleErrorMsg
		{
			get
			{
				return new LocalizedString("SiteInReadonlyOrNotAccessibleErrorMsg", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString VerboseDeleteBindingStorageObjects(string bindingObject, string policy)
		{
			return new LocalizedString("VerboseDeleteBindingStorageObjects", Strings.ResourceManager, new object[]
			{
				bindingObject,
				policy
			});
		}

		public static LocalizedString ErrorCompliancePolicyIsDeleted(string name)
		{
			return new LocalizedString("ErrorCompliancePolicyIsDeleted", Strings.ResourceManager, new object[]
			{
				name
			});
		}

		public static LocalizedString VerboseRetryDistributionNotifyingWorkload(string workload, string syncLevel)
		{
			return new LocalizedString("VerboseRetryDistributionNotifyingWorkload", Strings.ResourceManager, new object[]
			{
				workload,
				syncLevel
			});
		}

		public static LocalizedString VerboseLoadRuleStorageObjectsForPolicy(string ruleObject, string policy)
		{
			return new LocalizedString("VerboseLoadRuleStorageObjectsForPolicy", Strings.ResourceManager, new object[]
			{
				ruleObject,
				policy
			});
		}

		public static LocalizedString InvalidSensitiveInformationParameterValue
		{
			get
			{
				return new LocalizedString("InvalidSensitiveInformationParameterValue", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString GetLocalizedString(Strings.IDs key)
		{
			return new LocalizedString(Strings.stringIDs[(uint)key], Strings.ResourceManager, new object[0]);
		}

		private static Dictionary<uint, string> stringIDs = new Dictionary<uint, string>(58);

		private static ExchangeResourceManager ResourceManager = ExchangeResourceManager.GetResourceManager("Microsoft.Exchange.Management.Transport.Strings", typeof(Strings).GetTypeInfo().Assembly);

		public enum IDs : uint
		{
			ErrorOnlyAllowInEop = 2376986056U,
			InvalidContentDateFromAndContentDateToPredicate = 2845151070U,
			PolicyNotifyErrorErrorMsg = 3231761178U,
			CannotChangeDeviceConfigurationPolicyWorkload = 1183239240U,
			CannotChangeDeviceConditionalAccessRuleName = 2160431466U,
			VerboseValidatingAddSharepointBinding = 857423920U,
			UnexpectedConditionOrActionDetected = 629789999U,
			SiteOutOfQuotaErrorMsg = 107430575U,
			CanOnlyChangeDeviceConditionalAccessPolicy = 464180248U,
			InvalidCompliancePolicyWorkload = 3201126371U,
			VerboseValidatingExchangeBinding = 178440487U,
			InvalidAuditRuleWorkload = 1412691609U,
			CanOnlyChangeDeviceConfigurationPolicy = 4074170900U,
			VerboseValidatingRemoveSharepointBinding = 1306124383U,
			CannotChangeDeviceConfigurationPolicyScenario = 1508525573U,
			ShouldExpandGroups = 3499233439U,
			InvalidDeviceRuleWorkload = 1830331352U,
			ErrorInvalidPolicyCenterSiteOwner = 1930430118U,
			CannotChangeAuditConfigurationRuleWorkload = 1713451535U,
			FailedToOpenContainerErrorMsg = 1005978434U,
			CannotChangeDeviceTenantRuleName = 2981356586U,
			CannotChangeDeviceConfigurationRuleName = 228813078U,
			InvalidAccessScopeIsPredicate = 4091585218U,
			SpParserVersionNotSpecified = 2039724013U,
			VerboseValidatingRemoveExchangeBinding = 1066947153U,
			InvalidHoldContentAction = 317700209U,
			CannotSetDeviceConfigurationPolicyWorkload = 2522696186U,
			CanOnlyManipulateDeviceConfigurationRule = 167859032U,
			MulipleSpBindingObjectDetected = 82360181U,
			ErrorNeedOrganizationId = 605330666U,
			CanOnlyChangeDeviceTenantPolicy = 2976502162U,
			SkippingInvalidTypeInGroupExpansion = 631692221U,
			FailedToGetExecutingUser = 1853496069U,
			SensitiveInformationDoesNotContainId = 1582525285U,
			CannotChangeAuditConfigurationRuleName = 639724051U,
			PolicySyncTimeoutErrorMsg = 1614982109U,
			MulipleExBindingObjectDetected = 3781430831U,
			CannotManipulateAuditConfigurationPolicy = 766476708U,
			CanOnlyManipulateDeviceTenantRule = 2985777218U,
			ErrorSpBindingWithoutSpWorkload = 1450120914U,
			CannotManipulateDeviceConfigurationRule = 532439181U,
			ErrorExBindingWithoutExWorkload = 3642912594U,
			CanOnlyManipulateDeviceConditionalAccessRule = 2717805860U,
			DeploymentFailureWithNoImpact = 3278450928U,
			FailedToGetSpSiteUrlForTenant = 2628803538U,
			InvalidCombinationOfCompliancePolicyTypeAndWorkload = 1613362912U,
			FailedToGetCredentialsForTenant = 912843721U,
			InvalidContentPropertyContainsWordsPredicate = 3928915344U,
			VerboseValidatingAddExchangeBinding = 518140498U,
			CanOnlyManipulateAuditConfigurationPolicy = 1502130987U,
			VerboseRetryDistributionNotApplicable = 574146044U,
			UnknownErrorMsg = 4099235009U,
			AuditConfigurationPolicyNotAllowed = 1327345872U,
			CanOnlyManipulateAuditConfigurationRule = 445162545U,
			DeviceConfigurationPolicyNotAllowed = 4174366101U,
			CannotManipulateAuditConfigurationRule = 1615823280U,
			SiteInReadonlyOrNotAccessibleErrorMsg = 1905427409U,
			InvalidSensitiveInformationParameterValue = 2767511991U
		}

		private enum ParamIDs
		{
			SpParserInvalidVersionType,
			ErrorCreateSiteTimeOut,
			VerboseBeginCalculatePolicyDistributionStatus,
			ErrorMessageForNotificationFailure,
			ErrorTaskRuleIsTooAdvancedToModify,
			ErrorCompliancePolicyHasNoObjectsToRetry,
			ErrorCannotCreateRuleUnderPendingDeletionPolicy,
			VerbosePolicyStorageObjectLoadedForCommonRule,
			VerboseDeletePolicyStorageBaseObject,
			RemoveDeviceConditionalAccessRuleConfirmation,
			WarningTaskPolicyIsTooAdvancedToRead,
			DisplayBindingName,
			VerboseTreatAsWarning,
			SpParserInvalidSiteId,
			ErrorMultipleObjectTypeForObjectLevelSync,
			WarningDisabledRuleInEnabledPolicy,
			ErrorRuleContainsNoActions,
			ErrorCompliancePolicySyncNotificationClient,
			VerboseTrytoCheckSiteDeletedState,
			WarningInvalidTenant,
			VerboseNotifyWorkloadWithChangesSuccess,
			ErrorRulesInPolicyIsTooAdvancedToModify,
			VerbosePolicyCenterSiteOwner,
			ErrorCannotRemovePendingDeletionRule,
			ErrorSavingPolicyToWorkload,
			VerbosePolicyStorageObjectLoaded,
			ErrorCannotRemovePendingDeletionPolicy,
			InvalidSensitiveInformationParameterName,
			VerboseSpNotificationClientInfo,
			VerboseTrytoCreatePolicyCenterSite,
			ErrorPolicyNotFound,
			ErrorRuleNotUnique,
			DeviceConfigurationRuleAlreadyExists,
			CompliancePolicyCountExceedsLimit,
			ExCannotContainWideScopeBindings,
			RemoveDeviceTenantRuleConfirmation,
			ErrorCommonComplianceRuleIsDeleted,
			ErrorRuleNotFound,
			VerboseValidatingSharepointBinding,
			RemoveCompliancePolicyConfirmation,
			ErrorInvalidDeltaSyncAndFullSyncType,
			ErrorCannotInitializeNotificationClientToSharePoint,
			ErrorAddRemoveExBindingsOverlapped,
			WarningTaskRuleIsTooAdvancedToRead,
			VerboseNotifyWorkloadWithChanges,
			DiagnoseMissingStatusForScope,
			ErrorCannotInitializeNotificationClientToExchange,
			SpLocationValidationFailed,
			VerboseEndCalculatePolicyDistributionStatus,
			DiagnosePendingStatusTimeout,
			WarningNotificationClientIsMissing,
			SensitiveInformationNotFound,
			GroupsIsNotAllowedForHold,
			RemoveDeviceConfiguationRuleConfirmation,
			DisplayRuleName,
			RemoveComplianceRuleConfirmation,
			VerboseLoadBindingStorageObjects,
			DeviceConditionalAccessRuleAlreadyExists,
			SpParserUnexpectedNumberOfTokens,
			BindingCannotCombineAllWithIndividualBindings,
			CompliancePolicyAlreadyExists,
			MulipleComplianceRulesFoundInPolicy,
			ErrorPolicyNotUnique,
			VerboseExNotificationClientInfo,
			WarningNotifyWorkloadFailed,
			ErrorInvalidRecipientType,
			ErrorInvalidPolicyCenterSiteUrl,
			ErrorInvalidObjectSyncType,
			BindingCountExceedsLimit,
			VerboseRetryDistributionNotificationDetails,
			SpLocationHasMultipleSites,
			PolicyAndIdentityParameterUsedTogether,
			VerboseTryLoadPolicyCenterSite,
			ErrorMaxSiteLimit,
			SpParserVersionNotSupported,
			WarningFailurePublishingStatus,
			SpParserInvalidSiteUrl,
			WarningPolicyContainsDisabledRules,
			ErrorAddRemoveSpBindingsOverlapped,
			VerboseCreateNotificationClient,
			ErrorUserObjectNotFound,
			ErrorMaxMailboxLimit,
			SpParserInvalidWebId,
			VerboseDumpStatusObject,
			ErrorMaxMailboxLimitReachedInGroupExpansion,
			ComplianceRuleAlreadyExists,
			VerboseSaveBindingStorageObjects,
			DeviceTenantRuleAlreadyExists,
			DisplayPolicyName,
			ErrorUserObjectAmbiguous,
			SpParserUnexpectedContainerType,
			ErrorSharePointCallFailed,
			ErrorTaskPolicyIsTooAdvancedToModify,
			VerboseTrytoCheckSiteExistence,
			VerboseDeleteBindingStorageObjects,
			ErrorCompliancePolicyIsDeleted,
			VerboseRetryDistributionNotifyingWorkload,
			VerboseLoadRuleStorageObjectsForPolicy
		}
	}
}
