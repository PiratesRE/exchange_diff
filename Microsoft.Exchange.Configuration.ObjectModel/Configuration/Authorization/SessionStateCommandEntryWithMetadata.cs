using System;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Configuration.Authorization
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class SessionStateCommandEntryWithMetadata
	{
		internal SessionStateCommandEntryWithMetadata(SessionStateCommandEntry sessionStateCommandEntry, CommandMetadata commandMetadata)
		{
			this.SessionStateCommandEntry = sessionStateCommandEntry;
			this.CommandMetadata = commandMetadata;
		}

		internal readonly SessionStateCommandEntry SessionStateCommandEntry;

		internal readonly CommandMetadata CommandMetadata;
	}
}
