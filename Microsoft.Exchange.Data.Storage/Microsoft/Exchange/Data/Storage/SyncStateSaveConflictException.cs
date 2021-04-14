using System;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class SyncStateSaveConflictException : SaveConflictException
	{
		public SyncStateSaveConflictException(LocalizedString message, Exception innerException) : base(message, innerException)
		{
		}

		public SyncStateSaveConflictException(LocalizedString message) : base(message)
		{
		}
	}
}
