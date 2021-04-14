using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using Microsoft.Search.Platform.Parallax.Core.Model;

namespace Microsoft.Exchange.WorkloadManagement
{
	[GeneratedCode("microsoft.search.platform.parallax.tools.codegenerator.exe", "1.0.0.0")]
	[EditorBrowsable(EditorBrowsableState.Never)]
	internal sealed class _DynamicStorageSelection_ISystemWorkloadManagerSettings_DataAccessor_ : VariantObjectDataAccessorBase<ISystemWorkloadManagerSettings, _DynamicStorageSelection_ISystemWorkloadManagerSettings_Implementation_, _DynamicStorageSelection_ISystemWorkloadManagerSettings_DataAccessor_>
	{
		internal string _Name_MaterializedValue_;

		internal int _MaxConcurrency_MaterializedValue_;

		internal ValueProvider<int> _MaxConcurrency_ValueProvider_;

		internal TimeSpan _RefreshCycle_MaterializedValue_ = default(TimeSpan);

		internal ValueProvider<TimeSpan> _RefreshCycle_ValueProvider_;
	}
}
