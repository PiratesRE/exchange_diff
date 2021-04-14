using System;
using System.CodeDom.Compiler;
using System.Xml.Serialization;

namespace Microsoft.Exchange.SoapWebClient.EWS
{
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[GeneratedCode("wsdl", "4.0.30319.17627")]
	[Serializable]
	public enum RuleValidationErrorCodeType
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
