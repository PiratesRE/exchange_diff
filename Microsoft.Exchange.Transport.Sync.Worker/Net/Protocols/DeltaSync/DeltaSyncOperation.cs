using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Transport.Sync.Common;

namespace Microsoft.Exchange.Net.Protocols.DeltaSync
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal sealed class DeltaSyncOperation
	{
		internal DeltaSyncOperation(DeltaSyncOperation.Type type, DeltaSyncObject deltaSyncObject)
		{
			SyncUtilities.ThrowIfArgumentNull("deltaSyncObject", deltaSyncObject);
			this.deltaSyncObject = deltaSyncObject;
			this.type = type;
		}

		internal DeltaSyncOperation.Type OperationType
		{
			get
			{
				return this.type;
			}
		}

		internal DeltaSyncObject DeltaSyncObject
		{
			get
			{
				return this.deltaSyncObject;
			}
		}

		private DeltaSyncObject deltaSyncObject;

		private DeltaSyncOperation.Type type;

		internal enum Type
		{
			Add,
			Change,
			Delete
		}
	}
}
