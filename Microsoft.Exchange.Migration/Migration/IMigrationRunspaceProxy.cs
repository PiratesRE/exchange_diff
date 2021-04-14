using System;
using System.Management.Automation;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Migration
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal interface IMigrationRunspaceProxy : IDisposable
	{
		T RunPSCommand<T>(PSCommand command, out ErrorRecord error);
	}
}
