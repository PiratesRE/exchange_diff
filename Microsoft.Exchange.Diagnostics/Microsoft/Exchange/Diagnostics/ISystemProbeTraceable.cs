using System;

namespace Microsoft.Exchange.Diagnostics
{
	internal interface ISystemProbeTraceable
	{
		Guid SystemProbeId { get; }
	}
}
