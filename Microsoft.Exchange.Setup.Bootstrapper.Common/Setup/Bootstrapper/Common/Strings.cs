using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Setup.Bootstrapper.Common
{
	internal static class Strings
	{
		static Strings()
		{
			Strings.stringIDs.Add(1648760670U, "InsufficientDiskSpace");
			Strings.stringIDs.Add(3176520328U, "InvalidPSVersion");
			Strings.stringIDs.Add(1489536333U, "HalfAsterixLine");
			Strings.stringIDs.Add(700738506U, "ButtonTexkOk");
			Strings.stringIDs.Add(3878879664U, "SetupLogStarted");
			Strings.stringIDs.Add(2069127725U, "Bit64Only");
			Strings.stringIDs.Add(3926021942U, "EarlierVersionsExist");
			Strings.stringIDs.Add(1966425445U, "ExsetupInstallModeHelp");
			Strings.stringIDs.Add(4195359116U, "SetupLogEnd");
			Strings.stringIDs.Add(404444338U, "ExsetupUninstallModeHelp");
			Strings.stringIDs.Add(3167874725U, "InvalidNetFwVersion");
			Strings.stringIDs.Add(353833589U, "ExsetupDelegationHelp");
			Strings.stringIDs.Add(2953524114U, "NoLanguageAvailable");
			Strings.stringIDs.Add(469255115U, "Cancelled");
			Strings.stringIDs.Add(1207267855U, "ExsetupRecoverServerModeHelp");
			Strings.stringIDs.Add(2476235438U, "AsterixLine");
			Strings.stringIDs.Add(2387220348U, "ErrorExchangeNotInstalled");
			Strings.stringIDs.Add(2256981146U, "IAcceptLicenseParameterRequired");
			Strings.stringIDs.Add(2390584075U, "TreatPreReqErrorsAsWarnings");
			Strings.stringIDs.Add(1639033074U, "CannotRunMultipleInstances");
			Strings.stringIDs.Add(579043712U, "AttemptingToRunFromInstalledDirectory");
			Strings.stringIDs.Add(3536712912U, "ExsetupUpgradeModeHelp");
			Strings.stringIDs.Add(692269882U, "NotAdmin");
			Strings.stringIDs.Add(1773862023U, "ExsetupUmLanguagePacksHelp");
			Strings.stringIDs.Add(4145606535U, "ExsetupPrepareTopologyHelp");
			Strings.stringIDs.Add(71513441U, "MessageHeaderText");
			Strings.stringIDs.Add(3980248438U, "ExchangeNotInstalled");
			Strings.stringIDs.Add(2380070307U, "InvalidOSVersion");
			Strings.stringIDs.Add(2305405629U, "ExsetupGeneralHelp");
			Strings.stringIDs.Add(871634395U, "UnableToFindBuildVersion");
			Strings.stringIDs.Add(3231985017U, "AddRoleNotPossible");
		}

		public static LocalizedString InsufficientDiskSpace
		{
			get
			{
				return new LocalizedString("InsufficientDiskSpace", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InvalidPSVersion
		{
			get
			{
				return new LocalizedString("InvalidPSVersion", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString SetupLogInitializeFailure(string msg)
		{
			return new LocalizedString("SetupLogInitializeFailure", Strings.ResourceManager, new object[]
			{
				msg
			});
		}

		public static LocalizedString LocalCopyBSFilesStart(string srcDir, string dstDir)
		{
			return new LocalizedString("LocalCopyBSFilesStart", Strings.ResourceManager, new object[]
			{
				srcDir,
				dstDir
			});
		}

		public static LocalizedString HalfAsterixLine
		{
			get
			{
				return new LocalizedString("HalfAsterixLine", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString DirectoryAclInfo(string name, string acl)
		{
			return new LocalizedString("DirectoryAclInfo", Strings.ResourceManager, new object[]
			{
				name,
				acl
			});
		}

		public static LocalizedString CommandLine(string launcher, string cmdLine)
		{
			return new LocalizedString("CommandLine", Strings.ResourceManager, new object[]
			{
				launcher,
				cmdLine
			});
		}

		public static LocalizedString UserName(string userName)
		{
			return new LocalizedString("UserName", Strings.ResourceManager, new object[]
			{
				userName
			});
		}

		public static LocalizedString FileCopyFailed(string srcDir, string dstDir)
		{
			return new LocalizedString("FileCopyFailed", Strings.ResourceManager, new object[]
			{
				srcDir,
				dstDir
			});
		}

		public static LocalizedString OSVersion(string version)
		{
			return new LocalizedString("OSVersion", Strings.ResourceManager, new object[]
			{
				version
			});
		}

		public static LocalizedString ButtonTexkOk
		{
			get
			{
				return new LocalizedString("ButtonTexkOk", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString SecurityIssueFoundWhenInit(string errMsg)
		{
			return new LocalizedString("SecurityIssueFoundWhenInit", Strings.ResourceManager, new object[]
			{
				errMsg
			});
		}

		public static LocalizedString SetupLogStarted
		{
			get
			{
				return new LocalizedString("SetupLogStarted", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RemovePrivileges(int win32Error)
		{
			return new LocalizedString("RemovePrivileges", Strings.ResourceManager, new object[]
			{
				win32Error
			});
		}

		public static LocalizedString Bit64Only
		{
			get
			{
				return new LocalizedString("Bit64Only", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString LocalCopyAllSetupFilesEnd(string srcDir, string dstDir)
		{
			return new LocalizedString("LocalCopyAllSetupFilesEnd", Strings.ResourceManager, new object[]
			{
				srcDir,
				dstDir
			});
		}

		public static LocalizedString EarlierVersionsExist
		{
			get
			{
				return new LocalizedString("EarlierVersionsExist", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ExsetupInstallModeHelp
		{
			get
			{
				return new LocalizedString("ExsetupInstallModeHelp", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString LocalCopyBSFilesEnd(string srcDir, string dstDir)
		{
			return new LocalizedString("LocalCopyBSFilesEnd", Strings.ResourceManager, new object[]
			{
				srcDir,
				dstDir
			});
		}

		public static LocalizedString LocalTimeZone(string LocalZone)
		{
			return new LocalizedString("LocalTimeZone", Strings.ResourceManager, new object[]
			{
				LocalZone
			});
		}

		public static LocalizedString DirectoryNotFound(string dirName)
		{
			return new LocalizedString("DirectoryNotFound", Strings.ResourceManager, new object[]
			{
				dirName
			});
		}

		public static LocalizedString UserNameError(string error)
		{
			return new LocalizedString("UserNameError", Strings.ResourceManager, new object[]
			{
				error
			});
		}

		public static LocalizedString SetupLogEnd
		{
			get
			{
				return new LocalizedString("SetupLogEnd", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ExsetupUninstallModeHelp
		{
			get
			{
				return new LocalizedString("ExsetupUninstallModeHelp", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InvalidNetFwVersion
		{
			get
			{
				return new LocalizedString("InvalidNetFwVersion", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ExsetupDelegationHelp
		{
			get
			{
				return new LocalizedString("ExsetupDelegationHelp", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString NoLanguageAvailable
		{
			get
			{
				return new LocalizedString("NoLanguageAvailable", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString Cancelled
		{
			get
			{
				return new LocalizedString("Cancelled", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString LocalCopyAllSetupFilesStart(string srcDir, string dstDir)
		{
			return new LocalizedString("LocalCopyAllSetupFilesStart", Strings.ResourceManager, new object[]
			{
				srcDir,
				dstDir
			});
		}

		public static LocalizedString AssemblyVersion(string assemblyVersion)
		{
			return new LocalizedString("AssemblyVersion", Strings.ResourceManager, new object[]
			{
				assemblyVersion
			});
		}

		public static LocalizedString ExsetupRecoverServerModeHelp
		{
			get
			{
				return new LocalizedString("ExsetupRecoverServerModeHelp", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString AsterixLine
		{
			get
			{
				return new LocalizedString("AsterixLine", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorExchangeNotInstalled
		{
			get
			{
				return new LocalizedString("ErrorExchangeNotInstalled", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ParameterNotSupportedOnClientOS(string name)
		{
			return new LocalizedString("ParameterNotSupportedOnClientOS", Strings.ResourceManager, new object[]
			{
				name
			});
		}

		public static LocalizedString StartupText(string productName)
		{
			return new LocalizedString("StartupText", Strings.ResourceManager, new object[]
			{
				productName
			});
		}

		public static LocalizedString ExSetupCompleted(string message)
		{
			return new LocalizedString("ExSetupCompleted", Strings.ResourceManager, new object[]
			{
				message
			});
		}

		public static LocalizedString IAcceptLicenseParameterRequired
		{
			get
			{
				return new LocalizedString("IAcceptLicenseParameterRequired", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString TreatPreReqErrorsAsWarnings
		{
			get
			{
				return new LocalizedString("TreatPreReqErrorsAsWarnings", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString FileNotExistsError(string fullFilePath)
		{
			return new LocalizedString("FileNotExistsError", Strings.ResourceManager, new object[]
			{
				fullFilePath
			});
		}

		public static LocalizedString CannotRunMultipleInstances
		{
			get
			{
				return new LocalizedString("CannotRunMultipleInstances", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString AssemblyDLLFileLocation(string fullFilePath)
		{
			return new LocalizedString("AssemblyDLLFileLocation", Strings.ResourceManager, new object[]
			{
				fullFilePath
			});
		}

		public static LocalizedString AttemptingToRunFromInstalledDirectory
		{
			get
			{
				return new LocalizedString("AttemptingToRunFromInstalledDirectory", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ExsetupUpgradeModeHelp
		{
			get
			{
				return new LocalizedString("ExsetupUpgradeModeHelp", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString FileNotFound(string fullFilePath)
		{
			return new LocalizedString("FileNotFound", Strings.ResourceManager, new object[]
			{
				fullFilePath
			});
		}

		public static LocalizedString NotAdmin
		{
			get
			{
				return new LocalizedString("NotAdmin", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RemoteCopyAllSetupFilesEnd(string srcDir, string dstDir)
		{
			return new LocalizedString("RemoteCopyAllSetupFilesEnd", Strings.ResourceManager, new object[]
			{
				srcDir,
				dstDir
			});
		}

		public static LocalizedString StartSetupFileNotFound(string fileName)
		{
			return new LocalizedString("StartSetupFileNotFound", Strings.ResourceManager, new object[]
			{
				fileName
			});
		}

		public static LocalizedString ExsetupUmLanguagePacksHelp
		{
			get
			{
				return new LocalizedString("ExsetupUmLanguagePacksHelp", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ExsetupPrepareTopologyHelp
		{
			get
			{
				return new LocalizedString("ExsetupPrepareTopologyHelp", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MessageHeaderText
		{
			get
			{
				return new LocalizedString("MessageHeaderText", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ExchangeNotInstalled
		{
			get
			{
				return new LocalizedString("ExchangeNotInstalled", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString StartupTextCumulativeUpdate(string productName, int cumulativeUpdateVersion)
		{
			return new LocalizedString("StartupTextCumulativeUpdate", Strings.ResourceManager, new object[]
			{
				productName,
				cumulativeUpdateVersion
			});
		}

		public static LocalizedString RemoteCopyAllSetupFilesStart(string srcDir, string dstDir)
		{
			return new LocalizedString("RemoteCopyAllSetupFilesStart", Strings.ResourceManager, new object[]
			{
				srcDir,
				dstDir
			});
		}

		public static LocalizedString InvalidOSVersion
		{
			get
			{
				return new LocalizedString("InvalidOSVersion", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MissingRegistryKey(string regKey)
		{
			return new LocalizedString("MissingRegistryKey", Strings.ResourceManager, new object[]
			{
				regKey
			});
		}

		public static LocalizedString ExsetupGeneralHelp
		{
			get
			{
				return new LocalizedString("ExsetupGeneralHelp", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RemoteCopyBSFilesEnd(string srcDir, string dstDir)
		{
			return new LocalizedString("RemoteCopyBSFilesEnd", Strings.ResourceManager, new object[]
			{
				srcDir,
				dstDir
			});
		}

		public static LocalizedString RemoteCopyBSFilesStart(string srcDir, string dstDir)
		{
			return new LocalizedString("RemoteCopyBSFilesStart", Strings.ResourceManager, new object[]
			{
				srcDir,
				dstDir
			});
		}

		public static LocalizedString UnableToFindBuildVersion
		{
			get
			{
				return new LocalizedString("UnableToFindBuildVersion", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InvalidSourceDir(string sourceDir)
		{
			return new LocalizedString("InvalidSourceDir", Strings.ResourceManager, new object[]
			{
				sourceDir
			});
		}

		public static LocalizedString AddRoleNotPossible
		{
			get
			{
				return new LocalizedString("AddRoleNotPossible", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString UnhandledErrorMessage(string errMsg)
		{
			return new LocalizedString("UnhandledErrorMessage", Strings.ResourceManager, new object[]
			{
				errMsg
			});
		}

		public static LocalizedString SetupFailed(string errorMessage)
		{
			return new LocalizedString("SetupFailed", Strings.ResourceManager, new object[]
			{
				errorMessage
			});
		}

		public static LocalizedString GetLocalizedString(Strings.IDs key)
		{
			return new LocalizedString(Strings.stringIDs[(uint)key], Strings.ResourceManager, new object[0]);
		}

		private static Dictionary<uint, string> stringIDs = new Dictionary<uint, string>(31);

		private static ExchangeResourceManager ResourceManager = ExchangeResourceManager.GetResourceManager("Microsoft.Exchange.Setup.Bootstrapper.Common.Strings", typeof(Strings).GetTypeInfo().Assembly);

		public enum IDs : uint
		{
			InsufficientDiskSpace = 1648760670U,
			InvalidPSVersion = 3176520328U,
			HalfAsterixLine = 1489536333U,
			ButtonTexkOk = 700738506U,
			SetupLogStarted = 3878879664U,
			Bit64Only = 2069127725U,
			EarlierVersionsExist = 3926021942U,
			ExsetupInstallModeHelp = 1966425445U,
			SetupLogEnd = 4195359116U,
			ExsetupUninstallModeHelp = 404444338U,
			InvalidNetFwVersion = 3167874725U,
			ExsetupDelegationHelp = 353833589U,
			NoLanguageAvailable = 2953524114U,
			Cancelled = 469255115U,
			ExsetupRecoverServerModeHelp = 1207267855U,
			AsterixLine = 2476235438U,
			ErrorExchangeNotInstalled = 2387220348U,
			IAcceptLicenseParameterRequired = 2256981146U,
			TreatPreReqErrorsAsWarnings = 2390584075U,
			CannotRunMultipleInstances = 1639033074U,
			AttemptingToRunFromInstalledDirectory = 579043712U,
			ExsetupUpgradeModeHelp = 3536712912U,
			NotAdmin = 692269882U,
			ExsetupUmLanguagePacksHelp = 1773862023U,
			ExsetupPrepareTopologyHelp = 4145606535U,
			MessageHeaderText = 71513441U,
			ExchangeNotInstalled = 3980248438U,
			InvalidOSVersion = 2380070307U,
			ExsetupGeneralHelp = 2305405629U,
			UnableToFindBuildVersion = 871634395U,
			AddRoleNotPossible = 3231985017U
		}

		private enum ParamIDs
		{
			SetupLogInitializeFailure,
			LocalCopyBSFilesStart,
			DirectoryAclInfo,
			CommandLine,
			UserName,
			FileCopyFailed,
			OSVersion,
			SecurityIssueFoundWhenInit,
			RemovePrivileges,
			LocalCopyAllSetupFilesEnd,
			LocalCopyBSFilesEnd,
			LocalTimeZone,
			DirectoryNotFound,
			UserNameError,
			LocalCopyAllSetupFilesStart,
			AssemblyVersion,
			ParameterNotSupportedOnClientOS,
			StartupText,
			ExSetupCompleted,
			FileNotExistsError,
			AssemblyDLLFileLocation,
			FileNotFound,
			RemoteCopyAllSetupFilesEnd,
			StartSetupFileNotFound,
			StartupTextCumulativeUpdate,
			RemoteCopyAllSetupFilesStart,
			MissingRegistryKey,
			RemoteCopyBSFilesEnd,
			RemoteCopyBSFilesStart,
			InvalidSourceDir,
			UnhandledErrorMessage,
			SetupFailed
		}
	}
}
