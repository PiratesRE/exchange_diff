using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Transport.Sync.Common.Exceptions
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[Serializable]
	internal class PermanentOperationLevelForItemsException : LocalizedException, IOperationLevelForItemException
	{
		public PermanentOperationLevelForItemsException(LocalizedString localizedString) : base(localizedString)
		{
		}

		public PermanentOperationLevelForItemsException(LocalizedString localizedString, Exception innerException) : base(localizedString, innerException)
		{
		}

		protected PermanentOperationLevelForItemsException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
