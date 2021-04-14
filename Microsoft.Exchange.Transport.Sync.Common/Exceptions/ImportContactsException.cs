using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Transport.Sync.Common.Exceptions
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[Serializable]
	internal class ImportContactsException : LocalizedException
	{
		public ImportContactsException(LocalizedString localizedString) : base(localizedString)
		{
		}

		public ImportContactsException(LocalizedString localizedString, Exception innerException) : base(localizedString, innerException)
		{
		}

		protected ImportContactsException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
