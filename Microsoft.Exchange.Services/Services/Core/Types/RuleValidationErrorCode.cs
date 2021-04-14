using System;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Services.Core.Types
{
	[XmlType(TypeName = "RuleValidationErrorCodeType", Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	public enum RuleValidationErrorCode
	{
		ADOperationFailure,
		ConnectedAccountNotFound,
		CreateWithRuleId,
		EmptyValueFound,
		DuplicatedPriority,
		DuplicatedOperationOnTheSameRule,
		FolderDoesNotExist,
		InvalidAddress,
		InvalidDateRange,
		InvalidFolderId,
		InvalidSizeRange,
		InvalidValue,
		MessageClassificationNotFound,
		MissingAction,
		MissingParameter,
		MissingRangeValue,
		NotSettable,
		RecipientDoesNotExist,
		RuleNotFound,
		SizeLessThanZero,
		StringValueTooBig,
		UnsupportedAddress,
		UnexpectedError,
		UnsupportedRule
	}
}
