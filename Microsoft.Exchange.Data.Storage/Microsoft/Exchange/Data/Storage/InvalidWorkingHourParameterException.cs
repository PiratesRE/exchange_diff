using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[Serializable]
	internal class InvalidWorkingHourParameterException : CorruptDataException
	{
		public InvalidWorkingHourParameterException(LocalizedString message) : base(message)
		{
		}

		public InvalidWorkingHourParameterException(LocalizedString message, Exception innerException) : base(message, innerException)
		{
		}

		private InvalidWorkingHourParameterException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
