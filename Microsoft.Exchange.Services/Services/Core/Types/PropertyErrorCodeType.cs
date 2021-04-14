using System;

namespace Microsoft.Exchange.Services.Core.Types
{
	[Serializable]
	public enum PropertyErrorCodeType
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
