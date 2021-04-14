using System;
using System.Collections.Generic;
using System.Reflection;
using System.ServiceProcess;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Configuration.Common.LocStrings
{
	internal static class Strings
	{
		static Strings()
		{
			Strings.stringIDs.Add(3480090246U, "MissingImpersonatedUserSid");
			Strings.stringIDs.Add(3445110312U, "FalseString");
			Strings.stringIDs.Add(2329139938U, "VerboseFailedToGetServiceTopology");
			Strings.stringIDs.Add(831535795U, "NotGrantCustomScriptRole");
			Strings.stringIDs.Add(3609756192U, "ErrorChangeImmutableType");
			Strings.stringIDs.Add(1212809041U, "NotGrantAnyAdminRoles");
			Strings.stringIDs.Add(2716022819U, "ErrorOnlyDNSupportedWithIgnoreDefaultScope");
			Strings.stringIDs.Add(2866126871U, "ExceptionTaskNotInitialized");
			Strings.stringIDs.Add(311479382U, "VerboseLbNoDatabaseFoundInAD");
			Strings.stringIDs.Add(1464829243U, "WorkUnitStatusNotStarted");
			Strings.stringIDs.Add(444666707U, "TrueString");
			Strings.stringIDs.Add(1953440811U, "InvalidPswsDirectInvocationBlocked");
			Strings.stringIDs.Add(2103372448U, "InvalidProperties");
			Strings.stringIDs.Add(3344562832U, "ErrorInstanceObjectConatinsNullIdentity");
			Strings.stringIDs.Add(2895423986U, "ErrorAdminLoginUsingAppPassword");
			Strings.stringIDs.Add(2403641406U, "ErrorCannotFindMailboxToLogonPublicStore");
			Strings.stringIDs.Add(3498243205U, "WorkUnitStatusInProgress");
			Strings.stringIDs.Add(19923723U, "ErrorObjectHasValidationErrors");
			Strings.stringIDs.Add(452500353U, "ErrorNoProvisioningHandlerAvailable");
			Strings.stringIDs.Add(1811096880U, "WarningUnlicensedMailbox");
			Strings.stringIDs.Add(767622998U, "WorkUnitStatusFailed");
			Strings.stringIDs.Add(1306006239U, "VerboseInitializeRunspaceServerSettingsRemote");
			Strings.stringIDs.Add(585695277U, "ExceptionTaskInconsistent");
			Strings.stringIDs.Add(1520502444U, "WarningForceMessage");
			Strings.stringIDs.Add(4140272390U, "WarningCannotSetPrimarySmtpAddressWhenEapEnabled");
			Strings.stringIDs.Add(3204373538U, "ExecutingUserNameIsMissing");
			Strings.stringIDs.Add(2188568225U, "ExceptionNoChangesSpecified");
			Strings.stringIDs.Add(3805201920U, "ExceptionTaskAlreadyInitialized");
			Strings.stringIDs.Add(2951618283U, "ParameterValueTooLarge");
			Strings.stringIDs.Add(3718372071U, "ErrorNotSupportSingletonWildcard");
			Strings.stringIDs.Add(2784860353U, "WorkUnitCollectionConfigurationSummary");
			Strings.stringIDs.Add(143513383U, "ExceptionMDACommandNotExecuting");
			Strings.stringIDs.Add(1958023215U, "ErrorRemotePowershellConnectionBlocked");
			Strings.stringIDs.Add(2725322457U, "LogExecutionFailed");
			Strings.stringIDs.Add(3624978883U, "VerboseLbNoOabVDirReturned");
			Strings.stringIDs.Add(2158269158U, "VerboseLbEnterSiteMailboxEnterprise");
			Strings.stringIDs.Add(2890992798U, "HierarchicalIdentityNullOrEmpty");
			Strings.stringIDs.Add(1268762784U, "ExceptionObjectAlreadyExists");
			Strings.stringIDs.Add(2576106929U, "ErrorRemotePowerShellNotEnabled");
			Strings.stringIDs.Add(298822364U, "ErrorRbacConfigurationNotSupportedSharedConfiguration");
			Strings.stringIDs.Add(1356455742U, "UnknownEnumValue");
			Strings.stringIDs.Add(439815616U, "ErrorOperationRequiresManager");
			Strings.stringIDs.Add(2916171677U, "ErrorOrganizationWildcard");
			Strings.stringIDs.Add(687978330U, "ErrorDelegatedUserNotInOrg");
			Strings.stringIDs.Add(1837325848U, "VerboseSerializationDataNotExist");
			Strings.stringIDs.Add(1983570122U, "ErrorNoAvailablePublicFolderDatabase");
			Strings.stringIDs.Add(1655423524U, "SessionExpiredException");
			Strings.stringIDs.Add(2661346553U, "HierarchicalIdentityStartsOrEndsWithBackslash");
			Strings.stringIDs.Add(2859768776U, "VerboseInitializeRunspaceServerSettingsLocal");
			Strings.stringIDs.Add(1551194332U, "ExceptionMDACommandStillExecuting");
			Strings.stringIDs.Add(1960180119U, "WorkUnitError");
			Strings.stringIDs.Add(4205209694U, "VerboseNoSource");
			Strings.stringIDs.Add(797535012U, "ErrorFilteringOnlyUserLogin");
			Strings.stringIDs.Add(1654901580U, "ExceptionNullInstanceParameter");
			Strings.stringIDs.Add(498095919U, "VerboseLbCreateNewExRpcAdmin");
			Strings.stringIDs.Add(2137526570U, "ErrorCannotDiscoverDefaultOrganizationUnitForRecipient");
			Strings.stringIDs.Add(3713958116U, "CommandSucceeded");
			Strings.stringIDs.Add(84680862U, "ErrorUninitializedParameter");
			Strings.stringIDs.Add(2037166858U, "ErrorNotAllowedForPartnerAccess");
			Strings.stringIDs.Add(1859189834U, "ExceptionTaskNotExecuted");
			Strings.stringIDs.Add(3245318277U, "ErrorInvalidResultSize");
			Strings.stringIDs.Add(201309884U, "VerboseLbDeletedServer");
			Strings.stringIDs.Add(4282198592U, "TaskCompleted");
			Strings.stringIDs.Add(1234513041U, "ErrorMaxTenantPSConnectionLimitNotResolved");
			Strings.stringIDs.Add(1421372901U, "InvalidCharacterInComponentPartOfHierarchicalIdentity");
			Strings.stringIDs.Add(838517570U, "ExceptionReadOnlyPropertyBag");
			Strings.stringIDs.Add(3781767156U, "VerboseLbTryRetrieveDatabaseStatus");
			Strings.stringIDs.Add(3216817101U, "ErrorIgnoreDefaultScopeAndDCSetTogether");
			Strings.stringIDs.Add(3519173187U, "ExceptionGettingConditionObject");
			Strings.stringIDs.Add(4160063394U, "EnabledString");
			Strings.stringIDs.Add(3507110937U, "WorkUnitWarning");
			Strings.stringIDs.Add(1170623981U, "VerboseLbServerDownSoMarkDatabaseDown");
			Strings.stringIDs.Add(3828427927U, "BinaryDataStakeHodler");
			Strings.stringIDs.Add(1221524445U, "ErrorWriteOpOnDehydratedTenant");
			Strings.stringIDs.Add(3394388186U, "ExceptionMDACommandAlreadyExecuting");
			Strings.stringIDs.Add(2174831997U, "SipCultureInfoArgumentCheckFailure");
			Strings.stringIDs.Add(577240232U, "ConsecutiveWholeWildcardNamePartsInHierarchicalIdentity");
			Strings.stringIDs.Add(324189978U, "ErrorMapiPublicFolderTreeNotUnique");
			Strings.stringIDs.Add(3002944702U, "WarningMoreResultsAvailable");
			Strings.stringIDs.Add(3806422804U, "ErrorOperationOnInvalidObject");
			Strings.stringIDs.Add(4253079958U, "VerboseInitializeRunspaceServerSettingsAdam");
			Strings.stringIDs.Add(3746749985U, "VerboseLbNoAvailableDatabase");
			Strings.stringIDs.Add(3026495307U, "PswsManagementAutomationAssemblyLoadError");
			Strings.stringIDs.Add(2439453065U, "LogRollbackFailed");
			Strings.stringIDs.Add(2502725106U, "NullOrEmptyNamePartsInHierarchicalIdentity");
			Strings.stringIDs.Add(328156873U, "ErrorCloseServiceHandle");
			Strings.stringIDs.Add(3026665436U, "WorkUnitStatusCompleted");
			Strings.stringIDs.Add(2267892936U, "ErrorUrlInValid");
			Strings.stringIDs.Add(2717050851U, "ErrorNoMailboxUserInTheForest");
			Strings.stringIDs.Add(4074275529U, "ServerNotAvailable");
			Strings.stringIDs.Add(292337732U, "HelpUrlHeaderText");
			Strings.stringIDs.Add(2496835620U, "VersionMismatchDuringCreateRemoteRunspace");
			Strings.stringIDs.Add(2667845807U, "ErrorCannotOpenServiceControllerManager");
			Strings.stringIDs.Add(2472100876U, "VerboseLbNoEligibleServers");
			Strings.stringIDs.Add(3699804131U, "VerboseLbDatabaseFound");
			Strings.stringIDs.Add(3296071226U, "VerboseADObjectNoChangedProperties");
			Strings.stringIDs.Add(2442074922U, "VerbosePopulateScopeSet");
			Strings.stringIDs.Add(2293662344U, "ErrorCertificateDenied");
			Strings.stringIDs.Add(4083645591U, "ErrorCannotResolvePUIDToWindowsIdentity");
			Strings.stringIDs.Add(2468805291U, "ErrorMissOrganization");
			Strings.stringIDs.Add(1891802266U, "ExceptionMDAConnectionAlreadyOpened");
			Strings.stringIDs.Add(982491582U, "ExceptionObjectStillExists");
			Strings.stringIDs.Add(3844753652U, "ExceptionMDAConnectionMustBeOpened");
			Strings.stringIDs.Add(1602649260U, "WriteErrorMessage");
			Strings.stringIDs.Add(4100583810U, "GenericConditionFailure");
			Strings.stringIDs.Add(4285414215U, "VerboseLbExRpcAdminExists");
			Strings.stringIDs.Add(325596373U, "DisabledString");
			Strings.stringIDs.Add(2781337548U, "ExceptionRemoveNoneExistenceObject");
			Strings.stringIDs.Add(492587358U, "LogErrorPrefix");
			Strings.stringIDs.Add(1558360907U, "ErrorMapiPublicFolderTreeNotFound");
		}

		public static LocalizedString LoadingRole(string account)
		{
			return new LocalizedString("LoadingRole", "ExDEEEDC", false, true, Strings.ResourceManager, new object[]
			{
				account
			});
		}

		public static LocalizedString LookupUserAsSAMAccount(string user)
		{
			return new LocalizedString("LookupUserAsSAMAccount", "Ex2FA584", false, true, Strings.ResourceManager, new object[]
			{
				user
			});
		}

		public static LocalizedString MissingImpersonatedUserSid
		{
			get
			{
				return new LocalizedString("MissingImpersonatedUserSid", "Ex3FE822", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString VerboseLbOABOwnedByServer(string oab, string server)
		{
			return new LocalizedString("VerboseLbOABOwnedByServer", "ExA33016", false, true, Strings.ResourceManager, new object[]
			{
				oab,
				server
			});
		}

		public static LocalizedString VerboseLbNoServerForDatabaseException(string errorMessage)
		{
			return new LocalizedString("VerboseLbNoServerForDatabaseException", "Ex2AD546", false, true, Strings.ResourceManager, new object[]
			{
				errorMessage
			});
		}

		public static LocalizedString WrongTypeMailboxUser(string identity)
		{
			return new LocalizedString("WrongTypeMailboxUser", "ExD00A0F", false, true, Strings.ResourceManager, new object[]
			{
				identity
			});
		}

		public static LocalizedString VerboseRequestFilterInGetTask(string filter)
		{
			return new LocalizedString("VerboseRequestFilterInGetTask", "", false, false, Strings.ResourceManager, new object[]
			{
				filter
			});
		}

		public static LocalizedString CannotFindClassFactoryInAgentAssembly(string location)
		{
			return new LocalizedString("CannotFindClassFactoryInAgentAssembly", "ExCB2B56", false, true, Strings.ResourceManager, new object[]
			{
				location
			});
		}

		public static LocalizedString ErrorInvalidStatePartnerOrgNotNull(string account)
		{
			return new LocalizedString("ErrorInvalidStatePartnerOrgNotNull", "Ex6D944B", false, true, Strings.ResourceManager, new object[]
			{
				account
			});
		}

		public static LocalizedString LoadingScopeErrorText(string account)
		{
			return new LocalizedString("LoadingScopeErrorText", "Ex88621B", false, true, Strings.ResourceManager, new object[]
			{
				account
			});
		}

		public static LocalizedString VerboseLbOABIsCurrentlyOnServer(string currentServer)
		{
			return new LocalizedString("VerboseLbOABIsCurrentlyOnServer", "ExFF7A07", false, true, Strings.ResourceManager, new object[]
			{
				currentServer
			});
		}

		public static LocalizedString AgentAssemblyDuplicateFound(string assemblyLocation)
		{
			return new LocalizedString("AgentAssemblyDuplicateFound", "ExAE0974", false, true, Strings.ResourceManager, new object[]
			{
				assemblyLocation
			});
		}

		public static LocalizedString LogHelpUrl(string helpUrl)
		{
			return new LocalizedString("LogHelpUrl", "", false, false, Strings.ResourceManager, new object[]
			{
				helpUrl
			});
		}

		public static LocalizedString NoRoleEntriesFound(string exchangeCmdletName)
		{
			return new LocalizedString("NoRoleEntriesFound", "Ex0BA5B8", false, true, Strings.ResourceManager, new object[]
			{
				exchangeCmdletName
			});
		}

		public static LocalizedString VerboseLbPermanentException(string errorMessage)
		{
			return new LocalizedString("VerboseLbPermanentException", "Ex7DABDA", false, true, Strings.ResourceManager, new object[]
			{
				errorMessage
			});
		}

		public static LocalizedString ErrorIncompleteDCPublicFolderIdParameter(string parameterName)
		{
			return new LocalizedString("ErrorIncompleteDCPublicFolderIdParameter", "ExDA24DC", false, true, Strings.ResourceManager, new object[]
			{
				parameterName
			});
		}

		public static LocalizedString FalseString
		{
			get
			{
				return new LocalizedString("FalseString", "Ex38B631", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorInvalidServerName(string serverName)
		{
			return new LocalizedString("ErrorInvalidServerName", "ExD434BB", false, true, Strings.ResourceManager, new object[]
			{
				serverName
			});
		}

		public static LocalizedString VerboseLbNoAvailableE15Database(int count)
		{
			return new LocalizedString("VerboseLbNoAvailableE15Database", "", false, false, Strings.ResourceManager, new object[]
			{
				count
			});
		}

		public static LocalizedString MutuallyExclusiveArguments(string arg1, string arg2)
		{
			return new LocalizedString("MutuallyExclusiveArguments", "Ex190776", false, true, Strings.ResourceManager, new object[]
			{
				arg1,
				arg2
			});
		}

		public static LocalizedString LogResolverInstantiated(Type taskType, Condition condition)
		{
			return new LocalizedString("LogResolverInstantiated", "Ex234AF0", false, true, Strings.ResourceManager, new object[]
			{
				taskType,
				condition
			});
		}

		public static LocalizedString ProvisionDefaultProperties(int handlerIndex)
		{
			return new LocalizedString("ProvisionDefaultProperties", "", false, false, Strings.ResourceManager, new object[]
			{
				handlerIndex
			});
		}

		public static LocalizedString VerboseReadADObject(string id, string type)
		{
			return new LocalizedString("VerboseReadADObject", "Ex9FC538", false, true, Strings.ResourceManager, new object[]
			{
				id,
				type
			});
		}

		public static LocalizedString ResourceLoadDelayNotEnforcedMaxThreadsExceeded(int cappedDelay, bool required, string resource, string load, int threadNum)
		{
			return new LocalizedString("ResourceLoadDelayNotEnforcedMaxThreadsExceeded", "", false, false, Strings.ResourceManager, new object[]
			{
				cappedDelay,
				required,
				resource,
				load,
				threadNum
			});
		}

		public static LocalizedString ErrorNotAcceptedDomain(string domain)
		{
			return new LocalizedString("ErrorNotAcceptedDomain", "ExD48BF8", false, true, Strings.ResourceManager, new object[]
			{
				domain
			});
		}

		public static LocalizedString SortOrderFormatException(string input)
		{
			return new LocalizedString("SortOrderFormatException", "ExD6FEB2", false, true, Strings.ResourceManager, new object[]
			{
				input
			});
		}

		public static LocalizedString VerboseFailedToGetServiceTopology
		{
			get
			{
				return new LocalizedString("VerboseFailedToGetServiceTopology", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString LoadingScope(string account)
		{
			return new LocalizedString("LoadingScope", "Ex99C3E1", false, true, Strings.ResourceManager, new object[]
			{
				account
			});
		}

		public static LocalizedString ExceptionObjectNotFound(string objectType)
		{
			return new LocalizedString("ExceptionObjectNotFound", "Ex71B980", false, true, Strings.ResourceManager, new object[]
			{
				objectType
			});
		}

		public static LocalizedString WrongTypeMailboxOrMailUser(string identity)
		{
			return new LocalizedString("WrongTypeMailboxOrMailUser", "ExA506BE", false, true, Strings.ResourceManager, new object[]
			{
				identity
			});
		}

		public static LocalizedString ErrorPolicyUserOrSecurityGroupNotFound(string id)
		{
			return new LocalizedString("ErrorPolicyUserOrSecurityGroupNotFound", "Ex976174", false, true, Strings.ResourceManager, new object[]
			{
				id
			});
		}

		public static LocalizedString ExceptionSetupRegkeyMissing(string keyPath, string valueName)
		{
			return new LocalizedString("ExceptionSetupRegkeyMissing", "ExDA8280", false, true, Strings.ResourceManager, new object[]
			{
				keyPath,
				valueName
			});
		}

		public static LocalizedString PswsDeserializationError(string errorMessage)
		{
			return new LocalizedString("PswsDeserializationError", "", false, false, Strings.ResourceManager, new object[]
			{
				errorMessage
			});
		}

		public static LocalizedString OnbehalfOf(string logonIdentity, string accessIdentity)
		{
			return new LocalizedString("OnbehalfOf", "ExEB9E55", false, true, Strings.ResourceManager, new object[]
			{
				logonIdentity,
				accessIdentity
			});
		}

		public static LocalizedString NotGrantCustomScriptRole
		{
			get
			{
				return new LocalizedString("NotGrantCustomScriptRole", "ExB022F4", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorIncompletePublicFolderIdParameter(string parameterName)
		{
			return new LocalizedString("ErrorIncompletePublicFolderIdParameter", "", false, false, Strings.ResourceManager, new object[]
			{
				parameterName
			});
		}

		public static LocalizedString VerboseFailedToReadFromDC(string id, string dc)
		{
			return new LocalizedString("VerboseFailedToReadFromDC", "Ex9559FB", false, true, Strings.ResourceManager, new object[]
			{
				id,
				dc
			});
		}

		public static LocalizedString ErrorNotMailboxServer(string server)
		{
			return new LocalizedString("ErrorNotMailboxServer", "Ex383749", false, true, Strings.ResourceManager, new object[]
			{
				server
			});
		}

		public static LocalizedString ExceptionNoConversion(Type oldType, Type newType)
		{
			return new LocalizedString("ExceptionNoConversion", "Ex48AF25", false, true, Strings.ResourceManager, new object[]
			{
				oldType,
				newType
			});
		}

		public static LocalizedString ErrorChangeImmutableType
		{
			get
			{
				return new LocalizedString("ErrorChangeImmutableType", "Ex8959CE", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString PropertyProvisioned(int i, string propertyName, string propertyValue)
		{
			return new LocalizedString("PropertyProvisioned", "Ex81D4C3", false, true, Strings.ResourceManager, new object[]
			{
				i,
				propertyName,
				propertyValue
			});
		}

		public static LocalizedString VerboseTaskReadDataObject(string id, string type)
		{
			return new LocalizedString("VerboseTaskReadDataObject", "Ex398E60", false, true, Strings.ResourceManager, new object[]
			{
				id,
				type
			});
		}

		public static LocalizedString NotGrantAnyAdminRoles
		{
			get
			{
				return new LocalizedString("NotGrantAnyAdminRoles", "Ex57FC72", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString UnExpectedElement(string expectedElement, string actualElement)
		{
			return new LocalizedString("UnExpectedElement", "ExC11811", false, true, Strings.ResourceManager, new object[]
			{
				expectedElement,
				actualElement
			});
		}

		public static LocalizedString ExceptionLegacyObjects(string identities)
		{
			return new LocalizedString("ExceptionLegacyObjects", "ExC72065", false, true, Strings.ResourceManager, new object[]
			{
				identities
			});
		}

		public static LocalizedString ExceptionObjectAmbiguous(string objectType)
		{
			return new LocalizedString("ExceptionObjectAmbiguous", "ExE26139", false, true, Strings.ResourceManager, new object[]
			{
				objectType
			});
		}

		public static LocalizedString VerboseLbInitialProvisioningDatabaseExcluded(string databaseName)
		{
			return new LocalizedString("VerboseLbInitialProvisioningDatabaseExcluded", "", false, false, Strings.ResourceManager, new object[]
			{
				databaseName
			});
		}

		public static LocalizedString ErrorOnlyDNSupportedWithIgnoreDefaultScope
		{
			get
			{
				return new LocalizedString("ErrorOnlyDNSupportedWithIgnoreDefaultScope", "Ex922448", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ExceptionTaskNotInitialized
		{
			get
			{
				return new LocalizedString("ExceptionTaskNotInitialized", "Ex215352", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorMaxTenantPSConnectionLimit(string orgName)
		{
			return new LocalizedString("ErrorMaxTenantPSConnectionLimit", "Ex94B605", false, true, Strings.ResourceManager, new object[]
			{
				orgName
			});
		}

		public static LocalizedString VerboseLbNoDatabaseFoundInAD
		{
			get
			{
				return new LocalizedString("VerboseLbNoDatabaseFoundInAD", "ExEFC91E", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ProvisioningPreInternalProcessRecord(int handlerIndex, bool objectChangedFlag)
		{
			return new LocalizedString("ProvisioningPreInternalProcessRecord", "Ex8CC553", false, true, Strings.ResourceManager, new object[]
			{
				handlerIndex,
				objectChangedFlag
			});
		}

		public static LocalizedString VerboseLbDatabase(string databaseDn)
		{
			return new LocalizedString("VerboseLbDatabase", "Ex47BB9E", false, true, Strings.ResourceManager, new object[]
			{
				databaseDn
			});
		}

		public static LocalizedString WorkUnitStatusNotStarted
		{
			get
			{
				return new LocalizedString("WorkUnitStatusNotStarted", "Ex70C849", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString VerboseSourceFromDC(string source)
		{
			return new LocalizedString("VerboseSourceFromDC", "ExA2D61C", false, true, Strings.ResourceManager, new object[]
			{
				source
			});
		}

		public static LocalizedString TrueString
		{
			get
			{
				return new LocalizedString("TrueString", "ExE4D1B7", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString VerboseLbIsDatabaseLocal(string databaseName, string databaseSite, string localSite)
		{
			return new LocalizedString("VerboseLbIsDatabaseLocal", "", false, false, Strings.ResourceManager, new object[]
			{
				databaseName,
				databaseSite,
				localSite
			});
		}

		public static LocalizedString InvalidPswsDirectInvocationBlocked
		{
			get
			{
				return new LocalizedString("InvalidPswsDirectInvocationBlocked", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorOrgOutOfPartnerScope(string identity, string org)
		{
			return new LocalizedString("ErrorOrgOutOfPartnerScope", "Ex892E1E", false, true, Strings.ResourceManager, new object[]
			{
				identity,
				org
			});
		}

		public static LocalizedString ServiceAlreadyNotInstalled(string servicename)
		{
			return new LocalizedString("ServiceAlreadyNotInstalled", "Ex38C2AA", false, true, Strings.ResourceManager, new object[]
			{
				servicename
			});
		}

		public static LocalizedString WarningCmdletTarpittingByResourceLoad(string resourceKey, string delaySeconds)
		{
			return new LocalizedString("WarningCmdletTarpittingByResourceLoad", "", false, false, Strings.ResourceManager, new object[]
			{
				resourceKey,
				delaySeconds
			});
		}

		public static LocalizedString ErrorManagementObjectNotFoundByType(string type)
		{
			return new LocalizedString("ErrorManagementObjectNotFoundByType", "ExB4C70B", false, true, Strings.ResourceManager, new object[]
			{
				type
			});
		}

		public static LocalizedString VerboseLbSitesAreNotBalanced(string localsite, int numberofdbs)
		{
			return new LocalizedString("VerboseLbSitesAreNotBalanced", "Ex1D6A4F", false, true, Strings.ResourceManager, new object[]
			{
				localsite,
				numberofdbs
			});
		}

		public static LocalizedString ErrorUserNotFound(string idStringValue)
		{
			return new LocalizedString("ErrorUserNotFound", "Ex9BC9DD", false, true, Strings.ResourceManager, new object[]
			{
				idStringValue
			});
		}

		public static LocalizedString PswsResponseChildElementNotExisingError(string parentElement, string name)
		{
			return new LocalizedString("PswsResponseChildElementNotExisingError", "", false, false, Strings.ResourceManager, new object[]
			{
				parentElement,
				name
			});
		}

		public static LocalizedString VerboseLbReturningServer(string server)
		{
			return new LocalizedString("VerboseLbReturningServer", "Ex286002", false, true, Strings.ResourceManager, new object[]
			{
				server
			});
		}

		public static LocalizedString ErrorConfigurationUnitNotFound(string idStringValue)
		{
			return new LocalizedString("ErrorConfigurationUnitNotFound", "ExB12F6E", false, true, Strings.ResourceManager, new object[]
			{
				idStringValue
			});
		}

		public static LocalizedString InvalidProperties
		{
			get
			{
				return new LocalizedString("InvalidProperties", "Ex24B8E3", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString PswsRequestException(string errorMessage)
		{
			return new LocalizedString("PswsRequestException", "", false, false, Strings.ResourceManager, new object[]
			{
				errorMessage
			});
		}

		public static LocalizedString ErrorAddressListNotFound(string idStringValue)
		{
			return new LocalizedString("ErrorAddressListNotFound", "Ex54304E", false, true, Strings.ResourceManager, new object[]
			{
				idStringValue
			});
		}

		public static LocalizedString ErrorNoPartnerScopes(string identity)
		{
			return new LocalizedString("ErrorNoPartnerScopes", "Ex5F2E70", false, true, Strings.ResourceManager, new object[]
			{
				identity
			});
		}

		public static LocalizedString VerboseSkipObject(string id)
		{
			return new LocalizedString("VerboseSkipObject", "ExA55BB6", false, true, Strings.ResourceManager, new object[]
			{
				id
			});
		}

		public static LocalizedString ErrorPolicyUserOrSecurityGroupNotUnique(string id)
		{
			return new LocalizedString("ErrorPolicyUserOrSecurityGroupNotUnique", "ExF91DF1", false, true, Strings.ResourceManager, new object[]
			{
				id
			});
		}

		public static LocalizedString ErrorParentHasNewerVersion(string identity, string objectVersion, string parentVersion)
		{
			return new LocalizedString("ErrorParentHasNewerVersion", "ExA41B19", false, true, Strings.ResourceManager, new object[]
			{
				identity,
				objectVersion,
				parentVersion
			});
		}

		public static LocalizedString VerboseAdminSessionSettingsUserConfigDC(string configDC)
		{
			return new LocalizedString("VerboseAdminSessionSettingsUserConfigDC", "Ex6B05BB", false, true, Strings.ResourceManager, new object[]
			{
				configDC
			});
		}

		public static LocalizedString LogConditionFailed(Type conditionType, bool expectedResult)
		{
			return new LocalizedString("LogConditionFailed", "ExCB9B43", false, true, Strings.ResourceManager, new object[]
			{
				conditionType,
				expectedResult
			});
		}

		public static LocalizedString LogFunctionExit(Type type, string methodName)
		{
			return new LocalizedString("LogFunctionExit", "Ex689125", false, true, Strings.ResourceManager, new object[]
			{
				type,
				methodName
			});
		}

		public static LocalizedString WrongTypeMailboxRecipient(string identity)
		{
			return new LocalizedString("WrongTypeMailboxRecipient", "", false, false, Strings.ResourceManager, new object[]
			{
				identity
			});
		}

		public static LocalizedString WrongTypeUser(string identity)
		{
			return new LocalizedString("WrongTypeUser", "Ex01488F", false, true, Strings.ResourceManager, new object[]
			{
				identity
			});
		}

		public static LocalizedString WillUninstallInstalledService(string name)
		{
			return new LocalizedString("WillUninstallInstalledService", "ExDADBB1", false, true, Strings.ResourceManager, new object[]
			{
				name
			});
		}

		public static LocalizedString ExceptionMissingDetailSchemaFile(string masterSchemaFileName, string className)
		{
			return new LocalizedString("ExceptionMissingDetailSchemaFile", "ExC43184", false, true, Strings.ResourceManager, new object[]
			{
				masterSchemaFileName,
				className
			});
		}

		public static LocalizedString ErrorInstanceObjectConatinsNullIdentity
		{
			get
			{
				return new LocalizedString("ErrorInstanceObjectConatinsNullIdentity", "ExDAF7E8", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MonitoringPerfomanceCounterString(string perfObject, string counter, string instance, double value)
		{
			return new LocalizedString("MonitoringPerfomanceCounterString", "Ex293136", false, true, Strings.ResourceManager, new object[]
			{
				perfObject,
				counter,
				instance,
				value
			});
		}

		public static LocalizedString VerboseAdminSessionSettingsUserDCs(string DCs)
		{
			return new LocalizedString("VerboseAdminSessionSettingsUserDCs", "Ex9D7640", false, true, Strings.ResourceManager, new object[]
			{
				DCs
			});
		}

		public static LocalizedString WarningDefaultResultSizeReached(string resultSize)
		{
			return new LocalizedString("WarningDefaultResultSizeReached", "Ex2E94E0", false, true, Strings.ResourceManager, new object[]
			{
				resultSize
			});
		}

		public static LocalizedString ErrorAdminLoginUsingAppPassword
		{
			get
			{
				return new LocalizedString("ErrorAdminLoginUsingAppPassword", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorCannotFindMailboxToLogonPublicStore
		{
			get
			{
				return new LocalizedString("ErrorCannotFindMailboxToLogonPublicStore", "Ex6E586E", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString UnknownAuditManagerType(string auditorType)
		{
			return new LocalizedString("UnknownAuditManagerType", "ExD0AACE", false, true, Strings.ResourceManager, new object[]
			{
				auditorType
			});
		}

		public static LocalizedString ExceptionRoleNotFoundObjects(string identities)
		{
			return new LocalizedString("ExceptionRoleNotFoundObjects", "Ex144F4E", false, true, Strings.ResourceManager, new object[]
			{
				identities
			});
		}

		public static LocalizedString LogTaskCandidate(Type taskType)
		{
			return new LocalizedString("LogTaskCandidate", "ExA3168F", false, true, Strings.ResourceManager, new object[]
			{
				taskType
			});
		}

		public static LocalizedString WorkUnitStatusInProgress
		{
			get
			{
				return new LocalizedString("WorkUnitStatusInProgress", "Ex7DA896", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString VerboseTaskProcessingObject(string id)
		{
			return new LocalizedString("VerboseTaskProcessingObject", "ExC96184", false, true, Strings.ResourceManager, new object[]
			{
				id
			});
		}

		public static LocalizedString ErrorObjectHasValidationErrors
		{
			get
			{
				return new LocalizedString("ErrorObjectHasValidationErrors", "ExABE500", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorNoProvisioningHandlerAvailable
		{
			get
			{
				return new LocalizedString("ErrorNoProvisioningHandlerAvailable", "Ex4FB266", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString VerboseRereadADObject(string id, string type, string root)
		{
			return new LocalizedString("VerboseRereadADObject", "Ex0EC40A", false, true, Strings.ResourceManager, new object[]
			{
				id,
				type,
				root
			});
		}

		public static LocalizedString InvocationExceptionDescriptionWithoutError(string commandText)
		{
			return new LocalizedString("InvocationExceptionDescriptionWithoutError", "Ex8C7580", false, true, Strings.ResourceManager, new object[]
			{
				commandText
			});
		}

		public static LocalizedString CouldntFindClassFactoryInAssembly(string classFactoryName, string assembly)
		{
			return new LocalizedString("CouldntFindClassFactoryInAssembly", "Ex0BEA57", false, true, Strings.ResourceManager, new object[]
			{
				classFactoryName,
				assembly
			});
		}

		public static LocalizedString WarningUnlicensedMailbox
		{
			get
			{
				return new LocalizedString("WarningUnlicensedMailbox", "Ex185717", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString VerboseAdminSessionSettingsDefaultScope(string defaultScope)
		{
			return new LocalizedString("VerboseAdminSessionSettingsDefaultScope", "Ex0535F9", false, true, Strings.ResourceManager, new object[]
			{
				defaultScope
			});
		}

		public static LocalizedString WarningCannotWriteToEventLog(string reason)
		{
			return new LocalizedString("WarningCannotWriteToEventLog", "ExF96315", false, true, Strings.ResourceManager, new object[]
			{
				reason
			});
		}

		public static LocalizedString InvalidAttribute(string attribute)
		{
			return new LocalizedString("InvalidAttribute", "Ex248A23", false, true, Strings.ResourceManager, new object[]
			{
				attribute
			});
		}

		public static LocalizedString ExceptionMismatchedConfigObjectType(Type configuredType, Type usedType)
		{
			return new LocalizedString("ExceptionMismatchedConfigObjectType", "Ex112866", false, true, Strings.ResourceManager, new object[]
			{
				configuredType,
				usedType
			});
		}

		public static LocalizedString VerboseSource(string source)
		{
			return new LocalizedString("VerboseSource", "Ex6BDE38", false, true, Strings.ResourceManager, new object[]
			{
				source
			});
		}

		public static LocalizedString WorkUnitStatusFailed
		{
			get
			{
				return new LocalizedString("WorkUnitStatusFailed", "Ex83221A", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString VerboseInitializeRunspaceServerSettingsRemote
		{
			get
			{
				return new LocalizedString("VerboseInitializeRunspaceServerSettingsRemote", "ExF7B42B", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ExceptionTaskInconsistent
		{
			get
			{
				return new LocalizedString("ExceptionTaskInconsistent", "Ex179672", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString WarningForceMessage
		{
			get
			{
				return new LocalizedString("WarningForceMessage", "Ex2838CC", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString VerboseLbRetryableException(string errorMessage)
		{
			return new LocalizedString("VerboseLbRetryableException", "ExF7804A", false, true, Strings.ResourceManager, new object[]
			{
				errorMessage
			});
		}

		public static LocalizedString ErrorPartnerApplicationWithoutLinkedAccount(string pa)
		{
			return new LocalizedString("ErrorPartnerApplicationWithoutLinkedAccount", "", false, false, Strings.ResourceManager, new object[]
			{
				pa
			});
		}

		public static LocalizedString ErrorNoAvailablePublicFolderDatabaseInDatacenter(string organizationName)
		{
			return new LocalizedString("ErrorNoAvailablePublicFolderDatabaseInDatacenter", "ExAC8771", false, true, Strings.ResourceManager, new object[]
			{
				organizationName
			});
		}

		public static LocalizedString NoRequiredRole(string identity)
		{
			return new LocalizedString("NoRequiredRole", "Ex02B210", false, true, Strings.ResourceManager, new object[]
			{
				identity
			});
		}

		public static LocalizedString WarningCannotSetPrimarySmtpAddressWhenEapEnabled
		{
			get
			{
				return new LocalizedString("WarningCannotSetPrimarySmtpAddressWhenEapEnabled", "Ex58C382", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorOrganizationNotUnique(string idStringValue)
		{
			return new LocalizedString("ErrorOrganizationNotUnique", "ExD2094F", false, true, Strings.ResourceManager, new object[]
			{
				idStringValue
			});
		}

		public static LocalizedString ExecutingUserNameIsMissing
		{
			get
			{
				return new LocalizedString("ExecutingUserNameIsMissing", "ExC429BF", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString WrongTypeUserContactComputer(string identity)
		{
			return new LocalizedString("WrongTypeUserContactComputer", "ExF099FD", false, true, Strings.ResourceManager, new object[]
			{
				identity
			});
		}

		public static LocalizedString ErrorOperationTarpitting(int delaySeconds)
		{
			return new LocalizedString("ErrorOperationTarpitting", "", false, false, Strings.ResourceManager, new object[]
			{
				delaySeconds
			});
		}

		public static LocalizedString ConfigObjectAmbiguous(string identity, Type classType)
		{
			return new LocalizedString("ConfigObjectAmbiguous", "Ex3C06AB", false, true, Strings.ResourceManager, new object[]
			{
				identity,
				classType
			});
		}

		public static LocalizedString VerboseLbDatabaseIsNotOnline(int status)
		{
			return new LocalizedString("VerboseLbDatabaseIsNotOnline", "Ex9B0FE9", false, true, Strings.ResourceManager, new object[]
			{
				status
			});
		}

		public static LocalizedString InvalidNegativeValue(string name)
		{
			return new LocalizedString("InvalidNegativeValue", "Ex7DC091", false, true, Strings.ResourceManager, new object[]
			{
				name
			});
		}

		public static LocalizedString ErrorNotUserMailboxCanLogonPFDatabase(string pfdb)
		{
			return new LocalizedString("ErrorNotUserMailboxCanLogonPFDatabase", "ExC4308A", false, true, Strings.ResourceManager, new object[]
			{
				pfdb
			});
		}

		public static LocalizedString ExceptionNoChangesSpecified
		{
			get
			{
				return new LocalizedString("ExceptionNoChangesSpecified", "Ex960200", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString VerboseLbRemoteSiteDatabaseReturned(string database, string serverName)
		{
			return new LocalizedString("VerboseLbRemoteSiteDatabaseReturned", "", false, false, Strings.ResourceManager, new object[]
			{
				database,
				serverName
			});
		}

		public static LocalizedString RBACContextParserException(int lineNumber, int position, string reason)
		{
			return new LocalizedString("RBACContextParserException", "Ex25E296", false, true, Strings.ResourceManager, new object[]
			{
				lineNumber,
				position,
				reason
			});
		}

		public static LocalizedString ErrorCannotOpenService(string name)
		{
			return new LocalizedString("ErrorCannotOpenService", "Ex9D56F7", false, true, Strings.ResourceManager, new object[]
			{
				name
			});
		}

		public static LocalizedString ExArgumentNullException(string paramName)
		{
			return new LocalizedString("ExArgumentNullException", "Ex56EB59", false, true, Strings.ResourceManager, new object[]
			{
				paramName
			});
		}

		public static LocalizedString ExceptionTaskAlreadyInitialized
		{
			get
			{
				return new LocalizedString("ExceptionTaskAlreadyInitialized", "Ex9224EA", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString WarningWindowsEmailAddressTooLong(string recipient)
		{
			return new LocalizedString("WarningWindowsEmailAddressTooLong", "Ex4EE4B5", false, true, Strings.ResourceManager, new object[]
			{
				recipient
			});
		}

		public static LocalizedString PswsResponseElementNotExisingError(string documentXml, string xPath)
		{
			return new LocalizedString("PswsResponseElementNotExisingError", "", false, false, Strings.ResourceManager, new object[]
			{
				documentXml,
				xPath
			});
		}

		public static LocalizedString NoRoleEntriesWithParametersFound(string exchangeCmdletName)
		{
			return new LocalizedString("NoRoleEntriesWithParametersFound", "Ex5E8EA4", false, true, Strings.ResourceManager, new object[]
			{
				exchangeCmdletName
			});
		}

		public static LocalizedString VerbosePostConditions(string conditions)
		{
			return new LocalizedString("VerbosePostConditions", "Ex53712E", false, true, Strings.ResourceManager, new object[]
			{
				conditions
			});
		}

		public static LocalizedString ErrorProvisioningValidation(string description, string agentName)
		{
			return new LocalizedString("ErrorProvisioningValidation", "ExE71684", false, true, Strings.ResourceManager, new object[]
			{
				description,
				agentName
			});
		}

		public static LocalizedString ParameterValueTooLarge
		{
			get
			{
				return new LocalizedString("ParameterValueTooLarge", "Ex786531", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString UnhandledErrorMessage(string error)
		{
			return new LocalizedString("UnhandledErrorMessage", "Ex3BBD8E", false, true, Strings.ResourceManager, new object[]
			{
				error
			});
		}

		public static LocalizedString ErrorNotSupportSingletonWildcard
		{
			get
			{
				return new LocalizedString("ErrorNotSupportSingletonWildcard", "Ex047A07", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString WrongTypeMailContact(string identity)
		{
			return new LocalizedString("WrongTypeMailContact", "Ex72A78A", false, true, Strings.ResourceManager, new object[]
			{
				identity
			});
		}

		public static LocalizedString PiiRedactionInitializationFailed(string reason)
		{
			return new LocalizedString("PiiRedactionInitializationFailed", "", false, false, Strings.ResourceManager, new object[]
			{
				reason
			});
		}

		public static LocalizedString VerboseAdminSessionSettingsViewForest(string viewForest)
		{
			return new LocalizedString("VerboseAdminSessionSettingsViewForest", "Ex8656DA", false, true, Strings.ResourceManager, new object[]
			{
				viewForest
			});
		}

		public static LocalizedString VerboseRemovingRoleAssignment(string id)
		{
			return new LocalizedString("VerboseRemovingRoleAssignment", "Ex88CF02", false, true, Strings.ResourceManager, new object[]
			{
				id
			});
		}

		public static LocalizedString CommandNotFoundError(string cmdlet)
		{
			return new LocalizedString("CommandNotFoundError", "Ex61D0B7", false, true, Strings.ResourceManager, new object[]
			{
				cmdlet
			});
		}

		public static LocalizedString WrongTypeMailPublicFolder(string identity)
		{
			return new LocalizedString("WrongTypeMailPublicFolder", "Ex150E3C", false, true, Strings.ResourceManager, new object[]
			{
				identity
			});
		}

		public static LocalizedString WorkUnitCollectionConfigurationSummary
		{
			get
			{
				return new LocalizedString("WorkUnitCollectionConfigurationSummary", "ExFCCA37", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ExceptionMDACommandNotExecuting
		{
			get
			{
				return new LocalizedString("ExceptionMDACommandNotExecuting", "ExCAC498", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString OverallElapsedTimeDescription(int h, int mm, int ss)
		{
			return new LocalizedString("OverallElapsedTimeDescription", "Ex26F2ED", false, true, Strings.ResourceManager, new object[]
			{
				h,
				mm,
				ss
			});
		}

		public static LocalizedString ExceptionMDACommandExcutionError(int innerErrorCode, string command)
		{
			return new LocalizedString("ExceptionMDACommandExcutionError", "Ex699F1A", false, true, Strings.ResourceManager, new object[]
			{
				innerErrorCode,
				command
			});
		}

		public static LocalizedString ErrorRecipientPropertyValueAlreadybeUsed(string property, string value)
		{
			return new LocalizedString("ErrorRecipientPropertyValueAlreadybeUsed", "", false, false, Strings.ResourceManager, new object[]
			{
				property,
				value
			});
		}

		public static LocalizedString ErrorManagementObjectNotFound(string id)
		{
			return new LocalizedString("ErrorManagementObjectNotFound", "Ex43C0AC", false, true, Strings.ResourceManager, new object[]
			{
				id
			});
		}

		public static LocalizedString ErrorInvalidMailboxStoreObjectIdentity(string input)
		{
			return new LocalizedString("ErrorInvalidMailboxStoreObjectIdentity", "ExE2F600", false, true, Strings.ResourceManager, new object[]
			{
				input
			});
		}

		public static LocalizedString ErrorCmdletProxy(string command, string serverFqn, string serverVersion, string proxyMethod, string errorMessage)
		{
			return new LocalizedString("ErrorCmdletProxy", "", false, false, Strings.ResourceManager, new object[]
			{
				command,
				serverFqn,
				serverVersion,
				proxyMethod,
				errorMessage
			});
		}

		public static LocalizedString WarningCouldNotRemoveRoleAssignment(string id, string error)
		{
			return new LocalizedString("WarningCouldNotRemoveRoleAssignment", "Ex7E7F41", false, true, Strings.ResourceManager, new object[]
			{
				id,
				error
			});
		}

		public static LocalizedString VerboseDeleteObject(string id, string type)
		{
			return new LocalizedString("VerboseDeleteObject", "Ex65B31E", false, true, Strings.ResourceManager, new object[]
			{
				id,
				type
			});
		}

		public static LocalizedString VerboseCmdletProxiedToAnotherServer(string cmdlet, string server, string serverVersion, string proxyMethod)
		{
			return new LocalizedString("VerboseCmdletProxiedToAnotherServer", "", false, false, Strings.ResourceManager, new object[]
			{
				cmdlet,
				server,
				serverVersion,
				proxyMethod
			});
		}

		public static LocalizedString ExceptionInvalidDatabaseLegacyDnFormat(string legacyDn)
		{
			return new LocalizedString("ExceptionInvalidDatabaseLegacyDnFormat", "ExAAE39F", false, true, Strings.ResourceManager, new object[]
			{
				legacyDn
			});
		}

		public static LocalizedString LogFunctionEnter(Type type, string methodName, string argumentList)
		{
			return new LocalizedString("LogFunctionEnter", "Ex37DFC4", false, true, Strings.ResourceManager, new object[]
			{
				type,
				methodName,
				argumentList
			});
		}

		public static LocalizedString VerboseAdminSessionSettingsUserGlobalCatalog(string globalCatalog)
		{
			return new LocalizedString("VerboseAdminSessionSettingsUserGlobalCatalog", "ExED5741", false, true, Strings.ResourceManager, new object[]
			{
				globalCatalog
			});
		}

		public static LocalizedString CouldNotDeterimineServiceInstanceException(string domainName)
		{
			return new LocalizedString("CouldNotDeterimineServiceInstanceException", "Ex3EA890", false, true, Strings.ResourceManager, new object[]
			{
				domainName
			});
		}

		public static LocalizedString ErrorInvalidParameterFormat(string parameterName)
		{
			return new LocalizedString("ErrorInvalidParameterFormat", "ExAB5EC3", false, true, Strings.ResourceManager, new object[]
			{
				parameterName
			});
		}

		public static LocalizedString VerboseAdminSessionSettingsUserAFGlobalCatalog(string globalCatalog)
		{
			return new LocalizedString("VerboseAdminSessionSettingsUserAFGlobalCatalog", "", false, false, Strings.ResourceManager, new object[]
			{
				globalCatalog
			});
		}

		public static LocalizedString WrongTypeSecurityPrincipal(string identity)
		{
			return new LocalizedString("WrongTypeSecurityPrincipal", "ExD4DC3B", false, true, Strings.ResourceManager, new object[]
			{
				identity
			});
		}

		public static LocalizedString ErrorRemotePowershellConnectionBlocked
		{
			get
			{
				return new LocalizedString("ErrorRemotePowershellConnectionBlocked", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString LogExecutionFailed
		{
			get
			{
				return new LocalizedString("LogExecutionFailed", "ExCD1BDD", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorInvalidGlobalAddressListIdentity(string idValue)
		{
			return new LocalizedString("ErrorInvalidGlobalAddressListIdentity", "ExCACD99", false, true, Strings.ResourceManager, new object[]
			{
				idValue
			});
		}

		public static LocalizedString VerboseLbNoOabVDirReturned
		{
			get
			{
				return new LocalizedString("VerboseLbNoOabVDirReturned", "Ex5B5C0A", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString WarningTaskRetried(string exception)
		{
			return new LocalizedString("WarningTaskRetried", "", false, false, Strings.ResourceManager, new object[]
			{
				exception
			});
		}

		public static LocalizedString VerboseLbEnterSiteMailboxEnterprise
		{
			get
			{
				return new LocalizedString("VerboseLbEnterSiteMailboxEnterprise", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString AssemblyFileNotFound(string fileName)
		{
			return new LocalizedString("AssemblyFileNotFound", "Ex015D0B", false, true, Strings.ResourceManager, new object[]
			{
				fileName
			});
		}

		public static LocalizedString HierarchicalIdentityNullOrEmpty
		{
			get
			{
				return new LocalizedString("HierarchicalIdentityNullOrEmpty", "Ex9A9CC7", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString VerboseLbDatabaseContainer(string dbContainerDn)
		{
			return new LocalizedString("VerboseLbDatabaseContainer", "ExFF1AF9", false, true, Strings.ResourceManager, new object[]
			{
				dbContainerDn
			});
		}

		public static LocalizedString ExceptionObjectAlreadyExists
		{
			get
			{
				return new LocalizedString("ExceptionObjectAlreadyExists", "ExC69E3F", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorProxyAddressAlreadyExists(string address, string user)
		{
			return new LocalizedString("ErrorProxyAddressAlreadyExists", "Ex1472D1", false, true, Strings.ResourceManager, new object[]
			{
				address,
				user
			});
		}

		public static LocalizedString VerboseCannotGetRemoteServiceUriForUser(string id, string proxyAddress, string reason)
		{
			return new LocalizedString("VerboseCannotGetRemoteServiceUriForUser", "", false, false, Strings.ResourceManager, new object[]
			{
				id,
				proxyAddress,
				reason
			});
		}

		public static LocalizedString VerboseAdminSessionSettingsUserAFConfigDC(string configDC)
		{
			return new LocalizedString("VerboseAdminSessionSettingsUserAFConfigDC", "", false, false, Strings.ResourceManager, new object[]
			{
				configDC
			});
		}

		public static LocalizedString ErrorRemotePowerShellNotEnabled
		{
			get
			{
				return new LocalizedString("ErrorRemotePowerShellNotEnabled", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString VerboseLbCountOfOABRecordsOwnedByServer(string server, int count)
		{
			return new LocalizedString("VerboseLbCountOfOABRecordsOwnedByServer", "Ex5AE5F8", false, true, Strings.ResourceManager, new object[]
			{
				server,
				count
			});
		}

		public static LocalizedString PswsSerializationError(string errorMessage)
		{
			return new LocalizedString("PswsSerializationError", "", false, false, Strings.ResourceManager, new object[]
			{
				errorMessage
			});
		}

		public static LocalizedString CouldNotStartService(string servicename)
		{
			return new LocalizedString("CouldNotStartService", "Ex87696D", false, true, Strings.ResourceManager, new object[]
			{
				servicename
			});
		}

		public static LocalizedString VerboseTaskGetDataObjects(string id, string type, string root)
		{
			return new LocalizedString("VerboseTaskGetDataObjects", "ExB38F79", false, true, Strings.ResourceManager, new object[]
			{
				id,
				type,
				root
			});
		}

		public static LocalizedString ErrorPublicFolderMailDisabled(string identity)
		{
			return new LocalizedString("ErrorPublicFolderMailDisabled", "Ex2BC3FC", false, true, Strings.ResourceManager, new object[]
			{
				identity
			});
		}

		public static LocalizedString VerboseLbIsLocalSiteNotEnoughInformation(string database, string databaseSite, string localSite)
		{
			return new LocalizedString("VerboseLbIsLocalSiteNotEnoughInformation", "ExE42B4F", false, true, Strings.ResourceManager, new object[]
			{
				database,
				databaseSite,
				localSite
			});
		}

		public static LocalizedString ErrorRecipientNotFound(string idStringValue)
		{
			return new LocalizedString("ErrorRecipientNotFound", "Ex914C70", false, true, Strings.ResourceManager, new object[]
			{
				idStringValue
			});
		}

		public static LocalizedString VerboseAdminSessionSettingsRecipientViewRoot(string recipientViewRoot)
		{
			return new LocalizedString("VerboseAdminSessionSettingsRecipientViewRoot", "ExBD2683", false, true, Strings.ResourceManager, new object[]
			{
				recipientViewRoot
			});
		}

		public static LocalizedString VerboseLbFoundOabVDir(string vdirDn, int count)
		{
			return new LocalizedString("VerboseLbFoundOabVDir", "Ex8C45CE", false, true, Strings.ResourceManager, new object[]
			{
				vdirDn,
				count
			});
		}

		public static LocalizedString ErrorManagementObjectNotFoundWithSource(string id, string source)
		{
			return new LocalizedString("ErrorManagementObjectNotFoundWithSource", "Ex6F9304", false, true, Strings.ResourceManager, new object[]
			{
				id,
				source
			});
		}

		public static LocalizedString MicroDelayNotEnforcedMaxThreadsExceeded(int cappedDelay, bool required, int threadNum)
		{
			return new LocalizedString("MicroDelayNotEnforcedMaxThreadsExceeded", "", false, false, Strings.ResourceManager, new object[]
			{
				cappedDelay,
				required,
				threadNum
			});
		}

		public static LocalizedString ServiceStopFailure(string name, string msg)
		{
			return new LocalizedString("ServiceStopFailure", "Ex28E845", false, true, Strings.ResourceManager, new object[]
			{
				name,
				msg
			});
		}

		public static LocalizedString ErrorRbacConfigurationNotSupportedSharedConfiguration
		{
			get
			{
				return new LocalizedString("ErrorRbacConfigurationNotSupportedSharedConfiguration", "Ex041D05", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorRecipientPropertyValueAlreadyExists(string property, string value, string existingRecipientId)
		{
			return new LocalizedString("ErrorRecipientPropertyValueAlreadyExists", "Ex362523", false, true, Strings.ResourceManager, new object[]
			{
				property,
				value,
				existingRecipientId
			});
		}

		public static LocalizedString UnknownEnumValue
		{
			get
			{
				return new LocalizedString("UnknownEnumValue", "Ex813DAD", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString VerboseTaskFindDataObjectsInAL(string type, string addressList)
		{
			return new LocalizedString("VerboseTaskFindDataObjectsInAL", "Ex4E9AE8", false, true, Strings.ResourceManager, new object[]
			{
				type,
				addressList
			});
		}

		public static LocalizedString MultipleDefaultMailboxPlansFound(string id, string dc)
		{
			return new LocalizedString("MultipleDefaultMailboxPlansFound", "Ex6CCBC5", false, true, Strings.ResourceManager, new object[]
			{
				id,
				dc
			});
		}

		public static LocalizedString ElementMustNotHaveAttributes(string element)
		{
			return new LocalizedString("ElementMustNotHaveAttributes", "ExB007D2", false, true, Strings.ResourceManager, new object[]
			{
				element
			});
		}

		public static LocalizedString ErrorOperationRequiresManager
		{
			get
			{
				return new LocalizedString("ErrorOperationRequiresManager", "ExB6FAB5", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString WarningTaskModuleSkipped(string methodName, string exception)
		{
			return new LocalizedString("WarningTaskModuleSkipped", "", false, false, Strings.ResourceManager, new object[]
			{
				methodName,
				exception
			});
		}

		public static LocalizedString ErrorIsOutofDatabaseScope(string id, string exceptionDetails)
		{
			return new LocalizedString("ErrorIsOutofDatabaseScope", "ExBD293F", false, true, Strings.ResourceManager, new object[]
			{
				id,
				exceptionDetails
			});
		}

		public static LocalizedString ExceptionInvalidTaskType(Type taskType)
		{
			return new LocalizedString("ExceptionInvalidTaskType", "Ex190A7A", false, true, Strings.ResourceManager, new object[]
			{
				taskType
			});
		}

		public static LocalizedString WorkUnitCollectionStatus(int totalCount, int completedCount, int failedCount)
		{
			return new LocalizedString("WorkUnitCollectionStatus", "Ex23E40F", false, true, Strings.ResourceManager, new object[]
			{
				totalCount,
				completedCount,
				failedCount
			});
		}

		public static LocalizedString ErrorOrganizationWildcard
		{
			get
			{
				return new LocalizedString("ErrorOrganizationWildcard", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorAddressListNotUnique(string idStringValue)
		{
			return new LocalizedString("ErrorAddressListNotUnique", "ExA6530D", false, true, Strings.ResourceManager, new object[]
			{
				idStringValue
			});
		}

		public static LocalizedString VerboseTaskParameterLoggingFailed(string param, Exception e)
		{
			return new LocalizedString("VerboseTaskParameterLoggingFailed", "Ex6F5716", false, true, Strings.ResourceManager, new object[]
			{
				param,
				e
			});
		}

		public static LocalizedString ErrorConversionFailed(string name)
		{
			return new LocalizedString("ErrorConversionFailed", "Ex9E1222", false, true, Strings.ResourceManager, new object[]
			{
				name
			});
		}

		public static LocalizedString ErrorRecipientNotUnique(string idStringValue)
		{
			return new LocalizedString("ErrorRecipientNotUnique", "Ex55FBCD", false, true, Strings.ResourceManager, new object[]
			{
				idStringValue
			});
		}

		public static LocalizedString ExceptionInvalidConfigObjectType(Type type)
		{
			return new LocalizedString("ExceptionInvalidConfigObjectType", "Ex90DF2B", false, true, Strings.ResourceManager, new object[]
			{
				type
			});
		}

		public static LocalizedString LogAutoResolving(Type taskType)
		{
			return new LocalizedString("LogAutoResolving", "Ex5E25A9", false, true, Strings.ResourceManager, new object[]
			{
				taskType
			});
		}

		public static LocalizedString ErrorDelegatedUserNotInOrg
		{
			get
			{
				return new LocalizedString("ErrorDelegatedUserNotInOrg", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString VerboseSerializationDataNotExist
		{
			get
			{
				return new LocalizedString("VerboseSerializationDataNotExist", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString VerboseInternalQueryFilterInGetTasks(string filter)
		{
			return new LocalizedString("VerboseInternalQueryFilterInGetTasks", "", false, false, Strings.ResourceManager, new object[]
			{
				filter
			});
		}

		public static LocalizedString PswsCmdletError(string errorMessage)
		{
			return new LocalizedString("PswsCmdletError", "", false, false, Strings.ResourceManager, new object[]
			{
				errorMessage
			});
		}

		public static LocalizedString LogCheckpoint(object checkPoint)
		{
			return new LocalizedString("LogCheckpoint", "Ex359016", false, true, Strings.ResourceManager, new object[]
			{
				checkPoint
			});
		}

		public static LocalizedString ErrorOrganizationNotFound(string idStringValue)
		{
			return new LocalizedString("ErrorOrganizationNotFound", "Ex48C448", false, true, Strings.ResourceManager, new object[]
			{
				idStringValue
			});
		}

		public static LocalizedString ErrorGlobalAddressListNotFound(string idStringValue)
		{
			return new LocalizedString("ErrorGlobalAddressListNotFound", "Ex8DD552", false, true, Strings.ResourceManager, new object[]
			{
				idStringValue
			});
		}

		public static LocalizedString DependentArguments(string arg1, string arg2)
		{
			return new LocalizedString("DependentArguments", "Ex70684E", false, true, Strings.ResourceManager, new object[]
			{
				arg1,
				arg2
			});
		}

		public static LocalizedString ErrorParentNotFound(string identity, string parent)
		{
			return new LocalizedString("ErrorParentNotFound", "Ex574388", false, true, Strings.ResourceManager, new object[]
			{
				identity,
				parent
			});
		}

		public static LocalizedString ErrorNoAvailablePublicFolderDatabase
		{
			get
			{
				return new LocalizedString("ErrorNoAvailablePublicFolderDatabase", "Ex7469AC", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorNotFoundWithReason(string notFound, string reason)
		{
			return new LocalizedString("ErrorNotFoundWithReason", "Ex04C125", false, true, Strings.ResourceManager, new object[]
			{
				notFound,
				reason
			});
		}

		public static LocalizedString WrongTypeUserContactGroupIdParameter(string identity)
		{
			return new LocalizedString("WrongTypeUserContactGroupIdParameter", "ExC8FAF9", false, true, Strings.ResourceManager, new object[]
			{
				identity
			});
		}

		public static LocalizedString SessionExpiredException
		{
			get
			{
				return new LocalizedString("SessionExpiredException", "Ex96CE50", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString VerboseAdminSessionSettingsGlobalCatalog(string globalCatalog)
		{
			return new LocalizedString("VerboseAdminSessionSettingsGlobalCatalog", "Ex6BC584", false, true, Strings.ResourceManager, new object[]
			{
				globalCatalog
			});
		}

		public static LocalizedString ExceptionParameterRange(string invalidQuery, int position)
		{
			return new LocalizedString("ExceptionParameterRange", "Ex9F8581", false, true, Strings.ResourceManager, new object[]
			{
				invalidQuery,
				position
			});
		}

		public static LocalizedString ErrorUnsupportedValues(string unsupported, string allowed)
		{
			return new LocalizedString("ErrorUnsupportedValues", "Ex63FE45", false, true, Strings.ResourceManager, new object[]
			{
				unsupported,
				allowed
			});
		}

		public static LocalizedString HierarchicalIdentityStartsOrEndsWithBackslash
		{
			get
			{
				return new LocalizedString("HierarchicalIdentityStartsOrEndsWithBackslash", "Ex733D8B", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString LogServiceState(ServiceControllerStatus status)
		{
			return new LocalizedString("LogServiceState", "ExFD2C6C", false, true, Strings.ResourceManager, new object[]
			{
				status
			});
		}

		public static LocalizedString ExceptionMDAInvalidConnectionString(string connectionString)
		{
			return new LocalizedString("ExceptionMDAInvalidConnectionString", "ExD98FF9", false, true, Strings.ResourceManager, new object[]
			{
				connectionString
			});
		}

		public static LocalizedString ErrorParentNotFoundOnDomainController(string identity, string domainController, string parent, string domain)
		{
			return new LocalizedString("ErrorParentNotFoundOnDomainController", "Ex80484B", false, true, Strings.ResourceManager, new object[]
			{
				identity,
				domainController,
				parent,
				domain
			});
		}

		public static LocalizedString ClassFactoryDoesNotImplementIProvisioningAgent(string classFactoryName, string assembly)
		{
			return new LocalizedString("ClassFactoryDoesNotImplementIProvisioningAgent", "Ex1631A5", false, true, Strings.ResourceManager, new object[]
			{
				classFactoryName,
				assembly
			});
		}

		public static LocalizedString VerboseInitializeRunspaceServerSettingsLocal
		{
			get
			{
				return new LocalizedString("VerboseInitializeRunspaceServerSettingsLocal", "Ex323EEB", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorInvalidOrganizationalUnitDNFormat(string input)
		{
			return new LocalizedString("ErrorInvalidOrganizationalUnitDNFormat", "ExF12D53", false, true, Strings.ResourceManager, new object[]
			{
				input
			});
		}

		public static LocalizedString VerboseDatabaseNotFound(string databaseId, string message)
		{
			return new LocalizedString("VerboseDatabaseNotFound", "Ex068D33", false, true, Strings.ResourceManager, new object[]
			{
				databaseId,
				message
			});
		}

		public static LocalizedString VerboseRecipientTaskHelperGetOrgnization(string ou)
		{
			return new LocalizedString("VerboseRecipientTaskHelperGetOrgnization", "ExA76E2B", false, true, Strings.ResourceManager, new object[]
			{
				ou
			});
		}

		public static LocalizedString NoRoleEntriesWithParameterFound(string exchangeCmdletName, string parameterName)
		{
			return new LocalizedString("NoRoleEntriesWithParameterFound", "ExB880D8", false, true, Strings.ResourceManager, new object[]
			{
				exchangeCmdletName,
				parameterName
			});
		}

		public static LocalizedString ErrorEmptyParameter(string parameterName)
		{
			return new LocalizedString("ErrorEmptyParameter", "ExA08212", false, true, Strings.ResourceManager, new object[]
			{
				parameterName
			});
		}

		public static LocalizedString ExceptionMDACommandStillExecuting
		{
			get
			{
				return new LocalizedString("ExceptionMDACommandStillExecuting", "Ex0AF4A8", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorManagementObjectAmbiguous(string id)
		{
			return new LocalizedString("ErrorManagementObjectAmbiguous", "Ex9E65A2", false, true, Strings.ResourceManager, new object[]
			{
				id
			});
		}

		public static LocalizedString MicroDelayInfo(int actualDelayed, bool enforced, int cappedDelay, bool required, string additionalInfo)
		{
			return new LocalizedString("MicroDelayInfo", "", false, false, Strings.ResourceManager, new object[]
			{
				actualDelayed,
				enforced,
				cappedDelay,
				required,
				additionalInfo
			});
		}

		public static LocalizedString WorkUnitError
		{
			get
			{
				return new LocalizedString("WorkUnitError", "Ex9FC98D", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString WrongTypeRemoteMailbox(string identity)
		{
			return new LocalizedString("WrongTypeRemoteMailbox", "ExA34B5E", false, true, Strings.ResourceManager, new object[]
			{
				identity
			});
		}

		public static LocalizedString WrongTypeRoleGroup(string identity)
		{
			return new LocalizedString("WrongTypeRoleGroup", "Ex9E8A2B", false, true, Strings.ResourceManager, new object[]
			{
				identity
			});
		}

		public static LocalizedString LogServiceName(string serviceName)
		{
			return new LocalizedString("LogServiceName", "Ex74EF2A", false, true, Strings.ResourceManager, new object[]
			{
				serviceName
			});
		}

		public static LocalizedString ErrorGlobalAddressListNotUnique(string idStringValue)
		{
			return new LocalizedString("ErrorGlobalAddressListNotUnique", "Ex3C3297", false, true, Strings.ResourceManager, new object[]
			{
				idStringValue
			});
		}

		public static LocalizedString ErrorNoServerForPublicFolderDatabase(string databaseGuid)
		{
			return new LocalizedString("ErrorNoServerForPublicFolderDatabase", "", false, false, Strings.ResourceManager, new object[]
			{
				databaseGuid
			});
		}

		public static LocalizedString VerboseADObjectChangedProperties(string propertyList)
		{
			return new LocalizedString("VerboseADObjectChangedProperties", "Ex2C227C", false, true, Strings.ResourceManager, new object[]
			{
				propertyList
			});
		}

		public static LocalizedString PswsResponseIsnotXMLError(string response)
		{
			return new LocalizedString("PswsResponseIsnotXMLError", "", false, false, Strings.ResourceManager, new object[]
			{
				response
			});
		}

		public static LocalizedString CheckIfUserIsASID(string user)
		{
			return new LocalizedString("CheckIfUserIsASID", "Ex5DC055", false, true, Strings.ResourceManager, new object[]
			{
				user
			});
		}

		public static LocalizedString ExchangeSetupCannotResumeLog(string logFile)
		{
			return new LocalizedString("ExchangeSetupCannotResumeLog", "Ex434130", false, true, Strings.ResourceManager, new object[]
			{
				logFile
			});
		}

		public static LocalizedString VerboseNoSource
		{
			get
			{
				return new LocalizedString("VerboseNoSource", "ExAD3D60", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString VerboseAdminSessionSettingsAFConfigDC(string configDC)
		{
			return new LocalizedString("VerboseAdminSessionSettingsAFConfigDC", "", false, false, Strings.ResourceManager, new object[]
			{
				configDC
			});
		}

		public static LocalizedString VerboseTaskBeginProcessing(string name)
		{
			return new LocalizedString("VerboseTaskBeginProcessing", "Ex65EA52", false, true, Strings.ResourceManager, new object[]
			{
				name
			});
		}

		public static LocalizedString ErrorFilteringOnlyUserLogin
		{
			get
			{
				return new LocalizedString("ErrorFilteringOnlyUserLogin", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString CouldNotStopService(string servicename)
		{
			return new LocalizedString("CouldNotStopService", "Ex1BCA4F", false, true, Strings.ResourceManager, new object[]
			{
				servicename
			});
		}

		public static LocalizedString ExceptionNullInstanceParameter
		{
			get
			{
				return new LocalizedString("ExceptionNullInstanceParameter", "Ex1B1F8D", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString LoadingLogonUserErrorText(string account)
		{
			return new LocalizedString("LoadingLogonUserErrorText", "ExF2B709", false, true, Strings.ResourceManager, new object[]
			{
				account
			});
		}

		public static LocalizedString CrossForestAccount(string identity)
		{
			return new LocalizedString("CrossForestAccount", "ExF55A9F", false, true, Strings.ResourceManager, new object[]
			{
				identity
			});
		}

		public static LocalizedString VerboseLbCreateNewExRpcAdmin
		{
			get
			{
				return new LocalizedString("VerboseLbCreateNewExRpcAdmin", "Ex7BF6A5", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString WrongTypeDistributionGroup(string identity)
		{
			return new LocalizedString("WrongTypeDistributionGroup", "Ex856B6B", false, true, Strings.ResourceManager, new object[]
			{
				identity
			});
		}

		public static LocalizedString WrongTypeMailUser(string identity)
		{
			return new LocalizedString("WrongTypeMailUser", "ExC76758", false, true, Strings.ResourceManager, new object[]
			{
				identity
			});
		}

		public static LocalizedString ErrorRecipientNotInSameOrgWithDataObject(string doId, string doOrg, string rpId, string rpOrg)
		{
			return new LocalizedString("ErrorRecipientNotInSameOrgWithDataObject", "Ex1B4E93", false, true, Strings.ResourceManager, new object[]
			{
				doId,
				doOrg,
				rpId,
				rpOrg
			});
		}

		public static LocalizedString ErrorMustWriteToRidMaster(string dc)
		{
			return new LocalizedString("ErrorMustWriteToRidMaster", "", false, false, Strings.ResourceManager, new object[]
			{
				dc
			});
		}

		public static LocalizedString ServiceUninstallFailure(string servicename, string error)
		{
			return new LocalizedString("ServiceUninstallFailure", "ExA6A247", false, true, Strings.ResourceManager, new object[]
			{
				servicename,
				error
			});
		}

		public static LocalizedString ExceptionResolverConstructorMissing(Type taskType)
		{
			return new LocalizedString("ExceptionResolverConstructorMissing", "Ex3AC6F4", false, true, Strings.ResourceManager, new object[]
			{
				taskType
			});
		}

		public static LocalizedString ErrorCannotDiscoverDefaultOrganizationUnitForRecipient
		{
			get
			{
				return new LocalizedString("ErrorCannotDiscoverDefaultOrganizationUnitForRecipient", "Ex2685E5", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString VerboseAdminSessionSettingsAFGlobalCatalog(string globalCatalog)
		{
			return new LocalizedString("VerboseAdminSessionSettingsAFGlobalCatalog", "", false, false, Strings.ResourceManager, new object[]
			{
				globalCatalog
			});
		}

		public static LocalizedString CannotResolveParentOrganization(string ou)
		{
			return new LocalizedString("CannotResolveParentOrganization", "Ex91333F", false, true, Strings.ResourceManager, new object[]
			{
				ou
			});
		}

		public static LocalizedString WrongTypeNonMailEnabledUser(string identity)
		{
			return new LocalizedString("WrongTypeNonMailEnabledUser", "Ex470030", false, true, Strings.ResourceManager, new object[]
			{
				identity
			});
		}

		public static LocalizedString WarningDuplicateOrganizationSpecified(string organizationOrg, string identityOrg)
		{
			return new LocalizedString("WarningDuplicateOrganizationSpecified", "Ex79DD00", false, true, Strings.ResourceManager, new object[]
			{
				organizationOrg,
				identityOrg
			});
		}

		public static LocalizedString ResourceLoadDelayInfo(int actualDelayed, bool enforced, int cappedDelay, bool required, string resource, string load, string additionalInfo)
		{
			return new LocalizedString("ResourceLoadDelayInfo", "", false, false, Strings.ResourceManager, new object[]
			{
				actualDelayed,
				enforced,
				cappedDelay,
				required,
				resource,
				load,
				additionalInfo
			});
		}

		public static LocalizedString VerboseLbBestServerSoFar(string serverDn, int num)
		{
			return new LocalizedString("VerboseLbBestServerSoFar", "Ex1ABE89", false, true, Strings.ResourceManager, new object[]
			{
				serverDn,
				num
			});
		}

		public static LocalizedString ErrorIsOutOfDatabaseScopeNoServerCheck(string id, string exceptionDetails)
		{
			return new LocalizedString("ErrorIsOutOfDatabaseScopeNoServerCheck", "Ex9B0142", false, true, Strings.ResourceManager, new object[]
			{
				id,
				exceptionDetails
			});
		}

		public static LocalizedString ErrorIsAcceptedDomain(string domain)
		{
			return new LocalizedString("ErrorIsAcceptedDomain", "Ex4A146E", false, true, Strings.ResourceManager, new object[]
			{
				domain
			});
		}

		public static LocalizedString ValidationRuleNotFound(string ruleName)
		{
			return new LocalizedString("ValidationRuleNotFound", "Ex8D937E", false, true, Strings.ResourceManager, new object[]
			{
				ruleName
			});
		}

		public static LocalizedString ErrorInvalidUMHuntGroupIdentity(string idValue)
		{
			return new LocalizedString("ErrorInvalidUMHuntGroupIdentity", "ExE7C655", false, true, Strings.ResourceManager, new object[]
			{
				idValue
			});
		}

		public static LocalizedString ErrorNoServersForDatabase(string id)
		{
			return new LocalizedString("ErrorNoServersForDatabase", "Ex3A075D", false, true, Strings.ResourceManager, new object[]
			{
				id
			});
		}

		public static LocalizedString CommandSucceeded
		{
			get
			{
				return new LocalizedString("CommandSucceeded", "Ex963939", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ProvisioningValidationErrors(int handlerIndex, string errors)
		{
			return new LocalizedString("ProvisioningValidationErrors", "Ex18E25A", false, true, Strings.ResourceManager, new object[]
			{
				handlerIndex,
				errors
			});
		}

		public static LocalizedString ResubmitRequestDoesNotExist(long requestId)
		{
			return new LocalizedString("ResubmitRequestDoesNotExist", "", false, false, Strings.ResourceManager, new object[]
			{
				requestId
			});
		}

		public static LocalizedString UserQuotaDelayInfo(int actualDelayed, bool enforced, int cappedDelay, bool required, string part, string additionalInfo)
		{
			return new LocalizedString("UserQuotaDelayInfo", "", false, false, Strings.ResourceManager, new object[]
			{
				actualDelayed,
				enforced,
				cappedDelay,
				required,
				part,
				additionalInfo
			});
		}

		public static LocalizedString ErrorTaskWin32ExceptionVerbose(string error, string verbose)
		{
			return new LocalizedString("ErrorTaskWin32ExceptionVerbose", "Ex56C93E", false, true, Strings.ResourceManager, new object[]
			{
				error,
				verbose
			});
		}

		public static LocalizedString ErrorUninitializedParameter
		{
			get
			{
				return new LocalizedString("ErrorUninitializedParameter", "Ex82F788", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorSetServiceObjectSecurity(string name)
		{
			return new LocalizedString("ErrorSetServiceObjectSecurity", "Ex7AFF77", false, true, Strings.ResourceManager, new object[]
			{
				name
			});
		}

		public static LocalizedString ErrorQueryServiceObjectSecurity(string name)
		{
			return new LocalizedString("ErrorQueryServiceObjectSecurity", "Ex6F168C", false, true, Strings.ResourceManager, new object[]
			{
				name
			});
		}

		public static LocalizedString ErrorNotAllowedForPartnerAccess
		{
			get
			{
				return new LocalizedString("ErrorNotAllowedForPartnerAccess", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorNoAvailablePublicFolderDatabaseOnServer(string server)
		{
			return new LocalizedString("ErrorNoAvailablePublicFolderDatabaseOnServer", "Ex1C5F52", false, true, Strings.ResourceManager, new object[]
			{
				server
			});
		}

		public static LocalizedString LogConditionMatchingTypeMismacth(Type conditionType1, Type conditionType2)
		{
			return new LocalizedString("LogConditionMatchingTypeMismacth", "ExC4FFC5", false, true, Strings.ResourceManager, new object[]
			{
				conditionType1,
				conditionType2
			});
		}

		public static LocalizedString ExceptionTaskNotExecuted
		{
			get
			{
				return new LocalizedString("ExceptionTaskNotExecuted", "ExE6D900", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ConfigObjectNotFound(string identity, Type classType)
		{
			return new LocalizedString("ConfigObjectNotFound", "Ex37E8E1", false, true, Strings.ResourceManager, new object[]
			{
				identity,
				classType
			});
		}

		public static LocalizedString ExceptionLexError(string invalidQuery, int position)
		{
			return new LocalizedString("ExceptionLexError", "Ex5432C4", false, true, Strings.ResourceManager, new object[]
			{
				invalidQuery,
				position
			});
		}

		public static LocalizedString VerboseADObjectNoChangedPropertiesWithId(string identity)
		{
			return new LocalizedString("VerboseADObjectNoChangedPropertiesWithId", "ExFE1249", false, true, Strings.ResourceManager, new object[]
			{
				identity
			});
		}

		public static LocalizedString VerboseLbCheckingDatabaseIsAllowedOnScope(string databaseDn)
		{
			return new LocalizedString("VerboseLbCheckingDatabaseIsAllowedOnScope", "Ex84B091", false, true, Strings.ResourceManager, new object[]
			{
				databaseDn
			});
		}

		public static LocalizedString InvalidElementValue(string value, string element)
		{
			return new LocalizedString("InvalidElementValue", "Ex655322", false, true, Strings.ResourceManager, new object[]
			{
				value,
				element
			});
		}

		public static LocalizedString ErrorRoleAssignmentNotFound(string str)
		{
			return new LocalizedString("ErrorRoleAssignmentNotFound", "Ex6FF57D", false, true, Strings.ResourceManager, new object[]
			{
				str
			});
		}

		public static LocalizedString ErrorInvalidResultSize
		{
			get
			{
				return new LocalizedString("ErrorInvalidResultSize", "Ex6FEEE0", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString VerboseLbDeletedServer
		{
			get
			{
				return new LocalizedString("VerboseLbDeletedServer", "Ex58EC74", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString WrongTypeMailboxPlan(string identity)
		{
			return new LocalizedString("WrongTypeMailboxPlan", "Ex4AD36D", false, true, Strings.ResourceManager, new object[]
			{
				identity
			});
		}

		public static LocalizedString TaskCompleted
		{
			get
			{
				return new LocalizedString("TaskCompleted", "ExDA9525", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString WarningCmdletTarpittingByUserQuota(string policy, string delaySeconds, string computerName)
		{
			return new LocalizedString("WarningCmdletTarpittingByUserQuota", "", false, false, Strings.ResourceManager, new object[]
			{
				policy,
				delaySeconds,
				computerName
			});
		}

		public static LocalizedString ErrorMaxTenantPSConnectionLimitNotResolved
		{
			get
			{
				return new LocalizedString("ErrorMaxTenantPSConnectionLimitNotResolved", "Ex703EDE", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InvalidRBACContextType(string configType)
		{
			return new LocalizedString("InvalidRBACContextType", "Ex62E035", false, true, Strings.ResourceManager, new object[]
			{
				configType
			});
		}

		public static LocalizedString InvalidCharacterInComponentPartOfHierarchicalIdentity
		{
			get
			{
				return new LocalizedString("InvalidCharacterInComponentPartOfHierarchicalIdentity", "Ex2F63E0", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString VerboseLbOabVDirSelected(string vdirDn)
		{
			return new LocalizedString("VerboseLbOabVDirSelected", "ExE4D73B", false, true, Strings.ResourceManager, new object[]
			{
				vdirDn
			});
		}

		public static LocalizedString MonitoringEventStringWithInstanceName(string eventSource, int eventId, string eventType, string eventMessage, string eventInstanceName)
		{
			return new LocalizedString("MonitoringEventStringWithInstanceName", "Ex4D0114", false, true, Strings.ResourceManager, new object[]
			{
				eventSource,
				eventId,
				eventType,
				eventMessage,
				eventInstanceName
			});
		}

		public static LocalizedString NoRoleAssignmentsFound(string identity)
		{
			return new LocalizedString("NoRoleAssignmentsFound", "Ex34303A", false, true, Strings.ResourceManager, new object[]
			{
				identity
			});
		}

		public static LocalizedString VerboseSaveChange(string id, string type, string state)
		{
			return new LocalizedString("VerboseSaveChange", "ExCF5282", false, true, Strings.ResourceManager, new object[]
			{
				id,
				type,
				state
			});
		}

		public static LocalizedString WrongActiveSyncDeviceIdParameter(string identity)
		{
			return new LocalizedString("WrongActiveSyncDeviceIdParameter", "Ex0FBD0C", false, true, Strings.ResourceManager, new object[]
			{
				identity
			});
		}

		public static LocalizedString ExceptionReadOnlyPropertyBag
		{
			get
			{
				return new LocalizedString("ExceptionReadOnlyPropertyBag", "Ex4ED62A", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString VerboseLbTryRetrieveDatabaseStatus
		{
			get
			{
				return new LocalizedString("VerboseLbTryRetrieveDatabaseStatus", "ExDDF00D", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorDuplicateManagementObjectFound(IIdentityParameter id1, IIdentityParameter id2, object entry)
		{
			return new LocalizedString("ErrorDuplicateManagementObjectFound", "Ex199CCD", false, true, Strings.ResourceManager, new object[]
			{
				id1,
				id2,
				entry
			});
		}

		public static LocalizedString VerboseLbDatabaseNotFoundException(string errorMessage)
		{
			return new LocalizedString("VerboseLbDatabaseNotFoundException", "Ex329BD6", false, true, Strings.ResourceManager, new object[]
			{
				errorMessage
			});
		}

		public static LocalizedString ErrorIgnoreDefaultScopeAndDCSetTogether
		{
			get
			{
				return new LocalizedString("ErrorIgnoreDefaultScopeAndDCSetTogether", "Ex86A9F7", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ExceptionGettingConditionObject
		{
			get
			{
				return new LocalizedString("ExceptionGettingConditionObject", "Ex467FFB", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString VerboseLbGetServerForActiveDatabaseCopy(string name)
		{
			return new LocalizedString("VerboseLbGetServerForActiveDatabaseCopy", "", false, false, Strings.ResourceManager, new object[]
			{
				name
			});
		}

		public static LocalizedString ErrorOpenKeyDeniedForWrite(string keyPath)
		{
			return new LocalizedString("ErrorOpenKeyDeniedForWrite", "ExC1627A", false, true, Strings.ResourceManager, new object[]
			{
				keyPath
			});
		}

		public static LocalizedString WarningCmdletMicroDelay(string delayMSecs)
		{
			return new LocalizedString("WarningCmdletMicroDelay", "", false, false, Strings.ResourceManager, new object[]
			{
				delayMSecs
			});
		}

		public static LocalizedString ErrorManagementObjectNotFoundWithSourceByType(string type, string source)
		{
			return new LocalizedString("ErrorManagementObjectNotFoundWithSourceByType", "Ex727424", false, true, Strings.ResourceManager, new object[]
			{
				type,
				source
			});
		}

		public static LocalizedString ErrorInvalidParameterType(string parameterName, string parameterType)
		{
			return new LocalizedString("ErrorInvalidParameterType", "ExEDDBFE", false, true, Strings.ResourceManager, new object[]
			{
				parameterName,
				parameterType
			});
		}

		public static LocalizedString ExceptionCondition(LocalizedString failureDescription, Condition faultingCondition)
		{
			return new LocalizedString("ExceptionCondition", "ExC76D39", false, true, Strings.ResourceManager, new object[]
			{
				failureDescription,
				faultingCondition
			});
		}

		public static LocalizedString EnabledString
		{
			get
			{
				return new LocalizedString("EnabledString", "Ex667CAC", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ElapsedTimeDescription(int h, int mm, int ss)
		{
			return new LocalizedString("ElapsedTimeDescription", "ExB3E8C1", false, true, Strings.ResourceManager, new object[]
			{
				h,
				mm,
				ss
			});
		}

		public static LocalizedString ExceptionParseError(string invalidQuery, int position)
		{
			return new LocalizedString("ExceptionParseError", "Ex5420B7", false, true, Strings.ResourceManager, new object[]
			{
				invalidQuery,
				position
			});
		}

		public static LocalizedString VerboseLbGeneralTrace(string message)
		{
			return new LocalizedString("VerboseLbGeneralTrace", "", false, false, Strings.ResourceManager, new object[]
			{
				message
			});
		}

		public static LocalizedString ErrorPublicFolderDatabaseIsNotMounted(string database)
		{
			return new LocalizedString("ErrorPublicFolderDatabaseIsNotMounted", "ExE09C4F", false, true, Strings.ResourceManager, new object[]
			{
				database
			});
		}

		public static LocalizedString WarningForceMessageWithId(string identity)
		{
			return new LocalizedString("WarningForceMessageWithId", "Ex2E55C2", false, true, Strings.ResourceManager, new object[]
			{
				identity
			});
		}

		public static LocalizedString WorkUnitWarning
		{
			get
			{
				return new LocalizedString("WorkUnitWarning", "Ex6342E6", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorPublicFolderGeneratingProxy(string identity)
		{
			return new LocalizedString("ErrorPublicFolderGeneratingProxy", "ExE90F4A", false, true, Strings.ResourceManager, new object[]
			{
				identity
			});
		}

		public static LocalizedString ErrorMaxRunspacesLimit(string maxConnection, string policyPart)
		{
			return new LocalizedString("ErrorMaxRunspacesLimit", "", false, false, Strings.ResourceManager, new object[]
			{
				maxConnection,
				policyPart
			});
		}

		public static LocalizedString VerboseLbServerDownSoMarkDatabaseDown
		{
			get
			{
				return new LocalizedString("VerboseLbServerDownSoMarkDatabaseDown", "Ex6B0DE4", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString LogConditionMatchingPropertyMismatch(Type conditionType, string propertyName, object yourValue, object theirValue)
		{
			return new LocalizedString("LogConditionMatchingPropertyMismatch", "Ex96B080", false, true, Strings.ResourceManager, new object[]
			{
				conditionType,
				propertyName,
				yourValue,
				theirValue
			});
		}

		public static LocalizedString HandlerThronwExceptionInOnComplete(int i, string exception)
		{
			return new LocalizedString("HandlerThronwExceptionInOnComplete", "Ex7BABB4", false, true, Strings.ResourceManager, new object[]
			{
				i,
				exception
			});
		}

		public static LocalizedString InvalidGuidParameter(string parameterValue)
		{
			return new LocalizedString("InvalidGuidParameter", "", false, false, Strings.ResourceManager, new object[]
			{
				parameterValue
			});
		}

		public static LocalizedString LogTaskExecutionPlan(int taskIndex, Task task)
		{
			return new LocalizedString("LogTaskExecutionPlan", "Ex63378F", false, true, Strings.ResourceManager, new object[]
			{
				taskIndex,
				task
			});
		}

		public static LocalizedString WarningCannotGetLocalServerFqdn(string msg)
		{
			return new LocalizedString("WarningCannotGetLocalServerFqdn", "", false, false, Strings.ResourceManager, new object[]
			{
				msg
			});
		}

		public static LocalizedString BinaryDataStakeHodler
		{
			get
			{
				return new LocalizedString("BinaryDataStakeHodler", "Ex516277", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ExArgumentOutOfRangeException(string paramName, object actualValue)
		{
			return new LocalizedString("ExArgumentOutOfRangeException", "Ex5A5127", false, true, Strings.ResourceManager, new object[]
			{
				paramName,
				actualValue
			});
		}

		public static LocalizedString ErrorRemoveNewerObject(string identity, string version)
		{
			return new LocalizedString("ErrorRemoveNewerObject", "ExE88A71", false, true, Strings.ResourceManager, new object[]
			{
				identity,
				version
			});
		}

		public static LocalizedString DelegatedAdminAccount(string identity)
		{
			return new LocalizedString("DelegatedAdminAccount", "Ex9A9C5A", false, true, Strings.ResourceManager, new object[]
			{
				identity
			});
		}

		public static LocalizedString ErrorWriteOpOnDehydratedTenant
		{
			get
			{
				return new LocalizedString("ErrorWriteOpOnDehydratedTenant", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorInvalidHierarchicalIdentity(string identity, string reason)
		{
			return new LocalizedString("ErrorInvalidHierarchicalIdentity", "ExC635FE", false, true, Strings.ResourceManager, new object[]
			{
				identity,
				reason
			});
		}

		public static LocalizedString ExceptionRollbackFailed(Type taskType, Exception rollbackException)
		{
			return new LocalizedString("ExceptionRollbackFailed", "Ex749D4B", false, true, Strings.ResourceManager, new object[]
			{
				taskType,
				rollbackException
			});
		}

		public static LocalizedString ExceptionMDACommandAlreadyExecuting
		{
			get
			{
				return new LocalizedString("ExceptionMDACommandAlreadyExecuting", "Ex1848E9", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString VerboseTaskEndProcessing(string name)
		{
			return new LocalizedString("VerboseTaskEndProcessing", "ExD6F385", false, true, Strings.ResourceManager, new object[]
			{
				name
			});
		}

		public static LocalizedString ErrorInvalidMailboxFolderIdentity(string input)
		{
			return new LocalizedString("ErrorInvalidMailboxFolderIdentity", "Ex8944E7", false, true, Strings.ResourceManager, new object[]
			{
				input
			});
		}

		public static LocalizedString ErrorFoundMultipleRootRole(string roleType)
		{
			return new LocalizedString("ErrorFoundMultipleRootRole", "", false, false, Strings.ResourceManager, new object[]
			{
				roleType
			});
		}

		public static LocalizedString ImpersonationNotAllowed(string account, string impersonatedUser)
		{
			return new LocalizedString("ImpersonationNotAllowed", "Ex723AFD", false, true, Strings.ResourceManager, new object[]
			{
				account,
				impersonatedUser
			});
		}

		public static LocalizedString SipCultureInfoArgumentCheckFailure
		{
			get
			{
				return new LocalizedString("SipCultureInfoArgumentCheckFailure", "Ex33719F", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MonitoringEventString(string eventSource, int eventId, string eventType, string eventMessage)
		{
			return new LocalizedString("MonitoringEventString", "Ex89F6CA", false, true, Strings.ResourceManager, new object[]
			{
				eventSource,
				eventId,
				eventType,
				eventMessage
			});
		}

		public static LocalizedString ErrorFailedToReadFromDC(string id, string dc)
		{
			return new LocalizedString("ErrorFailedToReadFromDC", "Ex4215D1", false, true, Strings.ResourceManager, new object[]
			{
				id,
				dc
			});
		}

		public static LocalizedString ErrorTenantMaxRunspacesTarpitting(string orgName, int delaySeconds)
		{
			return new LocalizedString("ErrorTenantMaxRunspacesTarpitting", "", false, false, Strings.ResourceManager, new object[]
			{
				orgName,
				delaySeconds
			});
		}

		public static LocalizedString ForeignForestTrustFailedException(string user)
		{
			return new LocalizedString("ForeignForestTrustFailedException", "Ex19B768", false, true, Strings.ResourceManager, new object[]
			{
				user
			});
		}

		public static LocalizedString WrongTypeUserContact(string identity)
		{
			return new LocalizedString("WrongTypeUserContact", "Ex086491", false, true, Strings.ResourceManager, new object[]
			{
				identity
			});
		}

		public static LocalizedString ProvisioningUpdateAffectedObject(int handlerIndex, string detailes)
		{
			return new LocalizedString("ProvisioningUpdateAffectedObject", "ExD120E5", false, true, Strings.ResourceManager, new object[]
			{
				handlerIndex,
				detailes
			});
		}

		public static LocalizedString LookupUserAsDomainUser(string user)
		{
			return new LocalizedString("LookupUserAsDomainUser", "Ex81D909", false, true, Strings.ResourceManager, new object[]
			{
				user
			});
		}

		public static LocalizedString MissingAttribute(string attribute, string element)
		{
			return new LocalizedString("MissingAttribute", "Ex02D59E", false, true, Strings.ResourceManager, new object[]
			{
				attribute,
				element
			});
		}

		public static LocalizedString ErrorCannotFormatRecipient(int recipientType)
		{
			return new LocalizedString("ErrorCannotFormatRecipient", "Ex869E77", false, true, Strings.ResourceManager, new object[]
			{
				recipientType
			});
		}

		public static LocalizedString ConsecutiveWholeWildcardNamePartsInHierarchicalIdentity
		{
			get
			{
				return new LocalizedString("ConsecutiveWholeWildcardNamePartsInHierarchicalIdentity", "ExABED9B", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorCannotGetPublicFolderDatabaseByLegacyDn(string legacyDn)
		{
			return new LocalizedString("ErrorCannotGetPublicFolderDatabaseByLegacyDn", "ExF245BB", false, true, Strings.ResourceManager, new object[]
			{
				legacyDn
			});
		}

		public static LocalizedString ErrorMapiPublicFolderTreeNotUnique
		{
			get
			{
				return new LocalizedString("ErrorMapiPublicFolderTreeNotUnique", "ExCD8A8E", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString VerboseRemovedRoleAssignment(string id)
		{
			return new LocalizedString("VerboseRemovedRoleAssignment", "Ex65326E", false, true, Strings.ResourceManager, new object[]
			{
				id
			});
		}

		public static LocalizedString WarningMoreResultsAvailable
		{
			get
			{
				return new LocalizedString("WarningMoreResultsAvailable", "ExE002C0", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString LoadingLogonUser(string account)
		{
			return new LocalizedString("LoadingLogonUser", "Ex040F63", false, true, Strings.ResourceManager, new object[]
			{
				account
			});
		}

		public static LocalizedString ErrorObjectVersionChanged(string identity)
		{
			return new LocalizedString("ErrorObjectVersionChanged", "ExCA77A4", false, true, Strings.ResourceManager, new object[]
			{
				identity
			});
		}

		public static LocalizedString WrongTypeMailboxOrMailUserOrMailContact(string identity)
		{
			return new LocalizedString("WrongTypeMailboxOrMailUserOrMailContact", "ExC91979", false, true, Strings.ResourceManager, new object[]
			{
				identity
			});
		}

		public static LocalizedString ErrorOperationOnInvalidObject
		{
			get
			{
				return new LocalizedString("ErrorOperationOnInvalidObject", "Ex9D674A", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorTaskWin32Exception(string error)
		{
			return new LocalizedString("ErrorTaskWin32Exception", "ExF23ED4", false, true, Strings.ResourceManager, new object[]
			{
				error
			});
		}

		public static LocalizedString ServiceNotInstalled(string servicename)
		{
			return new LocalizedString("ServiceNotInstalled", "Ex216ACC", false, true, Strings.ResourceManager, new object[]
			{
				servicename
			});
		}

		public static LocalizedString PooledConnectionOpenTimeoutError(string msg)
		{
			return new LocalizedString("PooledConnectionOpenTimeoutError", "Ex9E338C", false, true, Strings.ResourceManager, new object[]
			{
				msg
			});
		}

		public static LocalizedString VerboseLbAddingEligibleServer(string mailboxServer)
		{
			return new LocalizedString("VerboseLbAddingEligibleServer", "Ex501298", false, true, Strings.ResourceManager, new object[]
			{
				mailboxServer
			});
		}

		public static LocalizedString VerboseInitializeRunspaceServerSettingsAdam
		{
			get
			{
				return new LocalizedString("VerboseInitializeRunspaceServerSettingsAdam", "Ex7E23DE", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorInvalidIdentity(string idValue)
		{
			return new LocalizedString("ErrorInvalidIdentity", "ExDDE39A", false, true, Strings.ResourceManager, new object[]
			{
				idValue
			});
		}

		public static LocalizedString VerboseLbNoAvailableDatabase
		{
			get
			{
				return new LocalizedString("VerboseLbNoAvailableDatabase", "Ex56A0A4", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString NonMigratedUserException(string identity)
		{
			return new LocalizedString("NonMigratedUserException", "Ex52DC13", false, true, Strings.ResourceManager, new object[]
			{
				identity
			});
		}

		public static LocalizedString InvalidPropertyName(string property)
		{
			return new LocalizedString("InvalidPropertyName", "Ex9E0328", false, true, Strings.ResourceManager, new object[]
			{
				property
			});
		}

		public static LocalizedString WrongTypeDynamicGroup(string identity)
		{
			return new LocalizedString("WrongTypeDynamicGroup", "Ex16CB89", false, true, Strings.ResourceManager, new object[]
			{
				identity
			});
		}

		public static LocalizedString PswsManagementAutomationAssemblyLoadError
		{
			get
			{
				return new LocalizedString("PswsManagementAutomationAssemblyLoadError", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString LoadingRoleErrorText(string account)
		{
			return new LocalizedString("LoadingRoleErrorText", "Ex637EA9", false, true, Strings.ResourceManager, new object[]
			{
				account
			});
		}

		public static LocalizedString LogRollbackFailed
		{
			get
			{
				return new LocalizedString("LogRollbackFailed", "Ex837B90", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString PowerShellTimeout(int timeout)
		{
			return new LocalizedString("PowerShellTimeout", "", false, false, Strings.ResourceManager, new object[]
			{
				timeout
			});
		}

		public static LocalizedString VerboseLbDatabaseAndServerTry(string databaseDn, string serverFqdn)
		{
			return new LocalizedString("VerboseLbDatabaseAndServerTry", "ExA64F9C", false, true, Strings.ResourceManager, new object[]
			{
				databaseDn,
				serverFqdn
			});
		}

		public static LocalizedString ErrorCannotResolveCertificate(string certName)
		{
			return new LocalizedString("ErrorCannotResolveCertificate", "", false, false, Strings.ResourceManager, new object[]
			{
				certName
			});
		}

		public static LocalizedString WrongTypeMailEnabledContact(string identity)
		{
			return new LocalizedString("WrongTypeMailEnabledContact", "Ex14B1F2", false, true, Strings.ResourceManager, new object[]
			{
				identity
			});
		}

		public static LocalizedString ExchangeSetupCannotCopyWatson(string logFile, string watsonFile)
		{
			return new LocalizedString("ExchangeSetupCannotCopyWatson", "Ex6E8702", false, true, Strings.ResourceManager, new object[]
			{
				logFile,
				watsonFile
			});
		}

		public static LocalizedString VerboseApplyRusPolicyForRecipient(string id, string homeDC)
		{
			return new LocalizedString("VerboseApplyRusPolicyForRecipient", "Ex3EAF98", false, true, Strings.ResourceManager, new object[]
			{
				id,
				homeDC
			});
		}

		public static LocalizedString NotInSameOrg(string logonIdentity, string impersonatedIdentity)
		{
			return new LocalizedString("NotInSameOrg", "Ex7DBC70", false, true, Strings.ResourceManager, new object[]
			{
				logonIdentity,
				impersonatedIdentity
			});
		}

		public static LocalizedString WrongTypeLogonableObjectIdParameter(string identity)
		{
			return new LocalizedString("WrongTypeLogonableObjectIdParameter", "ExEE7DA9", false, true, Strings.ResourceManager, new object[]
			{
				identity
			});
		}

		public static LocalizedString ClashingIdentities(string Assembly, string ClassFactory)
		{
			return new LocalizedString("ClashingIdentities", "ExE4FDE6", false, true, Strings.ResourceManager, new object[]
			{
				Assembly,
				ClassFactory
			});
		}

		public static LocalizedString ErrorMaxRunspacesTarpitting(int delaySeconds)
		{
			return new LocalizedString("ErrorMaxRunspacesTarpitting", "", false, false, Strings.ResourceManager, new object[]
			{
				delaySeconds
			});
		}

		public static LocalizedString VerboseExecutingUserContext(string executingUserId, string executingUserOrganizationId, string currentOrganizationId, string isRbacEnabled)
		{
			return new LocalizedString("VerboseExecutingUserContext", "Ex0E7FA3", false, true, Strings.ResourceManager, new object[]
			{
				executingUserId,
				executingUserOrganizationId,
				currentOrganizationId,
				isRbacEnabled
			});
		}

		public static LocalizedString ExceptionSetupFileNotFound(string fileName)
		{
			return new LocalizedString("ExceptionSetupFileNotFound", "Ex040A47", false, true, Strings.ResourceManager, new object[]
			{
				fileName
			});
		}

		public static LocalizedString VerboseFailedToGetProxyServer(int minServerVersion, string objectVersion)
		{
			return new LocalizedString("VerboseFailedToGetProxyServer", "", false, false, Strings.ResourceManager, new object[]
			{
				minServerVersion,
				objectVersion
			});
		}

		public static LocalizedString NullOrEmptyNamePartsInHierarchicalIdentity
		{
			get
			{
				return new LocalizedString("NullOrEmptyNamePartsInHierarchicalIdentity", "ExD28329", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorCloseServiceHandle
		{
			get
			{
				return new LocalizedString("ErrorCloseServiceHandle", "Ex9FC550", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ExceptionTaskContextConsistencyViolation(Type taskType)
		{
			return new LocalizedString("ExceptionTaskContextConsistencyViolation", "Ex9F100E", false, true, Strings.ResourceManager, new object[]
			{
				taskType
			});
		}

		public static LocalizedString LogPreconditionImmediate(Type conditionType)
		{
			return new LocalizedString("LogPreconditionImmediate", "ExEB4D08", false, true, Strings.ResourceManager, new object[]
			{
				conditionType
			});
		}

		public static LocalizedString ErrorInvalidType(string parameterType)
		{
			return new LocalizedString("ErrorInvalidType", "Ex4B61A0", false, true, Strings.ResourceManager, new object[]
			{
				parameterType
			});
		}

		public static LocalizedString WorkUnitStatusCompleted
		{
			get
			{
				return new LocalizedString("WorkUnitStatusCompleted", "Ex8BFED9", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString VerboseCannotGetRemoteServerForUser(string id, string proxyAddress, string reason)
		{
			return new LocalizedString("VerboseCannotGetRemoteServerForUser", "", false, false, Strings.ResourceManager, new object[]
			{
				id,
				proxyAddress,
				reason
			});
		}

		public static LocalizedString ErrorNoReplicaOnServer(string folder, string server)
		{
			return new LocalizedString("ErrorNoReplicaOnServer", "ExD77E9D", false, true, Strings.ResourceManager, new object[]
			{
				folder,
				server
			});
		}

		public static LocalizedString ErrorUserNotUnique(string idStringValue)
		{
			return new LocalizedString("ErrorUserNotUnique", "Ex0F741B", false, true, Strings.ResourceManager, new object[]
			{
				idStringValue
			});
		}

		public static LocalizedString ErrorMaxConnectionLimit(string vdirPath)
		{
			return new LocalizedString("ErrorMaxConnectionLimit", "ExED1E53", false, true, Strings.ResourceManager, new object[]
			{
				vdirPath
			});
		}

		public static LocalizedString ErrorUrlInValid
		{
			get
			{
				return new LocalizedString("ErrorUrlInValid", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString DuplicateExternalDirectoryObjectIdException(string objectName, string edoId)
		{
			return new LocalizedString("DuplicateExternalDirectoryObjectIdException", "", false, false, Strings.ResourceManager, new object[]
			{
				objectName,
				edoId
			});
		}

		public static LocalizedString InvalidElement(string element)
		{
			return new LocalizedString("InvalidElement", "Ex19A663", false, true, Strings.ResourceManager, new object[]
			{
				element
			});
		}

		public static LocalizedString VerbosePreConditions(string conditions)
		{
			return new LocalizedString("VerbosePreConditions", "Ex0A14CB", false, true, Strings.ResourceManager, new object[]
			{
				conditions
			});
		}

		public static LocalizedString ErrorNoMailboxUserInTheForest
		{
			get
			{
				return new LocalizedString("ErrorNoMailboxUserInTheForest", "Ex4DE0B7", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString WrongTypeGroup(string identity)
		{
			return new LocalizedString("WrongTypeGroup", "Ex62C8AC", false, true, Strings.ResourceManager, new object[]
			{
				identity
			});
		}

		public static LocalizedString ServerNotAvailable
		{
			get
			{
				return new LocalizedString("ServerNotAvailable", "Ex1A17BA", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString VerboseSelectedRusServer(string server)
		{
			return new LocalizedString("VerboseSelectedRusServer", "ExAC18CC", false, true, Strings.ResourceManager, new object[]
			{
				server
			});
		}

		public static LocalizedString VerboseLbDisposeExRpcAdmin(string serverFqdn)
		{
			return new LocalizedString("VerboseLbDisposeExRpcAdmin", "Ex4804CF", false, true, Strings.ResourceManager, new object[]
			{
				serverFqdn
			});
		}

		public static LocalizedString ErrorRelativeDn(string dn)
		{
			return new LocalizedString("ErrorRelativeDn", "ExFF31B9", false, true, Strings.ResourceManager, new object[]
			{
				dn
			});
		}

		public static LocalizedString HelpUrlHeaderText
		{
			get
			{
				return new LocalizedString("HelpUrlHeaderText", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InvalidAttributeValue(string value, string attribute)
		{
			return new LocalizedString("InvalidAttributeValue", "Ex4644BC", false, true, Strings.ResourceManager, new object[]
			{
				value,
				attribute
			});
		}

		public static LocalizedString PropertyIsAlreadyProvisioned(string propertyName, int i)
		{
			return new LocalizedString("PropertyIsAlreadyProvisioned", "Ex4F79CE", false, true, Strings.ResourceManager, new object[]
			{
				propertyName,
				i
			});
		}

		public static LocalizedString ErrorInvalidModerator(string moderator)
		{
			return new LocalizedString("ErrorInvalidModerator", "Ex33BBE9", false, true, Strings.ResourceManager, new object[]
			{
				moderator
			});
		}

		public static LocalizedString ErrorOrganizationalUnitNotUnique(string idStringValue)
		{
			return new LocalizedString("ErrorOrganizationalUnitNotUnique", "Ex52FDB2", false, true, Strings.ResourceManager, new object[]
			{
				idStringValue
			});
		}

		public static LocalizedString ExceptionMissingCreateInstance(Type type, string codeBase)
		{
			return new LocalizedString("ExceptionMissingCreateInstance", "Ex23D891", false, true, Strings.ResourceManager, new object[]
			{
				type,
				codeBase
			});
		}

		public static LocalizedString ExecutingUserPropertyNotFound(string propertyName)
		{
			return new LocalizedString("ExecutingUserPropertyNotFound", "Ex6B9416", false, true, Strings.ResourceManager, new object[]
			{
				propertyName
			});
		}

		public static LocalizedString CannotHaveLocalAccountException(string user)
		{
			return new LocalizedString("CannotHaveLocalAccountException", "ExE7A580", false, true, Strings.ResourceManager, new object[]
			{
				user
			});
		}

		public static LocalizedString VersionMismatchDuringCreateRemoteRunspace
		{
			get
			{
				return new LocalizedString("VersionMismatchDuringCreateRemoteRunspace", "ExB0FB5E", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString LoadingRoleAssignmentErrorText(string account)
		{
			return new LocalizedString("LoadingRoleAssignmentErrorText", "ExE848AF", false, true, Strings.ResourceManager, new object[]
			{
				account
			});
		}

		public static LocalizedString ExceptionMissingDataSourceManager(string codeBase)
		{
			return new LocalizedString("ExceptionMissingDataSourceManager", "ExCF7C58", false, true, Strings.ResourceManager, new object[]
			{
				codeBase
			});
		}

		public static LocalizedString ErrorCannotOpenServiceControllerManager
		{
			get
			{
				return new LocalizedString("ErrorCannotOpenServiceControllerManager", "ExFC4F1D", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ProvisioningExceptionMessage(string error)
		{
			return new LocalizedString("ProvisioningExceptionMessage", "ExA2F22D", false, true, Strings.ResourceManager, new object[]
			{
				error
			});
		}

		public static LocalizedString ErrorObjectHasValidationErrorsWithId(object identity)
		{
			return new LocalizedString("ErrorObjectHasValidationErrorsWithId", "Ex34751A", false, true, Strings.ResourceManager, new object[]
			{
				identity
			});
		}

		public static LocalizedString VerboseLbNoEligibleServers
		{
			get
			{
				return new LocalizedString("VerboseLbNoEligibleServers", "Ex1DA741", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ExceptionTaskExpansionTooDeep(Type taskType)
		{
			return new LocalizedString("ExceptionTaskExpansionTooDeep", "Ex20F3BB", false, true, Strings.ResourceManager, new object[]
			{
				taskType
			});
		}

		public static LocalizedString VerboseSourceFromGC(string source)
		{
			return new LocalizedString("VerboseSourceFromGC", "Ex69861F", false, true, Strings.ResourceManager, new object[]
			{
				source
			});
		}

		public static LocalizedString VerboseCannotResolveSid(string sid, string msg)
		{
			return new LocalizedString("VerboseCannotResolveSid", "Ex68400D", false, true, Strings.ResourceManager, new object[]
			{
				sid,
				msg
			});
		}

		public static LocalizedString ErrorCannotSendMailToPublicFolderMailbox(string id)
		{
			return new LocalizedString("ErrorCannotSendMailToPublicFolderMailbox", "", false, false, Strings.ResourceManager, new object[]
			{
				id
			});
		}

		public static LocalizedString ErrorCorruptedPartition(string partitionName)
		{
			return new LocalizedString("ErrorCorruptedPartition", "", false, false, Strings.ResourceManager, new object[]
			{
				partitionName
			});
		}

		public static LocalizedString VerboseADObjectChangedPropertiesWithId(string id, string propertyList)
		{
			return new LocalizedString("VerboseADObjectChangedPropertiesWithId", "Ex85EC0D", false, true, Strings.ResourceManager, new object[]
			{
				id,
				propertyList
			});
		}

		public static LocalizedString VerboseLbFoundMailboxServer(string mailboxServer)
		{
			return new LocalizedString("VerboseLbFoundMailboxServer", "ExC8624C", false, true, Strings.ResourceManager, new object[]
			{
				mailboxServer
			});
		}

		public static LocalizedString ErrorSetTaskChangeRecipientType(string id, string oldType, string newType)
		{
			return new LocalizedString("ErrorSetTaskChangeRecipientType", "ExA36246", false, true, Strings.ResourceManager, new object[]
			{
				id,
				oldType,
				newType
			});
		}

		public static LocalizedString ServiceStartFailure(string name, string msg)
		{
			return new LocalizedString("ServiceStartFailure", "ExA4C2D6", false, true, Strings.ResourceManager, new object[]
			{
				name,
				msg
			});
		}

		public static LocalizedString VerboseLbDatabaseFound
		{
			get
			{
				return new LocalizedString("VerboseLbDatabaseFound", "Ex3260C3", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString VerboseADObjectNoChangedProperties
		{
			get
			{
				return new LocalizedString("VerboseADObjectNoChangedProperties", "Ex13F412", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorCouldNotFindCorrespondingObject(string identity, Type type, string dc)
		{
			return new LocalizedString("ErrorCouldNotFindCorrespondingObject", "ExEC9D18", false, true, Strings.ResourceManager, new object[]
			{
				identity,
				type,
				dc
			});
		}

		public static LocalizedString VerboseWriteResultSize(string resultSize)
		{
			return new LocalizedString("VerboseWriteResultSize", "Ex07FDA9", false, true, Strings.ResourceManager, new object[]
			{
				resultSize
			});
		}

		public static LocalizedString VerbosePopulateScopeSet
		{
			get
			{
				return new LocalizedString("VerbosePopulateScopeSet", "Ex0A310D", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString VerboseTaskSpecifiedParameters(string paramInfo)
		{
			return new LocalizedString("VerboseTaskSpecifiedParameters", "", false, false, Strings.ResourceManager, new object[]
			{
				paramInfo
			});
		}

		public static LocalizedString VerboseLbDatabaseNotInUserScope(string databaseDn, string errorMessage)
		{
			return new LocalizedString("VerboseLbDatabaseNotInUserScope", "ExE29229", false, true, Strings.ResourceManager, new object[]
			{
				databaseDn,
				errorMessage
			});
		}

		public static LocalizedString ErrorCertificateDenied
		{
			get
			{
				return new LocalizedString("ErrorCertificateDenied", "ExCEFBB0", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorNoServersAndOutofServerScope(string databaseid, string serverid)
		{
			return new LocalizedString("ErrorNoServersAndOutofServerScope", "ExB07E6B", false, true, Strings.ResourceManager, new object[]
			{
				databaseid,
				serverid
			});
		}

		public static LocalizedString ErrorInvalidAddressListIdentity(string idValue)
		{
			return new LocalizedString("ErrorInvalidAddressListIdentity", "ExB3AA2D", false, true, Strings.ResourceManager, new object[]
			{
				idValue
			});
		}

		public static LocalizedString ErrorOrganizationalUnitNotFound(string idStringValue)
		{
			return new LocalizedString("ErrorOrganizationalUnitNotFound", "ExF75CBB", false, true, Strings.ResourceManager, new object[]
			{
				idStringValue
			});
		}

		public static LocalizedString ErrorCannotResolvePUIDToWindowsIdentity
		{
			get
			{
				return new LocalizedString("ErrorCannotResolvePUIDToWindowsIdentity", "ExB19BD7", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString VerboseAdminSessionSettingsConfigDC(string configDC)
		{
			return new LocalizedString("VerboseAdminSessionSettingsConfigDC", "Ex78E529", false, true, Strings.ResourceManager, new object[]
			{
				configDC
			});
		}

		public static LocalizedString ErrorMissOrganization
		{
			get
			{
				return new LocalizedString("ErrorMissOrganization", "Ex55298E", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString WrongTypeRecipientIdParamter(string identity)
		{
			return new LocalizedString("WrongTypeRecipientIdParamter", "Ex555066", false, true, Strings.ResourceManager, new object[]
			{
				identity
			});
		}

		public static LocalizedString ErrorOuOutOfOrganization(string ou)
		{
			return new LocalizedString("ErrorOuOutOfOrganization", "ExC44E57", false, true, Strings.ResourceManager, new object[]
			{
				ou
			});
		}

		public static LocalizedString InvocationExceptionDescription(string error, string commandText)
		{
			return new LocalizedString("InvocationExceptionDescription", "Ex9078DF", false, true, Strings.ResourceManager, new object[]
			{
				error,
				commandText
			});
		}

		public static LocalizedString VerboseLbNetworkError(string errorMessage)
		{
			return new LocalizedString("VerboseLbNetworkError", "ExDE3357", false, true, Strings.ResourceManager, new object[]
			{
				errorMessage
			});
		}

		public static LocalizedString ExceptionMDAConnectionAlreadyOpened
		{
			get
			{
				return new LocalizedString("ExceptionMDAConnectionAlreadyOpened", "Ex8C607C", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ExceptionObjectStillExists
		{
			get
			{
				return new LocalizedString("ExceptionObjectStillExists", "ExA36AA4", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ExceptionMDAConnectionMustBeOpened
		{
			get
			{
				return new LocalizedString("ExceptionMDAConnectionMustBeOpened", "ExAFC863", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString WriteErrorMessage
		{
			get
			{
				return new LocalizedString("WriteErrorMessage", "ExCCF2A6", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString UserNotSAMAccount(string user)
		{
			return new LocalizedString("UserNotSAMAccount", "ExC8F580", false, true, Strings.ResourceManager, new object[]
			{
				user
			});
		}

		public static LocalizedString VerboseADObjectChangedPropertiesWithDn(string id, string dn, string propertyList)
		{
			return new LocalizedString("VerboseADObjectChangedPropertiesWithDn", "ExB09D19", false, true, Strings.ResourceManager, new object[]
			{
				id,
				dn,
				propertyList
			});
		}

		public static LocalizedString ErrorChangeServiceConfig2(string name)
		{
			return new LocalizedString("ErrorChangeServiceConfig2", "Ex326B1C", false, true, Strings.ResourceManager, new object[]
			{
				name
			});
		}

		public static LocalizedString InstantiatingHandlerForAgent(int i, string agentName)
		{
			return new LocalizedString("InstantiatingHandlerForAgent", "Ex93D03D", false, true, Strings.ResourceManager, new object[]
			{
				i,
				agentName
			});
		}

		public static LocalizedString GenericConditionFailure
		{
			get
			{
				return new LocalizedString("GenericConditionFailure", "ExA30D5C", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString LoadingRoleAssignment(string account)
		{
			return new LocalizedString("LoadingRoleAssignment", "ExECD590", false, true, Strings.ResourceManager, new object[]
			{
				account
			});
		}

		public static LocalizedString ErrorServerNotFound(string idStringValue)
		{
			return new LocalizedString("ErrorServerNotFound", "Ex0290FB", false, true, Strings.ResourceManager, new object[]
			{
				idStringValue
			});
		}

		public static LocalizedString ErrorServerNotUnique(string idStringValue)
		{
			return new LocalizedString("ErrorServerNotUnique", "ExD07479", false, true, Strings.ResourceManager, new object[]
			{
				idStringValue
			});
		}

		public static LocalizedString PswsInvocationTimout(int timeoutMsec)
		{
			return new LocalizedString("PswsInvocationTimout", "", false, false, Strings.ResourceManager, new object[]
			{
				timeoutMsec
			});
		}

		public static LocalizedString VerboseAdminSessionSettingsDCs(string DCs)
		{
			return new LocalizedString("VerboseAdminSessionSettingsDCs", "Ex9365DA", false, true, Strings.ResourceManager, new object[]
			{
				DCs
			});
		}

		public static LocalizedString VerboseLbExRpcAdminExists
		{
			get
			{
				return new LocalizedString("VerboseLbExRpcAdminExists", "ExA32E96", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ClashingPriorities(string Priority, string Name, string otherAgentName)
		{
			return new LocalizedString("ClashingPriorities", "Ex9F2F5E", false, true, Strings.ResourceManager, new object[]
			{
				Priority,
				Name,
				otherAgentName
			});
		}

		public static LocalizedString VerboseResolvedOrganization(string orgId)
		{
			return new LocalizedString("VerboseResolvedOrganization", "Ex6C34B5", false, true, Strings.ResourceManager, new object[]
			{
				orgId
			});
		}

		public static LocalizedString WrongTypeComputer(string identity)
		{
			return new LocalizedString("WrongTypeComputer", "Ex828D5C", false, true, Strings.ResourceManager, new object[]
			{
				identity
			});
		}

		public static LocalizedString VerboseTaskFindDataObjects(string type, string filter, string scope, string root)
		{
			return new LocalizedString("VerboseTaskFindDataObjects", "ExEA61FA", false, true, Strings.ResourceManager, new object[]
			{
				type,
				filter,
				scope,
				root
			});
		}

		public static LocalizedString ExceptionTypeNotFound(string typeName)
		{
			return new LocalizedString("ExceptionTypeNotFound", "Ex873EFB", false, true, Strings.ResourceManager, new object[]
			{
				typeName
			});
		}

		public static LocalizedString VerboseFailedToDeserializePSObject(string msg)
		{
			return new LocalizedString("VerboseFailedToDeserializePSObject", "", false, false, Strings.ResourceManager, new object[]
			{
				msg
			});
		}

		public static LocalizedString WrongTypeGeneralMailboxIdParameter(string identity)
		{
			return new LocalizedString("WrongTypeGeneralMailboxIdParameter", "Ex2E1EF6", false, true, Strings.ResourceManager, new object[]
			{
				identity
			});
		}

		public static LocalizedString WrongTypeMailboxUserContact(string identity)
		{
			return new LocalizedString("WrongTypeMailboxUserContact", "Ex2B5F02", false, true, Strings.ResourceManager, new object[]
			{
				identity
			});
		}

		public static LocalizedString AgentAssemblyWithoutPathFound(string agentName)
		{
			return new LocalizedString("AgentAssemblyWithoutPathFound", "ExD7B056", false, true, Strings.ResourceManager, new object[]
			{
				agentName
			});
		}

		public static LocalizedString ConfirmSharedConfiguration(string id)
		{
			return new LocalizedString("ConfirmSharedConfiguration", "", false, false, Strings.ResourceManager, new object[]
			{
				id
			});
		}

		public static LocalizedString DisabledString
		{
			get
			{
				return new LocalizedString("DisabledString", "Ex1527F0", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ExceptionRemoveNoneExistenceObject
		{
			get
			{
				return new LocalizedString("ExceptionRemoveNoneExistenceObject", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString UserQuotaDelayNotEnforcedMaxThreadsExceeded(int cappedDelay, bool required, string part, int threadNum)
		{
			return new LocalizedString("UserQuotaDelayNotEnforcedMaxThreadsExceeded", "", false, false, Strings.ResourceManager, new object[]
			{
				cappedDelay,
				required,
				part,
				threadNum
			});
		}

		public static LocalizedString LogErrorPrefix
		{
			get
			{
				return new LocalizedString("LogErrorPrefix", "ExBFA257", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorOrgNotFound(string identity, string org)
		{
			return new LocalizedString("ErrorOrgNotFound", "Ex1F3BD8", false, true, Strings.ResourceManager, new object[]
			{
				identity,
				org
			});
		}

		public static LocalizedString MultipleHandlersForCmdlet(string cmdlet, string asm1, string asm2)
		{
			return new LocalizedString("MultipleHandlersForCmdlet", "Ex32A86B", false, true, Strings.ResourceManager, new object[]
			{
				cmdlet,
				asm1,
				asm2
			});
		}

		public static LocalizedString ErrorMapiPublicFolderTreeNotFound
		{
			get
			{
				return new LocalizedString("ErrorMapiPublicFolderTreeNotFound", "ExF42A65", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ConditionNotInitialized(string uninitializedProperty, Condition owningCondition)
		{
			return new LocalizedString("ConditionNotInitialized", "Ex2B6554", false, true, Strings.ResourceManager, new object[]
			{
				uninitializedProperty,
				owningCondition
			});
		}

		public static LocalizedString VerboseLbOnlyOneEligibleServer(string onlyServer)
		{
			return new LocalizedString("VerboseLbOnlyOneEligibleServer", "ExACACED", false, true, Strings.ResourceManager, new object[]
			{
				onlyServer
			});
		}

		public static LocalizedString ProvisioningBrokerInitializationFailed(string message)
		{
			return new LocalizedString("ProvisioningBrokerInitializationFailed", "Ex0CC47D", false, true, Strings.ResourceManager, new object[]
			{
				message
			});
		}

		public static LocalizedString LogPreconditionDeferred(Type conditionType)
		{
			return new LocalizedString("LogPreconditionDeferred", "Ex865596", false, true, Strings.ResourceManager, new object[]
			{
				conditionType
			});
		}

		public static LocalizedString VerboseAdminSessionSettings(string cmdletName)
		{
			return new LocalizedString("VerboseAdminSessionSettings", "ExD53031", false, true, Strings.ResourceManager, new object[]
			{
				cmdletName
			});
		}

		public static LocalizedString WrongTypeNonMailEnabledGroup(string identity)
		{
			return new LocalizedString("WrongTypeNonMailEnabledGroup", "Ex68F6BD", false, true, Strings.ResourceManager, new object[]
			{
				identity
			});
		}

		public static LocalizedString ErrorIsOutofConfigWriteScope(string type, string id)
		{
			return new LocalizedString("ErrorIsOutofConfigWriteScope", "Ex6404E2", false, true, Strings.ResourceManager, new object[]
			{
				type,
				id
			});
		}

		public static LocalizedString GetLocalizedString(Strings.IDs key)
		{
			return new LocalizedString(Strings.stringIDs[(uint)key], Strings.ResourceManager, new object[0]);
		}

		private static Dictionary<uint, string> stringIDs = new Dictionary<uint, string>(110);

		private static ExchangeResourceManager ResourceManager = ExchangeResourceManager.GetResourceManager("Microsoft.Exchange.Configuration.Common.LocStrings.Strings", typeof(Strings).GetTypeInfo().Assembly);

		public enum IDs : uint
		{
			MissingImpersonatedUserSid = 3480090246U,
			FalseString = 3445110312U,
			VerboseFailedToGetServiceTopology = 2329139938U,
			NotGrantCustomScriptRole = 831535795U,
			ErrorChangeImmutableType = 3609756192U,
			NotGrantAnyAdminRoles = 1212809041U,
			ErrorOnlyDNSupportedWithIgnoreDefaultScope = 2716022819U,
			ExceptionTaskNotInitialized = 2866126871U,
			VerboseLbNoDatabaseFoundInAD = 311479382U,
			WorkUnitStatusNotStarted = 1464829243U,
			TrueString = 444666707U,
			InvalidPswsDirectInvocationBlocked = 1953440811U,
			InvalidProperties = 2103372448U,
			ErrorInstanceObjectConatinsNullIdentity = 3344562832U,
			ErrorAdminLoginUsingAppPassword = 2895423986U,
			ErrorCannotFindMailboxToLogonPublicStore = 2403641406U,
			WorkUnitStatusInProgress = 3498243205U,
			ErrorObjectHasValidationErrors = 19923723U,
			ErrorNoProvisioningHandlerAvailable = 452500353U,
			WarningUnlicensedMailbox = 1811096880U,
			WorkUnitStatusFailed = 767622998U,
			VerboseInitializeRunspaceServerSettingsRemote = 1306006239U,
			ExceptionTaskInconsistent = 585695277U,
			WarningForceMessage = 1520502444U,
			WarningCannotSetPrimarySmtpAddressWhenEapEnabled = 4140272390U,
			ExecutingUserNameIsMissing = 3204373538U,
			ExceptionNoChangesSpecified = 2188568225U,
			ExceptionTaskAlreadyInitialized = 3805201920U,
			ParameterValueTooLarge = 2951618283U,
			ErrorNotSupportSingletonWildcard = 3718372071U,
			WorkUnitCollectionConfigurationSummary = 2784860353U,
			ExceptionMDACommandNotExecuting = 143513383U,
			ErrorRemotePowershellConnectionBlocked = 1958023215U,
			LogExecutionFailed = 2725322457U,
			VerboseLbNoOabVDirReturned = 3624978883U,
			VerboseLbEnterSiteMailboxEnterprise = 2158269158U,
			HierarchicalIdentityNullOrEmpty = 2890992798U,
			ExceptionObjectAlreadyExists = 1268762784U,
			ErrorRemotePowerShellNotEnabled = 2576106929U,
			ErrorRbacConfigurationNotSupportedSharedConfiguration = 298822364U,
			UnknownEnumValue = 1356455742U,
			ErrorOperationRequiresManager = 439815616U,
			ErrorOrganizationWildcard = 2916171677U,
			ErrorDelegatedUserNotInOrg = 687978330U,
			VerboseSerializationDataNotExist = 1837325848U,
			ErrorNoAvailablePublicFolderDatabase = 1983570122U,
			SessionExpiredException = 1655423524U,
			HierarchicalIdentityStartsOrEndsWithBackslash = 2661346553U,
			VerboseInitializeRunspaceServerSettingsLocal = 2859768776U,
			ExceptionMDACommandStillExecuting = 1551194332U,
			WorkUnitError = 1960180119U,
			VerboseNoSource = 4205209694U,
			ErrorFilteringOnlyUserLogin = 797535012U,
			ExceptionNullInstanceParameter = 1654901580U,
			VerboseLbCreateNewExRpcAdmin = 498095919U,
			ErrorCannotDiscoverDefaultOrganizationUnitForRecipient = 2137526570U,
			CommandSucceeded = 3713958116U,
			ErrorUninitializedParameter = 84680862U,
			ErrorNotAllowedForPartnerAccess = 2037166858U,
			ExceptionTaskNotExecuted = 1859189834U,
			ErrorInvalidResultSize = 3245318277U,
			VerboseLbDeletedServer = 201309884U,
			TaskCompleted = 4282198592U,
			ErrorMaxTenantPSConnectionLimitNotResolved = 1234513041U,
			InvalidCharacterInComponentPartOfHierarchicalIdentity = 1421372901U,
			ExceptionReadOnlyPropertyBag = 838517570U,
			VerboseLbTryRetrieveDatabaseStatus = 3781767156U,
			ErrorIgnoreDefaultScopeAndDCSetTogether = 3216817101U,
			ExceptionGettingConditionObject = 3519173187U,
			EnabledString = 4160063394U,
			WorkUnitWarning = 3507110937U,
			VerboseLbServerDownSoMarkDatabaseDown = 1170623981U,
			BinaryDataStakeHodler = 3828427927U,
			ErrorWriteOpOnDehydratedTenant = 1221524445U,
			ExceptionMDACommandAlreadyExecuting = 3394388186U,
			SipCultureInfoArgumentCheckFailure = 2174831997U,
			ConsecutiveWholeWildcardNamePartsInHierarchicalIdentity = 577240232U,
			ErrorMapiPublicFolderTreeNotUnique = 324189978U,
			WarningMoreResultsAvailable = 3002944702U,
			ErrorOperationOnInvalidObject = 3806422804U,
			VerboseInitializeRunspaceServerSettingsAdam = 4253079958U,
			VerboseLbNoAvailableDatabase = 3746749985U,
			PswsManagementAutomationAssemblyLoadError = 3026495307U,
			LogRollbackFailed = 2439453065U,
			NullOrEmptyNamePartsInHierarchicalIdentity = 2502725106U,
			ErrorCloseServiceHandle = 328156873U,
			WorkUnitStatusCompleted = 3026665436U,
			ErrorUrlInValid = 2267892936U,
			ErrorNoMailboxUserInTheForest = 2717050851U,
			ServerNotAvailable = 4074275529U,
			HelpUrlHeaderText = 292337732U,
			VersionMismatchDuringCreateRemoteRunspace = 2496835620U,
			ErrorCannotOpenServiceControllerManager = 2667845807U,
			VerboseLbNoEligibleServers = 2472100876U,
			VerboseLbDatabaseFound = 3699804131U,
			VerboseADObjectNoChangedProperties = 3296071226U,
			VerbosePopulateScopeSet = 2442074922U,
			ErrorCertificateDenied = 2293662344U,
			ErrorCannotResolvePUIDToWindowsIdentity = 4083645591U,
			ErrorMissOrganization = 2468805291U,
			ExceptionMDAConnectionAlreadyOpened = 1891802266U,
			ExceptionObjectStillExists = 982491582U,
			ExceptionMDAConnectionMustBeOpened = 3844753652U,
			WriteErrorMessage = 1602649260U,
			GenericConditionFailure = 4100583810U,
			VerboseLbExRpcAdminExists = 4285414215U,
			DisabledString = 325596373U,
			ExceptionRemoveNoneExistenceObject = 2781337548U,
			LogErrorPrefix = 492587358U,
			ErrorMapiPublicFolderTreeNotFound = 1558360907U
		}

		private enum ParamIDs
		{
			LoadingRole,
			LookupUserAsSAMAccount,
			VerboseLbOABOwnedByServer,
			VerboseLbNoServerForDatabaseException,
			WrongTypeMailboxUser,
			VerboseRequestFilterInGetTask,
			CannotFindClassFactoryInAgentAssembly,
			ErrorInvalidStatePartnerOrgNotNull,
			LoadingScopeErrorText,
			VerboseLbOABIsCurrentlyOnServer,
			AgentAssemblyDuplicateFound,
			LogHelpUrl,
			NoRoleEntriesFound,
			VerboseLbPermanentException,
			ErrorIncompleteDCPublicFolderIdParameter,
			ErrorInvalidServerName,
			VerboseLbNoAvailableE15Database,
			MutuallyExclusiveArguments,
			LogResolverInstantiated,
			ProvisionDefaultProperties,
			VerboseReadADObject,
			ResourceLoadDelayNotEnforcedMaxThreadsExceeded,
			ErrorNotAcceptedDomain,
			SortOrderFormatException,
			LoadingScope,
			ExceptionObjectNotFound,
			WrongTypeMailboxOrMailUser,
			ErrorPolicyUserOrSecurityGroupNotFound,
			ExceptionSetupRegkeyMissing,
			PswsDeserializationError,
			OnbehalfOf,
			ErrorIncompletePublicFolderIdParameter,
			VerboseFailedToReadFromDC,
			ErrorNotMailboxServer,
			ExceptionNoConversion,
			PropertyProvisioned,
			VerboseTaskReadDataObject,
			UnExpectedElement,
			ExceptionLegacyObjects,
			ExceptionObjectAmbiguous,
			VerboseLbInitialProvisioningDatabaseExcluded,
			ErrorMaxTenantPSConnectionLimit,
			ProvisioningPreInternalProcessRecord,
			VerboseLbDatabase,
			VerboseSourceFromDC,
			VerboseLbIsDatabaseLocal,
			ErrorOrgOutOfPartnerScope,
			ServiceAlreadyNotInstalled,
			WarningCmdletTarpittingByResourceLoad,
			ErrorManagementObjectNotFoundByType,
			VerboseLbSitesAreNotBalanced,
			ErrorUserNotFound,
			PswsResponseChildElementNotExisingError,
			VerboseLbReturningServer,
			ErrorConfigurationUnitNotFound,
			PswsRequestException,
			ErrorAddressListNotFound,
			ErrorNoPartnerScopes,
			VerboseSkipObject,
			ErrorPolicyUserOrSecurityGroupNotUnique,
			ErrorParentHasNewerVersion,
			VerboseAdminSessionSettingsUserConfigDC,
			LogConditionFailed,
			LogFunctionExit,
			WrongTypeMailboxRecipient,
			WrongTypeUser,
			WillUninstallInstalledService,
			ExceptionMissingDetailSchemaFile,
			MonitoringPerfomanceCounterString,
			VerboseAdminSessionSettingsUserDCs,
			WarningDefaultResultSizeReached,
			UnknownAuditManagerType,
			ExceptionRoleNotFoundObjects,
			LogTaskCandidate,
			VerboseTaskProcessingObject,
			VerboseRereadADObject,
			InvocationExceptionDescriptionWithoutError,
			CouldntFindClassFactoryInAssembly,
			VerboseAdminSessionSettingsDefaultScope,
			WarningCannotWriteToEventLog,
			InvalidAttribute,
			ExceptionMismatchedConfigObjectType,
			VerboseSource,
			VerboseLbRetryableException,
			ErrorPartnerApplicationWithoutLinkedAccount,
			ErrorNoAvailablePublicFolderDatabaseInDatacenter,
			NoRequiredRole,
			ErrorOrganizationNotUnique,
			WrongTypeUserContactComputer,
			ErrorOperationTarpitting,
			ConfigObjectAmbiguous,
			VerboseLbDatabaseIsNotOnline,
			InvalidNegativeValue,
			ErrorNotUserMailboxCanLogonPFDatabase,
			VerboseLbRemoteSiteDatabaseReturned,
			RBACContextParserException,
			ErrorCannotOpenService,
			ExArgumentNullException,
			WarningWindowsEmailAddressTooLong,
			PswsResponseElementNotExisingError,
			NoRoleEntriesWithParametersFound,
			VerbosePostConditions,
			ErrorProvisioningValidation,
			UnhandledErrorMessage,
			WrongTypeMailContact,
			PiiRedactionInitializationFailed,
			VerboseAdminSessionSettingsViewForest,
			VerboseRemovingRoleAssignment,
			CommandNotFoundError,
			WrongTypeMailPublicFolder,
			OverallElapsedTimeDescription,
			ExceptionMDACommandExcutionError,
			ErrorRecipientPropertyValueAlreadybeUsed,
			ErrorManagementObjectNotFound,
			ErrorInvalidMailboxStoreObjectIdentity,
			ErrorCmdletProxy,
			WarningCouldNotRemoveRoleAssignment,
			VerboseDeleteObject,
			VerboseCmdletProxiedToAnotherServer,
			ExceptionInvalidDatabaseLegacyDnFormat,
			LogFunctionEnter,
			VerboseAdminSessionSettingsUserGlobalCatalog,
			CouldNotDeterimineServiceInstanceException,
			ErrorInvalidParameterFormat,
			VerboseAdminSessionSettingsUserAFGlobalCatalog,
			WrongTypeSecurityPrincipal,
			ErrorInvalidGlobalAddressListIdentity,
			WarningTaskRetried,
			AssemblyFileNotFound,
			VerboseLbDatabaseContainer,
			ErrorProxyAddressAlreadyExists,
			VerboseCannotGetRemoteServiceUriForUser,
			VerboseAdminSessionSettingsUserAFConfigDC,
			VerboseLbCountOfOABRecordsOwnedByServer,
			PswsSerializationError,
			CouldNotStartService,
			VerboseTaskGetDataObjects,
			ErrorPublicFolderMailDisabled,
			VerboseLbIsLocalSiteNotEnoughInformation,
			ErrorRecipientNotFound,
			VerboseAdminSessionSettingsRecipientViewRoot,
			VerboseLbFoundOabVDir,
			ErrorManagementObjectNotFoundWithSource,
			MicroDelayNotEnforcedMaxThreadsExceeded,
			ServiceStopFailure,
			ErrorRecipientPropertyValueAlreadyExists,
			VerboseTaskFindDataObjectsInAL,
			MultipleDefaultMailboxPlansFound,
			ElementMustNotHaveAttributes,
			WarningTaskModuleSkipped,
			ErrorIsOutofDatabaseScope,
			ExceptionInvalidTaskType,
			WorkUnitCollectionStatus,
			ErrorAddressListNotUnique,
			VerboseTaskParameterLoggingFailed,
			ErrorConversionFailed,
			ErrorRecipientNotUnique,
			ExceptionInvalidConfigObjectType,
			LogAutoResolving,
			VerboseInternalQueryFilterInGetTasks,
			PswsCmdletError,
			LogCheckpoint,
			ErrorOrganizationNotFound,
			ErrorGlobalAddressListNotFound,
			DependentArguments,
			ErrorParentNotFound,
			ErrorNotFoundWithReason,
			WrongTypeUserContactGroupIdParameter,
			VerboseAdminSessionSettingsGlobalCatalog,
			ExceptionParameterRange,
			ErrorUnsupportedValues,
			LogServiceState,
			ExceptionMDAInvalidConnectionString,
			ErrorParentNotFoundOnDomainController,
			ClassFactoryDoesNotImplementIProvisioningAgent,
			ErrorInvalidOrganizationalUnitDNFormat,
			VerboseDatabaseNotFound,
			VerboseRecipientTaskHelperGetOrgnization,
			NoRoleEntriesWithParameterFound,
			ErrorEmptyParameter,
			ErrorManagementObjectAmbiguous,
			MicroDelayInfo,
			WrongTypeRemoteMailbox,
			WrongTypeRoleGroup,
			LogServiceName,
			ErrorGlobalAddressListNotUnique,
			ErrorNoServerForPublicFolderDatabase,
			VerboseADObjectChangedProperties,
			PswsResponseIsnotXMLError,
			CheckIfUserIsASID,
			ExchangeSetupCannotResumeLog,
			VerboseAdminSessionSettingsAFConfigDC,
			VerboseTaskBeginProcessing,
			CouldNotStopService,
			LoadingLogonUserErrorText,
			CrossForestAccount,
			WrongTypeDistributionGroup,
			WrongTypeMailUser,
			ErrorRecipientNotInSameOrgWithDataObject,
			ErrorMustWriteToRidMaster,
			ServiceUninstallFailure,
			ExceptionResolverConstructorMissing,
			VerboseAdminSessionSettingsAFGlobalCatalog,
			CannotResolveParentOrganization,
			WrongTypeNonMailEnabledUser,
			WarningDuplicateOrganizationSpecified,
			ResourceLoadDelayInfo,
			VerboseLbBestServerSoFar,
			ErrorIsOutOfDatabaseScopeNoServerCheck,
			ErrorIsAcceptedDomain,
			ValidationRuleNotFound,
			ErrorInvalidUMHuntGroupIdentity,
			ErrorNoServersForDatabase,
			ProvisioningValidationErrors,
			ResubmitRequestDoesNotExist,
			UserQuotaDelayInfo,
			ErrorTaskWin32ExceptionVerbose,
			ErrorSetServiceObjectSecurity,
			ErrorQueryServiceObjectSecurity,
			ErrorNoAvailablePublicFolderDatabaseOnServer,
			LogConditionMatchingTypeMismacth,
			ConfigObjectNotFound,
			ExceptionLexError,
			VerboseADObjectNoChangedPropertiesWithId,
			VerboseLbCheckingDatabaseIsAllowedOnScope,
			InvalidElementValue,
			ErrorRoleAssignmentNotFound,
			WrongTypeMailboxPlan,
			WarningCmdletTarpittingByUserQuota,
			InvalidRBACContextType,
			VerboseLbOabVDirSelected,
			MonitoringEventStringWithInstanceName,
			NoRoleAssignmentsFound,
			VerboseSaveChange,
			WrongActiveSyncDeviceIdParameter,
			ErrorDuplicateManagementObjectFound,
			VerboseLbDatabaseNotFoundException,
			VerboseLbGetServerForActiveDatabaseCopy,
			ErrorOpenKeyDeniedForWrite,
			WarningCmdletMicroDelay,
			ErrorManagementObjectNotFoundWithSourceByType,
			ErrorInvalidParameterType,
			ExceptionCondition,
			ElapsedTimeDescription,
			ExceptionParseError,
			VerboseLbGeneralTrace,
			ErrorPublicFolderDatabaseIsNotMounted,
			WarningForceMessageWithId,
			ErrorPublicFolderGeneratingProxy,
			ErrorMaxRunspacesLimit,
			LogConditionMatchingPropertyMismatch,
			HandlerThronwExceptionInOnComplete,
			InvalidGuidParameter,
			LogTaskExecutionPlan,
			WarningCannotGetLocalServerFqdn,
			ExArgumentOutOfRangeException,
			ErrorRemoveNewerObject,
			DelegatedAdminAccount,
			ErrorInvalidHierarchicalIdentity,
			ExceptionRollbackFailed,
			VerboseTaskEndProcessing,
			ErrorInvalidMailboxFolderIdentity,
			ErrorFoundMultipleRootRole,
			ImpersonationNotAllowed,
			MonitoringEventString,
			ErrorFailedToReadFromDC,
			ErrorTenantMaxRunspacesTarpitting,
			ForeignForestTrustFailedException,
			WrongTypeUserContact,
			ProvisioningUpdateAffectedObject,
			LookupUserAsDomainUser,
			MissingAttribute,
			ErrorCannotFormatRecipient,
			ErrorCannotGetPublicFolderDatabaseByLegacyDn,
			VerboseRemovedRoleAssignment,
			LoadingLogonUser,
			ErrorObjectVersionChanged,
			WrongTypeMailboxOrMailUserOrMailContact,
			ErrorTaskWin32Exception,
			ServiceNotInstalled,
			PooledConnectionOpenTimeoutError,
			VerboseLbAddingEligibleServer,
			ErrorInvalidIdentity,
			NonMigratedUserException,
			InvalidPropertyName,
			WrongTypeDynamicGroup,
			LoadingRoleErrorText,
			PowerShellTimeout,
			VerboseLbDatabaseAndServerTry,
			ErrorCannotResolveCertificate,
			WrongTypeMailEnabledContact,
			ExchangeSetupCannotCopyWatson,
			VerboseApplyRusPolicyForRecipient,
			NotInSameOrg,
			WrongTypeLogonableObjectIdParameter,
			ClashingIdentities,
			ErrorMaxRunspacesTarpitting,
			VerboseExecutingUserContext,
			ExceptionSetupFileNotFound,
			VerboseFailedToGetProxyServer,
			ExceptionTaskContextConsistencyViolation,
			LogPreconditionImmediate,
			ErrorInvalidType,
			VerboseCannotGetRemoteServerForUser,
			ErrorNoReplicaOnServer,
			ErrorUserNotUnique,
			ErrorMaxConnectionLimit,
			DuplicateExternalDirectoryObjectIdException,
			InvalidElement,
			VerbosePreConditions,
			WrongTypeGroup,
			VerboseSelectedRusServer,
			VerboseLbDisposeExRpcAdmin,
			ErrorRelativeDn,
			InvalidAttributeValue,
			PropertyIsAlreadyProvisioned,
			ErrorInvalidModerator,
			ErrorOrganizationalUnitNotUnique,
			ExceptionMissingCreateInstance,
			ExecutingUserPropertyNotFound,
			CannotHaveLocalAccountException,
			LoadingRoleAssignmentErrorText,
			ExceptionMissingDataSourceManager,
			ProvisioningExceptionMessage,
			ErrorObjectHasValidationErrorsWithId,
			ExceptionTaskExpansionTooDeep,
			VerboseSourceFromGC,
			VerboseCannotResolveSid,
			ErrorCannotSendMailToPublicFolderMailbox,
			ErrorCorruptedPartition,
			VerboseADObjectChangedPropertiesWithId,
			VerboseLbFoundMailboxServer,
			ErrorSetTaskChangeRecipientType,
			ServiceStartFailure,
			ErrorCouldNotFindCorrespondingObject,
			VerboseWriteResultSize,
			VerboseTaskSpecifiedParameters,
			VerboseLbDatabaseNotInUserScope,
			ErrorNoServersAndOutofServerScope,
			ErrorInvalidAddressListIdentity,
			ErrorOrganizationalUnitNotFound,
			VerboseAdminSessionSettingsConfigDC,
			WrongTypeRecipientIdParamter,
			ErrorOuOutOfOrganization,
			InvocationExceptionDescription,
			VerboseLbNetworkError,
			UserNotSAMAccount,
			VerboseADObjectChangedPropertiesWithDn,
			ErrorChangeServiceConfig2,
			InstantiatingHandlerForAgent,
			LoadingRoleAssignment,
			ErrorServerNotFound,
			ErrorServerNotUnique,
			PswsInvocationTimout,
			VerboseAdminSessionSettingsDCs,
			ClashingPriorities,
			VerboseResolvedOrganization,
			WrongTypeComputer,
			VerboseTaskFindDataObjects,
			ExceptionTypeNotFound,
			VerboseFailedToDeserializePSObject,
			WrongTypeGeneralMailboxIdParameter,
			WrongTypeMailboxUserContact,
			AgentAssemblyWithoutPathFound,
			ConfirmSharedConfiguration,
			UserQuotaDelayNotEnforcedMaxThreadsExceeded,
			ErrorOrgNotFound,
			MultipleHandlersForCmdlet,
			ConditionNotInitialized,
			VerboseLbOnlyOneEligibleServer,
			ProvisioningBrokerInitializationFailed,
			LogPreconditionDeferred,
			VerboseAdminSessionSettings,
			WrongTypeNonMailEnabledGroup,
			ErrorIsOutofConfigWriteScope
		}
	}
}
