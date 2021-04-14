using System;

namespace Microsoft.Exchange.Data.Storage
{
	internal enum ConversionFailureReason
	{
		ExceedsLimit = 1,
		MaliciousContent,
		CorruptContent,
		ConverterInternalFailure,
		ConverterUnsupportedContent
	}
}
