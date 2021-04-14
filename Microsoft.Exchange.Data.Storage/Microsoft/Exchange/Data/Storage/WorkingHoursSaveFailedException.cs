using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[Serializable]
	internal class WorkingHoursSaveFailedException : StoragePermanentException
	{
		public WorkingHoursSaveFailedException(LocalizedString message) : base(message)
		{
		}

		public WorkingHoursSaveFailedException(LocalizedString message, Exception innerException) : base(message, innerException)
		{
		}

		private WorkingHoursSaveFailedException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
