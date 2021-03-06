using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using Microsoft.Search.Platform.Parallax.Core.Model;

namespace Microsoft.Exchange.Search
{
	[GeneratedCode("microsoft.search.platform.parallax.tools.codegenerator.exe", "1.0.0.0")]
	[EditorBrowsable(EditorBrowsableState.Never)]
	internal sealed class _DynamicStorageSelection_IIndexStatusSettings_DataAccessor_ : VariantObjectDataAccessorBase<IIndexStatusSettings, _DynamicStorageSelection_IIndexStatusSettings_Implementation_, _DynamicStorageSelection_IIndexStatusSettings_DataAccessor_>
	{
		internal string _Name_MaterializedValue_;

		internal TimeSpan _InvalidateInterval_MaterializedValue_ = default(TimeSpan);

		internal ValueProvider<TimeSpan> _InvalidateInterval_ValueProvider_;
	}
}
