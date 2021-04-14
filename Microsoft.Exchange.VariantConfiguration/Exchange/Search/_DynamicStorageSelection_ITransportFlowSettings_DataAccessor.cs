using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using Microsoft.Search.Platform.Parallax.Core.Model;

namespace Microsoft.Exchange.Search
{
	[EditorBrowsable(EditorBrowsableState.Never)]
	[GeneratedCode("microsoft.search.platform.parallax.tools.codegenerator.exe", "1.0.0.0")]
	internal sealed class _DynamicStorageSelection_ITransportFlowSettings_DataAccessor_ : VariantObjectDataAccessorBase<ITransportFlowSettings, _DynamicStorageSelection_ITransportFlowSettings_Implementation_, _DynamicStorageSelection_ITransportFlowSettings_DataAccessor_>
	{
		internal string _Name_MaterializedValue_;

		internal bool _SkipTokenInfoGeneration_MaterializedValue_;

		internal ValueProvider<bool> _SkipTokenInfoGeneration_ValueProvider_;

		internal bool _SkipMdmGeneration_MaterializedValue_;

		internal ValueProvider<bool> _SkipMdmGeneration_ValueProvider_;

		internal bool _UseMdmFlow_MaterializedValue_;

		internal ValueProvider<bool> _UseMdmFlow_ValueProvider_;
	}
}
