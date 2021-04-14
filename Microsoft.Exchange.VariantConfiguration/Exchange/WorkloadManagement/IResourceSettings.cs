using System;
using System.CodeDom.Compiler;
using Microsoft.Exchange.VariantConfiguration;

namespace Microsoft.Exchange.WorkloadManagement
{
	[GeneratedCode("microsoft.search.platform.parallax.tools.codegenerator.exe", "1.0.0.0")]
	public interface IResourceSettings : ISettings
	{
		bool Enabled { get; }

		int MaxConcurrency { get; }

		int DiscretionaryUnderloaded { get; }

		int DiscretionaryOverloaded { get; }

		int DiscretionaryCritical { get; }

		int InternalMaintenanceUnderloaded { get; }

		int InternalMaintenanceOverloaded { get; }

		int InternalMaintenanceCritical { get; }

		int CustomerExpectationUnderloaded { get; }

		int CustomerExpectationOverloaded { get; }

		int CustomerExpectationCritical { get; }

		int UrgentUnderloaded { get; }

		int UrgentOverloaded { get; }

		int UrgentCritical { get; }
	}
}
