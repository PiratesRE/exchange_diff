using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using Microsoft.Exchange.VariantConfiguration;
using Microsoft.Search.Platform.Parallax.Core.Model;

namespace Microsoft.Exchange.WorkloadManagement
{
	[GeneratedCode("microsoft.search.platform.parallax.tools.codegenerator.exe", "1.0.0.0")]
	[EditorBrowsable(EditorBrowsableState.Never)]
	internal sealed class _DynamicStorageSelection_IWorkloadSettings_Implementation_ : IWorkloadSettings, ISettings, IDataAccessorBackedObject<_DynamicStorageSelection_IWorkloadSettings_DataAccessor_>, IVariantObjectInstance
	{
		VariantContextSnapshot IVariantObjectInstance.Context
		{
			get
			{
				return this.context;
			}
		}

		_DynamicStorageSelection_IWorkloadSettings_DataAccessor_ IDataAccessorBackedObject<_DynamicStorageSelection_IWorkloadSettings_DataAccessor_>.DataAccessor
		{
			get
			{
				return this.dataAccessor;
			}
		}

		void IDataAccessorBackedObject<_DynamicStorageSelection_IWorkloadSettings_DataAccessor_>.Initialize(_DynamicStorageSelection_IWorkloadSettings_DataAccessor_ dataAccessor, VariantContextSnapshot context)
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

		public WorkloadClassification Classification
		{
			get
			{
				if (this.dataAccessor._Classification_ValueProvider_ != null)
				{
					return this.dataAccessor._Classification_ValueProvider_.GetValue(this.context);
				}
				return this.dataAccessor._Classification_MaterializedValue_;
			}
		}

		public int MaxConcurrency
		{
			get
			{
				if (this.dataAccessor._MaxConcurrency_ValueProvider_ != null)
				{
					return this.dataAccessor._MaxConcurrency_ValueProvider_.GetValue(this.context);
				}
				return this.dataAccessor._MaxConcurrency_MaterializedValue_;
			}
		}

		public bool Enabled
		{
			get
			{
				if (this.dataAccessor._Enabled_ValueProvider_ != null)
				{
					return this.dataAccessor._Enabled_ValueProvider_.GetValue(this.context);
				}
				return this.dataAccessor._Enabled_MaterializedValue_;
			}
		}

		public bool EnabledDuringBlackout
		{
			get
			{
				if (this.dataAccessor._EnabledDuringBlackout_ValueProvider_ != null)
				{
					return this.dataAccessor._EnabledDuringBlackout_ValueProvider_.GetValue(this.context);
				}
				return this.dataAccessor._EnabledDuringBlackout_MaterializedValue_;
			}
		}

		private _DynamicStorageSelection_IWorkloadSettings_DataAccessor_ dataAccessor;

		private VariantContextSnapshot context;
	}
}
