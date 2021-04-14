using System;
using Microsoft.Exchange.Management.ControlPanel;

namespace Microsoft.Exchange.Management.DDIService
{
	public interface IEacHttpContext
	{
		ShouldContinueContext ShouldContinueContext { get; set; }

		bool PostHydrationActionPresent { get; }
	}
}
