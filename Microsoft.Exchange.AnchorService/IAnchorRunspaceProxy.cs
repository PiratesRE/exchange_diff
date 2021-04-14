using System;
using System.Collections.ObjectModel;
using System.Management.Automation;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.AnchorService
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal interface IAnchorRunspaceProxy : IDisposable
	{
		Collection<T> RunPSCommand<T>(PSCommand command, out ErrorRecord error);

		T RunPSCommandSingleOrDefault<T>(PSCommand command, out ErrorRecord error);
	}
}
