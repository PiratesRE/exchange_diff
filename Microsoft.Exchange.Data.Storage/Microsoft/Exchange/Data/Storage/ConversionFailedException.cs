using System;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class ConversionFailedException : StoragePermanentException
	{
		public ConversionFailedException(ConversionFailureReason reason) : this(reason, null)
		{
		}

		public ConversionFailedException(ConversionFailureReason reason, Exception innerException) : base(ConversionFailedException.GetReasonDescription(reason), innerException)
		{
			EnumValidator.ThrowIfInvalid<ConversionFailureReason>(reason, "reason");
			this.reason = reason;
		}

		public ConversionFailedException(ConversionFailureReason reason, LocalizedString message, Exception innerException) : base(message, innerException)
		{
			EnumValidator.ThrowIfInvalid<ConversionFailureReason>(reason, "reason");
			this.reason = reason;
		}

		public ConversionFailureReason ConversionFailureReason
		{
			get
			{
				return this.reason;
			}
		}

		private static LocalizedString GetReasonDescription(ConversionFailureReason reason)
		{
			switch (reason)
			{
			case ConversionFailureReason.ExceedsLimit:
				return ServerStrings.ConversionLimitsExceeded;
			case ConversionFailureReason.MaliciousContent:
				return ServerStrings.ConversionMaliciousContent;
			case ConversionFailureReason.CorruptContent:
				return ServerStrings.ConversionCorruptContent;
			case ConversionFailureReason.ConverterInternalFailure:
				return ServerStrings.ConversionInternalFailure;
			case ConversionFailureReason.ConverterUnsupportedContent:
				return ServerStrings.ConversionUnsupportedContent;
			default:
				return LocalizedString.Empty;
			}
		}

		private ConversionFailureReason reason;
	}
}
