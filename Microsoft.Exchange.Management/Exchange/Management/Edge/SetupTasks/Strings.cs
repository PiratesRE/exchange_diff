using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.Edge.SetupTasks
{
	internal static class Strings
	{
		static Strings()
		{
			Strings.stringIDs.Add(774587054U, "GatewayRoleIsNotUnpacked");
			Strings.stringIDs.Add(2478794571U, "AntispamUpdateServiceDescription");
			Strings.stringIDs.Add(988899670U, "AdamServiceFailedToUninstall");
			Strings.stringIDs.Add(2393646509U, "TransportLogSearchServiceDisplayName");
			Strings.stringIDs.Add(175509160U, "UninstallEdgeTransportServiceTask");
			Strings.stringIDs.Add(4124898948U, "UninstallEdgeSyncServiceTask");
			Strings.stringIDs.Add(2736467170U, "AntispamUpdateServiceDisplayName");
			Strings.stringIDs.Add(1953906065U, "InstallAuditTask");
			Strings.stringIDs.Add(2960500387U, "EdgeTransportServiceNotUninstalled");
			Strings.stringIDs.Add(4049824753U, "InstallEdgeTransportServiceTask");
			Strings.stringIDs.Add(2949158791U, "EdgeTransportServiceDescription");
			Strings.stringIDs.Add(2685419127U, "ServiceAlreadyInstalled");
			Strings.stringIDs.Add(576724013U, "SslPortSameAsLdapPort");
			Strings.stringIDs.Add(159382586U, "EdgeTransportServiceNotInstalled");
			Strings.stringIDs.Add(4271619264U, "TransportLogSearchServiceDescription");
			Strings.stringIDs.Add(1444113173U, "InstallEdgeSyncServiceTask");
			Strings.stringIDs.Add(3376781594U, "TransportLogSearchServiceNotUninstalled");
			Strings.stringIDs.Add(2900475730U, "EdgeCredentialServiceDisplayName");
			Strings.stringIDs.Add(767347686U, "UninstallAdamTask");
			Strings.stringIDs.Add(3473798678U, "UninstallAntispamUpdateServiceTask");
			Strings.stringIDs.Add(399958817U, "AdamServiceFailedToInstall");
			Strings.stringIDs.Add(4113916114U, "AntimalwareServiceDescription");
			Strings.stringIDs.Add(3105007247U, "EdgeSyncServiceDescription");
			Strings.stringIDs.Add(683215962U, "InstallAntimalwareServiceTask");
			Strings.stringIDs.Add(3584000439U, "PackagePath");
			Strings.stringIDs.Add(2829653254U, "FmsServiceNotInstalled");
			Strings.stringIDs.Add(871484781U, "InstallFmsServiceTask");
			Strings.stringIDs.Add(925743836U, "FmsServiceDisplayName");
			Strings.stringIDs.Add(2657970248U, "UninstallFmsServiceTask");
			Strings.stringIDs.Add(2391764367U, "EdgeCredentialServiceDescription");
			Strings.stringIDs.Add(3216105370U, "UninstallAuditTask");
			Strings.stringIDs.Add(292208835U, "FmsServiceNotUninstalled");
			Strings.stringIDs.Add(2034142329U, "InstallAntispamUpdateServiceTask");
			Strings.stringIDs.Add(1498913134U, "EdgeSyncServiceNotInstalled");
			Strings.stringIDs.Add(140982033U, "InvalidLdapFileName");
			Strings.stringIDs.Add(1307180415U, "AntimalwareServiceNotInstalled");
			Strings.stringIDs.Add(4193735315U, "AntimalwareServiceDisplayName");
			Strings.stringIDs.Add(1415188871U, "FmsServiceDescription");
			Strings.stringIDs.Add(3248437725U, "AdamServiceNotInstalled");
			Strings.stringIDs.Add(2855011956U, "EdgeSyncServiceDisplayName");
			Strings.stringIDs.Add(3104026373U, "UninstallAntimalwareServiceTask");
			Strings.stringIDs.Add(2915042048U, "InstallAdamSchemaTask");
			Strings.stringIDs.Add(3430669213U, "Port");
			Strings.stringIDs.Add(1957953568U, "EdgeCredentialServiceNotInstalled");
			Strings.stringIDs.Add(3698153928U, "EdgeTransportServiceDisplayName");
			Strings.stringIDs.Add(1795457964U, "AntimalwareServiceNotUninstalled");
			Strings.stringIDs.Add(1454104731U, "EdgeCredentialServiceNotUninstalled");
			Strings.stringIDs.Add(2145801173U, "InstallAdamTask");
			Strings.stringIDs.Add(4104091810U, "AdamServiceDescription");
			Strings.stringIDs.Add(303188103U, "AdamInstallFailureDataOrLogFolderNotEmpty");
			Strings.stringIDs.Add(2126302003U, "NoPathArgument");
			Strings.stringIDs.Add(2777897381U, "TransportLogSearchServiceNotInstalled");
			Strings.stringIDs.Add(1507908660U, "InstallDir");
			Strings.stringIDs.Add(2869926485U, "AdamServiceDisplayName");
			Strings.stringIDs.Add(523929713U, "InvalidPackagePathValue");
			Strings.stringIDs.Add(830945707U, "EdgeSyncServiceNotUninstalled");
			Strings.stringIDs.Add(588066505U, "UninstallOldEdgeTransportServiceTask");
		}

		public static LocalizedString GatewayRoleIsNotUnpacked
		{
			get
			{
				return new LocalizedString("GatewayRoleIsNotUnpacked", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString AntispamUpdateServiceDescription
		{
			get
			{
				return new LocalizedString("AntispamUpdateServiceDescription", "ExAC0AF2", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString AdamServiceFailedToUninstall
		{
			get
			{
				return new LocalizedString("AdamServiceFailedToUninstall", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString TransportLogSearchServiceDisplayName
		{
			get
			{
				return new LocalizedString("TransportLogSearchServiceDisplayName", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString UninstallEdgeTransportServiceTask
		{
			get
			{
				return new LocalizedString("UninstallEdgeTransportServiceTask", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString UninstallEdgeSyncServiceTask
		{
			get
			{
				return new LocalizedString("UninstallEdgeSyncServiceTask", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString AntispamUpdateServiceDisplayName
		{
			get
			{
				return new LocalizedString("AntispamUpdateServiceDisplayName", "ExACD0AC", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString PathTooLong(string path)
		{
			return new LocalizedString("PathTooLong", "", false, false, Strings.ResourceManager, new object[]
			{
				path
			});
		}

		public static LocalizedString AdamUninstallError(string error)
		{
			return new LocalizedString("AdamUninstallError", "", false, false, Strings.ResourceManager, new object[]
			{
				error
			});
		}

		public static LocalizedString InstallAuditTask
		{
			get
			{
				return new LocalizedString("InstallAuditTask", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString EdgeTransportServiceNotUninstalled
		{
			get
			{
				return new LocalizedString("EdgeTransportServiceNotUninstalled", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InvalidAdamInstanceName(string instanceName)
		{
			return new LocalizedString("InvalidAdamInstanceName", "", false, false, Strings.ResourceManager, new object[]
			{
				instanceName
			});
		}

		public static LocalizedString InstallEdgeTransportServiceTask
		{
			get
			{
				return new LocalizedString("InstallEdgeTransportServiceTask", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString EdgeTransportServiceDescription
		{
			get
			{
				return new LocalizedString("EdgeTransportServiceDescription", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ServiceAlreadyInstalled
		{
			get
			{
				return new LocalizedString("ServiceAlreadyInstalled", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString SslPortSameAsLdapPort
		{
			get
			{
				return new LocalizedString("SslPortSameAsLdapPort", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString EdgeTransportServiceNotInstalled
		{
			get
			{
				return new LocalizedString("EdgeTransportServiceNotInstalled", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString AdamUninstallGeneralFailureWithResult(int adamErrorCode)
		{
			return new LocalizedString("AdamUninstallGeneralFailureWithResult", "", false, false, Strings.ResourceManager, new object[]
			{
				adamErrorCode
			});
		}

		public static LocalizedString TransportLogSearchServiceDescription
		{
			get
			{
				return new LocalizedString("TransportLogSearchServiceDescription", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InvalidCharsInPath(string path)
		{
			return new LocalizedString("InvalidCharsInPath", "", false, false, Strings.ResourceManager, new object[]
			{
				path
			});
		}

		public static LocalizedString OccupiedPortsInformation(string processOutput)
		{
			return new LocalizedString("OccupiedPortsInformation", "", false, false, Strings.ResourceManager, new object[]
			{
				processOutput
			});
		}

		public static LocalizedString InstallEdgeSyncServiceTask
		{
			get
			{
				return new LocalizedString("InstallEdgeSyncServiceTask", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InvalidDriveInPath(string path)
		{
			return new LocalizedString("InvalidDriveInPath", "", false, false, Strings.ResourceManager, new object[]
			{
				path
			});
		}

		public static LocalizedString AdamInstallError(string error)
		{
			return new LocalizedString("AdamInstallError", "", false, false, Strings.ResourceManager, new object[]
			{
				error
			});
		}

		public static LocalizedString TransportLogSearchServiceNotUninstalled
		{
			get
			{
				return new LocalizedString("TransportLogSearchServiceNotUninstalled", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString EdgeCredentialServiceDisplayName
		{
			get
			{
				return new LocalizedString("EdgeCredentialServiceDisplayName", "Ex3A1FF6", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString UninstallAdamTask
		{
			get
			{
				return new LocalizedString("UninstallAdamTask", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString PortIsAvailable(int port)
		{
			return new LocalizedString("PortIsAvailable", "", false, false, Strings.ResourceManager, new object[]
			{
				port
			});
		}

		public static LocalizedString UninstallAntispamUpdateServiceTask
		{
			get
			{
				return new LocalizedString("UninstallAntispamUpdateServiceTask", "Ex512CDC", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString AdamInstallWarning(string warning)
		{
			return new LocalizedString("AdamInstallWarning", "", false, false, Strings.ResourceManager, new object[]
			{
				warning
			});
		}

		public static LocalizedString CouldNotConnectToAdamService(string instanceName)
		{
			return new LocalizedString("CouldNotConnectToAdamService", "", false, false, Strings.ResourceManager, new object[]
			{
				instanceName
			});
		}

		public static LocalizedString AdamServiceFailedToInstall
		{
			get
			{
				return new LocalizedString("AdamServiceFailedToInstall", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString AdamUninstallProcessFailure(string processName, int exitCode)
		{
			return new LocalizedString("AdamUninstallProcessFailure", "", false, false, Strings.ResourceManager, new object[]
			{
				processName,
				exitCode
			});
		}

		public static LocalizedString AntimalwareServiceDescription
		{
			get
			{
				return new LocalizedString("AntimalwareServiceDescription", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString EdgeSyncServiceDescription
		{
			get
			{
				return new LocalizedString("EdgeSyncServiceDescription", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InstallAntimalwareServiceTask
		{
			get
			{
				return new LocalizedString("InstallAntimalwareServiceTask", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString CouldNotGetInfoToConnectToAdamService(string instanceName)
		{
			return new LocalizedString("CouldNotGetInfoToConnectToAdamService", "", false, false, Strings.ResourceManager, new object[]
			{
				instanceName
			});
		}

		public static LocalizedString PackagePath
		{
			get
			{
				return new LocalizedString("PackagePath", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString FmsServiceNotInstalled
		{
			get
			{
				return new LocalizedString("FmsServiceNotInstalled", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InstallFmsServiceTask
		{
			get
			{
				return new LocalizedString("InstallFmsServiceTask", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InvalidServicesDependedOn(string serviceName)
		{
			return new LocalizedString("InvalidServicesDependedOn", "", false, false, Strings.ResourceManager, new object[]
			{
				serviceName
			});
		}

		public static LocalizedString FmsServiceDisplayName
		{
			get
			{
				return new LocalizedString("FmsServiceDisplayName", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString UninstallFmsServiceTask
		{
			get
			{
				return new LocalizedString("UninstallFmsServiceTask", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString PortIsBusy(int port)
		{
			return new LocalizedString("PortIsBusy", "", false, false, Strings.ResourceManager, new object[]
			{
				port
			});
		}

		public static LocalizedString EdgeCredentialServiceDescription
		{
			get
			{
				return new LocalizedString("EdgeCredentialServiceDescription", "Ex682626", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString UninstallAuditTask
		{
			get
			{
				return new LocalizedString("UninstallAuditTask", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString AdamInstallProcessFailure(string processName, int exitCode)
		{
			return new LocalizedString("AdamInstallProcessFailure", "", false, false, Strings.ResourceManager, new object[]
			{
				processName,
				exitCode
			});
		}

		public static LocalizedString FmsServiceNotUninstalled
		{
			get
			{
				return new LocalizedString("FmsServiceNotUninstalled", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InstallAntispamUpdateServiceTask
		{
			get
			{
				return new LocalizedString("InstallAntispamUpdateServiceTask", "ExDA2FEA", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString EdgeSyncServiceNotInstalled
		{
			get
			{
				return new LocalizedString("EdgeSyncServiceNotInstalled", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString AdamSchemaImportProcessFailure(string processName, int exitCode)
		{
			return new LocalizedString("AdamSchemaImportProcessFailure", "", false, false, Strings.ResourceManager, new object[]
			{
				processName,
				exitCode
			});
		}

		public static LocalizedString AdamSchemaImportFailure(string error)
		{
			return new LocalizedString("AdamSchemaImportFailure", "", false, false, Strings.ResourceManager, new object[]
			{
				error
			});
		}

		public static LocalizedString InvalidLdapFileName
		{
			get
			{
				return new LocalizedString("InvalidLdapFileName", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString AntimalwareServiceNotInstalled
		{
			get
			{
				return new LocalizedString("AntimalwareServiceNotInstalled", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString AntimalwareServiceDisplayName
		{
			get
			{
				return new LocalizedString("AntimalwareServiceDisplayName", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString AdamSetAclsProcessFailure(string processName, int exitCode, string dn)
		{
			return new LocalizedString("AdamSetAclsProcessFailure", "", false, false, Strings.ResourceManager, new object[]
			{
				processName,
				exitCode,
				dn
			});
		}

		public static LocalizedString FmsServiceDescription
		{
			get
			{
				return new LocalizedString("FmsServiceDescription", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString AdamUninstallWarning(string warning)
		{
			return new LocalizedString("AdamUninstallWarning", "", false, false, Strings.ResourceManager, new object[]
			{
				warning
			});
		}

		public static LocalizedString ReadOnlyPath(string path)
		{
			return new LocalizedString("ReadOnlyPath", "", false, false, Strings.ResourceManager, new object[]
			{
				path
			});
		}

		public static LocalizedString AdamServiceNotInstalled
		{
			get
			{
				return new LocalizedString("AdamServiceNotInstalled", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString EdgeSyncServiceDisplayName
		{
			get
			{
				return new LocalizedString("EdgeSyncServiceDisplayName", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString UninstallAntimalwareServiceTask
		{
			get
			{
				return new LocalizedString("UninstallAntimalwareServiceTask", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InstallAdamSchemaTask
		{
			get
			{
				return new LocalizedString("InstallAdamSchemaTask", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString Port
		{
			get
			{
				return new LocalizedString("Port", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString EdgeCredentialServiceNotInstalled
		{
			get
			{
				return new LocalizedString("EdgeCredentialServiceNotInstalled", "Ex77C32C", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString EdgeTransportServiceDisplayName
		{
			get
			{
				return new LocalizedString("EdgeTransportServiceDisplayName", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString AntimalwareServiceNotUninstalled
		{
			get
			{
				return new LocalizedString("AntimalwareServiceNotUninstalled", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString EdgeCredentialServiceNotUninstalled
		{
			get
			{
				return new LocalizedString("EdgeCredentialServiceNotUninstalled", "Ex2EADCA", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InstallAdamTask
		{
			get
			{
				return new LocalizedString("InstallAdamTask", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString AdamServiceDescription
		{
			get
			{
				return new LocalizedString("AdamServiceDescription", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString AdamInstallFailureDataOrLogFolderNotEmpty
		{
			get
			{
				return new LocalizedString("AdamInstallFailureDataOrLogFolderNotEmpty", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString NoPathArgument
		{
			get
			{
				return new LocalizedString("NoPathArgument", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString LogProcessExitCode(string processName, int exitCode)
		{
			return new LocalizedString("LogProcessExitCode", "", false, false, Strings.ResourceManager, new object[]
			{
				processName,
				exitCode
			});
		}

		public static LocalizedString TransportLogSearchServiceNotInstalled
		{
			get
			{
				return new LocalizedString("TransportLogSearchServiceNotInstalled", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InstallDir
		{
			get
			{
				return new LocalizedString("InstallDir", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString AdamServiceDisplayName
		{
			get
			{
				return new LocalizedString("AdamServiceDisplayName", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString NoPermissionsForPath(string path)
		{
			return new LocalizedString("NoPermissionsForPath", "", false, false, Strings.ResourceManager, new object[]
			{
				path
			});
		}

		public static LocalizedString InvalidPackagePathValue
		{
			get
			{
				return new LocalizedString("InvalidPackagePathValue", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString AdamInstallGeneralFailureWithResult(int adamErrorCode)
		{
			return new LocalizedString("AdamInstallGeneralFailureWithResult", "", false, false, Strings.ResourceManager, new object[]
			{
				adamErrorCode
			});
		}

		public static LocalizedString LogRunningCommand(string processName, string args)
		{
			return new LocalizedString("LogRunningCommand", "", false, false, Strings.ResourceManager, new object[]
			{
				processName,
				args
			});
		}

		public static LocalizedString AdamFailedSetServiceArgs(string processName, int exitCode, string argument)
		{
			return new LocalizedString("AdamFailedSetServiceArgs", "", false, false, Strings.ResourceManager, new object[]
			{
				processName,
				exitCode,
				argument
			});
		}

		public static LocalizedString EdgeSyncServiceNotUninstalled
		{
			get
			{
				return new LocalizedString("EdgeSyncServiceNotUninstalled", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString UninstallOldEdgeTransportServiceTask
		{
			get
			{
				return new LocalizedString("UninstallOldEdgeTransportServiceTask", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InvalidPortNumber(int port)
		{
			return new LocalizedString("InvalidPortNumber", "", false, false, Strings.ResourceManager, new object[]
			{
				port
			});
		}

		public static LocalizedString GetLocalizedString(Strings.IDs key)
		{
			return new LocalizedString(Strings.stringIDs[(uint)key], Strings.ResourceManager, new object[0]);
		}

		private static Dictionary<uint, string> stringIDs = new Dictionary<uint, string>(57);

		private static ExchangeResourceManager ResourceManager = ExchangeResourceManager.GetResourceManager("Microsoft.Exchange.Management.EdgeSetupStrings", typeof(Strings).GetTypeInfo().Assembly);

		public enum IDs : uint
		{
			GatewayRoleIsNotUnpacked = 774587054U,
			AntispamUpdateServiceDescription = 2478794571U,
			AdamServiceFailedToUninstall = 988899670U,
			TransportLogSearchServiceDisplayName = 2393646509U,
			UninstallEdgeTransportServiceTask = 175509160U,
			UninstallEdgeSyncServiceTask = 4124898948U,
			AntispamUpdateServiceDisplayName = 2736467170U,
			InstallAuditTask = 1953906065U,
			EdgeTransportServiceNotUninstalled = 2960500387U,
			InstallEdgeTransportServiceTask = 4049824753U,
			EdgeTransportServiceDescription = 2949158791U,
			ServiceAlreadyInstalled = 2685419127U,
			SslPortSameAsLdapPort = 576724013U,
			EdgeTransportServiceNotInstalled = 159382586U,
			TransportLogSearchServiceDescription = 4271619264U,
			InstallEdgeSyncServiceTask = 1444113173U,
			TransportLogSearchServiceNotUninstalled = 3376781594U,
			EdgeCredentialServiceDisplayName = 2900475730U,
			UninstallAdamTask = 767347686U,
			UninstallAntispamUpdateServiceTask = 3473798678U,
			AdamServiceFailedToInstall = 399958817U,
			AntimalwareServiceDescription = 4113916114U,
			EdgeSyncServiceDescription = 3105007247U,
			InstallAntimalwareServiceTask = 683215962U,
			PackagePath = 3584000439U,
			FmsServiceNotInstalled = 2829653254U,
			InstallFmsServiceTask = 871484781U,
			FmsServiceDisplayName = 925743836U,
			UninstallFmsServiceTask = 2657970248U,
			EdgeCredentialServiceDescription = 2391764367U,
			UninstallAuditTask = 3216105370U,
			FmsServiceNotUninstalled = 292208835U,
			InstallAntispamUpdateServiceTask = 2034142329U,
			EdgeSyncServiceNotInstalled = 1498913134U,
			InvalidLdapFileName = 140982033U,
			AntimalwareServiceNotInstalled = 1307180415U,
			AntimalwareServiceDisplayName = 4193735315U,
			FmsServiceDescription = 1415188871U,
			AdamServiceNotInstalled = 3248437725U,
			EdgeSyncServiceDisplayName = 2855011956U,
			UninstallAntimalwareServiceTask = 3104026373U,
			InstallAdamSchemaTask = 2915042048U,
			Port = 3430669213U,
			EdgeCredentialServiceNotInstalled = 1957953568U,
			EdgeTransportServiceDisplayName = 3698153928U,
			AntimalwareServiceNotUninstalled = 1795457964U,
			EdgeCredentialServiceNotUninstalled = 1454104731U,
			InstallAdamTask = 2145801173U,
			AdamServiceDescription = 4104091810U,
			AdamInstallFailureDataOrLogFolderNotEmpty = 303188103U,
			NoPathArgument = 2126302003U,
			TransportLogSearchServiceNotInstalled = 2777897381U,
			InstallDir = 1507908660U,
			AdamServiceDisplayName = 2869926485U,
			InvalidPackagePathValue = 523929713U,
			EdgeSyncServiceNotUninstalled = 830945707U,
			UninstallOldEdgeTransportServiceTask = 588066505U
		}

		private enum ParamIDs
		{
			PathTooLong,
			AdamUninstallError,
			InvalidAdamInstanceName,
			AdamUninstallGeneralFailureWithResult,
			InvalidCharsInPath,
			OccupiedPortsInformation,
			InvalidDriveInPath,
			AdamInstallError,
			PortIsAvailable,
			AdamInstallWarning,
			CouldNotConnectToAdamService,
			AdamUninstallProcessFailure,
			CouldNotGetInfoToConnectToAdamService,
			InvalidServicesDependedOn,
			PortIsBusy,
			AdamInstallProcessFailure,
			AdamSchemaImportProcessFailure,
			AdamSchemaImportFailure,
			AdamSetAclsProcessFailure,
			AdamUninstallWarning,
			ReadOnlyPath,
			LogProcessExitCode,
			NoPermissionsForPath,
			AdamInstallGeneralFailureWithResult,
			LogRunningCommand,
			AdamFailedSetServiceArgs,
			InvalidPortNumber
		}
	}
}
