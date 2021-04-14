using System;

namespace Microsoft.Exchange.Data.MsgStorage.Internal
{
	[Serializable]
	public enum MsgStorageErrorCode
	{
		Undetermined,
		CreateFileFailed,
		CreateStorageOnStreamFailed,
		OpenStorageFileFailed,
		OpenStorageOnStreamFailed,
		FailedRead,
		FailedWrite,
		FailedSeek,
		FailedCreateStream,
		FailedOpenStream,
		FailedCreateSubstorage,
		FailedOpenSubstorage,
		FailedCopyStorage,
		FailedWriteOle,
		StorageStreamTooLong,
		StorageStreamTruncated,
		InvalidPropertyType,
		PropertyListTruncated,
		PropertyValueTruncated,
		InvalidPropertyValueLength,
		MultivaluedPropertyDimensionTooLarge,
		MultivaluedValueTooLong,
		RecipientPropertyTooLong,
		NonStreamablePropertyTooLong,
		NamedPropertyNotFound,
		CorruptNamedPropertyData,
		NamedPropertiesListTooLong,
		EmptyPropertiesStream
	}
}
