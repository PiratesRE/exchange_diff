using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using Microsoft.Search.Platform.Parallax.Core.Model;

namespace Microsoft.Exchange.VariantConfiguration
{
	[EditorBrowsable(EditorBrowsableState.Never)]
	[GeneratedCode("microsoft.search.platform.parallax.tools.codegenerator.exe", "1.0.0.0")]
	internal sealed class _DynamicStorageSelection_IFeature_DataAccessor_ : VariantObjectDataAccessorBase<IFeature, _DynamicStorageSelection_IFeature_Implementation_, _DynamicStorageSelection_IFeature_DataAccessor_>
	{
		internal string _Name_MaterializedValue_;

		internal bool _Enabled_MaterializedValue_;

		internal ValueProvider<bool> _Enabled_ValueProvider_;
	}
}
