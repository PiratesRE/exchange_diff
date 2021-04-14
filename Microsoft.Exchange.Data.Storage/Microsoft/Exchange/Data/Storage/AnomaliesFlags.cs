using System;

namespace Microsoft.Exchange.Data.Storage
{
	[Flags]
	internal enum AnomaliesFlags : long
	{
		None = 0L,
		MultipleExceptionsWithSameDate = 1L,
		MismatchedOriginalDateFromExceptionList = 2L,
		ExtraExceptionNotInPattern = 4L,
		NumOccurIsZeroSoTreatAsNoEndRange = 8L
	}
}
