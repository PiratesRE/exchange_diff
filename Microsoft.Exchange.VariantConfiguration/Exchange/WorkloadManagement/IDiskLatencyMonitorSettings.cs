using System;
using System.CodeDom.Compiler;
using Microsoft.Exchange.VariantConfiguration;

namespace Microsoft.Exchange.WorkloadManagement
{
	[GeneratedCode("microsoft.search.platform.parallax.tools.codegenerator.exe", "1.0.0.0")]
	public interface IDiskLatencyMonitorSettings : ISettings
	{
		TimeSpan FixedTimeAverageWindowBucket { get; }

		int FixedTimeAverageNumberOfBuckets { get; }

		TimeSpan ResourceHealthPollerInterval { get; }
	}
}
