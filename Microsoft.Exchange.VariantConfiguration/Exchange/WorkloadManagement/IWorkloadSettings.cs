using System;
using System.CodeDom.Compiler;
using Microsoft.Exchange.VariantConfiguration;

namespace Microsoft.Exchange.WorkloadManagement
{
	[GeneratedCode("microsoft.search.platform.parallax.tools.codegenerator.exe", "1.0.0.0")]
	public interface IWorkloadSettings : ISettings
	{
		WorkloadClassification Classification { get; }

		int MaxConcurrency { get; }

		bool Enabled { get; }

		bool EnabledDuringBlackout { get; }
	}
}
