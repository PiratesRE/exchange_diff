using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using Microsoft.Search.Platform.Parallax.Core.Model;

namespace Microsoft.Exchange.WorkloadManagement
{
	[EditorBrowsable(EditorBrowsableState.Never)]
	[GeneratedCode("microsoft.search.platform.parallax.tools.codegenerator.exe", "1.0.0.0")]
	internal sealed class _DynamicStorageSelection_IBlackoutSettings_DataAccessor_ : VariantObjectDataAccessorBase<IBlackoutSettings, _DynamicStorageSelection_IBlackoutSettings_Implementation_, _DynamicStorageSelection_IBlackoutSettings_DataAccessor_>
	{
		internal string _Name_MaterializedValue_;

		internal TimeSpan _StartTime_MaterializedValue_ = default(TimeSpan);

		internal ValueProvider<TimeSpan> _StartTime_ValueProvider_;

		internal TimeSpan _EndTime_MaterializedValue_ = default(TimeSpan);

		internal ValueProvider<TimeSpan> _EndTime_ValueProvider_;
	}
}
