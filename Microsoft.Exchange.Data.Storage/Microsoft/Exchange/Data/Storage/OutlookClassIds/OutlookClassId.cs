using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage.OutlookClassIds
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal struct OutlookClassId
	{
		internal OutlookClassId(Guid value)
		{
			this.AsGuid = value;
			this.AsBytes = value.ToByteArray();
		}

		internal readonly Guid AsGuid;

		internal readonly byte[] AsBytes;
	}
}
