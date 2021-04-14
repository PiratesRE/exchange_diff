using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Cluster.Shared
{
	internal interface IDiagCoreImpl
	{
		ExEventLog EventLog { get; }
	}
}
