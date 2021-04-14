using System;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.Common
{
	public interface ICriticalDependency
	{
		string Name { get; }

		TimeSpan RetestDelay { get; }

		string EscalationService { get; }

		string EscalationTeam { get; }

		bool TestCriticalDependency();

		bool FixCriticalDependency();
	}
}
