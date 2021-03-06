using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using Microsoft.Exchange.VariantConfiguration;
using Microsoft.Search.Platform.Parallax.Core.Model;

namespace Microsoft.Exchange.AutoDiscover
{
	[GeneratedCode("microsoft.search.platform.parallax.tools.codegenerator.exe", "1.0.0.0")]
	[EditorBrowsable(EditorBrowsableState.Never)]
	internal sealed class _DynamicStorageSelection_IOWAUrl_Implementation_ : IOWAUrl, ISettings, IDataAccessorBackedObject<_DynamicStorageSelection_IOWAUrl_DataAccessor_>, IVariantObjectInstance
	{
		VariantContextSnapshot IVariantObjectInstance.Context
		{
			get
			{
				return this.context;
			}
		}

		_DynamicStorageSelection_IOWAUrl_DataAccessor_ IDataAccessorBackedObject<_DynamicStorageSelection_IOWAUrl_DataAccessor_>.DataAccessor
		{
			get
			{
				return this.dataAccessor;
			}
		}

		void IDataAccessorBackedObject<_DynamicStorageSelection_IOWAUrl_DataAccessor_>.Initialize(_DynamicStorageSelection_IOWAUrl_DataAccessor_ dataAccessor, VariantContextSnapshot context)
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

		public string OwaInternalAuthMethods
		{
			get
			{
				if (this.dataAccessor._OwaInternalAuthMethods_ValueProvider_ != null)
				{
					return this.dataAccessor._OwaInternalAuthMethods_ValueProvider_.GetValue(this.context);
				}
				return this.dataAccessor._OwaInternalAuthMethods_MaterializedValue_;
			}
		}

		public string OwaExternalAuthMethods
		{
			get
			{
				if (this.dataAccessor._OwaExternalAuthMethods_ValueProvider_ != null)
				{
					return this.dataAccessor._OwaExternalAuthMethods_ValueProvider_.GetValue(this.context);
				}
				return this.dataAccessor._OwaExternalAuthMethods_MaterializedValue_;
			}
		}

		private _DynamicStorageSelection_IOWAUrl_DataAccessor_ dataAccessor;

		private VariantContextSnapshot context;
	}
}
