using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using Microsoft.Search.Platform.Parallax.Core.Model;

namespace Microsoft.Exchange.Search
{
	[EditorBrowsable(EditorBrowsableState.Never)]
	[GeneratedCode("microsoft.search.platform.parallax.tools.codegenerator.exe", "1.0.0.0")]
	internal sealed class _DynamicStorageSelection_IDocumentFeederSettings_DataAccessor_ : VariantObjectDataAccessorBase<IDocumentFeederSettings, _DynamicStorageSelection_IDocumentFeederSettings_Implementation_, _DynamicStorageSelection_IDocumentFeederSettings_DataAccessor_>
	{
		internal string _Name_MaterializedValue_;

		internal TimeSpan _BatchTimeout_MaterializedValue_ = default(TimeSpan);

		internal ValueProvider<TimeSpan> _BatchTimeout_ValueProvider_;

		internal TimeSpan _LostCallbackTimeout_MaterializedValue_ = default(TimeSpan);

		internal ValueProvider<TimeSpan> _LostCallbackTimeout_ValueProvider_;
	}
}
