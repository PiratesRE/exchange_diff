using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[Serializable]
	internal sealed class SharingConflictException : StorageTransientException
	{
		public SharingConflictException() : this(null)
		{
		}

		public SharingConflictException(ConflictResolutionResult conflictResolutionResult) : base(ServerStrings.SharingConflictException)
		{
			this.ConflictResolutionResult = conflictResolutionResult;
		}

		public ConflictResolutionResult ConflictResolutionResult { get; private set; }
	}
}
