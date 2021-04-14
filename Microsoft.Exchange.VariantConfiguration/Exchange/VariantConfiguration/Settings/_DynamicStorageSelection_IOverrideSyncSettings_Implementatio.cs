using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using Microsoft.Search.Platform.Parallax.Core.Model;

namespace Microsoft.Exchange.VariantConfiguration.Settings
{
	[EditorBrowsable(EditorBrowsableState.Never)]
	[GeneratedCode("microsoft.search.platform.parallax.tools.codegenerator.exe", "1.0.0.0")]
	internal sealed class _DynamicStorageSelection_IOverrideSyncSettings_Implementation_ : IOverrideSyncSettings, IFeature, ISettings, IDataAccessorBackedObject<_DynamicStorageSelection_IOverrideSyncSettings_DataAccessor_>, IVariantObjectInstance
	{
		VariantContextSnapshot IVariantObjectInstance.Context
		{
			get
			{
				return this.context;
			}
		}

		_DynamicStorageSelection_IOverrideSyncSettings_DataAccessor_ IDataAccessorBackedObject<_DynamicStorageSelection_IOverrideSyncSettings_DataAccessor_>.DataAccessor
		{
			get
			{
				return this.dataAccessor;
			}
		}

		void IDataAccessorBackedObject<_DynamicStorageSelection_IOverrideSyncSettings_DataAccessor_>.Initialize(_DynamicStorageSelection_IOverrideSyncSettings_DataAccessor_ dataAccessor, VariantContextSnapshot context)
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

		public TimeSpan RefreshInterval
		{
			get
			{
				if (this.dataAccessor._RefreshInterval_ValueProvider_ != null)
				{
					return this.dataAccessor._RefreshInterval_ValueProvider_.GetValue(this.context);
				}
				return this.dataAccessor._RefreshInterval_MaterializedValue_;
			}
		}

		private _DynamicStorageSelection_IOverrideSyncSettings_DataAccessor_ dataAccessor;

		private VariantContextSnapshot context;
	}
}
