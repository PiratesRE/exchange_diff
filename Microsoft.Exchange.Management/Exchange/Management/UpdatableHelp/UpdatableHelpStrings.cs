using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.UpdatableHelp
{
	internal static class UpdatableHelpStrings
	{
		static UpdatableHelpStrings()
		{
			UpdatableHelpStrings.stringIDs.Add(70089395U, "UpdatePhaseExtracting");
			UpdatableHelpStrings.stringIDs.Add(3868898729U, "UpdateManifestXmlValidationFailureErrorID");
			UpdatableHelpStrings.stringIDs.Add(81386342U, "UpdatePhaseRollback");
			UpdatableHelpStrings.stringIDs.Add(1852788648U, "UpdateUnsupportedFileType");
			UpdatableHelpStrings.stringIDs.Add(2909551342U, "UpdateManifestXmlValidationFailure");
			UpdatableHelpStrings.stringIDs.Add(679383893U, "UpdateSubtaskCheckingManifest");
			UpdatableHelpStrings.stringIDs.Add(1467114449U, "UpdateCompleting");
			UpdatableHelpStrings.stringIDs.Add(4044097735U, "UpdateTooManySubdirectoryLevelsErrorID");
			UpdatableHelpStrings.stringIDs.Add(1547601052U, "UpdateRegkeyNotFoundErrorID");
			UpdatableHelpStrings.stringIDs.Add(788118530U, "UpdateInstallFilesException");
			UpdatableHelpStrings.stringIDs.Add(176320935U, "UpdateNoApplicableUpdates");
			UpdatableHelpStrings.stringIDs.Add(2180005744U, "UpdateContentXmlValidationFailure");
			UpdatableHelpStrings.stringIDs.Add(2074866964U, "UpdateInstallationNotFound");
			UpdatableHelpStrings.stringIDs.Add(1728313122U, "UpdateConfigRegKeyNotFoundErrorID");
			UpdatableHelpStrings.stringIDs.Add(62233880U, "UpdateModuleName");
			UpdatableHelpStrings.stringIDs.Add(2013838344U, "UpdateDownloadManifestFailure");
			UpdatableHelpStrings.stringIDs.Add(1297065558U, "UpdateGeneralException");
			UpdatableHelpStrings.stringIDs.Add(3997057207U, "UpdateInstallationNotFoundErrorID");
			UpdatableHelpStrings.stringIDs.Add(265648494U, "UpdateDownloadCancelled");
			UpdatableHelpStrings.stringIDs.Add(1996085167U, "UpdateDownloadManifestFailureErrorID");
			UpdatableHelpStrings.stringIDs.Add(2728214484U, "UpdateInvalidHelpInfoUriErrorID");
			UpdatableHelpStrings.stringIDs.Add(956754913U, "UpdateComponentManifest");
			UpdatableHelpStrings.stringIDs.Add(2549902347U, "UpdatePhaseInstalling");
			UpdatableHelpStrings.stringIDs.Add(1755136268U, "UpdateMultipleHelpItems");
			UpdatableHelpStrings.stringIDs.Add(2248401810U, "UpdatePhaseDownloading");
			UpdatableHelpStrings.stringIDs.Add(3942412966U, "UpdateDownloadTimeout");
			UpdatableHelpStrings.stringIDs.Add(2817975039U, "UpdateInvalidTextCharacters");
			UpdatableHelpStrings.stringIDs.Add(3503750553U, "UpdateGeneralExceptionErrorID");
			UpdatableHelpStrings.stringIDs.Add(3766433703U, "UpdateDownloadCabinetFailure");
			UpdatableHelpStrings.stringIDs.Add(498363221U, "UpdateExtractingFiles");
			UpdatableHelpStrings.stringIDs.Add(2602824201U, "UpdateInvalidXml");
			UpdatableHelpStrings.stringIDs.Add(4042931449U, "UpdateInstallingFiles");
			UpdatableHelpStrings.stringIDs.Add(3671982142U, "UpdatePhaseChecking");
			UpdatableHelpStrings.stringIDs.Add(1264360601U, "UpdatePhaseValidating");
			UpdatableHelpStrings.stringIDs.Add(1839718877U, "UpdatePhaseFinalizing");
			UpdatableHelpStrings.stringIDs.Add(1414053639U, "UpdateBuildingFileList");
			UpdatableHelpStrings.stringIDs.Add(2017474326U, "UpdateTooManySubdirectoryLevels");
			UpdatableHelpStrings.stringIDs.Add(3295990305U, "UpdateDuplicateUpdateRevisionErrorID");
			UpdatableHelpStrings.stringIDs.Add(2830044059U, "UpdateCancelledErrorID");
			UpdatableHelpStrings.stringIDs.Add(1033587391U, "UpdateInvalidXmlRoot");
			UpdatableHelpStrings.stringIDs.Add(2914353723U, "UpdateInstallFilesExceptionErrorID");
			UpdatableHelpStrings.stringIDs.Add(354644870U, "UpdateComponentCabinet");
			UpdatableHelpStrings.stringIDs.Add(189227583U, "UpdateContentXmlValidationFailureErrorID");
			UpdatableHelpStrings.stringIDs.Add(2406171372U, "UpdateDownloadCabinetFailureErrorID");
			UpdatableHelpStrings.stringIDs.Add(231130324U, "UpdateTooManyUriRedirectionsErrorID");
			UpdatableHelpStrings.stringIDs.Add(4290623698U, "UpdateInitializing");
			UpdatableHelpStrings.stringIDs.Add(606676512U, "UpdateInvalidHelpFiles");
			UpdatableHelpStrings.stringIDs.Add(975126351U, "UpdateCleaningTempDir");
			UpdatableHelpStrings.stringIDs.Add(3347512252U, "UpdateDownloadComplete");
			UpdatableHelpStrings.stringIDs.Add(3426332074U, "UpdateFileNotFound");
			UpdatableHelpStrings.stringIDs.Add(1325972642U, "UpdateInvalidVersionNumberErrorID");
			UpdatableHelpStrings.stringIDs.Add(3356496162U, "UpdateCancelled");
		}

		public static LocalizedString UpdatePhaseExtracting
		{
			get
			{
				return new LocalizedString("UpdatePhaseExtracting", UpdatableHelpStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString UpdateManifestXmlValidationFailureErrorID
		{
			get
			{
				return new LocalizedString("UpdateManifestXmlValidationFailureErrorID", UpdatableHelpStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString UpdatePhaseRollback
		{
			get
			{
				return new LocalizedString("UpdatePhaseRollback", UpdatableHelpStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString UpdateUnsupportedFileType
		{
			get
			{
				return new LocalizedString("UpdateUnsupportedFileType", UpdatableHelpStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString UpdateManifestXmlValidationFailure
		{
			get
			{
				return new LocalizedString("UpdateManifestXmlValidationFailure", UpdatableHelpStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString UpdateInvalidVersionNumber(string revision)
		{
			return new LocalizedString("UpdateInvalidVersionNumber", UpdatableHelpStrings.ResourceManager, new object[]
			{
				revision
			});
		}

		public static LocalizedString UpdateSubtaskCheckingManifest
		{
			get
			{
				return new LocalizedString("UpdateSubtaskCheckingManifest", UpdatableHelpStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString UpdateStatus1(string phase)
		{
			return new LocalizedString("UpdateStatus1", UpdatableHelpStrings.ResourceManager, new object[]
			{
				phase
			});
		}

		public static LocalizedString UpdateUseForceToUpdateHelp(int throttleHours)
		{
			return new LocalizedString("UpdateUseForceToUpdateHelp", UpdatableHelpStrings.ResourceManager, new object[]
			{
				throttleHours
			});
		}

		public static LocalizedString UpdateCompleting
		{
			get
			{
				return new LocalizedString("UpdateCompleting", UpdatableHelpStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString UpdateInstallationFound(string path)
		{
			return new LocalizedString("UpdateInstallationFound", UpdatableHelpStrings.ResourceManager, new object[]
			{
				path
			});
		}

		public static LocalizedString UpdateTooManySubdirectoryLevelsErrorID
		{
			get
			{
				return new LocalizedString("UpdateTooManySubdirectoryLevelsErrorID", UpdatableHelpStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString UpdateRevisionApplied(int revision)
		{
			return new LocalizedString("UpdateRevisionApplied", UpdatableHelpStrings.ResourceManager, new object[]
			{
				revision
			});
		}

		public static LocalizedString UpdateRegkeyNotFoundErrorID
		{
			get
			{
				return new LocalizedString("UpdateRegkeyNotFoundErrorID", UpdatableHelpStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString UpdateInstallFilesException
		{
			get
			{
				return new LocalizedString("UpdateInstallFilesException", UpdatableHelpStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString UpdateNoApplicableUpdates
		{
			get
			{
				return new LocalizedString("UpdateNoApplicableUpdates", UpdatableHelpStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString UpdateDownloadTimeElapsed(string description, string status, string elapsedTime)
		{
			return new LocalizedString("UpdateDownloadTimeElapsed", UpdatableHelpStrings.ResourceManager, new object[]
			{
				description,
				status,
				elapsedTime
			});
		}

		public static LocalizedString UpdateContentXmlValidationFailure
		{
			get
			{
				return new LocalizedString("UpdateContentXmlValidationFailure", UpdatableHelpStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString UpdateInstallationNotFound
		{
			get
			{
				return new LocalizedString("UpdateInstallationNotFound", UpdatableHelpStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString UpdateRedirectingToHost(string sourceUrl, string targetUrl)
		{
			return new LocalizedString("UpdateRedirectingToHost", UpdatableHelpStrings.ResourceManager, new object[]
			{
				sourceUrl,
				targetUrl
			});
		}

		public static LocalizedString UpdateConfigRegKeyNotFoundErrorID
		{
			get
			{
				return new LocalizedString("UpdateConfigRegKeyNotFoundErrorID", UpdatableHelpStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString UpdateModuleName
		{
			get
			{
				return new LocalizedString("UpdateModuleName", UpdatableHelpStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString UpdateDownloadManifestFailure
		{
			get
			{
				return new LocalizedString("UpdateDownloadManifestFailure", UpdatableHelpStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString UpdateGeneralException
		{
			get
			{
				return new LocalizedString("UpdateGeneralException", UpdatableHelpStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString UpdateInstallationNotFoundErrorID
		{
			get
			{
				return new LocalizedString("UpdateInstallationNotFoundErrorID", UpdatableHelpStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString UpdateDownloadCancelled
		{
			get
			{
				return new LocalizedString("UpdateDownloadCancelled", UpdatableHelpStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString UpdateDownloadManifestFailureErrorID
		{
			get
			{
				return new LocalizedString("UpdateDownloadManifestFailureErrorID", UpdatableHelpStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString UpdateInvalidHelpInfoUriErrorID
		{
			get
			{
				return new LocalizedString("UpdateInvalidHelpInfoUriErrorID", UpdatableHelpStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString UpdateComponentManifest
		{
			get
			{
				return new LocalizedString("UpdateComponentManifest", UpdatableHelpStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString UpdatePhaseInstalling
		{
			get
			{
				return new LocalizedString("UpdatePhaseInstalling", UpdatableHelpStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString UpdateStatus2(string phase, string subtask)
		{
			return new LocalizedString("UpdateStatus2", UpdatableHelpStrings.ResourceManager, new object[]
			{
				phase,
				subtask
			});
		}

		public static LocalizedString UpdateMultipleHelpItems
		{
			get
			{
				return new LocalizedString("UpdateMultipleHelpItems", UpdatableHelpStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString UpdateRegkeyNotFound(string key, string subkey, string value)
		{
			return new LocalizedString("UpdateRegkeyNotFound", UpdatableHelpStrings.ResourceManager, new object[]
			{
				key,
				subkey,
				value
			});
		}

		public static LocalizedString UpdatePhaseDownloading
		{
			get
			{
				return new LocalizedString("UpdatePhaseDownloading", UpdatableHelpStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString UpdateDownloadTimeout
		{
			get
			{
				return new LocalizedString("UpdateDownloadTimeout", UpdatableHelpStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString UpdateInvalidTextCharacters
		{
			get
			{
				return new LocalizedString("UpdateInvalidTextCharacters", UpdatableHelpStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString UpdateCurrentHelpVersion(string version)
		{
			return new LocalizedString("UpdateCurrentHelpVersion", UpdatableHelpStrings.ResourceManager, new object[]
			{
				version
			});
		}

		public static LocalizedString UpdateGeneralExceptionErrorID
		{
			get
			{
				return new LocalizedString("UpdateGeneralExceptionErrorID", UpdatableHelpStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString UpdateDownloadCabinetFailure
		{
			get
			{
				return new LocalizedString("UpdateDownloadCabinetFailure", UpdatableHelpStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString UpdateExtractingFiles
		{
			get
			{
				return new LocalizedString("UpdateExtractingFiles", UpdatableHelpStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString UpdateInvalidFileDescription(string relativePath, string reason)
		{
			return new LocalizedString("UpdateInvalidFileDescription", UpdatableHelpStrings.ResourceManager, new object[]
			{
				relativePath,
				reason
			});
		}

		public static LocalizedString UpdateInvalidXml
		{
			get
			{
				return new LocalizedString("UpdateInvalidXml", UpdatableHelpStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString UpdateInstallingFiles
		{
			get
			{
				return new LocalizedString("UpdateInstallingFiles", UpdatableHelpStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString UpdatePhaseChecking
		{
			get
			{
				return new LocalizedString("UpdatePhaseChecking", UpdatableHelpStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString UpdateApplyingRevision(int revision, string cultures)
		{
			return new LocalizedString("UpdateApplyingRevision", UpdatableHelpStrings.ResourceManager, new object[]
			{
				revision,
				cultures
			});
		}

		public static LocalizedString UpdateSkipRevision(int revision)
		{
			return new LocalizedString("UpdateSkipRevision", UpdatableHelpStrings.ResourceManager, new object[]
			{
				revision
			});
		}

		public static LocalizedString UpdatePhaseValidating
		{
			get
			{
				return new LocalizedString("UpdatePhaseValidating", UpdatableHelpStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString UpdatePhaseFinalizing
		{
			get
			{
				return new LocalizedString("UpdatePhaseFinalizing", UpdatableHelpStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString UpdateBuildingFileList
		{
			get
			{
				return new LocalizedString("UpdateBuildingFileList", UpdatableHelpStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString UpdateTooManySubdirectoryLevels
		{
			get
			{
				return new LocalizedString("UpdateTooManySubdirectoryLevels", UpdatableHelpStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString UpdateInvalidHelpInfoUri(string uri)
		{
			return new LocalizedString("UpdateInvalidHelpInfoUri", UpdatableHelpStrings.ResourceManager, new object[]
			{
				uri
			});
		}

		public static LocalizedString UpdateDuplicateUpdateRevision(string version, int revision)
		{
			return new LocalizedString("UpdateDuplicateUpdateRevision", UpdatableHelpStrings.ResourceManager, new object[]
			{
				version,
				revision
			});
		}

		public static LocalizedString UpdateDuplicateUpdateRevisionErrorID
		{
			get
			{
				return new LocalizedString("UpdateDuplicateUpdateRevisionErrorID", UpdatableHelpStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString UpdateCancelledErrorID
		{
			get
			{
				return new LocalizedString("UpdateCancelledErrorID", UpdatableHelpStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString UpdateTooManyUriRedirections(int redirections)
		{
			return new LocalizedString("UpdateTooManyUriRedirections", UpdatableHelpStrings.ResourceManager, new object[]
			{
				redirections
			});
		}

		public static LocalizedString UpdateInvalidXmlRoot
		{
			get
			{
				return new LocalizedString("UpdateInvalidXmlRoot", UpdatableHelpStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString UpdateInstallFilesExceptionErrorID
		{
			get
			{
				return new LocalizedString("UpdateInstallFilesExceptionErrorID", UpdatableHelpStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString UpdateComponentCabinet
		{
			get
			{
				return new LocalizedString("UpdateComponentCabinet", UpdatableHelpStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString UpdateContentXmlValidationFailureErrorID
		{
			get
			{
				return new LocalizedString("UpdateContentXmlValidationFailureErrorID", UpdatableHelpStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString UpdateManifestUrl(string url)
		{
			return new LocalizedString("UpdateManifestUrl", UpdatableHelpStrings.ResourceManager, new object[]
			{
				url
			});
		}

		public static LocalizedString UpdateInvalidXmlNamespace(string targetNamespace)
		{
			return new LocalizedString("UpdateInvalidXmlNamespace", UpdatableHelpStrings.ResourceManager, new object[]
			{
				targetNamespace
			});
		}

		public static LocalizedString UpdateDownloadCabinetFailureErrorID
		{
			get
			{
				return new LocalizedString("UpdateDownloadCabinetFailureErrorID", UpdatableHelpStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString UpdateTooManyUriRedirectionsErrorID
		{
			get
			{
				return new LocalizedString("UpdateTooManyUriRedirectionsErrorID", UpdatableHelpStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString UpdateInitializing
		{
			get
			{
				return new LocalizedString("UpdateInitializing", UpdatableHelpStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString UpdateConfigRegKeyNotFound(string key, string subkey)
		{
			return new LocalizedString("UpdateConfigRegKeyNotFound", UpdatableHelpStrings.ResourceManager, new object[]
			{
				key,
				subkey
			});
		}

		public static LocalizedString UpdateInvalidHelpFiles
		{
			get
			{
				return new LocalizedString("UpdateInvalidHelpFiles", UpdatableHelpStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString UpdateCleaningTempDir
		{
			get
			{
				return new LocalizedString("UpdateCleaningTempDir", UpdatableHelpStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString UpdateDownloadComplete
		{
			get
			{
				return new LocalizedString("UpdateDownloadComplete", UpdatableHelpStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString UpdateFileNotFound
		{
			get
			{
				return new LocalizedString("UpdateFileNotFound", UpdatableHelpStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString UpdateInvalidVersionNumberErrorID
		{
			get
			{
				return new LocalizedString("UpdateInvalidVersionNumberErrorID", UpdatableHelpStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString UpdateCancelled
		{
			get
			{
				return new LocalizedString("UpdateCancelled", UpdatableHelpStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString GetLocalizedString(UpdatableHelpStrings.IDs key)
		{
			return new LocalizedString(UpdatableHelpStrings.stringIDs[(uint)key], UpdatableHelpStrings.ResourceManager, new object[0]);
		}

		private static Dictionary<uint, string> stringIDs = new Dictionary<uint, string>(52);

		private static ExchangeResourceManager ResourceManager = ExchangeResourceManager.GetResourceManager("Microsoft.Exchange.Management.UpdatableHelp.Strings", typeof(UpdatableHelpStrings).GetTypeInfo().Assembly);

		public enum IDs : uint
		{
			UpdatePhaseExtracting = 70089395U,
			UpdateManifestXmlValidationFailureErrorID = 3868898729U,
			UpdatePhaseRollback = 81386342U,
			UpdateUnsupportedFileType = 1852788648U,
			UpdateManifestXmlValidationFailure = 2909551342U,
			UpdateSubtaskCheckingManifest = 679383893U,
			UpdateCompleting = 1467114449U,
			UpdateTooManySubdirectoryLevelsErrorID = 4044097735U,
			UpdateRegkeyNotFoundErrorID = 1547601052U,
			UpdateInstallFilesException = 788118530U,
			UpdateNoApplicableUpdates = 176320935U,
			UpdateContentXmlValidationFailure = 2180005744U,
			UpdateInstallationNotFound = 2074866964U,
			UpdateConfigRegKeyNotFoundErrorID = 1728313122U,
			UpdateModuleName = 62233880U,
			UpdateDownloadManifestFailure = 2013838344U,
			UpdateGeneralException = 1297065558U,
			UpdateInstallationNotFoundErrorID = 3997057207U,
			UpdateDownloadCancelled = 265648494U,
			UpdateDownloadManifestFailureErrorID = 1996085167U,
			UpdateInvalidHelpInfoUriErrorID = 2728214484U,
			UpdateComponentManifest = 956754913U,
			UpdatePhaseInstalling = 2549902347U,
			UpdateMultipleHelpItems = 1755136268U,
			UpdatePhaseDownloading = 2248401810U,
			UpdateDownloadTimeout = 3942412966U,
			UpdateInvalidTextCharacters = 2817975039U,
			UpdateGeneralExceptionErrorID = 3503750553U,
			UpdateDownloadCabinetFailure = 3766433703U,
			UpdateExtractingFiles = 498363221U,
			UpdateInvalidXml = 2602824201U,
			UpdateInstallingFiles = 4042931449U,
			UpdatePhaseChecking = 3671982142U,
			UpdatePhaseValidating = 1264360601U,
			UpdatePhaseFinalizing = 1839718877U,
			UpdateBuildingFileList = 1414053639U,
			UpdateTooManySubdirectoryLevels = 2017474326U,
			UpdateDuplicateUpdateRevisionErrorID = 3295990305U,
			UpdateCancelledErrorID = 2830044059U,
			UpdateInvalidXmlRoot = 1033587391U,
			UpdateInstallFilesExceptionErrorID = 2914353723U,
			UpdateComponentCabinet = 354644870U,
			UpdateContentXmlValidationFailureErrorID = 189227583U,
			UpdateDownloadCabinetFailureErrorID = 2406171372U,
			UpdateTooManyUriRedirectionsErrorID = 231130324U,
			UpdateInitializing = 4290623698U,
			UpdateInvalidHelpFiles = 606676512U,
			UpdateCleaningTempDir = 975126351U,
			UpdateDownloadComplete = 3347512252U,
			UpdateFileNotFound = 3426332074U,
			UpdateInvalidVersionNumberErrorID = 1325972642U,
			UpdateCancelled = 3356496162U
		}

		private enum ParamIDs
		{
			UpdateInvalidVersionNumber,
			UpdateStatus1,
			UpdateUseForceToUpdateHelp,
			UpdateInstallationFound,
			UpdateRevisionApplied,
			UpdateDownloadTimeElapsed,
			UpdateRedirectingToHost,
			UpdateStatus2,
			UpdateRegkeyNotFound,
			UpdateCurrentHelpVersion,
			UpdateInvalidFileDescription,
			UpdateApplyingRevision,
			UpdateSkipRevision,
			UpdateInvalidHelpInfoUri,
			UpdateDuplicateUpdateRevision,
			UpdateTooManyUriRedirections,
			UpdateManifestUrl,
			UpdateInvalidXmlNamespace,
			UpdateConfigRegKeyNotFound
		}
	}
}
