using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using Microsoft.Search.Platform.Parallax.Core.Model;

namespace Microsoft.Exchange.WorkloadManagement
{
	[GeneratedCode("microsoft.search.platform.parallax.tools.codegenerator.exe", "1.0.0.0")]
	[EditorBrowsable(EditorBrowsableState.Never)]
	internal sealed class _DynamicStorageSelection_IResourceSettings_DataAccessor_ : VariantObjectDataAccessorBase<IResourceSettings, _DynamicStorageSelection_IResourceSettings_Implementation_, _DynamicStorageSelection_IResourceSettings_DataAccessor_>
	{
		internal string _Name_MaterializedValue_;

		internal bool _Enabled_MaterializedValue_;

		internal ValueProvider<bool> _Enabled_ValueProvider_;

		internal int _MaxConcurrency_MaterializedValue_;

		internal ValueProvider<int> _MaxConcurrency_ValueProvider_;

		internal int _DiscretionaryUnderloaded_MaterializedValue_;

		internal ValueProvider<int> _DiscretionaryUnderloaded_ValueProvider_;

		internal int _DiscretionaryOverloaded_MaterializedValue_;

		internal ValueProvider<int> _DiscretionaryOverloaded_ValueProvider_;

		internal int _DiscretionaryCritical_MaterializedValue_;

		internal ValueProvider<int> _DiscretionaryCritical_ValueProvider_;

		internal int _InternalMaintenanceUnderloaded_MaterializedValue_;

		internal ValueProvider<int> _InternalMaintenanceUnderloaded_ValueProvider_;

		internal int _InternalMaintenanceOverloaded_MaterializedValue_;

		internal ValueProvider<int> _InternalMaintenanceOverloaded_ValueProvider_;

		internal int _InternalMaintenanceCritical_MaterializedValue_;

		internal ValueProvider<int> _InternalMaintenanceCritical_ValueProvider_;

		internal int _CustomerExpectationUnderloaded_MaterializedValue_;

		internal ValueProvider<int> _CustomerExpectationUnderloaded_ValueProvider_;

		internal int _CustomerExpectationOverloaded_MaterializedValue_;

		internal ValueProvider<int> _CustomerExpectationOverloaded_ValueProvider_;

		internal int _CustomerExpectationCritical_MaterializedValue_;

		internal ValueProvider<int> _CustomerExpectationCritical_ValueProvider_;

		internal int _UrgentUnderloaded_MaterializedValue_;

		internal ValueProvider<int> _UrgentUnderloaded_ValueProvider_;

		internal int _UrgentOverloaded_MaterializedValue_;

		internal ValueProvider<int> _UrgentOverloaded_ValueProvider_;

		internal int _UrgentCritical_MaterializedValue_;

		internal ValueProvider<int> _UrgentCritical_ValueProvider_;
	}
}
