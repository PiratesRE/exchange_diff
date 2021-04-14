using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using Microsoft.Search.Platform.Parallax.Core.Model;

namespace Microsoft.Exchange.Search
{
	[EditorBrowsable(EditorBrowsableState.Never)]
	[GeneratedCode("microsoft.search.platform.parallax.tools.codegenerator.exe", "1.0.0.0")]
	internal sealed class _DynamicStorageSelection_IFeederSettings_DataAccessor_ : VariantObjectDataAccessorBase<IFeederSettings, _DynamicStorageSelection_IFeederSettings_Implementation_, _DynamicStorageSelection_IFeederSettings_DataAccessor_>
	{
		internal string _Name_MaterializedValue_;

		internal int _QueueSize_MaterializedValue_;

		internal ValueProvider<int> _QueueSize_ValueProvider_;
	}
}
