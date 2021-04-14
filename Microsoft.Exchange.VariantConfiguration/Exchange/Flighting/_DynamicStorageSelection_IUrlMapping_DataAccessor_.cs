using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using Microsoft.Search.Platform.Parallax.Core.Model;

namespace Microsoft.Exchange.Flighting
{
	[GeneratedCode("microsoft.search.platform.parallax.tools.codegenerator.exe", "1.0.0.0")]
	[EditorBrowsable(EditorBrowsableState.Never)]
	internal sealed class _DynamicStorageSelection_IUrlMapping_DataAccessor_ : VariantObjectDataAccessorBase<IUrlMapping, _DynamicStorageSelection_IUrlMapping_Implementation_, _DynamicStorageSelection_IUrlMapping_DataAccessor_>
	{
		internal string _Name_MaterializedValue_;

		internal string _Url_MaterializedValue_;

		internal ValueProvider<string> _Url_ValueProvider_;

		internal string _RemapTo_MaterializedValue_;

		internal ValueProvider<string> _RemapTo_ValueProvider_;
	}
}
