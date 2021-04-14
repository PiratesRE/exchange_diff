using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using Microsoft.Exchange.VariantConfiguration;
using Microsoft.Search.Platform.Parallax.Core.Model;

namespace Microsoft.Exchange.Search
{
	[GeneratedCode("microsoft.search.platform.parallax.tools.codegenerator.exe", "1.0.0.0")]
	[EditorBrowsable(EditorBrowsableState.Never)]
	internal sealed class _DynamicStorageSelection_ICompletions_Implementation_ : ICompletions, ISettings, IDataAccessorBackedObject<_DynamicStorageSelection_ICompletions_DataAccessor_>, IVariantObjectInstance
	{
		VariantContextSnapshot IVariantObjectInstance.Context
		{
			get
			{
				return this.context;
			}
		}

		_DynamicStorageSelection_ICompletions_DataAccessor_ IDataAccessorBackedObject<_DynamicStorageSelection_ICompletions_DataAccessor_>.DataAccessor
		{
			get
			{
				return this.dataAccessor;
			}
		}

		void IDataAccessorBackedObject<_DynamicStorageSelection_ICompletions_DataAccessor_>.Initialize(_DynamicStorageSelection_ICompletions_DataAccessor_ dataAccessor, VariantContextSnapshot context)
		{
			this.dataAccessor = dataAccessor;
			this.context = context;
		}

		public string Name
		{
			get
			{
				return this.dataAccessor._Name_MaterializedValue_;
			}
		}

		public int MaxCompletionTraversalCount
		{
			get
			{
				if (this.dataAccessor._MaxCompletionTraversalCount_ValueProvider_ != null)
				{
					return this.dataAccessor._MaxCompletionTraversalCount_ValueProvider_.GetValue(this.context);
				}
				return this.dataAccessor._MaxCompletionTraversalCount_MaterializedValue_;
			}
		}

		public string TopNExclusionCharacters
		{
			get
			{
				if (this.dataAccessor._TopNExclusionCharacters_ValueProvider_ != null)
				{
					return this.dataAccessor._TopNExclusionCharacters_ValueProvider_.GetValue(this.context);
				}
				return this.dataAccessor._TopNExclusionCharacters_MaterializedValue_;
			}
		}

		public bool FinalWordSuggestionsEnabled
		{
			get
			{
				if (this.dataAccessor._FinalWordSuggestionsEnabled_ValueProvider_ != null)
				{
					return this.dataAccessor._FinalWordSuggestionsEnabled_ValueProvider_.GetValue(this.context);
				}
				return this.dataAccessor._FinalWordSuggestionsEnabled_MaterializedValue_;
			}
		}

		private _DynamicStorageSelection_ICompletions_DataAccessor_ dataAccessor;

		private VariantContextSnapshot context;
	}
}
