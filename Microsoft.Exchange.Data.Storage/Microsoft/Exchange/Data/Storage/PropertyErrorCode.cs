using System;

namespace Microsoft.Exchange.Data.Storage
{
	internal enum PropertyErrorCode
	{
		UnknownError,
		NotFound,
		NotEnoughMemory,
		NullValue,
		IncorrectValueType,
		SetCalculatedPropertyError,
		SetStoreComputedPropertyError,
		GetCalculatedPropertyError,
		NotSupported,
		CorruptedData,
		RequireStreamed,
		ConstraintViolation,
		MapiCallFailed,
		FolderNameConflict,
		TransientMapiCallFailed,
		FolderHasChanged,
		PropertyValueTruncated,
		AccessDenied,
		PropertyNotPromoted
	}
}
