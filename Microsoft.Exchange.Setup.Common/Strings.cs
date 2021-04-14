using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Setup.Common
{
	internal static class Strings
	{
		static Strings()
		{
			Strings.stringIDs.Add(436210406U, "ChoosingGlobalCatalog");
			Strings.stringIDs.Add(4081297114U, "LanguagePacksUpToDate");
			Strings.stringIDs.Add(2029598378U, "PostSetupText");
			Strings.stringIDs.Add(3976870610U, "CouldNotDeserializeStateFile");
			Strings.stringIDs.Add(590640951U, "AddLanguagePacksSuccessText");
			Strings.stringIDs.Add(944044270U, "CopyLanguagePacksDisplayName");
			Strings.stringIDs.Add(800186353U, "UmLanguagePacksToRemove");
			Strings.stringIDs.Add(3870436312U, "DRServerRoleText");
			Strings.stringIDs.Add(2394569239U, "AddSuccessText");
			Strings.stringIDs.Add(4289798847U, "RemoveIntroductionText");
			Strings.stringIDs.Add(406751805U, "HasConfiguredRoles");
			Strings.stringIDs.Add(776586043U, "AddUmLanguagePackText");
			Strings.stringIDs.Add(4258824323U, "SetupRebootRequired");
			Strings.stringIDs.Add(3988419612U, "UmLanguagePackDisplayName");
			Strings.stringIDs.Add(318591995U, "UnifiedMessagingRoleIsRequiredForLanguagePackInstalls");
			Strings.stringIDs.Add(3603864464U, "FrontendTransportRoleDisplayName");
			Strings.stringIDs.Add(3211475425U, "UpgradePrereq");
			Strings.stringIDs.Add(3150196311U, "CentralAdminFrontEndRoleDisplayName");
			Strings.stringIDs.Add(1623683896U, "PreConfigurationDisplayName");
			Strings.stringIDs.Add(523317789U, "RemoveFailText");
			Strings.stringIDs.Add(1831888326U, "AddSuccessStatus");
			Strings.stringIDs.Add(1377162733U, "PickingDomainController");
			Strings.stringIDs.Add(237451456U, "UmLanguagePacksToAdd");
			Strings.stringIDs.Add(3712765806U, "LPVersioningInvalidValue");
			Strings.stringIDs.Add(1055111548U, "LanguagePacksPackagePathNotSpecified");
			Strings.stringIDs.Add(2144854482U, "AddRolesToInstall");
			Strings.stringIDs.Add(2493777864U, "AddOtherRolesError");
			Strings.stringIDs.Add(35176418U, "NeedConfigureIpv6ForSecondSubnetWhenFirstSubnetConfiguresIPV6");
			Strings.stringIDs.Add(4234837096U, "ModeErrorForDisasterRecovery");
			Strings.stringIDs.Add(3904063070U, "CannotSpecifyIndustryTypeWhenOrgIsUpToDateDuringServerInstallation");
			Strings.stringIDs.Add(2285919320U, "RemoveUmLanguagePackFailText");
			Strings.stringIDs.Add(1771160928U, "ChoosingDomainController");
			Strings.stringIDs.Add(3069688065U, "CentralAdminRoleDisplayName");
			Strings.stringIDs.Add(2768015112U, "NeedConfigureIpv4StaticAddressForSecondSubnetWhenFirstSubnetConfiguresIPV4StaticAddress");
			Strings.stringIDs.Add(1833305185U, "PreSetupText");
			Strings.stringIDs.Add(2815048144U, "ModeErrorForUpgrade");
			Strings.stringIDs.Add(261522387U, "OrganizationInstallText");
			Strings.stringIDs.Add(415144177U, "CannotSpecifyServerCEIPWhenMachineIsNotCleanDuringServerInstallation");
			Strings.stringIDs.Add(1893181714U, "LanguagePacksToInstall");
			Strings.stringIDs.Add(1778403425U, "RemoveSuccessStatus");
			Strings.stringIDs.Add(4061842413U, "UnableToFindLPVersioning");
			Strings.stringIDs.Add(1048539684U, "CannotRemoveEnglishUSLanguagePack");
			Strings.stringIDs.Add(3469958538U, "LanguagePacksToAdd");
			Strings.stringIDs.Add(3908217517U, "UpgradeRolesToInstall");
			Strings.stringIDs.Add(3473605283U, "WillSkipAMEngineDownloadCheck");
			Strings.stringIDs.Add(1841681484U, "WillGetConfiguredRolesFromRegistry");
			Strings.stringIDs.Add(3319509477U, "MailboxRoleDisplayName");
			Strings.stringIDs.Add(3566628291U, "EnglishUSLanguagePackInstalled");
			Strings.stringIDs.Add(2515305841U, "NoRoleSelectedForUninstall");
			Strings.stringIDs.Add(2876792450U, "MustEnableLegacyOutlook");
			Strings.stringIDs.Add(1610115158U, "RemoveDatacenterFileText");
			Strings.stringIDs.Add(4024296328U, "CannotSpecifyCEIPWhenOrganizationHasE14OrLaterServersDuringPrepareAD");
			Strings.stringIDs.Add(1416621619U, "OrgAlreadyHasBridgeheadServers");
			Strings.stringIDs.Add(176205814U, "PrerequisiteAnalysis");
			Strings.stringIDs.Add(1959016418U, "NeedConfigureIpv4ForSecondSubnetWhenFirstSubnetConfiguresIPV4");
			Strings.stringIDs.Add(379286002U, "RemovePreCheckText");
			Strings.stringIDs.Add(590609003U, "RemoveUmLanguagePackSuccessText");
			Strings.stringIDs.Add(1869709987U, "DeterminingOrgPrepParameters");
			Strings.stringIDs.Add(3671143606U, "RemoveProgressText");
			Strings.stringIDs.Add(1281959852U, "RemoveUmLanguagePackText");
			Strings.stringIDs.Add(2770538346U, "DRServerNotFoundInAD");
			Strings.stringIDs.Add(1120765526U, "CannotSpecifyServerCEIPWhenGlobalCEIPIsOptedOutDuringServerInstallation");
			Strings.stringIDs.Add(3535416533U, "ServerOptDescriptionText");
			Strings.stringIDs.Add(2156205750U, "InstallationPathNotSet");
			Strings.stringIDs.Add(871634395U, "UnableToFindBuildVersion");
			Strings.stringIDs.Add(3002787153U, "AdminToolsRoleDisplayName");
			Strings.stringIDs.Add(3235153535U, "UmLanguagePackPackagePathNotSpecified");
			Strings.stringIDs.Add(1136733772U, "AddCannotChangeTargetDirectoryError");
			Strings.stringIDs.Add(2335571147U, "EdgeRoleInstalledButServerObjectNotFound");
			Strings.stringIDs.Add(1016568891U, "LanguagePacksNotFound");
			Strings.stringIDs.Add(2203024520U, "ApplyingDefaultRoleSelectionState");
			Strings.stringIDs.Add(2001599932U, "MustConfigureIPv4ForFirstSubnet");
			Strings.stringIDs.Add(2161842580U, "LanguagePacksLogFilePathNotSpecified");
			Strings.stringIDs.Add(1640598683U, "OrganizationPrereqText");
			Strings.stringIDs.Add(3360917926U, "AddFailText");
			Strings.stringIDs.Add(1124277255U, "RemoveRolesToInstall");
			Strings.stringIDs.Add(968377506U, "NeedConfigureIpv4DHCPForSecondSubnetWhenFirstSubnetConfiguresIPV4DHCP");
			Strings.stringIDs.Add(1772477316U, "MidFileCopyText");
			Strings.stringIDs.Add(2913262092U, "CentralAdminDatabaseRoleDisplayName");
			Strings.stringIDs.Add(3003755610U, "AddGatewayAloneError");
			Strings.stringIDs.Add(55657320U, "CafeRoleDisplayName");
			Strings.stringIDs.Add(3106188069U, "NoServerRolesToInstall");
			Strings.stringIDs.Add(3438721327U, "DRPrereq");
			Strings.stringIDs.Add(2059503147U, "ModeErrorForFreshInstall");
			Strings.stringIDs.Add(1931019259U, "LegacyRoutingServerNotValid");
			Strings.stringIDs.Add(1405894953U, "CopyDatacenterFileText");
			Strings.stringIDs.Add(3288868438U, "UnsupportedMode");
			Strings.stringIDs.Add(1947354702U, "ExchangeOrganizationNameRequired");
			Strings.stringIDs.Add(1144432722U, "UpgradePreCheckText");
			Strings.stringIDs.Add(55662122U, "Prereqs");
			Strings.stringIDs.Add(724167955U, "FailedToReadLCIDFromRegistryError");
			Strings.stringIDs.Add(2114641061U, "LanguagePacksDisplayName");
			Strings.stringIDs.Add(3856638130U, "SetupExitsBecauseOfTransientException");
			Strings.stringIDs.Add(3783483884U, "DRSuccessText");
			Strings.stringIDs.Add(2041466059U, "DRRolesToInstall");
			Strings.stringIDs.Add(3915936117U, "ParametersForTheTaskTitle");
			Strings.stringIDs.Add(2030347010U, "ADRelatedUnknownError");
			Strings.stringIDs.Add(16560395U, "AddFailStatus");
			Strings.stringIDs.Add(1439738700U, "AddLanguagePacksFailText");
			Strings.stringIDs.Add(4069246837U, "LanguagePacksInstalledVersionNull");
			Strings.stringIDs.Add(1695043038U, "SetupExitsBecauseOfLPPathNotFoundException");
			Strings.stringIDs.Add(4175779855U, "ServerIsProvisioned");
			Strings.stringIDs.Add(3668657674U, "NoUmLanguagePackSpecified");
			Strings.stringIDs.Add(209657916U, "PostFileCopyText");
			Strings.stringIDs.Add(2127358907U, "ExecutionCompleted");
			Strings.stringIDs.Add(3830028115U, "UnknownPreviousVersion");
			Strings.stringIDs.Add(3751538685U, "RemoveFileText");
			Strings.stringIDs.Add(2949666774U, "MSIIsCurrent");
			Strings.stringIDs.Add(139720102U, "AddPrereq");
			Strings.stringIDs.Add(3212518314U, "RemoveServerRoleText");
			Strings.stringIDs.Add(1066611882U, "DoesNotSupportCEIPForAdminOnlyInstallation");
			Strings.stringIDs.Add(4291904420U, "AddLanguagePacksText");
			Strings.stringIDs.Add(155941904U, "DRPreCheckText");
			Strings.stringIDs.Add(357116433U, "AddProgressText");
			Strings.stringIDs.Add(1940285462U, "DRFailStatus");
			Strings.stringIDs.Add(1540748327U, "RemoveUmLanguagePackFailStatus");
			Strings.stringIDs.Add(2888945725U, "UpgradeIntroductionText");
			Strings.stringIDs.Add(3353325242U, "NoUmLanguagePackInstalled");
			Strings.stringIDs.Add(806279811U, "RemoveUnifiedMessagingServerDescription");
			Strings.stringIDs.Add(3229808788U, "ClientAccessRoleDisplayName");
			Strings.stringIDs.Add(2809596612U, "UpgradeFailStatus");
			Strings.stringIDs.Add(1627958870U, "NoRoleSelectedForInstall");
			Strings.stringIDs.Add(1165007882U, "ExecutionError");
			Strings.stringIDs.Add(3076164745U, "InstallationLicenseAgreementShortDescription");
			Strings.stringIDs.Add(3635986314U, "RemoveSuccessText");
			Strings.stringIDs.Add(1223959701U, "InstalledLanguageComment");
			Strings.stringIDs.Add(3791960239U, "UpgradeFailText");
			Strings.stringIDs.Add(2036054501U, "PreFileCopyText");
			Strings.stringIDs.Add(3001810281U, "UnknownDestinationPath");
			Strings.stringIDs.Add(3520674066U, "UpgradeServerRoleText");
			Strings.stringIDs.Add(2572849171U, "GlobalOptDescriptionText");
			Strings.stringIDs.Add(1835758957U, "AddServerRoleText");
			Strings.stringIDs.Add(213042017U, "MaintenanceIntroduction");
			Strings.stringIDs.Add(3361199071U, "UmLanguagePathLogFilePathNotSpecified");
			Strings.stringIDs.Add(1232809750U, "UpgradeProgressText");
			Strings.stringIDs.Add(2702857678U, "ADHasBeenPrepared");
			Strings.stringIDs.Add(783977028U, "SpecifyExchangeOrganizationName");
			Strings.stringIDs.Add(2915089885U, "UpgradeMustUseBootStrapper");
			Strings.stringIDs.Add(1655961090U, "AddIntroductionText");
			Strings.stringIDs.Add(522237759U, "UpgradeSuccessStatus");
			Strings.stringIDs.Add(48624620U, "LegacyServerNameRequired");
			Strings.stringIDs.Add(2979515445U, "TheCurrentServerHasNoExchangeBits");
			Strings.stringIDs.Add(4193588254U, "UpgradeSuccessText");
			Strings.stringIDs.Add(1134594294U, "BridgeheadRoleDisplayName");
			Strings.stringIDs.Add(966578211U, "WaitForForestPrepReplicationToLocalDomainException");
			Strings.stringIDs.Add(49080419U, "RemovePrereq");
			Strings.stringIDs.Add(915558695U, "FreshIntroductionText");
			Strings.stringIDs.Add(126744571U, "UnifiedMessagingRoleDisplayName");
			Strings.stringIDs.Add(186454365U, "LanguagePackPathException");
			Strings.stringIDs.Add(2320733697U, "GatewayRoleDisplayName");
			Strings.stringIDs.Add(3864515574U, "WillDisableAMFiltering");
			Strings.stringIDs.Add(4222861002U, "TheCurrentServerHasExchangeBits");
			Strings.stringIDs.Add(2368998453U, "MonitoringRoleDisplayName");
			Strings.stringIDs.Add(3224205240U, "OrgAlreadyHasMailboxServers");
			Strings.stringIDs.Add(3656576577U, "LanguagePackPathNotFoundError");
			Strings.stringIDs.Add(1435960826U, "MustConfigureIPv6ForFirstSubnet");
			Strings.stringIDs.Add(2370029120U, "WillRestartSetupUI");
			Strings.stringIDs.Add(3059971080U, "RemoveFailStatus");
			Strings.stringIDs.Add(2070315917U, "AddConflictedRolesError");
			Strings.stringIDs.Add(3504188790U, "SchemaMasterAvailable");
			Strings.stringIDs.Add(2214617625U, "DRFailText");
			Strings.stringIDs.Add(1095004458U, "UpgradeIntroduction");
			Strings.stringIDs.Add(1616603373U, "AddPreCheckText");
			Strings.stringIDs.Add(3338459901U, "SourceDirNotSpecifiedError");
			Strings.stringIDs.Add(3315256202U, "FreshIntroduction");
			Strings.stringIDs.Add(2192506430U, "SchemaMasterDCNotFoundException");
			Strings.stringIDs.Add(2107252035U, "ADDriverBoundToAdam");
			Strings.stringIDs.Add(1331356275U, "CabUtilityWrapperError");
			Strings.stringIDs.Add(4044548133U, "SchemaMasterNotAvailable");
			Strings.stringIDs.Add(2024338705U, "OSPRoleDisplayName");
			Strings.stringIDs.Add(2927231269U, "UpgradeLicenseAgreementShortDescription");
			Strings.stringIDs.Add(759658685U, "DRSuccessStatus");
			Strings.stringIDs.Add(2340024118U, "CopyFileText");
			Strings.stringIDs.Add(302515857U, "WillNotStartTransportService");
			Strings.stringIDs.Add(3504655211U, "RemoveMESOObjectLink");
		}

		public static LocalizedString WillSearchForAServerObjectForLocalServer(string serverName)
		{
			return new LocalizedString("WillSearchForAServerObjectForLocalServer", "Ex0DF1DD", false, true, Strings.ResourceManager, new object[]
			{
				serverName
			});
		}

		public static LocalizedString ChoosingGlobalCatalog
		{
			get
			{
				return new LocalizedString("ChoosingGlobalCatalog", "Ex49FC41", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString LanguagePacksUpToDate
		{
			get
			{
				return new LocalizedString("LanguagePacksUpToDate", "Ex3830E7", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString PostSetupText
		{
			get
			{
				return new LocalizedString("PostSetupText", "Ex0C7BE7", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString TargetDirectoryCannotBeChanged(string targetDirectory)
		{
			return new LocalizedString("TargetDirectoryCannotBeChanged", "Ex96C8A9", false, true, Strings.ResourceManager, new object[]
			{
				targetDirectory
			});
		}

		public static LocalizedString CouldNotDeserializeStateFile
		{
			get
			{
				return new LocalizedString("CouldNotDeserializeStateFile", "ExCEC759", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString AddLanguagePacksSuccessText
		{
			get
			{
				return new LocalizedString("AddLanguagePacksSuccessText", "ExCBD714", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString CopyLanguagePacksDisplayName
		{
			get
			{
				return new LocalizedString("CopyLanguagePacksDisplayName", "Ex400368", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString UmLanguagePacksToRemove
		{
			get
			{
				return new LocalizedString("UmLanguagePacksToRemove", "Ex1C4775", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString DRServerRoleText
		{
			get
			{
				return new LocalizedString("DRServerRoleText", "ExD19D7B", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString AddSuccessText
		{
			get
			{
				return new LocalizedString("AddSuccessText", "Ex07B8B7", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RemoveIntroductionText
		{
			get
			{
				return new LocalizedString("RemoveIntroductionText", "Ex66717F", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString LanguagePackPackagePath(string path)
		{
			return new LocalizedString("LanguagePackPackagePath", "ExFF7939", false, true, Strings.ResourceManager, new object[]
			{
				path
			});
		}

		public static LocalizedString HasConfiguredRoles
		{
			get
			{
				return new LocalizedString("HasConfiguredRoles", "ExB05C95", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString AddUmLanguagePackText
		{
			get
			{
				return new LocalizedString("AddUmLanguagePackText", "Ex62E3B2", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ValidatingOptionsForRoles(int count)
		{
			return new LocalizedString("ValidatingOptionsForRoles", "Ex67CC0A", false, true, Strings.ResourceManager, new object[]
			{
				count
			});
		}

		public static LocalizedString ADSchemaVersionHigherThanSetupException(int adSchemaVersion, int setupSchemaVersion)
		{
			return new LocalizedString("ADSchemaVersionHigherThanSetupException", "Ex8467E9", false, true, Strings.ResourceManager, new object[]
			{
				adSchemaVersion,
				setupSchemaVersion
			});
		}

		public static LocalizedString SetupRebootRequired
		{
			get
			{
				return new LocalizedString("SetupRebootRequired", "Ex4030F9", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString UserSpecifiedDCIsNotInLocalDomainException(string dc)
		{
			return new LocalizedString("UserSpecifiedDCIsNotInLocalDomainException", "ExB3049E", false, true, Strings.ResourceManager, new object[]
			{
				dc
			});
		}

		public static LocalizedString DeserializedStateXML(string xml)
		{
			return new LocalizedString("DeserializedStateXML", "ExD4730E", false, true, Strings.ResourceManager, new object[]
			{
				xml
			});
		}

		public static LocalizedString InstallationPathInvalidDriveTypeInformation(string path)
		{
			return new LocalizedString("InstallationPathInvalidDriveTypeInformation", "Ex779C76", false, true, Strings.ResourceManager, new object[]
			{
				path
			});
		}

		public static LocalizedString UmLanguagePackDisplayName
		{
			get
			{
				return new LocalizedString("UmLanguagePackDisplayName", "Ex69990E", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString UnifiedMessagingRoleIsRequiredForLanguagePackInstalls
		{
			get
			{
				return new LocalizedString("UnifiedMessagingRoleIsRequiredForLanguagePackInstalls", "Ex42A83B", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString SettingArgumentBecauseItIsRequired(string argument)
		{
			return new LocalizedString("SettingArgumentBecauseItIsRequired", "Ex029459", false, true, Strings.ResourceManager, new object[]
			{
				argument
			});
		}

		public static LocalizedString FrontendTransportRoleDisplayName
		{
			get
			{
				return new LocalizedString("FrontendTransportRoleDisplayName", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString UpgradePrereq
		{
			get
			{
				return new LocalizedString("UpgradePrereq", "Ex49E36B", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString CentralAdminFrontEndRoleDisplayName
		{
			get
			{
				return new LocalizedString("CentralAdminFrontEndRoleDisplayName", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString NotEnoughSpace(string requiredDiskSpace)
		{
			return new LocalizedString("NotEnoughSpace", "ExB35E2B", false, true, Strings.ResourceManager, new object[]
			{
				requiredDiskSpace
			});
		}

		public static LocalizedString RoleInstalledOnServer(string rolename)
		{
			return new LocalizedString("RoleInstalledOnServer", "ExBB1015", false, true, Strings.ResourceManager, new object[]
			{
				rolename
			});
		}

		public static LocalizedString ExchangeVersionInvalid(string serverName, string message)
		{
			return new LocalizedString("ExchangeVersionInvalid", "ExA918F8", false, true, Strings.ResourceManager, new object[]
			{
				serverName,
				message
			});
		}

		public static LocalizedString PreConfigurationDisplayName
		{
			get
			{
				return new LocalizedString("PreConfigurationDisplayName", "Ex48B6C2", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RemoveFailText
		{
			get
			{
				return new LocalizedString("RemoveFailText", "ExD96F96", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ExchangeOrganizationAlreadyExists(string orgname, string newname)
		{
			return new LocalizedString("ExchangeOrganizationAlreadyExists", "Ex2C189D", false, true, Strings.ResourceManager, new object[]
			{
				orgname,
				newname
			});
		}

		public static LocalizedString InstallationPathInvalidProfilesInformation(string path)
		{
			return new LocalizedString("InstallationPathInvalidProfilesInformation", "Ex363458", false, true, Strings.ResourceManager, new object[]
			{
				path
			});
		}

		public static LocalizedString ServerNotFound(string name)
		{
			return new LocalizedString("ServerNotFound", "Ex13669F", false, true, Strings.ResourceManager, new object[]
			{
				name
			});
		}

		public static LocalizedString AddSuccessStatus
		{
			get
			{
				return new LocalizedString("AddSuccessStatus", "Ex42E48C", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString DomainControllerChosen(string dc)
		{
			return new LocalizedString("DomainControllerChosen", "Ex3D8FF4", false, true, Strings.ResourceManager, new object[]
			{
				dc
			});
		}

		public static LocalizedString PickingDomainController
		{
			get
			{
				return new LocalizedString("PickingDomainController", "Ex3F4D04", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString UmLanguagePacksToAdd
		{
			get
			{
				return new LocalizedString("UmLanguagePacksToAdd", "Ex99C84E", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString LPVersioningInvalidValue
		{
			get
			{
				return new LocalizedString("LPVersioningInvalidValue", "Ex5AE397", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString UserSpecifiedTargetDir(string target)
		{
			return new LocalizedString("UserSpecifiedTargetDir", "ExA90987", false, true, Strings.ResourceManager, new object[]
			{
				target
			});
		}

		public static LocalizedString NotAValidFqdn(string fqdn)
		{
			return new LocalizedString("NotAValidFqdn", "", false, false, Strings.ResourceManager, new object[]
			{
				fqdn
			});
		}

		public static LocalizedString LanguagePacksPackagePathNotSpecified
		{
			get
			{
				return new LocalizedString("LanguagePacksPackagePathNotSpecified", "Ex1DA650", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString AddRolesToInstall
		{
			get
			{
				return new LocalizedString("AddRolesToInstall", "ExD97522", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString AddOtherRolesError
		{
			get
			{
				return new LocalizedString("AddOtherRolesError", "Ex128927", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString NeedConfigureIpv6ForSecondSubnetWhenFirstSubnetConfiguresIPV6
		{
			get
			{
				return new LocalizedString("NeedConfigureIpv6ForSecondSubnetWhenFirstSubnetConfiguresIPV6", "Ex9882E9", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ModeErrorForDisasterRecovery
		{
			get
			{
				return new LocalizedString("ModeErrorForDisasterRecovery", "Ex08A873", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString HighLevelTaskStarted(string task)
		{
			return new LocalizedString("HighLevelTaskStarted", "Ex4AF958", false, true, Strings.ResourceManager, new object[]
			{
				task
			});
		}

		public static LocalizedString CannotSpecifyIndustryTypeWhenOrgIsUpToDateDuringServerInstallation
		{
			get
			{
				return new LocalizedString("CannotSpecifyIndustryTypeWhenOrgIsUpToDateDuringServerInstallation", "Ex4D051A", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RemoveUmLanguagePackFailText
		{
			get
			{
				return new LocalizedString("RemoveUmLanguagePackFailText", "Ex955291", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString UmLanguagePackPackagePath(string path)
		{
			return new LocalizedString("UmLanguagePackPackagePath", "Ex5D00DB", false, true, Strings.ResourceManager, new object[]
			{
				path
			});
		}

		public static LocalizedString ChoosingDomainController
		{
			get
			{
				return new LocalizedString("ChoosingDomainController", "ExFC1196", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString CentralAdminRoleDisplayName
		{
			get
			{
				return new LocalizedString("CentralAdminRoleDisplayName", "Ex6C64FD", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString NeedConfigureIpv4StaticAddressForSecondSubnetWhenFirstSubnetConfiguresIPV4StaticAddress
		{
			get
			{
				return new LocalizedString("NeedConfigureIpv4StaticAddressForSecondSubnetWhenFirstSubnetConfiguresIPV4StaticAddress", "ExCFFD09", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString UserSpecifiedDCIsNotAvailableException(string dc)
		{
			return new LocalizedString("UserSpecifiedDCIsNotAvailableException", "Ex353648", false, true, Strings.ResourceManager, new object[]
			{
				dc
			});
		}

		public static LocalizedString RootDataHandlerCount(int count)
		{
			return new LocalizedString("RootDataHandlerCount", "Ex1EDD13", false, true, Strings.ResourceManager, new object[]
			{
				count
			});
		}

		public static LocalizedString PreSetupText
		{
			get
			{
				return new LocalizedString("PreSetupText", "Ex83D051", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString SettingArgumentToValue(string argument, string value)
		{
			return new LocalizedString("SettingArgumentToValue", "Ex3D2685", false, true, Strings.ResourceManager, new object[]
			{
				argument,
				value
			});
		}

		public static LocalizedString ModeErrorForUpgrade
		{
			get
			{
				return new LocalizedString("ModeErrorForUpgrade", "ExE4AE75", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString OrganizationInstallText
		{
			get
			{
				return new LocalizedString("OrganizationInstallText", "ExCD0005", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString TheFirstPathUnderTheSecondPath(string firstPath, string secondPath)
		{
			return new LocalizedString("TheFirstPathUnderTheSecondPath", "ExCA4270", false, true, Strings.ResourceManager, new object[]
			{
				firstPath,
				secondPath
			});
		}

		public static LocalizedString CannotSpecifyServerCEIPWhenMachineIsNotCleanDuringServerInstallation
		{
			get
			{
				return new LocalizedString("CannotSpecifyServerCEIPWhenMachineIsNotCleanDuringServerInstallation", "Ex181B94", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString UnableToFindBuildVersion1(string xmlPath)
		{
			return new LocalizedString("UnableToFindBuildVersion1", "Ex8B0A6F", false, true, Strings.ResourceManager, new object[]
			{
				xmlPath
			});
		}

		public static LocalizedString BackupPath(string path)
		{
			return new LocalizedString("BackupPath", "ExEED501", false, true, Strings.ResourceManager, new object[]
			{
				path
			});
		}

		public static LocalizedString PackagePathSetTo(string path)
		{
			return new LocalizedString("PackagePathSetTo", "Ex20E52C", false, true, Strings.ResourceManager, new object[]
			{
				path
			});
		}

		public static LocalizedString ADRelatedError(string error)
		{
			return new LocalizedString("ADRelatedError", "ExC4BD72", false, true, Strings.ResourceManager, new object[]
			{
				error
			});
		}

		public static LocalizedString AlwaysWatsonForDebug(string key, string name)
		{
			return new LocalizedString("AlwaysWatsonForDebug", "", false, false, Strings.ResourceManager, new object[]
			{
				key,
				name
			});
		}

		public static LocalizedString LanguagePacksToInstall
		{
			get
			{
				return new LocalizedString("LanguagePacksToInstall", "Ex91DD3E", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString LanguagePacksPackagePathNotFound(string path)
		{
			return new LocalizedString("LanguagePacksPackagePathNotFound", "Ex46A348", false, true, Strings.ResourceManager, new object[]
			{
				path
			});
		}

		public static LocalizedString UnifiedMessagingLanguagePackInstalled(string culture)
		{
			return new LocalizedString("UnifiedMessagingLanguagePackInstalled", "Ex9752C0", false, true, Strings.ResourceManager, new object[]
			{
				culture
			});
		}

		public static LocalizedString InstalledVersion(Version version)
		{
			return new LocalizedString("InstalledVersion", "Ex47263B", false, true, Strings.ResourceManager, new object[]
			{
				version
			});
		}

		public static LocalizedString RemoveSuccessStatus
		{
			get
			{
				return new LocalizedString("RemoveSuccessStatus", "ExB940DB", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString SetupWillUseDomainController(string dc)
		{
			return new LocalizedString("SetupWillUseDomainController", "Ex711424", false, true, Strings.ResourceManager, new object[]
			{
				dc
			});
		}

		public static LocalizedString UnableToFindLPVersioning
		{
			get
			{
				return new LocalizedString("UnableToFindLPVersioning", "Ex6A6BA1", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString AddRoleAlreadyInstalledError(string installedRoles)
		{
			return new LocalizedString("AddRoleAlreadyInstalledError", "ExDDF583", false, true, Strings.ResourceManager, new object[]
			{
				installedRoles
			});
		}

		public static LocalizedString CommandLineParameterSpecified(string parameter)
		{
			return new LocalizedString("CommandLineParameterSpecified", "Ex5DF93D", false, true, Strings.ResourceManager, new object[]
			{
				parameter
			});
		}

		public static LocalizedString CannotRemoveEnglishUSLanguagePack
		{
			get
			{
				return new LocalizedString("CannotRemoveEnglishUSLanguagePack", "Ex4C25E0", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString LanguagePacksToAdd
		{
			get
			{
				return new LocalizedString("LanguagePacksToAdd", "ExC5DCD9", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString CannotFindFile(string file)
		{
			return new LocalizedString("CannotFindFile", "Ex622AD5", false, true, Strings.ResourceManager, new object[]
			{
				file
			});
		}

		public static LocalizedString UpgradeRolesToInstall
		{
			get
			{
				return new LocalizedString("UpgradeRolesToInstall", "Ex024F9F", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString WillSkipAMEngineDownloadCheck
		{
			get
			{
				return new LocalizedString("WillSkipAMEngineDownloadCheck", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString WillGetConfiguredRolesFromRegistry
		{
			get
			{
				return new LocalizedString("WillGetConfiguredRolesFromRegistry", "Ex23EA15", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MailboxRoleDisplayName
		{
			get
			{
				return new LocalizedString("MailboxRoleDisplayName", "ExD34A06", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString EnglishUSLanguagePackInstalled
		{
			get
			{
				return new LocalizedString("EnglishUSLanguagePackInstalled", "Ex0C9E29", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString NoRoleSelectedForUninstall
		{
			get
			{
				return new LocalizedString("NoRoleSelectedForUninstall", "Ex29C8F8", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ParameterNotValidForCurrentRoles(string parameter)
		{
			return new LocalizedString("ParameterNotValidForCurrentRoles", "", false, false, Strings.ResourceManager, new object[]
			{
				parameter
			});
		}

		public static LocalizedString MustEnableLegacyOutlook
		{
			get
			{
				return new LocalizedString("MustEnableLegacyOutlook", "ExB435E8", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RemoveDatacenterFileText
		{
			get
			{
				return new LocalizedString("RemoveDatacenterFileText", "Ex267B5E", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString CannotSpecifyCEIPWhenOrganizationHasE14OrLaterServersDuringPrepareAD
		{
			get
			{
				return new LocalizedString("CannotSpecifyCEIPWhenOrganizationHasE14OrLaterServersDuringPrepareAD", "ExE89E5A", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString OrgAlreadyHasBridgeheadServers
		{
			get
			{
				return new LocalizedString("OrgAlreadyHasBridgeheadServers", "ExEABAD8", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString PrerequisiteAnalysis
		{
			get
			{
				return new LocalizedString("PrerequisiteAnalysis", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ExchangeServerFound(string name)
		{
			return new LocalizedString("ExchangeServerFound", "Ex3258E6", false, true, Strings.ResourceManager, new object[]
			{
				name
			});
		}

		public static LocalizedString NeedConfigureIpv4ForSecondSubnetWhenFirstSubnetConfiguresIPV4
		{
			get
			{
				return new LocalizedString("NeedConfigureIpv4ForSecondSubnetWhenFirstSubnetConfiguresIPV4", "Ex21863C", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RemovePreCheckText
		{
			get
			{
				return new LocalizedString("RemovePreCheckText", "Ex9F1060", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RemoveUmLanguagePackSuccessText
		{
			get
			{
				return new LocalizedString("RemoveUmLanguagePackSuccessText", "ExEB9DB9", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString AdInitializationStatus(bool status)
		{
			return new LocalizedString("AdInitializationStatus", "ExC4BBB4", false, true, Strings.ResourceManager, new object[]
			{
				status
			});
		}

		public static LocalizedString InstallationPathUnderUserProfileInformation(string path)
		{
			return new LocalizedString("InstallationPathUnderUserProfileInformation", "Ex8CC260", false, true, Strings.ResourceManager, new object[]
			{
				path
			});
		}

		public static LocalizedString DeterminingOrgPrepParameters
		{
			get
			{
				return new LocalizedString("DeterminingOrgPrepParameters", "Ex86C4E4", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RemoveProgressText
		{
			get
			{
				return new LocalizedString("RemoveProgressText", "Ex0B81D8", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RemoveUmLanguagePackText
		{
			get
			{
				return new LocalizedString("RemoveUmLanguagePackText", "Ex232907", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString UmLanguagePackDirectoryNotAvailable(string directory)
		{
			return new LocalizedString("UmLanguagePackDirectoryNotAvailable", "Ex4E887C", false, true, Strings.ResourceManager, new object[]
			{
				directory
			});
		}

		public static LocalizedString UpgradeRoleNotInstalledError(string missingRoles)
		{
			return new LocalizedString("UpgradeRoleNotInstalledError", "Ex51FFDA", false, true, Strings.ResourceManager, new object[]
			{
				missingRoles
			});
		}

		public static LocalizedString MsiDirectoryNotFound(string path)
		{
			return new LocalizedString("MsiDirectoryNotFound", "Ex0ECC86", false, true, Strings.ResourceManager, new object[]
			{
				path
			});
		}

		public static LocalizedString ValidatingRoleOptions(int count)
		{
			return new LocalizedString("ValidatingRoleOptions", "ExA9D934", false, true, Strings.ResourceManager, new object[]
			{
				count
			});
		}

		public static LocalizedString DRServerNotFoundInAD
		{
			get
			{
				return new LocalizedString("DRServerNotFoundInAD", "ExC442F0", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString CannotSpecifyServerCEIPWhenGlobalCEIPIsOptedOutDuringServerInstallation
		{
			get
			{
				return new LocalizedString("CannotSpecifyServerCEIPWhenGlobalCEIPIsOptedOutDuringServerInstallation", "Ex2CF821", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ServerOptDescriptionText
		{
			get
			{
				return new LocalizedString("ServerOptDescriptionText", "Ex8A6675", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InstallationPathNotSet
		{
			get
			{
				return new LocalizedString("InstallationPathNotSet", "Ex85D389", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString UnexpectedFileFromBundle(string parameter)
		{
			return new LocalizedString("UnexpectedFileFromBundle", "Ex60031E", false, true, Strings.ResourceManager, new object[]
			{
				parameter
			});
		}

		public static LocalizedString DisplayServerName(string serverName)
		{
			return new LocalizedString("DisplayServerName", "", false, false, Strings.ResourceManager, new object[]
			{
				serverName
			});
		}

		public static LocalizedString InvalidFqdn(string fqdn)
		{
			return new LocalizedString("InvalidFqdn", "", false, false, Strings.ResourceManager, new object[]
			{
				fqdn
			});
		}

		public static LocalizedString UnableToFindBuildVersion
		{
			get
			{
				return new LocalizedString("UnableToFindBuildVersion", "Ex85B342", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString TargetInstallationDirectory(string directory)
		{
			return new LocalizedString("TargetInstallationDirectory", "ExCB6843", false, true, Strings.ResourceManager, new object[]
			{
				directory
			});
		}

		public static LocalizedString AdminToolsRoleDisplayName
		{
			get
			{
				return new LocalizedString("AdminToolsRoleDisplayName", "Ex841C1E", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString UmLanguagePackPackagePathNotSpecified
		{
			get
			{
				return new LocalizedString("UmLanguagePackPackagePathNotSpecified", "ExF5BDA9", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString AddUmLanguagePackModeDataHandlerCount(int count)
		{
			return new LocalizedString("AddUmLanguagePackModeDataHandlerCount", "ExC0E9B0", false, true, Strings.ResourceManager, new object[]
			{
				count
			});
		}

		public static LocalizedString AddCannotChangeTargetDirectoryError
		{
			get
			{
				return new LocalizedString("AddCannotChangeTargetDirectoryError", "Ex1CD429", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString EdgeRoleInstalledButServerObjectNotFound
		{
			get
			{
				return new LocalizedString("EdgeRoleInstalledButServerObjectNotFound", "ExEAEBA4", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString LanguagePacksNotFound
		{
			get
			{
				return new LocalizedString("LanguagePacksNotFound", "Ex25FA96", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RunForestPrepInSchemaMasterDomainException(string dom, string site)
		{
			return new LocalizedString("RunForestPrepInSchemaMasterDomainException", "Ex8FD345", false, true, Strings.ResourceManager, new object[]
			{
				dom,
				site
			});
		}

		public static LocalizedString ApplyingDefaultRoleSelectionState
		{
			get
			{
				return new LocalizedString("ApplyingDefaultRoleSelectionState", "Ex0A4DE9", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString LanguagePackagePathSetTo(string path)
		{
			return new LocalizedString("LanguagePackagePathSetTo", "ExA3F043", false, true, Strings.ResourceManager, new object[]
			{
				path
			});
		}

		public static LocalizedString SettingArgumentBecauseOfCommandLineParameter(string parameter, string argument)
		{
			return new LocalizedString("SettingArgumentBecauseOfCommandLineParameter", "Ex249244", false, true, Strings.ResourceManager, new object[]
			{
				parameter,
				argument
			});
		}

		public static LocalizedString MustConfigureIPv4ForFirstSubnet
		{
			get
			{
				return new LocalizedString("MustConfigureIPv4ForFirstSubnet", "ExAC2BAD", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString SetupNotFoundInSourceDirError(string fileName)
		{
			return new LocalizedString("SetupNotFoundInSourceDirError", "ExD40EF3", false, true, Strings.ResourceManager, new object[]
			{
				fileName
			});
		}

		public static LocalizedString LanguagePacksLogFilePathNotSpecified
		{
			get
			{
				return new LocalizedString("LanguagePacksLogFilePathNotSpecified", "Ex2522A2", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString OrganizationPrereqText
		{
			get
			{
				return new LocalizedString("OrganizationPrereqText", "Ex4CA534", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString AddFailText
		{
			get
			{
				return new LocalizedString("AddFailText", "ExE604BB", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RemoveRolesToInstall
		{
			get
			{
				return new LocalizedString("RemoveRolesToInstall", "Ex1C07AA", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString NeedConfigureIpv4DHCPForSecondSubnetWhenFirstSubnetConfiguresIPV4DHCP
		{
			get
			{
				return new LocalizedString("NeedConfigureIpv4DHCPForSecondSubnetWhenFirstSubnetConfiguresIPV4DHCP", "ExBE5185", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MidFileCopyText
		{
			get
			{
				return new LocalizedString("MidFileCopyText", "Ex02AED2", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString CentralAdminDatabaseRoleDisplayName
		{
			get
			{
				return new LocalizedString("CentralAdminDatabaseRoleDisplayName", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString SchemaUpdateRequired(bool status)
		{
			return new LocalizedString("SchemaUpdateRequired", "Ex55BD23", false, true, Strings.ResourceManager, new object[]
			{
				status
			});
		}

		public static LocalizedString AddGatewayAloneError
		{
			get
			{
				return new LocalizedString("AddGatewayAloneError", "Ex2CDDD2", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString VersionMismatchWarning(ExchangeBuild version)
		{
			return new LocalizedString("VersionMismatchWarning", "", false, false, Strings.ResourceManager, new object[]
			{
				version
			});
		}

		public static LocalizedString CafeRoleDisplayName
		{
			get
			{
				return new LocalizedString("CafeRoleDisplayName", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString NoServerRolesToInstall
		{
			get
			{
				return new LocalizedString("NoServerRolesToInstall", "Ex7B2B08", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString DCNotResponding(string dc)
		{
			return new LocalizedString("DCNotResponding", "ExBF2F12", false, true, Strings.ResourceManager, new object[]
			{
				dc
			});
		}

		public static LocalizedString ForestPrepAlreadyRun(string dc)
		{
			return new LocalizedString("ForestPrepAlreadyRun", "Ex46365A", false, true, Strings.ResourceManager, new object[]
			{
				dc
			});
		}

		public static LocalizedString DCNotInLocalDomain(string dc)
		{
			return new LocalizedString("DCNotInLocalDomain", "Ex5CBBFD", false, true, Strings.ResourceManager, new object[]
			{
				dc
			});
		}

		public static LocalizedString RemoveUmLanguagePackLogFilePath(string path)
		{
			return new LocalizedString("RemoveUmLanguagePackLogFilePath", "Ex00689F", false, true, Strings.ResourceManager, new object[]
			{
				path
			});
		}

		public static LocalizedString AddUmLanguagePackLogFilePath(string path)
		{
			return new LocalizedString("AddUmLanguagePackLogFilePath", "ExFDCD56", false, true, Strings.ResourceManager, new object[]
			{
				path
			});
		}

		public static LocalizedString ExistingOrganizationName(string name)
		{
			return new LocalizedString("ExistingOrganizationName", "ExD18E39", false, true, Strings.ResourceManager, new object[]
			{
				name
			});
		}

		public static LocalizedString DRPrereq
		{
			get
			{
				return new LocalizedString("DRPrereq", "Ex09295A", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ExceptionWhenDeserializingStateFile(Exception e)
		{
			return new LocalizedString("ExceptionWhenDeserializingStateFile", "ExDA30BB", false, true, Strings.ResourceManager, new object[]
			{
				e
			});
		}

		public static LocalizedString ModeErrorForFreshInstall
		{
			get
			{
				return new LocalizedString("ModeErrorForFreshInstall", "Ex98A60B", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString LanguagePacksVersionFileNotFound(string pathname)
		{
			return new LocalizedString("LanguagePacksVersionFileNotFound", "ExB754D8", false, true, Strings.ResourceManager, new object[]
			{
				pathname
			});
		}

		public static LocalizedString LegacyRoutingServerNotValid
		{
			get
			{
				return new LocalizedString("LegacyRoutingServerNotValid", "Ex6443D5", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString CopyDatacenterFileText
		{
			get
			{
				return new LocalizedString("CopyDatacenterFileText", "Ex45C8F9", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString UnsupportedMode
		{
			get
			{
				return new LocalizedString("UnsupportedMode", "ExA7BB2C", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ExchangeOrganizationNameRequired
		{
			get
			{
				return new LocalizedString("ExchangeOrganizationNameRequired", "ExA8F9EC", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString UpgradePreCheckText
		{
			get
			{
				return new LocalizedString("UpgradePreCheckText", "Ex045128", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString StateFileVersionMismatch(string versionFromFile, string versionFromContext)
		{
			return new LocalizedString("StateFileVersionMismatch", "Ex1CDE5B", false, true, Strings.ResourceManager, new object[]
			{
				versionFromFile,
				versionFromContext
			});
		}

		public static LocalizedString NoConfigurationInfoFoundForInstallableUnit(string name)
		{
			return new LocalizedString("NoConfigurationInfoFoundForInstallableUnit", "ExAD450A", false, true, Strings.ResourceManager, new object[]
			{
				name
			});
		}

		public static LocalizedString SetupWillRunFromPath(string path)
		{
			return new LocalizedString("SetupWillRunFromPath", "Ex6AF63C", false, true, Strings.ResourceManager, new object[]
			{
				path
			});
		}

		public static LocalizedString ADDomainConfigVersionHigherThanSetupException(int adDomainConfigVersion, int setupDomainConfigVersion)
		{
			return new LocalizedString("ADDomainConfigVersionHigherThanSetupException", "Ex5653B3", false, true, Strings.ResourceManager, new object[]
			{
				adDomainConfigVersion,
				setupDomainConfigVersion
			});
		}

		public static LocalizedString RemoveLanguagePacksLogFilePath(string path)
		{
			return new LocalizedString("RemoveLanguagePacksLogFilePath", "Ex40F3D2", false, true, Strings.ResourceManager, new object[]
			{
				path
			});
		}

		public static LocalizedString WarningUpperCase(string warning)
		{
			return new LocalizedString("WarningUpperCase", "", false, false, Strings.ResourceManager, new object[]
			{
				warning
			});
		}

		public static LocalizedString Prereqs
		{
			get
			{
				return new LocalizedString("Prereqs", "Ex24C36F", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString UmLanguagePackFullPackagePath(string path)
		{
			return new LocalizedString("UmLanguagePackFullPackagePath", "ExBE820B", false, true, Strings.ResourceManager, new object[]
			{
				path
			});
		}

		public static LocalizedString FailedToReadLCIDFromRegistryError
		{
			get
			{
				return new LocalizedString("FailedToReadLCIDFromRegistryError", "Ex9E48FE", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString LanguagePacksDisplayName
		{
			get
			{
				return new LocalizedString("LanguagePacksDisplayName", "ExC1C4F8", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RoleNotInstalledError(string missingRoles)
		{
			return new LocalizedString("RoleNotInstalledError", "ExF69611", false, true, Strings.ResourceManager, new object[]
			{
				missingRoles
			});
		}

		public static LocalizedString ErrorUpperCase(string error)
		{
			return new LocalizedString("ErrorUpperCase", "", false, false, Strings.ResourceManager, new object[]
			{
				error
			});
		}

		public static LocalizedString LanguagePacksBadVersionFormat(string version)
		{
			return new LocalizedString("LanguagePacksBadVersionFormat", "Ex548466", false, true, Strings.ResourceManager, new object[]
			{
				version
			});
		}

		public static LocalizedString NoSchemaEntry(string parameter)
		{
			return new LocalizedString("NoSchemaEntry", "", false, false, Strings.ResourceManager, new object[]
			{
				parameter
			});
		}

		public static LocalizedString InstallationPathInvalidDriveFormatInformation(string path)
		{
			return new LocalizedString("InstallationPathInvalidDriveFormatInformation", "Ex8B6E7D", false, true, Strings.ResourceManager, new object[]
			{
				path
			});
		}

		public static LocalizedString SetupExitsBecauseOfTransientException
		{
			get
			{
				return new LocalizedString("SetupExitsBecauseOfTransientException", "Ex59FD2E", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString NoExchangeConfigurationContainerFound(string message)
		{
			return new LocalizedString("NoExchangeConfigurationContainerFound", "", false, false, Strings.ResourceManager, new object[]
			{
				message
			});
		}

		public static LocalizedString OrganizationAlreadyExists(string name)
		{
			return new LocalizedString("OrganizationAlreadyExists", "ExC887C3", false, true, Strings.ResourceManager, new object[]
			{
				name
			});
		}

		public static LocalizedString DRSuccessText
		{
			get
			{
				return new LocalizedString("DRSuccessText", "ExD9453B", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString DRRolesToInstall
		{
			get
			{
				return new LocalizedString("DRRolesToInstall", "Ex5BF89B", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ParametersForTheTaskTitle
		{
			get
			{
				return new LocalizedString("ParametersForTheTaskTitle", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString DCAlreadySpecified(string dc)
		{
			return new LocalizedString("DCAlreadySpecified", "ExE93D5A", false, true, Strings.ResourceManager, new object[]
			{
				dc
			});
		}

		public static LocalizedString ExchangeServerNotFound(string name)
		{
			return new LocalizedString("ExchangeServerNotFound", "Ex9C871E", false, true, Strings.ResourceManager, new object[]
			{
				name
			});
		}

		public static LocalizedString SchemaMasterDCNotAvailableException(string dc)
		{
			return new LocalizedString("SchemaMasterDCNotAvailableException", "ExD1B80D", false, true, Strings.ResourceManager, new object[]
			{
				dc
			});
		}

		public static LocalizedString ADRelatedUnknownError
		{
			get
			{
				return new LocalizedString("ADRelatedUnknownError", "Ex724363", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString AddFailStatus
		{
			get
			{
				return new LocalizedString("AddFailStatus", "Ex16A8F8", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString BackupKeyIsWrongType(string keyName, string valueName)
		{
			return new LocalizedString("BackupKeyIsWrongType", "ExB275AF", false, true, Strings.ResourceManager, new object[]
			{
				keyName,
				valueName
			});
		}

		public static LocalizedString AddLanguagePacksFailText
		{
			get
			{
				return new LocalizedString("AddLanguagePacksFailText", "ExE1D010", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString CannotInstallDatacenterRole(string role)
		{
			return new LocalizedString("CannotInstallDatacenterRole", "ExCA2623", false, true, Strings.ResourceManager, new object[]
			{
				role
			});
		}

		public static LocalizedString LanguagePacksInstalledVersionNull
		{
			get
			{
				return new LocalizedString("LanguagePacksInstalledVersionNull", "Ex741310", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString PersistedDomainController(string dc)
		{
			return new LocalizedString("PersistedDomainController", "Ex54F661", false, true, Strings.ResourceManager, new object[]
			{
				dc
			});
		}

		public static LocalizedString DomainConfigUpdateRequired(bool status)
		{
			return new LocalizedString("DomainConfigUpdateRequired", "Ex7D6A22", false, true, Strings.ResourceManager, new object[]
			{
				status
			});
		}

		public static LocalizedString SetupExitsBecauseOfLPPathNotFoundException
		{
			get
			{
				return new LocalizedString("SetupExitsBecauseOfLPPathNotFoundException", "Ex19A6EB", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ServerIsProvisioned
		{
			get
			{
				return new LocalizedString("ServerIsProvisioned", "Ex48CE23", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString NoUmLanguagePackSpecified
		{
			get
			{
				return new LocalizedString("NoUmLanguagePackSpecified", "Ex62D03F", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString WillSearchForAServerObjectForServer(string servername)
		{
			return new LocalizedString("WillSearchForAServerObjectForServer", "Ex6F0B6B", false, true, Strings.ResourceManager, new object[]
			{
				servername
			});
		}

		public static LocalizedString PostFileCopyText
		{
			get
			{
				return new LocalizedString("PostFileCopyText", "Ex1E2312", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ExecutionCompleted
		{
			get
			{
				return new LocalizedString("ExecutionCompleted", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString UnknownPreviousVersion
		{
			get
			{
				return new LocalizedString("UnknownPreviousVersion", "ExC34950", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RemoveFileText
		{
			get
			{
				return new LocalizedString("RemoveFileText", "Ex2F8908", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString AddedConfigurationInfoForInstallableUnit(string name)
		{
			return new LocalizedString("AddedConfigurationInfoForInstallableUnit", "Ex8A9FD8", false, true, Strings.ResourceManager, new object[]
			{
				name
			});
		}

		public static LocalizedString MSIIsCurrent
		{
			get
			{
				return new LocalizedString("MSIIsCurrent", "ExFF9A2C", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString AddPrereq
		{
			get
			{
				return new LocalizedString("AddPrereq", "ExCC8158", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RemoveServerRoleText
		{
			get
			{
				return new LocalizedString("RemoveServerRoleText", "ExEB0FD2", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InstallationPathInvalidRootDriveInformation(string path)
		{
			return new LocalizedString("InstallationPathInvalidRootDriveInformation", "ExE1FD64", false, true, Strings.ResourceManager, new object[]
			{
				path
			});
		}

		public static LocalizedString ADOrgConfigVersionHigherThanSetupException(int adOrgConfigVersion, int setupOrgConfigVersion)
		{
			return new LocalizedString("ADOrgConfigVersionHigherThanSetupException", "Ex98A535", false, true, Strings.ResourceManager, new object[]
			{
				adOrgConfigVersion,
				setupOrgConfigVersion
			});
		}

		public static LocalizedString DoesNotSupportCEIPForAdminOnlyInstallation
		{
			get
			{
				return new LocalizedString("DoesNotSupportCEIPForAdminOnlyInstallation", "Ex783CB8", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString AddLanguagePacksText
		{
			get
			{
				return new LocalizedString("AddLanguagePacksText", "ExDBEEE1", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString AttemptToSearchExchangeServerFailed(string serverName, string message)
		{
			return new LocalizedString("AttemptToSearchExchangeServerFailed", "Ex1E0D45", false, true, Strings.ResourceManager, new object[]
			{
				serverName,
				message
			});
		}

		public static LocalizedString UninstallModeDataHandlerHandlersAndWorkUnits(int handlers, int workunits)
		{
			return new LocalizedString("UninstallModeDataHandlerHandlersAndWorkUnits", "Ex51B22D", false, true, Strings.ResourceManager, new object[]
			{
				handlers,
				workunits
			});
		}

		public static LocalizedString DRPreCheckText
		{
			get
			{
				return new LocalizedString("DRPreCheckText", "Ex1D4658", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString CommandLine(string launcher, string cmdLine)
		{
			return new LocalizedString("CommandLine", "Ex9B88C7", false, true, Strings.ResourceManager, new object[]
			{
				launcher,
				cmdLine
			});
		}

		public static LocalizedString AddProgressText
		{
			get
			{
				return new LocalizedString("AddProgressText", "Ex68024D", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString DRFailStatus
		{
			get
			{
				return new LocalizedString("DRFailStatus", "Ex910D3E", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RemoveUmLanguagePackFailStatus
		{
			get
			{
				return new LocalizedString("RemoveUmLanguagePackFailStatus", "ExCA2907", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString SetupSourceDirectory(string path)
		{
			return new LocalizedString("SetupSourceDirectory", "ExD92313", false, true, Strings.ResourceManager, new object[]
			{
				path
			});
		}

		public static LocalizedString UmLanguagePackNotFoundForCulture(string culture)
		{
			return new LocalizedString("UmLanguagePackNotFoundForCulture", "Ex62501F", false, true, Strings.ResourceManager, new object[]
			{
				culture
			});
		}

		public static LocalizedString UpgradeIntroductionText
		{
			get
			{
				return new LocalizedString("UpgradeIntroductionText", "ExA3000B", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString UmLanguagePackDisplayNameWithCulture(string culture)
		{
			return new LocalizedString("UmLanguagePackDisplayNameWithCulture", "Ex2C411F", false, true, Strings.ResourceManager, new object[]
			{
				culture
			});
		}

		public static LocalizedString NoUmLanguagePackInstalled
		{
			get
			{
				return new LocalizedString("NoUmLanguagePackInstalled", "Ex157C86", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RemoveUnifiedMessagingServerDescription
		{
			get
			{
				return new LocalizedString("RemoveUnifiedMessagingServerDescription", "Ex379FD8", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ClientAccessRoleDisplayName
		{
			get
			{
				return new LocalizedString("ClientAccessRoleDisplayName", "Ex0CDD1A", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString UpgradeFailStatus
		{
			get
			{
				return new LocalizedString("UpgradeFailStatus", "Ex3F26C0", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MsiFilesDirectoryCannotBeChanged(string msiFileDirectory)
		{
			return new LocalizedString("MsiFilesDirectoryCannotBeChanged", "Ex36425E", false, true, Strings.ResourceManager, new object[]
			{
				msiFileDirectory
			});
		}

		public static LocalizedString NoRoleSelectedForInstall
		{
			get
			{
				return new LocalizedString("NoRoleSelectedForInstall", "Ex36E433", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ExchangeConfigurationContainerName(string name)
		{
			return new LocalizedString("ExchangeConfigurationContainerName", "Ex6B0641", false, true, Strings.ResourceManager, new object[]
			{
				name
			});
		}

		public static LocalizedString ExecutionError
		{
			get
			{
				return new LocalizedString("ExecutionError", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InstallationLicenseAgreementShortDescription
		{
			get
			{
				return new LocalizedString("InstallationLicenseAgreementShortDescription", "ExB3A6B6", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ForestPrepNotRunOrNotReplicatedException(string dc)
		{
			return new LocalizedString("ForestPrepNotRunOrNotReplicatedException", "ExC0B44B", false, true, Strings.ResourceManager, new object[]
			{
				dc
			});
		}

		public static LocalizedString RemoveSuccessText
		{
			get
			{
				return new LocalizedString("RemoveSuccessText", "ExBE5014", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InstalledLanguageComment
		{
			get
			{
				return new LocalizedString("InstalledLanguageComment", "Ex20E2AB", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString LPVersioningExtractionFailed(string pathToBundle)
		{
			return new LocalizedString("LPVersioningExtractionFailed", "Ex37586B", false, true, Strings.ResourceManager, new object[]
			{
				pathToBundle
			});
		}

		public static LocalizedString UpgradeFailText
		{
			get
			{
				return new LocalizedString("UpgradeFailText", "Ex6DBDBF", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MSINotPresent(string path)
		{
			return new LocalizedString("MSINotPresent", "Ex686F6A", false, true, Strings.ResourceManager, new object[]
			{
				path
			});
		}

		public static LocalizedString SetupWillUseGlobalCatalog(string gc)
		{
			return new LocalizedString("SetupWillUseGlobalCatalog", "Ex391158", false, true, Strings.ResourceManager, new object[]
			{
				gc
			});
		}

		public static LocalizedString NotALegacyServer(string name)
		{
			return new LocalizedString("NotALegacyServer", "Ex3C2284", false, true, Strings.ResourceManager, new object[]
			{
				name
			});
		}

		public static LocalizedString WillGetConfiguredRolesFromServerObject(string servername)
		{
			return new LocalizedString("WillGetConfiguredRolesFromServerObject", "Ex7A23A1", false, true, Strings.ResourceManager, new object[]
			{
				servername
			});
		}

		public static LocalizedString PreFileCopyText
		{
			get
			{
				return new LocalizedString("PreFileCopyText", "Ex22878B", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString UninstallModeDataHandlerCount(int count)
		{
			return new LocalizedString("UninstallModeDataHandlerCount", "Ex60313D", false, true, Strings.ResourceManager, new object[]
			{
				count
			});
		}

		public static LocalizedString UnknownDestinationPath
		{
			get
			{
				return new LocalizedString("UnknownDestinationPath", "Ex264D80", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString UpgradeServerRoleText
		{
			get
			{
				return new LocalizedString("UpgradeServerRoleText", "ExE0C870", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString GlobalOptDescriptionText
		{
			get
			{
				return new LocalizedString("GlobalOptDescriptionText", "Ex12F431", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString WillExecuteHighLevelTask(string task)
		{
			return new LocalizedString("WillExecuteHighLevelTask", "Ex532305", false, true, Strings.ResourceManager, new object[]
			{
				task
			});
		}

		public static LocalizedString BackupVersion(Version version)
		{
			return new LocalizedString("BackupVersion", "ExF72BC4", false, true, Strings.ResourceManager, new object[]
			{
				version
			});
		}

		public static LocalizedString AddServerRoleText
		{
			get
			{
				return new LocalizedString("AddServerRoleText", "Ex69B15D", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString OrgConfigUpdateRequired(bool status)
		{
			return new LocalizedString("OrgConfigUpdateRequired", "Ex5E38B5", false, true, Strings.ResourceManager, new object[]
			{
				status
			});
		}

		public static LocalizedString MaintenanceIntroduction
		{
			get
			{
				return new LocalizedString("MaintenanceIntroduction", "ExDD5A89", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString UmLanguagePathLogFilePathNotSpecified
		{
			get
			{
				return new LocalizedString("UmLanguagePathLogFilePathNotSpecified", "ExC64CD8", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InstallModeDataHandlerCount(int count)
		{
			return new LocalizedString("InstallModeDataHandlerCount", "Ex5AA6C0", false, true, Strings.ResourceManager, new object[]
			{
				count
			});
		}

		public static LocalizedString UpgradeProgressText
		{
			get
			{
				return new LocalizedString("UpgradeProgressText", "ExAE01D4", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString DCNameNotValid(string dc)
		{
			return new LocalizedString("DCNameNotValid", "Ex46E1D1", false, true, Strings.ResourceManager, new object[]
			{
				dc
			});
		}

		public static LocalizedString GCChosen(string gc)
		{
			return new LocalizedString("GCChosen", "Ex936DF4", false, true, Strings.ResourceManager, new object[]
			{
				gc
			});
		}

		public static LocalizedString ADHasBeenPrepared
		{
			get
			{
				return new LocalizedString("ADHasBeenPrepared", "Ex04700F", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString SchemaMasterIsLocalDC(string dc)
		{
			return new LocalizedString("SchemaMasterIsLocalDC", "ExCA1161", false, true, Strings.ResourceManager, new object[]
			{
				dc
			});
		}

		public static LocalizedString InstallationPathInvalidInformation(string path)
		{
			return new LocalizedString("InstallationPathInvalidInformation", "Ex5EEC52", false, true, Strings.ResourceManager, new object[]
			{
				path
			});
		}

		public static LocalizedString InvalidOrganizationName(string name)
		{
			return new LocalizedString("InvalidOrganizationName", "", false, false, Strings.ResourceManager, new object[]
			{
				name
			});
		}

		public static LocalizedString SpecifyExchangeOrganizationName
		{
			get
			{
				return new LocalizedString("SpecifyExchangeOrganizationName", "Ex31CAD7", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString UpgradeMustUseBootStrapper
		{
			get
			{
				return new LocalizedString("UpgradeMustUseBootStrapper", "Ex826246", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString AddIntroductionText
		{
			get
			{
				return new LocalizedString("AddIntroductionText", "Ex5B0258", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString UpgradeSuccessStatus
		{
			get
			{
				return new LocalizedString("UpgradeSuccessStatus", "Ex7794F9", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString NonCultureRegistryEntryFound(string value)
		{
			return new LocalizedString("NonCultureRegistryEntryFound", "ExFC9714", false, true, Strings.ResourceManager, new object[]
			{
				value
			});
		}

		public static LocalizedString LegacyServerNameRequired
		{
			get
			{
				return new LocalizedString("LegacyServerNameRequired", "Ex3682B9", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString TheCurrentServerHasNoExchangeBits
		{
			get
			{
				return new LocalizedString("TheCurrentServerHasNoExchangeBits", "Ex5D0ED6", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString UpgradeSuccessText
		{
			get
			{
				return new LocalizedString("UpgradeSuccessText", "ExBDE2A1", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString BridgeheadRoleDisplayName
		{
			get
			{
				return new LocalizedString("BridgeheadRoleDisplayName", "Ex1A18F6", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString WaitForForestPrepReplicationToLocalDomainException
		{
			get
			{
				return new LocalizedString("WaitForForestPrepReplicationToLocalDomainException", "Ex34FF80", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ForestPrepNotRun(string dc)
		{
			return new LocalizedString("ForestPrepNotRun", "Ex048927", false, true, Strings.ResourceManager, new object[]
			{
				dc
			});
		}

		public static LocalizedString RemovePrereq
		{
			get
			{
				return new LocalizedString("RemovePrereq", "ExAA8B8F", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InvalidExchangeOrganizationName(string message)
		{
			return new LocalizedString("InvalidExchangeOrganizationName", "ExE89C70", false, true, Strings.ResourceManager, new object[]
			{
				message
			});
		}

		public static LocalizedString UmLanguagePackNotInstalledForCulture(string culture)
		{
			return new LocalizedString("UmLanguagePackNotInstalledForCulture", "ExB292FC", false, true, Strings.ResourceManager, new object[]
			{
				culture
			});
		}

		public static LocalizedString FreshIntroductionText
		{
			get
			{
				return new LocalizedString("FreshIntroductionText", "Ex5488AF", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString UserSpecifiedDCDoesNotExistException(string dc)
		{
			return new LocalizedString("UserSpecifiedDCDoesNotExistException", "ExE7D598", false, true, Strings.ResourceManager, new object[]
			{
				dc
			});
		}

		public static LocalizedString LocalServerNameInvalid(string name)
		{
			return new LocalizedString("LocalServerNameInvalid", "Ex834285", false, true, Strings.ResourceManager, new object[]
			{
				name
			});
		}

		public static LocalizedString UpgradeModeDataHandlerHandlersAndWorkUnits(int handlers, int workunits)
		{
			return new LocalizedString("UpgradeModeDataHandlerHandlersAndWorkUnits", "Ex10F94B", false, true, Strings.ResourceManager, new object[]
			{
				handlers,
				workunits
			});
		}

		public static LocalizedString CannotFindPath(string path)
		{
			return new LocalizedString("CannotFindPath", "Ex376692", false, true, Strings.ResourceManager, new object[]
			{
				path
			});
		}

		public static LocalizedString GCAlreadySpecified(string gc)
		{
			return new LocalizedString("GCAlreadySpecified", "ExF68485", false, true, Strings.ResourceManager, new object[]
			{
				gc
			});
		}

		public static LocalizedString UnifiedMessagingRoleDisplayName
		{
			get
			{
				return new LocalizedString("UnifiedMessagingRoleDisplayName", "ExCB5C80", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString LanguagePackPathException
		{
			get
			{
				return new LocalizedString("LanguagePackPathException", "ExC46EE5", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MsiFileNotFound(string path, string file)
		{
			return new LocalizedString("MsiFileNotFound", "Ex55B863", false, true, Strings.ResourceManager, new object[]
			{
				path,
				file
			});
		}

		public static LocalizedString AdminToolCannotBeUninstalledWhenSomeRolesRemained(string admintools)
		{
			return new LocalizedString("AdminToolCannotBeUninstalledWhenSomeRolesRemained", "ExBBE499", false, true, Strings.ResourceManager, new object[]
			{
				admintools
			});
		}

		public static LocalizedString DRRoleAlreadyInstalledError(string installedRoles)
		{
			return new LocalizedString("DRRoleAlreadyInstalledError", "Ex4104DB", false, true, Strings.ResourceManager, new object[]
			{
				installedRoles
			});
		}

		public static LocalizedString GatewayRoleDisplayName
		{
			get
			{
				return new LocalizedString("GatewayRoleDisplayName", "Ex5EE0FB", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RemoveUmLanguagePackModeDataHandlerCount(int count)
		{
			return new LocalizedString("RemoveUmLanguagePackModeDataHandlerCount", "Ex7DD30B", false, true, Strings.ResourceManager, new object[]
			{
				count
			});
		}

		public static LocalizedString WillDisableAMFiltering
		{
			get
			{
				return new LocalizedString("WillDisableAMFiltering", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString TheCurrentServerHasExchangeBits
		{
			get
			{
				return new LocalizedString("TheCurrentServerHasExchangeBits", "ExA2F1FB", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString BackupKeyInaccessible(string keyName)
		{
			return new LocalizedString("BackupKeyInaccessible", "Ex32CF3E", false, true, Strings.ResourceManager, new object[]
			{
				keyName
			});
		}

		public static LocalizedString MonitoringRoleDisplayName
		{
			get
			{
				return new LocalizedString("MonitoringRoleDisplayName", "Ex020ED3", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString OrgAlreadyHasMailboxServers
		{
			get
			{
				return new LocalizedString("OrgAlreadyHasMailboxServers", "Ex63EAA2", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString LanguagePackPathNotFoundError
		{
			get
			{
				return new LocalizedString("LanguagePackPathNotFoundError", "Ex17AD9D", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MustConfigureIPv6ForFirstSubnet
		{
			get
			{
				return new LocalizedString("MustConfigureIPv6ForFirstSubnet", "Ex864617", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString WillRestartSetupUI
		{
			get
			{
				return new LocalizedString("WillRestartSetupUI", "Ex7B8552", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString UserSpecifiedDCIsNotSchemaMasterException(string dc)
		{
			return new LocalizedString("UserSpecifiedDCIsNotSchemaMasterException", "Ex8B7120", false, true, Strings.ResourceManager, new object[]
			{
				dc
			});
		}

		public static LocalizedString RemoveFailStatus
		{
			get
			{
				return new LocalizedString("RemoveFailStatus", "ExCBE89C", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString AddConflictedRolesError
		{
			get
			{
				return new LocalizedString("AddConflictedRolesError", "ExDD7345", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString SchemaMasterAvailable
		{
			get
			{
				return new LocalizedString("SchemaMasterAvailable", "ExB09157", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString DRFailText
		{
			get
			{
				return new LocalizedString("DRFailText", "Ex53A96A", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString UmLanguagePackFileNotFound(string file)
		{
			return new LocalizedString("UmLanguagePackFileNotFound", "Ex564938", false, true, Strings.ResourceManager, new object[]
			{
				file
			});
		}

		public static LocalizedString NameValueFormat(string name, string value)
		{
			return new LocalizedString("NameValueFormat", "", false, false, Strings.ResourceManager, new object[]
			{
				name,
				value
			});
		}

		public static LocalizedString UpgradeIntroduction
		{
			get
			{
				return new LocalizedString("UpgradeIntroduction", "Ex84D44A", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString SettingOrganizationName(string name)
		{
			return new LocalizedString("SettingOrganizationName", "Ex095249", false, true, Strings.ResourceManager, new object[]
			{
				name
			});
		}

		public static LocalizedString AddPreCheckText
		{
			get
			{
				return new LocalizedString("AddPreCheckText", "Ex14D2FA", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InstallationModeSetTo(string mode)
		{
			return new LocalizedString("InstallationModeSetTo", "Ex3AFFFA", false, true, Strings.ResourceManager, new object[]
			{
				mode
			});
		}

		public static LocalizedString SourceDirNotSpecifiedError
		{
			get
			{
				return new LocalizedString("SourceDirNotSpecifiedError", "Ex5FD860", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString FreshIntroduction
		{
			get
			{
				return new LocalizedString("FreshIntroduction", "Ex714EFE", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString SchemaMasterDCNotFoundException
		{
			get
			{
				return new LocalizedString("SchemaMasterDCNotFoundException", "ExD715F3", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ADDriverBoundToAdam
		{
			get
			{
				return new LocalizedString("ADDriverBoundToAdam", "ExB03DEE", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InvalidMaximumRecordNumber(int maximumNumberToLog)
		{
			return new LocalizedString("InvalidMaximumRecordNumber", "", false, false, Strings.ResourceManager, new object[]
			{
				maximumNumberToLog
			});
		}

		public static LocalizedString CabUtilityWrapperError
		{
			get
			{
				return new LocalizedString("CabUtilityWrapperError", "ExED6BD4", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString SchemaMasterNotAvailable
		{
			get
			{
				return new LocalizedString("SchemaMasterNotAvailable", "ExAF3B47", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString NoExchangeOrganizationContainerFound(string message)
		{
			return new LocalizedString("NoExchangeOrganizationContainerFound", "", false, false, Strings.ResourceManager, new object[]
			{
				message
			});
		}

		public static LocalizedString OSPRoleDisplayName
		{
			get
			{
				return new LocalizedString("OSPRoleDisplayName", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString UpgradeLicenseAgreementShortDescription
		{
			get
			{
				return new LocalizedString("UpgradeLicenseAgreementShortDescription", "ExAE3253", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString AddLanguagePacksLogFilePath(string path)
		{
			return new LocalizedString("AddLanguagePacksLogFilePath", "Ex6A1CD4", false, true, Strings.ResourceManager, new object[]
			{
				path
			});
		}

		public static LocalizedString DRSuccessStatus
		{
			get
			{
				return new LocalizedString("DRSuccessStatus", "Ex9EE99D", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString DRModeDataHandlerCount(int count)
		{
			return new LocalizedString("DRModeDataHandlerCount", "Ex274B3E", false, true, Strings.ResourceManager, new object[]
			{
				count
			});
		}

		public static LocalizedString ExchangeOrganizationContainerName(string name)
		{
			return new LocalizedString("ExchangeOrganizationContainerName", "Ex99A953", false, true, Strings.ResourceManager, new object[]
			{
				name
			});
		}

		public static LocalizedString CopyFileText
		{
			get
			{
				return new LocalizedString("CopyFileText", "ExFC3446", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString WillNotStartTransportService
		{
			get
			{
				return new LocalizedString("WillNotStartTransportService", "ExE9F845", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RemoveMESOObjectLink
		{
			get
			{
				return new LocalizedString("RemoveMESOObjectLink", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString GetLocalizedString(Strings.IDs key)
		{
			return new LocalizedString(Strings.stringIDs[(uint)key], Strings.ResourceManager, new object[0]);
		}

		private static Dictionary<uint, string> stringIDs = new Dictionary<uint, string>(175);

		private static ExchangeResourceManager ResourceManager = ExchangeResourceManager.GetResourceManager("Microsoft.Exchange.Setup.Common.Strings", typeof(Strings).GetTypeInfo().Assembly);

		public enum IDs : uint
		{
			ChoosingGlobalCatalog = 436210406U,
			LanguagePacksUpToDate = 4081297114U,
			PostSetupText = 2029598378U,
			CouldNotDeserializeStateFile = 3976870610U,
			AddLanguagePacksSuccessText = 590640951U,
			CopyLanguagePacksDisplayName = 944044270U,
			UmLanguagePacksToRemove = 800186353U,
			DRServerRoleText = 3870436312U,
			AddSuccessText = 2394569239U,
			RemoveIntroductionText = 4289798847U,
			HasConfiguredRoles = 406751805U,
			AddUmLanguagePackText = 776586043U,
			SetupRebootRequired = 4258824323U,
			UmLanguagePackDisplayName = 3988419612U,
			UnifiedMessagingRoleIsRequiredForLanguagePackInstalls = 318591995U,
			FrontendTransportRoleDisplayName = 3603864464U,
			UpgradePrereq = 3211475425U,
			CentralAdminFrontEndRoleDisplayName = 3150196311U,
			PreConfigurationDisplayName = 1623683896U,
			RemoveFailText = 523317789U,
			AddSuccessStatus = 1831888326U,
			PickingDomainController = 1377162733U,
			UmLanguagePacksToAdd = 237451456U,
			LPVersioningInvalidValue = 3712765806U,
			LanguagePacksPackagePathNotSpecified = 1055111548U,
			AddRolesToInstall = 2144854482U,
			AddOtherRolesError = 2493777864U,
			NeedConfigureIpv6ForSecondSubnetWhenFirstSubnetConfiguresIPV6 = 35176418U,
			ModeErrorForDisasterRecovery = 4234837096U,
			CannotSpecifyIndustryTypeWhenOrgIsUpToDateDuringServerInstallation = 3904063070U,
			RemoveUmLanguagePackFailText = 2285919320U,
			ChoosingDomainController = 1771160928U,
			CentralAdminRoleDisplayName = 3069688065U,
			NeedConfigureIpv4StaticAddressForSecondSubnetWhenFirstSubnetConfiguresIPV4StaticAddress = 2768015112U,
			PreSetupText = 1833305185U,
			ModeErrorForUpgrade = 2815048144U,
			OrganizationInstallText = 261522387U,
			CannotSpecifyServerCEIPWhenMachineIsNotCleanDuringServerInstallation = 415144177U,
			LanguagePacksToInstall = 1893181714U,
			RemoveSuccessStatus = 1778403425U,
			UnableToFindLPVersioning = 4061842413U,
			CannotRemoveEnglishUSLanguagePack = 1048539684U,
			LanguagePacksToAdd = 3469958538U,
			UpgradeRolesToInstall = 3908217517U,
			WillSkipAMEngineDownloadCheck = 3473605283U,
			WillGetConfiguredRolesFromRegistry = 1841681484U,
			MailboxRoleDisplayName = 3319509477U,
			EnglishUSLanguagePackInstalled = 3566628291U,
			NoRoleSelectedForUninstall = 2515305841U,
			MustEnableLegacyOutlook = 2876792450U,
			RemoveDatacenterFileText = 1610115158U,
			CannotSpecifyCEIPWhenOrganizationHasE14OrLaterServersDuringPrepareAD = 4024296328U,
			OrgAlreadyHasBridgeheadServers = 1416621619U,
			PrerequisiteAnalysis = 176205814U,
			NeedConfigureIpv4ForSecondSubnetWhenFirstSubnetConfiguresIPV4 = 1959016418U,
			RemovePreCheckText = 379286002U,
			RemoveUmLanguagePackSuccessText = 590609003U,
			DeterminingOrgPrepParameters = 1869709987U,
			RemoveProgressText = 3671143606U,
			RemoveUmLanguagePackText = 1281959852U,
			DRServerNotFoundInAD = 2770538346U,
			CannotSpecifyServerCEIPWhenGlobalCEIPIsOptedOutDuringServerInstallation = 1120765526U,
			ServerOptDescriptionText = 3535416533U,
			InstallationPathNotSet = 2156205750U,
			UnableToFindBuildVersion = 871634395U,
			AdminToolsRoleDisplayName = 3002787153U,
			UmLanguagePackPackagePathNotSpecified = 3235153535U,
			AddCannotChangeTargetDirectoryError = 1136733772U,
			EdgeRoleInstalledButServerObjectNotFound = 2335571147U,
			LanguagePacksNotFound = 1016568891U,
			ApplyingDefaultRoleSelectionState = 2203024520U,
			MustConfigureIPv4ForFirstSubnet = 2001599932U,
			LanguagePacksLogFilePathNotSpecified = 2161842580U,
			OrganizationPrereqText = 1640598683U,
			AddFailText = 3360917926U,
			RemoveRolesToInstall = 1124277255U,
			NeedConfigureIpv4DHCPForSecondSubnetWhenFirstSubnetConfiguresIPV4DHCP = 968377506U,
			MidFileCopyText = 1772477316U,
			CentralAdminDatabaseRoleDisplayName = 2913262092U,
			AddGatewayAloneError = 3003755610U,
			CafeRoleDisplayName = 55657320U,
			NoServerRolesToInstall = 3106188069U,
			DRPrereq = 3438721327U,
			ModeErrorForFreshInstall = 2059503147U,
			LegacyRoutingServerNotValid = 1931019259U,
			CopyDatacenterFileText = 1405894953U,
			UnsupportedMode = 3288868438U,
			ExchangeOrganizationNameRequired = 1947354702U,
			UpgradePreCheckText = 1144432722U,
			Prereqs = 55662122U,
			FailedToReadLCIDFromRegistryError = 724167955U,
			LanguagePacksDisplayName = 2114641061U,
			SetupExitsBecauseOfTransientException = 3856638130U,
			DRSuccessText = 3783483884U,
			DRRolesToInstall = 2041466059U,
			ParametersForTheTaskTitle = 3915936117U,
			ADRelatedUnknownError = 2030347010U,
			AddFailStatus = 16560395U,
			AddLanguagePacksFailText = 1439738700U,
			LanguagePacksInstalledVersionNull = 4069246837U,
			SetupExitsBecauseOfLPPathNotFoundException = 1695043038U,
			ServerIsProvisioned = 4175779855U,
			NoUmLanguagePackSpecified = 3668657674U,
			PostFileCopyText = 209657916U,
			ExecutionCompleted = 2127358907U,
			UnknownPreviousVersion = 3830028115U,
			RemoveFileText = 3751538685U,
			MSIIsCurrent = 2949666774U,
			AddPrereq = 139720102U,
			RemoveServerRoleText = 3212518314U,
			DoesNotSupportCEIPForAdminOnlyInstallation = 1066611882U,
			AddLanguagePacksText = 4291904420U,
			DRPreCheckText = 155941904U,
			AddProgressText = 357116433U,
			DRFailStatus = 1940285462U,
			RemoveUmLanguagePackFailStatus = 1540748327U,
			UpgradeIntroductionText = 2888945725U,
			NoUmLanguagePackInstalled = 3353325242U,
			RemoveUnifiedMessagingServerDescription = 806279811U,
			ClientAccessRoleDisplayName = 3229808788U,
			UpgradeFailStatus = 2809596612U,
			NoRoleSelectedForInstall = 1627958870U,
			ExecutionError = 1165007882U,
			InstallationLicenseAgreementShortDescription = 3076164745U,
			RemoveSuccessText = 3635986314U,
			InstalledLanguageComment = 1223959701U,
			UpgradeFailText = 3791960239U,
			PreFileCopyText = 2036054501U,
			UnknownDestinationPath = 3001810281U,
			UpgradeServerRoleText = 3520674066U,
			GlobalOptDescriptionText = 2572849171U,
			AddServerRoleText = 1835758957U,
			MaintenanceIntroduction = 213042017U,
			UmLanguagePathLogFilePathNotSpecified = 3361199071U,
			UpgradeProgressText = 1232809750U,
			ADHasBeenPrepared = 2702857678U,
			SpecifyExchangeOrganizationName = 783977028U,
			UpgradeMustUseBootStrapper = 2915089885U,
			AddIntroductionText = 1655961090U,
			UpgradeSuccessStatus = 522237759U,
			LegacyServerNameRequired = 48624620U,
			TheCurrentServerHasNoExchangeBits = 2979515445U,
			UpgradeSuccessText = 4193588254U,
			BridgeheadRoleDisplayName = 1134594294U,
			WaitForForestPrepReplicationToLocalDomainException = 966578211U,
			RemovePrereq = 49080419U,
			FreshIntroductionText = 915558695U,
			UnifiedMessagingRoleDisplayName = 126744571U,
			LanguagePackPathException = 186454365U,
			GatewayRoleDisplayName = 2320733697U,
			WillDisableAMFiltering = 3864515574U,
			TheCurrentServerHasExchangeBits = 4222861002U,
			MonitoringRoleDisplayName = 2368998453U,
			OrgAlreadyHasMailboxServers = 3224205240U,
			LanguagePackPathNotFoundError = 3656576577U,
			MustConfigureIPv6ForFirstSubnet = 1435960826U,
			WillRestartSetupUI = 2370029120U,
			RemoveFailStatus = 3059971080U,
			AddConflictedRolesError = 2070315917U,
			SchemaMasterAvailable = 3504188790U,
			DRFailText = 2214617625U,
			UpgradeIntroduction = 1095004458U,
			AddPreCheckText = 1616603373U,
			SourceDirNotSpecifiedError = 3338459901U,
			FreshIntroduction = 3315256202U,
			SchemaMasterDCNotFoundException = 2192506430U,
			ADDriverBoundToAdam = 2107252035U,
			CabUtilityWrapperError = 1331356275U,
			SchemaMasterNotAvailable = 4044548133U,
			OSPRoleDisplayName = 2024338705U,
			UpgradeLicenseAgreementShortDescription = 2927231269U,
			DRSuccessStatus = 759658685U,
			CopyFileText = 2340024118U,
			WillNotStartTransportService = 302515857U,
			RemoveMESOObjectLink = 3504655211U
		}

		private enum ParamIDs
		{
			WillSearchForAServerObjectForLocalServer,
			TargetDirectoryCannotBeChanged,
			LanguagePackPackagePath,
			ValidatingOptionsForRoles,
			ADSchemaVersionHigherThanSetupException,
			UserSpecifiedDCIsNotInLocalDomainException,
			DeserializedStateXML,
			InstallationPathInvalidDriveTypeInformation,
			SettingArgumentBecauseItIsRequired,
			NotEnoughSpace,
			RoleInstalledOnServer,
			ExchangeVersionInvalid,
			ExchangeOrganizationAlreadyExists,
			InstallationPathInvalidProfilesInformation,
			ServerNotFound,
			DomainControllerChosen,
			UserSpecifiedTargetDir,
			NotAValidFqdn,
			HighLevelTaskStarted,
			UmLanguagePackPackagePath,
			UserSpecifiedDCIsNotAvailableException,
			RootDataHandlerCount,
			SettingArgumentToValue,
			TheFirstPathUnderTheSecondPath,
			UnableToFindBuildVersion1,
			BackupPath,
			PackagePathSetTo,
			ADRelatedError,
			AlwaysWatsonForDebug,
			LanguagePacksPackagePathNotFound,
			UnifiedMessagingLanguagePackInstalled,
			InstalledVersion,
			SetupWillUseDomainController,
			AddRoleAlreadyInstalledError,
			CommandLineParameterSpecified,
			CannotFindFile,
			ParameterNotValidForCurrentRoles,
			ExchangeServerFound,
			AdInitializationStatus,
			InstallationPathUnderUserProfileInformation,
			UmLanguagePackDirectoryNotAvailable,
			UpgradeRoleNotInstalledError,
			MsiDirectoryNotFound,
			ValidatingRoleOptions,
			UnexpectedFileFromBundle,
			DisplayServerName,
			InvalidFqdn,
			TargetInstallationDirectory,
			AddUmLanguagePackModeDataHandlerCount,
			RunForestPrepInSchemaMasterDomainException,
			LanguagePackagePathSetTo,
			SettingArgumentBecauseOfCommandLineParameter,
			SetupNotFoundInSourceDirError,
			SchemaUpdateRequired,
			VersionMismatchWarning,
			DCNotResponding,
			ForestPrepAlreadyRun,
			DCNotInLocalDomain,
			RemoveUmLanguagePackLogFilePath,
			AddUmLanguagePackLogFilePath,
			ExistingOrganizationName,
			ExceptionWhenDeserializingStateFile,
			LanguagePacksVersionFileNotFound,
			StateFileVersionMismatch,
			NoConfigurationInfoFoundForInstallableUnit,
			SetupWillRunFromPath,
			ADDomainConfigVersionHigherThanSetupException,
			RemoveLanguagePacksLogFilePath,
			WarningUpperCase,
			UmLanguagePackFullPackagePath,
			RoleNotInstalledError,
			ErrorUpperCase,
			LanguagePacksBadVersionFormat,
			NoSchemaEntry,
			InstallationPathInvalidDriveFormatInformation,
			NoExchangeConfigurationContainerFound,
			OrganizationAlreadyExists,
			DCAlreadySpecified,
			ExchangeServerNotFound,
			SchemaMasterDCNotAvailableException,
			BackupKeyIsWrongType,
			CannotInstallDatacenterRole,
			PersistedDomainController,
			DomainConfigUpdateRequired,
			WillSearchForAServerObjectForServer,
			AddedConfigurationInfoForInstallableUnit,
			InstallationPathInvalidRootDriveInformation,
			ADOrgConfigVersionHigherThanSetupException,
			AttemptToSearchExchangeServerFailed,
			UninstallModeDataHandlerHandlersAndWorkUnits,
			CommandLine,
			SetupSourceDirectory,
			UmLanguagePackNotFoundForCulture,
			UmLanguagePackDisplayNameWithCulture,
			MsiFilesDirectoryCannotBeChanged,
			ExchangeConfigurationContainerName,
			ForestPrepNotRunOrNotReplicatedException,
			LPVersioningExtractionFailed,
			MSINotPresent,
			SetupWillUseGlobalCatalog,
			NotALegacyServer,
			WillGetConfiguredRolesFromServerObject,
			UninstallModeDataHandlerCount,
			WillExecuteHighLevelTask,
			BackupVersion,
			OrgConfigUpdateRequired,
			InstallModeDataHandlerCount,
			DCNameNotValid,
			GCChosen,
			SchemaMasterIsLocalDC,
			InstallationPathInvalidInformation,
			InvalidOrganizationName,
			NonCultureRegistryEntryFound,
			ForestPrepNotRun,
			InvalidExchangeOrganizationName,
			UmLanguagePackNotInstalledForCulture,
			UserSpecifiedDCDoesNotExistException,
			LocalServerNameInvalid,
			UpgradeModeDataHandlerHandlersAndWorkUnits,
			CannotFindPath,
			GCAlreadySpecified,
			MsiFileNotFound,
			AdminToolCannotBeUninstalledWhenSomeRolesRemained,
			DRRoleAlreadyInstalledError,
			RemoveUmLanguagePackModeDataHandlerCount,
			BackupKeyInaccessible,
			UserSpecifiedDCIsNotSchemaMasterException,
			UmLanguagePackFileNotFound,
			NameValueFormat,
			SettingOrganizationName,
			InstallationModeSetTo,
			InvalidMaximumRecordNumber,
			NoExchangeOrganizationContainerFound,
			AddLanguagePacksLogFilePath,
			DRModeDataHandlerCount,
			ExchangeOrganizationContainerName
		}
	}
}
