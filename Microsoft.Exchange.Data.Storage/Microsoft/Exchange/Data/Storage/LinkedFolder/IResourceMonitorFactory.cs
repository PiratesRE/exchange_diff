using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage.LinkedFolder
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal interface IResourceMonitorFactory
	{
		IResourceMonitor Create(Guid teamMailboxMdbGuid);
	}
}
