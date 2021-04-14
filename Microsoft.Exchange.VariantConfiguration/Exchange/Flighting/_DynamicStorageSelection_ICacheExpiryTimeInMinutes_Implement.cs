using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using Microsoft.Exchange.VariantConfiguration;
using Microsoft.Search.Platform.Parallax.Core.Model;

namespace Microsoft.Exchange.Flighting
{
	[GeneratedCode("microsoft.search.platform.parallax.tools.codegenerator.exe", "1.0.0.0")]
	[EditorBrowsable(EditorBrowsableState.Never)]
	internal sealed class _DynamicStorageSelection_ICacheExpiryTimeInMinutes_Implementation_ : ICacheExpiryTimeInMinutes, ISettings, IDataAccessorBackedObject<_DynamicStorageSelection_ICacheExpiryTimeInMinutes_DataAccessor_>, IVariantObjectInstance
	{
		VariantContextSnapshot IVariantObjectInstance.Context
		{
			get
			{
				return this.context;
			}
		}

		_DynamicStorageSelection_ICacheExpiryTimeInMinutes_DataAccessor_ IDataAccessorBackedObject<_DynamicStorageSelection_ICacheExpiryTimeInMinutes_DataAccessor_>.DataAccessor
		{
			get
			{
				return this.dataAccessor;
			}
		}

		void IDataAccessorBackedObject<_DynamicStorageSelection_ICacheExpiryTimeInMinutes_DataAccessor_>.Initialize(_DynamicStorageSelection_ICacheExpiryTimeInMinutes_DataAccessor_ dataAccessor, VariantContextSnapshot context)
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

		public int Value
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

		private _DynamicStorageSelection_ICacheExpiryTimeInMinutes_DataAccessor_ dataAccessor;

		private VariantContextSnapshot context;
	}
}
