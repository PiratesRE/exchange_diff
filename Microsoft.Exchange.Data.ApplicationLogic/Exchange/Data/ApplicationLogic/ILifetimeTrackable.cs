using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.ApplicationLogic
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal interface ILifetimeTrackable
	{
		DateTime CreateTime { get; }

		DateTime LastAccessTime { get; set; }
	}
}
