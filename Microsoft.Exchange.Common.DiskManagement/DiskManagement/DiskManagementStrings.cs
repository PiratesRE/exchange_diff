using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Common.DiskManagement
{
	internal static class DiskManagementStrings
	{
		static DiskManagementStrings()
		{
			DiskManagementStrings.stringIDs.Add(2896272628U, "BitlockerCertificatesNotFoundError");
		}

		public static LocalizedString BitlockerUtilError(string errorMsg)
		{
			return new LocalizedString("BitlockerUtilError", DiskManagementStrings.ResourceManager, new object[]
			{
				errorMsg
			});
		}

		public static LocalizedString InvalidCallWMIMethodArgumentsError(string[] inParamNameList, object inParamValueList, int inParamNameListLenght, int inParamValueListLenght)
		{
			return new LocalizedString("InvalidCallWMIMethodArgumentsError", DiskManagementStrings.ResourceManager, new object[]
			{
				inParamNameList,
				inParamValueList,
				inParamNameListLenght,
				inParamValueListLenght
			});
		}

		public static LocalizedString Win8EmptyVolumeNotFullyEncryptedAfterWaitError(string volume, int milliseconds, string bitlockerState)
		{
			return new LocalizedString("Win8EmptyVolumeNotFullyEncryptedAfterWaitError", DiskManagementStrings.ResourceManager, new object[]
			{
				volume,
				milliseconds,
				bitlockerState
			});
		}

		public static LocalizedString BitlockerCertificatesNotFoundError
		{
			get
			{
				return new LocalizedString("BitlockerCertificatesNotFoundError", DiskManagementStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString FullVolumeEncryptionOnWin8ServerError(string volumeID)
		{
			return new LocalizedString("FullVolumeEncryptionOnWin8ServerError", DiskManagementStrings.ResourceManager, new object[]
			{
				volumeID
			});
		}

		public static LocalizedString WMIError(int returnValue, string methodName, string errorMessage)
		{
			return new LocalizedString("WMIError", DiskManagementStrings.ResourceManager, new object[]
			{
				returnValue,
				methodName,
				errorMessage
			});
		}

		public static LocalizedString MountpointsFindError(string error)
		{
			return new LocalizedString("MountpointsFindError", DiskManagementStrings.ResourceManager, new object[]
			{
				error
			});
		}

		public static LocalizedString Win7EmptyVolumeNotEncryptingAfterStartingEncryptionError(string volume, string bitlockerState)
		{
			return new LocalizedString("Win7EmptyVolumeNotEncryptingAfterStartingEncryptionError", DiskManagementStrings.ResourceManager, new object[]
			{
				volume,
				bitlockerState
			});
		}

		public static LocalizedString EncryptingVolumesFindError(string error)
		{
			return new LocalizedString("EncryptingVolumesFindError", DiskManagementStrings.ResourceManager, new object[]
			{
				error
			});
		}

		public static LocalizedString UsedOnlySpaceEncryptionAttemptOnANonEmptyVolumeError(string volume)
		{
			return new LocalizedString("UsedOnlySpaceEncryptionAttemptOnANonEmptyVolumeError", DiskManagementStrings.ResourceManager, new object[]
			{
				volume
			});
		}

		public static LocalizedString UsedOnlyEncryptionOnNonWin8ServerError(string volumeID)
		{
			return new LocalizedString("UsedOnlyEncryptionOnNonWin8ServerError", DiskManagementStrings.ResourceManager, new object[]
			{
				volumeID
			});
		}

		public static LocalizedString Win7EmptyVolumeNotStartedDueToPreviousFailureBadBlocksError(string volume, string mountPoint, string eventXML)
		{
			return new LocalizedString("Win7EmptyVolumeNotStartedDueToPreviousFailureBadBlocksError", DiskManagementStrings.ResourceManager, new object[]
			{
				volume,
				mountPoint,
				eventXML
			});
		}

		public static LocalizedString InvalidVolumeIdError(string volumeId)
		{
			return new LocalizedString("InvalidVolumeIdError", DiskManagementStrings.ResourceManager, new object[]
			{
				volumeId
			});
		}

		public static LocalizedString VolumeLockedError(string volume)
		{
			return new LocalizedString("VolumeLockedError", DiskManagementStrings.ResourceManager, new object[]
			{
				volume
			});
		}

		public static LocalizedString VolumeCannotBeBothEncryptingAndEncrypted(string volumeName)
		{
			return new LocalizedString("VolumeCannotBeBothEncryptingAndEncrypted", DiskManagementStrings.ResourceManager, new object[]
			{
				volumeName
			});
		}

		public static LocalizedString ReturnValueExceptionInconsistency(string methodName, string errorMsg)
		{
			return new LocalizedString("ReturnValueExceptionInconsistency", DiskManagementStrings.ResourceManager, new object[]
			{
				methodName,
				errorMsg
			});
		}

		public static LocalizedString EncryptableVolumeArgNullError(string methodName)
		{
			return new LocalizedString("EncryptableVolumeArgNullError", DiskManagementStrings.ResourceManager, new object[]
			{
				methodName
			});
		}

		public static LocalizedString LockedVolumesFindError(string error)
		{
			return new LocalizedString("LockedVolumesFindError", DiskManagementStrings.ResourceManager, new object[]
			{
				error
			});
		}

		public static LocalizedString InvalidFilePathError(string filePath)
		{
			return new LocalizedString("InvalidFilePathError", DiskManagementStrings.ResourceManager, new object[]
			{
				filePath
			});
		}

		public static LocalizedString FullVolumeEncryptionAttemptOnANonEmptyVolumeError(string volume)
		{
			return new LocalizedString("FullVolumeEncryptionAttemptOnANonEmptyVolumeError", DiskManagementStrings.ResourceManager, new object[]
			{
				volume
			});
		}

		public static LocalizedString VolumeLockedFindError(string volumeId, string error)
		{
			return new LocalizedString("VolumeLockedFindError", DiskManagementStrings.ResourceManager, new object[]
			{
				volumeId,
				error
			});
		}

		public static LocalizedString GetLocalizedString(DiskManagementStrings.IDs key)
		{
			return new LocalizedString(DiskManagementStrings.stringIDs[(uint)key], DiskManagementStrings.ResourceManager, new object[0]);
		}

		private static Dictionary<uint, string> stringIDs = new Dictionary<uint, string>(1);

		private static ExchangeResourceManager ResourceManager = ExchangeResourceManager.GetResourceManager("Microsoft.Exchange.Common.DiskManagement.Strings", typeof(DiskManagementStrings).GetTypeInfo().Assembly);

		public enum IDs : uint
		{
			BitlockerCertificatesNotFoundError = 2896272628U
		}

		private enum ParamIDs
		{
			BitlockerUtilError,
			InvalidCallWMIMethodArgumentsError,
			Win8EmptyVolumeNotFullyEncryptedAfterWaitError,
			FullVolumeEncryptionOnWin8ServerError,
			WMIError,
			MountpointsFindError,
			Win7EmptyVolumeNotEncryptingAfterStartingEncryptionError,
			EncryptingVolumesFindError,
			UsedOnlySpaceEncryptionAttemptOnANonEmptyVolumeError,
			UsedOnlyEncryptionOnNonWin8ServerError,
			Win7EmptyVolumeNotStartedDueToPreviousFailureBadBlocksError,
			InvalidVolumeIdError,
			VolumeLockedError,
			VolumeCannotBeBothEncryptingAndEncrypted,
			ReturnValueExceptionInconsistency,
			EncryptableVolumeArgNullError,
			LockedVolumesFindError,
			InvalidFilePathError,
			FullVolumeEncryptionAttemptOnANonEmptyVolumeError,
			VolumeLockedFindError
		}
	}
}
