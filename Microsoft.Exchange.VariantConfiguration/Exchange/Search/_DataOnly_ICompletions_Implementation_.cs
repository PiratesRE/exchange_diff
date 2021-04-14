using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using Microsoft.Exchange.VariantConfiguration;
using Microsoft.Search.Platform.Parallax.Core.Model;

namespace Microsoft.Exchange.Search
{
	[EditorBrowsable(EditorBrowsableState.Never)]
	[GeneratedCode("microsoft.search.platform.parallax.tools.codegenerator.exe", "1.0.0.0")]
	internal sealed class _DataOnly_ICompletions_Implementation_ : ICompletions, ISettings, IVariantObjectInstance, IVariantObjectInstanceProvider
	{
		VariantContextSnapshot IVariantObjectInstance.Context
		{
			get
			{
				return null;
			}
		}

		IVariantObjectInstance IVariantObjectInstanceProvider.GetVariantObjectInstance(VariantContextSnapshot context)
		{
			return this;
		}

		public string Name
		{
			get
			{
				return this._Name_MaterializedValue_;
			}
		}

		public int MaxCompletionTraversalCount
		{
			get
			{
				return this._MaxCompletionTraversalCount_MaterializedValue_;
			}
		}

		public string TopNExclusionCharacters
		{
			get
			{
				return this._TopNExclusionCharacters_MaterializedValue_;
			}
		}

		public bool FinalWordSuggestionsEnabled
		{
			get
			{
				return this._FinalWordSuggestionsEnabled_MaterializedValue_;
			}
		}

		internal string _Name_MaterializedValue_;

		internal int _MaxCompletionTraversalCount_MaterializedValue_;

		internal string _TopNExclusionCharacters_MaterializedValue_;

		internal bool _FinalWordSuggestionsEnabled_MaterializedValue_;
	}
}
