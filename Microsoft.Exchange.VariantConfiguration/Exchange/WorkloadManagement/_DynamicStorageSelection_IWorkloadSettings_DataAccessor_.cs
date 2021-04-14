using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using Microsoft.Search.Platform.Parallax.Core.Model;

namespace Microsoft.Exchange.WorkloadManagement
{
	[EditorBrowsable(EditorBrowsableState.Never)]
	[GeneratedCode("microsoft.search.platform.parallax.tools.codegenerator.exe", "1.0.0.0")]
	internal sealed class _DynamicStorageSelection_IWorkloadSettings_DataAccessor_ : VariantObjectDataAccessorBase<IWorkloadSettings, _DynamicStorageSelection_IWorkloadSettings_Implementation_, _DynamicStorageSelection_IWorkloadSettings_DataAccessor_>
	{
		internal string _Name_MaterializedValue_;

		internal WorkloadClassification _Classification_MaterializedValue_;

		internal ValueProvider<WorkloadClassification> _Classification_ValueProvider_;

		internal int _MaxConcurrency_MaterializedValue_;

		internal ValueProvider<int> _MaxConcurrency_ValueProvider_;

		internal bool _Enabled_MaterializedValue_;

		internal ValueProvider<bool> _Enabled_ValueProvider_;

		internal bool _EnabledDuringBlackout_MaterializedValue_;

		internal ValueProvider<bool> _EnabledDuringBlackout_ValueProvider_;
	}
}
