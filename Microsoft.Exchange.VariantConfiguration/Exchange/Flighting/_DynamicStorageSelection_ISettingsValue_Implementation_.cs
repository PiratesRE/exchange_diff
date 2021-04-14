using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using Microsoft.Exchange.VariantConfiguration;
using Microsoft.Search.Platform.Parallax.Core.Model;

namespace Microsoft.Exchange.Flighting
{
	[EditorBrowsable(EditorBrowsableState.Never)]
	[GeneratedCode("microsoft.search.platform.parallax.tools.codegenerator.exe", "1.0.0.0")]
	internal sealed class _DynamicStorageSelection_ISettingsValue_Implementation_ : ISettingsValue, ISettings, IDataAccessorBackedObject<_DynamicStorageSelection_ISettingsValue_DataAccessor_>, IVariantObjectInstance
	{
		VariantContextSnapshot IVariantObjectInstance.Context
		{
			get
			{
				return this.context;
			}
		}

		_DynamicStorageSelection_ISettingsValue_DataAccessor_ IDataAccessorBackedObject<_DynamicStorageSelection_ISettingsValue_DataAccessor_>.DataAccessor
		{
			get
			{
				return this.dataAccessor;
			}
		}

		void IDataAccessorBackedObject<_DynamicStorageSelection_ISettingsValue_DataAccessor_>.Initialize(_DynamicStorageSelection_ISettingsValue_DataAccessor_ dataAccessor, VariantContextSnapshot context)
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

		public string Value
		{
			get
			{
				if (this.dataAccessor._Value_ValueProvider_ != null)
				{
					return this.dataAccessor._Value_ValueProvider_.GetValue(this.context);
				}
				return this.dataAccessor._Value_MaterializedValue_;
			}
		}

		private _DynamicStorageSelection_ISettingsValue_DataAccessor_ dataAccessor;

		private VariantContextSnapshot context;
	}
}
