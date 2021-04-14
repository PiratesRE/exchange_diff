using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Mapi.Unmanaged
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal struct CHECKPOINTSTATUSRAW
	{
		internal Guid guidMdb;

		internal uint ulBeginCheckpointDepth;

		internal uint ulEndCheckpointDepth;
	}
}
