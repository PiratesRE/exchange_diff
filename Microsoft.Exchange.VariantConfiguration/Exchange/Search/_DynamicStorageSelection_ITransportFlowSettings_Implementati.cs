using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using Microsoft.Exchange.VariantConfiguration;
using Microsoft.Search.Platform.Parallax.Core.Model;

namespace Microsoft.Exchange.Search
{
	[GeneratedCode("microsoft.search.platform.parallax.tools.codegenerator.exe", "1.0.0.0")]
	[EditorBrowsable(EditorBrowsableState.Never)]
	internal sealed class _DynamicStorageSelection_ITransportFlowSettings_Implementation_ : ITransportFlowSettings, ISettings, IDataAccessorBackedObject<_DynamicStorageSelection_ITransportFlowSettings_DataAccessor_>, IVariantObjectInstance
	{
		VariantContextSnapshot IVariantObjectInstance.Context
		{
			get
			{
				return this.context;
			}
		}

		_DynamicStorageSelection_ITransportFlowSettings_DataAccessor_ IDataAccessorBackedObject<_DynamicStorageSelection_ITransportFlowSettings_DataAccessor_>.DataAccessor
		{
			get
			{
				return this.dataAccessor;
			}
		}

		void IDataAccessorBackedObject<_DynamicStorageSelection_ITransportFlowSettings_DataAccessor_>.Initialize(_DynamicStorageSelection_ITransportFlowSettings_DataAccessor_ dataAccessor, VariantContextSnapshot context)
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

		public bool SkipTokenInfoGeneration
		{
			get
			{
				if (this.dataAccessor._SkipTokenInfoGeneration_ValueProvider_ != null)
				{
					return this.dataAccessor._SkipTokenInfoGeneration_ValueProvider_.GetValue(this.context);
				}
				return this.dataAccessor._SkipTokenInfoGeneration_MaterializedValue_;
			}
		}

		public bool SkipMdmGeneration
		{
			get
			{
				if (this.dataAccessor._SkipMdmGeneration_ValueProvider_ != null)
				{
					return this.dataAccessor._SkipMdmGeneration_ValueProvider_.GetValue(this.context);
				}
				return this.dataAccessor._SkipMdmGeneration_MaterializedValue_;
			}
		}

		public bool UseMdmFlow
		{
			get
			{
				if (this.dataAccessor._UseMdmFlow_ValueProvider_ != null)
				{
					return this.dataAccessor._UseMdmFlow_ValueProvider_.GetValue(this.context);
				}
				return this.dataAccessor._UseMdmFlow_MaterializedValue_;
			}
		}

		private _DynamicStorageSelection_ITransportFlowSettings_DataAccessor_ dataAccessor;

		private VariantContextSnapshot context;
	}
}
