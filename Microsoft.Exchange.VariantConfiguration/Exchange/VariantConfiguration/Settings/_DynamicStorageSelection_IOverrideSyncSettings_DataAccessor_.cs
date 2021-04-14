using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using Microsoft.Search.Platform.Parallax.Core.Model;

namespace Microsoft.Exchange.VariantConfiguration.Settings
{
	[EditorBrowsable(EditorBrowsableState.Never)]
	[GeneratedCode("microsoft.search.platform.parallax.tools.codegenerator.exe", "1.0.0.0")]
	internal sealed class _DynamicStorageSelection_IOverrideSyncSettings_DataAccessor_ : VariantObjectDataAccessorBase<IOverrideSyncSettings, _DynamicStorageSelection_IOverrideSyncSettings_Implementation_, _DynamicStorageSelection_IOverrideSyncSettings_DataAccessor_>
	{
		internal string _Name_MaterializedValue_;

		internal bool _Enabled_MaterializedValue_;

		internal ValueProvider<bool> _Enabled_ValueProvider_;

		internal TimeSpan _RefreshInterval_MaterializedValue_ = default(TimeSpan);

		internal ValueProvider<TimeSpan> _RefreshInterval_ValueProvider_;
	}
}
