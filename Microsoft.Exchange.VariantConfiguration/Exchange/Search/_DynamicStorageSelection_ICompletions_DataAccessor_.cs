using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using Microsoft.Search.Platform.Parallax.Core.Model;

namespace Microsoft.Exchange.Search
{
	[EditorBrowsable(EditorBrowsableState.Never)]
	[GeneratedCode("microsoft.search.platform.parallax.tools.codegenerator.exe", "1.0.0.0")]
	internal sealed class _DynamicStorageSelection_ICompletions_DataAccessor_ : VariantObjectDataAccessorBase<ICompletions, _DynamicStorageSelection_ICompletions_Implementation_, _DynamicStorageSelection_ICompletions_DataAccessor_>
	{
		internal string _Name_MaterializedValue_;

		internal int _MaxCompletionTraversalCount_MaterializedValue_;

		internal ValueProvider<int> _MaxCompletionTraversalCount_ValueProvider_;

		internal string _TopNExclusionCharacters_MaterializedValue_;

		internal ValueProvider<string> _TopNExclusionCharacters_ValueProvider_;

		internal bool _FinalWordSuggestionsEnabled_MaterializedValue_;

		internal ValueProvider<bool> _FinalWordSuggestionsEnabled_ValueProvider_;
	}
}
