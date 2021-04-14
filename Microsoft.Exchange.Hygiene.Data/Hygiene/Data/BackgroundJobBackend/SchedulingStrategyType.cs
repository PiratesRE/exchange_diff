using System;

namespace Microsoft.Exchange.Hygiene.Data.BackgroundJobBackend
{
	public enum SchedulingStrategyType : byte
	{
		Greedy,
		Election
	}
}
