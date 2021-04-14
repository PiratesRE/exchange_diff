using System;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.MailboxTransport.StoreDriverCommon
{
	[Serializable]
	internal class InvalidQuotaWarningMessageException : StoragePermanentException
	{
		public InvalidQuotaWarningMessageException(string errorDescription) : base(LocalizedString.Empty)
		{
			this.errorDescription = errorDescription;
		}

		public static void ThrowExceptionForMissingProp(string propertyName)
		{
			throw new InvalidQuotaWarningMessageException("Property " + propertyName + " is not present");
		}

		public static void ThrowExceptionForUnexpectedProp(string propertyName)
		{
			throw new InvalidQuotaWarningMessageException("Property " + propertyName + " should not exist");
		}

		public override string ToString()
		{
			return this.errorDescription;
		}

		private readonly string errorDescription;
	}
}
