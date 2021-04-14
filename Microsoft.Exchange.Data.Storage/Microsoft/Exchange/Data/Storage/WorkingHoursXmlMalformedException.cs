using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[Serializable]
	internal class WorkingHoursXmlMalformedException : CorruptDataException
	{
		public WorkingHoursXmlMalformedException(LocalizedString message) : base(message)
		{
		}

		public WorkingHoursXmlMalformedException(LocalizedString message, Exception innerException) : base(message, innerException)
		{
		}

		private WorkingHoursXmlMalformedException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
