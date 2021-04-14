using System;

namespace Microsoft.Exchange.Data.Mime
{
	[Flags]
	public enum MimeComplianceStatus
	{
		Compliant = 0,
		MissingBoundary = 1,
		InvalidBoundary = 2,
		InvalidWrapping = 4,
		BareLinefeedInBody = 8,
		InvalidHeader = 16,
		MissingBodySeparator = 32,
		MissingBoundaryParameter = 64,
		InvalidTransferEncoding = 128,
		InvalidExternalBody = 256,
		BareLinefeedInHeader = 512,
		UnexpectedBinaryContent = 1024
	}
}
