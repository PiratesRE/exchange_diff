using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.Deployment
{
	internal static class Strings
	{
		static Strings()
		{
			Strings.stringIDs.Add(3076655694U, "ExchangeVersionBlock");
			Strings.stringIDs.Add(2041384979U, "InflatingAndDecodingPassedInHash");
			Strings.stringIDs.Add(3961637084U, "MSFilterPackV2NotInstalled");
			Strings.stringIDs.Add(286971094U, "MailboxRoleAlreadyExists");
			Strings.stringIDs.Add(3853989365U, "TooManyResults");
			Strings.stringIDs.Add(3498036640U, "EventSystemStopped");
			Strings.stringIDs.Add(971944032U, "WindowsServer2008CoreServerEdition");
			Strings.stringIDs.Add(2386277257U, "DelegatedClientAccessFirstInstall");
			Strings.stringIDs.Add(564094161U, "NoConnectorToStar");
			Strings.stringIDs.Add(1256831615U, "LangPackUpgradeVersioning");
			Strings.stringIDs.Add(2177637643U, "PreviousVersionOfExchangeAlreadyInstalled");
			Strings.stringIDs.Add(3383574593U, "CannotUninstallClusterNode");
			Strings.stringIDs.Add(173856461U, "TestDCSettingsForHybridEnabledTenantFailed");
			Strings.stringIDs.Add(975440992U, "NoE12ServerWarning");
			Strings.stringIDs.Add(2186576589U, "DelegatedCafeFirstInstall");
			Strings.stringIDs.Add(4277708119U, "ParsingXmlDataFromDCFileOrDCCmdlet");
			Strings.stringIDs.Add(3412170869U, "InvalidPSCredential");
			Strings.stringIDs.Add(233623067U, "FoundOrgConfigHashInConfigFile");
			Strings.stringIDs.Add(1884243248U, "PSCredentialAndTheOrganizationHashIsNull");
			Strings.stringIDs.Add(1760005143U, "ClientAccessRoleAlreadyExists");
			Strings.stringIDs.Add(2779888852U, "AdcFound");
			Strings.stringIDs.Add(1542399594U, "ForestLevelNotWin2003Native");
			Strings.stringIDs.Add(2107927772U, "InstallExchangeRolesOnDomainController");
			Strings.stringIDs.Add(1889541029U, "ServerFQDNDisplayName");
			Strings.stringIDs.Add(2033421306U, "MemberOfDatabaseAvailabilityGroup");
			Strings.stringIDs.Add(2446058696U, "DelegatedMailboxFirstInstall");
			Strings.stringIDs.Add(3207300105U, "CannotSetADSplitPermissionValidator");
			Strings.stringIDs.Add(168558763U, "FrontendTransportRoleAlreadyExists");
			Strings.stringIDs.Add(4109056594U, "GatewayUpgrade605Block");
			Strings.stringIDs.Add(3696305017U, "ShouldReRunSetupForW3SVC");
			Strings.stringIDs.Add(2679933748U, "InvalidLocalServerName");
			Strings.stringIDs.Add(3945170485U, "VC2012PrereqMissing");
			Strings.stringIDs.Add(3419466150U, "DelegatedClientAccessFirstSP1upgrade");
			Strings.stringIDs.Add(3480633415U, "TestNotRunDueToRegistryKey");
			Strings.stringIDs.Add(782041369U, "GetOrgConfigWasRunOnPremises");
			Strings.stringIDs.Add(1141647571U, "AccessedFailedResult");
			Strings.stringIDs.Add(3552416860U, "Iis32BitMode");
			Strings.stringIDs.Add(719584928U, "EitherTheOnPremAcceptedDomainListOrTheDCAcceptedDomainsAreEmpty");
			Strings.stringIDs.Add(3601945308U, "DCAdminDisplayVersionFound");
			Strings.stringIDs.Add(1667305647U, "GlobalServerInstall");
			Strings.stringIDs.Add(3878879664U, "SetupLogStarted");
			Strings.stringIDs.Add(3187185748U, "HybridInfoObjectNotFound");
			Strings.stringIDs.Add(1950741283U, "DelegatedMailboxFirstSP1upgrade");
			Strings.stringIDs.Add(2076519121U, "RunningTenantTestDCSettingsForHybridEnabledTenant");
			Strings.stringIDs.Add(2999486005U, "PowerShellExecutionPolicyCheck");
			Strings.stringIDs.Add(1793580687U, "DCEndPointIsEmpty");
			Strings.stringIDs.Add(3615196321U, "OSMinVersionNotMet");
			Strings.stringIDs.Add(704357583U, "AttemptingToParseTheXmlData");
			Strings.stringIDs.Add(2759777131U, "DCPreviousAdminDisplayVersionFound");
			Strings.stringIDs.Add(601266997U, "DotNetFrameworkMinVersionNotMet");
			Strings.stringIDs.Add(3940011457U, "WinRMIISExtensionInstalled");
			Strings.stringIDs.Add(1727977492U, "SearchFoundationAssemblyLoaderKBNotInstalled");
			Strings.stringIDs.Add(841751018U, "TenantIsRunningE15");
			Strings.stringIDs.Add(541611516U, "Win7RpcHttpShouldRejectDuplicateConnB2PacketsUpdateNotInstalled");
			Strings.stringIDs.Add(219074271U, "DelegatedBridgeheadFirstInstall");
			Strings.stringIDs.Add(3254033387U, "GetOrganizationConfigCmdletNotFound");
			Strings.stringIDs.Add(3375133662U, "TheFilePassedInIsNotFromGetOrganizationConfig");
			Strings.stringIDs.Add(3667045786U, "W3SVCDisabledOrNotInstalled");
			Strings.stringIDs.Add(1853686419U, "HostingModeNotAvailable");
			Strings.stringIDs.Add(1062077161U, "LangPackInstalled");
			Strings.stringIDs.Add(3068577726U, "InsufficientPrivledges");
			Strings.stringIDs.Add(1587969884U, "BadFilePassedIn");
			Strings.stringIDs.Add(3070759373U, "OSCheckedBuild");
			Strings.stringIDs.Add(3443808673U, "NetBIOSNameNotMatchesDNSHostName");
			Strings.stringIDs.Add(1073237881U, "NoGCInSite");
			Strings.stringIDs.Add(2097390153U, "InflateAndDecodeReturnedDataFromGetOrgConfig");
			Strings.stringIDs.Add(4278863281U, "LocalDomainNotPrepared");
			Strings.stringIDs.Add(1570727447U, "Exchange2000or2003PresentInOrg");
			Strings.stringIDs.Add(4143233965U, "Exchange2013AnyOnExchange2007or2010Server");
			Strings.stringIDs.Add(4199205241U, "RunningTenantHybridTest");
			Strings.stringIDs.Add(1023311823U, "DCIsDataCenterBitFound");
			Strings.stringIDs.Add(24167743U, "PrereqAnalysisNullValue");
			Strings.stringIDs.Add(3736002086U, "MinVersionCheck");
			Strings.stringIDs.Add(2170309289U, "ThereWasAnExceptionWhileCheckingForHybridConfiguration");
			Strings.stringIDs.Add(715361555U, "ConnectingToTheDCToRunGetOrgConfig");
			Strings.stringIDs.Add(3133604821U, "BridgeheadRoleAlreadyExists");
			Strings.stringIDs.Add(1976971220U, "MSFilterPackV2SP1NotInstalled");
			Strings.stringIDs.Add(4164286807U, "EmptyResults");
			Strings.stringIDs.Add(1626517757U, "ComputerRODC");
			Strings.stringIDs.Add(1186202058U, "DelegatedCafeFirstSP1upgrade");
			Strings.stringIDs.Add(3145781320U, "Win7RpcHttpAssocCookieGuidUpdateNotInstalled");
			Strings.stringIDs.Add(1376602368U, "AccessedValueWhenMultipleResults");
			Strings.stringIDs.Add(1828122210U, "UpgradeGateway605Block");
			Strings.stringIDs.Add(3401172205U, "NotLoggedOntoDomain");
			Strings.stringIDs.Add(4130092005U, "DataReturnedFromDCIsInvalid");
			Strings.stringIDs.Add(1040666117U, "ValueNotFoundInCollection");
			Strings.stringIDs.Add(841278233U, "MpsSvcStopped");
			Strings.stringIDs.Add(1554849195U, "DCAcceptedDomainNameFound");
			Strings.stringIDs.Add(641150786U, "CannotAccessAD");
			Strings.stringIDs.Add(1752859078U, "UpdateProgressForWrongAnalysis");
			Strings.stringIDs.Add(53953329U, "ServerNotPrepared");
			Strings.stringIDs.Add(2931842358U, "CheckingTheAcceptedDomainAgainstOrgRelationshipDomains");
			Strings.stringIDs.Add(1081610576U, "VC2013PrereqMissing");
			Strings.stringIDs.Add(1463453412U, "DomainPrepRequired");
			Strings.stringIDs.Add(4229980062U, "NoMatchWasFoundBetweenTheOrgRelDomainsAndDCAcceptedDomains");
			Strings.stringIDs.Add(3125428864U, "MSDTCStopped");
			Strings.stringIDs.Add(4101630194U, "CannotUninstallOABServer");
			Strings.stringIDs.Add(2459322720U, "TestDCSettingsForHybridEnabledTenantPassed");
			Strings.stringIDs.Add(1194462944U, "Win7LDRGDRRMSManifestExpiryUpdateNotInstalled");
			Strings.stringIDs.Add(3696227166U, "NoE14ServerWarning");
			Strings.stringIDs.Add(3265622739U, "InstallOnDCInADSplitPermissionMode");
			Strings.stringIDs.Add(2337843968U, "DelegatedBridgeheadFirstSP1upgrade");
			Strings.stringIDs.Add(2879980483U, "CafeRoleAlreadyExists");
			Strings.stringIDs.Add(3469704679U, "UnifiedMessagingRoleNotInstalled");
			Strings.stringIDs.Add(2121009437U, "CannotUninstallDelegatedServer");
			Strings.stringIDs.Add(3379016135U, "WinRMDisabledOrNotInstalled");
			Strings.stringIDs.Add(3219123481U, "W2K8R2PrepareAdLdifdeNotInstalled");
			Strings.stringIDs.Add(4056098580U, "DelegatedFrontendTransportFirstSP1upgrade");
			Strings.stringIDs.Add(3957588404U, "PendingReboot");
			Strings.stringIDs.Add(3440414664U, "LangPackDiskSpaceCheck");
			Strings.stringIDs.Add(4195359116U, "SetupLogEnd");
			Strings.stringIDs.Add(1829824776U, "DelegatedUnifiedMessagingFirstInstall");
			Strings.stringIDs.Add(569737677U, "Win7WindowsIdentityFoundationUpdateNotInstalled");
			Strings.stringIDs.Add(43746744U, "E14BridgeheadRoleNotFound");
			Strings.stringIDs.Add(2882191588U, "UnifiedMessagingRoleAlreadyExists");
			Strings.stringIDs.Add(3915669625U, "NetTcpPortSharingSvcNotAuto");
			Strings.stringIDs.Add(4251553116U, "WindowsInstallerServiceDisabledOrNotInstalled");
			Strings.stringIDs.Add(3733193906U, "SMTPSvcInstalled");
			Strings.stringIDs.Add(2364459045U, "DelegatedUnifiedMessagingFirstSP1upgrade");
			Strings.stringIDs.Add(392652342U, "OldADAMInstalled");
			Strings.stringIDs.Add(4254576536U, "RunningOnPremTest");
			Strings.stringIDs.Add(3978292472U, "UcmaRedistMsi");
			Strings.stringIDs.Add(3827371534U, "EitherOrgRelOrAcceptDomainsWhereNull");
			Strings.stringIDs.Add(2570206190U, "ADAMSvcStopped");
			Strings.stringIDs.Add(212290597U, "W2K8R2PrepareSchemaLdifdeNotInstalled");
			Strings.stringIDs.Add(2160575392U, "LocalComputerIsDCInChildDomain");
			Strings.stringIDs.Add(275692458U, "ClusSvcInstalledRoleBlock");
			Strings.stringIDs.Add(1958335472U, "PrereqAnalysisStarted");
			Strings.stringIDs.Add(2120465116U, "BridgeheadRoleNotInstalled");
			Strings.stringIDs.Add(2369286853U, "OrgConfigDataIsEmptyOrNull");
			Strings.stringIDs.Add(1550596250U, "EdgeSubscriptionExists");
			Strings.stringIDs.Add(1584908934U, "InvalidOrTamperedFile");
			Strings.stringIDs.Add(160644360U, "SendConnectorException");
			Strings.stringIDs.Add(1212051581U, "FqdnMissing");
			Strings.stringIDs.Add(1980036245U, "SchemaNotPreparedExtendedRights");
			Strings.stringIDs.Add(1622330535U, "LangPackBundleCheck");
			Strings.stringIDs.Add(2543736554U, "FailedResult");
			Strings.stringIDs.Add(2361234894U, "Win2k12RefsUpdateNotInstalled");
			Strings.stringIDs.Add(3990514141U, "CannotAccessHttpSiteForEngineUpdates");
			Strings.stringIDs.Add(1332821968U, "SpeechRedistMsi");
			Strings.stringIDs.Add(1132011277U, "PendingRebootWindowsComponents");
			Strings.stringIDs.Add(87247761U, "OSMinVersionForFSMONotMet");
			Strings.stringIDs.Add(2387324271U, "InvalidDNSDomainName");
			Strings.stringIDs.Add(1254031586U, "UMLangPackDiskSpaceCheck");
			Strings.stringIDs.Add(3272988597U, "Win7LDRRMSManifestExpiryUpdateNotInstalled");
			Strings.stringIDs.Add(2902280231U, "TenantIsBeingUpgradedFromE14");
			Strings.stringIDs.Add(1412296371U, "InvalidADSite");
			Strings.stringIDs.Add(3588122483U, "RemoteRegException");
			Strings.stringIDs.Add(1276355988U, "ADNotPreparedForHostingValidator");
			Strings.stringIDs.Add(4039112198U, "Win2k12RollupUpdateNotInstalled");
			Strings.stringIDs.Add(2380070307U, "InvalidOSVersion");
			Strings.stringIDs.Add(2017435239U, "MinimumFrameworkNotInstalled");
			Strings.stringIDs.Add(3014916009U, "SchemaNotPrepared");
			Strings.stringIDs.Add(2501606560U, "HostingActiveDirectorySplitPermissionsNotSupported");
			Strings.stringIDs.Add(2053317018U, "DisplayVersionDCBitUpgradeStatusBitAndVersionAreCorrect");
			Strings.stringIDs.Add(624846257U, "UpgradeMinVersionBlock");
			Strings.stringIDs.Add(2938058350U, "InvalidOSVersionForAdminTools");
			Strings.stringIDs.Add(3831181830U, "ConditionIsFalse");
			Strings.stringIDs.Add(346774553U, "OrgConfigHashDoesNotExist");
			Strings.stringIDs.Add(2890177564U, "ProvisionedUpdateRequired");
			Strings.stringIDs.Add(1981068904U, "ComputerNotPartofDomain");
			Strings.stringIDs.Add(985137777U, "Win2k12UrefsUpdateNotInstalled");
			Strings.stringIDs.Add(260127048U, "CannotReturnNullForResult");
			Strings.stringIDs.Add(2452722357U, "SetADSplitPermissionWhenExchangeServerRolesOnDC");
			Strings.stringIDs.Add(3769154951U, "InflateAndDecoding");
			Strings.stringIDs.Add(2393957831U, "ADAMLonghornWin7ServerNotInstalled");
			Strings.stringIDs.Add(2715471913U, "DCIsUpgradingOrganizationFound");
			Strings.stringIDs.Add(3301968605U, "MailboxRoleNotInstalled");
			Strings.stringIDs.Add(3344709799U, "PrereqAnalysisParentExceptionValue");
			Strings.stringIDs.Add(1535334837U, "TenantHasNotYetBeenUpgradedToE15");
			Strings.stringIDs.Add(758899952U, "DotNetFrameworkNeedsUpdate");
			Strings.stringIDs.Add(1799611416U, "OSMinVersionForAdminToolsNotMet");
			Strings.stringIDs.Add(3629514284U, "NullLoggerHasBeenPassedIn");
			Strings.stringIDs.Add(888551196U, "HybridInfoPurePSObjectsNotSupported");
			Strings.stringIDs.Add(3204362251U, "LangPackBundleVersioning");
			Strings.stringIDs.Add(3457078824U, "SearchingForAttributes");
		}

		public static LocalizedString ExchangeVersionBlock
		{
			get
			{
				return new LocalizedString("ExchangeVersionBlock", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString LessThan(string first, string second)
		{
			return new LocalizedString("LessThan", Strings.ResourceManager, new object[]
			{
				first,
				second
			});
		}

		public static LocalizedString OnPremisesOrgRelationshipDomainsCrossWithAcceptedDomainReturnResult(string result)
		{
			return new LocalizedString("OnPremisesOrgRelationshipDomainsCrossWithAcceptedDomainReturnResult", Strings.ResourceManager, new object[]
			{
				result
			});
		}

		public static LocalizedString InflatingAndDecodingPassedInHash
		{
			get
			{
				return new LocalizedString("InflatingAndDecodingPassedInHash", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MSFilterPackV2NotInstalled
		{
			get
			{
				return new LocalizedString("MSFilterPackV2NotInstalled", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MailboxRoleAlreadyExists
		{
			get
			{
				return new LocalizedString("MailboxRoleAlreadyExists", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString QueryReturnedNull(string queryName)
		{
			return new LocalizedString("QueryReturnedNull", Strings.ResourceManager, new object[]
			{
				queryName
			});
		}

		public static LocalizedString ShouldNotBeNullOrEmpty(string name)
		{
			return new LocalizedString("ShouldNotBeNullOrEmpty", Strings.ResourceManager, new object[]
			{
				name
			});
		}

		public static LocalizedString TooManyResults
		{
			get
			{
				return new LocalizedString("TooManyResults", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString EventSystemStopped
		{
			get
			{
				return new LocalizedString("EventSystemStopped", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ServicesAreMarkedForDeletion(string ListofServices)
		{
			return new LocalizedString("ServicesAreMarkedForDeletion", Strings.ResourceManager, new object[]
			{
				ListofServices
			});
		}

		public static LocalizedString WindowsServer2008CoreServerEdition
		{
			get
			{
				return new LocalizedString("WindowsServer2008CoreServerEdition", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString DelegatedClientAccessFirstInstall
		{
			get
			{
				return new LocalizedString("DelegatedClientAccessFirstInstall", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString NoConnectorToStar
		{
			get
			{
				return new LocalizedString("NoConnectorToStar", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString LangPackUpgradeVersioning
		{
			get
			{
				return new LocalizedString("LangPackUpgradeVersioning", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString AssemblyVersion(string assemblyVersion)
		{
			return new LocalizedString("AssemblyVersion", Strings.ResourceManager, new object[]
			{
				assemblyVersion
			});
		}

		public static LocalizedString PrimaryDNSTestFailed(string dns)
		{
			return new LocalizedString("PrimaryDNSTestFailed", Strings.ResourceManager, new object[]
			{
				dns
			});
		}

		public static LocalizedString DRMinVersionNotMet(string version)
		{
			return new LocalizedString("DRMinVersionNotMet", Strings.ResourceManager, new object[]
			{
				version
			});
		}

		public static LocalizedString ADUpdateForDomainPrep(string name)
		{
			return new LocalizedString("ADUpdateForDomainPrep", Strings.ResourceManager, new object[]
			{
				name
			});
		}

		public static LocalizedString PreviousVersionOfExchangeAlreadyInstalled
		{
			get
			{
				return new LocalizedString("PreviousVersionOfExchangeAlreadyInstalled", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ProcessNeedsToBeClosedOnUninstall(string list)
		{
			return new LocalizedString("ProcessNeedsToBeClosedOnUninstall", Strings.ResourceManager, new object[]
			{
				list
			});
		}

		public static LocalizedString InvalidDomainToPrepare(string name)
		{
			return new LocalizedString("InvalidDomainToPrepare", Strings.ResourceManager, new object[]
			{
				name
			});
		}

		public static LocalizedString CannotUninstallClusterNode
		{
			get
			{
				return new LocalizedString("CannotUninstallClusterNode", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString TestDCSettingsForHybridEnabledTenantFailed
		{
			get
			{
				return new LocalizedString("TestDCSettingsForHybridEnabledTenantFailed", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorWhileRunning(string exceptionMsg)
		{
			return new LocalizedString("ErrorWhileRunning", Strings.ResourceManager, new object[]
			{
				exceptionMsg
			});
		}

		public static LocalizedString NoE12ServerWarning
		{
			get
			{
				return new LocalizedString("NoE12ServerWarning", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RegistryKeyNotFound(string key)
		{
			return new LocalizedString("RegistryKeyNotFound", Strings.ResourceManager, new object[]
			{
				key
			});
		}

		public static LocalizedString DelegatedCafeFirstInstall
		{
			get
			{
				return new LocalizedString("DelegatedCafeFirstInstall", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ParsingXmlDataFromDCFileOrDCCmdlet
		{
			get
			{
				return new LocalizedString("ParsingXmlDataFromDCFileOrDCCmdlet", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InvalidPSCredential
		{
			get
			{
				return new LocalizedString("InvalidPSCredential", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ConfigDCHostNameMismatch(string dc, string dcInRegistry)
		{
			return new LocalizedString("ConfigDCHostNameMismatch", Strings.ResourceManager, new object[]
			{
				dc,
				dcInRegistry
			});
		}

		public static LocalizedString FoundOrgConfigHashInConfigFile
		{
			get
			{
				return new LocalizedString("FoundOrgConfigHashInConfigFile", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString PSCredentialAndTheOrganizationHashIsNull
		{
			get
			{
				return new LocalizedString("PSCredentialAndTheOrganizationHashIsNull", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ClientAccessRoleAlreadyExists
		{
			get
			{
				return new LocalizedString("ClientAccessRoleAlreadyExists", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString AdcFound
		{
			get
			{
				return new LocalizedString("AdcFound", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString LocalTimeZone(string LocalZone)
		{
			return new LocalizedString("LocalTimeZone", Strings.ResourceManager, new object[]
			{
				LocalZone
			});
		}

		public static LocalizedString ForestLevelNotWin2003Native
		{
			get
			{
				return new LocalizedString("ForestLevelNotWin2003Native", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InstallExchangeRolesOnDomainController
		{
			get
			{
				return new LocalizedString("InstallExchangeRolesOnDomainController", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString HybridInfoOpeningRunspace(string uri)
		{
			return new LocalizedString("HybridInfoOpeningRunspace", Strings.ResourceManager, new object[]
			{
				uri
			});
		}

		public static LocalizedString E15E14CoexistenceMinOSReqFailure(string servers)
		{
			return new LocalizedString("E15E14CoexistenceMinOSReqFailure", Strings.ResourceManager, new object[]
			{
				servers
			});
		}

		public static LocalizedString PrereqAnalysisFailureToAccessResults(string memberName, string message)
		{
			return new LocalizedString("PrereqAnalysisFailureToAccessResults", Strings.ResourceManager, new object[]
			{
				memberName,
				message
			});
		}

		public static LocalizedString ServerFQDNDisplayName
		{
			get
			{
				return new LocalizedString("ServerFQDNDisplayName", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InvalidOrTamperedConfigFile(string pathToConfigFile)
		{
			return new LocalizedString("InvalidOrTamperedConfigFile", Strings.ResourceManager, new object[]
			{
				pathToConfigFile
			});
		}

		public static LocalizedString MemberOfDatabaseAvailabilityGroup
		{
			get
			{
				return new LocalizedString("MemberOfDatabaseAvailabilityGroup", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString DelegatedMailboxFirstInstall
		{
			get
			{
				return new LocalizedString("DelegatedMailboxFirstInstall", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString CannotSetADSplitPermissionValidator
		{
			get
			{
				return new LocalizedString("CannotSetADSplitPermissionValidator", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MailboxEDBDriveDoesNotExist(string drive)
		{
			return new LocalizedString("MailboxEDBDriveDoesNotExist", Strings.ResourceManager, new object[]
			{
				drive
			});
		}

		public static LocalizedString OSVersion(string version)
		{
			return new LocalizedString("OSVersion", Strings.ResourceManager, new object[]
			{
				version
			});
		}

		public static LocalizedString LonghornIISManagementConsoleInstalledValidator(string iisversion)
		{
			return new LocalizedString("LonghornIISManagementConsoleInstalledValidator", Strings.ResourceManager, new object[]
			{
				iisversion
			});
		}

		public static LocalizedString FrontendTransportRoleAlreadyExists
		{
			get
			{
				return new LocalizedString("FrontendTransportRoleAlreadyExists", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString GatewayUpgrade605Block
		{
			get
			{
				return new LocalizedString("GatewayUpgrade605Block", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString WatermarkPresent(string role)
		{
			return new LocalizedString("WatermarkPresent", Strings.ResourceManager, new object[]
			{
				role
			});
		}

		public static LocalizedString LonghornIISMetabaseNotInstalled(string iisversion)
		{
			return new LocalizedString("LonghornIISMetabaseNotInstalled", Strings.ResourceManager, new object[]
			{
				iisversion
			});
		}

		public static LocalizedString VoiceMessagesInQueue(string path)
		{
			return new LocalizedString("VoiceMessagesInQueue", Strings.ResourceManager, new object[]
			{
				path
			});
		}

		public static LocalizedString ShouldReRunSetupForW3SVC
		{
			get
			{
				return new LocalizedString("ShouldReRunSetupForW3SVC", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InvalidLocalServerName
		{
			get
			{
				return new LocalizedString("InvalidLocalServerName", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString VC2012PrereqMissing
		{
			get
			{
				return new LocalizedString("VC2012PrereqMissing", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString DelegatedClientAccessFirstSP1upgrade
		{
			get
			{
				return new LocalizedString("DelegatedClientAccessFirstSP1upgrade", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString CannotRemoveProvisionedServerValidator(string server)
		{
			return new LocalizedString("CannotRemoveProvisionedServerValidator", Strings.ResourceManager, new object[]
			{
				server
			});
		}

		public static LocalizedString VistaNoIPv4(string product)
		{
			return new LocalizedString("VistaNoIPv4", Strings.ResourceManager, new object[]
			{
				product
			});
		}

		public static LocalizedString Equal(string first, string second)
		{
			return new LocalizedString("Equal", Strings.ResourceManager, new object[]
			{
				first,
				second
			});
		}

		public static LocalizedString FileInUse(string mode, string process)
		{
			return new LocalizedString("FileInUse", Strings.ResourceManager, new object[]
			{
				mode,
				process
			});
		}

		public static LocalizedString UserNameError(string error)
		{
			return new LocalizedString("UserNameError", Strings.ResourceManager, new object[]
			{
				error
			});
		}

		public static LocalizedString E15E14CoexistenceMinOSReqFailureInDC(string servers)
		{
			return new LocalizedString("E15E14CoexistenceMinOSReqFailureInDC", Strings.ResourceManager, new object[]
			{
				servers
			});
		}

		public static LocalizedString TestNotRunDueToRegistryKey
		{
			get
			{
				return new LocalizedString("TestNotRunDueToRegistryKey", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString GetOrgConfigWasRunOnPremises
		{
			get
			{
				return new LocalizedString("GetOrgConfigWasRunOnPremises", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString AccessedFailedResult
		{
			get
			{
				return new LocalizedString("AccessedFailedResult", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString Iis32BitMode
		{
			get
			{
				return new LocalizedString("Iis32BitMode", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString EitherTheOnPremAcceptedDomainListOrTheDCAcceptedDomainsAreEmpty
		{
			get
			{
				return new LocalizedString("EitherTheOnPremAcceptedDomainListOrTheDCAcceptedDomainsAreEmpty", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString DCAdminDisplayVersionFound
		{
			get
			{
				return new LocalizedString("DCAdminDisplayVersionFound", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString GlobalServerInstall
		{
			get
			{
				return new LocalizedString("GlobalServerInstall", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString DomainNameExistsInAcceptedDomainAndOrgRel(string domain)
		{
			return new LocalizedString("DomainNameExistsInAcceptedDomainAndOrgRel", Strings.ResourceManager, new object[]
			{
				domain
			});
		}

		public static LocalizedString RunningTentantHybridTestWithFile(string pathToConfigFile)
		{
			return new LocalizedString("RunningTentantHybridTestWithFile", Strings.ResourceManager, new object[]
			{
				pathToConfigFile
			});
		}

		public static LocalizedString SetupLogStarted
		{
			get
			{
				return new LocalizedString("SetupLogStarted", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString HybridInfoObjectNotFound
		{
			get
			{
				return new LocalizedString("HybridInfoObjectNotFound", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString DelegatedMailboxFirstSP1upgrade
		{
			get
			{
				return new LocalizedString("DelegatedMailboxFirstSP1upgrade", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RunningTenantTestDCSettingsForHybridEnabledTenant
		{
			get
			{
				return new LocalizedString("RunningTenantTestDCSettingsForHybridEnabledTenant", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString HybridInfoCmdletStart(string session, string cmdlet, string parameters)
		{
			return new LocalizedString("HybridInfoCmdletStart", Strings.ResourceManager, new object[]
			{
				session,
				cmdlet,
				parameters
			});
		}

		public static LocalizedString TraceFunctionEnter(Type type, string methodName, string argumentList)
		{
			return new LocalizedString("TraceFunctionEnter", Strings.ResourceManager, new object[]
			{
				type,
				methodName,
				argumentList
			});
		}

		public static LocalizedString PowerShellExecutionPolicyCheck
		{
			get
			{
				return new LocalizedString("PowerShellExecutionPolicyCheck", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MailboxLogDriveDoesNotExist(string drive)
		{
			return new LocalizedString("MailboxLogDriveDoesNotExist", Strings.ResourceManager, new object[]
			{
				drive
			});
		}

		public static LocalizedString DCEndPointIsEmpty
		{
			get
			{
				return new LocalizedString("DCEndPointIsEmpty", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString OSMinVersionNotMet
		{
			get
			{
				return new LocalizedString("OSMinVersionNotMet", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString AttemptingToParseTheXmlData
		{
			get
			{
				return new LocalizedString("AttemptingToParseTheXmlData", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString TestingConfigFile(string pathToConfigFile)
		{
			return new LocalizedString("TestingConfigFile", Strings.ResourceManager, new object[]
			{
				pathToConfigFile
			});
		}

		public static LocalizedString DCPreviousAdminDisplayVersionFound
		{
			get
			{
				return new LocalizedString("DCPreviousAdminDisplayVersionFound", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString DotNetFrameworkMinVersionNotMet
		{
			get
			{
				return new LocalizedString("DotNetFrameworkMinVersionNotMet", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InterruptedUninstallNotContinued(string role)
		{
			return new LocalizedString("InterruptedUninstallNotContinued", Strings.ResourceManager, new object[]
			{
				role
			});
		}

		public static LocalizedString WinRMIISExtensionInstalled
		{
			get
			{
				return new LocalizedString("WinRMIISExtensionInstalled", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorDNSQueryA(string ipDNSName, string pName, string message)
		{
			return new LocalizedString("ErrorDNSQueryA", Strings.ResourceManager, new object[]
			{
				ipDNSName,
				pName,
				message
			});
		}

		public static LocalizedString SearchFoundationAssemblyLoaderKBNotInstalled
		{
			get
			{
				return new LocalizedString("SearchFoundationAssemblyLoaderKBNotInstalled", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString TenantIsRunningE15
		{
			get
			{
				return new LocalizedString("TenantIsRunningE15", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString Win7RpcHttpShouldRejectDuplicateConnB2PacketsUpdateNotInstalled
		{
			get
			{
				return new LocalizedString("Win7RpcHttpShouldRejectDuplicateConnB2PacketsUpdateNotInstalled", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString LonghornNoIPv4(string product)
		{
			return new LocalizedString("LonghornNoIPv4", Strings.ResourceManager, new object[]
			{
				product
			});
		}

		public static LocalizedString DelegatedBridgeheadFirstInstall
		{
			get
			{
				return new LocalizedString("DelegatedBridgeheadFirstInstall", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString GetOrganizationConfigCmdletNotFound
		{
			get
			{
				return new LocalizedString("GetOrganizationConfigCmdletNotFound", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString TheFilePassedInIsNotFromGetOrganizationConfig
		{
			get
			{
				return new LocalizedString("TheFilePassedInIsNotFromGetOrganizationConfig", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString NotSupportedRecipientPolicyAddressFormatValidator(string name, string address)
		{
			return new LocalizedString("NotSupportedRecipientPolicyAddressFormatValidator", Strings.ResourceManager, new object[]
			{
				name,
				address
			});
		}

		public static LocalizedString W3SVCDisabledOrNotInstalled
		{
			get
			{
				return new LocalizedString("W3SVCDisabledOrNotInstalled", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString HostingModeNotAvailable
		{
			get
			{
				return new LocalizedString("HostingModeNotAvailable", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString LangPackInstalled
		{
			get
			{
				return new LocalizedString("LangPackInstalled", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InsufficientPrivledges
		{
			get
			{
				return new LocalizedString("InsufficientPrivledges", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString BadFilePassedIn
		{
			get
			{
				return new LocalizedString("BadFilePassedIn", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString OSCheckedBuild
		{
			get
			{
				return new LocalizedString("OSCheckedBuild", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString NetBIOSNameNotMatchesDNSHostName
		{
			get
			{
				return new LocalizedString("NetBIOSNameNotMatchesDNSHostName", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString NoGCInSite
		{
			get
			{
				return new LocalizedString("NoGCInSite", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InflateAndDecodeReturnedDataFromGetOrgConfig
		{
			get
			{
				return new LocalizedString("InflateAndDecodeReturnedDataFromGetOrgConfig", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString LocalDomainNotPrepared
		{
			get
			{
				return new LocalizedString("LocalDomainNotPrepared", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString Exchange2000or2003PresentInOrg
		{
			get
			{
				return new LocalizedString("Exchange2000or2003PresentInOrg", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InconsistentlyConfiguredDomain(string name)
		{
			return new LocalizedString("InconsistentlyConfiguredDomain", Strings.ResourceManager, new object[]
			{
				name
			});
		}

		public static LocalizedString Exchange2013AnyOnExchange2007or2010Server
		{
			get
			{
				return new LocalizedString("Exchange2013AnyOnExchange2007or2010Server", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString PrereqAnalysisSettingStarted(string memberName, string parentName, string valueType)
		{
			return new LocalizedString("PrereqAnalysisSettingStarted", Strings.ResourceManager, new object[]
			{
				memberName,
				parentName,
				valueType
			});
		}

		public static LocalizedString RunningTenantHybridTest
		{
			get
			{
				return new LocalizedString("RunningTenantHybridTest", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString DCIsDataCenterBitFound
		{
			get
			{
				return new LocalizedString("DCIsDataCenterBitFound", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ClientAccessRoleNotPresentInSite(string adSite)
		{
			return new LocalizedString("ClientAccessRoleNotPresentInSite", Strings.ResourceManager, new object[]
			{
				adSite
			});
		}

		public static LocalizedString PrereqAnalysisNullValue
		{
			get
			{
				return new LocalizedString("PrereqAnalysisNullValue", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MinVersionCheck
		{
			get
			{
				return new LocalizedString("MinVersionCheck", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ThereWasAnExceptionWhileCheckingForHybridConfiguration
		{
			get
			{
				return new LocalizedString("ThereWasAnExceptionWhileCheckingForHybridConfiguration", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ConnectingToTheDCToRunGetOrgConfig
		{
			get
			{
				return new LocalizedString("ConnectingToTheDCToRunGetOrgConfig", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString BridgeheadRoleAlreadyExists
		{
			get
			{
				return new LocalizedString("BridgeheadRoleAlreadyExists", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RegistryKeyDoesntExist(string regKey)
		{
			return new LocalizedString("RegistryKeyDoesntExist", Strings.ResourceManager, new object[]
			{
				regKey
			});
		}

		public static LocalizedString BridgeheadRoleNotPresentInSite(string adSite)
		{
			return new LocalizedString("BridgeheadRoleNotPresentInSite", Strings.ResourceManager, new object[]
			{
				adSite
			});
		}

		public static LocalizedString NoIPv4Assigned(string product)
		{
			return new LocalizedString("NoIPv4Assigned", Strings.ResourceManager, new object[]
			{
				product
			});
		}

		public static LocalizedString QueryStart(string queryName, bool forceRun)
		{
			return new LocalizedString("QueryStart", Strings.ResourceManager, new object[]
			{
				queryName,
				forceRun
			});
		}

		public static LocalizedString MSFilterPackV2SP1NotInstalled
		{
			get
			{
				return new LocalizedString("MSFilterPackV2SP1NotInstalled", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString EmptyResults
		{
			get
			{
				return new LocalizedString("EmptyResults", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ComputerRODC
		{
			get
			{
				return new LocalizedString("ComputerRODC", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RecipientUpdateServiceNotAvailable(string name)
		{
			return new LocalizedString("RecipientUpdateServiceNotAvailable", Strings.ResourceManager, new object[]
			{
				name
			});
		}

		public static LocalizedString DelegatedCafeFirstSP1upgrade
		{
			get
			{
				return new LocalizedString("DelegatedCafeFirstSP1upgrade", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString Win7RpcHttpAssocCookieGuidUpdateNotInstalled
		{
			get
			{
				return new LocalizedString("Win7RpcHttpAssocCookieGuidUpdateNotInstalled", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString AccessedValueWhenMultipleResults
		{
			get
			{
				return new LocalizedString("AccessedValueWhenMultipleResults", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ValidationFailed(string name)
		{
			return new LocalizedString("ValidationFailed", Strings.ResourceManager, new object[]
			{
				name
			});
		}

		public static LocalizedString UpgradeGateway605Block
		{
			get
			{
				return new LocalizedString("UpgradeGateway605Block", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString NotLoggedOntoDomain
		{
			get
			{
				return new LocalizedString("NotLoggedOntoDomain", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString DataReturnedFromDCIsInvalid
		{
			get
			{
				return new LocalizedString("DataReturnedFromDCIsInvalid", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString DelegatedFirstSP1Upgrade(string roleName)
		{
			return new LocalizedString("DelegatedFirstSP1Upgrade", Strings.ResourceManager, new object[]
			{
				roleName
			});
		}

		public static LocalizedString ValueNotFoundInCollection
		{
			get
			{
				return new LocalizedString("ValueNotFoundInCollection", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorTooManyMatchingResults(string identity)
		{
			return new LocalizedString("ErrorTooManyMatchingResults", Strings.ResourceManager, new object[]
			{
				identity
			});
		}

		public static LocalizedString InstallViaServerManager(string component)
		{
			return new LocalizedString("InstallViaServerManager", Strings.ResourceManager, new object[]
			{
				component
			});
		}

		public static LocalizedString ServerIsDynamicGroupExpansionServer(int count)
		{
			return new LocalizedString("ServerIsDynamicGroupExpansionServer", Strings.ResourceManager, new object[]
			{
				count
			});
		}

		public static LocalizedString NoIPv4AssignedForAdminTools(string product)
		{
			return new LocalizedString("NoIPv4AssignedForAdminTools", Strings.ResourceManager, new object[]
			{
				product
			});
		}

		public static LocalizedString NoPreviousExchangeExistsInTopoWhilePrepareAD(string product)
		{
			return new LocalizedString("NoPreviousExchangeExistsInTopoWhilePrepareAD", Strings.ResourceManager, new object[]
			{
				product
			});
		}

		public static LocalizedString MpsSvcStopped
		{
			get
			{
				return new LocalizedString("MpsSvcStopped", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString DCAcceptedDomainNameFound
		{
			get
			{
				return new LocalizedString("DCAcceptedDomainNameFound", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString CannotAccessAD
		{
			get
			{
				return new LocalizedString("CannotAccessAD", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString DuplicateShortProvisionedName(string name)
		{
			return new LocalizedString("DuplicateShortProvisionedName", Strings.ResourceManager, new object[]
			{
				name
			});
		}

		public static LocalizedString ServerIsLastHubForEdgeSubscription(string siteName)
		{
			return new LocalizedString("ServerIsLastHubForEdgeSubscription", Strings.ResourceManager, new object[]
			{
				siteName
			});
		}

		public static LocalizedString ServerIsSourceForSendConnector(int count)
		{
			return new LocalizedString("ServerIsSourceForSendConnector", Strings.ResourceManager, new object[]
			{
				count
			});
		}

		public static LocalizedString UpdateProgressForWrongAnalysis
		{
			get
			{
				return new LocalizedString("UpdateProgressForWrongAnalysis", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString OrgRelTargetAppUriToSearch(string uri)
		{
			return new LocalizedString("OrgRelTargetAppUriToSearch", Strings.ResourceManager, new object[]
			{
				uri
			});
		}

		public static LocalizedString ServerNotPrepared
		{
			get
			{
				return new LocalizedString("ServerNotPrepared", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString CheckingTheAcceptedDomainAgainstOrgRelationshipDomains
		{
			get
			{
				return new LocalizedString("CheckingTheAcceptedDomainAgainstOrgRelationshipDomains", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString VC2013PrereqMissing
		{
			get
			{
				return new LocalizedString("VC2013PrereqMissing", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString HttpSiteAccessError(string uri, string message)
		{
			return new LocalizedString("HttpSiteAccessError", Strings.ResourceManager, new object[]
			{
				uri,
				message
			});
		}

		public static LocalizedString DomainPrepRequired
		{
			get
			{
				return new LocalizedString("DomainPrepRequired", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString NoMatchWasFoundBetweenTheOrgRelDomainsAndDCAcceptedDomains
		{
			get
			{
				return new LocalizedString("NoMatchWasFoundBetweenTheOrgRelDomainsAndDCAcceptedDomains", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MSDTCStopped
		{
			get
			{
				return new LocalizedString("MSDTCStopped", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString CannotUninstallOABServer
		{
			get
			{
				return new LocalizedString("CannotUninstallOABServer", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString StringContains(string first, string second)
		{
			return new LocalizedString("StringContains", Strings.ResourceManager, new object[]
			{
				first,
				second
			});
		}

		public static LocalizedString TestDCSettingsForHybridEnabledTenantPassed
		{
			get
			{
				return new LocalizedString("TestDCSettingsForHybridEnabledTenantPassed", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString Win7LDRGDRRMSManifestExpiryUpdateNotInstalled
		{
			get
			{
				return new LocalizedString("Win7LDRGDRRMSManifestExpiryUpdateNotInstalled", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString NoE14ServerWarning
		{
			get
			{
				return new LocalizedString("NoE14ServerWarning", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InstallOnDCInADSplitPermissionMode
		{
			get
			{
				return new LocalizedString("InstallOnDCInADSplitPermissionMode", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString PrereqAnalysisRuleStarted(string memberName, string parentName, string ruleType)
		{
			return new LocalizedString("PrereqAnalysisRuleStarted", Strings.ResourceManager, new object[]
			{
				memberName,
				parentName,
				ruleType
			});
		}

		public static LocalizedString DelegatedBridgeheadFirstSP1upgrade
		{
			get
			{
				return new LocalizedString("DelegatedBridgeheadFirstSP1upgrade", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString CafeRoleAlreadyExists
		{
			get
			{
				return new LocalizedString("CafeRoleAlreadyExists", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString UnifiedMessagingRoleNotInstalled
		{
			get
			{
				return new LocalizedString("UnifiedMessagingRoleNotInstalled", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString CannotUninstallDelegatedServer
		{
			get
			{
				return new LocalizedString("CannotUninstallDelegatedServer", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ResultAncestorNotFound(string ancestorName)
		{
			return new LocalizedString("ResultAncestorNotFound", Strings.ResourceManager, new object[]
			{
				ancestorName
			});
		}

		public static LocalizedString MessagesInQueue(string queueNames)
		{
			return new LocalizedString("MessagesInQueue", Strings.ResourceManager, new object[]
			{
				queueNames
			});
		}

		public static LocalizedString WinRMDisabledOrNotInstalled
		{
			get
			{
				return new LocalizedString("WinRMDisabledOrNotInstalled", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString W2K8R2PrepareAdLdifdeNotInstalled
		{
			get
			{
				return new LocalizedString("W2K8R2PrepareAdLdifdeNotInstalled", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString DelegatedFrontendTransportFirstSP1upgrade
		{
			get
			{
				return new LocalizedString("DelegatedFrontendTransportFirstSP1upgrade", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString PendingReboot
		{
			get
			{
				return new LocalizedString("PendingReboot", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ScanningFailed(string name)
		{
			return new LocalizedString("ScanningFailed", Strings.ResourceManager, new object[]
			{
				name
			});
		}

		public static LocalizedString NotLocalAdmin(string currentLoggedOnUser)
		{
			return new LocalizedString("NotLocalAdmin", Strings.ResourceManager, new object[]
			{
				currentLoggedOnUser
			});
		}

		public static LocalizedString SGFilesExist(string path)
		{
			return new LocalizedString("SGFilesExist", Strings.ResourceManager, new object[]
			{
				path
			});
		}

		public static LocalizedString RootDomainMixedMode(string dn, string product)
		{
			return new LocalizedString("RootDomainMixedMode", Strings.ResourceManager, new object[]
			{
				dn,
				product
			});
		}

		public static LocalizedString LangPackDiskSpaceCheck
		{
			get
			{
				return new LocalizedString("LangPackDiskSpaceCheck", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString SetupLogEnd
		{
			get
			{
				return new LocalizedString("SetupLogEnd", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ServerRemoveProvisioningCheck(string removeServerName)
		{
			return new LocalizedString("ServerRemoveProvisioningCheck", Strings.ResourceManager, new object[]
			{
				removeServerName
			});
		}

		public static LocalizedString DelegatedUnifiedMessagingFirstInstall
		{
			get
			{
				return new LocalizedString("DelegatedUnifiedMessagingFirstInstall", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString HybridInfoTotalCmdletTime(string session, double timeSeconds)
		{
			return new LocalizedString("HybridInfoTotalCmdletTime", Strings.ResourceManager, new object[]
			{
				session,
				timeSeconds
			});
		}

		public static LocalizedString Win7WindowsIdentityFoundationUpdateNotInstalled
		{
			get
			{
				return new LocalizedString("Win7WindowsIdentityFoundationUpdateNotInstalled", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString DelegatedFirstInstall(string roleName)
		{
			return new LocalizedString("DelegatedFirstInstall", Strings.ResourceManager, new object[]
			{
				roleName
			});
		}

		public static LocalizedString E14BridgeheadRoleNotFound
		{
			get
			{
				return new LocalizedString("E14BridgeheadRoleNotFound", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString UnifiedMessagingRoleAlreadyExists
		{
			get
			{
				return new LocalizedString("UnifiedMessagingRoleAlreadyExists", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString PrereqAnalysisFailedRule(string ruleName, string message)
		{
			return new LocalizedString("PrereqAnalysisFailedRule", Strings.ResourceManager, new object[]
			{
				ruleName,
				message
			});
		}

		public static LocalizedString SetupLogInitializeFailure(string msg)
		{
			return new LocalizedString("SetupLogInitializeFailure", Strings.ResourceManager, new object[]
			{
				msg
			});
		}

		public static LocalizedString NetTcpPortSharingSvcNotAuto
		{
			get
			{
				return new LocalizedString("NetTcpPortSharingSvcNotAuto", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString HybridInfoGetObjectValue(string value, string identity, string command)
		{
			return new LocalizedString("HybridInfoGetObjectValue", Strings.ResourceManager, new object[]
			{
				value,
				identity,
				command
			});
		}

		public static LocalizedString InhBlockPublicFolderTree(string publicFolderTree, string distinguishedName)
		{
			return new LocalizedString("InhBlockPublicFolderTree", Strings.ResourceManager, new object[]
			{
				publicFolderTree,
				distinguishedName
			});
		}

		public static LocalizedString ResourcePropertySchemaException(string exception)
		{
			return new LocalizedString("ResourcePropertySchemaException", Strings.ResourceManager, new object[]
			{
				exception
			});
		}

		public static LocalizedString WindowsInstallerServiceDisabledOrNotInstalled
		{
			get
			{
				return new LocalizedString("WindowsInstallerServiceDisabledOrNotInstalled", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString SMTPSvcInstalled
		{
			get
			{
				return new LocalizedString("SMTPSvcInstalled", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString DelegatedUnifiedMessagingFirstSP1upgrade
		{
			get
			{
				return new LocalizedString("DelegatedUnifiedMessagingFirstSP1upgrade", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MsfteUpgradeIssue(string user)
		{
			return new LocalizedString("MsfteUpgradeIssue", Strings.ResourceManager, new object[]
			{
				user
			});
		}

		public static LocalizedString OnPremisesTestFailedWithException(string error)
		{
			return new LocalizedString("OnPremisesTestFailedWithException", Strings.ResourceManager, new object[]
			{
				error
			});
		}

		public static LocalizedString OldADAMInstalled
		{
			get
			{
				return new LocalizedString("OldADAMInstalled", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RunningOnPremTest
		{
			get
			{
				return new LocalizedString("RunningOnPremTest", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ServerIsGroupExpansionServer(int count)
		{
			return new LocalizedString("ServerIsGroupExpansionServer", Strings.ResourceManager, new object[]
			{
				count
			});
		}

		public static LocalizedString ProcessNeedsToBeClosedOnUpgrade(string list)
		{
			return new LocalizedString("ProcessNeedsToBeClosedOnUpgrade", Strings.ResourceManager, new object[]
			{
				list
			});
		}

		public static LocalizedString UcmaRedistMsi
		{
			get
			{
				return new LocalizedString("UcmaRedistMsi", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString EitherOrgRelOrAcceptDomainsWhereNull
		{
			get
			{
				return new LocalizedString("EitherOrgRelOrAcceptDomainsWhereNull", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ADAMSvcStopped
		{
			get
			{
				return new LocalizedString("ADAMSvcStopped", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString W2K8R2PrepareSchemaLdifdeNotInstalled
		{
			get
			{
				return new LocalizedString("W2K8R2PrepareSchemaLdifdeNotInstalled", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString LocalComputerIsDCInChildDomain
		{
			get
			{
				return new LocalizedString("LocalComputerIsDCInChildDomain", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ClusSvcInstalledRoleBlock
		{
			get
			{
				return new LocalizedString("ClusSvcInstalledRoleBlock", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ComponentIsRequiredToInstall(string name)
		{
			return new LocalizedString("ComponentIsRequiredToInstall", Strings.ResourceManager, new object[]
			{
				name
			});
		}

		public static LocalizedString HybridInfoCmdletEnd(string session, string cmdlet, string elapsed)
		{
			return new LocalizedString("HybridInfoCmdletEnd", Strings.ResourceManager, new object[]
			{
				session,
				cmdlet,
				elapsed
			});
		}

		public static LocalizedString ExchangeAlreadyInstalled(string name)
		{
			return new LocalizedString("ExchangeAlreadyInstalled", Strings.ResourceManager, new object[]
			{
				name
			});
		}

		public static LocalizedString PrereqAnalysisStarted
		{
			get
			{
				return new LocalizedString("PrereqAnalysisStarted", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString BridgeheadRoleNotInstalled
		{
			get
			{
				return new LocalizedString("BridgeheadRoleNotInstalled", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString OrgConfigDataIsEmptyOrNull
		{
			get
			{
				return new LocalizedString("OrgConfigDataIsEmptyOrNull", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString EdgeSubscriptionExists
		{
			get
			{
				return new LocalizedString("EdgeSubscriptionExists", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString QueryFinish(string queryName, string result)
		{
			return new LocalizedString("QueryFinish", Strings.ResourceManager, new object[]
			{
				queryName,
				result
			});
		}

		public static LocalizedString InvalidOrTamperedFile
		{
			get
			{
				return new LocalizedString("InvalidOrTamperedFile", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString SendConnectorException
		{
			get
			{
				return new LocalizedString("SendConnectorException", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString FqdnMissing
		{
			get
			{
				return new LocalizedString("FqdnMissing", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString SchemaNotPreparedExtendedRights
		{
			get
			{
				return new LocalizedString("SchemaNotPreparedExtendedRights", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString LangPackBundleCheck
		{
			get
			{
				return new LocalizedString("LangPackBundleCheck", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString FailedResult
		{
			get
			{
				return new LocalizedString("FailedResult", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ComponentIsRequired(string componentName)
		{
			return new LocalizedString("ComponentIsRequired", Strings.ResourceManager, new object[]
			{
				componentName
			});
		}

		public static LocalizedString Win2k12RefsUpdateNotInstalled
		{
			get
			{
				return new LocalizedString("Win2k12RefsUpdateNotInstalled", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString CannotAccessHttpSiteForEngineUpdates
		{
			get
			{
				return new LocalizedString("CannotAccessHttpSiteForEngineUpdates", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString SpeechRedistMsi
		{
			get
			{
				return new LocalizedString("SpeechRedistMsi", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString PrereqAnalysisStopped(TimeSpan duration)
		{
			return new LocalizedString("PrereqAnalysisStopped", Strings.ResourceManager, new object[]
			{
				duration
			});
		}

		public static LocalizedString InvalidLocalComputerFQDN(string name)
		{
			return new LocalizedString("InvalidLocalComputerFQDN", Strings.ResourceManager, new object[]
			{
				name
			});
		}

		public static LocalizedString PendingRebootWindowsComponents
		{
			get
			{
				return new LocalizedString("PendingRebootWindowsComponents", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ComponentIsRecommended(string componentName)
		{
			return new LocalizedString("ComponentIsRecommended", Strings.ResourceManager, new object[]
			{
				componentName
			});
		}

		public static LocalizedString OSMinVersionForFSMONotMet
		{
			get
			{
				return new LocalizedString("OSMinVersionForFSMONotMet", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString Exchange2007ServerInstalled(string server)
		{
			return new LocalizedString("Exchange2007ServerInstalled", Strings.ResourceManager, new object[]
			{
				server
			});
		}

		public static LocalizedString UserName(string userName)
		{
			return new LocalizedString("UserName", Strings.ResourceManager, new object[]
			{
				userName
			});
		}

		public static LocalizedString MissingDNSHostRecord(string dns)
		{
			return new LocalizedString("MissingDNSHostRecord", Strings.ResourceManager, new object[]
			{
				dns
			});
		}

		public static LocalizedString AdditionalUMLangPackExists(string languagePack)
		{
			return new LocalizedString("AdditionalUMLangPackExists", Strings.ResourceManager, new object[]
			{
				languagePack
			});
		}

		public static LocalizedString InstallWatermark(string role)
		{
			return new LocalizedString("InstallWatermark", Strings.ResourceManager, new object[]
			{
				role
			});
		}

		public static LocalizedString TargetPathCompressed(string targetDir)
		{
			return new LocalizedString("TargetPathCompressed", Strings.ResourceManager, new object[]
			{
				targetDir
			});
		}

		public static LocalizedString InvalidDNSDomainName
		{
			get
			{
				return new LocalizedString("InvalidDNSDomainName", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString UMLangPackDiskSpaceCheck
		{
			get
			{
				return new LocalizedString("UMLangPackDiskSpaceCheck", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString PrereqAnalysisMemberStopped(string memberType, string memberName, TimeSpan duration)
		{
			return new LocalizedString("PrereqAnalysisMemberStopped", Strings.ResourceManager, new object[]
			{
				memberType,
				memberName,
				duration
			});
		}

		public static LocalizedString PrepareDomainNotAdmin(string prepareDomain)
		{
			return new LocalizedString("PrepareDomainNotAdmin", Strings.ResourceManager, new object[]
			{
				prepareDomain
			});
		}

		public static LocalizedString ServerNotInSchemaMasterSite(string name)
		{
			return new LocalizedString("ServerNotInSchemaMasterSite", Strings.ResourceManager, new object[]
			{
				name
			});
		}

		public static LocalizedString Win7LDRRMSManifestExpiryUpdateNotInstalled
		{
			get
			{
				return new LocalizedString("Win7LDRRMSManifestExpiryUpdateNotInstalled", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString TenantIsBeingUpgradedFromE14
		{
			get
			{
				return new LocalizedString("TenantIsBeingUpgradedFromE14", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InvalidADSite
		{
			get
			{
				return new LocalizedString("InvalidADSite", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ShouldBeNullOrEmpty(string name)
		{
			return new LocalizedString("ShouldBeNullOrEmpty", Strings.ResourceManager, new object[]
			{
				name
			});
		}

		public static LocalizedString E15E12CoexistenceMinOSReqFailure(string servers)
		{
			return new LocalizedString("E15E12CoexistenceMinOSReqFailure", Strings.ResourceManager, new object[]
			{
				servers
			});
		}

		public static LocalizedString RemoteRegException
		{
			get
			{
				return new LocalizedString("RemoteRegException", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ADNotPreparedForHostingValidator
		{
			get
			{
				return new LocalizedString("ADNotPreparedForHostingValidator", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString AlreadyInstalledUMLangPacks(string cultures)
		{
			return new LocalizedString("AlreadyInstalledUMLangPacks", Strings.ResourceManager, new object[]
			{
				cultures
			});
		}

		public static LocalizedString StringNotContains(string first, string second)
		{
			return new LocalizedString("StringNotContains", Strings.ResourceManager, new object[]
			{
				first,
				second
			});
		}

		public static LocalizedString Win2k12RollupUpdateNotInstalled
		{
			get
			{
				return new LocalizedString("Win2k12RollupUpdateNotInstalled", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString NotEqual(string first, string second)
		{
			return new LocalizedString("NotEqual", Strings.ResourceManager, new object[]
			{
				first,
				second
			});
		}

		public static LocalizedString InvalidOSVersion
		{
			get
			{
				return new LocalizedString("InvalidOSVersion", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString StringNotStartsWith(string first, string second)
		{
			return new LocalizedString("StringNotStartsWith", Strings.ResourceManager, new object[]
			{
				first,
				second
			});
		}

		public static LocalizedString ServerNotInSchemaMasterDomain(string name)
		{
			return new LocalizedString("ServerNotInSchemaMasterDomain", Strings.ResourceManager, new object[]
			{
				name
			});
		}

		public static LocalizedString TraceFunctionExit(Type type, string methodName)
		{
			return new LocalizedString("TraceFunctionExit", Strings.ResourceManager, new object[]
			{
				type,
				methodName
			});
		}

		public static LocalizedString MinimumFrameworkNotInstalled
		{
			get
			{
				return new LocalizedString("MinimumFrameworkNotInstalled", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString SchemaNotPrepared
		{
			get
			{
				return new LocalizedString("SchemaNotPrepared", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString HostingActiveDirectorySplitPermissionsNotSupported
		{
			get
			{
				return new LocalizedString("HostingActiveDirectorySplitPermissionsNotSupported", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString DisplayVersionDCBitUpgradeStatusBitAndVersionAreCorrect
		{
			get
			{
				return new LocalizedString("DisplayVersionDCBitUpgradeStatusBitAndVersionAreCorrect", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ADAMPortAlreadyInUse(string adamPort)
		{
			return new LocalizedString("ADAMPortAlreadyInUse", Strings.ResourceManager, new object[]
			{
				adamPort
			});
		}

		public static LocalizedString UpgradeMinVersionBlock
		{
			get
			{
				return new LocalizedString("UpgradeMinVersionBlock", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InvalidOSVersionForAdminTools
		{
			get
			{
				return new LocalizedString("InvalidOSVersionForAdminTools", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString AllServersOfHigherVersionFailure(string servers)
		{
			return new LocalizedString("AllServersOfHigherVersionFailure", Strings.ResourceManager, new object[]
			{
				servers
			});
		}

		public static LocalizedString ConditionIsFalse
		{
			get
			{
				return new LocalizedString("ConditionIsFalse", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString OrgConfigHashDoesNotExist
		{
			get
			{
				return new LocalizedString("OrgConfigHashDoesNotExist", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString HelpIdNotDefined(string ruleName)
		{
			return new LocalizedString("HelpIdNotDefined", Strings.ResourceManager, new object[]
			{
				ruleName
			});
		}

		public static LocalizedString ProvisionedUpdateRequired
		{
			get
			{
				return new LocalizedString("ProvisionedUpdateRequired", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ComputerNotPartofDomain
		{
			get
			{
				return new LocalizedString("ComputerNotPartofDomain", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString StringStartsWith(string first, string second)
		{
			return new LocalizedString("StringStartsWith", Strings.ResourceManager, new object[]
			{
				first,
				second
			});
		}

		public static LocalizedString Win2k12UrefsUpdateNotInstalled
		{
			get
			{
				return new LocalizedString("Win2k12UrefsUpdateNotInstalled", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString CannotReturnNullForResult
		{
			get
			{
				return new LocalizedString("CannotReturnNullForResult", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString SetADSplitPermissionWhenExchangeServerRolesOnDC
		{
			get
			{
				return new LocalizedString("SetADSplitPermissionWhenExchangeServerRolesOnDC", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InflateAndDecoding
		{
			get
			{
				return new LocalizedString("InflateAndDecoding", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString OffLineABServerDeleted(string oabName)
		{
			return new LocalizedString("OffLineABServerDeleted", Strings.ResourceManager, new object[]
			{
				oabName
			});
		}

		public static LocalizedString ADAMLonghornWin7ServerNotInstalled
		{
			get
			{
				return new LocalizedString("ADAMLonghornWin7ServerNotInstalled", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString DCIsUpgradingOrganizationFound
		{
			get
			{
				return new LocalizedString("DCIsUpgradingOrganizationFound", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MailboxRoleNotInstalled
		{
			get
			{
				return new LocalizedString("MailboxRoleNotInstalled", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString PrereqAnalysisMemberEvaluated(string memberType, string memberName, bool hasException, string value, string parentValue, int threadId, TimeSpan duration)
		{
			return new LocalizedString("PrereqAnalysisMemberEvaluated", Strings.ResourceManager, new object[]
			{
				memberType,
				memberName,
				hasException,
				value,
				parentValue,
				threadId,
				duration
			});
		}

		public static LocalizedString PrereqAnalysisParentExceptionValue
		{
			get
			{
				return new LocalizedString("PrereqAnalysisParentExceptionValue", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString TenantHasNotYetBeenUpgradedToE15
		{
			get
			{
				return new LocalizedString("TenantHasNotYetBeenUpgradedToE15", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString OSWebEditionValidator(string product)
		{
			return new LocalizedString("OSWebEditionValidator", Strings.ResourceManager, new object[]
			{
				product
			});
		}

		public static LocalizedString DotNetFrameworkNeedsUpdate
		{
			get
			{
				return new LocalizedString("DotNetFrameworkNeedsUpdate", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString NotLessThan(string first, string second)
		{
			return new LocalizedString("NotLessThan", Strings.ResourceManager, new object[]
			{
				first,
				second
			});
		}

		public static LocalizedString ADAMSSLPortAlreadyInUse(string adamSSLPort)
		{
			return new LocalizedString("ADAMSSLPortAlreadyInUse", Strings.ResourceManager, new object[]
			{
				adamSSLPort
			});
		}

		public static LocalizedString OSMinVersionForAdminToolsNotMet
		{
			get
			{
				return new LocalizedString("OSMinVersionForAdminToolsNotMet", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InvalidRecordType(string recordType)
		{
			return new LocalizedString("InvalidRecordType", Strings.ResourceManager, new object[]
			{
				recordType
			});
		}

		public static LocalizedString NullLoggerHasBeenPassedIn
		{
			get
			{
				return new LocalizedString("NullLoggerHasBeenPassedIn", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString PrereqAnalysisExpectedFailure(string memberName, string message)
		{
			return new LocalizedString("PrereqAnalysisExpectedFailure", Strings.ResourceManager, new object[]
			{
				memberName,
				message
			});
		}

		public static LocalizedString HybridInfoPurePSObjectsNotSupported
		{
			get
			{
				return new LocalizedString("HybridInfoPurePSObjectsNotSupported", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString LangPackBundleVersioning
		{
			get
			{
				return new LocalizedString("LangPackBundleVersioning", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString DomainMixedMode(string dn, string product)
		{
			return new LocalizedString("DomainMixedMode", Strings.ResourceManager, new object[]
			{
				dn,
				product
			});
		}

		public static LocalizedString DomainControllerOutOfSiteValidator(string adSiteName, string dcSiteName, string dc)
		{
			return new LocalizedString("DomainControllerOutOfSiteValidator", Strings.ResourceManager, new object[]
			{
				adSiteName,
				dcSiteName,
				dc
			});
		}

		public static LocalizedString PrepareDomainModeMixed(string ncName)
		{
			return new LocalizedString("PrepareDomainModeMixed", Strings.ResourceManager, new object[]
			{
				ncName
			});
		}

		public static LocalizedString SearchingForAttributes
		{
			get
			{
				return new LocalizedString("SearchingForAttributes", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString LocalDomainMixedMode(string name, string product)
		{
			return new LocalizedString("LocalDomainMixedMode", Strings.ResourceManager, new object[]
			{
				name,
				product
			});
		}

		public static LocalizedString UnwillingToRemoveMailboxDatabase(string exception)
		{
			return new LocalizedString("UnwillingToRemoveMailboxDatabase", Strings.ResourceManager, new object[]
			{
				exception
			});
		}

		public static LocalizedString ADAMDataPathExists(string adamDataPath)
		{
			return new LocalizedString("ADAMDataPathExists", Strings.ResourceManager, new object[]
			{
				adamDataPath
			});
		}

		public static LocalizedString GetLocalizedString(Strings.IDs key)
		{
			return new LocalizedString(Strings.stringIDs[(uint)key], Strings.ResourceManager, new object[0]);
		}

		private static Dictionary<uint, string> stringIDs = new Dictionary<uint, string>(176);

		private static ExchangeResourceManager ResourceManager = ExchangeResourceManager.GetResourceManager("Microsoft.Exchange.Management.Deployment.Strings", typeof(Strings).GetTypeInfo().Assembly);

		public enum IDs : uint
		{
			ExchangeVersionBlock = 3076655694U,
			InflatingAndDecodingPassedInHash = 2041384979U,
			MSFilterPackV2NotInstalled = 3961637084U,
			MailboxRoleAlreadyExists = 286971094U,
			TooManyResults = 3853989365U,
			EventSystemStopped = 3498036640U,
			WindowsServer2008CoreServerEdition = 971944032U,
			DelegatedClientAccessFirstInstall = 2386277257U,
			NoConnectorToStar = 564094161U,
			LangPackUpgradeVersioning = 1256831615U,
			PreviousVersionOfExchangeAlreadyInstalled = 2177637643U,
			CannotUninstallClusterNode = 3383574593U,
			TestDCSettingsForHybridEnabledTenantFailed = 173856461U,
			NoE12ServerWarning = 975440992U,
			DelegatedCafeFirstInstall = 2186576589U,
			ParsingXmlDataFromDCFileOrDCCmdlet = 4277708119U,
			InvalidPSCredential = 3412170869U,
			FoundOrgConfigHashInConfigFile = 233623067U,
			PSCredentialAndTheOrganizationHashIsNull = 1884243248U,
			ClientAccessRoleAlreadyExists = 1760005143U,
			AdcFound = 2779888852U,
			ForestLevelNotWin2003Native = 1542399594U,
			InstallExchangeRolesOnDomainController = 2107927772U,
			ServerFQDNDisplayName = 1889541029U,
			MemberOfDatabaseAvailabilityGroup = 2033421306U,
			DelegatedMailboxFirstInstall = 2446058696U,
			CannotSetADSplitPermissionValidator = 3207300105U,
			FrontendTransportRoleAlreadyExists = 168558763U,
			GatewayUpgrade605Block = 4109056594U,
			ShouldReRunSetupForW3SVC = 3696305017U,
			InvalidLocalServerName = 2679933748U,
			VC2012PrereqMissing = 3945170485U,
			DelegatedClientAccessFirstSP1upgrade = 3419466150U,
			TestNotRunDueToRegistryKey = 3480633415U,
			GetOrgConfigWasRunOnPremises = 782041369U,
			AccessedFailedResult = 1141647571U,
			Iis32BitMode = 3552416860U,
			EitherTheOnPremAcceptedDomainListOrTheDCAcceptedDomainsAreEmpty = 719584928U,
			DCAdminDisplayVersionFound = 3601945308U,
			GlobalServerInstall = 1667305647U,
			SetupLogStarted = 3878879664U,
			HybridInfoObjectNotFound = 3187185748U,
			DelegatedMailboxFirstSP1upgrade = 1950741283U,
			RunningTenantTestDCSettingsForHybridEnabledTenant = 2076519121U,
			PowerShellExecutionPolicyCheck = 2999486005U,
			DCEndPointIsEmpty = 1793580687U,
			OSMinVersionNotMet = 3615196321U,
			AttemptingToParseTheXmlData = 704357583U,
			DCPreviousAdminDisplayVersionFound = 2759777131U,
			DotNetFrameworkMinVersionNotMet = 601266997U,
			WinRMIISExtensionInstalled = 3940011457U,
			SearchFoundationAssemblyLoaderKBNotInstalled = 1727977492U,
			TenantIsRunningE15 = 841751018U,
			Win7RpcHttpShouldRejectDuplicateConnB2PacketsUpdateNotInstalled = 541611516U,
			DelegatedBridgeheadFirstInstall = 219074271U,
			GetOrganizationConfigCmdletNotFound = 3254033387U,
			TheFilePassedInIsNotFromGetOrganizationConfig = 3375133662U,
			W3SVCDisabledOrNotInstalled = 3667045786U,
			HostingModeNotAvailable = 1853686419U,
			LangPackInstalled = 1062077161U,
			InsufficientPrivledges = 3068577726U,
			BadFilePassedIn = 1587969884U,
			OSCheckedBuild = 3070759373U,
			NetBIOSNameNotMatchesDNSHostName = 3443808673U,
			NoGCInSite = 1073237881U,
			InflateAndDecodeReturnedDataFromGetOrgConfig = 2097390153U,
			LocalDomainNotPrepared = 4278863281U,
			Exchange2000or2003PresentInOrg = 1570727447U,
			Exchange2013AnyOnExchange2007or2010Server = 4143233965U,
			RunningTenantHybridTest = 4199205241U,
			DCIsDataCenterBitFound = 1023311823U,
			PrereqAnalysisNullValue = 24167743U,
			MinVersionCheck = 3736002086U,
			ThereWasAnExceptionWhileCheckingForHybridConfiguration = 2170309289U,
			ConnectingToTheDCToRunGetOrgConfig = 715361555U,
			BridgeheadRoleAlreadyExists = 3133604821U,
			MSFilterPackV2SP1NotInstalled = 1976971220U,
			EmptyResults = 4164286807U,
			ComputerRODC = 1626517757U,
			DelegatedCafeFirstSP1upgrade = 1186202058U,
			Win7RpcHttpAssocCookieGuidUpdateNotInstalled = 3145781320U,
			AccessedValueWhenMultipleResults = 1376602368U,
			UpgradeGateway605Block = 1828122210U,
			NotLoggedOntoDomain = 3401172205U,
			DataReturnedFromDCIsInvalid = 4130092005U,
			ValueNotFoundInCollection = 1040666117U,
			MpsSvcStopped = 841278233U,
			DCAcceptedDomainNameFound = 1554849195U,
			CannotAccessAD = 641150786U,
			UpdateProgressForWrongAnalysis = 1752859078U,
			ServerNotPrepared = 53953329U,
			CheckingTheAcceptedDomainAgainstOrgRelationshipDomains = 2931842358U,
			VC2013PrereqMissing = 1081610576U,
			DomainPrepRequired = 1463453412U,
			NoMatchWasFoundBetweenTheOrgRelDomainsAndDCAcceptedDomains = 4229980062U,
			MSDTCStopped = 3125428864U,
			CannotUninstallOABServer = 4101630194U,
			TestDCSettingsForHybridEnabledTenantPassed = 2459322720U,
			Win7LDRGDRRMSManifestExpiryUpdateNotInstalled = 1194462944U,
			NoE14ServerWarning = 3696227166U,
			InstallOnDCInADSplitPermissionMode = 3265622739U,
			DelegatedBridgeheadFirstSP1upgrade = 2337843968U,
			CafeRoleAlreadyExists = 2879980483U,
			UnifiedMessagingRoleNotInstalled = 3469704679U,
			CannotUninstallDelegatedServer = 2121009437U,
			WinRMDisabledOrNotInstalled = 3379016135U,
			W2K8R2PrepareAdLdifdeNotInstalled = 3219123481U,
			DelegatedFrontendTransportFirstSP1upgrade = 4056098580U,
			PendingReboot = 3957588404U,
			LangPackDiskSpaceCheck = 3440414664U,
			SetupLogEnd = 4195359116U,
			DelegatedUnifiedMessagingFirstInstall = 1829824776U,
			Win7WindowsIdentityFoundationUpdateNotInstalled = 569737677U,
			E14BridgeheadRoleNotFound = 43746744U,
			UnifiedMessagingRoleAlreadyExists = 2882191588U,
			NetTcpPortSharingSvcNotAuto = 3915669625U,
			WindowsInstallerServiceDisabledOrNotInstalled = 4251553116U,
			SMTPSvcInstalled = 3733193906U,
			DelegatedUnifiedMessagingFirstSP1upgrade = 2364459045U,
			OldADAMInstalled = 392652342U,
			RunningOnPremTest = 4254576536U,
			UcmaRedistMsi = 3978292472U,
			EitherOrgRelOrAcceptDomainsWhereNull = 3827371534U,
			ADAMSvcStopped = 2570206190U,
			W2K8R2PrepareSchemaLdifdeNotInstalled = 212290597U,
			LocalComputerIsDCInChildDomain = 2160575392U,
			ClusSvcInstalledRoleBlock = 275692458U,
			PrereqAnalysisStarted = 1958335472U,
			BridgeheadRoleNotInstalled = 2120465116U,
			OrgConfigDataIsEmptyOrNull = 2369286853U,
			EdgeSubscriptionExists = 1550596250U,
			InvalidOrTamperedFile = 1584908934U,
			SendConnectorException = 160644360U,
			FqdnMissing = 1212051581U,
			SchemaNotPreparedExtendedRights = 1980036245U,
			LangPackBundleCheck = 1622330535U,
			FailedResult = 2543736554U,
			Win2k12RefsUpdateNotInstalled = 2361234894U,
			CannotAccessHttpSiteForEngineUpdates = 3990514141U,
			SpeechRedistMsi = 1332821968U,
			PendingRebootWindowsComponents = 1132011277U,
			OSMinVersionForFSMONotMet = 87247761U,
			InvalidDNSDomainName = 2387324271U,
			UMLangPackDiskSpaceCheck = 1254031586U,
			Win7LDRRMSManifestExpiryUpdateNotInstalled = 3272988597U,
			TenantIsBeingUpgradedFromE14 = 2902280231U,
			InvalidADSite = 1412296371U,
			RemoteRegException = 3588122483U,
			ADNotPreparedForHostingValidator = 1276355988U,
			Win2k12RollupUpdateNotInstalled = 4039112198U,
			InvalidOSVersion = 2380070307U,
			MinimumFrameworkNotInstalled = 2017435239U,
			SchemaNotPrepared = 3014916009U,
			HostingActiveDirectorySplitPermissionsNotSupported = 2501606560U,
			DisplayVersionDCBitUpgradeStatusBitAndVersionAreCorrect = 2053317018U,
			UpgradeMinVersionBlock = 624846257U,
			InvalidOSVersionForAdminTools = 2938058350U,
			ConditionIsFalse = 3831181830U,
			OrgConfigHashDoesNotExist = 346774553U,
			ProvisionedUpdateRequired = 2890177564U,
			ComputerNotPartofDomain = 1981068904U,
			Win2k12UrefsUpdateNotInstalled = 985137777U,
			CannotReturnNullForResult = 260127048U,
			SetADSplitPermissionWhenExchangeServerRolesOnDC = 2452722357U,
			InflateAndDecoding = 3769154951U,
			ADAMLonghornWin7ServerNotInstalled = 2393957831U,
			DCIsUpgradingOrganizationFound = 2715471913U,
			MailboxRoleNotInstalled = 3301968605U,
			PrereqAnalysisParentExceptionValue = 3344709799U,
			TenantHasNotYetBeenUpgradedToE15 = 1535334837U,
			DotNetFrameworkNeedsUpdate = 758899952U,
			OSMinVersionForAdminToolsNotMet = 1799611416U,
			NullLoggerHasBeenPassedIn = 3629514284U,
			HybridInfoPurePSObjectsNotSupported = 888551196U,
			LangPackBundleVersioning = 3204362251U,
			SearchingForAttributes = 3457078824U
		}

		private enum ParamIDs
		{
			LessThan,
			OnPremisesOrgRelationshipDomainsCrossWithAcceptedDomainReturnResult,
			QueryReturnedNull,
			ShouldNotBeNullOrEmpty,
			ServicesAreMarkedForDeletion,
			AssemblyVersion,
			PrimaryDNSTestFailed,
			DRMinVersionNotMet,
			ADUpdateForDomainPrep,
			ProcessNeedsToBeClosedOnUninstall,
			InvalidDomainToPrepare,
			ErrorWhileRunning,
			RegistryKeyNotFound,
			ConfigDCHostNameMismatch,
			LocalTimeZone,
			HybridInfoOpeningRunspace,
			E15E14CoexistenceMinOSReqFailure,
			PrereqAnalysisFailureToAccessResults,
			InvalidOrTamperedConfigFile,
			MailboxEDBDriveDoesNotExist,
			OSVersion,
			LonghornIISManagementConsoleInstalledValidator,
			WatermarkPresent,
			LonghornIISMetabaseNotInstalled,
			VoiceMessagesInQueue,
			CannotRemoveProvisionedServerValidator,
			VistaNoIPv4,
			Equal,
			FileInUse,
			UserNameError,
			E15E14CoexistenceMinOSReqFailureInDC,
			DomainNameExistsInAcceptedDomainAndOrgRel,
			RunningTentantHybridTestWithFile,
			HybridInfoCmdletStart,
			TraceFunctionEnter,
			MailboxLogDriveDoesNotExist,
			TestingConfigFile,
			InterruptedUninstallNotContinued,
			ErrorDNSQueryA,
			LonghornNoIPv4,
			NotSupportedRecipientPolicyAddressFormatValidator,
			InconsistentlyConfiguredDomain,
			PrereqAnalysisSettingStarted,
			ClientAccessRoleNotPresentInSite,
			RegistryKeyDoesntExist,
			BridgeheadRoleNotPresentInSite,
			NoIPv4Assigned,
			QueryStart,
			RecipientUpdateServiceNotAvailable,
			ValidationFailed,
			DelegatedFirstSP1Upgrade,
			ErrorTooManyMatchingResults,
			InstallViaServerManager,
			ServerIsDynamicGroupExpansionServer,
			NoIPv4AssignedForAdminTools,
			NoPreviousExchangeExistsInTopoWhilePrepareAD,
			DuplicateShortProvisionedName,
			ServerIsLastHubForEdgeSubscription,
			ServerIsSourceForSendConnector,
			OrgRelTargetAppUriToSearch,
			HttpSiteAccessError,
			StringContains,
			PrereqAnalysisRuleStarted,
			ResultAncestorNotFound,
			MessagesInQueue,
			ScanningFailed,
			NotLocalAdmin,
			SGFilesExist,
			RootDomainMixedMode,
			ServerRemoveProvisioningCheck,
			HybridInfoTotalCmdletTime,
			DelegatedFirstInstall,
			PrereqAnalysisFailedRule,
			SetupLogInitializeFailure,
			HybridInfoGetObjectValue,
			InhBlockPublicFolderTree,
			ResourcePropertySchemaException,
			MsfteUpgradeIssue,
			OnPremisesTestFailedWithException,
			ServerIsGroupExpansionServer,
			ProcessNeedsToBeClosedOnUpgrade,
			ComponentIsRequiredToInstall,
			HybridInfoCmdletEnd,
			ExchangeAlreadyInstalled,
			QueryFinish,
			ComponentIsRequired,
			PrereqAnalysisStopped,
			InvalidLocalComputerFQDN,
			ComponentIsRecommended,
			Exchange2007ServerInstalled,
			UserName,
			MissingDNSHostRecord,
			AdditionalUMLangPackExists,
			InstallWatermark,
			TargetPathCompressed,
			PrereqAnalysisMemberStopped,
			PrepareDomainNotAdmin,
			ServerNotInSchemaMasterSite,
			ShouldBeNullOrEmpty,
			E15E12CoexistenceMinOSReqFailure,
			AlreadyInstalledUMLangPacks,
			StringNotContains,
			NotEqual,
			StringNotStartsWith,
			ServerNotInSchemaMasterDomain,
			TraceFunctionExit,
			ADAMPortAlreadyInUse,
			AllServersOfHigherVersionFailure,
			HelpIdNotDefined,
			StringStartsWith,
			OffLineABServerDeleted,
			PrereqAnalysisMemberEvaluated,
			OSWebEditionValidator,
			NotLessThan,
			ADAMSSLPortAlreadyInUse,
			InvalidRecordType,
			PrereqAnalysisExpectedFailure,
			DomainMixedMode,
			DomainControllerOutOfSiteValidator,
			PrepareDomainModeMixed,
			LocalDomainMixedMode,
			UnwillingToRemoveMailboxDatabase,
			ADAMDataPathExists
		}
	}
}
