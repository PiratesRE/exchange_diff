using System;

namespace Microsoft.Exchange.Common.Bitlocker.Utilities
{
	public static class BitlockerUtilConstants
	{
		public const string MountPointWMIClassName = "Win32_MountPoint";

		public const string DefaultWMINamespace = "\\ROOT\\CIMV2";

		public const int WMISuccessReturnValue = 0;

		public const int WMIFailureReturnValue = -1;

		public const string MasterPassword = "563563-218372-416746-433752-541937-608069-594110-446754";

		public const string CertsPath = "C:\\LocalFiles\\Exchange\\DataCenter\\Certs";

		public const string BootVolumeQuery = "SELECT * FROM Win32_Volume Where BootVolume=True";

		public const string SystemVolumeQuery = "SELECT * FROM Win32_Volume Where SystemVolume=True";

		public const string VirtualDiskQuery = "Select * FROM Win32_DiskDrive WHERE Model like '%Virtual Disk%'";

		public const string VolumeQuery = "SELECT * FROM Win32_Volume WHERE DeviceId=\"{0}\"";

		public const string EncryptableVolumeWMIClassName = "Win32_EncryptableVolume";

		public const string EncryptableVolumeWMINamespace = "\\ROOT\\CIMV2\\Security\\MicrosoftVolumeEncryption";

		public const int EncryptionMethodAES128 = 3;

		public const int EncryptionMethodAES256 = 4;

		public const int UsedOnlyEncryptionModeFlag = 1;

		public const string EncryptMethodName = "Encrypt";

		public const string DecryptMethodName = "Decrypt";

		public const string LockMethodName = "Lock";

		public const string PauseConversionMethodName = "PauseConversion";

		public const string ResumeConversionMethodName = "ResumeConversion";

		public const string ProtectWithCertificateMethodName = "ProtectKeyWithCertificateThumbprint";

		public const string ProtectWithNumericalPasswordMethodName = "ProtectKeyWithNumericalPassword";

		public const string UnlockLockNumericalPasswordMethodName = "UnlockWithNumericalPassword";

		public const string NumericalPasswordMethodArgName = "NumericalPassword";

		public const string CertificateFilePathArgName = "PathWithFileName";

		public const string LockStatusMethodName = "GetLockStatus";

		public const string LockStatusPropertyName = "LockStatus";

		public const string EncryptionPercentagePropertyName = "EncryptionPercentage";

		public const string EncryptionStatusPropertyName = "ConversionStatus";

		public const string EncryptionStatusMethodName = "GetConversionStatus";

		public const string ReturnValuePropertyName = "ReturnValue";

		public const string DeviceIdPropertyName = "DeviceId";

		public const string WIn32VolumePropertyName = "Name";

		public const string VolumePropertyName = "Volume";

		public const string DirectoryPropertyName = "Directory";

		public const long EmptyVolumeMBinBytes = 524288000L;

		public const int Win8EmptyVolumeUsedOnlySpaceEncryptionTimeoutInSecs = 300;

		public const int Win8EmptyVolumeUsedOnlySpaceWaitIntervalInSecs = 1;
	}
}
