using System;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.HA.DirectoryServices
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal interface IADDatabaseCopy : IADObjectCommon
	{
		string HostServerName { get; }

		EnhancedTimeSpan ReplayLagTime { get; }

		EnhancedTimeSpan TruncationLagTime { get; }

		int ActivationPreference { get; }

		ADObjectId HostServer { get; }

		string DatabaseName { get; }
	}
}
