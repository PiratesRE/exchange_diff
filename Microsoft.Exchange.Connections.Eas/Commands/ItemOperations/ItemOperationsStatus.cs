using System;

namespace Microsoft.Exchange.Connections.Eas.Commands.ItemOperations
{
	[Flags]
	public enum ItemOperationsStatus
	{
		Success = 1,
		ProtocolError = 4098,
		ServerError = 259,
		DocLibAcccessError = 4100,
		DocLibAccessDenied = 4101,
		DocLibObjectNotFoundOrAccessDenied = 4102,
		DocLibFailedToConnectToServer = 263,
		BadByteRange = 4104,
		BadStore = 4105,
		FileIsEmpty = 266,
		RequestedDataSizeTooLarge = 267,
		DownloadFailure = 268,
		ItemConversionFailed = 270,
		AfpInvalidAttachmentOrAttachmentId = 4111,
		ResourceAccessDenied = 4112,
		PartialSuccess = 273,
		CredentialsRequired = 274,
		ServerBusy = 8302,
		MissingElements = 4251,
		ActionNotSupported = 4252
	}
}
