using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using Microsoft.Search.Platform.Parallax.Core.Model;

namespace Microsoft.Exchange.WorkloadManagement
{
	[EditorBrowsable(EditorBrowsableState.Never)]
	[GeneratedCode("microsoft.search.platform.parallax.tools.codegenerator.exe", "1.0.0.0")]
	internal sealed class _DynamicStorageSelection_IDiskLatencyMonitorSettings_DataAccessor_ : VariantObjectDataAccessorBase<IDiskLatencyMonitorSettings, _DynamicStorageSelection_IDiskLatencyMonitorSettings_Implementation_, _DynamicStorageSelection_IDiskLatencyMonitorSettings_DataAccessor_>
	{
		internal string _Name_MaterializedValue_;

		internal TimeSpan _FixedTimeAverageWindowBucket_MaterializedValue_ = default(TimeSpan);

		internal ValueProvider<TimeSpan> _FixedTimeAverageWindowBucket_ValueProvider_;

		internal int _FixedTimeAverageNumberOfBuckets_MaterializedValue_;

		internal ValueProvider<int> _FixedTimeAverageNumberOfBuckets_ValueProvider_;

		internal TimeSpan _ResourceHealthPollerInterval_MaterializedValue_ = default(TimeSpan);

		internal ValueProvider<TimeSpan> _ResourceHealthPollerInterval_ValueProvider_;
	}
}
