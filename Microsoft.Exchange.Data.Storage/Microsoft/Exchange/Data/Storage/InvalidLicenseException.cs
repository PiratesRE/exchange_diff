using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Data.Storage
{
	[Serializable]
	public class InvalidLicenseException : StoragePermanentException
	{
		public InvalidLicenseException(LocalizedString message) : base(message)
		{
		}

		public InvalidLicenseException(LocalizedString message, Exception innerException) : base(message, innerException)
		{
		}

		protected InvalidLicenseException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
