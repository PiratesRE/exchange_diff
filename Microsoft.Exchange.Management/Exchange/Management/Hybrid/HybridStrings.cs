using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.Hybrid
{
	internal static class HybridStrings
	{
		static HybridStrings()
		{
			HybridStrings.stringIDs.Add(2425718147U, "ErrorHybridUpgradedTo2013");
			HybridStrings.stringIDs.Add(270912202U, "ErrorMultipleFederationTrusts");
			HybridStrings.stringIDs.Add(820521406U, "ErrorHybridMustBeUpgraded");
			HybridStrings.stringIDs.Add(4267730599U, "ErrorNoFederatedDomainsOnTenant");
			HybridStrings.stringIDs.Add(1424719599U, "HybridErrorBothTransportServersNotSet");
			HybridStrings.stringIDs.Add(2925976073U, "ErrorNoHybridDomain");
			HybridStrings.stringIDs.Add(3187185748U, "HybridInfoObjectNotFound");
			HybridStrings.stringIDs.Add(2511569494U, "ErrorMultipleMatchingOrgRelationships");
			HybridStrings.stringIDs.Add(3431283964U, "ErrorOnPremUsingConsumerLiveID");
			HybridStrings.stringIDs.Add(4267028265U, "ErrorFederationIDNotProvisioned");
			HybridStrings.stringIDs.Add(4281839122U, "ErrorHybridClientAccessServersNotCleared");
			HybridStrings.stringIDs.Add(1430057223U, "ErrorNoTenantAcceptedDomains");
			HybridStrings.stringIDs.Add(2996915408U, "MRSProxyTaskName");
			HybridStrings.stringIDs.Add(458024348U, "InvalidOrganizationRelationship");
			HybridStrings.stringIDs.Add(786567629U, "HybridInfoValidateUnusedConfigurationAttributesAreCleared");
			HybridStrings.stringIDs.Add(423230028U, "HybridErrorOnlyOneAutoDiscoverDomainMayBeSet");
			HybridStrings.stringIDs.Add(2908580272U, "GlobalPrereqTaskName");
			HybridStrings.stringIDs.Add(127541045U, "HybridInfoExecutionComplete");
			HybridStrings.stringIDs.Add(3035608407U, "WarningUnableToCommunicateWithAutoDiscoveryEP");
			HybridStrings.stringIDs.Add(2863677737U, "ConfirmationMessageUpdateHybridConfiguration");
			HybridStrings.stringIDs.Add(2717467465U, "ErrorOrgRelNotFoundOnPrem");
			HybridStrings.stringIDs.Add(838311222U, "HybridInfoBasePrereqsFailed");
			HybridStrings.stringIDs.Add(876046568U, "ErrorNoOrganizationalUnitsFound");
			HybridStrings.stringIDs.Add(512075U, "IOCConfigurationTaskName");
			HybridStrings.stringIDs.Add(2570457594U, "ConfirmationMessageSetHybridConfiguration");
			HybridStrings.stringIDs.Add(2441367287U, "ReturnResultForHybridDetectionWasFalse");
			HybridStrings.stringIDs.Add(3815718049U, "ErrorIncompatibleServersDetected");
			HybridStrings.stringIDs.Add(2541891350U, "HybridErrorNoTlsCertificateNameSet");
			HybridStrings.stringIDs.Add(3364263621U, "ErrorHybridUpgradeNotAllTransportServersProperVersion");
			HybridStrings.stringIDs.Add(3426408323U, "HybridInfoVerifyTenantHasBeenUpgraded");
			HybridStrings.stringIDs.Add(3661239469U, "RecipientTaskName");
			HybridStrings.stringIDs.Add(3953578261U, "OrganizationRelationshipTaskName");
			HybridStrings.stringIDs.Add(573859146U, "HybridInfoUpdatingHybridConfigurationVersion");
			HybridStrings.stringIDs.Add(3251785458U, "ErrorAccountNamespace");
			HybridStrings.stringIDs.Add(2320637488U, "ErrorNoOnPremAcceptedDomains");
			HybridStrings.stringIDs.Add(2413435486U, "ErrorNoHybridDomains");
			HybridStrings.stringIDs.Add(888551196U, "HybridInfoPurePSObjectsNotSupported");
			HybridStrings.stringIDs.Add(2147167682U, "HybridInfoClearingUnusedHybridConfigurationProperties");
			HybridStrings.stringIDs.Add(1453693463U, "HybridFedInfoFallbackInfo");
			HybridStrings.stringIDs.Add(46137829U, "HybridUpgradeFrom14TaskName");
			HybridStrings.stringIDs.Add(2918546762U, "HybridInfoConnectedToTenant");
			HybridStrings.stringIDs.Add(3422685683U, "ErrorOrgRelNotFoundOnTenant");
			HybridStrings.stringIDs.Add(345297155U, "HybridEngineCheckingForUpgradeTenant");
			HybridStrings.stringIDs.Add(2844731914U, "HybridInfoRemovingUnnecessaryRemoteDomains");
			HybridStrings.stringIDs.Add(1876236381U, "TenantDetectionTaskName");
			HybridStrings.stringIDs.Add(144627946U, "HybridInfoVerifyTransportServers");
			HybridStrings.stringIDs.Add(430320558U, "ErrorNoOutboundConnector");
			HybridStrings.stringIDs.Add(3561336298U, "HybridErrorNoTransportServersSet");
			HybridStrings.stringIDs.Add(4039070452U, "HybridActivityEstablish");
			HybridStrings.stringIDs.Add(2473550644U, "HybridErrorNoSmartHostSet");
			HybridStrings.stringIDs.Add(2276570094U, "HybridInfoValidatingUnnecessaryRemoteDomainsAreRemoved");
			HybridStrings.stringIDs.Add(3952178692U, "WarningRedirectCU10HybridStandaloneConfiguration");
			HybridStrings.stringIDs.Add(2384567192U, "ErrorHybridNoCASWithEWSURL");
			HybridStrings.stringIDs.Add(383363188U, "ConfirmationMessageNewHybridConfiguration");
			HybridStrings.stringIDs.Add(1366871238U, "HybridErrorMixedTransportServersSet");
			HybridStrings.stringIDs.Add(1823864370U, "HybridConnectingToOnPrem");
			HybridStrings.stringIDs.Add(3524037511U, "RemoveHybidConfigurationConfirmation");
			HybridStrings.stringIDs.Add(3455711695U, "ErrorTenantUsingConsumerLiveID");
			HybridStrings.stringIDs.Add(856118049U, "HybridActivityConfigure");
			HybridStrings.stringIDs.Add(3567836490U, "HybridInfoNoNeedToUpgrade");
			HybridStrings.stringIDs.Add(1109246636U, "ErrorNoFederationTrustFound");
			HybridStrings.stringIDs.Add(2371375820U, "ErrorHybridConfigurationVersionNotUpdated");
			HybridStrings.stringIDs.Add(3910362397U, "ErrorHybridExternalIPAddressesNotCleared");
			HybridStrings.stringIDs.Add(1896684180U, "ErrorCASExternalUrlNotSet");
			HybridStrings.stringIDs.Add(2525757763U, "HybridInfoCheckForPermissionToUpgrade");
			HybridStrings.stringIDs.Add(1105710278U, "ErrorHybridOnPremisesOrganizationWasNotCreatedWithUpgradedConnectors");
			HybridStrings.stringIDs.Add(4077665459U, "ErrorNoInboundConnector");
			HybridStrings.stringIDs.Add(4171349911U, "ErrorHybridConfigurationAlreadyDefined");
			HybridStrings.stringIDs.Add(2951975549U, "MailFlowTaskName");
			HybridStrings.stringIDs.Add(969801930U, "ErrorHybridExternalIPAddressesRangeAddressesNotSupported");
			HybridStrings.stringIDs.Add(1250087182U, "HybridActivityCompleted");
			HybridStrings.stringIDs.Add(951022927U, "ErrorFederatedIdentifierDisabled");
			HybridStrings.stringIDs.Add(1869337625U, "HybridInfoConnectedToOnPrem");
			HybridStrings.stringIDs.Add(4016244689U, "HybridConnectingToTenant");
			HybridStrings.stringIDs.Add(2902086538U, "HybridUpgradePrompt");
			HybridStrings.stringIDs.Add(2366359160U, "ErrorHybridTenenatUpgradeRequired");
			HybridStrings.stringIDs.Add(3418341869U, "ErrorHybridExternalIPAddressesExceeded40Entries");
			HybridStrings.stringIDs.Add(3323479649U, "OnOffSettingsTaskName");
		}

		public static LocalizedString ErrorHybridUpgradedTo2013
		{
			get
			{
				return new LocalizedString("ErrorHybridUpgradedTo2013", HybridStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorSecureMailCertificateNotFound(string subject, string issuer, string server)
		{
			return new LocalizedString("ErrorSecureMailCertificateNotFound", HybridStrings.ResourceManager, new object[]
			{
				subject,
				issuer,
				server
			});
		}

		public static LocalizedString ErrorMultipleFederationTrusts
		{
			get
			{
				return new LocalizedString("ErrorMultipleFederationTrusts", HybridStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorHybridMustBeUpgraded
		{
			get
			{
				return new LocalizedString("ErrorHybridMustBeUpgraded", HybridStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorNoFederatedDomainsOnTenant
		{
			get
			{
				return new LocalizedString("ErrorNoFederatedDomainsOnTenant", HybridStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString HybridErrorBothTransportServersNotSet
		{
			get
			{
				return new LocalizedString("HybridErrorBothTransportServersNotSet", HybridStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorNoHybridDomain
		{
			get
			{
				return new LocalizedString("ErrorNoHybridDomain", HybridStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString HybridInfoObjectNotFound
		{
			get
			{
				return new LocalizedString("HybridInfoObjectNotFound", HybridStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorCASExternalUrlMatchNotFound(string serverName, string subjectName)
		{
			return new LocalizedString("ErrorCASExternalUrlMatchNotFound", HybridStrings.ResourceManager, new object[]
			{
				serverName,
				subjectName
			});
		}

		public static LocalizedString ErrorMultipleMatchingOrgRelationships
		{
			get
			{
				return new LocalizedString("ErrorMultipleMatchingOrgRelationships", HybridStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorTargetAutodiscoverEprNotFound(string domain)
		{
			return new LocalizedString("ErrorTargetAutodiscoverEprNotFound", HybridStrings.ResourceManager, new object[]
			{
				domain
			});
		}

		public static LocalizedString ErrorOnPremUsingConsumerLiveID
		{
			get
			{
				return new LocalizedString("ErrorOnPremUsingConsumerLiveID", HybridStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorFederationIDNotProvisioned
		{
			get
			{
				return new LocalizedString("ErrorFederationIDNotProvisioned", HybridStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorHybridClientAccessServersNotCleared
		{
			get
			{
				return new LocalizedString("ErrorHybridClientAccessServersNotCleared", HybridStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorHybridConfigurationTooNew(string objectVersion, string currentVersion)
		{
			return new LocalizedString("ErrorHybridConfigurationTooNew", HybridStrings.ResourceManager, new object[]
			{
				objectVersion,
				currentVersion
			});
		}

		public static LocalizedString ErrorNoTenantAcceptedDomains
		{
			get
			{
				return new LocalizedString("ErrorNoTenantAcceptedDomains", HybridStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MRSProxyTaskName
		{
			get
			{
				return new LocalizedString("MRSProxyTaskName", HybridStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InvalidOrganizationRelationship
		{
			get
			{
				return new LocalizedString("InvalidOrganizationRelationship", HybridStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString HybridErrorServerNotE14Edge(string server)
		{
			return new LocalizedString("HybridErrorServerNotE14Edge", HybridStrings.ResourceManager, new object[]
			{
				server
			});
		}

		public static LocalizedString HybridErrorServerNotE14CAS(string server)
		{
			return new LocalizedString("HybridErrorServerNotE14CAS", HybridStrings.ResourceManager, new object[]
			{
				server
			});
		}

		public static LocalizedString HybridInfoValidateUnusedConfigurationAttributesAreCleared
		{
			get
			{
				return new LocalizedString("HybridInfoValidateUnusedConfigurationAttributesAreCleared", HybridStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString HybridCouldNotResolveServerException(string server, Exception e)
		{
			return new LocalizedString("HybridCouldNotResolveServerException", HybridStrings.ResourceManager, new object[]
			{
				server,
				e
			});
		}

		public static LocalizedString HybridErrorOnlyOneAutoDiscoverDomainMayBeSet
		{
			get
			{
				return new LocalizedString("HybridErrorOnlyOneAutoDiscoverDomainMayBeSet", HybridStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString HybridInfoOpeningRunspace(string uri)
		{
			return new LocalizedString("HybridInfoOpeningRunspace", HybridStrings.ResourceManager, new object[]
			{
				uri
			});
		}

		public static LocalizedString HybridErrorSendingTransportServerNotE15Hub(string server)
		{
			return new LocalizedString("HybridErrorSendingTransportServerNotE15Hub", HybridStrings.ResourceManager, new object[]
			{
				server
			});
		}

		public static LocalizedString ErrorHybridTenantRemoteDomainNotRemoved(string remoteDomainName)
		{
			return new LocalizedString("ErrorHybridTenantRemoteDomainNotRemoved", HybridStrings.ResourceManager, new object[]
			{
				remoteDomainName
			});
		}

		public static LocalizedString WarningEdgeReceiveConnector(string server, string identity, string fqdn)
		{
			return new LocalizedString("WarningEdgeReceiveConnector", HybridStrings.ResourceManager, new object[]
			{
				server,
				identity,
				fqdn
			});
		}

		public static LocalizedString GlobalPrereqTaskName
		{
			get
			{
				return new LocalizedString("GlobalPrereqTaskName", HybridStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString HybridInfoExecutionComplete
		{
			get
			{
				return new LocalizedString("HybridInfoExecutionComplete", HybridStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString HybridInfoTaskSubStepFinish(string taskName, string subStepName, bool result, double time)
		{
			return new LocalizedString("HybridInfoTaskSubStepFinish", HybridStrings.ResourceManager, new object[]
			{
				taskName,
				subStepName,
				result,
				time
			});
		}

		public static LocalizedString WarningUnableToCommunicateWithAutoDiscoveryEP
		{
			get
			{
				return new LocalizedString("WarningUnableToCommunicateWithAutoDiscoveryEP", HybridStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorHybridOnPremRemoteDomainNotRemoved(string remoteDomainName)
		{
			return new LocalizedString("ErrorHybridOnPremRemoteDomainNotRemoved", HybridStrings.ResourceManager, new object[]
			{
				remoteDomainName
			});
		}

		public static LocalizedString ConfirmationMessageUpdateHybridConfiguration
		{
			get
			{
				return new LocalizedString("ConfirmationMessageUpdateHybridConfiguration", HybridStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorOrgRelNotFoundOnPrem
		{
			get
			{
				return new LocalizedString("ErrorOrgRelNotFoundOnPrem", HybridStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorCASCertInvalidDate(string serverName, string thumbprint)
		{
			return new LocalizedString("ErrorCASCertInvalidDate", HybridStrings.ResourceManager, new object[]
			{
				serverName,
				thumbprint
			});
		}

		public static LocalizedString HybridCouldNotOpenRunspaceException(Exception e)
		{
			return new LocalizedString("HybridCouldNotOpenRunspaceException", HybridStrings.ResourceManager, new object[]
			{
				e
			});
		}

		public static LocalizedString HybridInfoBasePrereqsFailed
		{
			get
			{
				return new LocalizedString("HybridInfoBasePrereqsFailed", HybridStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString HybridFedInfoFallbackError(string domain)
		{
			return new LocalizedString("HybridFedInfoFallbackError", HybridStrings.ResourceManager, new object[]
			{
				domain
			});
		}

		public static LocalizedString ErrorCASCertSelfSigned(string serverName, string thumbprint)
		{
			return new LocalizedString("ErrorCASCertSelfSigned", HybridStrings.ResourceManager, new object[]
			{
				serverName,
				thumbprint
			});
		}

		public static LocalizedString ErrorTargetApplicationUriNotFound(string domain)
		{
			return new LocalizedString("ErrorTargetApplicationUriNotFound", HybridStrings.ResourceManager, new object[]
			{
				domain
			});
		}

		public static LocalizedString ErrorNoOrganizationalUnitsFound
		{
			get
			{
				return new LocalizedString("ErrorNoOrganizationalUnitsFound", HybridStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorInvalidSignupDomain(string domain, string tenantDomain)
		{
			return new LocalizedString("ErrorInvalidSignupDomain", HybridStrings.ResourceManager, new object[]
			{
				domain,
				tenantDomain
			});
		}

		public static LocalizedString ErrorHybridDomainNotAcceptedOnPrem(string domain)
		{
			return new LocalizedString("ErrorHybridDomainNotAcceptedOnPrem", HybridStrings.ResourceManager, new object[]
			{
				domain
			});
		}

		public static LocalizedString IOCConfigurationTaskName
		{
			get
			{
				return new LocalizedString("IOCConfigurationTaskName", HybridStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ConfirmationMessageSetHybridConfiguration
		{
			get
			{
				return new LocalizedString("ConfirmationMessageSetHybridConfiguration", HybridStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorCASRoleInvalid(string serverName)
		{
			return new LocalizedString("ErrorCASRoleInvalid", HybridStrings.ResourceManager, new object[]
			{
				serverName
			});
		}

		public static LocalizedString ReturnResultForHybridDetectionWasFalse
		{
			get
			{
				return new LocalizedString("ReturnResultForHybridDetectionWasFalse", HybridStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorIncompatibleServersDetected
		{
			get
			{
				return new LocalizedString("ErrorIncompatibleServersDetected", HybridStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString HybridErrorNoTlsCertificateNameSet
		{
			get
			{
				return new LocalizedString("HybridErrorNoTlsCertificateNameSet", HybridStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorHybridUpgradeNotAllTransportServersProperVersion
		{
			get
			{
				return new LocalizedString("ErrorHybridUpgradeNotAllTransportServersProperVersion", HybridStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString HybridInfoVerifyTenantHasBeenUpgraded
		{
			get
			{
				return new LocalizedString("HybridInfoVerifyTenantHasBeenUpgraded", HybridStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RecipientTaskName
		{
			get
			{
				return new LocalizedString("RecipientTaskName", HybridStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorCmdletException(string cmdletName)
		{
			return new LocalizedString("ErrorCmdletException", HybridStrings.ResourceManager, new object[]
			{
				cmdletName
			});
		}

		public static LocalizedString HybridCouldNotCreateTenantSessionException(Exception e)
		{
			return new LocalizedString("HybridCouldNotCreateTenantSessionException", HybridStrings.ResourceManager, new object[]
			{
				e
			});
		}

		public static LocalizedString OrganizationRelationshipTaskName
		{
			get
			{
				return new LocalizedString("OrganizationRelationshipTaskName", HybridStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString HybridInfoUpdatingHybridConfigurationVersion
		{
			get
			{
				return new LocalizedString("HybridInfoUpdatingHybridConfigurationVersion", HybridStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorAccountNamespace
		{
			get
			{
				return new LocalizedString("ErrorAccountNamespace", HybridStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorOrgRelProvisionFailed(string domain)
		{
			return new LocalizedString("ErrorOrgRelProvisionFailed", HybridStrings.ResourceManager, new object[]
			{
				domain
			});
		}

		public static LocalizedString ErrorSignupDomain(string domain)
		{
			return new LocalizedString("ErrorSignupDomain", HybridStrings.ResourceManager, new object[]
			{
				domain
			});
		}

		public static LocalizedString ErrorNoOnPremAcceptedDomains
		{
			get
			{
				return new LocalizedString("ErrorNoOnPremAcceptedDomains", HybridStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString HybridCouldNotCreateOnPremisesSessionException(Exception e)
		{
			return new LocalizedString("HybridCouldNotCreateOnPremisesSessionException", HybridStrings.ResourceManager, new object[]
			{
				e
			});
		}

		public static LocalizedString HybridErrorReceivingTransportServerNotE15FrontEnd(string server)
		{
			return new LocalizedString("HybridErrorReceivingTransportServerNotE15FrontEnd", HybridStrings.ResourceManager, new object[]
			{
				server
			});
		}

		public static LocalizedString ErrorNoHybridDomains
		{
			get
			{
				return new LocalizedString("ErrorNoHybridDomains", HybridStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString HybridInfoPurePSObjectsNotSupported
		{
			get
			{
				return new LocalizedString("HybridInfoPurePSObjectsNotSupported", HybridStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString HybridInfoClearingUnusedHybridConfigurationProperties
		{
			get
			{
				return new LocalizedString("HybridInfoClearingUnusedHybridConfigurationProperties", HybridStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorSecureMailCertificateNoSmtp(string server)
		{
			return new LocalizedString("ErrorSecureMailCertificateNoSmtp", HybridStrings.ResourceManager, new object[]
			{
				server
			});
		}

		public static LocalizedString HybridFedInfoFallbackInfo
		{
			get
			{
				return new LocalizedString("HybridFedInfoFallbackInfo", HybridStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString HybridUpgradeFrom14TaskName
		{
			get
			{
				return new LocalizedString("HybridUpgradeFrom14TaskName", HybridStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString HybridInfoTaskLogTemplate(string taskName, string message)
		{
			return new LocalizedString("HybridInfoTaskLogTemplate", HybridStrings.ResourceManager, new object[]
			{
				taskName,
				message
			});
		}

		public static LocalizedString HybridInfoConnectedToTenant
		{
			get
			{
				return new LocalizedString("HybridInfoConnectedToTenant", HybridStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorOrgRelNotFoundOnTenant
		{
			get
			{
				return new LocalizedString("ErrorOrgRelNotFoundOnTenant", HybridStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString WarningTenantGetFedInfoFailed(string targetAutodiscoverEpr)
		{
			return new LocalizedString("WarningTenantGetFedInfoFailed", HybridStrings.ResourceManager, new object[]
			{
				targetAutodiscoverEpr
			});
		}

		public static LocalizedString HybridEngineCheckingForUpgradeTenant
		{
			get
			{
				return new LocalizedString("HybridEngineCheckingForUpgradeTenant", HybridStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString HybridInfoRemovingUnnecessaryRemoteDomains
		{
			get
			{
				return new LocalizedString("HybridInfoRemovingUnnecessaryRemoteDomains", HybridStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString TenantDetectionTaskName
		{
			get
			{
				return new LocalizedString("TenantDetectionTaskName", HybridStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorDefaultReceieveConnectorNotFound(string server)
		{
			return new LocalizedString("ErrorDefaultReceieveConnectorNotFound", HybridStrings.ResourceManager, new object[]
			{
				server
			});
		}

		public static LocalizedString HybridInfoVerifyTransportServers
		{
			get
			{
				return new LocalizedString("HybridInfoVerifyTransportServers", HybridStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorNoOutboundConnector
		{
			get
			{
				return new LocalizedString("ErrorNoOutboundConnector", HybridStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorTaskExceptionTemplate(string subtask, string message)
		{
			return new LocalizedString("ErrorTaskExceptionTemplate", HybridStrings.ResourceManager, new object[]
			{
				subtask,
				message
			});
		}

		public static LocalizedString HybridErrorNoTransportServersSet
		{
			get
			{
				return new LocalizedString("HybridErrorNoTransportServersSet", HybridStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString HybridActivityEstablish
		{
			get
			{
				return new LocalizedString("HybridActivityEstablish", HybridStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString HybridErrorNoSmartHostSet
		{
			get
			{
				return new LocalizedString("HybridErrorNoSmartHostSet", HybridStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorSecureMailCertificateSelfSigned(string server)
		{
			return new LocalizedString("ErrorSecureMailCertificateSelfSigned", HybridStrings.ResourceManager, new object[]
			{
				server
			});
		}

		public static LocalizedString ErrorCoexistenceDomainNotAcceptedOnTenant(string domain)
		{
			return new LocalizedString("ErrorCoexistenceDomainNotAcceptedOnTenant", HybridStrings.ResourceManager, new object[]
			{
				domain
			});
		}

		public static LocalizedString ErrorFederationInfoNotFound(string hybridDomain)
		{
			return new LocalizedString("ErrorFederationInfoNotFound", HybridStrings.ResourceManager, new object[]
			{
				hybridDomain
			});
		}

		public static LocalizedString HybridInfoValidatingUnnecessaryRemoteDomainsAreRemoved
		{
			get
			{
				return new LocalizedString("HybridInfoValidatingUnnecessaryRemoteDomainsAreRemoved", HybridStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString WarningRedirectCU10HybridStandaloneConfiguration
		{
			get
			{
				return new LocalizedString("WarningRedirectCU10HybridStandaloneConfiguration", HybridStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorHybridNoCASWithEWSURL
		{
			get
			{
				return new LocalizedString("ErrorHybridNoCASWithEWSURL", HybridStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ConfirmationMessageNewHybridConfiguration
		{
			get
			{
				return new LocalizedString("ConfirmationMessageNewHybridConfiguration", HybridStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString HybridErrorMixedTransportServersSet
		{
			get
			{
				return new LocalizedString("HybridErrorMixedTransportServersSet", HybridStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString HybridConnectingToOnPrem
		{
			get
			{
				return new LocalizedString("HybridConnectingToOnPrem", HybridStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString HybridInfoGetObjectValue(string value, string identity, string command)
		{
			return new LocalizedString("HybridInfoGetObjectValue", HybridStrings.ResourceManager, new object[]
			{
				value,
				identity,
				command
			});
		}

		public static LocalizedString ErrorCASCertNotTrusted(string serverName, string thumbprint)
		{
			return new LocalizedString("ErrorCASCertNotTrusted", HybridStrings.ResourceManager, new object[]
			{
				serverName,
				thumbprint
			});
		}

		public static LocalizedString ExceptionUpdateHybridConfigurationFailedWithLog(string errMsg, string machineLogFileLocation, string localPathLogFileLocation)
		{
			return new LocalizedString("ExceptionUpdateHybridConfigurationFailedWithLog", HybridStrings.ResourceManager, new object[]
			{
				errMsg,
				machineLogFileLocation,
				localPathLogFileLocation
			});
		}

		public static LocalizedString RemoveHybidConfigurationConfirmation
		{
			get
			{
				return new LocalizedString("RemoveHybidConfigurationConfirmation", HybridStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString HybridInfoTotalCmdletTime(string session, double timeSeconds)
		{
			return new LocalizedString("HybridInfoTotalCmdletTime", HybridStrings.ResourceManager, new object[]
			{
				session,
				timeSeconds
			});
		}

		public static LocalizedString ErrorTenantUsingConsumerLiveID
		{
			get
			{
				return new LocalizedString("ErrorTenantUsingConsumerLiveID", HybridStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString HybridActivityConfigure
		{
			get
			{
				return new LocalizedString("HybridActivityConfigure", HybridStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorTooManyMatchingResults(string identity)
		{
			return new LocalizedString("ErrorTooManyMatchingResults", HybridStrings.ResourceManager, new object[]
			{
				identity
			});
		}

		public static LocalizedString HybridInfoNoNeedToUpgrade
		{
			get
			{
				return new LocalizedString("HybridInfoNoNeedToUpgrade", HybridStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString HybridErrorSendingTransportServerNotHub(string server)
		{
			return new LocalizedString("HybridErrorSendingTransportServerNotHub", HybridStrings.ResourceManager, new object[]
			{
				server
			});
		}

		public static LocalizedString ErrorNoFederationTrustFound
		{
			get
			{
				return new LocalizedString("ErrorNoFederationTrustFound", HybridStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorHybridConfigurationVersionNotUpdated
		{
			get
			{
				return new LocalizedString("ErrorHybridConfigurationVersionNotUpdated", HybridStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString HybridInfoCmdletStart(string session, string cmdlet, string parameters)
		{
			return new LocalizedString("HybridInfoCmdletStart", HybridStrings.ResourceManager, new object[]
			{
				session,
				cmdlet,
				parameters
			});
		}

		public static LocalizedString ErrorHybridRegistryInvalidUri(string name)
		{
			return new LocalizedString("ErrorHybridRegistryInvalidUri", HybridStrings.ResourceManager, new object[]
			{
				name
			});
		}

		public static LocalizedString ErrorHybridExternalIPAddressesNotCleared
		{
			get
			{
				return new LocalizedString("ErrorHybridExternalIPAddressesNotCleared", HybridStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorDuplicateSendConnectorAddressSpace(string addressSpace)
		{
			return new LocalizedString("ErrorDuplicateSendConnectorAddressSpace", HybridStrings.ResourceManager, new object[]
			{
				addressSpace
			});
		}

		public static LocalizedString ErrorCASExternalUrlNotSet
		{
			get
			{
				return new LocalizedString("ErrorCASExternalUrlNotSet", HybridStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString HybridInfoCheckForPermissionToUpgrade
		{
			get
			{
				return new LocalizedString("HybridInfoCheckForPermissionToUpgrade", HybridStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorHybridServerAlreadyAssigned(string server)
		{
			return new LocalizedString("ErrorHybridServerAlreadyAssigned", HybridStrings.ResourceManager, new object[]
			{
				server
			});
		}

		public static LocalizedString ErrorHybridOnPremisesOrganizationWasNotCreatedWithUpgradedConnectors
		{
			get
			{
				return new LocalizedString("ErrorHybridOnPremisesOrganizationWasNotCreatedWithUpgradedConnectors", HybridStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorNoInboundConnector
		{
			get
			{
				return new LocalizedString("ErrorNoInboundConnector", HybridStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString HybridErrorReceivingTransportServerNotFrontEnd(string server)
		{
			return new LocalizedString("HybridErrorReceivingTransportServerNotFrontEnd", HybridStrings.ResourceManager, new object[]
			{
				server
			});
		}

		public static LocalizedString ErrorHybridDomainNotAcceptedOnTenant(string domain)
		{
			return new LocalizedString("ErrorHybridDomainNotAcceptedOnTenant", HybridStrings.ResourceManager, new object[]
			{
				domain
			});
		}

		public static LocalizedString HybridFedInfoFallbackWarning(string domain)
		{
			return new LocalizedString("HybridFedInfoFallbackWarning", HybridStrings.ResourceManager, new object[]
			{
				domain
			});
		}

		public static LocalizedString WarningHybridLegacyEmailAddressPolicyNotUpgraded(string policyName)
		{
			return new LocalizedString("WarningHybridLegacyEmailAddressPolicyNotUpgraded", HybridStrings.ResourceManager, new object[]
			{
				policyName
			});
		}

		public static LocalizedString ErrorHybridConfigurationAlreadyDefined
		{
			get
			{
				return new LocalizedString("ErrorHybridConfigurationAlreadyDefined", HybridStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString HybridErrorServerNotEdge(string server)
		{
			return new LocalizedString("HybridErrorServerNotEdge", HybridStrings.ResourceManager, new object[]
			{
				server
			});
		}

		public static LocalizedString MailFlowTaskName
		{
			get
			{
				return new LocalizedString("MailFlowTaskName", HybridStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString HybridInfoHybridConfigurationObjectVersion(ExchangeObjectVersion version)
		{
			return new LocalizedString("HybridInfoHybridConfigurationObjectVersion", HybridStrings.ResourceManager, new object[]
			{
				version
			});
		}

		public static LocalizedString ErrorHybridExternalIPAddressesRangeAddressesNotSupported
		{
			get
			{
				return new LocalizedString("ErrorHybridExternalIPAddressesRangeAddressesNotSupported", HybridStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString HybridInfoTaskSubStepStart(string taskName, string subStepName)
		{
			return new LocalizedString("HybridInfoTaskSubStepStart", HybridStrings.ResourceManager, new object[]
			{
				taskName,
				subStepName
			});
		}

		public static LocalizedString HybridActivityCompleted
		{
			get
			{
				return new LocalizedString("HybridActivityCompleted", HybridStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorSendConnectorAddressSpaceNotExclusive(string addressSpace)
		{
			return new LocalizedString("ErrorSendConnectorAddressSpaceNotExclusive", HybridStrings.ResourceManager, new object[]
			{
				addressSpace
			});
		}

		public static LocalizedString ErrorFederatedIdentifierDisabled
		{
			get
			{
				return new LocalizedString("ErrorFederatedIdentifierDisabled", HybridStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString HybridInfoConnectedToOnPrem
		{
			get
			{
				return new LocalizedString("HybridInfoConnectedToOnPrem", HybridStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString WarningOAuthNeedsConfiguration(string url)
		{
			return new LocalizedString("WarningOAuthNeedsConfiguration", HybridStrings.ResourceManager, new object[]
			{
				url
			});
		}

		public static LocalizedString HybridConnectingToTenant
		{
			get
			{
				return new LocalizedString("HybridConnectingToTenant", HybridStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorSecureMailCertificateInvalidDate(string server)
		{
			return new LocalizedString("ErrorSecureMailCertificateInvalidDate", HybridStrings.ResourceManager, new object[]
			{
				server
			});
		}

		public static LocalizedString HybridErrorServerNotCAS(string server)
		{
			return new LocalizedString("HybridErrorServerNotCAS", HybridStrings.ResourceManager, new object[]
			{
				server
			});
		}

		public static LocalizedString HybridUpgradePrompt
		{
			get
			{
				return new LocalizedString("HybridUpgradePrompt", HybridStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorHybridTenenatUpgradeRequired
		{
			get
			{
				return new LocalizedString("ErrorHybridTenenatUpgradeRequired", HybridStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString HybridInfoCmdletEnd(string session, string cmdlet, string elapsed)
		{
			return new LocalizedString("HybridInfoCmdletEnd", HybridStrings.ResourceManager, new object[]
			{
				session,
				cmdlet,
				elapsed
			});
		}

		public static LocalizedString ErrorHybridExternalIPAddressesExceeded40Entries
		{
			get
			{
				return new LocalizedString("ErrorHybridExternalIPAddressesExceeded40Entries", HybridStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString HybridInfoHybridConfigurationEngineVersion(ExchangeObjectVersion version)
		{
			return new LocalizedString("HybridInfoHybridConfigurationEngineVersion", HybridStrings.ResourceManager, new object[]
			{
				version
			});
		}

		public static LocalizedString OnOffSettingsTaskName
		{
			get
			{
				return new LocalizedString("OnOffSettingsTaskName", HybridStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString GetLocalizedString(HybridStrings.IDs key)
		{
			return new LocalizedString(HybridStrings.stringIDs[(uint)key], HybridStrings.ResourceManager, new object[0]);
		}

		private static Dictionary<uint, string> stringIDs = new Dictionary<uint, string>(78);

		private static ExchangeResourceManager ResourceManager = ExchangeResourceManager.GetResourceManager("Microsoft.Exchange.Management.Hybrid.Strings", typeof(HybridStrings).GetTypeInfo().Assembly);

		public enum IDs : uint
		{
			ErrorHybridUpgradedTo2013 = 2425718147U,
			ErrorMultipleFederationTrusts = 270912202U,
			ErrorHybridMustBeUpgraded = 820521406U,
			ErrorNoFederatedDomainsOnTenant = 4267730599U,
			HybridErrorBothTransportServersNotSet = 1424719599U,
			ErrorNoHybridDomain = 2925976073U,
			HybridInfoObjectNotFound = 3187185748U,
			ErrorMultipleMatchingOrgRelationships = 2511569494U,
			ErrorOnPremUsingConsumerLiveID = 3431283964U,
			ErrorFederationIDNotProvisioned = 4267028265U,
			ErrorHybridClientAccessServersNotCleared = 4281839122U,
			ErrorNoTenantAcceptedDomains = 1430057223U,
			MRSProxyTaskName = 2996915408U,
			InvalidOrganizationRelationship = 458024348U,
			HybridInfoValidateUnusedConfigurationAttributesAreCleared = 786567629U,
			HybridErrorOnlyOneAutoDiscoverDomainMayBeSet = 423230028U,
			GlobalPrereqTaskName = 2908580272U,
			HybridInfoExecutionComplete = 127541045U,
			WarningUnableToCommunicateWithAutoDiscoveryEP = 3035608407U,
			ConfirmationMessageUpdateHybridConfiguration = 2863677737U,
			ErrorOrgRelNotFoundOnPrem = 2717467465U,
			HybridInfoBasePrereqsFailed = 838311222U,
			ErrorNoOrganizationalUnitsFound = 876046568U,
			IOCConfigurationTaskName = 512075U,
			ConfirmationMessageSetHybridConfiguration = 2570457594U,
			ReturnResultForHybridDetectionWasFalse = 2441367287U,
			ErrorIncompatibleServersDetected = 3815718049U,
			HybridErrorNoTlsCertificateNameSet = 2541891350U,
			ErrorHybridUpgradeNotAllTransportServersProperVersion = 3364263621U,
			HybridInfoVerifyTenantHasBeenUpgraded = 3426408323U,
			RecipientTaskName = 3661239469U,
			OrganizationRelationshipTaskName = 3953578261U,
			HybridInfoUpdatingHybridConfigurationVersion = 573859146U,
			ErrorAccountNamespace = 3251785458U,
			ErrorNoOnPremAcceptedDomains = 2320637488U,
			ErrorNoHybridDomains = 2413435486U,
			HybridInfoPurePSObjectsNotSupported = 888551196U,
			HybridInfoClearingUnusedHybridConfigurationProperties = 2147167682U,
			HybridFedInfoFallbackInfo = 1453693463U,
			HybridUpgradeFrom14TaskName = 46137829U,
			HybridInfoConnectedToTenant = 2918546762U,
			ErrorOrgRelNotFoundOnTenant = 3422685683U,
			HybridEngineCheckingForUpgradeTenant = 345297155U,
			HybridInfoRemovingUnnecessaryRemoteDomains = 2844731914U,
			TenantDetectionTaskName = 1876236381U,
			HybridInfoVerifyTransportServers = 144627946U,
			ErrorNoOutboundConnector = 430320558U,
			HybridErrorNoTransportServersSet = 3561336298U,
			HybridActivityEstablish = 4039070452U,
			HybridErrorNoSmartHostSet = 2473550644U,
			HybridInfoValidatingUnnecessaryRemoteDomainsAreRemoved = 2276570094U,
			WarningRedirectCU10HybridStandaloneConfiguration = 3952178692U,
			ErrorHybridNoCASWithEWSURL = 2384567192U,
			ConfirmationMessageNewHybridConfiguration = 383363188U,
			HybridErrorMixedTransportServersSet = 1366871238U,
			HybridConnectingToOnPrem = 1823864370U,
			RemoveHybidConfigurationConfirmation = 3524037511U,
			ErrorTenantUsingConsumerLiveID = 3455711695U,
			HybridActivityConfigure = 856118049U,
			HybridInfoNoNeedToUpgrade = 3567836490U,
			ErrorNoFederationTrustFound = 1109246636U,
			ErrorHybridConfigurationVersionNotUpdated = 2371375820U,
			ErrorHybridExternalIPAddressesNotCleared = 3910362397U,
			ErrorCASExternalUrlNotSet = 1896684180U,
			HybridInfoCheckForPermissionToUpgrade = 2525757763U,
			ErrorHybridOnPremisesOrganizationWasNotCreatedWithUpgradedConnectors = 1105710278U,
			ErrorNoInboundConnector = 4077665459U,
			ErrorHybridConfigurationAlreadyDefined = 4171349911U,
			MailFlowTaskName = 2951975549U,
			ErrorHybridExternalIPAddressesRangeAddressesNotSupported = 969801930U,
			HybridActivityCompleted = 1250087182U,
			ErrorFederatedIdentifierDisabled = 951022927U,
			HybridInfoConnectedToOnPrem = 1869337625U,
			HybridConnectingToTenant = 4016244689U,
			HybridUpgradePrompt = 2902086538U,
			ErrorHybridTenenatUpgradeRequired = 2366359160U,
			ErrorHybridExternalIPAddressesExceeded40Entries = 3418341869U,
			OnOffSettingsTaskName = 3323479649U
		}

		private enum ParamIDs
		{
			ErrorSecureMailCertificateNotFound,
			ErrorCASExternalUrlMatchNotFound,
			ErrorTargetAutodiscoverEprNotFound,
			ErrorHybridConfigurationTooNew,
			HybridErrorServerNotE14Edge,
			HybridErrorServerNotE14CAS,
			HybridCouldNotResolveServerException,
			HybridInfoOpeningRunspace,
			HybridErrorSendingTransportServerNotE15Hub,
			ErrorHybridTenantRemoteDomainNotRemoved,
			WarningEdgeReceiveConnector,
			HybridInfoTaskSubStepFinish,
			ErrorHybridOnPremRemoteDomainNotRemoved,
			ErrorCASCertInvalidDate,
			HybridCouldNotOpenRunspaceException,
			HybridFedInfoFallbackError,
			ErrorCASCertSelfSigned,
			ErrorTargetApplicationUriNotFound,
			ErrorInvalidSignupDomain,
			ErrorHybridDomainNotAcceptedOnPrem,
			ErrorCASRoleInvalid,
			ErrorCmdletException,
			HybridCouldNotCreateTenantSessionException,
			ErrorOrgRelProvisionFailed,
			ErrorSignupDomain,
			HybridCouldNotCreateOnPremisesSessionException,
			HybridErrorReceivingTransportServerNotE15FrontEnd,
			ErrorSecureMailCertificateNoSmtp,
			HybridInfoTaskLogTemplate,
			WarningTenantGetFedInfoFailed,
			ErrorDefaultReceieveConnectorNotFound,
			ErrorTaskExceptionTemplate,
			ErrorSecureMailCertificateSelfSigned,
			ErrorCoexistenceDomainNotAcceptedOnTenant,
			ErrorFederationInfoNotFound,
			HybridInfoGetObjectValue,
			ErrorCASCertNotTrusted,
			ExceptionUpdateHybridConfigurationFailedWithLog,
			HybridInfoTotalCmdletTime,
			ErrorTooManyMatchingResults,
			HybridErrorSendingTransportServerNotHub,
			HybridInfoCmdletStart,
			ErrorHybridRegistryInvalidUri,
			ErrorDuplicateSendConnectorAddressSpace,
			ErrorHybridServerAlreadyAssigned,
			HybridErrorReceivingTransportServerNotFrontEnd,
			ErrorHybridDomainNotAcceptedOnTenant,
			HybridFedInfoFallbackWarning,
			WarningHybridLegacyEmailAddressPolicyNotUpgraded,
			HybridErrorServerNotEdge,
			HybridInfoHybridConfigurationObjectVersion,
			HybridInfoTaskSubStepStart,
			ErrorSendConnectorAddressSpaceNotExclusive,
			WarningOAuthNeedsConfiguration,
			ErrorSecureMailCertificateInvalidDate,
			HybridErrorServerNotCAS,
			HybridInfoCmdletEnd,
			HybridInfoHybridConfigurationEngineVersion
		}
	}
}
