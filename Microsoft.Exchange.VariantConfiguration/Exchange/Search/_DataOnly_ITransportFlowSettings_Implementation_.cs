using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using Microsoft.Exchange.VariantConfiguration;
using Microsoft.Search.Platform.Parallax.Core.Model;

namespace Microsoft.Exchange.Search
{
	[EditorBrowsable(EditorBrowsableState.Never)]
	[GeneratedCode("microsoft.search.platform.parallax.tools.codegenerator.exe", "1.0.0.0")]
	internal sealed class _DataOnly_ITransportFlowSettings_Implementation_ : ITransportFlowSettings, ISettings, IVariantObjectInstance, IVariantObjectInstanceProvider
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

		public bool SkipTokenInfoGeneration
		{
			get
			{
				return this._SkipTokenInfoGeneration_MaterializedValue_;
			}
		}

		public bool SkipMdmGeneration
		{
			get
			{
				return this._SkipMdmGeneration_MaterializedValue_;
			}
		}

		public bool UseMdmFlow
		{
			get
			{
				return this._UseMdmFlow_MaterializedValue_;
			}
		}

		internal string _Name_MaterializedValue_;

		internal bool _SkipTokenInfoGeneration_MaterializedValue_;

		internal bool _SkipMdmGeneration_MaterializedValue_;

		internal bool _UseMdmFlow_MaterializedValue_;
	}
}
